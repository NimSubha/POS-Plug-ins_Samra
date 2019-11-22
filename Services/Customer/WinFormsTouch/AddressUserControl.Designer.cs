/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
namespace Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch
{
    partial class AddressUserControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.labelStreet = new System.Windows.Forms.Label();
            this.bindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.textBoxStreet = new DevExpress.XtraEditors.TextEdit();
            this.labelCity = new System.Windows.Forms.Label();
            this.textBoxCity = new DevExpress.XtraEditors.TextEdit();
            this.labelDistrict = new System.Windows.Forms.Label();
            this.labelOrganization = new System.Windows.Forms.Label();
            this.textBoxDistrict = new DevExpress.XtraEditors.TextEdit();
            this.textBoxOrgNumber = new DevExpress.XtraEditors.TextEdit();
            this.labelAddress = new System.Windows.Forms.Label();
            this.labelStreetNumber = new System.Windows.Forms.Label();
            this.textBoxStreetNumber = new DevExpress.XtraEditors.TextEdit();
            this.labelBuildingComplement = new System.Windows.Forms.Label();
            this.textBoxBuildingComplement = new DevExpress.XtraEditors.TextEdit();
            this.labelPostbox = new System.Windows.Forms.Label();
            this.textBoxPostbox = new DevExpress.XtraEditors.TextEdit();
            this.labelHouse = new System.Windows.Forms.Label();
            this.textBoxHouse = new DevExpress.XtraEditors.TextEdit();
            this.labelFlat = new System.Windows.Forms.Label();
            this.textBoxFlat = new DevExpress.XtraEditors.TextEdit();
            this.labelCountryOKSMCode = new System.Windows.Forms.Label();
            this.textBoxCountryOKSMCode = new DevExpress.XtraEditors.TextEdit();
            this.textBoxZip = new DevExpress.XtraEditors.TextEdit();
            this.labelZip = new System.Windows.Forms.Label();
            this.labelCounty = new System.Windows.Forms.Label();
            this.textBoxCounty = new DevExpress.XtraEditors.TextEdit();
            this.btnZip = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.textBoxState = new DevExpress.XtraEditors.TextEdit();
            this.textBoxCountry = new DevExpress.XtraEditors.TextEdit();
            this.labelCountry = new System.Windows.Forms.Label();
            this.labelState = new System.Windows.Forms.Label();
            this.btnCountry = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnState = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxStreet.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxCity.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxDistrict.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxOrgNumber.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxStreetNumber.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxBuildingComplement.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxPostbox.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxHouse.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxFlat.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxCountryOKSMCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxZip.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxCounty.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxState.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxCountry.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.AutoSize = true;
            this.tableLayoutPanel.ColumnCount = 3;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.Controls.Add(this.labelStreet, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.textBoxStreet, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.labelCity, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.textBoxCity, 0, 4);
            this.tableLayoutPanel.Controls.Add(this.labelDistrict, 0, 7);
            this.tableLayoutPanel.Controls.Add(this.labelOrganization, 0, 9);
            this.tableLayoutPanel.Controls.Add(this.textBoxDistrict, 0, 8);
            this.tableLayoutPanel.Controls.Add(this.textBoxOrgNumber, 0, 10);
            this.tableLayoutPanel.Controls.Add(this.labelAddress, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.labelStreetNumber, 0, 11);
            this.tableLayoutPanel.Controls.Add(this.textBoxStreetNumber, 0, 12);
            this.tableLayoutPanel.Controls.Add(this.labelBuildingComplement, 0, 13);
            this.tableLayoutPanel.Controls.Add(this.textBoxBuildingComplement, 0, 14);
            this.tableLayoutPanel.Controls.Add(this.labelPostbox, 0, 15);
            this.tableLayoutPanel.Controls.Add(this.textBoxPostbox, 0, 16);
            this.tableLayoutPanel.Controls.Add(this.labelHouse, 0, 17);
            this.tableLayoutPanel.Controls.Add(this.textBoxHouse, 0, 18);
            this.tableLayoutPanel.Controls.Add(this.labelFlat, 0, 19);
            this.tableLayoutPanel.Controls.Add(this.textBoxFlat, 0, 20);
            this.tableLayoutPanel.Controls.Add(this.labelCountryOKSMCode, 0, 21);
            this.tableLayoutPanel.Controls.Add(this.textBoxCountryOKSMCode, 0, 22);
            this.tableLayoutPanel.Controls.Add(this.textBoxZip, 0, 28);
            this.tableLayoutPanel.Controls.Add(this.labelZip, 0, 27);
            this.tableLayoutPanel.Controls.Add(this.labelCounty, 0, 5);
            this.tableLayoutPanel.Controls.Add(this.textBoxCounty, 0, 6);
            this.tableLayoutPanel.Controls.Add(this.btnZip, 2, 28);
            this.tableLayoutPanel.Controls.Add(this.textBoxState, 0, 26);
            this.tableLayoutPanel.Controls.Add(this.textBoxCountry, 0, 24);
            this.tableLayoutPanel.Controls.Add(this.labelCountry, 0, 23);
            this.tableLayoutPanel.Controls.Add(this.labelState, 0, 25);
            this.tableLayoutPanel.Controls.Add(this.btnCountry, 2, 24);
            this.tableLayoutPanel.Controls.Add(this.btnState, 2, 26);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 30;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(606, 885);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // labelStreet
            // 
            this.labelStreet.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelStreet.AutoSize = true;
            this.labelStreet.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "InputStreetName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.labelStreet.Location = new System.Drawing.Point(3, 26);
            this.labelStreet.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.labelStreet.Name = "labelStreet";
            this.labelStreet.Size = new System.Drawing.Size(53, 21);
            this.labelStreet.TabIndex = 0;
            this.labelStreet.Text = "Street:";
            // 
            // bindingSource
            // 
            this.bindingSource.DataSource = typeof(Microsoft.Dynamics.Retail.Pos.Customer.ViewModels.AddressViewModel);
            // 
            // textBoxStreet
            // 
            this.tableLayoutPanel.SetColumnSpan(this.textBoxStreet, 2);
            this.textBoxStreet.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "Street", true));
            this.textBoxStreet.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "InputStreetName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxStreet.Location = new System.Drawing.Point(3, 50);
            this.textBoxStreet.Name = "textBoxStreet";
            this.textBoxStreet.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textBoxStreet.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.textBoxStreet.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textBoxStreet.Properties.Appearance.Options.UseBackColor = true;
            this.textBoxStreet.Properties.Appearance.Options.UseFont = true;
            this.textBoxStreet.Properties.Appearance.Options.UseForeColor = true;
            this.textBoxStreet.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textBoxStreet.Properties.MaxLength = 250;
            this.textBoxStreet.Size = new System.Drawing.Size(537, 28);
            this.textBoxStreet.TabIndex = 1;
            // 
            // labelCity
            // 
            this.labelCity.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelCity.AutoSize = true;
            this.labelCity.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "InputCity", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.labelCity.Location = new System.Drawing.Point(3, 86);
            this.labelCity.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.labelCity.Name = "labelCity";
            this.labelCity.Size = new System.Drawing.Size(40, 21);
            this.labelCity.TabIndex = 2;
            this.labelCity.Text = "City:";
            // 
            // textBoxCity
            // 
            this.tableLayoutPanel.SetColumnSpan(this.textBoxCity, 2);
            this.textBoxCity.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "City", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxCity.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "InputCity", true));
            this.textBoxCity.Location = new System.Drawing.Point(3, 110);
            this.textBoxCity.Name = "textBoxCity";
            this.textBoxCity.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textBoxCity.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.textBoxCity.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textBoxCity.Properties.Appearance.Options.UseBackColor = true;
            this.textBoxCity.Properties.Appearance.Options.UseFont = true;
            this.textBoxCity.Properties.Appearance.Options.UseForeColor = true;
            this.textBoxCity.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textBoxCity.Properties.MaxLength = 60;
            this.textBoxCity.Size = new System.Drawing.Size(537, 28);
            this.textBoxCity.TabIndex = 2;
            // 
            // labelDistrict
            // 
            this.labelDistrict.AutoSize = true;
            this.labelDistrict.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "InputDistrictName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.labelDistrict.Location = new System.Drawing.Point(3, 206);
            this.labelDistrict.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.labelDistrict.Name = "labelDistrict";
            this.labelDistrict.Size = new System.Drawing.Size(105, 21);
            this.labelDistrict.TabIndex = 17;
            this.labelDistrict.Text = "District name:";
            // 
            // labelOrganization
            // 
            this.labelOrganization.AutoSize = true;
            this.labelOrganization.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "InputOrganizationNumber", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.labelOrganization.Location = new System.Drawing.Point(3, 266);
            this.labelOrganization.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.labelOrganization.Name = "labelOrganization";
            this.labelOrganization.Size = new System.Drawing.Size(162, 21);
            this.labelOrganization.TabIndex = 19;
            this.labelOrganization.Text = "Organization number:";
            // 
            // textBoxDistrict
            // 
            this.tableLayoutPanel.SetColumnSpan(this.textBoxDistrict, 2);
            this.textBoxDistrict.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "DistrictName", true));
            this.textBoxDistrict.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "InputDistrictName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxDistrict.Location = new System.Drawing.Point(3, 230);
            this.textBoxDistrict.Name = "textBoxDistrict";
            this.textBoxDistrict.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textBoxDistrict.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.textBoxDistrict.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textBoxDistrict.Properties.Appearance.Options.UseBackColor = true;
            this.textBoxDistrict.Properties.Appearance.Options.UseFont = true;
            this.textBoxDistrict.Properties.Appearance.Options.UseForeColor = true;
            this.textBoxDistrict.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textBoxDistrict.Properties.MaxLength = 30;
            this.textBoxDistrict.Size = new System.Drawing.Size(537, 28);
            this.textBoxDistrict.TabIndex = 4;
            // 
            // textBoxOrgNumber
            // 
            this.tableLayoutPanel.SetColumnSpan(this.textBoxOrgNumber, 2);
            this.textBoxOrgNumber.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "OrganizationNumber", true));
            this.textBoxOrgNumber.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "InputOrganizationNumber", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxOrgNumber.Location = new System.Drawing.Point(3, 290);
            this.textBoxOrgNumber.Name = "textBoxOrgNumber";
            this.textBoxOrgNumber.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textBoxOrgNumber.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.textBoxOrgNumber.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textBoxOrgNumber.Properties.Appearance.Options.UseBackColor = true;
            this.textBoxOrgNumber.Properties.Appearance.Options.UseFont = true;
            this.textBoxOrgNumber.Properties.Appearance.Options.UseForeColor = true;
            this.textBoxOrgNumber.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textBoxOrgNumber.Properties.MaxLength = 10;
            this.textBoxOrgNumber.Size = new System.Drawing.Size(537, 28);
            this.textBoxOrgNumber.TabIndex = 5;
            // 
            // labelAddress
            // 
            this.labelAddress.AutoSize = true;
            this.labelAddress.Location = new System.Drawing.Point(3, 0);
            this.labelAddress.Name = "labelAddress";
            this.labelAddress.Size = new System.Drawing.Size(78, 21);
            this.labelAddress.TabIndex = 23;
            this.labelAddress.Text = "ADDRESS";
            // 
            // labelStreetNumber
            // 
            this.labelStreetNumber.AutoSize = true;
            this.labelStreetNumber.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "InputStreetNumber", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.labelStreetNumber.Location = new System.Drawing.Point(3, 326);
            this.labelStreetNumber.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.labelStreetNumber.Name = "labelStreetNumber";
            this.labelStreetNumber.Size = new System.Drawing.Size(112, 21);
            this.labelStreetNumber.TabIndex = 21;
            this.labelStreetNumber.Text = "Street number:";
            // 
            // textBoxStreetNumber
            // 
            this.tableLayoutPanel.SetColumnSpan(this.textBoxStreetNumber, 2);
            this.textBoxStreetNumber.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "StreetNumber", true));
            this.textBoxStreetNumber.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "InputStreetNumber", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxStreetNumber.Location = new System.Drawing.Point(3, 350);
            this.textBoxStreetNumber.Name = "textBoxStreetNumber";
            this.textBoxStreetNumber.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textBoxStreetNumber.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.textBoxStreetNumber.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textBoxStreetNumber.Properties.Appearance.Options.UseBackColor = true;
            this.textBoxStreetNumber.Properties.Appearance.Options.UseFont = true;
            this.textBoxStreetNumber.Properties.Appearance.Options.UseForeColor = true;
            this.textBoxStreetNumber.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textBoxStreetNumber.Properties.MaxLength = 30;
            this.textBoxStreetNumber.Size = new System.Drawing.Size(537, 28);
            this.textBoxStreetNumber.TabIndex = 6;
            // 
            // labelBuildingComplement
            // 
            this.labelBuildingComplement.AutoSize = true;
            this.labelBuildingComplement.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "InputBuildingComplement", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.labelBuildingComplement.Location = new System.Drawing.Point(3, 386);
            this.labelBuildingComplement.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.labelBuildingComplement.Name = "labelBuildingComplement";
            this.labelBuildingComplement.Size = new System.Drawing.Size(161, 21);
            this.labelBuildingComplement.TabIndex = 21;
            this.labelBuildingComplement.Text = "Building complement:";
            // 
            // textBoxBuildingComplement
            // 
            this.tableLayoutPanel.SetColumnSpan(this.textBoxBuildingComplement, 2);
            this.textBoxBuildingComplement.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "BuildingComplement", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxBuildingComplement.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "InputBuildingComplement", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxBuildingComplement.Location = new System.Drawing.Point(3, 410);
            this.textBoxBuildingComplement.Name = "textBoxBuildingComplement";
            this.textBoxBuildingComplement.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textBoxBuildingComplement.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.textBoxBuildingComplement.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textBoxBuildingComplement.Properties.Appearance.Options.UseBackColor = true;
            this.textBoxBuildingComplement.Properties.Appearance.Options.UseFont = true;
            this.textBoxBuildingComplement.Properties.Appearance.Options.UseForeColor = true;
            this.textBoxBuildingComplement.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textBoxBuildingComplement.Properties.MaxLength = 30;
            this.textBoxBuildingComplement.Size = new System.Drawing.Size(537, 28);
            this.textBoxBuildingComplement.TabIndex = 7;
            // 
            // labelPostbox
            // 
            this.labelPostbox.AutoSize = true;
            this.labelPostbox.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "InputPostbox", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.labelPostbox.Location = new System.Drawing.Point(3, 446);
            this.labelPostbox.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.labelPostbox.Name = "labelPostbox";
            this.labelPostbox.Size = new System.Drawing.Size(67, 21);
            this.labelPostbox.TabIndex = 21;
            this.labelPostbox.Text = "Postbox:";
            // 
            // textBoxPostbox
            // 
            this.tableLayoutPanel.SetColumnSpan(this.textBoxPostbox, 2);
            this.textBoxPostbox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "Postbox", true));
            this.textBoxPostbox.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "InputPostbox", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxPostbox.Location = new System.Drawing.Point(3, 470);
            this.textBoxPostbox.Name = "textBoxPostbox";
            this.textBoxPostbox.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textBoxPostbox.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.textBoxPostbox.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textBoxPostbox.Properties.Appearance.Options.UseBackColor = true;
            this.textBoxPostbox.Properties.Appearance.Options.UseFont = true;
            this.textBoxPostbox.Properties.Appearance.Options.UseForeColor = true;
            this.textBoxPostbox.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textBoxPostbox.Properties.MaxLength = 30;
            this.textBoxPostbox.Size = new System.Drawing.Size(537, 28);
            this.textBoxPostbox.TabIndex = 8;
            // 
            // labelHouse
            // 
            this.labelHouse.AutoSize = true;
            this.labelHouse.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "InputHouse", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.labelHouse.Location = new System.Drawing.Point(3, 506);
            this.labelHouse.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.labelHouse.Name = "labelHouse";
            this.labelHouse.Size = new System.Drawing.Size(57, 21);
            this.labelHouse.TabIndex = 21;
            this.labelHouse.Text = "House:";
            // 
            // textBoxHouse
            // 
            this.tableLayoutPanel.SetColumnSpan(this.textBoxHouse, 2);
            this.textBoxHouse.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "House", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxHouse.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "InputHouse", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxHouse.Location = new System.Drawing.Point(3, 530);
            this.textBoxHouse.Name = "textBoxHouse";
            this.textBoxHouse.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textBoxHouse.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.textBoxHouse.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textBoxHouse.Properties.Appearance.Options.UseBackColor = true;
            this.textBoxHouse.Properties.Appearance.Options.UseFont = true;
            this.textBoxHouse.Properties.Appearance.Options.UseForeColor = true;
            this.textBoxHouse.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textBoxHouse.Properties.MaxLength = 30;
            this.textBoxHouse.Size = new System.Drawing.Size(537, 28);
            this.textBoxHouse.TabIndex = 9;
            // 
            // labelFlat
            // 
            this.labelFlat.AutoSize = true;
            this.labelFlat.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "InputFlat", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.labelFlat.Location = new System.Drawing.Point(3, 566);
            this.labelFlat.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.labelFlat.Name = "labelFlat";
            this.labelFlat.Size = new System.Drawing.Size(38, 21);
            this.labelFlat.TabIndex = 21;
            this.labelFlat.Text = "Flat:";
            // 
            // textBoxFlat
            // 
            this.tableLayoutPanel.SetColumnSpan(this.textBoxFlat, 2);
            this.textBoxFlat.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "Flat", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxFlat.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "InputFlat", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxFlat.Location = new System.Drawing.Point(3, 590);
            this.textBoxFlat.Name = "textBoxFlat";
            this.textBoxFlat.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textBoxFlat.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.textBoxFlat.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textBoxFlat.Properties.Appearance.Options.UseBackColor = true;
            this.textBoxFlat.Properties.Appearance.Options.UseFont = true;
            this.textBoxFlat.Properties.Appearance.Options.UseForeColor = true;
            this.textBoxFlat.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textBoxFlat.Properties.MaxLength = 30;
            this.textBoxFlat.Size = new System.Drawing.Size(537, 28);
            this.textBoxFlat.TabIndex = 10;
            // 
            // labelCountryOKSMCode
            // 
            this.labelCountryOKSMCode.AutoSize = true;
            this.labelCountryOKSMCode.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "InputCountryOKSMCode", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.labelCountryOKSMCode.Location = new System.Drawing.Point(3, 626);
            this.labelCountryOKSMCode.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.labelCountryOKSMCode.Name = "labelCountryOKSMCode";
            this.labelCountryOKSMCode.Size = new System.Drawing.Size(53, 21);
            this.labelCountryOKSMCode.TabIndex = 21;
            this.labelCountryOKSMCode.Text = "Street:";
            // 
            // textBoxCountryOKSMCode
            // 
            this.tableLayoutPanel.SetColumnSpan(this.textBoxCountryOKSMCode, 2);
            this.textBoxCountryOKSMCode.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "CountryOKSMCode", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxCountryOKSMCode.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "InputCountryOKSMCode", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxCountryOKSMCode.Location = new System.Drawing.Point(3, 650);
            this.textBoxCountryOKSMCode.Name = "textBoxCountryOKSMCode";
            this.textBoxCountryOKSMCode.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textBoxCountryOKSMCode.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.textBoxCountryOKSMCode.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textBoxCountryOKSMCode.Properties.Appearance.Options.UseBackColor = true;
            this.textBoxCountryOKSMCode.Properties.Appearance.Options.UseFont = true;
            this.textBoxCountryOKSMCode.Properties.Appearance.Options.UseForeColor = true;
            this.textBoxCountryOKSMCode.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textBoxCountryOKSMCode.Properties.MaxLength = 30;
            this.textBoxCountryOKSMCode.Size = new System.Drawing.Size(537, 28);
            this.textBoxCountryOKSMCode.TabIndex = 11;
            // 
            // textBoxZip
            // 
            this.textBoxZip.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel.SetColumnSpan(this.textBoxZip, 2);
            this.textBoxZip.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "Zip", true));
            this.textBoxZip.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "InputPostalCode", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxZip.Location = new System.Drawing.Point(5, 830);
            this.textBoxZip.Name = "textBoxZip";
            this.textBoxZip.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textBoxZip.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.textBoxZip.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textBoxZip.Properties.Appearance.Options.UseBackColor = true;
            this.textBoxZip.Properties.Appearance.Options.UseFont = true;
            this.textBoxZip.Properties.Appearance.Options.UseForeColor = true;
            this.textBoxZip.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textBoxZip.Properties.ReadOnly = true;
            this.textBoxZip.Size = new System.Drawing.Size(537, 28);
            this.textBoxZip.TabIndex = 17;
            // 
            // labelZip
            // 
            this.labelZip.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelZip.AutoSize = true;
            this.labelZip.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "InputPostalCode", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.labelZip.Location = new System.Drawing.Point(3, 806);
            this.labelZip.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.labelZip.Name = "labelZip";
            this.labelZip.Size = new System.Drawing.Size(119, 21);
            this.labelZip.TabIndex = 7;
            this.labelZip.Text = "ZIP/Postal code:";
            // 
            // labelCounty
            // 
            this.labelCounty.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelCounty.AutoSize = true;
            this.labelCounty.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "InputCounty", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.labelCounty.Location = new System.Drawing.Point(3, 146);
            this.labelCounty.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.labelCounty.Name = "labelCounty";
            this.labelCounty.Size = new System.Drawing.Size(63, 21);
            this.labelCounty.TabIndex = 2;
            this.labelCounty.Text = "County:";
            // 
            // textBoxCounty
            // 
            this.tableLayoutPanel.SetColumnSpan(this.textBoxCounty, 2);
            this.textBoxCounty.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "County", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxCounty.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "InputCounty", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxCounty.Location = new System.Drawing.Point(3, 170);
            this.textBoxCounty.Name = "textBoxCounty";
            this.textBoxCounty.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textBoxCounty.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.textBoxCounty.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textBoxCounty.Properties.Appearance.Options.UseBackColor = true;
            this.textBoxCounty.Properties.Appearance.Options.UseFont = true;
            this.textBoxCounty.Properties.Appearance.Options.UseForeColor = true;
            this.textBoxCounty.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textBoxCounty.Properties.MaxLength = 60;
            this.textBoxCounty.Size = new System.Drawing.Size(537, 28);
            this.textBoxCounty.TabIndex = 3;
            // 
            // btnZip
            // 
            this.btnZip.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnZip.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F);
            this.btnZip.Appearance.Options.UseFont = true;
            this.btnZip.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "InputPostalCode", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.btnZip.Image = global::Microsoft.Dynamics.Retail.Pos.Customer.Properties.Resources.search;
            this.btnZip.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnZip.Location = new System.Drawing.Point(548, 829);
            this.btnZip.Margin = new System.Windows.Forms.Padding(1);
            this.btnZip.MaximumSize = new System.Drawing.Size(57, 30);
            this.btnZip.Name = "btnZip";
            this.btnZip.Padding = new System.Windows.Forms.Padding(0);
            this.btnZip.Size = new System.Drawing.Size(57, 30);
            this.btnZip.TabIndex = 16;
            this.btnZip.Text = "$";
            this.btnZip.Click += new System.EventHandler(this.OnZipButton_Click);
            // 
            // textBoxState
            // 
            this.textBoxState.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel.SetColumnSpan(this.textBoxState, 2);
            this.textBoxState.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "State", true));
            this.textBoxState.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "InputState", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxState.Location = new System.Drawing.Point(5, 770);
            this.textBoxState.Name = "textBoxState";
            this.textBoxState.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textBoxState.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.textBoxState.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textBoxState.Properties.Appearance.Options.UseBackColor = true;
            this.textBoxState.Properties.Appearance.Options.UseFont = true;
            this.textBoxState.Properties.Appearance.Options.UseForeColor = true;
            this.textBoxState.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textBoxState.Properties.ReadOnly = true;
            this.textBoxState.Size = new System.Drawing.Size(537, 28);
            this.textBoxState.TabIndex = 13;
            // 
            // textBoxCountry
            // 
            this.textBoxCountry.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel.SetColumnSpan(this.textBoxCountry, 2);
            this.textBoxCountry.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "Country", true));
            this.textBoxCountry.Location = new System.Drawing.Point(5, 710);
            this.textBoxCountry.Name = "textBoxCountry";
            this.textBoxCountry.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textBoxCountry.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.textBoxCountry.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textBoxCountry.Properties.Appearance.Options.UseBackColor = true;
            this.textBoxCountry.Properties.Appearance.Options.UseFont = true;
            this.textBoxCountry.Properties.Appearance.Options.UseForeColor = true;
            this.textBoxCountry.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textBoxCountry.Properties.ReadOnly = true;
            this.textBoxCountry.Size = new System.Drawing.Size(537, 28);
            this.textBoxCountry.TabIndex = 15;
            // 
            // labelCountry
            // 
            this.labelCountry.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelCountry.AutoSize = true;
            this.labelCountry.Location = new System.Drawing.Point(3, 686);
            this.labelCountry.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.labelCountry.Name = "labelCountry";
            this.labelCountry.Size = new System.Drawing.Size(124, 21);
            this.labelCountry.TabIndex = 10;
            this.labelCountry.Text = "Country/Region:";
            // 
            // labelState
            // 
            this.labelState.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelState.AutoSize = true;
            this.labelState.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "InputState", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.labelState.Location = new System.Drawing.Point(3, 746);
            this.labelState.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.labelState.Name = "labelState";
            this.labelState.Size = new System.Drawing.Size(47, 21);
            this.labelState.TabIndex = 4;
            this.labelState.Text = "State:";
            // 
            // btnCountry
            // 
            this.btnCountry.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCountry.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F);
            this.btnCountry.Appearance.Options.UseFont = true;
            this.btnCountry.Image = global::Microsoft.Dynamics.Retail.Pos.Customer.Properties.Resources.search;
            this.btnCountry.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnCountry.Location = new System.Drawing.Point(548, 709);
            this.btnCountry.Margin = new System.Windows.Forms.Padding(1);
            this.btnCountry.MaximumSize = new System.Drawing.Size(57, 30);
            this.btnCountry.Name = "btnCountry";
            this.btnCountry.Padding = new System.Windows.Forms.Padding(0);
            this.btnCountry.Size = new System.Drawing.Size(57, 30);
            this.btnCountry.TabIndex = 14;
            this.btnCountry.Text = "$";
            this.btnCountry.Click += new System.EventHandler(this.OnCountryButton_Click);
            // 
            // btnState
            // 
            this.btnState.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnState.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F);
            this.btnState.Appearance.Options.UseFont = true;
            this.btnState.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "InputState", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.btnState.Image = global::Microsoft.Dynamics.Retail.Pos.Customer.Properties.Resources.search;
            this.btnState.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnState.Location = new System.Drawing.Point(548, 769);
            this.btnState.Margin = new System.Windows.Forms.Padding(1);
            this.btnState.MaximumSize = new System.Drawing.Size(57, 30);
            this.btnState.Name = "btnState";
            this.btnState.Padding = new System.Windows.Forms.Padding(0);
            this.btnState.Size = new System.Drawing.Size(57, 30);
            this.btnState.TabIndex = 12;
            this.btnState.Text = "$";
            this.btnState.Click += new System.EventHandler(this.OnStateButton_Click);
            // 
            // AddressUserControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.tableLayoutPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "AddressUserControl";
            this.Size = new System.Drawing.Size(606, 885);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxStreet.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxCity.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxDistrict.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxOrgNumber.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxStreetNumber.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxBuildingComplement.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxPostbox.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxHouse.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxFlat.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxCountryOKSMCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxZip.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxCounty.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxState.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxCountry.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label labelStreet;
        private System.Windows.Forms.Label labelCity;
        private System.Windows.Forms.Label labelState;
        private System.Windows.Forms.Label labelZip;
        private System.Windows.Forms.Label labelCountry;
        private DevExpress.XtraEditors.TextEdit textBoxStreet;
        private DevExpress.XtraEditors.TextEdit textBoxCity;
        private DevExpress.XtraEditors.TextEdit textBoxState;
        private DevExpress.XtraEditors.TextEdit textBoxZip;
        private DevExpress.XtraEditors.TextEdit textBoxCountry;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnState;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnZip;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCountry;
        private System.Windows.Forms.Label labelDistrict;
        private System.Windows.Forms.Label labelOrganization;
        private DevExpress.XtraEditors.TextEdit textBoxDistrict;
        private DevExpress.XtraEditors.TextEdit textBoxOrgNumber;
        private System.Windows.Forms.BindingSource bindingSource;
        private System.Windows.Forms.Label labelAddress;
        private System.Windows.Forms.Label labelStreetNumber;
        private DevExpress.XtraEditors.TextEdit textBoxStreetNumber;
        private System.Windows.Forms.Label labelBuildingComplement;
        private DevExpress.XtraEditors.TextEdit textBoxBuildingComplement;
        private System.Windows.Forms.Label labelPostbox;
        private DevExpress.XtraEditors.TextEdit textBoxPostbox;
        private System.Windows.Forms.Label labelHouse;
        private DevExpress.XtraEditors.TextEdit textBoxHouse;
        private System.Windows.Forms.Label labelFlat;
        private DevExpress.XtraEditors.TextEdit textBoxFlat;
        private System.Windows.Forms.Label labelCountryOKSMCode;
        private DevExpress.XtraEditors.TextEdit textBoxCountryOKSMCode;
        private System.Windows.Forms.Label labelCounty;
        private DevExpress.XtraEditors.TextEdit textBoxCounty;
    }
}
