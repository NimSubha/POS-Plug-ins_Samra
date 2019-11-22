
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using System.Data.SqlClient;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using LSRetailPosis.Transaction;
using LSRetailPosis.Settings;
using System.IO;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    public partial class frmSaleInvControlCopy : Form
    {
        BlankOperations oBlank = new BlankOperations();
        SqlConnection connection;
        // string sTransactionId = "";
        string sCustName = "";
        string sCustAddress = "";
        string sCPinCode = "";
        string sContactNo = "";
        string sCPanNo = "";

        string sInvoiceNo = "";
        string sInvDt = "";

        string sStoreName = "";
        string sStoreAddress = "";
        string sArStoreName = "";
        string sArStoreAddress = "";
        string sStorePhNo = "";

        string sDataAreaId = "";
        string sInventLocationId = "";

        string sAmtinwds = "";
        string sCustCode = "";
        string sReceiptNo = "";

        string sTerminal = string.Empty;
        string sTitle = string.Empty;

        // string sStorePh = "-";
        string sInvoiceFooter = "";
        string sInvoiceFooterArabic = "";
        string sTime = "";

        string sCompanyName = string.Empty; //aded on 14/04/2014 R.Hossain
        string sCINNo = string.Empty;//aded on 14/04/2014 R.Hossain
        string sDuplicateCopy = string.Empty;//aded on 14/04/2014 R.Hossain
        byte[] bCompImage = null;
        string sCurrencySymbol = "";
        Double dTotAmt = 0;
        int iGiftPrint = 0;
        int iInvLang = 0;
        string sFooterComments = "";
        string sCustEmail = "";

        string sStoreEmail = "";
        string sStoreFax = "";

        string sOperatorId = string.Empty;
        string sSaleItem = string.Empty;
        string sItemParentId = string.Empty;
        string sArchivePath = string.Empty;
        string sCustomTransType = string.Empty;
        RetailTransaction retailTrans;
        decimal totTaxableamt = 0m;
        decimal gsale = 0m;
        decimal totTaxAmt = 0m;
        string sAmtinwdsArabic = "";
        int isItFromShowJournal = 0;
        string sSM = "";

        #region enum MetalType
        enum MetalType
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


        public frmSaleInvControlCopy()
        {
            InitializeComponent();
        }

        public frmSaleInvControlCopy(SqlConnection conn)
        {
            InitializeComponent();
            connection = conn;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
        }

        public frmSaleInvControlCopy(IPosTransaction posTransaction, SqlConnection conn, bool bDuplicate, int iIsGift, int iLanForInv, int isItFromShowJour)
        {
            InitializeComponent();

            #region[Param Info]
            RetailTransaction retailTrans = posTransaction as RetailTransaction;
            if (retailTrans != null)
            {
                sTerminal = retailTrans.TerminalId;

                sArchivePath = GetArchivePathFromImage();

                if (Convert.ToString(retailTrans.Customer.Name) != string.Empty)
                    sCustName = Convert.ToString(retailTrans.Customer.Name);
                if (Convert.ToString(retailTrans.Customer.Address) != string.Empty)  //PrimaryAddress
                    sCustAddress = Convert.ToString(retailTrans.Customer.Address);
                if (Convert.ToString(retailTrans.Customer.PostalCode) != string.Empty)
                    sCPinCode = Convert.ToString(retailTrans.Customer.PostalCode);
                //if (Convert.ToString(retailTrans.Customer.MobilePhone) != string.Empty)
                //    sContactNo = Convert.ToString(retailTrans.Customer.MobilePhone);

                if (!string.IsNullOrEmpty(retailTrans.Customer.Telephone))
                    sContactNo = "Mobile " + ":" + Convert.ToString(retailTrans.Customer.Telephone);

                if (Convert.ToString(retailTrans.TransactionId) != string.Empty)
                    sInvoiceNo = Convert.ToString(retailTrans.TransactionId);
                if (Convert.ToString(retailTrans.OperatorId) != string.Empty)
                    sOperatorId = getStaffName(Convert.ToString(retailTrans.OperatorId));

                //sCPanNo
                if (string.IsNullOrEmpty(retailTrans.Customer.Name))
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(retailTrans.PartnerData.LCCustomerName)))
                    {
                        sCustName = Convert.ToString(retailTrans.PartnerData.LCCustomerName);
                        sCustAddress = Convert.ToString(retailTrans.PartnerData.LCCustomerAddress);
                        sContactNo = Convert.ToString(retailTrans.PartnerData.LCCustomerContactNo);
                        sCPinCode = "-";
                    }
                    else //  added on 12/04/2014 for print local cust later ( from show journal)
                    {
                        GetLocalCustomerInfo(sInvoiceNo);
                    }

                }
                //-------

                if (retailTrans.EndDateTime != null)
                    sInvDt = retailTrans.EndDateTime.ToShortDateString();

                if (retailTrans.EndDateTime != null)
                    sTime = retailTrans.EndDateTime.ToShortTimeString(); //("HH:mm")
                //----------store Info
                //if (Convert.ToString(retailTrans.StoreName) != string.Empty)
                //    sStoreName = Convert.ToString(retailTrans.StoreName);
                //if (Convert.ToString(retailTrans.StoreAddress) != string.Empty)
                //    sStoreAddress = Convert.ToString(retailTrans.StoreAddress);
                //if (Convert.ToString(retailTrans.StorePhone) != string.Empty)
                //    sStorePhNo = Convert.ToString(retailTrans.StorePhone);

                if (Convert.ToString(ApplicationSettings.Terminal.StoreName) != string.Empty)
                    sStoreName = Convert.ToString(ApplicationSettings.Terminal.StoreName);
                if (Convert.ToString(ApplicationSettings.Terminal.StoreAddress) != string.Empty)
                    sStoreAddress = Convert.ToString(ApplicationSettings.Terminal.StoreAddress);
                //if (!string.IsNullOrEmpty(Convert.ToString(ApplicationSettings.Terminal.StorePhone)))
                //sStorePhNo = Convert.ToString(ApplicationSettings.Terminal.StorePhone);





                sDataAreaId = Convert.ToString(ApplicationSettings.Database.DATAAREAID);

                if (Convert.ToString(retailTrans.InventLocationId) != string.Empty)
                    sInventLocationId = Convert.ToString(retailTrans.InventLocationId);
                if (!string.IsNullOrEmpty(Convert.ToString(retailTrans.Customer.CustomerId)))
                    sCustCode = Convert.ToString(retailTrans.Customer.CustomerId);
                if (Convert.ToString(retailTrans.ReceiptId) != string.Empty)
                    sReceiptNo = Convert.ToString(retailTrans.ReceiptId);
                if (Convert.ToString(retailTrans.Customer.Email) != string.Empty)
                    sCustEmail = "Email " + ":" + Convert.ToString(retailTrans.Customer.Email);



                ////string sCustomTransType = string.Empty; will open RHossain 
                ////sCustomTransType = GetCustomTransType(sInvoiceNo,retailTrans.TerminalId);

                if (retailTrans.SaleIsReturnSale)
                    sTitle = "RETURN SALES INVOICE";
                ////else if (sCustomTransType == "1" || sCustomTransType == "2" || sCustomTransType == "3" || sCustomTransType == "4")
                ////    sTitle = "URD Purchase";
                else
                    sTitle = "SALES INVOICE";

                // sCINNo = oBlank.getValue("select CINNO  from RETAILSTORETABLE where STORENUMBER ='" + Convert.ToString(ApplicationSettings.Terminal.StoreId) + "'");

            }
            #endregion
            connection = conn;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            GetStoreInfo(ref sStorePhNo, ref sInvoiceFooter, ref sCINNo, ref sInvoiceFooterArabic);

            isItFromShowJournal = isItFromShowJour;
            sSM = getSalesManName(retailTrans.TransactionId, retailTrans.TerminalId);

            sCompanyName = oBlank.GetCompanyName(conn);//aded on 14/04/2014 R.Hossain
            if (bDuplicate)
                sDuplicateCopy = ""; // "DUPLICATE"; It will open later RHossain 21/04/2014

            sStorePhNo = getStoreEmailAndFax("PRIMARYCONTACTPHONE");

            sStoreEmail = getStoreEmailAndFax("PrimaryContactEmail");

            sStoreFax = getStoreEmailAndFax("PrimaryContactFax"); // PrimaryContactEmail/PrimaryContactFax

            sCustomTransType = GetCustomTransType(sInvoiceNo, retailTrans.TerminalId);

            iInvLang = iLanForInv;
            iGiftPrint = iIsGift;
            if (retailTrans.SaleIsReturnSale)
            {
                sTitle = "Invoice return \n ACCOUNT COPY";
                sFooterComments = "";
                ////else if (sCustomTransType == "1" || sCustomTransType == "2" || sCustomTransType == "3" || sCustomTransType == "4")
                ////    sTitle = "URD Purchase";
            }
            else
            {
                sFooterComments = "Goods once sold can only be exchanged with in 7 days";

                if (iGiftPrint == 1)
                {
                    sTitle = "Gift invoice";
                }
                if (iLanForInv == 1)
                {
                    sTitle = "Tax Invoice";
                }
                else if (iLanForInv == 2)
                {
                    sInvoiceFooter = sInvoiceFooterArabic;
                    sTitle = "";
                }
                else if (iLanForInv == 3)
                    sTitle = "Tax Invoice" + "   " + "فاتورة ضريبية";
            }

        }

        private string getSalesManName(string sTransId, string sTerminalId)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            string commandText = "select top 1 d.NAME as Name from RETAILTRANSACTIONSALESTRANS A" +
                      " LEFT JOIN dbo.RETAILSTAFFTABLE AS T11 on A.STAFF =T11.STAFFID " +
                      " LEFT JOIN dbo.HCMWORKER AS T22 ON T11.STAFFID = T22.PERSONNELNUMBER" +
                      " left join dbo.DIRPARTYTABLE as d on d.RECID = T22.PERSON " +
                      " Where TRANSACTIONID='" + sTransId + "' and a.TERMINALID='" + sTerminalId + "' and isnull(A.STAFF,'')!=''";

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
                return sResult.Trim();
            else
                return "-";
        }

        public frmSaleInvControlCopy(string sTransactionId, SqlConnection conn)
        {
            InitializeComponent();

            /*
            RetailTransaction retailTrans = posTransaction as RetailTransaction;
            if (retailTrans != null)
            {
                if(Convert.ToString(retailTrans.Customer.Name) != string.Empty)
                sCustName = Convert.ToString(retailTrans.Customer.Name);
                if (Convert.ToString(retailTrans.Customer.Address) != string.Empty)  //PrimaryAddress
                    sCustAddress = Convert.ToString(retailTrans.Customer.Address);
                if (Convert.ToString(retailTrans.Customer.PostalCode) != string.Empty)  
                    sCPinCode = Convert.ToString(retailTrans.Customer.PostalCode);
                if (Convert.ToString(retailTrans.Customer.MobilePhone) != string.Empty)
                    sContactNo = Convert.ToString(retailTrans.Customer.MobilePhone);
                sCPanNo
                
                -------
                if (Convert.ToString(retailTrans.TransactionId) != string.Empty)
                    sInvoiceNo = Convert.ToString(retailTrans.TransactionId);

                if (retailTrans.BeginDateTime != null)
                    sInvDt = retailTrans.BeginDateTime.ToShortDateString();
                
                ----------store Info
                if (Convert.ToString(retailTrans.StoreName) != string.Empty)
                    sStoreName = Convert.ToString(retailTrans.StoreName);
                if (Convert.ToString(retailTrans.StoreAddress) != string.Empty)
                    sStoreAddress = Convert.ToString(retailTrans.StoreAddress);
                if (Convert.ToString(retailTrans.StorePhone) != string.Empty)
                    sStorePhNo = Convert.ToString(retailTrans.StorePhone);
            */


            sDataAreaId = Convert.ToString(ApplicationSettings.Database.DATAAREAID);

            //if (Convert.ToString(retailTrans.InventLocationId) != string.Empty)
            //sInventLocationId = Convert.ToString(retailTrans.InventLocationId);

            //}
            connection = conn;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
        }

        
        #region [GetSalesInvData --old]
        //private void GetSalesInvData(ref DataSet dsSalesOrder)
        //{
        //    string sqlSalesDtl =
        //        //"  DECLARE @IsIngredient as int" +
        //        // " SELECT @IsIngredient = IsIngredient FROM RETAIL_CUSTOMCALCULATIONS_TABLE WHERE TRANSACTIONID = @TRANSACTIONID" +
        //        // " IF ISNULL(@IsIngredient,0) = 1  BEGIN" +
        //            " BEGIN  WITH T(SKUNUMBER,ITEMDESC,QTY,PRICE,GROSSWT,DISCOUNT,TAX,AMOUNT)" +
        //            " AS(SELECT A.ITEMID AS SKUNUMBER, F.NAME AS ITEMDESC,B.PIECES AS QTY," +
        //            " (ABS(CAST(ISNULL(A.NETAMOUNT,0)AS DECIMAL(28,2))) + ABS(CAST(ISNULL(A.DISCAMOUNT,0)AS DECIMAL(18,2)))" +
        //            "  + ABS(CAST(ISNULL(A.TOTALDISCAMOUNT,0)AS DECIMAL(18,2))) )AS PRICE,ISNULL(B.QUANTITY,0) AS GROSSWT," +
        //            " (ABS(CAST(ISNULL(A.DISCAMOUNT,0)AS DECIMAL(18,2))) + ABS(CAST(ISNULL(A.TOTALDISCAMOUNT,0)AS DECIMAL(18,2))) )AS DISCOUNT," +
        //            "  ABS(CAST(ISNULL(A.TAXAMOUNT,0)AS DECIMAL(18,2)))AS TAX," +
        //            " (ABS(CAST(ISNULL(A.TAXAMOUNT,0)AS DECIMAL(18,2))) + ABS(CAST(ISNULL(A.NETAMOUNT,0)AS DECIMAL(28,2))))AS AMOUNT" +
        //            " FROM RETAILTRANSACTIONSALESTRANS A INNER JOIN RETAIL_CUSTOMCALCULATIONS_TABLE B ON A.TRANSACTIONID = B.TRANSACTIONID AND A.LINENUM = B.LINENUM" +
        //            " INNER JOIN INVENTTABLE D ON A.ITEMID = D.ITEMID  LEFT OUTER JOIN ECORESPRODUCT E ON D.PRODUCT = E.RECID" +
        //            " LEFT OUTER JOIN ECORESPRODUCTTRANSLATION F ON E.RECID = F.PRODUCT" +
        //            " WHERE A.transactionid = @TRANSACTIONID AND A.TransactionStatus = 0)," +
        //        //" AND B.TRANSACTIONTYPE = 0 )," +
        //            " M(SKUNUMBER,NETWT) AS (SELECT X.SKUNUMBER,SUM(ISNULL(X.QTY,0)) AS NETWT" +
        //            " FROM RETAIL_SALE_INGREDIENTS_DETAILS X INNER JOIN INVENTTABLE Y ON X.ITEMID = Y.ITEMID" +
        //            " where transactionid = @TRANSACTIONID AND Y.METALTYPE IN (1,2,3,13) GROUP BY X.SKUNUMBER)," +
        //            " D(SKUNUMBER,DIAWT) AS(SELECT X.SKUNUMBER,SUM(ISNULL(X.QTY,0)) AS DIAWT" +
        //            " FROM RETAIL_SALE_INGREDIENTS_DETAILS X INNER JOIN INVENTTABLE Y ON X.ITEMID = Y.ITEMID" +
        //            " where transactionid = @TRANSACTIONID  AND Y.METALTYPE IN (5,12) GROUP BY X.SKUNUMBER)," +
        //            " S(SKUNUMBER,STNWT) AS(SELECT X.SKUNUMBER,SUM(ISNULL(X.QTY,0)) AS STNWT" +
        //            " FROM RETAIL_SALE_INGREDIENTS_DETAILS X INNER JOIN INVENTTABLE Y ON X.ITEMID = Y.ITEMID" +
        //            " where transactionid = @TRANSACTIONID AND Y.METALTYPE = 7 GROUP BY X.SKUNUMBER)" +
        //            " SELECT T.SKUNUMBER,T.ITEMDESC,T.QTY,T.PRICE,T.GROSSWT," +
        //            " (CASE WHEN ISNULL(M.NETWT,0)= 0 THEN T.GROSSWT ELSE M.NETWT END)AS NETWT," +
        //           // " ISNULL(M.NETWT,0)AS NETWT," +
        //           // " ISNULL(D.DIAWT,0)AS DIAWT," +
        //            " ISNULL(D.DIAWT,0)AS DIAWT,ISNULL(S.STNWT,0)AS STNWT,T.DISCOUNT,T.TAX,T.AMOUNT" +
        //            " FROM T LEFT OUTER JOIN M ON T.SKUNUMBER = M.SKUNUMBER  LEFT OUTER JOIN D ON T.SKUNUMBER = D.SKUNUMBER" +
        //            " LEFT OUTER JOIN S ON T.SKUNUMBER = S.SKUNUMBER  END";
        //            //" ELSE   BEGIN " +
        //            //" SELECT A.ITEMID AS SKUNUMBER, F.NAME AS ITEMDESC,B.PIECES AS QTY," +
        //            //" (ABS(CAST(ISNULL(A.NETAMOUNT,0)AS DECIMAL(28,2))) + ABS(CAST(ISNULL(A.DISCAMOUNT,0)AS DECIMAL(18,2)))" +
        //            //" + ABS(CAST(ISNULL(A.TOTALDISCAMOUNT,0)AS DECIMAL(18,2))) )AS PRICE," +
        //            //" ISNULL(B.QUANTITY,0) AS GROSSWT,ISNULL(B.QUANTITY,0) AS NETWT,0 AS DIAWT, 0 AS STNWT," +
        //            //" (ABS(CAST(ISNULL(A.DISCAMOUNT,0)AS DECIMAL(18,2))) + ABS(CAST(ISNULL(A.TOTALDISCAMOUNT,0)AS DECIMAL(18,2))))AS DISCOUNT," +
        //            //" ABS(CAST(ISNULL(A.TAXAMOUNT,0)AS DECIMAL(18,2)))AS TAX," +
        //            //" (ABS(CAST(ISNULL(A.TAXAMOUNT,0)AS DECIMAL(18,2))) + ABS(CAST(ISNULL(A.NETAMOUNT,0)AS DECIMAL(28,2))))AS AMOUNT" +
        //            //" FROM RETAILTRANSACTIONSALESTRANS A  INNER JOIN RETAIL_CUSTOMCALCULATIONS_TABLE B ON A.TRANSACTIONID = B.TRANSACTIONID AND A.LINENUM = B.LINENUM" +
        //            //" INNER JOIN INVENTTABLE D ON A.ITEMID = D.ITEMID LEFT OUTER JOIN ECORESPRODUCT E ON D.PRODUCT = E.RECID" +
        //            //" LEFT OUTER JOIN ECORESPRODUCTTRANSLATION F ON E.RECID = F.PRODUCT WHERE A.transactionid = @TRANSACTIONID" +
        //            //" AND A.TransactionStatus = 0 AND B.TRANSACTIONTYPE = 0  END";	            


        //    SqlCommand command = new SqlCommand(sqlSalesDtl, connection);
        //    command.Parameters.Clear();
        //    command.Parameters.Add("@TRANSACTIONID", SqlDbType.NVarChar).Value = sInvoiceNo;//"0000002495";

        //    //     command.Parameters.Add(new SqlParameter("@fromdate", Convert.ToDateTime(dateTimePicker1.Text)));
        //    //    command.Parameters.Add(new SqlParameter("@todate", Convert.ToDateTime(dateTimePicker2.Text)));
        //    //    command.Parameters.Add(new SqlParameter("@terminal", (string.IsNullOrEmpty(textBox1.Text)) ? "null" : textBox1.Text));

        //    SqlDataAdapter salesInvAdapter = new SqlDataAdapter(command);

        //    salesInvAdapter.Fill(dsSalesOrder, "SaleInv");
        //}
        #endregion

        private void GetSalesInvData(ref DataTable dtCol)
        {
            string sIngrDetails = string.Empty;

            string sQuery = "GETSALESORPURCHASEINVOICE";
            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@TRANSID", SqlDbType.NVarChar).Value = sInvoiceNo;
            command.Parameters.Add("@TerminalID", SqlDbType.NVarChar).Value = sTerminal;
            command.Parameters.Add("@Language", SqlDbType.NVarChar).Value = GetCompanyNativeLanguage(iInvLang);
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daCol = new SqlDataAdapter(command);
            daCol.Fill(dtCol);


            DataTable dtIng = new DataTable();


            for (int i = 0; i <= dtCol.Rows.Count - 1; i++) //.Select("SomeIntColumn > 0")
            {

                if (i == 0)
                    sOperatorId = getStaffName(Convert.ToString(dtCol.Rows[i]["Staff"]));

                dTotAmt = dTotAmt + Convert.ToDouble(decimal.Round(Convert.ToDecimal(dtCol.Rows[i]["AMOUNT"]), 2, MidpointRounding.AwayFromZero));

                dtIng = GetIngredientItemData(sInvoiceNo, Convert.ToString(dtCol.Rows[i]["SKUNUMBER"]));
                decimal tax = (Convert.ToDecimal(dtCol.Rows[i]["Tax"]));
                if (tax != 0)
                {
                    totTaxableamt += Math.Abs(Convert.ToDecimal(dtCol.Rows[i]["AMOUNT"]));
                    totTaxAmt += (Convert.ToDecimal(dtCol.Rows[i]["Tax"]));
                }


                sIngrDetails = "";
                decimal dTotIngStnWt = 0;
                decimal dTotIngDmdWt = 0;

                for (int j = 0; j <= dtIng.Rows.Count - 1; j++)
                {
                    int iMetalType = GetMetalType(Convert.ToString(dtIng.Rows[j]["ITEMID"]));

                    if (iMetalType == (int)MetalType.Stone)
                        dTotIngStnWt += Convert.ToDecimal(dtIng.Rows[j]["QTY"]);
                    else if (iMetalType == (int)MetalType.LooseDmd)
                        dTotIngDmdWt += Convert.ToDecimal(dtIng.Rows[j]["QTY"]);
                }
                if (dTotIngStnWt > 0)
                {
                    sIngrDetails = "Stone" + " : " + Convert.ToString(dTotIngStnWt);
                    if (dTotIngDmdWt > 0)
                    {
                        if (!string.IsNullOrEmpty(sIngrDetails))
                            sIngrDetails = sIngrDetails + ", Diamond" + " : " + Convert.ToString(dTotIngDmdWt);
                    }
                }
                else if (dTotIngDmdWt > 0)
                {
                    if (!string.IsNullOrEmpty(sIngrDetails))
                        sIngrDetails = sIngrDetails + ", Diamond" + " : " + Convert.ToString(dTotIngDmdWt);
                    else
                        sIngrDetails = "Diamond" + " : " + Convert.ToString(dTotIngDmdWt);
                }

                dtCol.Rows[i]["ITEMDESC"] = dtCol.Rows[i]["ITEMDESC"] + " " + sIngrDetails;

                dtCol.AcceptChanges();
            }
        }

        protected int GetMetalType(string sItemId)
        {
            int iMetalType = 100;
            SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT METALTYPE FROM INVENTTABLE WHERE ITEMID='" + sItemId + "'");

            if (SqlCon.State == ConnectionState.Closed)
                SqlCon.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), SqlCon);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    iMetalType = (int)reader.GetValue(0);
                }
            }
            if (SqlCon.State == ConnectionState.Open)
                SqlCon.Close();
            return iMetalType;

        }

        public DataTable GetIngredientItemData(string sTransId, string sSKUNo)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = "SELECT ITEMID,QTY  FROM  RETAIL_SALE_INGREDIENTS_DETAILS WHERE TRANSACTIONID = '" + sTransId + "'  and SKUNUMBER ='" + sSKUNo + "'";

                DataTable dt = new DataTable();

                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(dt);

                return dt;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void GetSubTotal(ref DataSet dsSTotal)
        {
            string sqlsubDtl = "   SELECT ABS(CAST(ISNULL(A.NETAMOUNT,0) AS DECIMAL(28,2))) AS NETAMOUNT" +
            " ,ABS(CAST(ISNULL(A.GROSSAMOUNT,0) AS DECIMAL(28,2))) AS GROSSAMOUNT" +
            " ,ABS(CAST(ISNULL(A.PAYMENTAMOUNT,0) AS DECIMAL(28,2))) AS PAYMENTAMOUNT " +
            " ,(ABS(CAST(ISNULL(A.DISCAMOUNT,0) AS DECIMAL(18,2))) " +
            "  + ABS(CAST(ISNULL(A.TOTALDISCAMOUNT,0) AS DECIMAL(18,2)))" +
             " )AS DISCAMOUNT," +
                //" ABS(CAST(ISNULL(A.SALESPAYMENTDIFFERENCE,0) AS DECIMAL(28,2)))AS PaymentDiffAmt" +
            " (CAST(ISNULL(A.SALESPAYMENTDIFFERENCE,0) AS DECIMAL(28,2))* (-1)) AS PaymentDiffAmt" +
            " FROM RETAILTRANSACTIONTABLE A where transactionid = @TRANSACTIONID AND A.TERMINAL = '" + sTerminal + "' AND A.TYPE <> 1";

            SqlCommand command = new SqlCommand(sqlsubDtl, connection);
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@TRANSACTIONID", SqlDbType.NVarChar).Value = sInvoiceNo;

            SqlDataAdapter daSTotal = new SqlDataAdapter(command);

            daSTotal.Fill(dsSTotal, "SubTotal");

            if (dsSTotal != null && dsSTotal.Tables[0].Rows.Count > 0)
            {
                dTotAmt = dTotAmt + Convert.ToDouble(dsSTotal.Tables[0].Rows[0]["PaymentDiffAmt"]);
            }
        }

        private void GetTender(ref DataSet dsSTender)
        {
            string sStoreNo = ApplicationSettings.Terminal.StoreId;

            string sqlsubDtl = " DECLARE @CHANNEL bigint  SELECT @CHANNEL = ISNULL(RECID,0) FROM RETAILSTORETABLE WHERE STORENUMBER = '" + sStoreNo + "'" +
                                "  SELECT N.NAME + ' (' + M.CURRENCY + ')' as NAME, M.TENDERTYPE,CAST(ISNULL(M.AMOUNTCUR,0) AS DECIMAL(28,2)) AS AMOUNT," +
                                " (ISNULL(CARDORACCOUNT,'') + ISNULL(CREDITVOUCHERID,'') + ISNULL(stuff(ISNULL(GIFTCARDID,''),1,LEN(ISNULL(GIFTCARDID,''))-4,REPLICATE('x', LEN(ISNULL(GIFTCARDID,''))-4)),'')) AS CARDNO" + //+ ISNULL(GIFTCARDID,'') ADDED ON 16/07/2014
                                " FROM RETAILTRANSACTIONPAYMENTTRANS M" +
                                " LEFT JOIN RETAILSTORETENDERTYPETABLE N ON M.TENDERTYPE = N.TENDERTYPEID" +
                                " WHERE M.TRANSACTIONID = @TRANSACTIONID AND N.CHANNEL = @CHANNEL  AND M.TERMINAL = '" + sTerminal + "' AND M.TRANSACTIONSTATUS = 0 " +
                //" UNION SELECT F.NAME AS NAME,100 AS TENDERTYPE, (ABS(CAST(ISNULL(A.TAXAMOUNT,0)AS DECIMAL(18,2)))" +
                                " UNION ALL SELECT F.NAME AS NAME,100 AS TENDERTYPE, (ABS(CAST(ISNULL(A.TAXAMOUNT,0)AS DECIMAL(18,2)))" +
                                " + ABS(CAST(ISNULL(A.NETAMOUNT,0)AS DECIMAL(28,2))))AS AMOUNT," +
                                " + ' TransactionId : ' + b.ADVANCEADJUSTMENTID " +
                                " + ' Receipt : ' + (select RECEIPTID from RETAILTRANSACTIONPAYMENTTRANS where TRANSACTIONID=b.ADVANCEADJUSTMENTID) " +
                                " CARDNO" +
                                " FROM RETAILTRANSACTIONSALESTRANS A INNER JOIN RETAIL_CUSTOMCALCULATIONS_TABLE B ON A.TRANSACTIONID = B.TRANSACTIONID AND A.LINENUM = B.LINENUM AND A.TERMINALID = B.TERMINALID" +
                                " INNER JOIN INVENTTABLE D ON A.ITEMID = D.ITEMID  LEFT OUTER JOIN ECORESPRODUCT E ON D.PRODUCT = E.RECID " +
                                " LEFT OUTER JOIN ECORESPRODUCTTRANSLATION F ON E.RECID = F.PRODUCT" +
                                " WHERE A.transactionid = @TRANSACTIONID AND A.TERMINALID = '" + sTerminal + "'" +
                                " AND A.TransactionStatus = 0  AND D.ITEMTYPE = 2" +

                                " ORDER BY M.TENDERTYPE ";


            SqlCommand command = new SqlCommand(sqlsubDtl, connection);
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@TRANSACTIONID", SqlDbType.NVarChar).Value = sInvoiceNo;

            SqlDataAdapter daSTotal = new SqlDataAdapter(command);

            daSTotal.Fill(dsSTender, "Tender");
        }

        private void GetStdRate(ref DataSet dsSStdRate)
        {
            string sqlStdRate = " SELECT TOP 1 CAST(RATES AS decimal (16,2)) AS STDGOLDRATE," +
                                " (SELECT DEFAULTCONFIGIDGOLD FROM RETAILPARAMETERS WHERE DATAAREAID = @DATAAREAID) AS BASECONFIG " +
                                " FROM METALRATES WHERE INVENTLOCATIONID= @INVENTLOCATIONID AND METALTYPE=1 AND RETAIL=1 AND RATETYPE = 3" +
                                " AND ACTIVE=1 AND CONFIGIDSTANDARD = (SELECT DEFAULTCONFIGIDGOLD FROM RETAILPARAMETERS WHERE DATAAREAID = @DATAAREAID)" +
                                " ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC";

            SqlCommand command = new SqlCommand(sqlStdRate, connection);
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@INVENTLOCATIONID", SqlDbType.NVarChar).Value = sInventLocationId;
            command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar).Value = sDataAreaId;

            //     command.Parameters.Add(new SqlParameter("@fromdate", Convert.ToDateTime(dateTimePicker1.Text)));
            //    command.Parameters.Add(new SqlParameter("@todate", Convert.ToDateTime(dateTimePicker2.Text)));
            //    command.Parameters.Add(new SqlParameter("@terminal", (string.IsNullOrEmpty(textBox1.Text)) ? "null" : textBox1.Text));

            SqlDataAdapter daSTotal = new SqlDataAdapter(command);

            daSTotal.Fill(dsSStdRate, "STDRate");
        }

        private void GetTaxInfo(ref DataSet dsSTaxInfo)
        {
            string sqlStdRate = " DECLARE @TINCST AS NVARCHAR(20)  DECLARE @TINVAT AS NVARCHAR(20)  SELECT @TINCST = ISNULL(A.REGISTRATIONNUMBER,'') + ISNULL(A.TTYPE,'')" +
                               " FROM TAXREGISTRATIONNUMBERS_IN AS A, TAXINFORMATION_IN AS B, INVENTLOCATIONLOGISTICSLOCATION AS C," +
                               " INVENTLOCATION AS D,RETAILCHANNELTABLE AS E WHERE D.INVENTLOCATIONID=E.INVENTLOCATION AND C.INVENTLOCATION=D.RECID" +
                               " AND B.ISPRIMARY=1" + // ADDED ON 02/06/2014
                               " AND C.ISPRIMARY=1" + // ADDED ON 02/06/2014
                               " AND B.REGISTRATIONLOCATION=C.LOCATION AND B.SALESTAXREGISTRATIONNUMBER=A.RECID AND D.INVENTLOCATIONID = @INVENTLOCATIONID  " +
                               "  SELECT @TINVAT = ISNULL(A.REGISTRATIONNUMBER,'') + ISNULL(A.TTYPE,'') FROM TAXREGISTRATIONNUMBERS_IN AS A, TAXINFORMATION_IN AS B, INVENTLOCATIONLOGISTICSLOCATION AS C," +
                               " INVENTLOCATION AS D,RETAILCHANNELTABLE AS E WHERE D.INVENTLOCATIONID=E.INVENTLOCATION AND C.INVENTLOCATION=D.RECID" +
                               " AND B.ISPRIMARY=1" + // ADDED ON 02/06/2014
                               " AND C.ISPRIMARY=1" + // ADDED ON 02/06/2014
                               " AND B.REGISTRATIONLOCATION=C.LOCATION AND B.[TIN]=A.RECID" +
                               " AND D.INVENTLOCATIONID = @INVENTLOCATIONID  SELECT ISNULL(@TINCST,'') AS TINCST,ISNULL(@TINVAT,'') AS TINVAT";

            SqlCommand command = new SqlCommand(sqlStdRate, connection);
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@INVENTLOCATIONID", SqlDbType.NVarChar).Value = sInventLocationId;
            //command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar).Value = sDataAreaId;

            //     command.Parameters.Add(new SqlParameter("@fromdate", Convert.ToDateTime(dateTimePicker1.Text)));
            //    command.Parameters.Add(new SqlParameter("@todate", Convert.ToDateTime(dateTimePicker2.Text)));
            //    command.Parameters.Add(new SqlParameter("@terminal", (string.IsNullOrEmpty(textBox1.Text)) ? "null" : textBox1.Text));

            SqlDataAdapter daTax = new SqlDataAdapter(command);

            daTax.Fill(dsSTaxInfo, "TaxInfo");
        }

        private void GetPayInfo(ref DataSet dsSPaymentInfo)
        {
            DataSet dsTemp = new DataSet();

            string sqlsubDtl = " SELECT (ISNULL(B.DESCRIPTION,'') + ' : '+ ISNULL(A.INFORMATION,'')) AS DTLPAYINFO, A.TYPE,A.INFOCODEID,A.PARENTLINENUM,A.TRANSACTIONID" +
                                " ,CAST(ISNULL(A.AMOUNT,0) AS DECIMAL(28,2)) AS AMOUNT" + //R.AMOUNTCUR ->  A.AMOUNT ADDED RHossain on 21/04/2014
                                " FROM [dbo].[RETAILTRANSACTIONINFOCODETRANS] A" +
                                " INNER JOIN	 RETAILINFOCODETABLE B ON A.INFOCODEID = B.INFOCODEID" +
                                " INNER JOIN RETAILTRANSACTIONPAYMENTTRANS R ON A.TRANSACTIONID = R.TRANSACTIONID" +
                                " AND A.TERMINAL=R.TERMINAL AND A.PARENTLINENUM = R.LINENUM" + //AND A.TERMINAL=R.TERMINAL -->ADDED RHossain on 21/04/2014
                                " WHERE A.TRANSACTIONID = @TRANSACTIONID AND R.TERMINAL = '" + sTerminal + "' ORDER BY A.PARENTLINENUM,A.INFOCODEID";

            SqlCommand command = new SqlCommand(sqlsubDtl, connection);
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@TRANSACTIONID", SqlDbType.NVarChar).Value = sInvoiceNo;
            SqlDataAdapter daSTotal = new SqlDataAdapter(command);
            daSTotal.Fill(dsTemp, "PaymentInfo");
            DataTable dt = new DataTable();
            DataRow dr;
            if (dsTemp != null && dsTemp.Tables.Count > 0 && dsTemp.Tables[0].Rows.Count > 0)
            {
                string sPayInfo = string.Empty;
                dt.Columns.Add("PAYINFO", typeof(string));

                for (int i = 1; i <= dsTemp.Tables[0].Rows.Count; i++)
                {
                    if (i == 1)
                    {
                        sPayInfo = Convert.ToString(dsTemp.Tables[0].Rows[i - 1]["DTLPAYINFO"]);
                    }
                    else
                    {
                        if (Convert.ToInt32(dsTemp.Tables[0].Rows[i - 1]["PARENTLINENUM"])
                            != Convert.ToInt32(dsTemp.Tables[0].Rows[i - 2]["PARENTLINENUM"]))
                        {
                            // sPayInfo = sPayInfo + "; Amount : " + Convert.ToString(dsTemp.Tables[0].Rows[i - 2]["AMOUNT"]);  //commented on 21/04/2014 Req by S.Sarma
                            dr = dt.NewRow();
                            dr["PAYINFO"] = sPayInfo;
                            dt.Rows.Add(dr);
                            dt.AcceptChanges();
                            sPayInfo = Convert.ToString(dsTemp.Tables[0].Rows[i - 1]["DTLPAYINFO"]);
                        }
                        else
                        {
                            sPayInfo = sPayInfo + "; " + Convert.ToString(dsTemp.Tables[0].Rows[i - 1]["DTLPAYINFO"]);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(sPayInfo))
                {
                    //sPayInfo = sPayInfo + "; Amount : " + Convert.ToString(dsTemp.Tables[0].Rows[dsTemp.Tables[0].Rows.Count - 1]["AMOUNT"]);
                    dr = dt.NewRow();
                    dr["PAYINFO"] = sPayInfo;
                    dt.Rows.Add(dr);
                    dt.AcceptChanges();
                    sPayInfo = string.Empty;
                }
            }
            dsSPaymentInfo.Tables.Add(dt);
        }

        private void GetStoreInfo(ref string sStorePh, ref string sInvFooter, ref string sCINNo, ref string sFooterArabicNote)
        {
            string sql = " SELECT ISNULL(STORECONTACT,'-') AS STORECONTACT, ISNULL(INVOICEFOOTERNOTE,'') AS INVOICEFOOTERNOTE," +
                " ISNULL(CINNO,'') as CINNO,ISNULL(INVOICEFOOTERNOTEARABIC,'') AS INVOICEFOOTERNOTEARABIC,RETAILSTORENAME_ARABIC,RETAILSTOREADD_ARABIC"+
                " FROM RETAILSTORETABLE WHERE STORENUMBER='" + ApplicationSettings.Terminal.StoreId + "'";

            DataTable dtStoreInfo = new DataTable();
            SqlCommand command = new SqlCommand(sql, connection);
            command.CommandTimeout = 0;
            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dtStoreInfo);

            if (dtStoreInfo != null && dtStoreInfo.Rows.Count > 0)
            {
                if (Convert.ToString(dtStoreInfo.Rows[0]["STORECONTACT"]) == string.Empty)
                    sStorePh = "...";
                else
                    sStorePh = Convert.ToString(dtStoreInfo.Rows[0]["STORECONTACT"]);
                if (Convert.ToString(dtStoreInfo.Rows[0]["INVOICEFOOTERNOTE"]) == string.Empty)
                    sInvFooter = "-";
                else
                    sInvFooter = Convert.ToString(dtStoreInfo.Rows[0]["INVOICEFOOTERNOTE"]);

                if (Convert.ToString(dtStoreInfo.Rows[0]["CINNO"]) == string.Empty)
                    sCINNo = "-";
                else
                    sCINNo = Convert.ToString(dtStoreInfo.Rows[0]["CINNO"]);

                if (Convert.ToString(dtStoreInfo.Rows[0]["INVOICEFOOTERNOTEARABIC"]) == string.Empty)
                    sFooterArabicNote = "-";
                else
                    sFooterArabicNote = Convert.ToString(dtStoreInfo.Rows[0]["INVOICEFOOTERNOTEARABIC"]);

                if (Convert.ToString(dtStoreInfo.Rows[0]["RETAILSTORENAME_ARABIC"]) == string.Empty)
                    sArStoreName = "-";
                else
                    sArStoreName = Convert.ToString(dtStoreInfo.Rows[0]["RETAILSTORENAME_ARABIC"]);

                if (Convert.ToString(dtStoreInfo.Rows[0]["RETAILSTOREADD_ARABIC"]) == string.Empty)
                    sArStoreAddress = "-";
                else
                    sArStoreAddress = Convert.ToString(dtStoreInfo.Rows[0]["RETAILSTOREADD_ARABIC"]);
            }
        }

        private void GetTaxDetail(ref DataSet dsTaxDetail) // added on 31/03/2014 RHossain for  show the tax detail in line
        {
            //string sqlsubDtl = " select TAXCODE,ABS(CAST(ISNULL(sum(Amount),0)AS DECIMAL(18,2)))AS TAX from RETAILTRANSACTIONTAXTRANS " +
            //                   " where TRANSACTIONID = @TRANSACTIONID" +
            //                   " group by TRANSACTIONID,TERMINALID,STOREID ,TAXCODE";


            string sqlsubDtl = "select TAXCODE,ABS(CAST(ISNULL(sum(Amount),0)AS DECIMAL(18,2)))AS TAX" +
                              "  from RETAILTRANSACTIONTAXTRANS A " +
                              " join RETAILTRANSACTIONSALESTRANS B " +
                              "  on A.TRANSACTIONID =B.TRANSACTIONID and A.SALELINENUM =B.LINENUM " +
                              "  and B.TRANSACTIONSTATUS =0" +
                              " and A.TERMINALID=b.TERMINALID and A.STOREID =B.STORE " +
                              "  where A.TRANSACTIONID =@TRANSACTIONID and B.TERMINALID='" + sTerminal + "' " +
                              " group by A.TRANSACTIONID,A.TERMINALID,STOREID ,taxcode ";


            SqlCommand command = new SqlCommand(sqlsubDtl, connection);
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@TRANSACTIONID", SqlDbType.NVarChar).Value = sInvoiceNo;

            //     command.Parameters.Add(new SqlParameter("@fromdate", Convert.ToDateTime(dateTimePicker1.Text)));
            //    command.Parameters.Add(new SqlParameter("@todate", Convert.ToDateTime(dateTimePicker2.Text)));
            //    command.Parameters.Add(new SqlParameter("@terminal", (string.IsNullOrEmpty(textBox1.Text)) ? "null" : textBox1.Text));

            SqlDataAdapter daSTaxDetail = new SqlDataAdapter(command);

            daSTaxDetail.Fill(dsTaxDetail, "Tender");

            for (int i = 0; i <= dsTaxDetail.Tables[0].Rows.Count - 1; i++)
            {
                dTotAmt = dTotAmt + Convert.ToDouble(dsTaxDetail.Tables[0].Rows[i]["TAX"]);
            }
        }


        /// <summary>
        /// Purpose : Invoice title change whether it is sales invoice or others.
        /// Created Date : 07/04/2014
        /// Created By : RHossain
        /// </summary>
        /// <param name="sTransId"></param>
        /// <returns></returns>
        private string GetCustomTransType(string sTransId, string sTerminalId) // Sales/Purchase/Exchange.....
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT TRANSACTIONTYPE FROM RETAIL_CUSTOMCALCULATIONS_TABLE WHERE TRANSACTIONID='" + sTransId + "' and STOREID='" + ApplicationSettings.Terminal.StoreId + "' and TERMINALID='" + sTerminalId + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
            {
                return sResult.Trim();
            }
            else
            {
                return "-";
            }
        }

        /// <summary>
        /// Created on 12/05/2014
        /// Purppose: After craeting sales invoice, print SalesInvoice from sjow journal with local customer
        /// </summary>
        /// <param name="sTransId"></param>
        private void GetLocalCustomerInfo(string sTransId)
        {

            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;


            string sql = "select ISNULL(LocalCustomerName,'') AS LocalCustomerName," +
                         " ISNULL(LocalCustomerAddress,'') AS LocalCustomerAddress," +
                         " ISNULL(LocalCustomerContactNo,'') AS LocalCustomerContactNo " +
                         " FROM RETAILTRANSACTIONTABLE " +
                         " WHERE TRANSACTIONID='" + sTransId + "'";
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            DataTable dtCustInfo = new DataTable();
            SqlCommand command = new SqlCommand(sql, conn);
            command.CommandTimeout = 0;
            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dtCustInfo);

            if (dtCustInfo != null && dtCustInfo.Rows.Count > 0)
            {
                if (Convert.ToString(dtCustInfo.Rows[0]["LocalCustomerName"]) == string.Empty)
                    sCustName = "-";
                else
                    sCustName = Convert.ToString(dtCustInfo.Rows[0]["LocalCustomerName"]);
                if (Convert.ToString(dtCustInfo.Rows[0]["LocalCustomerAddress"]) == string.Empty)
                    sCustAddress = "-";
                else
                    sCustAddress = Convert.ToString(dtCustInfo.Rows[0]["LocalCustomerAddress"]);

                if (Convert.ToString(dtCustInfo.Rows[0]["LocalCustomerContactNo"]) == string.Empty)
                    sContactNo = "-";
                else
                    sContactNo = Convert.ToString(dtCustInfo.Rows[0]["LocalCustomerContactNo"]);
            }
        }

        private void GetCompanyLogo(ref DataSet dsCompanyLogo)
        {
            string sqlStdRate = "SELECT [IMAGE] as COMPLOGO FROM CompanyImage WHERE DATAAREAID = @DATAAREAID";

            SqlCommand command = new SqlCommand(sqlStdRate, connection);
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar).Value = ApplicationSettings.Database.DATAAREAID;

            SqlDataAdapter daCL = new SqlDataAdapter(command);

            daCL.Fill(dsCompanyLogo, "CompanyLogo");
        }

        private string getStoreEmailAndFax(string sFieldName) // PrimaryContactEmail/PrimaryContactFax
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            string commandText = string.Empty;

            commandText = " select LOCATOR from LOGISTICSELECTRONICADDRESS A" +
                          " left join DIRPARTYTABLE P on a.RECID=P." + sFieldName + "" +
                          "  left join RETAILCHANNELTABLE c on p.RECID= c.OMOPERATINGUNITID" +
                          "  left join RETAILSTORETABLE s on c.RECID=s.RECID" +
                          "  where s.STORENUMBER = '" + ApplicationSettings.Terminal.StoreId + "'";

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
            {
                return sResult.Trim();
            }
            else
            {
                return "-";
            }
        }

        private string getStaffName(string sOpId)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            string commandText = string.Empty;

            commandText = "select d.name as Name from RETAILSTAFFTABLE r  " +
                        " left join dbo.HCMWORKER as h on h.PERSONNELNUMBER = r.STAFFID " +
                        " left join dbo.DIRPARTYTABLE as d on d.RECID = h.PERSON " +
                        "  where R.STAFFID = '" + sOpId + "'";

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
            {
                return sResult.Trim();
            }
            else
            {
                return "-";
            }
        }

        private string GetArchivePathFromImage()
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select TOP 1 ARCHIVEPATH from RETAILSTORETABLE  WHERE STORENUMBER='" + ApplicationSettings.Database.StoreID + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
                return sResult.Trim();
            else
                return "-";
        }

        private string GetCompanyNativeLanguage(int iL) // Sales/Purchase/Exchange.....
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            if (iL == 1)
                commandText.Append("SELECT TOP 1 CULTURENAME  FROM RETAILSTORETABLE  WHERE  STORENUMBER='" + ApplicationSettings.Database.StoreID + "'");
            //else if(iL == 4)
            //    commandText.Append("SELECT TOP 1 FRENCHLANGUAGE FROM COMPANYINFO WHERE  DATAAREA='" + ApplicationSettings.Database.DATAAREAID + "'");
            else
                commandText.Append("SELECT TOP 1 NATIVELANGUAGE FROM COMPANYINFO WHERE  DATAAREA='" + ApplicationSettings.Database.DATAAREAID + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
                return sResult.Trim();
            else
                return "-";
        }

        private string GetCompanyTRN()
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();

            commandText.Append("SELECT TOP 1 COREGNO FROM COMPANYINFO WHERE  DATAAREA='" + ApplicationSettings.Database.DATAAREAID + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
                return sResult.Trim();
            else
                return "-";
        }

        private void frmSaleInvControlCopy_Load(object sender, EventArgs e)
        {
            //// TODO: This line of code loads data into the 'DSSaleInv.Detail' table. You can move, or remove it, as needed.
            //this.DetailTableAdapter.Fill(this.DSSaleInv.Detail);
            //// TODO: This line of code loads data into the 'DSSaleInv.SubTotal' table. You can move, or remove it, as needed.
            //this.SubTotalTableAdapter.Fill(this.DSSaleInv.SubTotal);
            //// TODO: This line of code loads data into the 'DSSaleInv.Tender' table. You can move, or remove it, as needed.
            //this.TenderTableAdapter.Fill(this.DSSaleInv.Tender);
            //// TODO: This line of code loads data into the 'DSSaleInv.StdRate' table. You can move, or remove it, as needed.
            //this.StdRateTableAdapter.Fill(this.DSSaleInv.StdRate);
            //// TODO: This line of code loads data into the 'DSSaleInv.TaxInfo' table. You can move, or remove it, as needed.
            //this.TaxInfoTableAdapter.Fill(this.DSSaleInv.TaxInfo);

            reportViewer1.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = reportViewer1.LocalReport;
            if (iInvLang == 1 && iGiftPrint == 0)
                localReport.ReportPath = "RptSaleInv.rdlc";
            else if (iInvLang == 2)
                localReport.ReportPath = "RptSaleInvArabic.rdlc";
            else if (iGiftPrint == 1)
                localReport.ReportPath = "RptSaleInv_Gift.rdlc";
            else if (iInvLang == 3)//both
                localReport.ReportPath = "rptSaleInvBoth.rdlc";
            else
                localReport.ReportPath = "rptSaleInvBoth.rdlc";


            //localReport.ReportPath = @"Microsoft.Dynamics.Retail.Pos.BlankOperations.Report." + reportName + ".rdlc";


            DataSet dataset = new DataSet();
            DataSet dsSubTotal = new DataSet();
            DataSet dsTender = new DataSet();

            DataSet dsStdRate = new DataSet();
            DataSet dsTaxInfo = new DataSet();
            DataSet dsPaymentInfo = new DataSet();

            DataSet dsTaxDetail = new DataSet(); // added on 31/03/2014 RHossain
            DataSet dsCompanyLogo = new DataSet(); // added on 27/08/2014 RHossain
            DataTable dtSalesOrPurchInv = new DataTable();
            DataTable dtDetail = new DataTable();
            string path = string.Empty;

            GetSalesInvData(ref dtSalesOrPurchInv);
            dtDetail = dtSalesOrPurchInv;
            string sTRN = GetCompanyTRN();

            # region //ADDED ON 28/10/14 RH For image show in sales voucher



            dtDetail.Columns.Add("ITEMIMAGE", typeof(string));
            int i = 1;
            foreach (DataRow d in dtDetail.Rows)
            {
                sSaleItem = Convert.ToString(d["SKUNUMBER"]);
                #region Return SKUNUMBER from Item Description @ 12/12/2014 palas jana
                string[] ItemDesc = sSaleItem.Split(';');
                string ItemId = ItemDesc[0].Trim();
                #endregion
                //sItemParentId = GetItemParentId(sSaleItem);
                //sItemParentId = GetItemParentId(ItemId);
                if (sCustomTransType == "1")// || sCustomTransType == "2" || sCustomTransType == "3" || sCustomTransType == "4")
                {
                    //sTitle = "Purchase Invoice";

                    path = sArchivePath + "" + sReceiptNo + "_" + i + ".jpeg"; //

                    if (File.Exists(path))
                    {
                        Image img = Image.FromFile(path);
                        byte[] arr;
                        using (MemoryStream ms1 = new MemoryStream())
                        {
                            img.Save(ms1, System.Drawing.Imaging.ImageFormat.Jpeg);
                            arr = ms1.ToArray();
                        }

                        d["ITEMIMAGE"] = Convert.ToBase64String(arr);
                    }
                    else
                        d["ITEMIMAGE"] = "";

                }
                else
                {
                    // path = sArchivePath + "" + sItemParentId + "" + ".jpg"; 
                    string sSKUPath = sArchivePath + "" + ItemId + "" + ".jpg";

                    if (File.Exists(sSKUPath))
                    {
                        Image img = Image.FromFile(sSKUPath);
                        byte[] arr;
                        using (MemoryStream ms1 = new MemoryStream())
                        {
                            img.Save(ms1, System.Drawing.Imaging.ImageFormat.Jpeg);
                            arr = ms1.ToArray();
                        }

                        d["ITEMIMAGE"] = Convert.ToBase64String(arr);
                    }
                    //else if(File.Exists(path))
                    //{
                    //    Image img = Image.FromFile(path);
                    //    byte[] arr;
                    //    using(MemoryStream ms1 = new MemoryStream())
                    //    {
                    //        img.Save(ms1, System.Drawing.Imaging.ImageFormat.Jpeg);
                    //        arr = ms1.ToArray();
                    //    }

                    //    d["ITEMIMAGE"] = Convert.ToBase64String(arr);
                    //}
                    else
                        d["ITEMIMAGE"] = "";
                }
                i++;
            }
            dtDetail.AcceptChanges();
            #endregion//end


            GetSubTotal(ref dsSubTotal);
            GetTender(ref dsTender);
            GetStdRate(ref dsStdRate);
            GetTaxInfo(ref dsTaxInfo);
            GetPayInfo(ref dsPaymentInfo);

            GetTaxDetail(ref dsTaxDetail); // added on 31/03/2014 RHossain

            GetCompanyLogo(ref dsCompanyLogo);// added on 26/08/2014 RHossain

            sAmtinwds = oBlank.Amtinwds(Math.Abs(dTotAmt)); // added on 28/04/2014 RHossain     

            decimal dAmt = decimal.Round((Math.Abs(Convert.ToDecimal(dTotAmt))), 2, MidpointRounding.AwayFromZero);
            sAmtinwds = oBlank.Amtinwds(Convert.ToDouble(dAmt)); // added on 28/04/2014 RHossain               
            sAmtinwdsArabic = oBlank.AmtinwdsInArabic(Math.Abs(dTotAmt));

            if (iInvLang == 2)
                sAmtinwds = sAmtinwdsArabic;
            else if (iInvLang == 3)
                sAmtinwds = sAmtinwds + System.Environment.NewLine + "" + sAmtinwdsArabic;



            bCompImage = oBlank.GetCompLogo(Convert.ToString(ApplicationSettings.Database.DATAAREAID));// added on 26/08/2014 for MME

            sCurrencySymbol = ApplicationSettings.Terminal.StoreCurrency;// oBlank.GetCurrencySymbol();

            ReportDataSource dsSalesOrder = new ReportDataSource();
            dsSalesOrder.Name = "Detail";
            dsSalesOrder.Value = dtDetail;
            this.reportViewer1.LocalReport.DataSources.Clear();
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Detail", dtDetail));


            ReportDataSource RDSubTotal = new ReportDataSource();
            RDSubTotal.Name = "SubTotal";
            RDSubTotal.Value = dsSubTotal.Tables[0];
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SubTotal", dsSubTotal.Tables[0]));


            ReportDataSource RDTender = new ReportDataSource();
            RDTender.Name = "Tender";
            RDTender.Value = dsTender.Tables[0];
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Tender", dsTender.Tables[0]));

            ReportDataSource RDStdRate = new ReportDataSource();
            RDStdRate.Name = "StdRate";
            RDStdRate.Value = dsStdRate.Tables[0];
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("StdRate", dsStdRate.Tables[0]));

            ReportDataSource RDTaxInfo = new ReportDataSource();
            RDTaxInfo.Name = "TaxInfo";
            RDTaxInfo.Value = dsTaxInfo.Tables[0];
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("TaxInfo", dsTaxInfo.Tables[0]));

            ReportDataSource RDPaymentInfo = new ReportDataSource();
            RDPaymentInfo.Name = "PaymentInfo";
            RDPaymentInfo.Value = dsPaymentInfo.Tables[0];
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("PaymentInfo", dsPaymentInfo.Tables[0]));

            ReportDataSource RDTaxInDetail = new ReportDataSource();
            RDTaxInDetail.Name = "TaxInDetails";
            RDTaxInDetail.Value = dsTaxDetail.Tables[0];
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("TaxInDetails", dsTaxDetail.Tables[0]));


            ReportDataSource RDCompanyLogo = new ReportDataSource();
            RDCompanyLogo.Name = "CompanyLogo";
            RDCompanyLogo.Value = dsCompanyLogo.Tables[0];
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("CompanyLogo", dsCompanyLogo.Tables[0]));// added on 28/08/2014

            //this.reportViewer1.LocalReport.EnableExternalImages = true;// added on 26/08/2014


            ReportParameter[] param = new ReportParameter[31];

            param[0] = new ReportParameter("CName", sCustName);
            param[1] = new ReportParameter("CAddress", sCustAddress);
            param[2] = new ReportParameter("CPinCode", sCPinCode);
            param[3] = new ReportParameter("CContactNo", sContactNo);
            param[4] = new ReportParameter("CPanNo", sCPanNo);
            param[5] = new ReportParameter("InvoiceNo", sInvoiceNo);
            param[6] = new ReportParameter("InvDate", sInvDt);

            param[7] = new ReportParameter("StoreName", sStoreName);
            param[8] = new ReportParameter("StoreAddress", sStoreAddress);
            param[9] = new ReportParameter("StorePhone", sStorePhNo);

            if (!string.IsNullOrEmpty(sAmtinwds))
                param[10] = new ReportParameter("Amtinwds", sAmtinwds);
            else
                param[10] = new ReportParameter("Amtinwds", "zero");
            param[11] = new ReportParameter("CCode", sCustCode);
            param[12] = new ReportParameter("ReceiptNo", sReceiptNo);
            param[13] = new ReportParameter("Title", sTitle);
            param[14] = new ReportParameter("InvoiceFooter", sInvoiceFooter);
            param[15] = new ReportParameter("InvoiceTime", sTime); // added on 29/03/214 req from Sailendra Da
            param[16] = new ReportParameter("CompName", sCompanyName); // added on 14/04/214 req from Sailendra Da
            param[17] = new ReportParameter("CIN", sCINNo); // added on 14/04/214 req from Sailendra Da
            param[18] = new ReportParameter("DuplicateCopy", sDuplicateCopy); // added on 14/04/2014 req from Sailendra Da
            param[19] = new ReportParameter("cs", sCurrencySymbol); //Currencysymbol added on 02/09/2014 req from Sailendra Da
            param[20] = new ReportParameter("Cashier", sOperatorId);
            param[21] = new ReportParameter("FooterComments", sFooterComments);
            param[22] = new ReportParameter("CustEmail", sCustEmail);
            param[23] = new ReportParameter("StoreEmail", sStoreEmail);
            param[24] = new ReportParameter("StoreFax", sStoreFax);
            param[25] = new ReportParameter("TRN", sTRN);
            param[26] = new ReportParameter("totTaxableamt", Convert.ToString(totTaxableamt));
            param[27] = new ReportParameter("totTaxAmt", Convert.ToString(totTaxAmt));
            param[28] = new ReportParameter("CopyName", "Control Copy");
            param[29] = new ReportParameter("ArStoreName", sArStoreName);
            param[30] = new ReportParameter("ArStoreAddress", sArStoreAddress);

            //param[19] = new ReportParameter() // added for dynamically comp logo into report on 26/08/2014 --"CompLogo", Convert.ToBase64String(bCompImage)
            //   {
            //       Name = "CompLogo",
            //       Values = { Convert.ToBase64String(bCompImage) }
            //   };


            this.reportViewer1.LocalReport.SetParameters(param);
            this.reportViewer1.RefreshReport();

            if (isItFromShowJournal == 0)
            {
                oBlank.Export(reportViewer1.LocalReport);
                oBlank.Print_Invoice(reportViewer1.LocalReport, 1);
                this.Close();
            }
        }

    }
}
