using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace AFPParser
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
                    afpParser.Parse(dialog.FileName);

                    // Databind the list box
                    afpFileBindingSource.DataSource = null;
                    afpFileBindingSource.DataSource = afpParser.AfpFile;
                    dgvFields.Focus();
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
                txtDescription.Text = sf.BuildDescription().Trim();
            }
        }

        private void dgvFields_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Copy hex data to clipboard
            StructuredField sf = (StructuredField)dgvFields.CurrentRow.DataBoundItem;
            Clipboard.SetText(sf.DataHex);
            MessageBox.Show("Hex values copied to clipboard.", "Hex", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
    }
}
