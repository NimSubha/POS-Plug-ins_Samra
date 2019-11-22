using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmOptionSelectionExchangeBuyback : frmTouchBase //Form
    {
        public bool isExchange = false;
        public bool isFullReturn = false;
        public bool isCancel = false;

        public frmOptionSelectionExchangeBuyback()
        {
            InitializeComponent();
        }

        private void btnExchange_Click(object sender, EventArgs e)
        {
            isExchange = true;
            this.Close();
        }
        private void btnBuyBack_Click(object sender, EventArgs e)
        {
            isExchange = false;
            this.Close();
        }

        private void btnExchangeFull_Click(object sender, EventArgs e)
        {
            isFullReturn = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            isCancel = true;
            this.Close();
        }

        
    }
}
