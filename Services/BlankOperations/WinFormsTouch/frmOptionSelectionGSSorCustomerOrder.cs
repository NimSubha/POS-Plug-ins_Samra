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
    public partial class frmOptionSelectionGSSorCustomerOrder : frmTouchBase
    {
        public bool isGSS = false;
        public frmOptionSelectionGSSorCustomerOrder()
        {
            InitializeComponent();
        }

        private void btnNormalCustomerDeposit_Click(object sender, EventArgs e)
        {
            isGSS = false;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            isGSS = true;
            this.Close();
        }
    }
}
