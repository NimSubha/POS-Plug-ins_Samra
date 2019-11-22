/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.ComponentModel.Composition;
using System.Data.SqlClient;
using LSRetailPosis.DataAccess;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using CustomerOrderType = LSRetailPosis.Transaction.CustomerOrderType;
using LSRetailPosis.Transaction.Line.SaleItem;
using System.Data;

namespace Microsoft.Dynamics.Retail.Pos.ApplicationService
{
    /// <summary>
    /// Class that implements the Application interface
    /// </summary>
    [Export(typeof(IApplicationService))]
    public class ApplicationService : IApplicationService
    {
        /// <summary>
        /// Enum representing the type of transaction for which the receipt is being generated
        /// </summary>
        /// <remarks>This must be kept in sync with RetailReceiptTransaction Base Enum in AX</remarks>
        private enum ReceiptTransactionType
        {
            Unknown = 0,
            Sale = 1,
            Return = 2,
            SalesOrder = 3,
            SalesInvoice = 4,
            Payment = 5,
            CustomerSalesOrder = 6,
            CustomerQuote = 7,
            CustomerGoldOrder = 8,
            //GSSPayment = 9
            OrnamentRepair = 9,
            OrnamentRepairReturn = 10,
            StockTransfer = 11,
            AdvRefund=12 
        }

        #region Member variables

        [Import]
        public IApplication Application { get; set; }

        #endregion

        /// <summary>
        /// Determines the type of receipt transaction from POS transaction
        /// </summary>
        /// <param name="posTransaction">POS transaction</param>
        /// <returns>Appropriate value of ReceiptTransactionType</returns>
        static private ReceiptTransactionType GetReceiptTransType(IPosTransaction posTransaction)
        {
            IRetailTransaction retailTrans;
            CustomerOrderTransaction customerOrderTrans;

            if (posTransaction is CustomerPaymentTransaction)
            {
                #region Commented - GSS Different Sequence Number
                //if (((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationType == "GSS")
                //{
                //    return ReceiptTransactionType.GSSPayment;
                //}
                //else
                //{
                //    return ReceiptTransactionType.Payment;
                // }
                #endregion

                return ReceiptTransactionType.Payment;
            }
            else if (posTransaction is SalesOrderTransaction)
            {
                return ReceiptTransactionType.SalesOrder;
            }
            else if (posTransaction is SalesInvoiceTransaction)
            {
                return ReceiptTransactionType.SalesInvoice;
            }
            else if ((customerOrderTrans = posTransaction as CustomerOrderTransaction) != null)
            {
                switch (customerOrderTrans.OrderType)
                {
                    case CustomerOrderType.SalesOrder:
                        return ReceiptTransactionType.CustomerSalesOrder;

                    case CustomerOrderType.Quote:
                        return ReceiptTransactionType.CustomerQuote;
                }
            }
            else if ((retailTrans = posTransaction as IRetailTransaction) != null)
            {
                //if (retailTrans.NetAmountWithNoTax < 0) // commented this on 17/06/2014 after discussing A.Mitra and S.Sharma
                //{
                //    return ReceiptTransactionType.Return;
                //}
                //else
                //{
                //    return ReceiptTransactionType.Sale;
                //}

                if (retailTrans.NetAmountWithTax < 0)
                {
                    if(retailTrans.RefundReceiptId == "1")
                        return ReceiptTransactionType.AdvRefund;
                    else
                        return ReceiptTransactionType.Return;
                }
                else
                {
                    return ReceiptTransactionType.Sale;
                }
            }
            throw new ArgumentException("Invalid transaction type");
        }

        /// <summary>
        /// Translates receipt transaction type to proper value of the SeedTypes class
        /// </summary>
        /// <param name="transType">Receipt transaction type</param>
        /// <returns>Appropriate value from SeedTypes</returns>
        static private NumberSequenceSeedType GetSeedType(ReceiptTransactionType transType)
        {
            switch (transType)
            {
                case ReceiptTransactionType.Sale:
                    return NumberSequenceSeedType.ReceiptSale;

                case ReceiptTransactionType.Return:
                    return NumberSequenceSeedType.ReceiptReturn;

                case ReceiptTransactionType.SalesOrder:
                    return NumberSequenceSeedType.ReceiptSalesOrder;

                case ReceiptTransactionType.SalesInvoice:
                    return NumberSequenceSeedType.ReceiptSalesInvoice;

                case ReceiptTransactionType.Payment:
                    return NumberSequenceSeedType.ReceiptPayment;

                case ReceiptTransactionType.CustomerSalesOrder:
                    return NumberSequenceSeedType.ReceiptCustomerSalesOrder;

                case ReceiptTransactionType.CustomerQuote:
                    return NumberSequenceSeedType.ReceiptCustomerQuote;

                default:
                    return NumberSequenceSeedType.ReceiptDefault;
            }
        }

