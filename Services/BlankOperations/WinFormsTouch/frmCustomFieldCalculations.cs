using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections;
using LSRetailPosis.Settings;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch;
using Microsoft.Dynamics.Retail.Pos.RoundingService;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.SaleItem;


namespace BlankOperations.WinFormsTouch
{
    public partial class frmCustomFieldCalculations:frmTouchBase
    {

        string preTransType = string.Empty; ////Start : Changes on 08/04/2014 RHossain   

        #region Variable Declaration
        Rounding oRounding = new Rounding();
        string sEnteredNum = string.Empty;
        public string RadioChecked { get; set; }
        bool isViewing = false;
        public bool isCancelClick = false;
        public List<KeyValuePair<string, string>> saleList;
        public List<KeyValuePair<string, string>> purchaseList;
        SqlConnection conn;
        string ItemID = string.Empty;
        string StoreID = string.Empty;
        string ConfigID = string.Empty;
        string ColorID = string.Empty;
        string BatchID = string.Empty;
        string SizeID = string.Empty;
        string GrWeight = string.Empty;
        decimal dStoneWtRange = 0m;
        int lineNum = 0;
        string index = string.Empty;
        public DataTable dtIngredients = null;
        public DataTable dtIngredientsClone = null;
        ArrayList alist = new ArrayList();
        DataSet dsCustOrder = new DataSet();
        string LineNum = string.Empty;
        string sNumPadEntryType = string.Empty;
        string MRPUCP = string.Empty;
        bool isMRPUCP = false;

        // GSS Maturity
        bool IsGSSTransaction = false;
        decimal dGSSMaturityQty = 0m;
        decimal dGSSAvgRate = 0m;

        decimal dPrevGSSSaleWt = 0m;
        decimal dPrevRunningQtyGSS = 0m;
        decimal dActualGSSSaleWt = 0m;

        string sGSSAdjustmentGoldRate = string.Empty;
        RetailTransaction retailTrans;

        // Avg Gold Rate Adjustment
        bool IsSaleAdjustment = false;
        decimal dSaleAdjustmentGoldAmt = 0m;
        decimal dSaleAdjustmentGoldQty = 0m;
        decimal dSaleAdjustmentAvgGoldRate = 0m;

        decimal dPrevTransAdjGoldQty = 0m;
        decimal dPrevRunningTransAdjGoldQty = 0m;
        decimal dActualTransAdjGoldQty = 0m;

        string sAdvAdjustmentGoldRate = string.Empty;

        // Added for wastage
        string sBaseItemID = string.Empty;
        string sBaseConfigID = string.Empty;
        Decimal dWMetalRate = 0m;
        //
        // Making Discount Type  
        decimal dMakingDiscDbAmt = 0m;

        decimal dCustomerOrderFixedRate = 0m;

        int iCallfrombase = 0;

        public DataTable dtExtndPurchExchng = null;
        string sBaseUnitId;
        string sMkPromoCode = "";
        string sCustomerId = string.Empty;
        int iIsMkDiscFlat = 0;

        #endregion

        #region enum  RateType
        enum RateType
        {
            Weight = 0,
            Pieces = 1,
            Tot = 2,
        }
        #endregion

        #region enum  MakingType
        enum MakingType
        {
            Weight = 2,
            Pieces = 0,
            Tot = 3,
            Percentage = 4,
        }

        #endregion

        #region Wastage Type

        enum WastageType
        {
            //  None    = 0,
            Weight = 0,
            Percentage = 1,
        }

        #endregion
        #region // Making Discount Type

        enum MakingDiscType
        {
            Percentage = 0,
            //Weight = 1,
            //Amount = 2,

        }
        #endregion

        #region enum CRWRetailDiscPermission
        private enum CRWRetailDiscPermission // added on 29/08/2014
        {
            Cashier = 0,
            Salesperson = 1,
            Manager = 2,
            Other = 3,
            Manager2 = 4,
        }
        #endregion

        private IApplication application;

        [Import]
        public IApplication Application
        {
            get
            {
                return this.application;
            }
            set
            {
                this.application = value;
                InternalApplication = value;
            }
        }

        internal static IApplication InternalApplication { get; private set; }

        #region enum  RateTypeNew
        enum RateTypeNew
        {
            Purchase = 0,
            OGP = 1,
            OGOP = 2,
            Sale = 3,
            GSS = 4,
            Exchange = 6,
            OtherExchange = 8,
        }
        #endregion

        #region enum MetalType
        public enum MetalType
        {
            Other = 0,
            Gold = 1,
            Silver = 2,
            Platinum = 3,
            Alloy = 4,
            Diamond = 5,
            Pearl = 6,
            Stone = 7,
            Consumables = 8,
            Watch = 11,
            LooseDmd = 12,
            Palladium = 13,
            Jewellery = 14,
            Metal = 15,
            PackingMaterial = 16,
            Certificate = 17,
        }
        #endregion

        #region enum LType
        enum LType
        {
            Main = 0,
            Sub = 1,

        }
        #endregion

        #region TransactionType
        public enum TransactionType
        {
            Sale = 0,
            Purchase = 1,
            Exchange = 3,
            PurchaseReturn = 2,
            ExchangeReturn = 4,
        }
        #endregion

        #region Initialization

        public frmCustomFieldCalculations()
        {
            InitializeComponent();
            btnCLose.Visible = false;
            BindRateTypeMakingTypeCombo();
            BindWastage();
            BindMakingDiscount(); // Making Discount Type
            EnableSaleButtons();
            RadioChecked = Convert.ToString((int)TransactionType.Sale);
            chkOwn.Visible = false;
        }

