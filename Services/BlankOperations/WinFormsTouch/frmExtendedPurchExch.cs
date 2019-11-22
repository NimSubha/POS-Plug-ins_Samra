using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.ComponentModel.Composition;
using System.Collections.ObjectModel;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using BlankOperations.WinFormsTouch;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmExtendedPurchExch : frmTouchBase
    {
        DataTable dtExtndPurchExchng;

        frmCustomFieldCalculations objCustomFieldCalculations;

        public frmExtendedPurchExch()
        {
            InitializeComponent();
        }

        public frmExtendedPurchExch(string sBaseItemID, string sBaseUnit, string sConfigId, string sRate,
            frmCustomFieldCalculations objCFC, DataTable dt)
        {
            InitializeComponent();

            dtExtndPurchExchng = dt;

            objCustomFieldCalculations = objCFC;

            txtGrossUnit.Text = sBaseUnit;
            txtNetUnit.Text = sBaseUnit;
            txtNetRate.Text = sRate;
            txtNetPurity.Text = sConfigId;

            txtDiamondUnit.Text = "CT";
            txtStoneUnit.Text = "CT";

            if (dtExtndPurchExchng != null && dtExtndPurchExchng.Rows.Count > 0)
                SetValue();

        }

        private void SetValue()
        {
            if (dtExtndPurchExchng != null && dtExtndPurchExchng.Rows.Count > 0)
            {
                txtRefInvoiceNo.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["REFINVOICENO"]);
                txtGrossWt.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["GROSSWT"]);
                txtGrossUnit.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["GROSSUNIT"]);
                txtDiamondWt.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["DMDWT"]);
                txtDiamondPcs.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["DMDPCS"]);
                txtDiamondUnit.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["DMDUNIT"]);
                txtDiamondAmount.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["DMDAMOUNT"]);
                txtStoneWt.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["STONEWT"]);
                txtStonePcs.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["STONEPCS"]);
                txtStoneUnit.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["STONEUNIT"]);
                txtStoneAmount.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["STONEAMOUNT"]);
                txtNetWt.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["NETWT"]);
                txtNetRate.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["NETRATE"]);
                txtNetUnit.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["NETUNIT"]);
                txtNetPurity.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["NETPURITY"]);
                txtNetAmount.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["NETAMOUNT"]);
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtNetWt.Text) && Convert.ToDecimal(txtNetWt.Text) > 0) // added on 09/07/2014 req by S.Sharma
            {
                if (dtExtndPurchExchng == null || dtExtndPurchExchng.Columns.Count == 0)
                {
                    dtExtndPurchExchng = new DataTable();
                    dtExtndPurchExchng.Columns.Add("REFINVOICENO", typeof(string));
                    dtExtndPurchExchng.Columns.Add("GROSSWT", typeof(decimal));
                    dtExtndPurchExchng.Columns.Add("GROSSUNIT", typeof(string));
                    dtExtndPurchExchng.Columns.Add("DMDWT", typeof(decimal));
                    dtExtndPurchExchng.Columns.Add("DMDPCS", typeof(decimal));
                    dtExtndPurchExchng.Columns.Add("DMDUNIT", typeof(string));
                    dtExtndPurchExchng.Columns.Add("DMDAMOUNT", typeof(decimal));
                    dtExtndPurchExchng.Columns.Add("STONEWT", typeof(decimal));
                    dtExtndPurchExchng.Columns.Add("STONEPCS", typeof(decimal));
                    dtExtndPurchExchng.Columns.Add("STONEUNIT", typeof(string));
                    dtExtndPurchExchng.Columns.Add("STONEAMOUNT", typeof(decimal));
                    dtExtndPurchExchng.Columns.Add("NETWT", typeof(decimal));
                    dtExtndPurchExchng.Columns.Add("NETRATE", typeof(decimal));
                    dtExtndPurchExchng.Columns.Add("NETUNIT", typeof(string));
                    dtExtndPurchExchng.Columns.Add("NETPURITY", typeof(string));
                    dtExtndPurchExchng.Columns.Add("NETAMOUNT", typeof(decimal));
                    // }
                    DataRow dr;
                    dr = dtExtndPurchExchng.NewRow();

                    dr["REFINVOICENO"] = txtRefInvoiceNo.Text.Trim();
                    dr["GROSSWT"] = !string.IsNullOrEmpty(txtGrossWt.Text) ? decimal.Round(Convert.ToDecimal(txtGrossWt.Text), 3, MidpointRounding.AwayFromZero).ToString() : "0";
                    dr["GROSSUNIT"] = txtGrossUnit.Text.Trim();
                    dr["DMDWT"] = !string.IsNullOrEmpty(txtDiamondWt.Text) ? decimal.Round(Convert.ToDecimal(txtDiamondWt.Text), 3, MidpointRounding.AwayFromZero).ToString() : "0";
                    dr["DMDPCS"] = !string.IsNullOrEmpty(txtDiamondPcs.Text) ? decimal.Round(Convert.ToDecimal(txtDiamondPcs.Text), 3, MidpointRounding.AwayFromZero).ToString() : "0";
                    dr["DMDUNIT"] = txtDiamondUnit.Text.Trim();
                    dr["DMDAMOUNT"] = !string.IsNullOrEmpty(txtDiamondAmount.Text) ? decimal.Round(Convert.ToDecimal(txtDiamondAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : "0";
                    dr["STONEWT"] = !string.IsNullOrEmpty(txtStoneWt.Text) ? decimal.Round(Convert.ToDecimal(txtStoneWt.Text), 3, MidpointRounding.AwayFromZero).ToString() : "0";
                    dr["STONEPCS"] = !string.IsNullOrEmpty(txtStonePcs.Text) ? decimal.Round(Convert.ToDecimal(txtStonePcs.Text), 3, MidpointRounding.AwayFromZero).ToString() : "0";
                    dr["STONEUNIT"] = txtStoneUnit.Text.Trim();
                    dr["STONEAMOUNT"] = !string.IsNullOrEmpty(txtStoneAmount.Text) ? decimal.Round(Convert.ToDecimal(txtStoneAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : "0";
                    dr["NETWT"] = !string.IsNullOrEmpty(txtNetWt.Text) ? decimal.Round(Convert.ToDecimal(txtNetWt.Text), 3, MidpointRounding.AwayFromZero).ToString() : "0";
                    dr["NETRATE"] = !string.IsNullOrEmpty(txtNetRate.Text) ? decimal.Round(Convert.ToDecimal(txtNetRate.Text), 2, MidpointRounding.AwayFromZero).ToString() : "0";
                    dr["NETUNIT"] = !string.IsNullOrEmpty(txtNetUnit.Text) ? txtNetUnit.Text.Trim() : "0";
                    dr["NETPURITY"] = txtNetPurity.Text.Trim();
                    dr["NETAMOUNT"] = !string.IsNullOrEmpty(txtNetAmount.Text) ? decimal.Round(Convert.ToDecimal(txtNetAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : "0";
                    dtExtndPurchExchng.Rows.Add(dr);
                }
                else
                {
                    dtExtndPurchExchng.Rows[0]["REFINVOICENO"] = txtRefInvoiceNo.Text.Trim();
                    dtExtndPurchExchng.Rows[0]["GROSSWT"] = !string.IsNullOrEmpty(txtGrossWt.Text) ? decimal.Round(Convert.ToDecimal(txtGrossWt.Text), 3, MidpointRounding.AwayFromZero).ToString() : "0";
                    dtExtndPurchExchng.Rows[0]["GROSSUNIT"] = txtGrossUnit.Text.Trim();
                    dtExtndPurchExchng.Rows[0]["DMDWT"] = !string.IsNullOrEmpty(txtDiamondWt.Text) ? decimal.Round(Convert.ToDecimal(txtDiamondWt.Text), 3, MidpointRounding.AwayFromZero).ToString() : "0";
                    dtExtndPurchExchng.Rows[0]["DMDPCS"] = !string.IsNullOrEmpty(txtDiamondPcs.Text) ? decimal.Round(Convert.ToDecimal(txtDiamondPcs.Text), 3, MidpointRounding.AwayFromZero).ToString() : "0";
                    dtExtndPurchExchng.Rows[0]["DMDUNIT"] = txtDiamondUnit.Text.Trim();
                    dtExtndPurchExchng.Rows[0]["DMDAMOUNT"] = !string.IsNullOrEmpty(txtDiamondAmount.Text) ? decimal.Round(Convert.ToDecimal(txtDiamondAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : "0";
                    dtExtndPurchExchng.Rows[0]["STONEWT"] = !string.IsNullOrEmpty(txtStoneWt.Text) ? decimal.Round(Convert.ToDecimal(txtStoneWt.Text), 3, MidpointRounding.AwayFromZero).ToString() : "0";
                    dtExtndPurchExchng.Rows[0]["STONEPCS"] = !string.IsNullOrEmpty(txtStonePcs.Text) ? decimal.Round(Convert.ToDecimal(txtStonePcs.Text), 3, MidpointRounding.AwayFromZero).ToString() : "0";
                    dtExtndPurchExchng.Rows[0]["STONEUNIT"] = txtStoneUnit.Text.Trim();
                    dtExtndPurchExchng.Rows[0]["STONEAMOUNT"] = !string.IsNullOrEmpty(txtStoneAmount.Text) ? decimal.Round(Convert.ToDecimal(txtStoneAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : "0";
                    dtExtndPurchExchng.Rows[0]["NETWT"] = !string.IsNullOrEmpty(txtNetWt.Text) ? decimal.Round(Convert.ToDecimal(txtNetWt.Text), 3, MidpointRounding.AwayFromZero).ToString() : "0";
                    dtExtndPurchExchng.Rows[0]["NETRATE"] = !string.IsNullOrEmpty(txtNetRate.Text) ? decimal.Round(Convert.ToDecimal(txtNetRate.Text), 2, MidpointRounding.AwayFromZero).ToString() : "0";
                    dtExtndPurchExchng.Rows[0]["NETUNIT"] = !string.IsNullOrEmpty(txtNetUnit.Text) ? txtNetUnit.Text.Trim() : "0";
                    dtExtndPurchExchng.Rows[0]["NETPURITY"] = txtNetPurity.Text.Trim();
                    dtExtndPurchExchng.Rows[0]["NETAMOUNT"] = !string.IsNullOrEmpty(txtNetAmount.Text) ? decimal.Round(Convert.ToDecimal(txtNetAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : "0";
                    dtExtndPurchExchng.AcceptChanges();
                }

                objCustomFieldCalculations.dtExtndPurchExchng = dtExtndPurchExchng;
                objCustomFieldCalculations.txtTotalWeight.Text = txtGrossWt.Text;
                objCustomFieldCalculations.txtQuantity.Text = txtNetWt.Text;
                objCustomFieldCalculations.txtExpectedQuantity.Text = txtNetWt.Text;

                if (dtExtndPurchExchng != null && dtExtndPurchExchng.Rows.Count > 0)
                {
                    decimal dTotalAmt = decimal.Round((Convert.ToDecimal(dtExtndPurchExchng.Rows[0]["DMDAMOUNT"])
                                        + Convert.ToDecimal(dtExtndPurchExchng.Rows[0]["STONEAMOUNT"])
                                        + Convert.ToDecimal(dtExtndPurchExchng.Rows[0]["NETAMOUNT"])), 2, MidpointRounding.AwayFromZero);
                    objCustomFieldCalculations.txtRate.Text = Convert.ToString(dTotalAmt);
                    objCustomFieldCalculations.txtAmount.Text = Convert.ToString(dTotalAmt);
                }
                else
                {
                    objCustomFieldCalculations.txtRate.Text = string.Empty;
                    objCustomFieldCalculations.txtAmount.Text = string.Empty;
                }

                objCustomFieldCalculations.txtLossWeight.Text = "0";

                this.Close();
            }
            else
                MessageBox.Show("Invalid Net Wt");

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            objCustomFieldCalculations.txtTotalWeight.Text = string.Empty;
            objCustomFieldCalculations.txtQuantity.Text = string.Empty;
            objCustomFieldCalculations.txtExpectedQuantity.Text = string.Empty;
            objCustomFieldCalculations.txtRate.Text = txtNetRate.Text;
            objCustomFieldCalculations.txtAmount.Text = string.Empty;
            this.Close();
        }

        #region
        private void txtGrossWt_KeyPress(object sender, KeyPressEventArgs e)
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
            }

        }

        private void txtDiamondPcs_KeyPress(object sender, KeyPressEventArgs e)
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
            }
        }

        private void txtDiamondWt_KeyPress(object sender, KeyPressEventArgs e)
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
            }

        }

        private void txtDiamondAmount_KeyPress(object sender, KeyPressEventArgs e)
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
            }

        }

        private void txtStonePcs_KeyPress(object sender, KeyPressEventArgs e)
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
            }

        }

        private void txtStoneAmount_KeyPress(object sender, KeyPressEventArgs e)
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
            }
        }

        private void txtStoneWt_KeyPress(object sender, KeyPressEventArgs e)
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
            }
        }

        private void txtNetWt_KeyPress(object sender, KeyPressEventArgs e)
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
            }
        }

        private void txtNetRate_KeyPress(object sender, KeyPressEventArgs e)
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
            }
        }
        #endregion

        private void txtGrossWt_Leave(object sender, EventArgs e)
        {
            CalculateNetWtAmount();
        }

        private void txtDiamondWt_Leave(object sender, EventArgs e)
        {
            CalculateNetWtAmount();
        }

        private void txtStoneWt_Leave(object sender, EventArgs e)
        {
            CalculateNetWtAmount();
        }

        private void CalculateNetWtAmount()
        {
            decimal dNetWt = 0m;
            decimal dNetAmt = 0m;
            decimal dGrossWt = (!string.IsNullOrEmpty(txtGrossWt.Text.Trim())) ? Convert.ToDecimal(txtGrossWt.Text.Trim()) : 0m;
            decimal dDmdWt = (!string.IsNullOrEmpty(txtDiamondWt.Text.Trim()) && (Convert.ToDecimal(txtDiamondWt.Text.Trim()) != 0)) ? Convert.ToDecimal(txtDiamondWt.Text.Trim()) / 5 : 0m;
            decimal dStoneWt = (!string.IsNullOrEmpty(txtStoneWt.Text.Trim()) && (Convert.ToDecimal(txtStoneWt.Text.Trim()) != 0)) ? Convert.ToDecimal(txtStoneWt.Text.Trim()) / 5 : 0m;
            decimal dRate = (!string.IsNullOrEmpty(txtNetRate.Text.Trim())) ? Convert.ToDecimal(txtNetRate.Text.Trim()) : 0m;

            if (dGrossWt > 0)
                dNetWt = decimal.Round((dGrossWt - dDmdWt - dStoneWt), 3, MidpointRounding.AwayFromZero);

            if (dNetWt > 0 && dRate > 0)
                dNetAmt = decimal.Round((dNetWt * dRate), 2, MidpointRounding.AwayFromZero);

            txtNetWt.Text = Convert.ToString(dNetWt);
            txtNetAmount.Text = Convert.ToString(dNetAmt);

        }
    }
}
