/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using LSRetailPosis.POSProcesses;
using LSRetailPosis.POSProcesses.WinControls;
using DM = Microsoft.Dynamics.Retail.Pos.DataManager;
using BlankOperations;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using System.Collections.ObjectModel;
using LSRetailPosis.Settings;
using System.IO;

namespace Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch
{
    /// <summary>
    /// Summary description for frmCustomerSearch.
    /// </summary>
    public class frmCustomerSearch : frmTouchBase
    {
        private String selectedCustomerName;
        private String selectedCustomerId;
        private PanelControl basePanel;
        private const int maxRowsAtEachQuery = 200;
        private int loadedCount; // = 0;
        private string sortBy = "Party.Name";
        private bool sortAsc = true;
        private string lastSearch = string.Empty;
        private TableLayoutPanel tableLayoutPanel1;
        private DevExpress.XtraGrid.GridControl grCustomers;
        private DevExpress.XtraGrid.Views.Grid.GridView grdView;
        private DevExpress.XtraGrid.Columns.GridColumn colAccountNum;
        private DevExpress.XtraGrid.Columns.GridColumn colName;
        private DevExpress.XtraGrid.Columns.GridColumn colAddress;
        private DevExpress.XtraGrid.Columns.GridColumn colOrgID;
        private DevExpress.XtraGrid.Columns.GridColumn colEmail;
        private DevExpress.XtraGrid.Columns.GridColumn colPhoneNumber;
        private TableLayoutPanel tableLayoutPanel3;
        private DevExpress.XtraEditors.TextEdit txtKeyboardInput;
        private SimpleButton btnSearch;
        private SimpleButton btnClear;
        private SimpleButtonEx btnSelect;
        private SimpleButtonEx btnUp;
        private SimpleButtonEx btnPgUp;
        private SimpleButtonEx btnPgDown;
        private SimpleButtonEx btnDown;
        private IList<DM.CustomerSearchResult> customerResultList;
        private DM.CustomerDataManager customerDataManager = new DM.CustomerDataManager(
                    LSRetailPosis.Settings.ApplicationSettings.Database.LocalConnection,
                    LSRetailPosis.Settings.ApplicationSettings.Database.DATAAREAID);
        private SimpleButtonEx btnNew;
        private SimpleButtonEx btnCancel;
        private TableLayoutPanel tableLayoutPanel2;
        private TableLayoutPanel tableLayoutPanel4;
        private Label lblHeading;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        public frmCustomerSearch()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
        }

        frmCustomerOrder formCustOrder;
        frmRepair formRepair;
        frmGSSAcOpenning formGSSAcOpenning;
        frmGSSAccStatment frmGSSAccStatment; // added on 29/04/2014 Report


        public frmCustomerSearch(frmCustomerOrder F)
        {
            InitializeComponent();
            formCustOrder = F;
        }

        public frmCustomerSearch(frmRepair F) // repair
        {
            InitializeComponent();
            formRepair = F; ///-----------
        }

        public frmCustomerSearch(frmGSSAcOpenning F) // GSSAcOpenning
        {
            InitializeComponent();
            formGSSAcOpenning = F; ///-----------
        }

        public frmCustomerSearch(frmGSSAccStatment F) // GSSAcStateMnet  // added on 29/04/2014 Report
        {
            InitializeComponent();
            frmGSSAccStatment = F; ///-----------
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!this.DesignMode)
            {           
                GetCustomerResultList(string.Empty, loadedCount, maxRowsAtEachQuery, sortBy, sortAsc, false);

                this.loadedCount = maxRowsAtEachQuery;

                this.grCustomers.DataSource = customerResultList;

                this.grdView.FocusedColumn = this.grdView.Columns["Name"];

                // Show Org ID column if in Iceland
                if (System.Threading.Thread.CurrentThread.CurrentUICulture.Name == "is" || System.Threading.Thread.CurrentThread.CurrentUICulture.Name == "is-IS")
                    colOrgID.Visible = true;

                TranslateLabels();
                CheckRowPosition();
                SetFormFocus();
            }

