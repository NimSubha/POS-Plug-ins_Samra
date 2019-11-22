namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmTransferOrder
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnWHSearch = new DevExpress.XtraEditors.SimpleButton();
            this.txtWarehouse = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.btnSKUSearch = new DevExpress.XtraEditors.SimpleButton();
            this.lblSKUNo = new System.Windows.Forms.Label();
            this.txtSKUNo = new System.Windows.Forms.TextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnEnter = new DevExpress.XtraEditors.SimpleButton();
            this.lblAirwayBillNo = new System.Windows.Forms.Label();
            this.txtAirwayBillNo = new System.Windows.Forms.TextBox();
            this.lblWayBillNo = new System.Windows.Forms.Label();
            this.txtWayBillNo = new System.Windows.Forms.TextBox();
            this.btnSKUClear = new DevExpress.XtraEditors.SimpleButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.grItems = new DevExpress.XtraGrid.GridControl();
            this.grdView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.ColSKUNo = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblTitle.Location = new System.Drawing.Point(238, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(245, 32);
            this.lblTitle.TabIndex = 14;
            this.lblTitle.Text = "Transfer Order Create";
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.btnWHSearch);
            this.panel2.Controls.Add(this.txtWarehouse);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Location = new System.Drawing.Point(9, 75);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(720, 65);
            this.panel2.TabIndex = 15;
            // 
            // btnWHSearch
            // 
            this.btnWHSearch.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.search;
            this.btnWHSearch.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnWHSearch.Location = new System.Drawing.Point(278, 18);
            this.btnWHSearch.Name = "btnWHSearch";
            this.btnWHSearch.Size = new System.Drawing.Size(57, 32);
            this.btnWHSearch.TabIndex = 6;
            this.btnWHSearch.Text = "Search";
            this.btnWHSearch.Click += new System.EventHandler(this.btnWHSearch_Click);
            // 
            // txtWarehouse
            // 
            this.txtWarehouse.BackColor = System.Drawing.SystemColors.Control;
            this.txtWarehouse.Location = new System.Drawing.Point(102, 23);
            this.txtWarehouse.MaxLength = 20;
            this.txtWarehouse.Name = "txtWarehouse";
            this.txtWarehouse.ReadOnly = true;
            this.txtWarehouse.Size = new System.Drawing.Size(170, 21);
            this.txtWarehouse.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label2.Location = new System.Drawing.Point(6, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "To Warehouse:";
            // 
            // btnSubmit
            // 
            this.btnSubmit.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSubmit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnSubmit.Location = new System.Drawing.Point(532, 428);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(94, 43);
            this.btnSubmit.TabIndex = 16;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // btnSKUSearch
            // 
            this.btnSKUSearch.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.search;
            this.btnSKUSearch.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSKUSearch.Location = new System.Drawing.Point(405, 12);
            this.btnSKUSearch.Name = "btnSKUSearch";
            this.btnSKUSearch.Size = new System.Drawing.Size(57, 32);
            this.btnSKUSearch.TabIndex = 9;
            this.btnSKUSearch.Text = "Search";
            this.btnSKUSearch.Visible = false;
            this.btnSKUSearch.Click += new System.EventHandler(this.btnSKUSearch_Click);
            // 
            // lblSKUNo
            // 
            this.lblSKUNo.AutoSize = true;
            this.lblSKUNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblSKUNo.Location = new System.Drawing.Point(7, 20);
            this.lblSKUNo.Name = "lblSKUNo";
            this.lblSKUNo.Size = new System.Drawing.Size(94, 13);
            this.lblSKUNo.TabIndex = 7;
            this.lblSKUNo.Text = "SKU No / Item No:";
            // 
            // txtSKUNo
            // 
            this.txtSKUNo.BackColor = System.Drawing.SystemColors.Control;
            this.txtSKUNo.Location = new System.Drawing.Point(102, 18);
            this.txtSKUNo.MaxLength = 60;
            this.txtSKUNo.Name = "txtSKUNo";
            this.txtSKUNo.Size = new System.Drawing.Size(170, 21);
            this.txtSKUNo.TabIndex = 8;
            this.txtSKUNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSKUNo_KeyDown);
            this.txtSKUNo.Leave += new System.EventHandler(this.txtSKUNo_Leave);
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.btnEnter);
            this.panel3.Controls.Add(this.lblAirwayBillNo);
            this.panel3.Controls.Add(this.txtAirwayBillNo);
            this.panel3.Controls.Add(this.lblWayBillNo);
            this.panel3.Controls.Add(this.txtWayBillNo);
            this.panel3.Controls.Add(this.btnSKUClear);
            this.panel3.Controls.Add(this.lblSKUNo);
            this.panel3.Controls.Add(this.btnSKUSearch);
            this.panel3.Controls.Add(this.txtSKUNo);
            this.panel3.Location = new System.Drawing.Point(9, 146);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(720, 96);
            this.panel3.TabIndex = 18;
            // 
            // btnEnter
            // 
            this.btnEnter.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.down;
            this.btnEnter.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnEnter.Location = new System.Drawing.Point(278, 12);
            this.btnEnter.Name = "btnEnter";
            this.btnEnter.Size = new System.Drawing.Size(57, 32);
            this.btnEnter.TabIndex = 179;
            this.btnEnter.Text = "Clear";
            this.btnEnter.Click += new System.EventHandler(this.btnEnter_Click);
            // 
            // lblAirwayBillNo
            // 
            this.lblAirwayBillNo.AutoSize = true;
            this.lblAirwayBillNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblAirwayBillNo.Location = new System.Drawing.Point(337, 64);
            this.lblAirwayBillNo.Name = "lblAirwayBillNo";
            this.lblAirwayBillNo.Size = new System.Drawing.Size(75, 13);
            this.lblAirwayBillNo.TabIndex = 182;
            this.lblAirwayBillNo.Text = "Airway Bill No:";
            // 
            // txtAirwayBillNo
            // 
            this.txtAirwayBillNo.BackColor = System.Drawing.SystemColors.Control;
            this.txtAirwayBillNo.Location = new System.Drawing.Point(432, 62);
            this.txtAirwayBillNo.MaxLength = 20;
            this.txtAirwayBillNo.Name = "txtAirwayBillNo";
            this.txtAirwayBillNo.Size = new System.Drawing.Size(170, 21);
            this.txtAirwayBillNo.TabIndex = 183;
            // 
            // lblWayBillNo
            // 
            this.lblWayBillNo.AutoSize = true;
            this.lblWayBillNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblWayBillNo.Location = new System.Drawing.Point(7, 64);
            this.lblWayBillNo.Name = "lblWayBillNo";
            this.lblWayBillNo.Size = new System.Drawing.Size(64, 13);
            this.lblWayBillNo.TabIndex = 180;
            this.lblWayBillNo.Text = "Way Bill No:";
            // 
            // txtWayBillNo
            // 
            this.txtWayBillNo.BackColor = System.Drawing.SystemColors.Control;
            this.txtWayBillNo.Location = new System.Drawing.Point(102, 62);
            this.txtWayBillNo.MaxLength = 20;
            this.txtWayBillNo.Name = "txtWayBillNo";
            this.txtWayBillNo.Size = new System.Drawing.Size(170, 21);
            this.txtWayBillNo.TabIndex = 181;
            // 
            // btnSKUClear
            // 
            this.btnSKUClear.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.remove;
            this.btnSKUClear.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSKUClear.Location = new System.Drawing.Point(341, 12);
            this.btnSKUClear.Name = "btnSKUClear";
            this.btnSKUClear.Size = new System.Drawing.Size(57, 32);
            this.btnSKUClear.TabIndex = 179;
            this.btnSKUClear.Text = "Clear";
            this.btnSKUClear.Click += new System.EventHandler(this.btnClearProduct_Click);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.grItems);
            this.panel1.Location = new System.Drawing.Point(10, 249);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(718, 173);
            this.panel1.TabIndex = 19;
            // 
            // grItems
            // 
            this.grItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grItems.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.grItems.Location = new System.Drawing.Point(4, 3);
            this.grItems.MainView = this.grdView;
            this.grItems.Name = "grItems";
            this.grItems.Size = new System.Drawing.Size(711, 165);
            this.grItems.TabIndex = 12;
            this.grItems.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdView});
            // 
            // grdView
            // 
            this.grdView.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.grdView.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.grdView.Appearance.HeaderPanel.Options.UseFont = true;
            this.grdView.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.grdView.Appearance.Row.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.grdView.Appearance.Row.Options.UseFont = true;
            this.grdView.Appearance.Row.Options.UseForeColor = true;
            this.grdView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.ColSKUNo});
            this.grdView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.grdView.GridControl = this.grItems;
            this.grdView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.grdView.Name = "grdView";
            this.grdView.OptionsBehavior.Editable = false;
            this.grdView.OptionsCustomization.AllowFilter = false;
            this.grdView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.grdView.OptionsView.AutoCalcPreviewLineCount = true;
            this.grdView.OptionsView.ShowGroupPanel = false;
            this.grdView.OptionsView.ShowIndicator = false;
            this.grdView.OptionsView.ShowPreview = true;
            this.grdView.PreviewFieldName = "DIMENSIONS";
            this.grdView.PreviewIndent = 2;
            this.grdView.PreviewLineCount = 1;
            this.grdView.RowHeight = 30;
            this.grdView.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.None;
            this.grdView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.Default;
            this.grdView.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            // 
            // ColSKUNo
            // 
            this.ColSKUNo.Caption = "  ";
            this.ColSKUNo.FieldName = "SKUNumber";
            this.ColSKUNo.Name = "ColSKUNo";
            this.ColSKUNo.Visible = true;
            this.ColSKUNo.VisibleIndex = 0;
            this.ColSKUNo.Width = 111;
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnClose.Location = new System.Drawing.Point(635, 428);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(94, 43);
            this.btnClose.TabIndex = 20;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnDelete.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnDelete.Location = new System.Drawing.Point(428, 428);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(94, 43);
            this.btnDelete.TabIndex = 21;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // frmTransferOrder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(739, 481);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.lblTitle);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmTransferOrder";
            this.Text = "Transfer Order Create";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnSubmit;
        private DevExpress.XtraEditors.SimpleButton btnSKUSearch;
        private DevExpress.XtraEditors.SimpleButton btnWHSearch;
        private System.Windows.Forms.Label lblSKUNo;
        public System.Windows.Forms.TextBox txtSKUNo;
        public System.Windows.Forms.TextBox txtWarehouse;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraGrid.GridControl grItems;
        private DevExpress.XtraGrid.Views.Grid.GridView grdView;
        private DevExpress.XtraGrid.Columns.GridColumn ColSKUNo;
        private DevExpress.XtraEditors.SimpleButton btnSKUClear;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblAirwayBillNo;
        public System.Windows.Forms.TextBox txtAirwayBillNo;
        private System.Windows.Forms.Label lblWayBillNo;
        public System.Windows.Forms.TextBox txtWayBillNo;
        private DevExpress.XtraEditors.SimpleButton btnEnter;
        private System.Windows.Forms.Button btnDelete;
    }
}