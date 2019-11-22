using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class frmEstimationReport : Form
    {
        SqlConnection connection;
        DataTable dtEstDetail;
        DataTable dtEstIngrd;
      
        string sInvoiceNo = string.Empty;
        string sEstDate = string.Empty;
        string sProdDesc = "-";
        string sSKUNo = "-";
        string sDesignNo = "-";
        string sComplexity = "-";
        string sGrossWt = "0";
        string sStoreCode = string.Empty;
        string sStoreName = string.Empty;
        string sGoldRate = string.Empty;
        string sSilverRate = string.Empty;
        string sTerminalCode = string.Empty;


        public frmEstimationReport()
        {
            InitializeComponent();
        }

        public frmEstimationReport(IPosTransaction posTransaction, DataTable dtDetail, DataTable dtIngredient, SqlConnection conn = null)
        {
            InitializeComponent();

            RetailTransaction retailTrans = posTransaction as RetailTransaction;
            if (retailTrans != null)
            {
                if (Convert.ToString(retailTrans.TransactionId) != string.Empty)
                    sInvoiceNo = Convert.ToString(retailTrans.TransactionId);
                sEstDate = DateTime.Now.ToShortDateString();
                dtEstDetail = dtDetail;
                dtEstIngrd = dtIngredient;

                //Start: added on 24-03-2014 R.Hossain
                if (Convert.ToString(ApplicationSettings.Terminal.StoreId) != string.Empty)
                    sStoreCode = Convert.ToString(ApplicationSettings.Terminal.StoreId);
                if (Convert.ToString(ApplicationSettings.Terminal.StoreName) != string.Empty)
                    sStoreName = Convert.ToString(ApplicationSettings.Terminal.StoreName);
                if (Convert.ToString(ApplicationSettings.Terminal.TerminalId) != string.Empty)
                    sTerminalCode = Convert.ToString(ApplicationSettings.Terminal.TerminalId);

                sGoldRate = getConfigId(1) + " ; " + getMetalRate(1);
                sSilverRate = getConfigId(2) + " ; " + getMetalRate(2);
                //sPltRate = getConfigId(3) + " ; " + getMetalRate(3);

                if (dtEstDetail != null && dtEstDetail.Rows.Count > 0)
                {
                  sSKUNo = Convert.ToString(dtEstDetail.Rows[0]["ITEMID"]); ;
                }

                sGrossWt = getValue("SELECT CAST(QTY AS decimal (6,3)) AS QTY  FROM SKUTABLE_POSTED WHERE SKUNumber ='" + sSKUNo + "'");
                sProdDesc = getValue("SELECT R.NAME FROM ECORESPRODUCTTRANSLATION AS R, INVENTTABLE AS E WHERE E.PRODUCT  = R.PRODUCT and E.ITEMID='" + sSKUNo + "'");
                sDesignNo = getValue("SELECT ITEMIDPARENT FROM INVENTTABLE WHERE ITEMID='" + sSKUNo + "'");
                sComplexity = getValue("select COMPLEXITY_CODE from INVENTTABLE  WHERE ITEMID='" + sSKUNo + "'");
                //End: added on 24-03-2014 R.Hossain

            }
        }


        private void frmEstimationReport_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'dsEstimation.Detail' table. You can move, or remove it, as needed.
            //  this.DetailTableAdapter.Fill(this.dsEstimation.Detail);
            // TODO: This line of code loads data into the 'dsEstimation.Ingredient' table. You can move, or remove it, as needed.
            //  this.IngredientTableAdapter.Fill(this.dsEstimation.Ingredient);
            //// TODO: This line of code loads data into the 'dsEstimation.Detail' table. You can move, or remove it, as needed.
            //this.DetailTableAdapter.Fill(this.dsEstimation.Detail);
            //// TODO: This line of code loads data into the 'dsEstimation.Ingredient' table. You can move, or remove it, as needed.
            //this.IngredientTableAdapter.Fill(this.dsEstimation.Ingredient);

            reportViewer1.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = reportViewer1.LocalReport;
            localReport.ReportPath = "rptEstimation.rdlc";

            DataSet dsDetail = new DataSet();
            dsDetail.Tables.Add(dtEstDetail);

            DataSet dsIngredient = new DataSet();
            dsIngredient.Tables.Add(dtEstIngrd);

            this.reportViewer1.LocalReport.DataSources.Clear();
            ReportDataSource rdsDetail = new ReportDataSource();
            rdsDetail.Name = "Detail";
            //  rdsDetail.Value = dtEstDetail;
            rdsDetail.Value = dsDetail.Tables[0];
            // this.reportViewer1.LocalReport.DataSources.Clear();

            //this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Detail", dtEstDetail));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Detail", dsDetail.Tables[0]));

            // this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Detail", rdsDetail.Value));

            ReportDataSource rdsIngredient = new ReportDataSource();
            rdsIngredient.Name = "Ingredient";
            //rdsIngredient.Value = dtEstIngrd;
            rdsIngredient.Value = dsIngredient.Tables[0];

            // this.reportViewer1.LocalReport.DataSources.Clear();
            //  this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Ingredient", dtEstIngrd));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Ingredient", dsIngredient.Tables[0]));

            //  this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Ingredient", rdsIngredient.Value));

            ReportParameter[] param = new ReportParameter[12];
            param[0] = new ReportParameter("InvoiceNo", sInvoiceNo);
            param[1] = new ReportParameter("EstDate", sEstDate);

            //Start: added on 24-03-2014 R.Hossain
            param[2] = new ReportParameter("ProdDesc", sProdDesc);
            param[3] = new ReportParameter("SKUNo", sSKUNo);
            param[4] = new ReportParameter("DesignNo", sDesignNo);
            param[5] = new ReportParameter("Complexity", sComplexity);
            param[6] = new ReportParameter("GrossWt", sGrossWt);
            param[7] = new ReportParameter("StoreCode", sStoreCode);
            param[8] = new ReportParameter("StoreName", sStoreName);
            param[9] = new ReportParameter("GoldRate", sGoldRate);
            param[10] = new ReportParameter("SilverRate", sSilverRate);
            param[11] = new ReportParameter("TerminalCode", sTerminalCode);
            //End  :  added on 24-03-2014 R.Hossain

            this.reportViewer1.LocalReport.SetParameters(param);

            this.reportViewer1.RefreshReport();
        }

        private decimal getMetalRate(int iMetalType)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) ");
            commandText.Append(" DECLARE @CONFIGIDSTANDARD VARCHAR(20) ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM         RETAILCHANNELTABLE INNER JOIN ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + sStoreCode.Trim() + "'  ");
            if (iMetalType == 1)
                commandText.Append(" SELECT @CONFIGIDSTANDARD = DEFAULTCONFIGIDGOLD FROM INVENTPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");
            else if (iMetalType == 2)
                commandText.Append(" SELECT @CONFIGIDSTANDARD = DEFAULTCONFIGIDSILVER FROM INVENTPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");
            else
                commandText.Append(" SELECT @CONFIGIDSTANDARD = DEFAULTCONFIGIDPLATINUM FROM INVENTPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

            commandText.Append(" BEGIN ");
            commandText.Append(" SELECT TOP 1 CAST(ISNULL(RATES,0) AS decimal (6,2)) FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION ");
            commandText.Append(" AND METALTYPE=" + iMetalType + " ");
            commandText.Append(" AND RETAIL=1 AND RATETYPE=3"); // SAle            
            commandText.Append(" AND ACTIVE=1 AND ");
            commandText.Append(" CONFIGIDSTANDARD =@CONFIGIDSTANDARD");
            commandText.Append(" ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC ");
            commandText.Append(" END ");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
            {
                return Convert.ToDecimal(sResult.Trim());
            }
            else
            {
                return 0;
            }
        }

        private string getConfigId(int iMetalType) // from inventparameter
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            if (iMetalType == 1)
                commandText.Append(" SELECT DEFAULTCONFIGIDGOLD FROM INVENTPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");
            else if (iMetalType == 2)
                commandText.Append(" SELECT DEFAULTCONFIGIDSILVER FROM INVENTPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");
            else
                commandText.Append(" SELECT DEFAULTCONFIGIDPLATINUM FROM INVENTPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

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

        private string getValue(string sSqlString) // passing sql query string return  one string value
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(sSqlString);

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
    }
}
