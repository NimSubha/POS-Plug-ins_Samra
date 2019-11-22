using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch;
using LSRetailPosis.Transaction.Line.SaleItem;
using Microsoft.Dynamics.Retail.Pos.Dialog;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using System.IO;
using System.Collections.ObjectModel;
using Microsoft.Dynamics.Retail.Pos.RoundingService;

namespace BlankOperations.WinFormsTouch
{
    public partial class frmCustomerFeedBack:frmTouchBase
    {
        private SaleLineItem saleLineItem;
        Rounding objRounding = new Rounding();
        Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations objBlank = new Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations();
        enum CRWEnquiryOrPurchase
        {
            Enquiry = 0,
            Purchase = 1,
        }

        enum Gender
        {
            Unknown = 0,
            Male = 1,
            Female = 2,
        }
        enum CRWPurchaseType
        {
            None = 0,
            Gift = 1,
            Self = 2,
        }

        enum CRWJeweleryLocation
        {
            WallDisplay = 0,
            FloorCabinet = 1,
            WindowTrays = 2,
            BackupTrays = 3,
        }
        DataTable dtCust = new DataTable("dtCust");
        DataTable dtItemInfo = new DataTable("dtItemInfo");
        enum CRWPaidBy
        {
            Self = 0,
            SomeOneElse = 1,
        }
        bool IsEdit = false;
        int EditselectedIndex = 0;
        Random randUnique = new Random();

        public frmCustomerFeedBack()
        {
            InitializeComponent();

            LoadSalutaion();
            LoadCountry();
            cmbEnOrPurc.DataSource = Enum.GetValues(typeof(CRWEnquiryOrPurchase));
            cmbGender.DataSource = Enum.GetValues(typeof(Gender));
            cmbPurchType.DataSource = Enum.GetValues(typeof(CRWPurchaseType));
            cmbJewelleryLocation.DataSource = Enum.GetValues(typeof(CRWJeweleryLocation));
            cmbPaidBy.DataSource = Enum.GetValues(typeof(CRWPaidBy));

            dtCust = new DataTable("CUSTINFO");

            dtCust.Columns.Add("Affix", typeof(string));
            dtCust.Columns.Add("FirstName", typeof(string));
            dtCust.Columns.Add("LastName", typeof(string));
            dtCust.Columns.Add("PhoneNum", typeof(string));
            dtCust.Columns.Add("CountryRegionId", typeof(string));
            dtCust.Columns.Add("EmailID", typeof(string));
            dtCust.Columns.Add("EnquiryOrPurchase", typeof(int));
            dtCust.Columns.Add("Gender", typeof(int));
            dtCust.Columns.Add("Resident", typeof(int));
            dtCust.Columns.Add("StoreId", typeof(string));
            dtCust.Columns.Add("FullAddress", typeof(string));

            dtCust.AcceptChanges();
            DataColumn[] columns = new DataColumn[1];
            columns[0] = dtCust.Columns["Affix"];
            dtCust.PrimaryKey = columns;
        }

        private void LoadSalutaion()
        {
            DataTable dtSalutation = objBlank.NIM_LoadCombo("DirNameAffix", "Affix as Name,RECID as Id", " where AffixType=1");
            if(dtSalutation.Rows.Count > 0)
            {
                cmbTitle.DataSource = dtSalutation;
                cmbTitle.DisplayMember = "Name";
                cmbTitle.ValueMember = "Id";
            }
        }

        private void LoadCountry()
        {
            DataTable dtNationality = new DataTable();
            string sSqlstr = "select r.COUNTRYREGIONID AS Country,t.SHORTNAME as Description  from LOGISTICSADDRESSCOUNTRYREGION  R " +
                             " left join  LOGISTICSADDRESSCOUNTRYREGIONTRANSLATION T on r.COUNTRYREGIONID =t.COUNTRYREGIONID " +
                             " where t.LANGUAGEID ='" + ApplicationSettings.Terminal.CultureName + "'";
            dtNationality = GetDataTable(sSqlstr);

            if(dtNationality.Rows.Count > 0)
            {
                cmbCountry.DataSource = dtNationality;
                cmbCountry.DisplayMember = "Description";
                cmbCountry.ValueMember = "Country";
            }
        }

