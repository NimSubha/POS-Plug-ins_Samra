/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using LSRetailPosis;
using LSRetailPosis.POSProcesses;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.GiftCertificateItem;
using LSRetailPosis.Transaction.Line.TenderItem;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;

namespace Microsoft.Dynamics.Retail.Pos.GiftCard
{
    // Get all text through the Translation function in the ApplicationLocalizer
    //
    // TextID's for the GiftCard service are reserved at 55000 - 55999
    // TextID's for the following modules are as follows:
    //
    // GiftCard.cs:             55000 - 55399. The last in use: 55001
    // GiftCardForm.cs:         55400 - 55499  The last in use:

    [Export(typeof(IGiftCard))]
    public class GiftCard : IGiftCard
    {

        #region IGiftCard Members
        /// <summary>
        /// IApplication instance.
        /// </summary>
        private IApplication application;

        /// <summary>
        /// Gets or sets the IApplication instance.
        /// </summary>
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
                InternalApplication = value;
            }
        }

        /// <summary>
        /// Gets or sets the static IApplication instance.
        /// </summary>
        internal static IApplication InternalApplication { get; private set; }

        /// <summary>
        /// Issues gift card.
        /// </summary>
        /// <param name="posTransaction"></param>
        /// <param name="gcTenderInfo"></param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Grandfather")]
        public void IssueGiftCard(IPosTransaction posTransaction, ITender gcTenderInfo)
        {
            //Start: added on 16/07/2014 for customer selection is must
            RetailTransaction retailTrans = posTransaction as RetailTransaction;

            if (retailTrans != null)
            {
                if (Convert.ToString(retailTrans.Customer.CustomerId) == string.Empty || string.IsNullOrEmpty(retailTrans.Customer.CustomerId))
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Add a customer to transaction before making a deposit", MessageBoxButtons.OK, MessageBoxIcon.Error))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    }
                    return;
                }
            }
            //End :added on 16/07/2014 for customer selection is must

            LogMessage("Issuing a gift card",
                LSRetailPosis.LogTraceLevel.Trace,
                "GiftCard.IssueGiftCard");

            if (FiscalPrinter.FiscalPrinter.Instance.FiscalPrinterEnabled())
            {
                //The operation should proceed after the fiscal printer handles IssueGiftCard
                FiscalPrinter.FiscalPrinter.Instance.IssueGiftCard(posTransaction, gcTenderInfo);
            }

            GiftCardController controller = new GiftCardController(GiftCardController.ContextType.GiftCardIssue, (PosTransaction)posTransaction, (Tender)gcTenderInfo);
            //controller.CardNumber = "111111"; testing for autogenerate no req sailendra da
            
            using (GiftCardForm giftCardForm = new GiftCardForm(controller))
            {
                POSFormsManager.ShowPOSForm(giftCardForm);

                if (giftCardForm.DialogResult == DialogResult.OK)
                {
                    // Add the gift card to the transaction.
                    RetailTransaction retailTransaction = posTransaction as RetailTransaction;
                    GiftCertificateItem giftCardItem = (GiftCertificateItem)this.Application.BusinessLogic.Utility.CreateGiftCardLineItem(
                        ApplicationSettings.Terminal.StoreCurrency,
                        this.Application.Services.Rounding, retailTransaction);

                    giftCardItem.SerialNumber = controller.CardNumber;
                    giftCardItem.StoreId = posTransaction.StoreId;
                    giftCardItem.TerminalId = posTransaction.TerminalId;
                    giftCardItem.StaffId = posTransaction.OperatorId;
                    giftCardItem.TransactionId = posTransaction.TransactionId;
                    giftCardItem.ReceiptId = posTransaction.ReceiptId;
                    giftCardItem.Amount = controller.Amount;
                    giftCardItem.Balance = controller.Balance;
                    giftCardItem.Date = DateTime.Now;

                    // Necessary property settings for the the gift certificate "item"...
                    giftCardItem.Price = giftCardItem.Amount;
                    giftCardItem.StandardRetailPrice = giftCardItem.Amount;
                    giftCardItem.Quantity = 1;
                    giftCardItem.TaxRatePct = 0;
                    giftCardItem.Description = ApplicationLocalizer.Language.Translate(55001);  // Gift Card
                    giftCardItem.Comment = controller.CardNumber;
                    giftCardItem.NoDiscountAllowed = true;
                    giftCardItem.Found = true;

                    retailTransaction.Add(giftCardItem);
                }
            }
        }

        /// <summary>
        /// Updates gift card.
        /// </summary>
        /// <param name="voided"></param>
        /// <param name="comment"></param>
        /// <param name="gcLineItem"></param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Grandfather")]
        public void VoidGiftCard(ref bool voided, ref string comment, IGiftCardLineItem gcLineItem)
        {
            LogMessage("Voiding a gift card",
                LSRetailPosis.LogTraceLevel.Trace,
                "GiftCard.VoidGiftCard");

            if (this.Application.TransactionServices.CheckConnection())
            {
                this.Application.TransactionServices.VoidGiftCard(ref voided, ref comment, gcLineItem.SerialNumber);
            }
        }

        /// <summary>
        /// Gift card payment related methods
        /// </summary>
        /// <param name="valid"></param>
        /// <param name="comment"></param>
        /// <param name="posTransaction"></param>
        /// <param name="cardInfo"></param>
        /// <param name="gcTenderInfo"></param>
        public void AuthorizeGiftCardPayment(ref bool valid, ref string comment, IPosTransaction posTransaction, ICardInfo cardInfo, ITender gcTenderInfo)
        {
            if (cardInfo == null)
                throw new ArgumentNullException("cardInfo");

            LogMessage("Authorizing a gift card payment", LogTraceLevel.Trace);

            valid = false;

            GiftCardController controller = new GiftCardController(GiftCardController.ContextType.GiftCardPayment, (PosTransaction)posTransaction, (Tender)gcTenderInfo);

            controller.CardNumber = cardInfo.CardNumber;
            using (GiftCardForm giftCardForm = new GiftCardForm(controller))
            {
                POSFormsManager.ShowPOSForm(giftCardForm);

                if (giftCardForm.DialogResult == DialogResult.OK)
                {
                    valid = true;
                    cardInfo.CardNumber = controller.CardNumber;
                    cardInfo.Amount = controller.Amount;
                    cardInfo.CurrencyCode = controller.Currency;
                }
            }
        }

        /// <summary>
        /// Void payment of gift card.
        /// </summary>
        /// <param name="voided"></param>
        /// <param name="comment"></param>
        /// <param name="gcTenderLineItem"></param>
        public void VoidGiftCardPayment(ref bool voided, ref string comment, IGiftCardTenderLineItem gcTenderLineItem)
        {
            LogMessage("Cancelling the used marking of the gift card.",
                LSRetailPosis.LogTraceLevel.Trace,
                "GiftCard.VoidGiftCardPayment");

            GiftCertificateTenderLineItem giftCardTenderLineItem = gcTenderLineItem as GiftCertificateTenderLineItem;
            if (giftCardTenderLineItem == null)
            {
                throw new ArgumentNullException("gcTenderLineItem");
            }

            if (this.Application.TransactionServices.CheckConnection())
            {
                this.Application.TransactionServices.VoidGiftCardPayment(ref voided, ref comment, giftCardTenderLineItem.SerialNumber,
                    giftCardTenderLineItem.Transaction.StoreId, giftCardTenderLineItem.Transaction.TerminalId);
            }
        }

        /// <summary>
        /// Updates gift card details.
        /// </summary>
        /// <param name="updated"></param>
        /// <param name="comment"></param>
        /// <param name="gcTenderLineItem"></param>
        public void UpdateGiftCard(ref bool updated, ref string comment, IGiftCardTenderLineItem gcTenderLineItem)
        {
            LogMessage("Reedming money from gift card.",
                LSRetailPosis.LogTraceLevel.Trace,
                "GiftCard.UpdateGiftCard");

            GiftCertificateTenderLineItem giftCardTenderLineItem = gcTenderLineItem as GiftCertificateTenderLineItem;
            if (giftCardTenderLineItem == null)
            {
                throw new ArgumentNullException("gcTenderLineItem");
            }

            decimal balance = 0;

            // Begin by checking if there is a connection to the Transaction Service
            if (this.Application.TransactionServices.CheckConnection())
            {
                this.Application.TransactionServices.GiftCardPayment(ref updated, ref comment, ref balance,
                    giftCardTenderLineItem.SerialNumber, giftCardTenderLineItem.Transaction.StoreId,
                    giftCardTenderLineItem.Transaction.TerminalId, giftCardTenderLineItem.Transaction.OperatorId,
                    giftCardTenderLineItem.Transaction.TransactionId, giftCardTenderLineItem.Transaction.ReceiptId,
                    ApplicationSettings.Terminal.StoreCurrency, giftCardTenderLineItem.Amount, DateTime.Now);

                // Update the balance in Tender line item.
                giftCardTenderLineItem.Balance = balance;
            }
        }

        /// <summary>
        /// Handles Gift Card Balance operation.
        /// </summary>
        public void GiftCardBalance(IPosTransaction posTransaction)
        {
            LogMessage("Inquiring gift card balance.",
                LSRetailPosis.LogTraceLevel.Trace,
                "GiftCard.GiftCardBalance");

            GiftCardController controller = new GiftCardController(GiftCardController.ContextType.GiftCardBalance, (PosTransaction)posTransaction);

            using (GiftCardForm giftCardForm = new GiftCardForm(controller))
            {
                POSFormsManager.ShowPOSForm(giftCardForm);
            }
        }

        /// <summary>
        /// Handles AddTo Gift Card operation.
        /// </summary>
        /// <param name="posTransaction"></param>
        /// <param name="gcTenderInfo"></param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Grandfather")]
        public void AddToGiftCard(IRetailTransaction retailTransaction, ITender gcTenderInfo)
        {
            //Start: added on 16/07/2014 for customer selection is must
            RetailTransaction retailTrans = retailTransaction as RetailTransaction;

            if (retailTrans != null)
            {
                if (Convert.ToString(retailTrans.Customer.CustomerId) == string.Empty || string.IsNullOrEmpty(retailTrans.Customer.CustomerId))
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Add a customer to transaction before making a deposit", MessageBoxButtons.OK, MessageBoxIcon.Error))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    }
                    return;
                }
            }
            //End: added on 16/07/2014 for customer selection is must

            LogMessage("Adding money to gift card.",
                LSRetailPosis.LogTraceLevel.Trace,
                "GiftCard.AddToGiftCard");

            GiftCardController controller = new GiftCardController(GiftCardController.ContextType.GiftCardAddTo, (PosTransaction)retailTransaction, (Tender)gcTenderInfo);

            using (GiftCardForm giftCardForm = new GiftCardForm(controller))
            {
                POSFormsManager.ShowPOSForm(giftCardForm);

                if (giftCardForm.DialogResult == DialogResult.OK)
                {
                    // Add the gift card to the transaction.
                    GiftCertificateItem giftCardItem = (GiftCertificateItem)this.Application.BusinessLogic.Utility.CreateGiftCardLineItem(
                        ApplicationSettings.Terminal.StoreCurrency, this.Application.Services.Rounding, retailTransaction);

                    giftCardItem.SerialNumber   = controller.CardNumber;
                    giftCardItem.StoreId        = retailTransaction.StoreId;
                    giftCardItem.TerminalId     = retailTransaction.TerminalId;
                    giftCardItem.StaffId        = retailTransaction.OperatorId;
                    giftCardItem.TransactionId  = retailTransaction.TransactionId;
                    giftCardItem.ReceiptId      = retailTransaction.ReceiptId;
                    giftCardItem.Amount         = controller.Amount;
                    giftCardItem.Balance        = controller.Balance;
                    giftCardItem.Date           = DateTime.Now;
                    giftCardItem.AddTo          = true;

                    giftCardItem.Price          = giftCardItem.Amount;
                    giftCardItem.StandardRetailPrice = giftCardItem.Amount;
                    giftCardItem.Quantity       = 1;
                    giftCardItem.TaxRatePct     = 0;
                    giftCardItem.Description    = ApplicationLocalizer.Language.Translate(55000);  // Add to Gift Card
                    giftCardItem.Comment        = controller.CardNumber;
                    giftCardItem.NoDiscountAllowed = true;
                    giftCardItem.Found          = true;

                    ((RetailTransaction)retailTransaction).Add(giftCardItem);
                }
            }
        }

        /// <summary>
        /// Void a gift card deposit line item.
        /// </summary>
        /// <param name="voided">Return true if sucessful, false otherwise.</param>
        /// <param name="comment">Error text if failed.</param>
        /// <param name="gcLineItem">Gift card line item.</param>
        public void VoidAddToGiftCard(ref bool voided, ref string comment, IGiftCardLineItem gcLineItem)
        {
            LogMessage("Voiding money addition to gift card.",
                LSRetailPosis.LogTraceLevel.Trace,
                "GiftCard.VoidGiftCardDeposit");

            if (gcLineItem == null)
                throw new ArgumentNullException("gcLineItem");

            decimal balance = 0;

            if (this.Application.TransactionServices.CheckConnection())
            {
                this.Application.TransactionServices.AddToGiftCard(ref voided, ref comment, ref balance, gcLineItem.SerialNumber,
                    gcLineItem.StoreId, gcLineItem.TerminalId, gcLineItem.StaffId, gcLineItem.TransactionId, gcLineItem.ReceiptId,
                    ApplicationSettings.Terminal.StoreCurrency, decimal.Negate(gcLineItem.Amount), DateTime.Now);
            }
        }

        /// <summary>
        /// Log a message to the file.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="traceLevel"></param>
        /// <param name="args"></param>
        private void LogMessage(string message, LogTraceLevel traceLevel, params object[] args)
        {
            ApplicationLog.Log(this.GetType().Name, string.Format(message, args), traceLevel);
        }

        #endregion
    }
}
