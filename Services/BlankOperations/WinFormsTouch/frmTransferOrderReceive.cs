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
    public partial class frmTransferOrderReceive : frmTouchBase
    {
        public frmTransferOrderReceive()
        {
            InitializeComponent();
        }

        private void btnTransferSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (PosApplication.Instance.TransactionServices.CheckConnection())
                {
                    ReadOnlyCollection<object> containerArray;
                    string sStoreId = ApplicationSettings.Terminal.StoreId;
                    containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("GetTransferId", sStoreId); 

                    DataSet dsTransfer = new DataSet();
                    StringReader srTransDetail = new StringReader(Convert.ToString(containerArray[3]));

                    if (Convert.ToString(containerArray[3]).Trim().Length > 38)
                    {
                        dsTransfer.ReadXml(srTransDetail);
                    }
                    if (dsTransfer != null && dsTransfer.Tables[0].Rows.Count > 0)
                    {
                        Dialog.WinFormsTouch.frmGenericSearch Osearch = new Dialog.WinFormsTouch.frmGenericSearch(dsTransfer.Tables[0], null, "Transfer Order Id");
                        Osearch.ShowDialog();

                        DataRow dr = Osearch.SelectedDataRow;

                        if (dr != null)
                        {
                            txtTransferId.Text = Convert.ToString(dr["TransferId"]); 
                        }
                    }
                }
            }
            catch (Exception ex)
            {


            }

        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                ReadOnlyCollection<object> containerArray;
                string sMsg = string.Empty;

                if (PosApplication.Instance.TransactionServices.CheckConnection())
                {
                    containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("TransferOrderReceipt", txtTransferId.Text.Trim());
                    sMsg = Convert.ToString(containerArray[2]);
                    MessageBox.Show(sMsg);
                    txtTransferId.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {


            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
