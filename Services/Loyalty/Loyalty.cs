/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.ComponentModel.Composition;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Forms;
using LSRetailPosis;
using LSRetailPosis.DataAccess;
using LSRetailPosis.DataAccess.DataUtil;
using LSRetailPosis.POSControls.Touch;
using LSRetailPosis.POSProcesses.Common;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.LoyaltyItem;
using LSRetailPosis.Transaction.Line.SaleItem;
using LSRetailPosis.Transaction.Line.TenderItem;
using Microsoft.Dynamics.Retail.Diagnostics;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.BusinessLogic;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using Microsoft.Dynamics.Retail.Pos.Contracts.UI;
using LoyaltyItemUsageType = LSRetailPosis.Transaction.Line.LoyaltyItem.LoyaltyItemUsageType;
using LP = LSRetailPosis.POSProcesses;

namespace Microsoft.Dynamics.Retail.Pos.Loyalty
{
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    [Export(typeof(ILoyalty))]
    public class Loyalty:ILoyalty
    {
        // Get all text through the Translation function in the ApplicationLocalizer
        // TextID's for Loyalty are reserved at 50050 - 50099

        //transaction context
        private RetailTransaction transaction;

        //local UI message
        private frmMessage popupDialog;

        #region Properties

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

        private IUtility Utility
        {
            get { return this.Application.BusinessLogic.Utility; }
        }

        private ICustomerSystem CustomerSystem
        {
            get { return this.Application.BusinessLogic.CustomerSystem; }
        }

        #endregion

        #region ILoyalty Members

        /// <summary>
        /// Returns true if add/update loyalty item to transaction is successfull.
        /// </summary>
        /// <param name="retailTransaction"></param>
        /// <param name="cardNumber"></param>
        /// <returns></returns>
        public bool AddLoyaltyRequest(IRetailTransaction retailTransaction, string cardNumber)
        {
            ICardInfo cardInfo = Utility.CreateCardInfo();
            cardInfo.CardNumber = cardNumber;

            return AddLoyaltyRequest(retailTransaction, cardInfo);
        }

