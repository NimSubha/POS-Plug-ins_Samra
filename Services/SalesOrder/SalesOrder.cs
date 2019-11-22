/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using LSRetailPosis;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.SaleItem;
using LSRetailPosis.Transaction.Line.SalesOrderItem;
using Microsoft.Dynamics.Retail.Diagnostics;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.BusinessLogic;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using Microsoft.Dynamics.Retail.Pos.Contracts.UI;
using Microsoft.Dynamics.Retail.Pos.DataEntity;
using Microsoft.Dynamics.Retail.Pos.SalesOrder.CustomerOrderParameters;
using Microsoft.Dynamics.Retail.Pos.SalesOrder.WinFormsTouch;
using CustomerOrderMode = LSRetailPosis.Transaction.CustomerOrderMode;
using CustomerOrderType = LSRetailPosis.Transaction.CustomerOrderType;
using DE = Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using DM = Microsoft.Dynamics.Retail.Pos.DataManager;
using SalesOrderItemType = LSRetailPosis.Transaction.Line.SalesOrderItem.SalesOrderItemType;

namespace Microsoft.Dynamics.Retail.Pos.SalesOrder
{
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    [Export(typeof(ISalesOrder))]
    public class SalesOrder : ISalesOrder
    {
        // Get all text through the Translation function in the ApplicationLocalizer
        //
        // TextID's for the SalesOrder service are reserved at 56000 - 56999
        // TextID's for the following modules are as follows:
        //
        // SalesOrder.cs:               56000 - 56099.  The last in use: 56005
        // frmSalesOrder.cs:            56100 - 56199.  The last in use: 
        // frmGetSalesOrder.cs:         56200 - 56299.  The last in use: 56211 
        // Customer Order operations    56300 - 56399

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

        // Date time format based on ISO8601 standard
        // AX DateTimeUtl::ToStr(aDateTime) method also outputs the same format
        // e.g. 2006-07-14T19:05:49
        //
        private const string FixedDateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
        private const string FixedDateFormat = "yyyy-MM-dd";

        private static IList<ReturnReasonCode> returnReasonCodes = null;

        #region UI/workflow related