        private bool ValidateItem(string item)
        {
            bool ValidItem = true;

            SqlCommand cmd = null;
            try
            {
                if(conn.State == ConnectionState.Closed)
                    conn.Open();
                string query = " DECLARE @ITEMID NVARCHAR(20) " +
                               " SELECT TOP(1) @ITEMID=ITEMID FROM INVENTTABLE WHERE ITEMID='" + item + "' AND RETAIL=1" +
                               " IF ISNULL(@ITEMID,'')='' " +
                               " BEGIN " +
                               " SELECT 'True' " +
                               " END " +
                    // " ELSE IF EXISTS(SELECT * FROM SKUTable_Posted WHERE SKUNUMBER=@ITEMID) " +
                               " ELSE IF EXISTS(SELECT * FROM SKUTableTrans WHERE SKUNUMBER=@ITEMID) " +
                               " BEGIN " +
                               " SELECT 'True' " +
                               " END " +
                               " ELSE " +
                               " BEGIN " +
                               " SELECT 'False' " +
                               " END ";
                cmd = new SqlCommand(query, conn);
                ValidItem = Convert.ToBoolean(cmd.ExecuteScalar());
            }
            catch(Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
            finally
            {
                if(conn.State == ConnectionState.Open)
                    conn.Close();

            }
            return ValidItem;



        }

        private bool ValidateServiceItem(string item)
        {
            bool ValidItem = true;

            SqlCommand cmd = null;
            try
            {
                if(conn.State == ConnectionState.Closed)
                    conn.Open();
                string query = " DECLARE @status INT IF EXISTS (SELECT ADJUSTMENTITEMID+GSSADJUSTMENTITEMID+GSSDISCOUNTITEMID " +
                                " FROM [RETAILPARAMETERS] WHERE  (ADJUSTMENTITEMID = '" + item + "' OR GSSADJUSTMENTITEMID = '" + item + "' " +
                                 " OR GSSDISCOUNTITEMID = '" + item + "') AND DATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "')" +
                                   " BEGIN SET @status = 0 END ELSE BEGIN SET @status = 1 END SELECT @status";

                cmd = new SqlCommand(query, conn);
                ValidItem = Convert.ToBoolean(cmd.ExecuteScalar());
            }
            catch(Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
            finally
            {
                if(conn.State == ConnectionState.Open)
                    conn.Close();

            }
            return ValidItem;

        }

        public frmCustomFieldCalculations(SqlConnection connection, string itemId, string storeId, string configId, ArrayList al, string MRP, bool isMRP, IPosTransaction posTransaction = null, string UnitId = null) // GSS Maturity
        {
            InitializeComponent();

            retailTrans = posTransaction as RetailTransaction;
            if(retailTrans != null)
            {
                IsGSSTransaction = retailTrans.PartnerData.IsGSSMaturity;

                IsSaleAdjustment = retailTrans.PartnerData.SaleAdjustment;

                if(!string.IsNullOrEmpty(retailTrans.Customer.CustomerId))
                {
                    sCustomerId = retailTrans.Customer.CustomerId;
                }
            }

            btnCLose.Visible = false;
            conn = connection;
            if(!ValidateItem(itemId) || !ValidateServiceItem(itemId))
            {
                using(LSRetailPosis.POSProcesses.frmMessage message = new LSRetailPosis.POSProcesses.frmMessage("This item is not properly defined on other modules.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(message);
                }
                isCancelClick = true;
                this.Close();
            }
            else
            {

                BindRateTypeMakingTypeCombo();
                BindWastage();
                BindMakingDiscount();
                cmbMakingType.SelectedIndex = (int)MakingType.Weight;


                ItemID = itemId;
                StoreID = storeId;
                ConfigID = configId;
                MRPUCP = MRP;
                isMRPUCP = isMRP;
                if(!IsRetailItem(ItemID)) // for samra gift item should allow price override in sales screen
                    EnableSaleButtons();
                else
                {
                    if(IsGiftItem(ItemID))
                    {
                        EnableSaleButtons();
                    }
                    else
                    {
                        txtPCS.Enabled = false;
                        txtRate.Enabled = false;
                        txtTotalWeight.Enabled = false;
                        txtPurity.Enabled = false;
                        txtLossPct.Enabled = false;
                    }
                }
                lblSelectedSKU.Text = itemId;// added on 25/02/2015
                //txtTotalWeight.Enabled = false;
                //txtPurity.Enabled = false; 

                sBaseItemID = itemId;
                sBaseConfigID = configId;
                sBaseUnitId = UnitId;
                //

                //  RadioChecked = "Sale";
                RadioChecked = Convert.ToString((int)TransactionType.Sale);
                chkOwn.Checked = false;
                chkOwn.Visible = false;

                if(IsGSSTransaction)
                {
                    dGSSMaturityQty = Convert.ToDecimal(retailTrans.PartnerData.GSSTotQty);
                    dGSSAvgRate = Convert.ToDecimal(retailTrans.PartnerData.GSSAvgRate);
                    dPrevGSSSaleWt = Convert.ToDecimal(retailTrans.PartnerData.GSSSaleWt);
                }

                if(IsSaleAdjustment)
                {
                    dSaleAdjustmentGoldAmt = Convert.ToDecimal(retailTrans.PartnerData.SaleAdjustmentGoldAmt);
                    dSaleAdjustmentGoldQty = Convert.ToDecimal(retailTrans.PartnerData.SaleAdjustmentGoldQty);

                    if(dSaleAdjustmentGoldQty > 0)
                    {
                        dSaleAdjustmentAvgGoldRate = (dSaleAdjustmentGoldAmt / dSaleAdjustmentGoldQty);
                    }
                    else
                    {
                        dSaleAdjustmentAvgGoldRate = 0;
                    }

                    dPrevTransAdjGoldQty = Convert.ToDecimal(retailTrans.PartnerData.TransAdjGoldQty);
                }

                BindIngredientGrid();

                FillQtyPcsFromSKUTable();

                iCallfrombase = 1;

                alist = al;
                FnArrayList();

                iCallfrombase = 0;

                if(isMRPUCP)
                {
                    txtRate.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(txtRate.Text), 3, MidpointRounding.AwayFromZero));
                    txtMakingRate.Text = string.Empty;   // On Request of Urvi
                    cmbRateType.SelectedIndex = (int)RateType.Tot;   // On Request of Urvi

                }


                txtQuantity.Text = !string.IsNullOrEmpty(txtQuantity.Text) ? decimal.Round(Convert.ToDecimal(txtQuantity.Text), 3, MidpointRounding.AwayFromZero).ToString() : string.Empty;
                txtRate.Text = !string.IsNullOrEmpty(txtRate.Text) ? decimal.Round(Convert.ToDecimal(txtRate.Text), 2, MidpointRounding.AwayFromZero).ToString() : string.Empty;
                txtPCS.Text = !string.IsNullOrEmpty(txtPCS.Text) ? decimal.Round(Convert.ToDecimal(txtPCS.Text), 0, MidpointRounding.AwayFromZero).ToString() : string.Empty;

                if(!string.IsNullOrEmpty(txtQuantity.Text) && !isMRPUCP)
                {
                    CheckRateFromDB();

                    #region new Making disc
                    //string sFieldName = string.Empty;
                    //int iSPId = 0;
                    //iSPId = getUserDiscountPermissionId();
                    decimal dinitDiscValue = 0;

                    decimal dQty = 0;
                    if(!string.IsNullOrEmpty(txtQuantity.Text))
                        dQty = Convert.ToDecimal(txtQuantity.Text);
                    else
                        dQty = 0;
                    if(!isMRPUCP)
                        dinitDiscValue = GetMkDiscountFromDiscPolicy(sBaseItemID, dQty, "OPENINGDISCPCT");// get OPENINGDISCPCT field value FOR THE OPENING
                    //else
                    //    MessageBox.Show("Invalid discount policy for this item");

                    decimal dMkPerDisc = 0;
                    if(!string.IsNullOrEmpty(txtMakingDisc.Text))
                        dMkPerDisc = Convert.ToDecimal(txtMakingDisc.Text);
                    else
                        dMkPerDisc = 0;

                    if(dinitDiscValue > 0)
                        txtMakingDisc.Enabled = false;
                    else
                        txtMakingDisc.Enabled = true;

                    if(dinitDiscValue >= 0)
                    {
                        if((dMkPerDisc > dinitDiscValue))
                        {
                            MessageBox.Show("Line discount percentage should not more than '" + dinitDiscValue + "'");
                            txtMakingDisc.Focus();
                        }
                        //else
                        //{
                        //    CheckMakingDiscountFromDB(dinitDiscValue);
                        //}
                    }
                    //else
                    //{
                    //    MessageBox.Show("Not allowed for this item");
                    //}
                    CheckMakingDiscountFromDB(dinitDiscValue);
                    #endregion
                }


                txtAmount.Text = !string.IsNullOrEmpty(txtAmount.Text) ? decimal.Round(Convert.ToDecimal(txtAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : string.Empty;
                txtTotalAmount.Text = !string.IsNullOrEmpty(txtTotalAmount.Text) ? decimal.Round(Convert.ToDecimal(txtTotalAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : string.Empty;
                txtMakingAmount.Text = !string.IsNullOrEmpty(txtMakingAmount.Text) ? decimal.Round(Convert.ToDecimal(txtMakingAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : string.Empty;

                if(string.IsNullOrEmpty(txtMakingRate.Text))
                    txtMakingRate.Text = "0";
            }

        }

        #region Qty and Pcs Fill up
        private void FillQtyPcsFromSKUTable()
        {
            if(!isViewing)
            {
                if(rdbSale.Checked)
                {
                    if(conn.State == ConnectionState.Closed)
                        conn.Open();

                    string commandText = " SELECT     TOP (1) SKUTableTrans.PDSCWQTY AS PCS , SKUTableTrans.QTY AS QTY " + //SKU Table New
                                         " FROM         SKUTableTrans " +
                                         " WHERE     (SKUTableTrans.SkuNumber = '" + ItemID.Trim() + "') ";

                    SqlCommand command = new SqlCommand(commandText, conn);
                    command.CommandTimeout = 0;

                    using(SqlDataReader reader = command.ExecuteReader())
                    {
                        if(reader.HasRows)
                        {
                            while(reader.Read())
                            {
                                txtPCS.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(reader.GetValue(0)), 3, MidpointRounding.AwayFromZero));
                                txtQuantity.Text = Convert.ToString(reader.GetValue(1));

                            }
                        }
                    }
                    if(conn.State == ConnectionState.Open)
                        conn.Close();

                }
            }

        }
        #endregion

        private void FnArrayList()
        {
            if(alist.Count == 0)
            {
                alist.Add("NO CUSTOMER ORDER");
                cmbCustomerOrder.DataSource = alist;
                cmbCustomerOrder.Enabled = false;

                btnSelectItems.Visible = false;
            }
            else if(alist.Count == 1 && alist.Contains(Convert.ToString("NO CUSTOMER ORDER")))
            {

                cmbCustomerOrder.Enabled = false;

                btnSelectItems.Visible = false;
            }
            else
            {
                if(!alist.Contains(Convert.ToString("NO SELECTION")))
                {

                    alist.Add("NO SELECTION");
                    cmbCustomerOrder.DataSource = alist;
                    cmbCustomerOrder.Enabled = true;

                    btnSelectItems.Visible = true;
                }
                else if(alist.Count > 1)
                {
                    cmbCustomerOrder.DataSource = alist;
                    cmbCustomerOrder.Enabled = true;

                    btnSelectItems.Visible = true;
                }

            }
        }

        private void BindIngredientGrid()
        {
            if(rdbSale.Checked)
            {
                dtIngredients = new DataTable();

                if(conn.State == ConnectionState.Closed)
                    conn.Open();

                StringBuilder commandText = new StringBuilder();
                commandText.Append("  IF EXISTS(SELECT TOP(1) [SkuNumber] FROM [SKULine_Posted] WHERE  [SkuNumber] = '" + ItemID.Trim() + "')");
                commandText.Append("  BEGIN  ");
                //commandText.Append(" SELECT [SkuNumber] ,[SkuDate] ,[LType] ,[ItemID] ,[InventDimID] ,[InventLocationID] ,[InventSizeID] ,[InventColorID]  ");
                //commandText.Append("  ,[ConfigID] ,[InventBatchID],[PCS],[GrWeight] ,[QTY] ,[CValue] ,[Rate] ,[Ctype],[InventSiteId] ,[UnitID] FROM [SKULine_Posted] ");
                //commandText.Append("  END ");
                #region Query Changed
                //commandText.Append(" SELECT [SkuNumber] ,[SkuDate] ,[ItemID] ,[InventDimID] ,[InventSizeID] ,[InventColorID]   ");
                //commandText.Append("   ,[ConfigID] ,[InventBatchID],[PDSCWQTY] AS PCS,[QTY] ,[CValue] ,[Rate] ,[UnitID] FROM [SKULine_Posted] ");
                //commandText.Append("  END ");
                #endregion

                /* 
                 
                commandText.Append(" SELECT     SKULine_Posted.SkuNumber, SKULine_Posted.SkuDate, SKULine_Posted.ItemID, INVENTDIM.InventDimID, INVENTDIM.InventSizeID,  ");
                commandText.Append(" INVENTDIM.InventColorID, INVENTDIM.ConfigID, INVENTDIM.InventBatchID, CAST(SKULine_Posted.PDSCWQTY AS INT) AS PCS, CAST(SKULine_Posted.QTY AS NUMERIC(16,3)) AS QTY,  ");
                commandText.Append(" SKULine_Posted.CValue, SKULine_Posted.CRate AS Rate, SKULine_Posted.UnitID ");
                commandText.Append(" FROM         SKULine_Posted INNER JOIN ");
                commandText.Append(" INVENTDIM ON SKULine_Posted.InventDimID = INVENTDIM.INVENTDIMID ");
                commandText.Append(" WHERE INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "' ");
                commandText.Append("  AND  [SkuNumber] = '" + ItemID.Trim() + "' END ");
                 */

                commandText.Append(" SELECT     SKULine_Posted.SkuNumber, SKULine_Posted.SkuDate, SKULine_Posted.ItemID, INVENTDIM.InventDimID, INVENTDIM.InventSizeID,  ");
                // commandText.Append(" INVENTDIM.InventColorID, INVENTDIM.ConfigID, INVENTDIM.InventBatchID, CAST(SKULine_Posted.PDSCWQTY AS INT) AS PCS, CAST(SKULine_Posted.QTY AS NUMERIC(16,3)) AS QTY,  ");
                commandText.Append(" INVENTDIM.InventColorID, INVENTDIM.ConfigID, INVENTDIM.InventBatchID, CAST(ISNULL(SKULine_Posted.PDSCWQTY,0) AS INT) AS PCS, CAST(ISNULL(SKULine_Posted.QTY,0) AS NUMERIC(16,3)) AS QTY,  ");
                commandText.Append(" SKULine_Posted.CValue, SKULine_Posted.CRate AS Rate, SKULine_Posted.UnitID,X.METALTYPE");
                //  commandText.Append(" ,0 AS IngrdDiscType,0 AS IngrdDiscAmt,0 AS IngrdDiscTotAmt "); 
                commandText.Append(" FROM         SKULine_Posted INNER JOIN ");
                commandText.Append(" INVENTDIM ON SKULine_Posted.InventDimID = INVENTDIM.INVENTDIMID ");
                commandText.Append(" INNER JOIN INVENTTABLE X ON SKULine_Posted.ITEMID = X.ITEMID ");
                commandText.Append(" WHERE INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "' ");
                commandText.Append("  AND  [SkuNumber] = '" + ItemID.Trim() + "' ORDER BY X.METALTYPE END ");


                SqlCommand command = new SqlCommand(commandText.ToString(), conn);
                command.CommandTimeout = 0;

                using(SqlDataReader reader = command.ExecuteReader())
                {
                    dtIngredients.Load(reader);
                }

                if(dtIngredients != null && dtIngredients.Rows.Count > 0)
                {
                    #region // Stone Discount

                    dtIngredients.Columns.Add("IngrdDiscType", typeof(int));
                    dtIngredients.Columns.Add("IngrdDiscAmt", typeof(decimal));
                    dtIngredients.Columns.Add("IngrdDiscTotAmt", typeof(decimal));
                    dtIngredients.Columns.Add("CTYPE", typeof(int));

                    #endregion

                    txtRate.Text = string.Empty;
                    dtIngredientsClone = new DataTable();
                    dtIngredientsClone = dtIngredients.Clone();
                    // dtIngredientsClone.Columns["LType"].DataType = typeof(string);
                    //   dtIngredientsClone.Columns["CType"].DataType = typeof(string);

                    foreach(DataRow drClone in dtIngredients.Rows)
                    {
                        if(isMRPUCP)
                        {
                            drClone["CValue"] = 0;
                            drClone["Rate"] = 0;
                            drClone["CTYPE"] = 0;
                            drClone["IngrdDiscType"] = 0;
                            drClone["IngrdDiscAmt"] = 0;
                            drClone["IngrdDiscTotAmt"] = 0;
                        }
                        dtIngredientsClone.ImportRow(drClone);
                    }

                    if(!isMRPUCP)
                    {
                        foreach(DataRow dr in dtIngredientsClone.Rows)
                        {
                            string item = ItemID;
                            string config = ConfigID;
                            ConfigID = string.Empty;
                            ItemID = string.Empty;
                            ConfigID = Convert.ToString(dr["ConfigID"]);
                            ItemID = Convert.ToString(dr["ItemID"]);
                            BatchID = Convert.ToString(dr["InventBatchID"]);
                            ColorID = Convert.ToString(dr["InventColorID"]);
                            SizeID = Convert.ToString(dr["InventSizeID"]);
                            GrWeight = Convert.ToString(dr["QTY"]);
                            string sRate = string.Empty;
                            string sCalcType = "";

                            if(Convert.ToDecimal(dr["PCS"]) > 0)
                            {
                                dStoneWtRange = decimal.Round(Convert.ToDecimal(dr["QTY"]) / Convert.ToDecimal(dr["PCS"]), 3, MidpointRounding.AwayFromZero);
                            }
                            else
                            {
                                dStoneWtRange = decimal.Round(Convert.ToDecimal(dr["QTY"]), 3, MidpointRounding.AwayFromZero);
                            }

                            // Stone Discount
                            int iStoneDiscType = 0;
                            decimal dStoneDiscAmt = 0;
                            decimal dStoneDiscTotAmt = 0m;

                            if((IsGSSTransaction) && (dGSSMaturityQty > 0) && (Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Gold))
                            {
                                #region GSS New
                                //sRate = Convert.ToString(getGSSRate(GrWeight));
                                sRate = Convert.ToString(getGSSRate(GrWeight, ConfigID));
                                sGSSAdjustmentGoldRate = sRate;

                                #endregion

                                #region GSS
                                // //decimal dActualSaleQty = 0;
                                // //decimal dGSSMaturityQty = //Convert.ToDecimal(retailTrans.PartnerData.GSSTotQty);

                                //// if(!string.IsNullOrEmpty(txtQuantity.Text))
                                // if (!string.IsNullOrEmpty(GrWeight))
                                // {
                                //     if (dPrevRunningQtyGSS == 0)
                                //     {
                                //         dActualGSSSaleWt = dPrevGSSSaleWt + Convert.ToDecimal(GrWeight);
                                //        // dPrevRunningQtyGSS += Convert.ToDecimal(GrWeight);
                                //     }
                                //     else
                                //     {
                                //         dActualGSSSaleWt = dPrevGSSSaleWt + Convert.ToDecimal(GrWeight) + dPrevRunningQtyGSS;
                                //     }
                                // }

                                // if (dActualGSSSaleWt <= dGSSMaturityQty)
                                // {
                                //     sRate = Convert.ToString(dGSSAvgRate);
                                // }
                                // else if (dActualGSSSaleWt > dGSSMaturityQty)
                                // {
                                //     if (dPrevGSSSaleWt >= dGSSMaturityQty)
                                //     {
                                //         sRate = getRateFromMetalTable();
                                //     }
                                //     else
                                //     {
                                //         //  if ((dPrevGSSSaleWt + Convert.ToDecimal(GrWeight)+ dPrevRunningQtyGSS ) == dGSSMaturityQty)
                                //         if (dPrevGSSSaleWt + dPrevRunningQtyGSS >= dGSSMaturityQty)
                                //         {
                                //             sRate = getRateFromMetalTable();
                                //         }
                                //         else
                                //         {
                                //             decimal dAvgRateQty = 0m;
                                //             decimal dCurrentRateQty = 0m;
                                //             decimal dCurrentRate = 0m;

                                //             dCurrentRateQty = (dActualGSSSaleWt - dGSSMaturityQty - dPrevRunningQtyGSS);
                                //             dAvgRateQty = Convert.ToDecimal(GrWeight) - dCurrentRateQty;

                                //             dCurrentRate = getMetalRate(ItemID, ConfigID);

                                //             if (dGSSAvgRate > 0 && dCurrentRate > 0)
                                //             {
                                //                 sRate = Convert.ToString(((dGSSAvgRate * dAvgRateQty) + (dCurrentRateQty * dCurrentRate)) / Convert.ToDecimal(GrWeight));
                                //             }
                                //         }
                                //     }

                                // }


                                #endregion

                            }
                            else if(IsSaleAdjustment   // Avg Gold Rate Adjustment
                                    && (Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Gold)
                                    && (dSaleAdjustmentAvgGoldRate > 0))
                            {
                                sRate = Convert.ToString(getAdjustmentRate(GrWeight, ConfigID));

                                sAdvAdjustmentGoldRate = sRate;
                            }

                            else
                            {
                                sRate = getRateFromMetalTable();
                            }

                            //  sRate = getRateFromMetalTable();

                            // ------*********


                            if(!string.IsNullOrEmpty(sRate))
                            {
                                sCalcType = GetIngredientCalcType(ref iStoneDiscType, ref dStoneDiscAmt);
                                if(!string.IsNullOrEmpty(sCalcType))
                                    dr["CTYPE"] = Convert.ToInt32(sCalcType);
                                else
                                    dr["CTYPE"] = 0;

                                dr["Rate"] = sRate;
                                ItemID = item;
                                ConfigID = config;
                                BatchID = string.Empty;
                                ColorID = string.Empty;
                                SizeID = string.Empty;
                                GrWeight = string.Empty;
                            }
                            else
                            {
                                dr["Rate"] = "0"; // Added on 08.08.2013 -- Instructed by Urminavo Das 
                                // if not rate found make it 0 -- validation related issues will be raised by RGJL in CR
                                dr["CTYPE"] = 0;

                                ItemID = item;
                                ConfigID = config;
                                BatchID = string.Empty;
                                ColorID = string.Empty;
                                SizeID = string.Empty;
                                GrWeight = string.Empty;
                            }

                            // If Ingredient item is LooseDmd or Stone and stone rate is 0 -- cancel the operation 

                            if((Convert.ToDecimal(dr["Rate"]) <= 0)
                                && (Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.LooseDmd
                                    || Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Stone))
                            {
                                using(LSRetailPosis.POSProcesses.frmMessage message = new LSRetailPosis.POSProcesses.frmMessage("0 Stone Rate is not valid for this item ", MessageBoxButtons.OK, MessageBoxIcon.Error))
                                {
                                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(message);
                                }
                                isCancelClick = true;
                                this.Close();
                                return;
                            }

                            //

                            StringBuilder commandText1 = new StringBuilder();
                            commandText1.Append("select metaltype from inventtable where itemid='" + Convert.ToString(dr["ItemID"]) + "'");

                            if(conn.State == ConnectionState.Closed)
                                conn.Open();

                            SqlCommand command1 = new SqlCommand(commandText1.ToString(), conn);
                            command1.CommandTimeout = 0;
                            SqlDataReader reader1 = command1.ExecuteReader();
                            if(reader1.HasRows)
                            {
                                #region Reader
                                while(reader1.Read())
                                {
                                    //---- add code --  -- for CalcType
                                    if(((int)reader1.GetValue(0) == (int)MetalType.LooseDmd) || ((int)reader1.GetValue(0) == (int)MetalType.Stone))
                                    {

                                        if(!string.IsNullOrEmpty(sCalcType))
                                        {
                                            #region // Stone Discount Calculation


                                            #endregion

                                            if(Convert.ToInt32(sCalcType) == Convert.ToInt32(RateType.Weight))
                                            {
                                                if(dStoneDiscAmt > 0)
                                                {
                                                    dStoneDiscTotAmt = CalcStoneDiscount(dStoneDiscAmt, iStoneDiscType, Convert.ToDecimal(dr["QTY"]), Convert.ToDecimal(dr["Rate"]));
                                                }
                                                // dr["CValue"] = decimal.Round(Convert.ToDecimal(dr["QTY"]) * Convert.ToDecimal(dr["Rate"]), 2, MidpointRounding.AwayFromZero);
                                                dr["CValue"] = decimal.Round((Convert.ToDecimal(dr["QTY"]) * Convert.ToDecimal(dr["Rate"])) - dStoneDiscTotAmt, 2, MidpointRounding.AwayFromZero);
                                            }

                                            else if(Convert.ToInt32(sCalcType) == Convert.ToInt32(RateType.Pieces))
                                            {
                                                if(dStoneDiscAmt > 0)
                                                {
                                                    dStoneDiscTotAmt = CalcStoneDiscount(dStoneDiscAmt, iStoneDiscType, Convert.ToDecimal(dr["PCS"]), Convert.ToDecimal(dr["Rate"]));
                                                }
                                                // dr["CValue"] = decimal.Round(Convert.ToDecimal(dr["PCS"]) * Convert.ToDecimal(dr["Rate"]), 2, MidpointRounding.AwayFromZero);
                                                dr["CValue"] = decimal.Round((Convert.ToDecimal(dr["PCS"]) * Convert.ToDecimal(dr["Rate"])) - dStoneDiscTotAmt, 2, MidpointRounding.AwayFromZero);
                                            }

                                            else // Tot
                                            {
                                                if(dStoneDiscAmt > 0)
                                                {
                                                    dStoneDiscTotAmt = CalcStoneDiscount(dStoneDiscAmt, iStoneDiscType, Convert.ToDecimal(dr["QTY"]), Convert.ToDecimal(dr["Rate"]));
                                                }
                                                //dr["CValue"] = decimal.Round(Convert.ToDecimal(dr["Rate"]), 2, MidpointRounding.AwayFromZero);
                                                dr["CValue"] = decimal.Round(Convert.ToDecimal(dr["Rate"]) - dStoneDiscTotAmt, 2, MidpointRounding.AwayFromZero);
                                            }
                                        }
                                        else
                                        {
                                            if(dStoneDiscAmt > 0)
                                            {
                                                dStoneDiscTotAmt = CalcStoneDiscount(dStoneDiscAmt, iStoneDiscType, Convert.ToDecimal(dr["QTY"]), Convert.ToDecimal(dr["Rate"]));
                                            }
                                            // dr["CValue"] = decimal.Round(Convert.ToDecimal(dr["QTY"]) * Convert.ToDecimal(dr["Rate"]), 2, MidpointRounding.AwayFromZero);
                                            dr["CValue"] = decimal.Round((Convert.ToDecimal(dr["QTY"]) * Convert.ToDecimal(dr["Rate"])) - dStoneDiscTotAmt, 2, MidpointRounding.AwayFromZero);
                                        }
                                    }
                                    else
                                    {
                                        //if (dStoneDiscAmt > 0)
                                        //{
                                        //    dStoneDiscTotAmt = CalcStoneDiscount(dStoneDiscAmt, iStoneDiscType, Convert.ToDecimal(dr["QTY"]), Convert.ToDecimal(dr["Rate"]));
                                        //}

                                        dr["CValue"] = decimal.Round(Convert.ToDecimal(dr["QTY"]) * Convert.ToDecimal(dr["Rate"]), 2, MidpointRounding.AwayFromZero);
                                        // dr["CValue"] = decimal.Round((Convert.ToDecimal(dr["QTY"]) * Convert.ToDecimal(dr["Rate"])) - dStoneDiscTotAmt, 2, MidpointRounding.AwayFromZero);
                                    }

                                    // --- end

                                    if((int)reader1.GetValue(0) == (int)MetalType.Gold)
                                    {
                                        txtgval.Text = (string.IsNullOrEmpty(txtRate.Text)) ? Convert.ToString(dr["CValue"]) : Convert.ToString(decimal.Round(Convert.ToDecimal(txtgval.Text) + Convert.ToDecimal(dr["CValue"]), 2, MidpointRounding.AwayFromZero));
                                    }
                                }
                                #endregion
                            }
                            if(conn.State == ConnectionState.Open)
                                conn.Close();

                            // Stone Discount
                            if(dStoneDiscAmt > 0)
                            {
                                dr["IngrdDiscType"] = iStoneDiscType;
                                dr["IngrdDiscAmt"] = dStoneDiscAmt;
                                dr["IngrdDiscTotAmt"] = decimal.Round(dStoneDiscTotAmt, 2, MidpointRounding.AwayFromZero);

                            }
                            else
                            {
                                dr["IngrdDiscType"] = 0;
                                dr["IngrdDiscAmt"] = 0;
                                dr["IngrdDiscTotAmt"] = 0;
                            }

                            //

                            txtRate.Text = (string.IsNullOrEmpty(txtRate.Text)) ? Convert.ToString(dr["CValue"]) : Convert.ToString(decimal.Round(Convert.ToDecimal(txtRate.Text) + Convert.ToDecimal(dr["CValue"]), 2, MidpointRounding.AwayFromZero));

                        }

                        dtIngredientsClone.AcceptChanges();

                        cmbRateType.SelectedIndex = (int)RateType.Tot;
                        cmbRateType.Enabled = false;

                        // Added on 07.06.2013
                        txtPCS.Enabled = false;
                        txtQuantity.Enabled = false;
                        txtRate.Enabled = false;
                        txtMakingRate.Enabled = false;
                        txtMakingDisc.Enabled = false;
                        cmbMakingType.Enabled = false;
                        // txtPurity.Enabled = false;
                        // btnAdd.Focus();
                        //
                    }
                    else
                    {
                        txtRate.Text = MRPUCP;
                    }
                    btnIngrdientsDetails.Visible = true;
                }
                else
                {
                    if(!isMRPUCP)

                        #region [ GSS Transaction]
                        if(IsGSSTransaction)
                        {
                            int iMetalType = GetMetalType(ItemID);
                            //getMetalRate(string sItemId, string sConfigId)
                            if(iMetalType == (int)MetalType.Gold)
                            {
                                if(!string.IsNullOrEmpty(txtQuantity.Text))
                                {
                                    decimal dMetalRate = 0;

                                    dMetalRate = getGSSRate(txtQuantity.Text.Trim(), ConfigID);
                                    if(dMetalRate > 0)
                                    {
                                        txtRate.Text = Convert.ToString(dMetalRate);
                                    }
                                    else
                                    {
                                        txtRate.Text = getRateFromMetalTable();
                                    }

                                }
                                else
                                {
                                    decimal dSKUQty = 0;

                                    dSKUQty = GetSKUQty(ItemID);

                                    if(dSKUQty > 0)
                                    {
                                        decimal dMetalRate = 0;

                                        dMetalRate = getGSSRate(Convert.ToString(dSKUQty), ConfigID);
                                        if(dMetalRate > 0)
                                        {
                                            txtRate.Text = Convert.ToString(dMetalRate);
                                        }
                                        else
                                        {
                                            txtRate.Text = getRateFromMetalTable();
                                        }
                                    }
                                    else
                                    {
                                        txtRate.Text = getRateFromMetalTable();
                                    }

                                }
                            }
                            else
                            {
                                txtRate.Text = getRateFromMetalTable();
                            }

                        }
                        #endregion

                        #region [Sale Adjustment] // Avg Gold Rate Adjustment
                        else if(IsSaleAdjustment)
                        {
                            int iMetalType = GetMetalType(ItemID);
                            //getMetalRate(string sItemId, string sConfigId)
                            if(iMetalType == (int)MetalType.Gold)
                            {
                                if(!string.IsNullOrEmpty(txtQuantity.Text))
                                {
                                    decimal dMetalRate = 0;

                                    dMetalRate = getAdjustmentRate(txtQuantity.Text.Trim(), ConfigID);
                                    if(dMetalRate > 0)
                                    {
                                        txtRate.Text = Convert.ToString(dMetalRate);
                                    }
                                    else
                                    {
                                        txtRate.Text = getRateFromMetalTable();
                                    }

                                }
                                else
                                {
                                    decimal dSKUQty = 0;

                                    dSKUQty = GetSKUQty(ItemID);

                                    if(dSKUQty > 0)
                                    {
                                        decimal dMetalRate = 0;

                                        dMetalRate = getAdjustmentRate(Convert.ToString(dSKUQty), ConfigID);
                                        if(dMetalRate > 0)
                                        {
                                            txtRate.Text = Convert.ToString(dMetalRate);
                                        }
                                        else
                                        {
                                            txtRate.Text = getRateFromMetalTable();
                                        }
                                    }
                                    else
                                    {
                                        txtRate.Text = getRateFromMetalTable();
                                    }

                                }
                            }
                            else
                            {
                                txtRate.Text = getRateFromMetalTable();
                            }

                        }//
                        #endregion

                        else
                        {
                            txtRate.Text = getRateFromMetalTable();
                        }
                    // txtRate.Text = getRateFromMetalTable();
                    else
                        txtRate.Text = MRPUCP;
                    btnIngrdientsDetails.Visible = false;
                }


                if(conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        public frmCustomFieldCalculations(string sPcs, string sQty, string sRate, string sRateType, string sMRate, string sMType, string sAmt,
                                      string sMDisc, string sMAmt, string sTAmt, string sTWeight, string sLossPct, string sLossWt,
                                      string sEQty, string sTType, string sOChecked, DataSet dsIng, string ordernum, string orderlinenum,
                                      string sWastageType, string sWastagePercentage, string sWastageQty, string sWastageAmount,
                                      string sMkDiscType, string sTotMkAmt, string sPurity, SqlConnection connection) // changed for wastage // Making Discount
        {
            InitializeComponent();
            btnCLose.Visible = false;
            BindRateTypeMakingTypeCombo();
            BindWastage();
            BindMakingDiscount();
            isViewing = true;
            conn = connection;
            panel1.Enabled = false;

            panel3.Enabled = false;
            btnAdd.Enabled = false;

            if(!string.IsNullOrEmpty(sPcs))
                txtPCS.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(sPcs), 3, MidpointRounding.AwayFromZero));
            else
                txtPCS.Text = sPcs;
            if(!string.IsNullOrEmpty(sQty))
                txtQuantity.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(sQty), 3, MidpointRounding.AwayFromZero));
            else
                txtQuantity.Text = sQty;

            if(!string.IsNullOrEmpty(sMAmt))
                txtMakingAmount.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(sMAmt), 2, MidpointRounding.AwayFromZero));
            else
                txtMakingAmount.Text = sMAmt;

            if(!string.IsNullOrEmpty(sRate))
                txtRate.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(sRate), 2, MidpointRounding.AwayFromZero));
            else
                txtRate.Text = sRate;

            cmbRateType.SelectedIndex = Convert.ToInt16(sRateType);

            switch(Convert.ToInt16(sMType))
            {
                case 0:
                    cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Pieces));
                    break;
                case 2:
                    cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Weight));
                    break;
                case 3:
                    cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Tot));
                    break;
                case 4:
                    cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Percentage));
                    break;
                default:
                    cmbMakingType.SelectedIndex = 0;
                    break;
            }


            txtMakingRate.Text = Convert.ToString(oRounding.Round(Convert.ToDecimal(sMRate), 2));

            if(!string.IsNullOrEmpty(sAmt))
                txtAmount.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(sAmt), 2, MidpointRounding.AwayFromZero));
            else
                txtAmount.Text = sAmt;

            txtMakingDisc.Text = sMDisc;

            if(!string.IsNullOrEmpty(sTAmt))
                txtTotalAmount.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(sTAmt), 2, MidpointRounding.AwayFromZero));
            else
                txtTotalAmount.Text = sTAmt;

            if(!string.IsNullOrEmpty(sTWeight))
                txtTotalWeight.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(sTWeight), 3, MidpointRounding.AwayFromZero));
            else
                txtTotalWeight.Text = sTWeight;

            if(!string.IsNullOrEmpty(sLossPct))
                txtLossPct.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(sLossPct), 2, MidpointRounding.AwayFromZero));
            else
                txtLossPct.Text = sLossPct;

            if(!string.IsNullOrEmpty(sLossWt))
                txtLossWeight.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(sLossWt), 3, MidpointRounding.AwayFromZero));
            else
                txtLossWeight.Text = sLossWt;

            if(!string.IsNullOrEmpty(sPurity)) // -- 
                txtPurity.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(sPurity), 4, MidpointRounding.AwayFromZero));
            else
                txtPurity.Text = sPurity;

            if(!string.IsNullOrEmpty(sEQty))
                txtExpectedQuantity.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(sEQty), 3, MidpointRounding.AwayFromZero));
            else
                txtExpectedQuantity.Text = sEQty;

            if(Convert.ToInt16(sTType) == (int)TransactionType.Sale)
            {
                rdbSale.Checked = true;
                chkOwn.Visible = false;
                if(dsIng != null && dsIng.Tables.Count > 0 && dsIng.Tables[0].Rows.Count > 0)
                {
                    dtIngredientsClone = dsIng.Tables[0];
                    btnIngrdientsDetails.Visible = true;
                }
                else
                {
                    dtIngredientsClone = new DataTable();
                    btnIngrdientsDetails.Visible = false;
                }

                cmbCustomerOrder.Enabled = false;
                alist = new ArrayList();
                if(!string.IsNullOrEmpty(ordernum))
                {
                    alist.Add(ordernum);
                    cmbCustomerOrder.DataSource = alist;


                }
                else
                {
                    alist.Add("NO CUSTOMER ORDER");
                    cmbCustomerOrder.DataSource = alist;
                    btnSelectItems.Visible = false;
                }

                if(string.IsNullOrEmpty(orderlinenum) || orderlinenum == "0")
                    lblItemSelected.Text = "NO ITEM SELECTED";

                else
                {
                    lblItemSelected.Text = " SELECTED LINE NO. : " + orderlinenum;
                    btnSelectItems.Visible = true;
                    LineNum = orderlinenum;

                }

                if(!string.IsNullOrEmpty(sWastageType))
                    cmbWastage.SelectedIndex = Convert.ToInt32(sWastageType);
                else
                    cmbWastage.SelectedIndex = 0;

                if(!string.IsNullOrEmpty(sWastageQty))
                    txtWastageQty.Text = sWastageQty;
                else
                    txtWastageQty.Text = "0";

                if(!string.IsNullOrEmpty(sWastageAmount))
                    txtWastageAmount.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(sWastageAmount), 2, MidpointRounding.AwayFromZero));
                else
                    txtWastageAmount.Text = "0";

                if(!string.IsNullOrEmpty(sWastagePercentage) && (cmbWastage.SelectedIndex == 1))
                    txtWastagePercentage.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(sWastagePercentage), 2, MidpointRounding.AwayFromZero));
                else
                    txtWastagePercentage.Text = string.Empty;

                // Making Discount Type
                if(!string.IsNullOrEmpty(sMkDiscType))
                    cmbMakingDiscType.SelectedIndex = Convert.ToInt32(sMkDiscType);
                else
                    cmbMakingDiscType.SelectedIndex = 0;

                if(!string.IsNullOrEmpty(sTotMkAmt))
                    txtMakingDiscTotAmt.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(sTotMkAmt), 2, MidpointRounding.AwayFromZero));
                else
                    txtMakingDiscTotAmt.Text = "0";

                //
            }
            else
            {

                if(Convert.ToInt16(sTType) == (int)TransactionType.Purchase)
                    rdbPurchase.Checked = true;
                else if(Convert.ToInt16(sTType) == (int)TransactionType.Exchange)
                    rdbExchange.Checked = true;
                else if(Convert.ToInt16(sTType) == (int)TransactionType.ExchangeReturn)
                    rdbExchangeReturn.Checked = true;
                else if(Convert.ToInt16(sTType) == (int)TransactionType.PurchaseReturn)
                    rdbPurchReturn.Checked = true;
                chkOwn.Visible = true;
                chkOwn.Checked = Convert.ToBoolean(sOChecked);

            }
        }
        #endregion

        #region Bind Rate Type and making Type Combo
        void BindRateTypeMakingTypeCombo()
        {
            cmbRateType.DataSource = Enum.GetValues(typeof(RateType));
            cmbMakingType.DataSource = Enum.GetValues(typeof(MakingType));
        }
        #endregion

        #region Bind Wastage Combo

        void BindWastage()
        {
            cmbWastage.DataSource = Enum.GetValues(typeof(WastageType));
        }

        #endregion

        #region Bind Making Discount Combo

        void BindMakingDiscount()
        {
            cmbMakingDiscType.DataSource = Enum.GetValues(typeof(MakingDiscType));
        }

        #endregion


        #region Btn Cancel Click
        private void btnCancel_Click(object sender, EventArgs e)
        {
            isCancelClick = true;
            this.Close();
        }
        #endregion

        #region Submit Button Click
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if(ValidateControls())
            {
                if(retailTrans != null)
                {
                    if((string.IsNullOrEmpty(retailTrans.Customer.CustomerId))
                        && (string.IsNullOrEmpty(Convert.ToString(retailTrans.PartnerData.LCCustomerName))))
                    {
                        MessageBox.Show("Customer not selected");
                    }
                    else
                        sCustomerId = retailTrans.Customer.CustomerId;
                }

                if(rdbSale.Checked)
                {
                    if(string.IsNullOrEmpty(txtMakingDisc.Text))
                    {
                        txtMakingDisc.Text = "0";
                    }
                    BuildSaleList();

                    purchaseList = new List<KeyValuePair<string, string>>();

                    if(IsGSSTransaction)
                    {
                        retailTrans.PartnerData.RunningQtyGSS = dPrevRunningQtyGSS;
                        retailTrans.PartnerData.GSSSaleWt = dActualGSSSaleWt;
                    }

                    if(IsSaleAdjustment)
                    {
                        retailTrans.PartnerData.RunningQtyAdjustment = dPrevRunningTransAdjGoldQty;
                        retailTrans.PartnerData.TransAdjGoldQty = dActualTransAdjGoldQty;
                    }

                }
                else
                {
                    BuildPurchaseList();

                    saleList = new List<KeyValuePair<string, string>>();

                }

                this.Close();
            }
        }
        #endregion

        #region Key Press Events
        private void txtPCS_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            if(e.KeyChar == (Char)Keys.Enter)
            {
                e.Handled = true;
                btnAdd_Click(sender, e);
            }
        }




        private void txtQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(txtQuantity.Text == string.Empty && e.KeyChar == '.')
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
                    btnAdd_Click(sender, e);
                }
            }
        }

        private void txtRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(txtRate.Text == string.Empty && e.KeyChar == '.')
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
                    btnAdd_Click(sender, e);
                }
            }
        }

        private void txtMakingRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(txtMakingRate.Text == string.Empty && e.KeyChar == '.')
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
                    btnAdd_Click(sender, e);
                }
            }
        }

        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
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
                btnAdd_Click(sender, e);
            }
        }

        private void txtMakingDisc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(txtMakingDisc.Text == string.Empty && e.KeyChar == '.')
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
                    btnAdd_Click(sender, e);
                }
            }
        }

        private void txtMakingAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(txtMakingAmount.Text == string.Empty && e.KeyChar == '.')
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
                    btnAdd_Click(sender, e);
                }
            }
        }

        private void txtTotalAmount_KeyPress(object sender, KeyPressEventArgs e)
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
                btnAdd_Click(sender, e);
            }
        }

        private void txtTotalWeight_KeyPress(object sender, KeyPressEventArgs e)
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
                btnAdd_Click(sender, e);
            }
        }

        private void txtPurity_KeyPress(object sender, KeyPressEventArgs e)
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
                btnAdd_Click(sender, e);
            }

        }
        private void txtLossPct_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(txtLossPct.Text == string.Empty && e.KeyChar == '.')
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
                    btnAdd_Click(sender, e);
                }
            }
        }
        #endregion

        #region Pruchase List
        private void BuildPurchaseList()
        {
            purchaseList = new List<KeyValuePair<string, string>>();
            if(rdbPurchase.Checked || rdbExchange.Checked)
                purchaseList.Add(new KeyValuePair<string, string>("Pieces", !string.IsNullOrEmpty(txtPCS.Text.Trim()) ? Convert.ToString((-1) * Convert.ToDecimal(txtPCS.Text.Trim())) : txtPCS.Text.Trim()));
            else
                purchaseList.Add(new KeyValuePair<string, string>("Pieces", txtPCS.Text.Trim()));

            if(rdbPurchase.Checked || rdbExchange.Checked)
                purchaseList.Add(new KeyValuePair<string, string>("Quantity", !string.IsNullOrEmpty(txtQuantity.Text.Trim()) ? Convert.ToString((-1) * Convert.ToDecimal(txtQuantity.Text.Trim())) : txtQuantity.Text.Trim()));
            else
                purchaseList.Add(new KeyValuePair<string, string>("Quantity", txtQuantity.Text.Trim()));

            purchaseList.Add(new KeyValuePair<string, string>("Rate", txtRate.Text.Trim()));

            purchaseList.Add(new KeyValuePair<string, string>("RateType", Convert.ToString(cmbRateType.SelectedIndex)));
            purchaseList.Add(new KeyValuePair<string, string>("MakingRate", "0"));
            purchaseList.Add(new KeyValuePair<string, string>("MakingType", "0"));

            if(rdbPurchase.Checked || rdbExchange.Checked)
                purchaseList.Add(new KeyValuePair<string, string>("Amount", !string.IsNullOrEmpty(txtAmount.Text.Trim()) ? Convert.ToString((-1) * Convert.ToDecimal(txtAmount.Text.Trim())) : txtAmount.Text.Trim()));
            else
                purchaseList.Add(new KeyValuePair<string, string>("Amount", txtAmount.Text.Trim()));

            purchaseList.Add(new KeyValuePair<string, string>("MakingDiscount", "0"));
            purchaseList.Add(new KeyValuePair<string, string>("MakingAmount", "0"));

            if(rdbPurchase.Checked || rdbExchange.Checked)
                purchaseList.Add(new KeyValuePair<string, string>("Amount", !string.IsNullOrEmpty(txtAmount.Text.Trim()) ? Convert.ToString((-1) * Convert.ToDecimal(txtAmount.Text.Trim())) : txtAmount.Text.Trim()));
            else
                purchaseList.Add(new KeyValuePair<string, string>("TotalAmount", txtAmount.Text.Trim()));

            purchaseList.Add(new KeyValuePair<string, string>("TotalWeight", txtTotalWeight.Text.Trim()));
            // purchaseList.Add(new KeyValuePair<string, string>("Purity", txtPurity.Text.Trim())); // 

            purchaseList.Add(new KeyValuePair<string, string>("LossPct", txtLossPct.Text.Trim()));
            purchaseList.Add(new KeyValuePair<string, string>("LossWeight", txtLossWeight.Text.Trim()));
            purchaseList.Add(new KeyValuePair<string, string>("ExpectedQuantity", txtExpectedQuantity.Text.Trim()));
            purchaseList.Add(new KeyValuePair<string, string>("OwnCheckBox", Convert.ToString(chkOwn.Checked)));
            purchaseList.Add(new KeyValuePair<string, string>("OrderNum", string.Empty));
            purchaseList.Add(new KeyValuePair<string, string>("OrderLineNum", string.Empty));
            purchaseList.Add(new KeyValuePair<string, string>("SampleReturn", string.Empty));
            // Added for wastage 
            purchaseList.Add(new KeyValuePair<string, string>("WastageType", "0"));
            purchaseList.Add(new KeyValuePair<string, string>("WastageQty", "0"));
            purchaseList.Add(new KeyValuePair<string, string>("WastageAmount", "0"));
            purchaseList.Add(new KeyValuePair<string, string>("WastagePercentage", "0"));
            purchaseList.Add(new KeyValuePair<string, string>("WastageRate", "0"));
            //
            // Making Discount Type  
            purchaseList.Add(new KeyValuePair<string, string>("MakingDiscountType", "0"));
            purchaseList.Add(new KeyValuePair<string, string>("MakingTotalDiscount", "0"));
            //
            purchaseList.Add(new KeyValuePair<string, string>("ConfigId", sBaseConfigID));
            purchaseList.Add(new KeyValuePair<string, string>("Purity", string.IsNullOrEmpty(txtPurity.Text.Trim()) ? "0" : txtPurity.Text.Trim()));

            //
            if((chkOwn.Checked) && (dtExtndPurchExchng.Rows.Count > 0))
            {
                purchaseList.Add(new KeyValuePair<string, string>("GROSSWT", Convert.ToString(dtExtndPurchExchng.Rows[0]["GROSSWT"])));
                purchaseList.Add(new KeyValuePair<string, string>("GROSSUNIT", Convert.ToString(dtExtndPurchExchng.Rows[0]["GROSSUNIT"])));
                purchaseList.Add(new KeyValuePair<string, string>("DMDWT", Convert.ToString(dtExtndPurchExchng.Rows[0]["DMDWT"])));
                purchaseList.Add(new KeyValuePair<string, string>("DMDPCS", Convert.ToString(dtExtndPurchExchng.Rows[0]["DMDPCS"])));
                purchaseList.Add(new KeyValuePair<string, string>("DMDUNIT", Convert.ToString(dtExtndPurchExchng.Rows[0]["DMDUNIT"])));
                purchaseList.Add(new KeyValuePair<string, string>("DMDAMOUNT", Convert.ToString(dtExtndPurchExchng.Rows[0]["DMDAMOUNT"])));
                purchaseList.Add(new KeyValuePair<string, string>("STONEWT", Convert.ToString(dtExtndPurchExchng.Rows[0]["STONEWT"])));
                purchaseList.Add(new KeyValuePair<string, string>("STONEPCS", Convert.ToString(dtExtndPurchExchng.Rows[0]["STONEPCS"])));
                purchaseList.Add(new KeyValuePair<string, string>("STONEUNIT", Convert.ToString(dtExtndPurchExchng.Rows[0]["STONEUNIT"])));
                purchaseList.Add(new KeyValuePair<string, string>("STONEAMOUNT", Convert.ToString(dtExtndPurchExchng.Rows[0]["STONEAMOUNT"])));
                purchaseList.Add(new KeyValuePair<string, string>("NETWT", Convert.ToString(dtExtndPurchExchng.Rows[0]["NETWT"])));
                purchaseList.Add(new KeyValuePair<string, string>("NETRATE", Convert.ToString(dtExtndPurchExchng.Rows[0]["NETRATE"])));
                purchaseList.Add(new KeyValuePair<string, string>("NETUNIT", Convert.ToString(dtExtndPurchExchng.Rows[0]["NETUNIT"])));
                purchaseList.Add(new KeyValuePair<string, string>("NETPURITY", Convert.ToString(dtExtndPurchExchng.Rows[0]["NETPURITY"])));
                purchaseList.Add(new KeyValuePair<string, string>("NETAMOUNT", Convert.ToString(dtExtndPurchExchng.Rows[0]["NETAMOUNT"])));
                purchaseList.Add(new KeyValuePair<string, string>("REFINVOICENO", Convert.ToString(dtExtndPurchExchng.Rows[0]["REFINVOICENO"])));

                purchaseList.Add(new KeyValuePair<string, string>("FLAT", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("PROMOCODE", ""));


            }
            else
            {
                purchaseList.Add(new KeyValuePair<string, string>("GROSSWT", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("GROSSUNIT", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("DMDWT", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("DMDPCS", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("DMDUNIT", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("DMDAMOUNT", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("STONEWT", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("STONEPCS", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("STONEUNIT", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("STONEAMOUNT", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("NETWT", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("NETRATE", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("NETUNIT", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("NETPURITY", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("NETAMOUNT", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("REFINVOICENO", string.Empty));

                purchaseList.Add(new KeyValuePair<string, string>("FLAT", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("PROMOCODE", string.Empty));

            }

            purchaseList.Add(new KeyValuePair<string, string>("REMARKS", txtRemarks.Text.Trim()));

        }
        #endregion

        #region SaleList
        private void BuildSaleList()
        {
            saleList = new List<KeyValuePair<string, string>>();

            saleList.Add(new KeyValuePair<string, string>("Pieces", txtPCS.Text.Trim()));
            saleList.Add(new KeyValuePair<string, string>("Quantity", txtQuantity.Text.Trim()));
            saleList.Add(new KeyValuePair<string, string>("Rate", txtRate.Text.Trim()));
            saleList.Add(new KeyValuePair<string, string>("RateType", Convert.ToString(cmbRateType.SelectedIndex)));
            saleList.Add(new KeyValuePair<string, string>("MakingRate", txtMakingRate.Text.Trim()));
            int makingtype = 0;
            switch(cmbMakingType.SelectedIndex)
            {
                case 0:
                    makingtype = (int)MakingType.Pieces;
                    break;
                case 1:
                    makingtype = (int)MakingType.Weight;
                    break;
                case 2:
                    makingtype = (int)MakingType.Tot;
                    break;
                case 3:
                    makingtype = (int)MakingType.Percentage;
                    break;
            }
            saleList.Add(new KeyValuePair<string, string>("MakingType", Convert.ToString(makingtype)));
            saleList.Add(new KeyValuePair<string, string>("Amount", txtAmount.Text.Trim()));
            saleList.Add(new KeyValuePair<string, string>("MakingDiscount", txtMakingDisc.Text.Trim()));
            saleList.Add(new KeyValuePair<string, string>("MakingAmount", txtMakingAmount.Text.Trim()));
            saleList.Add(new KeyValuePair<string, string>("TotalAmount", txtTotalAmount.Text.Trim()));
            saleList.Add(new KeyValuePair<string, string>("TotalWeight", "0"));
            saleList.Add(new KeyValuePair<string, string>("LossPct", "0"));
            saleList.Add(new KeyValuePair<string, string>("LossWeight", "0"));
            saleList.Add(new KeyValuePair<string, string>("ExpectedQuantity", "0"));
            saleList.Add(new KeyValuePair<string, string>("OwnCheckBox", Convert.ToString(false)));
            saleList.Add(new KeyValuePair<string, string>("OrderNum", (Convert.ToString(cmbCustomerOrder.SelectedValue).ToUpper().Trim() == "NO CUSTOMER ORDER"
                                                        || (Convert.ToString(cmbCustomerOrder.SelectedValue).ToUpper().Trim() == "NO SELECTION")) ? string.Empty : Convert.ToString(cmbCustomerOrder.SelectedValue)));
            saleList.Add(new KeyValuePair<string, string>("OrderLineNum", Convert.ToString(lineNum)));

            saleList.Add(new KeyValuePair<string, string>("SampleReturn", Convert.ToString(chkSampleReturn.Checked)));

            // Added for wastage 
            saleList.Add(new KeyValuePair<string, string>("WastageType", Convert.ToString(cmbWastage.SelectedIndex)));

            if(!string.IsNullOrEmpty(txtWastageQty.Text))
                saleList.Add(new KeyValuePair<string, string>("WastageQty", Convert.ToString(txtWastageQty.Text)));
            else
                saleList.Add(new KeyValuePair<string, string>("WastageQty", "0"));

            if(!string.IsNullOrEmpty(txtWastageAmount.Text))
                saleList.Add(new KeyValuePair<string, string>("WastageAmount", Convert.ToString(txtWastageAmount.Text)));
            else
                saleList.Add(new KeyValuePair<string, string>("WastageAmount", "0"));
            //--------
            if(!string.IsNullOrEmpty(txtWastagePercentage.Text))
                saleList.Add(new KeyValuePair<string, string>("WastagePercentage", Convert.ToString(txtWastagePercentage.Text)));
            else
                saleList.Add(new KeyValuePair<string, string>("WastagePercentage", "0"));
            //-------------

            //--------
            // if (!string.IsNullOrEmpty(txtRate.Text)) 
            if(dWMetalRate > 0)
                saleList.Add(new KeyValuePair<string, string>("WastageRate", Convert.ToString(dWMetalRate)));
            else
                saleList.Add(new KeyValuePair<string, string>("WastageRate", "0"));
            //-------------

            //--- end

            // Making Discount Type 

            saleList.Add(new KeyValuePair<string, string>("MakingDiscountType", Convert.ToString(cmbMakingDiscType.SelectedIndex)));
            if(!string.IsNullOrEmpty(txtMakingDiscTotAmt.Text))
                saleList.Add(new KeyValuePair<string, string>("MakingTotalDiscount", txtMakingDiscTotAmt.Text.Trim()));
            else
                saleList.Add(new KeyValuePair<string, string>("MakingTotalDiscount", "0"));
            //

            saleList.Add(new KeyValuePair<string, string>("ConfigId", sBaseConfigID));

            saleList.Add(new KeyValuePair<string, string>("Purity", "0"));

            saleList.Add(new KeyValuePair<string, string>("GROSSWT", "0"));
            saleList.Add(new KeyValuePair<string, string>("GROSSUNIT", "0"));
            saleList.Add(new KeyValuePair<string, string>("DMDWT", "0"));
            saleList.Add(new KeyValuePair<string, string>("DMDPCS", "0"));
            saleList.Add(new KeyValuePair<string, string>("DMDUNIT", "0"));
            saleList.Add(new KeyValuePair<string, string>("DMDAMOUNT", "0"));
            saleList.Add(new KeyValuePair<string, string>("STONEWT", "0"));
            saleList.Add(new KeyValuePair<string, string>("STONEPCS", "0"));
            saleList.Add(new KeyValuePair<string, string>("STONEUNIT", "0"));
            saleList.Add(new KeyValuePair<string, string>("STONEAMOUNT", "0"));
            saleList.Add(new KeyValuePair<string, string>("NETWT", "0"));
            saleList.Add(new KeyValuePair<string, string>("NETRATE", "0"));
            saleList.Add(new KeyValuePair<string, string>("NETUNIT", "0"));
            saleList.Add(new KeyValuePair<string, string>("NETPURITY", "0"));
            saleList.Add(new KeyValuePair<string, string>("NETAMOUNT", "0"));
            saleList.Add(new KeyValuePair<string, string>("REFINVOICENO", string.Empty));

            saleList.Add(new KeyValuePair<string, string>("FLAT", Convert.ToString(iIsMkDiscFlat)));
            saleList.Add(new KeyValuePair<string, string>("PROMOCODE", Convert.ToString(sMkPromoCode)));

            saleList.Add(new KeyValuePair<string, string>("REMARKS", txtRemarks.Text.Trim()));

        }
        #endregion

        #region Text Changed
        private void txtPCS_TextChanged(object sender, EventArgs e)
        {
            cmbRateType_SelectedIndexChanged(sender, e);
            cmbMakingType_SelectedIndexChanged(sender, e);
        }

        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            cmbRateType_SelectedIndexChanged(sender, e);
            //if (rdbSale.Checked)
            //{
            //    CheckRateFromDB();
            //    //if (cmbMakingDiscType.SelectedIndex == 0 && dMakingDiscDbAmt > 0) // Weight
            //    //{
            //    //    CalcMakingDiscount(dMakingDiscDbAmt);
            //    //}

            //}
            cmbMakingType_SelectedIndexChanged(sender, e);
        }

        private void txtRate_TextChanged(object sender, EventArgs e)
        {
            cmbRateType_SelectedIndexChanged(sender, e);
            cmbMakingType_SelectedIndexChanged(sender, e);
            txtTotalAmount.Text = !string.IsNullOrEmpty(txtTotalAmount.Text) ? decimal.Round(Convert.ToDecimal(txtTotalAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : string.Empty;
            txtMakingAmount.Text = !string.IsNullOrEmpty(txtMakingAmount.Text) ? decimal.Round(Convert.ToDecimal(txtMakingAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : string.Empty;
        }

        private void txtMakingRate_TextChanged(object sender, EventArgs e)
        {
            cmbRateType_SelectedIndexChanged(sender, e);
            cmbMakingType_SelectedIndexChanged(sender, e);
            txtTotalAmount.Text = !string.IsNullOrEmpty(txtTotalAmount.Text) ? decimal.Round(Convert.ToDecimal(txtTotalAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : string.Empty;
            txtMakingAmount.Text = !string.IsNullOrEmpty(txtMakingAmount.Text) ? decimal.Round(Convert.ToDecimal(txtMakingAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : string.Empty;
        }

        private void txtMakingDisc_TextChanged(object sender, EventArgs e)
        {
            //cmbRateType_SelectedIndexChanged(sender, e);  // 
            //cmbMakingType_SelectedIndexChanged(sender, e);
            //string sFieldName = string.Empty;
            //int iSPId = 0;
            //iSPId = getUserDiscountPermissionId();
            //decimal dinitDiscValue = 0;

            //if (Convert.ToInt16(iSPId) == (int)CRWRetailDiscPermission.Salesperson)
            //    sFieldName = "MAXSALESPERSONSDISCPCT";
            //else if (Convert.ToInt16(iSPId) == (int)CRWRetailDiscPermission.Manager)
            //    sFieldName = "MAXMANAGERLEVELDISCPCT";
            //else if (Convert.ToInt16(iSPId) == (int)CRWRetailDiscPermission.Manager2)
            //    sFieldName = "MAXMANAGERLEVEL2DISCPCT";
            //else
            ////{
            //   MessageBox.Show("You are not authorized for this action.");
            ////    sFieldName = "OPENINGDISCPCT";
            ////}
            //decimal dQty = 0;
            //if (!string.IsNullOrEmpty(txtQuantity.Text))
            //    dQty = Convert.ToDecimal(txtQuantity.Text);
            //else
            //    dQty = 0;

            //if (!isMRPUCP)
            //    dinitDiscValue = GetMkDiscountFromDiscPolicy(sBaseItemID, dQty, sFieldName);// get OPENINGDISCPCT field value FOR THE OPENING
            ////else
            ////    MessageBox.Show("Invalid discount policy for this item");

            //decimal dMkPerDisc = 0;
            //if (!string.IsNullOrEmpty(txtMakingDisc.Text))
            //    dMkPerDisc = Convert.ToDecimal(txtMakingDisc.Text);
            //else
            //    dMkPerDisc = 0;

            //if (dinitDiscValue > 0)
            //{
            //    if ((dMkPerDisc > dinitDiscValue))
            //    {
            //        MessageBox.Show("Line discount percentage should not more than '" + dinitDiscValue + "'");
            //        txtMakingDisc.Focus();
            //    }
            //    else
            //    {
            //        CheckMakingDiscountFromDB(dinitDiscValue);
            //    }
            //}

            //else
            //{
            //    MessageBox.Show("Not allowed for this item");
            //}



        }

        private void txtMakingDiscTotAmt_TextChanged(object sender, EventArgs e) // Making Discount Type
        {
            cmbRateType_SelectedIndexChanged(sender, e);
            cmbMakingType_SelectedIndexChanged(sender, e);
        }
        private void txtWastageAmount_TextChanged(object sender, EventArgs e)  //wastage
        {
            cmbRateType_SelectedIndexChanged(sender, e);
            cmbMakingType_SelectedIndexChanged(sender, e);
        }

        private void txtTotalWeight_TextChanged(object sender, EventArgs e)
        {
            if(!isViewing)  // BLOCKED ON 19.11.2013
            {
                //txtQuantity.Text = string.IsNullOrEmpty(txtTotalWeight.Text.Trim()) ? string.Empty : txtTotalWeight.Text.Trim();
                //txtLossPct_TextChanged(sender, e);

            }
        }

        private void txtTotalWeight_Leave(object sender, EventArgs e)
        {
            //if (!isViewing)
            if((!isViewing) && (!rdbSale.Checked))
            {
                if(chkOwn.Checked)
                {
                    txtQuantity.Text = string.IsNullOrEmpty(txtTotalWeight.Text.Trim()) ? string.Empty : txtTotalWeight.Text.Trim();
                    txtLossPct_TextChanged(sender, e);
                }
                else
                {
                    GetPurcExchngGoldQty();
                    txtLossPct_TextChanged(sender, e);
                }
            }

        }

        private void txtPurity_Leave(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(txtPurity.Text.Trim()))
            {
                if(Convert.ToDecimal(txtPurity.Text.Trim()) > 1)
                {
                    MessageBox.Show("Purity value cannot greter than 1");
                    txtPurity.Text = string.Empty;
                    txtPurity.Focus();
                }
                else
                {
                    txtPurity.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(txtPurity.Text.Trim()), 4, MidpointRounding.AwayFromZero));
                    // if (!isViewing)
                    if(!isViewing && (!rdbSale.Checked))
                    {
                        if(chkOwn.Checked)
                        {
                            txtQuantity.Text = string.IsNullOrEmpty(txtTotalWeight.Text.Trim()) ? string.Empty : txtTotalWeight.Text.Trim();
                            txtPurity.Text = string.Empty;
                            txtLossPct_TextChanged(sender, e);
                        }
                        else
                        {
                            GetPurcExchngGoldQty();
                            txtLossPct_TextChanged(sender, e);
                        }
                    }
                }
            }

        }

        private void txtLossPct_TextChanged(object sender, EventArgs e)
        {
            if(!isViewing)
            {
                if(!string.IsNullOrEmpty(txtQuantity.Text.Trim()) && !string.IsNullOrEmpty(txtLossPct.Text.Trim()))
                {
                    decimal CalPerLossWt = Convert.ToDecimal(txtTotalWeight.Text.Trim()) * Convert.ToDecimal(txtLossPct.Text.Trim()) / 100;//txtQuantity
                    decimal CalLossWt = Convert.ToDecimal(txtTotalWeight.Text.Trim()) - Convert.ToDecimal(CalPerLossWt);//txtQuantity
                    //txtExpectedQuantity.Text = Convert.ToString(CalLossWt);
                    txtExpectedQuantity.Text = Convert.ToString(decimal.Round(CalLossWt, 3, MidpointRounding.AwayFromZero));
                    txtQuantity.Text = Convert.ToString(decimal.Round(CalLossWt, 3, MidpointRounding.AwayFromZero));
                    //txtLossWeight.Text = Convert.ToString(CalPerLossWt);
                    txtLossWeight.Text = Convert.ToString(decimal.Round(CalPerLossWt, 3, MidpointRounding.AwayFromZero));

                    cmbRateType_SelectedIndexChanged(sender, e);
                }
                if(string.IsNullOrEmpty(txtQuantity.Text.Trim()) || string.IsNullOrEmpty(txtLossPct.Text.Trim()))
                {
                    txtLossWeight.Text = string.Empty;
                    txtExpectedQuantity.Text = string.Empty;
                    cmbRateType_SelectedIndexChanged(sender, e);
                }
                txtLossWeight_Leave(sender, e);
                cmbRateType_SelectedIndexChanged(sender, e);
            }
        }
        #endregion

        #region Selected Index Changed
        private void cmbRateType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(!isViewing)
            {
                if(rdbSale.Checked)
                {
                    if(!string.IsNullOrEmpty(txtRate.Text.Trim()) && cmbRateType.SelectedIndex == 0 && !string.IsNullOrEmpty(txtQuantity.Text.Trim()))
                    {
                        Decimal decimalAmount = 0m;
                        decimalAmount = Convert.ToDecimal(txtRate.Text.Trim()) * Convert.ToDecimal(txtQuantity.Text.Trim());
                        txtAmount.Text = Convert.ToString(decimalAmount);
                    }
                    if(cmbRateType.SelectedIndex == 0 && (string.IsNullOrEmpty(txtRate.Text.Trim()) || string.IsNullOrEmpty(txtQuantity.Text.Trim())))
                    {
                        txtAmount.Text = string.Empty;
                        txtTotalAmount.Text = string.Empty;
                    }
                    if(cmbRateType.SelectedIndex == 1 && !string.IsNullOrEmpty(txtRate.Text.Trim()) && !string.IsNullOrEmpty(txtPCS.Text.Trim()))
                    {
                        Decimal decimalAmount = 0m;
                        decimalAmount = Convert.ToDecimal(txtRate.Text.Trim()) * Convert.ToDecimal(txtPCS.Text.Trim());
                        txtAmount.Text = Convert.ToString(decimalAmount);
                    }
                    if(cmbRateType.SelectedIndex == 1 && (string.IsNullOrEmpty(txtRate.Text.Trim()) || string.IsNullOrEmpty(txtPCS.Text.Trim())))
                    {
                        txtAmount.Text = string.Empty;
                        txtTotalAmount.Text = string.Empty;

                    }
                    if(cmbRateType.SelectedIndex == 2 && !string.IsNullOrEmpty(txtRate.Text.Trim().Trim()))
                    {
                        Decimal decimalAmount = 0m;
                        decimalAmount = Convert.ToDecimal(txtRate.Text.Trim());
                        txtAmount.Text = Convert.ToString(decimalAmount);
                    }
                    if(!string.IsNullOrEmpty(txtAmount.Text.Trim()) && !string.IsNullOrEmpty(txtMakingAmount.Text.Trim()))
                    {
                        txtTotalAmount.Text = Convert.ToString(Convert.ToDecimal(txtAmount.Text.Trim()) + Convert.ToDecimal(txtMakingAmount.Text.Trim()));
                    }

                    // Making Discount Type -- 29.04.2013
                    //if (!string.IsNullOrEmpty(txtMakingDisc.Text) && !string.IsNullOrEmpty(txtMakingAmount.Text)
                    //     && !string.IsNullOrEmpty(txtAmount.Text))
                    //{
                    //    Decimal decimalAmount = 0m;
                    //    decimalAmount = (Convert.ToDecimal(txtMakingAmount.Text.Trim())) - ((Convert.ToDecimal(txtMakingDisc.Text.Trim()) / 100) * (Convert.ToDecimal(txtMakingAmount.Text.Trim()))) + (Convert.ToDecimal(txtAmount.Text));
                    //    txtTotalAmount.Text = Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero));
                    //}
                    if(!string.IsNullOrEmpty(txtMakingDiscTotAmt.Text) && !string.IsNullOrEmpty(txtMakingAmount.Text)
                        && !string.IsNullOrEmpty(txtAmount.Text))
                    {
                        Decimal decimalAmount = 0m;
                        //decimalAmount = (Convert.ToDecimal(txtMakingAmount.Text.Trim())) - ((Convert.ToDecimal(txtMakingDisc.Text.Trim()) / 100) * (Convert.ToDecimal(txtMakingAmount.Text.Trim()))) + (Convert.ToDecimal(txtAmount.Text));
                        decimalAmount = (Convert.ToDecimal(txtMakingAmount.Text.Trim())) - (Convert.ToDecimal(txtMakingDiscTotAmt.Text)) + (Convert.ToDecimal(txtAmount.Text));
                        txtTotalAmount.Text = Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero));
                    }

                    //
                    if(string.IsNullOrEmpty(txtRate.Text.Trim()))
                    {
                        txtAmount.Text = string.Empty;
                        txtTotalAmount.Text = string.Empty;
                    }

                    // Added  -- wastage
                    if((!string.IsNullOrEmpty(txtTotalAmount.Text.Trim())) && (!string.IsNullOrEmpty(txtWastageAmount.Text.Trim())))
                    {
                        if((Convert.ToDecimal(txtTotalAmount.Text) > 0) && (Convert.ToDecimal(txtWastageAmount.Text) > 0))
                        {
                            txtTotalAmount.Text = Convert.ToString(Convert.ToDecimal(txtTotalAmount.Text.Trim()) + Convert.ToDecimal(txtWastageAmount.Text.Trim()));
                        }
                    }

                    //
                }
                else
                {

                    if(cmbRateType.SelectedIndex == 0 && !string.IsNullOrEmpty(txtRate.Text.Trim()) && !string.IsNullOrEmpty(txtLossWeight.Text.Trim()))
                    {
                        Decimal decimalAmount = 0m;
                        decimalAmount = Convert.ToDecimal(txtRate.Text.Trim()) * Convert.ToDecimal(txtExpectedQuantity.Text.Trim());
                        //  txtAmount.Text = Convert.ToString(decimalAmount);
                        txtAmount.Text = Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero));
                    }
                    if(cmbRateType.SelectedIndex == 0 && (string.IsNullOrEmpty(txtRate.Text.Trim()) || string.IsNullOrEmpty(txtLossWeight.Text.Trim())))
                    {
                        txtAmount.Text = string.Empty;
                    }
                    if(cmbRateType.SelectedIndex == 1 && !string.IsNullOrEmpty(txtRate.Text.Trim()) && !string.IsNullOrEmpty(txtPCS.Text.Trim()))
                    {
                        Decimal decimalAmount = 0m;
                        decimalAmount = Convert.ToDecimal(txtRate.Text.Trim()) * Convert.ToDecimal(txtPCS.Text.Trim());
                        //txtAmount.Text = Convert.ToString(decimalAmount);
                        txtAmount.Text = Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero));
                    }
                    if(cmbRateType.SelectedIndex == 1 && (string.IsNullOrEmpty(txtRate.Text.Trim()) || string.IsNullOrEmpty(txtPCS.Text.Trim())))
                    {
                        txtAmount.Text = string.Empty;
                    }
                    if(cmbRateType.SelectedIndex == 2 && !string.IsNullOrEmpty(txtRate.Text.Trim().Trim()))
                    {
                        Decimal decimalAmount = 0m;
                        decimalAmount = Convert.ToDecimal(txtRate.Text.Trim());
                        txtAmount.Text = Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero));
                    }
                    if(string.IsNullOrEmpty(txtRate.Text.Trim()))
                    {
                        txtAmount.Text = string.Empty;
                    }
                }
            }
        }

        private void cmbMakingType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(!isViewing)
            {
                if(!string.IsNullOrEmpty(txtMakingRate.Text.Trim()) && cmbMakingType.SelectedIndex == 1 && !string.IsNullOrEmpty(txtQuantity.Text.Trim()))
                {
                    Decimal decimalAmount = 0m;

                    Decimal dMakingQty = 0;

                    if(dtIngredients != null && dtIngredients.Rows.Count > 0) // not considered Multi Metal
                    {
                        dMakingQty = Convert.ToDecimal(dtIngredients.Rows[0]["QTY"]);
                    }
                    else
                    {
                        dMakingQty = Convert.ToDecimal(txtQuantity.Text.Trim());
                    }
                    // End

                    decimalAmount = Convert.ToDecimal(txtMakingRate.Text.Trim()) * dMakingQty;

                    // txtMakingAmount.Text = Convert.ToString(oRounding.Round(decimalAmount, 2));
                    txtMakingAmount.Text = Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero));
                }
                if(cmbMakingType.SelectedIndex == 1 && (string.IsNullOrEmpty(txtMakingRate.Text.Trim()) || string.IsNullOrEmpty(txtQuantity.Text.Trim())))
                {
                    txtMakingAmount.Text = string.Empty;
                    // txtTotalAmount.Text = string.Empty;
                }
                if(cmbMakingType.SelectedIndex == 0 && !string.IsNullOrEmpty(txtMakingRate.Text.Trim()) && !string.IsNullOrEmpty(txtPCS.Text.Trim()))
                {
                    Decimal decimalAmount = 0m;
                    decimalAmount = Convert.ToDecimal(txtMakingRate.Text.Trim()) * Convert.ToDecimal(txtPCS.Text.Trim());
                    //  txtMakingAmount.Text = Convert.ToString(decimalAmount);
                    txtMakingAmount.Text = Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero));
                }
                if(cmbMakingType.SelectedIndex == 0 && (string.IsNullOrEmpty(txtMakingRate.Text.Trim()) || string.IsNullOrEmpty(txtPCS.Text.Trim())))
                {
                    txtMakingAmount.Text = string.Empty;
                    //txtTotalAmount.Text = string.Empty;
                }
                if(cmbMakingType.SelectedIndex == 2 && !string.IsNullOrEmpty(txtMakingRate.Text.Trim().Trim()))
                {
                    Decimal decimalAmount = 0m;
                    decimalAmount = Convert.ToDecimal(txtMakingRate.Text.Trim());
                    // txtMakingAmount.Text = Convert.ToString(decimalAmount);
                    txtMakingAmount.Text = Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero));
                }
                if(cmbMakingType.SelectedIndex == 3 && !string.IsNullOrEmpty(txtMakingRate.Text.Trim()) && !string.IsNullOrEmpty(txtAmount.Text.Trim()))
                {
                    Decimal decimalAmount = 0m;
                    if(!string.IsNullOrEmpty(txtgval.Text))
                    {
                        if(Convert.ToDecimal(txtgval.Text.Trim()) != 0)
                            decimalAmount = (Convert.ToDecimal(txtMakingRate.Text.Trim()) / 100) * (Convert.ToDecimal(txtgval.Text.Trim()));
                        else
                            decimalAmount = (Convert.ToDecimal(txtMakingRate.Text.Trim()) / 100) * (Convert.ToDecimal(txtAmount.Text.Trim()));
                    }
                    else
                    {
                        decimalAmount = (Convert.ToDecimal(txtMakingRate.Text.Trim()) / 100) * (Convert.ToDecimal(txtAmount.Text.Trim()));
                    }

                    txtMakingAmount.Text = Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero));
                }
                if(cmbMakingType.SelectedIndex == 3 && string.IsNullOrEmpty(txtAmount.Text.Trim()))
                {
                    txtMakingAmount.Text = string.Empty;
                    // txtTotalAmount.Text = string.Empty;
                }

                if(!string.IsNullOrEmpty(txtAmount.Text.Trim()) && !string.IsNullOrEmpty(txtMakingAmount.Text.Trim()))
                {
                    txtTotalAmount.Text = Convert.ToString(Convert.ToDecimal(txtAmount.Text.Trim()) + Convert.ToDecimal(txtMakingAmount.Text.Trim()));
                }


                if(!string.IsNullOrEmpty(txtMakingDiscTotAmt.Text) && !string.IsNullOrEmpty(txtMakingAmount.Text) && !string.IsNullOrEmpty(txtAmount.Text))
                {
                    Decimal decimalAmount = 0m;
                    // decimalAmount = (Convert.ToDecimal(txtMakingAmount.Text.Trim())) - ((Convert.ToDecimal(txtMakingDisc.Text.Trim()) / 100) * (Convert.ToDecimal(txtMakingAmount.Text.Trim()))) + (Convert.ToDecimal(txtAmount.Text));
                    decimalAmount = (Convert.ToDecimal(txtMakingAmount.Text.Trim())) - (Convert.ToDecimal(txtMakingDiscTotAmt.Text)) + (Convert.ToDecimal(txtAmount.Text));
                    txtTotalAmount.Text = Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero));
                }

                //
                if(string.IsNullOrEmpty(txtMakingRate.Text.Trim()))
                {
                    txtMakingAmount.Text = string.Empty;
                    //txtTotalAmount.Text = string.Empty;
                }

                // Added  -- wastage
                if((!string.IsNullOrEmpty(txtTotalAmount.Text.Trim())) && (!string.IsNullOrEmpty(txtWastageAmount.Text.Trim())))
                {
                    if((Convert.ToDecimal(txtTotalAmount.Text) > 0) && (Convert.ToDecimal(txtWastageAmount.Text) > 0))
                    {
                        txtTotalAmount.Text = Convert.ToString(Convert.ToDecimal(txtTotalAmount.Text.Trim()) + Convert.ToDecimal(txtWastageAmount.Text.Trim()));
                    }
                }
            }
        }

        #endregion

        #region Validate Controls
        private bool ValidateControls()
        {

            if(IsCatchWtItem(sBaseItemID))
            {
                if(string.IsNullOrEmpty(txtPCS.Text.Trim()))
                {
                    using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("PCS can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        txtPCS.Focus();
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        return false;
                    }
                }
                else if(Convert.ToDecimal(txtPCS.Text) == 0)
                {
                    txtPCS.Focus();
                    MessageBox.Show("Pieces cannot be zero for catch weight product.", "Pieces cannot be zero catch weight product.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
            }
            else
            {
                txtPCS.Enabled = false;
            }

            //if(string.IsNullOrEmpty(txtPCS.Text))
            //{
            //    txtPCS.Focus();
            //    MessageBox.Show("Pieces cannot be Empty.", "Pieces cannot be Empty.", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return false;
            //}

            if(IsRetailItem(sBaseItemID) && (rdbSale.Checked == false)) // added on 17/04/2014 for retail=1 item can be only sale trans
            {
                MessageBox.Show("Item is not valid for this transaction.", "Item is not valid for this transaction.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            if(string.IsNullOrEmpty(txtQuantity.Text))
            {
                txtQuantity.Focus();
                MessageBox.Show("Quantity cannot be Empty.", "Quantity cannot be Empty.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            if(string.IsNullOrEmpty(txtRate.Text))
            {
                txtRate.Focus();
                MessageBox.Show("Rate cannot be Empty.", "Rate cannot be Empty.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            if(string.IsNullOrEmpty(txtAmount.Text))
            {
                MessageBox.Show("Amount cannot be Empty.", "Amount cannot be Empty.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            if(rdbSale.Checked)
            {

                decimal dSalesValue = 0m;

                if(!string.IsNullOrEmpty(txtTotalAmount.Text))
                {
                    dSalesValue = Convert.ToDecimal(txtTotalAmount.Text);// - metal value if is metal=gold
                    if(isMRPUCP)
                    {
                        decimal dCostPrice = 0m;
                        dCostPrice = getCostPrice(sBaseItemID, dSalesValue);

                        if (ApplicationSettings.Terminal.StoreCurrency != ApplicationSettings.Terminal.CompanyCurrency)
                        {

                            Microsoft.Dynamics.Retail.Pos.Currency.Currency objCurrency = new Microsoft.Dynamics.Retail.Pos.Currency.Currency();
                            decimal dExchangeCurrency = objCurrency.ExchangeRate(ApplicationSettings.Terminal.StoreCurrency);

                            dCostPrice = dCostPrice * dExchangeCurrency;
                        }

                        if (dCostPrice > dSalesValue)
                        {
                            MessageBox.Show("Sales price can not be lower than cost price for '" + sBaseItemID + "'.", "Information.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                    }
                    //else
                    //{
                    //    int iMetalType = GetMetalType(sBaseItemID);

                    //    if(iMetalType == (int)MetalType.Gold && !string.IsNullOrEmpty(txtgval.Text))
                    //    {
                    //        decimal dGoldValue = Convert.ToDecimal(txtgval.Text);
                    //        dSalesValue = dSalesValue - dGoldValue;

                    //        if(!getCostPrice(sBaseItemID, dSalesValue))
                    //        {
                    //            MessageBox.Show("Sales price can not less than cost price.", "Sales price can not less than cost price.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //            return false;
                    //        }
                    //    }
                    //}
                }

                if(string.IsNullOrEmpty(txtMakingRate.Text))
                {
                    txtMakingRate.Focus();
                    txtMakingRate.Text = "0";
                    //  MessageBox.Show("Making Rate cannot be Empty.", "Making Rate cannot be Empty.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //  return false;
                }


                string sFieldName = string.Empty;
                int iSPId = 0;
                iSPId = getUserDiscountPermissionId();
                decimal dinitDiscValue = 0;
                decimal dQty = 0;

                if(Convert.ToInt16(iSPId) == (int)CRWRetailDiscPermission.Salesperson)
                    sFieldName = "MAXSALESPERSONSDISCPCT";
                else if(Convert.ToInt16(iSPId) == (int)CRWRetailDiscPermission.Manager)
                    sFieldName = "MAXMANAGERLEVELDISCPCT";
                else if(Convert.ToInt16(iSPId) == (int)CRWRetailDiscPermission.Manager2)
                    sFieldName = "MAXMANAGERLEVEL2DISCPCT";
                else
                    MessageBox.Show("You are not authorized for this action.");
                // sFieldName = "OPENINGDISCPCT";


                if(!string.IsNullOrEmpty(txtQuantity.Text))
                    dQty = Convert.ToDecimal(txtQuantity.Text);
                else
                    dQty = 0;
                if(!isMRPUCP)
                {
                    if(txtMakingDisc.Enabled == true)
                    {
                        if(!string.IsNullOrEmpty(sFieldName))
                            dinitDiscValue = GetMkDiscountFromDiscPolicy(sBaseItemID, dQty, sFieldName);// get OPENINGDISCPCT field value FOR THE OPENING
                    }
                    //dinitDiscValue = GetMkDiscountFromDiscPolicy(sBaseItemID, dQty, sFieldName);
                    else
                        dinitDiscValue = GetMkDiscountFromDiscPolicy(sBaseItemID, dQty, "OPENINGDISCPCT");
                }
                //else
                //    MessageBox.Show("Invalid discount policy for this item");

                decimal dMkPerDisc = 0;
                if(!string.IsNullOrEmpty(txtMakingDisc.Text))
                    dMkPerDisc = Convert.ToDecimal(txtMakingDisc.Text);
                else
                    dMkPerDisc = 0;

                if (!isMRPUCP)
                {
                    if (dinitDiscValue >= 0)
                    {
                        if ((dMkPerDisc > dinitDiscValue))
                        {
                            MessageBox.Show("Line discount percentage should not more than '" + dinitDiscValue + "'");
                            txtMakingDisc.Focus();
                            return false;
                        }
                        else if (dMkPerDisc == 0 && dinitDiscValue > 0)
                            CheckMakingDiscountFromDB(dinitDiscValue);
                        else
                            CheckMakingDiscountFromDB(dMkPerDisc);
                    }
                }

                if(string.IsNullOrEmpty(txtTotalAmount.Text))
                {
                    MessageBox.Show("Total Amount cannot be Empty.", "Total Amount cannot be Empty.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
            }
            else
            {
                if(string.IsNullOrEmpty(txtTotalWeight.Text))
                {
                    txtMakingRate.Focus();
                    MessageBox.Show("Total Weight cannot be Empty.", "Total Weight cannot be Empty.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                #region commented for Samra
                //if((string.IsNullOrEmpty(txtPurity.Text)) && (!chkOwn.Checked))
                //{
                //    txtPurity.Focus();
                //    MessageBox.Show("Purity cannot be Empty.", "Purity cannot be Empty.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return false;
                //}

                //if(string.IsNullOrEmpty(txtLossPct.Text))
                //{
                //    txtMakingDisc.Focus();
                //    MessageBox.Show("Loss Percentage cannot be Empty.", "Loss Percentage cannot be Empty.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return false;
                //}
                //if(string.IsNullOrEmpty(txtLossWeight.Text))
                //{
                //    MessageBox.Show("Loss Weight cannot be Empty.", "Loss Weight cannot be Empty.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return false;
                //}
                #endregion

                if(string.IsNullOrEmpty(txtExpectedQuantity.Text))
                {
                    MessageBox.Show("Expected Quantity cannot be Empty.", "Expected Quantity cannot be Empty.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

            }
            #region MyRegion //Start : Changes on 08/04/2014 RHossain
            //RetailTransaction retailTrans = posTransaction as RetailTransaction;
            if(retailTrans != null
            && retailTrans.SaleItems != null
            && retailTrans.SaleItems.Count > 0)
            {
                SaleLineItem saleItem = retailTrans.SaleItems.Last.Value;
                foreach(SaleLineItem saleLineItem in retailTrans.SaleItems)
                {
                    // if (saleLineItem.ItemId == saleItem.ItemId)
                    preTransType = saleItem.PartnerData.TransactionType;

                    switch(Convert.ToInt16(preTransType))
                    {
                        case 0:
                            if(rdbSale.Checked || rdbExchange.Checked)
                                return true;
                            else
                            {
                                MessageBox.Show("Invalid transaction.", "Invalid transaction.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return false;
                            }
                        case 1:
                            if(rdbPurchase.Checked)
                                return true;
                            else
                            {
                                MessageBox.Show("Invalid transaction.", "Invalid transaction.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return false;
                            }
                        case 2:
                            if(rdbPurchReturn.Checked)
                                return true;
                            else
                            {
                                MessageBox.Show("Invalid transaction.", "Invalid transaction.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return false;
                            }
                        case 3:
                            if(rdbSale.Checked || rdbExchange.Checked)
                                return true;
                            else
                            {
                                MessageBox.Show("Invalid transaction.", "Invalid transaction.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return false;
                            }
                        case 4:
                            if(rdbExchangeReturn.Checked)
                                return true;
                            else
                            {
                                MessageBox.Show("Invalid transaction.", "Invalid transaction.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return false;
                            }
                        default:
                            MessageBox.Show("Invalid transaction.", "Invalid transaction.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;

                    }

                }
            }
            #endregion //End : Changes on 08/04/2014 RHossain
            return true;
        }
        #endregion

        #region Close Click
        private void btnCLose_Click(object sender, EventArgs e)
        {
            ClearControls();
            if(rdbSale.Checked)
            {
                BuildSaleList();
                purchaseList = new List<KeyValuePair<string, string>>();
            }
            else
            {
                BuildPurchaseList();
                saleList = new List<KeyValuePair<string, string>>();
            }
            dtIngredients = new DataTable();
            dtIngredientsClone = new DataTable();
            this.Close();
        }
        #endregion

        #region Radio Buttons Checked Changed
        private void rdbSale_CheckedChanged(object sender, EventArgs e)
        {
            if(!isViewing)
            {
                //  RadioChecked = "Sale";
                RadioChecked = Convert.ToString((int)TransactionType.Sale);
                if(!IsRetailItem(ItemID))
                    EnableSaleButtons();
                else // On Request of S.Sharma on 09/07/2014
                {
                    txtPCS.Enabled = false;
                    txtRate.Enabled = false;
                    txtTotalWeight.Enabled = false;
                    txtPurity.Enabled = false;
                    txtLossPct.Enabled = false;
                }
                if(saleList != null && saleList.Count > 0)
                {
                    BindSaleControls();
                }
                else
                {
                    BuildSaleList();
                    ClearControls();
                }
                chkOwn.Checked = false;
                chkOwn.Visible = false;
                cmbRateType.SelectedIndex = 0;
                txtRate.Text = string.Empty;
                //txtRate.Text = getRateFromMetalTable();
                BindIngredientGrid();
                panel4.Enabled = true;
                FnArrayList();
                //chkSampleReturn.Visible = true;

                if(isMRPUCP)// On Request of S.Sharma on 09/07/2014
                {
                    cmbRateType.SelectedIndex = (int)RateType.Tot;
                    cmbRateType.Enabled = false;
                }
            }
        }

        private void rdbPurchase_CheckedChanged(object sender, EventArgs e)
        {
            chkOwn.Enabled = true;
            if(!isViewing)
            {

                // RadioChecked = "Purchase";
                RadioChecked = Convert.ToString((int)TransactionType.Purchase);
                EnablePurchaseExchangeButtons();
                //if (purchaseList != null && purchaseList.Count > 0)
                //{
                //    BuildPurchaseList();
                //    BindPurchaseControls();
                //}
                //else
                //{
                //    BuildPurchaseList();
                //    ClearControls();
                //}

                ClearControls();

                CommonHelper();
                chkSampleReturn.Visible = false;
                txtLossPct.Text = "0";
            }
        }

        private void rdbExchange_CheckedChanged(object sender, EventArgs e)
        {
            if(!isViewing)
            {
                txtLossPct.Text = "0";
                //   RadioChecked = "Exchange";
                RadioChecked = Convert.ToString((int)TransactionType.Exchange);
                EnablePurchaseExchangeButtons();
                //if (purchaseList != null && purchaseList.Count > 0)
                //{
                //    BindPurchaseControls();
                //}
                //else
                //{
                //    BuildPurchaseList();
                //    ClearControls();
                //}

                ClearControls(); //

                CommonHelper();
                chkSampleReturn.Visible = false;
                txtLossPct.Text = "0"; //

            }
        }

        private void rdbPurchReturn_CheckedChanged(object sender, EventArgs e)
        {
            chkOwn.Enabled = true;
            if(!isViewing)
            {

                //  RadioChecked = "PurchaseReturn";
                RadioChecked = Convert.ToString((int)TransactionType.PurchaseReturn);
                EnablePurchaseExchangeButtons();
                //if (purchaseList != null && purchaseList.Count > 0)
                //{
                //    BindPurchaseControls();
                //}
                //else
                //{
                //    BuildPurchaseList();
                //    ClearControls();
                //}

                ClearControls(); //

                CommonHelper();
                chkSampleReturn.Visible = false;
                txtLossPct.Text = "0";
            }
        }

        private void rdbExchangeReturn_CheckedChanged(object sender, EventArgs e)
        {
            if(!isViewing)
            {
                txtLossPct.Text = "0";
                //  RadioChecked = "ExchangeReturn";
                RadioChecked = Convert.ToString((int)TransactionType.ExchangeReturn);
                EnablePurchaseExchangeButtons();
                //if (purchaseList != null && purchaseList.Count > 0)
                //{
                //    BindPurchaseControls();
                //}
                //else
                //{
                //    BuildPurchaseList();
                //    ClearControls();
                //}

                ClearControls(); //

                CommonHelper();
                chkSampleReturn.Visible = false;
                txtLossPct.Text = "0";
            }
        }
        #endregion

        #region Bind Sale Purchase COntrols
        private void BindSaleControls()
        {
            txtPCS.Text = saleList[0].Value;
            txtQuantity.Text = saleList[1].Value;
            txtRate.Text = saleList[2].Value;
            //    cmbRateType.SelectedIndex = cmbRateType.FindStringExact(Convert.ToString(saleList[3].Value));
            cmbRateType.SelectedIndex = Convert.ToInt16(saleList[3].Value);
            txtMakingRate.Text = saleList[4].Value;
            //    cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(saleList[5].Value));
            switch(Convert.ToInt16(saleList[5].Value))
            {
                case 0:
                    cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Pieces));
                    break;
                case 2:
                    cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Weight));
                    break;
                case 3:
                    cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Tot));
                    break;
                case 4:
                    cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Percentage));
                    break;
            }

            txtAmount.Text = saleList[6].Value;
            txtMakingDisc.Text = saleList[7].Value;
            txtMakingAmount.Text = saleList[8].Value;
            txtTotalAmount.Text = saleList[9].Value;

            txtTotalWeight.Text = string.Empty;
            txtPurity.Text = string.Empty;
            txtLossPct.Text = string.Empty;
            txtLossWeight.Text = string.Empty;
            txtExpectedQuantity.Text = string.Empty;

            // Added for wastage
            cmbWastage.SelectedIndex = Convert.ToInt32(saleList[18].Value);
            txtWastageQty.Text = saleList[19].Value;
            txtWastageAmount.Text = saleList[20].Value;

            txtWastagePercentage.Text = saleList[21].Value;
            // wastage rate
            //
            // Making Discount Type
            cmbMakingDiscType.SelectedIndex = Convert.ToInt32(saleList[23].Value);
            txtMakingDiscTotAmt.Text = saleList[24].Value;

            //

        }

        private void BindPurchaseControls()
        {
            txtPCS.Text = purchaseList[0].Value;
            txtQuantity.Text = purchaseList[1].Value;
            txtRate.Text = purchaseList[2].Value;
            //   cmbRateType.SelectedIndex = cmbRateType.FindStringExact(Convert.ToString(purchaseList[3].Value));
            cmbRateType.SelectedIndex = Convert.ToInt16(purchaseList[3].Value);
            txtTotalWeight.Text = purchaseList[10].Value;
            txtLossPct.Text = Convert.ToString(purchaseList[11].Value);
            txtAmount.Text = purchaseList[6].Value;
            txtLossWeight.Text = purchaseList[12].Value;
            txtExpectedQuantity.Text = purchaseList[13].Value;
            txtPurity.Text = purchaseList[26].Value;

            txtMakingRate.Text = string.Empty;
            cmbMakingType.SelectedIndex = 0;
            txtMakingDisc.Text = string.Empty;
            txtMakingAmount.Text = string.Empty;
            txtTotalAmount.Text = string.Empty;

            // Added for wastage 
            cmbWastage.SelectedIndex = 0;
            txtWastageQty.Text = "0";
            txtWastageAmount.Text = "0";
            txtWastagePercentage.Text = "";
            //

            // Making Discount Type
            cmbMakingDiscType.SelectedIndex = 0;
            txtMakingDiscTotAmt.Text = "0";

            //
        }
        #endregion

        #region Enable Disable Buttons
        private void EnablePurchaseExchangeButtons()
        {
            txtPCS.Enabled = true;
            if(chkOwn.Checked)
            {
                txtTotalWeight.Enabled = false;
                txtPurity.Enabled = false;
                txtLossPct.Enabled = false;
            }
            else
            {
                txtTotalWeight.Enabled = true;
                txtPurity.Enabled = true;
                txtLossPct.Enabled = true;
            }

            txtLossPct.Enabled = true;
            txtRate.Enabled = true;
            cmbRateType.Enabled = true;

            txtQuantity.Enabled = false;
            txtMakingRate.Enabled = false;
            txtMakingDisc.Enabled = false;
            cmbMakingType.Enabled = false;

        }

        private void EnableSaleButtons() //if (IsRetailItem(sBaseItemID)
        {
            if(IsCatchWtItem(ItemID))
            {
                txtPCS.Enabled = true;
                txtPCS.Focus();
            }
            else
            {
                txtPCS.Text = "";
                txtQuantity.Focus();
                txtPCS.Enabled = false;
            }
            //txtPCS.Enabled = true;
            txtQuantity.Enabled = true;
            txtRate.Enabled = true;
            txtMakingRate.Enabled = true;
            txtMakingDisc.Enabled = true;
            cmbRateType.Enabled = true;
            cmbMakingType.Enabled = true;

            txtTotalWeight.Enabled = false;
            txtPurity.Enabled = false;

            txtLossPct.Enabled = false;
        }
        #endregion

        #region Clear Controls
        private void ClearControls()
        {
            //rdbSale.Checked = true;
            txtPCS.Text = string.Empty;
            txtQuantity.Text = string.Empty;
            txtRate.Text = string.Empty;
            cmbRateType.SelectedIndex = 0;
            txtMakingRate.Text = string.Empty;
            cmbMakingType.SelectedIndex = 0;
            txtAmount.Text = string.Empty;
            txtMakingDisc.Text = string.Empty; // Making Discount
            txtTotalWeight.Text = string.Empty;
            txtPurity.Text = string.Empty;

            txtLossPct.Text = string.Empty;
            txtLossWeight.Text = string.Empty;
            txtExpectedQuantity.Text = string.Empty;
            txtMakingAmount.Text = string.Empty;
            txtTotalAmount.Text = string.Empty;
            // Making Discount Type
            txtMakingDiscTotAmt.Text = string.Empty;
            cmbMakingDiscType.SelectedIndex = 0;

            // Added for wastage
            cmbWastage.SelectedIndex = 0;
            txtWastageQty.Text = "0";
            txtWastageAmount.Text = "0";
            txtWastagePercentage.Text = "";
            //
        }
        #endregion

        #region Text Loss Weight Leave
        private void txtLossWeight_Leave(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(txtQuantity.Text.Trim()) && !string.IsNullOrEmpty(txtLossWeight.Text.Trim()))
            {
                decimal dLossWt = 0m;
                dLossWt = Convert.ToDecimal(txtTotalWeight.Text.Trim()) - Convert.ToDecimal(txtExpectedQuantity.Text.Trim());//txtQuantity
                txtLossWeight.Text = Convert.ToString(dLossWt);
                cmbRateType_SelectedIndexChanged(sender, e);
            }
        }
        #endregion

        #region txtRate_Leave

        #endregion

        #region txtQuantity_Leave
        private void txtQuantity_Leave(object sender, EventArgs e)
        {
            if(rdbSale.Checked)
            {
                CheckRateFromDB();

                #region new Making disc

                decimal dinitDiscValue = 0;

                decimal dQty = 0;
                if(!string.IsNullOrEmpty(txtQuantity.Text))
                    dQty = Convert.ToDecimal(txtQuantity.Text);
                else
                    dQty = 0;
                if(!isMRPUCP)
                    dinitDiscValue = GetMkDiscountFromDiscPolicy(sBaseItemID, dQty, "OPENINGDISCPCT");// get OPENINGDISCPCT field value FOR THE OPENING
                //else
                //    MessageBox.Show("Invalid discount policy for this item");

                decimal dMkPerDisc = 0;
                if(!string.IsNullOrEmpty(txtMakingDisc.Text))
                    dMkPerDisc = Convert.ToDecimal(txtMakingDisc.Text);
                else
                    dMkPerDisc = 0;

                if(dinitDiscValue > 0)
                    txtMakingDisc.Enabled = false;
                else
                    txtMakingDisc.Enabled = true;

                if(dinitDiscValue >= 0)
                {
                    if((dMkPerDisc > dinitDiscValue))
                    {
                        MessageBox.Show("Line discount percentage should not more than '" + dinitDiscValue + "'");
                        // txtMakingDisc.Focus();
                    }
                    //else
                    //{
                    //    CheckMakingDiscountFromDB(dinitDiscValue);
                    //}
                }
                //else
                //{
                //    MessageBox.Show("Not allowed for this item");
                //}
                CheckMakingDiscountFromDB(dinitDiscValue);
                #endregion

                //if (cmbMakingDiscType.SelectedIndex == 0 && dMakingDiscDbAmt > 0) // Weight
                //{
                //    CalcMakingDiscount(dMakingDiscDbAmt);
                //}

            }
        }


        private void CheckRateFromDB(bool IsFixedRate = false)
        {
            string sWtRange = string.Empty;

            if(conn.State == ConnectionState.Closed)
                conn.Open();

            // Quantity calculation //// NOT Consider Multi Metal in Ingredient [Order by Metaltype in Ingredient]
            if(dtIngredients != null && dtIngredients.Rows.Count > 0)
            {
                int iIngMetalType;
                iIngMetalType = Convert.ToInt32(dtIngredients.Rows[0]["METALTYPE"]);
                if((iIngMetalType == (int)MetalType.Gold) || (iIngMetalType == (int)MetalType.Silver)
                    || (iIngMetalType == (int)MetalType.Platinum) || (iIngMetalType == (int)MetalType.Palladium))
                {
                    sWtRange = Convert.ToString(dtIngredients.Rows[0]["Qty"]);
                }
                else
                {
                    sWtRange = txtQuantity.Text.Trim();
                }
            }
            else
            {
                sWtRange = txtQuantity.Text.Trim();
            }


            #region commented [ changed for SKU Item wise --> Parent Item --> ''filter 22.03.13]

            //string commandText = "  DECLARE @INVENTLOCATION VARCHAR(20)    DECLARE @LOCATION VARCHAR(20)   DECLARE @ITEM VARCHAR(20)  DECLARE @PARENTITEM VARCHAR(20)   DECLARE @MFGCODE VARCHAR(20)" +  // CHANGED
            //                     "  SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM  " +
            //                     "  RETAILCHANNELTABLE INNER JOIN  RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID  " +
            //                     "  WHERE STORENUMBER='" + StoreID + "' " +
            //                     "  SELECT @MFGCODE = MFG_CODE FROM INVENTTABLE WHERE ITEMID='" + ItemID + "'" +
            //                     "  IF EXISTS(SELECT TOP(1) * FROM RETAIL_SALES_AGREEMENT_DETAIL WHERE INVENTLOCATIONID=@INVENTLOCATION) " +
            //                     "  BEGIN SET @LOCATION=@INVENTLOCATION  END ELSE BEGIN  SET @LOCATION='' END  " +


            //            " SET @PARENTITEM = ''" +
            //            " IF EXISTS(SELECT TOP(1) ITEMID FROM  INVENTTABLE WHERE ITEMID='" + ItemID + "' AND RETAIL=1)" + //ADDED
            //            " BEGIN SELECT @PARENTITEM = ITEMIDPARENT FROM [INVENTTABLE] WHERE ITEMID = '" + ItemID + "' " +  // ADDED
            //            " END  SET @ITEM='" + ItemID + "'";
            ////" IF (ISNULL(@PARENTITEM,'') <> '') BEGIN SET @ITEM = @PARENTITEM END ELSE BEGIN SET @ITEM = '' END " + // ADDED
            ////" END  ELSE  BEGIN  SET @ITEM='" + ItemID + "' END";

            //commandText += " SELECT  TOP (1) CAST(RETAIL_SALES_AGREEMENT_DETAIL.MK_RATE AS decimal(18, 2)) , RETAIL_SALES_AGREEMENT_DETAIL.MK_TYPE " +
            //                " ,WAST_TYPE,CAST(WAST_QTY AS decimal(18,2))" + // added for wastage
            //             " FROM         RETAIL_SALES_AGREEMENT_DETAIL INNER JOIN " +
            //    //  " INVENTTABLE ON RETAIL_SALES_AGREEMENT_DETAIL.MFG_CODE = INVENTTABLE.MFG_CODE " + 
            //               " (SELECT MFG_CODE FROM INVENTTABLE WHERE  ITEMID=  '" + ItemID.Trim() + "') T ON RETAIL_SALES_AGREEMENT_DETAIL.MFG_CODE = T.MFG_CODE " +
            //               " WHERE    " +
            //    // " RETAIL_SALES_AGREEMENT_DETAIL.ITEMID=@ITEM  " + 
            //               " (RETAIL_SALES_AGREEMENT_DETAIL.ITEMID=@ITEM OR RETAIL_SALES_AGREEMENT_DETAIL.ITEMID = @PARENTITEM OR RETAIL_SALES_AGREEMENT_DETAIL.ITEMID='')" +
            //               " AND RETAIL_SALES_AGREEMENT_DETAIL.INVENTLOCATIONID=@LOCATION  " +
            //               " AND RETAIL_SALES_AGREEMENT_DETAIL.MFG_CODE = @MFGCODE" +
            //               " AND  (RETAIL_SALES_AGREEMENT_DETAIL.ARTICLE_CODE=(SELECT ARTICLE_CODE FROM [INVENTTABLE] WHERE ITEMID = '" + ItemID.Trim() + "') " +
            //               " OR RETAIL_SALES_AGREEMENT_DETAIL.ARTICLE_CODE='') AND " +
            //               "  (RETAIL_SALES_AGREEMENT_DETAIL.COMPLEXITY_CODE=(SELECT COMPLEXITY_CODE FROM [INVENTTABLE] WHERE ITEMID = '" + ItemID.Trim() + "') " +
            //               "  OR RETAIL_SALES_AGREEMENT_DETAIL.COMPLEXITY_CODE='')  AND " +
            //               "  " +
            //    // "  ('" + txtQuantity.Text.Trim() + "' BETWEEN RETAIL_SALES_AGREEMENT_DETAIL.FROM_WEIGHT AND RETAIL_SALES_AGREEMENT_DETAIL.TO_WEIGHT)  " + 
            //               "  ('" + sWtRange + "' BETWEEN RETAIL_SALES_AGREEMENT_DETAIL.FROM_WEIGHT AND RETAIL_SALES_AGREEMENT_DETAIL.TO_WEIGHT)  " +
            //               " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN RETAIL_SALES_AGREEMENT_DETAIL.FROMDATE AND RETAIL_SALES_AGREEMENT_DETAIL.TODATE)  " +
            //               " AND RETAIL_SALES_AGREEMENT_DETAIL.ACTIVATE = 1 " + // ADDED ON 14.03.13 FOR ACTIVATE filter 
            //    //  "  ORDER BY RETAIL_SALES_AGREEMENT_DETAIL.ITEMID,RETAIL_SALES_AGREEMENT_DETAIL.ARTICLE_CODE,RETAIL_SALES_AGREEMENT_DETAIL.COMPLEXITY_CODE" +
            //    //  " ,RETAIL_SALES_AGREEMENT_DETAIL.FROMDATE DESC";  // Added on 29.05.2013
            //    // Changed order sequence on 03.06.2013 as per u.das
            //            "  ORDER BY RETAIL_SALES_AGREEMENT_DETAIL.ITEMID DESC,RETAIL_SALES_AGREEMENT_DETAIL.COMPLEXITY_CODE DESC,RETAIL_SALES_AGREEMENT_DETAIL.ARTICLE_CODE DESC" +
            //               " ,RETAIL_SALES_AGREEMENT_DETAIL.FROMDATE DESC";
            #endregion

            #region New for Samara changes
            string commandText = "  DECLARE @INVENTLOCATION VARCHAR(20)    DECLARE @LOCATION VARCHAR(20)   DECLARE @ITEM VARCHAR(20)  DECLARE @PARENTITEM VARCHAR(20)   DECLARE @MFGCODE VARCHAR(20)" +  // CHANGED
                                "  DECLARE @CUSTCLASSCODE VARCHAR(20) " +
                                "  SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM  " +
                                "  RETAILCHANNELTABLE INNER JOIN  RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID  " +
                                "  WHERE STORENUMBER='" + StoreID + "' " +
                                "  SELECT @MFGCODE = MFG_CODE FROM INVENTTABLE WHERE ITEMID='" + ItemID + "'" +
                                "  IF EXISTS(SELECT TOP(1) * FROM RETAIL_SALES_AGREEMENT_DETAIL WHERE INVENTLOCATIONID=@INVENTLOCATION) " +
                                "  BEGIN SET @LOCATION=@INVENTLOCATION  END ELSE BEGIN  SET @LOCATION='' END  " +

                                " SET @PARENTITEM = ''" +
                                " IF EXISTS(SELECT TOP(1) ITEMID FROM  INVENTTABLE WHERE ITEMID='" + ItemID + "' AND RETAIL=1)" + //ADDED
                                " BEGIN SELECT @PARENTITEM = ITEMIDPARENT FROM [INVENTTABLE] WHERE ITEMID = '" + ItemID + "' " +  // ADDED
                                " END  SET @ITEM='" + ItemID + "'" +
                //added on 09/02/16
                                "  SELECT @CUSTCLASSCODE = CUSTCLASSIFICATIONID FROM [CUSTTABLE] WHERE ACCOUNTNUM = '" + sCustomerId + "'";

            commandText += " SELECT  TOP (1) CAST(RETAIL_SALES_AGREEMENT_DETAIL.MK_RATE AS decimal(18, 2)) , RETAIL_SALES_AGREEMENT_DETAIL.MK_TYPE " +
                           " ,WAST_TYPE,CAST(WAST_QTY AS decimal(18,2))" + // added for wastage
                           " FROM         RETAIL_SALES_AGREEMENT_DETAIL  " +

                           //" INNER JOIN (SELECT MFG_CODE FROM INVENTTABLE WHERE  ITEMID=  '" + ItemID.Trim() + "') T ON RETAIL_SALES_AGREEMENT_DETAIL.MFG_CODE = T.MFG_CODE " +
                           " WHERE    " +

                           " (RETAIL_SALES_AGREEMENT_DETAIL.ITEMID=@ITEM OR RETAIL_SALES_AGREEMENT_DETAIL.ITEMID = @PARENTITEM OR RETAIL_SALES_AGREEMENT_DETAIL.ITEMID='')" +
                           " AND RETAIL_SALES_AGREEMENT_DETAIL.INVENTLOCATIONID=@LOCATION  " +

                           //" AND RETAIL_SALES_AGREEMENT_DETAIL.MFG_CODE = @MFGCODE" +

                           //changes on 09/02/16
                           " AND  (RETAIL_SALES_AGREEMENT_DETAIL.COLLECTIONCODE=(SELECT COLLECTIONCODE FROM [INVENTTABLE] WHERE ITEMID = '" + ItemID.Trim() + "') " +
                           " OR RETAIL_SALES_AGREEMENT_DETAIL.COLLECTIONCODE='') AND " +
                           " (RETAIL_SALES_AGREEMENT_DETAIL.PRODUCTCODE=(SELECT PRODUCTTYPECODE FROM [INVENTTABLE] WHERE ITEMID = '" + ItemID.Trim() + "') " +
                           " OR RETAIL_SALES_AGREEMENT_DETAIL.PRODUCTCODE='')  AND " +
                           " " +

                           " (RETAIL_SALES_AGREEMENT_DETAIL.AccountNum='" + sCustomerId.Trim() + "' " +
                           " OR RETAIL_SALES_AGREEMENT_DETAIL.AccountNum='') AND " +
                           " (RETAIL_SALES_AGREEMENT_DETAIL.CustClassificationId=@CUSTCLASSCODE " +
                           " OR RETAIL_SALES_AGREEMENT_DETAIL.CustClassificationId='') AND " +

                           " ('" + sWtRange + "' BETWEEN RETAIL_SALES_AGREEMENT_DETAIL.FROM_WEIGHT AND RETAIL_SALES_AGREEMENT_DETAIL.TO_WEIGHT)  " +
                           " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN RETAIL_SALES_AGREEMENT_DETAIL.FROMDATE AND RETAIL_SALES_AGREEMENT_DETAIL.TODATE)  " +
                           " AND RETAIL_SALES_AGREEMENT_DETAIL.ACTIVATE = 1 " + // ADDED ON 14.03.13 FOR ACTIVATE filter 

                           "  ORDER BY RETAIL_SALES_AGREEMENT_DETAIL.ITEMID DESC,RETAIL_SALES_AGREEMENT_DETAIL.COMPLEXITY_CODE DESC,RETAIL_SALES_AGREEMENT_DETAIL.ARTICLE_CODE DESC" +
                           " ,RETAIL_SALES_AGREEMENT_DETAIL.FROMDATE DESC";
            #endregion

            SqlCommand command = new SqlCommand(commandText, conn);
            command.CommandTimeout = 0;

            decimal dWastQty = 0m;
            using(SqlDataReader reader = command.ExecuteReader())
            {
                if(reader.HasRows)
                {
                    while(reader.Read())
                    {
                        txtMakingRate.Text = Convert.ToString(reader.GetValue(0));
                        switch(Convert.ToString(reader.GetValue(1)))
                        {
                            case "0":
                                cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Pieces));
                                break;
                            case "2":
                                cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Weight));
                                break;
                            case "3":
                                cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Tot));
                                break;
                            case "4":
                                cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Percentage));
                                break;
                            default:
                                cmbMakingType.SelectedIndex = 2;
                                break;
                        }

                        dWastQty = Convert.ToDecimal(reader.GetValue(3));

                        switch(Convert.ToString(reader.GetValue(2)))
                        {
                            case "0":
                                cmbWastage.SelectedIndex = cmbWastage.FindStringExact(Convert.ToString(WastageType.Weight));
                                break;
                            case "1":
                                cmbWastage.SelectedIndex = cmbWastage.FindStringExact(Convert.ToString(WastageType.Percentage));
                                break;
                            default:
                                cmbWastage.SelectedIndex = 0;
                                break;
                        }

                        //
                    }
                }
            }
            if(conn.State == ConnectionState.Open)
                conn.Close();


            // --------- check MFG_CODE AND SALE_COMPLEXITYCODE  against the item if Making Rate is 0 // 30.08.2013

            if(string.IsNullOrEmpty(txtMakingRate.Text))
            {
                txtMakingRate.Text = "0";
            }

            if((Convert.ToDecimal(txtMakingRate.Text) <= 0) && (dWastQty <= 0))
            {
                // get sku -- Metal Type
                int iSKUMetaltype = GetMetalType(sBaseItemID);

                if(iSKUMetaltype == (int)MetalType.Gold
                    || iSKUMetaltype == (int)MetalType.Silver
                    || iSKUMetaltype == (int)MetalType.Platinum
                    || iSKUMetaltype == (int)MetalType.Palladium)
                {

                    if(!IsValidMakingRate(ItemID.Trim()))
                    {
                        using(LSRetailPosis.POSProcesses.frmMessage message = new LSRetailPosis.POSProcesses.frmMessage("0 Making Rate is not valid for this item", MessageBoxButtons.OK, MessageBoxIcon.Error))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(message);
                        }
                        isCancelClick = true;
                        this.Close();
                        return;
                    }
                }

            }

            //----------------------


            #region [ Wastage Amount Calculation] // for wastage  // NOT Consider Multi Metal in Ingredient
            // if (!string.IsNullOrEmpty(txtRate.Text.Trim()) && cmbWastage.SelectedIndex == 0 && !string.IsNullOrEmpty(txtWastageQty.Text.Trim()))
            if(cmbWastage.SelectedIndex == 0 && dWastQty > 0)
            {
                // if (Convert.ToDecimal(txtWastageQty.Text) > 0)
                //{
                Decimal dAmount = 0m;
                int iMetalType = 0;
                string sRate = "";

                // enum MetalType  ---> Gold = 1,

                //  iMetalType = GetMetalType(ItemID);
                //  iMetalType = GetMetalType(sBaseItemID);

                if(dtIngredients != null && dtIngredients.Rows.Count > 0)
                {
                    iMetalType = Convert.ToInt32(dtIngredients.Rows[0]["METALTYPE"]);

                }
                else
                {
                    iMetalType = GetMetalType(sBaseItemID);
                }

                if(iMetalType == (int)MetalType.Gold
                    || iMetalType == (int)MetalType.Silver
                    || iMetalType == (int)MetalType.Palladium
                    || iMetalType == (int)MetalType.Platinum
                    )
                {
                    if(dtIngredients != null && dtIngredients.Rows.Count > 0)
                    {
                        //sRate = getWastageMetalRate(Convert.ToString(dtIngredients.Rows[0]["ITEMID"])
                        //                            , Convert.ToString(dtIngredients.Rows[0]["ConfigID"]));
                        if(IsFixedRate)
                        {
                            sRate = Convert.ToString(dCustomerOrderFixedRate);
                        }
                        else
                        {
                            sRate = getWastageMetalRate(Convert.ToString(dtIngredients.Rows[0]["ITEMID"])
                                                        , Convert.ToString(dtIngredients.Rows[0]["ConfigID"]));
                        }


                    }
                    else
                    {
                        sRate = getWastageMetalRate(sBaseItemID, sBaseConfigID);
                    }

                    if(!string.IsNullOrEmpty(sRate))
                        dWMetalRate = Convert.ToDecimal(sRate);

                    if(dWMetalRate > 0)
                    {
                        txtWastageQty.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(dWastQty), 3, MidpointRounding.AwayFromZero)); ;//Convert.ToString(dWastQty);

                        // decimalAmount = Convert.ToDecimal(txtRate.Text.Trim()) * Convert.ToDecimal(txtWastageQty.Text.Trim());

                        //decimalAmount = Convert.ToDecimal(txtRate.Text.Trim()) * dWastQty;
                        dAmount = dWMetalRate * dWastQty;

                        txtWastageAmount.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(dAmount), 2, MidpointRounding.AwayFromZero));
                    }
                }



                // }
            }
            //if (cmbWastage.SelectedIndex == 0 && (string.IsNullOrEmpty(txtRate.Text.Trim()) || string.IsNullOrEmpty(txtWastageQty.Text.Trim())))
            //{
            //    txtWastageAmount.Text = string.Empty;

            //}


            //if (!string.IsNullOrEmpty(txtRate.Text.Trim()) && cmbWastage.SelectedIndex == 1 && !string.IsNullOrEmpty(txtWastageQty.Text.Trim()))
            else if(cmbWastage.SelectedIndex == 1 && dWastQty > 0)
            {
                //if (Convert.ToDecimal(txtWastageQty.Text) > 0)
                //{

                // Calculate wastage Qty  -- consider Ingredient

                if(dtIngredients != null && dtIngredients.Rows.Count > 0)
                {
                    Decimal dIngrdTotQty = 0m;
                    Decimal dWastageQty = 0m;
                    Decimal decimalAmount = 0m;

                    int iMetalType = 100;

                    int iIngrdMType = Convert.ToInt32(dtIngredients.Rows[0]["METALTYPE"]);
                    // iMetalType = Convert.ToInt32(dtIngredients.Rows[0]["METALTYPE"]);
                    if(iIngrdMType == (int)MetalType.Gold
                        || iIngrdMType == (int)MetalType.Silver
                        || iIngrdMType == (int)MetalType.Palladium
                        || iIngrdMType == (int)MetalType.Platinum)
                    {
                        iMetalType = iIngrdMType;
                    }

                    foreach(DataRow dr in dtIngredients.Rows)
                    {
                        //iMetalType = GetMetalType(ItemID);  ///

                        //if (Convert.ToString(dr["METALTYPE"]) != "")
                        //{
                        //    iMetalType = Convert.ToInt32(dr["METALTYPE"]);
                        //}

                        //if (iMetalType == (int)MetalType.Gold)
                        if(iMetalType == Convert.ToInt32(dr["METALTYPE"]))
                        {
                            if(Convert.ToString(dr["Qty"]) != "")
                            {
                                dIngrdTotQty += Convert.ToDecimal(dr["Qty"]);
                            }
                        }
                    }
                    if(dIngrdTotQty > 0) //dWastQty
                    {
                        string sRate = "";

                        //// sRate = getWastageMetalRate(sBaseItemID, sBaseConfigID);
                        //sRate = getWastageMetalRate(Convert.ToString(dtIngredients.Rows[0]["ITEMID"])
                        //                            , Convert.ToString(dtIngredients.Rows[0]["ConfigID"]));
                        if(IsFixedRate)
                        {
                            sRate = Convert.ToString(dCustomerOrderFixedRate);
                        }
                        else
                        {
                            if((IsSaleAdjustment) && (!string.IsNullOrEmpty(sAdvAdjustmentGoldRate))) //14.01.2014
                            {
                                sRate = sAdvAdjustmentGoldRate;
                            }
                            else if((IsGSSTransaction) && (!string.IsNullOrEmpty(sGSSAdjustmentGoldRate))) //14.01.2014
                            {
                                sRate = sGSSAdjustmentGoldRate;
                            }
                            else
                            {
                                sRate = getWastageMetalRate(Convert.ToString(dtIngredients.Rows[0]["ITEMID"])
                                                            , Convert.ToString(dtIngredients.Rows[0]["ConfigID"]));
                            }
                        }

                        if(!string.IsNullOrEmpty(sRate))
                            dWMetalRate = Convert.ToDecimal(sRate);

                        if(dWMetalRate > 0)
                        {
                            // dWastageQty = dIngrdTotQty * (dWastQty / 100);

                            dWastageQty = decimal.Round(Convert.ToDecimal(dIngrdTotQty * (dWastQty / 100)), 3, MidpointRounding.AwayFromZero);

                            //decimalAmount = Convert.ToDecimal(txtRate.Text.Trim()) * dWastageQty;

                            decimalAmount = dWMetalRate * dWastageQty;

                            // txtWastageAmount.Text = Convert.ToString(decimalAmount);

                            txtWastagePercentage.Text = Convert.ToString(dWastQty);

                            txtWastageQty.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(dWastageQty), 3, MidpointRounding.AwayFromZero));//Convert.ToString(dWastageQty);

                            txtWastageAmount.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(decimalAmount), 2, MidpointRounding.AwayFromZero));
                        }
                    }

                }
                else
                {
                    if(!string.IsNullOrEmpty(txtQuantity.Text.Trim()))
                    {
                        if(Convert.ToDecimal(txtQuantity.Text) > 0)
                        {
                            Decimal dWastageQty = 0m;
                            Decimal decimalAmount = 0m;
                            int iMetalType;
                            string sRate = "";

                            //    iMetalType = GetMetalType(ItemID);
                            iMetalType = GetMetalType(sBaseItemID);

                            if(iMetalType == (int)MetalType.Gold)
                            {
                                sRate = getWastageMetalRate(sBaseItemID, sBaseConfigID);
                                if(!string.IsNullOrEmpty(sRate))
                                    dWMetalRate = Convert.ToDecimal(sRate);

                                if(dWMetalRate > 0)
                                {

                                    // dWastageQty = Convert.ToDecimal(txtQuantity.Text) * (Convert.ToDecimal(txtWastageQty.Text) / 100);
                                    dWastageQty = Convert.ToDecimal(txtQuantity.Text) * (dWastQty / 100);

                                    //decimalAmount = Convert.ToDecimal(txtRate.Text.Trim()) * dWastageQty;

                                    decimalAmount = dWMetalRate * dWastageQty;

                                    txtWastagePercentage.Text = Convert.ToString(dWastQty);
                                    txtWastageQty.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(dWastageQty), 3, MidpointRounding.AwayFromZero));//Convert.ToString(dWastageQty);

                                    // txtWastageAmount.Text = Convert.ToString(decimalAmount);
                                    txtWastageAmount.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(decimalAmount), 2, MidpointRounding.AwayFromZero));
                                }
                            }
                        }
                    }
                }


                //}
            }
            //if (cmbWastage.SelectedIndex == 1 && (string.IsNullOrEmpty(txtRate.Text.Trim()) || string.IsNullOrEmpty(txtWastageQty.Text.Trim())))
            else
            {
                txtWastagePercentage.Text = string.Empty;
                txtWastageQty.Text = string.Empty;
                txtWastageAmount.Text = string.Empty;
            }


            #endregion


        }
        #endregion

        #region Check Box Checked Changed
        private void chkOwn_CheckedChanged(object sender, EventArgs e)
        {
            if(!isViewing)
            {
                if(!isMRPUCP)
                {
                    txtRate.Text = getRateFromMetalTable();
                    //

                    if(chkOwn.Checked)
                    {
                        // string sBaseUnit = string.Empty;
                        cmbRateType.SelectedIndex = (int)RateType.Tot;

                        txtQuantity.Text = string.Empty;
                        txtLossPct.Text = "0";
                        txtLossPct.Enabled = false;
                        //  txtLossWeight.Text = "0";

                        txtTotalWeight.Text = string.Empty;
                        txtTotalWeight.Enabled = false;

                        txtPurity.Text = string.Empty;
                        txtPurity.Enabled = false;

                        #region

                        //if (dtExtndPurchExchng == null || dtExtndPurchExchng.Columns.Count == 0)
                        //{
                        //    dtExtndPurchExchng = new DataTable();
                        //    dtExtndPurchExchng.Columns.Add("GROSSWT", typeof(decimal));
                        //    dtExtndPurchExchng.Columns.Add("GROSSUNIT", typeof(decimal));
                        //    dtExtndPurchExchng.Columns.Add("DMDWT", typeof(decimal));
                        //    dtExtndPurchExchng.Columns.Add("DMDPCS", typeof(decimal));
                        //    dtExtndPurchExchng.Columns.Add("DMDUNIT", typeof(decimal));
                        //    dtExtndPurchExchng.Columns.Add("DMDAMOUNT", typeof(decimal));
                        //    dtExtndPurchExchng.Columns.Add("STONEWT", typeof(decimal));
                        //    dtExtndPurchExchng.Columns.Add("STONEPCS", typeof(decimal));
                        //    dtExtndPurchExchng.Columns.Add("STONEUNIT", typeof(decimal));
                        //    dtExtndPurchExchng.Columns.Add("STONEAMOUNT", typeof(decimal));
                        //    dtExtndPurchExchng.Columns.Add("NETWT", typeof(decimal));
                        //    dtExtndPurchExchng.Columns.Add("NETPCS", typeof(decimal));
                        //    dtExtndPurchExchng.Columns.Add("NETUNIT", typeof(decimal));
                        //    dtExtndPurchExchng.Columns.Add("NETPURITY", typeof(decimal));
                        //    dtExtndPurchExchng.Columns.Add("NETAMOUNT", typeof(decimal));

                        //    dtExtndPurchExchng.AcceptChanges();

                        //}

                        #endregion

                        frmExtendedPurchExch ObjExtendedPurchExch = new frmExtendedPurchExch(sBaseItemID, sBaseUnitId, sBaseConfigID, txtRate.Text,
                                                                    this, dtExtndPurchExchng);

                        ObjExtendedPurchExch.Show();

                        // txtQuantity.Text = string.IsNullOrEmpty(txtTotalWeight.Text.Trim()) ? string.Empty : txtTotalWeight.Text.Trim();


                        txtLossPct_TextChanged(sender, e);
                    }
                    else
                    {
                        if(!rdbSale.Checked)
                        {
                            txtTotalWeight.Enabled = true;
                            txtPurity.Enabled = true;
                            txtLossPct.Enabled = true;

                            txtQuantity.Text = string.Empty;
                            GetPurcExchngGoldQty();
                            txtLossPct_TextChanged(sender, e);
                        }
                    }

                    //
                }


            }
        }
        #endregion

        #region  GET RATE FROM METAL TABLE
        private string getRateFromMetalTable()
        {

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) DECLARE @CUSTCLASSCODE VARCHAR(20)");
            commandText.Append(" DECLARE @METALTYPE INT ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM         RETAILCHANNELTABLE INNER JOIN ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + StoreID.Trim() + "'  ");
            commandText.Append(" SELECT @METALTYPE=METALTYPE FROM [INVENTTABLE] WHERE ITEMID='" + ItemID.Trim() + "' ");
            //added on 09/02/16
            commandText.Append(" SELECT @CUSTCLASSCODE = CUSTCLASSIFICATIONID FROM [CUSTTABLE] WHERE ACCOUNTNUM = '" + sCustomerId + "' ");

            if(dtIngredientsClone != null && dtIngredientsClone.Rows.Count > 0)
            {
                commandText.Append(" IF(@METALTYPE IN ('" + (int)MetalType.Gold + "','" + (int)MetalType.Silver + "','" + (int)MetalType.Platinum + "','" + (int)MetalType.Palladium + "')) ");
                commandText.Append(" BEGIN ");
            }

            commandText.Append(" SELECT TOP 1 CAST(RATES AS decimal (16,2)) FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION  ");
            //  commandText.Append(" AND DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0)<=TRANSDATE AND METALTYPE=@METALTYPE ");
            commandText.Append(" AND METALTYPE=@METALTYPE ");

            if(rdbSale.Checked)
            {
                commandText.Append(" AND RETAIL=1 AND RATETYPE='" + (int)RateTypeNew.Sale + "' ");
                chkOwn.Enabled = true;

            }
            else if(rdbExchange.Checked || rdbExchangeReturn.Checked)
            {
                /* Blocked on 31.08.2013 ---  Rate Type -- > Other Exchange rate added
                commandText.Append(" AND RATETYPE='" + (int)RateTypeNew.Exchange + "' ");
                isViewing = true;
                chkOwn.Checked = true;
                chkOwn.Enabled = false;
                isViewing = false;
                 */
                if(chkOwn.Checked)
                    commandText.Append(" AND RATETYPE='" + (int)RateTypeNew.Exchange + "' ");
                else
                    commandText.Append(" AND RATETYPE='" + (int)RateTypeNew.OtherExchange + "' ");
                chkOwn.Enabled = true;

            }
            else
            {
                if(chkOwn.Checked)
                    commandText.Append(" AND RATETYPE='" + (int)RateTypeNew.OGP + "' ");
                else
                    commandText.Append(" AND RATETYPE='" + (int)RateTypeNew.OGOP + "' ");
                chkOwn.Enabled = true;
                // chkOwn.Checked = false;
            }

            commandText.Append(" AND ACTIVE=1 AND CONFIGIDSTANDARD='" + ConfigID.Trim() + "' ");
            commandText.Append("   ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC ");
            //    commandText.Append(" ORDER BY [TRANSDATE] ");
            //  commandText.Append(" ,[TIME] ");
            //   commandText.Append(" DESC ");

            //   SELECT CONVERT(DATETIME,SUBSTRING(CONVERT(VARCHAR(10),DATEADD(dd,DATEDIFF(dd,0,CAST(TRANSDATE AS VARCHAR)),0),120) + ' ' +
            //   CAST(CAST(cast(([TIME] / 3600) as varchar(10)) + ':' + cast(([TIME] % 60) as varchar(10)) AS TIME) AS VARCHAR),0,24),121)  FROM METALRATES
            string grweigh = string.Empty;
            if(string.IsNullOrEmpty(GrWeight))
                grweigh = "0";
            else
                grweigh = GrWeight;

            if(dtIngredientsClone != null && dtIngredientsClone.Rows.Count > 0)
            {
                //commandText.Append(" END ");
                //commandText.Append(" ELSE ");
                //commandText.Append(" BEGIN ");
                //commandText.Append(" SELECT  CAST(UNIT_RATE AS decimal (6,2)) FROM RETAILCUSTOMERSTONEAGGREEMENTDETAIL WHERE  ");
                //commandText.Append(" WAREHOUSE=@INVENTLOCATION AND ('" + grweigh + "' BETWEEN FROM_WEIGHT AND TO_WEIGHT)  ");
                //commandText.Append(" AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN FROMDATE AND TODATE) ");
                //commandText.Append(" AND ITEMID='" + ItemID.Trim() + "' AND INVENTBATCHID='" + BatchID.Trim() + "' ");
                //commandText.Append(" AND INVENTSIZEID='" + SizeID.Trim() + "' AND INVENTCOLORID='" + ColorID.Trim() + "' ");
                //commandText.Append(" END ");

                commandText.Append(" END ");
                commandText.Append(" ELSE ");
                commandText.Append(" BEGIN ");

                commandText.Append(" SELECT     CAST(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.UNIT_RATE AS numeric(28, 2))   ");
                commandText.Append(" FROM         RETAILCUSTOMERSTONEAGGREEMENTDETAIL INNER JOIN ");
                commandText.Append(" INVENTDIM ON RETAILCUSTOMERSTONEAGGREEMENTDETAIL.INVENTDIMID = INVENTDIM.INVENTDIMID ");
                //commandText.Append(" WHERE     (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE = @INVENTLOCATION) AND ('" + grweigh + "' BETWEEN RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FROM_WEIGHT AND  ");
                commandText.Append(" WHERE     (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE = @INVENTLOCATION) AND (  ");
                commandText.Append(dStoneWtRange);
                commandText.Append(" BETWEEN RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FROM_WEIGHT AND  ");
                commandText.Append("  RETAILCUSTOMERSTONEAGGREEMENTDETAIL.TO_WEIGHT) AND (DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0) BETWEEN  ");
                commandText.Append(" RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ToDate) AND  ");
                //  commandText.Append("  (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID = '" + ItemID.Trim() + "') AND (INVENTDIM.INVENTBATCHID = '" + BatchID.Trim() + "') AND  ");
                commandText.Append("  (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID = '" + ItemID.Trim() + "')  AND  ");
                //    commandText.Append("  (INVENTDIM.INVENTSIZEID = '" + SizeID.Trim() + "') AND (INVENTDIM.INVENTCOLORID = '" + ColorID.Trim() + "') "); // 21.01.2014 // REMOVE TRIM
                commandText.Append("  (INVENTDIM.INVENTSIZEID = '" + SizeID + "') AND (INVENTDIM.INVENTCOLORID = '" + ColorID + "') ");

                commandText.Append(" AND(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.AccountNum='" + sCustomerId.Trim() + "' ");
                commandText.Append(" OR RETAILCUSTOMERSTONEAGGREEMENTDETAIL.AccountNum='') AND ");
                commandText.Append(" (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.CustClassificationId=@CUSTCLASSCODE ");
                commandText.Append(" OR RETAILCUSTOMERSTONEAGGREEMENTDETAIL.CustClassificationId='')");


                //commandText.Append(" AND (INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "')");
                commandText.Append(" AND (INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "') AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ACTIVATE = 1"); // added on 02.09.2013
                // commandText.Append(" ORDER BY RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate DESC"); // Added on 29.05.2013
                commandText.Append(" ORDER BY RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID DESC, RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate DESC");  // Changed order sequence on 03.06.2013 as per u.das
                commandText.Append(" END ");
            }



            //   commandText.Append("AND CAST(cast(([TIME] / 3600) as varchar(10)) + ':' + cast(([TIME] % 60) as varchar(10)) AS TIME)<=CAST(CONVERT(VARCHAR(8),GETDATE(),108) AS TIME)  ");

            if(conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if(conn.State == ConnectionState.Open)
                conn.Close();

            if(string.IsNullOrEmpty(sResult))
                sResult = "0";

            return Convert.ToString(sResult.Trim());

        }
        #endregion

        #region  Get Ingredient Calculation Type

        // private string GetIngredientCalcType()  
        private string GetIngredientCalcType(ref int iSDisctype, ref decimal dSDiscAmt)  // modified on 30.04.2013 // Stone Discount
        {
            string sResult = string.Empty;
            StringBuilder commandText = new StringBuilder();

            string grweigh = string.Empty;
            if(string.IsNullOrEmpty(GrWeight))
                grweigh = "0";
            else
                grweigh = GrWeight;

            // if (dtIngredientsClone != null && dtIngredientsClone.Rows.Count > 0)
            // {
            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) DECLARE @CUSTCLASSCODE VARCHAR(20) ");
            commandText.Append(" DECLARE @METALTYPE INT ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM  RETAILCHANNELTABLE INNER JOIN ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + StoreID.Trim() + "'  ");
            commandText.Append(" SELECT @METALTYPE=METALTYPE FROM [INVENTTABLE] WHERE ITEMID='" + ItemID.Trim() + "' ");
            //added on 09/02/16
            commandText.Append(" SELECT @CUSTCLASSCODE = CUSTCLASSIFICATIONID FROM [CUSTTABLE] WHERE ACCOUNTNUM = '" + sCustomerId + "' ");

            commandText.Append(" IF(@METALTYPE IN ('" + (int)MetalType.LooseDmd + "','" + (int)MetalType.Stone + "')) ");
            commandText.Append(" BEGIN ");

            // commandText.Append(" SELECT     CAST(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.UNIT_RATE AS numeric(28, 2))   ");
            commandText.Append(" SELECT   TOP 1  ISNULL(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.CALCTYPE,0) AS CALCTYPE,ISNULL(DISCTYPE,0) AS DISCTYPE, ISNULL(DISCAMT,0) AS DISCAMT ");
            commandText.Append(" FROM         RETAILCUSTOMERSTONEAGGREEMENTDETAIL INNER JOIN ");
            commandText.Append(" INVENTDIM ON RETAILCUSTOMERSTONEAGGREEMENTDETAIL.INVENTDIMID = INVENTDIM.INVENTDIMID ");
            //  commandText.Append(" WHERE     (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE = @INVENTLOCATION) AND ('" + grweigh + "' BETWEEN RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FROM_WEIGHT AND  ");
            commandText.Append(" WHERE     (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE = @INVENTLOCATION) AND (  ");
            commandText.Append(dStoneWtRange);
            commandText.Append(" BETWEEN RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FROM_WEIGHT AND  ");
            commandText.Append("  RETAILCUSTOMERSTONEAGGREEMENTDETAIL.TO_WEIGHT) AND (DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0) BETWEEN  ");
            commandText.Append(" RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ToDate) AND  ");
            //  commandText.Append("  (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID = '" + ItemID.Trim() + "') AND (INVENTDIM.INVENTBATCHID = '" + BatchID.Trim() + "') AND  ");
            commandText.Append("  (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID = '" + ItemID.Trim() + "')  AND  ");
            // commandText.Append("  (INVENTDIM.INVENTSIZEID = '" + SizeID.Trim() + "') AND (INVENTDIM.INVENTCOLORID = '" + ColorID.Trim() + "') "); //21.01.2014
            commandText.Append("  (INVENTDIM.INVENTSIZEID = '" + SizeID + "') AND (INVENTDIM.INVENTCOLORID = '" + ColorID + "') ");

            commandText.Append(" AND(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.AccountNum='" + sCustomerId.Trim() + "' ");
            commandText.Append(" OR RETAILCUSTOMERSTONEAGGREEMENTDETAIL.AccountNum='') AND ");
            commandText.Append(" (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.CustClassificationId=@CUSTCLASSCODE ");
            commandText.Append(" OR RETAILCUSTOMERSTONEAGGREEMENTDETAIL.CustClassificationId='')");

            //commandText.Append(" AND (INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "')"); 
            commandText.Append(" AND (INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "') AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ACTIVATE = 1"); // modified on 02.09.2013
            // commandText.Append(" ORDER BY RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate DESC"); // Added on 29.05.2013
            commandText.Append(" ORDER BY RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID DESC, RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate DESC");  // Changed order sequence on 03.06.2013 as per u.das
            commandText.Append(" END ");
            // }



            //   commandText.Append("AND CAST(cast(([TIME] / 3600) as varchar(10)) + ':' + cast(([TIME] % 60) as varchar(10)) AS TIME)<=CAST(CONVERT(VARCHAR(8),GETDATE(),108) AS TIME)  ");

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
                        sResult = Convert.ToString(reader.GetValue(0));
                        iSDisctype = Convert.ToInt32(reader.GetValue(1));
                        dSDiscAmt = Convert.ToDecimal(reader.GetValue(2));
                    }
                }
            }
            //string sResult = Convert.ToString(command.ExecuteScalar());

            if(conn.State == ConnectionState.Open)
                conn.Close();

            return Convert.ToString(sResult.Trim());

        }
        #endregion


        #region Ingredient Details Click
        private void btnIngrdientsDetails_Click(object sender, EventArgs e)
        {
            frmSKUDetails oSKUDetails = new frmSKUDetails(dtIngredientsClone);
            oSKUDetails.ShowDialog();
        }
        #endregion

        #region Select Item
        private void btnSelectItems_Click(object sender, EventArgs e)
        {
            if(isViewing)
            {
                SelectItems(LineNum);

                CustomerOrderDetails oCustOrder = new CustomerOrderDetails(dsCustOrder, string.Empty);
                oCustOrder.ShowDialog();
            }
            else
            {
                SelectItems("");

                CustomerOrderDetails oCustOrder = new CustomerOrderDetails(dsCustOrder, index);
                oCustOrder.ShowDialog();
                index = oCustOrder.index;
                if(oCustOrder.lineId == 0)
                {
                    lineNum = oCustOrder.lineId;
                    lblItemSelected.Text = "NO ITEM SELECTED";
                    // Fixed Metal Rate New
                    BindIngredientGrid();
                    if(!string.IsNullOrEmpty(txtQuantity.Text) && !isMRPUCP)
                    {
                        CheckRateFromDB();
                        //CheckMakingDiscountFromDB();
                    }
                }
                else
                {
                    lineNum = oCustOrder.lineId;
                    lblItemSelected.Text = " SELECTED LINE NO. : " + oCustOrder.lineId;
                    // Fixed Metal Rate New
                    decimal dFixedMetalRateVal = 0m;
                    string sFixedMetalRateConfigId = string.Empty;

                    if(!IsGSSTransaction)
                    {
                        if(Convert.ToBoolean(GetCustOrderFixedRateInfo(Convert.ToString(cmbCustomerOrder.SelectedValue),
                                                                         ref dFixedMetalRateVal, ref sFixedMetalRateConfigId)))
                        {
                            // -- give pop up window for fixed rate yes / no -- if yes --> then
                            // -- give metal rate value in Message Box
                            using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Consider Fixed Metal Rate?", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                if(Convert.ToString(dialog.DialogResult).ToUpper().Trim() == "YES")
                                {
                                    BindIngredientGrid(Convert.ToString(dFixedMetalRateVal), sFixedMetalRateConfigId);

                                    if(!string.IsNullOrEmpty(txtQuantity.Text) && !isMRPUCP)
                                    {
                                        CheckRateFromDB(true);
                                        // CheckMakingDiscountFromDB();
                                    }
                                }
                            }
                        }
                    }

                    //--- End
                }
            }
        }
        #endregion

        #region Common Helper
        private void CommonHelper()
        {
            chkOwn.Visible = true;
            txtRate.Text = string.Empty;
            txtRate.Text = getRateFromMetalTable();

            btnIngrdientsDetails.Visible = false;
            dtIngredients = new DataTable();
            dtIngredientsClone = new DataTable();
            panel4.Enabled = false;
            cmbCustomerOrder.DataSource = null;
            cmbCustomerOrder.Items.Add("NO CUSTOMER ORDER");
            cmbCustomerOrder.SelectedIndex = 0;
            btnSelectItems.Visible = false;

            cmbCustomerOrder.Enabled = false;
            lblItemSelected.Text = string.Empty;
        }
        #endregion

        #region Selected Index Change
        private void cmbCustomerOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Fixed Metal Rate New
            //decimal dFixedMetalRateVal = 0m;
            //string sFixedMetalRateConfigId = string.Empty;
            //if (!IsGSSTransaction)
            //{
            //    if (Convert.ToBoolean(GetCustOrderFixedRateInfo(Convert.ToString(cmbCustomerOrder.SelectedValue),
            //                                                    ref dFixedMetalRateVal, ref sFixedMetalRateConfigId)))
            //    {
            if(iCallfrombase == 0)
            {
                BindIngredientGrid();

                if(!string.IsNullOrEmpty(txtQuantity.Text) && !isMRPUCP)
                {
                    CheckRateFromDB();
                    //CheckMakingDiscountFromDB();
                }
            }
            //    }
            //}
            // ----------

            if(Convert.ToString(cmbCustomerOrder.SelectedValue).ToUpper().Trim() == "NO SELECTION")
            {

                lblItemSelected.Text = string.Empty;
                btnSelectItems.Visible = false;

                return;
            }
            else
            {
                lblItemSelected.Text = string.Empty;
                btnSelectItems.Visible = true;
            }

            // Fixed Metal Rate New
            // BindIngredientGrid();
        }
        #endregion

        #region Select Items
        private void SelectItems(string linenum)
        {
            dsCustOrder = new DataSet();
            if(conn.State == ConnectionState.Closed)
                conn.Open();
            string commandText = string.Empty;
            if(isViewing)
            {
                commandText = " SELECT [ORDERNUM],[LINENUM],[STOREID],[TERMINALID],[ITEMID] " +
                                      " ,[CONFIGID],[CODE],[SIZEID] ,[STYLE],[PCS],[QTY],[CRATE] AS [RATE],[RATETYPE] " +
                                      " ,[AMOUNT],[MAKINGRATE],[MAKINGRATETYPE],[MAKINGAMOUNT] " +
                                      " ,[EXTENDEDDETAILSAMOUNT],[DATAAREAID],[CREATEDON],[STAFFID] " +
                                      " FROM [CUSTORDER_DETAILS] " +
                                      " WHERE [ORDERNUM]='" + cmbCustomerOrder.SelectedValue + "' AND  [LINENUM]=" + linenum + ";" +
                                      " SELECT  [ORDERNUM],[ORDERDETAILNUM],[LINENUM],[STOREID] " +
                                      " ,[TERMINALID],[ITEMID],[CONFIGID],[CODE],[SIZEID] " +
                                      " ,[STYLE],[PCS],[QTY],[CRATE] AS [RATE],[RATETYPE],[DATAAREAID] " +
                                      " ,[AMOUNT] FROM [CUSTORDER_SUBDETAILS] " +
                                      " WHERE [ORDERNUM]='" + cmbCustomerOrder.SelectedValue + "' ;";
            }
            else
            {
                commandText = " SELECT [ORDERNUM],[LINENUM],[STOREID],[TERMINALID],[ITEMID] " +
                                     " ,[CONFIGID],[CODE],[SIZEID] ,[STYLE],[PCS],[QTY],[CRATE] AS [RATE],[RATETYPE] " +
                                     " ,[AMOUNT],[MAKINGRATE],[MAKINGRATETYPE],[MAKINGAMOUNT] " +
                                     " ,[EXTENDEDDETAILSAMOUNT],[DATAAREAID],[CREATEDON],[STAFFID] " +
                                     " FROM [CUSTORDER_DETAILS] " +
                                     " WHERE [ORDERNUM]='" + cmbCustomerOrder.SelectedValue + "' AND isDelivered='0';" +
                                     " SELECT  [ORDERNUM],[ORDERDETAILNUM],[LINENUM],[STOREID] " +
                                     " ,[TERMINALID],[ITEMID],[CONFIGID],[CODE],[SIZEID] " +
                                     " ,[STYLE],[PCS],[QTY],[CRATE] AS [RATE],[RATETYPE],[DATAAREAID] " +
                                     " ,[AMOUNT] FROM [CUSTORDER_SUBDETAILS] " +
                                     " WHERE [ORDERNUM]='" + cmbCustomerOrder.SelectedValue + "' ;";
            }

            //  SqlCommand command = new SqlCommand(commandText, conn);
            //  command.CommandTimeout = 0;
            SqlDataAdapter adapter = new SqlDataAdapter(commandText, conn);
            adapter.Fill(dsCustOrder);

            if(dsCustOrder == null && dsCustOrder.Tables.Count <= 0 && dsCustOrder.Tables[0].Rows.Count <= 0)
            {
                MessageBox.Show("NO ITEM DETAILS PRESENT.", "WARNING ! ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }



            if(conn.State == ConnectionState.Open)
                conn.Close();


        }
        #endregion

        #region Numpad Enter Pressed
        private void numAmount_EnterButtonPressed()
        {
            if(string.IsNullOrEmpty(txtPCS.Text) || sNumPadEntryType.ToUpper().Trim() == "P")
            {
                txtPCS.Text = numpadEntry.EnteredValue;
                numpadEntry.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Numeric;
                if(rdbSale.Checked)
                {
                    txtQuantity.Focus();
                    numpadEntry.PromptText = "Enter Quantity:";
                }
                else
                {
                    txtRate.Focus();
                    numpadEntry.PromptText = "Enter Rate:";
                }

                numpadEntry.Refresh();
                numpadEntry.Focus();

                return;
            }
            if(rdbSale.Checked)
            {

                if(string.IsNullOrEmpty(txtQuantity.Text) || sNumPadEntryType.ToUpper().Trim() == "Q")
                {
                    txtQuantity.Text = numpadEntry.EnteredValue;
                    numpadEntry.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Price;
                    txtRate.Focus();
                    numpadEntry.PromptText = "Enter Rate:";
                    numpadEntry.Refresh();
                    numpadEntry.Focus();


                }

                else if(string.IsNullOrEmpty(txtRate.Text) || sNumPadEntryType.ToUpper().Trim() == "R")
                {
                    txtRate.Text = numpadEntry.EnteredValue;
                    numpadEntry.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Numeric;
                    txtMakingRate.Focus();
                    numpadEntry.PromptText = "Enter Making Rate:";
                    numpadEntry.Refresh();
                    numpadEntry.Focus();


                }

                else if(string.IsNullOrEmpty(txtMakingRate.Text) || sNumPadEntryType.ToUpper().Trim() == "MR")
                {
                    txtMakingRate.Text = numpadEntry.EnteredValue;
                    numpadEntry.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Price;
                    txtMakingDisc.Focus();
                    numpadEntry.PromptText = "Enter Making Discount:";

                    numpadEntry.Refresh();
                    numpadEntry.Focus();

                }

                else if(string.IsNullOrEmpty(txtMakingDisc.Text) || sNumPadEntryType.ToUpper().Trim() == "MD")
                {
                    txtMakingDisc.Text = numpadEntry.EnteredValue;
                    numpadEntry.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Numeric;
                    numpadEntry.PromptText = "Enter Pieces:";
                    sNumPadEntryType = "P";
                    numpadEntry.Refresh();
                    numpadEntry.Focus();


                }



                return;
            }
            else
            {
                if(string.IsNullOrEmpty(txtRate.Text) || sNumPadEntryType.ToUpper().Trim() == "R")
                {
                    txtRate.Text = numpadEntry.EnteredValue;
                    numpadEntry.PromptText = "Enter Total Weight:";
                    numpadEntry.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Price;
                    txtTotalWeight.Focus();
                }

                else if(string.IsNullOrEmpty(txtTotalWeight.Text) || sNumPadEntryType.ToUpper().Trim() == "TW")
                {
                    txtTotalWeight.Text = numpadEntry.EnteredValue;
                    numpadEntry.PromptText = "";
                    numpadEntry.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Numeric;
                    txtLossPct.Focus();
                }

                else if(string.IsNullOrEmpty(txtPurity.Text) || sNumPadEntryType.ToUpper().Trim() == "PUR") //
                {
                    txtPurity.Text = numpadEntry.EnteredValue;
                    numpadEntry.PromptText = "";
                    numpadEntry.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Numeric;
                    txtLossPct.Focus();
                }

                else if(string.IsNullOrEmpty(txtLossPct.Text) || sNumPadEntryType.ToUpper().Trim() == "LP")
                {
                    txtLossPct.Text = numpadEntry.EnteredValue;
                    numpadEntry.PromptText = "Enter Pieces:";
                    numpadEntry.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Price;
                    sNumPadEntryType = "P";
                }

                numpadEntry.Refresh();
                numpadEntry.Focus();
                numpadEntry.EnteredValue = string.Empty;
                return;
            }
        }
        #endregion

        private void txtPCS_Enter(object sender, EventArgs e)
        {
            sNumPadEntryType = "P";
            numpadEntry.PromptText = "Enter Pieces:";
        }

        private void txtQuantity_Enter(object sender, EventArgs e)
        {
            sNumPadEntryType = "Q";
            numpadEntry.PromptText = "Enter Quantity:";
        }

        private void txtRate_Enter(object sender, EventArgs e)
        {
            sNumPadEntryType = "R";
            numpadEntry.PromptText = "Enter Rate:";
        }

        private void txtMakingRate_Enter(object sender, EventArgs e)
        {
            sNumPadEntryType = "MR";
            numpadEntry.PromptText = "Enter Making Rate:";
        }

        private void txtMakingDisc_Enter(object sender, EventArgs e)
        {
            sNumPadEntryType = "MD";
            numpadEntry.PromptText = "Enter Making Discount:";
        }

        private void txtTotalWeight_Enter(object sender, EventArgs e)
        {
            sNumPadEntryType = "TW";
            numpadEntry.PromptText = "Enter Total Weight:";
        }

        private void txtPurity_Enter(object sender, EventArgs e)
        {
            sNumPadEntryType = "PUR";
            numpadEntry.PromptText = "Enter Purity:";
        }

        private void txtLossPct_Enter(object sender, EventArgs e)
        {
            sNumPadEntryType = "LP";
            numpadEntry.PromptText = "Enter Loss Percentage:";

        }

        protected int GetMetalType(string sItemId)
        {
            int iMetalType = 100;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select metaltype from inventtable where itemid='" + sItemId + "'");

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
                        iMetalType = (int)reader.GetValue(0);
                    }
                }
            }
            if(conn.State == ConnectionState.Open)
                conn.Close();
            return iMetalType;

        }

        #region  GET Wastage Metal rate
        private string getWastageMetalRate(string sItemId, string sConfigId)
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) ");
            commandText.Append(" DECLARE @METALTYPE INT ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM         RETAILCHANNELTABLE INNER JOIN ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + StoreID.Trim() + "'  ");
            commandText.Append(" SELECT @METALTYPE=METALTYPE FROM [INVENTTABLE] WHERE ITEMID='" + sItemId.Trim() + "' ");

            //if (dtIngredientsClone != null && dtIngredientsClone.Rows.Count > 0)
            //{
            commandText.Append(" IF(@METALTYPE IN ('" + (int)MetalType.Gold + "','" + (int)MetalType.Silver + "','" + (int)MetalType.Platinum + "','" + (int)MetalType.Palladium + "')) ");
            commandText.Append(" BEGIN ");
            //}

            commandText.Append(" SELECT TOP 1 CAST(RATES AS decimal (6,2)) FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION  ");
            //  commandText.Append(" AND DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0)<=TRANSDATE AND METALTYPE=@METALTYPE ");
            commandText.Append(" AND METALTYPE=@METALTYPE ");

            if(rdbSale.Checked)
            {
                commandText.Append(" AND RETAIL=1 AND RATETYPE='" + (int)RateTypeNew.Sale + "' ");
                // chkOwn.Enabled = true;

            }
            //else if (rdbExchange.Checked || rdbExchangeReturn.Checked)
            //{
            //    commandText.Append(" AND RATETYPE='" + (int)RateTypeNew.Exchange + "' ");
            //    isViewing = true;
            //    chkOwn.Checked = true;
            //    chkOwn.Enabled = false;
            //    isViewing = false;
            //}
            //else
            //{
            //    if (chkOwn.Checked)
            //        commandText.Append(" AND RATETYPE='" + (int)RateTypeNew.OGP + "' ");
            //    else
            //        commandText.Append(" AND RATETYPE='" + (int)RateTypeNew.OGOP + "' ");
            //    chkOwn.Enabled = true;
            //    // chkOwn.Checked = false;
            //}

            commandText.Append(" AND ACTIVE=1 AND CONFIGIDSTANDARD='" + sConfigId.Trim() + "' ");
            commandText.Append("   ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC ");
            commandText.Append(" END ");
            //    commandText.Append(" ORDER BY [TRANSDATE] ");
            //  commandText.Append(" ,[TIME] ");
            //   commandText.Append(" DESC ");

            //   SELECT CONVERT(DATETIME,SUBSTRING(CONVERT(VARCHAR(10),DATEADD(dd,DATEDIFF(dd,0,CAST(TRANSDATE AS VARCHAR)),0),120) + ' ' +
            //   CAST(CAST(cast(([TIME] / 3600) as varchar(10)) + ':' + cast(([TIME] % 60) as varchar(10)) AS TIME) AS VARCHAR),0,24),121)  FROM METALRATES

            //   commandText.Append("AND CAST(cast(([TIME] / 3600) as varchar(10)) + ':' + cast(([TIME] % 60) as varchar(10)) AS TIME)<=CAST(CONVERT(VARCHAR(8),GETDATE(),108) AS TIME)  ");

            if(conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if(conn.State == ConnectionState.Open)
                conn.Close();

            return Convert.ToString(sResult.Trim());

        }
        #endregion
        #region GSS Maturity

        private decimal getMetalRate(string sItemId, string sConfigId)
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) ");
            commandText.Append(" DECLARE @METALTYPE INT ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM         RETAILCHANNELTABLE INNER JOIN ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + StoreID.Trim() + "'  ");
            commandText.Append(" SELECT @METALTYPE=METALTYPE FROM [INVENTTABLE] WHERE ITEMID='" + sItemId.Trim() + "' ");

            //if (dtIngredientsClone != null && dtIngredientsClone.Rows.Count > 0)
            //{
            commandText.Append(" IF(@METALTYPE IN ('" + (int)MetalType.Gold + "','" + (int)MetalType.Silver + "','" + (int)MetalType.Platinum + "','" + (int)MetalType.Palladium + "')) ");
            commandText.Append(" BEGIN ");
            //}

            commandText.Append(" SELECT TOP 1 CAST(ISNULL(RATES,0) AS decimal (6,2)) FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION ");
            //  commandText.Append(" AND DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0)<=TRANSDATE AND METALTYPE=@METALTYPE ");
            commandText.Append(" AND METALTYPE=@METALTYPE ");

            if(rdbSale.Checked)
            {
                commandText.Append(" AND RETAIL=1 AND RATETYPE='" + (int)RateTypeNew.Sale + "' ");
                // chkOwn.Enabled = true;

            }
            //else if (rdbExchange.Checked || rdbExchangeReturn.Checked)
            //{
            //    commandText.Append(" AND RATETYPE='" + (int)RateTypeNew.Exchange + "' ");
            //    isViewing = true;
            //    chkOwn.Checked = true;
            //    chkOwn.Enabled = false;
            //    isViewing = false;
            //}
            //else
            //{
            //    if (chkOwn.Checked)
            //        commandText.Append(" AND RATETYPE='" + (int)RateTypeNew.OGP + "' ");
            //    else
            //        commandText.Append(" AND RATETYPE='" + (int)RateTypeNew.OGOP + "' ");
            //    chkOwn.Enabled = true;
            //    // chkOwn.Checked = false;
            //}

            commandText.Append(" AND ACTIVE=1 AND CONFIGIDSTANDARD='" + sConfigId.Trim() + "' ");
            commandText.Append("   ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC ");
            commandText.Append(" END ");
            //    commandText.Append(" ORDER BY [TRANSDATE] ");
            //  commandText.Append(" ,[TIME] ");
            //   commandText.Append(" DESC ");

            //   SELECT CONVERT(DATETIME,SUBSTRING(CONVERT(VARCHAR(10),DATEADD(dd,DATEDIFF(dd,0,CAST(TRANSDATE AS VARCHAR)),0),120) + ' ' +
            //   CAST(CAST(cast(([TIME] / 3600) as varchar(10)) + ':' + cast(([TIME] % 60) as varchar(10)) AS TIME) AS VARCHAR),0,24),121)  FROM METALRATES

            //   commandText.Append("AND CAST(cast(([TIME] / 3600) as varchar(10)) + ':' + cast(([TIME] % 60) as varchar(10)) AS TIME)<=CAST(CONVERT(VARCHAR(8),GETDATE(),108) AS TIME)  ");

            if(conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if(conn.State == ConnectionState.Open)
                conn.Close();
            if(!string.IsNullOrEmpty(sResult))
            {
                return Convert.ToDecimal(sResult.Trim());
            }
            else
            {
                return 0;
            }

        }

        private decimal GetSKUQty(string sItemId)
        {
            decimal dSKUQty = 0m;

            /* //SKU Table New
            string commandText = " SELECT QTY FROM SKUTable_Posted " +
                                    " WHERE (SKUTable_Posted.SkuNumber = '" + ItemID.Trim() + "') ";
             */

            string commandText = " SELECT QTY FROM SKUTableTrans " + //SKU Table New
                                    " WHERE (SKUTableTrans.SkuNumber = '" + ItemID.Trim() + "') ";
            //-------

            SqlCommand command = new SqlCommand(commandText, conn);
            command.CommandTimeout = 0;
            if(conn.State == ConnectionState.Closed)
                conn.Open();

            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if(conn.State == ConnectionState.Open)
                conn.Close();
            if(!string.IsNullOrEmpty(sResult))
            {
                dSKUQty = Convert.ToDecimal(sResult.Trim());
            }
            else
            {
                dSKUQty = 0;
            }
            //using (SqlDataReader reader = command.ExecuteReader())
            //{
            //    if (reader.HasRows)
            //    {
            //        string strVal =Convert.ToString(reader.GetValue(0));
            //        dSKUQty = decimal.Round(Convert.ToDecimal(strVal), 3, MidpointRounding.AwayFromZero);
            //    }
            //}
            if(conn.State == ConnectionState.Open)
                conn.Close();
            return dSKUQty;

        }

        #region [ old -- without purity]
        //private decimal getGSSRate(string GrWeight)
        //{
        //    string sRate = string.Empty;
        //    #region GSS
        //    //decimal dActualSaleQty = 0;
        //    //decimal dGSSMaturityQty = //Convert.ToDecimal(retailTrans.PartnerData.GSSTotQty);

        //    // if(!string.IsNullOrEmpty(txtQuantity.Text))
        //    if (!string.IsNullOrEmpty(GrWeight))
        //    {
        //        if (dPrevRunningQtyGSS == 0)
        //        {
        //            dActualGSSSaleWt = dPrevGSSSaleWt + Convert.ToDecimal(GrWeight);
        //            // dPrevRunningQtyGSS += Convert.ToDecimal(GrWeight);
        //        }
        //        else
        //        {
        //            dActualGSSSaleWt = dPrevGSSSaleWt + Convert.ToDecimal(GrWeight) + dPrevRunningQtyGSS;
        //        }
        //    }

        //    if (dActualGSSSaleWt <= dGSSMaturityQty)
        //    {
        //        sRate = Convert.ToString(dGSSAvgRate);
        //    }
        //    else  //if (dActualGSSSaleWt > dGSSMaturityQty)
        //    {
        //        if (dPrevGSSSaleWt >= dGSSMaturityQty)
        //        {
        //            sRate = getRateFromMetalTable();
        //        }
        //        else
        //        {
        //            //  if ((dPrevGSSSaleWt + Convert.ToDecimal(GrWeight)+ dPrevRunningQtyGSS ) == dGSSMaturityQty)
        //            if (dPrevGSSSaleWt + dPrevRunningQtyGSS >= dGSSMaturityQty)
        //            {
        //                sRate = getRateFromMetalTable();
        //            }
        //            else
        //            {
        //                decimal dAvgRateQty = 0m;
        //                decimal dCurrentRateQty = 0m;
        //                decimal dCurrentRate = 0m;

        //                dCurrentRateQty = (dActualGSSSaleWt - dGSSMaturityQty - dPrevRunningQtyGSS);
        //                dAvgRateQty = Convert.ToDecimal(GrWeight) - dCurrentRateQty;

        //                dCurrentRate = getMetalRate(ItemID, ConfigID);

        //                if (dGSSAvgRate > 0 && dCurrentRate > 0)
        //                {
        //                    sRate = Convert.ToString(((dGSSAvgRate * dAvgRateQty) + (dCurrentRateQty * dCurrentRate)) / Convert.ToDecimal(GrWeight));
        //                }
        //            }
        //        }

        //    }

        //    if (!string.IsNullOrEmpty(sRate))
        //    {
        //        dPrevRunningQtyGSS += Convert.ToDecimal(GrWeight);
        //        return decimal.Round(Convert.ToDecimal(sRate), 2, MidpointRounding.AwayFromZero);
        //    }
        //    else
        //    {
        //        return 0;
        //    }


        //    #endregion

        //}

        #endregion

        // private decimal getGSSRate(string GrWeight) // TransItem config id
        private decimal getGSSRate(string GrWeight, string sTransItemConfigID) // TransItem config id
        {
            decimal dConvertedToFixingQty = 0m;
            // fixing config ratio

            //transaction config ratio

            // conversion Gold qty(transconfigid,bool istranstofixing)



            string sRate = string.Empty;
            #region GSS
            //decimal dActualSaleQty = 0;
            //decimal dGSSMaturityQty = //Convert.ToDecimal(retailTrans.PartnerData.GSSTotQty);

            // if(!string.IsNullOrEmpty(txtQuantity.Text))
            if(!string.IsNullOrEmpty(GrWeight))
            {
                dConvertedToFixingQty = decimal.Round(getConvertedGoldQty(sTransItemConfigID, Convert.ToDecimal(GrWeight), true), 3, MidpointRounding.AwayFromZero);

                if(dPrevRunningQtyGSS == 0)
                {
                    //  dActualGSSSaleWt = dPrevGSSSaleWt + Convert.ToDecimal(GrWeight);
                    dActualGSSSaleWt = dPrevGSSSaleWt + dConvertedToFixingQty;

                }
                else
                {
                    // dActualGSSSaleWt = dPrevGSSSaleWt + Convert.ToDecimal(GrWeight) + dPrevRunningQtyGSS;
                    dActualGSSSaleWt = dPrevGSSSaleWt + dConvertedToFixingQty + dPrevRunningQtyGSS;
                }
            }

            if(dActualGSSSaleWt <= dGSSMaturityQty)
            {
                //  sRate = Convert.ToString(dGSSAvgRate);
                // decimal dPureVal = dActualGSSSaleWt * dGSSAvgRate;
                decimal dPureVal = dConvertedToFixingQty * dGSSAvgRate;
                sRate = Convert.ToString(decimal.Round(dPureVal / Convert.ToDecimal(GrWeight), 2, MidpointRounding.AwayFromZero));
            }
            else  //if (dActualGSSSaleWt > dGSSMaturityQty)
            {
                if(dPrevGSSSaleWt >= dGSSMaturityQty)
                {
                    sRate = getRateFromMetalTable();
                }
                else
                {
                    //  if ((dPrevGSSSaleWt + Convert.ToDecimal(GrWeight)+ dPrevRunningQtyGSS ) == dGSSMaturityQty)
                    if(dPrevGSSSaleWt + dPrevRunningQtyGSS >= dGSSMaturityQty)
                    {
                        sRate = getRateFromMetalTable();
                    }
                    else
                    {
                        decimal dAvgRateQty = 0m;
                        decimal dCurrentRateQty = 0m;
                        decimal dCurrentRateConvertedtoTransQty = 0m;
                        decimal dCurrentRate = 0m;

                        // dCurrentRateQty = (dActualGSSSaleWt - dGSSMaturityQty - dPrevRunningQtyGSS);

                        dCurrentRateQty = (dActualGSSSaleWt - dGSSMaturityQty);


                        dCurrentRateConvertedtoTransQty = decimal.Round(getConvertedGoldQty(sTransItemConfigID, dCurrentRateQty, false), 3, MidpointRounding.AwayFromZero);  // new


                        //  dAvgRateQty = Convert.ToDecimal(GrWeight) - dCurrentRateQty;

                        dAvgRateQty = dConvertedToFixingQty - dCurrentRateQty;


                        dCurrentRate = getMetalRate(ItemID, ConfigID);

                        if(dGSSAvgRate > 0 && dCurrentRate > 0)
                        {
                            // sRate = Convert.ToString(((dGSSAvgRate * dAvgRateQty) + (dCurrentRateQty * dCurrentRate)) / Convert.ToDecimal(GrWeight));
                            sRate = Convert.ToString(decimal.Round(((dGSSAvgRate * dAvgRateQty) + (dCurrentRateConvertedtoTransQty * dCurrentRate)) / Convert.ToDecimal(GrWeight), 2, MidpointRounding.AwayFromZero));
                        }
                    }
                }

            }

            if(!string.IsNullOrEmpty(sRate))
            {
                //  dPrevRunningQtyGSS += Convert.ToDecimal(GrWeight); 
                dPrevRunningQtyGSS += dConvertedToFixingQty;
                return decimal.Round(Convert.ToDecimal(sRate), 2, MidpointRounding.AwayFromZero);
            }
            else
            {
                return 0;
            }

            #endregion

        }
        #endregion

        private decimal getConvertedGoldQty(string sTransConfigid, decimal dQtyToConvert, bool istranstofixing)
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION NVARCHAR(20)  DECLARE @FIXINGCONFIGID NVARCHAR(20)  DECLARE @TRANSCONFIGID NVARCHAR(20)  DECLARE @istranstofixing NVARCHAR(5) ");
            commandText.Append(" DECLARE @QTY NUMERIC(32,10)  DECLARE @FIXINGCONFIGRATIO NUMERIC(32,16)  DECLARE @TRANSCONFIGRATIO NUMERIC(32,16) ");
            commandText.Append("SET @TRANSCONFIGID = '" + sTransConfigid + "'");
            commandText.Append("SET @QTY = ");
            commandText.Append(dQtyToConvert);
            if(istranstofixing)
                commandText.Append("SET @istranstofixing = 'Y'");
            else
                commandText.Append("SET @istranstofixing = 'N'");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM  RETAILCHANNELTABLE INNER JOIN ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + StoreID.Trim() + "'  ");
            commandText.Append(" SELECT @FIXINGCONFIGID = DefaultConfigIdGold FROM INVENTPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");
            commandText.Append(" SELECT @FIXINGCONFIGRATIO = CONFRATIO FROM CONFIGRATIO WHERE INVENTLOCATIONID = @INVENTLOCATION AND METALTYPE = 1 AND CONFIGIDSTANDARD = @FIXINGCONFIGID AND  DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");
            commandText.Append(" SELECT @TRANSCONFIGRATIO = CONFRATIO FROM CONFIGRATIO WHERE INVENTLOCATIONID = @INVENTLOCATION AND METALTYPE = 1 AND CONFIGIDSTANDARD = @TRANSCONFIGID AND  DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");
            commandText.Append(" IF @istranstofixing = 'Y' BEGIN  IF(@FIXINGCONFIGRATIO > 0 AND @TRANSCONFIGRATIO > 0) BEGIN  SELECT ISNULL(((@FIXINGCONFIGRATIO * @QTY) / @TRANSCONFIGRATIO),0) AS CONVERTEDQTY   END ");
            commandText.Append(" ELSE BEGIN SELECT @QTY AS CONVERTEDQTY END END");
            commandText.Append(" ELSE BEGIN IF(@FIXINGCONFIGRATIO > 0 AND @TRANSCONFIGRATIO > 0) BEGIN SELECT ISNULL(((@TRANSCONFIGRATIO * @QTY) / @FIXINGCONFIGRATIO),0) AS CONVERTEDQTY   END ");
            commandText.Append(" ELSE BEGIN SELECT @QTY AS CONVERTEDQTY END END");

            if(conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            decimal dResult = Convert.ToDecimal(command.ExecuteScalar());

            if(conn.State == ConnectionState.Open)
                conn.Close();

            return dResult;

        }

        #region  // Avg Gold Rate Adjustment  calc

        private decimal getAdjustmentRate(string GrWeight, string sTransItemConfigID)
        {
            decimal dConvertedToFixingQty = 0m;
            string sRate = string.Empty;

            #region AdjustmentRate
            //decimal dActualSaleQty = 0;
            //decimal dGSSMaturityQty = //Convert.ToDecimal(retailTrans.PartnerData.GSSTotQty);

            // if(!string.IsNullOrEmpty(txtQuantity.Text))
            if(!string.IsNullOrEmpty(GrWeight))
            {
                dConvertedToFixingQty = decimal.Round(getConvertedGoldQty(sTransItemConfigID, Convert.ToDecimal(GrWeight), true), 3, MidpointRounding.AwayFromZero);

                if(dPrevRunningTransAdjGoldQty == 0)
                {
                    // dActualTransAdjGoldQty = dPrevTransAdjGoldQty + Convert.ToDecimal(GrWeight);
                    dActualTransAdjGoldQty = dPrevTransAdjGoldQty + dConvertedToFixingQty;

                }
                else
                {
                    // dActualTransAdjGoldQty = dPrevTransAdjGoldQty + Convert.ToDecimal(GrWeight) + dPrevRunningTransAdjGoldQty;
                    dActualTransAdjGoldQty = dPrevTransAdjGoldQty + dConvertedToFixingQty + dPrevRunningTransAdjGoldQty;
                }
            }

            if(dActualTransAdjGoldQty <= dSaleAdjustmentGoldQty)
            {
                //sRate = Convert.ToString(dSaleAdjustmentAvgGoldRate);
                //  decimal dPureVal = dActualTransAdjGoldQty * dSaleAdjustmentAvgGoldRate;
                decimal dPureVal = dConvertedToFixingQty * dSaleAdjustmentAvgGoldRate;
                sRate = Convert.ToString(decimal.Round(dPureVal / Convert.ToDecimal(GrWeight), 2, MidpointRounding.AwayFromZero));
            }
            else //if (dActualTransAdjGoldQty > dSaleAdjustmentGoldQty)
            {
                if(dPrevTransAdjGoldQty >= dSaleAdjustmentGoldQty)
                {
                    sRate = getRateFromMetalTable();
                }
                else
                {
                    //  if ((dPrevTransAdjGoldQty + Convert.ToDecimal(GrWeight)+ dPrevRunningQtyGSS ) == dGSSMaturityQty)
                    if(dPrevTransAdjGoldQty + dPrevRunningTransAdjGoldQty >= dSaleAdjustmentGoldQty)
                    {
                        sRate = getRateFromMetalTable();
                    }
                    else
                    {
                        decimal dAvgRateQty = 0m;
                        decimal dCurrentRateQty = 0m;
                        decimal dCurrentRateConvertedtoTransQty = 0m;
                        decimal dCurrentRate = 0m;

                        // dCurrentRateQty = (dActualTransAdjGoldQty - dSaleAdjustmentGoldQty - dPrevRunningTransAdjGoldQty);
                        dCurrentRateQty = (dActualTransAdjGoldQty - dSaleAdjustmentGoldQty);
                        dCurrentRateConvertedtoTransQty = decimal.Round(getConvertedGoldQty(sTransItemConfigID, dCurrentRateQty, false), 3, MidpointRounding.AwayFromZero);  // new

                        // dAvgRateQty = Convert.ToDecimal(GrWeight) - dCurrentRateQty;

                        dAvgRateQty = dConvertedToFixingQty - dCurrentRateQty;

                        dCurrentRate = getMetalRate(ItemID, ConfigID);

                        if(dSaleAdjustmentAvgGoldRate > 0 && dCurrentRate > 0)
                        {
                            //sRate = Convert.ToString(((dSaleAdjustmentAvgGoldRate * dAvgRateQty) + (dCurrentRateQty * dCurrentRate)) / Convert.ToDecimal(GrWeight));

                            sRate = Convert.ToString(decimal.Round(((dSaleAdjustmentAvgGoldRate * dAvgRateQty) + (dCurrentRateConvertedtoTransQty * dCurrentRate)) / Convert.ToDecimal(GrWeight), 2, MidpointRounding.AwayFromZero));
                        }
                    }
                }

            }

            if(!string.IsNullOrEmpty(sRate))
            {
                // dPrevRunningTransAdjGoldQty += Convert.ToDecimal(GrWeight);
                dPrevRunningTransAdjGoldQty += dConvertedToFixingQty;
                return decimal.Round(Convert.ToDecimal(sRate), 3, MidpointRounding.AwayFromZero);
            }
            else
            {
                return 0;
            }


            #endregion

        }
        #endregion


        #region // Making Discount Type

        private void CheckMakingDiscountFromDB(decimal dDiscValue)
        {
            string sWtRange = string.Empty;
            decimal dDiscAmt = 0m;
            dDiscAmt = dDiscValue;
            if(cmbMakingDiscType.SelectedIndex == 0)
                txtMakingDisc.Text = Convert.ToString(dDiscAmt);
            if(dDiscAmt >= 0)
            {
                dMakingDiscDbAmt = dDiscAmt;
                CalcMakingDiscount(dDiscAmt);
            }

        }
        private int getUserDiscountPermissionId()
        {
            SqlConnection connection = new SqlConnection();
            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("select RETAILDISCPERMISSION from RETAILSTAFFTABLE where STAFFID='" + ApplicationSettings.Terminal.TerminalOperator.OperatorId + "'");

            if(connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToInt16(cmd.ExecuteScalar());
        }
        private decimal GetMkDiscountFromDiscPolicy(string sItemId, decimal dItemMainValue, string sWhichFieldValueWillGet)
        {
            decimal dResult = 0;


            SqlConnection connection = new SqlConnection();
            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if(connection.State == ConnectionState.Closed)
                connection.Open();


            string sFlatQry = "";
            if(sWhichFieldValueWillGet == "OPENINGDISCPCT")
                sFlatQry = " AND CRWMAKINGDISCOUNT.FLAT =1";
            else
                sFlatQry = "";

            string commandText = "  DECLARE @LOCATION VARCHAR(20) " +
                                 "  DECLARE @PRODUCT BIGINT " +
                                 "  DECLARE @PARENTITEM VARCHAR(20) " +
                                 "  DECLARE @CUSTCODE VARCHAR(20) " +
                                 "  IF EXISTS(SELECT TOP(1) ACTIVATE FROM CRWMAKINGDISCOUNT )" + // WHERE RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "'
                                 "  IF EXISTS(SELECT TOP(1) ITEMID FROM  INVENTTABLE WHERE ITEMID='" + sItemId + "' AND RETAIL=1)" +
                                 "  BEGIN SELECT @PARENTITEM = ITEMIDPARENT FROM [INVENTTABLE] WHERE ITEMID = '" + sItemId + "' " +
                                 " END " +
                                 " ELSE" +
                                 " BEGIN" +
                                 " SET @PARENTITEM='" + sItemId + "' " +
                                 " END" +
                                 "  SELECT @CUSTCODE = CUSTCLASSIFICATIONID FROM [CUSTTABLE] WHERE ACCOUNTNUM = '" + sCustomerId + "'";
            #region
            //commandText += " SELECT  TOP (1) CAST(CRWMAKINGDISCOUNT." + sWhichFieldValueWillGet.Trim() + " AS decimal(18, 2)),ISNULL(PROMOTIONCODE,'') AS PROMOTIONCODE,ISNULL(FLAT,0) as FLAT " +
            //                " FROM   CRWMAKINGDISCOUNT  " +
            //                " WHERE    " +
            //                " (CRWMAKINGDISCOUNT.ITEMID=@PARENTITEM  or CRWMAKINGDISCOUNT.ITEMID='')" +
            //                " AND (CRWMAKINGDISCOUNT.CODE=@CUSTCODE  or CRWMAKINGDISCOUNT.CODE='')" +
            //    //" AND (CRWMAKINGDISCOUNT.ITEMID=@PARENTITEM  OR CRWMAKINGDISCOUNT.ITEMID='')" +
            //                " AND (CRWMAKINGDISCOUNT.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "' or CRWMAKINGDISCOUNT.RETAILSTOREID='')" +
            //    //" AND  (CRWMAKINGDISCOUNT.CATEGORY=(SELECT CATEGORY FROM [ECORESPRODUCTCATEGORY] WHERE PRODUCT = @PRODUCT) " +
            //    //" OR CRWMAKINGDISCOUNT.CATEGORY=0) AND " +
            //                " and ('" + dItemMainValue + "' BETWEEN CRWMAKINGDISCOUNT.FROMWT AND CRWMAKINGDISCOUNT.TOWT)  " +
            //                " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWMAKINGDISCOUNT.FROMDATE AND CRWMAKINGDISCOUNT.TODATE)  " +
            //                " AND CRWMAKINGDISCOUNT.ACTIVATE = 1 " +
            //                " " + sFlatQry + "" +
            //                " ORDER BY CRWMAKINGDISCOUNT.ITEMID DESC,CRWMAKINGDISCOUNT.CODE DESC";
            //if (string.IsNullOrEmpty(sFlatQry))
            //    commandText += ", CRWMAKINGDISCOUNT.FLAT ASC";
            //else
            //    commandText += ", CRWMAKINGDISCOUNT.FLAT DESC";
            #endregion
            commandText += " IF EXISTS ( SELECT  TOP (1) RETAILSTOREID" + //1
                " FROM   CRWMAKINGDISCOUNT  " +
                " WHERE (CRWMAKINGDISCOUNT.ITEMID=@PARENTITEM) " +
                " AND (CRWMAKINGDISCOUNT.CODE=@CUSTCODE) " +
                " AND (CRWMAKINGDISCOUNT.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "')" +
                " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWMAKINGDISCOUNT.FROMDATE AND CRWMAKINGDISCOUNT.TODATE))" +
                     " BEGIN " +
                     "     SELECT  TOP (1) CAST(CRWMAKINGDISCOUNT." + sWhichFieldValueWillGet.Trim() + " AS decimal(18, 2))" +
                     "     ,ISNULL(PROMOTIONCODE,'') AS PROMOTIONCODE,ISNULL(FLAT,0) as FLAT  FROM   CRWMAKINGDISCOUNT   " +
                     "     WHERE     " +
                     "     (CRWMAKINGDISCOUNT.ITEMID=@PARENTITEM ) AND (CRWMAKINGDISCOUNT.CODE=@CUSTCODE ) " +
                     "     AND (CRWMAKINGDISCOUNT.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "')" +
                     "      and ('" + dItemMainValue + "'  BETWEEN CRWMAKINGDISCOUNT.FROMWT AND CRWMAKINGDISCOUNT.TOWT)  " +
                     "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWMAKINGDISCOUNT.FROMDATE AND CRWMAKINGDISCOUNT.TODATE)   " +
                     "       AND CRWMAKINGDISCOUNT.ACTIVATE = 1" +
                     " " + sFlatQry + "" +
                     " ORDER BY CRWMAKINGDISCOUNT.ITEMID DESC,CRWMAKINGDISCOUNT.CODE DESC ";
            if(string.IsNullOrEmpty(sFlatQry))
                commandText += ", CRWMAKINGDISCOUNT.FLAT ASC END";
            else
                commandText += ", CRWMAKINGDISCOUNT.FLAT DESC END";

            commandText += " IF EXISTS ( SELECT  TOP (1) RETAILSTOREID" + //2
               " FROM   CRWMAKINGDISCOUNT  " +
               " WHERE (CRWMAKINGDISCOUNT.ITEMID=@PARENTITEM) " +
               " AND (CRWMAKINGDISCOUNT.CODE='') " +
               " AND (CRWMAKINGDISCOUNT.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "')" +
               " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWMAKINGDISCOUNT.FROMDATE AND CRWMAKINGDISCOUNT.TODATE))" +
                    " BEGIN " +
                    "     SELECT  TOP (1) CAST(CRWMAKINGDISCOUNT." + sWhichFieldValueWillGet.Trim() + " AS decimal(18, 2))" +
                    "     ,ISNULL(PROMOTIONCODE,'') AS PROMOTIONCODE,ISNULL(FLAT,0) as FLAT  FROM   CRWMAKINGDISCOUNT   " +
                    "     WHERE     " +
                    "     (CRWMAKINGDISCOUNT.ITEMID=@PARENTITEM ) AND (CRWMAKINGDISCOUNT.CODE='' ) " +
                    "     AND (CRWMAKINGDISCOUNT.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "')" +
                    "      and ('" + dItemMainValue + "'  BETWEEN CRWMAKINGDISCOUNT.FROMWT AND CRWMAKINGDISCOUNT.TOWT)  " +
                    "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWMAKINGDISCOUNT.FROMDATE AND CRWMAKINGDISCOUNT.TODATE)   " +
                    "       AND CRWMAKINGDISCOUNT.ACTIVATE = 1" +
                    " " + sFlatQry + "" +
                    " ORDER BY CRWMAKINGDISCOUNT.ITEMID DESC,CRWMAKINGDISCOUNT.CODE DESC ";
            if(string.IsNullOrEmpty(sFlatQry))
                commandText += ", CRWMAKINGDISCOUNT.FLAT ASC END";
            else
                commandText += ", CRWMAKINGDISCOUNT.FLAT DESC END";

            commandText += " IF EXISTS ( SELECT  TOP (1) RETAILSTOREID" + //3
                " FROM   CRWMAKINGDISCOUNT  " +
                " WHERE (CRWMAKINGDISCOUNT.ITEMID='') " +
                " AND (CRWMAKINGDISCOUNT.CODE=@CUSTCODE) " +
                " AND (CRWMAKINGDISCOUNT.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "')" +
                " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWMAKINGDISCOUNT.FROMDATE AND CRWMAKINGDISCOUNT.TODATE))" +
                     " BEGIN " +
                     "     SELECT  TOP (1) CAST(CRWMAKINGDISCOUNT." + sWhichFieldValueWillGet.Trim() + " AS decimal(18, 2))" +
                     "     ,ISNULL(PROMOTIONCODE,'') AS PROMOTIONCODE,ISNULL(FLAT,0) as FLAT  FROM   CRWMAKINGDISCOUNT   " +
                     "     WHERE     " +
                     "     (CRWMAKINGDISCOUNT.ITEMID='' ) AND (CRWMAKINGDISCOUNT.CODE=@CUSTCODE ) " +
                     "     AND (CRWMAKINGDISCOUNT.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "')" +
                     "      and ('" + dItemMainValue + "'  BETWEEN CRWMAKINGDISCOUNT.FROMWT AND CRWMAKINGDISCOUNT.TOWT)  " +
                     "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWMAKINGDISCOUNT.FROMDATE AND CRWMAKINGDISCOUNT.TODATE)   " +
                     "       AND CRWMAKINGDISCOUNT.ACTIVATE = 1" +
                     " " + sFlatQry + "" +
                     " ORDER BY CRWMAKINGDISCOUNT.ITEMID DESC,CRWMAKINGDISCOUNT.CODE DESC ";
            if(string.IsNullOrEmpty(sFlatQry))
                commandText += ", CRWMAKINGDISCOUNT.FLAT ASC END";
            else
                commandText += ", CRWMAKINGDISCOUNT.FLAT DESC END";

            commandText += " IF EXISTS ( SELECT  TOP (1) RETAILSTOREID" + //4
               " FROM   CRWMAKINGDISCOUNT  " +
               " WHERE (CRWMAKINGDISCOUNT.ITEMID='') " +
               " AND (CRWMAKINGDISCOUNT.CODE='') " +
               " AND (CRWMAKINGDISCOUNT.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "')" +
               " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWMAKINGDISCOUNT.FROMDATE AND CRWMAKINGDISCOUNT.TODATE))" +
                    " BEGIN " +
                    "     SELECT  TOP (1) CAST(CRWMAKINGDISCOUNT." + sWhichFieldValueWillGet.Trim() + " AS decimal(18, 2))" +
                    "     ,ISNULL(PROMOTIONCODE,'') AS PROMOTIONCODE,ISNULL(FLAT,0) as FLAT  FROM   CRWMAKINGDISCOUNT   " +
                    "     WHERE     " +
                    "     (CRWMAKINGDISCOUNT.ITEMID='' ) AND (CRWMAKINGDISCOUNT.CODE='' ) " +
                    "     AND (CRWMAKINGDISCOUNT.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "')" +
                    "      and ('" + dItemMainValue + "'  BETWEEN CRWMAKINGDISCOUNT.FROMWT AND CRWMAKINGDISCOUNT.TOWT)  " +
                    "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWMAKINGDISCOUNT.FROMDATE AND CRWMAKINGDISCOUNT.TODATE)   " +
                    "       AND CRWMAKINGDISCOUNT.ACTIVATE = 1" +
                    " " + sFlatQry + "" +
                    " ORDER BY CRWMAKINGDISCOUNT.ITEMID DESC,CRWMAKINGDISCOUNT.CODE DESC ";
            if(string.IsNullOrEmpty(sFlatQry))
                commandText += ", CRWMAKINGDISCOUNT.FLAT ASC END";
            else
                commandText += ", CRWMAKINGDISCOUNT.FLAT DESC END";

            SqlCommand commandMk = new SqlCommand(commandText.ToString(), connection);
            commandMk.CommandTimeout = 0;
            //SqlDataReader mkDiscRd = commandMk.ExecuteReader();

            using(SqlDataReader mkDiscRd = commandMk.ExecuteReader())
            {
                while(mkDiscRd.Read())
                {
                    dResult = Convert.ToDecimal(mkDiscRd.GetValue(0));
                    sMkPromoCode = Convert.ToString(mkDiscRd.GetValue(1));
                    iIsMkDiscFlat = Convert.ToInt16(mkDiscRd.GetValue(2));
                }
            }


            if(connection.State == ConnectionState.Open)
                connection.Close();


            return dResult;
        }
        private void CalcMakingDiscount(decimal dMkDiscAmt)
        {
            #region Making Discount Calculation

            if(cmbMakingDiscType.SelectedIndex == 0)   // Percentage  dDiscAmt
            {
                if(!string.IsNullOrEmpty(txtMakingAmount.Text))
                {
                    //if (Convert.ToDecimal(txtMakingAmount.Text) > 0)// commented on 29/05/2014 by S.Sharma
                    //{
                    decimal dTotMkDiscAmt = 0m;
                    decimal dWastageAmt = 0m;
                    if(string.IsNullOrEmpty(txtWastageAmount.Text))//Added txtWastageAmount on 29/05/2014 by S.Sharma
                        dWastageAmt = 0;
                    else
                        dWastageAmt = Convert.ToDecimal(txtWastageAmount.Text.Trim());
                    // dTotMkAmt = ((Convert.ToDecimal(txtMakingDisc.Text.Trim()) / 100) * (Convert.ToDecimal(txtMakingAmount.Text.Trim())));

                    dTotMkDiscAmt = ((dMkDiscAmt / 100) * ((Convert.ToDecimal(txtMakingAmount.Text.Trim())) + dWastageAmt));//Added txtWastageAmount on 29/05/2014 by S.Sharma
                    txtMakingDiscTotAmt.Text = Convert.ToString(decimal.Round(dTotMkDiscAmt, 2, MidpointRounding.AwayFromZero));
                    //}
                }
            }
            #endregion
        }
        #endregion

        #region [ // Stone Discount Calculation]

        private decimal CalcStoneDiscount(decimal dStnDiscAmt, int iStnDiscType, decimal dStnDiscWt, decimal dStnRate)
        {
            decimal dStnDiscTotAmt = 0m;

            if(iStnDiscType == 0) // Percentage
            {
                dStnDiscTotAmt = (dStnDiscAmt / 100) * (dStnDiscWt * dStnRate);
            }
            else if(iStnDiscType == 1) // Weight
            {
                dStnDiscTotAmt = dStnDiscAmt * dStnDiscWt;
            }
            else if(iStnDiscType == 2)
            {
                dStnDiscTotAmt = dStnDiscAmt;
            }

            return dStnDiscTotAmt;
        }

        #endregion

        #region // Fixed Metal Rate New
        private void BindIngredientGrid(string sCustOrdrFixedMetalRate, string sFixedRateConfigId)
        {
            if(rdbSale.Checked)
            {
                dtIngredients = new DataTable();

                if(conn.State == ConnectionState.Closed)
                    conn.Open();

                StringBuilder commandText = new StringBuilder();
                commandText.Append("  IF EXISTS(SELECT TOP(1) [SkuNumber] FROM [SKULine_Posted] WHERE  [SkuNumber] = '" + ItemID.Trim() + "')");
                commandText.Append("  BEGIN  ");
                commandText.Append(" SELECT     SKULine_Posted.SkuNumber, SKULine_Posted.SkuDate, SKULine_Posted.ItemID, INVENTDIM.InventDimID, INVENTDIM.InventSizeID,  ");
                // commandText.Append(" INVENTDIM.InventColorID, INVENTDIM.ConfigID, INVENTDIM.InventBatchID, CAST(SKULine_Posted.PDSCWQTY AS INT) AS PCS, CAST(SKULine_Posted.QTY AS NUMERIC(16,3)) AS QTY,  ");
                commandText.Append(" INVENTDIM.InventColorID, INVENTDIM.ConfigID, INVENTDIM.InventBatchID, CAST(ISNULL(SKULine_Posted.PDSCWQTY,0) AS INT) AS PCS, CAST(ISNULL(SKULine_Posted.QTY,0) AS NUMERIC(16,3)) AS QTY,  ");
                commandText.Append(" SKULine_Posted.CValue, SKULine_Posted.CRate AS Rate, SKULine_Posted.UnitID,X.METALTYPE");
                //  commandText.Append(" ,0 AS IngrdDiscType,0 AS IngrdDiscAmt,0 AS IngrdDiscTotAmt "); 
                commandText.Append(" FROM         SKULine_Posted INNER JOIN ");
                commandText.Append(" INVENTDIM ON SKULine_Posted.InventDimID = INVENTDIM.INVENTDIMID ");
                commandText.Append(" INNER JOIN INVENTTABLE X ON SKULine_Posted.ITEMID = X.ITEMID ");
                commandText.Append(" WHERE INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "' ");
                commandText.Append("  AND  [SkuNumber] = '" + ItemID.Trim() + "' ORDER BY X.METALTYPE END ");


                SqlCommand command = new SqlCommand(commandText.ToString(), conn);
                command.CommandTimeout = 0;
                using(SqlDataReader reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        dtIngredients.Load(reader);
                    }
                }


                if(dtIngredients != null && dtIngredients.Rows.Count > 0)
                {
                    #region // Stone Discount

                    dtIngredients.Columns.Add("IngrdDiscType", typeof(int));
                    dtIngredients.Columns.Add("IngrdDiscAmt", typeof(decimal));
                    dtIngredients.Columns.Add("IngrdDiscTotAmt", typeof(decimal));
                    dtIngredients.Columns.Add("CTYPE", typeof(int));

                    #endregion

                    txtRate.Text = string.Empty;
                    dtIngredientsClone = new DataTable();
                    dtIngredientsClone = dtIngredients.Clone();
                    // dtIngredientsClone.Columns["LType"].DataType = typeof(string);
                    //   dtIngredientsClone.Columns["CType"].DataType = typeof(string);

                    foreach(DataRow drClone in dtIngredients.Rows)
                    {
                        if(isMRPUCP)
                        {
                            drClone["CValue"] = 0;
                            drClone["Rate"] = 0;
                            drClone["CTYPE"] = 0;
                            drClone["IngrdDiscType"] = 0;
                            drClone["IngrdDiscAmt"] = 0;
                            drClone["IngrdDiscTotAmt"] = 0;
                        }
                        dtIngredientsClone.ImportRow(drClone);
                    }

                    if(!isMRPUCP)
                    {
                        foreach(DataRow dr in dtIngredientsClone.Rows)
                        {
                            string item = ItemID;
                            string config = ConfigID;
                            ConfigID = string.Empty;
                            ItemID = string.Empty;
                            ConfigID = Convert.ToString(dr["ConfigID"]);
                            ItemID = Convert.ToString(dr["ItemID"]);
                            BatchID = Convert.ToString(dr["InventBatchID"]);
                            ColorID = Convert.ToString(dr["InventColorID"]);
                            SizeID = Convert.ToString(dr["InventSizeID"]);
                            GrWeight = Convert.ToString(dr["QTY"]);
                            string sRate = string.Empty;
                            string sCalcType = "";

                            if(Convert.ToDecimal(dr["PCS"]) > 0)
                                dStoneWtRange = decimal.Round((Convert.ToDecimal(dr["QTY"]) / Convert.ToDecimal(dr["PCS"])), 3, MidpointRounding.AwayFromZero);//Changes on 11/04/2014 R.Hossain
                            else
                                dStoneWtRange = decimal.Round((Convert.ToDecimal(dr["QTY"])), 3, MidpointRounding.AwayFromZero); //Changes on 11/04/2014 R.Hossain

                            // Stone Discount
                            int iStoneDiscType = 0;
                            decimal dStoneDiscAmt = 0;
                            decimal dStoneDiscTotAmt = 0m;


                            if(Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Gold)
                            {
                                //sRate = sCustOrdrFixedMetalRate;  // ConfigID

                                decimal dFixedRateConfigRatio = 0m;
                                decimal dTransConfigRatio = 0m;
                                decimal dTransIncrSalePct = 0m;
                                decimal dTransIncrSaleAmt = 0m;
                                decimal dActualMetalRate = 0m;

                                dFixedRateConfigRatio = GetConfigRatioInfo(sFixedRateConfigId,
                                                                           ConfigID, ref dTransConfigRatio, ref dTransIncrSalePct);

                                if(dTransIncrSalePct > 0)
                                {
                                    dTransIncrSaleAmt = Convert.ToDecimal(sCustOrdrFixedMetalRate) * (dTransIncrSalePct * 0.01m);
                                }

                                // calculate metal rate based on configuration
                                if(Convert.ToInt32(sFixedRateConfigId.Substring(0, 2)) == Convert.ToInt32(ConfigID.Substring(0, 2)))
                                {
                                    sRate = sCustOrdrFixedMetalRate;
                                }
                                else if(Convert.ToInt32(sFixedRateConfigId.Substring(0, 2)) > Convert.ToInt32(ConfigID.Substring(0, 2)))
                                {
                                    dActualMetalRate = (Convert.ToDecimal(sCustOrdrFixedMetalRate) + dTransIncrSaleAmt) * (dFixedRateConfigRatio / dTransConfigRatio);
                                    sRate = Convert.ToString(decimal.Round(dActualMetalRate, MidpointRounding.AwayFromZero));
                                }
                                else
                                {
                                    dActualMetalRate = (Convert.ToDecimal(sCustOrdrFixedMetalRate) + dTransIncrSaleAmt) * (dTransConfigRatio / dFixedRateConfigRatio);
                                    sRate = Convert.ToString(decimal.Round(dActualMetalRate, MidpointRounding.AwayFromZero));
                                }

                                // -- end
                                dCustomerOrderFixedRate = Convert.ToDecimal(sRate);
                            }
                            else
                            {
                                sRate = getRateFromMetalTable();
                            }

                            // ------*********

                            if(!string.IsNullOrEmpty(sRate))
                            {
                                sCalcType = GetIngredientCalcType(ref iStoneDiscType, ref dStoneDiscAmt);

                                if(!string.IsNullOrEmpty(sCalcType))
                                    dr["CTYPE"] = Convert.ToInt32(sCalcType);
                                else
                                    dr["CTYPE"] = 0;


                                dr["Rate"] = sRate;
                                ItemID = item;
                                ConfigID = config;
                                BatchID = string.Empty;
                                ColorID = string.Empty;
                                SizeID = string.Empty;
                                GrWeight = string.Empty;
                            }
                            else
                            {
                                dr["Rate"] = "0"; // Added on 08.08.2013 -- Instructed by Urminavo Das 
                                // if not rate found make it 0 -- validation related issues will be raised by RGJL in CR
                                dr["CTYPE"] = 0;

                                ItemID = item;
                                ConfigID = config;
                                BatchID = string.Empty;
                                ColorID = string.Empty;
                                SizeID = string.Empty;
                                GrWeight = string.Empty;
                            }


                            StringBuilder commandText1 = new StringBuilder();
                            commandText1.Append("select metaltype from inventtable where itemid='" + Convert.ToString(dr["ItemID"]) + "'");

                            if(conn.State == ConnectionState.Closed)
                                conn.Open();

                            SqlCommand command1 = new SqlCommand(commandText1.ToString(), conn);
                            command1.CommandTimeout = 0;

                            using(SqlDataReader reader1 = command1.ExecuteReader())
                            {
                                if(reader1.HasRows)
                                {
                                    #region Reader
                                    while(reader1.Read())
                                    {
                                        //---- add code --  for CalcType

                                        if(((int)reader1.GetValue(0) == (int)MetalType.LooseDmd) || ((int)reader1.GetValue(0) == (int)MetalType.Stone))
                                        {

                                            if(!string.IsNullOrEmpty(sCalcType))
                                            {
                                                #region // Stone Discount Calculation


                                                #endregion

                                                if(Convert.ToInt32(sCalcType) == Convert.ToInt32(RateType.Weight))
                                                {
                                                    if(dStoneDiscAmt > 0)
                                                    {
                                                        dStoneDiscTotAmt = CalcStoneDiscount(dStoneDiscAmt, iStoneDiscType, Convert.ToDecimal(dr["QTY"]), Convert.ToDecimal(dr["Rate"]));
                                                    }
                                                    // dr["CValue"] = decimal.Round(Convert.ToDecimal(dr["QTY"]) * Convert.ToDecimal(dr["Rate"]), 2, MidpointRounding.AwayFromZero);
                                                    dr["CValue"] = decimal.Round((Convert.ToDecimal(dr["QTY"]) * Convert.ToDecimal(dr["Rate"])) - dStoneDiscTotAmt, 2, MidpointRounding.AwayFromZero);
                                                }

                                                else if(Convert.ToInt32(sCalcType) == Convert.ToInt32(RateType.Pieces))
                                                {
                                                    if(dStoneDiscAmt > 0)
                                                    {
                                                        dStoneDiscTotAmt = CalcStoneDiscount(dStoneDiscAmt, iStoneDiscType, Convert.ToDecimal(dr["PCS"]), Convert.ToDecimal(dr["Rate"]));
                                                    }
                                                    // dr["CValue"] = decimal.Round(Convert.ToDecimal(dr["PCS"]) * Convert.ToDecimal(dr["Rate"]), 2, MidpointRounding.AwayFromZero);
                                                    dr["CValue"] = decimal.Round((Convert.ToDecimal(dr["PCS"]) * Convert.ToDecimal(dr["Rate"])) - dStoneDiscTotAmt, 2, MidpointRounding.AwayFromZero);
                                                }

                                                else // Tot
                                                {
                                                    if(dStoneDiscAmt > 0)
                                                    {
                                                        dStoneDiscTotAmt = CalcStoneDiscount(dStoneDiscAmt, iStoneDiscType, Convert.ToDecimal(dr["QTY"]), Convert.ToDecimal(dr["Rate"]));
                                                    }
                                                    //dr["CValue"] = decimal.Round(Convert.ToDecimal(dr["Rate"]), 2, MidpointRounding.AwayFromZero);
                                                    dr["CValue"] = decimal.Round(Convert.ToDecimal(dr["Rate"]) - dStoneDiscTotAmt, 2, MidpointRounding.AwayFromZero);
                                                }
                                            }
                                            else
                                            {
                                                if(dStoneDiscAmt > 0)
                                                {
                                                    dStoneDiscTotAmt = CalcStoneDiscount(dStoneDiscAmt, iStoneDiscType, Convert.ToDecimal(dr["QTY"]), Convert.ToDecimal(dr["Rate"]));
                                                }
                                                // dr["CValue"] = decimal.Round(Convert.ToDecimal(dr["QTY"]) * Convert.ToDecimal(dr["Rate"]), 2, MidpointRounding.AwayFromZero);
                                                dr["CValue"] = decimal.Round((Convert.ToDecimal(dr["QTY"]) * Convert.ToDecimal(dr["Rate"])) - dStoneDiscTotAmt, 2, MidpointRounding.AwayFromZero);
                                            }
                                        }
                                        else
                                        {
                                            //if (dStoneDiscAmt > 0)
                                            //{
                                            //    dStoneDiscTotAmt = CalcStoneDiscount(dStoneDiscAmt, iStoneDiscType, Convert.ToDecimal(dr["QTY"]), Convert.ToDecimal(dr["Rate"]));
                                            //}

                                            dr["CValue"] = decimal.Round(Convert.ToDecimal(dr["QTY"]) * Convert.ToDecimal(dr["Rate"]), 2, MidpointRounding.AwayFromZero);
                                            // dr["CValue"] = decimal.Round((Convert.ToDecimal(dr["QTY"]) * Convert.ToDecimal(dr["Rate"])) - dStoneDiscTotAmt, 2, MidpointRounding.AwayFromZero);
                                        }

                                        // --- end

                                        if((int)reader1.GetValue(0) == (int)MetalType.Gold)
                                        {
                                            txtgval.Text = (string.IsNullOrEmpty(txtRate.Text)) ? Convert.ToString(dr["CValue"]) : Convert.ToString(decimal.Round(Convert.ToDecimal(txtgval.Text) + Convert.ToDecimal(dr["CValue"]), 2, MidpointRounding.AwayFromZero));
                                        }
                                    }
                                    #endregion
                                }
                            }



                            if(conn.State == ConnectionState.Open)
                                conn.Close();

                            if(dStoneDiscAmt > 0)
                            {
                                dr["IngrdDiscType"] = iStoneDiscType;
                                dr["IngrdDiscAmt"] = dStoneDiscAmt;
                                dr["IngrdDiscTotAmt"] = decimal.Round(dStoneDiscTotAmt, 2, MidpointRounding.AwayFromZero);

                            }
                            else
                            {
                                dr["IngrdDiscType"] = 0;
                                dr["IngrdDiscAmt"] = 0;
                                dr["IngrdDiscTotAmt"] = 0;
                            }

                            //

                            txtRate.Text = (string.IsNullOrEmpty(txtRate.Text)) ? Convert.ToString(dr["CValue"]) : Convert.ToString(decimal.Round(Convert.ToDecimal(txtRate.Text) + Convert.ToDecimal(dr["CValue"]), 2, MidpointRounding.AwayFromZero));

                        }

                        dtIngredientsClone.AcceptChanges();

                        cmbRateType.SelectedIndex = (int)RateType.Tot;
                        cmbRateType.Enabled = false;

                        // Added on 07.06.2013
                        txtPCS.Enabled = false;
                        txtQuantity.Enabled = false;
                        txtRate.Enabled = false;
                        txtMakingRate.Enabled = false;
                        txtMakingDisc.Enabled = false;
                        cmbMakingType.Enabled = false;
                        //
                    }
                    else
                    {
                        txtRate.Text = MRPUCP;
                    }
                    btnIngrdientsDetails.Visible = true;
                }
                else
                {
                    if(!isMRPUCP)

                        #region [ GSS Transaction]
                        if(IsGSSTransaction)
                        {
                            int iMetalType = GetMetalType(ItemID);
                            //getMetalRate(string sItemId, string sConfigId)
                            if(iMetalType == (int)MetalType.Gold)
                            {
                                if(!string.IsNullOrEmpty(txtQuantity.Text))
                                {
                                    decimal dMetalRate = 0;

                                    dMetalRate = getGSSRate(txtQuantity.Text.Trim(), ConfigID);
                                    if(dMetalRate > 0)
                                    {
                                        txtRate.Text = Convert.ToString(dMetalRate);
                                    }
                                    else
                                    {
                                        txtRate.Text = getRateFromMetalTable();
                                    }

                                }
                                else
                                {
                                    // Get Qty
                                    decimal dSKUQty = 0;

                                    dSKUQty = GetSKUQty(ItemID);

                                    if(dSKUQty > 0)
                                    {
                                        decimal dMetalRate = 0;

                                        dMetalRate = getGSSRate(Convert.ToString(dSKUQty), ConfigID);
                                        if(dMetalRate > 0)
                                        {
                                            txtRate.Text = Convert.ToString(dMetalRate);
                                        }
                                        else
                                        {
                                            txtRate.Text = getRateFromMetalTable();
                                        }
                                    }
                                    else
                                    {
                                        txtRate.Text = getRateFromMetalTable();
                                    }

                                }
                            }
                            else
                            {
                                txtRate.Text = getRateFromMetalTable();
                            }

                        }//
                        #endregion

                        #region [Sale Adjustment]
                        else if(IsSaleAdjustment)
                        {
                            int iMetalType = GetMetalType(ItemID);
                            //getMetalRate(string sItemId, string sConfigId)
                            if(iMetalType == (int)MetalType.Gold)
                            {
                                if(!string.IsNullOrEmpty(txtQuantity.Text))
                                {
                                    decimal dMetalRate = 0;

                                    dMetalRate = getAdjustmentRate(txtQuantity.Text.Trim(), ConfigID);
                                    if(dMetalRate > 0)
                                    {
                                        txtRate.Text = Convert.ToString(dMetalRate);
                                    }
                                    else
                                    {
                                        txtRate.Text = getRateFromMetalTable();
                                    }

                                }
                                else
                                {
                                    decimal dSKUQty = 0;

                                    dSKUQty = GetSKUQty(ItemID);

                                    if(dSKUQty > 0)
                                    {
                                        decimal dMetalRate = 0;

                                        dMetalRate = getAdjustmentRate(Convert.ToString(dSKUQty), ConfigID);
                                        if(dMetalRate > 0)
                                        {
                                            txtRate.Text = Convert.ToString(dMetalRate);
                                        }
                                        else
                                        {
                                            txtRate.Text = getRateFromMetalTable();
                                        }
                                    }
                                    else
                                    {
                                        txtRate.Text = getRateFromMetalTable();
                                    }

                                }
                            }
                            else
                            {
                                txtRate.Text = getRateFromMetalTable();
                            }

                        }//
                        #endregion

                        else
                        {
                            txtRate.Text = getRateFromMetalTable();
                        }

                    else
                        txtRate.Text = MRPUCP;
                    btnIngrdientsDetails.Visible = false;
                }


                if(conn.State == ConnectionState.Open)
                    conn.Close();


            }
        }
        private int GetCustOrderFixedRateInfo(string sOrderNo, ref decimal dFixedMetalRateVal, ref string sConfigId)  // 
        {
            int IsFixedMetalRate = 0;
            SqlConnection connection = new SqlConnection();

            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if(connection.State == ConnectionState.Closed)
                connection.Open();


            string commandText = " SELECT ISNULL(ISFIXEDMETALRATE,0), ISNULL(FIXEDMETALRATE,0),FIXEDMETALRATECONFIGID FROM CUSTORDER_HEADER WHERE ORDERNUM = '" + sOrderNo + "' ";

            SqlCommand command = new SqlCommand(commandText, connection);
            // string strCustOrder = Convert.ToString(command.ExecuteScalar());
            command.CommandTimeout = 0;

            using(SqlDataReader reader = command.ExecuteReader())
            {
                if(reader.HasRows)
                {
                    while(reader.Read())
                    {
                        IsFixedMetalRate = Convert.ToInt32(reader.GetValue(0));
                        dFixedMetalRateVal = Convert.ToDecimal(reader.GetValue(1));
                        sConfigId = Convert.ToString(reader.GetValue(2));
                    }
                }
            }

            if(connection.State == ConnectionState.Open)
                connection.Close();

            return IsFixedMetalRate;
        }
        private decimal GetConfigRatioInfo(string sFixedConfigId, string sTransConfigId, ref decimal dTransConfigRatio, ref decimal dTransIncrSalePct)  // // Fixed Metal Rate New
        {
            decimal dFixedRateConfigRatio = 0m;
            SqlConnection connection = new SqlConnection();

            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "DECLARE @INVENTLOCATION VARCHAR(20) " +
                                " DECLARE @FIXEDCONFRATIO NUMERIC(32,10)  SELECT @INVENTLOCATION = RETAILCHANNELTABLE.INVENTLOCATION" +
                                " FROM   RETAILCHANNELTABLE INNER JOIN RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID" +
                                " WHERE STORENUMBER = '" + ApplicationSettings.Database.StoreID + "'" +
                                " SELECT @FIXEDCONFRATIO = ISNULL(CONFRATIO,0) FROM CONFIGRATIO WHERE METALTYPE = 1 AND INVENTLOCATIONID = @INVENTLOCATION AND CONFIGIDSTANDARD = @FIXEDCONFIGID" +
                                " SELECT @FIXEDCONFRATIO AS FIXEDCONFRATIO,ISNULL(CONFRATIO,0) AS TRANSCONFRATIO ,ISNULL(INCRSALEPERCENT,0) AS TRANSINCRSALEPCT" +
                                " FROM CONFIGRATIO WHERE METALTYPE = 1 AND INVENTLOCATIONID = @INVENTLOCATION AND CONFIGIDSTANDARD = @TRANSCONFIGID";

            if(connection.State == ConnectionState.Closed)
                connection.Open();


            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            command.Parameters.Add("@FIXEDCONFIGID", SqlDbType.NVarChar, 10).Value = sFixedConfigId;
            command.Parameters.Add("@TRANSCONFIGID", SqlDbType.NVarChar, 10).Value = sTransConfigId;


            using(SqlDataReader reader = command.ExecuteReader())
            {
                if(reader.HasRows)
                {
                    while(reader.Read())
                    {
                        dFixedRateConfigRatio = Convert.ToDecimal(reader.GetValue(0));
                        dTransConfigRatio = Convert.ToDecimal(reader.GetValue(1));
                        dTransIncrSalePct = Convert.ToDecimal(reader.GetValue(2));
                    }
                }
            }

            if(connection.State == ConnectionState.Open)
                connection.Close();

            return dFixedRateConfigRatio;
        }

        private void GetPurcExchngConfigRatio(string sConfigId, ref decimal dTransConfigRatio, ref decimal dParamConfigRatio)  // OG Retagging
        {
            SqlConnection connection = new SqlConnection();

            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "DECLARE @INVENTLOCATION VARCHAR(20) DECLARE @PARAMCONFIGID NVARCHAR(10)  DECLARE @TransCONFRATIO NUMERIC(32,10)  DECLARE @ParamCONFRATIO NUMERIC(32,10) " + //DECLARE @FIXEDCONFRATIO NUMERIC(32,10)
                                " SELECT @INVENTLOCATION = RETAILCHANNELTABLE.INVENTLOCATION" +
                                " FROM   RETAILCHANNELTABLE INNER JOIN RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID" +
                                " WHERE STORENUMBER = '" + ApplicationSettings.Database.StoreID + "'" +
                                " SELECT  @PARAMCONFIGID = ISNULL(DEFAULTCONFIGIDGOLD,'') FROM INVENTPARAMETERS WHERE DATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "'" +
                                " SELECT @TransCONFRATIO = ISNULL(CONFRATIO,0) FROM CONFIGRATIO WHERE METALTYPE = 1 AND INVENTLOCATIONID = @INVENTLOCATION AND CONFIGIDSTANDARD = '" + sConfigId + "'" +
                                " SELECT @ParamCONFRATIO = ISNULL(CONFRATIO,0) FROM CONFIGRATIO WHERE METALTYPE = 1 AND INVENTLOCATIONID = @INVENTLOCATION AND CONFIGIDSTANDARD = @PARAMCONFIGID" +
                                " SELECT @TransCONFRATIO, @ParamCONFRATIO";

            if(connection.State == ConnectionState.Closed)
                connection.Open();


            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            //command.Parameters.Add("@FIXEDCONFIGID", SqlDbType.NVarChar, 10).Value = sFixedConfigId;
            // command.Parameters.Add("@TRANSCONFIGID", SqlDbType.NVarChar, 10).Value = sTransConfigId;


            using(SqlDataReader reader = command.ExecuteReader())
            {
                if(reader.HasRows)
                {
                    while(reader.Read())
                    {
                        dTransConfigRatio = Convert.ToDecimal(reader.GetValue(0));
                        dParamConfigRatio = Convert.ToDecimal(reader.GetValue(1));
                    }
                }
            }

            if(connection.State == ConnectionState.Open)
                connection.Close();

        }

        private void GetPurcExchngGoldQty()
        {
            decimal dGoldQty = 0m;

            int iPEMetalType = 0;
            iPEMetalType = GetMetalType(sBaseItemID);

            if(iPEMetalType == (int)MetalType.Gold)
            {
                decimal dTransCR = 0;
                decimal dParamCR = 0;

                #region commented for samra
                //GetPurcExchngConfigRatio(sBaseConfigID, ref dTransCR, ref dParamCR);

                //if(dTransCR > 0 && dParamCR > 0)
                //{

                //    if(!string.IsNullOrEmpty(txtTotalWeight.Text.Trim()) && !string.IsNullOrEmpty(txtPurity.Text.Trim()))
                //    {
                //        dGoldQty = (Convert.ToDecimal(txtTotalWeight.Text.Trim()) * Convert.ToDecimal(txtPurity.Text.Trim()))
                //                        * (dTransCR / dParamCR);
                //    }

                //    if(dGoldQty > 0)
                //        txtQuantity.Text = Convert.ToString(decimal.Round(dGoldQty, 3, MidpointRounding.AwayFromZero));
                //    else
                //        txtQuantity.Text = string.Empty;
                //}
                #endregion
                if(!string.IsNullOrEmpty(txtTotalWeight.Text.Trim()))
                {
                    dGoldQty = Convert.ToDecimal(txtTotalWeight.Text.Trim());
                }

                if(dGoldQty > 0)
                    txtQuantity.Text = Convert.ToString(decimal.Round(dGoldQty, 3, MidpointRounding.AwayFromZero));
                else
                    txtQuantity.Text = string.Empty;

            }

        }


        private bool IsValidMakingRate(string sMKItemId)
        {
            bool bValid = true;

            SqlConnection connection = new SqlConnection();

            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "DECLARE @MFG_CODE AS NVARCHAR(20)  DECLARE @COMPLEXITY_CODE AS NVARCHAR(20) " +
                                 " DECLARE @RESULT AS INT  SELECT @MFG_CODE = ISNULL(MFG_CODE,'') FROM INVENTTABLE WHERE ITEMID = '" + sMKItemId + "'" +
                                 " SELECT @COMPLEXITY_CODE = ISNULL(COMPLEXITY_CODE,'') FROM INVENTTABLE WHERE ITEMID = '" + sMKItemId + "'" +
                                 " IF(@MFG_CODE <> '' AND @COMPLEXITY_CODE <> '') BEGIN SET @RESULT = 0 END ELSE" +
                                 " BEGIN SET @RESULT = 1 END  SELECT @RESULT";

            if(connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            bValid = Convert.ToBoolean(command.ExecuteScalar());
            if(connection.State == ConnectionState.Open)
                connection.Close();

            return bValid;
        }

        private bool IsCatchWtItem(string sItemId)
        {
            bool bCatchWtItem = false;

            SqlConnection connection = new SqlConnection();

            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "DECLARE @ISCATCHWT INT  SET @ISCATCHWT = 0 IF EXISTS (SELECT ITEMID FROM pdscatchweightitem WHERE ITEMID = '" + sItemId + "')" +
                                 " BEGIN SET @ISCATCHWT = 1 END SELECT @ISCATCHWT";


            if(connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            bCatchWtItem = Convert.ToBoolean(command.ExecuteScalar());
            if(connection.State == ConnectionState.Open)
                connection.Close();

            return bCatchWtItem;
        }
        #endregion

        /// <summary>
        /// retail =1 in inventtable , item can only sale
        /// Dev on  : 17/04/2014 by : RHossain
        /// </summary>
        /// <param name="sItemId"></param>
        /// <returns></returns>
        private bool IsRetailItem(string sItemId)
        {
            bool bRetailItem = false;

            SqlConnection connection = new SqlConnection();

            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "select RETAIL from inventtable WHERE ITEMID = '" + sItemId + "'";


            if(connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            bRetailItem = Convert.ToBoolean(command.ExecuteScalar());
            if(connection.State == ConnectionState.Open)
                connection.Close();

            return bRetailItem;
        }

        /// <summary>
        /// GIFT =1 in inventtable 
        /// Dev on  : 14/08/2015 by : RHossain
        /// </summary>
        /// <param name="sItemId"></param>
        /// <returns></returns>
        private bool IsGiftItem(string sItemId)
        {
            bool bGiftItem = false;

            SqlConnection connection = new SqlConnection();

            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "select GIFT from inventtable WHERE ITEMID = '" + sItemId + "'";


            if(connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            bGiftItem = Convert.ToBoolean(command.ExecuteScalar());
            if(connection.State == ConnectionState.Open)
                connection.Close();

            return bGiftItem;
        }

        private void txtMakingDisc_Leave(object sender, EventArgs e)
        {
            //txtMakingDisc_TextChanged(sender, e);
            string sFieldName = string.Empty;
            int iSPId = 0;
            iSPId = getUserDiscountPermissionId();
            decimal dinitDiscValue = 0;

            if(Convert.ToInt16(iSPId) == (int)CRWRetailDiscPermission.Salesperson)
                sFieldName = "MAXSALESPERSONSDISCPCT";
            else if(Convert.ToInt16(iSPId) == (int)CRWRetailDiscPermission.Manager)
                sFieldName = "MAXMANAGERLEVELDISCPCT";
            else if(Convert.ToInt16(iSPId) == (int)CRWRetailDiscPermission.Manager2)
                sFieldName = "MAXMANAGERLEVEL2DISCPCT";
            else
                //{
                MessageBox.Show("You are not authorized for this action.");
            //    sFieldName = "OPENINGDISCPCT";
            //}
            decimal dQty = 0;
            if(!string.IsNullOrEmpty(txtQuantity.Text))
                dQty = Convert.ToDecimal(txtQuantity.Text);
            else
                dQty = 0;

            if(!isMRPUCP)
            {
                if(!string.IsNullOrEmpty(sFieldName))
                    dinitDiscValue = GetMkDiscountFromDiscPolicy(sBaseItemID, dQty, sFieldName);// get OPENINGDISCPCT field value FOR THE OPENING
            }
            //else
            //    MessageBox.Show("Invalid discount policy for this item");

            decimal dMkPerDisc = 0;
            if(!string.IsNullOrEmpty(txtMakingDisc.Text))
                dMkPerDisc = Convert.ToDecimal(txtMakingDisc.Text);
            else
                dMkPerDisc = 0;

            if(dinitDiscValue >= 0)
            {
                if((dMkPerDisc > dinitDiscValue))
                {
                    MessageBox.Show("Line discount percentage should not more than '" + dinitDiscValue + "'");
                    txtMakingDisc.Focus();
                }
                else
                {
                    CheckMakingDiscountFromDB(dMkPerDisc);
                }
            }

            //else
            //{
            //    MessageBox.Show("Not allowed for this item");
            //}


        }

        private decimal getCostPrice(string sItemId, decimal dSalesValue)
        {
            //bool bReturn = false;

            SqlConnection connection = new SqlConnection();

            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "select ABS(CAST(ISNULL(CostPrice,0)AS DECIMAL(28,2)))  from SKUTABLE_POSTED WHERE skunumber = '" + sItemId + "'";


            if(connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            decimal _dCostValue = Convert.ToDecimal(command.ExecuteScalar());

            //if(_dCostValue <= dSalesValue)
            //    bReturn = true;
            //else
            //    bReturn = false;

            if(connection.State == ConnectionState.Open)
                connection.Close();

            return _dCostValue;
        }

        private bool IsBulkItem(string sItemId)
        {
            bool bBulkItem = false;

            SqlConnection connection = new SqlConnection();

            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "DECLARE @ISBULK INT  SET @ISBULK = 0 IF EXISTS (SELECT ITEMID FROM INVENTTABLE WHERE ITEMID = '" + sItemId + "' AND RETAIL=0)" +
                                 " BEGIN SET @ISBULK = 1 END SELECT @ISBULK";


            if(connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            bBulkItem = Convert.ToBoolean(command.ExecuteScalar());
            if(connection.State == ConnectionState.Open)
                connection.Close();

            return bBulkItem;
        }

        private void txtDiscPerc_TextChanged(object sender, EventArgs e)
        {
            decimal dDiscPct = 0m;
            decimal dDiscAmt = 0m;
            decimal dTotAmt = 0m;

            if (!string.IsNullOrEmpty(txtRate.Text))
                dTotAmt = Convert.ToDecimal(txtRate.Text);
            if (!string.IsNullOrEmpty(txtDiscPerc.Text))
                dDiscPct = Convert.ToDecimal(txtDiscPerc.Text);
            if (dDiscPct > 0)
            {
                if (dTotAmt > 0 && dDiscPct > 0)
                {
                    dDiscAmt = dTotAmt * dDiscPct / 100;
                    dTotAmt = dTotAmt - dDiscAmt;

                    txtTotalAmount.Text = decimal.Round(dTotAmt, 2, MidpointRounding.AwayFromZero).ToString();
                }
            }
            else
            {
                txtTotalAmount.Text = decimal.Round(dTotAmt, 2, MidpointRounding.AwayFromZero).ToString();
            }
        }
    }
}
