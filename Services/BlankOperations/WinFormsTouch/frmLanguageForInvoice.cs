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
    public partial class frmLanguageForInvoice:frmTouchBase
    {

        public bool isEnglish = false;
        public bool isArabic = false;
        public bool isBoth = false;

        public frmLanguageForInvoice()
        {
            InitializeComponent();
        }

        private void btnEnglish_Click(object sender, EventArgs e)
        {
            isEnglish = true;
            this.Close();
        }

        private void btnArabic_Click(object sender, EventArgs e)
        {
            isArabic = true;
            this.Close();
        }

        private void btnBoth_Click(object sender, EventArgs e)
        {
            isBoth = true;
            this.Close();
        }
    }
}
