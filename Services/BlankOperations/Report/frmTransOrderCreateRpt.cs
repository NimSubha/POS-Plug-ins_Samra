using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.ComponentModel.Composition;
using System.Data.SqlClient;
using LSRetailPosis.Settings;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    public partial class frmTransOrderCreateRpt : Form
    {

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
            }
        }

        DataSet dsTOHdr, dsTODtl;
        string sAmtword = string.Empty;
        int iPrintOption = 0; // 0 -- summary and 1-- Detail
        string sCompanyName = string.Empty;

        public frmTransOrderCreateRpt()
        {
            InitializeComponent();
        }

        public frmTransOrderCreateRpt(DataSet dsHdr, DataSet dsDtl,int iOption)
        {
            InitializeComponent();
            if (dsHdr != null && dsDtl != null)
            {
                iPrintOption = iOption;
                dsTOHdr = new DataSet();
                dsTOHdr = dsHdr;
                dsTODtl = new DataSet();
                dsTODtl = dsDtl;
                sAmtword = Convert.ToString(dsTOHdr.Tables[0].Rows[0]["AMTINWORDS"]);
                sCompanyName = GetCompanyName();
            }
        }

        private void frmTransOrderCreateRpt_Load(object sender, EventArgs e)
        {
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = reportViewer1.LocalReport;
            if (iPrintOption == 0)
            {
                localReport.ReportPath = "rptTransOrderCreate.rdlc";
            }
            else
            {
                localReport.ReportPath = "rptTransOrderCreateDtl.rdlc";
            }

            ReportDataSource rdsHdr = new ReportDataSource("dsTransOrder",dsTOHdr.Tables[0]);
            this.reportViewer1.LocalReport.DataSources.Clear();
            this.reportViewer1.LocalReport.DataSources.Add(rdsHdr);
            ReportDataSource rdsDtl = new ReportDataSource("dsTransOrderDtl", dsTODtl.Tables[0]);
            this.reportViewer1.LocalReport.DataSources.Add(rdsDtl);
            ReportParameter[] param = new ReportParameter[2];
            param[0] = new ReportParameter("amtinwords", sAmtword);
            param[1] = new ReportParameter("CompanyName", sCompanyName);
            this.reportViewer1.LocalReport.SetParameters(param);
            this.reportViewer1.RefreshReport();
        }

        private string GetCompanyName()
        {
            string sCName = string.Empty;

            SqlConnection conn = new SqlConnection();

            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;
            
            string sQry = "SELECT ISNULL(A.NAME,'') FROM DIRPARTYTABLE A INNER JOIN COMPANYINFO B" +
                " ON A.RECID = B.RECID WHERE B.DATAAREA = '"+ApplicationSettings.Database.DATAAREAID+"'";

            //using (SqlCommand cmd = new SqlCommand(sQry, conn))
            //{
                SqlCommand cmd = new SqlCommand(sQry, conn);
                cmd.CommandTimeout = 0;
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                sCName = Convert.ToString(cmd.ExecuteScalar());

                if (conn.State == ConnectionState.Open)
                    conn.Close();
            //}

            return sCName;
        
        }

    }
}
