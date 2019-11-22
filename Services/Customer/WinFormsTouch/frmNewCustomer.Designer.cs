/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
namespace Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch
{
    partial class frmNewCustomer
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
            this.themedPanel = new DevExpress.XtraEditors.PanelControl();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelCenter = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelBottom = new System.Windows.Forms.TableLayoutPanel();
            this.label10 = new System.Windows.Forms.Label();
            this.btnCancel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSave = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.bindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.btnClear = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.tabControlParent = new DevExpress.XtraTab.XtraTabControl();
            this.tabPageContact = new DevExpress.XtraTab.XtraTabPage();
            this.panelDetailsTab = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelLeft = new System.Windows.Forms.TableLayoutPanel();
            this.label11 = new System.Windows.Forms.Label();
            this.txtSTD = new DevExpress.XtraEditors.TextEdit();
            this.txtMobilePrimary = new DevExpress.XtraEditors.TextEdit();
            this.textBoxLastName = new DevExpress.XtraEditors.TextEdit();
            this.labelLastName = new System.Windows.Forms.Label();
            this.textBoxMiddleName = new DevExpress.XtraEditors.TextEdit();
            this.labelMiddleName = new System.Windows.Forms.Label();
            this.labelName = new System.Windows.Forms.Label();
            this.textBoxName = new DevExpress.XtraEditors.TextEdit();
            this.labelType = new System.Windows.Forms.Label();
            this.labelFirstName = new System.Windows.Forms.Label();
            this.textBoxFirstName = new DevExpress.XtraEditors.TextEdit();
            this.tableLayoutPanelType = new System.Windows.Forms.TableLayoutPanel();
            this.radioPerson = new System.Windows.Forms.RadioButton();
            this.radioOrg = new System.Windows.Forms.RadioButton();
            this.lblSalutation = new System.Windows.Forms.Label();
            this.txtSalutation = new DevExpress.XtraEditors.TextEdit();
            this.labelPhone = new System.Windows.Forms.Label();
            this.lblMobileSecondary = new System.Windows.Forms.Label();
            this.btnSalutation = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.lblMblPrimary = new System.Windows.Forms.Label();
            this.textBoxPhone = new DevExpress.XtraEditors.TextEdit();
            this.viewAddressUserControl1 = new Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch.ViewAddressUserControl();
            this.txtMobileSecondary = new DevExpress.XtraEditors.TextEdit();
            this.label3 = new System.Windows.Forms.Label();
            this.chkResidence = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.tableLayoutPanelRight = new System.Windows.Forms.TableLayoutPanel();
            this.label12 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtGender = new DevExpress.XtraEditors.TextEdit();
            this.textBoxLanguage = new DevExpress.XtraEditors.TextEdit();
            this.labelLanguage = new System.Windows.Forms.Label();
            this.labelGroup = new System.Windows.Forms.Label();
            this.textBoxCurrency = new DevExpress.XtraEditors.TextEdit();
            this.labelEmail = new System.Windows.Forms.Label();
            this.labelCurrency = new System.Windows.Forms.Label();
            this.textBoxEmail = new DevExpress.XtraEditors.TextEdit();
            this.textBoxGroup = new DevExpress.XtraEditors.TextEdit();
            this.lblDOB = new System.Windows.Forms.Label();
            this.lblGender = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textCustAgeBracket = new DevExpress.XtraEditors.TextEdit();
            this.label2 = new System.Windows.Forms.Label();
            this.textNationality = new DevExpress.XtraEditors.TextEdit();
            this.lblMarriageDate = new System.Windows.Forms.Label();
            this.btnGroup = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnCurrency = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnLanguage = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnGender = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.txtBDay = new DevExpress.XtraEditors.TextEdit();
            this.txtBYear = new DevExpress.XtraEditors.TextEdit();
            this.btnNationality = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnAgeBracket = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.txtAnnDay = new DevExpress.XtraEditors.TextEdit();
            this.txtAnnYear = new DevExpress.XtraEditors.TextEdit();
            this.cmbBMonth = new System.Windows.Forms.ComboBox();
            this.cmbAnnMonth = new System.Windows.Forms.ComboBox();
            this.lblReligion = new System.Windows.Forms.Label();
            this.lblOccupation = new System.Windows.Forms.Label();
            this.txtCustClassificationGroup = new DevExpress.XtraEditors.TextEdit();
            this.txtOccupation = new DevExpress.XtraEditors.TextEdit();
            this.btnCustClassificationGrp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxReceiptEmail = new DevExpress.XtraEditors.TextEdit();
            this.textBoxWebSite = new DevExpress.XtraEditors.TextEdit();
            this.labelReceiptEmail = new System.Windows.Forms.Label();
            this.tabPageHistory = new DevExpress.XtraTab.XtraTabPage();
            this.panelHistoryTab = new System.Windows.Forms.TableLayoutPanel();
            this.labelDateCreated = new System.Windows.Forms.Label();
            this.labelTotalVisits = new System.Windows.Forms.Label();
            this.gridOrders = new DevExpress.XtraGrid.GridControl();
            this.gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.columnDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.columnOrderNumber = new DevExpress.XtraGrid.Columns.GridColumn();
            this.columnOrderStatus = new DevExpress.XtraGrid.Columns.GridColumn();
            this.columnStore = new DevExpress.XtraGrid.Columns.GridColumn();
            this.columnItem = new DevExpress.XtraGrid.Columns.GridColumn();
            this.columnQuantity = new DevExpress.XtraGrid.Columns.GridColumn();
            this.columnAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.textDateCreated = new DevExpress.XtraEditors.TextEdit();
            this.textTotalVisits = new DevExpress.XtraEditors.TextEdit();
            this.btnPgDown = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnDown = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.labelDateLastVisit = new System.Windows.Forms.Label();
            this.textDateLastVisit = new DevExpress.XtraEditors.TextEdit();
            this.labelStoreLastVisited = new System.Windows.Forms.Label();
            this.textStoreLastVisited = new DevExpress.XtraEditors.TextEdit();
            this.labelTotalSales = new System.Windows.Forms.Label();
            this.textTotalSales = new DevExpress.XtraEditors.TextEdit();
            this.labelSearch = new System.Windows.Forms.Label();
            this.textSearch = new DevExpress.XtraEditors.TextEdit();
            this.buttonSearch = new DevExpress.XtraEditors.SimpleButton();
            this.buttonClear = new DevExpress.XtraEditors.SimpleButton();
            this.btnUp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnPgUp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.lblHeader = new System.Windows.Forms.Label();
            this.labelCpfCnpjNumber = new System.Windows.Forms.Label();
            this.textBoxCpfCnpjNumber = new DevExpress.XtraEditors.TextEdit();
            this.btnCity = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.themedPanel)).BeginInit();
            this.themedPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanelCenter.SuspendLayout();
            this.tableLayoutPanelBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabControlParent)).BeginInit();
            this.tabControlParent.SuspendLayout();
            this.tabPageContact.SuspendLayout();
            this.panelDetailsTab.SuspendLayout();
            this.tableLayoutPanelLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSTD.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMobilePrimary.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxLastName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxMiddleName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxFirstName.Properties)).BeginInit();
            this.tableLayoutPanelType.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSalutation.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxPhone.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMobileSecondary.Properties)).BeginInit();
            this.tableLayoutPanelRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtGender.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxLanguage.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxCurrency.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxEmail.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxGroup.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textCustAgeBracket.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textNationality.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBDay.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBYear.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAnnDay.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAnnYear.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustClassificationGroup.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOccupation.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxReceiptEmail.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxWebSite.Properties)).BeginInit();
            this.tabPageHistory.SuspendLayout();
            this.panelHistoryTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridOrders)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textDateCreated.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textTotalVisits.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textDateLastVisit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textStoreLastVisited.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textTotalSales.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textSearch.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxCpfCnpjNumber.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // themedPanel
            // 
            this.themedPanel.Controls.Add(this.tableLayoutPanel1);
            this.themedPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.themedPanel.Location = new System.Drawing.Point(0, 0);
            this.themedPanel.Margin = new System.Windows.Forms.Padding(0);
            this.themedPanel.Name = "themedPanel";
            this.themedPanel.Size = new System.Drawing.Size(1029, 780);
            this.themedPanel.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoScroll = true;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanelCenter, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblHeader, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(30, 10, 30, 0);
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1025, 776);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanelCenter
            // 
            this.tableLayoutPanelCenter.ColumnCount = 1;
            this.tableLayoutPanelCenter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelCenter.Controls.Add(this.tableLayoutPanelBottom, 0, 1);
            this.tableLayoutPanelCenter.Controls.Add(this.tabControlParent, 0, 0);
            this.tableLayoutPanelCenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelCenter.Location = new System.Drawing.Point(33, 78);
            this.tableLayoutPanelCenter.Name = "tableLayoutPanelCenter";
            this.tableLayoutPanelCenter.RowCount = 2;
            this.tableLayoutPanelCenter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelCenter.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelCenter.Size = new System.Drawing.Size(959, 695);
            this.tableLayoutPanelCenter.TabIndex = 0;
            // 
            // tableLayoutPanelBottom
            // 
            this.tableLayoutPanelBottom.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelBottom.AutoSize = true;
            this.tableLayoutPanelBottom.ColumnCount = 3;
            this.tableLayoutPanelBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelBottom.Controls.Add(this.label10, 0, 1);
            this.tableLayoutPanelBottom.Controls.Add(this.btnCancel, 2, 0);
            this.tableLayoutPanelBottom.Controls.Add(this.btnSave, 1, 0);
            this.tableLayoutPanelBottom.Controls.Add(this.btnClear, 0, 0);
            this.tableLayoutPanelBottom.Location = new System.Drawing.Point(3, 619);
            this.tableLayoutPanelBottom.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.tableLayoutPanelBottom.Name = "tableLayoutPanelBottom";
            this.tableLayoutPanelBottom.RowCount = 2;
            this.tableLayoutPanelBottom.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelBottom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelBottom.Size = new System.Drawing.Size(953, 73);
            this.tableLayoutPanelBottom.TabIndex = 2;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.tableLayoutPanelBottom.SetColumnSpan(this.label10, 2);
            this.label10.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.Red;
            this.label10.Location = new System.Drawing.Point(3, 55);
            this.label10.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(129, 13);
            this.label10.TabIndex = 31;
            this.label10.Text = "* is the mandatory field";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(551, 3);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(5, 3, 3, 10);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(130, 40);
            this.btnCancel.TabIndex = 30;
            this.btnCancel.Text = "Cancel";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSave.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Appearance.Options.UseFont = true;
            this.btnSave.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.bindingSource, "Validated", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.Location = new System.Drawing.Point(411, 3);
            this.btnSave.Margin = new System.Windows.Forms.Padding(5, 3, 5, 10);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(130, 40);
            this.btnSave.TabIndex = 24;
            this.btnSave.Text = "Save";
            // 
            // bindingSource
            // 
            this.bindingSource.DataSource = typeof(Microsoft.Dynamics.Retail.Pos.Customer.ViewModels.CustomerViewModel);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnClear.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Appearance.Options.UseFont = true;
            this.btnClear.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnClear.Location = new System.Drawing.Point(271, 3);
            this.btnClear.Margin = new System.Windows.Forms.Padding(3, 3, 5, 10);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(130, 40);
            this.btnClear.TabIndex = 28;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // tabControlParent
            // 
            this.tabControlParent.AppearancePage.Header.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControlParent.AppearancePage.Header.Options.UseFont = true;
            this.tabControlParent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlParent.Location = new System.Drawing.Point(3, 3);
            this.tabControlParent.Name = "tabControlParent";
            this.tabControlParent.SelectedTabPage = this.tabPageContact;
            this.tabControlParent.Size = new System.Drawing.Size(953, 598);
            this.tabControlParent.TabIndex = 1;
            this.tabControlParent.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.tabPageContact,
            this.tabPageHistory});
            this.tabControlParent.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.tabControlParent_SelectedPageChanged);
            // 
            // tabPageContact
            // 
            this.tabPageContact.Controls.Add(this.panelDetailsTab);
            this.tabPageContact.Controls.Add(this.textBoxReceiptEmail);
            this.tabPageContact.Controls.Add(this.textBoxWebSite);
            this.tabPageContact.Controls.Add(this.labelReceiptEmail);
            this.tabPageContact.Name = "tabPageContact";
            this.tabPageContact.Padding = new System.Windows.Forms.Padding(10);
            this.tabPageContact.Size = new System.Drawing.Size(947, 562);
            this.tabPageContact.Text = "Contact details";
            // 
            // panelDetailsTab
            // 
            this.panelDetailsTab.ColumnCount = 2;
            this.panelDetailsTab.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.panelDetailsTab.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.panelDetailsTab.Controls.Add(this.tableLayoutPanelLeft, 0, 0);
            this.panelDetailsTab.Controls.Add(this.tableLayoutPanelRight, 1, 0);
            this.panelDetailsTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDetailsTab.Location = new System.Drawing.Point(10, 10);
            this.panelDetailsTab.Name = "panelDetailsTab";
            this.panelDetailsTab.RowCount = 1;
            this.panelDetailsTab.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panelDetailsTab.Size = new System.Drawing.Size(927, 542);
            this.panelDetailsTab.TabIndex = 0;
            // 
            // tableLayoutPanelLeft
            // 
            this.tableLayoutPanelLeft.AllowDrop = true;
            this.tableLayoutPanelLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelLeft.AutoSize = true;
            this.tableLayoutPanelLeft.ColumnCount = 3;
            this.tableLayoutPanelLeft.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 31.28205F));
            this.tableLayoutPanelLeft.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 68.71795F));
            this.tableLayoutPanelLeft.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 66F));
            this.tableLayoutPanelLeft.Controls.Add(this.label11, 1, 2);
            this.tableLayoutPanelLeft.Controls.Add(this.txtSTD, 0, 13);
            this.tableLayoutPanelLeft.Controls.Add(this.txtMobilePrimary, 0, 16);
            this.tableLayoutPanelLeft.Controls.Add(this.textBoxLastName, 0, 9);
            this.tableLayoutPanelLeft.Controls.Add(this.labelLastName, 0, 8);
            this.tableLayoutPanelLeft.Controls.Add(this.textBoxMiddleName, 0, 7);
            this.tableLayoutPanelLeft.Controls.Add(this.labelMiddleName, 0, 6);
            this.tableLayoutPanelLeft.Controls.Add(this.labelName, 0, 10);
            this.tableLayoutPanelLeft.Controls.Add(this.textBoxName, 0, 11);
            this.tableLayoutPanelLeft.Controls.Add(this.labelType, 0, 0);
            this.tableLayoutPanelLeft.Controls.Add(this.labelFirstName, 0, 4);
            this.tableLayoutPanelLeft.Controls.Add(this.textBoxFirstName, 0, 5);
            this.tableLayoutPanelLeft.Controls.Add(this.tableLayoutPanelType, 0, 1);
            this.tableLayoutPanelLeft.Controls.Add(this.lblSalutation, 0, 2);
            this.tableLayoutPanelLeft.Controls.Add(this.txtSalutation, 0, 3);
            this.tableLayoutPanelLeft.Controls.Add(this.labelPhone, 0, 12);
            this.tableLayoutPanelLeft.Controls.Add(this.lblMobileSecondary, 0, 17);
            this.tableLayoutPanelLeft.Controls.Add(this.btnSalutation, 2, 3);
            this.tableLayoutPanelLeft.Controls.Add(this.lblMblPrimary, 0, 15);
            this.tableLayoutPanelLeft.Controls.Add(this.textBoxPhone, 1, 13);
            this.tableLayoutPanelLeft.Controls.Add(this.viewAddressUserControl1, 0, 21);
            this.tableLayoutPanelLeft.Controls.Add(this.txtMobileSecondary, 0, 18);
            this.tableLayoutPanelLeft.Controls.Add(this.label3, 0, 19);
            this.tableLayoutPanelLeft.Controls.Add(this.chkResidence, 1, 19);
            this.tableLayoutPanelLeft.Controls.Add(this.label7, 1, 8);
            this.tableLayoutPanelLeft.Controls.Add(this.label4, 1, 4);
            this.tableLayoutPanelLeft.Controls.Add(this.label8, 1, 15);
            this.tableLayoutPanelLeft.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableLayoutPanelLeft.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanelLeft.Name = "tableLayoutPanelLeft";
            this.tableLayoutPanelLeft.RowCount = 23;
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 19F));
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelLeft.Size = new System.Drawing.Size(457, 536);
            this.tableLayoutPanelLeft.TabIndex = 0;
            // 
            // label11
            // 
            this.label11.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.Red;
            this.label11.Location = new System.Drawing.Point(125, 50);
            this.label11.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(15, 15);
            this.label11.TabIndex = 53;
            this.label11.Text = "*";
            // 
            // txtSTD
            // 
            this.txtSTD.AllowDrop = true;
            this.txtSTD.EditValue = "";
            this.txtSTD.Location = new System.Drawing.Point(3, 321);
            this.txtSTD.Margin = new System.Windows.Forms.Padding(3, 5, 3, 6);
            this.txtSTD.Name = "txtSTD";
            this.txtSTD.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.txtSTD.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtSTD.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtSTD.Properties.Appearance.Options.UseBackColor = true;
            this.txtSTD.Properties.Appearance.Options.UseFont = true;
            this.txtSTD.Properties.Appearance.Options.UseForeColor = true;
            this.txtSTD.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtSTD.Properties.MaxLength = 8;
            this.txtSTD.Size = new System.Drawing.Size(116, 24);
            this.txtSTD.TabIndex = 10;
            this.txtSTD.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSTD_KeyPress);
            this.txtSTD.Leave += new System.EventHandler(this.txtSTD_Leave);
            // 
            // txtMobilePrimary
            // 
            this.txtMobilePrimary.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelLeft.SetColumnSpan(this.txtMobilePrimary, 2);
            this.txtMobilePrimary.EditValue = "";
            this.txtMobilePrimary.Location = new System.Drawing.Point(3, 373);
            this.txtMobilePrimary.Name = "txtMobilePrimary";
            this.txtMobilePrimary.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.txtMobilePrimary.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtMobilePrimary.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtMobilePrimary.Properties.Appearance.Options.UseBackColor = true;
            this.txtMobilePrimary.Properties.Appearance.Options.UseFont = true;
            this.txtMobilePrimary.Properties.Appearance.Options.UseForeColor = true;
            this.txtMobilePrimary.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtMobilePrimary.Properties.MaxLength = 13;
            this.txtMobilePrimary.Size = new System.Drawing.Size(384, 24);
            this.txtMobilePrimary.TabIndex = 6;
            this.txtMobilePrimary.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMobilePrimary_KeyPress);
            this.txtMobilePrimary.Leave += new System.EventHandler(this.txtMobilePrimary_Leave);
            // 
            // textBoxLastName
            // 
            this.textBoxLastName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelLeft.SetColumnSpan(this.textBoxLastName, 2);
            this.textBoxLastName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "LastName", true));
            this.textBoxLastName.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "AreSplitNameFieldsVisible", true));
            this.textBoxLastName.Location = new System.Drawing.Point(3, 221);
            this.textBoxLastName.Margin = new System.Windows.Forms.Padding(3, 5, 3, 6);
            this.textBoxLastName.Name = "textBoxLastName";
            this.textBoxLastName.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textBoxLastName.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.textBoxLastName.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textBoxLastName.Properties.Appearance.Options.UseBackColor = true;
            this.textBoxLastName.Properties.Appearance.Options.UseFont = true;
            this.textBoxLastName.Properties.Appearance.Options.UseForeColor = true;
            this.textBoxLastName.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textBoxLastName.Properties.MaxLength = 20;
            this.textBoxLastName.Size = new System.Drawing.Size(384, 24);
            this.textBoxLastName.TabIndex = 3;
            this.textBoxLastName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxLastName_KeyPress);
            this.textBoxLastName.Leave += new System.EventHandler(this.textBoxLastName_Leave);
            // 
            // labelLastName
            // 
            this.labelLastName.AutoSize = true;
            this.labelLastName.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "AreSplitNameFieldsVisible", true));
            this.labelLastName.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLastName.Location = new System.Drawing.Point(3, 196);
            this.labelLastName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
            this.labelLastName.Name = "labelLastName";
            this.labelLastName.Size = new System.Drawing.Size(63, 13);
            this.labelLastName.TabIndex = 8;
            this.labelLastName.Text = "Last name:";
            // 
            // textBoxMiddleName
            // 
            this.textBoxMiddleName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelLeft.SetColumnSpan(this.textBoxMiddleName, 2);
            this.textBoxMiddleName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "MiddleName", true));
            this.textBoxMiddleName.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "AreSplitNameFieldsVisible", true));
            this.textBoxMiddleName.Location = new System.Drawing.Point(3, 167);
            this.textBoxMiddleName.Name = "textBoxMiddleName";
            this.textBoxMiddleName.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textBoxMiddleName.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.textBoxMiddleName.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textBoxMiddleName.Properties.Appearance.Options.UseBackColor = true;
            this.textBoxMiddleName.Properties.Appearance.Options.UseFont = true;
            this.textBoxMiddleName.Properties.Appearance.Options.UseForeColor = true;
            this.textBoxMiddleName.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textBoxMiddleName.Properties.MaxLength = 20;
            this.textBoxMiddleName.Size = new System.Drawing.Size(384, 24);
            this.textBoxMiddleName.TabIndex = 2;
            this.textBoxMiddleName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxMiddleName_KeyPress);
            this.textBoxMiddleName.Leave += new System.EventHandler(this.textBoxMiddleName_Leave);
            // 
            // labelMiddleName
            // 
            this.labelMiddleName.AutoSize = true;
            this.tableLayoutPanelLeft.SetColumnSpan(this.labelMiddleName, 2);
            this.labelMiddleName.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "AreSplitNameFieldsVisible", true));
            this.labelMiddleName.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMiddleName.Location = new System.Drawing.Point(3, 151);
            this.labelMiddleName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
            this.labelMiddleName.Name = "labelMiddleName";
            this.labelMiddleName.Size = new System.Drawing.Size(79, 13);
            this.labelMiddleName.TabIndex = 6;
            this.labelMiddleName.Text = "Middle name:";
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.tableLayoutPanelLeft.SetColumnSpan(this.labelName, 2);
            this.labelName.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "IsNameVisible", true));
            this.labelName.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelName.Location = new System.Drawing.Point(3, 253);
            this.labelName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(41, 13);
            this.labelName.TabIndex = 8;
            this.labelName.Text = "Name:";
            // 
            // textBoxName
            // 
            this.textBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelLeft.SetColumnSpan(this.textBoxName, 2);
            this.textBoxName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "Name", true));
            this.textBoxName.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "IsNameVisible", true));
            this.textBoxName.Location = new System.Drawing.Point(3, 271);
            this.textBoxName.Margin = new System.Windows.Forms.Padding(3, 5, 3, 6);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textBoxName.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.textBoxName.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textBoxName.Properties.Appearance.Options.UseBackColor = true;
            this.textBoxName.Properties.Appearance.Options.UseFont = true;
            this.textBoxName.Properties.Appearance.Options.UseForeColor = true;
            this.textBoxName.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textBoxName.Properties.MaxLength = 100;
            this.textBoxName.Size = new System.Drawing.Size(384, 24);
            this.textBoxName.TabIndex = 4;
            // 
            // labelType
            // 
            this.labelType.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelType.AutoSize = true;
            this.labelType.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelType.Location = new System.Drawing.Point(3, 2);
            this.labelType.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
            this.labelType.Name = "labelType";
            this.labelType.Size = new System.Drawing.Size(35, 13);
            this.labelType.TabIndex = 0;
            this.labelType.Text = "Type:";
            // 
            // labelFirstName
            // 
            this.labelFirstName.AutoSize = true;
            this.labelFirstName.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "AreSplitNameFieldsVisible", true));
            this.labelFirstName.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFirstName.Location = new System.Drawing.Point(3, 97);
            this.labelFirstName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
            this.labelFirstName.Name = "labelFirstName";
            this.labelFirstName.Size = new System.Drawing.Size(64, 13);
            this.labelFirstName.TabIndex = 4;
            this.labelFirstName.Text = "First name:";
            // 
            // textBoxFirstName
            // 
            this.textBoxFirstName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelLeft.SetColumnSpan(this.textBoxFirstName, 2);
            this.textBoxFirstName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "FirstName", true));
            this.textBoxFirstName.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "AreSplitNameFieldsVisible", true));
            this.textBoxFirstName.Location = new System.Drawing.Point(3, 122);
            this.textBoxFirstName.Name = "textBoxFirstName";
            this.textBoxFirstName.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textBoxFirstName.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.textBoxFirstName.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textBoxFirstName.Properties.Appearance.Options.UseBackColor = true;
            this.textBoxFirstName.Properties.Appearance.Options.UseFont = true;
            this.textBoxFirstName.Properties.Appearance.Options.UseForeColor = true;
            this.textBoxFirstName.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textBoxFirstName.Properties.MaxLength = 20;
            this.textBoxFirstName.Size = new System.Drawing.Size(384, 24);
            this.textBoxFirstName.TabIndex = 1;
            this.textBoxFirstName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxFirstName_KeyPress);
            this.textBoxFirstName.Leave += new System.EventHandler(this.textBoxFirstName_Leave);
            // 
            // tableLayoutPanelType
            // 
            this.tableLayoutPanelType.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelType.AutoSize = true;
            this.tableLayoutPanelType.ColumnCount = 2;
            this.tableLayoutPanelLeft.SetColumnSpan(this.tableLayoutPanelType, 2);
            this.tableLayoutPanelType.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelType.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelType.Controls.Add(this.radioPerson, 0, 0);
            this.tableLayoutPanelType.Controls.Add(this.radioOrg, 1, 0);
            this.tableLayoutPanelType.Location = new System.Drawing.Point(3, 18);
            this.tableLayoutPanelType.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.tableLayoutPanelType.Name = "tableLayoutPanelType";
            this.tableLayoutPanelType.RowCount = 1;
            this.tableLayoutPanelType.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelType.Size = new System.Drawing.Size(384, 27);
            this.tableLayoutPanelType.TabIndex = 1;
            // 
            // radioPerson
            // 
            this.radioPerson.AutoSize = true;
            this.radioPerson.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.bindingSource, "IsPerson", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.radioPerson.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.radioPerson.Location = new System.Drawing.Point(3, 3);
            this.radioPerson.Name = "radioPerson";
            this.radioPerson.Size = new System.Drawing.Size(66, 21);
            this.radioPerson.TabIndex = 0;
            this.radioPerson.TabStop = true;
            this.radioPerson.Text = "Person";
            this.radioPerson.UseVisualStyleBackColor = true;
            // 
            // radioOrg
            // 
            this.radioOrg.AutoSize = true;
            this.radioOrg.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.bindingSource, "IsOrganization", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.radioOrg.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.radioOrg.Location = new System.Drawing.Point(75, 3);
            this.radioOrg.Name = "radioOrg";
            this.radioOrg.Size = new System.Drawing.Size(101, 21);
            this.radioOrg.TabIndex = 1;
            this.radioOrg.TabStop = true;
            this.radioOrg.Text = "Organization";
            this.radioOrg.UseVisualStyleBackColor = true;
            // 
            // lblSalutation
            // 
            this.lblSalutation.AutoSize = true;
            this.lblSalutation.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSalutation.Location = new System.Drawing.Point(3, 47);
            this.lblSalutation.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
            this.lblSalutation.Name = "lblSalutation";
            this.lblSalutation.Size = new System.Drawing.Size(63, 13);
            this.lblSalutation.TabIndex = 2;
            this.lblSalutation.Text = "Salutation:";
            // 
            // txtSalutation
            // 
            this.txtSalutation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelLeft.SetColumnSpan(this.txtSalutation, 2);
            this.txtSalutation.Enabled = false;
            this.txtSalutation.Location = new System.Drawing.Point(3, 68);
            this.txtSalutation.Name = "txtSalutation";
            this.txtSalutation.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.txtSalutation.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtSalutation.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtSalutation.Properties.Appearance.Options.UseBackColor = true;
            this.txtSalutation.Properties.Appearance.Options.UseFont = true;
            this.txtSalutation.Properties.Appearance.Options.UseForeColor = true;
            this.txtSalutation.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtSalutation.Properties.MaxLength = 20;
            this.txtSalutation.Size = new System.Drawing.Size(384, 24);
            this.txtSalutation.TabIndex = 3;
            // 
            // labelPhone
            // 
            this.labelPhone.AutoSize = true;
            this.tableLayoutPanelLeft.SetColumnSpan(this.labelPhone, 2);
            this.labelPhone.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPhone.Location = new System.Drawing.Point(3, 303);
            this.labelPhone.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
            this.labelPhone.Name = "labelPhone";
            this.labelPhone.Size = new System.Drawing.Size(88, 13);
            this.labelPhone.TabIndex = 9;
            this.labelPhone.Text = "Phone number:";
            // 
            // lblMobileSecondary
            // 
            this.lblMobileSecondary.AutoSize = true;
            this.tableLayoutPanelLeft.SetColumnSpan(this.lblMobileSecondary, 2);
            this.lblMobileSecondary.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMobileSecondary.Location = new System.Drawing.Point(3, 401);
            this.lblMobileSecondary.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
            this.lblMobileSecondary.Name = "lblMobileSecondary";
            this.lblMobileSecondary.Size = new System.Drawing.Size(112, 13);
            this.lblMobileSecondary.TabIndex = 14;
            this.lblMobileSecondary.Text = "Mobile (Secondary):";
            // 
            // btnSalutation
            // 
            this.btnSalutation.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSalutation.Image = global::Microsoft.Dynamics.Retail.Pos.Customer.Properties.Resources.search;
            this.btnSalutation.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSalutation.Location = new System.Drawing.Point(390, 65);
            this.btnSalutation.Margin = new System.Windows.Forms.Padding(0);
            this.btnSalutation.Name = "btnSalutation";
            this.btnSalutation.Padding = new System.Windows.Forms.Padding(3);
            this.btnSalutation.Size = new System.Drawing.Size(67, 30);
            this.btnSalutation.TabIndex = 0;
            this.btnSalutation.Click += new System.EventHandler(this.btnSalutation_Click);
            // 
            // lblMblPrimary
            // 
            this.lblMblPrimary.AutoSize = true;
            this.lblMblPrimary.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMblPrimary.Location = new System.Drawing.Point(3, 353);
            this.lblMblPrimary.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
            this.lblMblPrimary.Name = "lblMblPrimary";
            this.lblMblPrimary.Size = new System.Drawing.Size(98, 13);
            this.lblMblPrimary.TabIndex = 12;
            this.lblMblPrimary.Text = "Mobile (Primary):";
            // 
            // textBoxPhone
            // 
            this.textBoxPhone.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPhone.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "Phone", true));
            this.textBoxPhone.Location = new System.Drawing.Point(125, 321);
            this.textBoxPhone.Margin = new System.Windows.Forms.Padding(3, 5, 3, 6);
            this.textBoxPhone.Name = "textBoxPhone";
            this.textBoxPhone.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textBoxPhone.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.textBoxPhone.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textBoxPhone.Properties.Appearance.Options.UseBackColor = true;
            this.textBoxPhone.Properties.Appearance.Options.UseFont = true;
            this.textBoxPhone.Properties.Appearance.Options.UseForeColor = true;
            this.textBoxPhone.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textBoxPhone.Properties.MaxLength = 10;
            this.textBoxPhone.Size = new System.Drawing.Size(262, 24);
            this.textBoxPhone.TabIndex = 5;
            this.textBoxPhone.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxPhone_KeyPress);
            this.textBoxPhone.Leave += new System.EventHandler(this.textBoxPhone_Leave);
            // 
            // viewAddressUserControl1
            // 
            this.viewAddressUserControl1.AutoSize = true;
            this.tableLayoutPanelLeft.SetColumnSpan(this.viewAddressUserControl1, 2);
            this.viewAddressUserControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewAddressUserControl1.Location = new System.Drawing.Point(3, 474);
            this.viewAddressUserControl1.MinimumSize = new System.Drawing.Size(285, 60);
            this.viewAddressUserControl1.Name = "viewAddressUserControl1";
            this.viewAddressUserControl1.Size = new System.Drawing.Size(384, 60);
            this.viewAddressUserControl1.TabIndex = 8;
            // 
            // txtMobileSecondary
            // 
            this.txtMobileSecondary.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelLeft.SetColumnSpan(this.txtMobileSecondary, 2);
            this.txtMobileSecondary.EditValue = "";
            this.txtMobileSecondary.Location = new System.Drawing.Point(3, 422);
            this.txtMobileSecondary.Name = "txtMobileSecondary";
            this.txtMobileSecondary.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.txtMobileSecondary.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtMobileSecondary.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtMobileSecondary.Properties.Appearance.Options.UseBackColor = true;
            this.txtMobileSecondary.Properties.Appearance.Options.UseFont = true;
            this.txtMobileSecondary.Properties.Appearance.Options.UseForeColor = true;
            this.txtMobileSecondary.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtMobileSecondary.Properties.MaxLength = 13;
            this.txtMobileSecondary.Size = new System.Drawing.Size(384, 24);
            this.txtMobileSecondary.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(3, 451);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Resident:";
            // 
            // chkResidence
            // 
            this.chkResidence.AutoSize = true;
            this.chkResidence.Location = new System.Drawing.Point(125, 452);
            this.chkResidence.Name = "chkResidence";
            this.chkResidence.Size = new System.Drawing.Size(15, 14);
            this.chkResidence.TabIndex = 16;
            this.chkResidence.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label7.ForeColor = System.Drawing.Color.Red;
            this.label7.Location = new System.Drawing.Point(125, 199);
            this.label7.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(13, 17);
            this.label7.TabIndex = 48;
            this.label7.Text = "*";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(125, 100);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(15, 19);
            this.label4.TabIndex = 49;
            this.label4.Text = "*";
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label8.ForeColor = System.Drawing.Color.Red;
            this.label8.Location = new System.Drawing.Point(125, 356);
            this.label8.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(13, 14);
            this.label8.TabIndex = 52;
            this.label8.Text = "*";
            // 
            // tableLayoutPanelRight
            // 
            this.tableLayoutPanelRight.AllowDrop = true;
            this.tableLayoutPanelRight.ColumnCount = 4;
            this.tableLayoutPanelRight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 73.50993F));
            this.tableLayoutPanelRight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26.49007F));
            this.tableLayoutPanelRight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelRight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 156F));
            this.tableLayoutPanelRight.Controls.Add(this.label12, 1, 12);
            this.tableLayoutPanelRight.Controls.Add(this.label5, 1, 18);
            this.tableLayoutPanelRight.Controls.Add(this.txtGender, 0, 13);
            this.tableLayoutPanelRight.Controls.Add(this.textBoxLanguage, 0, 11);
            this.tableLayoutPanelRight.Controls.Add(this.labelLanguage, 0, 10);
            this.tableLayoutPanelRight.Controls.Add(this.labelGroup, 0, 6);
            this.tableLayoutPanelRight.Controls.Add(this.textBoxCurrency, 0, 9);
            this.tableLayoutPanelRight.Controls.Add(this.labelEmail, 0, 0);
            this.tableLayoutPanelRight.Controls.Add(this.labelCurrency, 0, 8);
            this.tableLayoutPanelRight.Controls.Add(this.textBoxEmail, 0, 1);
            this.tableLayoutPanelRight.Controls.Add(this.textBoxGroup, 0, 7);
            this.tableLayoutPanelRight.Controls.Add(this.lblDOB, 0, 14);
            this.tableLayoutPanelRight.Controls.Add(this.lblGender, 0, 12);
            this.tableLayoutPanelRight.Controls.Add(this.label1, 0, 20);
            this.tableLayoutPanelRight.Controls.Add(this.textCustAgeBracket, 0, 21);
            this.tableLayoutPanelRight.Controls.Add(this.label2, 0, 18);
            this.tableLayoutPanelRight.Controls.Add(this.textNationality, 0, 19);
            this.tableLayoutPanelRight.Controls.Add(this.lblMarriageDate, 0, 16);
            this.tableLayoutPanelRight.Controls.Add(this.btnGroup, 3, 7);
            this.tableLayoutPanelRight.Controls.Add(this.btnCurrency, 3, 9);
            this.tableLayoutPanelRight.Controls.Add(this.btnLanguage, 3, 11);
            this.tableLayoutPanelRight.Controls.Add(this.btnGender, 3, 13);
            this.tableLayoutPanelRight.Controls.Add(this.txtBDay, 1, 15);
            this.tableLayoutPanelRight.Controls.Add(this.txtBYear, 2, 15);
            this.tableLayoutPanelRight.Controls.Add(this.btnNationality, 3, 19);
            this.tableLayoutPanelRight.Controls.Add(this.btnAgeBracket, 3, 21);
            this.tableLayoutPanelRight.Controls.Add(this.txtAnnDay, 1, 17);
            this.tableLayoutPanelRight.Controls.Add(this.txtAnnYear, 2, 17);
            this.tableLayoutPanelRight.Controls.Add(this.cmbBMonth, 0, 15);
            this.tableLayoutPanelRight.Controls.Add(this.cmbAnnMonth, 0, 17);
            this.tableLayoutPanelRight.Controls.Add(this.lblReligion, 0, 4);
            this.tableLayoutPanelRight.Controls.Add(this.lblOccupation, 0, 2);
            this.tableLayoutPanelRight.Controls.Add(this.txtCustClassificationGroup, 0, 5);
            this.tableLayoutPanelRight.Controls.Add(this.txtOccupation, 0, 3);
            this.tableLayoutPanelRight.Controls.Add(this.btnCustClassificationGrp, 3, 5);
            this.tableLayoutPanelRight.Controls.Add(this.label9, 1, 6);
            this.tableLayoutPanelRight.Controls.Add(this.btnCity, 3, 3);
            this.tableLayoutPanelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelRight.Location = new System.Drawing.Point(466, 3);
            this.tableLayoutPanelRight.Name = "tableLayoutPanelRight";
            this.tableLayoutPanelRight.RowCount = 23;
            this.tableLayoutPanelRight.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRight.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRight.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRight.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRight.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRight.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRight.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRight.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRight.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRight.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRight.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRight.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 17F));
            this.tableLayoutPanelRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanelRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 17F));
            this.tableLayoutPanelRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanelRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 19F));
            this.tableLayoutPanelRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanelRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanelRight.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanelRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 11F));
            this.tableLayoutPanelRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelRight.Size = new System.Drawing.Size(458, 536);
            this.tableLayoutPanelRight.TabIndex = 4;
            // 
            // label12
            // 
            this.label12.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.Color.Red;
            this.label12.Location = new System.Drawing.Point(154, 282);
            this.label12.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(15, 12);
            this.label12.TabIndex = 53;
            this.label12.Text = "*";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(154, 428);
            this.label5.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(15, 15);
            this.label5.TabIndex = 50;
            this.label5.Text = "*";
            // 
            // txtGender
            // 
            this.txtGender.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelRight.SetColumnSpan(this.txtGender, 3);
            this.txtGender.Enabled = false;
            this.txtGender.Location = new System.Drawing.Point(3, 297);
            this.txtGender.Name = "txtGender";
            this.txtGender.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.txtGender.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtGender.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtGender.Properties.Appearance.Options.UseBackColor = true;
            this.txtGender.Properties.Appearance.Options.UseFont = true;
            this.txtGender.Properties.Appearance.Options.UseForeColor = true;
            this.txtGender.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtGender.Properties.MaxLength = 20;
            this.txtGender.Size = new System.Drawing.Size(295, 24);
            this.txtGender.TabIndex = 30;
            // 
            // textBoxLanguage
            // 
            this.textBoxLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelRight.SetColumnSpan(this.textBoxLanguage, 3);
            this.textBoxLanguage.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "Language.DisplayName", true));
            this.textBoxLanguage.Location = new System.Drawing.Point(3, 250);
            this.textBoxLanguage.Name = "textBoxLanguage";
            this.textBoxLanguage.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textBoxLanguage.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.textBoxLanguage.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textBoxLanguage.Properties.Appearance.Options.UseBackColor = true;
            this.textBoxLanguage.Properties.Appearance.Options.UseFont = true;
            this.textBoxLanguage.Properties.Appearance.Options.UseForeColor = true;
            this.textBoxLanguage.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textBoxLanguage.Properties.ReadOnly = true;
            this.textBoxLanguage.Size = new System.Drawing.Size(295, 24);
            this.textBoxLanguage.TabIndex = 28;
            // 
            // labelLanguage
            // 
            this.labelLanguage.AutoSize = true;
            this.tableLayoutPanelRight.SetColumnSpan(this.labelLanguage, 3);
            this.labelLanguage.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLanguage.Location = new System.Drawing.Point(3, 234);
            this.labelLanguage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
            this.labelLanguage.Name = "labelLanguage";
            this.labelLanguage.Size = new System.Drawing.Size(62, 13);
            this.labelLanguage.TabIndex = 27;
            this.labelLanguage.Text = "Language:";
            // 
            // labelGroup
            // 
            this.labelGroup.AutoSize = true;
            this.labelGroup.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelGroup.Location = new System.Drawing.Point(3, 137);
            this.labelGroup.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
            this.labelGroup.Name = "labelGroup";
            this.labelGroup.Size = new System.Drawing.Size(95, 13);
            this.labelGroup.TabIndex = 23;
            this.labelGroup.Text = "Customer group:";
            // 
            // textBoxCurrency
            // 
            this.textBoxCurrency.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelRight.SetColumnSpan(this.textBoxCurrency, 3);
            this.textBoxCurrency.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "Currency", true));
            this.textBoxCurrency.Location = new System.Drawing.Point(3, 205);
            this.textBoxCurrency.Name = "textBoxCurrency";
            this.textBoxCurrency.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textBoxCurrency.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.textBoxCurrency.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textBoxCurrency.Properties.Appearance.Options.UseBackColor = true;
            this.textBoxCurrency.Properties.Appearance.Options.UseFont = true;
            this.textBoxCurrency.Properties.Appearance.Options.UseForeColor = true;
            this.textBoxCurrency.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textBoxCurrency.Properties.ReadOnly = true;
            this.textBoxCurrency.Size = new System.Drawing.Size(295, 24);
            this.textBoxCurrency.TabIndex = 26;
            // 
            // labelEmail
            // 
            this.labelEmail.AutoSize = true;
            this.tableLayoutPanelRight.SetColumnSpan(this.labelEmail, 3);
            this.labelEmail.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEmail.Location = new System.Drawing.Point(3, 2);
            this.labelEmail.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
            this.labelEmail.Name = "labelEmail";
            this.labelEmail.Size = new System.Drawing.Size(42, 13);
            this.labelEmail.TabIndex = 17;
            this.labelEmail.Text = "E-mail:";
            // 
            // labelCurrency
            // 
            this.labelCurrency.AutoSize = true;
            this.tableLayoutPanelRight.SetColumnSpan(this.labelCurrency, 3);
            this.labelCurrency.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCurrency.Location = new System.Drawing.Point(3, 189);
            this.labelCurrency.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
            this.labelCurrency.Name = "labelCurrency";
            this.labelCurrency.Size = new System.Drawing.Size(60, 13);
            this.labelCurrency.TabIndex = 25;
            this.labelCurrency.Text = "Currrency:";
            // 
            // textBoxEmail
            // 
            this.textBoxEmail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelRight.SetColumnSpan(this.textBoxEmail, 3);
            this.textBoxEmail.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "Email", true));
            this.textBoxEmail.Location = new System.Drawing.Point(3, 18);
            this.textBoxEmail.Name = "textBoxEmail";
            this.textBoxEmail.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textBoxEmail.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.textBoxEmail.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textBoxEmail.Properties.Appearance.Options.UseBackColor = true;
            this.textBoxEmail.Properties.Appearance.Options.UseFont = true;
            this.textBoxEmail.Properties.Appearance.Options.UseForeColor = true;
            this.textBoxEmail.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textBoxEmail.Properties.MaxLength = 80;
            this.textBoxEmail.Size = new System.Drawing.Size(295, 24);
            this.textBoxEmail.TabIndex = 9;
            // 
            // textBoxGroup
            // 
            this.textBoxGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelRight.SetColumnSpan(this.textBoxGroup, 3);
            this.textBoxGroup.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "CustomerGroup", true));
            this.textBoxGroup.Location = new System.Drawing.Point(3, 160);
            this.textBoxGroup.Name = "textBoxGroup";
            this.textBoxGroup.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textBoxGroup.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.textBoxGroup.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textBoxGroup.Properties.Appearance.Options.UseBackColor = true;
            this.textBoxGroup.Properties.Appearance.Options.UseFont = true;
            this.textBoxGroup.Properties.Appearance.Options.UseForeColor = true;
            this.textBoxGroup.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textBoxGroup.Properties.ReadOnly = true;
            this.textBoxGroup.Size = new System.Drawing.Size(295, 24);
            this.textBoxGroup.TabIndex = 24;
            // 
            // lblDOB
            // 
            this.lblDOB.AutoSize = true;
            this.tableLayoutPanelRight.SetColumnSpan(this.lblDOB, 3);
            this.lblDOB.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDOB.Location = new System.Drawing.Point(3, 324);
            this.lblDOB.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
            this.lblDOB.Name = "lblDOB";
            this.lblDOB.Size = new System.Drawing.Size(62, 13);
            this.lblDOB.TabIndex = 31;
            this.lblDOB.Text = "Birth Date:";
            // 
            // lblGender
            // 
            this.lblGender.AutoSize = true;
            this.lblGender.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGender.Location = new System.Drawing.Point(3, 279);
            this.lblGender.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
            this.lblGender.Name = "lblGender";
            this.lblGender.Size = new System.Drawing.Size(48, 13);
            this.lblGender.TabIndex = 29;
            this.lblGender.Text = "Gender:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.tableLayoutPanelRight.SetColumnSpan(this.label1, 3);
            this.label1.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 473);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 43;
            this.label1.Text = "Age bracket:";
            // 
            // textCustAgeBracket
            // 
            this.textCustAgeBracket.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelRight.SetColumnSpan(this.textCustAgeBracket, 3);
            this.textCustAgeBracket.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "ReceiptEmail", true));
            this.textCustAgeBracket.Location = new System.Drawing.Point(3, 489);
            this.textCustAgeBracket.Name = "textCustAgeBracket";
            this.textCustAgeBracket.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textCustAgeBracket.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.textCustAgeBracket.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textCustAgeBracket.Properties.Appearance.Options.UseBackColor = true;
            this.textCustAgeBracket.Properties.Appearance.Options.UseFont = true;
            this.textCustAgeBracket.Properties.Appearance.Options.UseForeColor = true;
            this.textCustAgeBracket.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textCustAgeBracket.Properties.MaxLength = 80;
            this.textCustAgeBracket.Size = new System.Drawing.Size(295, 24);
            this.textCustAgeBracket.TabIndex = 39;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 425);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 44;
            this.label2.Text = "Nationality :";
            // 
            // textNationality
            // 
            this.textNationality.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelRight.SetColumnSpan(this.textNationality, 3);
            this.textNationality.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "NationalityId", true));
            this.textNationality.Location = new System.Drawing.Point(3, 446);
            this.textNationality.Name = "textNationality";
            this.textNationality.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textNationality.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.textNationality.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textNationality.Properties.Appearance.Options.UseBackColor = true;
            this.textNationality.Properties.Appearance.Options.UseFont = true;
            this.textNationality.Properties.Appearance.Options.UseForeColor = true;
            this.textNationality.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textNationality.Properties.MaxLength = 80;
            this.textNationality.Size = new System.Drawing.Size(295, 24);
            this.textNationality.TabIndex = 40;
            // 
            // lblMarriageDate
            // 
            this.lblMarriageDate.AutoSize = true;
            this.tableLayoutPanelRight.SetColumnSpan(this.lblMarriageDate, 3);
            this.lblMarriageDate.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMarriageDate.Location = new System.Drawing.Point(3, 373);
            this.lblMarriageDate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
            this.lblMarriageDate.Name = "lblMarriageDate";
            this.lblMarriageDate.Size = new System.Drawing.Size(84, 13);
            this.lblMarriageDate.TabIndex = 37;
            this.lblMarriageDate.Text = "Marriage Date:";
            // 
            // btnGroup
            // 
            this.btnGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGroup.Image = global::Microsoft.Dynamics.Retail.Pos.Customer.Properties.Resources.search;
            this.btnGroup.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnGroup.Location = new System.Drawing.Point(301, 157);
            this.btnGroup.Margin = new System.Windows.Forms.Padding(0);
            this.btnGroup.Name = "btnGroup";
            this.btnGroup.Padding = new System.Windows.Forms.Padding(3);
            this.btnGroup.Size = new System.Drawing.Size(157, 30);
            this.btnGroup.TabIndex = 12;
            this.btnGroup.Click += new System.EventHandler(this.OnCustomerGroup_Click);
            // 
            // btnCurrency
            // 
            this.btnCurrency.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCurrency.Image = global::Microsoft.Dynamics.Retail.Pos.Customer.Properties.Resources.search;
            this.btnCurrency.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnCurrency.Location = new System.Drawing.Point(301, 202);
            this.btnCurrency.Margin = new System.Windows.Forms.Padding(0);
            this.btnCurrency.Name = "btnCurrency";
            this.btnCurrency.Padding = new System.Windows.Forms.Padding(3);
            this.btnCurrency.Size = new System.Drawing.Size(157, 30);
            this.btnCurrency.TabIndex = 13;
            this.btnCurrency.Click += new System.EventHandler(this.OnCurrency_Click);
            // 
            // btnLanguage
            // 
            this.btnLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLanguage.Image = global::Microsoft.Dynamics.Retail.Pos.Customer.Properties.Resources.search;
            this.btnLanguage.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnLanguage.Location = new System.Drawing.Point(301, 247);
            this.btnLanguage.Margin = new System.Windows.Forms.Padding(0);
            this.btnLanguage.Name = "btnLanguage";
            this.btnLanguage.Padding = new System.Windows.Forms.Padding(3);
            this.btnLanguage.Size = new System.Drawing.Size(157, 30);
            this.btnLanguage.TabIndex = 14;
            this.btnLanguage.Click += new System.EventHandler(this.OnLanguage_Click);
            // 
            // btnGender
            // 
            this.btnGender.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGender.Image = global::Microsoft.Dynamics.Retail.Pos.Customer.Properties.Resources.search;
            this.btnGender.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnGender.Location = new System.Drawing.Point(301, 294);
            this.btnGender.Margin = new System.Windows.Forms.Padding(0);
            this.btnGender.Name = "btnGender";
            this.btnGender.Padding = new System.Windows.Forms.Padding(3);
            this.btnGender.Size = new System.Drawing.Size(157, 28);
            this.btnGender.TabIndex = 15;
            this.btnGender.Click += new System.EventHandler(this.btnGender_Click);
            // 
            // txtBDay
            // 
            this.txtBDay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBDay.Location = new System.Drawing.Point(154, 342);
            this.txtBDay.Name = "txtBDay";
            this.txtBDay.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.txtBDay.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtBDay.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtBDay.Properties.Appearance.Options.UseBackColor = true;
            this.txtBDay.Properties.Appearance.Options.UseFont = true;
            this.txtBDay.Properties.Appearance.Options.UseForeColor = true;
            this.txtBDay.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtBDay.Properties.EditFormat.FormatString = "##";
            this.txtBDay.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txtBDay.Properties.MaxLength = 2;
            this.txtBDay.Size = new System.Drawing.Size(48, 24);
            this.txtBDay.TabIndex = 17;
            this.txtBDay.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBDay_KeyPress);
            // 
            // txtBYear
            // 
            this.txtBYear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBYear.Location = new System.Drawing.Point(208, 342);
            this.txtBYear.Name = "txtBYear";
            this.txtBYear.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.txtBYear.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtBYear.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtBYear.Properties.Appearance.Options.UseBackColor = true;
            this.txtBYear.Properties.Appearance.Options.UseFont = true;
            this.txtBYear.Properties.Appearance.Options.UseForeColor = true;
            this.txtBYear.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtBYear.Properties.EditFormat.FormatString = "####";
            this.txtBYear.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txtBYear.Properties.MaxLength = 4;
            this.txtBYear.Size = new System.Drawing.Size(90, 24);
            this.txtBYear.TabIndex = 18;
            this.txtBYear.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBYear_KeyPress);
            // 
            // btnNationality
            // 
            this.btnNationality.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNationality.Image = global::Microsoft.Dynamics.Retail.Pos.Customer.Properties.Resources.search;
            this.btnNationality.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnNationality.Location = new System.Drawing.Point(301, 443);
            this.btnNationality.Margin = new System.Windows.Forms.Padding(0);
            this.btnNationality.Name = "btnNationality";
            this.btnNationality.Padding = new System.Windows.Forms.Padding(3);
            this.btnNationality.Size = new System.Drawing.Size(157, 28);
            this.btnNationality.TabIndex = 22;
            this.btnNationality.Click += new System.EventHandler(this.btnNationality_Click);
            // 
            // btnAgeBracket
            // 
            this.btnAgeBracket.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAgeBracket.Image = global::Microsoft.Dynamics.Retail.Pos.Customer.Properties.Resources.search;
            this.btnAgeBracket.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnAgeBracket.Location = new System.Drawing.Point(301, 486);
            this.btnAgeBracket.Margin = new System.Windows.Forms.Padding(0);
            this.btnAgeBracket.Name = "btnAgeBracket";
            this.btnAgeBracket.Padding = new System.Windows.Forms.Padding(3);
            this.btnAgeBracket.Size = new System.Drawing.Size(157, 28);
            this.btnAgeBracket.TabIndex = 23;
            this.btnAgeBracket.Click += new System.EventHandler(this.btnAgeBracket_Click);
            // 
            // txtAnnDay
            // 
            this.txtAnnDay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAnnDay.Location = new System.Drawing.Point(154, 393);
            this.txtAnnDay.Name = "txtAnnDay";
            this.txtAnnDay.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.txtAnnDay.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtAnnDay.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtAnnDay.Properties.Appearance.Options.UseBackColor = true;
            this.txtAnnDay.Properties.Appearance.Options.UseFont = true;
            this.txtAnnDay.Properties.Appearance.Options.UseForeColor = true;
            this.txtAnnDay.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtAnnDay.Properties.EditFormat.FormatString = "##";
            this.txtAnnDay.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txtAnnDay.Properties.MaxLength = 2;
            this.txtAnnDay.Size = new System.Drawing.Size(48, 24);
            this.txtAnnDay.TabIndex = 20;
            this.txtAnnDay.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtAnnDay_KeyPress);
            // 
            // txtAnnYear
            // 
            this.txtAnnYear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAnnYear.Location = new System.Drawing.Point(208, 393);
            this.txtAnnYear.Name = "txtAnnYear";
            this.txtAnnYear.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.txtAnnYear.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtAnnYear.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtAnnYear.Properties.Appearance.Options.UseBackColor = true;
            this.txtAnnYear.Properties.Appearance.Options.UseFont = true;
            this.txtAnnYear.Properties.Appearance.Options.UseForeColor = true;
            this.txtAnnYear.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtAnnYear.Properties.EditFormat.FormatString = "####";
            this.txtAnnYear.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txtAnnYear.Properties.MaxLength = 4;
            this.txtAnnYear.Size = new System.Drawing.Size(90, 24);
            this.txtAnnYear.TabIndex = 21;
            this.txtAnnYear.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtAnnYear_KeyPress);
            // 
            // cmbBMonth
            // 
            this.cmbBMonth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBMonth.FormattingEnabled = true;
            this.cmbBMonth.IntegralHeight = false;
            this.cmbBMonth.Location = new System.Drawing.Point(3, 342);
            this.cmbBMonth.Name = "cmbBMonth";
            this.cmbBMonth.Size = new System.Drawing.Size(145, 29);
            this.cmbBMonth.TabIndex = 16;
            // 
            // cmbAnnMonth
            // 
            this.cmbAnnMonth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAnnMonth.FormattingEnabled = true;
            this.cmbAnnMonth.Location = new System.Drawing.Point(3, 393);
            this.cmbAnnMonth.Name = "cmbAnnMonth";
            this.cmbAnnMonth.Size = new System.Drawing.Size(145, 29);
            this.cmbAnnMonth.TabIndex = 19;
            // 
            // lblReligion
            // 
            this.lblReligion.AutoSize = true;
            this.tableLayoutPanelRight.SetColumnSpan(this.lblReligion, 2);
            this.lblReligion.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReligion.Location = new System.Drawing.Point(3, 92);
            this.lblReligion.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
            this.lblReligion.Name = "lblReligion";
            this.lblReligion.Size = new System.Drawing.Size(164, 13);
            this.lblReligion.TabIndex = 33;
            this.lblReligion.Text = "Customer classification group:";
            // 
            // lblOccupation
            // 
            this.lblOccupation.AutoSize = true;
            this.tableLayoutPanelRight.SetColumnSpan(this.lblOccupation, 3);
            this.lblOccupation.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOccupation.Location = new System.Drawing.Point(3, 47);
            this.lblOccupation.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
            this.lblOccupation.Name = "lblOccupation";
            this.lblOccupation.Size = new System.Drawing.Size(30, 13);
            this.lblOccupation.TabIndex = 35;
            this.lblOccupation.Text = "City:";
            // 
            // txtCustClassificationGroup
            // 
            this.txtCustClassificationGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelRight.SetColumnSpan(this.txtCustClassificationGroup, 3);
            this.txtCustClassificationGroup.Location = new System.Drawing.Point(3, 108);
            this.txtCustClassificationGroup.Name = "txtCustClassificationGroup";
            this.txtCustClassificationGroup.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.txtCustClassificationGroup.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtCustClassificationGroup.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtCustClassificationGroup.Properties.Appearance.Options.UseBackColor = true;
            this.txtCustClassificationGroup.Properties.Appearance.Options.UseFont = true;
            this.txtCustClassificationGroup.Properties.Appearance.Options.UseForeColor = true;
            this.txtCustClassificationGroup.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtCustClassificationGroup.Properties.MaxLength = 20;
            this.txtCustClassificationGroup.Size = new System.Drawing.Size(295, 24);
            this.txtCustClassificationGroup.TabIndex = 10;
            // 
            // txtOccupation
            // 
            this.txtOccupation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelRight.SetColumnSpan(this.txtOccupation, 3);
            this.txtOccupation.Location = new System.Drawing.Point(3, 63);
            this.txtOccupation.Name = "txtOccupation";
            this.txtOccupation.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.txtOccupation.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtOccupation.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtOccupation.Properties.Appearance.Options.UseBackColor = true;
            this.txtOccupation.Properties.Appearance.Options.UseFont = true;
            this.txtOccupation.Properties.Appearance.Options.UseForeColor = true;
            this.txtOccupation.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtOccupation.Properties.MaxLength = 255;
            this.txtOccupation.Size = new System.Drawing.Size(295, 24);
            this.txtOccupation.TabIndex = 11;
            // 
            // btnCustClassificationGrp
            // 
            this.btnCustClassificationGrp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCustClassificationGrp.Image = global::Microsoft.Dynamics.Retail.Pos.Customer.Properties.Resources.search;
            this.btnCustClassificationGrp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnCustClassificationGrp.Location = new System.Drawing.Point(301, 105);
            this.btnCustClassificationGrp.Margin = new System.Windows.Forms.Padding(0);
            this.btnCustClassificationGrp.Name = "btnCustClassificationGrp";
            this.btnCustClassificationGrp.Padding = new System.Windows.Forms.Padding(3);
            this.btnCustClassificationGrp.Size = new System.Drawing.Size(157, 30);
            this.btnCustClassificationGrp.TabIndex = 45;
            this.btnCustClassificationGrp.Click += new System.EventHandler(this.btnCustClassificationGrp_Click);
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label9.ForeColor = System.Drawing.Color.Red;
            this.label9.Location = new System.Drawing.Point(154, 140);
            this.label9.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(13, 17);
            this.label9.TabIndex = 52;
            this.label9.Text = "*";
            // 
            // textBoxReceiptEmail
            // 
            this.textBoxReceiptEmail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxReceiptEmail.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "ReceiptEmail", true));
            this.textBoxReceiptEmail.Location = new System.Drawing.Point(583, -3);
            this.textBoxReceiptEmail.Name = "textBoxReceiptEmail";
            this.textBoxReceiptEmail.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textBoxReceiptEmail.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.textBoxReceiptEmail.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textBoxReceiptEmail.Properties.Appearance.Options.UseBackColor = true;
            this.textBoxReceiptEmail.Properties.Appearance.Options.UseFont = true;
            this.textBoxReceiptEmail.Properties.Appearance.Options.UseForeColor = true;
            this.textBoxReceiptEmail.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textBoxReceiptEmail.Properties.MaxLength = 80;
            this.textBoxReceiptEmail.Size = new System.Drawing.Size(295, 24);
            this.textBoxReceiptEmail.TabIndex = 20;
            this.textBoxReceiptEmail.Visible = false;
            // 
            // textBoxWebSite
            // 
            this.textBoxWebSite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxWebSite.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "WebSite", true));
            this.textBoxWebSite.Location = new System.Drawing.Point(428, -14);
            this.textBoxWebSite.Name = "textBoxWebSite";
            this.textBoxWebSite.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textBoxWebSite.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.textBoxWebSite.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textBoxWebSite.Properties.Appearance.Options.UseBackColor = true;
            this.textBoxWebSite.Properties.Appearance.Options.UseFont = true;
            this.textBoxWebSite.Properties.Appearance.Options.UseForeColor = true;
            this.textBoxWebSite.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textBoxWebSite.Properties.MaxLength = 255;
            this.textBoxWebSite.Size = new System.Drawing.Size(308, 24);
            this.textBoxWebSite.TabIndex = 100;
            this.textBoxWebSite.Visible = false;
            // 
            // labelReceiptEmail
            // 
            this.labelReceiptEmail.AutoSize = true;
            this.labelReceiptEmail.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelReceiptEmail.Location = new System.Drawing.Point(384, -7);
            this.labelReceiptEmail.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
            this.labelReceiptEmail.Name = "labelReceiptEmail";
            this.labelReceiptEmail.Size = new System.Drawing.Size(100, 17);
            this.labelReceiptEmail.TabIndex = 19;
            this.labelReceiptEmail.Text = "Receipt e-mail:";
            this.labelReceiptEmail.Visible = false;
            // 
            // tabPageHistory
            // 
            this.tabPageHistory.Controls.Add(this.panelHistoryTab);
            this.tabPageHistory.Name = "tabPageHistory";
            this.tabPageHistory.Padding = new System.Windows.Forms.Padding(10);
            this.tabPageHistory.Size = new System.Drawing.Size(947, 562);
            this.tabPageHistory.Text = "History";
            // 
            // panelHistoryTab
            // 
            this.panelHistoryTab.ColumnCount = 8;
            this.panelHistoryTab.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 155F));
            this.panelHistoryTab.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.panelHistoryTab.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.panelHistoryTab.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.panelHistoryTab.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.panelHistoryTab.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.panelHistoryTab.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.panelHistoryTab.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.panelHistoryTab.Controls.Add(this.labelDateCreated, 0, 0);
            this.panelHistoryTab.Controls.Add(this.labelTotalVisits, 0, 2);
            this.panelHistoryTab.Controls.Add(this.gridOrders, 2, 2);
            this.panelHistoryTab.Controls.Add(this.textDateCreated, 0, 1);
            this.panelHistoryTab.Controls.Add(this.textTotalVisits, 0, 3);
            this.panelHistoryTab.Controls.Add(this.btnPgDown, 7, 10);
            this.panelHistoryTab.Controls.Add(this.btnDown, 6, 10);
            this.panelHistoryTab.Controls.Add(this.labelDateLastVisit, 0, 4);
            this.panelHistoryTab.Controls.Add(this.textDateLastVisit, 0, 5);
            this.panelHistoryTab.Controls.Add(this.labelStoreLastVisited, 0, 6);
            this.panelHistoryTab.Controls.Add(this.textStoreLastVisited, 0, 7);
            this.panelHistoryTab.Controls.Add(this.labelTotalSales, 0, 8);
            this.panelHistoryTab.Controls.Add(this.textTotalSales, 0, 9);
            this.panelHistoryTab.Controls.Add(this.labelSearch, 2, 0);
            this.panelHistoryTab.Controls.Add(this.textSearch, 2, 1);
            this.panelHistoryTab.Controls.Add(this.buttonSearch, 6, 1);
            this.panelHistoryTab.Controls.Add(this.buttonClear, 7, 1);
            this.panelHistoryTab.Controls.Add(this.btnUp, 3, 10);
            this.panelHistoryTab.Controls.Add(this.btnPgUp, 2, 10);
            this.panelHistoryTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelHistoryTab.Location = new System.Drawing.Point(10, 10);
            this.panelHistoryTab.Name = "panelHistoryTab";
            this.panelHistoryTab.RowCount = 11;
            this.panelHistoryTab.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panelHistoryTab.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panelHistoryTab.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panelHistoryTab.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panelHistoryTab.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panelHistoryTab.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panelHistoryTab.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.panelHistoryTab.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panelHistoryTab.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panelHistoryTab.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panelHistoryTab.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panelHistoryTab.Size = new System.Drawing.Size(927, 542);
            this.panelHistoryTab.TabIndex = 0;
            // 
            // labelDateCreated
            // 
            this.labelDateCreated.AutoSize = true;
            this.panelHistoryTab.SetColumnSpan(this.labelDateCreated, 2);
            this.labelDateCreated.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDateCreated.Location = new System.Drawing.Point(3, 2);
            this.labelDateCreated.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
            this.labelDateCreated.Name = "labelDateCreated";
            this.labelDateCreated.Size = new System.Drawing.Size(90, 17);
            this.labelDateCreated.TabIndex = 0;
            this.labelDateCreated.Text = "Date created:";
            // 
            // labelTotalVisits
            // 
            this.labelTotalVisits.AutoSize = true;
            this.panelHistoryTab.SetColumnSpan(this.labelTotalVisits, 2);
            this.labelTotalVisits.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTotalVisits.Location = new System.Drawing.Point(3, 61);
            this.labelTotalVisits.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
            this.labelTotalVisits.Name = "labelTotalVisits";
            this.labelTotalVisits.Size = new System.Drawing.Size(79, 17);
            this.labelTotalVisits.TabIndex = 2;
            this.labelTotalVisits.Text = "Total visits:";
            // 
            // gridOrders
            // 
            this.panelHistoryTab.SetColumnSpan(this.gridOrders, 6);
            this.gridOrders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridOrders.Location = new System.Drawing.Point(197, 62);
            this.gridOrders.MainView = this.gridView;
            this.gridOrders.Name = "gridOrders";
            this.panelHistoryTab.SetRowSpan(this.gridOrders, 8);
            this.gridOrders.Size = new System.Drawing.Size(727, 408);
            this.gridOrders.TabIndex = 14;
            this.gridOrders.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView});
            // 
            // gridView
            // 
            this.gridView.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridView.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.columnDate,
            this.columnOrderNumber,
            this.columnOrderStatus,
            this.columnStore,
            this.columnItem,
            this.columnQuantity,
            this.columnAmount});
            this.gridView.GridControl = this.gridOrders;
            this.gridView.Name = "gridView";
            this.gridView.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            this.gridView.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.False;
            this.gridView.OptionsBehavior.Editable = false;
            this.gridView.OptionsBehavior.ReadOnly = true;
            this.gridView.OptionsCustomization.AllowColumnMoving = false;
            this.gridView.OptionsCustomization.AllowGroup = false;
            this.gridView.OptionsCustomization.AllowQuickHideColumns = false;
            this.gridView.OptionsCustomization.AllowRowSizing = true;
            this.gridView.OptionsMenu.EnableColumnMenu = false;
            this.gridView.OptionsMenu.EnableFooterMenu = false;
            this.gridView.OptionsMenu.EnableGroupPanelMenu = false;
            this.gridView.OptionsNavigation.UseTabKey = false;
            this.gridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
            this.gridView.OptionsView.ShowGroupPanel = false;
            this.gridView.OptionsView.ShowIndicator = false;
            // 
            // columnDate
            // 
            this.columnDate.Caption = "Date";
            this.columnDate.DisplayFormat.FormatString = "d";
            this.columnDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.columnDate.FieldName = "OrderDate";
            this.columnDate.Name = "columnDate";
            this.columnDate.Visible = true;
            this.columnDate.VisibleIndex = 0;
            // 
            // columnOrderNumber
            // 
            this.columnOrderNumber.Caption = "Order number";
            this.columnOrderNumber.FieldName = "OrderNumber";
            this.columnOrderNumber.Name = "columnOrderNumber";
            this.columnOrderNumber.Visible = true;
            this.columnOrderNumber.VisibleIndex = 1;
            // 
            // columnOrderStatus
            // 
            this.columnOrderStatus.AppearanceCell.Options.UseTextOptions = true;
            this.columnOrderStatus.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.columnOrderStatus.Caption = "Order status";
            this.columnOrderStatus.FieldName = "OrderStatus";
            this.columnOrderStatus.Name = "columnOrderStatus";
            this.columnOrderStatus.Visible = true;
            this.columnOrderStatus.VisibleIndex = 2;
            // 
            // columnStore
            // 
            this.columnStore.Caption = "Store";
            this.columnStore.FieldName = "StoreName";
            this.columnStore.Name = "columnStore";
            this.columnStore.Visible = true;
            this.columnStore.VisibleIndex = 3;
            // 
            // columnItem
            // 
            this.columnItem.Caption = "Item";
            this.columnItem.FieldName = "ItemName";
            this.columnItem.Name = "columnItem";
            this.columnItem.Visible = true;
            this.columnItem.VisibleIndex = 4;
            // 
            // columnQuantity
            // 
            this.columnQuantity.Caption = "Quantity";
            this.columnQuantity.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.columnQuantity.FieldName = "ItemQuantity";
            this.columnQuantity.Name = "columnQuantity";
            this.columnQuantity.Visible = true;
            this.columnQuantity.VisibleIndex = 5;
            // 
            // columnAmount
            // 
            this.columnAmount.Caption = "Amount";
            this.columnAmount.DisplayFormat.FormatString = "C";
            this.columnAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.columnAmount.FieldName = "ItemAmount";
            this.columnAmount.Name = "columnAmount";
            this.columnAmount.Visible = true;
            this.columnAmount.VisibleIndex = 6;
            // 
            // textDateCreated
            // 
            this.textDateCreated.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.textDateCreated.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "DateCreated", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "d"));
            this.textDateCreated.Location = new System.Drawing.Point(3, 30);
            this.textDateCreated.Name = "textDateCreated";
            this.textDateCreated.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textDateCreated.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.textDateCreated.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textDateCreated.Properties.Appearance.Options.UseBackColor = true;
            this.textDateCreated.Properties.Appearance.Options.UseFont = true;
            this.textDateCreated.Properties.Appearance.Options.UseForeColor = true;
            this.textDateCreated.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.textDateCreated.Properties.MaxLength = 20;
            this.textDateCreated.Properties.ReadOnly = true;
            this.textDateCreated.Size = new System.Drawing.Size(149, 22);
            this.textDateCreated.TabIndex = 1;
            // 
            // textTotalVisits
            // 
            this.textTotalVisits.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textTotalVisits.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "TotalVisitsCount", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
            this.textTotalVisits.Location = new System.Drawing.Point(3, 81);
            this.textTotalVisits.Name = "textTotalVisits";
            this.textTotalVisits.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textTotalVisits.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.textTotalVisits.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textTotalVisits.Properties.Appearance.Options.UseBackColor = true;
            this.textTotalVisits.Properties.Appearance.Options.UseFont = true;
            this.textTotalVisits.Properties.Appearance.Options.UseForeColor = true;
            this.textTotalVisits.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.textTotalVisits.Properties.MaxLength = 20;
            this.textTotalVisits.Properties.ReadOnly = true;
            this.textTotalVisits.Size = new System.Drawing.Size(149, 22);
            this.textTotalVisits.TabIndex = 3;
            // 
            // btnPgDown
            // 
            this.btnPgDown.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnPgDown.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPgDown.Appearance.Options.UseFont = true;
            this.btnPgDown.Image = global::Microsoft.Dynamics.Retail.Pos.Customer.Properties.Resources.bottom;
            this.btnPgDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPgDown.Location = new System.Drawing.Point(858, 477);
            this.btnPgDown.Margin = new System.Windows.Forms.Padding(4);
            this.btnPgDown.Name = "btnPgDown";
            this.btnPgDown.Size = new System.Drawing.Size(65, 61);
            this.btnPgDown.TabIndex = 18;
            this.btnPgDown.Tag = "";
            this.btnPgDown.Text = "Ê";
            this.btnPgDown.Click += new System.EventHandler(this.btnPgDown_Click);
            // 
            // btnDown
            // 
            this.btnDown.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnDown.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnDown.Appearance.Options.UseFont = true;
            this.btnDown.Image = global::Microsoft.Dynamics.Retail.Pos.Customer.Properties.Resources.down;
            this.btnDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnDown.Location = new System.Drawing.Point(784, 477);
            this.btnDown.Margin = new System.Windows.Forms.Padding(4);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(65, 61);
            this.btnDown.TabIndex = 17;
            this.btnDown.Tag = "";
            this.btnDown.Text = "ò";
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // labelDateLastVisit
            // 
            this.labelDateLastVisit.AutoSize = true;
            this.panelHistoryTab.SetColumnSpan(this.labelDateLastVisit, 2);
            this.labelDateLastVisit.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDateLastVisit.Location = new System.Drawing.Point(3, 108);
            this.labelDateLastVisit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
            this.labelDateLastVisit.Name = "labelDateLastVisit";
            this.labelDateLastVisit.Size = new System.Drawing.Size(114, 17);
            this.labelDateLastVisit.TabIndex = 4;
            this.labelDateLastVisit.Text = "Date of last visit:";
            // 
            // textDateLastVisit
            // 
            this.textDateLastVisit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textDateLastVisit.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "LastVisitedDate", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "d"));
            this.textDateLastVisit.Location = new System.Drawing.Point(3, 128);
            this.textDateLastVisit.Name = "textDateLastVisit";
            this.textDateLastVisit.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textDateLastVisit.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.textDateLastVisit.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textDateLastVisit.Properties.Appearance.Options.UseBackColor = true;
            this.textDateLastVisit.Properties.Appearance.Options.UseFont = true;
            this.textDateLastVisit.Properties.Appearance.Options.UseForeColor = true;
            this.textDateLastVisit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.textDateLastVisit.Properties.MaxLength = 20;
            this.textDateLastVisit.Properties.ReadOnly = true;
            this.textDateLastVisit.Size = new System.Drawing.Size(149, 22);
            this.textDateLastVisit.TabIndex = 5;
            // 
            // labelStoreLastVisited
            // 
            this.labelStoreLastVisited.AutoSize = true;
            this.panelHistoryTab.SetColumnSpan(this.labelStoreLastVisited, 2);
            this.labelStoreLastVisited.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStoreLastVisited.Location = new System.Drawing.Point(3, 155);
            this.labelStoreLastVisited.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
            this.labelStoreLastVisited.Name = "labelStoreLastVisited";
            this.labelStoreLastVisited.Size = new System.Drawing.Size(115, 17);
            this.labelStoreLastVisited.TabIndex = 6;
            this.labelStoreLastVisited.Text = "Store last visited:";
            // 
            // textStoreLastVisited
            // 
            this.textStoreLastVisited.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textStoreLastVisited.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "LastVisitedStore", true));
            this.textStoreLastVisited.Location = new System.Drawing.Point(3, 176);
            this.textStoreLastVisited.Name = "textStoreLastVisited";
            this.textStoreLastVisited.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textStoreLastVisited.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.textStoreLastVisited.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textStoreLastVisited.Properties.Appearance.Options.UseBackColor = true;
            this.textStoreLastVisited.Properties.Appearance.Options.UseFont = true;
            this.textStoreLastVisited.Properties.Appearance.Options.UseForeColor = true;
            this.textStoreLastVisited.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.textStoreLastVisited.Properties.MaxLength = 20;
            this.textStoreLastVisited.Properties.ReadOnly = true;
            this.textStoreLastVisited.Size = new System.Drawing.Size(149, 22);
            this.textStoreLastVisited.TabIndex = 7;
            // 
            // labelTotalSales
            // 
            this.labelTotalSales.AutoSize = true;
            this.panelHistoryTab.SetColumnSpan(this.labelTotalSales, 2);
            this.labelTotalSales.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTotalSales.Location = new System.Drawing.Point(3, 203);
            this.labelTotalSales.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
            this.labelTotalSales.Name = "labelTotalSales";
            this.labelTotalSales.Size = new System.Drawing.Size(77, 17);
            this.labelTotalSales.TabIndex = 8;
            this.labelTotalSales.Text = "Total sales:";
            // 
            // textTotalSales
            // 
            this.textTotalSales.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textTotalSales.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "TotalSalesAmount", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "C2"));
            this.textTotalSales.Location = new System.Drawing.Point(3, 223);
            this.textTotalSales.Name = "textTotalSales";
            this.textTotalSales.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textTotalSales.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.textTotalSales.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textTotalSales.Properties.Appearance.Options.UseBackColor = true;
            this.textTotalSales.Properties.Appearance.Options.UseFont = true;
            this.textTotalSales.Properties.Appearance.Options.UseForeColor = true;
            this.textTotalSales.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.textTotalSales.Properties.MaxLength = 20;
            this.textTotalSales.Properties.ReadOnly = true;
            this.textTotalSales.Size = new System.Drawing.Size(149, 22);
            this.textTotalSales.TabIndex = 9;
            // 
            // labelSearch
            // 
            this.labelSearch.AutoSize = true;
            this.panelHistoryTab.SetColumnSpan(this.labelSearch, 2);
            this.labelSearch.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSearch.Location = new System.Drawing.Point(197, 3);
            this.labelSearch.Margin = new System.Windows.Forms.Padding(3);
            this.labelSearch.Name = "labelSearch";
            this.labelSearch.Size = new System.Drawing.Size(82, 17);
            this.labelSearch.TabIndex = 10;
            this.labelSearch.Text = "Sales orders";
            // 
            // textSearch
            // 
            this.textSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.panelHistoryTab.SetColumnSpan(this.textSearch, 4);
            this.textSearch.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "FirstName", true));
            this.textSearch.Location = new System.Drawing.Point(197, 29);
            this.textSearch.Name = "textSearch";
            this.textSearch.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textSearch.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.textSearch.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textSearch.Properties.Appearance.Options.UseBackColor = true;
            this.textSearch.Properties.Appearance.Options.UseFont = true;
            this.textSearch.Properties.Appearance.Options.UseForeColor = true;
            this.textSearch.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textSearch.Properties.MaxLength = 20;
            this.textSearch.Size = new System.Drawing.Size(580, 24);
            this.textSearch.TabIndex = 11;
            this.textSearch.Enter += new System.EventHandler(this.textSearch_Enter);
            this.textSearch.Leave += new System.EventHandler(this.textSearch_Leave);
            // 
            // buttonSearch
            // 
            this.buttonSearch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonSearch.Image = global::Microsoft.Dynamics.Retail.Pos.Customer.Properties.Resources.search;
            this.buttonSearch.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.buttonSearch.Location = new System.Drawing.Point(788, 26);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Padding = new System.Windows.Forms.Padding(3);
            this.buttonSearch.Size = new System.Drawing.Size(57, 30);
            this.buttonSearch.TabIndex = 12;
            this.buttonSearch.Text = "Search";
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // buttonClear
            // 
            this.buttonClear.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonClear.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F);
            this.buttonClear.Appearance.Options.UseFont = true;
            this.buttonClear.Image = global::Microsoft.Dynamics.Retail.Pos.Customer.Properties.Resources.remove;
            this.buttonClear.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.buttonClear.Location = new System.Drawing.Point(856, 26);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Padding = new System.Windows.Forms.Padding(3);
            this.buttonClear.Size = new System.Drawing.Size(57, 30);
            this.buttonClear.TabIndex = 13;
            this.buttonClear.Text = "û";
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // btnUp
            // 
            this.btnUp.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnUp.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnUp.Appearance.Options.UseFont = true;
            this.btnUp.Image = global::Microsoft.Dynamics.Retail.Pos.Customer.Properties.Resources.up;
            this.btnUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnUp.Location = new System.Drawing.Point(271, 477);
            this.btnUp.Margin = new System.Windows.Forms.Padding(4);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(65, 61);
            this.btnUp.TabIndex = 16;
            this.btnUp.Tag = "";
            this.btnUp.Text = "ñ";
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnPgUp
            // 
            this.btnPgUp.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnPgUp.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPgUp.Appearance.Options.UseFont = true;
            this.btnPgUp.Image = global::Microsoft.Dynamics.Retail.Pos.Customer.Properties.Resources.top;
            this.btnPgUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPgUp.Location = new System.Drawing.Point(198, 477);
            this.btnPgUp.Margin = new System.Windows.Forms.Padding(4);
            this.btnPgUp.Name = "btnPgUp";
            this.btnPgUp.Size = new System.Drawing.Size(65, 61);
            this.btnPgUp.TabIndex = 15;
            this.btnPgUp.Tag = "";
            this.btnPgUp.Text = "Ç";
            this.btnPgUp.Click += new System.EventHandler(this.btnPgUp_Click);
            // 
            // lblHeader
            // 
            this.lblHeader.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI Light", 36F);
            this.lblHeader.Location = new System.Drawing.Point(330, 10);
            this.lblHeader.Margin = new System.Windows.Forms.Padding(0);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(364, 65);
            this.lblHeader.TabIndex = 1;
            this.lblHeader.Text = "Customer details";
            // 
            // labelCpfCnpjNumber
            // 
            this.labelCpfCnpjNumber.AutoSize = true;
            this.labelCpfCnpjNumber.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.labelCpfCnpjNumber.Location = new System.Drawing.Point(3, 657);
            this.labelCpfCnpjNumber.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.labelCpfCnpjNumber.Name = "labelCpfCnpjNumber";
            this.labelCpfCnpjNumber.Size = new System.Drawing.Size(57, 17);
            this.labelCpfCnpjNumber.TabIndex = 6;
            this.labelCpfCnpjNumber.Text = "CpfCnpj:";
            // 
            // textBoxCpfCnpjNumber
            // 
            this.textBoxCpfCnpjNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCpfCnpjNumber.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "CNPJCPFNumber", true));
            this.textBoxCpfCnpjNumber.Location = new System.Drawing.Point(3, 677);
            this.textBoxCpfCnpjNumber.Name = "textBoxCpfCnpjNumber";
            this.textBoxCpfCnpjNumber.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.textBoxCpfCnpjNumber.Properties.Appearance.Options.UseFont = true;
            this.textBoxCpfCnpjNumber.Properties.MaxLength = 255;
            this.textBoxCpfCnpjNumber.Size = new System.Drawing.Size(394, 24);
            this.textBoxCpfCnpjNumber.TabIndex = 25;
            // 
            // btnCity
            // 
            this.btnCity.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCity.Image = global::Microsoft.Dynamics.Retail.Pos.Customer.Properties.Resources.search;
            this.btnCity.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnCity.Location = new System.Drawing.Point(301, 60);
            this.btnCity.Margin = new System.Windows.Forms.Padding(0);
            this.btnCity.Name = "btnCity";
            this.btnCity.Padding = new System.Windows.Forms.Padding(3);
            this.btnCity.Size = new System.Drawing.Size(157, 30);
            this.btnCity.TabIndex = 54;
            this.btnCity.Click += new System.EventHandler(this.btnCity_Click);
            // 
            // frmNewCustomer
            // 
            this.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Appearance.Options.UseFont = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1029, 780);
            this.Controls.Add(this.themedPanel);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmNewCustomer";
            this.Text = "Customer";
            this.Activated += new System.EventHandler(this.frmNewCustomer_Activated);
            this.Controls.SetChildIndex(this.themedPanel, 0);
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.themedPanel)).EndInit();
            this.themedPanel.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanelCenter.ResumeLayout(false);
            this.tableLayoutPanelCenter.PerformLayout();
            this.tableLayoutPanelBottom.ResumeLayout(false);
            this.tableLayoutPanelBottom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabControlParent)).EndInit();
            this.tabControlParent.ResumeLayout(false);
            this.tabPageContact.ResumeLayout(false);
            this.tabPageContact.PerformLayout();
            this.panelDetailsTab.ResumeLayout(false);
            this.panelDetailsTab.PerformLayout();
            this.tableLayoutPanelLeft.ResumeLayout(false);
            this.tableLayoutPanelLeft.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSTD.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMobilePrimary.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxLastName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxMiddleName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxFirstName.Properties)).EndInit();
            this.tableLayoutPanelType.ResumeLayout(false);
            this.tableLayoutPanelType.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSalutation.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxPhone.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMobileSecondary.Properties)).EndInit();
            this.tableLayoutPanelRight.ResumeLayout(false);
            this.tableLayoutPanelRight.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtGender.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxLanguage.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxCurrency.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxEmail.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxGroup.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textCustAgeBracket.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textNationality.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBDay.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBYear.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAnnDay.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAnnYear.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustClassificationGroup.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOccupation.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxReceiptEmail.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxWebSite.Properties)).EndInit();
            this.tabPageHistory.ResumeLayout(false);
            this.panelHistoryTab.ResumeLayout(false);
            this.panelHistoryTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridOrders)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textDateCreated.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textTotalVisits.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textDateLastVisit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textStoreLastVisited.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textTotalSales.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textSearch.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxCpfCnpjNumber.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl themedPanel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelCenter;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelLeft;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelGroup;
        private System.Windows.Forms.Label labelCurrency;
        private System.Windows.Forms.Label labelLanguage;
        private System.Windows.Forms.Label labelEmail;
        private System.Windows.Forms.Label labelReceiptEmail;
        private System.Windows.Forms.Label labelCpfCnpjNumber;
        private DevExpress.XtraEditors.TextEdit textBoxName;
        private DevExpress.XtraEditors.TextEdit textBoxGroup;
        private DevExpress.XtraEditors.TextEdit textBoxCurrency;
        private DevExpress.XtraEditors.TextEdit textBoxLanguage;
        private DevExpress.XtraEditors.TextEdit textBoxPhone;
        private DevExpress.XtraEditors.TextEdit textBoxEmail;
        private DevExpress.XtraEditors.TextEdit textBoxReceiptEmail;
        private DevExpress.XtraEditors.TextEdit textBoxWebSite;
        private DevExpress.XtraEditors.TextEdit textBoxCpfCnpjNumber;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnGroup;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCurrency;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnLanguage;
        private System.Windows.Forms.BindingSource bindingSource;
        private System.Windows.Forms.Label labelType;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelType;
        private System.Windows.Forms.RadioButton radioPerson;
        private System.Windows.Forms.RadioButton radioOrg;
        private DevExpress.XtraEditors.TextEdit textBoxLastName;
        private System.Windows.Forms.Label labelLastName;
        private DevExpress.XtraEditors.TextEdit textBoxMiddleName;
        private System.Windows.Forms.Label labelMiddleName;
        private System.Windows.Forms.Label labelFirstName;
        private DevExpress.XtraEditors.TextEdit textBoxFirstName;
        private ViewAddressUserControl viewAddressUserControl1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelRight;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelBottom;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCancel;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSave;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnClear;
        private DevExpress.XtraTab.XtraTabControl tabControlParent;
        private DevExpress.XtraTab.XtraTabPage tabPageContact;
        private System.Windows.Forms.TableLayoutPanel panelDetailsTab;
        private DevExpress.XtraTab.XtraTabPage tabPageHistory;
        private System.Windows.Forms.TableLayoutPanel panelHistoryTab;
        private System.Windows.Forms.Label labelDateCreated;
        private System.Windows.Forms.Label labelTotalVisits;
        private System.Windows.Forms.Label labelSearch;
        private DevExpress.XtraGrid.GridControl gridOrders;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView;
        private DevExpress.XtraEditors.TextEdit textDateCreated;
        private DevExpress.XtraEditors.TextEdit textTotalVisits;
        private DevExpress.XtraEditors.TextEdit textSearch;
        private DevExpress.XtraEditors.TextEdit textStoreLastVisited;
        private DevExpress.XtraEditors.TextEdit textDateLastVisit;
        private DevExpress.XtraEditors.TextEdit textTotalSales;
        private System.Windows.Forms.Label labelStoreLastVisited;
        private System.Windows.Forms.Label labelTotalSales;
        private System.Windows.Forms.Label labelDateLastVisit;
        private DevExpress.XtraGrid.Columns.GridColumn columnDate;
        private DevExpress.XtraGrid.Columns.GridColumn columnOrderNumber;
        private DevExpress.XtraGrid.Columns.GridColumn columnOrderStatus;
        private DevExpress.XtraGrid.Columns.GridColumn columnStore;
        private DevExpress.XtraGrid.Columns.GridColumn columnItem;
        private DevExpress.XtraGrid.Columns.GridColumn columnQuantity;
        private DevExpress.XtraGrid.Columns.GridColumn columnAmount;
        private DevExpress.XtraEditors.SimpleButton buttonSearch;
        private DevExpress.XtraEditors.SimpleButton buttonClear;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnPgUp;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnUp;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnDown;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnPgDown;
        private System.Windows.Forms.Label lblSalutation;
        private System.Windows.Forms.Label lblOccupation;
        private System.Windows.Forms.Label lblMarriageDate;
        private DevExpress.XtraEditors.TextEdit txtOccupation;
        private System.Windows.Forms.Label lblDOB;
        private System.Windows.Forms.Label lblGender;
        private DevExpress.XtraEditors.TextEdit txtSalutation;
        private DevExpress.XtraEditors.TextEdit txtGender;
        private System.Windows.Forms.Label labelPhone;
        private System.Windows.Forms.Label lblMobileSecondary;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSalutation;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnGender;
        private System.Windows.Forms.Label lblMblPrimary;
        private DevExpress.XtraEditors.TextEdit txtSTD;
        private DevExpress.XtraEditors.TextEdit txtMobilePrimary;
        private DevExpress.XtraEditors.TextEdit txtMobileSecondary;
        private DevExpress.XtraEditors.TextEdit textCustAgeBracket;
        private DevExpress.XtraEditors.TextEdit textNationality;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnAgeBracket;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnNationality;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private DevExpress.XtraEditors.TextEdit txtBDay;
        private DevExpress.XtraEditors.TextEdit txtBYear;
        private DevExpress.XtraEditors.TextEdit txtAnnDay;
        private DevExpress.XtraEditors.TextEdit txtAnnYear;
        private System.Windows.Forms.ComboBox cmbBMonth;
        private System.Windows.Forms.ComboBox cmbAnnMonth;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkResidence;
        private System.Windows.Forms.Label lblReligion;
        private DevExpress.XtraEditors.TextEdit txtCustClassificationGroup;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCustClassificationGrp;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCity;
    }
}