        /// <summary>
        /// Returns true if add/update loyalty item to transaction is successfull.
        /// </summary>
        /// <param name="cardInfo"></param>
        /// <param name="retailTransaction"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Grandfather")]
        public bool AddLoyaltyRequest(IRetailTransaction retailTransaction, ICardInfo cardInfo)
        {
            try
            {
                try
                {

                    NewMessageWindow(50050, LSPosMessageTypeButton.NoButtons, System.Windows.Forms.MessageBoxIcon.Information);

                    LogMessage("Adding a loyalty record to the transaction...",
                        LSRetailPosis.LogTraceLevel.Trace,
                        "Loyalty.AddLoyaltyItem");

                    this.transaction = (RetailTransaction)retailTransaction;

                    // If a previous loyalty item exists on the transaction, the system should prompt the user whether to 
                    // overwrite the existing loyalty item or cancel the operation.
                    if(transaction.LoyaltyItem.LoyaltyCardNumber != null)
                    {
                        // Display the dialog
                        using(frmMessage dialog = new frmMessage(50055, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                        {
                            LP.POSFormsManager.ShowPOSForm(dialog);
                            DialogResult result = dialog.DialogResult;

                            if(result != System.Windows.Forms.DialogResult.Yes)
                            {
                                return false;
                            }
                        }

                        // If card to be overridden is being used as tender type then block loyalty payment.
                        if(transaction.LoyaltyItem.UsageType == LoyaltyItemUsageType.UsedForLoyaltyTender)
                        {
                            LP.POSFormsManager.ShowPOSMessageDialog(3223);  // This transaction already contains a loyalty request.
                            return false;
                        }
                    }

                    // Add the loyalty item to the transaction

                    LoyaltyItem loyaltyItem = GetLoyaltyItem(ref cardInfo);

                    if(loyaltyItem != null)
                    {
                        transaction.LoyaltyItem = loyaltyItem;
                        this.transaction.LoyaltyItem.UsageType = LoyaltyItemUsageType.UsedForLoyaltyRequest;

                        UpdateTransactionWithNewCustomer(loyaltyItem.CustID);

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                finally
                {
                    CloseExistingMessageWindow();
                }
            }
            catch(Exception ex)
            {
                NetTracer.Error(ex, "Loyalty::AddLoyaltyRequest failed for retailTransaction {0} cardInfo {1}", retailTransaction.TransactionId, cardInfo.CardNumber);
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        /// <summary>
        /// This creates a loyalty line item based on the passed in card info, or prompts
        ///  to initialize default loyalty card info if no card info is supplied.
        /// </summary>
        /// <param name="cardInfo">Card info of loyalty card. If null, it will be initialized through user prompt.</param>
        /// <returns>Loyalty line item and loyalty card info if un-initialized</returns>
        private LoyaltyItem GetLoyaltyItem(ref ICardInfo cardInfo)
        {
            if(cardInfo == null)
            {
                // The loyalty card was not swiped and therefore we need to prompt for the card number...

                using(LSRetailPosis.POSProcesses.frmInputNumpad inputDialog = new LSRetailPosis.POSProcesses.frmInputNumpad(true, true))
                {
                    inputDialog.EntryTypes = NumpadEntryTypes.CardValidation;
                    inputDialog.PromptText = LSRetailPosis.ApplicationLocalizer.Language.Translate(50056);   //Loyalty card number
                    inputDialog.Text = ApplicationLocalizer.Language.Translate(50062); // Add loyalty card
                    LP.POSFormsManager.ShowPOSForm(inputDialog);
                    DialogResult result = inputDialog.DialogResult;
                    // Quit if cancel is pressed...
                    if(result != System.Windows.Forms.DialogResult.OK)
                    {
                        return null;
                    }
                    else
                    {
                        cardInfo = Utility.CreateCardInfo();
                        cardInfo.CardEntryType = CardEntryTypes.MANUALLY_ENTERED;
                        cardInfo.CardNumber = inputDialog.InputText;
                        // Set card type to Loyalty card since this is a loyalty payment
                        //  Calling GetCardType sets the tender type properties on the cardInfo object or prompts user for more information.
                        cardInfo.CardType = CardTypes.LoyaltyCard;
                        this.Application.Services.Card.GetCardType(ref cardInfo);
                    }
                }
            }

            // Create the loyalty item
            LoyaltyItem loyaltyItem = (LoyaltyItem)this.Application.BusinessLogic.Utility.CreateLoyaltyItem();

            // Set its properties
            if(cardInfo.CardEntryType == CardEntryTypes.MAGNETIC_STRIPE_READ)
            {
                loyaltyItem.LoyaltyCardNumber = cardInfo.Track2Parts[0];
            }
            else
            {
                loyaltyItem.LoyaltyCardNumber = cardInfo.CardNumber;
            }

            // Check whether the card is allowed to collect loyalty points
            bool cardIsValid = false;
            string comment = string.Empty;
            int loyaltyTenderTypeBase = 0;
            decimal pointsEarned = 0;

            GetPointStatus(ref pointsEarned, ref cardIsValid, ref comment, ref loyaltyTenderTypeBase, loyaltyItem.LoyaltyCardNumber);

            if(cardIsValid)
            {
                loyaltyItem.AccumulatedLoyaltyPoints = pointsEarned;
                GetLoyaltyInfoFromDB(loyaltyItem);

                if(string.IsNullOrEmpty(this.transaction.Customer.CustomerId)
                    || string.IsNullOrEmpty(loyaltyItem.CustID)
                    || string.Compare(this.transaction.Customer.CustomerId, loyaltyItem.CustID, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return loyaltyItem;
                }
                else
                {
                    // loyalty payment could change customer, which is not desirable under various condition. 
                    // All logic is captured in CustomerClear action, so we ask cashier to do that first.              
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSMessageDialog(3222);  // You must clear the customer before performing this operation.
                    return null;
                }
            }
            else
            {
                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSStatusBarText("Card ID: " + loyaltyItem.LoyaltyCardNumber); //License only allows limited number of item sales
                LSRetailPosis.POSProcesses.frmMessage dialog = null;
                try
                {
                    if(string.IsNullOrEmpty(comment))
                    {
                        dialog = new LSRetailPosis.POSProcesses.frmMessage(50058, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    }
                    else
                    {
                        dialog = new LSRetailPosis.POSProcesses.frmMessage(comment, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    }

                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);    // Invalid loyaltycard
                    return null;
                }
                finally
                {
                    if(dialog != null)
                    {
                        dialog.Dispose();
                    }
                }
            }
        }

        private void GetPointStatus(ref decimal points, ref bool valid, ref string comment, ref int loyaltyTenderTypeBase, string loyaltyCardNumber)
        {
            try
            {
                try
                {
                    NewMessageWindow(50051, LSPosMessageTypeButton.NoButtons, System.Windows.Forms.MessageBoxIcon.Information); // Retrieving loyalty status 
                    this.Application.TransactionServices.GetLoyaltyPointsStatus(ref valid, ref comment, ref points, ref loyaltyTenderTypeBase, loyaltyCardNumber);
                }
                finally
                {
                    CloseExistingMessageWindow();
                }
            }
            catch(Exception ex)
            {
                NetTracer.Warning(ex, ex.Message);
                valid = false;
            }
        }

        /// <summary>
        /// Adds loyalty points as per given information.
        /// </summary>
        /// <param name="retailTransaction"></param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Grandfather")]
        public void CalculateLoyaltyPoints(IRetailTransaction retailTransaction)
        {
            try
            {
                LogMessage("Adding loyalty points...",
                    LSRetailPosis.LogTraceLevel.Trace,
                    "Loyalty.CalculateLoyaltyPoints");

                this.transaction = (RetailTransaction)retailTransaction;

                //if we already have a loyalty item for tender, we don't accumulated points for this transaction.
                if(this.transaction.LoyaltyItem != null && this.transaction.LoyaltyItem.UsageType == LoyaltyItemUsageType.UsedForLoyaltyTender)
                {
                    return;
                }

                //calculate points.
                this.transaction.LoyaltyItem.UsageType = LoyaltyItemUsageType.NotUsed;
                decimal totalNumberOfPoints = 0;

                // Get the table containing the point logic
                DataTable loyaltyPointsTable = GetLoyaltyPointsSchemeFromDB(this.transaction.LoyaltyItem.SchemeID);

                // Loop through the transaction and calculate the aquired loyalty points. 
                if(loyaltyPointsTable != null && loyaltyPointsTable.Rows.Count > 0)
                {
                    totalNumberOfPoints = CalculatePointsForTransaction(loyaltyPointsTable);

                    this.transaction.LoyaltyItem.CalculatedLoyaltyPoints = totalNumberOfPoints;
                    this.transaction.LoyaltyItem.UsageType = LoyaltyItemUsageType.UsedForLoyaltyRequest;
                }

                UpdateTransactionAccumulatedLoyaltyPoint();
            }
            catch(Exception ex)
            {
                NetTracer.Error(ex, "Loyalty::CalculateLoyaltyPoints failed for retailTransaction {0}", retailTransaction.TransactionId);
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        /// <summary>
        /// Adds loyalty payments for the given card information.
        /// </summary>
        /// <param name="cardInfo"></param>
        /// <param name="amount"></param>
        /// <param name="retailTransaction"></param>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Grandfather")]
        public void AddLoyaltyPayment(IRetailTransaction retailTransaction, ICardInfo cardInfo, decimal amount)
        {
            try
            {
                try
                {
                    NewMessageWindow(50051, LSPosMessageTypeButton.NoButtons, System.Windows.Forms.MessageBoxIcon.Information);

                    this.transaction = (RetailTransaction)retailTransaction;

                    // Getting the loyalty info for the card, how many points have previously been earned
                    LoyaltyItem paymentLoyaltyItem = GetLoyaltyItem(ref cardInfo);

                    if(paymentLoyaltyItem != null)
                    {
                        //customerData.
                        if(this.transaction.Customer == null || string.Equals(this.transaction.Customer.CustomerId, paymentLoyaltyItem.CustID, StringComparison.OrdinalIgnoreCase) == false)
                        {
                            UpdateTransactionWithNewCustomer(paymentLoyaltyItem.CustID);
                        }

                        // if the amount is higher than the "new" NetAmountWithTax, then it is acceptable to lower the amount
                        if(Math.Abs(amount) > Math.Abs(this.transaction.TransSalePmtDiff))
                        {
                            amount = this.transaction.TransSalePmtDiff;
                        }

                        // Getting all possible loyalty posssiblities for the found scheme id
                        DataTable loyaltyPointsTable = GetLoyaltyPointsSchemeFromDB(paymentLoyaltyItem.SchemeID);

                        decimal totalNumberOfPoints = 0;
                        bool tenderRuleFound = false;

                        // now we add the points needed to pay current tender
                        totalNumberOfPoints = CalculatePointsForTender(ref tenderRuleFound, cardInfo.TenderTypeId, amount, loyaltyPointsTable);

                        if(tenderRuleFound)
                        {
                            bool cardIsValid = false;
                            string comment = string.Empty;
                            int loyaltyTenderTypeBase = 0;
                            decimal pointsEarned = 0;

                            // check to see if the user can afford so many points
                            GetPointStatus(ref pointsEarned, ref cardIsValid, ref comment, ref loyaltyTenderTypeBase, paymentLoyaltyItem.LoyaltyCardNumber);

                            if((cardIsValid) && ((LoyaltyTenderTypeBase)loyaltyTenderTypeBase != LoyaltyTenderTypeBase.NoTender))
                            {
                                if(pointsEarned >= (totalNumberOfPoints * -1))
                                {
                                    //customerData.
                                    if(this.transaction.Customer == null || string.Equals(this.transaction.Customer.CustomerId, paymentLoyaltyItem.CustID, StringComparison.OrdinalIgnoreCase) == false)
                                    {
                                        UpdateTransactionWithNewCustomer(paymentLoyaltyItem.CustID);
                                    }

                                    //Add loyalty item to transaction.
                                    this.transaction.LoyaltyItem = paymentLoyaltyItem;
                                    this.transaction.LoyaltyItem.UsageType = LoyaltyItemUsageType.UsedForLoyaltyTender;

                                    // Gathering tender information
                                    TenderData tenderData = new TenderData(ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID);
                                    ITender tenderInfo = tenderData.GetTender(cardInfo.TenderTypeId, ApplicationSettings.Terminal.StoreId);

                                    // this is the grand total
                                    decimal totalAmountDue = this.transaction.TransSalePmtDiff - amount;

                                    TenderRequirement tenderRequirement = new TenderRequirement((Tender)tenderInfo, amount, true, this.transaction.TransSalePmtDiff);
                                    if(!string.IsNullOrWhiteSpace(tenderRequirement.ErrorText))
                                    {
                                        using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(tenderRequirement.ErrorText, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error))
                                        {
                                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                        }
                                    }

                                    //Add a loyalty tender item to transaction.
                                    LoyaltyTenderLineItem loyaltyTenderItem = (LoyaltyTenderLineItem)this.Application.BusinessLogic.Utility.CreateLoyaltyTenderLineItem();
                                    loyaltyTenderItem.CardNumber = paymentLoyaltyItem.LoyaltyCardNumber;
                                    loyaltyTenderItem.CardTypeId = cardInfo.CardTypeId;
                                    loyaltyTenderItem.Amount = amount;

                                    //tenderInfo.
                                    loyaltyTenderItem.Description = tenderInfo.TenderName;
                                    loyaltyTenderItem.TenderTypeId = cardInfo.TenderTypeId;
                                    loyaltyTenderItem.LoyaltyPoints = totalNumberOfPoints;

                                    //convert from the store-currency to the company-currency...
                                    loyaltyTenderItem.CompanyCurrencyAmount = this.Application.Services.Currency.CurrencyToCurrency(
                                        ApplicationSettings.Terminal.StoreCurrency,
                                        ApplicationSettings.Terminal.CompanyCurrency,
                                        amount);

                                    // the exchange rate between the store amount(not the paid amount) and the company currency
                                    loyaltyTenderItem.ExchrateMST = this.Application.Services.Currency.ExchangeRate(
                                        ApplicationSettings.Terminal.StoreCurrency) * 100;

                                    // card tender processing and printing require an EFTInfo object to be attached. 
                                    // however, we don't want loyalty info to show up where other EFT card info would on the receipt 
                                    //  because loyalty has its own receipt template fields, so we just assign empty EFTInfo object
                                    loyaltyTenderItem.EFTInfo = Application.BusinessLogic.Utility.CreateEFTInfo();
                                    // we don't want Loyalty to be 'captured' by payment service, so explicitly set not to capture to be safe
                                    loyaltyTenderItem.EFTInfo.IsPendingCapture = false;

                                    loyaltyTenderItem.SignatureData = LSRetailPosis.POSProcesses.TenderOperation.ProcessSignatureCapture(tenderInfo, loyaltyTenderItem);

                                    this.transaction.Add(loyaltyTenderItem);
                                }
                                else
                                {
                                    using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(50057, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error))
                                    {
                                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                    } // Not enough points available to complete payment
                                }
                            }
                            else
                            {
                                LSRetailPosis.POSProcesses.frmMessage dialog = null;
                                try
                                {
                                    if(string.IsNullOrEmpty(comment))
                                    {
                                        dialog = new LSRetailPosis.POSProcesses.frmMessage(50058, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                    }
                                    else
                                    {
                                        dialog = new LSRetailPosis.POSProcesses.frmMessage(comment, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                    }

                                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);    // Invalid loyaltycard  
                                }
                                finally
                                {
                                    if(dialog != null)
                                    {
                                        dialog.Dispose();
                                    }
                                }
                            }
                        }
                        else
                        {
                            using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(50059, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog); // Not enough points available to complete payment
                            }
                        }
                    }
                }
                finally
                {
                    CloseExistingMessageWindow();
                }
            }
            catch(Exception ex)
            {
                NetTracer.Error(ex, "Loyalty::AddLoyaltyPayment failed for retailTransaction {0} cardInfo {1} amount {2}", retailTransaction.TransactionId, cardInfo.CardNumber, amount);
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        /// <summary>
        /// Called when used points are being voided.
        /// </summary>
        /// <param name="voided"></param>
        /// <param name="comment"></param>
        /// <param name="retailTransaction"></param>
        /// <param name="loyaltyTenderItem"></param>
        public bool VoidLoyaltyPayment(IRetailTransaction retailTransaction, ILoyaltyTenderLineItem loyaltyTenderItem)
        {
            if(retailTransaction == null)
                throw new ArgumentNullException("retailTransaction");

            if(loyaltyTenderItem == null)
                throw new ArgumentNullException("loyaltyTenderItem");

            this.transaction = (RetailTransaction)retailTransaction;

            this.transaction.VoidPaymentLine(loyaltyTenderItem.LineId);
            this.transaction.LoyaltyItem = (LoyaltyItem)this.Application.BusinessLogic.Utility.CreateLoyaltyItem();

            UpdateTransactionWithNewCustomer(null);

            return true;
        }

        /// <summary>
        /// Called to update points when conclude a transaction.
        /// </summary>
        /// <param name="retailTransaction"></param>
        public void UpdateLoyaltyPoints(IRetailTransaction retailTransaction)
        {
            if(retailTransaction == null)
                throw new ArgumentNullException("retailTransaction");

            this.transaction = (RetailTransaction)retailTransaction;

            // Sending confirmation to the transactions service about earned points
            if(this.transaction.LoyaltyItem != null)
            {
                if(this.transaction.LoyaltyItem.UsageType == LoyaltyItemUsageType.UsedForLoyaltyRequest)
                {
                    UpdateIssuedLoyaltyPoints(this.transaction.LoyaltyItem);

                    if(this.transaction.LoyaltyItem.CalculatedLoyaltyPoints < 0)
                    {
                        bool cardIsValid = false;
                        string comment = string.Empty;
                        int loyaltyTenderTypeBase = 0;
                        decimal pointsEarned = 0M;

                        GetPointStatus(ref pointsEarned, ref cardIsValid, ref comment, ref loyaltyTenderTypeBase, this.transaction.LoyaltyItem.LoyaltyCardNumber);

                        if(cardIsValid && pointsEarned < 0)
                        {
                            string message = string.Format(LSRetailPosis.ApplicationLocalizer.Language.Translate(50500), pointsEarned);

                            using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(message, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog); // Not enough points available to complete payment
                            }
                        }
                    }
                }
                else if(this.transaction.LoyaltyItem.UsageType == LoyaltyItemUsageType.UsedForLoyaltyTender)
                {
                    // Sending confirmation to the transaction service about used points
                    foreach(ITenderLineItem tenderItem in this.transaction.TenderLines)
                    {
                        ILoyaltyTenderLineItem asLoyaltyTenderLineItem = tenderItem as ILoyaltyTenderLineItem;

                        if((asLoyaltyTenderLineItem != null) && !asLoyaltyTenderLineItem.Voided)
                        {
                            if(asLoyaltyTenderLineItem.LoyaltyPoints != 0)
                            {
                                UpdateUsedLoyaltyPoints(asLoyaltyTenderLineItem);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        private void GetLoyaltyInfoFromDB(LoyaltyItem loyaltyItem)
        {
            SqlSelect sql = new SqlSelect("RETAILLOYALTYMSRCARDTABLE M "
                + "INNER JOIN RETAILLOYALTYCUSTTABLE C ON M.LOYALTYCUSTID = C.LOYALTYCUSTID "
                + "INNER JOIN RETAILLOYALTYSCHEMESTABLE S ON S.LOYALTYSCHEMEID = M.LOYALTYSCHEMEID ");
            sql.Select("M.LOYALTYSCHEMEID");
            sql.Select("M.LOYALTYCUSTID");
            sql.Select("ACCOUNTNUM");
            sql.Select("EXPIRATIONTIMEUNIT");
            sql.Select("EXPIRATIONTIMEVALUE");
            sql.Where("M.DATAAREAID", Application.Settings.Database.DataAreaID, true);
            sql.Where("M.CARDNUMBER", loyaltyItem.LoyaltyCardNumber, true);   // Sale Unit of Measure

            DataTable dataTable = new DBUtil(Application.Settings.Database.Connection).GetTable(sql);
            if(dataTable.Rows.Count > 0)
            {
                loyaltyItem.SchemeID = Utility.ToString(dataTable.Rows[0]["LOYALTYSCHEMEID"]);
                loyaltyItem.LoyaltyCustID = Utility.ToString(dataTable.Rows[0]["LOYALTYCUSTID"]);
                loyaltyItem.CustID = Utility.ToString(dataTable.Rows[0]["ACCOUNTNUM"]);
                loyaltyItem.ExpireUnit = Utility.ToInt(dataTable.Rows[0]["EXPIRATIONTIMEUNIT"]);
                loyaltyItem.ExpireValue = Utility.ToInt(dataTable.Rows[0]["EXPIRATIONTIMEVALUE"]);
            }
        }

        private void UpdateTransactionAccumulatedLoyaltyPoint()
        {
            try
            {
                bool valid = false;
                decimal points = 0;
                int loyaltyTenderTypeBase = 0;
                string comment1 = string.Empty;

                GetPointStatus(ref points, ref valid, ref comment1, ref loyaltyTenderTypeBase, this.transaction.LoyaltyItem.LoyaltyCardNumber);
                if(valid && (this.transaction.LoyaltyItem != null))
                {
                    this.transaction.LoyaltyItem.AccumulatedLoyaltyPoints = points;
                }
            }
            catch(Exception ex)
            {
                NetTracer.Error(ex, "Loyalty::UpdateTransactionAccumulatedLoyaltyPoint failed for LoyaltyCardNumber {0}", this.transaction.LoyaltyItem.LoyaltyCardNumber);
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        private DataRow GetRetailGroupLineMember(Int64 recId)
        {
            try
            {
                SqlSelect sqlSelect = new SqlSelect("RETAILGROUPMEMBERLINE");
                sqlSelect.Select("*");
                sqlSelect.Where("RECID", recId, true);
                DataTable groupLineMember = new DBUtil(Application.Settings.Database.Connection).GetTable(sqlSelect);
                if(groupLineMember.Rows.Count == 1)
                {
                    return groupLineMember.Rows[0];
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                NetTracer.Warning(ex, "Loyalty::GetRetailGroupLineMember failed for recId {0}. Returning null.", recId);
                return null;
            }
        }

        private DataTable GetLoyaltyPointsSchemeFromDB(string schemeID)
        {
            try
            {
                SqlSelect sqlSelect = new SqlSelect("RETAILLOYALTYPOINTSTABLE");
                sqlSelect.Select("*");
                sqlSelect.Where("LOYALTYSCHEMEID", schemeID, true);
                sqlSelect.Where("DATAAREAID", Application.Settings.Database.DataAreaID, true);
                return new DBUtil(Application.Settings.Database.Connection).GetTable(sqlSelect);
            }
            catch(Exception ex)
            {
                NetTracer.Warning(ex, "Loyalty::GetLoyaltyPointsSchemeFromDB failed for schemeID {0}. Returning null.", schemeID);
                return null;
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Grandfather")]
        private decimal CalculatePointsForTransactionByScheme(Int64 groupMemberLineRecId, decimal qtyAmountLimit, decimal points, CalculationTypeBase baseType, LoyaltyPointTypeBase type)
        {
            try
            {
                decimal totalQty = 0;
                decimal totalAmount = 0;
                Int64 variantId;
                Int64 productId;
                Int64 categoryId;

                DataRow groupMemberLine = GetRetailGroupLineMember(groupMemberLineRecId);
                if(groupMemberLine == null)
                {
                    NetTracer.Warning("Loyalty:CalculatePointsForTranactionByScheme: groupMemberLine is null");
                    return decimal.Zero;
                }

                categoryId = (Int64)groupMemberLine["Category"];
                productId = (Int64)groupMemberLine["Product"];
                variantId = (Int64)groupMemberLine["Variant"];

                if(type != LoyaltyPointTypeBase.Tender)
                {
                    foreach(SaleLineItem saleLineItem in this.transaction.SaleItems)
                    {
                        bool found = false;

                        if(!saleLineItem.Voided)
                        {
                            ItemData itemData = new ItemData(
                                ApplicationSettings.Database.LocalConnection,
                                ApplicationSettings.Database.DATAAREAID,
                                ApplicationSettings.Terminal.StorePrimaryId);

                            // check for a variant being put on loyalty
                            if(variantId != 0)
                            {
                                found = (variantId == saleLineItem.Dimension.DistinctProductVariantId);
                            }
                            // Check for a product or product master being put on loyalty
                            else if(productId != 0)
                            {
                                found = (productId == saleLineItem.ProductId);
                            }
                            // Check for a category being put on loyalty
                            else if(categoryId != 0)
                            {
                                found = itemData.ProductInCategory(saleLineItem.ProductId, saleLineItem.Dimension.DistinctProductVariantId, categoryId);
                            }
                        }

                        if(found)
                        {
                            totalQty += saleLineItem.UnitQtyConversion.Convert(saleLineItem.Quantity);
                            totalAmount += saleLineItem.NetAmount;
                        }
                    }
                }

                //when check limit, we use absolute value, as in return transaction, qty and amount could be nagative.
                if(qtyAmountLimit > 0)
                {
                    if(baseType == CalculationTypeBase.Amounts)
                    {
                        decimal companyCurrencyAmount = this.Application.Services.Currency.CurrencyToCurrency(
                                        ApplicationSettings.Terminal.StoreCurrency,
                                        ApplicationSettings.Terminal.CompanyCurrency,
                                        totalAmount);

                        //Check QtyAmountLimit only for non-tender loyalty point type.
                        if(Math.Abs(companyCurrencyAmount) >= qtyAmountLimit || type == LoyaltyPointTypeBase.Tender)
                        {
                            return companyCurrencyAmount > 0 ?
                                Math.Floor(companyCurrencyAmount / qtyAmountLimit * points) :
                                Math.Ceiling(companyCurrencyAmount / qtyAmountLimit * points);
                        }
                    }
                    else
                    {
                        if(Math.Abs(totalQty) >= qtyAmountLimit)
                        {
                            return totalQty > 0 ?
                                Math.Floor(totalQty / qtyAmountLimit * points) :
                                Math.Ceiling(totalQty / qtyAmountLimit * points);
                        }
                    }
                }

                // default
                return 0;
            }
            catch(Exception ex)
            {
                NetTracer.Error(ex, "Loyalty::CalculatePointsForTransactionByScheme failed for groupMemberLineRecId {0} qtyAmountLimit {1} points {2}", groupMemberLineRecId, qtyAmountLimit, points);
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        private decimal CalculatePointsForTransaction(DataTable loyaltyPointsTable)
        {
            decimal totalCollectedPoints = 0;
            foreach(DataRow row in loyaltyPointsTable.Rows)
            {
                LoyaltyPointTypeBase type = (LoyaltyPointTypeBase)row["PRODUCTTENDERTYPE"];
                Int64 groupMemberLine = (Int64)row["RETAILGROUPMEMBERLINE"];
                decimal qtyAmountLimit = Convert.ToDecimal(row["QTYAMOUNTLIMIT"].ToString());
                decimal points = Convert.ToDecimal(row["POINTS"]);
                CalculationTypeBase baseCalculationOn = (CalculationTypeBase)row["BASECALCULATIONON"];

                // Loyalty scheme ending date on the AX server is in a date only format
                // E.g. an expiry date of 4/12/2010 means that the loyalty scheme expires by the end of 4/12/2010
                // 

                if(IsValidDate(Convert.ToDateTime(row["VALIDFROM"]), Convert.ToDateTime(row["VALIDTO"])))
                {
                    totalCollectedPoints += CalculatePointsForTransactionByScheme(groupMemberLine, qtyAmountLimit, points, baseCalculationOn, type);
                }
            }

            return totalCollectedPoints;
        }

        private static bool IsValidDate(DateTime validFrom, DateTime validTo)
        {
            DateTime emptyDate = new DateTime(1900, 1, 1);
            bool isValidDate = ((validFrom <= DateTime.Today) && (DateTime.Today <= validTo)
                    || ((validFrom <= DateTime.Today) && (validTo == emptyDate))
                    || ((validFrom == emptyDate) && (validTo == emptyDate)));
            return isValidDate;
        }

        private static decimal CalculatePointsForTender(ref bool tenderRuleFound, string tenderTypeID, decimal amount, DataTable loyaltyPointsTable)
        {
            tenderRuleFound = false;
            foreach(DataRow row in loyaltyPointsTable.Rows)
            {
                LoyaltyPointTypeBase type = (LoyaltyPointTypeBase)row["PRODUCTTENDERTYPE"];
                if(type == LoyaltyPointTypeBase.Tender)
                {
                    string loyaltytenderTypeId = row["RETAILTENDERTYPEID"].ToString();
                    decimal qtyAmountLimit = Convert.ToDecimal(row["QTYAMOUNTLIMIT"].ToString());
                    decimal points = Convert.ToDecimal(row["POINTS"]);
                    CalculationTypeBase baseCalculationOn = (CalculationTypeBase)row["BASECALCULATIONON"];

                    if(IsValidDate(Convert.ToDateTime(row["VALIDFROM"]), Convert.ToDateTime(row["VALIDTO"])))
                    {
                        if(tenderTypeID == loyaltytenderTypeId)
                        {
                            tenderRuleFound = true;
                            if((qtyAmountLimit > 0) && (baseCalculationOn == CalculationTypeBase.Amounts))
                            {
                                decimal companyCurrencyAmount = Loyalty.InternalApplication.Services.Currency.CurrencyToCurrency(
                                        ApplicationSettings.Terminal.StoreCurrency,
                                        ApplicationSettings.Terminal.CompanyCurrency,
                                        amount);

                                // If the company currency amount sign is not held constant, we give back less points than we charge.
                                int sign = Math.Sign(companyCurrencyAmount);
                                return Math.Floor(Math.Abs(companyCurrencyAmount) / qtyAmountLimit * points) * (decimal)sign;
                            }
                        }
                    }
                }
            }

            return 0;
        }

        private void NewMessageWindow(int textID, LSPosMessageTypeButton buttonType, System.Windows.Forms.MessageBoxIcon icon)
        {
            CloseExistingMessageWindow();
            popupDialog = new frmMessage(textID, buttonType, icon);
            this.Application.ApplicationFramework.POSShowFormModeless(popupDialog);
        }

        private void CloseExistingMessageWindow()
        {
            if(popupDialog != null)
            {
                popupDialog.Close();
                popupDialog.Dispose();
            }
        }

        private void UpdateTransactionWithNewCustomer(string customerID)
        {
            if(string.IsNullOrEmpty(customerID))
            {
                NetTracer.Warning("Loyalty::UpdateTransactionWithNewCustomer: customerID is null");
                return;
            }

            Contracts.DataEntity.ICustomer customer = this.CustomerSystem.GetCustomerInfo(customerID);

            if(this.transaction.Customer == null || this.transaction.Customer.CustomerId != customerID)
            {
                this.CustomerSystem.SetCustomer(this.transaction, customer, customer);
            }

            IItemSystem itemSystem = this.Application.BusinessLogic.ItemSystem;
            itemSystem.RecalcPriceTaxDiscount(transaction, true);

            // Calc total.
            this.transaction.CalcTotals();
        }

        private void UpdateIssuedLoyaltyPoints(LoyaltyItem loyaltyItem)
        {
            bool valid = false;
            string comment = string.Empty;
            try
            {
                try
                {
                    NewMessageWindow(50054, LSPosMessageTypeButton.NoButtons, System.Windows.Forms.MessageBoxIcon.Information);

                    this.Application.TransactionServices.UpdateIssuedLoyaltyPoints(
                        ref valid,
                        ref comment,
                        this.transaction.TransactionId,
                        "1",
                        this.transaction.StoreId,
                        this.transaction.TerminalId,
                        loyaltyItem.LoyaltyCardNumber,
                        ((IPosTransactionV1)this.transaction).BeginDateTime,
                        loyaltyItem.CalculatedLoyaltyPoints,
                        this.transaction.ReceiptId,
                        this.transaction.OperatorId);

                    if(valid)
                    {
                        UpdateTransactionAccumulatedLoyaltyPoint();
                    }
                }
                finally
                {
                    CloseExistingMessageWindow();
                }
            }
            catch(Exception ex)
            {
                NetTracer.Warning(ex, "Loyalty::UpdateIssuedLoyaltyPoints failed for loyaltyItem {0}. Setting valid to false.", loyaltyItem.LoyaltyCardNumber);
                valid = false;
            }

            if(!valid)
            {
                frmMessage errDialog = null;
                try
                {
                    if(string.IsNullOrEmpty(comment))
                    {
                        errDialog = new frmMessage(50058, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        errDialog = new frmMessage(comment, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    this.Application.ApplicationFramework.POSShowForm(errDialog);
                    CloseExistingMessageWindow();
                }
                finally
                {
                    if(errDialog != null)
                    {
                        errDialog.Dispose();
                    }
                }
            }
        }

        private void UpdateUsedLoyaltyPoints(ILoyaltyTenderLineItem loyaltyTenderItem)
        {
            bool valid = false;
            string comment = string.Empty;

            try
            {
                try
                {
                    NewMessageWindow(50053, LSPosMessageTypeButton.NoButtons, System.Windows.Forms.MessageBoxIcon.Information);

                    this.Application.TransactionServices.UpdateUsedLoyaltyPoints(
                        ref valid,
                        ref comment,
                        this.transaction.TransactionId,
                        "1",
                        this.transaction.StoreId,
                        this.transaction.TerminalId,
                        loyaltyTenderItem.CardNumber,
                        ((IPosTransactionV1)this.transaction).BeginDateTime,
                        loyaltyTenderItem.LoyaltyPoints,
                        this.transaction.ReceiptId,
                        this.transaction.OperatorId);

                    if(valid)
                    {
                        UpdateTransactionAccumulatedLoyaltyPoint();
                    }
                }
                finally
                {
                    CloseExistingMessageWindow();
                }
            }
            catch(Exception ex)
            {
                NetTracer.Warning(ex, "Loyalty::UpdateUsedLoyaltyPoints failed for loyaltyTenderItem {0}. Setting valid to false.", loyaltyTenderItem.CardNumber);
                valid = false;
            }

            if(!valid)
            {
                frmMessage errDialog = null;
                try
                {
                    if(string.IsNullOrEmpty(comment))
                    {
                        errDialog = new frmMessage(50058, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        errDialog = new frmMessage(comment, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    this.Application.ApplicationFramework.POSShowForm(errDialog);
                    loyaltyTenderItem.LoyaltyPoints = 0;
                    CloseExistingMessageWindow();
                }
                finally
                {
                    if(errDialog != null)
                    {
                        errDialog.Dispose();
                    }
                }
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
    }
}
