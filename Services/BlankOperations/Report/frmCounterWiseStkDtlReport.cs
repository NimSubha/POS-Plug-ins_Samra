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
    public partial class frmCounterWiseStkDtlReport : Form
    {
        SqlConnection connection;

        public frmCounterWiseStkDtlReport()
        {
            InitializeComponent();
        }

        public frmCounterWiseStkDtlReport(SqlConnection Conn)
        {
            InitializeComponent();
            connection = Conn;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
        }

        private void frmCounterWiseStkDtlReport_Load(object sender, EventArgs e)
        {
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = reportViewer1.LocalReport;
            localReport.ReportPath = "rptCounterWiseStockDtl.rdlc";
            DataSet dataset = new DataSet();
            GetStockData(ref dataset);
            ReportDataSource rdsStockCount = new ReportDataSource("dsCWStkDtl", dataset.Tables[0]);
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rdsStockCount);

            this.reportViewer1.RefreshReport();
        }

        private void GetStockData(ref DataSet dsStock)
        {
            string sQuery = " SELECT A.TOCOUNTER AS [Counter],A.SkuNumber," + 
                            " SKP.ECORESCONFIGURATIONNAME AS CONFIG," +
                            " SKP.INVENTSIZEID  AS SIZE,"+
                            //" CAST(ISNULL(A.QTY,0) AS NUMERIC (28,3)) AS Quantity,"+
                            " F.[DESCRIPTION] AS ITEMDESC,"+
                            "  SKP.VEND_ACCOUNT," +
                            " B.ITEMIDPARENT ,"+ 
                            " ISNULL(B.SETOF,0) AS SETOF," +
                            //" CAST(ISNULL(SKP.NETQTY,0) AS NUMERIC (28,3)) NETQTY,"+
                            " CAST(ISNULL(SKP.PDSCWQTY,0) AS NUMERIC (10)) CWQty, " +
                            " CAST(ISNULL(A.QTY,0) AS NUMERIC (28,3)) AS Quantity,"+
                            " CAST(ISNULL(SKP.GROSSWEIGHT,0) AS NUMERIC (28,3)) GrossWt, "+
                            " CAST(ISNULL(SKP.NETWEIGHT,0) AS NUMERIC (28,3)) NETQTY, "+
                            " ISNULL(SKP.TagCurrency,'') as TagCurrency,"+
                            " CAST(ISNULL(SKP.TagPrice,0) AS NUMERIC (28,2)) TagPrice,"+

                            " ISNULL(SKP.DMDCWQTY,0) DMDCWQTY,"+
                            " CAST(ISNULL(SKP.DMDQTY,0) AS NUMERIC (28,3)) DMDQTY, "+
                            " ISNULL(SKP.STNCWQTY,0) STNCWQTY,"+
                            " CAST(ISNULL(SKP.STNQTY,0) AS NUMERIC (28,3)) STNQTY "+
                            " FROM SKUTableTrans A INNER JOIN INVENTTABLE B ON A.SKUNUMBER = B.ITEMID " +
                            " LEFT OUTER JOIN ECORESPRODUCT E ON B.PRODUCT = E.RECID" +
                            " LEFT OUTER JOIN ECORESPRODUCTTRANSLATION F ON E.RECID = F.PRODUCT" +
                            " LEFT JOIN SKUTable_Posted SKP ON A.SkuNumber=SKP.SkuNumber "+
                            " WHERE ISNULL(A.TOCOUNTER,'') <> '' AND ISNULL(A.ISAVAILABLE,0) = 1" +
                            " ORDER BY A.TOCOUNTER, SkuNumber";

            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandTimeout = 0;
            SqlDataAdapter daStock = new SqlDataAdapter(command);

            daStock.Fill(dsStock, "Transaction");

        }
    }
}