        /// <summary>
        /// Create a seed value data
        /// </summary>
        /// <returns></returns>
        private SeedValueData CreateSeedValueData(bool includeTerminal)
        {
            string dataAreaId = Application.Settings.Database.DataAreaID;
            string storeId = ApplicationSettings.Terminal.StoreId;
            SqlConnection conn = Application.Settings.Database.Connection;

            if (includeTerminal)
            {
                string terminalId = ApplicationSettings.Terminal.TerminalId;
                return new SeedValueData(conn, dataAreaId, storeId, terminalId);
            }

            return new SeedValueData(conn, dataAreaId, storeId);
        }

        #region IApplication Members

        /// <summary>
        /// Gets and increments the next available receipt id for the type of transaction given
        /// </summary>
        /// <param name="transaction">Transaction for which the receipt ID is being generated</param>
        /// <returns>Next receipt ID</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Grandfather")]
        public string GetNextReceiptId(IPosTransaction transaction)
        {
            try
            {
                RetailTransaction retailTrans = transaction as RetailTransaction;
                if ((retailTrans != null) && (retailTrans.EntryStatus == PosTransaction.TransactionStatus.Voided))
                {
                    return string.Empty;
                }


                if (FiscalPrinter.FiscalPrinter.Instance.FiscalPrinterEnabled())
                {
                    return FiscalPrinter.FiscalPrinter.Instance.GetNextReceiptId(transaction);
                }

                ReceiptTransactionType transType = GetReceiptTransType(transaction);
                string storeId = LSRetailPosis.Settings.ApplicationSettings.Terminal.StoreId;
                string terminalId = LSRetailPosis.Settings.ApplicationSettings.Terminal.TerminalId;
                string staffId = transaction.OperatorId;

                // Get the template mask for this type of transaction
                string mask;
                bool isIndependent;
                string funcProfileId = LSRetailPosis.Settings.FunctionalityProfiles.Functions.ProfileId;
                ReceiptData receiptData = new ReceiptData(Application.Settings.Database.Connection, Application.Settings.Database.DataAreaID, funcProfileId);
                receiptData.GetReceiptMask((int)transType, out mask, out isIndependent);

                // Get the next receipt seed (sequential numeric) value
                NumberSequenceSeedType seedType = NumberSequenceSeedType.ReceiptDefault;
                if (isIndependent)
                {
                    seedType = GetSeedType(transType);
                }
                string seedValue = GetAndIncrementTerminalSeed(seedType).ToString();
              
                return ReceiptMaskFiller.FillMask(mask, seedValue, storeId, terminalId, staffId);
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        /// <summary>
        /// Returns the current store seed value for the type, then increments in the database
        /// </summary>
        /// <param name="type">Seed type ID (from SeedTypes class)</param>
        /// <returns>Seed value</returns>
        public long GetAndIncrementStoreSeed(NumberSequenceSeedType type)
        {
            SeedValueData seedData = CreateSeedValueData(false);
            return seedData.GetAndIncrementStoreSeed(type);
        }

        /// <summary>
        /// Returns the current terminal seed value for the type, then increments in the database
        /// </summary>
        /// <param name="type">Seed type ID (from SeedTypes class)</param>
        /// <returns>Seed value</returns>
        public long GetAndIncrementTerminalSeed(NumberSequenceSeedType type)
        {
            SeedValueData seedData = CreateSeedValueData(true);
            return seedData.GetAndIncrementTerminalSeed(type);
        }

        /// <summary>
        /// Returns the current terminal seed value for the type
        /// </summary>
        /// <param name="type">Seed type ID (from SeedTypes class)</param>
        /// <returns>Seed value</returns>
        public long GetTerminalSeed(NumberSequenceSeedType type)
        {
            SeedValueData seedData = CreateSeedValueData(true);
            return seedData.GetTerminalSeed(type);
        }

        /// <summary>
        /// Increments the terminal seed value for the type
        /// </summary>
        /// <param name="type">Seed type ID (from SeedTypes class)</param>
        public void IncrementTerminalSeed(NumberSequenceSeedType type)
        {
            SeedValueData seedData = CreateSeedValueData(true);
            seedData.IncrementTerminalSeed(type);
        }

        /// <summary>
        /// Returns the text to be displayed on the task bar icon if it returns an empty string a default Reatil POS string is displayed
        /// </summary>
        public string ApplicationWindowCaption
        {
            get { return null; }
        }

        /// <summary>
        /// Returns the icon to be displayed in the task bar if null is returned then the default Retail POS icon is displayed
        /// </summary>
        public System.Drawing.Icon ApplicationWindowIcon
        {
            get { return null; }
        }

        /// <summary>
        /// Returns the image to be displayed on the logon dialog in the top left corner if null is returned then the default Retail POS image is displayed.
        /// </summary>
        public System.Drawing.Image LoginWindowImage
        {
            get { return null; }
        }

       
        #endregion

    }
}

