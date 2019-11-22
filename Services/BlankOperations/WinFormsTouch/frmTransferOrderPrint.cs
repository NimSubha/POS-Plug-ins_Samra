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
    public partial class frmTransferOrderPrint : frmTouchBase
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

        public frmTransferOrderPrint()
        {
            InitializeComponent();
        }

        private void btnSummary_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtTransOrdrId.Text.Trim()))
            {
                PrintTransferOrder(txtTransOrdrId.Text.Trim(), 0);
            }
        }

        private void btnDetail_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtTransOrdrId.Text.Trim()))
            {
                PrintTransferOrder(txtTransOrdrId.Text.Trim(), 1);
            }
        }

        private void PrintTransferOrder(string sTransOrdrId, int iOption)
        {
            try
            {
                if (PosApplication.Instance.TransactionServices.CheckConnection())
                {
                    DataSet dsHdr = new DataSet();
                    DataSet dsDtl = new DataSet();
                    ReadOnlyCollection<object> cTransReport;


                    cTransReport = PosApplication.Instance.TransactionServices.InvokeExtension("GetTransferVoucherInfo", sTransOrdrId);


                    StringReader srTransHdr = new StringReader(Convert.ToString(cTransReport[3]));

                    if (Convert.ToString(cTransReport[3]).Trim().Length > 38)
                    {
                        dsHdr.ReadXml(srTransHdr);
                    }

                    StringReader srTransDetail = new StringReader(Convert.ToString(cTransReport[4]));

                    if (Convert.ToString(cTransReport[4]).Trim().Length > 38)
                    {
                        dsDtl.ReadXml(srTransDetail);
                    }

                    Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.frmTransOrderCreateRpt reportfrm
                        = new Report.frmTransOrderCreateRpt(dsHdr, dsDtl, iOption);

                    reportfrm.ShowDialog();
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
