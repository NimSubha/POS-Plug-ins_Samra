using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.Dialog;
using LSRetailPosis.Transaction.Line.SaleItem;
using Microsoft.Dynamics.Retail.Pos.Item;
using Microsoft.Dynamics.Retail.Pos.Dimension;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch;
using Microsoft.Dynamics.Retail.Pos.RoundingService;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Notification.Contracts;
using Microsoft.Dynamics.Retail.Pos.Interaction;
using LSRetailPosis.Settings.FunctionalityProfiles;

namespace BlankOperations.WinFormsTouch
{
    public partial class frmOrderDetails:frmTouchBase
    {
        #region Variable Declaration
        bool bIsSKUItem = false;
        private SaleLineItem saleLineItem;
        Rounding objRounding = new Rounding();
        public IPosTransaction pos { get; set; }
        LSRetailPosis.POSProcesses.WinControls.NumPad obj;
        [Import]
        private IApplication application;
        DataTable dtItemInfo = new DataTable("dtItemInfo");
        public DataTable dtSubOrderDetails = new DataTable("dtSubOrderDetails");
        public decimal sExtendedDetailsAmount = 0m;
        frmCustomerOrder frmCustOrder;
        Random randUnique = new Random();
        bool IsEdit = false;
        int EditselectedIndex = 0;
        DataSet dsSearchedOrder = new DataSet();
        string inventDimId = string.Empty;
        string PreviousPcs = string.Empty;
        public DataTable dtSample = new DataTable();
        string unitid = string.Empty;
        string Previewdimensions = string.Empty;
        bool isItemExists = false;
        decimal dStoneWtRange = 0m;

        decimal dIngrdMetalWtRange = 0m;
        decimal dIngrdTotalGoldQty = 0m;
        decimal dIngrdTotalGoldValue = 0m;
        string sBaseItemID = string.Empty;
        string sBaseConfigID = string.Empty;
        Decimal dWMetalRate = 0m;
        decimal dWastQty = 0m; // Added for wastage
        //
        string sBookedSKUItem = string.Empty;
        public DataTable dtSketch = new DataTable();
        public DataTable dtSampleSketch = new DataTable();

        public DataTable dtStone = new DataTable();
        public DataTable dtPaySchedule = new DataTable();

        private bool isMRPExists = false;

        #region enum  RateType
        enum RateType
        {
            Weight = 0,
            Pcs = 1,
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

        #region Wastage Type

        enum WastageType
        {
            //  None    = 0,
            Weight = 0,
            Percentage = 1,
        }

        #endregion

        #endregion

        #region InventDimId
        public string GetInventID(string distinctProductVariantID)
        {
            string commandText = "select top(1)  INVENTDIMID from assortedinventdimcombination WHERE DISTINCTPRODUCTVARIANT='" + distinctProductVariantID + "'";

            SqlConnection connection = new SqlConnection();

            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if(connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }


            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;
            return Convert.ToString(command.ExecuteScalar());

        }
        #endregion

        #region frmOrderDetails
        public frmOrderDetails(IPosTransaction posTransaction, IApplication Application, frmCustomerOrder fCustOrder)
        {
            InitializeComponent();

            pos = posTransaction;
            application = Application;
            frmCustOrder = fCustOrder;
            BindRateTypeMakingTypeCombo();
            BindWastage();
            btnPOSItemSearch.Focus();
            //btnSample.Visible = true;
        }