        /// <summary>
        /// Sales order process of keyboard hardware and the application.
        /// </summary>
        /// <param name="posTransaction"></param>
        public void SalesOrders(IPosTransaction posTransaction)
        {
            RetailTransaction retailTransaction = (RetailTransaction)posTransaction;
            try
            {
                SalesOrderLineItem soLineItem = (SalesOrderLineItem)this.Application.BusinessLogic.Utility.CreateSalesOrderLineItem(
                    ApplicationSettings.Terminal.StoreCurrency,
                    this.Application.Services.Rounding, retailTransaction);

                // Touch based hardware...
                using (frmSalesOrder salesOrderDialog = new frmSalesOrder())
                {
                    salesOrderDialog.Transaction = retailTransaction;
                    this.Application.ApplicationFramework.POSShowForm(salesOrderDialog);

                    // Quit if cancel is pressed...
                    if (salesOrderDialog.DialogResult == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return;
                    }
                }

                System.Windows.Forms.Application.DoEvents();
            }
            catch (PosisException px)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), px);
                throw;
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        /// <summary>
        /// Sales order process of keyboard hardware and the application.
        /// </summary>
        /// <param name="posTransaction"></param>
        public IPosTransaction SelectCustomerOrder(IPosTransaction posTransaction)
        {
            RetailTransaction retailTransaction = (RetailTransaction)posTransaction;
            try
            {
                // Touch based hardware...
                using (frmSalesOrder salesOrderDialog = new frmSalesOrder())
                {
                    salesOrderDialog.Transaction = retailTransaction;
                    this.Application.ApplicationFramework.POSShowForm(salesOrderDialog);

                    // Quit if cancel is pressed...
                    if (salesOrderDialog.DialogResult == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return posTransaction;
                    }
                    else
                    {
                        ((PosTransaction)salesOrderDialog.Transaction).RecalledOrder = true;
                        SalesOrderActions.WarnAndFlagForRecalculation((CustomerOrderTransaction)salesOrderDialog.Transaction);
                        return salesOrderDialog.Transaction;
                    }
                }

            }
            catch (PosisException px)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), px);
                throw;
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        /// <summary>
        /// Show Sales order form (for pack slip operations)
        /// </summary>
        /// <param name="posTransaction"></param>
        public void ShowSalesOrders(IPosTransaction posTransaction)
        {
            RetailTransaction transaction = (RetailTransaction)posTransaction;
            string custId = (transaction.Customer.CustomerId) ?? string.Empty;
            PackslipOrderListModel list = new PackslipOrderListModel(custId);

            try
            {
                // Show the available sales orders for selection...
                using (frmGetSalesOrder salesOrdersDialog = new frmGetSalesOrder(list))
                {
                    SalesOrder.InternalApplication.ApplicationFramework.POSShowForm(salesOrdersDialog);
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        /// <summary>
        /// Overrides the price as per given transaction information.
        /// </summary>
        /// <param name="posTransaction"></param>
        /// <param name="operationInfo"></param>
        public void PriceOverride(IPosTransaction posTransaction, object operationInfo)
        {
            try
            {
                //Get the item to be overridden.
                SalesOrderLineItem saleLineItem = null;
                LSRetailPosis.POSProcesses.OperationInfo asOperationInfo = operationInfo as LSRetailPosis.POSProcesses.OperationInfo;

                saleLineItem = (SalesOrderLineItem)((RetailTransaction)posTransaction).GetItem(asOperationInfo.ItemLineId);

                //Price can not be overridden for sales orders retrieved for full payment.  Only for sales orders retrieved for prepayment...
                if (saleLineItem.SalesOrderItemType == SalesOrderItemType.FullPayment)
                {
                    using (LSRetailPosis.POSProcesses.frmMessage msgDialog = new LSRetailPosis.POSProcesses.frmMessage(LSRetailPosis.ApplicationLocalizer.Language.Translate(56001), MessageBoxButtons.OK, MessageBoxIcon.Error))  //The price can only be overridden on sales orders marked for full payment.
                    {
                        this.Application.ApplicationFramework.POSShowForm(msgDialog);

                        return;
                    }
                }

                bool inputValid = true;
                do
                {
                    inputValid = true;
                    // Get the new price...
                    string inputText;

                    if ((operationInfo != null) && !string.IsNullOrEmpty(asOperationInfo.NumpadValue))
                    {
                        // The user entered a specific quantity in the numpad prior to the operation
                        inputText = asOperationInfo.NumpadValue;
                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmInputNumpad inputDialog = new LSRetailPosis.POSProcesses.frmInputNumpad())
                        {
                            inputDialog.EntryTypes = NumpadEntryTypes.Price;
                            inputDialog.PromptText = LSRetailPosis.ApplicationLocalizer.Language.Translate(56002);  // "Enter amount";
                            this.Application.ApplicationFramework.POSShowForm(inputDialog);

                            // Quit if cancel is pressed...
                            if (inputDialog.DialogResult == System.Windows.Forms.DialogResult.Cancel)
                            {
                                return;
                            }

                            inputText = inputDialog.InputText;
                        }
                    }

                    if (!string.IsNullOrEmpty(inputText))
                    {
                        decimal price = Convert.ToDecimal(inputText);
                        if (price == 0)
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage msgDialog = new LSRetailPosis.POSProcesses.frmMessage(LSRetailPosis.ApplicationLocalizer.Language.Translate(56003), MessageBoxButtons.OK, MessageBoxIcon.Error))   // The amount cannot be zero.
                            {
                                this.Application.ApplicationFramework.POSShowForm(msgDialog);
                                return;
                            }
                        }

                        if (price < saleLineItem.PrepayAmount)
                        {
                            //The new price must not be lower then the calculated amount for prepayment
                            using (LSRetailPosis.POSProcesses.frmMessage msgDialog = new LSRetailPosis.POSProcesses.frmMessage(LSRetailPosis.ApplicationLocalizer.Language.Translate(56004), MessageBoxButtons.OK, MessageBoxIcon.Error))  // The amount cannot be lower then the calculated amount for prepayment.
                            {
                                this.Application.ApplicationFramework.POSShowForm(msgDialog);

                                inputValid = false;
                            }
                        }
                        else if (price > saleLineItem.Balance)
                        {
                            //The new price must not be higher then the balance of the sales order
                            using (LSRetailPosis.POSProcesses.frmMessage msgDialog = new LSRetailPosis.POSProcesses.frmMessage(LSRetailPosis.ApplicationLocalizer.Language.Translate(56005), MessageBoxButtons.OK, MessageBoxIcon.Error))   // The amount cannot be higher then the balance of the sales order.
                            {
                                this.Application.ApplicationFramework.POSShowForm(msgDialog);

                                inputValid = false;
                            }
                        }
                        else
                        {
                            //Price can be overridden
                            saleLineItem.Amount = price;
                            saleLineItem.Price = price;
                            saleLineItem.StandardRetailPrice = price;
                        }
                    }
                    else
                    {
                        inputValid = false;
                    }
                } while (!inputValid);

            }
            catch (PosisException px)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), px);
                throw;
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        /// <summary>
        /// Displays the Customer Order UI
        /// </summary>
        /// <param name="posTransaction"></param>
        public IRetailTransaction CustomerOrderDetails(IRetailTransaction retailTransaction)
        {
            if (retailTransaction == null)
            {
                NetTracer.Warning("retailTransaction parameter is null");
                throw new ArgumentNullException("retailTransaction");
            }

            ICustomerOrderTransaction customerOrderTransaction = retailTransaction as ICustomerOrderTransaction;
            if (customerOrderTransaction == null)
            {
                // If the current transaction isn't already an order, convert it to the default order type (SalesOrder|Quote) for this terminal.
                customerOrderTransaction = ConvertToDefaultCustomerOrder(retailTransaction);
            }

            if (string.IsNullOrWhiteSpace(customerOrderTransaction.OrderId))
            {
                // Show form in create mode
                SalesOrderActions.ShowOrderDetails(customerOrderTransaction, OrderDetailsSelection.ViewDetails);
            }
            else
            {
                // Show options for existing
                customerOrderTransaction = SalesOrderActions.ShowOrderDetailsOptions(customerOrderTransaction);
            }

            // Refresh prices/totals
            Application.BusinessLogic.ItemSystem.CalculatePriceTaxDiscount(customerOrderTransaction);
            customerOrderTransaction.CalcTotals();

            return customerOrderTransaction;
        }


        /// <summary>
        /// Calculate total deposit required for the customer order transaction
        /// </summary>
        /// <param name="retailTransaction"></param>
        public void CalculateDeposit(IRetailTransaction retailTransaction)
        {
            if (retailTransaction == null)
            {
                NetTracer.Warning("retailTransaction parameter is null");
                throw new ArgumentNullException("retailTransaction");
            }

            CustomerOrderTransaction coTrans = retailTransaction as CustomerOrderTransaction;

            if (coTrans == null || coTrans.OrderType == CustomerOrderType.Quote)
            {
                coTrans.PrepaymentAmountRequired = decimal.Zero;
                coTrans.PrepaymentAmountInvoiced = decimal.Zero;
                return;
            }

            if (!coTrans.PrepaymentAmountOverridden)
            {
                if ((coTrans.OrderType == CustomerOrderType.SalesOrder) && (coTrans.Mode != CustomerOrderMode.Return))
                {
                    decimal depositPercent = ApplicationSettings.Terminal.MinimumDepositForSalesOrder;
                    decimal depositAmount = 0m;

                    decimal totalInvoicedAmount = 0m;
                    decimal invoicedDepositAmount = 0m;

                    // Calculate the total amount of already picked up items.
                    if (coTrans.Mode != CustomerOrderMode.Pickup)
                    {
                        foreach (ISaleLineItem saleLineItem in coTrans.SaleItems)
                        {
                            if (saleLineItem.QuantityOrdered > decimal.Zero && saleLineItem.QuantityPickedUp > decimal.Zero)
                            {
                                totalInvoicedAmount += (saleLineItem.NetAmountWithTax / saleLineItem.QuantityOrdered) * saleLineItem.QuantityPickedUp;
                            }
                        }

                        if (depositPercent >= decimal.Zero)
                        {
                            // % deposit used for the invoiced amounts
                            invoicedDepositAmount = Application.Services.Rounding.Round((totalInvoicedAmount * depositPercent) / 100m);
                        }
                        coTrans.PrepaymentAmountInvoiced = Application.Services.Rounding.Round(invoicedDepositAmount, coTrans.StoreCurrencyCode);
                    }

                    if (depositPercent >= decimal.Zero)
                    {   // Compute deposit amount.  It is based upon the netAmountWithTaxAndCharges excluding Cancelation Charges

                        // Get the Cancelation Charge amount
                        decimal cancellationCharge = CalculateCancellationCharge(coTrans);
                        decimal netAmountWithTaxAndCharges = coTrans.NetAmountWithTaxAndCharges - cancellationCharge;

                        // % deposit required for un invoiced amount
                        depositAmount = (netAmountWithTaxAndCharges * depositPercent) / 100m;
                    }
                    coTrans.PrepaymentAmountRequired = Application.Services.Rounding.Round(depositAmount, coTrans.StoreCurrencyCode);

                    //Determine how much deposit to apply for a pickup
                    if (coTrans.Mode == CustomerOrderMode.Pickup)
                    {
                        // Determine how much deposit we have available (paid less invoiced).
                        // Cap at zero if we have invoiced more than we paid, this happens if we underpaid deposit during creation and paid the difference at pick-up time.
                        decimal availableDeposit = (coTrans.PrepaymentAmountInvoiced > coTrans.PrepaymentAmountPaid) ? decimal.Zero : (coTrans.PrepaymentAmountPaid - coTrans.PrepaymentAmountInvoiced);

                        // Due is the normal amount for the items, with credit for up to the calculated deposit for those items (but also bounded by the actual available deposit)
                        coTrans.PrepaymentAmountApplied = Math.Min(availableDeposit, coTrans.PrepaymentAmountRequired);
                    }
                    else
                    {
                        coTrans.PrepaymentAmountApplied = decimal.Zero;
                    }
                }
            }
        }

        /// Calculate the cancellation charge for this customer order transaction
        /// </summary>
        /// <param name="retailTransaction"></param>
        public decimal CalculateCancellationCharge(IRetailTransaction retailTransaction)
        {
            if (retailTransaction == null)
            {
                NetTracer.Warning("retailTransaction parameter is null");
                throw new ArgumentNullException("retailTransaction");
            }

            CustomerOrderTransaction coTrans = retailTransaction as CustomerOrderTransaction;

            decimal cancellationChargeAmount = Decimal.Zero;

            if (coTrans != null)
            {
                cancellationChargeAmount = coTrans.MiscellaneousCharges.Where(c => (c.ChargeCode == ApplicationSettings.Terminal.CancellationChargeCode)).Sum(c => (c.Amount + c.TaxAmountExclusive));
            }

            cancellationChargeAmount = Application.Services.Rounding.Round(cancellationChargeAmount, retailTransaction.StoreCurrencyCode);
            return cancellationChargeAmount;
        }

        /// <summary>
        /// Calculate the remaining balance to authorize for shipping
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public decimal CalculateShippingBalanceToAuthorize(ICustomerOrderTransaction transaction)
        {
            CustomerOrderTransaction cot = transaction as CustomerOrderTransaction;
            if (transaction == null)
            {
                NetTracer.Warning("transaction parameter is null");
                throw new ArgumentNullException("transaction");
            }

            decimal balance = decimal.Zero;

            // valid edit mode
            bool validMode;
            switch (cot.Mode)
            {
                case CustomerOrderMode.Convert:
                case CustomerOrderMode.Create:
                case CustomerOrderMode.Edit:
                    validMode = true;
                    break;

                default:
                    NetTracer.Information("SalesOrder::CalculateShippingBalanceToAuthorize: unsupported CustomerOrderTransaction Mode {0}", cot.Mode);
                    validMode = false;
                    break;
            }

            // delivery method is shipping (not blank and not equal to the PickUp delivery method, everything else is considered Shipping)
            bool deliveryIsShipping = (cot.DeliveryMode != null
                    || string.Equals(cot.DeliveryMode.Code, ApplicationSettings.Terminal.PickupDeliveryModeCode, StringComparison.OrdinalIgnoreCase));

            if (validMode && deliveryIsShipping)
            {
                balance = transaction.NetAmountWithTaxAndCharges - transaction.PrepaymentAmountRequired;
            }
            return balance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="retailTransaction"></param>
        /// <returns></returns>
        private ICustomerOrderTransaction ConvertToDefaultCustomerOrder(IRetailTransaction retailTransaction)
        {
            switch (ApplicationSettings.Terminal.DefaultOrderType)
            {
                case Terminal.RetailCustomerOrderType.Quotation:
                    return ConvertToCustomerQuote(retailTransaction);
                default:
                    return ConvertToCustomerOrder(retailTransaction);
            }
        }

        /// <summary>
        /// Convert a retail transaction into a Customer Order
        /// </summary>
        /// <param name="posTransaction"></param>
        /// <returns></returns>
        public ICustomerOrderTransaction ConvertToCustomerOrder(IRetailTransaction retailTransaction)
        {
            if (retailTransaction == null)
            {
                NetTracer.Warning("retailTransaction parameter is null");
                throw new ArgumentNullException("IRetailTransaction");
            }

            CustomerOrderTransaction trans = retailTransaction as CustomerOrderTransaction;

            if (trans != null)
            {
                // Transaction already was a CustomerOrderTransaction
                if (trans.OrderType == CustomerOrderType.Quote)
                {
                    // converting a sales order into a quote
                    SalesOrderActions.WarnAndFlagForRecalculation(trans);
                }
            }
            else
            {
                trans = (CustomerOrderTransaction)this.Application.BusinessLogic.Utility.CreateCustomerOrderTransaction(retailTransaction, this);
            }

            trans.OrderType = CustomerOrderType.SalesOrder;
            SalesOrderActions.SetCustomerOrderDefaults(trans);

            return trans;
        }

        /// <summary>
        /// Convert a retail transaction into a Customer Quote
        /// </summary>
        /// <param name="retailTransaction"></param>
        /// <returns></returns>
        public ICustomerOrderTransaction ConvertToCustomerQuote(IRetailTransaction retailTransaction)
        {
            if (retailTransaction == null)
            {
                NetTracer.Warning("retailTransaction parameter is null");
                throw new ArgumentNullException("retailTransaction");
            }

            CustomerOrderTransaction trans;

            if ((trans = retailTransaction as CustomerOrderTransaction) != null)
            {
                // Fail if this Order is a SalesOrder and has already been issued an ID from AX.
                if ((trans.OrderType == CustomerOrderType.SalesOrder)
                    && (!string.IsNullOrEmpty(trans.OrderId)))
                {
                    NetTracer.Error("SalesOrder::ConvertToCustomerQuote: this order is a sales order that has already been issued an ID from AX (retailTransaction: {0})", retailTransaction.TransactionId);
                    throw new InvalidOperationException();
                }
            }
            else
            {
                trans = (CustomerOrderTransaction)this.Application.BusinessLogic.Utility.CreateCustomerOrderTransaction(retailTransaction, this);
            }
            trans.OrderType = CustomerOrderType.Quote;
            SalesOrderActions.SetCustomerOrderDefaults(trans);
            return trans;
        }

        /// <summary>
        /// Validate that the given order contains all required information
        /// </summary>
        /// <param name="posTransaction"></param>
        /// <returns></returns>
        internal static bool ValidateCustomerOrder(IRetailTransaction posTransaction)
        {
            CustomerOrderTransaction cot = posTransaction as CustomerOrderTransaction;

            // Is a customer order?
            if (cot == null)
            {
                NetTracer.Warning("SalesOrder::ValidateCustomerOrder: cot is null");
                return false;
            }

            // Has line items?
            if (cot.SaleItems.Count == 0)
            {
                NetTracer.Warning("SalesOrder::ValidateCustomerOrder: cot.SalesItems.Count is zero");
                return false;
            }

            //No customer?
            if (cot.Customer == null || cot.Customer.IsEmptyCustomer())
            {
                NetTracer.Warning("SalesOrder::ValidateCustomerOrder: cot.Customer is null");
                return false;
            }

            //Has delivery information?
            if (cot.DeliveryMode != null
                && string.Equals(cot.DeliveryMode.Code, ApplicationSettings.Terminal.PickupDeliveryModeCode, StringComparison.OrdinalIgnoreCase))
            {
                //pick-all
                // For Pickup Order. it may or may not have the Store Information at order level.
                // Hence each sale line item must be verified for Store info.
                if (string.IsNullOrWhiteSpace(cot.WarehouseId))
                {
                    foreach (ISaleLineItem item in cot.SaleItems)
                    {
                        if ((item.DeliveryMode != null) && ((string.Equals(item.DeliveryMode.Code, ApplicationSettings.Terminal.PickupDeliveryModeCode, StringComparison.OrdinalIgnoreCase)
                            && string.IsNullOrWhiteSpace(item.DeliveryWarehouse))))
                        {
                            //pick-all
                            //no pickup store/warehouse
                            NetTracer.Information(string.Format("SalesOrder::ValidateCustomerOrder: no pickup store/warehouse for item :{0}", item.Description));
                            return false;
                        }
                    }
                }
            }
            else if (cot.DeliveryMode != null)
            {
                //ship-all
                if (string.IsNullOrWhiteSpace(cot.WarehouseId))
                {
                    //no shipping store/warehouse
                    NetTracer.Information("SalesOrder::ValidateCustomerOrder: no shipping store/warehouse");
                    return false;
                }

                if (cot.ShippingAddress == null || string.IsNullOrWhiteSpace(cot.ShippingAddress.AddressRecId))
                {
                    //No Shipping Address specified
                    NetTracer.Information("SalesOrder::ValidateCustomerOrder: no shipping address specified");
                    return false;
                }
            }
            else
            {
                //line level, so check each item's delivery settings
                foreach (ISaleLineItem line in cot.SaleItems)
                {
                    if (line.DeliveryMode != null
                        && string.Equals(line.DeliveryMode.Code, ApplicationSettings.Terminal.PickupDeliveryModeCode, StringComparison.OrdinalIgnoreCase))
                    {
                        //pick-up
                        // No additional validation required
                    }
                    else if (line.DeliveryMode != null)
                    {
                        //Ship
                        if (line.ShippingAddress == null || string.IsNullOrWhiteSpace(line.ShippingAddress.AddressRecId))
                        {
                            //No shipping address
                            NetTracer.Information("SalesOrder::ValidateCustomerOrder: no shipping address");
                            return false;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(line.DeliveryWarehouse))
                    {
                        //no pickup store/warehouse
                        NetTracer.Information("SalesOrder::ValidateCustomerOrder: no pickup store/warehouse");
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Validate the Customer Order after a tender operation.
        /// </summary>
        /// <param name="retailTransaction"></param>
        /// <returns></returns>
        public bool PerformPostTenderValidation(IRetailTransaction retailTransaction)
        {
            bool valid = true;
            CustomerOrderTransaction cot = retailTransaction as CustomerOrderTransaction;

            // We need a credit card token if the customer order has an item for shipping and there is an outstanding balance
            if ((cot != null) &&
                (cot.TransSalePmtDiff <= 0) &&                    // Have achieved (or exceeded) the minimum deposit amount
                (cot.NetAmountWithTaxAndCharges > cot.Payment) && // Order still has a balance due
                cot.LastRunOperationIsValidPayment &&             // User just performed a payment operation
                (cot.Mode == CustomerOrderMode.Convert            // Order is being created/edited (NOT required for pickup/cancel/return)
                    || cot.Mode == CustomerOrderMode.Create 
                    || cot.Mode == CustomerOrderMode.Edit))
            {	// Get the card information now that we have exceeded the minimum deposit amount (so we only ask once) and there is still payment pending 

                bool isDeliveryMode = false;

                // Check to see if delivery mode is set to shipping on the header
                if ((cot.DeliveryMode != null)
                    && !string.Equals(cot.DeliveryMode.Code, ApplicationSettings.Terminal.PickupDeliveryModeCode, StringComparison.OrdinalIgnoreCase))
                {
                    isDeliveryMode = true;
                }
                else
                {
                    // Delivery mode wasn't set on the header, check each line items delivery mode
                    foreach (SaleLineItem item in cot.SaleItems)
                    {
                        // Check to see if delivery mode is set to shipping for this line item
                        if ((item.DeliveryMode != null)
                            && !string.Equals(item.DeliveryMode.Code, ApplicationSettings.Terminal.PickupDeliveryModeCode, StringComparison.OrdinalIgnoreCase))
                        {
                            // Found this item has a delivery mode of shipping, don't need to check anymore break.
                            isDeliveryMode = true;
                            break;
                        }
                    }
                }

                if (isDeliveryMode
                    && (cot.OrderType != LSRetailPosis.Transaction.CustomerOrderType.Quote)
                    && (cot.Mode != CustomerOrderMode.Cancel))
                {
                    // Prompt: "Do you want to collect CC info for shipping at this time?"
                    DialogResult result = this.Application.Services.Dialog.ShowMessage(56138, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    // If 'YES', call EFT to get credit card token
                    if (result == DialogResult.Yes)
                    {
                        // Call EFT to get credit card token - NOTE: May throw exception
                        this.Application.Services.EFT.GenerateCardToken(CalculateShippingDue(cot), cot);
                        valid = !(string.IsNullOrEmpty(cot.CreditCardToken));
                    }
                }
            }
            return valid;
        }

        /// <summary>
        /// Calculates the amount due at the time of placing the order excluding items that are to be picked up and payment already made
        /// The amount includes the items to be shipped, the shipping charge and tax on both.
        /// </summary>
        /// <param name="cot">The transaction object containing the line items.</param>
        /// <returns>The amount due</returns>
        private static decimal CalculateShippingDue(CustomerOrderTransaction cot)
        {
            decimal totalDue = cot.NetAmountWithTaxAndCharges;

            string pickupCode = ApplicationSettings.Terminal.PickupDeliveryModeCode;

            foreach (SaleLineItem lineItem in cot.SaleItems)
            {
                if (!lineItem.Voided
                    && lineItem.DeliveryMode != null
                    && string.Equals(lineItem.DeliveryMode.Code, pickupCode, StringComparison.OrdinalIgnoreCase))
                {
                    totalDue -= lineItem.NetAmountWithTax;
                }
            }

            totalDue -= (cot.Payment + cot.TransSalePmtDiff);

            //Total due can be negative if payment made is more than for shipping items and their shipping charge
            totalDue = Math.Max(decimal.Zero, totalDue);

            return totalDue;
        }
        #endregion

        #region TS methods

        /// <summary>
        /// Save a Customer Order (either creating a new one, or updateing an existing one)
        /// </summary>
        /// <param name="retValue"></param>
        /// <param name="comment"></param>
        /// <param name="posTransaction"></param>
        /// <exception cref="InvalidOperationException">Thrown if the actual IRetailTransaction instance is not a supported type (CustomerOrderTranaction).</exception>
        public void SaveCustomerOrder(
            ref bool retValue,
            out string comment,
            IRetailTransaction posTransaction)
        {
            CustomerOrderTransaction order = posTransaction as CustomerOrderTransaction;

            if (order == null)
            {
                NetTracer.Warning("SalesOrder::SaveCustomerOrder: order is null");
                throw new InvalidOperationException("The instance of IRetailTransaction is not supported.  IRetailTransaction must be of type CustomerOrderTransaction.");
            }
            int successMessageId = 0;

            if (ValidateCustomerOrder(order))
            {

                switch (order.Mode)
                {
                    case CustomerOrderMode.Create:
                        // Create a new sales order
                        CreateCustomerOrder(ref retValue, out comment, order);
                        successMessageId = 56240;
                        break;

                    case CustomerOrderMode.Return:
                        // Create a new (return) sales order
                        CreateCustomerOrder(ref retValue, out comment, order);
                        successMessageId = 56243;
                        break;

                    case CustomerOrderMode.Convert:
                        // Convert the quote into an order
                        CreateCustomerOrderFromQuote(ref retValue, out comment, order);
                        successMessageId = 56240;
                        break;

                    case CustomerOrderMode.Pickup:
                        // Settle the order (including previous payments)
                        SettleCustomerOrder(ref retValue, out comment, order);
                        successMessageId = 56244;
                        break;

                    case CustomerOrderMode.Edit:
                        // Update an existing sales order
                        UpdateCustomerOrder(ref retValue, out comment, order);
                        successMessageId = 56240;
                        break;

                    case CustomerOrderMode.Cancel:
                        // Have AX cancel the order
                        CancelCustomerOrder(ref retValue, out comment, order);
                        successMessageId = 56242;
                        break;

                    default:
                        NetTracer.Error("Unsupported CustomerOrderTransaction Mode: {0}", order.Mode);
                        throw new ArgumentException();
                }
            }
            else
            {
                //(!)Required fields are empty.
                retValue = false;
                comment = ApplicationLocalizer.Language.Translate(56245);
                InternalApplication.Services.Dialog.ShowMessage(comment, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }


            if (retValue)
            {
                // Show the "success!" message for each operation
                InternalApplication.Services.Dialog.ShowMessage(successMessageId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="retValue"></param>
        /// <param name="comment"></param>
        /// <param name="salesId"></param>
        public void CreatePackingSlip(
            out bool retValue,
            out string comment,
            string salesId)
        {
            try
            {
                ReadOnlyCollection<object> containerArray = this.application.TransactionServices.Invoke("createPackingSlip", salesId);
                retValue = (bool)containerArray[1];
                comment = containerArray[2].ToString();
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        /// <summary>
        /// Create a picking list for the given order id
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="retValue"></param>
        /// <param name="comment"></param>
        internal static void CreatePickingList(string orderId, out bool retValue, out string comment)
        {
            try
            {
                ReadOnlyCollection<object> containerArray = SalesOrder.InternalApplication.TransactionServices.Invoke(
                    "createPickingList",
                    orderId,
                    ApplicationSettings.Terminal.InventLocationId);

                retValue = (bool)containerArray[1];
                comment = containerArray[2].ToString();
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(typeof(SalesOrder).ToString(), ex);
                throw;
            }
        }

        /// <summary>
        /// Create a Customer Sales Order in AX
        /// </summary>
        /// <param name="retValue"></param>
        /// <param name="comment"></param>
        /// <param name="posTransaction"></param>
        public void CreateCustomerOrderFromQuote(
            ref bool retValue,
            out string comment,
            IRetailTransaction posTransaction)
        {
            CustomerOrderTransaction customerOrder = posTransaction as CustomerOrderTransaction;
            if (customerOrder == null)
            {
                NetTracer.Warning("SalesOrder::CreateCustomerOrderFromQuote: customerOrder is null");
                throw new ArgumentNullException("CustomerOrderTransaction");
            }

            try
            {
                // Now attempt to update the newly converted Order with any changes performed (and deposit collected)
                //Build order info parameters and serialize
                CustomerOrderInfo parameters = SerializationHelper.GetInfoFromTransaction(customerOrder);
                string xmlString = parameters.ToXml();

                // Send XML doc to AX
                ReadOnlyCollection<object> containerArray;
                containerArray = Application.TransactionServices.Invoke("ConvertCustomerQuoteToOrder", xmlString);

                retValue = (bool)containerArray[1];
                comment = containerArray[2].ToString();

                if (retValue)
                {
                    // Only set the Id if we successfully created the order
                    customerOrder.OrderId = containerArray[3].ToString();
                }
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        /// <summary>
        /// Create a Customer Sales Order in AX
        /// </summary>
        /// <param name="retValue"></param>
        /// <param name="comment"></param>
        /// <param name="posTransaction"></param>
        public void CreateCustomerOrder(
            ref bool retValue,
            out string comment,
            IRetailTransaction posTransaction)
        {
            CustomerOrderTransaction customerOrder = posTransaction as CustomerOrderTransaction;
            if (customerOrder == null)
            {
                NetTracer.Warning("SalesOrder::CreateCustomerOrder: customerOrder is null");
                throw new ArgumentNullException("CustomerOrderTransaction");
            }

            try
            {
                //Build order info parameters
                CustomerOrderInfo parameters = SerializationHelper.GetInfoFromTransaction(customerOrder);

                // Serialize the Customer Order info into an XML doc
                string xmlString = parameters.ToXml();

                ApplicationLog.Log(
                    "SalesOrder.CreateCustomerOrder()",
                    string.Format("Customer Order XML: \n {0}", xmlString),
                     LogTraceLevel.Debug);

                // Send XML doc to AX
                ReadOnlyCollection<object> containerArray;
                switch (customerOrder.OrderType)
                {
                    case CustomerOrderType.Quote:
                        containerArray = Application.TransactionServices.Invoke("CreateCustomerQuote", xmlString);
                        break;
                    case CustomerOrderType.SalesOrder:
                        if (customerOrder.Mode == CustomerOrderMode.Return)
                        {
                            containerArray = Application.TransactionServices.Invoke("CreateCustomerReturnOrder", xmlString);
                        }
                        else
                        {
                            containerArray = Application.TransactionServices.Invoke("CreateCustomerOrder", xmlString);
                        }
                        break;
                    default:
                        NetTracer.Error("SalesOrder::CreateCustomerOrder: unsupported customerOrder.OrderType: {0}", customerOrder.OrderType);
                        throw new InvalidOperationException();
                }

                retValue = (bool)containerArray[1];
                comment = containerArray[2].ToString();

                if (retValue)
                {
                    // Only set the Id if we successfully created the order/quote
                    customerOrder.OrderId = containerArray[3].ToString();
                }
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        /// <summary>
        /// Update a customer order
        /// </summary>
        /// <param name="retValue"></param>
        /// <param name="comment"></param>
        /// <param name="posTransaction"></param>
        public void UpdateCustomerOrder(
            ref bool retValue,
            out string comment,
            IRetailTransaction posTransaction)
        {
            CustomerOrderTransaction customerOrder = posTransaction as CustomerOrderTransaction;
            if (customerOrder == null)
            {
                NetTracer.Warning("SalesOrder::UpdateCustomerOrder: customerOrder is null");
                throw new ArgumentNullException("CustomerOrderTransaction");
            }

            try
            {
                //Build order info parameters
                CustomerOrderInfo parameters = SerializationHelper.GetInfoFromTransaction(customerOrder);

                // Serialize the Customer Order info into an XML doc
                string xmlString = parameters.ToXml();

                ApplicationLog.Log(
                    "SalesOrder.UpdateCustomerOrder()",
                    string.Format("Customer Order XML: \n {0}", xmlString),
                     LogTraceLevel.Debug);

                // Send XML doc to AX
                ReadOnlyCollection<object> containerArray;
                switch (customerOrder.OrderType)
                {
                    case CustomerOrderType.Quote:
                        containerArray = Application.TransactionServices.Invoke("UpdateCustomerQuote", xmlString);
                        break;
                    case CustomerOrderType.SalesOrder:
                        containerArray = Application.TransactionServices.Invoke("UpdateCustomerOrder", xmlString);
                        break;
                    default:
                        NetTracer.Error("SalesOrder::UpdateCustomerOrder: unsupported customerOrder.OrderType {0}", customerOrder.OrderType);
                        throw new InvalidOperationException();
                }

                retValue = (bool)containerArray[1];
                comment = containerArray[2].ToString();

                if (retValue)
                {
                    // Only set the Id if we successfully created the order/quote
                    customerOrder.OrderId = containerArray[3].ToString();
                }
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        /// <summary>
        /// Send the Cancellation request to AX
        /// </summary>
        /// <param name="retValue"></param>
        /// <param name="comment"></param>
        /// <param name="posTransaction"></param>
        public void CancelCustomerOrder(
            ref bool retValue,
            out string comment,
            IRetailTransaction posTransaction)
        {
            CustomerOrderTransaction customerOrder = posTransaction as CustomerOrderTransaction;
            if (customerOrder == null)
            {
                NetTracer.Warning("SalesOrder::CancelCustomerOrder: customerOrder is null");
                throw new ArgumentNullException("CustomerOrderTransaction");
            }

            try
            {
                //Build order info parameters
                CustomerOrderInfo parameters = SerializationHelper.GetInfoFromTransaction(customerOrder);

                //Keep only the cancellation charges
                parameters.Charges = new Collection<ChargeInfo>(parameters.Charges.Where(
                    c => c.Code.Equals(ApplicationSettings.Terminal.CancellationChargeCode, StringComparison.OrdinalIgnoreCase)).ToList());

                // Serialize the Customer Order info into an XML doc
                string xmlString = parameters.ToXml();

                ApplicationLog.Log(
                    "SalesOrder.CancelCustomerOrder()",
                    string.Format("Customer Order XML: \n {0}", xmlString),
                     LogTraceLevel.Debug);

                // Send XML doc to AX
                ReadOnlyCollection<object> containerArray;
                containerArray = Application.TransactionServices.Invoke("CancelCustomerOrder", xmlString);

                retValue = (bool)containerArray[1];
                comment = containerArray[2].ToString();
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        /// <summary>
        /// Settle/Pickup a customer order
        /// </summary>
        /// <param name="retValue"></param>
        /// <param name="comment"></param>
        /// <param name="posTransaction"></param>
        public void SettleCustomerOrder(
            ref bool retValue,
            out string comment,
            IRetailTransaction posTransaction)
        {
            CustomerOrderTransaction customerOrder = posTransaction as CustomerOrderTransaction;
            if (customerOrder == null)
            {
                NetTracer.Warning("SalesOrder::SettleCustomerOrder: customerOrder is null");
                throw new ArgumentNullException("CustomerOrderTransaction");
            }

            try
            {
                //Build order info parameters
                CustomerOrderInfo parameters = SerializationHelper.GetInfoFromTransaction(customerOrder);

                // Serialize the Customer Order info into an XML doc
                string xmlString = parameters.ToXml();

                ApplicationLog.Log(
                    "SalesOrder.SettleCustomerOrder()",
                    string.Format("Customer Order XML: \n {0}", xmlString),
                     LogTraceLevel.Debug);

                // Send XML doc to AX
                ReadOnlyCollection<object> containerArray;
                switch (customerOrder.OrderType)
                {
                    case CustomerOrderType.SalesOrder:
                        containerArray = Application.TransactionServices.Invoke("SettleCustomerOrder", xmlString);
                        break;

                    case CustomerOrderType.Quote:
                    default:
                        NetTracer.Error("SalesOrder::SettleCustomerOrder: unsupported customerOrder.OrderType {0}", customerOrder.OrderType);
                        throw new InvalidOperationException();
                }

                retValue = (bool)containerArray[1];
                comment = containerArray[2].ToString();

                if (retValue)
                {
                    // Only set the Id if we successfully created the order/quote
                    //customerOrder.OrderId = containerArray[3].ToString();
                }
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        /// <summary>
        /// Get a customer order
        /// </summary>
        /// <param name="retValue"></param>
        /// <param name="orderId"></param>
        /// <param name="comment"></param>
        /// <param name="posTransaction"></param>
        public void GetCustomerOrder(
            ref bool retValue,
            string orderId,
            out string comment,
            out IRetailTransaction posTransaction)
        {
            try
            {
                posTransaction = null;
                ReadOnlyCollection<object> containerArray;

                // Begin by checking if there is a connection to the Transaction Service
                if (!SalesOrder.InternalApplication.TransactionServices.CheckConnection())
                {
                    retValue = false;
                    comment = string.Empty;
                    return;
                }

                // Send request to AX
                containerArray = InternalApplication.TransactionServices.Invoke("GetCustomerOrder", orderId, true);

                retValue = (bool)containerArray[1];
                comment = containerArray[2].ToString();

                if (retValue)
                {
                    // Only set the Id if we successfully created the order/quote
                    string xmlString = containerArray[3].ToString();

                    CustomerOrderInfo orderInfo = CustomerOrderInfo.FromXml(xmlString);
                    CustomerOrderTransaction customerOrder = SerializationHelper.GetTransactionFromInfo(orderInfo, this);

                    if (string.IsNullOrEmpty(customerOrder.OrderId))
                    {
                        customerOrder.OrderId = orderId;
                    }
                    customerOrder.OrderType = CustomerOrderType.SalesOrder;
                    posTransaction = customerOrder;
                }
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        /// <summary>
        /// Get a customer Quote
        /// </summary>
        /// <param name="retValue"></param>
        /// <param name="orderId"></param>
        /// <param name="comment"></param>
        /// <param name="posTransaction"></param>
        public void GetCustomerQuote(
            ref bool retValue,
            string quoteId,
            out string comment,
            out IRetailTransaction posTransaction)
        {
            try
            {
                posTransaction = null;
                ReadOnlyCollection<object> containerArray;

                // Begin by checking if there is a connection to the Transaction Service
                if (!SalesOrder.InternalApplication.TransactionServices.CheckConnection())
                {
                    retValue = false;
                    comment = string.Empty;
                    return;
                }

                // Send request to AX
                containerArray = InternalApplication.TransactionServices.Invoke("GetCustomerQuote", quoteId);

                retValue = (bool)containerArray[1];
                comment = containerArray[2].ToString();

                if (retValue)
                {
                    // Only set the Id if we successfully created the order/quote
                    string xmlString = containerArray[3].ToString();

                    CustomerOrderInfo orderInfo = CustomerOrderInfo.FromXml(xmlString);
                    CustomerOrderTransaction customerOrder = SerializationHelper.GetTransactionFromInfo(orderInfo, this);
                    customerOrder.OrderType = CustomerOrderType.Quote;
                    posTransaction = customerOrder;
                }
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        /// <summary>
        /// Get packing slip
        /// </summary>
        /// <param name="retValue"></param>
        /// <param name="orderId"></param>
        /// <param name="comment"></param>
        /// <param name="posTransaction"></param>
        public void GetPackingSlip(
            ref bool retValue,
            string orderId,
            out string comment,
            out IRetailTransaction posTransaction)
        {
            try
            {
                posTransaction = null;
                ReadOnlyCollection<object> containerArray;

                // Send request to AX
                containerArray = InternalApplication.TransactionServices.Invoke("GetPackingSlip", orderId);

                retValue = (bool)containerArray[1];
                comment = containerArray[2].ToString();

                if (retValue)
                {
                    // Only set the Id if we successfully created the order/quote
                    string xmlString = containerArray[3].ToString();

                    CustomerOrderInfo orderInfo = CustomerOrderInfo.FromXml(xmlString);
                    ICustomerOrderTransaction customerOrder = SerializationHelper.GetTransactionFromInfo(orderInfo, this);
                    posTransaction = customerOrder;
                }
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        /// <summary>
        /// Get list of customer orders for pack-slip creation
        /// </summary>
        /// <param name="retValue"></param>
        /// <param name="comment"></param>
        /// <param name="salesOrders"></param>
        /// <param name="customerAccount">If blank, retrieves list for ALL customers</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Argument is sufficiently validated")]
        public void GetCustomerOrdersForPackSlip(
            out bool retValue,
            out string comment,
            ref DataTable salesOrders,
            string customerAccount)
        {
            ReadOnlyCollection<object> containerArray;
            containerArray = application.TransactionServices.Invoke("GetCustomerOrdersForPackSlip", customerAccount);

            retValue = (bool)containerArray[1];
            comment = containerArray[2].ToString();

            salesOrders = new DataTable();
            salesOrders.Columns.Add("SALESID", typeof(string));
            salesOrders.Columns.Add("PREPAYAMOUNT", typeof(decimal));
            salesOrders.Columns.Add("PREPAIDAMOUNT", typeof(decimal));
            salesOrders.Columns.Add("TOTALAMOUNT", typeof(decimal));
            salesOrders.Columns.Add("CUSTOMERACCOUNT", typeof(string));
            salesOrders.Columns.Add("CUSTOMERNAME", typeof(string));
            salesOrders.Columns.Add("DATE", typeof(DateTime));
            salesOrders.Columns.Add("ORDERTYPE", typeof(string));
            salesOrders.Columns.Add("STATUS", typeof(string));
            salesOrders.Columns.Add("DELIVERYMODE", typeof(string));

            for (int i = 3; i < containerArray.Count; i++)
            {
                IList salesRecord = (IList)containerArray[i];

                bool recordRetVal = SerializationHelper.ConvertToBooleanAtIndex(salesRecord, 0);
                string recordComment = SerializationHelper.ConvertToStringAtIndex(salesRecord, 1);

                // The particular sales order at this position in the container is blank so we need
                // to jump over it process the next one...
                if (!recordRetVal)
                    continue;
                if (salesOrders == null)
                {
                    NetTracer.Warning("SalesOrder::GetCustomerOrdersForPackSlip: salesOrders is null");
                    throw new NullReferenceException("salesOrders");
                }
                DataRow row = salesOrders.NewRow();

                // some of these fields may not be properly initialized even if recordRetVal is true
                row["SALESID"] = SerializationHelper.ConvertToStringAtIndex(salesRecord, 2);
                row["PREPAYAMOUNT"] = SerializationHelper.ConvertToDecimalAtIndex(salesRecord, 3);
                row["PREPAIDAMOUNT"] = SerializationHelper.ConvertToDecimalAtIndex(salesRecord, 4);
                row["TOTALAMOUNT"] = SerializationHelper.ConvertToDecimalAtIndex(salesRecord, 5);
                row["CUSTOMERACCOUNT"] = SerializationHelper.ConvertToStringAtIndex(salesRecord, 6);
                row["CUSTOMERNAME"] = SerializationHelper.ConvertToStringAtIndex(salesRecord, 7);
                row["DATE"] = SerializationHelper.ConvertToDateTimeAtIndex(salesRecord, 8);
                row["ORDERTYPE"] = SerializationHelper.ConvertToStringAtIndex(salesRecord, 9);
                row["STATUS"] = SerializationHelper.ConvertToStringAtIndex(salesRecord, 10);
                row["DELIVERYMODE"] = SerializationHelper.ConvertToStringAtIndex(salesRecord, 13);

                if (SerializationHelper.ConvertToBooleanAtIndex(salesRecord, 12))
                {
                    row["ORDERTYPE"] = CustomerOrderType.SalesOrder;
                    row["STATUS"] = SerializationHelper.GetSalesStatus((SalesOrderStatus)salesRecord[14], (DocumentStatus)salesRecord[11]);
                }
                else
                {
                    row["ORDERTYPE"] = CustomerOrderType.Quote;
                    row["STATUS"] = SerializationHelper.GetSalesStatus((SalesQuotationStatus)salesRecord[11]);
                }

                salesOrders.Rows.Add(row);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="retValue"></param>
        /// <param name="comment"></param>
        /// <param name="salesOrders"></param>
        /// <param name="customerAccount"></param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller's will dispose of datatable")]
        internal static DataTable GetCustomerOrdersList(ref bool retValue, ref string comment, string customerSearchTerm, string orderSearchTerm, DateTime? startDate, DateTime? endDate, int? resultMaxCount)
        {
            if (comment == null)
            {
                NetTracer.Warning("comment parameter is null");
                throw new ArgumentNullException("comment");
            }
            if (customerSearchTerm == null)
            {
                NetTracer.Warning("customerSearchTerm parameter is null");
                throw new ArgumentNullException("customerSearchTerm");
            }
            if (orderSearchTerm == null)
            {
                NetTracer.Warning("orderSearchTerm parameter is null");
                throw new ArgumentNullException("orderSearchTerm");
            }

            DataTable salesOrders = new DataTable();

            // Begin by checking if there is a connection to the Transaction Service
            if (!SalesOrder.InternalApplication.TransactionServices.CheckConnection())
            {
                retValue = false;
                comment = string.Empty;
                return salesOrders;
            }

            ReadOnlyCollection<object> containerArray;
            string fromDateString = SerializationHelper.GetDateString(startDate);
            string toDateString = SerializationHelper.GetDateString(endDate);
            containerArray = SalesOrder.InternalApplication.TransactionServices.Invoke("SearchCustomerOrderList", customerSearchTerm, orderSearchTerm, fromDateString, toDateString, resultMaxCount);

            retValue = SerializationHelper.ConvertToBooleanAtIndex(containerArray, 1);
            comment = SerializationHelper.ConvertToStringAtIndex(containerArray, 2);

            salesOrders.Columns.Add("SALESID", typeof(string));
            salesOrders.Columns.Add("PREPAYAMOUNT", typeof(decimal));
            salesOrders.Columns.Add("PREPAIDAMOUNT", typeof(decimal));
            salesOrders.Columns.Add("TOTALAMOUNT", typeof(decimal));
            salesOrders.Columns.Add("CUSTOMERACCOUNT", typeof(string));
            salesOrders.Columns.Add("CUSTOMERNAME", typeof(string));
            salesOrders.Columns.Add("DATE", typeof(DateTime));
            salesOrders.Columns.Add("ORDERTYPE", typeof(int));   //SalesOrder | Quote
            salesOrders.Columns.Add("STATUS", typeof(int));         //generic status
            salesOrders.Columns.Add("DELIVERYMODE", typeof(string));
            salesOrders.Columns.Add("CHANNELREFERENCEID", typeof(string));
            salesOrders.Columns.Add("EMAIL", typeof(string));

            for (int i = 3; i < containerArray.Count; i++)
            {
                IList salesRecord = (IList)containerArray[i];

                bool recordRetVal = SerializationHelper.ConvertToBooleanAtIndex(salesRecord, 0);
                string recordComment = SerializationHelper.ConvertToStringAtIndex(salesRecord, 1);

                // The particular sales order at this position in the container is blank so we need
                // to jump over it process the next one...
                if (!recordRetVal)
                    continue;

                DataRow row = salesOrders.NewRow();

                // some of these fields may not be properly initialized even if recordRetVal is true
                row["SALESID"] = SerializationHelper.ConvertToStringAtIndex(salesRecord, 2);
                row["PREPAYAMOUNT"] = SerializationHelper.ConvertToDecimalAtIndex(salesRecord, 3);
                row["PREPAIDAMOUNT"] = SerializationHelper.ConvertToDecimalAtIndex(salesRecord, 4);
                row["TOTALAMOUNT"] = SerializationHelper.ConvertToDecimalAtIndex(salesRecord, 5);
                row["CUSTOMERACCOUNT"] = SerializationHelper.ConvertToStringAtIndex(salesRecord, 6);

                row["CUSTOMERNAME"] = string.Format("{0} ({1})",
                    SerializationHelper.ConvertToStringAtIndex(salesRecord, 7),
                    SerializationHelper.ConvertToStringAtIndex(salesRecord, 6));

                row["DATE"] = SerializationHelper.ConvertToDateTimeAtIndex(salesRecord, 8);

                if (SerializationHelper.ConvertToBooleanAtIndex(salesRecord, 12))
                {
                    row["ORDERTYPE"] = CustomerOrderType.SalesOrder;
                    row["STATUS"] = SerializationHelper.GetSalesStatus((SalesOrderStatus)salesRecord[14], (DocumentStatus)salesRecord[11]);
                }
                else
                {
                    row["ORDERTYPE"] = CustomerOrderType.Quote;
                    row["STATUS"] = SerializationHelper.GetSalesStatus((SalesQuotationStatus)salesRecord[11]);
                }

                if (salesRecord.Count > 13)
                {
                    row["DELIVERYMODE"] = SerializationHelper.ConvertToStringAtIndex(salesRecord, 13);
                }

                if (salesRecord.Count > 20)
                {
                    row["CHANNELREFERENCEID"] = SerializationHelper.ConvertToStringAtIndex(salesRecord, 20);
                }

                if (salesRecord.Count > 22)
                {
                    row["EMAIL"] = SerializationHelper.ConvertToStringAtIndex(salesRecord, 22);
                }

                salesOrders.Rows.Add(row);
            }
            return salesOrders;
        }

        /// <summary>
        /// Retreive a list of invoices for a given sales order
        /// </summary>
        /// <param name="retValue"></param>
        /// <param name="comment"></param>
        /// <param name="salesOrders"></param>
        /// <param name="salesOrderId"></param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller's will dispose of datatable")]
        internal static DataTable GetSalesInvoiceList(ref bool retValue, ref string comment, string salesOrderId)
        {
            if (comment == null)
            {
                NetTracer.Warning("comment parameter is null");
                throw new ArgumentNullException("comment");
            }
            if (salesOrderId == null)
            {
                NetTracer.Warning("salesOrderId parameter is null");
                throw new ArgumentNullException("customerAccount");
            }

            DataTable invoices = new DataTable();
            invoices.Columns.Add("RECID", typeof(long));
            invoices.Columns.Add("INVOICEID", typeof(string));
            invoices.Columns.Add("SALESID", typeof(string));
            invoices.Columns.Add("SALESTYPE", typeof(int));
            invoices.Columns.Add("INVOICEDATE", typeof(DateTime));
            invoices.Columns.Add("CURRENCYCODE", typeof(string));
            invoices.Columns.Add("INVOICEAMOUNT", typeof(decimal));
            invoices.Columns.Add("INVOICEACCOUNT", typeof(string));
            invoices.Columns.Add("INVOICINGNAME", typeof(string));

            // Begin by checking if there is a connection to the Transaction Service
            if (!SalesOrder.InternalApplication.TransactionServices.CheckConnection())
            {
                retValue = false;
                comment = string.Empty;
                return invoices;
            }

            ReadOnlyCollection<object> containerArray;
            containerArray = SalesOrder.InternalApplication.TransactionServices.Invoke("GetSalesInvoicesBySalesId", salesOrderId);

            retValue = SerializationHelper.ConvertToBooleanAtIndex(containerArray, 1);
            comment = SerializationHelper.ConvertToStringAtIndex(containerArray, 2);
            string xml = SerializationHelper.ConvertToStringAtIndex(containerArray, 3);

            if (retValue)
            {
                InvoiceJournalList list = InvoiceJournalList.FromXml(xml);

                foreach (InvoiceJournal j in list.Invoices)
                {
                    DataRow row = invoices.NewRow();
                    row["RECID"] = j.RecId;
                    row["INVOICEID"] = j.InvoiceId;
                    row["SALESID"] = j.SalesId;
                    row["INVOICEDATE"] = j.InvoiceDate;
                    row["CURRENCYCODE"] = j.CurrencyCode;
                    row["INVOICEAMOUNT"] = Convert.ToDecimal(j.InvoiceAmount, CultureInfo.InvariantCulture);
                    row["INVOICEACCOUNT"] = j.InvoiceAccount;
                    row["INVOICINGNAME"] = j.InvoicingName;
                    invoices.Rows.Add(row);
                }
            }
            else
            {
                LSRetailPosis.ApplicationLog.Log("SalesOrder.GetSalesInvoiceList()",
                    string.Format("{0}\n{1}", LSRetailPosis.ApplicationLocalizer.Language.Translate(1002), comment),
                    LogTraceLevel.Trace);
            }
            return invoices;
        }

        /// <summary>
        /// Get a transaction representing a given invoice
        /// </summary>
        /// <param name="retValue"></param>
        /// <param name="comment"></param>
        /// <param name="invoiceId"></param>
        /// <param name="invoiceTransaction"></param>
        internal static void GetSalesInvoiceTransaction(ref bool retValue, ref string comment, string invoiceId, out CustomerOrderTransaction invoiceTransaction)
        {
            invoiceTransaction = null;

            try
            {
                invoiceTransaction = null;
                ReadOnlyCollection<object> containerArray;

                // Send request to AX
                containerArray = SalesOrder.InternalApplication.TransactionServices.Invoke("GetSalesInvoiceDetail", invoiceId);

                retValue = SerializationHelper.ConvertToBooleanAtIndex(containerArray, 1);
                comment = containerArray[2].ToString();
                string xmlString = SerializationHelper.ConvertToStringAtIndex(containerArray, 3);

                if (retValue)
                {
                    InvoiceJournal invoice = InvoiceJournal.FromXml(xmlString);
                    CustomerOrderTransaction customerOrder = SalesOrderActions.GetTransactionFromInvoice(invoice);
                    invoiceTransaction = customerOrder;
                }
                else
                {
                    LSRetailPosis.ApplicationLog.Log("SalesOrder.GetSalesInvoiceTransaction()",
                        string.Format("{0}\n{1}", LSRetailPosis.ApplicationLocalizer.Language.Translate(1002), comment),
                        LogTraceLevel.Trace);
                }
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(typeof(SalesOrderActions).ToString(), ex);
                throw;
            }
        }

        /// <summary>
        /// Get a list of reason codes.
        /// </summary>
        /// <returns>List of reason codes.</returns>
        internal static IList<ReturnReasonCode> GetReturnReasonCodes()
        {
            bool retValue;
            string comment;
            ReadOnlyCollection<object> containerArray;
            string xmlString;

            if (returnReasonCodes == null)
            {
                try
                {
                    // Send request to AX
                    containerArray = SalesOrder.InternalApplication.TransactionServices.Invoke("GetReturnReasonCodes");

                    retValue = SerializationHelper.ConvertToBooleanAtIndex(containerArray, 1);
                    comment = containerArray[2].ToString();
                    xmlString = SerializationHelper.ConvertToStringAtIndex(containerArray, 3);

                    if (retValue)
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<ReturnReasonCode>));
                        using (StringReader reader = new StringReader(xmlString))
                        {
                            returnReasonCodes = serializer.Deserialize(reader) as List<ReturnReasonCode>;
                        }
                    }
                    else
                    {
                        LSRetailPosis.ApplicationLog.Log("SalesOrder.GetReturnReasonCodes()",
                            string.Format("{0}\n{1}", LSRetailPosis.ApplicationLocalizer.Language.Translate(1002), comment),
                            LogTraceLevel.Trace);
                    }
                }
                catch (Exception ex)
                {
                    LSRetailPosis.ApplicationExceptionHandler.HandleException(typeof(SalesOrderActions).ToString(), ex);
                    throw;
                }
            }

            return returnReasonCodes ?? new List<ReturnReasonCode>();
        }

        internal static DE.ICustomer GetCustomerFromAX(
            string accountNumber,
            ICustomerSystem customerSystem,
            DM.CustomerDataManager customerDataManager)
        {
            ReadOnlyCollection<object> containerArray;
            containerArray = SalesOrder.InternalApplication.TransactionServices.Invoke("GetCustomer", accountNumber);

            bool retValue = SerializationHelper.ConvertToBooleanAtIndex(containerArray, 1);
            string comment = containerArray[2].ToString();

            if (retValue)
            {
                string xml = containerArray[3].ToString();

                GetCustomerResponse response = GetCustomerResponse.FromXml(xml);

                if (response != null && response.CustomerInfo != null)
                {
                    customerDataManager.SaveCustomers(new DataEntity.Customer[] { GetCustomer(response.CustomerInfo) });

                    customerDataManager.SaveParties(new Party[] { GetParty(response.DirPartyInfo) });

                    if (response.RetailCustomerInfo.RecordId != 0)
                    {
                        customerDataManager.SaveRetailCustomers(new RetailCustomer[] { GetRetailCustomer(response.RetailCustomerInfo) });
                    }

                    List<Location> locations = GetLocationList(response);
                    customerDataManager.SaveLocations(locations);

                    List<LocationRole> locationRoles = GetLocationRoleList(response);
                    customerDataManager.SaveLocationRoles(locationRoles);

                    List<PartyLocation> partyLocations = GetPartyLocationList(response);
                    customerDataManager.SavePartyLocations(partyLocations);

                    List<PartyLocationRole> partyLocationRoles = GetPartyLocationRoleList(response);
                    customerDataManager.SavePartyLocationRoles(partyLocationRoles);

                    List<DataEntity.PostalAddress> postalAddresses = GetPostalAddressList(response);
                    customerDataManager.SavePostalAddresses(postalAddresses);

                    List<DataEntity.ElectronicAddress> electronicAddresses = GetElectronicAddressList(response);
                    customerDataManager.SaveElectronicAddresses(electronicAddresses);

                    // this will reconstruct the customer.
                    return customerSystem.GetCustomerInfo(accountNumber);
                }
            }

            return null;
        }

        private static DataEntity.Customer GetCustomer(CustTableRow custTableRow)
        {
            return new DataEntity.Customer()
            {
                Id = custTableRow.RecordId,
                DataAreaId = SalesOrder.InternalApplication.Settings.Database.DataAreaID,
                AccountNumber = custTableRow.AccountNumber,
                PartyId = custTableRow.Party,
                CustomerGroup = custTableRow.CustomerGroup,
                InvoiceAccount = custTableRow.InvoiceAccount,
                SearchName = custTableRow.NameAlias,
                Currency = custTableRow.Currency,
                Language = custTableRow.LanguageId,
                MultipleLineDiscountGroup = custTableRow.MultilineDiscount,
                TotalDiscountGroup = custTableRow.EndDiscount,
                LineDiscountGroup = custTableRow.LineDiscount,
                PriceGroup = custTableRow.PriceGroup,
                CreditLimit = custTableRow.CreditMax,
                Blocked = custTableRow.Blocked,
                SalesTaxGroup = custTableRow.TaxGroup,
                VATNumber = custTableRow.VATNumber,
                OrgId = custTableRow.OrgId,
                UsePurchaseRequest = custTableRow.UsePurchaseRequest,
                PricesIncludeSalesTax = custTableRow.IncludeTax,
                IdentificationNumber = custTableRow.IdentificationNumber,
                CNPJCPFNumber = custTableRow.CNPJCPFNumber,
            };
        }

        private static Party GetParty(DirPartyTableRow dirPartyTableRow)
        {
            return new Party()
            {
                Id = dirPartyTableRow.RecordId,
                Name = dirPartyTableRow.Name,
                Number = dirPartyTableRow.PartyNumber,
                RelationType = dirPartyTableRow.RelationType,
            };
        }

        private static RetailCustomer GetRetailCustomer(RetailCustTableRow retailCustTableRow)
        {
            return new RetailCustomer()
            {
                AccountNumber = retailCustTableRow.AccountNumber,
                DataAreaId = SalesOrder.InternalApplication.Settings.Database.DataAreaID,
                Id = retailCustTableRow.RecordId,
                NonChargeableAccount = retailCustTableRow.NonChargeableAccount,
                OtherTenderInFinalizing = retailCustTableRow.OtherTenderInFinalizing,
                PostAsShipping = retailCustTableRow.PostAsShipment,
                ReceiptEmail = retailCustTableRow.ReceiptEmail,
                ReceiptOption = retailCustTableRow.ReceiptOption,
                RequiresApproval = retailCustTableRow.RequiresApproval,
                UseOrderNumberReference = retailCustTableRow.UseOrderNumberReference,
            };
        }

        private static List<Location> GetLocationList(GetCustomerResponse response)
        {
            List<Location> locations = new List<Location>();

            locations.AddRange(response.PostalAddresses.Select(
                p => GetLocation(p.LogisticsLocation)));

            locations.AddRange(response.ElectronicAddresses.Select(
                e => GetLocation(e.LogisticsLocation)));

            return locations;
        }

        private static Location GetLocation(LogisticsLocationRow logisticsLocationRow)
        {
            return new Location()
            {
                Id = logisticsLocationRow.RecordId,
                Description = logisticsLocationRow.Description,
                IsPostalAddress = logisticsLocationRow.IsPostalAddress,
                ParentLocationId = logisticsLocationRow.ParentLocation,
                Name = logisticsLocationRow.LocationId,
            };
        }

        private static List<LocationRole> GetLocationRoleList(GetCustomerResponse response)
        {
            List<LocationRole> locationRoles = new List<LocationRole>();

            locationRoles.AddRange(response.PostalAddresses.Select(
                p => GetLocationRole(p.LogisticsLocationRole)));

            return locationRoles;
        }

        private static LocationRole GetLocationRole(LogisticsLocationRoleRow logisticsLocationRoleRow)
        {
            return new LocationRole()
            {
                Id = logisticsLocationRoleRow.RecordId,
                IsContactInfo = logisticsLocationRoleRow.IsContactInfo,
                IsPostalAddress = logisticsLocationRoleRow.IsPostalAddress,
                Name = logisticsLocationRoleRow.Name,
                RoleType = logisticsLocationRoleRow.Type,
            };
        }

        private static List<PartyLocation> GetPartyLocationList(GetCustomerResponse response)
        {
            List<PartyLocation> partyLocations = new List<PartyLocation>();

            partyLocations.AddRange(response.PostalAddresses.Select(
                p => GetPartyLocation(p.DirPartyLocation)));

            partyLocations.AddRange(response.ElectronicAddresses.Select(
                e => GetPartyLocation(e.DirPartyLocation)));

            return partyLocations;
        }

        private static PartyLocation GetPartyLocation(CustomerOrderParameters.DirPartyLocationRow dirPartyLocation)
        {
            return new PartyLocation()
            {
                Id = dirPartyLocation.RecordId,
                IsLocationOwner = dirPartyLocation.IsLocationOwner,
                IsPostalAddress = dirPartyLocation.IsPostalAddress,
                IsPrimary = dirPartyLocation.IsPrimary,
                IsPrivate = dirPartyLocation.IsPrivate,
                LocationId = dirPartyLocation.Location,
                PartyId = dirPartyLocation.Party,
            };
        }

        private static List<PartyLocationRole> GetPartyLocationRoleList(GetCustomerResponse response)
        {
            List<PartyLocationRole> partyLocationsRoles = new List<PartyLocationRole>();

            partyLocationsRoles.AddRange(response.PostalAddresses.Select(
                p => new PartyLocationRole()
                {
                    Id = p.DirPartyLocationRole.RecordId,
                    LocationRoleId = p.DirPartyLocationRole.LocationRole,
                    PartyLocationId = p.DirPartyLocationRole.PartyLocation,
                }));

            return partyLocationsRoles;
        }

        private static List<DataEntity.PostalAddress> GetPostalAddressList(GetCustomerResponse response)
        {
            List<DataEntity.PostalAddress> postalAddresses = new List<DataEntity.PostalAddress>();

            postalAddresses.AddRange(response.PostalAddresses.Select(
                p => GetPostalAddress(p.LogisticsPostalAddress)));

            return postalAddresses;
        }

        private static DataEntity.PostalAddress GetPostalAddress(LogisticsPostalAddressRow logisticsPostalAddressRow)
        {
            return new DataEntity.PostalAddress()
            {
                Address = logisticsPostalAddressRow.Address,
                BuildingCompliment = logisticsPostalAddressRow.BuildingCompliment,
                City = logisticsPostalAddressRow.City,
                CountryRegion = logisticsPostalAddressRow.CountryRegionId,
                County = logisticsPostalAddressRow.County,
                DistrictName = logisticsPostalAddressRow.DistrictName,
                Id = logisticsPostalAddressRow.RecordId,
                LocationId = logisticsPostalAddressRow.Location,
                State = logisticsPostalAddressRow.State,
                Street = logisticsPostalAddressRow.Street,
                StreetNumber = logisticsPostalAddressRow.StreetNumber,
                TimeZone = logisticsPostalAddressRow.TimeZone,
                ValidFrom = SerializationHelper.ParseDateString(logisticsPostalAddressRow.ValidFrom, DateTime.UtcNow),
                ValidTo = SerializationHelper.ParseDateString(logisticsPostalAddressRow.ValidTo, DateTime.UtcNow),
                ZipCode = logisticsPostalAddressRow.ZipCode,
            };
        }

        private static List<DataEntity.ElectronicAddress> GetElectronicAddressList(GetCustomerResponse response)
        {
            List<DataEntity.ElectronicAddress> electronicAddresses = new List<DataEntity.ElectronicAddress>();

            electronicAddresses.AddRange(response.ElectronicAddresses.Select(
                e => GetElectronicAddress(e.LogisticsElectronicAddress)));

            return electronicAddresses;
        }

        private static DataEntity.ElectronicAddress GetElectronicAddress(LogisticsElectronicAddressRow logisticsElectronicAddressRow)
        {
            return new DataEntity.ElectronicAddress()
            {
                CountryRegionCode = logisticsElectronicAddressRow.CountryRegionCode,
                Id = logisticsElectronicAddressRow.RecordId,
                LocationId = logisticsElectronicAddressRow.Location,
                Locator = logisticsElectronicAddressRow.Locator,
                MethodType = logisticsElectronicAddressRow.Type,
            };
        }

        #endregion

        /// <summary>
        /// Log a message to the file.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="traceLevel"></param>
        /// <param name="args"></param>
        internal static void LogMessage(string message, LogTraceLevel traceLevel, params object[] args)
        {
            ApplicationLog.Log(typeof(SalesOrder).Name, string.Format(message, args), traceLevel);
        }
    }
}
