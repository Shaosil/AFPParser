using System;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

namespace AFPParser.UI
{
    public partial class FrmManageResources : Form
    {
        private BindingList<string> resourceDirectories;
        private AFPFile File;

        public FrmManageResources(AFPFile file)
        {
            InitializeComponent();
            File = file;
            resourceDirectories = new BindingList<string>(file.ResourceDirectories);

            lstDirectories.DataSource = resourceDirectories;
            dgvResources.DataSource = file.Resources;
        }

        private void lstDirectories_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            btnRemove.Enabled = lstDirectories.SelectedItems.Count > 0;
        }

        private void btnAdd_Click(object sender, System.EventArgs e)
        {
            string selectedPath = lstDirectories.Items.Count > 0 ? lstDirectories.SelectedItems[0].ToString() : Environment.CurrentDirectory;
            FolderBrowserDialog fbd = new FolderBrowserDialog() { SelectedPath = selectedPath };
            if (fbd.ShowDialog() == DialogResult.OK && !resourceDirectories.Contains(fbd.SelectedPath))
            {
                resourceDirectories.Add(fbd.SelectedPath);
                RescanResources();
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            foreach (string s in lstDirectories.SelectedItems.Cast<string>().ToList())
                resourceDirectories.Remove(s);

            RescanResources();
        }

        private void RescanResources()
        {
            File.ScanDirectoriesForResources(File.Resources.Where(r => !r.IsLoaded));
            dgvResources.DataSource = File.Resources;
        }
    }
}