        private static DataTable GetDataTable(string sSQL)
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCollection_Click(object sender, EventArgs e)
        {
            DataTable dtCustAgeBracket = new DataTable();
            string sSqlstr = "SELECT COLLECTIONCODE,[COLLECTIONDESC]  FROM [DBO].[COLLECTIONMASTER]";
            dtCustAgeBracket = GetDataTable(sSqlstr);

            if(dtCustAgeBracket != null && dtCustAgeBracket.Rows.Count > 0)
            {
                DataRow drCur = null;

                Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch oSearch = new Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch(dtCustAgeBracket, drCur, "Collection");
                oSearch.ShowDialog();
                drCur = oSearch.SelectedDataRow;
                if(drCur != null)
                {
                    lblCollection.Tag = Convert.ToString(drCur["COLLECTIONCODE"]);
                    lblCollection.Text = Convert.ToString(drCur["COLLECTIONDESC"]);
                }
            }
        }

        private void btnCurrency_Click(object sender, EventArgs e)
        {
            DataTable dtCurrency = new DataTable();
            string sSqlstr = "SELECT  [CURRENCYCODE]  ,[TXT] FROM [dbo].[CURRENCY]";
            dtCurrency = GetDataTable(sSqlstr);

            if(dtCurrency != null && dtCurrency.Rows.Count > 0)
            {
                DataRow drCur = null;

                Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch oSearch = new Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch(dtCurrency, drCur, "Currency");
                oSearch.ShowDialog();
                drCur = oSearch.SelectedDataRow;
                if(drCur != null)
                {
                    lblCurrency.Tag = Convert.ToString(drCur["CURRENCYCODE"]);
                    lblCurrency.Text = Convert.ToString(drCur["CURRENCYCODE"]);
                }
            }
        }

        private void btnSubCollection_Click(object sender, EventArgs e)
        {
            DataTable dtCurrency = new DataTable();
            string sSqlstr = "SELECT  [SUBPRODUCTCODE]  ,[DESCRIPTION] FROM [dbo].[SUBPRODUCT_MASTER]";
            dtCurrency = GetDataTable(sSqlstr);

            if(dtCurrency != null && dtCurrency.Rows.Count > 0)
            {
                DataRow drCur = null;

                Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch oSearch = new Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch(dtCurrency, drCur, "Currency");
                oSearch.ShowDialog();
                drCur = oSearch.SelectedDataRow;
                if(drCur != null)
                {
                    lblSubCollection.Tag = Convert.ToString(drCur["SUBPRODUCTCODE"]);
                    lblSubCollection.Text = Convert.ToString(drCur["SUBPRODUCTCODE"]);
                }
            }
        }

        private void btnProductType_Click(object sender, EventArgs e)
        {
            DataTable dtCurrency = new DataTable();
            string sSqlstr = "SELECT  [PRODUCTCODE]  ,[DESCRIPTION] FROM [dbo].[PRODUCTTYPEMASTER]";
            dtCurrency = GetDataTable(sSqlstr);

            if(dtCurrency != null && dtCurrency.Rows.Count > 0)
            {
                DataRow drCur = null;
                Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch oSearch = new Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch(dtCurrency, drCur, "Currency");
                oSearch.ShowDialog();
                drCur = oSearch.SelectedDataRow;
                if(drCur != null)
                {
                    lblProductType.Tag = Convert.ToString(drCur["PRODUCTCODE"]);
                    lblProductType.Text = Convert.ToString(drCur["PRODUCTCODE"]);
                }
            }
        }

        private void btnEMployee_Click(object sender, EventArgs e)
        {
            LSRetailPosis.POSProcesses.frmSalesPerson dialog = new LSRetailPosis.POSProcesses.frmSalesPerson();
            dialog.ShowDialog();
            lblEmployee.Tag = dialog.SelectedEmployeeId;
            lblEmployee.Text = dialog.SelectEmployeeName;
        }

