/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System.ComponentModel.Composition;
using System.Data.SqlClient;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using LSRetailPosis.POSProcesses;
using System;
using System.Text;
using LSRetailPosis;
using System.Windows.Forms;
using System.Data;
using System.IO;
using LSRetailPosis.Transaction;
using LSRetailPosis.Settings;


namespace EFT
{
    /// <summary>
    /// EFT service provider for processing card payments.
    /// </summary>
    [Export(typeof(IEFT))]
    public class EFT : IEFT
    {
        #region Member variables

        protected SqlConnection Connection { get; set; }
        protected string DataAreaId { get; set; }
        protected string ServerName { get; set; }
        protected string ServerPort { get; set; }
        protected string CompanyId { get; set; }
        protected string TerminalId { get; set; }
        protected string Password { get; set; }
        protected string StoreId { get; set; }
        protected string UserId { get; set; }
        protected int Configuration { get; set; }
        protected int Environment { get; set; }
        protected string ProfileId { get; set; }
        protected int TestMode { get; set; }

        protected string sEFTApprovalCode = string.Empty; // Approval Code
        protected string sCExpMonth = string.Empty;
        protected string sCExpYear = string.Empty;

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

        #endregion

        #region IEFT Members

        /// <summary>
        /// Generate Credit card token is created by a payment SDK processor and represents the credit card number
        /// and is used within AX to process customer order securely
        /// Needs to internally set the posTransaction.CreditCardToken
        /// </summary>
        /// <param name="amount">Amount to display.</param>
        /// <param name="posTransaction">Reference to an IPosTransaction object.</param>
        public void GenerateCardToken(decimal amount, IPosTransaction posTransaction)
        {
            posTransaction.CreditCardToken = "dummy token";
        }

        /// <summary>
        /// Gets the card information and amount for the specific card.
        /// </summary>
        /// <param name="cardInfo">Card information</param>
        /// <returns>True if the system should continue with the authorization process and false otherwise.</returns>
        public bool GetCardInfoAndAmount(ref ICardInfo cardInfo)
        {
            // Show UI. Input Card number, Amount, etc.

            //cardInfo.CardType = CardTypes.InternationalCreditCard;
            //cardInfo.CardNumber = "1234-5678-9012-3456";
            //cardInfo.ExpDate = "05/10";
            //cardInfo.CardReady = true;

            //return cardInfo.CardReady;

            // We have access to all information from the EFT section of the current Hardware Profile
            //      For instance, set "IsTestMode" based on the TestMode checkbox.  Can also get things
            //      like the provider Server Name, UserID, Password, etc.
            cardInfo.IsTestMode = LSRetailPosis.Settings.HardwareProfiles.EFT.TestMode == 1 ? true : false;

            //Other useful settings are in LSRetailPosis.Settings.ApplicationSettings.Terminal.  Examples:
            // LSRetailPosis.Settings.ApplicationSettings.Terminal.StoreCurrency
            // LSRetailPosis.Settings.ApplicationSettings.Terminal.TerminalOperator.OperatorId

            // If the card info is already known from a magnetic swipe, we only need to get the Amount from the window
            bool getAmountOnly = (cardInfo.CardEntryType == CardEntryTypes.MAGNETIC_STRIPE_READ);

            // Need to gather information needed to process the payment.  This can be done interactively with a window
            //      or by interacting with another device (pinpad, signature capture, etc.).  Note that if a card is swiped
            //      from the main window, and OPOS event will be fired which will call this method.

            if (cardInfo.CardType == CardTypes.InternationalDebitCard)
            {
                // We know that the card was swiped and matched a Debit Card
                // TODO:  Use the frmCard as a basis for a Debit Card window
            }
            else
            {
                // We either know the card was swiped and matched a Credit Card or the Card Payment
                //      button was pressed and we need to manually type in a card number.
                using (frmCard cardDialog = new frmCard(cardInfo, getAmountOnly))
                {
                    POSFormsManager.ShowPOSForm(cardDialog);  // Display the window
                    if (getAmountOnly)
                    {
                        //We only want to grab the amount from the window since all other card information was read from the swipe
                        cardInfo.Amount = cardDialog.amountPaid;
                    }
                    else
                    {
                        //Otherwise, we we want to set the cardInfo object to the local version from the window
                        cardInfo = cardDialog.CardInfo;
                    }
                    cardInfo.CardReady = cardDialog.everythingValid; // Pull the valid flag from the window
                    sEFTApprovalCode = cardDialog.sApprovalCode;
                    sCExpMonth = cardDialog.sCardExpMonth;
                    sCExpYear = cardDialog.sCardExpYear;
                }

            }

            // Need to return a true or false for whether everything validated and we can proceed.  CardReady
            //  property has this information so return that.
            return cardInfo.CardReady;

        }

