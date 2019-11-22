/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
namespace Microsoft.Dynamics.Retail.Pos.Interaction
{
    partial class frmDimensions
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
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.configButton = new DevExpress.XtraEditors.CheckButton();
            this.styleButton = new DevExpress.XtraEditors.CheckButton();
            this.sizeButton = new DevExpress.XtraEditors.CheckButton();
            this.colorButton = new DevExpress.XtraEditors.CheckButton();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.lblHeader = new System.Windows.Forms.Label();
            this.btnCancel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.panelbase = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.panelbase.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelControl1.Controls.Add(this.flowLayoutPanel);
            this.panelControl1.Location = new System.Drawing.Point(219, 138);
            this.panelControl1.Name = "panelControl1";
            this.tableLayoutPanel.SetRowSpan(this.panelControl1, 4);
            this.panelControl1.Size = new System.Drawing.Size(766, 534);
            this.panelControl1.TabIndex = 0;
            // 
            // flowLayoutPanel
            // 
            this.flowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel.Location = new System.Drawing.Point(2, 2);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Size = new System.Drawing.Size(762, 530);
            this.flowLayoutPanel.TabIndex = 6;
            // 
            // configButton
            // 
            this.configButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.configButton.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.configButton.Appearance.Options.UseFont = true;
            this.configButton.Location = new System.Drawing.Point(33, 579);
            this.configButton.Name = "configButton";
            this.configButton.Size = new System.Drawing.Size(130, 57);
            this.configButton.TabIndex = 9;
            this.configButton.Text = "Configuration";
            this.configButton.Visible = false;
            this.configButton.CheckedChanged += new System.EventHandler(this.OnConfigButton_CheckedChanged);
            this.configButton.Click += new System.EventHandler(this.OnConfigButton_Click);
            // 
            // styleButton
            // 
            this.styleButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.styleButton.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.styleButton.Appearance.Options.UseFont = true;
            this.styleButton.Location = new System.Drawing.Point(33, 444);
            this.styleButton.Name = "styleButton";
            this.styleButton.Size = new System.Drawing.Size(130, 57);
            this.styleButton.TabIndex = 8;
            this.styleButton.Text = "Style";
            this.styleButton.Visible = false;
            this.styleButton.CheckedChanged += new System.EventHandler(this.OnStyleButton_CheckedChanged);
            this.styleButton.Click += new System.EventHandler(this.OnStyleButton_Click);
            // 
            // sizeButton
            // 
            this.sizeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.sizeButton.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sizeButton.Appearance.Options.UseFont = true;
            this.sizeButton.Location = new System.Drawing.Point(33, 309);
            this.sizeButton.Name = "sizeButton";
            this.sizeButton.Size = new System.Drawing.Size(130, 57);
            this.sizeButton.TabIndex = 7;
            this.sizeButton.Text = "Size";
            this.sizeButton.Visible = false;
            this.sizeButton.CheckedChanged += new System.EventHandler(this.OnSizeButton_CheckedChanged);
            this.sizeButton.Click += new System.EventHandler(this.OnSizeButton_Click);
            // 
            // colorButton
            // 
            this.colorButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.colorButton.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colorButton.Appearance.Options.UseFont = true;
            this.colorButton.Location = new System.Drawing.Point(33, 174);
            this.colorButton.Name = "colorButton";
            this.colorButton.Size = new System.Drawing.Size(130, 57);
            this.colorButton.TabIndex = 5;
            this.colorButton.Text = "Color";
            this.colorButton.Visible = false;
            this.colorButton.CheckedChanged += new System.EventHandler(this.OnColorButton_CheckedChanged);
            this.colorButton.Click += new System.EventHandler(this.OnColorButton_Click);
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 3;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.Controls.Add(this.panelControl1, 2, 1);
            this.tableLayoutPanel.Controls.Add(this.colorButton, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.configButton, 0, 4);
            this.tableLayoutPanel.Controls.Add(this.sizeButton, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.styleButton, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.lblHeader, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.btnCancel, 0, 5);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.Padding = new System.Windows.Forms.Padding(30, 40, 30, 15);
            this.tableLayoutPanel.RowCount = 6;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(1018, 766);
            this.tableLayoutPanel.TabIndex = 10;
            this.tableLayoutPanel.CellPaint += new System.Windows.Forms.TableLayoutCellPaintEventHandler(this.OnTableLayoutPanel_CellPaint);
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.tableLayoutPanel.SetColumnSpan(this.lblHeader, 3);
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI Light", 36F);
            this.lblHeader.Location = new System.Drawing.Point(33, 40);
            this.lblHeader.Margin = new System.Windows.Forms.Padding(3, 0, 3, 30);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(790, 65);
            this.lblHeader.TabIndex = 10;
            this.lblHeader.Text = "Select Color, Size, Style, Configuration";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.tableLayoutPanel.SetColumnSpan(this.btnCancel, 3);
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(445, 690);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(127, 57);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            // 
            // panelbase
            // 
            this.panelbase.Controls.Add(this.tableLayoutPanel);
            this.panelbase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelbase.Location = new System.Drawing.Point(0, 0);
            this.panelbase.Name = "panelbase";
            this.panelbase.Size = new System.Drawing.Size(1018, 766);
            this.panelbase.TabIndex = 11;
            // 
            // frmDimensions
            // 
            this.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Appearance.Options.UseFont = true;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1018, 766);
            this.Controls.Add(this.panelbase);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmDimensions";
            this.Controls.SetChildIndex(this.panelbase, 0);
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.panelbase.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
        private DevExpress.XtraEditors.CheckButton colorButton;
        private DevExpress.XtraEditors.CheckButton sizeButton;
        private DevExpress.XtraEditors.CheckButton configButton;
        private DevExpress.XtraEditors.CheckButton styleButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
		private System.Windows.Forms.Label lblHeader;
		private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCancel;
		private System.Windows.Forms.Panel panelbase;
    }
}