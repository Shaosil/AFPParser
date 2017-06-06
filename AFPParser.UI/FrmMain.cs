using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Printing;
using AFPParser.StructuredFields;

namespace AFPParser.UI
{
    public partial class FrmMain : Form
    {
        private readonly string optionsFile = Environment.CurrentDirectory + "\\Options.xml";
        private Options opts;
        private Parser afpParser;

        public FrmMain()
        {
            InitializeComponent();

            // Store things like last opened directory
            opts = Options.LoadSettings(optionsFile);

            afpParser = new Parser();
            afpParser.ErrorEvent += (string message) => { MessageBox.Show(message, "Parser Error", MessageBoxButtons.OK, MessageBoxIcon.Error); };
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog() { InitialDirectory = opts.LastDirectory, Filter = "AFP Files(*.afp)|*.afp" };
            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;

                    // Store the last used directory
                    opts.LastDirectory = new FileInfo(dialog.FileName).DirectoryName;

                    // Parse the AFP file
                    afpParser.LoadData(dialog.FileName);
                    
                    // Databind the list box
                    afpFileBindingSource.DataSource = null;
                    afpFileBindingSource.DataSource = Parser.AfpFile;
                    dgvFields.Focus();

                    // Enable/disable the preview button
                    btnPreview.Enabled = Parser.AfpFile.Any();
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private void dgvFields_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvFields.CurrentRow != null)
            {
                StructuredField sf = (StructuredField)dgvFields.CurrentRow.DataBoundItem;
                txtDescription.Text = sf.GetFullDescription().Trim();
            }
        }

        private void dgvFields_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            StructuredField sf = (StructuredField)dgvFields.CurrentRow.DataBoundItem;

            if (!string.IsNullOrWhiteSpace(sf.DataHex))
            {
                try
                {
                    // Copy hex data to clipboard
                    Clipboard.SetText(sf.DataHex);
                    MessageBox.Show("Hex values copied to clipboard.", "Hex", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch { }
            }
        }

        private void UpdateSelectedGridRecord(StructuredField field)
        {
            int oldIndex = dgvFields.SelectedRows[0].Index - dgvFields.FirstDisplayedScrollingRowIndex;

            dgvFields.ClearSelection();
            dgvFields.Rows.Cast<DataGridViewRow>().First(r => r.DataBoundItem == field).Selected = true;

            int newIndex = dgvFields.SelectedRows[0].Index - dgvFields.FirstDisplayedScrollingRowIndex;
            int newOffset = dgvFields.FirstDisplayedScrollingRowIndex + (newIndex - oldIndex);

            // Only scroll if the new offset is above 0 AND the selected row is not near the top when we're moving down
            if (newOffset >= 0 && !(newIndex < 10 && newIndex - oldIndex > 0))
                dgvFields.FirstDisplayedScrollingRowIndex = newOffset;
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            opts.SaveSettings(optionsFile);
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            // Get the first page's PGD
            PGD pgd = Parser.AfpFile.OfType<PGD>().First();

            // Pop a new form assuming 90 DPI
            Form f = new Form()
            {
                Width = (pgd.XSize / (pgd.UnitsPerXBase / 10)) * 90,
                Height = (pgd.YSize / (pgd.UnitsPerYBase / 10)) * 90,
                FormBorderStyle = FormBorderStyle.FixedSingle,
                MaximizeBox = false,
                ShowInTaskbar = false,
                MinimizeBox = false
            };

            PrintPreviewControl ppc = new PrintPreviewControl() { Dock = DockStyle.Fill };
            ppc.Document = new PrintDocument();
            ppc.Document.PrintPage += TestBuildPrintPreview;

            f.Controls.Add(ppc);

            f.ShowDialog();
        }

        private void TestBuildPrintPreview(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawString("Work in progress...", new Font(FontFamily.GenericMonospace, 16), Brushes.Black, 100, 100);
        }
    }
}