        static internal void DisplayInvalidSwipeMessage()
        {
            //  1112 = "Card swipe is not supported."
            //  This is not a great message and the developer should consider changing it.  It really means that "Card swipe is not supported _at this time_."
            //  The process flow is set up so that the card swipe should happen from the main POS form.  If the user entered the dialog from pressing the "pay card"
            //  button, we want them to manually enter the card so this message is fired if the swipe happens AFTER the window is open.
            using (frmMessage dialog = new frmMessage(1112, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information))
            {
                POSFormsManager.ShowPOSForm(dialog);
            }

            EFT.InternalApplication.Services.Peripherals.MSR.EnableForSwipe();
        }

        /// <summary>
        /// Processes the card payment by establishing a connection with the configured payment processor.
        /// </summary>
        /// <param name="eftInfo">Reference to an EftInfo object.</param>
        /// <param name="posTransaction">Current transaction.</param>
        /// <returns>True if the processing succeeded.</returns>
        public bool ProcessCardPayment(ref IEFTInfo eftInfo, IPosTransaction posTransaction)
        {
            //eftInfo.Authorized = true;
            //eftInfo.IsPendingCapture = true;

            //return eftInfo.Authorized;

            // This is the main entry point for the actual card processing.  All card information that was 
            //    gathered by GetCardInfoAndAmount() has been copied to similar fields in the "eftInfo" object.
            //    Use this information when calling your provider's service.

            // Our example will simulate successful authorization (or refund) randomly based on whether
            //    this method gets hit on an odd or even second.  In a real scenario, this is where you 
            //    would call your payment provider directly (web service, direct sockets, etc.) or call 
            //    out to a DLL with the necessary parameters.

            // All information is stored back to the eftInfo object which is used for printing the receipt,
            //    storing information to the database (RetailTransactionPaymentTrans table),  statement posting, etc.  

            // In this example, we are simulating a 1-step Auth/Capture process, so we will
            //    always leave "IsPendingCapture" as false.

            // The eftInfo class has many properties; most of them are used only for printing on the receipt.
            //      See the EFT_NewSample2012_Readme.docx file for a mapping.
            //
            // Only a few are stored to the RetailTransactionPaymentTrans table:
            //      eftInfo.BatchCode = BATCHID
            //      eftInfo.AuthCode = EFTAUTHENTICATIONCODE
            //      eftInfo.IssuerNumber = CARDTYPEID  ** Only used if cardInfo.CardTypeId was blank after ID'ing the card
            // In addition, the cardInfo.CardNumber (masked) is stored to the CARDORACCOUNT column.
            // You may wish to save additional information to your own table.

            // Note:  Both a SALE and a REFUND process will end up here.  To determine if we're charging the card
            //  or refunding money to the card, simply look at eftInfo.Amount.  If the amount is negative, send a refund request.

            //if (DateTime.Now.Second % 2 == 1) //Uncomment this line to RANDOMLY succeed (odd or even second)
            ////if (true)  //Uncomment this line to ALWAYS succeed
            //{
            //    //Auth was successful
            //    eftInfo.Authorized = true;

            //    //Authorization code returned from processor.  
            //    eftInfo.AuthCode = "123456";

            //    // Some fields only for printing on the receipt (see note above)
            //    eftInfo.TransactionDateTime = DateTime.Now;
            //    eftInfo.Message = String.Empty;
            //    eftInfo.SequenceCode = String.Empty;
            //    eftInfo.EntrySourceCode = String.Empty;
            //    eftInfo.RetrievalReferenceNumber = String.Empty;
            //    eftInfo.AccountType = "Checking";
            //    eftInfo.ResponseCode = String.Empty;
            //    eftInfo.AuthorizedText = "Success";  // Printed on receipt as "EFT Info Message" field

            //    // If performing a two-step Auth/Capture process, would set this to true in order
            //    //    to be caught by the Capture process at the end of the transaction.  Otherwise,
            //    //    the Authorization process must send a Capture request at the same time as the Auth request.
            //    eftInfo.IsPendingCapture = false;

            //}
            //else
            //{
            //    //Authorization failed
            //    eftInfo.Authorized = false;

            //    eftInfo.TransactionDateTime = DateTime.Now;
            //    eftInfo.ProviderResponseCode = "99";
            //    eftInfo.NotAuthorizedText = "Not Approved";

            //    //Error Code and Error Message are shown to the user in pop-up window.
            //    eftInfo.ErrorCode = "99999";
            //    eftInfo.ErrorMessage = "This is why the auth failed.";
            //}

            //if (!eftInfo.Authorized)
            //{
            //    // May want to print decline receipt if authorization failed.
            //    PrintDeclineReceipt(eftInfo);

            //    // Otherwise, set ErrorCode and ErrorMessage will be shown on the screen if Authorized = false
            //    // Note:  If authorization was successful, by default no message is given.  You can add functionality
            //    //      to notify user of success.

            //}

            /* Blocked on 29.07.2013 // 
             
            string sReqXML = "22,0,<?xml version='1.0' encoding='UTF-8' ?><purchase-request><TransactionInput ID='ID000001'><Card><IsManualEntry>false</IsManualEntry><Track1>4550381512383001^SUBHANKAR  KUNDU          ^1012101154730000000000774000000</Track1><Track2>5566204200063157=10031010019400</Track2></Card><Amount><BaseAmount>100</BaseAmount><CurrencyCode>INR</CurrencyCode></Amount><POS><POSReferenceNumber>TL993812</POSReferenceNumber><TransactionTime>2008-09-17T09:30:47.0Z</TransactionTime><Product><Code>SK9881277736</Code><Name>NOKIA 6610</Name><Description>Nokia mobile phone</Description><Category>MBL</Category></Product><Product><Code>SK9881277788</Code><Name>NOKIA 78GL ADPTR</Name><Description>Nokia travel adaptor</Description><Category>MBA</Category></Product><User><ID>16</ID><Name>AMITKUMAR</Name></User><TrackingNumber>818</TrackingNumber></POS></TransactionInput></purchase-request>"; 
            Project1.Class1 oEFTClass = new Project1.Class1();
            String sResponseXML = oEFTClass.Inno(sReqXML);

            DataSet ds = new DataSet();
            using (StringReader stringReader = new StringReader(sResponseXML))
            {
                ds = new DataSet();
                ds.ReadXml(stringReader);
            }

            DataTable dt = new DataTable();
            if (ds != null && ds.Tables.Count > 0)
            {
                 dt = ds.Tables["HostResponse"];
            }

            if (dt != null && dt.Rows.Count > 0)
            {
                string code = Convert.ToString(dt.Rows[0]["ResponseCode"]);
                string message = Convert.ToString(dt.Rows[0]["ResponseMessage"]);
                string approvalcode = Convert.ToString(dt.Rows[0]["ApprovalCode"]);
                if (string.IsNullOrEmpty(approvalcode))
                {
                    eftInfo.Authorized = false;
                    eftInfo.ErrorCode = code;
                    eftInfo.ErrorMessage = message;
                }
            }
            else
            {
                eftInfo.Authorized = false;
                eftInfo.ErrorCode = "99999";
                eftInfo.ErrorMessage = "No Connection Available to authorize the connection.";
            }
             */

            //using (LSRetailPosis.POSProcesses.frmMessage dialog = new frmMessage(sResponseXML, MessageBoxButtons.OK, MessageBoxIcon.Information))
            //{
            //    Application.ApplicationFramework.POSShowForm(dialog);
            //}
            // eftInfo.Authorized = true;
            
            eftInfo.Authorized = true;

            string sTblName = "EXTNDCARDINFO" + ApplicationSettings.Terminal.TerminalId;
            string sCardNo = EFT.InternalApplication.BusinessLogic.Utility.MaskCardNumber(eftInfo.CardNumber);

            RetailTransaction retailTrans = posTransaction as RetailTransaction;
            if (retailTrans != null)
            {

                retailTrans.PartnerData.EFTCardNo = eftInfo.CardNumber; 
                UpdateCardIfo(sTblName, retailTrans.TransactionId, sCardNo, sEFTApprovalCode, sCExpMonth, sCExpYear);
            }

            LSRetailPosis.Transaction.CustomerPaymentTransaction custTrans = posTransaction as LSRetailPosis.Transaction.CustomerPaymentTransaction;
            if (custTrans != null)
            {
                custTrans.PartnerData.EFTCardNo = eftInfo.CardNumber; 
                UpdateCardIfo(sTblName, custTrans.TransactionId, sCardNo, sEFTApprovalCode, sCExpMonth, sCExpYear);
            }

            return eftInfo.Authorized;
        }