        public frmOrderDetails(DataTable dtSampleDetails, DataTable dtSubOrder, DataTable dtOrderDetails, DataTable dtPaySch, IPosTransaction posTransaction, IApplication Application, frmCustomerOrder fCustOrder)
        {
            InitializeComponent();

            dtItemInfo = dtOrderDetails;
            dtSubOrderDetails = dtSubOrder;
            dtSample = dtSampleDetails;
            dtPaySchedule = dtPaySch;

            pos = posTransaction;
            application = Application;
            frmCustOrder = fCustOrder;
            BindRateTypeMakingTypeCombo();
            BindWastage();
            btnPOSItemSearch.Focus();
            grItems.DataSource = dtItemInfo.DefaultView;

            if(dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                Decimal dTotalAmount = 0m;
                foreach(DataRow drTotal in dtItemInfo.Rows)
                {
                    // dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["AMOUNT"])) ? Convert.ToDecimal(drTotal["AMOUNT"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["EXTENDEDDETAILS"])) ? Convert.ToDecimal(drTotal["EXTENDEDDETAILS"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["MAKINGAMOUNT"])) ? Convert.ToDecimal(drTotal["MAKINGAMOUNT"]) : 0); //COs  Modification
                    // dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["AMOUNT"])) ? Convert.ToDecimal(drTotal["AMOUNT"]) : 0) +  (!string.IsNullOrEmpty(Convert.ToString(drTotal["MAKINGAMOUNT"])) ? Convert.ToDecimal(drTotal["MAKINGAMOUNT"]) : 0); //COs  Modification
                    dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["AMOUNT"])) ? Convert.ToDecimal(drTotal["AMOUNT"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["MAKINGAMOUNT"])) ? Convert.ToDecimal(drTotal["MAKINGAMOUNT"]) : 0)
                                    + (!string.IsNullOrEmpty(Convert.ToString(drTotal["WastageAmount"])) ? Convert.ToDecimal(drTotal["WastageAmount"]) : 0); // Added for wastage 
                }
                txtTotalAmount.Text = Convert.ToString(objRounding.Round(dTotalAmount, 2));
            }

        }

        public frmOrderDetails(DataSet dsSearchedDetails, IPosTransaction posTransaction, IApplication Application, frmCustomerOrder fCustOrder)
        {
            InitializeComponent();

            dsSearchedOrder = dsSearchedDetails;
            pos = posTransaction;
            application = Application;
            frmCustOrder = fCustOrder;
            BindRateTypeMakingTypeCombo();
            BindWastage();
            btnPOSItemSearch.Focus();
            grItems.DataSource = dsSearchedDetails.Tables[1].DefaultView;

            if(dsSearchedDetails.Tables[1] != null && dsSearchedDetails.Tables[1].Rows.Count > 0)
            {
                Decimal dTotalAmount = 0m;
                foreach(DataRow drTotal in dsSearchedDetails.Tables[1].Rows)
                {
                    // dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["AMOUNT"])) ? Convert.ToDecimal(drTotal["AMOUNT"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["EXTENDEDDETAILS"])) ? Convert.ToDecimal(drTotal["EXTENDEDDETAILS"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["MAKINGAMOUNT"])) ? Convert.ToDecimal(drTotal["MAKINGAMOUNT"]) : 0); //COs  Modification
                    // dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["AMOUNT"])) ? Convert.ToDecimal(drTotal["AMOUNT"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["MAKINGAMOUNT"])) ? Convert.ToDecimal(drTotal["MAKINGAMOUNT"]) : 0); //COs  Modification
                    dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["AMOUNT"])) ? Convert.ToDecimal(drTotal["AMOUNT"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["MAKINGAMOUNT"])) ? Convert.ToDecimal(drTotal["MAKINGAMOUNT"]) : 0)
                                    + (!string.IsNullOrEmpty(Convert.ToString(drTotal["WastageAmount"])) ? Convert.ToDecimal(drTotal["WastageAmount"]) : 0);
                }
                txtTotalAmount.Text = Convert.ToString(objRounding.Round(dTotalAmount, 2));
            }
            panel1.Enabled = false;
            btnSubmit.Enabled = false;
            btnEdit.Enabled = false;
            btnClear.Enabled = false;
            btnDelete.Enabled = false;
            btnPOSItemSearch.Enabled = false;
            btnAddItem.Enabled = false;

            if(dsSearchedDetails.Tables.Count > 4)
            {
                frmCustOrder.dtSampleDetails = dsSearchedDetails.Tables[3];
            }

            dtSample = frmCustOrder.dtSampleDetails;
            dtSampleSketch = frmCustOrder.dtSampleSketch;

            if(dtSample.Rows.Count > 0)
                btnSampleReturn.Enabled = true;

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

        private string ColorSizeStyleConfig()
        {
            string dash = " - ";
            StringBuilder colorSizeStyleConfig;
            if(!string.IsNullOrEmpty(cmbCode.Text))
                colorSizeStyleConfig = new StringBuilder("Color : " + cmbCode.Text);
            else
                colorSizeStyleConfig = new StringBuilder(cmbCode.Text);

            if(!string.IsNullOrEmpty(cmbSize.Text))
            {
                if(colorSizeStyleConfig.Length > 0)
                {
                    colorSizeStyleConfig.Append(dash);
                }
                colorSizeStyleConfig.Append(" Size : " + cmbSize.Text);
            }

            if(!string.IsNullOrEmpty(cmbStyle.Text))
            {
                if(colorSizeStyleConfig.Length > 0)
                {
                    colorSizeStyleConfig.Append(dash);
                }
                colorSizeStyleConfig.Append(" Style : " + cmbStyle.Text);
            }

            if(!string.IsNullOrEmpty(cmbConfig.Text))
            {
                if(colorSizeStyleConfig.Length > 0) { colorSizeStyleConfig.Append(dash); }
                colorSizeStyleConfig.Append(" Configuration : " + cmbConfig.Text);
            }

            return colorSizeStyleConfig.ToString();
        }

        #region POS-ITEM-SEARCH-CLICK
        private void btnPOSItemSearch_Click(object sender, EventArgs e)
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
            //  objdialog.MyItemSearch(500, ref str, out  dsItem, " AND  I.ITEMID NOT IN (SELECT ITEMID FROM INVENTTABLE WHERE RETAIL=1) "); // blocked on 12.09.2013 // SKU allow

            // objdialog.MyItemSearch(500, ref str, out  dsItem, "");
            objdialog.MyItemSearch(500, ref str, out  dsItem);

            saleLineItem = new SaleLineItem();

            if(dsItem != null && dsItem.Tables.Count > 0 && dsItem.Tables[0].Rows.Count > 0)
            {
                if(!string.IsNullOrEmpty(Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"])))
                {
                    saleLineItem.ItemId = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"]);

                    Item objItem = new Item();
                    objItem.MYProcessItem(saleLineItem, application);

                    Dimension objDim = new Dimension();
                    DataTable dtDimension = objDim.GetDimensions(saleLineItem.ItemId);

                    #region testing
                    //DataRow dr = null;
                    //dr = dtDimension.NewRow();
                    //dr[0] = "";
                    //dr[1] = "NSPL-00016";
                    //dr[2] = "5637144869";
                    //dr[3] = "42";
                    //dr[4] = "01";
                    //dr[5] = "1";
                    //dr[6] = "HD";
                    //dr[7] = "Black";
                    //dr[8] = "42";
                    //dr[9] = "";
                    //dr[10] = "High Definition";
                    //dr[11] = "1";
                    //dr[12] = "1";
                    //dr[13] = "0.0000000000000000";
                    //dr[14] = "0.0000000000000000";
                    //dr[15] = "0.0000000000000000";
                    //dtDimension.Rows.Add(dr);
                    #endregion
                    if(dtDimension != null && dtDimension.Rows.Count > 0)
                    {
                        DimensionConfirmation dimConfirmation = new DimensionConfirmation();
                        dimConfirmation.InventDimCombination = dtDimension;
                        dimConfirmation.DimensionData = saleLineItem.Dimension;

                        //frmDimensions objfrmDim = new frmDimensions(dimConfirmation);
                        //objfrmDim.ShowDialog();

                        if(!IsSKUItem(saleLineItem.ItemId))
                        {
                            bIsSKUItem = false;
                            frmDimensions objfrmDim = new frmDimensions(dimConfirmation);
                            objfrmDim.ShowDialog();

                            if(objfrmDim.SelectDimCombination != null)
                            {
                                inventDimId = GetInventID(Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[2]));
                                DataTable dtcmbCode = new DataTable();
                                dtcmbCode.Columns.Add("CodeID", typeof(string));
                                dtcmbCode.Columns.Add("CodeValue", typeof(string));
                                DataRow drCode;
                                drCode = dtcmbCode.NewRow();
                                drCode["CodeID"] = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[4]);
                                drCode["CodeValue"] = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[4]);
                                dtcmbCode.Rows.Add(drCode);
                                cmbCode.DataSource = dtcmbCode;
                                cmbCode.DisplayMember = "CodeValue";
                                cmbCode.ValueMember = "CodeID";

                                DataTable dtSize = new DataTable();
                                dtSize.Columns.Add("SizeID", typeof(string));
                                dtSize.Columns.Add("SizeValue", typeof(string));
                                DataRow drSize;
                                drSize = dtSize.NewRow();
                                drSize["SizeID"] = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[3]);
                                drSize["SizeValue"] = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[3]);
                                dtSize.Rows.Add(drSize);
                                cmbSize.DataSource = dtSize;
                                cmbSize.DisplayMember = "SizeID";
                                cmbSize.ValueMember = "SizeValue";

                                DataTable dtConfig = new DataTable();
                                dtConfig.Columns.Add("ConfigID", typeof(string));
                                dtConfig.Columns.Add("ConfigValue", typeof(string));
                                DataRow drConfig;
                                drConfig = dtConfig.NewRow();
                                drConfig["ConfigID"] = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[6]);
                                drConfig["ConfigValue"] = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[6]);
                                dtConfig.Rows.Add(drConfig);
                                cmbConfig.DataSource = dtConfig;
                                cmbConfig.DisplayMember = "ConfigID";
                                cmbConfig.ValueMember = "ConfigValue";

                                DataTable dtStyle = new DataTable();
                                dtStyle.Columns.Add("StyleID", typeof(string));
                                dtStyle.Columns.Add("StyleValue", typeof(string));
                                DataRow drStyle;
                                drStyle = dtStyle.NewRow();
                                drStyle["StyleID"] = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[5]);
                                drStyle["StyleValue"] = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[5]);
                                dtStyle.Rows.Add(drStyle);
                                cmbStyle.DataSource = dtStyle;
                                cmbStyle.DisplayMember = "StyleID";
                                cmbStyle.ValueMember = "StyleValue";
                                cmbConfig.Enabled = false;
                                cmbCode.Enabled = false;
                                cmbSize.Enabled = false;
                                cmbStyle.Enabled = false;

                                Previewdimensions = ColorSizeStyleConfig();
                            }
                        }
                        else
                        {
                            bIsSKUItem = true;
                            cmbSize.Text = dtDimension.Rows[0]["sizeid"].ToString();
                            cmbConfig.Text = dtDimension.Rows[0]["configid"].ToString();
                        }
                    }
                    else
                    {
                        cmbStyle.Text = string.Empty;
                        cmbConfig.Text = string.Empty;
                        cmbCode.Text = string.Empty;
                        cmbSize.Text = string.Empty;
                        cmbConfig.Enabled = false;
                        cmbCode.Enabled = false;
                        cmbSize.Enabled = false;
                        cmbStyle.Enabled = false;
                    }
                    SqlConnection conn = new SqlConnection();
                    if(application != null)
                        conn = application.Settings.Database.Connection;
                    else
                        conn = ApplicationSettings.Database.LocalConnection;
                    if(IsCatchWtItem(Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"])))
                    {
                        txtPCS.Enabled = true;
                        txtPCS.Focus();
                        txtPCS.Text = "1";
                    }
                    else
                    {
                        txtPCS.Text = "";
                        txtQuantity.Focus();
                        txtPCS.Enabled = false;
                    }
                    string sQty = string.Empty;
                    sQty = GetStandardQuantityFromDB(conn, Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"]));
                    if(!string.IsNullOrEmpty(sQty))
                    {
                        sQty = Convert.ToString(decimal.Round(Convert.ToDecimal(sQty), 3, MidpointRounding.AwayFromZero));
                        txtQuantity.Text = Convert.ToString(Convert.ToDecimal(sQty) == 0 ? string.Empty : Convert.ToString(sQty));
                    }
                    txtItemId.Text = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"]);
                    txtItemName.Text = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMNAME"]);
                    txtRate.Text = getRateFromMetalTable(txtItemId.Text, cmbConfig.Text, cmbStyle.Text, cmbCode.Text, cmbSize.Text, txtQuantity.Text);



                    unitid = saleLineItem.BackofficeSalesOrderUnitOfMeasure;
                    //  txtRate.Text = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMPRICE"]).Remove(0, 1).Trim();

                    if(Convert.ToString(txtItemId.Text.Trim()) == string.Empty
                        || Convert.ToString(cmbConfig.Text.Trim()) == string.Empty)
                    {
                        sBaseItemID = string.Empty;
                        sBaseConfigID = string.Empty;
                    }
                    else
                    {
                        sBaseItemID = txtItemId.Text.Trim();
                        sBaseConfigID = cmbConfig.Text;
                    }

                    if(IsSKUItem(txtItemId.Text.Trim()))
                    {
                        chkBookedSKU.Enabled = true;
                        lblItemId.Text = "SKU number";
                    }
                    else
                    {
                        chkBookedSKU.Enabled = false;
                        lblItemId.Text = "Item Id";
                    }

                    isItemExists = true;
                }
            }
            else
            {
                isItemExists = false;
            }

            SqlConnection connection = new SqlConnection();
            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            isMRPExists = isMRP(txtItemId.Text, connection);

            if(isMRPExists)
            {
                decimal dAmt = GetMRPPrice(saleLineItem.ItemId, connection);
                txtRate.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(dAmt), 2, MidpointRounding.AwayFromZero)); //Convert.ToString(dAmt);
            }

            if(!string.IsNullOrEmpty(txtRate.Text))
                cmbRateType.SelectedIndex = cmbRateType.FindStringExact("Tot");
        }
        #endregion
        private bool isMRP(string itemid, SqlConnection connection)
        {
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT TOP(1) MRP FROM [INVENTTABLE] WHERE ITEMID='" + itemid + "' ");


            if(connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToBoolean(cmd.ExecuteScalar());
        }

        private decimal GetMRPPrice(string sItemId, SqlConnection connection)
        {

            //List<DE.PriceDiscTable> tradeAgreements = new List<DE.PriceDiscTable>();
            //string itemRelation = args.GetItemRelation(itemCode);
            //IList<string> accountRelations = args.GetAccountRelations(accountCode);
            //string unitId = args.GetUnitId(itemCode);
            DateTime today = DateTime.Now.Date;

            decimal dAmount = 0;
            string dataAreaId = ApplicationSettings.Database.DATAAREAID;

            bool isIndia = Functions.CountryRegion == SupportedCountryRegion.IN;
            try
            {
                // convert account relations list to data-table for use as TVP in the query.
                string queryString = @"
                        SELECT 
                            ta.PRICEUNIT,
                            ta.ALLOCATEMARKUP,
                            ta.AMOUNT,
                            ta.SEARCHAGAIN"
                            + (isIndia ? ", ta.MAXIMUMRETAILPRICE_IN" : string.Empty)
                            + @"
                        FROM 
                            PRICEDISCTABLE ta LEFT JOIN 
                                INVENTDIM invdim ON ta.INVENTDIMID = invdim.INVENTDIMID AND ta.DATAAREAID = invdim.DATAAREAID
                        WHERE
                            ta.RELATION = 4
                            AND ta.ITEMCODE = @ITEMCODE
                            AND ta.ITEMRELATION = @ITEMRELATION
                            AND ta.ACCOUNTCODE = @ACCOUNTCODE
                    
                            -- USES Tvp: CREATE TYPE FINDPRICEAGREEMENT_ACCOUNTRELATIONS_TABLETYPE AS TABLE(ACCOUNTRELATION nvarchar(20) NOT NULL);
                            --AND (ta.ACCOUNTRELATION) IN (SELECT ar.ACCOUNTRELATION FROM @ACCOUNTRELATIONS ar)

                            AND ta.CURRENCY = @CURRENCYCODE
                            AND ta.UNITID = @UNITID
                            AND ta.QUANTITYAMOUNTFROM <= abs(@QUANTITY)
                            AND (ta.QUANTITYAMOUNTTO >= abs(@QUANTITY) OR ta.QUANTITYAMOUNTTO = 0)
                            AND ta.DATAAREAID = @DATAAREAID
                            AND ((ta.FROMDATE <= @TODAY OR ta.FROMDATE <= @NODATE)
                                    AND (ta.TODATE >= @TODAY OR ta.TODATE <= @NODATE))
                            AND (invdim.INVENTCOLORID in (@COLORID, ''))
                            AND (invdim.INVENTSIZEID in (@SIZEID,''))
                            AND (invdim.INVENTSTYLEID in (@STYLEID, ''))
                            AND (invdim.CONFIGID in (@CONFIGID, ''))

                            --// ORDERBY CLAUSE MUST MATCH THAT IN AX TO ENSURE COMPATIBLE PRICING BEHAVIOR.
                            --// SEE THE CLASS PRICEDISC.FINDPRICEAGREEMENT() AND TABLE PRICEDISCTABLE.PRICEDISCIDX
                            order by ta.AMOUNT, ta.QUANTITYAMOUNTFROM, ta.QUANTITYAMOUNTTO, ta.FROMDATE";

                using(SqlCommand command = new SqlCommand(queryString, connection))
                {
                    command.Parameters.AddWithValue("@DATAAREAID", dataAreaId);
                    command.Parameters.AddWithValue("@ITEMCODE", 0);
                    command.Parameters.AddWithValue("@ITEMRELATION", sItemId);
                    command.Parameters.AddWithValue("@ACCOUNTCODE", 2);
                    command.Parameters.AddWithValue("@UNITID", unitid);
                    command.Parameters.AddWithValue("@CURRENCYCODE", ApplicationSettings.Terminal.CompanyCurrency);
                    command.Parameters.AddWithValue("@QUANTITY", 1);
                    command.Parameters.AddWithValue("@COLORID", ("") ?? string.Empty);
                    command.Parameters.AddWithValue("@SIZEID", (cmbSize.Text) ?? string.Empty);
                    command.Parameters.AddWithValue("@STYLEID", ("") ?? string.Empty);
                    command.Parameters.AddWithValue("@CONFIGID", (cmbConfig.Text) ?? string.Empty);
                    command.Parameters.AddWithValue("@TODAY", today);
                    command.Parameters.AddWithValue("@NODATE", DateTime.Parse("1900-01-01"));

                    // Fill out TVP for account relations list
                    using(DataTable accountRelationsTable = new DataTable())
                    {
                        accountRelationsTable.Columns.Add("ACCOUNTRELATION", typeof(string));
                        //foreach(string relation in accountRelations)
                        //{
                        //    accountRelationsTable.Rows.Add(relation); 
                        //}

                        SqlParameter param = command.Parameters.Add("@ACCOUNTRELATIONS", SqlDbType.Structured);
                        param.Direction = ParameterDirection.Input;
                        param.TypeName = "FINDPRICEAGREEMENT_ACCOUNTRELATIONS_TABLETYPE";
                        param.Value = accountRelationsTable;

                        if(connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }

                        SqlDataReader reader = command.ExecuteReader();

                        if(reader.HasRows)
                        {
                            while(reader.Read())
                            {
                                dAmount = reader.GetDecimal(reader.GetOrdinal("AMOUNT"));
                            }
                        }
                    }
                }
            }
            finally
            {
                if(connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dAmount;
        }

        #region GetIngredientDetails
        public DataTable GetIngredientDetails(string sUnique, bool IsSKUItem = false)
        {
            DataTable dt = new DataTable();
            DataTable dtClone = new DataTable();
            string commandText = string.Empty;

            decimal dPcs = 0;

            if(!string.IsNullOrEmpty(txtPCS.Text.Trim()))
                dPcs = Convert.ToDecimal(txtPCS.Text.Trim());

            if(IsSKUItem)
            {
                commandText = "SELECT " + sUnique + " AS UNIQUEID, A.ItemID," +
                              " (SELECT     ISNULL(TR.NAME, I.ITEMNAME) AS Expr1 FROM ASSORTEDINVENTITEMS AS I INNER JOIN " +
                              " ECORESPRODUCT AS PR ON PR.RECID = I.PRODUCT LEFT OUTER JOIN ECORESPRODUCTTRANSLATION AS TR ON PR.RECID = TR.PRODUCT  " +
                              "  AND TR.LANGUAGEID = '" + ApplicationSettings.Terminal.CultureName + "' WHERE (I.ITEMID = A.ITEMID) AND " +
                              " (I.DATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "') AND (I.STORERECID = '" + ApplicationSettings.Terminal.StorePrimaryId + "')) AS ITEMNAME, " +
                              " INVENTDIM.CONFIGID AS CONFIGURATION,INVENTDIM.INVENTCOLORID AS COLOR, INVENTDIM.INVENTSIZEID AS SIZE,INVENTDIM.INVENTSTYLEID AS STYLE, " +
                              " CAST(ISNULL(A.PDSCWQTY,0) AS INT) AS PCS, CAST(ISNULL(A.QTY,0) AS NUMERIC(16,3)) AS QUANTITY," +
                              " INVENTDIM.INVENTBATCHID, 0.00 AS RATE, 0 AS RATETYPE, 0.00 AS AMOUNT ,INVENTDIM.INVENTDIMID,A.UNITID,ISNULL(X.METALTYPE,0) AS METALTYPE " +
                              " FROM         SKULine_Posted A INNER JOIN " +
                              " INVENTDIM ON A.InventDimID = INVENTDIM.INVENTDIMID" +
                              " INNER JOIN INVENTTABLE X ON A.ITEMID = X.ITEMID" +
                              " WHERE INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'" +
                              " AND  A.[SkuNumber] = '" + txtItemId.Text.Trim() + "' ORDER BY X.METALTYPE ";

                /*
                commandText.Append(" SELECT     SKULine_Posted.SkuNumber, SKULine_Posted.SkuDate, SKULine_Posted.ItemID, INVENTDIM.InventDimID, INVENTDIM.InventSizeID,  ");
                // commandText.Append(" INVENTDIM.InventColorID, INVENTDIM.ConfigID, INVENTDIM.InventBatchID, CAST(SKULine_Posted.PDSCWQTY AS INT) AS PCS, CAST(SKULine_Posted.QTY AS NUMERIC(16,3)) AS QTY,  ");
                commandText.Append(" INVENTDIM.InventColorID, INVENTDIM.ConfigID, INVENTDIM.InventBatchID, CAST(ISNULL(SKULine_Posted.PDSCWQTY,0) AS INT) AS PCS, CAST(ISNULL(SKULine_Posted.QTY,0) AS NUMERIC(16,3)) AS QTY,  ");
                commandText.Append(" SKULine_Posted.CValue, SKULine_Posted.CRate AS Rate, SKULine_Posted.UnitID,X.METALTYPE");
                //  commandText.Append(" ,0 AS IngrdDiscType,0 AS IngrdDiscAmt,0 AS IngrdDiscTotAmt "); // Stone Discount
                commandText.Append(" FROM         SKULine_Posted INNER JOIN ");
                commandText.Append(" INVENTDIM ON SKULine_Posted.InventDimID = INVENTDIM.INVENTDIMID ");
                commandText.Append(" INNER JOIN INVENTTABLE X ON SKULine_Posted.ITEMID = X.ITEMID ");
                commandText.Append(" WHERE INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "' ");
                commandText.Append("  AND  [SkuNumber] = '" + ItemID.Trim() + "' ORDER BY X.METALTYPE END ");
                 * */
            }
            else
            {

                //string commandText = " DECLARE @BOMID VARCHAR(20) " +
                //                      " DECLARE @QUANTITY NUMERIC(36,18) " +
                //                      " DECLARE @PCS NUMERIC(36,18) " +
                //                      " DECLARE @ITEMNAME VARCHAR(100) " +
                //                      " SELECT @BOMID=BOMID FROM BOMVERSION WHERE ITEMID='" + txtItemId.Text.Trim() + "' AND ACTIVE=1 AND APPROVED=1 AND DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "' " +
                //                      " SET @PCS = " + Convert.ToDecimal(txtPCS.Text.Trim()) + "; " +
                //                      " SET @QUANTITY = " + Convert.ToDecimal(txtQuantity.Text.Trim()) + "; " +
                //                     " SELECT " + sUnique + " AS UNIQUEID, ITEMID AS ITEMID,  " +
                //                     " (SELECT ISNULL(TR.NAME, I.ITEMNAME) FROM ASSORTEDINVENTITEMS I  " +
                //                     " JOIN ECORESPRODUCT AS PR ON PR.RECID = I.PRODUCT " +
                //                     " LEFT JOIN ECORESPRODUCTTRANSLATION AS TR ON PR.RECID = TR.PRODUCT " +
                //                     " AND LANGUAGEID = '" + ApplicationSettings.Terminal.CultureName + "' WHERE I.ITEMID = BOM.ITEMID AND  " +
                //                     " I.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "' AND I.STORERECID = '" + ApplicationSettings.Terminal.StorePrimaryId + "') AS ITEMNAME, " +
                //                     " CONFIGID AS CONFIGURATION , INVENTCOLORID AS COLOR,  " +
                //                     " INVENTSIZEID AS SIZE, INVENTSTYLEID AS STYLE, (BOMQTYSERIE * @PCS) AS PCS,  " +
                //                     " (@QUANTITY * BOMQTY) AS QUANTITY,INVENTBATCHID,  0.00 AS RATE,CALCULATION AS RATETYPE, 0.00 AS AMOUNT " +
                //                     " FROM BOM WHERE BOMID=@BOMID ";


                commandText = " DECLARE @BOMID VARCHAR(20) " +
                                     " DECLARE @QUANTITY NUMERIC(36,18) " +
                                     " DECLARE @PCS NUMERIC(36,18) " +
                                     " DECLARE @ITEMNAME VARCHAR(100) " +
                                     " SELECT @BOMID=BOMID FROM BOMVERSION WHERE ITEMID='" + txtItemId.Text.Trim() + "' AND ACTIVE=1 AND APPROVED=1 AND DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "' " +
                                     " SET @PCS = " + dPcs + "; " +
                                     " SET @QUANTITY = " + Convert.ToDecimal(txtQuantity.Text.Trim()) + "; " +
                                    " SELECT " + sUnique + " AS UNIQUEID, BOM.ITEMID,  " +
                                    "  (SELECT     ISNULL(TR.NAME, I.ITEMNAME) AS Expr1 FROM ASSORTEDINVENTITEMS AS I INNER JOIN  " +
                                    " ECORESPRODUCT AS PR ON PR.RECID = I.PRODUCT LEFT OUTER JOIN ECORESPRODUCTTRANSLATION AS TR ON PR.RECID = TR.PRODUCT  " +
                                    "  AND TR.LANGUAGEID = '" + ApplicationSettings.Terminal.CultureName + "' WHERE (I.ITEMID = BOM.ITEMID) AND " +
                                    " (I.DATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "') AND (I.STORERECID = '" + ApplicationSettings.Terminal.StorePrimaryId + "')) AS ITEMNAME, " +
                                    " INVENTDIM.CONFIGID AS CONFIGURATION,INVENTDIM.INVENTCOLORID AS COLOR, INVENTDIM.INVENTSIZEID AS SIZE,  " +
                    // Changed on 21-01-2012 - On Request of urvi and arunava   //  " INVENTDIM.INVENTSTYLEID AS STYLE, CAST(BOM.BOMQTYSERIE * @PCS AS INT) AS PCS, CAST(@QUANTITY * BOM.BOMQTY AS NUMERIC(16,3)) AS QUANTITY, " +
                    // Changed on 22-02-2012 - On request of urvi   //  " INVENTDIM.INVENTSTYLEID AS STYLE, CAST(BOM.PDSCWQTY * @PCS AS INT) AS PCS, CAST(@QUANTITY * BOM.BOMQTY AS NUMERIC(16,3)) AS QUANTITY, " +
                                    " INVENTDIM.INVENTSTYLEID AS STYLE, CAST(ROUND(((BOM.PDSCWQTY/BOM.BOMQTYSERIE) * @QUANTITY),0) AS INT) AS PCS, CAST(((BOM.BOMQTY/BOM.BOMQTYSERIE) * @QUANTITY) AS NUMERIC(16,3)) AS QUANTITY, " +
                                    " INVENTDIM.INVENTBATCHID, 0.00 AS RATE, 0 AS RATETYPE, 0.00 AS AMOUNT ,INVENTDIM.INVENTDIMID " +
                                    " ,BOM.UNITID, ISNULL(X.METALTYPE,0) AS METALTYPE " + //COs  Modification
                                    " FROM         BOM INNER JOIN INVENTDIM ON BOM.INVENTDIMID = INVENTDIM.INVENTDIMID " +
                                    "  INNER JOIN INVENTTABLE X ON BOM.ITEMID = X.ITEMID" + //COs  Modification
                                    " WHERE     (BOM.BOMID = @BOMID) AND (INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "') " +
                                    " ORDER BY X.METALTYPE";
            }


            SqlConnection connection = new SqlConnection();
            try
            {
                if(application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;


                if(connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }


                SqlCommand command = new SqlCommand(commandText, connection);
                command.CommandTimeout = 0;
                SqlDataReader reader = command.ExecuteReader();
                if(reader.HasRows)
                {
                    dt.Load(reader);
                    dtClone = dt.Clone();
                    dtClone.Columns["UNIQUEID"].DataType = typeof(double);
                    dtClone.Columns["RATETYPE"].DataType = typeof(string);
                    dtClone.Columns["PCS"].ReadOnly = false;
                    dtClone.Columns["QUANTITY"].ReadOnly = false;
                    dtClone.Columns["RATE"].ReadOnly = false;
                    dtClone.Columns["RATETYPE"].ReadOnly = false;

                    dtClone.Columns["AMOUNT"].ReadOnly = false;

                    foreach(DataRow dr in dt.Rows)
                    {
                        dtClone.ImportRow(dr);
                    }
                    foreach(DataRow dr in dtClone.Rows)
                    {
                        string sRate = string.Empty;
                        string sCalcType = "";

                        if(Convert.ToDecimal(dr["PCS"]) > 0)
                        {
                            dStoneWtRange = Convert.ToDecimal(dr["QUANTITY"]) / Convert.ToDecimal(dr["PCS"]);
                        }
                        else
                        {
                            dStoneWtRange = Convert.ToDecimal(dr["QUANTITY"]);
                        }

                        sRate = getRateFromMetalTable(Convert.ToString(dr["ITEMID"]), Convert.ToString(dr["CONFIGURATION"]), Convert.ToString(dr["INVENTBATCHID"]),
                                                      Convert.ToString(dr["COLOR"]), Convert.ToString(dr["SIZE"]), Convert.ToString(dr["QUANTITY"]), Convert.ToString(dr["PCS"]));

                        if(!string.IsNullOrEmpty(sRate))
                        {
                            sCalcType = GetIngredientCalcType(Convert.ToString(dr["ITEMID"]), Convert.ToString(dr["CONFIGURATION"]), Convert.ToString(dr["INVENTBATCHID"]),
                                                      Convert.ToString(dr["COLOR"]), Convert.ToString(dr["SIZE"]), Convert.ToString(dr["QUANTITY"]), Convert.ToString(dr["PCS"]));   // added 
                            dr["RATE"] = sRate;

                        }

                        /*
                        if (Convert.ToInt32(dr["RATETYPE"]) == (int)RateType.Weight)
                            dr["RATETYPE"] = Convert.ToString(RateType.Weight);
                        else if (Convert.ToInt32(dr["RATETYPE"]) == (int)RateType.Pcs)
                            dr["RATETYPE"] = Convert.ToString(RateType.Pcs);
                        else
                            dr["RATETYPE"] = Convert.ToString(RateType.Tot);
                        */

                        if(!string.IsNullOrEmpty(sCalcType))
                        {
                            //  dr["RATETYPE"] = sCalcType; 
                            if(Convert.ToInt32(sCalcType) == (int)RateType.Weight)
                                dr["RATETYPE"] = Convert.ToString(RateType.Weight);
                            else if(Convert.ToInt32(sCalcType) == (int)RateType.Pcs)
                                dr["RATETYPE"] = Convert.ToString(RateType.Pcs);
                            else
                                dr["RATETYPE"] = Convert.ToString(RateType.Tot);

                            //
                        }
                        else
                        {
                            dr["RATETYPE"] = Convert.ToString(RateType.Weight);
                        }
                        //

                        /* Blocked 
                        if (Convert.ToString(dr["RATETYPE"]) == Convert.ToString(RateType.Weight))
                            dr["AMOUNT"] = decimal.Round(Convert.ToDecimal(dr["QUANTITY"]) * Convert.ToDecimal(dr["RATE"]), 2, MidpointRounding.AwayFromZero);
                        else if (Convert.ToString(dr["RATETYPE"]) == Convert.ToString(RateType.Pcs))
                            dr["AMOUNT"] = decimal.Round(Convert.ToDecimal(dr["PCS"]) * Convert.ToDecimal(dr["RATE"]), 2, MidpointRounding.AwayFromZero);
                        else
                            dr["AMOUNT"] = decimal.Round(Convert.ToDecimal(dr["RATE"]), 2, MidpointRounding.AwayFromZero);
                        */


                        if(Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.LooseDmd || Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Stone)
                        {

                            // Added 
                            if(!string.IsNullOrEmpty(sCalcType))
                            {

                                // if (Convert.ToInt32(dr["RATETYPE"]) == Convert.ToInt32(RateType.Weight)) //COs  Modification
                                if(Convert.ToString(dr["RATETYPE"]) == Convert.ToString(RateType.Weight)) //COs  Modification
                                    dr["AMOUNT"] = decimal.Round(Convert.ToDecimal(dr["QUANTITY"]) * Convert.ToDecimal(dr["RATE"]), 2, MidpointRounding.AwayFromZero);
                                // else if (Convert.ToInt32(dr["RATETYPE"]) == Convert.ToInt32(RateType.Pcs))
                                else if(Convert.ToString(dr["RATETYPE"]) == Convert.ToString(RateType.Pcs))
                                    dr["AMOUNT"] = decimal.Round(Convert.ToDecimal(dr["PCS"]) * Convert.ToDecimal(dr["RATE"]), 2, MidpointRounding.AwayFromZero);
                                else
                                    dr["AMOUNT"] = decimal.Round(Convert.ToDecimal(dr["RATE"]), 2, MidpointRounding.AwayFromZero);
                            }
                            else
                            {
                                dr["AMOUNT"] = decimal.Round(Convert.ToDecimal(dr["QUANTITY"]) * Convert.ToDecimal(dr["RATE"]), 2, MidpointRounding.AwayFromZero);
                            }
                        }
                        else
                        {
                            // dr["AMOUNT"] = decimal.Round(Convert.ToDecimal(dr["RATE"]), 2, MidpointRounding.AwayFromZero); //COs  Modification
                            dr["AMOUNT"] = decimal.Round(Convert.ToDecimal(dr["QUANTITY"]) * Convert.ToDecimal(dr["RATE"]), 2, MidpointRounding.AwayFromZero);

                        }

                        dStoneWtRange = 0m;

                    }
                    dtClone.Columns.Remove("INVENTBATCHID");
                    dtClone.AcceptChanges();
                }
                return dtClone;
            }
            catch(LSRetailPosis.PosisException ex)
            {
                throw ex;
            }
            finally
            {
                if(connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }

            }


        }


        #endregion

        #region  GET RATE FROM METAL TABLE
        public string getRateFromMetalTable(string itemid, string configuration, string batchid, string colorid, string sizeid, string weight, string pcs = null)
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) ");
            commandText.Append(" DECLARE @METALTYPE INT ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM         RETAILCHANNELTABLE INNER JOIN  ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + ApplicationSettings.Database.StoreID.Trim() + "'  ");
            commandText.Append(" SELECT @METALTYPE=METALTYPE FROM [INVENTTABLE] WHERE ITEMID='" + itemid.Trim() + "' ");

            commandText.Append(" IF(@METALTYPE IN ('" + (int)MetalType.Gold + "','" + (int)MetalType.Silver + "','" + (int)MetalType.Platinum + "','" + (int)MetalType.Palladium + "')) ");
            commandText.Append(" BEGIN ");
            //   }
            commandText.Append(" SELECT TOP 1 CAST(RATES AS numeric(26,2)) FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION  ");
            commandText.Append(" AND METALTYPE=@METALTYPE ");
            commandText.Append(" AND RETAIL=1 AND RATETYPE='" + (int)RateTypeNew.Sale + "' ");
            commandText.Append(" AND ACTIVE=1 AND CONFIGIDSTANDARD='" + configuration.Trim() + "' ");
            commandText.Append("   ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC ");

            if(string.IsNullOrEmpty(weight))
            {
                weight = "0";
            }
            string ldweight = string.Empty;
            ldweight = "0";
            if(pcs != null && Convert.ToDecimal(pcs) != Convert.ToDecimal(0))   // Convert.ToDecimal(pcs) != Convert.ToDecimal(0) added as per BOM.PDSCWQTY added in BOM on request of urvi and arunava 
            {
                ldweight = Convert.ToString(decimal.Round((Convert.ToDecimal(weight) / Convert.ToDecimal(pcs)), 3, MidpointRounding.AwayFromZero)); //Changes on 11/04/2014 R.Hossain
                //dStoneWtRange = decimal.Round(Convert.ToDecimal(dr["QTY"]) / Convert.ToDecimal(dr["PCS"]), 3, MidpointRounding.AwayFromZero);
            }
            else
            {
                ldweight = Convert.ToString(Convert.ToDecimal(weight));
            }

            //commandText.Append(" END ");
            //commandText.Append(" ELSE ");
            //commandText.Append(" BEGIN ");
            //commandText.Append(" SELECT  CAST(UNIT_RATE AS decimal (6,2)) FROM RETAILCUSTOMERSTONEAGGREEMENTDETAIL WHERE  ");
            //commandText.Append(" WAREHOUSE=@INVENTLOCATION AND ('" + weight.Trim() + "' BETWEEN FROM_WEIGHT AND TO_WEIGHT)  ");
            //commandText.Append(" AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN FROMDATE AND TODATE) ");
            //commandText.Append(" AND ITEMID='" + itemid.Trim() + "' AND INVENTBATCHID='" + batchid.Trim() + "' ");
            //commandText.Append(" AND INVENTSIZEID='" + sizeid.Trim() + "' AND INVENTCOLORID='" + colorid.Trim() + "' ");
            //commandText.Append(" END ");

            commandText.Append(" END ");
            commandText.Append(" ELSE IF(@METALTYPE IN ('" + (int)MetalType.LooseDmd + "','" + (int)MetalType.Stone + "')) ");
            commandText.Append(" BEGIN ");

            commandText.Append(" SELECT     CAST(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.UNIT_RATE AS numeric(26, 2))   ");
            commandText.Append(" FROM         RETAILCUSTOMERSTONEAGGREEMENTDETAIL INNER JOIN ");
            commandText.Append(" INVENTDIM ON RETAILCUSTOMERSTONEAGGREEMENTDETAIL.INVENTDIMID = INVENTDIM.INVENTDIMID ");
            commandText.Append(" WHERE     (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE = @INVENTLOCATION) AND ('" + ldweight.Trim() + "' BETWEEN RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FROM_WEIGHT AND  ");
            commandText.Append("  RETAILCUSTOMERSTONEAGGREEMENTDETAIL.TO_WEIGHT) AND (DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0) BETWEEN  ");
            commandText.Append(" RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ToDate) AND  ");
            commandText.Append("  (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID = '" + itemid.Trim() + "') AND   "); //(INVENTDIM.INVENTBATCHID = '" + batchid.Trim() + "') AND
            //commandText.Append("  (INVENTDIM.INVENTSIZEID = '" + sizeid.Trim() + "') AND (INVENTDIM.INVENTCOLORID = '" + colorid.Trim() + "') ");
            commandText.Append("  (INVENTDIM.INVENTSIZEID = '" + sizeid + "') AND (INVENTDIM.INVENTCOLORID = '" + colorid + "') "); //22.01.2014 -- off trim
            // commandText.Append(" AND (INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "')");
            commandText.Append(" AND (INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "') AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ACTIVATE = 1"); // modified on 02.09.2013

            commandText.Append(" END ");
            commandText.Append(" ELSE ");
            commandText.Append(" BEGIN ");
            commandText.Append(" SELECT     CAST(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.UNIT_RATE AS numeric(26, 2))   ");
            commandText.Append(" FROM         RETAILCUSTOMERSTONEAGGREEMENTDETAIL INNER JOIN ");
            commandText.Append(" INVENTDIM ON RETAILCUSTOMERSTONEAGGREEMENTDETAIL.INVENTDIMID = INVENTDIM.INVENTDIMID ");
            commandText.Append(" WHERE     (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE = @INVENTLOCATION) AND ('" + weight.Trim() + "' BETWEEN RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FROM_WEIGHT AND  ");
            commandText.Append("  RETAILCUSTOMERSTONEAGGREEMENTDETAIL.TO_WEIGHT) AND (DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0) BETWEEN  ");
            commandText.Append(" RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ToDate) AND  ");
            commandText.Append("  (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID = '" + itemid.Trim() + "') AND (INVENTDIM.INVENTBATCHID = '" + batchid.Trim() + "') AND  ");
            //commandText.Append("  (INVENTDIM.INVENTSIZEID = '" + sizeid.Trim() + "') AND (INVENTDIM.INVENTCOLORID = '" + colorid.Trim() + "') ");
            commandText.Append("  (INVENTDIM.INVENTSIZEID = '" + sizeid + "') AND (INVENTDIM.INVENTCOLORID = '" + colorid + "') "); //22.01.2014 -- off trim
            commandText.Append(" AND (INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "') AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ACTIVATE = 1"); // modified on 02.09.2013

            commandText.Append(" END ");
            //   }

            //    SELECT CONVERT(DATETIME,SUBSTRING(CONVERT(VARCHAR(10),DATEADD(dd,DATEDIFF(dd,0,CAST(TRANSDATE AS VARCHAR)),0),120) + ' ' +
            //    CAST(CAST(cast(([TIME] / 3600) as varchar(10)) + ':' + cast(([TIME] % 60) as varchar(10)) AS TIME) AS VARCHAR),0,24),121)  FROM METALRATES

            SqlConnection conn = new SqlConnection();
            if(application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;


            if(conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if(conn.State == ConnectionState.Open)
                conn.Close();
            if(!string.IsNullOrEmpty(sResult))
                return Convert.ToString(objRounding.Round(Convert.ToDecimal(sResult.Trim())));
            else
                return string.Empty;

        }
        #endregion

        #region  Get Ingredient Calculation Type

        private string GetIngredientCalcType(string ItemID, string configuration, string batchid, string ColorID, string SizeID, string GrWeight, string pcs = null)  // added
        {
            StringBuilder commandText = new StringBuilder();

            string grweigh = string.Empty;
            if(string.IsNullOrEmpty(GrWeight))
                grweigh = "0";
            else
                grweigh = GrWeight;

            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) ");
            commandText.Append(" DECLARE @METALTYPE INT ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM         RETAILCHANNELTABLE INNER JOIN  ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + ApplicationSettings.Database.StoreID.Trim() + "'  ");
            commandText.Append(" SELECT @METALTYPE=METALTYPE FROM [INVENTTABLE] WHERE ITEMID='" + ItemID.Trim() + "' ");

            commandText.Append(" IF(@METALTYPE IN ('" + (int)MetalType.LooseDmd + "','" + (int)MetalType.Stone + "')) ");
            commandText.Append(" BEGIN ");

            // commandText.Append(" SELECT     CAST(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.UNIT_RATE AS numeric(28, 2))   ");
            commandText.Append(" SELECT     ISNULL(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.CALCTYPE,0) AS CALCTYPE ");
            commandText.Append(" FROM         RETAILCUSTOMERSTONEAGGREEMENTDETAIL INNER JOIN ");
            commandText.Append(" INVENTDIM ON RETAILCUSTOMERSTONEAGGREEMENTDETAIL.INVENTDIMID = INVENTDIM.INVENTDIMID ");
            // commandText.Append(" WHERE     (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE = @INVENTLOCATION) AND ('" + grweigh + "' BETWEEN RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FROM_WEIGHT AND  ");
            commandText.Append(" WHERE     (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE = @INVENTLOCATION) AND (  ");
            commandText.Append(dStoneWtRange); //COs  Modification
            commandText.Append(" BETWEEN RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FROM_WEIGHT AND  ");

            commandText.Append("  RETAILCUSTOMERSTONEAGGREEMENTDETAIL.TO_WEIGHT) AND (DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0) BETWEEN  ");
            commandText.Append(" RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ToDate) AND  ");
            //  commandText.Append("  (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID = '" + ItemID.Trim() + "') AND (INVENTDIM.INVENTBATCHID = '" + BatchID.Trim() + "') AND  ");
            commandText.Append("  (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID = '" + ItemID.Trim() + "')  AND  ");
            commandText.Append("  (INVENTDIM.INVENTSIZEID = '" + SizeID.Trim() + "') AND (INVENTDIM.INVENTCOLORID = '" + ColorID.Trim() + "') ");

            commandText.Append(" AND (INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "') AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ACTIVATE = 1");

            commandText.Append(" ORDER BY RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID DESC, RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate DESC");  // Changed order sequence on 03.06.2013 as per u.das
            commandText.Append(" END ");
            // }

            //   commandText.Append("AND CAST(cast(([TIME] / 3600) as varchar(10)) + ':' + cast(([TIME] % 60) as varchar(10)) AS TIME)<=CAST(CONVERT(VARCHAR(8),GETDATE(),108) AS TIME)  ");

            SqlConnection conn = new SqlConnection();
            if(application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

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

        #region ADD CLICK
        private void btnAddItem_Click(object sender, EventArgs e)
        {
            string sUniqueNo = string.Empty;
            if(isValied())
            {

                DataRow dr;
                if(IsEdit == false && dtItemInfo != null && dtItemInfo.Rows.Count == 0 && dtItemInfo.Columns.Count == 0)
                {
                    IsEdit = false;
                    dtItemInfo.Columns.Add("UNIQUEID", typeof(string));
                    dtItemInfo.Columns.Add("ITEMID", typeof(string));
                    dtItemInfo.Columns.Add("ITEMNAME", typeof(string));
                    dtItemInfo.Columns.Add("CONFIGURATION", typeof(string));
                    dtItemInfo.Columns.Add("COLOR", typeof(string));
                    dtItemInfo.Columns.Add("SIZE", typeof(string));
                    dtItemInfo.Columns.Add("STYLE", typeof(string));
                    dtItemInfo.Columns.Add("PCS", typeof(decimal));
                    dtItemInfo.Columns.Add("QUANTITY", typeof(decimal));
                    dtItemInfo.Columns.Add("RATE", typeof(decimal));
                    dtItemInfo.Columns.Add("RATETYPE", typeof(string));
                    dtItemInfo.Columns.Add("MAKINGRATE", typeof(decimal));
                    dtItemInfo.Columns.Add("MAKINGTYPE", typeof(string));
                    dtItemInfo.Columns.Add("AMOUNT", typeof(decimal));
                    dtItemInfo.Columns.Add("MAKINGAMOUNT", typeof(decimal));
                    dtItemInfo.Columns.Add("EXTENDEDDETAILS", typeof(decimal));
                    dtItemInfo.Columns.Add("TOTALAMOUNT", typeof(decimal));
                    dtItemInfo.Columns.Add("ROWTOTALAMOUNT", typeof(decimal));
                    dtItemInfo.Columns.Add("INVENTDIMID", typeof(string));
                    dtItemInfo.Columns.Add("DIMENSIONS", typeof(string));

                    //Added on 15-01-2012
                    dtItemInfo.Columns.Add("UNITID", typeof(string));

                    dtItemInfo.Columns.Add("WastageRate", typeof(decimal));
                    dtItemInfo.Columns.Add("WastageType", typeof(string));
                    dtItemInfo.Columns.Add("WastageQty", typeof(decimal));
                    dtItemInfo.Columns.Add("WastagePercentage", typeof(decimal));
                    dtItemInfo.Columns.Add("WastageAmount", typeof(decimal));

                    dtItemInfo.Columns.Add("IsBookedSKU", typeof(bool));
                    dtItemInfo.Columns.Add("RemarksDtl", typeof(string)); // 

                }
                if(IsEdit == false)
                {
                    dr = dtItemInfo.NewRow();
                    dr["UNIQUEID"] = sUniqueNo = Convert.ToString(randUnique.Next());
                    dr["ITEMID"] = Convert.ToString(txtItemId.Text.Trim());
                    dr["ITEMNAME"] = Convert.ToString(txtItemName.Text.Trim());
                    dr["CONFIGURATION"] = Convert.ToString(cmbConfig.Text.Trim());
                    dr["COLOR"] = Convert.ToString(cmbCode.Text.Trim());
                    dr["STYLE"] = Convert.ToString(cmbStyle.Text.Trim());
                    dr["SIZE"] = Convert.ToString(cmbSize.Text.Trim());
                    if(!string.IsNullOrEmpty(txtPCS.Text.Trim()))
                        dr["PCS"] = Convert.ToDecimal(txtPCS.Text.Trim());
                    else
                        dr["PCS"] = DBNull.Value;
                    if(!string.IsNullOrEmpty(txtQuantity.Text.Trim()))
                        dr["QUANTITY"] = decimal.Round(Convert.ToDecimal(txtQuantity.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                    else
                        dr["QUANTITY"] = DBNull.Value;

                    if(!string.IsNullOrEmpty(txtRate.Text.Trim()))
                        dr["RATE"] = decimal.Round(Convert.ToDecimal(txtRate.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                    else
                        dr["RATE"] = DBNull.Value;

                    if(!string.IsNullOrEmpty(cmbRateType.Text.Trim()))
                        dr["RATETYPE"] = Convert.ToString(cmbRateType.Text.Trim());
                    else
                        dr["RATETYPE"] = DBNull.Value;

                    if(!string.IsNullOrEmpty(txtMakingRate.Text.Trim()))
                        dr["MAKINGRATE"] = decimal.Round(Convert.ToDecimal(txtMakingRate.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                    else
                        dr["MAKINGRATE"] = DBNull.Value;

                    if(!string.IsNullOrEmpty(cmbMakingType.Text.Trim()))
                        dr["MAKINGTYPE"] = Convert.ToString(cmbMakingType.Text.Trim());
                    else
                        dr["MAKINGTYPE"] = DBNull.Value;

                    if(!string.IsNullOrEmpty(txtAmount.Text.Trim()))
                        dr["AMOUNT"] = decimal.Round(Convert.ToDecimal(txtAmount.Text.Trim()), 2, MidpointRounding.AwayFromZero);
                    else
                        dr["AMOUNT"] = DBNull.Value;

                    if(!string.IsNullOrEmpty(txtMakingAmount.Text.Trim()))
                        dr["MAKINGAMOUNT"] = decimal.Round(Convert.ToDecimal(txtMakingAmount.Text.Trim()), 2, MidpointRounding.AwayFromZero);
                    else
                        dr["MAKINGAMOUNT"] = DBNull.Value;

                    if(!string.IsNullOrEmpty(txtMakingAmount.Text.Trim()))
                        dr["ROWTOTALAMOUNT"] = decimal.Round((Convert.ToDecimal(txtAmount.Text.Trim()) + Convert.ToDecimal(txtMakingAmount.Text.Trim())), 2, MidpointRounding.AwayFromZero);
                    else
                        dr["ROWTOTALAMOUNT"] = decimal.Round((Convert.ToDecimal(txtAmount.Text.Trim())), 2, MidpointRounding.AwayFromZero);



                    if(!string.IsNullOrEmpty(txtTotalAmount.Text.Trim()))
                        dr["TOTALAMOUNT"] = decimal.Round(Convert.ToDecimal(txtTotalAmount.Text.Trim()), 2, MidpointRounding.AwayFromZero);
                    else
                        dr["TOTALAMOUNT"] = DBNull.Value;

                    dr["INVENTDIMID"] = string.IsNullOrEmpty(inventDimId) ? string.Empty : inventDimId;
                    dr["UNITID"] = string.IsNullOrEmpty(unitid) ? string.Empty : unitid;

                    dr["DIMENSIONS"] = Previewdimensions;

                    dr["WastageRate"] = 0;
                    if(!string.IsNullOrEmpty(cmbWastage.Text.Trim()))
                    {
                        dr["WastageType"] = Convert.ToString(cmbWastage.Text.Trim());
                    }
                    else
                    {
                        dr["WastageType"] = "Weight";
                    }

                    dr["WastageQty"] = 0;
                    dr["WastagePercentage"] = 0;
                    dr["WastageAmount"] = 0;


                    dr["IsBookedSKU"] = (chkBookedSKU.Checked) ? 1 : 0;
                    dr["RemarksDtl"] = txtRemarks.Text.Trim();

                    dtItemInfo.Rows.Add(dr);

                    grItems.DataSource = dtItemInfo.DefaultView;
                }
                if(IsEdit == true)
                {
                    decimal dWastageRowAmt = 0m;
                    DataRow EditRow = dtItemInfo.Rows[EditselectedIndex];
                    EditRow["PCS"] = string.IsNullOrEmpty(txtPCS.Text.Trim()) ? "0" : txtPCS.Text.Trim();
                    sUniqueNo = Convert.ToString(EditRow["UNIQUEID"]);
                    EditRow["QUANTITY"] = txtQuantity.Text.Trim();
                    EditRow["RATETYPE"] = cmbRateType.Text.Trim();
                    EditRow["RATE"] = txtRate.Text.Trim();
                    EditRow["AMOUNT"] = txtAmount.Text.Trim();
                    EditRow["RATETYPE"] = Convert.ToString(cmbRateType.Text.Trim());
                    EditRow["MAKINGRATE"] = decimal.Round(Convert.ToDecimal(string.IsNullOrEmpty(txtMakingRate.Text.Trim()) ? "0" : txtMakingRate.Text.Trim()), 2, MidpointRounding.AwayFromZero);
                    EditRow["MAKINGTYPE"] = Convert.ToString(cmbMakingType.Text.Trim());
                    EditRow["MAKINGAMOUNT"] = decimal.Round(Convert.ToDecimal(string.IsNullOrEmpty(txtMakingAmount.Text.Trim()) ? "0" : txtMakingAmount.Text.Trim()), 2, MidpointRounding.AwayFromZero);
                    EditRow["RemarksDtl"] = txtRemarks.Text.Trim();
                    if(!string.IsNullOrEmpty(Convert.ToString(txtWastageAmount.Text.Trim())))
                    {
                        dWastageRowAmt = Convert.ToDecimal(txtWastageAmount.Text.Trim());
                    }

                    EditRow["ROWTOTALAMOUNT"] = Convert.ToDecimal(string.IsNullOrEmpty(txtAmount.Text.Trim()) ? "0" : txtAmount.Text.Trim()) + Convert.ToDecimal(string.IsNullOrEmpty(txtMakingAmount.Text.Trim()) ? "0" : txtMakingAmount.Text.Trim()) + dWastageRowAmt;

                    dtItemInfo.AcceptChanges();

                    grItems.DataSource = dtItemInfo.DefaultView;
                }

                #region FOR FILLING INGREDIENT DETAILS
                //if (!IsEdit)
                //{
                if(IsEdit)
                {
                    /*
                    if (dtSubOrderDetails != null && dtSubOrderDetails.Rows.Count > 0)
                    {
                        foreach (DataRow dr1 in dtSubOrderDetails.Rows)
                        {
                            if (dr1["UNIQUEID"].ToString().Trim() == sUniqueNo.Trim())
                            {
                                dr1.Delete();
                                dtSubOrderDetails.AcceptChanges();
                                if (dtSubOrderDetails.Rows.Count == 0)
                                {
                                    if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
                                    {
                                        foreach (DataRow drTotal in dtItemInfo.Rows)
                                        {
                                            // drTotal["ROWTOTALAMOUNT"] = Math.Round((Convert.ToDecimal(drTotal["AMOUNT"]) + Convert.ToDecimal(drTotal["MAKINGAMOUNT"])), 2);
                                            drTotal["ROWTOTALAMOUNT"] = Math.Round((Convert.ToDecimal(drTotal["AMOUNT"]) + Convert.ToDecimal(drTotal["MAKINGAMOUNT"]) + Convert.ToDecimal(drTotal["WastageAmount"])), 2); // Added for wastage 
                                        }
                                        dtItemInfo.AcceptChanges();
                                    }
                                    break;
                                }
                            }
                        }
                        dtSubOrderDetails.AcceptChanges();
                    }

                    */

                    if(dtSubOrderDetails != null && dtSubOrderDetails.Rows.Count > 0)
                    {
                        foreach(DataRow dr1 in dtSubOrderDetails.Rows)
                        {
                            if(dr1["UNIQUEID"].ToString().Trim() == sUniqueNo.Trim())
                            {
                                dr1.Delete();

                            }
                            //if (dtSubOrderDetails.Rows.Count == 0)
                            //{
                            //    if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
                            //    {
                            //        foreach (DataRow drTotal in dtItemInfo.Rows)
                            //        {
                            //            // drTotal["ROWTOTALAMOUNT"] = Math.Round((Convert.ToDecimal(drTotal["AMOUNT"]) + Convert.ToDecimal(drTotal["MAKINGAMOUNT"])), 2);
                            //            drTotal["ROWTOTALAMOUNT"] = Math.Round((Convert.ToDecimal(drTotal["AMOUNT"]) + Convert.ToDecimal(drTotal["MAKINGAMOUNT"]) + Convert.ToDecimal(drTotal["WastageAmount"])), 2); // Added for wastage 
                            //        }
                            //        dtItemInfo.AcceptChanges();
                            //    }
                            //    break;
                            //}


                            // }

                        }
                        dtSubOrderDetails.AcceptChanges();
                    }



                    IsEdit = false;
                }

                DataTable dt;

                if(IsSKUItem(txtItemId.Text.Trim()))
                {
                    dt = GetIngredientDetails(sUniqueNo, true);
                }
                else
                {
                    dt = GetIngredientDetails(sUniqueNo);
                }

                decimal eAmt = 0;
                string UID = string.Empty;
                if(dtSubOrderDetails != null && dtSubOrderDetails.Rows.Count > 0)
                {
                    if(dt != null && dt.Rows.Count > 0)
                    {
                        foreach(DataRow drIngredients in dt.Rows)
                        {
                            UID = Convert.ToString(drIngredients["UNIQUEID"]);
                            eAmt += Convert.ToDecimal(drIngredients["AMOUNT"]);
                            dtSubOrderDetails.ImportRow(drIngredients);
                        }
                    }
                }
                else
                {
                    if(dt != null && dt.Rows.Count > 0)
                    {
                        foreach(DataRow drIngredients in dt.Rows)
                        {
                            UID = Convert.ToString(drIngredients["UNIQUEID"]);
                            eAmt += decimal.Round(Convert.ToDecimal(drIngredients["AMOUNT"]), 2, MidpointRounding.AwayFromZero);
                        }
                    }

                    dtSubOrderDetails = dt;
                }

                #endregion

                if(dt != null && dt.Rows.Count > 0)
                {
                    dIngrdMetalWtRange = Convert.ToDecimal(dt.Rows[0]["QUANTITY"]);

                    foreach(DataRow drIngrd in dt.Rows)
                    {
                        int iMetalType = 0;
                        iMetalType = Convert.ToInt32(drIngrd["METALTYPE"]);

                        //if (iMetalType == (int)MetalType.Gold // As discussed with AM
                        //                   || iMetalType == (int)MetalType.Silver
                        //                   || iMetalType == (int)MetalType.Platinum
                        //                   || iMetalType == (int)MetalType.Palladium)
                        //{
                        //    dIngrdMetalWtRange += Convert.ToDecimal(drIngrd["QUANTITY"]);

                        //}

                        if(iMetalType == (int)MetalType.Gold)
                        {
                            dIngrdTotalGoldQty += Convert.ToDecimal(drIngrd["QUANTITY"]);
                            dIngrdTotalGoldValue += Convert.ToDecimal(drIngrd["AMOUNT"]);

                            dWMetalRate = getWastageMetalRate(pos.StoreId,
                                            Convert.ToString(drIngrd["ITEMID"]), Convert.ToString(drIngrd["CONFIGURATION"]));// Added for wastage // not consider -- Multi Ingredient gold
                        }
                    }
                }

                // if (dIngrdTotalGoldQty > 0) // blocked on 05.08.2013

                SqlConnection conn = new SqlConnection();
                if(application != null)
                    conn = application.Settings.Database.Connection;
                else
                    conn = ApplicationSettings.Database.LocalConnection;

                //  dWMetalRate = getWastageMetalRate(pos.StoreId, sBaseItemID, sBaseConfigID);// Added for wastage

                CheckMakingRateFromDB(conn, pos.StoreId, txtItemId.Text);

                #region [ Wastage Amount Calculation] // for wastage

                if(cmbWastage.SelectedIndex == 0 && dWastQty > 0)
                {

                    Decimal dAmount = 0m;
                    int iMetalType = 0;

                    if(dWMetalRate > 0)
                    {
                        txtWastageQty.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(dWastQty), 3, MidpointRounding.AwayFromZero));
                        dAmount = dWMetalRate * dWastQty;

                        txtWastageAmount.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(dAmount), 2, MidpointRounding.AwayFromZero));
                    }
                    else
                    {
                        iMetalType = GetMetalType(sBaseItemID);
                        if(iMetalType == (int)MetalType.Gold)
                        {
                            dWMetalRate = getWastageMetalRate(pos.StoreId, sBaseItemID, sBaseConfigID);
                            if(dWMetalRate > 0)
                            {
                                txtWastageQty.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(dWastQty), 3, MidpointRounding.AwayFromZero));
                                dAmount = dWMetalRate * dWastQty;

                                txtWastageAmount.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(dAmount), 2, MidpointRounding.AwayFromZero));
                            }
                        }
                    }

                    // }

                }

                else if(cmbWastage.SelectedIndex == 1 && dWastQty > 0)
                {
                    if(dIngrdTotalGoldQty > 0 && dWMetalRate > 0)
                    {
                        // Decimal dIngrdTotQty = 0m;
                        Decimal dWastageQty = 0m;
                        Decimal decimalAmount = 0m;

                        dWastageQty = decimal.Round((dIngrdTotalGoldQty * (dWastQty / 100)), 3, MidpointRounding.AwayFromZero); //Changes on 11/04/2014 R.Hossain
                        decimalAmount = dWMetalRate * dWastageQty;

                        txtWastagePercentage.Text = Convert.ToString(dWastQty);

                        txtWastageQty.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(dWastageQty), 3, MidpointRounding.AwayFromZero));

                        txtWastageAmount.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(decimalAmount), 2, MidpointRounding.AwayFromZero));
                    }
                    else
                    {
                        Decimal dWastageQty = 0m;
                        Decimal decimalAmount = 0m;
                        if(GetMetalType(sBaseItemID) == (int)MetalType.Gold)
                        {
                            dWMetalRate = getWastageMetalRate(pos.StoreId, sBaseItemID, sBaseConfigID);
                            if(dWMetalRate > 0)
                            {
                                dWastageQty = decimal.Round((Convert.ToDecimal(txtQuantity.Text) * (dWastQty / 100)), 3, MidpointRounding.AwayFromZero);//Changes on 11/04/2014 R.Hossain
                                decimalAmount = dWMetalRate * dWastageQty;

                                txtWastagePercentage.Text = Convert.ToString(dWastQty);
                                txtWastageQty.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(dWastageQty), 3, MidpointRounding.AwayFromZero));
                                txtWastageAmount.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(decimalAmount), 2, MidpointRounding.AwayFromZero));
                            }
                        }

                    }

                }
                else
                {
                    txtWastagePercentage.Text = string.Empty;
                    txtWastageQty.Text = string.Empty;
                    txtWastageAmount.Text = string.Empty;
                }


                #endregion

                if(eAmt != 0)
                {
                    foreach(DataRow dr1 in dtItemInfo.Select("UNIQUEID='" + UID.Trim() + "'"))
                    {
                        //if (!IsEdit) //COs  Modification
                        //  {
                        // if (dt != null && dt.Rows.Count > 0) // --check   

                        if(!string.IsNullOrEmpty(cmbMakingType.Text.Trim()))
                        {
                            dr1["MAKINGTYPE"] = Convert.ToString(cmbMakingType.Text.Trim());
                        }
                        if(!string.IsNullOrEmpty(txtMakingRate.Text.Trim()))
                            dr1["MAKINGRATE"] = decimal.Round(Convert.ToDecimal(txtMakingRate.Text.Trim()), 2, MidpointRounding.AwayFromZero);
                        else
                            dr1["MAKINGRATE"] = 0m;

                        if(!string.IsNullOrEmpty(txtMakingAmount.Text.Trim()))
                            dr1["MAKINGAMOUNT"] = decimal.Round(Convert.ToDecimal(txtMakingAmount.Text.Trim()), 2, MidpointRounding.AwayFromZero);
                        else
                            dr1["MAKINGAMOUNT"] = 0m;

                        // Added for wastage  
                        dr1["WastageRate"] = dWMetalRate;

                        if(!string.IsNullOrEmpty(cmbWastage.Text.Trim()))
                        {
                            dr1["WastageType"] = Convert.ToString(cmbWastage.Text.Trim());
                        }

                        if(Convert.ToString(txtWastageQty.Text) != string.Empty)
                            dr1["WastageQty"] = Convert.ToDecimal(txtWastageQty.Text);
                        else
                            dr1["WastageQty"] = 0m;

                        if(Convert.ToString(txtWastagePercentage.Text) != string.Empty)
                            dr1["WastagePercentage"] = Convert.ToDecimal(txtWastagePercentage.Text);
                        else
                            dr1["WastagePercentage"] = 0m;

                        if(Convert.ToString(txtWastageAmount.Text) != string.Empty)
                            dr1["WastageAmount"] = Convert.ToDecimal(txtWastageAmount.Text);
                        else
                            dr1["WastageAmount"] = 0m;

                        dr1["EXTENDEDDETAILS"] = eAmt;

                        dr1["ROWTOTALAMOUNT"] = eAmt + Convert.ToDecimal(dr1["MAKINGAMOUNT"]) + Convert.ToDecimal(dr1["WastageAmount"]);

                        //  dr1["AMOUNT"] = Convert.ToString(dt.Rows[0]["AMOUNT"]);
                        //  dr1["RATE"] = Convert.ToDecimal(Convert.ToString(dt.Rows[0]["AMOUNT"]));
                        dr1["AMOUNT"] = eAmt;
                        dr1["RATE"] = eAmt;
                        dtItemInfo.AcceptChanges();
                        break;
                    }
                    grItems.DataSource = dtItemInfo.DefaultView;
                }

                if(dtItemInfo != null && dtItemInfo.Rows.Count > 0)
                {
                    Decimal dTotalAmount = 0m;
                    foreach(DataRow drTotal in dtItemInfo.Rows)
                    {
                        dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["ROWTOTALAMOUNT"])) ? Convert.ToDecimal(drTotal["ROWTOTALAMOUNT"]) : 0);
                    }
                    txtTotalAmount.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(dTotalAmount), 2, MidpointRounding.AwayFromZero));
                }

                ClearControls();

                dIngrdMetalWtRange = 0m;
                dIngrdTotalGoldQty = 0m;
                dIngrdTotalGoldValue = 0m;

                dWastQty = 0m;
                dWMetalRate = 0m;
            }


        }
        #endregion

        #region Extended Details CLick
        private void btnExtendedDetails_Click(object sender, EventArgs e)
        {
            BlankOperations.WinFormsTouch.frmSubOrderDetails objSubOrderdetails = null;
            if(dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                if(grdView.RowCount > 0)
                {
                    int selectedRow = grdView.GetSelectedRows()[0];
                    DataRow drRowToUpdate = dtItemInfo.Rows[selectedRow];
                    sExtendedDetailsAmount = 0m;


                    if(dtSubOrderDetails != null && dtSubOrderDetails.Rows.Count > 0)
                    {
                        objSubOrderdetails = new BlankOperations.WinFormsTouch.frmSubOrderDetails(dtSubOrderDetails, pos, application, this, Convert.ToString(drRowToUpdate["UNIQUEID"]));
                    }
                    else
                    {
                        dtSubOrderDetails = new DataTable();
                        objSubOrderdetails = new BlankOperations.WinFormsTouch.frmSubOrderDetails(pos, application, this, Convert.ToString(drRowToUpdate["UNIQUEID"]));
                    }
                    objSubOrderdetails.ShowDialog();

                    decimal dMkAmt = 0;
                    decimal dWastAmt = 0;
                    if(!string.IsNullOrEmpty(Convert.ToString(drRowToUpdate["MAKINGAMOUNT"])))
                        dMkAmt = Convert.ToDecimal(drRowToUpdate["MAKINGAMOUNT"]);

                    if(!string.IsNullOrEmpty(Convert.ToString(drRowToUpdate["WastageAmount"])))
                        dWastAmt = Convert.ToDecimal(drRowToUpdate["WastageAmount"]);

                    if(sExtendedDetailsAmount != 0)
                    {
                        drRowToUpdate["AMOUNT"] = sExtendedDetailsAmount;
                        drRowToUpdate["RATE"] = sExtendedDetailsAmount;
                        // drRowToUpdate["ROWTOTALAMOUNT"] = sExtendedDetailsAmount + Convert.ToDecimal(drRowToUpdate["MAKINGAMOUNT"]);


                        drRowToUpdate["ROWTOTALAMOUNT"] = sExtendedDetailsAmount + dMkAmt + dWastAmt;
                    }
                    else
                    {
                        drRowToUpdate["ROWTOTALAMOUNT"] = Convert.ToDecimal(drRowToUpdate["AMOUNT"]) + +dMkAmt + dWastAmt;
                    }
                    drRowToUpdate["EXTENDEDDETAILS"] = sExtendedDetailsAmount;
                    //  drRowToUpdate["ROWTOTALAMOUNT"] = Convert.ToDecimal(drRowToUpdate["EXTENDEDDETAILS"]) + Convert.ToDecimal(drRowToUpdate["MAKINGAMOUNT"]);

                    //   }
                    dtItemInfo.AcceptChanges();
                    grItems.DataSource = null;
                    grItems.DataSource = dtItemInfo;
                    if(dtItemInfo != null && dtItemInfo.Rows.Count > 0)
                    {
                        Decimal dTotalAmount = 0m;
                        foreach(DataRow drTotal in dtItemInfo.Rows)
                        {
                            //  dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["AMOUNT"])) ? Convert.ToDecimal(drTotal["AMOUNT"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["EXTENDEDDETAILS"])) ? Convert.ToDecimal(drTotal["EXTENDEDDETAILS"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["MAKINGAMOUNT"])) ? Convert.ToDecimal(drTotal["MAKINGAMOUNT"]) : 0);
                            dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["ROWTOTALAMOUNT"])) ? Convert.ToDecimal(drTotal["ROWTOTALAMOUNT"]) : 0);
                        }
                        txtTotalAmount.Text = Convert.ToString(objRounding.Round(dTotalAmount, 2));
                    }

                }
            }
            else if(dsSearchedOrder != null && dsSearchedOrder.Tables.Count > 0 && dsSearchedOrder.Tables[2].Rows.Count > 0)
            {
                if(grdView.RowCount > 0)
                {
                    int selectedRow = grdView.GetSelectedRows()[0];
                    DataRow drRowToUpdate = dsSearchedOrder.Tables[1].Rows[selectedRow];
                    sExtendedDetailsAmount = 0m;
                    objSubOrderdetails = new BlankOperations.WinFormsTouch.frmSubOrderDetails(dsSearchedOrder, pos, application, this, Convert.ToString(drRowToUpdate["LINENUM"]));
                    objSubOrderdetails.ShowDialog();

                }

            }
            else
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please enter at least one row to enter the details or No Ingredients Exists.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);

                }
            }
        }
        #endregion

        #region PCS TEXT CHANGED
        private void txtPCS_TextChanged(object sender, EventArgs e)
        {
            if(isItemExists)
            {
                if(!string.IsNullOrEmpty(txtPCS.Text) && !string.IsNullOrEmpty(txtQuantity.Text))
                {
                    if(!string.IsNullOrEmpty(PreviousPcs))
                        txtQuantity.Text = Convert.ToString(Convert.ToDecimal(txtQuantity.Text) / Convert.ToDecimal(PreviousPcs));
                    txtQuantity.Text = Convert.ToString(Convert.ToDecimal(txtPCS.Text) * Convert.ToDecimal(txtQuantity.Text));
                    //if (!string.IsNullOrEmpty(txtMakingRate.Text))
                    //{
                    //    if (!string.IsNullOrEmpty(PreviousPcs))
                    //        txtMakingRate.Text = Convert.ToString(Convert.ToDecimal(txtMakingRate.Text) / Convert.ToDecimal(PreviousPcs));
                    //    txtMakingRate.Text = Convert.ToString(Convert.ToDecimal(txtPCS.Text) * Convert.ToDecimal(txtMakingRate.Text));
                    //}
                }
                if(string.IsNullOrEmpty(txtPCS.Text))
                {
                    txtQuantity.Text = string.Empty;
                    txtMakingRate.Text = string.Empty;
                }
                cmbRateType_SelectedIndexChanged(sender, e);
                cmbMakingType_SelectedIndexChanged(sender, e);
                PreviousPcs = txtPCS.Text;
            }
        }
        #endregion

        #region QUANTITY TEXT CHANGED
        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            if(isItemExists)
            {
                SqlConnection conn = new SqlConnection();
                if(application != null)
                    conn = application.Settings.Database.Connection;
                else
                    conn = ApplicationSettings.Database.LocalConnection;
                txtRate.Text = getRateFromMetalTable(txtItemId.Text, cmbConfig.Text, cmbStyle.Text, cmbCode.Text, cmbSize.Text, txtQuantity.Text);
                CheckMakingRateFromDB(conn, pos.StoreId, txtItemId.Text);
                if(string.IsNullOrEmpty(txtQuantity.Text))
                    txtMakingRate.Text = string.Empty;
                cmbRateType_SelectedIndexChanged(sender, e);
                cmbMakingType_SelectedIndexChanged(sender, e);
            }
        }
        #endregion

        #region RATE TEXT CHANGED
        private void txtRate_TextChanged(object sender, EventArgs e)
        {
            cmbRateType_SelectedIndexChanged(sender, e);
            cmbMakingType_SelectedIndexChanged(sender, e);
        }
        #endregion

        #region RATE TYPE SELECTED INDEX CHANGED
        private void cmbRateType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(txtRate.Text.Trim()) && cmbRateType.SelectedIndex == 0 && !string.IsNullOrEmpty(txtQuantity.Text.Trim()))
            {
                Decimal decimalAmount = 0m;
                decimalAmount = objRounding.Round(Convert.ToDecimal(txtRate.Text.Trim()), 2) * objRounding.Round(Convert.ToDecimal(txtQuantity.Text.Trim()), 2);
                txtAmount.Text = Convert.ToString(objRounding.Round(decimalAmount, 2));
            }
            if(cmbRateType.SelectedIndex == 0 && (string.IsNullOrEmpty(txtRate.Text.Trim()) || string.IsNullOrEmpty(txtQuantity.Text.Trim())))
            {
                txtAmount.Text = string.Empty;
            }
            if(cmbRateType.SelectedIndex == 1 && !string.IsNullOrEmpty(txtRate.Text.Trim()) && !string.IsNullOrEmpty(txtPCS.Text.Trim()))
            {
                Decimal decimalAmount = 0m;
                decimalAmount = objRounding.Round(Convert.ToDecimal(txtRate.Text.Trim()), 2) * objRounding.Round(Convert.ToDecimal(txtPCS.Text.Trim()), 2);
                txtAmount.Text = Convert.ToString(objRounding.Round(decimalAmount, 2));
            }
            if(cmbRateType.SelectedIndex == 1 && (string.IsNullOrEmpty(txtRate.Text.Trim()) || string.IsNullOrEmpty(txtPCS.Text.Trim())))
            {
                txtAmount.Text = string.Empty;
            }
            if(cmbRateType.SelectedIndex == 2 && !string.IsNullOrEmpty(txtRate.Text.Trim().Trim()))
            {
                Decimal decimalAmount = 0m;
                decimalAmount = objRounding.Round(Convert.ToDecimal(txtRate.Text.Trim()), 2);
                txtAmount.Text = Convert.ToString(objRounding.Round(decimalAmount, 2));
            }
            if(string.IsNullOrEmpty(txtRate.Text.Trim()))
            {
                txtAmount.Text = string.Empty;
            }
        }
        #endregion

        #region MAKING RATE TEXT CHANGED
        private void txtMakingRate_TextChanged(object sender, EventArgs e)
        {
            cmbRateType_SelectedIndexChanged(sender, e);
            cmbMakingType_SelectedIndexChanged(sender, e);
        }
        #endregion

        #region MAKING TYPE SELECTED INDEX CHANGED
        private void cmbMakingType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(txtMakingRate.Text.Trim()) && cmbMakingType.SelectedIndex == 1 && !string.IsNullOrEmpty(txtQuantity.Text.Trim()))
            {
                Decimal decimalAmount = 0m;
                //decimalAmount = Convert.ToDecimal(txtMakingRate.Text.Trim()) * Convert.ToDecimal(txtQuantity.Text.Trim()); 
                if(dIngrdTotalGoldQty > 0)
                {
                    decimalAmount = Convert.ToDecimal(txtMakingRate.Text.Trim()) * dIngrdTotalGoldQty;
                }
                else
                {
                    decimalAmount = Convert.ToDecimal(txtMakingRate.Text.Trim()) * Convert.ToDecimal(txtQuantity.Text.Trim());
                }

                txtMakingAmount.Text = Convert.ToString(Math.Round(decimalAmount, 2));
            }
            if(cmbMakingType.SelectedIndex == 1 && (string.IsNullOrEmpty(txtMakingRate.Text.Trim()) || string.IsNullOrEmpty(txtQuantity.Text.Trim())))
            {
                txtMakingAmount.Text = string.Empty;
            }
            if(cmbMakingType.SelectedIndex == 0 && !string.IsNullOrEmpty(txtMakingRate.Text.Trim()) && !string.IsNullOrEmpty(txtPCS.Text.Trim()))
            {
                Decimal decimalAmount = 0m;
                decimalAmount = objRounding.Round(Convert.ToDecimal(txtMakingRate.Text.Trim()), 2) * objRounding.Round(Convert.ToDecimal(txtPCS.Text.Trim()), 0);
                txtMakingAmount.Text = Convert.ToString(Math.Round(decimalAmount, 2));
            }
            if(cmbMakingType.SelectedIndex == 0 && (string.IsNullOrEmpty(txtMakingRate.Text.Trim()) || string.IsNullOrEmpty(txtPCS.Text.Trim())))
            {
                txtMakingAmount.Text = string.Empty;
            }
            if(cmbMakingType.SelectedIndex == 2 && !string.IsNullOrEmpty(txtMakingRate.Text.Trim().Trim()))
            {
                Decimal decimalAmount = 0m;
                decimalAmount = objRounding.Round(Convert.ToDecimal(txtMakingRate.Text.Trim()), 2);
                txtMakingAmount.Text = Convert.ToString(Math.Round(decimalAmount, 2));
            }
            if(cmbMakingType.SelectedIndex == 3 && !string.IsNullOrEmpty(txtMakingRate.Text.Trim()) && !string.IsNullOrEmpty(txtAmount.Text.Trim()))
            {
                Decimal decimalAmount = 0m;

                if(dIngrdTotalGoldValue > 0)
                {
                    decimalAmount = objRounding.Round((Convert.ToDecimal(txtMakingRate.Text.Trim()) / 100) * dIngrdTotalGoldValue, 2);
                }
                else
                {
                    decimalAmount = objRounding.Round((Convert.ToDecimal(txtMakingRate.Text.Trim()) / 100) * (Convert.ToDecimal(txtAmount.Text.Trim())), 2);
                }

                txtMakingAmount.Text = Convert.ToString(Math.Round(decimalAmount, 2));
            }
            if(cmbMakingType.SelectedIndex == 3 && string.IsNullOrEmpty(txtAmount.Text.Trim()))
            {
                txtMakingAmount.Text = string.Empty;
            }
            if(string.IsNullOrEmpty(txtMakingRate.Text.Trim()))
            {
                txtMakingAmount.Text = string.Empty;
            }
        }
        #endregion

        #region Submmit Click
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            frmCustOrder.dtOrderDetails = dtItemInfo;
            frmCustOrder.dtSubOrderDetails = dtSubOrderDetails;
            frmCustOrder.dtSampleDetails = dtSample;
            frmCustOrder.sOrderDetailsAmount = txtTotalAmount.Text.Trim();
            frmCustOrder.sSubOrderDetailsAmount = Convert.ToString(sExtendedDetailsAmount);
            frmCustOrder.dtSketchDetails = dtSketch;
            frmCustOrder.dtSampleSketch = dtSampleSketch;
            frmCustOrder.dtRecvStoneDetails = dtStone;
            frmCustOrder.dtPaySchedule = dtPaySchedule;

            this.Close();
        }
        #endregion

        #region MAKING RATE TEXT CHANGED
        private void txtMakingRate_TextChanged_1(object sender, EventArgs e)
        {
            cmbRateType_SelectedIndexChanged(sender, e);
            cmbMakingType_SelectedIndexChanged(sender, e);
        }
        #endregion

        #region EDIT CLICK
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
                    cmbStyle.Text = Convert.ToString(theRowToSelect["STYLE"]);
                    cmbCode.Text = Convert.ToString(theRowToSelect["COLOR"]);
                    cmbSize.Text = Convert.ToString(theRowToSelect["SIZE"]);
                    cmbConfig.Text = Convert.ToString(theRowToSelect["CONFIGURATION"]);
                    txtItemId.Text = Convert.ToString(theRowToSelect["ITEMID"]);
                    txtItemName.Text = Convert.ToString(theRowToSelect["ITEMNAME"]);
                    txtPCS.Text = Convert.ToString(theRowToSelect["PCS"]);
                    txtQuantity.Text = Convert.ToString(theRowToSelect["QUANTITY"]);
                    txtRate.Text = Convert.ToString(theRowToSelect["RATE"]);
                    cmbRateType.SelectedIndex = cmbRateType.FindString(Convert.ToString(theRowToSelect["RATETYPE"]).Trim());
                    txtMakingRate.Text = Convert.ToString(theRowToSelect["MAKINGRATE"]);
                    cmbMakingType.SelectedIndex = cmbMakingType.FindString(Convert.ToString(theRowToSelect["MAKINGTYPE"]).Trim());
                    txtAmount.Text = Convert.ToString(theRowToSelect["AMOUNT"]);
                    txtMakingAmount.Text = Convert.ToString(theRowToSelect["MAKINGAMOUNT"]);

                    cmbWastage.SelectedIndex = cmbWastage.FindString(Convert.ToString(theRowToSelect["WastageType"]).Trim());
                    txtWastagePercentage.Text = Convert.ToString(theRowToSelect["WastagePercentage"]);
                    txtWastageQty.Text = Convert.ToString(theRowToSelect["WastageQty"]);
                    txtWastageAmount.Text = Convert.ToString(theRowToSelect["WastageAmount"]);

                    chkBookedSKU.Checked = Convert.ToBoolean(theRowToSelect["IsBookedSKU"]);

                    chkBookedSKU.Enabled = false;

                    txtRemarks.Text = Convert.ToString(theRowToSelect["RemarksDtl"]);


                    if(dtItemInfo != null && dtItemInfo.Rows.Count > 0)
                    {
                        Decimal dTotalAmount = 0m;
                        foreach(DataRow drTotal in dtItemInfo.Rows)
                        {
                            dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["AMOUNT"])) ? Convert.ToDecimal(drTotal["AMOUNT"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["MAKINGAMOUNT"])) ? Convert.ToDecimal(drTotal["MAKINGAMOUNT"]) : 0)
                                            + (!string.IsNullOrEmpty(Convert.ToString(drTotal["WastageAmount"])) ? Convert.ToDecimal(drTotal["WastageAmount"]) : 0); // Added for wastage 
                        }
                        txtTotalAmount.Text = Convert.ToString(objRounding.Round(dTotalAmount, 2));
                    }

                }
            }
        }
        #endregion

        #region DELETE CLICK
        private void btnDelete_Click(object sender, EventArgs e)
        {
            int DeleteSelectedIndex = 0;
            if(dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                if(grdView.RowCount > 0)
                {
                    DeleteSelectedIndex = grdView.GetSelectedRows()[0];
                    DataRow theRowToDelete = dtItemInfo.Rows[DeleteSelectedIndex];
                    if(Convert.ToBoolean(theRowToDelete["IsBookedSKU"]))
                    {
                        SKUInfo(Convert.ToString(theRowToDelete["ITEMID"]), false);
                    }

                    dtItemInfo.Rows.Remove(theRowToDelete);
                    grItems.DataSource = dtItemInfo.DefaultView;
                    if(dtItemInfo != null && dtItemInfo.Rows.Count > 0)
                    {
                        Decimal dTotalAmount = 0m;
                        foreach(DataRow drTotal in dtItemInfo.Rows)
                        {
                            // dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["AMOUNT"])) ? Convert.ToDecimal(drTotal["AMOUNT"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["EXTENDEDDETAILS"])) ? Convert.ToDecimal(drTotal["EXTENDEDDETAILS"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["MAKINGAMOUNT"])) ? Convert.ToDecimal(drTotal["MAKINGAMOUNT"]) : 0);
                            // dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["AMOUNT"])) ? Convert.ToDecimal(drTotal["AMOUNT"]) : 0) +  (!string.IsNullOrEmpty(Convert.ToString(drTotal["MAKINGAMOUNT"])) ? Convert.ToDecimal(drTotal["MAKINGAMOUNT"]) : 0);
                            dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["AMOUNT"])) ? Convert.ToDecimal(drTotal["AMOUNT"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["MAKINGAMOUNT"])) ? Convert.ToDecimal(drTotal["MAKINGAMOUNT"]) : 0)
                                                + (!string.IsNullOrEmpty(Convert.ToString(drTotal["WastageAmount"])) ? Convert.ToDecimal(drTotal["WastageAmount"]) : 0); // Added for wastage 
                        }
                        txtTotalAmount.Text = Convert.ToString(objRounding.Round(dTotalAmount, 2));
                    }
                }

            }
            if(DeleteSelectedIndex == 0 && dtItemInfo != null && dtItemInfo.Rows.Count == 0)
            {
                txtTotalAmount.Text = "0.00";
                grItems.DataSource = null;
                dtItemInfo.Clear();
            }
            IsEdit = false;

        }
        #endregion

        #region Sample Click
        private void btnSample_Click(object sender, EventArgs e)
        {
            if(dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                if(grdView.RowCount > 0)
                {
                    int selectedRow = grdView.GetSelectedRows()[0];
                    frmCustOrderSample frm = new frmCustOrderSample(pos, application, dtSample, selectedRow + 1, dtSampleSketch);

                    frm.ShowDialog();
                    dtSample = frm.dtSample;
                    dtSampleSketch = frm.dtSketch;
                }
            }
            else if(dsSearchedOrder != null && dsSearchedOrder.Tables.Count > 0 && dsSearchedOrder.Tables[3].Rows.Count > 0)
            {
                if(grdView.RowCount > 0)
                {
                    int selectedRow = grdView.GetSelectedRows()[0];

                    frmCustOrderSample frm = new frmCustOrderSample(pos, application, dsSearchedOrder.Tables[3], selectedRow + 1, dtSampleSketch, true);
                    frm.ShowDialog();
                }
            }
            else
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please enter at least one row to enter the sample details or there are no sample present for the selected item.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }
            }
        }
        #endregion

        #region CLear Click
        private void btnClear_Click(object sender, EventArgs e)
        {
            if(dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                foreach(DataRow dr in dtItemInfo.Rows)
                {
                    if(Convert.ToBoolean(dr["IsBookedSKU"]))
                    {
                        SKUInfo(Convert.ToString(dr["ITEMID"]), false);
                    }
                }
            }
            saleLineItem = new SaleLineItem();
            objRounding = new Rounding();
            dtItemInfo = new DataTable();
            dtSubOrderDetails = new DataTable();
            sExtendedDetailsAmount = 0m;
            randUnique = new Random();
            IsEdit = false;
            EditselectedIndex = 0;
            dsSearchedOrder = new DataSet();
            grItems.DataSource = null;
            ClearControls();
            txtTotalAmount.Text = string.Empty;
        }
        #endregion

        #region Cancel Click
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if(dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                foreach(DataRow dr in dtItemInfo.Rows)
                {
                    if(Convert.ToBoolean(dr["IsBookedSKU"]))
                    {
                        SKUInfo(Convert.ToString(dr["ITEMID"]), false);
                    }
                }
            }
            else if(!string.IsNullOrEmpty(txtItemId.Text))
            {
                if(Convert.ToBoolean(chkBookedSKU.Checked))
                {
                    SKUInfo(Convert.ToString(txtItemId.Text), false);
                }
            }
            this.Close();
        }
        #endregion

        bool isValied() // added on 05/06/2014 for after validate all req field chk the isbook or not chkbox , not before
        {
            if(ValidateControls())
            {
                //if(chkBookedSKU.Checked)
                //{
                //    if(!SKUInfo(txtItemId.Text.Trim(), true))
                //    {
                //        using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("SKU is already booked/ not in counter", MessageBoxButtons.OK, MessageBoxIcon.Information))
                //        {
                //            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                //        }
                //        chkBookedSKU.Enabled = false;
                //        chkBookedSKU.Checked = false;
                //        btnPOSItemSearch.Focus();
                //        return false;
                //    }
                //    else
                //    {
                //        return true;
                //    }
                //}
                //else
                //{
                //    return true;
                //}
                return true;
            }
            else
            {
                return false;
            }
        }

        #region ValidateControls()
        bool ValidateControls()
        {

            if(txtItemId.Text.ToUpper().Trim() == "ITEM ID" || string.IsNullOrEmpty(txtItemId.Text))
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Item Id can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    btnPOSItemSearch.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }

            if(txtItemName.Text.ToUpper().Trim() == "ITEM NAME" || string.IsNullOrEmpty(txtItemName.Text))
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Item name can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    btnPOSItemSearch.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if(IsCatchWtItem(txtItemId.Text.Trim()))
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
            }
            else
            {
                txtPCS.Enabled = false;
            }

            //if ((!string.IsNullOrEmpty(txtPCS.Text.Trim())) && (Convert.ToDecimal(txtPCS.Text) > 1))
            //{
            //    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("PCS can not be greater than 1", MessageBoxButtons.OK, MessageBoxIcon.Information))
            //    {
            //        txtPCS.Focus();
            //        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            //        return false;
            //    }
            //}

            if(string.IsNullOrEmpty(txtQuantity.Text.Trim()))
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Quantity can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtQuantity.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }

            if(string.IsNullOrEmpty(txtRate.Text.Trim()))
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Rate can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtRate.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if(!string.IsNullOrEmpty(txtRate.Text.Trim()) && Convert.ToDecimal(txtRate.Text.Trim()) == 0m && !saleLineItem.ZeroPriceValid)
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Zero Rate is not allowed.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtRate.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if(string.IsNullOrEmpty(txtAmount.Text.Trim()))
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Amount can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtAmount.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if(!string.IsNullOrEmpty(txtAmount.Text.Trim()) && Convert.ToDecimal(txtAmount.Text.Trim()) == 0m && !saleLineItem.ZeroPriceValid)
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Amount can not be Zero", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtAmount.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }

            //if (string.IsNullOrEmpty(txtMakingRate.Text.Trim()))
            //{

            //    txtMakingRate.Text = "0";
            //}
            //if (string.IsNullOrEmpty(txtMakingAmount.Text.Trim()))
            //{
            //    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Making Amount can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
            //    {
            //        txtMakingAmount.Focus();
            //        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            //        return false;
            //    }
            //}
            else
            {
                return true;
            }
        }
        #endregion

        #region ClearControls
        private void ClearControls()
        {
            txtItemId.Text = string.Empty;
            txtItemName.Text = string.Empty;
            cmbCode.Text = string.Empty;
            cmbStyle.Text = string.Empty;
            cmbSize.Text = string.Empty;
            cmbConfig.Text = string.Empty;
            txtPCS.Text = string.Empty;
            txtQuantity.Text = string.Empty;
            txtRate.Text = string.Empty;
            cmbRateType.SelectedIndex = 0;
            txtAmount.Text = string.Empty;
            txtMakingRate.Text = string.Empty;
            cmbMakingType.SelectedIndex = 0;
            txtMakingAmount.Text = string.Empty;

            cmbWastage.SelectedIndex = 0;
            txtWastageQty.Text = string.Empty;
            txtWastagePercentage.Text = string.Empty;
            txtWastageAmount.Text = string.Empty;
            //
            chkBookedSKU.Checked = false;
            txtRemarks.Text = string.Empty;
        }
        #endregion

        #region Key Press Events

        private void txtPCS_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

        }

        private void txtQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if(e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }


        }

        private void txtRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if(e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void txtMakingRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if(e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }
        #endregion

        #region  NumPad()
        void NumPad()
        {
            if(obj != null)
            {
                obj.Dispose();
            }
            obj = new LSRetailPosis.POSProcesses.WinControls.NumPad();


            obj.Anchor = System.Windows.Forms.AnchorStyles.None;
            obj.Appearance.BackColor = System.Drawing.Color.White;
            obj.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            obj.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            obj.Appearance.Options.UseBackColor = true;
            obj.Appearance.Options.UseFont = true;
            obj.Appearance.Options.UseForeColor = true;
            obj.AutoSize = true;
            obj.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            obj.EnteredValue = "";
            obj.EntryStartsInDecimals = false;
            obj.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.None;
            obj.Location = new System.Drawing.Point(738, 65);
            obj.LookAndFeel.SkinName = "The Asphalt World";
            obj.LookAndFeel.UseDefaultLookAndFeel = false;
            obj.MaskChar = null;
            obj.MaskInterval = 0;
            obj.MaxNumberOfDigits = 13;
            obj.MinimumSize = new System.Drawing.Size(170, 242);
            obj.Name = "numPad1";
            obj.NegativeMode = false;
            obj.NoOfTries = 0;
            obj.NumberOfDecimals = 3;
            obj.PromptText = "Enter Details: ";
            obj.ShortcutKeysActive = false;
            obj.Size = new System.Drawing.Size(170, 242);
            obj.TabIndex = 14;
            obj.TimerEnabled = false;
            obj.Visible = true;

            numPad1.Visible = false;

            obj.EnterButtonPressed += new LSRetailPosis.POSProcesses.WinControls.NumPad.enterbuttonDelegate(obj_EnterButtonPressed);
        }
        #endregion

        #region NUMPAD ENTER PRESSED
        private void numPad1_EnterButtonPressed()
        {
            if(!string.IsNullOrEmpty(txtPCS.Text.Trim()))
            {
                txtPCS.Text = numPad1.EnteredValue;

                numPad1.Refresh();
            }
            else if(string.IsNullOrEmpty(txtQuantity.Text.Trim()))
            {
                txtQuantity.Text = numPad1.EnteredValue;
                numPad1.Refresh();
            }
            else if(string.IsNullOrEmpty(txtRate.Text.Trim()))
            {
                txtRate.Text = numPad1.EnteredValue;
                numPad1.Refresh();
            }
            else if(string.IsNullOrEmpty(txtMakingRate.Text.Trim()))
            {
                txtMakingRate.Text = numPad1.EnteredValue;
                numPad1.Refresh();
            }
        }
        #endregion

        #region obj_EnterButtonPressed
        private void obj_EnterButtonPressed()
        {
            if(string.IsNullOrEmpty(txtQuantity.Text.Trim()))
            {
                txtQuantity.Text = obj.EnteredValue;
                numPad1.Refresh();
            }
            else if(string.IsNullOrEmpty(txtRate.Text.Trim()))
            {
                txtRate.Text = obj.EnteredValue;
                numPad1.Refresh();
            }
            else if(string.IsNullOrEmpty(txtMakingRate.Text.Trim()))
            {
                txtMakingRate.Text = obj.EnteredValue;
                numPad1.Refresh();
            }

        }
        #endregion

        #region Making Rate
        private void CheckMakingRateFromDB(SqlConnection conn, string StoreID, string ItemID)
        {
            decimal dWtRange = 0m;

            if(dIngrdMetalWtRange != 0)
            {
                dWtRange = dIngrdMetalWtRange;
            }
            else if(!string.IsNullOrEmpty(Convert.ToString(txtQuantity.Text.Trim())))
            {
                dWtRange = Convert.ToDecimal(txtQuantity.Text.Trim());
            }

            if(conn.State == ConnectionState.Closed)
                conn.Open();
            StoreID = pos.StoreId;


            /* // blocked on 26.07.2013 
            string commandText = "  DECLARE @INVENTLOCATION VARCHAR(20)    DECLARE @LOCATION VARCHAR(20)      DECLARE @ITEM VARCHAR(20) " +
                                 "  SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM  " +
                                 "  RETAILCHANNELTABLE INNER JOIN  RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID  " +
                                 "  WHERE STORENUMBER='" + StoreID + "' " +
                                 "  IF EXISTS(SELECT TOP(1) * FROM RETAIL_SALES_AGREEMENT_DETAIL WHERE INVENTLOCATIONID=@INVENTLOCATION) " +
                                 "  BEGIN SET @LOCATION=@INVENTLOCATION  END ELSE BEGIN  SET @LOCATION='' END  " +
                                 "  IF NOT EXISTS(SELECT TOP(1) ITEMID FROM RETAIL_SALES_AGREEMENT_DETAIL WHERE ITEMID='" + ItemID + "') " +
                                 "  BEGIN SELECT @ITEM= ITEMIDPARENT FROM [INVENTTABLE] WHERE ITEMID = '" + ItemID + "'  END ELSE BEGIN SET @ITEM='" + ItemID + "' END  ";


            commandText += " SELECT  TOP (1) CAST(RETAIL_SALES_AGREEMENT_DETAIL.MK_RATE AS decimal(6, 2)) , RETAIL_SALES_AGREEMENT_DETAIL.MK_TYPE " +
                         " FROM         RETAIL_SALES_AGREEMENT_DETAIL INNER JOIN " +
                         " INVENTTABLE ON RETAIL_SALES_AGREEMENT_DETAIL.MFG_CODE = INVENTTABLE.MFG_CODE " +
                           " WHERE    " +
                           " RETAIL_SALES_AGREEMENT_DETAIL.ITEMID=@ITEM  " +
                           " AND RETAIL_SALES_AGREEMENT_DETAIL.INVENTLOCATIONID=@LOCATION  " +
                           " AND  (RETAIL_SALES_AGREEMENT_DETAIL.ARTICLE_CODE=(SELECT ARTICLE_CODE FROM [INVENTTABLE] WHERE ITEMID = '" + ItemID.Trim() + "') " +
                           " OR RETAIL_SALES_AGREEMENT_DETAIL.ARTICLE_CODE='') AND " +
                           "  (RETAIL_SALES_AGREEMENT_DETAIL.COMPLEXITY_CODE=(SELECT COMPLEXITY_CODE FROM [INVENTTABLE] WHERE ITEMID = '" + ItemID.Trim() + "') " +
                           "  OR RETAIL_SALES_AGREEMENT_DETAIL.COMPLEXITY_CODE='')  AND " +
                           "  " +
                           "  ('" + txtQuantity.Text.Trim() + "' BETWEEN RETAIL_SALES_AGREEMENT_DETAIL.FROM_WEIGHT AND RETAIL_SALES_AGREEMENT_DETAIL.TO_WEIGHT)  " +
                           " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN RETAIL_SALES_AGREEMENT_DETAIL.FROMDATE AND RETAIL_SALES_AGREEMENT_DETAIL.TODATE)  " +
                           " ";
            */

            #region

            string commandText = "  DECLARE @INVENTLOCATION VARCHAR(20)    DECLARE @LOCATION VARCHAR(20)   DECLARE @ITEM VARCHAR(20)  DECLARE @PARENTITEM VARCHAR(20)   DECLARE @MFGCODE VARCHAR(20)" +  // CHANGED
                                "  SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM  " +
                                "  RETAILCHANNELTABLE INNER JOIN  RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID  " +
                                "  WHERE STORENUMBER='" + StoreID + "' " +
                                "  SELECT @MFGCODE = MFG_CODE FROM INVENTTABLE WHERE ITEMID='" + ItemID + "'" +
                                "  IF EXISTS(SELECT TOP(1) * FROM RETAIL_SALES_AGREEMENT_DETAIL WHERE INVENTLOCATIONID=@INVENTLOCATION) " +
                                "  BEGIN SET @LOCATION=@INVENTLOCATION  END ELSE BEGIN  SET @LOCATION='' END  " +
                // "  IF NOT EXISTS(SELECT TOP(1) ITEMID FROM RETAIL_SALES_AGREEMENT_DETAIL WHERE ITEMID='" + ItemID + "') " + // CHANGED

                               //  //  "  BEGIN SELECT @ITEM= ITEMIDPARENT FROM [INVENTTABLE] WHERE ITEMID = '" + ItemID + "'  END ELSE BEGIN SET @ITEM='" + ItemID + "' END  ";
                //  // "  BEGIN SET @ITEM='' END ELSE BEGIN SET @ITEM='' END  ";

                               //"  BEGIN SET @ITEM='' END ELSE BEGIN SET @ITEM='" + ItemID + "' END  "; // CHANGED

                               /* BLOCKED ON 22.03.13
                                " IF EXISTS(SELECT TOP(1) ITEMID FROM  INVENTTABLE WHERE ITEMID='" + ItemID + "' AND RETAIL=1)" + 
                                " BEGIN SELECT @PARENTITEM = ITEMIDPARENT FROM [INVENTTABLE] WHERE ITEMID = '" + ItemID + "' " +  
                                " IF (ISNULL(@PARENTITEM,'') <> '') BEGIN SET @ITEM = @PARENTITEM END ELSE BEGIN SET @ITEM = '' END " + 
                                " END  ELSE  BEGIN  SET @ITEM='" + ItemID + "' END";
                               */

                       " SET @PARENTITEM = ''" +
                       " IF EXISTS(SELECT TOP(1) ITEMID FROM  INVENTTABLE WHERE ITEMID='" + ItemID + "' AND RETAIL=1)" +
                       " BEGIN SELECT @PARENTITEM = ITEMIDPARENT FROM [INVENTTABLE] WHERE ITEMID = '" + ItemID + "' " +
                       " END  SET @ITEM='" + ItemID + "'";
            //" IF (ISNULL(@PARENTITEM,'') <> '') BEGIN SET @ITEM = @PARENTITEM END ELSE BEGIN SET @ITEM = '' END " + 
            //" END  ELSE  BEGIN  SET @ITEM='" + ItemID + "' END";

            commandText += " SELECT  TOP (1) CAST(RETAIL_SALES_AGREEMENT_DETAIL.MK_RATE AS decimal(18, 2)) , RETAIL_SALES_AGREEMENT_DETAIL.MK_TYPE " +
                            " ,WAST_TYPE,CAST(WAST_QTY AS decimal(18,2))" + // added for wastage
                         " FROM         RETAIL_SALES_AGREEMENT_DETAIL INNER JOIN " +
                //  " INVENTTABLE ON RETAIL_SALES_AGREEMENT_DETAIL.MFG_CODE = INVENTTABLE.MFG_CODE " + // Blocked on 29.05.2013
                           " (SELECT MFG_CODE FROM INVENTTABLE WHERE  ITEMID=  '" + ItemID.Trim() + "') T ON RETAIL_SALES_AGREEMENT_DETAIL.MFG_CODE = T.MFG_CODE " + // Added on 29.05.2013
                           " WHERE    " +
                // " RETAIL_SALES_AGREEMENT_DETAIL.ITEMID=@ITEM  " + 
                           " (RETAIL_SALES_AGREEMENT_DETAIL.ITEMID=@ITEM OR RETAIL_SALES_AGREEMENT_DETAIL.ITEMID = @PARENTITEM OR RETAIL_SALES_AGREEMENT_DETAIL.ITEMID='')" +
                           " AND RETAIL_SALES_AGREEMENT_DETAIL.INVENTLOCATIONID=@LOCATION  " +
                           " AND RETAIL_SALES_AGREEMENT_DETAIL.MFG_CODE = @MFGCODE" +
                           " AND  (RETAIL_SALES_AGREEMENT_DETAIL.ARTICLE_CODE=(SELECT ARTICLE_CODE FROM [INVENTTABLE] WHERE ITEMID = '" + ItemID.Trim() + "') " +
                           " OR RETAIL_SALES_AGREEMENT_DETAIL.ARTICLE_CODE='') AND " +
                           "  (RETAIL_SALES_AGREEMENT_DETAIL.COMPLEXITY_CODE=(SELECT COMPLEXITY_CODE FROM [INVENTTABLE] WHERE ITEMID = '" + ItemID.Trim() + "') " +
                           "  OR RETAIL_SALES_AGREEMENT_DETAIL.COMPLEXITY_CODE='')  AND " +
                           "  " +
                // "  ('" + txtQuantity.Text.Trim() + "' BETWEEN RETAIL_SALES_AGREEMENT_DETAIL.FROM_WEIGHT AND RETAIL_SALES_AGREEMENT_DETAIL.TO_WEIGHT)  " + // blocked 
                          "  (" + dWtRange + " BETWEEN RETAIL_SALES_AGREEMENT_DETAIL.FROM_WEIGHT AND RETAIL_SALES_AGREEMENT_DETAIL.TO_WEIGHT)  " +
                           " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN RETAIL_SALES_AGREEMENT_DETAIL.FROMDATE AND RETAIL_SALES_AGREEMENT_DETAIL.TODATE)  " +
                           " AND RETAIL_SALES_AGREEMENT_DETAIL.ACTIVATE = 1 " + // ADDED ON 14.03.13 FOR ACTIVATE filter 
                //  "  ORDER BY RETAIL_SALES_AGREEMENT_DETAIL.ITEMID,RETAIL_SALES_AGREEMENT_DETAIL.ARTICLE_CODE,RETAIL_SALES_AGREEMENT_DETAIL.COMPLEXITY_CODE" +
                //  " ,RETAIL_SALES_AGREEMENT_DETAIL.FROMDATE DESC";  // Added on 29.05.2013
                // Changed order sequence on 03.06.2013 as per u.das
                        "  ORDER BY RETAIL_SALES_AGREEMENT_DETAIL.ITEMID DESC,RETAIL_SALES_AGREEMENT_DETAIL.COMPLEXITY_CODE DESC,RETAIL_SALES_AGREEMENT_DETAIL.ARTICLE_CODE DESC" +
                           " ,RETAIL_SALES_AGREEMENT_DETAIL.FROMDATE DESC";

            #endregion

            SqlCommand command = new SqlCommand(commandText, conn);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();

            if(reader.HasRows)
            {
                txtMakingRate.Text = "0";
                while(reader.Read())
                {
                    // txtMakingRate.Text = Convert.ToString(objRounding.Round(Convert.ToDecimal(reader.GetValue(0)), 2));
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
                    txtMakingRate.Text = Convert.ToString(objRounding.Round(Convert.ToDecimal(reader.GetValue(0)), 2));

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

                }
            }
            if(conn.State == ConnectionState.Open)
                conn.Close();


        }
        #endregion

        #region Get Quantity
        public string GetStandardQuantityFromDB(SqlConnection conn, string itemid)
        {
            if(conn.State == ConnectionState.Closed)
                conn.Open();

            string commandText = " SELECT STDQTY FROM INVENTTABLE WHERE ITEMID='" + itemid + "'";

            SqlCommand command = new SqlCommand(commandText, conn);
            command.CommandTimeout = 0;

            string sQty = Convert.ToString(command.ExecuteScalar());

            if(conn.State == ConnectionState.Open)
                conn.Close();

            return sQty;

        }
        #endregion

        #region  GET Wastage Metal rate
        private decimal getWastageMetalRate(string StoreID, string sItemId, string sConfigId)
        {
            SqlConnection conn = new SqlConnection();
            if(application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) ");
            commandText.Append(" DECLARE @METALTYPE INT ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM         RETAILCHANNELTABLE INNER JOIN ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + StoreID.Trim() + "'  ");
            commandText.Append(" SELECT @METALTYPE=METALTYPE FROM [INVENTTABLE] WHERE ITEMID='" + sItemId.Trim() + "' ");
            commandText.Append(" IF(@METALTYPE IN ('" + (int)MetalType.Gold + "','" + (int)MetalType.Silver + "','" + (int)MetalType.Platinum + "','" + (int)MetalType.Palladium + "')) ");
            commandText.Append(" BEGIN ");
            commandText.Append(" SELECT TOP 1 CAST(ISNULL(RATES,0) AS decimal (6,2)) FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION  ");
            //  commandText.Append(" AND DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0)<=TRANSDATE AND METALTYPE=@METALTYPE ");
            commandText.Append(" AND METALTYPE=@METALTYPE ");
            commandText.Append(" AND RETAIL=1 AND RATETYPE='" + (int)RateTypeNew.Sale + "' ");
            // chkOwn.Enabled = true;
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
            //string sResult = Convert.ToString(command.ExecuteScalar());

            decimal dResult = Convert.ToDecimal(command.ExecuteScalar());

            if(conn.State == ConnectionState.Open)
                conn.Close();

            return dResult;

        }

        protected int GetMetalType(string sItemId)
        {
            SqlConnection conn = new SqlConnection();
            if(application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;
            int iMetalType = 100;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select metaltype from inventtable where itemid='" + sItemId + "'");

            if(conn.State == ConnectionState.Closed)
                conn.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();
            if(reader.HasRows)
            {
                while(reader.Read())
                {
                    iMetalType = (int)reader.GetValue(0);
                }
            }
            if(conn.State == ConnectionState.Open)
                conn.Close();
            return iMetalType;

        }
        #endregion

        private bool IsSKUItem(string sItemId)
        {
            SqlConnection conn = new SqlConnection();
            if(application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @IsSKU AS INT; SET @IsSKU = 0; IF EXISTS(SELECT TOP(1) [SkuNumber] FROM [SKUTable_Posted] WHERE  [SkuNumber] = '" + sItemId + "') ");
            commandText.Append(" BEGIN SET @IsSKU = 1 END SELECT @IsSKU");


            if(conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            bool bVal = Convert.ToBoolean(command.ExecuteScalar());

            if(conn.State == ConnectionState.Open)
                conn.Close();

            return bVal;

        }

        private void chkBookedSKU_CheckedChanged(object sender, EventArgs e)
        {
            //int i = 0;
            //if (chkBookedSKU.Checked)
            //{
            //    if (!SKUInfo(txtItemId.Text.Trim(), true))
            //    {
            //        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("SKU is already booked", MessageBoxButtons.OK, MessageBoxIcon.Information))
            //        {
            //            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            //        }
            //        chkBookedSKU.Enabled = false;
            //        chkBookedSKU.Checked = false;
            //    }

            //   // i = 1;
            //    chkBookedSKU.Enabled = false;
            //}
            //else if (chkBookedSKU.Checked == false)
            //{
            //    SKUInfo(txtItemId.Text.Trim(), false);
            //}

            int i = 0;
            if(chkBookedSKU.Checked)
            {
                int iSkuStatus = SKUInfo(txtItemId.Text.Trim(), true);
                if(iSkuStatus == 2)
                {
                    using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("SKU is already booked", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    }
                    chkBookedSKU.Enabled = false;
                    chkBookedSKU.Checked = false;
                    return;
                }
                if(iSkuStatus == 3)
                {
                    using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("SKU is not in counter", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    }
                    chkBookedSKU.Enabled = false;
                    chkBookedSKU.Checked = false;
                    return;
                }

                // i = 1;
                chkBookedSKU.Enabled = false;
            }
        }

        private int SKUInfo(string sItemId, bool bMode)
        {
            //SqlConnection conn = new SqlConnection();
            //if(application != null)
            //    conn = application.Settings.Database.Connection;
            //else
            //    conn = ApplicationSettings.Database.LocalConnection;

            //StringBuilder commandText = new StringBuilder();
            //if(bMode)
            //{
            //    commandText.Append(" DECLARE @IsSKU AS INT; SET @IsSKU = 0; IF EXISTS   (SELECT TOP(1) [SkuNumber] FROM [SKUTableTrans] WHERE  [SkuNumber] = '" + sItemId + "'");
            //    commandText.Append(" AND isAvailable='True' AND isLocked='False' AND DATAAREAID='" + application.Settings.Database.DataAreaID + "') ");
            //    commandText.Append(" BEGIN SET @IsSKU = 1  UPDATE SKUTableTrans SET isAvailable='False',isLocked='False' WHERE SkuNumber = '" + sItemId + "' AND DATAAREAID='" + application.Settings.Database.DataAreaID + "' END SELECT @IsSKU");
            //}
            //else
            //{
            //    commandText.Append("DECLARE @IsSKU AS INT; SET @IsSKU = 1; UPDATE SKUTableTrans SET isAvailable='True',isLocked='False' WHERE SkuNumber = '" + sItemId + "' AND DATAAREAID='" + application.Settings.Database.DataAreaID + "'  SELECT @IsSKU");
            //}


            //if(conn.State == ConnectionState.Closed)
            //    conn.Open();
            //SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            //command.CommandTimeout = 0;

            //bool bVal = Convert.ToBoolean(command.ExecuteScalar());

            //if(conn.State == ConnectionState.Open)
            //    conn.Close();

            //return bVal;

            SqlConnection conn = new SqlConnection();
            if(application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            if(bMode)
            {
                //commandText.Append(" DECLARE @IsSKU AS INT; SET @IsSKU = 0; IF EXISTS   (SELECT TOP(1) [SkuNumber] FROM [SKUTableTrans] WHERE  [SkuNumber] = '" + sItemId + "'");
                //commandText.Append(" AND isAvailable='True' AND isLocked='False' AND DATAAREAID='" + application.Settings.Database.DataAreaID + "') ");
                //commandText.Append(" BEGIN SET @IsSKU = 1  UPDATE SKUTableTrans SET isLocked='True' WHERE SkuNumber = '" + sItemId + "'");
                //commandText.Append(" AND DATAAREAID='" + application.Settings.Database.DataAreaID + "' END  else SET @IsSKU = 2 SELECT @IsSKU");

                commandText.Append(" DECLARE @IsSKU AS INT; SET @IsSKU = 0;");
                commandText.Append(" IF EXISTS   (SELECT TOP(1) [SkuNumber] FROM [SKUTableTrans] ");
                commandText.Append(" WHERE  [SkuNumber] = '" + sItemId + "')");
                commandText.Append(" BEGIN  ");
                commandText.Append("IF EXISTS   (SELECT TOP(1) [SkuNumber] FROM [SKUTableTrans] WHERE  [SkuNumber] = '" + sItemId + "' ");
                commandText.Append("AND isLocked='False' AND isAvailable='True' AND DATAAREAID='" + application.Settings.Database.DataAreaID + "')  ");
                commandText.Append("BEGIN  SET @IsSKU = 1  ");
                commandText.Append("UPDATE SKUTableTrans SET isAvailable='False',isLocked='False' WHERE SkuNumber = '" + sItemId + "' AND DATAAREAID='" + application.Settings.Database.DataAreaID + "'");
                commandText.Append("END  ");
                commandText.Append("else ");
                commandText.Append("SET @IsSKU = 2 ");
                commandText.Append("end ");
                commandText.Append("ELSE ");
                commandText.Append("SET @IsSKU = 3 SELECT @IsSKU");
            }
            else
            {
                commandText.Append("DECLARE @IsSKU AS INT; SET @IsSKU = 1; UPDATE SKUTableTrans SET isAvailable='True',isLocked='False'");
                commandText.Append(" WHERE SkuNumber = '" + sItemId + "' AND DATAAREAID='" + application.Settings.Database.DataAreaID + "'  SELECT @IsSKU");
            }


            if(conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            int bVal = Convert.ToInt16(command.ExecuteScalar());

            if(conn.State == ConnectionState.Open)
                conn.Close();

            return bVal;

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

        private void btnAddImage_Click(object sender, EventArgs e)
        {
            if(dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                DataTable dt = new DataTable();

                if(grdView.RowCount > 0)
                {
                    int selectedRow = grdView.GetSelectedRows()[0];
                    DataRow drRowToUpdate = dtItemInfo.Rows[selectedRow];
                    DataTable dtSketchDownload = new DataTable();
                    if(dtSketch != null && dtSketch.Rows.Count > 0)
                    {
                        foreach(DataRow dtDown in dtSketch.Rows)
                        {
                            if(Convert.ToString(drRowToUpdate["UNIQUEID"]) == Convert.ToString(dtDown["UNIQUEID"]))
                            {
                                dtSketchDownload = dtSketch.Clone();
                                dtSketchDownload.ImportRow(dtDown);
                                dtSketchDownload.AcceptChanges();
                                break;
                            }
                        }
                    }
                    frmOrderSketch oSketch = new frmOrderSketch(dt, Convert.ToString(drRowToUpdate["UNIQUEID"]), dtSketchDownload, selectedRow + 1);

                    //oSketch.lblBreadCrumbs.Text = lblBreadCrumbs.Text + ">" + " line no:" + " " + Convert.ToInt16(selectedRow + 1);
                    oSketch.ShowDialog();
                    if(oSketch.dtUploadSketch != null && oSketch.dtUploadSketch.Rows.Count > 0)
                    {
                        if(dtSketch != null && dtSketch.Rows.Count > 0)
                        {
                            foreach(DataRow dr in oSketch.dtUploadSketch.Rows)
                            {
                                foreach(DataRow drSk in dtSketch.Rows)
                                {
                                    if(Convert.ToString(dr["UNIQUEID"]) == Convert.ToString(drSk["UNIQUEID"]))
                                    {
                                        drSk.Delete();
                                        dtSketch.AcceptChanges();
                                        break;
                                    }
                                }
                                dtSketch.ImportRow(dr);
                            }
                            dtSketch.AcceptChanges();
                            oSketch.dtUploadSketch = new DataTable();
                        }
                        else
                        {
                            dtSketch = oSketch.dtUploadSketch;
                        }
                    }
                    else
                    {
                        foreach(DataRow drSk in dtSketch.Rows)
                        {
                            if(Convert.ToString(drRowToUpdate["UNIQUEID"]) == Convert.ToString(drSk["UNIQUEID"]))
                            {
                                drSk.Delete();
                                dtSketch.AcceptChanges();
                                break;
                            }
                        }
                    }
                }
            }
            else if(dsSearchedOrder != null && dsSearchedOrder.Tables.Count > 0 && dsSearchedOrder.Tables[1].Rows.Count > 0)
            {
                if(grdView.RowCount > 0)
                {
                    int selectedRow = grdView.GetSelectedRows()[0];
                    DataRow drRowToUpdate = dsSearchedOrder.Tables[1].Rows[selectedRow];

                    frmOrderSketch objSketch = new frmOrderSketch(dsSearchedOrder.Tables[1], Convert.ToDecimal(drRowToUpdate["LINENUM"]), "");
                }
            }
            else
            {
                MessageBox.Show("Please enter at least one row to upload the sketch details or " +
                "there are no sketch present for the selected item");
            }
        }

        #region Update Local Sample Return table Status from db
        public void UpdatelocalSampleTable(DataTable dt)
        {
            frmCustOrder.dtSampleDetails = dt;
            dtSample = dt;
        }
        #endregion

        private void btnSampleReturn_Click(object sender, EventArgs e)
        {
            frmCustOrderSampleReturn frmSamRet = new frmCustOrderSampleReturn(pos, application, dtSample, this);
            frmSamRet.ShowDialog();
        }

        private void btnRcvStone_Click(object sender, EventArgs e)
        {
            if(dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                if(grdView.RowCount > 0)
                {
                    int selectedRow = grdView.GetSelectedRows()[0];

                    if(dtStone.Rows.Count == 0)
                    {
                        dtStone = frmCustOrder.dtRecvStoneDetails;
                    }
                    frmCustOrderSampleStoneStone frm = new frmCustOrderSampleStoneStone(pos, application, dtStone, selectedRow + 1);

                    frm.ShowDialog();
                    dtStone = frm.dtRecvStoneDetails;
                }
            }
            else if(dsSearchedOrder != null && dsSearchedOrder.Tables.Count > 0 && dsSearchedOrder.Tables[4].Rows.Count > 0)
            {
                if(grdView.RowCount > 0)
                {
                    int selectedRow = grdView.GetSelectedRows()[0];
                    frmCustOrderSampleStoneStone frm = new frmCustOrderSampleStoneStone(pos, application, dsSearchedOrder.Tables[4], selectedRow + 1);

                    frm.ShowDialog();
                }

            }
            else
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please enter at least one row to enter the sample details or there are no sample present for the selected item.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);

                }
            }
        }

        private void btnPaySchedule_Click(object sender, EventArgs e)
        {
            if(dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                if(grdView.RowCount > 0)
                {
                    int selectedRow = grdView.GetSelectedRows()[0];
                    //frmCustOrderSample frm = new frmCustOrderSample(pos, application, dtSample, selectedRow + 1, dtSampleSketch);
                    frmCustOrderPaySchedule frmPay = new frmCustOrderPaySchedule(pos, application, dtPaySchedule, Convert.ToDecimal(txtTotalAmount.Text));

                    frmPay.ShowDialog();
                    dtPaySchedule = frmPay.dtPaySched;
                }
            }
            else if(dsSearchedOrder != null && dsSearchedOrder.Tables.Count > 0 && dsSearchedOrder.Tables[5].Rows.Count > 0)
            {
                if(grdView.RowCount > 0)
                {
                    int selectedRow = grdView.GetSelectedRows()[0];

                    frmCustOrderPaySchedule frmPay = new frmCustOrderPaySchedule(pos, application, dsSearchedOrder.Tables[5], Convert.ToDecimal(txtTotalAmount.Text), 1);
                    frmPay.ShowDialog();
                }
            }
            else
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please enter at least one row to enter the sample details or there are no sample present for the selected item.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }
            }
        }
    }

}
