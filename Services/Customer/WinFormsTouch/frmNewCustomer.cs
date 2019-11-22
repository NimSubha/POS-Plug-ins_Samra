/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using LSRetailPosis.DevUtilities;
using LSRetailPosis.POSProcesses;
using LSRetailPosis.Settings;
using LSRetailPosis.Settings.FunctionalityProfiles;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Customer.ViewModels;
using DM = Microsoft.Dynamics.Retail.Pos.DataManager;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Dynamics.Retail.Pos;
using CS = Microsoft.Dynamics.Retail.Pos.Customer.Customer;
using System.Text.RegularExpressions;


namespace Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch
{
    partial class frmNewCustomer:frmTouchBase
    {
        private CustomerViewModel viewModel;
        private AddressViewModel addressViewModel;
        string sISDCode = string.Empty;
        private const string FilterFormat =
            @"[OrderDate] LIKE '%{0}%' 
            OR [OrderNumber] LIKE '%{0}%' 
            OR [StoreName] LIKE '%{0}%' 
            OR [OrderStatus] LIKE '%{0}%' 
            OR [ItemName] LIKE '%{0}%' 
            OR [ItemQuantity] LIKE '%{0}%' 
            OR [ItemAmount] LIKE '%{0}%'";

        #region Nimbus

        DataTable dtSalutation;
        DataTable dtGender;
        Int64  iSalutation = 5; //other
        int iGender = 0;


        string sISDC = string.Empty;
        string sCustAgeBracket = string.Empty;
        string sNationality = string.Empty;
        string sCustClassGrp = string.Empty;
        string sCustTaxGrp = "";
        //===================Soutik==============================
        string sCustCityGrp = string.Empty;

        #endregion

        public frmNewCustomer(ICustomer newCustomer, IAddress newAddress)
        {
            InitializeComponent();

            // set and bind the VM for the customer fields
            this.viewModel = new CustomerViewModel(newCustomer);
            this.bindingSource.Add(this.viewModel);

            // set the VM for the address control
            addressViewModel = new AddressViewModel(newAddress, newCustomer);
            this.viewAddressUserControl1.SetProperties(newCustomer, addressViewModel);
            this.viewAddressUserControl1.SetEditable(true);

            this.gridOrders.DataBindings.Add("DataSource", this.bindingSource, "Items");

            if(Functions.CountryRegion != SupportedCountryRegion.BR)
            {
                labelCpfCnpjNumber.Visible = false;
                textBoxCpfCnpjNumber.Visible = false;
            }

            // Set formatter to convert sales status enum to text
            this.columnOrderStatus.DisplayFormat.Format = new SalesOrderStatusFormatter();

            #region Nimbus

            #region Salutation
            //dtSalutation = new DataTable();

            //dtSalutation.Columns.Add("Name", typeof(String));
            //dtSalutation.Columns.Add("Value", typeof(int));

            //dtSalutation.Rows.Add("HE", 0);
            //dtSalutation.Rows.Add("HH", 1);
            //dtSalutation.Rows.Add("M/S", 2);
            //dtSalutation.Rows.Add("Mr.", 3);
            //dtSalutation.Rows.Add("Mrs.", 4);
            //dtSalutation.Rows.Add("Ms.", 5);
            //dtSalutation.Rows.Add("Sayyid", 6);
            //dtSalutation.Rows.Add("Sayyida", 7);
            //dtSalutation.Rows.Add("Sheikh", 8);
            //dtSalutation.Rows.Add("Sheikha", 9);
            //dtSalutation.AcceptChanges();
            #endregion

            #region Gender

            dtGender = new DataTable();
            dtGender.Columns.Add("Name", typeof(String));
            dtGender.Columns.Add("Value", typeof(int));

            dtGender.Rows.Add("Unknown", 0);
            dtGender.Rows.Add("Male", 1);
            dtGender.Rows.Add("Female", 2);
            dtGender.AcceptChanges();

            #endregion


            #endregion
        }

        public ICustomer Customer
        {
            get { return this.viewModel.Customer; }
        }
        public IAddress Address
        {
            get { return addressViewModel.Address; }
        }

        protected override void OnLoad(EventArgs e)
        {
            if(!this.DesignMode)
            {
                TranslateLabels();
                SetTextBoxFocus();
                SetRadioButtonsEnabledState();
                cmbBMonth.DataSource = Enum.GetValues(typeof(MonthsOfYear));
                cmbAnnMonth.DataSource = Enum.GetValues(typeof(MonthsOfYear));
            }

            base.OnLoad(e);
        }

