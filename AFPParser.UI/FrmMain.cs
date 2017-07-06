using AFPParser.Containers;
using AFPParser.StructuredFields;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace AFPParser.UI
{
    public partial class FrmMain : Form
    {
        public enum eFileType { Unknown, Document, IOCAImage, IMImage, Font }

        private readonly string optionsFile = Environment.CurrentDirectory + "\\Options.xml";
        private Options opts;
        private AFPFile afpFile;
        private PrintParser printParser;
        private eFileType DocType;

        public FrmMain()
        {
            InitializeComponent();

            // Store things like last opened directory
            opts = Options.LoadSettings(optionsFile);

            afpFile = new AFPFile();
            if (opts.ResourceDirectories.Any()) afpFile.ResourceDirectories = opts.ResourceDirectories;
            afpFile.ErrorEvent += (string message) => { MessageBox.Show(message, "Parser Error", MessageBoxButtons.OK, MessageBoxIcon.Error); };
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                InitialDirectory = opts.LastDirectory,
                Filter = "AFP Files (*.afp)|*.afp|All Files|*.*"
            };
            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;

                    // Store the last used options
                    FileInfo fInfo = new FileInfo(dialog.FileName);
                    opts.LastDirectory = fInfo.DirectoryName;
                    opts.LastOpenedFile = fInfo.Name;

                    // Parse the AFP file
                    if (afpFile.LoadData(dialog.FileName, true))
                    {
                        // Data bind the list box
                        afpFileBindingSource.DataSource = null;
                        afpFileBindingSource.DataSource = afpFile.Fields;
                        dgvFields.Focus();

                        // Enable/disable the preview button if there are pages, or the first field is a page segment (resource)
                        DocType = GetFileType();
                        btnPreview.Enabled = DocType == eFileType.Document || DocType == eFileType.IOCAImage || DocType == eFileType.IMImage;
                        btnManageResources.Enabled = true;

                        // Change form title
                        Text = $"AFP Parser - {fInfo.Name}";

                        // Create new print parser object for this file
                        printParser = new PrintParser(afpFile);
                    }
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private eFileType GetFileType()
        {
            eFileType fType = eFileType.Unknown;

            if (afpFile != null && afpFile.Fields.Any())
            {
                // If there are pages, it's a document
                if (afpFile.Fields.OfType<BPG>().Any())
                    fType = eFileType.Document;
                else
                {
                    // If it's a page segment, check for images or fonts
                    Type f1 = afpFile.Fields[0].GetType();
                    Type f2 = afpFile.Fields[1].GetType();
                    if (f1 == typeof(BPS) && f2 == typeof(BIM))
                        fType = eFileType.IOCAImage;
                    else if (f1 == typeof(BPS) && f2 == typeof(BII))
                        fType = eFileType.IMImage;
                    else if (f1 == typeof(BFN))
                        fType = eFileType.Font;
                }
            }

            return fType;
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

        private void btnManageResources_Click(object sender, EventArgs e)
        {
            new FrmManageResources(afpFile, opts).ShowDialog();
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            // Only display a print preview if it's a paged document. Otherwise, display an image from the page segment if we can
            switch (DocType)
            {
                case eFileType.Document:
                    // Verify they want to view if there are missing resources
                    if (afpFile.Resources.All(r => r.IsLoaded || r.IsNETCodePage) || MessageBox.Show("There are referenced resources that have not been located. Preview anyway?"
                    , "Missing Resources", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        // Set up a print preview dialog and wire it to our print parser's build event
                        PrintPreviewDialog ppd = new PrintPreviewDialog() { Document = new PrintDocument() { DocumentName = opts.LastOpenedFile } };
                        ppd.Controls.OfType<ToolStrip>().First().Items["printToolStripButton"].Visible = false; // Temp disable until we actually might want to print something
                        ((Form)ppd).WindowState = FormWindowState.Maximized;
                        ppd.Document.PrintPage += printParser.BuildPrintPage;

                        // Set page size by checking the first PGD. Width and height are in 1/100 inch
                        PGD pgd = afpFile.Fields.OfType<PGD>().First();
                        int xWidth = (int)(Converters.GetInches(pgd.XSize, pgd.UnitsPerXBase, pgd.BaseUnit) * 100);
                        int yWidth = (int)(Converters.GetInches(pgd.YSize, pgd.UnitsPerYBase, pgd.BaseUnit) * 100);
                        ppd.Document.DefaultPageSettings.PaperSize = new PaperSize("Custom", xWidth, yWidth);

                        ppd.ShowDialog();
                    }

                    break;

                case eFileType.IOCAImage:
                case eFileType.IMImage:
                    int fileCounter = 1;

                    List<IOCAImageContainer> iocs = afpFile.Fields.Select(f => f.LowestLevelContainer).OfType<IOCAImageContainer>().Distinct().ToList();
                    List<IMImageContainer> imcs = afpFile.Fields.Select(f => f.LowestLevelContainer).OfType<IMImageContainer>().Distinct().ToList();

                    if (iocs.Any() || imcs.Any())
                    {
                        Cursor = Cursors.WaitCursor;

                        // Clear out the directory of existing pngs
                        foreach (string file in Directory.GetFiles(Environment.CurrentDirectory))
                            if (new FileInfo(file).Extension.ToUpper() == ".PNG")
                                File.Delete(file);

                        // Generate a .png from the image data and save it to the exe directory
                        if (DocType == eFileType.IOCAImage)
                        {
                            foreach (IOCAImageContainer ioc in iocs)
                            {
                                foreach (ImageContentContainer.ImageInfo image in ioc.Images)
                                {
                                    Bitmap png = new Bitmap(new MemoryStream(image.Data));

                                    // Get resolution from descriptor
                                    IDD descriptor = ioc.GetStructure<IDD>();
                                    float xScale = (float)Converters.GetInches(png.Width, descriptor.HResolution, descriptor.BaseUnit);
                                    float yScale = (float)Converters.GetInches(png.Height, descriptor.VResolution, descriptor.BaseUnit);
                                    png.SetResolution(png.Width / xScale, png.Height / yScale);

                                    // Generate image
                                    png.Save($"{Environment.CurrentDirectory}\\Image {fileCounter++}.png", System.Drawing.Imaging.ImageFormat.Png);
                                }
                            }
                        }
                        else
                            foreach (IMImageContainer imc in imcs)
                                imc.GenerateBitmap().Save($"{Environment.CurrentDirectory}\\Image {fileCounter++}.png", System.Drawing.Imaging.ImageFormat.Png);

                        btnPreview.Enabled = false;
                        Cursor = Cursors.Default;
                        if (MessageBox.Show($"{fileCounter} image(s) created in executing directory. Open directory?",
                            "Images Created", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
                            System.Diagnostics.Process.Start(Environment.CurrentDirectory);
                    }
                    else
                    {
                        MessageBox.Show("No image containers found, though this does appear to be an image file.");
                    }
                    break;
            }
        }
    }
}