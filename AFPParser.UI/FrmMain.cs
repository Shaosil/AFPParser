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
        private Parser parser;
        private PrintParser printParser;

        public FrmMain()
        {
            InitializeComponent();

            // Store things like last opened directory
            opts = Options.LoadSettings(optionsFile);

            parser = new Parser();
            parser.ErrorEvent += (string message) => { MessageBox.Show(message, "Parser Error", MessageBoxButtons.OK, MessageBoxIcon.Error); };
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog() { InitialDirectory = opts.LastDirectory, Filter = "AFP Files (*.afp)|*.afp|All Files|*.*" };
            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;

                    // Store the last used directory and file
                    FileInfo fInfo = new FileInfo(dialog.FileName);
                    opts.LastDirectory = fInfo.DirectoryName;
                    opts.LastOpenedFile = fInfo.Name;

                    // Parse the AFP file
                    parser.LoadData(dialog.FileName);

                    // Data bind the list box
                    afpFileBindingSource.DataSource = null;
                    afpFileBindingSource.DataSource = parser.StructuredFields;
                    dgvFields.Focus();

                    // Enable/disable the preview button
                    btnPreview.Enabled = parser.StructuredFields.Any();

                    // Load the print parser data
                    if (btnPreview.Enabled)
                        printParser = new PrintParser(parser.StructuredFields);
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
            // Reset print preview variables
            printParser.Reset();

            // Set up a print preview dialog and wire it to our print parser's build event
            PrintPreviewDialog ppd = new PrintPreviewDialog() { Document = new PrintDocument() { DocumentName = opts.LastOpenedFile } };
            ppd.Controls.OfType<ToolStrip>().First().Items["printToolStripButton"].Visible = false; // Temp disable until we actually might want to print something
            ((Form)ppd).WindowState = FormWindowState.Maximized;
            ppd.Document.PrintPage += printParser.BuildPrintPage;

            // Set page size by checking the first PGD. Width and height are in 1/100 inch
            PGD pgd = parser.StructuredFields.OfType<PGD>().First();
            int xWidth = (int)(Lookups.GetInches(pgd.XSize, pgd.UnitsPerXBase, pgd.BaseUnit) * 100);
            int yWidth = (int)(Lookups.GetInches(pgd.YSize, pgd.UnitsPerYBase, pgd.BaseUnit) * 100);
            ppd.Document.DefaultPageSettings.PaperSize = new PaperSize("Custom", xWidth, yWidth);

            ppd.ShowDialog();
        }
    }
}
