
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using Microsoft.Reporting.WinForms;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    public partial class frmR_ProductAdvanceReceipt : Form
    {
        SqlConnection connection;
        BlankOperations oBlank = new BlankOperations();
        // string sTransactionId = "";
        string sCustName = "-";
        string sCustId = "-";
        string sCustAddress = "-";
        string sCPinCode = "-";
        string sContactNo = "-";
        string sCustEmail = "";

        string sReceiptNo = "";
        string sReceiptVoucherNo = "";
        string sAmount = "";
        string sReceiptDate = "-";

        string sStoreName = "-";
        string sStoreAddress = "-";
        string sStorePhNo = "...";
        string sArStoreName = "";
        string sArStoreAddress = "";
        string sFooterArabicNote = "";


        string sDataAreaId = "";
        string sInventLocationId = "";
        string sDetailsLine = "";
        string sBookedSkuList = "";
        string sFixedRate = "";
        string sInvoiceFooter = "-";

        string sTerminal = string.Empty;
        string sCompanyName = string.Empty; //aded on 14/04/2014 R.Hossain
        string sCINNo = string.Empty;//aded on 14/04/2014 R.Hossain
        string sRTitle = string.Empty;
        string sFooterText = string.Empty;
        string sCurrencySymbol = "";
        string sRefRceiptNo = string.Empty;
        string sStoreEmail = "";
        string sStoreFax = "";
        string sOperatorId = "";
        decimal dTaxPct = 0m;
        decimal dTaxAmt = 0m;
        decimal dExcludTaxAmt = 0m;
        decimal dBookedQty = 0m;
        int iInvLang = 0;

        string sInvoiceNo = "";
        string sTime = "";
        string sInvDt = "";
        string sTRN = "";
        decimal totTaxableamt = 0m;
        decimal totTaxAmt = 0m;
        string sAmtinwdsArabic = "";
        string sRemarks ="";

        public frmR_ProductAdvanceReceipt(SqlConnection conn)
        {
            InitializeComponent();
            connection = conn;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
        }

        public frmR_ProductAdvanceReceipt(IPosTransaction posTransaction, SqlConnection conn, string _sTransId, string _sAmt, string sTerminalId, string sGiftCardItemName = "", string sGiftCardNo = "", int iIsAdvRefund = 0,int iLanguage=0)
        {
            InitializeComponent();
            sTerminal = sTerminalId;

            #region[Param Info]
            RetailTransaction retailTrans = posTransaction as RetailTransaction;
            if (retailTrans != null)
            {
                iInvLang = iLanguage;
                if (Convert.ToString(retailTrans.Customer.Name) != string.Empty)
                    sCustName = Convert.ToString(retailTrans.Customer.Name);
                if (!string.IsNullOrEmpty(Convert.ToString(retailTrans.Customer.CustomerId)))
                    sCustId = Convert.ToString(retailTrans.Customer.CustomerId);
                if (Convert.ToString(retailTrans.Customer.Address) != string.Empty)  //PrimaryAddress
                    sCustAddress = Convert.ToString(retailTrans.Customer.Address);
                if (Convert.ToString(retailTrans.Customer.PostalCode) != string.Empty)
                    sCPinCode = Convert.ToString(retailTrans.Customer.PostalCode);
                if (Convert.ToString(retailTrans.Customer.Email) != string.Empty)
                    sCustEmail = Convert.ToString(retailTrans.Customer.Email);

                //if(Convert.ToString(retailTrans.Customer.MobilePhone) != string.Empty)
                //    sContactNo = Convert.ToString(retailTrans.Customer.MobilePhone);

                if (!string.IsNullOrEmpty(retailTrans.Customer.Telephone))
                    sContactNo = Convert.ToString(retailTrans.Customer.Telephone);

                //sCPanNo

                //-------
                if (Convert.ToString(retailTrans.TransactionId) != string.Empty)
                {
                    sReceiptNo = _sTransId;// Convert.ToString(retailTrans.TransactionId);
                    sReceiptVoucherNo = retailTrans.ReceiptId;
                }


                sInvoiceNo = sReceiptNo;

                if (retailTrans.EndDateTime != null)
                    sInvDt = retailTrans.EndDateTime.ToShortDateString();

                if (retailTrans.EndDateTime != null)
                    sTime = retailTrans.EndDateTime.ToString("hh:mm tt"); //("HH:mm")


                //if(retailTrans.BeginDateTime != null)
                //    sReceiptDate = retailTrans.BeginDateTime.ToShortDateString();

                sAmount = oBlank.Amtinwds(Convert.ToDouble(_sAmt));

                sAmtinwdsArabic = oBlank.AmtinwdsInArabic(Convert.ToDouble(_sAmt));

                if (iInvLang == 2)
                    sAmount = sAmtinwdsArabic;
                else if (iInvLang == 3)
                    sAmount = sAmount + System.Environment.NewLine + "" + sAmtinwdsArabic;


                sCurrencySymbol = oBlank.GetCurrencySymbol();
                string sAdJustItem = AdjustmentItemID();
                string ItemTaxCode = getTaxGropCode(sAdJustItem);

                dTaxPct = getTaxPctValue(ItemTaxCode); //getItemTaxPercentage();// ;
                decimal dAmt = decimal.Round(Convert.ToDecimal(_sAmt), 2, MidpointRounding.AwayFromZero);//Convert.ToDecimal(_sAmt);
                dTaxAmt = decimal.Round(Convert.ToDecimal(dAmt * dTaxPct / (100 + dTaxPct)), 2, MidpointRounding.AwayFromZero);
                dExcludTaxAmt = dAmt - dTaxAmt;


                if (string.IsNullOrEmpty(sGiftCardItemName))
                {
                    sFooterText = "Request to handover Advance Receipt at the time of billing";
                    sRTitle = "Advance Receipt" + "  " + "وصل بالدفعة المسبقة";
                    sDetailsLine = "Received with thanks from " + "" + sCustName + "                                                                         " + sCurrencySymbol + " " + _sAmt;
                }
                else if (iIsAdvRefund == 1)
                {
                    sFooterText = "";
                    sRTitle = "Advance Refund";
                    sDetailsLine = "Advance Refund" + "                                                                                                                " + sCurrencySymbol + " " + _sAmt;
                }
                else
                {
                    sFooterText = "";
                    sRTitle = "GIFT CARD RECEIPT";
                    sDetailsLine = sGiftCardItemName + "            " + sGiftCardNo + "                                                                         " + sCurrencySymbol + " " + _sAmt;
                }
                //----------store Info

                //if(Convert.ToString(retailTrans.StoreName) != string.Empty)
                //    sStoreName = Convert.ToString(retailTrans.StoreName);
                //if(Convert.ToString(retailTrans.StoreAddress) != string.Empty)
                //    sStoreAddress = Convert.ToString(retailTrans.StoreAddress);

                //if (! string.IsNullOrEmpty(Convert.ToString(retailTrans.StorePhone)))
                //    sStorePhNo = Convert.ToString(retailTrans.StorePhone);

                if (Convert.ToString(ApplicationSettings.Terminal.StoreName) != string.Empty)
                    sStoreName = Convert.ToString(ApplicationSettings.Terminal.StoreName);
                if (Convert.ToString(ApplicationSettings.Terminal.StoreAddress) != string.Empty)
                    sStoreAddress = Convert.ToString(ApplicationSettings.Terminal.StoreAddress);
                if (!string.IsNullOrEmpty(Convert.ToString(ApplicationSettings.Terminal.StorePhone)))
                    sStorePhNo = Convert.ToString(ApplicationSettings.Terminal.StorePhone);

                if (Convert.ToString(retailTrans.OperatorId) != string.Empty)
                    sOperatorId = getStaffName(Convert.ToString(retailTrans.OperatorId));

                sDataAreaId = Convert.ToString(ApplicationSettings.Database.DATAAREAID);

                // if (Convert.ToString(retailTrans.InventLocationId) != string.Empty)

                sInventLocationId = ApplicationSettings.Terminal.InventLocationId; //Convert.ToString(retailTrans.InventLocationId);

            }
            connection = conn;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            GetStoreInfo(ref sStorePhNo, ref sInvoiceFooter, ref sCINNo);

            //sCINNo = oBlank.getValue("select CINNO  from RETAILSTORETABLE where STORENUMBER ='" + Convert.ToString(ApplicationSettings.Terminal.StoreId) + "'");
            sCompanyName = oBlank.GetCompanyName(conn);//aded on 14/04/2014 R.Hossain

            if (iIsAdvRefund == 1)
            {
                sRefRceiptNo = GetRefReciptId(retailTrans.TransactionId);
            }

            sRemarks = getTransactionRemarks(retailTrans.TransactionId, retailTrans.TerminalId);

            sStorePhNo = getStoreEmailAndFax("PRIMARYCONTACTPHONE");

            sStoreEmail = getStoreEmailAndFax("PrimaryContactEmail");

            sStoreFax = getStoreEmailAndFax("PrimaryContactFax"); // PrimaryContactEmail/PrimaryContactFax

            sTRN = GetCompanyTRN();
            //sCurrencySymbol = ApplicationSettings.Terminal.StoreCurrency;

            #endregion
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

        private void frmR_ProductAdvanceReceipt_Load(object sender, EventArgs e)
        {
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = reportViewer1.LocalReport;

            if (iInvLang == 1)
            {
                if (string.IsNullOrEmpty(sRefRceiptNo))
                    localReport.ReportPath = "taxRptProductAdvanceReceiptEng.rdlc";
                else
                    localReport.ReportPath = "taxRptProductAdvanceRefundReceiptBoth.rdlc";
            }
            else if (iInvLang == 2)
            {
                if (string.IsNullOrEmpty(sRefRceiptNo))
                    localReport.ReportPath = "taxRptProductAdvanceReceiptArabic.rdlc";
                else
                    localReport.ReportPath = "taxRptProductAdvanceRefundReceiptBoth.rdlc";
            }
            else if (iInvLang == 3)
            {
                if (string.IsNullOrEmpty(sRefRceiptNo))
                    localReport.ReportPath = "taxRptProductAdvanceReceiptBoth.rdlc";
                else
                    localReport.ReportPath = "taxRptProductAdvanceRefundReceiptBoth.rdlc";
            }
            else
            {
                if (string.IsNullOrEmpty(sRefRceiptNo))
                    localReport.ReportPath = "taxRptProductAdvanceReceiptBoth.rdlc";
                else
                    localReport.ReportPath = "taxRptProductAdvanceRefundReceiptBoth.rdlc";
            }

            DataSet dsTender = new DataSet();
            DataSet dsTaxInfo = new DataSet();
            DataSet dsBookedSku = new DataSet();
            DataSet dsPaymentInfo = new DataSet();
            DataTable dtTaxAdv = new DataTable();

            GetTender(ref dsTender);
            GetTaxInfo(ref dsTaxInfo);
            GetBookedSku(ref dsBookedSku);
            getFixedMetalRate();
            GetPayInfo(ref dsPaymentInfo);
            GetTaxAdvanceData(ref dtTaxAdv);

            ReportDataSource dsGSSInstalmentReceipt = new ReportDataSource();

            dsGSSInstalmentReceipt.Name = "Tender";
            dsGSSInstalmentReceipt.Value = dsTender.Tables[0];

            this.reportViewer1.LocalReport.DataSources.Clear();
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Tender", dsTender.Tables[0]));

            ReportDataSource RDTaxInfo = new ReportDataSource();
            RDTaxInfo.Name = "TaxInfo";
            RDTaxInfo.Value = dsTaxInfo.Tables[0];
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("TaxInfo", dsTaxInfo.Tables[0]));

            ReportDataSource RDPaymentInfo = new ReportDataSource();
            RDPaymentInfo.Name = "PaymentInfo";
            RDPaymentInfo.Value = dsPaymentInfo.Tables[0];
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("PaymentInfo", dsPaymentInfo.Tables[0]));


            ReportDataSource RDTaxAdvInfo = new ReportDataSource();
            RDPaymentInfo.Name = "TAXADV";
            RDPaymentInfo.Value = dtTaxAdv;
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("TAXADV", dtTaxAdv));


            ReportParameter[] param;

            if (string.IsNullOrEmpty(sRefRceiptNo))
                param = new ReportParameter[32];
            else
                param = new ReportParameter[33];

            param[0] = new ReportParameter("CName", sCustName);
            param[1] = new ReportParameter("CAddress", sCustAddress);
            param[2] = new ReportParameter("CPinCode", sCPinCode);
            param[3] = new ReportParameter("CContactNo", sContactNo);
            param[4] = new ReportParameter("CPanNo", "");
            param[5] = new ReportParameter("InvoiceNo", sInvoiceNo);
            param[6] = new ReportParameter("InvDate", sInvDt);

            param[7] = new ReportParameter("StoreName", sStoreName);
            param[8] = new ReportParameter("StoreAddress", sStoreAddress);
            param[9] = new ReportParameter("StorePhone", sStorePhNo);

            if (!string.IsNullOrEmpty(sAmount))
                param[10] = new ReportParameter("Amtinwds", sAmount);
            else
                param[10] = new ReportParameter("Amtinwds", "zero");

            param[11] = new ReportParameter("CCode", sCustId);
            param[12] = new ReportParameter("ReceiptNo", sReceiptNo);
            param[13] = new ReportParameter("Title", sRTitle);
            param[14] = new ReportParameter("InvoiceFooter", sInvoiceFooter);
            param[15] = new ReportParameter("InvoiceTime", sTime); 
            param[16] = new ReportParameter("CompName", sCompanyName); 
            param[17] = new ReportParameter("CIN", sCINNo); 
            param[18] = new ReportParameter("DuplicateCopy", ""); 
            param[19] = new ReportParameter("cs", sCurrencySymbol); 
            param[20] = new ReportParameter("Cashier", sOperatorId);
            param[21] = new ReportParameter("FooterComments", "");
            param[22] = new ReportParameter("CustEmail", sCustEmail);
            param[23] = new ReportParameter("StoreEmail", sStoreEmail);
            param[24] = new ReportParameter("StoreFax", sStoreFax);
            param[25] = new ReportParameter("TRN", sTRN);
            param[26] = new ReportParameter("totTaxableamt", Convert.ToString(dExcludTaxAmt.ToString("#,0.00;(#,0.00)")));
            param[27] = new ReportParameter("totTaxAmt", Convert.ToString(dTaxAmt.ToString("#,0.00;(#,0.00)")));
            param[28] = new ReportParameter("CopyName", "");
            param[29] = new ReportParameter("ArStoreName", sArStoreName);
            param[30] = new ReportParameter("ArStoreAddress", sArStoreAddress);
            param[31] = new ReportParameter("TransRemarks", sRemarks);
            

            if (!string.IsNullOrEmpty(sRefRceiptNo))
                param[32] = new ReportParameter("RefReceiptNo", sRefRceiptNo);

            this.reportViewer1.LocalReport.SetParameters(param);
            this.reportViewer1.RefreshReport();
        }

        private void GetTender(ref DataSet dsSTender)
        {
            //string sqlsubDtl = " SELECT DISTINCT N.NAME, M.TENDERTYPE,M.CARDORACCOUNT," +
            //                    " CAST(ISNULL(M.AMOUNTCUR,0) AS DECIMAL(28,2)) AS AMOUNT" +
            //                    " FROM RETAILTRANSACTIONPAYMENTTRANS M" +
            //                    " LEFT JOIN RETAILSTORETENDERTYPETABLE N ON M.TENDERTYPE = N.TENDERTYPEID" +
            //                    " WHERE M.TRANSACTIONID = @TRANSACTIONID" +
            //                    " ORDER BY M.TENDERTYPE ";

            string sStoreNo = ApplicationSettings.Terminal.StoreId;

            string sqlsubDtl = " DECLARE @CHANNEL bigint  SELECT @CHANNEL = ISNULL(RECID,0) FROM RETAILSTORETABLE WHERE STORENUMBER = '" + sStoreNo + "'" +
                                "  SELECT N.NAME + ' (' + M.CURRENCY + ')' as NAME, M.TENDERTYPE,CAST(ISNULL(ABS(M.AMOUNTCUR),0) AS DECIMAL(28,2)) AS AMOUNT," +
                                " (ISNULL(CARDORACCOUNT,'') + ISNULL(CREDITVOUCHERID,'') + ISNULL(stuff(ISNULL(GIFTCARDID,''),1,LEN(ISNULL(GIFTCARDID,''))-4,REPLICATE('x', LEN(ISNULL(GIFTCARDID,''))-4)),'')) AS CARDORACCOUNT" +
                                " ,CONVERT(VARCHAR,M.TRANSDATE,106) AS TRANSDATE" +
                                " FROM RETAILTRANSACTIONPAYMENTTRANS M" +
                                " LEFT JOIN RETAILSTORETENDERTYPETABLE N ON M.TENDERTYPE = N.TENDERTYPEID" +
                                " WHERE M.TRANSACTIONID = @TRANSACTIONID AND N.CHANNEL = @CHANNEL AND M.TERMINAL = '" + sTerminal + "' AND M.TRANSACTIONSTATUS = 0" +
                                " ORDER BY M.TENDERTYPE ";

            SqlCommand command = new SqlCommand(sqlsubDtl, connection);
            command.Parameters.Clear();
            command.Parameters.Add("@TRANSACTIONID", SqlDbType.NVarChar).Value = sReceiptNo;

            SqlDataAdapter daSTotal = new SqlDataAdapter(command);

            daSTotal.Fill(dsSTender, "Tender");
            if (dsSTender.Tables[0].Rows.Count > 0)
            {
                sReceiptDate = Convert.ToString(dsSTender.Tables[0].Rows[0]["TRANSDATE"]);
               
            }
        }

        private void GetTaxInfo(ref DataSet dsSTaxInfo)
        {
            string sqlTaxInfo = " DECLARE @TINCST AS NVARCHAR(20)  DECLARE @TINVAT AS NVARCHAR(20)  SELECT @TINCST = ISNULL(A.REGISTRATIONNUMBER,'') + ISNULL(A.TTYPE,'')" +
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

            SqlCommand command = new SqlCommand(sqlTaxInfo, connection);
            command.Parameters.Clear();
            command.Parameters.Add("@INVENTLOCATIONID", SqlDbType.NVarChar).Value = sInventLocationId;

            SqlDataAdapter daTax = new SqlDataAdapter(command);

            daTax.Fill(dsSTaxInfo, "TaxInfo");
        }

        //RETAILTRANSACTIONTABLE SET Remarks
        //WHERE TRANSACTIONID=
        //AND TERMINAL = '" + 

        private string getTransactionRemarks(string sTransId,string sTerminal) 
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            string commandText = string.Empty;

            commandText = "select isnull(Remarks,'') from RETAILTRANSACTIONTABLE " +
                        " WHERE TRANSACTIONID='" + sTransId  + "'" +
                        " AND TERMINAL =  '" + sTerminal + "'";

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

        private void GetBookedSku(ref DataSet dsBookedSku)
        {
            string sqlBookedSku = " DECLARE @ORDERNUM VARCHAR(20)" +
                                 " SELECT @ORDERNUM = ORDERNUM from [RETAILDEPOSITTABLE] WHERE TRANSACTIONID = @TRANSACTIONID AND TERMINALID = '" + sTerminal + "' " +
                //" select SKUNUMBER from retailcustomerdepositskudetails WHERE ORDERNUMBER = @ORDERNUM and ORDERNUMBER!=''";
                                  " select SKUNUMBER from retailcustomerdepositskudetails WHERE TRANSID = @TRANSACTIONID AND TERMINALID = '" + sTerminal + "' "; // changes on req by S.Shrma on 23/02/2015

            SqlCommand command = new SqlCommand(sqlBookedSku, connection);
            command.Parameters.Clear();
            command.Parameters.Add("@TRANSACTIONID", SqlDbType.NVarChar).Value = sReceiptNo;

            SqlDataAdapter daBookedSku = new SqlDataAdapter(command);

            daBookedSku.Fill(dsBookedSku, "BookedSkuInfo");

            if (dsBookedSku.Tables[0] != null && dsBookedSku.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow drNew in dsBookedSku.Tables[0].Rows)
                {
                    if (string.IsNullOrEmpty(sBookedSkuList))
                        sBookedSkuList = "Against booking of " + Convert.ToString(drNew["SKUNUMBER"]);
                    else
                        sBookedSkuList = sBookedSkuList + ", " + Convert.ToString(drNew["SKUNUMBER"]);
                }
            }

        }

        private void getFixedMetalRate()
        {
            SqlConnection conn = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" declare @ORDERNUM VARCHAR(20) ");
            commandText.Append(" declare @IsFixedRate int ");
            commandText.Append(" DECLARE @RATE numeric(28, 3) ");
            commandText.Append(" SELECT @ORDERNUM = ORDERNUM from [RETAILDEPOSITTABLE] WHERE TRANSACTIONID =@TRANSACTIONID");
            commandText.Append(" select @IsFixedRate=IsFIXEDMETALRATE from CUSTORDER_HEADER WHERE ORDERNUM = @ORDERNUM and ORDERNUM!='' ");
            commandText.Append(" IF(@IsFixedRate=1)");
            commandText.Append(" BEGIN");
            commandText.Append(" select FIXEDMETALRATE from CUSTORDER_HEADER WHERE ORDERNUM = @ORDERNUM and ORDERNUM!=''");
            commandText.Append(" END");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.Parameters.Clear();
            command.Parameters.Add("@TRANSACTIONID", SqlDbType.NVarChar).Value = sReceiptNo;
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    sFixedRate = "Gold Rate Freezed";// at "+ Convert.ToString(reader.GetValue(0)) + "/gm"; blocked on 25/04/2014 req by S.Sharma
                }
            }

            if (conn.State == ConnectionState.Open)
                conn.Close();

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
                                " WHERE A.TRANSACTIONID = @TRANSACTIONID AND A.TERMINAL = '" + sTerminal + "' ORDER BY A.PARENTLINENUM,A.INFOCODEID";

            SqlCommand command = new SqlCommand(sqlsubDtl, connection);
            command.Parameters.Clear();
            command.Parameters.Add("@TRANSACTIONID", SqlDbType.NVarChar).Value = sReceiptNo;
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
                            //sPayInfo = sPayInfo + "; Amount : " + Convert.ToString(dsTemp.Tables[0].Rows[i - 2]["AMOUNT"]); //commented on 21/04/2014 Req by S.Sarma
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
                    //sPayInfo = sPayInfo + "; Amount : " + Convert.ToString(dsTemp.Tables[0].Rows[dsTemp.Tables[0].Rows.Count - 1]["AMOUNT"]);//commented on 21/04/2014 Req by S.Sarma
                    dr = dt.NewRow();
                    dr["PAYINFO"] = sPayInfo;
                    dt.Rows.Add(dr);
                    dt.AcceptChanges();
                    sPayInfo = string.Empty;
                }
            }
            dsSPaymentInfo.Tables.Add(dt);
        }

        private void GetStoreInfo(ref string sStorePh, ref string sInvFooter, ref string sCINNo)
        {
            string sql = " SELECT ISNULL(STORECONTACT,'-') AS STORECONTACT, ISNULL(INVOICEFOOTERNOTE,'') AS INVOICEFOOTERNOTE," +
                " ISNULL(CINNO,'') as CINNO,ISNULL(INVOICEFOOTERNOTEARABIC,'') AS INVOICEFOOTERNOTEARABIC,RETAILSTORENAME_ARABIC,RETAILSTOREADD_ARABIC" +
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

        private string GetRefReciptId(string sTransId)
        {
            string sRecNo = string.Empty;


            string sql = " select RECEIPTID from RETAILTRANSACTIONPAYMENTTRANS " +
                 " where TRANSACTIONID in (select ADVANCEADJUSTMENTID from RETAIL_CUSTOMCALCULATIONS_TABLE where transactionid ='" + sTransId + "')";

            DataTable dtStoreInfo = new DataTable();
            SqlCommand command = new SqlCommand(sql, connection);
            command.CommandTimeout = 0;

            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dtStoreInfo);

            if (dtStoreInfo != null && dtStoreInfo.Rows.Count > 0)
            {
                for (int i = 1; i <= dtStoreInfo.Rows.Count; i++)
                {
                    if (i > 1)
                        sRecNo = sRecNo + ",";
                    if (Convert.ToString(dtStoreInfo.Rows[0]["RECEIPTID"]) == string.Empty)
                        sRecNo = "...";
                    else
                        sRecNo += Convert.ToString(dtStoreInfo.Rows[0]["RECEIPTID"]);
                }
            }

            return "Ref Receipt No: " + "" + sRecNo;
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

        private void GetTaxAdvanceData(ref DataTable dtCol)
        {
            DataTable tblTaxInfo = new DataTable("GSTTAX");

            string sAdJustItem = AdjustmentItemID();
            string sItemName = AdjustmentItemName(sAdJustItem);

            tblTaxInfo.Columns.Add("EXCLVATAMT", typeof(decimal));
            tblTaxInfo.Columns.Add("TAXPCT", typeof(decimal));
            tblTaxInfo.Columns.Add("TAXAMOUNT", typeof(decimal));
            tblTaxInfo.Columns.Add("TOTAMOUNT", typeof(decimal));
            tblTaxInfo.Columns.Add("BOOKEDQTY", typeof(decimal));
            tblTaxInfo.Columns.Add("NATUREOFADV", typeof(string));

            tblTaxInfo.Rows.Add(dExcludTaxAmt, dTaxPct, dTaxAmt, dExcludTaxAmt + dTaxAmt, dBookedQty, "");

            dtCol = tblTaxInfo;

            if (dtCol != null && dtCol.Rows.Count < 6 && dtCol.Rows.Count > 0)
            {
                for (int j = dtCol.Rows.Count; j < 6; j++)
                    dtCol.Rows.Add();
            }
        }
        #region Adjustment Item Name
        private string AdjustmentItemID()
        {
            SqlConnection connection = new SqlConnection();
            connection = ApplicationSettings.Database.LocalConnection;

            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT TOP(1) ADJUSTMENTITEMID FROM [RETAILPARAMETERS]");
            sbQuery.Append(" where DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToString(cmd.ExecuteScalar());
        }

        private string AdjustmentItemName(string sItemId)
        {
            SqlConnection connection = new SqlConnection();
            connection = ApplicationSettings.Database.LocalConnection;

            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("select F.Name from INVENTTABLE D ");
            sbQuery.Append(" LEFT OUTER JOIN ECORESPRODUCT E ON D.PRODUCT = E.RECID ");
            sbQuery.Append(" LEFT OUTER JOIN ECORESPRODUCTTRANSLATION F");
            sbQuery.Append(" ON E.RECID = F.PRODUCT and F.LANGUAGEID='en-us' where d.ItemId='" + sItemId + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToString(cmd.ExecuteScalar());
        }

        private string getTaxGropCode(string sItemId) 
        {
            SqlConnection connection = new SqlConnection();
            connection = ApplicationSettings.Database.LocalConnection;

            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("select TAXITEMGROUPID from INVENTTABLEMODULE where itemid='" + sItemId + "' and MODULETYPE=2 ");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToString(cmd.ExecuteScalar());
        }

        private decimal getTaxPctValue(string sTaxCodeGrp)
        {
            SqlConnection connection = new SqlConnection();
            connection = ApplicationSettings.Database.LocalConnection;

            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("select CAST(ISNULL(TAXVALUE,0)AS DECIMAL(28,2)) TAXVALUE from TAXDATA");
            sbQuery.Append(" where  TAXCODE='" + sTaxCodeGrp + "' and DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToDecimal(cmd.ExecuteScalar());
        }

        private decimal getItemTaxPercentage()
        {
            SqlConnection connection = new SqlConnection();
            connection = ApplicationSettings.Database.LocalConnection;
            decimal sResult = 0m;

            string commandText = string.Empty;
            commandText = "select top 1 isnull(ADVANCETAXPERCENTAGE,0) from RETAILPARAMETERS where DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'";
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            sResult = Convert.ToDecimal(command.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();
            return sResult;

        }
        #endregion
    }
}
