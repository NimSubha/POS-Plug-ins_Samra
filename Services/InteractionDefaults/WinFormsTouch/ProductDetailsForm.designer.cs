/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
namespace Microsoft.Dynamics.Retail.Pos.Interaction.WinFormsTouch
{
    partial class ProductDetailsForm
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
            this.lblHeader = new System.Windows.Forms.Label();
            this.tlpContent = new System.Windows.Forms.TableLayoutPanel();
            this.tlpButtons = new System.Windows.Forms.TableLayoutPanel();
            this.btnInventoryLookup = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnAddToSale = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnCancel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.tlpProductInformation = new System.Windows.Forms.TableLayoutPanel();
            this.lblBarCode = new System.Windows.Forms.Label();
            this.lblBarCodeValue = new System.Windows.Forms.Label();
            this.bindingSource = new System.Windows.Forms.BindingSource();
            this.lblSearchName = new System.Windows.Forms.Label();
            this.lblSearchNameValue = new System.Windows.Forms.Label();
            this.lblCategory = new System.Windows.Forms.Label();
            this.lblCategoryValue = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblDescriptionValue = new System.Windows.Forms.Label();
            this.lblPrice = new System.Windows.Forms.Label();
            this.lblIssueDate = new System.Windows.Forms.Label();
            this.lblDateBlocked = new System.Windows.Forms.Label();
            this.lblDateToBeBlocked = new System.Windows.Forms.Label();
            this.lblProductAttributes = new System.Windows.Forms.Label();
            this.lblPriceValue = new System.Windows.Forms.Label();
            this.lblIssueDateValue = new System.Windows.Forms.Label();
            this.lblDateBlockedValue = new System.Windows.Forms.Label();
            this.lblDateToBeBlockedValue = new System.Windows.Forms.Label();
            this.tlpProductAttributes = new System.Windows.Forms.TableLayoutPanel();
            this.gridProductAttributes = new DevExpress.XtraGrid.GridControl();
            this.gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colValue = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnPgUp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnUp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnDown = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnPgDown = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.pbProductImage = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.tlpContent.SuspendLayout();
            this.tlpButtons.SuspendLayout();
            this.tlpProductInformation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).BeginInit();
            this.tlpProductAttributes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridProductAttributes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbProductImage)).BeginInit();
            this.SuspendLayout();
            // 
            // lblHeader
            // 
            this.lblHeader.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI Light", 36F);
            this.lblHeader.Location = new System.Drawing.Point(26, 40);
            this.lblHeader.Margin = new System.Windows.Forms.Padding(0);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Padding = new System.Windows.Forms.Padding(0, 0, 0, 30);
            this.lblHeader.Size = new System.Drawing.Size(972, 95);
            this.lblHeader.TabIndex = 1;
            this.lblHeader.Text = "Item number: item name";
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tlpContent
            // 
            this.tlpContent.AutoSize = true;
            this.tlpContent.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpContent.ColumnCount = 1;
            this.tlpContent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpContent.Controls.Add(this.tlpButtons, 0, 3);
            this.tlpContent.Controls.Add(this.lblHeader, 0, 0);
            this.tlpContent.Controls.Add(this.tlpProductInformation, 0, 2);
            this.tlpContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpContent.Location = new System.Drawing.Point(0, 0);
            this.tlpContent.Margin = new System.Windows.Forms.Padding(0);
            this.tlpContent.Name = "tlpContent";
            this.tlpContent.Padding = new System.Windows.Forms.Padding(26, 40, 26, 11);
            this.tlpContent.RowCount = 4;
            this.tlpContent.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpContent.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpContent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpContent.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpContent.Size = new System.Drawing.Size(1024, 768);
            this.tlpContent.TabIndex = 0;
            // 
            // tlpButtons
            // 
            this.tlpButtons.ColumnCount = 5;
            this.tlpContent.SetColumnSpan(this.tlpButtons, 7);
            this.tlpButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpButtons.Controls.Add(this.btnInventoryLookup, 2, 0);
            this.tlpButtons.Controls.Add(this.btnAddToSale, 1, 0);
            this.tlpButtons.Controls.Add(this.btnCancel, 3, 0);
            this.tlpButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpButtons.Location = new System.Drawing.Point(29, 689);
            this.tlpButtons.Name = "tlpButtons";
            this.tlpButtons.RowCount = 1;
            this.tlpButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpButtons.Size = new System.Drawing.Size(966, 65);
            this.tlpButtons.TabIndex = 0;
            // 
            // btnInventoryLookup
            // 
            this.btnInventoryLookup.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnInventoryLookup.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnInventoryLookup.Appearance.Options.UseFont = true;
            this.btnInventoryLookup.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnInventoryLookup.Location = new System.Drawing.Point(407, 4);
            this.btnInventoryLookup.Margin = new System.Windows.Forms.Padding(4);
            this.btnInventoryLookup.Name = "btnInventoryLookup";
            this.btnInventoryLookup.Size = new System.Drawing.Size(151, 57);
            this.btnInventoryLookup.TabIndex = 1;
            this.btnInventoryLookup.Tag = "";
            this.btnInventoryLookup.Text = "Inventory lookup";
            this.btnInventoryLookup.Click += new System.EventHandler(this.OnBtnInventoryLookup_Click);
            // 
            // btnAddToSale
            // 
            this.btnAddToSale.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnAddToSale.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnAddToSale.Appearance.Options.UseFont = true;
            this.btnAddToSale.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnAddToSale.Location = new System.Drawing.Point(272, 4);
            this.btnAddToSale.Margin = new System.Windows.Forms.Padding(4);
            this.btnAddToSale.Name = "btnAddToSale";
            this.btnAddToSale.Size = new System.Drawing.Size(127, 57);
            this.btnAddToSale.TabIndex = 0;
            this.btnAddToSale.Text = "Add to sale";
            this.btnAddToSale.Click += new System.EventHandler(this.OnBtnAddToSale_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnCancel.Location = new System.Drawing.Point(566, 4);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(127, 57);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Tag = "";
            this.btnCancel.Text = "Cancel";
            // 
            // tlpProductInformation
            // 
            this.tlpProductInformation.ColumnCount = 3;
            this.tlpProductInformation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpProductInformation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpProductInformation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpProductInformation.Controls.Add(this.lblPrice, 0, 0);
            this.tlpProductInformation.Controls.Add(this.lblPriceValue, 0, 1);
            this.tlpProductInformation.Controls.Add(this.lblBarCode, 0, 2);
            this.tlpProductInformation.Controls.Add(this.lblBarCodeValue, 0, 3);
            this.tlpProductInformation.Controls.Add(this.lblSearchName, 0, 4);
            this.tlpProductInformation.Controls.Add(this.lblSearchNameValue, 0, 5);
            this.tlpProductInformation.Controls.Add(this.lblCategory, 0, 6);
            this.tlpProductInformation.Controls.Add(this.lblCategoryValue, 0, 7);
            this.tlpProductInformation.Controls.Add(this.lblDescription, 0, 8);
            this.tlpProductInformation.Controls.Add(this.lblDescriptionValue, 0, 9);
            this.tlpProductInformation.Controls.Add(this.lblIssueDate, 0, 10);
            this.tlpProductInformation.Controls.Add(this.lblIssueDateValue, 0, 11);
            this.tlpProductInformation.Controls.Add(this.lblDateBlocked, 0, 12);
            this.tlpProductInformation.Controls.Add(this.lblDateBlockedValue, 0, 13);
            this.tlpProductInformation.Controls.Add(this.lblDateToBeBlocked, 0, 14);
            this.tlpProductInformation.Controls.Add(this.lblDateToBeBlockedValue, 0, 15);
            this.tlpProductInformation.Controls.Add(this.lblProductAttributes, 2, 10);
            this.tlpProductInformation.Controls.Add(this.tlpProductAttributes, 2, 11);
            this.tlpProductInformation.Controls.Add(this.pbProductImage, 2, 0);
            this.tlpProductInformation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpProductInformation.Location = new System.Drawing.Point(29, 138);
            this.tlpProductInformation.Name = "tlpProductInformation";
            this.tlpProductInformation.RowCount = 18;
            this.tlpProductInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.882352F));
            this.tlpProductInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.882352F));
            this.tlpProductInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.882352F));
            this.tlpProductInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.882352F));
            this.tlpProductInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.882352F));
            this.tlpProductInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.882352F));
            this.tlpProductInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.882352F));
            this.tlpProductInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.882352F));
            this.tlpProductInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.882352F));
            this.tlpProductInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpProductInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.882352F));
            this.tlpProductInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.882352F));
            this.tlpProductInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.882352F));
            this.tlpProductInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.882352F));
            this.tlpProductInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.882352F));
            this.tlpProductInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.882352F));
            this.tlpProductInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.882352F));
            this.tlpProductInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.882352F));
            this.tlpProductInformation.Size = new System.Drawing.Size(966, 545);
            this.tlpProductInformation.TabIndex = 2;
            // 
            // lblBarCode
            // 
            this.lblBarCode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblBarCode.AutoSize = true;
            this.lblBarCode.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblBarCode.Location = new System.Drawing.Point(501, 0);
            this.lblBarCode.Name = "lblBarCode";
            this.lblBarCode.Size = new System.Drawing.Size(462, 28);
            this.lblBarCode.TabIndex = 2;
            this.lblBarCode.Text = "Bar code:";
            this.lblBarCode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblBarCodeValue
            // 
            this.lblBarCodeValue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblBarCodeValue.AutoSize = true;
            this.lblBarCodeValue.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "Barcode", true));
            this.lblBarCodeValue.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblBarCodeValue.Location = new System.Drawing.Point(501, 28);
            this.lblBarCodeValue.Name = "lblBarCodeValue";
            this.lblBarCodeValue.Size = new System.Drawing.Size(462, 28);
            this.lblBarCodeValue.TabIndex = 3;
            this.lblBarCodeValue.Text = "00000000000000";
            // 
            // bindingSource
            // 
            this.bindingSource.DataSource = typeof(Microsoft.Dynamics.Retail.Pos.Interaction.ViewModels.ProductDetailsViewModel);
            // 
            // lblSearchName
            // 
            this.lblSearchName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSearchName.AutoSize = true;
            this.lblSearchName.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblSearchName.Location = new System.Drawing.Point(501, 56);
            this.lblSearchName.Name = "lblSearchName";
            this.lblSearchName.Size = new System.Drawing.Size(462, 28);
            this.lblSearchName.TabIndex = 4;
            this.lblSearchName.Text = "Search name:";
            this.lblSearchName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSearchNameValue
            // 
            this.lblSearchNameValue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSearchNameValue.AutoSize = true;
            this.lblSearchNameValue.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "SearchName", true));
            this.lblSearchNameValue.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSearchNameValue.Location = new System.Drawing.Point(501, 84);
            this.lblSearchNameValue.Name = "lblSearchNameValue";
            this.lblSearchNameValue.Size = new System.Drawing.Size(462, 28);
            this.lblSearchNameValue.TabIndex = 5;
            this.lblSearchNameValue.Text = "Item name";
            // 
            // lblCategory
            // 
            this.lblCategory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCategory.AutoSize = true;
            this.lblCategory.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblCategory.Location = new System.Drawing.Point(501, 112);
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.Size = new System.Drawing.Size(462, 28);
            this.lblCategory.TabIndex = 6;
            this.lblCategory.Text = "Category:";
            this.lblCategory.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCategoryValue
            // 
            this.lblCategoryValue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCategoryValue.AutoSize = true;
            this.lblCategoryValue.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "FormattedProductCategoryHierarchy", true));
            this.lblCategoryValue.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblCategoryValue.Location = new System.Drawing.Point(501, 140);
            this.lblCategoryValue.Name = "lblCategoryValue";
            this.lblCategoryValue.Size = new System.Drawing.Size(462, 28);
            this.lblCategoryValue.TabIndex = 7;
            this.lblCategoryValue.Text = "Category";
            // 
            // lblDescription
            // 
            this.lblDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDescription.AutoSize = true;
            this.lblDescription.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblDescription.Location = new System.Drawing.Point(501, 168);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(462, 28);
            this.lblDescription.TabIndex = 8;
            this.lblDescription.Text = "Description:";
            this.lblDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblDescriptionValue
            // 
            this.lblDescriptionValue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDescriptionValue.AutoSize = true;
            this.lblDescriptionValue.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "Description", true));
            this.lblDescriptionValue.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblDescriptionValue.Location = new System.Drawing.Point(501, 196);
            this.lblDescriptionValue.Name = "lblDescriptionValue";
            this.lblDescriptionValue.Size = new System.Drawing.Size(462, 60);
            this.lblDescriptionValue.TabIndex = 9;
            this.lblDescriptionValue.Text = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor i" +
                "ncididunt ut labore et dolore magna aliqua.";
            // 
            // lblPrice
            // 
            this.lblPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPrice.AutoSize = true;
            this.lblPrice.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblPrice.Location = new System.Drawing.Point(501, 256);
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(462, 28);
            this.lblPrice.TabIndex = 10;
            this.lblPrice.Text = "Price:";
            this.lblPrice.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblIssueDate
            // 
            this.lblIssueDate.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblIssueDate.AutoSize = true;
            this.lblIssueDate.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblIssueDate.Location = new System.Drawing.Point(501, 312);
            this.lblIssueDate.Name = "lblIssueDate";
            this.lblIssueDate.Size = new System.Drawing.Size(462, 28);
            this.lblIssueDate.TabIndex = 12;
            this.lblIssueDate.Text = "Issue date:";
            this.lblIssueDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblDateBlocked
            // 
            this.lblDateBlocked.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDateBlocked.AutoSize = true;
            this.lblDateBlocked.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblDateBlocked.Location = new System.Drawing.Point(501, 368);
            this.lblDateBlocked.Name = "lblDateBlocked";
            this.lblDateBlocked.Size = new System.Drawing.Size(462, 28);
            this.lblDateBlocked.TabIndex = 14;
            this.lblDateBlocked.Text = "Date blocked:";
            this.lblDateBlocked.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblDateToBeBlocked
            // 
            this.lblDateToBeBlocked.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDateToBeBlocked.AutoSize = true;
            this.lblDateToBeBlocked.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblDateToBeBlocked.Location = new System.Drawing.Point(501, 424);
            this.lblDateToBeBlocked.Name = "lblDateToBeBlocked";
            this.lblDateToBeBlocked.Size = new System.Drawing.Size(462, 28);
            this.lblDateToBeBlocked.TabIndex = 16;
            this.lblDateToBeBlocked.Text = "Date to be blocked:";
            this.lblDateToBeBlocked.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblProductAttributes
            // 
            this.lblProductAttributes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProductAttributes.AutoSize = true;
            this.lblProductAttributes.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblProductAttributes.Location = new System.Drawing.Point(3, 312);
            this.lblProductAttributes.Name = "lblProductAttributes";
            this.lblProductAttributes.Size = new System.Drawing.Size(462, 28);
            this.lblProductAttributes.TabIndex = 0;
            this.lblProductAttributes.Text = "Product attributes:";
            this.lblProductAttributes.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPriceValue
            // 
            this.lblPriceValue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPriceValue.AutoSize = true;
            this.lblPriceValue.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "Price", true));
            this.lblPriceValue.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblPriceValue.Location = new System.Drawing.Point(501, 284);
            this.lblPriceValue.Name = "lblPriceValue";
            this.lblPriceValue.Size = new System.Drawing.Size(462, 28);
            this.lblPriceValue.TabIndex = 11;
            this.lblPriceValue.Text = "$20.00";
            // 
            // lblIssueDateValue
            // 
            this.lblIssueDateValue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblIssueDateValue.AutoSize = true;
            this.lblIssueDateValue.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "IssueDate", true));
            this.lblIssueDateValue.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblIssueDateValue.Location = new System.Drawing.Point(501, 340);
            this.lblIssueDateValue.Name = "lblIssueDateValue";
            this.lblIssueDateValue.Size = new System.Drawing.Size(462, 28);
            this.lblIssueDateValue.TabIndex = 13;
            this.lblIssueDateValue.Text = "12/21/2012";
            // 
            // lblDateBlockedValue
            // 
            this.lblDateBlockedValue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDateBlockedValue.AutoSize = true;
            this.lblDateBlockedValue.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "DateBlocked", true));
            this.lblDateBlockedValue.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblDateBlockedValue.Location = new System.Drawing.Point(501, 396);
            this.lblDateBlockedValue.Name = "lblDateBlockedValue";
            this.lblDateBlockedValue.Size = new System.Drawing.Size(462, 28);
            this.lblDateBlockedValue.TabIndex = 15;
            this.lblDateBlockedValue.Text = "12/21/2012";
            // 
            // lblDateToBeBlockedValue
            // 
            this.lblDateToBeBlockedValue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDateToBeBlockedValue.AutoSize = true;
            this.lblDateToBeBlockedValue.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "DateToBeBlocked", true));
            this.lblDateToBeBlockedValue.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblDateToBeBlockedValue.Location = new System.Drawing.Point(501, 452);
            this.lblDateToBeBlockedValue.Name = "lblDateToBeBlockedValue";
            this.lblDateToBeBlockedValue.Size = new System.Drawing.Size(462, 28);
            this.lblDateToBeBlockedValue.TabIndex = 17;
            this.lblDateToBeBlockedValue.Text = "12/21/2012";
            // 
            // tlpProductAttributes
            // 
            this.tlpProductAttributes.ColumnCount = 5;
            this.tlpProductAttributes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpProductAttributes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpProductAttributes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpProductAttributes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpProductAttributes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpProductAttributes.Controls.Add(this.gridProductAttributes, 0, 0);
            this.tlpProductAttributes.Controls.Add(this.btnPgUp, 0, 1);
            this.tlpProductAttributes.Controls.Add(this.btnUp, 1, 1);
            this.tlpProductAttributes.Controls.Add(this.btnDown, 3, 1);
            this.tlpProductAttributes.Controls.Add(this.btnPgDown, 4, 1);
            this.tlpProductAttributes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpProductAttributes.Location = new System.Drawing.Point(3, 343);
            this.tlpProductAttributes.Name = "tlpProductAttributes";
            this.tlpProductAttributes.RowCount = 2;
            this.tlpProductInformation.SetRowSpan(this.tlpProductAttributes, 7);
            this.tlpProductAttributes.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpProductAttributes.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpProductAttributes.Size = new System.Drawing.Size(462, 199);
            this.tlpProductAttributes.TabIndex = 1;
            // 
            // gridProductAttributes
            // 
            this.gridProductAttributes.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tlpProductAttributes.SetColumnSpan(this.gridProductAttributes, 5);
            this.gridProductAttributes.DataBindings.Add(new System.Windows.Forms.Binding("DataSource", this.bindingSource, "ProductAttributes", true));
            this.gridProductAttributes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridProductAttributes.Location = new System.Drawing.Point(3, 3);
            this.gridProductAttributes.MainView = this.gridView;
            this.gridProductAttributes.Name = "gridProductAttributes";
            this.gridProductAttributes.Size = new System.Drawing.Size(456, 128);
            this.gridProductAttributes.TabIndex = 0;
            this.gridProductAttributes.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView});
            // 
            // gridView
            // 
            this.gridView.Appearance.FooterPanel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridView.Appearance.FooterPanel.Options.UseFont = true;
            this.gridView.Appearance.FooterPanel.Options.UseTextOptions = true;
            this.gridView.Appearance.FooterPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridView.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridView.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridView.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.gridView.Appearance.Row.Options.UseFont = true;
            this.gridView.ColumnPanelRowHeight = 40;
            this.gridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colName,
            this.colValue});
            this.gridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.gridView.GridControl = this.gridProductAttributes;
            this.gridView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.gridView.Name = "gridView";
            this.gridView.OptionsBehavior.Editable = false;
            this.gridView.OptionsCustomization.AllowColumnMoving = false;
            this.gridView.OptionsCustomization.AllowColumnResizing = false;
            this.gridView.OptionsCustomization.AllowFilter = false;
            this.gridView.OptionsCustomization.AllowGroup = false;
            this.gridView.OptionsCustomization.AllowQuickHideColumns = false;
            this.gridView.OptionsCustomization.AllowSort = false;
            this.gridView.OptionsMenu.EnableColumnMenu = false;
            this.gridView.OptionsMenu.EnableFooterMenu = false;
            this.gridView.OptionsMenu.EnableGroupPanelMenu = false;
            this.gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView.OptionsSelection.EnableAppearanceHideSelection = false;
            this.gridView.OptionsView.ShowGroupPanel = false;
            this.gridView.OptionsView.ShowIndicator = false;
            this.gridView.RowHeight = 40;
            this.gridView.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.None;
            this.gridView.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.gridView.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.OnGridView_FocusedRowChanged);
            // 
            // colName
            // 
            this.colName.AppearanceCell.Options.UseTextOptions = true;
            this.colName.AppearanceCell.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.colName.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.colName.Caption = "Name";
            this.colName.FieldName = "Name";
            this.colName.MinWidth = 125;
            this.colName.Name = "colName";
            this.colName.Visible = true;
            this.colName.VisibleIndex = 0;
            this.colName.Width = 125;
            // 
            // colValue
            // 
            this.colValue.AppearanceCell.Options.UseTextOptions = true;
            this.colValue.AppearanceCell.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.colValue.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.colValue.Caption = "Value";
            this.colValue.FieldName = "RawValue";
            this.colValue.MinWidth = 125;
            this.colValue.Name = "colValue";
            this.colValue.Visible = true;
            this.colValue.VisibleIndex = 1;
            this.colValue.Width = 125;
            // 
            // btnPgUp
            // 
            this.btnPgUp.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPgUp.Appearance.Options.UseFont = true;
            this.btnPgUp.Image = global::Microsoft.Dynamics.Retail.Pos.Interaction.Properties.Resources.top;
            this.btnPgUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPgUp.Location = new System.Drawing.Point(4, 138);
            this.btnPgUp.Margin = new System.Windows.Forms.Padding(4);
            this.btnPgUp.Name = "btnPgUp";
            this.btnPgUp.Size = new System.Drawing.Size(57, 57);
            this.btnPgUp.TabIndex = 1;
            this.btnPgUp.Tag = "";
            this.btnPgUp.Text = "Ç";
            this.btnPgUp.Click += new System.EventHandler(this.OnBtnPgUp_Click);
            // 
            // btnUp
            // 
            this.btnUp.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnUp.Appearance.Options.UseFont = true;
            this.btnUp.Image = global::Microsoft.Dynamics.Retail.Pos.Interaction.Properties.Resources.up;
            this.btnUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnUp.Location = new System.Drawing.Point(69, 138);
            this.btnUp.Margin = new System.Windows.Forms.Padding(4);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(57, 57);
            this.btnUp.TabIndex = 2;
            this.btnUp.Tag = "";
            this.btnUp.Text = "ñ";
            this.btnUp.Click += new System.EventHandler(this.OnBtnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDown.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnDown.Appearance.Options.UseFont = true;
            this.btnDown.Image = global::Microsoft.Dynamics.Retail.Pos.Interaction.Properties.Resources.down;
            this.btnDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnDown.Location = new System.Drawing.Point(336, 138);
            this.btnDown.Margin = new System.Windows.Forms.Padding(4);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(57, 57);
            this.btnDown.TabIndex = 3;
            this.btnDown.Tag = "";
            this.btnDown.Text = "ò";
            this.btnDown.Click += new System.EventHandler(this.OnBtnDown_Click);
            // 
            // btnPgDown
            // 
            this.btnPgDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPgDown.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPgDown.Appearance.Options.UseFont = true;
            this.btnPgDown.Image = global::Microsoft.Dynamics.Retail.Pos.Interaction.Properties.Resources.bottom;
            this.btnPgDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPgDown.Location = new System.Drawing.Point(401, 138);
            this.btnPgDown.Margin = new System.Windows.Forms.Padding(4);
            this.btnPgDown.Name = "btnPgDown";
            this.btnPgDown.Size = new System.Drawing.Size(57, 57);
            this.btnPgDown.TabIndex = 4;
            this.btnPgDown.Tag = "";
            this.btnPgDown.Text = "Ê";
            this.btnPgDown.Click += new System.EventHandler(this.OnBtnPgDown_Click);
            // 
            // pbProductImage
            // 
            this.pbProductImage.BackColor = System.Drawing.Color.White;
            this.pbProductImage.DataBindings.Add(new System.Windows.Forms.Binding("Image", this.bindingSource, "Image", true));
            this.pbProductImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbProductImage.ErrorImage = global::Microsoft.Dynamics.Retail.Pos.Interaction.Properties.Resources.ProductUnavailable;
            this.pbProductImage.Image = global::Microsoft.Dynamics.Retail.Pos.Interaction.Properties.Resources.ProductUnavailable;
            this.pbProductImage.InitialImage = global::Microsoft.Dynamics.Retail.Pos.Interaction.Properties.Resources.ProductUnavailable;
            this.pbProductImage.Location = new System.Drawing.Point(3, 3);
            this.pbProductImage.Name = "pbProductImage";
            this.tlpProductInformation.SetRowSpan(this.pbProductImage, 9);
            this.pbProductImage.Size = new System.Drawing.Size(462, 278);
            this.pbProductImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbProductImage.TabIndex = 20;
            this.pbProductImage.TabStop = false;
            // 
            // ProductDetailsForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1024, 768);
            this.Controls.Add(this.tlpContent);
            this.HelpButton = false;
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "ProductDetailsForm";
            this.Text = "ProductDetailsForm";
            this.Controls.SetChildIndex(this.tlpContent, 0);
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.tlpContent.ResumeLayout(false);
            this.tlpContent.PerformLayout();
            this.tlpButtons.ResumeLayout(false);
            this.tlpProductInformation.ResumeLayout(false);
            this.tlpProductInformation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).EndInit();
            this.tlpProductAttributes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridProductAttributes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbProductImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.TableLayoutPanel tlpContent;
        private System.Windows.Forms.TableLayoutPanel tlpButtons;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnPgUp;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnUp;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnInventoryLookup;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnAddToSale;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCancel;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnPgDown;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnDown;
        private System.Windows.Forms.TableLayoutPanel tlpProductInformation;
        private System.Windows.Forms.Label lblBarCode;
        private System.Windows.Forms.Label lblBarCodeValue;
        private System.Windows.Forms.Label lblSearchName;
        private System.Windows.Forms.Label lblSearchNameValue;
        private System.Windows.Forms.Label lblCategory;
        private System.Windows.Forms.Label lblCategoryValue;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label lblDescriptionValue;
        private System.Windows.Forms.Label lblPrice;
        private System.Windows.Forms.Label lblIssueDate;
        private System.Windows.Forms.Label lblDateBlocked;
        private System.Windows.Forms.Label lblDateToBeBlocked;
        private System.Windows.Forms.Label lblProductAttributes;
        private System.Windows.Forms.Label lblPriceValue;
        private System.Windows.Forms.Label lblIssueDateValue;
        private System.Windows.Forms.Label lblDateBlockedValue;
        private System.Windows.Forms.Label lblDateToBeBlockedValue;
        private System.Windows.Forms.TableLayoutPanel tlpProductAttributes;
        private System.Windows.Forms.PictureBox pbProductImage;
        private System.Windows.Forms.BindingSource bindingSource;
        private DevExpress.XtraGrid.GridControl gridProductAttributes;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView;
        private DevExpress.XtraGrid.Columns.GridColumn colName;
        private DevExpress.XtraGrid.Columns.GridColumn colValue;
    }
}