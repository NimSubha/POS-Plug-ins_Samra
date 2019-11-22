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
using LSRetailPosis.Transaction;
using LSRetailPosis.Settings;
using Microsoft.Reporting.WinForms;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    public partial class frmR_GSSInstalmentReceipt:Form
    {
        SqlConnection connection;
        BlankOperations oBlank = new BlankOperations();
        // string sTransactionId = "";
        string sCustName = "-";
        string sCustId = "-";
        string sCustAddress = "-";
        string sCPinCode = "-";
        string sContactNo = "-";      

        string sReceiptNo = "";
        string sReceiptVoucherNo = "";
        string sAmount = "";
        string sReceiptDate = "-";

        string sStoreName = "-";
        string sStoreAddress = "-";
        string sStorePhNo = "...";

        string sDataAreaId = "";
        string sInventLocationId = "";
        string sDetailsLine = "";
        string sSchemeCode = "";
        string sInvoiceFooter = "-";
        string sTerminal = string.Empty;
        string sMaturityDate = string.Empty;
        string sCompanyName = string.Empty; //aded on 14/04/2014 R.Hossain
        string sCINNo = string.Empty;//aded on 14/04/2014 R.Hossain
        string sGSSAccNumber = "";
        string sCurrencySymbol = "";

        public frmR_GSSInstalmentReceipt(SqlConnection conn)
        {
            InitializeComponent();
            connection = conn;
            if(connection.State == ConnectionState.Closed)
                connection.Open();
        }
        public frmR_GSSInstalmentReceipt(IPosTransaction posTransaction, SqlConnection conn, string _sTransId, string _sAmt, string sGSSAccNo, string sTerminalId)
        {
            InitializeComponent();
            sTerminal = sTerminalId;

            #region[Param Info]
            RetailTransaction retailTrans = posTransaction as RetailTransaction;
            if(retailTrans != null)
            {
                if(Convert.ToString(retailTrans.Customer.Name) != string.Empty)
                    sCustName = Convert.ToString(retailTrans.Customer.Name);
                if (!string.IsNullOrEmpty(Convert.ToString(retailTrans.Customer.CustomerId)))
                    sCustId = Convert.ToString(retailTrans.Customer.CustomerId);
                if(Convert.ToString(retailTrans.Customer.Address) != string.Empty)  //PrimaryAddress
                    sCustAddress = Convert.ToString(retailTrans.Customer.Address);
                if(Convert.ToString(retailTrans.Customer.PostalCode) != string.Empty)
                    sCPinCode = Convert.ToString(retailTrans.Customer.PostalCode);
                //if(Convert.ToString(retailTrans.Customer.MobilePhone) != string.Empty)
                //    sContactNo = Convert.ToString(retailTrans.Customer.MobilePhone);

                if (!string.IsNullOrEmpty(retailTrans.Customer.Telephone))
                    sContactNo = Convert.ToString(retailTrans.Customer.Telephone);
                //sCPanNo
                sCurrencySymbol = oBlank.GetCurrencySymbol();
                //-------
                if (Convert.ToString(retailTrans.TransactionId) != string.Empty)
                {
                    sReceiptNo = _sTransId;// Convert.ToString(retailTrans.TransactionId);
                    sReceiptVoucherNo = retailTrans.ReceiptId;
                }

                if (Convert.ToString(sReceiptVoucherNo) == string.Empty)
                    sReceiptVoucherNo = GetReciptVouNo(_sTransId, conn);

                //if(retailTrans.BeginDateTime != null)
                //    sReceiptDate = retailTrans.BeginDateTime.ToShortDateString();

                sAmount =  oBlank.Amtinwds(Convert.ToDouble(_sAmt));
                sDetailsLine = "Received with thanks from " + "" + sCustName + "                                                                          " + sCurrencySymbol + " " + _sAmt;
                //----------store Info

                GetGSSMaturityDate(sGSSAccNo, conn); // added on 29/03/2014 req by Sailendra da. dev by R.Hossain
                //if(Convert.ToString(retailTrans.StoreName) != string.Empty)
                //    sStoreName = Convert.ToString(retailTrans.StoreName);
                //if(Convert.ToString(retailTrans.StoreAddress) != string.Empty)
                //    sStoreAddress = Convert.ToString(retailTrans.StoreAddress);
                ////if(Convert.ToString(retailTrans.StorePhone) != string.Empty)
                //if (!string.IsNullOrEmpty(Convert.ToString(retailTrans.StorePhone)))
                //    sStorePhNo = Convert.ToString(retailTrans.StorePhone);


                if (Convert.ToString(ApplicationSettings.Terminal.StoreName) != string.Empty)
                    sStoreName = Convert.ToString(ApplicationSettings.Terminal.StoreName);
                if (Convert.ToString(ApplicationSettings.Terminal.StoreAddress) != string.Empty)
                    sStoreAddress = Convert.ToString(ApplicationSettings.Terminal.StoreAddress);
                if (!string.IsNullOrEmpty(Convert.ToString(ApplicationSettings.Terminal.StorePhone)))
                    sStorePhNo = Convert.ToString(ApplicationSettings.Terminal.StorePhone);

                sDataAreaId = Convert.ToString(ApplicationSettings.Database.DATAAREAID);

                //if(Convert.ToString(retailTrans.InventLocationId) != string.Empty)
                //    sInventLocationId = Convert.ToString(retailTrans.InventLocationId);
                sInventLocationId = ApplicationSettings.Terminal.InventLocationId; 


            }
            connection = conn;
            if(connection.State == ConnectionState.Closed)
                connection.Open();
            GetStoreInfo(ref sStorePhNo, ref sInvoiceFooter, ref sCINNo);
            sCompanyName = oBlank.GetCompanyName(conn);//aded on 14/04/2014 R.Hossain
            sGSSAccNumber = sGSSAccNo; // added on 18/04/2014
            #endregion
        }
            
        private void frmR_GSSInstalmentReceipt_Load(object sender, EventArgs e)
        {
            rptGSSViewer.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = rptGSSViewer.LocalReport;
            localReport.ReportPath = "rptGSSInstalmentReceipt.rdlc";
                       
            DataSet dsTender = new DataSet();
            DataSet dsTaxInfo = new DataSet();
            DataSet dsPaymentInfo = new DataSet();

            GetTender(ref dsTender);
            GetTaxInfo(ref dsTaxInfo);
            GetPayInfo(ref dsPaymentInfo);
            
            ReportDataSource dsGSSInstalmentReceipt = new ReportDataSource();

            dsGSSInstalmentReceipt.Name = "DataSet1";
            dsGSSInstalmentReceipt.Value = dsTender.Tables[0];
          
            this.rptGSSViewer.LocalReport.DataSources.Clear();
            this.rptGSSViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dsTender.Tables[0]));

            ReportDataSource RDTaxInfo = new ReportDataSource();
            RDTaxInfo.Name = "DataSet2";
            RDTaxInfo.Value = dsTaxInfo.Tables[0];
            this.rptGSSViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet2", dsTaxInfo.Tables[0]));

            ReportDataSource RDPaymentInfo = new ReportDataSource();
            RDPaymentInfo.Name = "PaymentInfo";
            RDPaymentInfo.Value = dsPaymentInfo.Tables[0];
            this.rptGSSViewer.LocalReport.DataSources.Add(new ReportDataSource("PaymentInfo", dsPaymentInfo.Tables[0]));

            ReportParameter[] param = new ReportParameter[18];

            param[0] = new ReportParameter("CName", sCustName);
            param[1] = new ReportParameter("CAddress", sCustAddress);
            param[2] = new ReportParameter("CPinCode", sCPinCode);
            param[3] = new ReportParameter("CContactNo", sContactNo);
         //   param[4] = new ReportParameter("ReceiptNo", sReceiptVoucherNo);
            param[4] = new ReportParameter("ReceiptNo", sReceiptNo);
            param[5] = new ReportParameter("RecDate", sReceiptDate);
            if(!string.IsNullOrEmpty(sSchemeCode))
                param[6] = new ReportParameter("SchemeCode", sSchemeCode);
            else
                param[6] = new ReportParameter("SchemeCode", "-");

            param[7] = new ReportParameter("InstallNo", "-");
                      
            param[8] = new ReportParameter("StoreAddress", sStoreAddress);
            param[9] = new ReportParameter("StorePhone", sStorePhNo);

            if(!string.IsNullOrEmpty(sAmount))
                param[10] = new ReportParameter("Amtinwds", sAmount);
            else
                param[10] = new ReportParameter("Amtinwds", "zero");

            param[11] = new ReportParameter("DetailsLine", sDetailsLine);
            param[12] = new ReportParameter("CId", sCustId);
            param[13] = new ReportParameter("MaturityDate", sMaturityDate);
            param[14] = new ReportParameter("VoucherNo", sReceiptVoucherNo);
            param[15] = new ReportParameter("CompName", sCompanyName); // added on 14/04/214 req from Sailendra Da
            param[16] = new ReportParameter("CIN", sCINNo); // added on 14/04/214 req from Sailendra Da
            param[17] = new ReportParameter("GSSAccNumber", sGSSAccNumber); // added on 14/04/214 req from Sailendra Da

            this.rptGSSViewer.LocalReport.SetParameters(param);

            this.rptGSSViewer.RefreshReport();
        }

               
        private void GetTender(ref DataSet dsSTender)
        {
            //string sqlsubDtl = " SELECT DISTINCT N.NAME, M.TENDERTYPE,M.CARDORACCOUNT,"+
            //                    " CAST(ISNULL(M.AMOUNTCUR,0) AS DECIMAL(28,2)) AS AMOUNT" +
            //                    " FROM RETAILTRANSACTIONPAYMENTTRANS M" +
            //                    " LEFT JOIN RETAILSTORETENDERTYPETABLE N ON M.TENDERTYPE = N.TENDERTYPEID" +
            //                    " WHERE M.TRANSACTIONID = @TRANSACTIONID" +
            //                    " ORDER BY M.TENDERTYPE ";

            string sStoreNo = ApplicationSettings.Terminal.StoreId;

            string sqlsubDtl = " DECLARE @CHANNEL bigint  SELECT @CHANNEL = ISNULL(RECID,0) FROM RETAILSTORETABLE WHERE STORENUMBER = '" + sStoreNo + "'" +
                                "  SELECT N.NAME, M.TENDERTYPE,CAST(ISNULL(M.AMOUNTCUR,0) AS DECIMAL(28,2)) AS AMOUNT,"+
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
                sReceiptDate = Convert.ToString(dsSTender.Tables[0].Rows[0]["TRANSDATE"]);
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

        private void GetPayInfo(ref DataSet dsSPaymentInfo)
        {
            DataSet dsTemp = new DataSet();

            string sqlsubDtl = " SELECT (ISNULL(B.DESCRIPTION,'') + ' : '+ ISNULL(A.INFORMATION,'')) AS DTLPAYINFO, A.TYPE,A.INFOCODEID,A.PARENTLINENUM,A.TRANSACTIONID" +
                                " ,CAST(ISNULL(A.AMOUNT,0) AS DECIMAL(28,2)) AS AMOUNT" + //R.AMOUNTCUR ->  A.AMOUNT ADDED RHossain on 21/04/2014
                                " FROM [dbo].[RETAILTRANSACTIONINFOCODETRANS] A" +
                                " INNER JOIN	 RETAILINFOCODETABLE B ON A.INFOCODEID = B.INFOCODEID" +
                                " INNER JOIN RETAILTRANSACTIONPAYMENTTRANS R ON A.TRANSACTIONID = R.TRANSACTIONID"+
                                " AND A.TERMINAL=R.TERMINAL AND A.PARENTLINENUM = R.LINENUM" + //AND A.TERMINAL=R.TERMINAL -->ADDED RHossain on 21/04/2014
                                " WHERE A.TRANSACTIONID = @TRANSACTIONID  AND R.TERMINAL = '" + sTerminal + "' ORDER BY A.PARENTLINENUM,A.INFOCODEID"; 

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
                            //sPayInfo = sPayInfo + "; Amount : " + Convert.ToString(dsTemp.Tables[0].Rows[i - 2]["AMOUNT"]);//commented on 21/04/2014 Req by S.Sarma
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
            string sql = " SELECT ISNULL(STORECONTACT,'-') AS STORECONTACT, ISNULL(INVOICEFOOTERNOTE,'') AS INVOICEFOOTERNOTE,"+
                " ISNULL(CINNO,'') as CINNO FROM RETAILSTORETABLE WHERE STORENUMBER='" + ApplicationSettings.Terminal.StoreId + "'";

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
            }
        }

        private void GetGSSMaturityDate(string sGSSNo, SqlConnection conn)
        {
            string sql = "select CONVERT(VARCHAR(11),CLOSUREDATE,106) as CLOSUREDATE,SCHEMECODE  from GSSACCOUNTOPENINGPOSTED"+
                        " WHERE GSSACCOUNTNO ='" + sGSSNo + "'";

            DataTable dtGSSAccInfo = new DataTable();
            SqlCommand command = new SqlCommand(sql, conn);
            command.CommandTimeout = 0;

            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dtGSSAccInfo);

            if (dtGSSAccInfo != null && dtGSSAccInfo.Rows.Count > 0)
            {
                if (Convert.ToString(dtGSSAccInfo.Rows[0]["CLOSUREDATE"]) == string.Empty)
                    sMaturityDate = "-";
                else
                    sMaturityDate = Convert.ToString(dtGSSAccInfo.Rows[0]["CLOSUREDATE"]);
                if (Convert.ToString(dtGSSAccInfo.Rows[0]["SCHEMECODE"]) == string.Empty)
                    sSchemeCode = "-";
                else
                    sSchemeCode = Convert.ToString(dtGSSAccInfo.Rows[0]["SCHEMECODE"]);
            }
        }

        private string  GetReciptVouNo(string sTransId, SqlConnection conn)
        {
            string invNo = string.Empty;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "select RECEIPTID  from RETAILTRANSACTIONPAYMENTTRANS where TRANSACTIONID='" + sTransId  + "'";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;

            conn.Open();
            invNo = Convert.ToString(cmd.ExecuteScalar());
            conn.Close();

            return invNo;
        }
    }
}