        public static void PrintDeclineReceipt(IEFTInfo eftInfo)
        {

            //Prints Simple decline receipt for Merchant and Customer
            //    This is is just simple text, so you can build it any way you want with eftInfo values
            StringBuilder recLayout = new StringBuilder();

            recLayout.AppendLine("--------------------");
            recLayout.AppendLine(eftInfo.CardName);
            recLayout.AppendLine(EFT.InternalApplication.BusinessLogic.Utility.MaskCardNumber(eftInfo.CardNumber));
            recLayout.AppendLine(eftInfo.ProviderResponseCode);
            recLayout.AppendLine("--------------------");
            string receipt = recLayout.ToString();

            EFT.InternalApplication.Services.Peripherals.Printer.PrintReceipt(
                string.Format(receipt, ApplicationLocalizer.Language.Translate(50078))); //Merchant Copy
            EFT.InternalApplication.Services.Peripherals.Printer.PrintReceipt(
                string.Format(receipt, ApplicationLocalizer.Language.Translate(50077))); //Customer Copy
        }

        /// <summary>
        /// Voids the card payment by establishing a connection with the configured payment processor.
        /// </summary>
        /// <param name="eftInfo">Reference to an EftInfo object.</param>
        /// <param name="posTransaction">Current transaction.</param>
        /// <returns>True if the processing succeeded.</returns>
        public bool VoidTransaction(ref IEFTInfo eftInfo, IPosTransaction posTransaction)
        {
            //eftInfo.CardNumberHidden = true;
            //eftInfo.Authorized = true;

            //return eftInfo.Authorized;
            //VoidTransaction() is only called in two very specific scenarios.  The second one is quite rare.

            //   1.  If a partial payment is manually voided before the entire transaction is
            //   completed (by using the "Void Payment" button in the POS).  This will only happen
            //   in partial card payment (split tender) scenarios.

            //   2.  If a card payment is made against a Sales Order or Sales Invoice and the
            //   Retail Transaction Service becomes unavailable before the Sales Order or Sales
            //   Invoice can be marked as "Paid" at Headquarters.  In this case, an error will be thrown
            //   and the card payment or authorization must be rolled back.

            //  All other negative (refund) card transactions will flow through the standard ProcessCardPayment() method


            // TODO:  Add code to call to the processer's API
            eftInfo.Authorized = true;  // Set to true if Void was successful
            return eftInfo.Authorized;
        }