            base.OnLoad(e);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCustomerSearch));
            this.grCustomers = new DevExpress.XtraGrid.GridControl();
            this.grdView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colAccountNum = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAddress = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colOrgID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colEmail = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPhoneNumber = new DevExpress.XtraGrid.Columns.GridColumn();
            this.basePanel = new DevExpress.XtraEditors.PanelControl();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lblHeading = new System.Windows.Forms.Label();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.btnCancel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnPgUp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnUp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnPgDown = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnDown = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnNew = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSelect = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.btnClear = new DevExpress.XtraEditors.SimpleButton();
            this.txtKeyboardInput = new DevExpress.XtraEditors.TextEdit();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grCustomers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.basePanel)).BeginInit();
            this.basePanel.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyboardInput.Properties)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grCustomers
            // 
            this.grCustomers.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tableLayoutPanel1.SetColumnSpan(this.grCustomers, 9);
            this.grCustomers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grCustomers.Location = new System.Drawing.Point(0, 0);
            this.grCustomers.MainView = this.grdView;
            this.grCustomers.Margin = new System.Windows.Forms.Padding(0);
            this.grCustomers.Name = "grCustomers";
            this.grCustomers.Size = new System.Drawing.Size(962, 492);
            this.grCustomers.TabIndex = 0;
            this.grCustomers.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdView});
            this.grCustomers.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grItems_KeyDown);
            // 
            // grdView
            // 
            this.grdView.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.grdView.Appearance.HeaderPanel.Options.UseFont = true;
            this.grdView.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.grdView.Appearance.Row.Options.UseFont = true;
            this.grdView.ColumnPanelRowHeight = 40;
            this.grdView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colAccountNum,
            this.colName,
            this.colAddress,
            this.colOrgID,
            this.colEmail,
            this.colPhoneNumber});
            this.grdView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.grdView.GridControl = this.grCustomers;
            this.grdView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.grdView.Name = "grdView";
            this.grdView.OptionsBehavior.Editable = false;
            this.grdView.OptionsCustomization.AllowFilter = false;
            this.grdView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.grdView.OptionsView.ShowGroupPanel = false;
            this.grdView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.True;
            this.grdView.OptionsView.ShowIndicator = false;
            this.grdView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.True;
            this.grdView.RowHeight = 40;
            this.grdView.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.None;
            this.grdView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.Default;
            this.grdView.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.colName, DevExpress.Data.ColumnSortOrder.Ascending)});
            this.grdView.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.grdView.FocusedColumnChanged += new DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventHandler(this.gridView1_FocusedColumnChanged);
            this.grdView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grItems_KeyDown);
            this.grdView.Click += new System.EventHandler(this.grdView_Click);
            this.grdView.DoubleClick += new System.EventHandler(this.gridView1_DoubleClick);
            // 
            // colAccountNum
            // 
            this.colAccountNum.Caption = "Customer ID";
            this.colAccountNum.FieldName = "AccountNumber";
            this.colAccountNum.Name = "colAccountNum";
            this.colAccountNum.OptionsColumn.AllowEdit = false;
            this.colAccountNum.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.colAccountNum.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            this.colAccountNum.Visible = true;
            this.colAccountNum.VisibleIndex = 0;
            this.colAccountNum.Width = 142;
            // 
            // colName
            // 
            this.colName.Caption = "Name";
            this.colName.FieldName = "Name";
            this.colName.Name = "colName";
            this.colName.OptionsColumn.AllowEdit = false;
            this.colName.OptionsColumn.AllowIncrementalSearch = false;
            this.colName.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colName.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.colName.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            this.colName.Visible = true;
            this.colName.VisibleIndex = 1;
            this.colName.Width = 233;
            // 
            // colAddress
            // 
            this.colAddress.Caption = "Address";
            this.colAddress.FieldName = "PrimaryStreetAddress";
            this.colAddress.Name = "colAddress";
            this.colAddress.OptionsColumn.AllowEdit = false;
            this.colAddress.OptionsColumn.AllowIncrementalSearch = false;
            this.colAddress.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colAddress.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colAddress.ShowUnboundExpressionMenu = true;
            this.colAddress.Visible = true;
            this.colAddress.VisibleIndex = 2;
            this.colAddress.Width = 291;
            // 
            // colOrgID
            // 
            this.colOrgID.Caption = "Org Id";
            this.colOrgID.FieldName = "ORGID";
            this.colOrgID.Name = "colOrgID";
            this.colOrgID.OptionsColumn.AllowEdit = false;
            this.colOrgID.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.colOrgID.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            this.colOrgID.Width = 127;
            // 
            // colEmail
            // 
            this.colEmail.Caption = "E-mail";
            this.colEmail.FieldName = "Email";
            this.colEmail.Name = "colEmail";
            this.colEmail.OptionsColumn.AllowEdit = false;
            this.colEmail.OptionsColumn.AllowIncrementalSearch = false;
            this.colEmail.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colEmail.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colEmail.Visible = true;
            this.colEmail.VisibleIndex = 3;
            this.colEmail.Width = 167;
            // 
            // colPhoneNumber
            // 
            this.colPhoneNumber.Caption = "Phone number";
            this.colPhoneNumber.FieldName = "Phone";
            this.colPhoneNumber.Name = "colPhoneNumber";
            this.colPhoneNumber.OptionsColumn.AllowEdit = false;
            this.colPhoneNumber.OptionsColumn.AllowIncrementalSearch = false;
            this.colPhoneNumber.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colPhoneNumber.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colPhoneNumber.Visible = true;
            this.colPhoneNumber.VisibleIndex = 4;
            this.colPhoneNumber.Width = 166;
            // 
            // basePanel
            // 
            this.basePanel.Controls.Add(this.tableLayoutPanel2);
            this.basePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.basePanel.Location = new System.Drawing.Point(0, 0);
            this.basePanel.Name = "basePanel";
            this.basePanel.Size = new System.Drawing.Size(1024, 768);
            this.basePanel.TabIndex = 1;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.lblHeading, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel4, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 0, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.Padding = new System.Windows.Forms.Padding(26, 40, 26, 11);
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1020, 764);
            this.tableLayoutPanel2.TabIndex = 10;
            // 
            // lblHeading
            // 
            this.lblHeading.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblHeading.AutoSize = true;
            this.lblHeading.Font = new System.Drawing.Font("Segoe UI Light", 36F);
            this.lblHeading.Location = new System.Drawing.Point(327, 40);
            this.lblHeading.Margin = new System.Windows.Forms.Padding(0);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Padding = new System.Windows.Forms.Padding(0, 0, 0, 30);
            this.lblHeading.Size = new System.Drawing.Size(366, 95);
            this.lblHeading.TabIndex = 41;
            this.lblHeading.Text = "Customer search";
            this.lblHeading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 9;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.Controls.Add(this.btnCancel, 5, 0);
            this.tableLayoutPanel4.Controls.Add(this.btnPgUp, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.btnUp, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.btnPgDown, 8, 0);
            this.tableLayoutPanel4.Controls.Add(this.btnDown, 7, 0);
            this.tableLayoutPanel4.Controls.Add(this.btnNew, 3, 0);
            this.tableLayoutPanel4.Controls.Add(this.btnSelect, 4, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(26, 687);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0, 11, 0, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(968, 66);
            this.tableLayoutPanel4.TabIndex = 17;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnCancel.Location = new System.Drawing.Point(555, 4);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(127, 57);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Tag = "";
            this.btnCancel.Text = "Cancel";
            // 
            // btnPgUp
            // 
            this.btnPgUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPgUp.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPgUp.Appearance.Options.UseFont = true;
            this.btnPgUp.Image = global::Microsoft.Dynamics.Retail.Pos.Customer.Properties.Resources.top;
            this.btnPgUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPgUp.Location = new System.Drawing.Point(4, 4);
            this.btnPgUp.Margin = new System.Windows.Forms.Padding(4);
            this.btnPgUp.Name = "btnPgUp";
            this.btnPgUp.Size = new System.Drawing.Size(57, 57);
            this.btnPgUp.TabIndex = 10;
            this.btnPgUp.Tag = "";
            this.btnPgUp.Text = "�";
            this.btnPgUp.Click += new System.EventHandler(this.btnPgUp_Click);
            // 
            // btnUp
            // 
            this.btnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUp.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnUp.Appearance.Options.UseFont = true;
            this.btnUp.Image = global::Microsoft.Dynamics.Retail.Pos.Customer.Properties.Resources.up;
            this.btnUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnUp.Location = new System.Drawing.Point(69, 4);
            this.btnUp.Margin = new System.Windows.Forms.Padding(4);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(57, 57);
            this.btnUp.TabIndex = 11;
            this.btnUp.Tag = "";
            this.btnUp.Text = "�";
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnPgDown
            // 
            this.btnPgDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPgDown.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPgDown.Appearance.Options.UseFont = true;
            this.btnPgDown.Image = global::Microsoft.Dynamics.Retail.Pos.Customer.Properties.Resources.bottom;
            this.btnPgDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPgDown.Location = new System.Drawing.Point(906, 4);
            this.btnPgDown.Margin = new System.Windows.Forms.Padding(4);
            this.btnPgDown.Name = "btnPgDown";
            this.btnPgDown.Size = new System.Drawing.Size(58, 57);
            this.btnPgDown.TabIndex = 16;
            this.btnPgDown.Tag = "";
            this.btnPgDown.Text = "�";
            this.btnPgDown.Click += new System.EventHandler(this.btnPgDown_Click);
            // 
            // btnDown
            // 
            this.btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDown.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnDown.Appearance.Options.UseFont = true;
            this.btnDown.Image = global::Microsoft.Dynamics.Retail.Pos.Customer.Properties.Resources.down;
            this.btnDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnDown.Location = new System.Drawing.Point(841, 4);
            this.btnDown.Margin = new System.Windows.Forms.Padding(4);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(57, 57);
            this.btnDown.TabIndex = 15;
            this.btnDown.Tag = "";
            this.btnDown.Text = "�";
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnNew
            // 
            this.btnNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNew.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnNew.Appearance.Options.UseFont = true;
            this.btnNew.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnNew.Location = new System.Drawing.Point(285, 4);
            this.btnNew.Margin = new System.Windows.Forms.Padding(4);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(127, 57);
            this.btnNew.TabIndex = 12;
            this.btnNew.Tag = "";
            this.btnNew.Text = "New";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnSelect.Appearance.Options.UseFont = true;
            this.btnSelect.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSelect.Location = new System.Drawing.Point(420, 4);
            this.btnSelect.Margin = new System.Windows.Forms.Padding(4);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(127, 57);
            this.btnSelect.TabIndex = 13;
            this.btnSelect.Tag = "";
            this.btnSelect.Text = "Select";
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.btnSearch, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnClear, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.txtKeyboardInput, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(29, 138);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(962, 37);
            this.tableLayoutPanel3.TabIndex = 9;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSearch.Image = global::Microsoft.Dynamics.Retail.Pos.Customer.Properties.Resources.search;
            this.btnSearch.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSearch.Location = new System.Drawing.Point(839, 3);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Padding = new System.Windows.Forms.Padding(3);
            this.btnSearch.Size = new System.Drawing.Size(57, 31);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Search";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClear.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F);
            this.btnClear.Appearance.Options.UseFont = true;
            this.btnClear.Image = global::Microsoft.Dynamics.Retail.Pos.Customer.Properties.Resources.remove;
            this.btnClear.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnClear.Location = new System.Drawing.Point(902, 3);
            this.btnClear.Name = "btnClear";
            this.btnClear.Padding = new System.Windows.Forms.Padding(3);
            this.btnClear.Size = new System.Drawing.Size(57, 31);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "�";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // txtKeyboardInput
            // 
            this.txtKeyboardInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtKeyboardInput.Location = new System.Drawing.Point(3, 3);
            this.txtKeyboardInput.Name = "txtKeyboardInput";
            this.txtKeyboardInput.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.txtKeyboardInput.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.txtKeyboardInput.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtKeyboardInput.Properties.Appearance.Options.UseBackColor = true;
            this.txtKeyboardInput.Properties.Appearance.Options.UseFont = true;
            this.txtKeyboardInput.Properties.Appearance.Options.UseForeColor = true;
            this.txtKeyboardInput.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtKeyboardInput.Size = new System.Drawing.Size(830, 32);
            this.txtKeyboardInput.TabIndex = 1;
            this.txtKeyboardInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CustomerSearch_KeyDown);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 9;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.grCustomers, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(29, 181);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(962, 492);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // frmCustomerSearch
            // 
            this.Appearance.Options.UseBackColor = true;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1024, 768);
            this.Controls.Add(this.basePanel);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmCustomerSearch";
            this.Text = "Customer search";
            this.Controls.SetChildIndex(this.basePanel, 0);
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grCustomers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.basePanel)).EndInit();
            this.basePanel.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyboardInput.Properties)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// Get selected customer name.
        /// </summary>
        public String SelectedCustomerName
        {
            get { return selectedCustomerName; }
        }

        /// <summary>
        /// Get selected customer id.
        /// </summary>
        public String SelectedCustomerId
        {
            get { return selectedCustomerId; }
        }

        private void TranslateLabels()
        {
            // Get all text through the Translation function in the ApplicationLocalizer
            // TextID's for Customer BalanceReport are reserved at 51100 - 51119
            // Used textid's 51100 - 51110

            grdView.Columns[0].Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(51112); //Customer ID
            grdView.Columns[1].Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(51107); //Name
            grdView.Columns[2].Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(51108); //Address
            grdView.Columns[3].Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(51111); //OrgId
            grdView.Columns[4].Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(51138); //E-mail
            grdView.Columns[5].Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(51134); //Phone number

            btnSelect.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51105); //OK
            btnNew.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51113); //New
            btnCancel.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51114); //Cancel
            this.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(1552); //"Customer search"
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            grdView.MoveNext();
            int topRowIndex = grdView.TopRowIndex;
            if ((grdView.IsLastRow) && (grdView.RowCount > 0))
            {
                GetCustomerResultList(txtKeyboardInput.Text, loadedCount, maxRowsAtEachQuery, sortBy, sortAsc, true);

                this.grCustomers.DataSource = this.customerResultList;

                loadedCount += maxRowsAtEachQuery;

                grdView.TopRowIndex = topRowIndex;
            }
            SetFormFocus();
            CheckRowPosition();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            grdView.MovePrev();
            SetFormFocus();
            CheckRowPosition();
        }

        private void btnPgDown_Click(object sender, EventArgs e)
        {
            grdView.MoveNextPage();
            int topRowIndex = grdView.TopRowIndex;
            if ((grdView.IsLastRow) && (grdView.RowCount > 0))
            {
                GetCustomerResultList(txtKeyboardInput.Text, loadedCount, maxRowsAtEachQuery, sortBy, sortAsc, true);

                this.grCustomers.DataSource = this.customerResultList;

                loadedCount += maxRowsAtEachQuery;
                grdView.TopRowIndex = topRowIndex;
            }
            SetFormFocus();
            CheckRowPosition();
        }

        private void btnPgUp_Click(object sender, EventArgs e)
        {
            grdView.MovePrevPage();
            SetFormFocus();
            CheckRowPosition();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectCustomer();
        }

        private void GetCustomerResultList(string searchValue, int fromRow, int numberOfRows, string sortByColumn, bool sortAscending, bool mergeList)
        {
            var customerList = customerDataManager.GetCustomerList(searchValue, fromRow, numberOfRows, sortByColumn, sortAscending);

            if (mergeList)
            {
                foreach (var customerInfo in customerList)
                {
                    this.customerResultList.Add(customerInfo);
                }
            }
            else
            {
                this.customerResultList = customerList;
            }
        }

        private void keyboard1_EnterButtonPressed()
        {
            if (txtKeyboardInput.Text.Trim() == lastSearch)
            {
                btnSelect_Click(null, null);
            }
            else
            {
                lastSearch = txtKeyboardInput.Text;
                loadedCount = 0;

                GetCustomerResultList(lastSearch, loadedCount, maxRowsAtEachQuery, sortBy, sortAsc, false);

                this.loadedCount = maxRowsAtEachQuery;

                this.grCustomers.DataSource = this.customerResultList;
            }
            CheckRowPosition();
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            Point p = grdView.GridControl.PointToClient(MousePosition);
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo info = grdView.CalcHitInfo(p);

            if (info.HitTest != DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitTest.Column)
            {
                SelectCustomer();
            }
        }

        private void SelectCustomer()
        {
            if (grdView.RowCount > 0)
            {
                int selectedIndex = grdView.GetSelectedRows()[0];

                if (this.customerResultList.Count() > selectedIndex)
                {
                    var selectedCustomer = this.customerResultList[selectedIndex];

                    selectedCustomerName = selectedCustomer.Name;
                    selectedCustomerId = selectedCustomer.AccountNumber;

                    this.DialogResult = System.Windows.Forms.DialogResult.OK;

                    //CheckBirthDayOrMarriageDay
                    try
                    {
                        if(PosApplication.Instance.TransactionServices.CheckConnection())
                        {
                            ReadOnlyCollection<object> containerArray;
                            containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("CheckBirthDayOrMarriageDay", selectedCustomerId);


                            StringReader srTransDetail = new StringReader(Convert.ToString(containerArray[2]));

                            if(Convert.ToString(containerArray[2]).Trim().Length > 5)
                            {
                                MessageBox.Show(Convert.ToString(containerArray[2]));
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                    }

                    if (formCustOrder != null)
                    {
                        formCustOrder.txtCustomerAccount.Text = selectedCustomerId.Trim();
                        formCustOrder.txtCustomerName.Text = selectedCustomer.Name.Trim();
                        formCustOrder.txtCustomerAddress.Text = formCustOrder.CustAddress = string.IsNullOrEmpty(selectedCustomer.PrimaryStreetAddress) ? string.Empty : selectedCustomer.PrimaryStreetAddress;
                        formCustOrder.txtPhoneNumber.Text = formCustOrder.CustPhoneNo = string.IsNullOrEmpty(selectedCustomer.Phone) ? string.Empty : selectedCustomer.Phone;
                    }
                    if (formRepair != null)  // repair
                    {
                        formRepair.txtCustomerAccount.Text = selectedCustomerId.Trim();
                        formRepair.txtCustomerName.Text = selectedCustomer.Name.Trim();
                        formRepair.txtCustomerAddress.Text = formRepair.CustAddress = string.IsNullOrEmpty(selectedCustomer.PrimaryStreetAddress) ? string.Empty : selectedCustomer.PrimaryStreetAddress;
                        formRepair.txtPhoneNumber.Text = formRepair.CustPhoneNo = string.IsNullOrEmpty(selectedCustomer.Phone) ? string.Empty : selectedCustomer.Phone;
                    }
                    if (formGSSAcOpenning != null)  // GSS
                    {
                        formGSSAcOpenning.txtCustomerAccount.Text = selectedCustomerId.Trim();
                        formGSSAcOpenning.txtCustomerName.Text = selectedCustomer.Name.Trim();
                        formGSSAcOpenning.txtCustomerAddress.Text = formGSSAcOpenning.CustAddress = string.IsNullOrEmpty(selectedCustomer.PrimaryStreetAddress) ? string.Empty : selectedCustomer.PrimaryStreetAddress;
                        formGSSAcOpenning.txtPhoneNumber.Text = formGSSAcOpenning.CustPhoneNo = string.IsNullOrEmpty(selectedCustomer.Phone) ? string.Empty : selectedCustomer.Phone;
                    }
                    if (frmGSSAccStatment != null)  // GSSAccStatement // added on 29/04/2014 Report
                    {
                        frmGSSAccStatment.txtCustomerAccount.Text = selectedCustomerId.Trim();
                        frmGSSAccStatment.txtCustomerName.Text = selectedCustomer.Name.Trim();
                        frmGSSAccStatment.txtCustomerAddress.Text = frmGSSAccStatment.CustAddress = string.IsNullOrEmpty(selectedCustomer.PrimaryStreetAddress) ? string.Empty : selectedCustomer.PrimaryStreetAddress;
                        frmGSSAccStatment.txtPhoneNumber.Text = frmGSSAccStatment.CustPhoneNo = string.IsNullOrEmpty(selectedCustomer.Phone) ? string.Empty : selectedCustomer.Phone;
                    }
                }
                else
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                }

                Close();
            }
            else
            {
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                Close();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtKeyboardInput.Text = string.Empty;

            this.loadedCount = 0;

            GetCustomerResultList(txtKeyboardInput.Text, loadedCount, maxRowsAtEachQuery, sortBy, sortAsc, false);

            this.loadedCount = maxRowsAtEachQuery;


            grCustomers.DataSource = this.customerResultList;
        }

        private void gridView1_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            SetFormFocus();           
        }

        private void SetFormFocus()
        {
            txtKeyboardInput.Select();          
        }

        private void keyboard1_UpButtonPressed()
        {
            btnUp_Click(this, new EventArgs());
        }

        private void keyboard1_DownButtonPressed()
        {
            btnDown_Click(this, new EventArgs());
        }

        private void keyboard1_PgUpButtonPressed()
        {
            btnPgUp_Click(this, new EventArgs());
        }

        private void keyboard1_PgDownButtonPressed()
        {
            btnPgDown_Click(this, new EventArgs());
        } 

        private void grItems_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                btnSelect_Click(null, null);
            }
            CheckRowPosition();
        }

       

        private void CustomerSearch_KeyDown(object sender, KeyEventArgs e)
        {           
                switch (e.KeyCode)
                {
                    case Keys.Return:
                        keyboard1_EnterButtonPressed();
                        break;
                    case Keys.Up:
                        keyboard1_UpButtonPressed();
                        break;
                    case Keys.Down:
                        keyboard1_DownButtonPressed();
                        break;
                    case Keys.PageUp:
                        keyboard1_PgUpButtonPressed();
                        break;
                    case Keys.PageDown:
                        keyboard1_PgDownButtonPressed();
                        break;
                    case Keys.Escape:
                        this.Close();
                        break;
                    default:
                        break;
                }               
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {

            if (!(txtKeyboardInput.Text.Trim() == lastSearch))
            {
                loadedCount = 0;

                GetCustomerResultList(txtKeyboardInput.Text, loadedCount, maxRowsAtEachQuery, sortBy, sortAsc, false);

                loadedCount = maxRowsAtEachQuery;

                grCustomers.DataSource = this.customerResultList;
            }
            CheckRowPosition();
        }

        private void CheckRowPosition()
        {            
            btnPgDown.Enabled = (grdView.IsLastRow) ? false : true;
            btnDown.Enabled = btnPgDown.Enabled;
            btnPgUp.Enabled = (grdView.IsFirstRow) ? false : true;
            btnUp.Enabled = btnPgUp.Enabled;
        }

        private void grdView_Click(object sender, EventArgs e)
        {
            Point p = grdView.GridControl.PointToClient(MousePosition);
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo info = grdView.CalcHitInfo(p);

            if (info.HitTest == DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitTest.Column &&
                info.Column.OptionsColumn.AllowSort == DevExpress.Utils.DefaultBoolean.True)
            {
                if (sortBy == info.Column.FieldName)
                {
                    sortAsc = !sortAsc;
                }
                else
                {
                    sortAsc = true;
                }

                sortBy = info.Column.FieldName;

                loadedCount = 0;

                GetCustomerResultList(txtKeyboardInput.Text, loadedCount, maxRowsAtEachQuery, sortBy, sortAsc, false);

                loadedCount = maxRowsAtEachQuery;

                grCustomers.DataSource = this.customerResultList;

                foreach (DevExpress.XtraGrid.Columns.GridColumn col in grdView.Columns)
                {
                    col.SortOrder = DevExpress.Data.ColumnSortOrder.None;
                }

                grdView.Columns[sortBy].SortOrder = sortAsc ? DevExpress.Data.ColumnSortOrder.Ascending : DevExpress.Data.ColumnSortOrder.Descending;
            }
            CheckRowPosition();
            txtKeyboardInput.Select();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            Customer.InternalApplication.RunOperation(Contracts.PosisOperations.CustomerAdd, null);
        }
    }
}
