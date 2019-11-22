/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.SaleItem;
using Microsoft.Dynamics.Retail.Diagnostics;
using Microsoft.Dynamics.Retail.Pos.Contracts.BusinessLogic;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.DataEntity;
using Microsoft.Dynamics.Retail.Pos.SalesOrder.CustomerOrderParameters;
using CustomerOrderType = LSRetailPosis.Transaction.CustomerOrderType;
using DE = Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using DM = Microsoft.Dynamics.Retail.Pos.DataManager;
using SalesStatus = LSRetailPosis.Transaction.SalesStatus;
using Tax = LSRetailPosis.Transaction.Line.Tax;

namespace Microsoft.Dynamics.Retail.Pos.SalesOrder
{
    internal static class SerializationHelper
    {
        // Date time format based on ISO8601 standard
        // AX DateTimeUtl::ToStr(aDateTime) method also outputs the same format
        // e.g. 2006-07-14T19:05:49
        //
        private const string FixedDateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
        private const string FixedDateFormat = "yyyy-MM-dd";
        //Default date value of AX if the DateTime field is non-nullable
        private static readonly DateTime NoDate = new DateTime(1900 , 1 , 1 );

        /// <summary>
        /// Get UTC string version of the date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        internal static string GetDateString(DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToString(FixedDateTimeFormat);
            }
            return string.Empty;
        }

        /// <summary>
        /// Build a new OrderInfo object from a CustomerOrderTransaction
        /// </summary>
        /// <param name="customerOrder"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Needs refactoring")]
        internal static CustomerOrderInfo GetInfoFromTransaction(CustomerOrderTransaction customerOrder)
        {
            CustomerOrderInfo parameters = new CustomerOrderInfo();
            parameters.OrderType = customerOrder.OrderType;
            parameters.Id = customerOrder.OrderId;
            parameters.QuotationId = customerOrder.OrderId;
            parameters.AutoPickOrder = false;
            parameters.WarehouseId = customerOrder.WarehouseId;
            parameters.CurrencyCode = customerOrder.StoreCurrencyCode;
            parameters.StoreId = customerOrder.StoreId;
            parameters.TerminalId = customerOrder.TerminalId;
            parameters.LocalHourOfDay = customerOrder.LocalHourOfDay;

            parameters.AddressRecordId = (customerOrder.ShippingAddress != null) ? customerOrder.ShippingAddress.AddressRecId : string.Empty;
            parameters.CustomerAccount = (customerOrder.Customer != null) ? customerOrder.Customer.CustomerId : string.Empty;
            parameters.SalespersonStaffId = (customerOrder.SalesPersonId) ?? string.Empty;

            // The format must match the expected format in AX RetailTransactionService.CreateCustomerOrder: "dd/MM/yyyy"
            parameters.ExpiryDateString = customerOrder.ExpirationDate.ToString(FixedDateFormat);
            parameters.RequestedDeliveryDateString = customerOrder.RequestedDeliveryDate.ToString(FixedDateFormat);
            parameters.DeliveryMode = customerOrder.DeliveryMode != null ? customerOrder.DeliveryMode.Code : string.Empty;
            parameters.PrepaymentAmountOverridden = customerOrder.PrepaymentAmountOverridden;

            parameters.Email = customerOrder.ReceiptEmailAddress;
            parameters.Comment = ((IPosTransactionV1)customerOrder).Comment;
            parameters.ReturnReasonCodeId = customerOrder.ReturnReasonCodeId;
            parameters.LoyaltyCardId = (customerOrder.LoyaltyItem != null && customerOrder.LoyaltyItem.UsageType == LSRetailPosis.Transaction.Line.LoyaltyItem.LoyaltyItemUsageType.UsedForLoyaltyRequest) ? 
                customerOrder.LoyaltyItem.LoyaltyCardNumber : string.Empty;

            // If we do not have the channel reference identifier, we create a new receipt identifier instead.
            parameters.ChannelReferenceId = !string.IsNullOrWhiteSpace(customerOrder.ChannelReferenceId)
                ? customerOrder.ChannelReferenceId
                : SalesOrder.InternalApplication.Services.ApplicationService.GetNextReceiptId(customerOrder);

            parameters.CreditCardToken = customerOrder.CreditCardToken;

            // Discount codes
            parameters.DiscountCodes = new Collection<string>();
            foreach (string code in customerOrder.DiscountCodes)
            {
                parameters.DiscountCodes.Add(code);
            }

            // Line Items
            parameters.Items = new Collection<ItemInfo>();
            foreach (SaleLineItem item in customerOrder.SaleItems)
            {
                if (!item.Voided)
                {
                    string deliveryMode = parameters.DeliveryMode;
                    if (item.DeliveryMode != null)
                    {
                        deliveryMode = item.DeliveryMode.Code;
                    }

                    string deliveryDateString = parameters.RequestedDeliveryDateString;
                    if (item.DeliveryDate.HasValue)
                    {
                        deliveryDateString = item.DeliveryDate.Value.ToString(FixedDateFormat);
                    }

                    Collection<ChargeInfo> lineChargeInfo = new Collection<ChargeInfo>();
                    foreach (Tax.MiscellaneousCharge charge in item.MiscellaneousCharges)
                    {
                        lineChargeInfo.Add(new ChargeInfo()
                        {
                            Amount = charge.Amount,
                            Code = charge.ChargeCode,
                            SalesTaxGroup = charge.SalesTaxGroupId,
                            TaxGroup = charge.TaxGroupId
                        });
                    }

                    // If no line-level warehouse is specified, fall back to the header warehouse
                    string inventLocationId = string.IsNullOrWhiteSpace(item.DeliveryWarehouse) ? customerOrder.WarehouseId : item.DeliveryWarehouse;

                    //AX SO line stores discount amount per item, POS stores for whole line, calculate per item discount amount
                    decimal lineDiscount = (item.Quantity == 0M ? 0M : (item.TotalDiscount + item.LineDiscount + item.PeriodicDiscount) / (item.Quantity));

                    parameters.Items.Add(new ItemInfo()
                    {
                        RecId = item.OrderLineRecordId,

                        //quantity
                        ItemId = item.ItemId,
                        Quantity = item.Quantity,
                        Unit = item.SalesOrderUnitOfMeasure,

                        //pricing
                        Price = item.Price,
                        Discount = lineDiscount,
                        NetAmount = item.NetAmount,
                        ItemTaxGroup = item.TaxGroupId,
                        SalesTaxGroup = item.SalesTaxGroupId,
                        SalesMarkup = item.SalesMarkup,

                        //delivery
                        WarehouseId = inventLocationId,
                        AddressRecordId = item.ShippingAddress != null ? item.ShippingAddress.AddressRecId : null,
                        DeliveryMode = deliveryMode,
                        RequestedDeliveryDateString = deliveryDateString,

                        //inventDim
                        BatchId = item.BatchId,
                        SerialId = item.SerialId,
                        VariantId = item.Dimension.VariantId,
                        ColorId = item.Dimension.ColorId,
                        SizeId = item.Dimension.SizeId,
                        StyleId = item.Dimension.StyleId,
                        ConfigId = item.Dimension.ConfigId,

                        //Return
                        InvoiceId = item.ReturnInvoiceId,
                        InventTransId = item.ReturnInvoiceInventTransId,

                        //line-level misc. charges
                        Charges = lineChargeInfo,
                    });
                }
            }

            // Header level Misc Charges
            parameters.Charges = new Collection<ChargeInfo>();
            foreach (Tax.MiscellaneousCharge charge in customerOrder.MiscellaneousCharges)
            {
                parameters.Charges.Add(new ChargeInfo()
                {
                    Code = charge.ChargeCode,
                    Amount = charge.Amount,
                    SalesTaxGroup = charge.SalesTaxGroupId,
                    TaxGroup = charge.TaxGroupId
                });
            }

            // Payments
            parameters.Payments = new Collection<PaymentInfo>();
            foreach (ITenderLineItem tender in customerOrder.TenderLines)
            {
                if (!tender.Voided)
                {
                    ICardTenderLineItem cardTender = tender as ICardTenderLineItem;
                    if (cardTender != null && cardTender.EFTInfo.IsAuthOnly)
                    {
                        // This is a Pre-Authorization record.
                        IEFTInfo eft = cardTender.EFTInfo;
                        parameters.Preauthorization = new Preauthorization()
                        {
                            PaymentPropertiesBlob = eft.PaymentProviderPropertiesXML
                        };
                    }
                    else if (tender.Amount != decimal.Zero)
                    {
                        // This is an actual payment record.
                        parameters.Payments.Add(new PaymentInfo()
                        {
                            PaymentType = tender.TenderTypeId,
                            Amount = tender.Amount,
                            Currency = (tender.CurrencyCode) ?? string.Empty
                        });
                    }
                }
            }
            return parameters;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode")]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Needs refactoring")]
        internal static CustomerOrderTransaction GetTransactionFromInfo(CustomerOrderInfo orderInfo, SalesOrder salesOrderService)
        {
            CustomerOrderTransaction transaction = (CustomerOrderTransaction)SalesOrder.InternalApplication.BusinessLogic.Utility.CreateCustomerOrderTransaction(
                            ApplicationSettings.Terminal.StoreId,
                            ApplicationSettings.Terminal.StoreCurrency,
                            ApplicationSettings.Terminal.TaxIncludedInPrice,
                            SalesOrder.InternalApplication.Services.Rounding,
                            salesOrderService);

            // Get all the defaults
            SalesOrder.InternalApplication.BusinessLogic.TransactionSystem.LoadTransactionStatus(transaction);

            //General header properties
            transaction.OrderId = orderInfo.Id;
            transaction.OrderType = orderInfo.OrderType;

            if (orderInfo.OrderType == CustomerOrderType.Quote)
            {
                transaction.QuotationId = orderInfo.Id;
            }

            transaction.OriginalOrderType = orderInfo.OrderType;

            switch (orderInfo.OrderType)
            {
                case CustomerOrderType.Quote:
                    transaction.OrderStatus = GetSalesStatus((SalesQuotationStatus)orderInfo.Status);
                    break;
                case CustomerOrderType.SalesOrder:
                    transaction.OrderStatus = GetSalesStatus((SalesOrderStatus)orderInfo.Status, (DocumentStatus)orderInfo.DocumentStatus);
                    break;
                default:
                    transaction.OrderStatus = SalesStatus.Unknown;
                    NetTracer.Information("SalesOrder::CustomerOrderTransaction: CustomerOrderInfo OrderType is unknown: {0}", orderInfo.OrderType);
                    break;
            }
            transaction.LockPrices = true;

            transaction.ExpirationDate = ParseDateString(orderInfo.ExpiryDateString, DateTime.Today);

            //RequestedDeliveryDate is directly input from user. It is stored in the local timezone
            transaction.RequestedDeliveryDate = ParseDateString(orderInfo.RequestedDeliveryDateString, DateTime.Today);

            //CreationDate is stored in UTC. It needs to be converted to local time zone where order is accessed.
            ((IPosTransactionV2)transaction).BeginDateTime = ParseDateString(orderInfo.CreationDateString, DateTime.UtcNow, DateTimeStyles.AdjustToUniversal).ToLocalTime();
            transaction.LocalHourOfDay = orderInfo.LocalHourOfDay;

            ((IPosTransactionV2)transaction).Comment = orderInfo.Comment;

            // Header delivery 
            DM.StoreDataManager storeDataManager = new DM.StoreDataManager(
                SalesOrder.InternalApplication.Settings.Database.Connection, 
                SalesOrder.InternalApplication.Settings.Database.DataAreaID);

            transaction.WarehouseId = orderInfo.WarehouseId;
            transaction.DeliveryMode = storeDataManager.GetDeliveryMode(orderInfo.DeliveryMode);

            foreach (var code in orderInfo.DiscountCodes)
            {
                transaction.DiscountCodes.AddLast(code);
            }

            // Customer info
            ICustomerSystem customerSystem = SalesOrder.InternalApplication.BusinessLogic.CustomerSystem;
            DM.CustomerDataManager customerDataManager = new DM.CustomerDataManager(
                SalesOrder.InternalApplication.Settings.Database.Connection, 
                SalesOrder.InternalApplication.Settings.Database.DataAreaID);

            DE.ICustomer customer = customerSystem.GetCustomerInfo(orderInfo.CustomerAccount);

            // try to get the customer from transaction service
            if (customer == null || customer.IsEmptyCustomer())
            {
                DE.ICustomer tempCustomer = SalesOrder.GetCustomerFromAX(orderInfo.CustomerAccount, customerSystem, customerDataManager);
                if (tempCustomer != null)
                {
                    customer = tempCustomer;
                }
            }

            customerSystem.SetCustomer(transaction, customer, customer);

            if (transaction.DeliveryMode != null && !string.IsNullOrWhiteSpace(orderInfo.AddressRecordId))
            {
                DE.IAddress shippingAddress = customerDataManager.GetAddress(Int64.Parse(orderInfo.AddressRecordId));
                customerSystem.SetShippingAddress(transaction, shippingAddress);
            }

            if (!string.IsNullOrEmpty(orderInfo.SalespersonStaffId))
            {
                // Sets the sales person id and name according to AX values
                // This is done because we do not know whether the sales person information is available on this store
                transaction.SalesPersonId = orderInfo.SalespersonStaffId;
                transaction.SalesPersonName = orderInfo.SalespersonName;

                DM.EmployeeDataManager employees = new DM.EmployeeDataManager(
                    SalesOrder.InternalApplication.Settings.Database.Connection, 
                    SalesOrder.InternalApplication.Settings.Database.DataAreaID);

                Employee employee = employees.GetEmployee(ApplicationSettings.Terminal.StoreId, orderInfo.SalespersonStaffId);
                if (employee != null)
                {
                    transaction.SalesPersonId = employee.StaffId;
                    transaction.SalesPersonName = employee.Name;
                    transaction.SalesPersonNameOnReceipt = employee.NameOnReceipt;
                }
            }

            transaction.ChannelReferenceId = orderInfo.ChannelReferenceId;
            if (transaction.LoyaltyItem != null && !string.IsNullOrEmpty(orderInfo.LoyaltyCardId))
            {
                transaction.LoyaltyItem.LoyaltyCardNumber = orderInfo.LoyaltyCardId;
                transaction.LoyaltyItem.UsageType = LSRetailPosis.Transaction.Line.LoyaltyItem.LoyaltyItemUsageType.UsedForLoyaltyRequest;
            }

            transaction.ReceiptEmailAddress = orderInfo.Email;

            DateTime earliestDeliveryDate = DateTime.MaxValue;

            // Items
            foreach (ItemInfo item in orderInfo.Items)
            {
                Collection<Tax.MiscellaneousCharge> lineCharges = new Collection<Tax.MiscellaneousCharge>();
                foreach (ChargeInfo charge in item.Charges)
                {
                    Tax.MiscellaneousCharge lineCharge = (Tax.MiscellaneousCharge)SalesOrder.InternalApplication.BusinessLogic.Utility.CreateMiscellaneousCharge(
                        charge.Amount, charge.SalesTaxGroup, charge.TaxGroup, charge.Code, string.Empty, transaction);
                    lineCharges.Add(lineCharge);
                }

                // add item
                SaleLineItem lineItem = (SaleLineItem)SalesOrder.InternalApplication.BusinessLogic.Utility.CreateSaleLineItem(
                    ApplicationSettings.Terminal.StoreCurrency,
                    SalesOrder.InternalApplication.Services.Rounding,
                    transaction);

                lineItem.Found = true;
                lineItem.OrderLineRecordId = item.RecId;
                lineItem.ItemId = item.ItemId;
                lineItem.Quantity = item.Quantity;
                lineItem.ReturnQtyAllowed = item.Quantity;
                lineItem.SalesOrderUnitOfMeasure = item.Unit;
                lineItem.Price = item.Price;
                lineItem.NetAmount = item.NetAmount;
                lineItem.QuantityOrdered = item.Quantity;
                lineItem.QuantityPickedUp = item.QuantityPicked;
                lineItem.DeliveryMode = storeDataManager.GetDeliveryMode(item.DeliveryMode);
                lineItem.DeliveryDate = ParseDateString(item.RequestedDeliveryDateString, DateTime.Today);
                lineItem.DeliveryStoreNumber = item.StoreId;
                lineItem.DeliveryWarehouse = item.WarehouseId;
                lineItem.SerialId = item.SerialId;
                lineItem.BatchId = item.BatchId;
                lineItem.HasBeenRecalled = true;
                lineItem.SalesMarkup = item.SalesMarkup;
                lineItem.LineStatus = GetSalesStatus((SalesOrderStatus)item.Status);

                foreach (Tax.MiscellaneousCharge charge in lineCharges)
                {
                    lineItem.MiscellaneousCharges.Add(charge);
                }

                if (lineItem.DeliveryMode != null && !string.IsNullOrWhiteSpace(item.AddressRecordId))
                {
                    lineItem.ShippingAddress = customerDataManager.GetAddress(Int64.Parse(item.AddressRecordId));
                }

                lineItem.Dimension.VariantId = item.VariantId;
                lineItem.Dimension.ColorId = item.ColorId;
                lineItem.Dimension.StyleId = item.StyleId;
                lineItem.Dimension.SizeId = item.SizeId;
                lineItem.Dimension.ConfigId = item.ConfigId;
                lineItem.Dimension.ColorName = item.ColorName;
                lineItem.Dimension.StyleName = item.StyleName;
                lineItem.Dimension.SizeName = item.SizeName;
                lineItem.Dimension.ConfigName = item.ConfigName;

                if (!String.IsNullOrEmpty(lineItem.Dimension.VariantId))
                {
                    Dimensions dimension = (Dimensions)SalesOrder.InternalApplication.BusinessLogic.Utility.CreateDimension();
                    dimension.VariantId = lineItem.Dimension.VariantId;
                    SalesOrder.InternalApplication.Services.Dimension.GetDimensionForVariant(dimension);
                    lineItem.Dimension = dimension;
                }

                // set discount, everything is converted into a LineDiscount
                if ((item.Discount != decimal.Zero) && (item.Quantity != decimal.Zero))
                {
                    ILineDiscountItem lineDiscountItem = SalesOrder.InternalApplication.BusinessLogic.Utility.CreateLineDiscountItem();

                    // AX discount amount is per item
                    lineDiscountItem.Amount = item.Discount;

                    // this method takes the per item discount amount
                    SalesOrder.InternalApplication.Services.Discount.AddLineDiscountAmount(lineItem, lineDiscountItem);
                }

                // Set other default properties for this item
                SalesOrder.InternalApplication.Services.Item.ProcessItem(lineItem);

                // Set tax info after defaults, as it may have been overridden.
                lineItem.SalesTaxGroupId = item.SalesTaxGroup;
                lineItem.SalesTaxGroupIdOriginal = item.SalesTaxGroup;
                lineItem.TaxGroupId = item.ItemTaxGroup;
                lineItem.TaxGroupIdOriginal = item.ItemTaxGroup;

                // Add it to the transaction
                transaction.Add(lineItem);

                if (lineItem.DeliveryDate < earliestDeliveryDate)
                {
                    earliestDeliveryDate = lineItem.DeliveryDate.HasValue ? lineItem.DeliveryDate.Value : earliestDeliveryDate;
                }
            }
            //The order can be created through some other channel other than POS which has set header delivery date as NoDate.
            //This must not be interpreted as a valid date. Instead the earliestDeliveryDate is used.
            if (transaction.RequestedDeliveryDate == NoDate)
            {
                transaction.RequestedDeliveryDate = earliestDeliveryDate;
            }

            // Charges
            foreach (ChargeInfo charge in orderInfo.Charges)
            {
                //add charges
                Tax.MiscellaneousCharge newCharge = (Tax.MiscellaneousCharge)SalesOrder.InternalApplication.BusinessLogic.Utility.CreateMiscellaneousCharge(
                    charge.Amount, charge.SalesTaxGroup, charge.TaxGroup, charge.Code, string.Empty, transaction);
                transaction.Add(newCharge);
            }

            SalesOrder.InternalApplication.BusinessLogic.ItemSystem.CalculatePriceTaxDiscount(transaction);
            transaction.CalculateAmountDue();

            // Payments
            // - total up amounts
            // - add history entries
            transaction.PrepaymentAmountPaid = decimal.Zero;
            transaction.PrepaymentAmountInvoiced = decimal.Zero;
            decimal nonPrepayments = decimal.Zero;
            foreach (PaymentInfo payment in orderInfo.Payments)
            {
                // sum up payments
                decimal amount = (string.IsNullOrWhiteSpace(payment.Currency))
                    ? payment.Amount
                    : (SalesOrder.InternalApplication.Services.Currency.CurrencyToCurrency(
                        payment.Currency,
                        ApplicationSettings.Terminal.StoreCurrency,
                        payment.Amount));
               
                if (payment.Prepayment)
                {
                    // Sum prepayments to track total deposits paid
                    transaction.PrepaymentAmountPaid += amount;
                }
                else
                {
                    // Sum non-prepayments as base for calculating deposits applied to pickups
                    nonPrepayments += amount;
                }

                CustomerOrderPaymentHistory paymentHistory = (CustomerOrderPaymentHistory)SalesOrder.InternalApplication.BusinessLogic.Utility.CreateCustomerOrderPaymentHistory();
                paymentHistory.Amount = payment.Amount;
                paymentHistory.Currency = payment.Currency;
                paymentHistory.Date = ParseDateString(payment.DateString, DateTime.MinValue);
                paymentHistory.Balance = transaction.NetAmountWithTaxAndCharges - transaction.PrepaymentAmountPaid;

                transaction.PaymentHistory.Add(paymentHistory);
            }

            // Prepayment/Deposit override info
            transaction.PrepaymentAmountOverridden = orderInfo.PrepaymentAmountOverridden;
            if (transaction.PrepaymentAmountOverridden)
            {
                transaction.PrepaymentAmountRequired = transaction.PrepaymentAmountPaid;
            }

            // Amount that has been previously invoiced (picked-up)
            transaction.PreviouslyInvoicedAmount = orderInfo.PreviouslyInvoicedAmount;

            // Portion of the prepayment that has been applied to invoices 
            // (total amount invoiced less payments, difference is the deposit applied)
            transaction.PrepaymentAmountInvoiced = transaction.PreviouslyInvoicedAmount - nonPrepayments;   

            return transaction;
        }

        /// <summary>
        /// Parse the date from an AX-sent date string
        /// </summary>
        /// <param name="dateString"></param>
        /// <returns></returns>
        internal static DateTime ParseDateString(string dateString, DateTime defaultDate, DateTimeStyles dateTimeStyle = DateTimeStyles.AssumeLocal)
        {
            DateTimeFormatInfo info = new DateTimeFormatInfo();
            DateTime result = defaultDate;

            if (string.IsNullOrWhiteSpace(dateString)
                || (!DateTime.TryParseExact(dateString, FixedDateFormat, info, dateTimeStyle, out result)
                    && !DateTime.TryParseExact(dateString, FixedDateTimeFormat, info, dateTimeStyle, out result)
                   )
                )
            {
                return defaultDate;
            }
            else
            {
                return result;
            }
        }

        internal static decimal ConvertToDecimalAtIndex(IList list, int index)
        {
            try
            {
                return Convert.ToDecimal(list[index]);
            }
            catch
            {
                return 0m;
            }
        }

        internal static bool ConvertToBooleanAtIndex(IList list, int index)
        {
            try
            {
                return Convert.ToBoolean(list[index]);
            }
            catch
            {
                return false;
            }
        }

        internal static string ConvertToStringAtIndex(IList list, int index)
        {
            try
            {
                return Convert.ToString(list[index]);
            }
            catch
            {
                return string.Empty;
            }
        }

        internal static DateTime ConvertToDateTimeAtIndex(IList list, int index)
        {
            try
            {
                return Convert.ToDateTime(list[index]).ToLocalTime();
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

                    /// <summary>
        /// Gets the SalesStatus from SalesOrderStatus and DocumentStatus
        /// </summary>
        /// <param name="docStatus"></param>
        /// <param name="salesOrderStatus"></param>
        /// <returns></returns>
        internal static SalesStatus GetSalesStatus(SalesOrderStatus salesOrderStatus, DocumentStatus docStatus)
        {
            SalesStatus salesStatus = GetSalesStatus(salesOrderStatus);

            switch (salesStatus)
            {
                case SalesStatus.Unknown:
                case SalesStatus.Created:
                    SalesStatus documentStatus = GetSalesStatus(docStatus);
                    if (documentStatus == SalesStatus.Unknown)
                    {
                        return salesStatus;
                    }

                    return documentStatus;
                default:
                    return salesStatus;
            }
        }

        /// <summary>
        /// Convert SalesOrderStatus to SalesStatus
        /// </summary>
        /// <param name="salesOrderStatus"></param>
        /// <returns></returns>
        internal static SalesStatus GetSalesStatus(SalesOrderStatus salesOrderStatus)
        {
            switch (salesOrderStatus)
            {
                case SalesOrderStatus.Backorder:    return SalesStatus.Created;
                case SalesOrderStatus.Delivered:    return SalesStatus.Delivered;
                case SalesOrderStatus.Invoiced:     return SalesStatus.Invoiced;
                case SalesOrderStatus.Canceled:     return SalesStatus.Canceled;
                default:                            return SalesStatus.Unknown;
            }
        }

        /// <summary>
        /// Convert DocumentStatus to SalesStatus
        /// </summary>
        /// <param name="docStatus"></param>
        /// <returns></returns>
        internal static SalesStatus GetSalesStatus(DocumentStatus docStatus)
        {
            switch (docStatus)
            {
                case DocumentStatus.None:           return SalesStatus.Created;
                case DocumentStatus.PickingList:    return SalesStatus.Processing;
                case DocumentStatus.PackingSlip:    return SalesStatus.Delivered;
                case DocumentStatus.Invoice:        return SalesStatus.Invoiced;
                case DocumentStatus.Canceled:       return SalesStatus.Canceled;
                case DocumentStatus.Lost:           return SalesStatus.Lost;
                default:                            return SalesStatus.Unknown;
            }
        }

        /// <summary>
        /// Convert SalesQuotationStatus to SalesStatus
        /// </summary>
        /// <param name="quoteStatus"></param>
        /// <returns></returns>
        internal static SalesStatus GetSalesStatus(SalesQuotationStatus quoteStatus)
        {
            switch (quoteStatus)
            {
                case SalesQuotationStatus.Created:      return SalesStatus.Created;
                case SalesQuotationStatus.Confirmed:    return SalesStatus.Confirmed;
                case SalesQuotationStatus.Sent:         return SalesStatus.Canceled;
                case SalesQuotationStatus.Canceled:     return SalesStatus.Canceled;
                case SalesQuotationStatus.Lost:         return SalesStatus.Lost;
                default:                                return SalesStatus.Unknown;
            }
        }

    }
}
