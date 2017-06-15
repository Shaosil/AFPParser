namespace AFPParser.UI
{
    partial class FrmManageResources
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.lstDirectories = new System.Windows.Forms.ListBox();
            this.dgvResources = new System.Windows.Forms.DataGridView();
            this.ResourceName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resourceTypeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.messageDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resourceBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.lblResources = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResources)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.resourceBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(12, 127);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(54, 23);
            this.btnAdd.TabIndex = 1;
            this.btnAdd.Text = "Add...";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemove.Enabled = false;
            this.btnRemove.Location = new System.Drawing.Point(417, 127);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(119, 23);
            this.btnRemove.TabIndex = 2;
            this.btnRemove.Text = "Remove Selected";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // lstDirectories
            // 
            this.lstDirectories.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstDirectories.FormattingEnabled = true;
            this.lstDirectories.Location = new System.Drawing.Point(12, 12);
            this.lstDirectories.Name = "lstDirectories";
            this.lstDirectories.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstDirectories.Size = new System.Drawing.Size(524, 108);
            this.lstDirectories.TabIndex = 0;
            this.lstDirectories.SelectedIndexChanged += new System.EventHandler(this.lstDirectories_SelectedIndexChanged);
            // 
            // dgvResources
            // 
            this.dgvResources.AllowUserToAddRows = false;
            this.dgvResources.AllowUserToDeleteRows = false;
            this.dgvResources.AutoGenerateColumns = false;
            this.dgvResources.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvResources.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvResources.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvResources.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ResourceName,
            this.resourceTypeDataGridViewTextBoxColumn,
            this.messageDataGridViewTextBoxColumn});
            this.dgvResources.DataSource = this.resourceBindingSource;
            this.dgvResources.Location = new System.Drawing.Point(12, 180);
            this.dgvResources.Name = "dgvResources";
            this.dgvResources.ReadOnly = true;
            this.dgvResources.RowHeadersVisible = false;
            this.dgvResources.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvResources.Size = new System.Drawing.Size(524, 374);
            this.dgvResources.TabIndex = 3;
            // 
            // ResourceName
            // 
            this.ResourceName.DataPropertyName = "ResourceName";
            this.ResourceName.FillWeight = 20F;
            this.ResourceName.HeaderText = "ResourceName";
            this.ResourceName.Name = "ResourceName";
            this.ResourceName.ReadOnly = true;
            // 
            // resourceTypeDataGridViewTextBoxColumn
            // 
            this.resourceTypeDataGridViewTextBoxColumn.DataPropertyName = "ResourceType";
            this.resourceTypeDataGridViewTextBoxColumn.FillWeight = 20F;
            this.resourceTypeDataGridViewTextBoxColumn.HeaderText = "ResourceType";
            this.resourceTypeDataGridViewTextBoxColumn.Name = "resourceTypeDataGridViewTextBoxColumn";
            this.resourceTypeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // messageDataGridViewTextBoxColumn
            // 
            this.messageDataGridViewTextBoxColumn.DataPropertyName = "Message";
            this.messageDataGridViewTextBoxColumn.FillWeight = 60F;
            this.messageDataGridViewTextBoxColumn.HeaderText = "Message";
            this.messageDataGridViewTextBoxColumn.Name = "messageDataGridViewTextBoxColumn";
            this.messageDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // resourceBindingSource
            // 
            this.resourceBindingSource.DataSource = typeof(AFPParser.AFPFile.Resource);
            // 
            // lblResources
            // 
            this.lblResources.AutoSize = true;
            this.lblResources.Location = new System.Drawing.Point(12, 164);
            this.lblResources.Name = "lblResources";
            this.lblResources.Size = new System.Drawing.Size(58, 13);
            this.lblResources.TabIndex = 4;
            this.lblResources.Text = "Resources";
            // 
            // FrmManageResources
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(548, 566);
            this.Controls.Add(this.lblResources);
            this.Controls.Add(this.dgvResources);
            this.Controls.Add(this.lstDirectories);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(564, 604);
            this.Name = "FrmManageResources";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FrmManageResources";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmManageResources_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResources)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.resourceBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.ListBox lstDirectories;
        private System.Windows.Forms.DataGridView dgvResources;
        private System.Windows.Forms.Label lblResources;
        private System.Windows.Forms.DataGridViewTextBoxColumn fileNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.BindingSource resourceBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn ResourceName;
        private System.Windows.Forms.DataGridViewTextBoxColumn resourceTypeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn messageDataGridViewTextBoxColumn;
    }
}