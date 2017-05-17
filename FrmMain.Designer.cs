namespace AFPParser
{
    partial class FrmMain
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
            this.btnBrowse = new System.Windows.Forms.Button();
            this.dgvFields = new System.Windows.Forms.DataGridView();
            this.afpFileBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.lblParsedData = new System.Windows.Forms.Label();
            this.IdentifierAbbr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IdentifierHexCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IdentifierTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.flagDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sequenceDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFields)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.afpFileBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(12, 12);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 0;
            this.btnBrowse.Text = "Select File...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // dgvFields
            // 
            this.dgvFields.AllowUserToAddRows = false;
            this.dgvFields.AllowUserToDeleteRows = false;
            this.dgvFields.AllowUserToOrderColumns = true;
            this.dgvFields.AllowUserToResizeRows = false;
            this.dgvFields.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dgvFields.AutoGenerateColumns = false;
            this.dgvFields.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvFields.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            this.dgvFields.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFields.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IdentifierAbbr,
            this.IdentifierHexCode,
            this.IdentifierTitle,
            this.flagDataGridViewTextBoxColumn,
            this.sequenceDataGridViewTextBoxColumn});
            this.dgvFields.DataSource = this.afpFileBindingSource;
            this.dgvFields.Location = new System.Drawing.Point(12, 41);
            this.dgvFields.Name = "dgvFields";
            this.dgvFields.ReadOnly = true;
            this.dgvFields.RowHeadersVisible = false;
            this.dgvFields.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFields.Size = new System.Drawing.Size(628, 609);
            this.dgvFields.TabIndex = 1;
            this.dgvFields.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvFields_CellDoubleClick);
            this.dgvFields.SelectionChanged += new System.EventHandler(this.dgvFields_SelectionChanged);
            // 
            // afpFileBindingSource
            // 
            this.afpFileBindingSource.DataMember = "AfpFile";
            this.afpFileBindingSource.DataSource = typeof(AFPParser.Parser);
            // 
            // txtDescription
            // 
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescription.BackColor = System.Drawing.SystemColors.Window;
            this.txtDescription.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDescription.Location = new System.Drawing.Point(646, 41);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ReadOnly = true;
            this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescription.Size = new System.Drawing.Size(576, 609);
            this.txtDescription.TabIndex = 3;
            // 
            // lblParsedData
            // 
            this.lblParsedData.AutoSize = true;
            this.lblParsedData.Location = new System.Drawing.Point(643, 22);
            this.lblParsedData.Name = "lblParsedData";
            this.lblParsedData.Size = new System.Drawing.Size(66, 13);
            this.lblParsedData.TabIndex = 4;
            this.lblParsedData.Text = "Parsed Data";
            // 
            // IdentifierAbbr
            // 
            this.IdentifierAbbr.DataPropertyName = "Abbreviation";
            this.IdentifierAbbr.FillWeight = 5F;
            this.IdentifierAbbr.HeaderText = "Identifier";
            this.IdentifierAbbr.Name = "IdentifierAbbr";
            this.IdentifierAbbr.ReadOnly = true;
            this.IdentifierAbbr.Width = 60;
            // 
            // IdentifierHexCode
            // 
            this.IdentifierHexCode.DataPropertyName = "ID";
            this.IdentifierHexCode.FillWeight = 8F;
            this.IdentifierHexCode.HeaderText = "Hex Code";
            this.IdentifierHexCode.Name = "IdentifierHexCode";
            this.IdentifierHexCode.ReadOnly = true;
            this.IdentifierHexCode.Width = 97;
            // 
            // IdentifierTitle
            // 
            this.IdentifierTitle.DataPropertyName = "Title";
            this.IdentifierTitle.FillWeight = 25F;
            this.IdentifierTitle.HeaderText = "Description";
            this.IdentifierTitle.Name = "IdentifierTitle";
            this.IdentifierTitle.ReadOnly = true;
            this.IdentifierTitle.Width = 302;
            // 
            // flagDataGridViewTextBoxColumn
            // 
            this.flagDataGridViewTextBoxColumn.DataPropertyName = "Flag";
            this.flagDataGridViewTextBoxColumn.FillWeight = 5F;
            this.flagDataGridViewTextBoxColumn.HeaderText = "Flag";
            this.flagDataGridViewTextBoxColumn.Name = "flagDataGridViewTextBoxColumn";
            this.flagDataGridViewTextBoxColumn.ReadOnly = true;
            this.flagDataGridViewTextBoxColumn.Width = 60;
            // 
            // sequenceDataGridViewTextBoxColumn
            // 
            this.sequenceDataGridViewTextBoxColumn.DataPropertyName = "Sequence";
            this.sequenceDataGridViewTextBoxColumn.FillWeight = 7F;
            this.sequenceDataGridViewTextBoxColumn.HeaderText = "Sequence";
            this.sequenceDataGridViewTextBoxColumn.Name = "sequenceDataGridViewTextBoxColumn";
            this.sequenceDataGridViewTextBoxColumn.ReadOnly = true;
            this.sequenceDataGridViewTextBoxColumn.Width = 85;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1234, 662);
            this.Controls.Add(this.lblParsedData);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.dgvFields);
            this.Controls.Add(this.btnBrowse);
            this.MinimumSize = new System.Drawing.Size(1250, 700);
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AFP Parser";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFields)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.afpFileBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.DataGridView dgvFields;
        private System.Windows.Forms.BindingSource afpFileBindingSource;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblParsedData;
        private System.Windows.Forms.DataGridViewTextBoxColumn IdentifierAbbr;
        private System.Windows.Forms.DataGridViewTextBoxColumn IdentifierHexCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn IdentifierTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn flagDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sequenceDataGridViewTextBoxColumn;
    }
}