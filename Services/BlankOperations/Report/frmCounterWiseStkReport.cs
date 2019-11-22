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


namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    public partial class frmCounterWiseStkReport : Form
    {
        SqlConnection connection;

        public frmCounterWiseStkReport()
        {
            InitializeComponent();
        }

        public frmCounterWiseStkReport(SqlConnection Conn)
        {
            InitializeComponent();
            connection = Conn;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            
        }

        private void frmCounterWiseStkReport_Load(object sender, EventArgs e)
        {
            rptViewerStk.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = rptViewerStk.LocalReport;
            localReport.ReportPath = "rptCounterWiseStock.rdlc";
            DataSet dataset = new DataSet();
            GetStockData(ref dataset);
            ReportDataSource rdsStockCount = new ReportDataSource();
            rdsStockCount.Name = "dsCWStk";
            rdsStockCount.Value = dataset.Tables[0];
            rptViewerStk.LocalReport.DataSources.Clear();
            rptViewerStk.LocalReport.DataSources.Add(rdsStockCount);

            this.rptViewerStk.RefreshReport();
        }

        private void GetStockData(ref DataSet dsStock)
        {
            string sQuery = " SELECT  CAST(SUM(ISNULL(A.PDSCWQTY,0)) AS NUMERIC (28,0)) AS PCS," +
                            " CAST(SUM(ISNULL(A.QTY,0)) AS NUMERIC (28,3)) AS Quantity," +
                            " A.TOCOUNTER AS [Counter]" +
                            " ,B.ARTICLE_CODE" +
                            " ,ISNULL(C.[DESCRIPTION],'')AS [DESCRIPTION], SUM(ISNULL(B.SETOF,0)) AS SETOF" +
                            // " FROM SKUTable_Posted A INNER JOIN INVENTTABLE B ON A.SKUNUMBER = B.ITEMID" + 
                            " FROM SKUTableTrans A INNER JOIN INVENTTABLE B ON A.SKUNUMBER = B.ITEMID" + 
                            " LEFT OUTER JOIN Article_Master C ON B.ARTICLE_CODE = C.ARTICLE_CODE" +
                            " WHERE ISNULL(TOCOUNTER,'') <> ''" +
                            " AND ISNULL(ISAVAILABLE,0) = 1" +
                            " GROUP BY B.ARTICLE_CODE,C.[DESCRIPTION],A.TOCOUNTER" +
                            " ORDER BY  A.TOCOUNTER,B.ARTICLE_CODE";

            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandTimeout = 0;
            SqlDataAdapter daStock = new SqlDataAdapter(command);
            daStock.Fill(dsStock, "Transaction");
        }
    }
}
