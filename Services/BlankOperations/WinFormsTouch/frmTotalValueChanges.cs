using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmTotalValueChanges : frmTouchBase
    {
       public decimal dTotAmt = 0m;

        public frmTotalValueChanges()
        {
            InitializeComponent();
        }

        public frmTotalValueChanges(decimal dTotValue)
        {
            InitializeComponent();
            txtTotValue.Text = Convert.ToString(dTotValue);
        }

        private void txtValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtValue.Text == string.Empty && e.KeyChar == '.')
            {
                e.Handled = true;
            }
            else
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }

                if (e.KeyChar == (Char)Keys.Enter)
                {
                    e.Handled = true;
                    btnSubmit_Click(sender, e);
                }
            }
        }

        private void simpleButtonEx1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtValue.Text))
            {
                dTotAmt = Convert.ToDecimal(txtValue.Text);
                this.Close();
            }
            else
            {
                MessageBox.Show("Please enter total value");
                txtValue.Focus();
            }
        }
    }
}
