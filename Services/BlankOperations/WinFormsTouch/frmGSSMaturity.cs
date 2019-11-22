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
using System.Collections.ObjectModel;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using System.IO;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.ComponentModel.Composition;
namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmGSSMaturity : frmTouchBase
    {
        DataRow drGSS = null;
        DataRow drGSSCust = null;
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

        public frmGSSMaturity()
        {
            InitializeComponent();
        }

        private void btnGSSSearch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCustAcc.Text.Trim()))
            {
                try
                {
                    SqlConnection connection = new SqlConnection();

                    if (application != null)
                        connection = application.Settings.Database.Connection;
                    else
                        connection = ApplicationSettings.Database.LocalConnection;

                    string sQry = "SELECT  GSSACCOUNTNO AS [GSSNumber]  FROM GSSACCOUNTOPENINGPOSTED" +
                                  " WHERE GSSMATURED = 0 and CustAccount='" + txtCustAcc.Text.Trim() + "'"; // Changes on 01/05/2014

                    SqlCommand cmd = new SqlCommand(sQry, connection);
                    cmd.CommandTimeout = 0;

                    DataTable dtGSSNo = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dtGSSNo);

                    if (dtGSSNo != null && dtGSSNo.Rows.Count > 0)
                    {
                        Dialog.WinFormsTouch.frmGenericSearch oSearch = new Dialog.WinFormsTouch.frmGenericSearch(dtGSSNo, drGSS, "GSS Number");
                        oSearch.ShowDialog();
                        drGSS = oSearch.SelectedDataRow;
                        if (drGSS != null)
                            txtGSSNumber.Text = Convert.ToString(drGSS[0]);
                    }
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                MessageBox.Show("Please select customer account.");
            }
                            
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtGSSNumber.Text.Trim()))
            {
                try
                {
                    if (drGSS != null)
                    {
                        string sGSSNo = Convert.ToString(drGSS[0]);
                        if (PosApplication.Instance.TransactionServices.CheckConnection())
                        {
                            ReadOnlyCollection<object> containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("GSSMaturiry", sGSSNo);
                            string sMsg = Convert.ToString(containerArray[2]);
                            MessageBox.Show(sMsg);
                            txtGSSNumber.Text = string.Empty;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to mature");
                }
            }
            else
            {
                MessageBox.Show("Please select GSS number.");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSearchCustomer_Click(object sender, EventArgs e) // added on 01/05/2014 on req of S.Sharma
        {
            try
            {
                SqlConnection connection = new SqlConnection();

                if (application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;

                string sQry = "SELECT distinct CUSTACCOUNT,CUSTNAME FROM GSSACCOUNTOPENINGPOSTED WHERE GSSMATURED = 0 and isnull(CUSTACCOUNT,'')<>''";

                SqlCommand cmd = new SqlCommand(sQry, connection);
                cmd.CommandTimeout = 0;

                DataTable dtGSSCust = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dtGSSCust);

                if (dtGSSCust != null && dtGSSCust.Rows.Count > 0)
                {
                    Dialog.WinFormsTouch.frmGenericSearch oSearch = new Dialog.WinFormsTouch.frmGenericSearch(dtGSSCust, drGSSCust, "Customer Search");
                    oSearch.ShowDialog();
                    drGSSCust = oSearch.SelectedDataRow;
                    if (drGSSCust != null)
                    {
                        txtCustAcc.Text = Convert.ToString(drGSSCust[0]);
                        txtCustName.Text = Convert.ToString(drGSSCust[1]);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        
    }
}
