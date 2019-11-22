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

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmTransferOrder:frmTouchBase
    {
        string sInventLocationId = string.Empty;
        DataTable dtSelectedSKU;
        DataSet dsSKU;

        SqlConnection connection;
        public frmTransferOrder(SqlConnection Conn)
        {
            InitializeComponent();

            connection = Conn;
            dtSelectedSKU = new DataTable("SKUINFO");

            dtSelectedSKU.Columns.Add("STOREID", typeof(string));
            dtSelectedSKU.Columns.Add("TWH", typeof(string));
            dtSelectedSKU.Columns.Add("WAYBILL", typeof(string));
            dtSelectedSKU.Columns.Add("AWBNUM", typeof(string));

            dtSelectedSKU.Columns.Add("SKUNumber", typeof(string));
            dtSelectedSKU.AcceptChanges();
            DataColumn[] columns = new DataColumn[1];
            columns[0] = dtSelectedSKU.Columns["SKUNumber"];
            dtSelectedSKU.PrimaryKey = columns;
        }

        private void btnWHSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if(PosApplication.Instance.TransactionServices.CheckConnection())
                {
                    ReadOnlyCollection<object> containerArray;
                    string sStoreId = ApplicationSettings.Terminal.StoreId;
                    containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("GetToWarehouse", sStoreId);

                    DataSet dsWH = new DataSet();
                    StringReader srTransDetail = new StringReader(Convert.ToString(containerArray[3]));

                    if(Convert.ToString(containerArray[3]).Trim().Length > 38)
                    {
                        dsWH.ReadXml(srTransDetail);
                    }
                    if(dsWH != null && dsWH.Tables[0].Rows.Count > 0)
                    {
                        Dialog.WinFormsTouch.frmGenericSearch Osearch = new Dialog.WinFormsTouch.frmGenericSearch(dsWH.Tables[0], null, "Warehouse");
                        Osearch.ShowDialog();

                        DataRow dr = Osearch.SelectedDataRow;

                        if(dr != null)
                        {
                            sInventLocationId = Convert.ToString(dr["InventLocationId"]);
                            txtWarehouse.Text = Convert.ToString(dr["Name"]);
                        }
                    }
                }
            }
            catch(Exception ex)
            {


            }
        }



        private void btnClearProduct_Click(object sender, EventArgs e)
        {
            txtSKUNo.Text = string.Empty;
            txtSKUNo.Focus();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if(dtSelectedSKU != null && dtSelectedSKU.Rows.Count > 0)
            {
                try
                {
                    ReadOnlyCollection<object> containerArray;
                    string sMsg = string.Empty;
                    MemoryStream mstr = new MemoryStream();
                    foreach(DataRow dr in dtSelectedSKU.Rows)
                    {
                        dr["WAYBILL"] = txtWayBillNo.Text.Trim();
                        dr["AWBNUM"] = txtAirwayBillNo.Text.Trim();
                    }
                    dtSelectedSKU.AcceptChanges();

                    dtSelectedSKU.WriteXml(mstr, true);

                    mstr.Seek(0, SeekOrigin.Begin);
                    StreamReader sr = new StreamReader(mstr);
                    string sSKU = string.Empty;
                    sSKU = sr.ReadToEnd();
                    if(PosApplication.Instance.TransactionServices.CheckConnection())
                    {
                        bool bStatus = false;
                        string sTransferId = string.Empty;

                        containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("TransferOrderCreate", sSKU);
                        bStatus = Convert.ToBoolean(containerArray[1]);

                        if(bStatus)
                        {
                            sMsg = Convert.ToString(containerArray[2]);
                            MessageBox.Show(sMsg);
                            sTransferId = Convert.ToString(containerArray[3]);
                            if(!string.IsNullOrEmpty(sTransferId))
                            {
                                DataSet dsHdr = new DataSet();
                                DataSet dsDtl = new DataSet();
                                ReadOnlyCollection<object> cTransReport;
                                cTransReport = PosApplication.Instance.TransactionServices.InvokeExtension("GetTransferVoucherInfo", sTransferId);
                                StringReader srTransHdr = new StringReader(Convert.ToString(cTransReport[3]));
                                if(Convert.ToString(cTransReport[3]).Trim().Length > 38)
                                {
                                    dsHdr.ReadXml(srTransHdr);
                                }
                                StringReader srTransDetail = new StringReader(Convert.ToString(cTransReport[4]));
                                if(Convert.ToString(cTransReport[4]).Trim().Length > 38)
                                {
                                    dsDtl.ReadXml(srTransDetail);
                                }
                                //if (dsDtl.Tables[0].Rows.Count > 0)
                                //{

                                //foreach (DataRow dr in dsDtl.Tables[0].Rows)
                                //{ 
                                //    string sCatgNm = co
                                //    if(Convert

                                //}

                                //for (int i = 0; i <(dsDtl.Tables[0].Rows.Count - 1); i++)
                                //{
                                //    string sCatgNm = string
                                //    //if (i == 0)
                                //    //{
                                //    //    string sCatgNm = Convert.ToString(dsDtl.Tables[0].Rows[0]["CATEGORYNAME"]).Trim();
                                //    //}

                                //}

                                // }


                                Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.frmTransOrderCreateRpt reportfrm
                                    = new Report.frmTransOrderCreateRpt(dsHdr, dsDtl, 0);

                                reportfrm.ShowDialog();



                            }
                        }
                        else
                        {
                            MessageBox.Show("Transfer Order failed to create");
                        }
                        ClearControls();
                    }
                }

                catch(Exception ex)
                {
                    MessageBox.Show("Transfer Order failed to create");
                    ClearControls();

                }
            }

        }

        private void btnSKUSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if(PosApplication.Instance.TransactionServices.CheckConnection())
                {
                    ReadOnlyCollection<object> containerArray;
                    string sStoreId = ApplicationSettings.Terminal.StoreId;
                    containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("GetSKUNumber");

                    dsSKU = new DataSet();
                    StringReader srTransDetail = new StringReader(Convert.ToString(containerArray[3]));

                    if(Convert.ToString(containerArray[3]).Trim().Length > 38)
                    {
                        dsSKU.ReadXml(srTransDetail);
                    }

                    if(dsSKU != null && dsSKU.Tables[0].Rows.Count > 0)
                    {
                        Dialog.WinFormsTouch.frmGenericSearch Osearch = new Dialog.WinFormsTouch.frmGenericSearch(dsSKU.Tables[0], null, "SKU");
                        Osearch.ShowDialog();

                        DataRow dr = Osearch.SelectedDataRow;

                        DataRow drSKU;

                        if(dr != null)
                        {
                            txtSKUNo.Text = Convert.ToString(dr["itemid"]);

                            drSKU = dtSelectedSKU.NewRow();

                            drSKU["STOREID"] = ApplicationSettings.Terminal.StoreId;
                            drSKU["TWH"] = sInventLocationId;
                            drSKU["SKUNumber"] = Convert.ToString(dr["itemid"]);
                            dtSelectedSKU.Rows.Add(drSKU);
                            dtSelectedSKU.AcceptChanges();
                            grItems.DataSource = dtSelectedSKU.DefaultView;

                        }
                    }
                }
            }
            catch(Exception ex)
            {


            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ClearControls()
        {
            txtWarehouse.Text = string.Empty;
            txtSKUNo.Text = string.Empty;
            txtWayBillNo.Text = string.Empty;
            txtAirwayBillNo.Text = string.Empty;
            grItems.DataSource = null;
            dtSelectedSKU.Clear();
        }

        private void txtSKUNo_KeyDown(object sender, KeyEventArgs e)
        {

            if(e.KeyValue == 13)
            {
                btnEnter_Click(sender, e);
            }
            /*
            if (dsSKU == null )
            {
                try
                {
                if (PosApplication.Instance.TransactionServices.CheckConnection())
                {
                    ReadOnlyCollection<object> containerArray;
                    string sStoreId = ApplicationSettings.Terminal.StoreId;
                    containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("GetSKUNumber");

                    dsSKU = new DataSet();
                    StringReader srTransDetail = new StringReader(Convert.ToString(containerArray[3]));

                    if (Convert.ToString(containerArray[3]).Trim().Length > 38)
                    {
                        dsSKU.ReadXml(srTransDetail);
                    }
                }
            }
            catch (Exception ex)
            {


            }
            }
            */

            //if (dsSKU != null && dsSKU.Tables[0].Rows.Count > 0)
            //{
            //  string sSelectedSKUNo = txtSKUNo.Text;

            // string sValidSKU = string.Empty;

            //DataRow[] drArr = dsSKU.Tables[0].Select("ITEMID = '" + txtSKUNo.Text.Trim() + "' ");

            //if (drArr != null && drArr.Length > 0)
            //{
            //    sValidSKU = Convert.ToString(drArr[0]["ITEMID"]);
            //}

            //  if (!string.IsNullOrEmpty(sValidSKU))

            //if(!string.IsNullOrEmpty(txtSKUNo.Text.Trim()))
            //  {
            //     // string sSKUExist = string.Empty;

            //    //  DataRow[] drArrEx = dtSelectedSKU.Select("SKUNumber = '" + sValidSKU + "' ");

            //      //if (drArrEx != null && drArrEx.Length > 0)
            //      //{
            //      //    sSKUExist = Convert.ToString(drArrEx[0]["SKUNumber"]);
            //      //}

            //     // if (string.IsNullOrEmpty(sSKUExist))
            //    if(IsValidSKU(txtSKUNo.Text.Trim()))
            //      {
            //          DataRow[] drArrEx = dtSelectedSKU.Select("SKUNumber = '" + txtSKUNo.Text.Trim() + "' ");
            //          if (drArrEx != null && drArrEx.Length == 0)
            //          {
            //              DataRow drSKU;
            //              drSKU = dtSelectedSKU.NewRow();
            //              drSKU["STOREID"] = ApplicationSettings.Terminal.StoreId;
            //              drSKU["TWH"] = txtWarehouse.Text.Trim();
            //              drSKU["SKUNumber"] = txtSKUNo.Text.Trim();// sValidSKU;
            //              dtSelectedSKU.Rows.Add(drSKU);
            //              dtSelectedSKU.AcceptChanges();
            //              grItems.DataSource = dtSelectedSKU.DefaultView;
            //          }
            //      }
            //  }



            // }
        }

        private bool IsValidSKU(string sSKUNo)
        {
            bool bStatus = false;

            //string sQry = "DECLARE @ISVALID INT IF EXISTS (SELECT ITEMID FROM INVENTTABLE WHERE ITEMID = '" + sSKUNo + "' AND DATAAREAID = '"+ ApplicationSettings.Database.DATAAREAID +"' AND RETAIL = 1) "+
            //               " BEGIN SET @ISVALID = 1 END ELSE BEGIN SET @ISVALID = 0 END SELECT ISNULL(@ISVALID,0)";

            string sQry = "DECLARE @ISVALID INT" +
                         " IF EXISTS (SELECT ITEMID FROM INVENTTABLE WHERE ITEMID = '" + sSKUNo + "'" +
                         " AND DATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "' AND RETAIL = 1) " +
                         " AND EXISTS (SELECT SkuNumber FROM SKUTableTrans WHERE SkuNumber = '" + sSKUNo + "'" +
                         " AND DATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "' AND isAvailable  = 0) " +
                         " BEGIN SET @ISVALID = 1 END ELSE BEGIN SET @ISVALID = 0 END SELECT ISNULL(@ISVALID,0)";

            if(connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            using(SqlCommand cmd = new SqlCommand(sQry, connection))
            {
                cmd.CommandTimeout = 0;
                bStatus = Convert.ToBoolean(cmd.ExecuteScalar());
            }

            return bStatus;

        }

        private void txtSKUNo_Leave(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(txtSKUNo.Text.Trim()))
            {
                if(IsValidSKU(txtSKUNo.Text.Trim()))
                {
                    DataRow[] drArrEx = dtSelectedSKU.Select("SKUNumber = '" + txtSKUNo.Text.Trim() + "' ");
                    if(drArrEx != null && drArrEx.Length == 0)
                    {
                        DataRow drSKU;
                        drSKU = dtSelectedSKU.NewRow();
                        drSKU["STOREID"] = ApplicationSettings.Terminal.StoreId;
                        drSKU["TWH"] = sInventLocationId;
                        drSKU["SKUNumber"] = txtSKUNo.Text.Trim();
                        dtSelectedSKU.Rows.Add(drSKU);
                        dtSelectedSKU.AcceptChanges();
                        grItems.DataSource = dtSelectedSKU.DefaultView;
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Item Selected.");
                }
            }
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(txtSKUNo.Text.Trim()))
            {
                if(IsValidSKU(txtSKUNo.Text.Trim()))
                {
                    DataRow[] drArrEx = dtSelectedSKU.Select("SKUNumber = '" + txtSKUNo.Text.Trim() + "' ");
                    if(drArrEx != null && drArrEx.Length == 0)
                    {
                        DataRow drSKU;
                        drSKU = dtSelectedSKU.NewRow();
                        drSKU["STOREID"] = ApplicationSettings.Terminal.StoreId;
                        drSKU["TWH"] = sInventLocationId;
                        drSKU["SKUNumber"] = txtSKUNo.Text.Trim();
                        dtSelectedSKU.Rows.Add(drSKU);
                        dtSelectedSKU.AcceptChanges();
                        grItems.DataSource = dtSelectedSKU.DefaultView;
                        txtSKUNo.Text = "";
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Item Selected.");
                }
            }

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int DeleteSelectedIndex = 0;
            if(dtSelectedSKU != null && dtSelectedSKU.Rows.Count > 0)
            {
                if(grdView.RowCount > 0)
                {
                    DeleteSelectedIndex = grdView.GetSelectedRows()[0];
                    DataRow theRowToDelete = dtSelectedSKU.Rows[DeleteSelectedIndex];

                    dtSelectedSKU.Rows.Remove(theRowToDelete);
                    grItems.DataSource = dtSelectedSKU.DefaultView;

                }

            }

        }
    }
}
