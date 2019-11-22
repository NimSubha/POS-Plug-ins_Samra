using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using LSRetailPosis.Settings;
using LSRetailPosis;
using System.Text.RegularExpressions;
using LSRetailPosis.POSProcesses;

namespace EFT
{
    public partial class frmCard : Form
    {
        private bool getAmountOnly;
        private ICardInfo localCardInfo;
        internal decimal amountPaid = 0.00M;
        internal bool everythingValid = false;

        public string sApprovalCode = string.Empty; // Approval Code
        public string sCardExpMonth = string.Empty;
        public string sCardExpYear = string.Empty;

        public frmCard()
        {
            InitializeComponent();
        }

        public ICardInfo CardInfo
        {
            get { return this.localCardInfo; }
        }


        public frmCard(ICardInfo cardinfo, bool getAmountOnly)
            : this()
        {
            this.localCardInfo = cardinfo;
            this.getAmountOnly = getAmountOnly;

            // When the form opens, CardInfo.Amount represents the current balance due on the transaction
            //      The rounding will get rid of trailing 0.0000000's
            AmountDue.Text = EFT.InternalApplication.Services.Rounding.Round(localCardInfo.Amount, true);  // true to show currency symbol
            AmountPaid.Text = EFT.InternalApplication.Services.Rounding.Round(localCardInfo.Amount, false);

            if (getAmountOnly)
            {
                //Card was swiped to open the window - disable certain fields and mask the credit card number
                cardNumber.Text = EFT.InternalApplication.BusinessLogic.Utility.MaskCardNumber(localCardInfo.CardNumber);
                expDate.Text = localCardInfo.ExpDate;
                expDate.Enabled = false;
                cardNumber.Enabled = false;
            }

            // Setup our local event handler for card swipe.  Really only needed because we don't want the user to swipe when the window
            //  is open.  They should swipe the card from the main POS form which will open our window (with "getAmountOnly")
            EFT.InternalApplication.Services.Peripherals.MSR.MSRMessageEvent -= new MSRMessageEventHandler(this.MSR_MSRMessageEvent);
            EFT.InternalApplication.Services.Peripherals.MSR.MSRMessageEvent += new MSRMessageEventHandler(this.MSR_MSRMessageEvent);
            EFT.InternalApplication.Services.Peripherals.MSR.EnableForSwipe();  //Need to enable to handle the swipe on our window.

        }

        private void MSR_MSRMessageEvent(ICardInfo cardInfo)
        {
            EFT.DisplayInvalidSwipeMessage();
        }


        private void btnOk_Click(object sender, EventArgs e)
        {
            string errorMessage = string.Empty;
            
            //------
            if (string.IsNullOrEmpty(txtApprovalCode.Text))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new frmMessage("Please enter approval code.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                   // Application.ApplicationFramework.POSShowForm(dialog);
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }
                return;
            }

            //----------
            if (!string.IsNullOrEmpty(txtExpMonth.Text) && !string.IsNullOrEmpty(txtExpYear.Text))
            {
                int iCurMonth = Convert.ToInt32(Convert.ToString(DateTime.Now.Month));
                int iCurYear = Convert.ToInt32(Convert.ToString(DateTime.Now.Year).Substring(2));

                if ((iCurYear == Convert.ToInt32(txtExpYear.Text.Trim()))
                    && (iCurMonth > Convert.ToInt32(txtExpMonth.Text.Trim())))
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new frmMessage("Please enter valid month", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    }
                    return;
                
                }
            
            }

            //-------------

            //Perform some basic validation on entered values.  Only set
            //  everythingValid to true if everything is good.