        /// <summary>
        /// Identifies the card if a match with pre-configured card types in not found by the application.
        /// </summary>
        /// <param name="cardInfo">Reference to an EftInfo object.</param>
        /// <param name="eftInfo">Reference to an eftInfo object.</param>
        public void IdentifyCard(ref ICardInfo cardInfo, ref IEFTInfo eftInfo)
        {
            //if (cardInfo != null)
            //{
            //    cardInfo.CardType = CardTypes.Unknown;
            //}
            if (cardInfo != null)
            {
                // This API is exposed for ISVs to identify card types not known internally.
                //
                // Normally, the card type should be determined by matching a number mask set up
                //     in AX for Retail Headquarters.  However, if there are other means to
                //     identify a card, create the logic here and set the TenderTypeID property.
                //     The TenderTypeID should match a CARDID in the RETAILSTORETENDERTYPECARDTABLE table.

                cardInfo.CardType = CardTypes.Unknown;  // InternationalCreditCard = 0, InternationalDebitCard = 1, etc.
                cardInfo.TenderTypeId = String.Empty;

                // At least one of the following two fields needs to be set.  If cardInfo.CardTypeId has a value, it will be saved to
                //      the CARDTYPEID column of the RetailTransactionPaymentTrans table.  Otherwise, the eftInfo.IssuerNumber
                //      will be saved there.  If a NULL is saved to that table, there will be problems down the road.

                //cardInfo.CardTypeId = String.Empty;
                //eftInfo.IssuerNumber = string.Empty;
            }
        }