        private void SetRadioButtonsEnabledState()
        {
            // enable the customer type radio buttons only if a new customer 
            this.radioOrg.Enabled = this.viewModel.Customer.IsEmptyCustomer();
            this.radioPerson.Enabled = this.viewModel.Customer.IsEmptyCustomer();
        }

        private void TranslateLabels()
        {
            lblHeader.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(56329); // customer information:
            labelName.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51129); // Name:
            labelFirstName.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51166); // First Name:
            labelMiddleName.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51167); // Middle Name:
            labelLastName.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51168); // Last Name:
            labelGroup.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51124); // Customer group:
            labelCurrency.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51125); // Currency:
            labelLanguage.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51137); // Language:

            labelEmail.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51138); // E-mail:
            labelReceiptEmail.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51128); // Receipt e-mail:
            //labelWebSite.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51127); // Web site:
            labelCpfCnpjNumber.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51207); // CPF / CNPJ:

            labelDateCreated.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51214); // Date created:
            labelTotalVisits.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51213); // Total visits:
            labelSearch.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51209); // Sales orders:
            labelStoreLastVisited.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51211); // Store last visited:
            labelTotalSales.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51210); // Total sales:
            labelDateLastVisit.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51212); // Date of last visit:

            tabPageContact.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51216); // Customer details
            tabPageHistory.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51215); // Transaction history

            columnAmount.Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(51217); // Amount
            columnDate.Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(51223); // Date
            columnItem.Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(51220); // Item
            columnOrderNumber.Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(51222); // Order number
            columnOrderStatus.Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(51224); // Order status
            columnQuantity.Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(51219); // Quantity
            columnStore.Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(51221); // Store

            labelType.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51169); // Type:
            radioOrg.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51165); // Organization
            radioPerson.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51164); // Person

            btnCancel.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(56319); // Cancel
            btnSave.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(56318); // Save
            btnClear.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51149); // Clear

            this.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51041); // Customer
        }

        #region events
        protected override void OnHelpRequested(HelpEventArgs hevent)
        {
            if(hevent == null)
                throw new ArgumentNullException("hevent");

            LSRetailPosis.POSControls.POSFormsManager.ShowHelp(this);

            hevent.Handled = true;
            base.OnHelpRequested(hevent);
        }

        private void OnCustomerGroup_Click(object sender, EventArgs e)
        {
            this.viewModel.ExecuteSelectGroup();
            textBoxGroup.Text = this.viewModel.CustomerGroup;
        }

        private void OnCurrency_Click(object sender, EventArgs e)
        {
            this.viewModel.ExecuteSelectCurrency();
        }

        private void OnLanguage_Click(object sender, EventArgs e)
        {
            this.viewModel.ExecuteSelectLanguage();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.viewModel.ExecuteClear();
            this.addressViewModel.ExecuteClear();
            // sISDC = GetCompanyISDCode(newAddress.Country); // added on 01/09/2014
            // ---- Added on 12.08.2013
            txtSalutation.Text = string.Empty;
            txtSTD.Text = sISDCode;
            txtMobilePrimary.Text = sISDCode;
            txtMobileSecondary.Text = sISDCode;
            txtGender.Text = string.Empty;
            //dtDOB.EditValue = null;  
            //dtMarriage.EditValue = null;

            txtCustClassificationGroup.Text = string.Empty;
            txtOccupation.Text = string.Empty;
            txtSalutation.Focus();
            // -----------
            textNationality.Text = string.Empty;
            textCustAgeBracket.Text = string.Empty;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        protected override void OnClosing(CancelEventArgs e)
        {
            if(this.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                if(!this.SaveCustomer())
                {
                    e.Cancel = true;
                }
            }
            base.OnClosing(e);
        }

        #endregion

        private void SetSearchFilter(string p)
        {
            string filter = string.Empty;

            if(!string.IsNullOrWhiteSpace(p))
            {
                filter = string.Format(FilterFormat, p);
            }
            this.gridView.ActiveFilterString = filter;
        }

        private void SetTextBoxFocus()
        {
            if(this.viewModel.IsPerson)
            {
                //Person
                textBoxFirstName.Select();
            }
            else
            {
                //Organization
                textBoxName.Select();
                
            }
        }

        private bool SaveCustomer()
        {
            try
            {
                bool createdLocal = false;
                bool createdAx = false;
                string comment = null;

                string sReligion = null;
                string sCustClassificationFGrp = null;
                

                sReligion = "";
                sCustClassificationFGrp = txtCustClassificationGroup.Text;

                DialogResult prompt = Pos.Customer.Customer.InternalApplication.Services.Dialog.ShowMessage(51148, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if(prompt == System.Windows.Forms.DialogResult.Yes)
                {
                    IList<Int64> entityKeys = new List<Int64>();
                    ICustomer tempCustomer = MergeAddress(this.viewModel.Customer, this.addressViewModel.Address);
                    bool isEmptyCustomer = this.viewModel.Customer.IsEmptyCustomer();
                    sCustTaxGrp = this.addressViewModel.SalesTaxGroup;//added on 300318

                    // this.isEmptyCustomer is initialized at form load and uses the incoming customer object
                    if(isEmptyCustomer)
                    {

                        if(ValidateControls()) // nimbus
                            // Attempt to save in AX
                            Pos.Customer.Customer.InternalApplication.TransactionServices.NewCustomer(ref createdAx, ref comment, ref tempCustomer, ApplicationSettings.Terminal.StorePrimaryId, ref entityKeys);

                        tempCustomer.SalesTaxGroup = sCustTaxGrp;//added on 300318

                        #region Nimbus
                        if(createdAx)
                        {
                            //UpdateRetailCustomerInfo
                            //  ReadOnlyCollection<object> containerArray;

                            //DateTime dBirthDate = Convert.ToDateTime("1900/01/01 00:00:00.000");
                            //DateTime dtMarriageDate = Convert.ToDateTime("1900/01/01 00:00:00.000");

                            //if (dtDOB.EditValue == null)
                            //{
                            //    dtDOB.EditValue = dBirthDate;
                            //}
                            //if (dtMarriage.EditValue == null)
                            //{
                            //    dtMarriage.EditValue = dtMarriageDate;
                            //}
                            int intRes = 0;
                            if(chkResidence.Checked)// added on 07/12/2015 req by K.Saha
                                intRes = 1;
                            else
                                intRes = 0;
                            string sCustId = tempCustomer.CustomerId;
                            string sStoreId=ApplicationSettings.Database.StoreID;
                            Pos.Customer.Customer.InternalApplication.TransactionServices.InvokeExtension("UpdateRetailCustomerInfo",
                                                                                                      sCustId, iSalutation, iGender, //dtDOB.EditValue,
                                                                                                      sReligion.Trim(), txtOccupation.Text.Trim(),// dtMarriage.EditValue,
                                                                                                      txtSTD.Text.Trim(), txtMobilePrimary.Text.Trim(), txtMobileSecondary.Text.Trim(),
                                                                                                      textNationality.Text.Trim(), textCustAgeBracket.Text.Trim(), cmbBMonth.SelectedIndex,
                                                                                                      txtBDay.Text.Trim(), txtBYear.Text.Trim(), cmbAnnMonth.SelectedIndex, txtAnnDay.Text.Trim(),
                                                                                                      txtAnnYear.Text.Trim(), sStoreId, intRes,sCustTaxGrp, txtCustClassificationGroup.Text.Trim() // added this 2 field RH on 05/11/2014
                                                                                                     );
                        }
                        #endregion

                    }
                    else
                    {
                        Pos.Customer.Customer.UpdateCustomer(ref createdAx, ref comment, ref tempCustomer, ref entityKeys);
                    }

                    // Was the customer created in AX
                    if(createdAx)
                    {
                        // Was the customer created locally
                        DM.CustomerDataManager customerDataManager = new DM.CustomerDataManager(
                            ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID);

                        LSRetailPosis.Transaction.Customer transactionalCustomer = tempCustomer as LSRetailPosis.Transaction.Customer;

                        if(isEmptyCustomer)
                        {
                            createdLocal = customerDataManager.SaveTransactionalCustomer(transactionalCustomer, entityKeys);
                        }
                        else
                        {
                            createdLocal = customerDataManager.UpdateTransactionalCustomer(transactionalCustomer, entityKeys);
                        }

                        //Update the VM
                        this.viewModel = new CustomerViewModel(tempCustomer);

                        #region Nimbus

                         UpdateCustomerInfo(tempCustomer.CustomerId); // blocked on 12.08.2013

                        #endregion
                    }

                    if(!createdAx)
                    {
                        Pos.Customer.Customer.InternalApplication.Services.Dialog.ShowMessage(51159, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if(!createdLocal)
                    {
                        Pos.Customer.Customer.InternalApplication.Services.Dialog.ShowMessage(51156, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                return createdAx && createdLocal;
            }
            catch(Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                Pos.Customer.Customer.InternalApplication.Services.Dialog.ShowMessage(51158, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private static ICustomer MergeAddress(ICustomer iCustomer, IAddress iAddress)
        {
            if(iCustomer != null && iAddress != null)
            {
                iCustomer.AddressComplement = iAddress.BuildingComplement;
                iCustomer.City = iAddress.City;
                iCustomer.Country = iAddress.Country;
                iCustomer.County = iAddress.County;
                iCustomer.DistrictName = iAddress.DistrictName;
                iCustomer.Extension = iAddress.Extension;
                iCustomer.OrgId = iAddress.OrgId;
                iCustomer.PostalCode = iAddress.PostalCode;
                iCustomer.State = iAddress.State;
                iCustomer.StreetName = iAddress.StreetName;
                iCustomer.AddressNumber = iAddress.StreetNumber;
                iCustomer.Address = Utility.JoinStrings(" ", iCustomer.StreetName, iCustomer.City, iCustomer.State, iCustomer.PostalCode, iCustomer.Country);
                iCustomer.PrimaryAddress.AddressType = iAddress.AddressType;
                iCustomer.PrimaryAddress.Name = iAddress.Name;
                iCustomer.PrimaryAddress.Email = iAddress.Email;
                iCustomer.PrimaryAddress.Telephone = iAddress.Telephone;
                iCustomer.PrimaryAddress.URL = iAddress.URL;
                iCustomer.PrimaryAddress.SalesTaxGroup = iAddress.SalesTaxGroup;

                switch(iCustomer.RelationType)
                {
                    case RelationType.Person:
                        // send party name in fixed format
                        string name = Utility.JoinStrings(" ", iCustomer.FirstName, iCustomer.MiddleName, iCustomer.LastName);

                        if(!string.IsNullOrWhiteSpace(name))
                        {
                            iCustomer.Name = name;
                        }
                        break;
                    default:
                        // No change
                        break;
                }
            }
            return iCustomer;
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            SetSearchFilter(this.textSearch.Text.Trim());
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            SetSearchFilter(string.Empty);
            this.textSearch.Text = string.Empty;
        }

        private void tabControlParent_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if(e.Page == this.tabPageHistory)
            {
                this.BeginInvoke(new Action(this.viewModel.ExecuteLoadHistory));
            }
        }

        private void btnPgUp_Click(object sender, EventArgs e)
        {
            gridView.MovePrevPage();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            gridView.MovePrev();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            gridView.MoveNext();
        }

        private void btnPgDown_Click(object sender, EventArgs e)
        {
            gridView.MoveNextPage();
        }

        private void textSearch_Enter(object sender, EventArgs e)
        {
            this.AcceptButton = buttonSearch;
        }

        private void textSearch_Leave(object sender, EventArgs e)
        {
            this.AcceptButton = null;
        }

        #region Nimbus

        private void UpdateCustomerInfo(string sCustomerId)
        {
            SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            DateTime dtMarrage = Convert.ToDateTime("01-01-1900");
            DateTime dtDOB = Convert.ToDateTime("01-01-1900");

            //if (application != null)
            //    connection = application.Settings.Database.Connection;
            //else
            //    connection = ApplicationSettings.Database.LocalConnection;

            string commandText = " UPDATE CUSTTABLE SET RETAILSALUTATION=@RETAILSALUTATION,RETAILGENDER=@RETAILGENDER," +
                                 " RETAILDOB=@RETAILDOB,RELIGION=@RELIGION,OCCUPATION = @OCCUPATION, RETAILMARRIAGEDATE = @RETAILMARRIAGEDATE," +
                                 " RETAILSTDCODE =@RETAILSTDCODE, RETAILMOBILEPRIMARY = @RETAILMOBILEPRIMARY," +
                                 " RETAILMOBILESECONDARY=@RETAILMOBILESECONDARY,CustClassificationId=@CustClassificationId, "+
                                 " TAXGROUP=@TAXGROUP WHERE ACCOUNTNUM = '" + sCustomerId + "' ";

            SqlCommand command = new SqlCommand(commandText, SqlCon);

            SqlCon.Open();

            command.Parameters.Clear();
            command.Parameters.Add("@RETAILSALUTATION", SqlDbType.BigInt).Value = iSalutation;
            command.Parameters.Add("@RETAILGENDER", SqlDbType.Int).Value = iGender;
            command.Parameters.Add("@RETAILDOB", SqlDbType.DateTime).Value = dtDOB;
            command.Parameters.Add("@RELIGION", SqlDbType.NVarChar, 20).Value = "";
            command.Parameters.Add("@OCCUPATION", SqlDbType.NVarChar, 60).Value = txtOccupation.Text.Trim();
            command.Parameters.Add("@RETAILMARRIAGEDATE", SqlDbType.DateTime).Value = dtMarrage;

            command.Parameters.Add("@RETAILSTDCODE", SqlDbType.NVarChar, 10).Value = txtSTD.Text.Trim();
            command.Parameters.Add("@RETAILMOBILEPRIMARY", SqlDbType.NVarChar, 20).Value = txtMobilePrimary.Text.Trim();
            command.Parameters.Add("@RETAILMOBILESECONDARY", SqlDbType.NVarChar, 20).Value = txtMobileSecondary.Text.Trim();
            command.Parameters.Add("@CustClassificationId", SqlDbType.NVarChar, 20).Value = txtCustClassificationGroup.Text.Trim();
            command.Parameters.Add("@TAXGROUP", SqlDbType.NVarChar, 20).Value = sCustTaxGrp; 

            command.ExecuteNonQuery();
            SqlCon.Close();
        }

        private void GetCompanyISDCode(string sCountryCode = "")// added on 01/09/2014
        {

            SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
            string sQry = string.Empty;
            if(!string.IsNullOrEmpty(sCountryCode))
            {
                sQry = " SELECT ISNULL(ISDCODE,'') AS ISDCODE FROM LOGISTICSADDRESSCOUNTRYREGION" +
                        "  WHERE COUNTRYREGIONID='" + sCountryCode + "' ";
            }
            else
            {
                sQry = "  DECLARE @INVENTLOCATION VARCHAR(20)" +
                            "  SELECT @INVENTLOCATION=RETAILCHANNELTABLE.RECID FROM  " +
                            "  RETAILCHANNELTABLE INNER JOIN  RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID  " +
                            "  WHERE STORENUMBER='" + ApplicationSettings.Database.StoreID + "' " +
                            "  SELECT ISNULL(ISDCODE,'') AS ISDCODE FROM LOGISTICSADDRESSCOUNTRYREGION " +
                            "  WHERE COUNTRYREGIONID =(SELECT top 1 COUNTRYREGIONID  " +
                            "  FROM [DBO].[LOGISTICSPOSTALADDRESSVIEW] WHERE  CAST(LOCATION AS NVARCHAR(20)) =@INVENTLOCATION)";
            }

            SqlCommand cmd = new SqlCommand(sQry, SqlCon);
            cmd.CommandTimeout = 0;
            if(SqlCon.State == ConnectionState.Closed)
                SqlCon.Open();
            sISDCode = Convert.ToString(cmd.ExecuteScalar());

            if(SqlCon.State == ConnectionState.Open)
                SqlCon.Close();

            txtSTD.Text = string.Empty;
            txtSTD.Text = sISDCode;

            if(txtMobilePrimary.Text.Trim().Length < 5) // added on 27/06/2014 req by S.Sharma
            {
                txtMobilePrimary.Text = string.Empty;
                txtMobileSecondary.Text = string.Empty;

                txtMobilePrimary.Text = sISDCode;
                txtMobileSecondary.Text = sISDCode;
            }
        }

        private void GetDefSalesTaxGroupCode()// added on 28/12/2017
        {
            SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
            string sQry = string.Empty;

            sQry = " SELECT top 1 ISNULL(TAXGROUP,'') AS TAXGROUP FROM TAXGROUPHEADING" +//DATAAREAID='mms'  and DEFAULTTAXGROUP=1
                    "  WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "' and DEFAULTTAXGROUP=1";


            SqlCommand cmd = new SqlCommand(sQry, SqlCon);
            cmd.CommandTimeout = 0;
            if (SqlCon.State == ConnectionState.Closed)
                SqlCon.Open();
            string sCode = Convert.ToString(cmd.ExecuteScalar());

            if (SqlCon.State == ConnectionState.Open)
                SqlCon.Close();

            this.addressViewModel.SalesTaxGroup = string.Empty;
            this.addressViewModel.SalesTaxGroup = sCode;

        }

        private void GetDefCustomerGroupCode()
        {
            SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
            string sQry = string.Empty;

            sQry = " SELECT ISNULL(DEFAULTCUSTOMERGROUP,'') AS DEFAULTCUSTOMERGROUP FROM RETAILPARAMETERS" +
                    "  WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'";

            //txtCustClassificationGroup.Text = sCustClassGrp;//RETAILPARAMETERS ADD DEFAULTCUSTOMERGROUP
            SqlCommand cmd = new SqlCommand(sQry, SqlCon);
            cmd.CommandTimeout = 0;
            if (SqlCon.State == ConnectionState.Closed)
                SqlCon.Open();
            string sCode = Convert.ToString(cmd.ExecuteScalar());

            if (SqlCon.State == ConnectionState.Open)
                SqlCon.Close();

            this.viewModel.CustomerGroup = string.Empty;
            this.viewModel.CustomerGroup = sCode;

            //textBoxGroup.Text = string.Empty;
            //textBoxGroup.Text = sCode;
        }

        #endregion

        private void btnSalutation_Click(object sender, EventArgs e)
        {
            //Dialog.WinFormsTouch.frmGenericLookup oLook = new Dialog.WinFormsTouch.frmGenericLookup(CustBalDt, 3, "");
            BlankOperations.BlankOperations objBl = new BlankOperations.BlankOperations();
            dtSalutation = objBl.NIM_LoadCombo("DirNameAffix", "Affix as Name,RECID as Id", " where AffixType=1");

            DataRow drSal = null;
            var dialogResult = CS.InternalApplication.Services.Dialog.GenericLookup(dtSalutation, 0, ref drSal, "Salutation");
            if (dialogResult == DialogResult.OK && drSal != null)
            {
                iSalutation = Convert.ToInt64(drSal["Id"]);
                txtSalutation.Text = Convert.ToString(drSal["Name"]);
            }

           
        }

        private void btnGender_Click(object sender, EventArgs e)
        {
            DataRow drGen = null;
            var dialogResult = CS.InternalApplication.Services.Dialog.GenericLookup(dtGender, 0, ref drGen, "Gender");
            if(dialogResult == DialogResult.OK && drGen != null)
            {
                iGender = Convert.ToInt16(drGen["Value"]);
                txtGender.Text = Convert.ToString(drGen["Name"]);
            }
        }


        private void textBoxFirstName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((!char.IsControl(e.KeyChar)) && (!Regex.IsMatch(Convert.ToString(e.KeyChar), @"[a-zA-Z\s\S]$")))
            {
                e.Handled = true;
            }
        }

        private void textBoxMiddleName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((!char.IsControl(e.KeyChar)) && (!Regex.IsMatch(Convert.ToString(e.KeyChar), @"[a-zA-Z\s\S]$")))
            {
                e.Handled = true;
            }

        }

        private void textBoxLastName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((!char.IsControl(e.KeyChar)) && (!Regex.IsMatch(Convert.ToString(e.KeyChar), @"[a-zA-Z\s\S]$")))
            {
                e.Handled = true;
            }
        }

        private void textBoxFirstName_Leave(object sender, EventArgs e)
        {
            textBoxFirstName.Text = Convert.ToString(textBoxFirstName.Text).ToUpper();
        }

        private void textBoxMiddleName_Leave(object sender, EventArgs e)
        {
            textBoxMiddleName.Text = Convert.ToString(textBoxMiddleName.Text).ToUpper();
        }

        private void textBoxLastName_Leave(object sender, EventArgs e)
        {
            textBoxLastName.Text = Convert.ToString(textBoxLastName.Text).ToUpper();
        }

        private void txtMobilePrimary_KeyPress(object sender, KeyPressEventArgs e)
        {
            if((!char.IsControl(e.KeyChar)) && (!Regex.IsMatch(Convert.ToString(e.KeyChar), @"[0-9+]$")))
            {
                e.Handled = true;
            }
        }

        private void txtSTD_KeyPress(object sender, KeyPressEventArgs e)
        {
            if((!char.IsControl(e.KeyChar)) && (!Regex.IsMatch(Convert.ToString(e.KeyChar), @"[0-9+]$")))
            {
                e.Handled = true;
            }
        }

        private void textBoxPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if((!char.IsControl(e.KeyChar)) && (!Regex.IsMatch(Convert.ToString(e.KeyChar), @"[0-9]$")))
            {
                e.Handled = true;
            }
        }

        private void txtSTD_Leave(object sender, EventArgs e)
        {
            string sPhone = "";

            sPhone = Convert.ToString(txtSTD.Text).Trim() + Convert.ToString(textBoxPhone.Text).Trim();

            if(sPhone.Length > 13)
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Phone number should be within 10 digits", MessageBoxButtons.OK, MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }
            }
        }

        private void textBoxPhone_Leave(object sender, EventArgs e)
        {
            string sPhone = "";

            sPhone = Convert.ToString(txtSTD.Text).Trim() + Convert.ToString(textBoxPhone.Text).Trim();

            if(sPhone.Length > 13)
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Phone number should be within 10 digits", MessageBoxButtons.OK, MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    textBoxPhone.Focus();
                }
            }

            string retString = string.Empty;
            string sMyStr = textBoxPhone.Text;

            //if(!string.IsNullOrEmpty(txtSTD.Text) && !string.IsNullOrEmpty(txtMobilePrimary.Text))
            //{
            //    if(sMyStr.Length > 7)
            //        retString = sMyStr.Substring(Convert.ToInt16(txtSTD.Text.Length), Convert.ToInt16(txtMobilePrimary.Text.Length) - Convert.ToInt16(txtSTD.Text.Length));
            //}
            txtMobilePrimary.Text = sMyStr;
        }

        private bool ValidateControls()
        {
            bool bReturn = true;

            //if(string.IsNullOrEmpty(textBoxEmail.Text.Trim())) // added on 03/12/2014 req by S.Sharma
            //{
            //    using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.
            //                                            frmMessage("Please enter a valid email.", MessageBoxButtons.OK, MessageBoxIcon.Error))
            //    {
            //        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            //        textBoxEmail.Focus();
            //    }

            //    bReturn = false;

            //}

            if (string.IsNullOrEmpty(txtSalutation.Text.Trim())) // added on 03/12/2014 req by S.Sharma
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.
                                                        frmMessage("Please select a salutation.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }

                bReturn = false;
            }
            if(string.IsNullOrEmpty(this.textBoxFirstName.Text.Trim())) 
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.
                                                        frmMessage("Please enter first name.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }

                bReturn = false;
            }

            if(string.IsNullOrEmpty(this.textBoxLastName.Text.Trim())) 
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.
                                                        frmMessage("Please enter last name.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }

                bReturn = false;
            }

            //if(string.IsNullOrEmpty(this.addressViewModel.City.Trim())) 
            //{
            //    using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.
            //                                            frmMessage("Please enter city.", MessageBoxButtons.OK, MessageBoxIcon.Error))
            //    {
            //        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            //    }

            //    bReturn = false;
            //}
            //if(string.IsNullOrEmpty(this.addressViewModel.Country.Trim())) 
            //{
            //    using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.
            //                                            frmMessage("Please enter country.", MessageBoxButtons.OK, MessageBoxIcon.Error))
            //    {
            //        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            //    }

            //    bReturn = false;
            //}

            if (string.IsNullOrEmpty(this.addressViewModel.SalesTaxGroup)) // added on 300318 req by U.Mustafi
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.
                                                        frmMessage("Please select a tax group.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }

                bReturn = false;
            }

            if(txtMobilePrimary.Text.Trim().Length > 13 || txtMobilePrimary.Text.Trim().Length < 7) // added on 27/06/2014 req by S.Sharma
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.
                                                        frmMessage("Please enter a valid mobile no.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    txtMobilePrimary.Focus();
                }

                bReturn = false;
            }
            if(string.IsNullOrEmpty(textNationality.Text.Trim()))
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.
                                                        frmMessage("Please select nationality.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    textNationality.Focus();
                }

                bReturn = false;
            }
            //if(string.IsNullOrEmpty(txtCustClassificationGroup.Text.Trim()))
            //{
            //    using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.
            //                                            frmMessage("Please select customer classification group.", MessageBoxButtons.OK, MessageBoxIcon.Error))
            //    {
            //        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            //        txtCustClassificationGroup.Focus();
            //    }

            //    bReturn = false;
            //}

            if (string.IsNullOrEmpty(txtGender.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.
                                                        frmMessage("Please select gender.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    txtGender.Focus();
                }

                bReturn = false;
            }
            

            return bReturn;
        }

        private void frmNewCustomer_Activated(object sender, EventArgs e)
        {
            //textCustAgeBracket.Text = sCustAgeBracket;
            //textNationality.Text = sNationality;
            GetCompanyISDCode(this.addressViewModel.Country);
            GetDefSalesTaxGroupCode();
            GetDefCustomerGroupCode();
        }

        private void btnAgeBracket_Click(object sender, EventArgs e)
        {
            DataTable dtCustAgeBracket = new DataTable();
            string sSqlstr = "SELECT BracketCode,[Description]  FROM [DBO].[CRWCUSTAGEBRACKET]";
            dtCustAgeBracket = GetDataTable(sSqlstr);

            DataRow drSal = null;
            //var dialogResult = CS.InternalApplication.Services.Dialog.GenericLookup(dtCustAgeBracket, 2, ref drSal, "Cust Age Bracket");
            var dialogResult = CS.InternalApplication.Services.Dialog.GenericSearch(dtCustAgeBracket, ref drSal, "Cust Age Bracket");
            if(dialogResult == DialogResult.OK && drSal != null)
            {
                sCustAgeBracket = Convert.ToString(drSal["BracketCode"]);
                textCustAgeBracket.Text = sCustAgeBracket;
            }
        }

        private void btnNationality_Click(object sender, EventArgs e)
        {
            this.viewModel.ExecuteNationality();
            
            //DataTable dtNationality = new DataTable();
            //string sSqlstr = "select r.COUNTRYREGIONID AS Country,t.SHORTNAME as Descriptin  from LOGISTICSADDRESSCOUNTRYREGION  R " +
            //                 " left join  LOGISTICSADDRESSCOUNTRYREGIONTRANSLATION T on r.COUNTRYREGIONID =t.COUNTRYREGIONID " +
            //                 " where t.LANGUAGEID ='" + ApplicationSettings.Terminal.CultureName + "'";
            //dtNationality = GetDataTable(sSqlstr);

            //DataRow drSal = null;
            //var dialogResult = CS.InternalApplication.Services.Dialog.GenericSearch(dtNationality, ref drSal, "Nationality");
            //if(dialogResult == DialogResult.OK && drSal != null)
            //{
            //    sNationality = Convert.ToString(drSal["Country"]);
            //    textNationality.Text = sNationality;
            //}
        }

        private DataTable GetDataTable(string sSQL)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;

                SqlComm.CommandText = sSQL;// "SELECT REPAIRID AS [Repair No],OrderDate as [Order Date],CUSTACCOUNT AS [Customer Account], CustName as [Customer Name] from RetailRepairReturnHdr where CUSTACCOUNT = '" + sCustAccount + "' AND IsDelivered = 0";

                DataTable dt = new DataTable();
                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(dt);

                return dt;

            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        enum MonthsOfYear // added on 03/12/2014 RH
        {
            None = 0,
            January = 1,
            February = 2,
            March = 3,
            April = 4,
            May = 5,
            June = 6,
            July = 7,
            August = 8,
            September = 9,
            October = 10,
            November = 11,
            December = 12,
        }

        private void txtBDay_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtBYear_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtAnnDay_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtAnnYear_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtMobilePrimary_Leave(object sender, EventArgs e)
        {
            string retString = string.Empty;
            string sMyStr = txtMobilePrimary.Text;

            //if(!string.IsNullOrEmpty(txtSTD.Text) && !string.IsNullOrEmpty(txtMobilePrimary.Text))
            //{
            //    if(sMyStr.Length > 7)
            //        retString = sMyStr.Substring(Convert.ToInt16(txtSTD.Text.Length), Convert.ToInt16(txtMobilePrimary.Text.Length) - Convert.ToInt16(txtSTD.Text.Length));
            //}
            textBoxPhone.Text = sMyStr;
            this.viewModel.Phone = sMyStr;
           // Customer.MobilePhone = retString;
        }

        private void Nim_LoadTitle()
        {
            //cmbPersonalTitle.Items.Add(" ");
            //cmbPersonalTitle.DataSource = NIM_ObjCClass.NIM_LoadCombo("DirNameAffix", "Affix", " where AffixType=1");
            //cmbPersonalTitle.DisplayMember = "Affix";
        }

        private void btnCustClassificationGrp_Click(object sender, EventArgs e)
        {
            DataTable dtCustClassGrp = new DataTable();
            string sSqlstr = "SELECT Code,[TXT] as [Description] FROM [DBO].[CustClassificationGroup]";
            dtCustClassGrp = GetDataTable(sSqlstr);

            DataRow drSal = null;
            //var dialogResult = CS.InternalApplication.Services.Dialog.GenericLookup(dtCustAgeBracket, 2, ref drSal, "Cust Age Bracket");
            var dialogResult = CS.InternalApplication.Services.Dialog.GenericSearch(dtCustClassGrp, ref drSal, "Cust classification group");
            if(dialogResult == DialogResult.OK && drSal != null)
            {
                sCustClassGrp = Convert.ToString(drSal["Code"]);
                txtCustClassificationGroup.Text = sCustClassGrp;//RETAILPARAMETERS ADD DEFAULTCUSTOMERGROUP
            }
        }

        private void btnCity_Click(object sender, EventArgs e)
        {
            DataTable dtCustClassGrp = new DataTable();
            string sSqlstr = "SELECT Name as Code,[Description] as [Description] FROM [DBO].[LOGISTICSADDRESSSCITY]";
            dtCustClassGrp = GetDataTable(sSqlstr);

            DataRow drSal = null;
            //var dialogResult = CS.InternalApplication.Services.Dialog.GenericLookup(dtCustAgeBracket, 2, ref drSal, "Cust Age Bracket");
            var dialogResult = CS.InternalApplication.Services.Dialog.GenericSearch(dtCustClassGrp, ref drSal, "Customer City");
            if (dialogResult == DialogResult.OK && drSal != null)
            {
                sCustCityGrp = Convert.ToString(drSal["Code"]);
                txtOccupation.Text = sCustCityGrp;//RETAILPARAMETERS ADD DEFAULTCUSTOMERGROUP
            }
        }
    }
}