            try
            {
                everythingValid = true;  // Start by assuming true

                if (!getAmountOnly)
                {
                    // If a card was manually entered, we need to make sure it is a valid card as defined in HQ
                    //      We will also set some additional CardInfo fields (they would have been set automatically when swiping)
                    //      We will set these values in the local CardInfo object and then return a new copy.
                    localCardInfo.CardNumber = cardNumber.Text.Trim();

                    // This call will do most of the work for us.  It will attempt to match to a known card type and
                    //      fill in the fields from the RETAILSTORETENDERTYPECARDTABLE and  RETAILTENDERTYPECARDTABLE tables.
                    //      If more than one possible match is found, it will prompt for the correct card type.
                    //  All of this code is in the Card service (Services\Card\Card.cs)
                    EFT.InternalApplication.Services.Card.GetCardType(ref localCardInfo);

                    // Important fields for Statement Posting:  CardTypeId, TenderTypeId

                    // Other fields that can be used to determine if further validations are needed:  
                    //     ModulusCheck, CardFee, ExpDateCheck, ProcessLocally, AllowManualInput, 

                    // Other informational fields:  Issuer, EnterFleetInfo, CardFee, CashBackLimit

                    // And the CardType field tells what card type it is (not the same as CardTypeID):
                    //      0 = InternationalCreditCard, 1 = InternationalDebitCard, 2 = LoyaltyCard, 3 = CorporateCard
                    //      4 = CustomerCard, 5 = EmployeeCard, 6 = SalespersonCard, 7 = GiftCard, 500 = Unknown
                    // The EFT plugin is mainly concerned with the first two (Credit and Debit cards).

                }

                if (localCardInfo.CardType == CardTypes.Unknown)
                {
                    // Card number was not recognized by the POS system.  Two options here:
                    //  A)  Write own logic to identify the card
                    //          This logic would be written in the IdentifyCard() method in EFT.cs and
                    //          called from here.
                    IEFTInfo eftInfo = EFT.InternalApplication.BusinessLogic.Utility.CreateEFTInfo();
                    EFT.InternalApplication.Services.EFT.IdentifyCard(ref localCardInfo, ref eftInfo);

                    //  or B) stop processing and notify user that the card number entered is invalid

                    //errorMessage = ApplicationLocalizer.Language.Translate(1370); // "Broker did not identify the card"
                    //everythingValid = false;

                }

                //Amount paid has to be a non-zero
                amountPaid = decimal.Parse(AmountPaid.Text.Trim());
                localCardInfo.Amount = amountPaid;                      // Added By Nimbus
                localCardInfo.CardNumber = cardNumber.Text.Trim();

                //localCardInfo.

                sApprovalCode = txtApprovalCode.Text.Trim(); // Approval Code
                sCardExpMonth = txtExpMonth.Text.Trim();
                sCardExpYear = txtExpYear.Text.Trim();

                if (everythingValid && amountPaid == 0)
                {
                    errorMessage = ApplicationLocalizer.Language.Translate(50041); // "The amount cannot be zero"
                    everythingValid = false;
                }

                // Add other validations here if needed - set the errorMessage to give feedback to user.
                // Here are a few that the standard product does:
                // Expiration Date:  5366 = "The expiration date is not valid."
                // Manual card not allowed:  1369 = "The card number for this card type cannot be manually entered."
                // Card length:  1371 = "The card number's number of digits is not valid."
                // Debit cards:  Cashback amount:  50027 = "The cashback amount that the customer has requested exceeds the store's cashback limit."
                // Debit cards:  50090 = "The cashback amount must be a positive value."

                // Of course you can add your own as well...

            }
            catch (System.Exception)
            {
                // Give a generic error message here and log the exception
                errorMessage = ApplicationLocalizer.Language.Translate(1000); // "An error occurred performing the action"
                everythingValid = false;
            }

            if (!everythingValid)
            {
                //Report the error message
                EFT.InternalApplication.Services.Dialog.ShowMessage(errorMessage, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                this.Close();
            }
        }

        private void AmountPaid_Validating(object sender, CancelEventArgs e)
        {
            //ToDo:  This is a good place to verify a valid numeric value was entered
        }

        private void expDate_Validating(object sender, CancelEventArgs e)
        {
            //ToDo:  This is a good place to convert expy date to the format needed by your processor.  Strip out extra characters, etc.
        }

        private void cardNumber_Validating(object sender, CancelEventArgs e)
        {
            //ToDo:  A good place strip non-numeric characters, truncate string
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            everythingValid = false;
            this.Close();
        }

        private void frmCard_Load(object sender, EventArgs e)
        {

        }

        private void frmCard_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Need to remove our local event handler for the card swipe and disable the reader (since we enabled it).  The reader will actually
            //  get re-enabled after the card processing is completed.
            EFT.InternalApplication.Services.Peripherals.MSR.MSRMessageEvent -= new MSRMessageEventHandler(MSR_MSRMessageEvent);
            EFT.InternalApplication.Services.Peripherals.MSR.DisableForSwipe();


        }

        private void txtExpMonth_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((!char.IsControl(e.KeyChar)) && (!Regex.IsMatch(Convert.ToString(e.KeyChar), @"[0-9]$")))
            {
                e.Handled = true;
            }
        }

        private void txtExpYear_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((!char.IsControl(e.KeyChar)) && (!Regex.IsMatch(Convert.ToString(e.KeyChar), @"[0-9]$")))
            {
                e.Handled = true;
            }
        }

        private void txtExpMonth_Leave(object sender, EventArgs e)
        {
            if ((txtExpMonth.Text.Trim() != string.Empty) && Convert.ToInt32(txtExpMonth.Text.Trim()) > 12)
            {
                MessageBox.Show("Please enter valid month");
                txtExpMonth.Focus();
            }
        }

        private void txtExpYear_Leave(object sender, EventArgs e)
        {
            if (txtExpYear.Text != string.Empty)
            {
                int iYear = Convert.ToInt32(Convert.ToString(DateTime.Now.Year).Substring(2));

                if (iYear > Convert.ToInt32(txtExpYear.Text.Trim()))
                {
                    MessageBox.Show("Please enter valid year");
                    txtExpYear.Focus();
                }
            }

        }

    }
}