        /// <summary>
        /// Captures the card payment by establishing a connection with the configured payment processor.
        /// </summary>
        /// <param name="eftInfo">Reference to an EftInfo object.</param>
        /// <param name="posTransaction">Current transaction.</param>
        public void CapturePayment(IEFTInfo eftInfo, IPosTransaction posTransaction)
        {
            // Similar logic to ProcessCardPayment():  send a previously-authorized payment to the processor for capture
            //    This will ONLY get called at the end of a transaction if the IsPendingCapture property is set to true.
            //    Unless the transaction is split tender (two or more card payments) this will normally get called shortly
            //    after the call to ProcessCardPayment().  Any information needed to match up an authorization to the capture
            //    request will be stored in the eftInfo object

            // ToDO:  Add code to call the processer's API
        }

        /// <summary>
        /// Get transaction token for authorized transaction (Secure card storage).
        /// </summary>
        /// <param name="eftInfo">EftInfo object.</param>
        /// <param name="posTransaction">Reference to an IPosTransaction object.</param>
        public void GetTransactionToken(IEFTInfo eftInfo, IPosTransaction posTransaction)
        {
            // call payment processor's credit card tokenization
            // This token can sent to HQ via Transaction Service for Sales Order processing
            string token = string.Empty;
            eftInfo.TransactionToken = token;
        }

        /// <summary>
        /// Return a SigCap service (if availbale)
        /// </summary>
        /// <returns>null if the service is not provided</returns>
        public ISignatureCapture GetSignatureCapture()
        {
            return null;
        }

        /// <summary>
        /// Return a PIN Pad service (if available)
        /// </summary>
        /// <returns>null if service is not provided</returns>
        public IPinPad GetPinPad()
        {
            return null;
        }

        private void UpdateCardIfo(string sTableName, string sTransactionId, string sCardNo,
                                string sApprovalCode, string sExpMonth, string sExpYear)
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("IF NOT EXISTS (SELECT A.NAME FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTableName + "')");
            commandText.Append(" BEGIN CREATE TABLE " + sTableName + "(");
            commandText.Append(" TRANSACTIONID NVARCHAR (20),CARDNO NVARCHAR(30),APPROVALCODE NVARCHAR (20),");
            commandText.Append(" CARDEXPIRYMONTH INT,CARDEXPIRYYEAR INT) END");
            commandText.Append(" IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTableName + "')");
            commandText.Append(" BEGIN  INSERT INTO  " + sTableName + "  VALUES('" + sTransactionId + "','" + sCardNo + "','" + sApprovalCode + "',");
            commandText.Append(" '" + sExpMonth + "','" + sExpYear + "') END");


            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            command.ExecuteNonQuery();
            if (conn.State == ConnectionState.Open)
                conn.Close();

        }
        #endregion
    }
}