        private void btnProductSearch_Click(object sender, EventArgs e)
        {
            if(IsEdit)
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("You are in editing mode", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return;
                }
            }
            Dialog objdialog = new Dialog();
            string str = string.Empty;
            DataSet dsItem = new DataSet();
            objdialog.MyItemSearch(50000, ref str, out  dsItem, " AND  I.ITEMID IN (SELECT ITEMID FROM INVENTTABLE WHERE RETAIL=1) ");

            saleLineItem = new SaleLineItem();

            if(dsItem != null && dsItem.Tables.Count > 0 && dsItem.Tables[0].Rows.Count > 0)
            {
                if(!string.IsNullOrEmpty(Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"])))
                {
                    saleLineItem.ItemId = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"]);
                    txtItemId.Text = saleLineItem.ItemId;
                    txtItemName.Text = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMNAME"]);
                    string sPType = string.Empty;
                    string sSubPType = string.Empty;
                    string sCollCode = string.Empty;

                    GetItemInfo(saleLineItem.ItemId, ref sPType, ref sSubPType, ref sCollCode);

                    lblProductType.Text = sPType;
                    lblSubCollection.Text = sSubPType;
                    lblCollection.Text = sCollCode;
                }
            }
        }

        protected void GetItemInfo(string sItemId, ref string sProductType, ref string sSubProd, ref string sCollectionCode)
        {

            SqlConnection conn = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select isnull(PRODUCTTYPECODE,''),isnull(SubProductCode,''),isnull(CollectionCode,'') from inventtable where itemid='" + sItemId + "'");

            if(conn.State == ConnectionState.Closed)
                conn.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            using(SqlDataReader reader = command.ExecuteReader())
            {
                if(reader.HasRows)
                {
                    while(reader.Read())
                    {
                        sProductType = (string)reader.GetValue(0);
                        sSubProd = (string)reader.GetValue(1);
                        sCollectionCode = (string)reader.GetValue(2);
                    }
                }
            }
            if(conn.State == ConnectionState.Open)
                conn.Close();
        }

        private void txtRRP_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(txtRRP.Text == string.Empty && e.KeyChar == '.')
            {
                e.Handled = true;
            }
            else
            {
                if(!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                if(e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }
                if(e.KeyChar == (Char)Keys.Enter)
                {
                    e.Handled = true;
                }
            }
        }

        private void txtFinalPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(txtFinalPrice.Text == string.Empty && e.KeyChar == '.')
            {
                e.Handled = true;
            }
            else
            {
                if(!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                if(e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }
                if(e.KeyChar == (Char)Keys.Enter)
                {
                    e.Handled = true;
                }
            }
        }

        private void txtDisc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(txtDisc.Text == string.Empty && e.KeyChar == '.')
            {
                e.Handled = true;
            }
            else
            {
                if(!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                if(e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }
                if(e.KeyChar == (Char)Keys.Enter)
                {
                    e.Handled = true;
                }
            }
        }

        private void txtBudget_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(txtBudget.Text == string.Empty && e.KeyChar == '.')
            {
                e.Handled = true;
            }
            else
            {
                if(!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                if(e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }
                if(e.KeyChar == (Char)Keys.Enter)
                {
                    e.Handled = true;
                }
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            DataRow drCust;
            drCust = dtCust.NewRow();
            if(dtCust != null && dtCust.Rows.Count == 0)
            {
                drCust["StoreId"] = ApplicationSettings.Terminal.StoreId;
                drCust["Affix"] = cmbTitle.Text;
                drCust["FirstName"] = txtFirstName.Text.Trim();
                drCust["LastName"] = txtLastName.Text.Trim();
                drCust["PhoneNum"] = txtPhone.Text.Trim();
                drCust["CountryRegionId"] = cmbCountry.Text.Trim();
                drCust["EmailID"] = txtEmail.Text.Trim();
                drCust["EnquiryOrPurchase"] = Convert.ToInt16(cmbEnOrPurc.SelectedIndex);
                drCust["Gender"] = cmbGender.SelectedIndex;
                drCust["Resident"] = Convert.ToInt16(chkResidence.Checked);
                drCust["FullAddress"] = txtAddress.Text;

                dtCust.Rows.Add(drCust);
                dtCust.AcceptChanges();
            }

            if((dtCust != null && dtCust.Rows.Count > 0) && (dtItemInfo != null && dtItemInfo.Rows.Count > 0))
            {
                try
                {
                    ReadOnlyCollection<object> containerArray;
                    string sMsg = string.Empty;
                    MemoryStream mstr = new MemoryStream();
                    dtCust.WriteXml(mstr, true);

                    mstr.Seek(0, SeekOrigin.Begin);
                    StreamReader sr = new StreamReader(mstr);
                    string sCustInfo = string.Empty;
                    sCustInfo = sr.ReadToEnd();


                    MemoryStream mstr1 = new MemoryStream();
                    dtItemInfo.WriteXml(mstr1, true);

                    mstr1.Seek(0, SeekOrigin.Begin);
                    StreamReader srItem = new StreamReader(mstr1);
                    string sItemInfo = string.Empty;
                    sItemInfo = srItem.ReadToEnd();


                    if(PosApplication.Instance.TransactionServices.CheckConnection())
                    {
                        bool bStatus = false;
                        string sTransferId = string.Empty;

                        containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("createCustFeedBackForm", sCustInfo, sItemInfo);
                        bStatus = Convert.ToBoolean(containerArray[1]);
                        //dtItemInfo = new DataTable("dtItemInfo");
                        if(bStatus)
                        {
                            MessageBox.Show("Customer feedback successfully submited.");
                            dtItemInfo.Clear();
                            dtItemInfo = null;
                            grItems.DataSource = dtItemInfo;
                        }
                    }
                }

                catch(Exception ex)
                {
                    MessageBox.Show("Customer feedback faild to submit.");
                }
            }
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            string sUniqueNo = string.Empty;
            if(ValidateControls())
            {
                DataRow dr;
                if(IsEdit == false && dtItemInfo != null && dtItemInfo.Rows.Count == 0 && dtItemInfo.Columns.Count == 0)
                {
                    IsEdit = false;
                    dtItemInfo.Columns.Add("UNIQUEID", typeof(string));
                    dtItemInfo.Columns.Add("ItemId", typeof(string));
                    dtItemInfo.Columns.Add("ItemName", typeof(string));
                    dtItemInfo.Columns.Add("ProductTypeCode", typeof(string));
                    dtItemInfo.Columns.Add("SubProductCode", typeof(string));
                    dtItemInfo.Columns.Add("CollectionCode", typeof(string));
                    dtItemInfo.Columns.Add("PurchaseType", typeof(string));
                    dtItemInfo.Columns.Add("JeweleryLocation", typeof(string));
                    dtItemInfo.Columns.Add("Currency", typeof(string));
                    dtItemInfo.Columns.Add("Budget", typeof(decimal));
                    dtItemInfo.Columns.Add("FinalPrice", typeof(decimal));
                    dtItemInfo.Columns.Add("RRP", typeof(decimal));
                    dtItemInfo.Columns.Add("Discount", typeof(decimal));
                    dtItemInfo.Columns.Add("PaidBy", typeof(string));
                    dtItemInfo.Columns.Add("Employee", typeof(string));
                    dtItemInfo.Columns.Add("Remarks", typeof(string));
                }
                if(IsEdit == false)
                {
                    dr = dtItemInfo.NewRow();
                    dr["UNIQUEID"] = sUniqueNo = Convert.ToString(randUnique.Next());
                    dr["ItemId"] = Convert.ToString(txtItemId.Text.Trim());
                    dr["ItemName"] = Convert.ToString(txtItemId.Text.Trim());
                    dr["ProductTypeCode"] = Convert.ToString(lblProductType.Text.Trim());
                    dr["SubProductCode"] = Convert.ToString(lblSubCollection.Text.Trim());
                    dr["CollectionCode"] = Convert.ToString(lblCollection.Text.Trim());
                    dr["PurchaseType"] = Convert.ToString(cmbPurchType.Text);// (int)((CRWPurchaseType)Enum.Parse(typeof(CRWPurchaseType), Convert.ToString(cmbPurchType.Text)));
                    dr["JeweleryLocation"] = Convert.ToString(cmbJewelleryLocation.Text);// (int)((CRWJeweleryLocation)Enum.Parse(typeof(CRWJeweleryLocation), Convert.ToString(cmbJewelleryLocation.Text)));
                    dr["Currency"] = Convert.ToString(lblCurrency.Text);


                    if(!string.IsNullOrEmpty(txtBudget.Text.Trim()))
                        dr["Budget"] = decimal.Round(Convert.ToDecimal(txtBudget.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                    else
                        dr["Budget"] = DBNull.Value;

                    if(!string.IsNullOrEmpty(txtFinalPrice.Text.Trim()))
                        dr["FinalPrice"] = decimal.Round(Convert.ToDecimal(txtFinalPrice.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                    else
                        dr["FinalPrice"] = 0m;
                    if(!string.IsNullOrEmpty(txtRRP.Text.Trim()))
                        dr["RRP"] = decimal.Round(Convert.ToDecimal(txtRRP.Text.Trim()), 2, MidpointRounding.AwayFromZero);
                    else
                        dr["RRP"] = 0m;
                    if(!string.IsNullOrEmpty(txtDisc.Text.Trim()))
                        dr["Discount"] = decimal.Round(Convert.ToDecimal(txtDisc.Text.Trim()), 2, MidpointRounding.AwayFromZero);
                    else
                        dr["Discount"] = 0m;

                    dr["PaidBy"] = Convert.ToString(cmbPaidBy.Text);// (int)((CRWPaidBy)Enum.Parse(typeof(CRWPaidBy), Convert.ToString(cmbPaidBy.Text)));
                    dr["Employee"] = Convert.ToString(lblEmployee.Tag);
                    dr["Remarks"] = Convert.ToString(txtRemarks.Text);
                    dtItemInfo.Rows.Add(dr);

                    grItems.DataSource = dtItemInfo.DefaultView;
                    grdView.Columns[0].Visible = false; ;
                }
                if(IsEdit == true)
                {

                    DataRow EditRow = dtItemInfo.Rows[EditselectedIndex];
                    EditRow["ItemId"] = txtItemId.Text.Trim();
                    EditRow["ItemName"] = txtItemName.Text.Trim();
                    sUniqueNo = Convert.ToString(EditRow["UNIQUEID"]);  // CHECK -- S
                    EditRow["ProductTypeCode"] = lblProductType.Text.Trim();
                    EditRow["SubProductCode"] = lblSubCollection.Text.Trim();
                    EditRow["CollectionCode"] = lblCollection.Text.Trim();
                    EditRow["PurchaseType"] = cmbPurchType.Text.Trim();
                    EditRow["JeweleryLocation"] = cmbJewelleryLocation.Text;
                    EditRow["Currency"] = lblCurrency.Text.Trim();

                    if(!string.IsNullOrEmpty(txtBudget.Text.Trim()))
                        EditRow["Budget"] = objRounding.Round(Convert.ToDecimal(txtBudget.Text.Trim()), 2);
                    else
                        EditRow["Budget"] = objRounding.Round(decimal.Zero, 2);

                    if(!string.IsNullOrEmpty(txtFinalPrice.Text.Trim()))
                        EditRow["FinalPrice"] = objRounding.Round(Convert.ToDecimal(txtFinalPrice.Text.Trim()), 2);
                    else
                        EditRow["FinalPrice"] = objRounding.Round(decimal.Zero, 2);

                    if(!string.IsNullOrEmpty(txtRRP.Text.Trim()))
                        EditRow["RRP"] = objRounding.Round(Convert.ToDecimal(txtRRP.Text.Trim()), 2);
                    else
                        EditRow["RRP"] = objRounding.Round(decimal.Zero, 2);

                    if(!string.IsNullOrEmpty(txtDisc.Text.Trim()))
                        EditRow["Discount"] = objRounding.Round(Convert.ToDecimal(txtDisc.Text.Trim()), 2);
                    else
                        EditRow["Discount"] = objRounding.Round(decimal.Zero, 2);

                    EditRow["PaidBy"] = Convert.ToString(cmbPaidBy.Text);// (int)((CRWPaidBy)Enum.Parse(typeof(CRWPaidBy), Convert.ToString(cmbPaidBy.Text)));
                    EditRow["Employee"] = Convert.ToString(lblEmployee.Tag);
                    EditRow["Remarks"] = Convert.ToString(txtRemarks.Text);

                    dtItemInfo.AcceptChanges();

                    grItems.DataSource = dtItemInfo.DefaultView;
                    IsEdit = false;
                }
            }
            ClearControls();
        }

        private void ClearControls()
        {
            txtItemId.Text = "";
            txtItemId.Text = "";
            lblProductType.Text = "";
            lblSubCollection.Text = "";
            lblCollection.Text = "";
            cmbPurchType.Text = "";
            cmbJewelleryLocation.Text = "";
            lblCurrency.Text = "";
            txtBudget.Text = "";
            txtFinalPrice.Text = "";
            txtRRP.Text = "";
            txtDisc.Text = "";
            txtItemName.Text = "";
            txtRemarks.Text = "";
            cmbPaidBy.Text = "";
            lblEmployee.Text = "";

        }

        private bool ValidateControls()
        {
            bool bReturn = true;

            return bReturn;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            IsEdit = false;
            if(dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                if(grdView.RowCount > 0)
                {
                    IsEdit = true;
                    EditselectedIndex = grdView.GetSelectedRows()[0];
                    DataRow theRowToSelect = dtItemInfo.Rows[EditselectedIndex];

                    txtItemId.Text = Convert.ToString(theRowToSelect["ItemId"]);
                    txtItemName.Text = Convert.ToString(theRowToSelect["ItemName"]);
                    lblProductType.Text = Convert.ToString(theRowToSelect["ProductTypeCode"]);
                    lblSubCollection.Text = Convert.ToString(theRowToSelect["SubProductCode"]);
                    lblCollection.Text = Convert.ToString(theRowToSelect["CollectionCode"]);
                    cmbPurchType.Text = Convert.ToString(theRowToSelect["PurchaseType"]);
                    cmbJewelleryLocation.Text = Convert.ToString(theRowToSelect["JeweleryLocation"]);
                    lblCurrency.Text = Convert.ToString(theRowToSelect["Currency"]);
                    txtBudget.Text = Convert.ToString(theRowToSelect["Budget"]);
                    txtFinalPrice.Text = Convert.ToString(theRowToSelect["FinalPrice"]);
                    txtRRP.Text = Convert.ToString(theRowToSelect["RRP"]);
                    txtDisc.Text = Convert.ToString(theRowToSelect["Discount"]);
                    cmbPaidBy.Text = Convert.ToString(theRowToSelect["PaidBy"]);
                    lblEmployee.Tag = Convert.ToString(theRowToSelect["Employee"]);
                    lblEmployee.Text = getStaffName(Convert.ToString(theRowToSelect["Employee"]));
                    txtRemarks.Text = Convert.ToString(theRowToSelect["Remarks"]);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int DeleteSelectedIndex = 0;
            if(dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                if(grdView.RowCount > 0)
                {
                    DeleteSelectedIndex = grdView.GetSelectedRows()[0];
                    DataRow theRowToDelete = dtItemInfo.Rows[DeleteSelectedIndex];

                    dtItemInfo.Rows.Remove(theRowToDelete);
                    grItems.DataSource = dtItemInfo.DefaultView;
                }
            }
            if(DeleteSelectedIndex == 0 && dtItemInfo != null && dtItemInfo.Rows.Count == 0)
            {
                grItems.DataSource = null;
                dtItemInfo.Clear();
            }
            IsEdit = false;

            //if(dtItemInfo.Rows.Count == 0)
            //{
            //    btnAddItem.Enabled = true;
            //}
            //else if(dtItemInfo.Rows.Count == 1)
            //{
            //    btnAddItem.Enabled = false;
            //}
        }

        private string getStaffName(string sOpId)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            string commandText = string.Empty;

            commandText = "select d.Name as Name from RETAILSTAFFTABLE r  " +
                        " left join dbo.HCMWORKER as h on h.PERSONNELNUMBER = r.STAFFID " +
                        " left join dbo.DIRPARTYTABLE as d on d.RECID = h.PERSON " +
                        "  where R.STAFFID = '" + sOpId + "'";

            if(conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if(conn.State == ConnectionState.Open)
                conn.Close();
            if(!string.IsNullOrEmpty(sResult))
                return sResult.Trim();
            else
                return "-";
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ReadOnlyCollection<object> containerArray;
                string sMsg = string.Empty;

                string sPhone = txtSearchPhone.Text;
                string sEmail = txtSearchEmail.Text;

                if(!string.IsNullOrEmpty(sPhone) || !string.IsNullOrEmpty(sEmail))
                {
                    if(PosApplication.Instance.TransactionServices.CheckConnection())
                    {
                        bool bStatus = false;

                        containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("getFeedBackCustInfo", sEmail, sPhone);
                        bStatus = Convert.ToBoolean(containerArray[1]);
                        DataRow drSelected = null;
                        DataSet dsWH = new DataSet();
                        DataSet dsDetails = new DataSet();
                        StringReader srTransH = new StringReader(Convert.ToString(containerArray[3]));
                        StringReader srTransDetail = new StringReader(Convert.ToString(containerArray[4]));

                        if(Convert.ToString(containerArray[3]).Trim().Length > 38)
                        {
                            dsWH.ReadXml(srTransH);
                        }


                        if(dsWH != null && dsWH.Tables[0].Rows.Count > 0)
                        {
                            Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch OSearch =
                                new Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch(dsWH.Tables[0], drSelected, "Customer Feedback");
                            OSearch.ShowDialog();
                            drSelected = OSearch.SelectedDataRow;

                            groupBox1.Enabled = false;
                            groupBox2.Enabled = false;
                            btnAddItem.Enabled = false;
                            btnEdit.Enabled = false;
                            btnDelete.Enabled = false;
                            btnSubmit.Enabled = false;


                            if(Convert.ToString(containerArray[4]).Trim().Length > 38)
                            {
                                dsDetails.ReadXml(srTransDetail);
                            }
                            DataTable dtDetails = new DataTable();

                            if(drSelected != null)
                            {
                                string sRecId = Convert.ToString(drSelected["RECID"]);
                                cmbTitle.Text = Convert.ToString(drSelected["Affix"]);
                                txtFirstName.Text = Convert.ToString(drSelected["FirstName"]);
                                txtLastName.Text = Convert.ToString(drSelected["LastName"]);
                                txtPhone.Text = Convert.ToString(drSelected["PhoneNum"]);
                                cmbCountry.Text = Convert.ToString(drSelected["CountryRegionId"]);
                                txtEmail.Text = Convert.ToString(drSelected["EmailID"]);
                                cmbEnOrPurc.Text = Convert.ToString(drSelected["EnquiryOrPurchase"]);
                                cmbGender.Text = Convert.ToString(drSelected["Gender"]);
                                if(Convert.ToString(drSelected["Resident"]) == "True")
                                    chkResidence.Checked = true;
                                else
                                    chkResidence.Checked = false;

                                txtAddress.Text = Convert.ToString(drSelected["FullAddress"]);


                                if(dsDetails != null && dsDetails.Tables[0].Rows.Count > 0)
                                {
                                    dtDetails = dsDetails.Tables[0].Select("FeedbackRefRecID = '" + sRecId + "'").CopyToDataTable();
                                }

                                if(dtDetails != null && dtDetails.Rows.Count > 0)
                                {
                                    dtItemInfo = dtDetails;
                                    grItems.DataSource = dtItemInfo;

                                    if(grdView.Columns.Count > 1)
                                    {
                                        grdView.Columns[grdView.Columns.Count - 1].Visible = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No record found.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                        }
                    }
                }
            }

            catch(Exception ex)
            {
                MessageBox.Show("Customer feedback faild to search.");
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            groupBox1.Enabled = true;
            groupBox2.Enabled = true;
            btnAddItem.Enabled = true;
            btnEdit.Enabled = true;
            btnDelete.Enabled = true;
            btnSubmit.Enabled = true;
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtPhone.Text = "";
            txtEmail.Text = "";
            cmbGender.SelectedIndex = 0;
            cmbCountry.SelectedIndex = 0;
            cmbEnOrPurc.SelectedIndex = 0;
            cmbCountry.Text = "";
            chkResidence.Checked = false;
            grItems.DataSource = null;
            txtAddress.Text = "";
            ClearControls();
        }
    }
}
