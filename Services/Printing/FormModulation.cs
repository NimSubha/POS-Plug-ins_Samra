/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using LSRetailPosis;
using LSRetailPosis.DataAccess;
using LSRetailPosis.Settings;
using LSRetailPosis.Settings.HardwareProfiles;
using LSRetailPosis.Settings.FunctionalityProfiles;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.SaleItem;
using LSRetailPosis.Transaction.Line.TenderItem;
using Microsoft.Dynamics.Retail.Diagnostics;
using Microsoft.Dynamics.Retail.Pos.Contracts.BusinessLogic;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using System.Collections.Generic;
using LSRetailPosis.Transaction.Line.TaxItems;
using LSRetailPosis.Transaction.Line.Tax;

namespace Microsoft.Dynamics.Retail.Pos.Printing
{
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    public class FormModulation
    {
        #region Member variables

        private SqlConnection connection;
        private string copyText = string.Empty;
        char esc = Convert.ToChar(27);
        private const string LOGO_MESSAGE = "<L>";
        private ReceiptData receiptData;
        private bool isDetailTable = false;

        #endregion

        #region Properties
        public SqlConnection Connection
        {
            get { return connection; }
            set { connection = value; }
        }

        private static IUtility Utility
        {
            get { return Printing.InternalApplication.BusinessLogic.Utility; }
        }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="connection"></param>
        public FormModulation(SqlConnection connection)
        {
            //
            // Get all text through the Translation function in the ApplicationLocalizer
            //
            // TextID's for FormModulation are reserved at 13020 - 13049
            // In use now are ID's: 13020 - 13040
            //


            this.connection = connection;
            this.receiptData = new ReceiptData(connection, string.Empty);
        }

        private static string CreateWhitespace(int stringLength, char seperator)
        {
            StringBuilder whiteString = new StringBuilder();
            for (int i = 1; i <= stringLength; i++)
            {
                whiteString.Append(seperator);
            }

            return whiteString.ToString();
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Grandfather")]
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Grand Father PS6015")]
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Grandfather")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Grandfather")]
        private string GetInfoFromTransaction(FormItemInfo itemInfo, IEFTInfo eftInfo, ITenderLineItem tenderItem, RetailTransaction theTransaction, DataTable dt = null)
        {
            CustomerOrderTransaction cot;

            try
            {
                if (theTransaction != null || dt != null)
                {
                    switch (itemInfo.Variable.ToUpperInvariant().Replace(" ", string.Empty))
                    {
                        case "DATE":
                            if (dt != null && dt.Rows.Count > 0)
                                return Convert.ToString(dt.Rows[0]["ORDERDATE"]);
                            return ((IPosTransactionV1)theTransaction).BeginDateTime.ToShortDateString();
                        case "TIME24H":
                            if (dt != null && dt.Rows.Count > 0)
                                return DateTime.Now.ToString("HH:mm");
                            return ((IPosTransactionV1)theTransaction).BeginDateTime.ToString("HH:mm");
                        case "EFTDATE":
                            return eftInfo.TransactionDateTime.ToShortDateString();
                        case "EFTTIME24H":
                            return eftInfo.TransactionDateTime.ToString("HH:mm");
                        case "EFTACCOUNTTYPE":
                            return eftInfo.AccountType;
                        case "EFTAPPLICATIONID":
                            return eftInfo.ApplicationIdentifier;
                        case "TIME12H":
                            return ((IPosTransactionV1)theTransaction).BeginDateTime.ToString("hh:mm tt");
                        case "TRANSNO":
                            return theTransaction.TransactionId ?? string.Empty;
                        case "TRANSACTIONNUMBER":
                            if (dt != null && dt.Rows.Count > 0)
                                return Convert.ToString(dt.Rows[0]["ORDERNUM"]);
                            return theTransaction.TransactionId ?? string.Empty;
                        case "RECEIPTNUMBER":
                            return theTransaction.ReceiptId;
                        case "STAFF_ID":
                            return theTransaction.OperatorId ?? string.Empty;
                        case "EMPLOYEENAME":
                            return theTransaction.OperatorName;
                        case "OPERATORID":
                            if (dt != null && dt.Rows.Count > 0)
                                return Convert.ToString(dt.Rows[0]["STAFFID"]);
                            return theTransaction.OperatorId ?? string.Empty;
                        case "OPERATORNAME":
                            return theTransaction.OperatorName;
                        case "OPERATORNAMEONRECEIPT":
                            return theTransaction.OperatorNameOnReceipt;
                        case "EMPLOYEEID":
                            return theTransaction.OperatorId;
                        case "SALESPERSONID":
                            return theTransaction.SalesPersonId;
                        case "SALESPERSONNAME":
                            return theTransaction.SalesPersonName;
                        case "SALESPERSONNAMEONRECEIPT":
                            return theTransaction.SalesPersonNameOnReceipt;
                        case "TOTALWITHTAX":
                            if (dt != null && dt.Rows.Count > 0)
                                return Convert.ToString(dt.Rows[0]["TOTALAMOUNT"]);
                            return Printing.InternalApplication.Services.Rounding.Round(theTransaction.NetAmountWithTaxAndCharges, theTransaction.StoreCurrencyCode, true);
                        case "REMAININGBALANCE":
                            return Printing.InternalApplication.Services.Rounding.Round(eftInfo.RemainingBalance, theTransaction.StoreCurrencyCode, true);
                        case "TOTAL":
                            if (dt != null && dt.Rows.Count > 0)
                                return Convert.ToString(dt.Rows[0]["TOTALAMOUNT"]);
                            return Printing.InternalApplication.Services.Rounding.Round(theTransaction.NetAmountWithNoTax, theTransaction.StoreCurrencyCode, true);
                        case "TAXTOTAL":
                            if (dt != null && dt.Rows.Count > 0)
                                return "0";
                            return Printing.InternalApplication.Services.Rounding.Round(theTransaction.TaxAmount, theTransaction.StoreCurrencyCode, true);
                        case "SUMTOTALDISCOUNT":
                            return Printing.InternalApplication.Services.Rounding.Round(theTransaction.TotalDiscount, theTransaction.StoreCurrencyCode, true);
                        case "SUMLINEDISCOUNT":
                            return Printing.InternalApplication.Services.Rounding.Round(theTransaction.LineDiscount, theTransaction.StoreCurrencyCode, true);
                        case "TERMINALID":
                            if (dt != null && dt.Rows.Count > 0)
                                return Convert.ToString(dt.Rows[0]["TERMINALID"]);
                            return theTransaction.TerminalId;
                        case "CASHIER":
                            return LSRetailPosis.Settings.ApplicationSettings.Terminal.TerminalOperator.Name;
                        case "CUSTOMERNAME":
                            if (dt != null && dt.Rows.Count > 0)
                                return Convert.ToString(dt.Rows[0]["CUSTNAME"]);
                            return theTransaction.Customer.Name;
                        case "CUSTOMERACCOUNTNUMBER":
                            if (dt != null && dt.Rows.Count > 0)
                                return Convert.ToString(dt.Rows[0]["CUSTACCOUNT"]);
                            return theTransaction.Customer.CustomerId ?? string.Empty;
                        case "CUSTOMERADDRESS":
                            return theTransaction.Customer.Address;
                        case "CUSTOMERAMOUNT":
                            return Printing.InternalApplication.Services.Rounding.Round(theTransaction.AmountToAccount, theTransaction.StoreCurrencyCode, true);
                        case "CUSTOMERVAT":
                            return theTransaction.Customer.VatNum;
                        case "CUSTOMERTAXOFFICE":
                            return theTransaction.Customer.TaxOffice;
                        case "CARDEXPIREDATE":
                            return eftInfo.ExpDate.Substring(0, 2) + "/" + eftInfo.ExpDate.Substring(2, 2);
                        case "CARDNUMBER":
                            return Utility.MaskCardNumber(eftInfo.CardNumber);
                        case "CARDNUMBERPARTLYHIDDEN":
                            return "**-" + eftInfo.CardNumber.Substring(eftInfo.CardNumber.Length - 4, 4);
                        case "CARDTYPE":
                            return eftInfo.CardName;
                        case "CARDISSUERNAME":
                            return eftInfo.IssuerName;
                        case "CARDAMOUNT":
                            return Printing.InternalApplication.Services.Rounding.Round(Math.Abs(eftInfo.Amount), theTransaction.StoreCurrencyCode, true);
                        case "CARDAUTHNUMBER":
                            return eftInfo.AuthCode;
                        case "BATCHCODE":
                            return eftInfo.BatchCode;
                        case "ACQUIRERNAME":
                            return eftInfo.AcquirerName;
                        case "VISAAUTHCODE":
                            return eftInfo.VisaAuthCode;
                        case "EUROAUTHCODE":
                            return eftInfo.EuropayAuthCode;
                        case "EFTSTORECODE":
                            return eftInfo.StoreNumber.PadRight(4, '0').Substring(eftInfo.StoreNumber.Length - 4, 4);
                        case "EFTTERMINALNUMBER":
                            return eftInfo.TerminalNumber.PadLeft(4, '0');
                        case "EFTINFOMESSAGE":
                            if (eftInfo.Authorized)
                            {
                                return copyText + eftInfo.AuthorizedText;
                            }
                            else
                            {
                                return eftInfo.NotAuthorizedText;
                            }
                        case "EFTTERMINALID":
                            return eftInfo.TerminalId.PadLeft(8, '0');
                        case "EFTMERCHANTID":
                            return EFT.MerchantId;
                        case "ENTRYSOURCECODE":
                            return eftInfo.EntrySourceCode;
                        case "AUTHSOURCECODE":
                            return eftInfo.AuthSourceCode;
                        case "AUTHORIZATIONCODE":
                            return eftInfo.AuthCode;
                        case "SEQUENCECODE":
                            return eftInfo.SequenceCode;
                        case "EFTMESSAGE":
                            return eftInfo.Message;
                        case "EFTRETRIEVALREFERENCENUMBER":
                            return eftInfo.RetrievalReferenceNumber;
                        case "CUSTOMERTENDERAMOUNT":
                            return Printing.InternalApplication.Services.Rounding.Round(tenderItem.Amount, theTransaction.StoreCurrencyCode, true);
                        case "TENDERROUNDING":
                            return Printing.InternalApplication.Services.Rounding.Round(decimal.Negate(theTransaction.TransSalePmtDiff), theTransaction.StoreCurrencyCode, true);
                        case "INVOICECOMMENT":
                            if (dt != null && dt.Rows.Count > 0)
                                return string.Empty;
                            return theTransaction.InvoiceComment;
                        case "TRANSACTIONCOMMENT":
                            return theTransaction.Comment;
                        case "LOGO":
                            return LOGO_MESSAGE;
                        case "RECEIPTNUMBERBARCODE":
                            if (dt != null && dt.Rows.Count > 0)
                                return Convert.ToString("<B: " + dt.Rows[0]["ORDERNUM"] + "> - BY NSPL");
                            return "<B: " + theTransaction.ReceiptId + ">";
                        case "CUSTOMERORDERBARCODE":
                            if ((cot = theTransaction as CustomerOrderTransaction) != null)
                            {
                                return "<B: " + cot.OrderId + ">";
                            }
                            return string.Empty;
                        case "REPRINTMESSAGE":
                            return copyText;
                        case "OFFLINEINDICATOR":
                            if (theTransaction.CreatedOffline)
                                return ApplicationLocalizer.Language.Translate(13043);
                            else
                                return string.Empty;
                        case "STOREID":
                            return theTransaction.StoreId;
                        case "STORENAME":
                            return theTransaction.StoreName;
                        case "STOREADDRESS":
                            return theTransaction.StoreAddress;
                        case "STOREPHONE":
                            return theTransaction.StorePhone;
                        case "STORETAXIDENTIFICATIONNUMBER":
                            return ApplicationSettings.Terminal.TaxIdNumber;
                        case "TENDERAMOUNT":
                            return Printing.InternalApplication.Services.Rounding.Round(Math.Abs(tenderItem.Amount), theTransaction.StoreCurrencyCode, true);
                        case "USEDLOYALTYPOINTS":
                            return Printing.InternalApplication.Services.Rounding.Round(theTransaction.TenderLines.OfType<ILoyaltyTenderLineItem>().Sum(l => l.LoyaltyPoints), 0, false);
                        case "ISSUEDLOYALTYPOINTS":
                            {
                                if (theTransaction.LoyaltyItem != null)
                                {
                                    return Printing.InternalApplication.Services.Rounding.Round(theTransaction.LoyaltyItem.CalculatedLoyaltyPoints, 0, false);
                                }
                                else
                                {
                                    return string.Empty;
                                }
                            }
                        case "ACCUMULATEDLOYALTYPOINTS":
                            {
                                if (theTransaction.LoyaltyItem != null)
                                {
                                    return Printing.InternalApplication.Services.Rounding.Round(theTransaction.LoyaltyItem.AccumulatedLoyaltyPoints, 0, false);
                                }
                                else
                                {
                                    return string.Empty;
                                }
                            }
                        case "LOYALTYCARDNUMBER":
                            {
                                if (theTransaction.LoyaltyItem != null)
                                {
                                    return theTransaction.LoyaltyItem.LoyaltyCardNumber;
                                }
                                else
                                {
                                    return string.Empty;
                                }
                            }
                        case "CREDITMEMONUMBER":
                            {
                                if ((theTransaction.CreditMemoItem == null)
                                    || (theTransaction.CreditMemoItem.CreditMemoNumber == null))
                                {
                                    return string.Empty;
                                }

                                return theTransaction.CreditMemoItem.CreditMemoNumber;
                            }
                        case "CREDITMEMOAMOUNT":
                            {
                                if (theTransaction.CreditMemoItem == null)
                                {
                                    return string.Empty;
                                }

                                return Printing.InternalApplication.Services.Rounding.Round(
                                    theTransaction.CreditMemoItem.Amount, theTransaction.StoreCurrencyCode, true);
                            }
                        case "ALLTENDERCOMMENTS":
                            {
                                string comments = string.Empty;
                                foreach (ITenderLineItem tenderLineItem in theTransaction.TenderLines)
                                {
                                    if (tenderLineItem.Comment != null)
                                    {
                                        comments += tenderLineItem.Comment;
                                    }
                                }

                                return comments;
                            }
                        case "ALLITEMCOMMENTS":
                            {
                                string comments = string.Empty;
                                foreach (ISaleLineItem saleLineItem in theTransaction.SaleItems)
                                {
                                    if (saleLineItem.Comment != null)
                                    {
                                        comments += saleLineItem.Comment;
                                    }
                                }

                                return comments;
                            }
                        case "DEPOSITDUE":
                            if ((cot = theTransaction as CustomerOrderTransaction) != null)
                            {
                                return Printing.InternalApplication.Services.Rounding.Round(cot.PrepaymentAmountRequired, theTransaction.StoreCurrencyCode, true);
                            }
                            return string.Empty;
                        case "DEPOSITPAID":
                            if ((cot = theTransaction as CustomerOrderTransaction) != null)
                            {
                                //When there is a possibility for override, a partial deposit amount can be paid
                                return Printing.InternalApplication.Services.Rounding.Round(cot.PrepaymentAmountPaid, theTransaction.StoreCurrencyCode, true);
                            }
                            return string.Empty;
                        case "DELIVERYTYPE":
                            if ((theTransaction as CustomerOrderTransaction) != null)
                            {
                                return string.Equals(theTransaction.DeliveryMode.Code, ApplicationSettings.Terminal.PickupDeliveryModeCode, StringComparison.Ordinal)
                                ? LSRetailPosis.ApplicationLocalizer.Language.Translate(56349)  // "Pick up all"
                                : LSRetailPosis.ApplicationLocalizer.Language.Translate(56348); // "Ship all"
                            }
                            return string.Empty;

                        case "DELIVERYMETHOD":
                            if ((cot = theTransaction as CustomerOrderTransaction) != null)
                            {
                                return cot.DeliveryMode != null ? cot.DeliveryMode.DescriptionText : string.Empty;
                            }
                            return string.Empty;
                        case "DELIVERYDATE":
                            if ((cot = theTransaction as CustomerOrderTransaction) != null)
                            {
                                return cot.RequestedDeliveryDate.ToShortDateString();
                            }
                            return string.Empty;
                        case "ORDERTYPE":
                            if ((cot = theTransaction as CustomerOrderTransaction) != null)
                            {
                                return cot.OrderType.ToString();
                            }
                            return string.Empty;
                        case "ORDERSTATUS":
                            if ((cot = theTransaction as CustomerOrderTransaction) != null)
                            {
                                switch (cot.OrderStatus)
                                {
                                    case LSRetailPosis.Transaction.SalesStatus.Canceled:
                                        return LSRetailPosis.ApplicationLocalizer.Language.Translate(56379);
                                    case LSRetailPosis.Transaction.SalesStatus.Confirmed:
                                        return LSRetailPosis.ApplicationLocalizer.Language.Translate(56404);
                                    case LSRetailPosis.Transaction.SalesStatus.Created:
                                        return LSRetailPosis.ApplicationLocalizer.Language.Translate(99709);
                                    case LSRetailPosis.Transaction.SalesStatus.Delivered:
                                        return LSRetailPosis.ApplicationLocalizer.Language.Translate(56377);
                                    case LSRetailPosis.Transaction.SalesStatus.Invoiced:
                                        return LSRetailPosis.ApplicationLocalizer.Language.Translate(56378);
                                    case LSRetailPosis.Transaction.SalesStatus.Lost:
                                        return LSRetailPosis.ApplicationLocalizer.Language.Translate(56403);
                                    case LSRetailPosis.Transaction.SalesStatus.Processing:
                                        return LSRetailPosis.ApplicationLocalizer.Language.Translate(56402);
                                    case LSRetailPosis.Transaction.SalesStatus.Sent:
                                        return LSRetailPosis.ApplicationLocalizer.Language.Translate(56405);
                                    case LSRetailPosis.Transaction.SalesStatus.Unknown:
                                        return string.Empty;
                                    default:
                                        return string.Empty;
                                }
                            }
                            return string.Empty;
                        case "REFERENCENO":
                            if ((cot = theTransaction as CustomerOrderTransaction) != null)
                            {
                                return cot.OrderId ?? string.Empty;
                            }
                            return string.Empty;
                        case "EXPIRYDATE":
                            if ((cot = theTransaction as CustomerOrderTransaction) != null)
                            {
                                return cot.ExpirationDate.ToShortDateString();
                            }
                            return string.Empty;
                        case "ORDERID":
                            if ((cot = theTransaction as CustomerOrderTransaction) != null)
                            {
                                return cot.OrderId;
                            }
                            return string.Empty;
                        case "TOTALSHIPPIINGCHARGES":
                        case "SHIPPINGCHARGE":
                            if ((cot = theTransaction as CustomerOrderTransaction) != null)
                            {
                                string shippingCode = ApplicationSettings.Terminal.ShippingChargeCode;
                                decimal lineCharges = cot.SaleItems.Where(i => !i.Voided).Sum(c => c.MiscellaneousCharges.Sum(m => (m.ChargeCode == shippingCode ? m.Amount : decimal.Zero)));
                                decimal headerCharges = cot.MiscellaneousCharges.Sum(m => (m.ChargeCode == shippingCode ? m.Amount : decimal.Zero));
                                return Printing.InternalApplication.Services.Rounding.Round(lineCharges + headerCharges, theTransaction.StoreCurrencyCode, true);
                            }
                            return string.Empty;
                        case "CANCELLATIONCHARGE":
                            if ((cot = theTransaction as CustomerOrderTransaction) != null)
                            {
                                string code = ApplicationSettings.Terminal.CancellationChargeCode;
                                decimal sum = cot.MiscellaneousCharges.Sum(c => (c.ChargeCode == code ? c.Amount : decimal.Zero));
                                return Printing.InternalApplication.Services.Rounding.Round(sum, theTransaction.StoreCurrencyCode, true);
                            }
                            return string.Empty;
                        case "TAXONSHIPPING":
                            //Select all charge lines containing chargecode as shipping code. Sum all tax items and return
                            if ((cot = theTransaction as CustomerOrderTransaction) != null)
                            {
                                string shippingCode = ApplicationSettings.Terminal.ShippingChargeCode;
                                var sum = decimal.Zero;
                                foreach (var charge in cot.MiscellaneousCharges)
                                {
                                    if (charge.ChargeCode == shippingCode)
                                    {
                                        sum += charge.TaxAmount;
                                    }
                                }
                                return Printing.InternalApplication.Services.Rounding.Round(sum, theTransaction.StoreCurrencyCode, true);
                            }
                            return string.Empty;
                        case "MISCCHARGETOTAL":
                            if ((cot = theTransaction as CustomerOrderTransaction) != null)
                            {
                                decimal lineCharges = cot.SaleItems.Where(i => !i.Voided).Sum(c => c.MiscellaneousCharges.Sum(m => m.Amount));
                                decimal headerCharges = cot.MiscellaneousCharges.Sum(m => m.Amount);
                                return Printing.InternalApplication.Services.Rounding.Round(lineCharges + headerCharges, theTransaction.StoreCurrencyCode, true);
                            }
                            return string.Empty;

                        case "DEPOSITAPPLIED":
                            if ((cot = theTransaction as CustomerOrderTransaction) != null)
                            {
                                return Printing.InternalApplication.Services.Rounding.Round(cot.PrepaymentAmountApplied, theTransaction.StoreCurrencyCode, true);
                            }
                            return string.Empty;
                        case "TOTALLINEITEMSHIPPINGCHARGES":
                            if ((cot = theTransaction as CustomerOrderTransaction) != null)
                            {
                                string shippingCode = ApplicationSettings.Terminal.ShippingChargeCode;
                                decimal lineCharges = cot.SaleItems.Where(i => !i.Voided).Sum(c => c.MiscellaneousCharges.Sum(m => (m.ChargeCode == shippingCode ? m.Amount : decimal.Zero)));
                                return Printing.InternalApplication.Services.Rounding.Round(lineCharges, theTransaction.StoreCurrencyCode, true);
                            }
                            return string.Empty;

                        case "ORDERSHIPPINGCHARGE":
                            if ((cot = theTransaction as CustomerOrderTransaction) != null)
                            {
                                string shippingCode = ApplicationSettings.Terminal.ShippingChargeCode;
                                decimal headerCharges = cot.MiscellaneousCharges.Sum(m => (m.ChargeCode == shippingCode ? m.Amount : decimal.Zero));
                                return Printing.InternalApplication.Services.Rounding.Round(headerCharges, theTransaction.StoreCurrencyCode, true);
                            }
                            return string.Empty;
                        case "TOTALPAYMENTS":
                            if ((cot = theTransaction as CustomerOrderTransaction) != null)
                            {
                                var totalPayments = TotalPayments(cot);
                                return Printing.InternalApplication.Services.Rounding.Round(totalPayments, cot.StoreCurrencyCode, true);
                            }
                            return string.Empty;
                        case "BALANCE":
                            if ((cot = theTransaction as CustomerOrderTransaction) != null)
                            {
                                var balance = cot.NetAmountWithTaxAndCharges - TotalPayments(cot);
                                Printing.InternalApplication.Services.Rounding.Round(balance, cot.StoreCurrencyCode, true);
                            }
                            return string.Empty;

                        // India receipt tax summary
                        case "COMPANYPANNO_IN":
                            return ApplicationSettings.Terminal.IndiaCompanyPanNumber;
                        case "VATTINNO_IN":
                            return ApplicationSettings.Terminal.IndiaVatTinNumber;
                        case "CSTTINNO_IN":
                            return ApplicationSettings.Terminal.IndiaCstTinNumber;
                        case "STCNUMBER_IN":
                            return ApplicationSettings.Terminal.IndiaSTCNumber;
                        case "ECCNUMBER_IN":
                            return ApplicationSettings.Terminal.IndiaECCNumber;
                    }
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                NetTracer.Warning("Printing [FormModulation] - Invalid Argument when trying to get information from transcation");
                return string.Empty;
            }
            catch (NullReferenceException)
            {
                NetTracer.Warning("Printing [FormModulation] - Null reference when trying to get information from transcation");
                return string.Empty;
            }

            return string.Empty;
        }
        /// <summary>
        /// All payments paid in any form (deposit, pickup etc.) 
        /// </summary>
        /// <param name="cot"></param>
        /// <returns></returns>
        private static decimal TotalPayments(CustomerOrderTransaction cot)
        {
            decimal totalPayments = 0.0M;
            foreach (var payment in cot.PaymentHistory)
            {
                // sum up payments
                decimal amount = (string.IsNullOrWhiteSpace(payment.Currency))
                    ? payment.Amount
                    : (Printing.InternalApplication.Services.Currency.CurrencyToCurrency(
                        payment.Currency,
                        ApplicationSettings.Terminal.StoreCurrency,
                        payment.Amount));

                totalPayments += amount;
            }
            return totalPayments;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Grandfather")]
        private static string GetInfoFromSaleLineItem(FormItemInfo itemInfo, SaleLineItem saleLine, IPosTransaction theTransaction, DataRow drItem = null, DataTable dtSubDetails = null)
        {

            string returnValue = string.Empty;

            if (saleLine == null && drItem == null)
            {
                return returnValue;
            }
            try
            {
                PharmacySalesLineItem asPharmacySalesLineItem = saleLine as PharmacySalesLineItem;

                switch (itemInfo.Variable.ToUpperInvariant().Replace(" ", string.Empty))
                {
                    case "TAXID":
                        returnValue = saleLine.TaxGroupId;
                        break;
                    case "TAXPERCENT":
                        returnValue = Printing.InternalApplication.Services.Rounding.Round(saleLine.TaxRatePct, false);
                        break;

                    case "TAXFLAG":
                        returnValue = GetTaxFlag(saleLine);
                        break;
                    case "ITEMNAME":
                        if (drItem != null)
                        {
                            ItemData Oitem = new ItemData(ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID, ApplicationSettings.Terminal.StorePrimaryId);
                            returnValue = Oitem.GetProductName(Convert.ToString(drItem["ITEMID"]), ApplicationSettings.Terminal.CultureName);
                        }
                        else
                            returnValue = saleLine.Description;
                        break;
                    case "ITEMNAMEALIAS":
                        returnValue = saleLine.DescriptionAlias;
                        break;
                    case "ITEMID":
                        returnValue = saleLine.ItemId;
                        break;
                    case "ITEMBARCODE":
                        returnValue = saleLine.BarcodeId;
                        break;
                    case "QTY":
                        if (drItem != null)
                            returnValue = Convert.ToString(drItem["QTY"]);
                        else
                            returnValue = Printing.InternalApplication.Services.Rounding.RoundQuantity(saleLine.Quantity, saleLine.SalesOrderUnitOfMeasure);
                        break;
                    case "UNITPRICE":
                        if (drItem != null)
                            returnValue = Convert.ToString((Convert.ToDecimal(drItem["AMOUNT"]) + Convert.ToDecimal(drItem["MAKINGAMOUNT"]) + (string.IsNullOrEmpty(Convert.ToString(drItem["EXTENDEDDETAILSAMOUNT"])) ? 0 : Convert.ToDecimal(drItem["EXTENDEDDETAILSAMOUNT"]))) / Convert.ToDecimal(drItem["QTY"]));
                        else
                            returnValue = Printing.InternalApplication.Services.Rounding.Round(saleLine.Price, false);
                        break;
                    case "TOTALPRICE":
                        if (drItem != null)
                            returnValue = Convert.ToString((Convert.ToDecimal(drItem["AMOUNT"]) + Convert.ToDecimal(drItem["MAKINGAMOUNT"]) + (string.IsNullOrEmpty(Convert.ToString(drItem["EXTENDEDDETAILSAMOUNT"])) ? 0 : Convert.ToDecimal(drItem["EXTENDEDDETAILSAMOUNT"]))));
                        else
                            returnValue = Printing.InternalApplication.Services.Rounding.Round(saleLine.GrossAmount, false);
                        break;
                    case "TOTALPRICEWITHTAX":
                        returnValue = Printing.InternalApplication.Services.Rounding.Round(saleLine.NetAmountWithTax, false);
                        break;
                    case "ITEMUNITID":
                        returnValue = saleLine.SalesOrderUnitOfMeasure;
                        break;
                    case "ITEMUNITIDNAME":
                        returnValue = saleLine.SalesOrderUnitOfMeasureName;
                        break;
                    case "LINEDISCOUNTAMOUNT":
                        returnValue = Printing.InternalApplication.Services.Rounding.Round(decimal.Negate(saleLine.LineDiscount), false);
                        break;
                    case "LINEDISCOUNTPERCENT":
                        returnValue = Printing.InternalApplication.Services.Rounding.Round(saleLine.LinePctDiscount, false);
                        break;
                    case "PERIODICDISCOUNTAMOUNT":
                        returnValue = Printing.InternalApplication.Services.Rounding.Round(decimal.Negate(saleLine.PeriodicDiscount), false);
                        break;
                    case "PERIODICDISCOUNTNAME":
                        returnValue = saleLine.TotalPeriodicOfferId;
                        break;
                    case "PERIODICDISCOUNTPERCENT":
                        returnValue = Printing.InternalApplication.Services.Rounding.Round(saleLine.PeriodicPctDiscount, false);
                        break;
                    case "TOTALDISCOUNTAMOUNT":
                        returnValue = Printing.InternalApplication.Services.Rounding.Round(decimal.Negate(saleLine.TotalDiscount), false);
                        break;
                    case "TOTALDISCOUNTPERCENT":
                        returnValue = Printing.InternalApplication.Services.Rounding.Round(saleLine.TotalPctDiscount, false);
                        break;
                    case "PHARMACYDOSAGESTRENGTH":
                        returnValue = Utility.ToString(asPharmacySalesLineItem.DosageStrength);
                        break;
                    case "PHARMACYPRESCRIPTIONNUMBER":
                        returnValue = Utility.ToString(asPharmacySalesLineItem.PrescriptionId);
                        break;
                    case "PHARMACYDOSAGESTRENGTHUNIT":
                        returnValue = asPharmacySalesLineItem.DosageStrengthUnit;
                        break;
                    case "PHARMACYDOSAGEUNITQTY":
                        returnValue = Utility.ToString(asPharmacySalesLineItem.DosageUnitQuantity);
                        break;
                    case "PHARMACYDOSAGETYPE":
                        returnValue = asPharmacySalesLineItem.DosageType;
                        break;
                    case "BATCHID":
                        returnValue = saleLine.BatchId;
                        break;
                    case "BATCHEXPDATE":
                        returnValue = saleLine.BatchExpDate.ToShortDateString();
                        break;
                    case "ITEMGROUP":
                        returnValue = saleLine.ItemGroupId;
                        break;
                    case "DIMENSIONCOLORID":
                        returnValue = saleLine.Dimension.ColorId;
                        break;
                    case "DIMENSIONCOLORVALUE":
                        returnValue = saleLine.Dimension.ColorName;
                        break;
                    case "DIMENSIONSIZEID":
                        returnValue = saleLine.Dimension.SizeId;
                        break;
                    case "DIMENSIONSIZEVALUE":
                        returnValue = saleLine.Dimension.SizeName;
                        break;
                    case "DIMENSIONSTYLEID":
                        returnValue = saleLine.Dimension.StyleId;
                        break;
                    case "DIMENSIONSTYLEVALUE":
                        returnValue = saleLine.Dimension.StyleName;
                        break;
                    case "DIMENSIONCONFIGID":
                        returnValue = saleLine.Dimension.ConfigId;
                        break;
                    case "DIMENSIONCONFIGVALUE":
                        returnValue = saleLine.Dimension.ConfigName;
                        break;
                    case "ITEMCOMMENT":
                        returnValue = saleLine.Comment;
                        break;
                    case "SERIALID":
                        returnValue = saleLine.SerialId;
                        break;
                    case "RFID":
                        returnValue = saleLine.RFIDTagId;
                        break;
                    case "UNITOFMEASURE":
                        // (e.g. 1.5Lb @ $1.99/Lb)
                        returnValue = string.Format("{0}{2} @ {1}/{2}", Printing.InternalApplication.Services.Rounding.RoundQuantity(saleLine.Quantity, saleLine.SalesOrderUnitOfMeasure),
                            Printing.InternalApplication.Services.Rounding.Round(saleLine.Price, theTransaction.StoreCurrencyCode, true), saleLine.SalesOrderUnitOfMeasure);
                        break;
                    case "LINEDELIVERYTYPE":
                        returnValue = string.Empty;
                        break;
                    case "LINEDELIVERYMETHOD":
                        returnValue = saleLine.DeliveryMode != null ? saleLine.DeliveryMode.DescriptionText : string.Empty;
                        break;
                    case "LINEDELIVERYDATE":
                        returnValue = (saleLine.DeliveryDate.HasValue ? saleLine.DeliveryDate.Value.ToShortDateString() : string.Empty);
                        break;
                    case "PICKUPQTY":
                        returnValue = Printing.InternalApplication.Services.Rounding.RoundQuantity(saleLine.Quantity, saleLine.SalesOrderUnitOfMeasure);
                        break;
                    case "LINEITEMSHIPPINGCHARGE":
                        returnValue = Printing.InternalApplication.Services.Rounding.Round(GetShippingCharge(saleLine), theTransaction.StoreCurrencyCode, true);
                        break;
                    case "ITEMTAX":
                        returnValue = Printing.InternalApplication.Services.Rounding.Round(saleLine.TaxAmount, theTransaction.StoreCurrencyCode, true);
                        break;
                }

                if (returnValue == null)
                {
                    returnValue = string.Empty;
                }
                else
                {
                    if (itemInfo.Prefix.Length > 0)
                    {
                        returnValue = itemInfo.Prefix + returnValue;
                    }
                }
            }
            catch (NullReferenceException)
            {
                NetTracer.Warning("Printing [FormModulation] - Null reference when trying to get information from sales line item");

                returnValue = string.Empty;
            }

            return returnValue;
        }

        private static string GetTaxFlag(SaleLineItem saleLine)
        {
            var returnValue = string.Empty;
            var hasExempt = false;
            var hasZero = (saleLine.TaxLines.Count == 0); // Init with false if any tax lines; otherwise, set to true if no tax lines

            foreach (ITaxItem taxLine in saleLine.TaxLines)
            {
                if (taxLine.Exempt)
                {
                    hasExempt = true;
                }
                if (taxLine.Percentage == decimal.Zero)
                {
                    hasZero = true;
                }
            }

            if (hasZero && hasExempt)
            {
                returnValue = ApplicationLocalizer.Language.Translate(13041) + " " + ApplicationLocalizer.Language.Translate(13040); //Zero Tax & Exempt
            }
            else if (hasZero)
            {
                returnValue = ApplicationLocalizer.Language.Translate(13041); //Zero tax
            }
            else if (hasExempt)
            {
                returnValue = ApplicationLocalizer.Language.Translate(13040); //Exempt
            }
            return returnValue;
        }

        private static decimal GetShippingCharge(SaleLineItem saleLine)
        {
            var shippingCode = ApplicationSettings.Terminal.ShippingChargeCode;
            return saleLine.MiscellaneousCharges.Sum(m => (m.ChargeCode == shippingCode ? m.Amount : decimal.Zero));
        }

        private static string GetInfoFromTenderLineItem(FormItemInfo itemInfo, ITenderLineItem tenderLine, IPosTransaction theTransaction)
        {
            string returnValue = string.Empty;
            ITender tenderInfo;
            try
            {
                if (tenderLine != null)
                {
                    switch (itemInfo.Variable.ToUpperInvariant().Replace(" ", string.Empty))
                    {
                        case "TENDERNAME":
                            if (tenderLine.Amount < 0)
                            {
                                TenderData tenderData = new TenderData(ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID);
                                tenderInfo = tenderData.GetTender(tenderLine.TenderTypeId, theTransaction.StoreId);
                                if (tenderInfo.FunctionType == 1 || tenderInfo.FunctionType == 3) // FunctionType = 1 (cards), FunctionType = 3 (customer account)
                                {
                                    returnValue = ApplicationLocalizer.Language.Translate(13033); // "charge back"
                                }
                                else
                                {
                                    returnValue = ApplicationLocalizer.Language.Translate(13031); // "change back"
                                }
                                returnValue += " (" + tenderLine.Description + ")";
                            }
                            else
                                returnValue = tenderLine.Description;

                            if (tenderLine.ForeignCurrencyAmount != 0)
                            {
                                returnValue += " (" + Printing.InternalApplication.Services.Rounding.Round(tenderLine.ForeignCurrencyAmount, false) + " " + tenderLine.CurrencyCode + ")";
                            }
                            break;
                        case "TENDERAMOUNT":
                            returnValue = Printing.InternalApplication.Services.Rounding.Round(Math.Abs(tenderLine.Amount), theTransaction.StoreCurrencyCode, true);
                            break;
                        case "TENDERCOMMENT":
                            returnValue = tenderLine.Comment;
                            break;
                    }

                    if (returnValue == null)
                    {
                        returnValue = string.Empty;
                    }
                }
            }
            catch (NullReferenceException)
            {
                NetTracer.Warning("Printing [FormModulation] - Null reference when trying to get information from tender line item");

                returnValue = string.Empty;
            }

            if (returnValue.Length > itemInfo.Length)
            {
                returnValue = Wrap(returnValue, itemInfo);
            }

            return returnValue;
        }

        private static string GetInfoFromTaxItem(FormItemInfo itemInfo, ITaxItem taxLine)
        {
            string returnValue = string.Empty;
            try
            {
                if (taxLine != null)
                {
                    switch (itemInfo.Variable.ToUpperInvariant().Replace(" ", string.Empty))
                    {
                        case "TAXID":
                            returnValue = taxLine.TaxCode;
                            break;
                        case "TAXGROUP":
                            returnValue = taxLine.TaxGroup;
                            break;
                        case "TAXPERCENTAGE":
                            returnValue = Printing.InternalApplication.Services.Rounding.Round(taxLine.Percentage, 2, false);
                            break;
                        case "TOTAL":
                            returnValue = Printing.InternalApplication.Services.Rounding.Round(taxLine.Price, false);
                            break;
                        case "TAXAMOUNT":
                            returnValue = Printing.InternalApplication.Services.Rounding.Round(taxLine.Amount, false);
                            break;
                        case "PRICE":
                            returnValue = Printing.InternalApplication.Services.Rounding.Round(taxLine.Price, false);
                            break;
                        case "TAXFLAG":
                            if (taxLine.Exempt)
                            {
                                returnValue = ApplicationLocalizer.Language.Translate(13040);   //Exempt
                            }
                            else if (taxLine.Percentage == decimal.Zero)
                            {
                                returnValue = ApplicationLocalizer.Language.Translate(13041); //Zero tax
                            }
                            break;

                        case "BASICAMOUNT_IN":
                            returnValue = Printing.InternalApplication.Services.Rounding.Round(taxLine.TaxBasis, false);
                            break;
                        case "TOTALAMOUNT_IN":
                            returnValue = Printing.InternalApplication.Services.Rounding.Round(taxLine.TaxBasis + taxLine.Amount, false);
                            break;
                        case "TAXCOMPONENT_IN":
                            TaxItemIndia taxLineIN = taxLine as TaxItemIndia;
                            if (taxLineIN != null)
                            {
                                returnValue = taxLineIN.TaxComponent;
                            }
                            break;
                    }

                    if (returnValue == null)
                    {
                        returnValue = string.Empty;
                    }
                }
            }
            catch (NullReferenceException)
            {
                NetTracer.Warning("Printing [FormModulation] - Null reference when trying to get information from tax item");

                returnValue = string.Empty;
            }

            if (returnValue.Length > itemInfo.Length)
            {
                returnValue = Wrap(returnValue, itemInfo);
            }

            return returnValue;
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "itemInfo", Justification = "Grandfather")]
        private static int CountWordLength(string wordString, FormItemInfo itemInfo)
        {
            int count = 0;
            foreach (char ch in wordString)
            {
                if ((ch != '\r') && (ch != '\n'))
                {
                    count += 1;
                }
            }

            return count;
        }

        private static string GetAlignmentSettings(string parsedString, FormItemInfo itemInfo)
        {
            // the indication of a logo is passed on unchanged.
            if (parsedString == LOGO_MESSAGE)
            {
                return parsedString;
            }

            if (parsedString.Length > itemInfo.Length)
            {
                // don't trim strings that contain carrage return, they are considered to be previously handled
                if (!parsedString.Contains(Environment.NewLine))
                {
                    // The value seems to need to be trimmed
                    switch (itemInfo.VertAlign)
                    {
                        case valign.right:
                            parsedString = parsedString.Substring(parsedString.Length - itemInfo.Length, itemInfo.Length);
                            break;
                        default:
                            parsedString = parsedString.Substring(0, itemInfo.Length);
                            break;
                    }
                }
                else
                {
                    // If it contains carriage returns, don't forget to insert indents
                    string indent = Environment.NewLine + CreateWhitespace(itemInfo.CharIndex - 1, ' ');
                    parsedString = parsedString.Replace(Environment.NewLine, indent);
                }
            }
            else if (parsedString.Length < itemInfo.Length)
            {
                // The value seems to need to be filled
                int charCountUsableForSpace = itemInfo.Length - CountWordLength(parsedString, itemInfo);
                switch (itemInfo.VertAlign)
                {
                    case valign.left:
                        parsedString = parsedString.PadRight(itemInfo.Length, itemInfo.Fill);
                        break;
                    case valign.center:
                        int spaceOnLeftSide = (int)charCountUsableForSpace / 2;
                        int spaceOnRightSide = charCountUsableForSpace - spaceOnLeftSide;
                        parsedString = parsedString.PadLeft(spaceOnLeftSide + parsedString.Length, itemInfo.Fill);
                        parsedString = parsedString.PadRight(spaceOnRightSide + parsedString.Length, itemInfo.Fill);
                        break;
                    case valign.right:
                        parsedString = parsedString.PadLeft(charCountUsableForSpace + parsedString.Length, itemInfo.Fill);
                        break;
                }
            }

            return parsedString;
        }

        private static string ParseTenderLineVariable(FormItemInfo itemInfo, ITenderLineItem tenderLineItem, IPosTransaction theTransaction)
        {
            string variableString = GetInfoFromTenderLineItem(itemInfo, tenderLineItem, theTransaction);

            // Setting the align if neccessary
            return GetAlignmentSettings(variableString, itemInfo);
        }

        private static string ParseItemVariable(FormItemInfo itemInfo, SaleLineItem saleLineItem, IPosTransaction theTransaction, DataRow drItem = null, DataTable dtSubDetails = null)
        {
            string parsedString = string.Empty;

            if (itemInfo.IsVariable)
            {
                if (drItem != null)
                    parsedString = GetInfoFromSaleLineItem(itemInfo, null, null, drItem, dtSubDetails);
                else
                    parsedString = GetInfoFromSaleLineItem(itemInfo, saleLineItem, theTransaction);
            }
            else
            {
                parsedString = itemInfo.ValueString;
            }

            // Setting the align if neccessary
            parsedString = GetAlignmentSettings(parsedString, itemInfo);

            return parsedString;
        }

        private string ParseCardTenderVariable(FormItemInfo itemInfo, IEFTInfo eftInfo, RetailTransaction theTransaction)
        {
            string tmpString = string.Empty;

            if (itemInfo.IsVariable)
            {
                tmpString = GetInfoFromTransaction(itemInfo, eftInfo, null, theTransaction);
            }
            else
            {
                tmpString = itemInfo.ValueString;
            }

            // Setting the align if neccessary
            tmpString = GetAlignmentSettings(tmpString, itemInfo);

            return tmpString;
        }

        private string ParseTenderVariable(FormItemInfo itemInfo, ITenderLineItem tenderLineItem, RetailTransaction theTransaction)
        {
            string tmpString = string.Empty;

            if (itemInfo.IsVariable)
            {
                tmpString = GetInfoFromTransaction(itemInfo, null, tenderLineItem, theTransaction);
            }
            else
            {
                tmpString = itemInfo.ValueString;
            }

            // Setting the align if neccessary
            tmpString = GetAlignmentSettings(tmpString, itemInfo);

            return tmpString;
        }

        private static string ParseTaxVariable(FormItemInfo itemInfo, ITaxItem taxItem)
        {
            string tmpString = string.Empty;

            if (itemInfo.IsVariable)
            {
                tmpString = GetInfoFromTaxItem(itemInfo, taxItem);
            }
            else
            {
                tmpString = itemInfo.ValueString;
            }

            // Setting the align if neccessary
            tmpString = GetAlignmentSettings(tmpString, itemInfo);

            return tmpString;
        }

        private string ParseVariable(FormItemInfo itemInfo, ITenderLineItem tenderItem, RetailTransaction theTransaction, DataTable dt = null)
        {
            string tmpString = string.Empty;

            if (itemInfo.IsVariable)
            {
                tmpString = GetInfoFromTransaction(itemInfo, null, tenderItem, theTransaction, dt);
            }
            else
            {
                tmpString = itemInfo.ValueString;
            }

            if (tmpString == null)
            {
                tmpString = string.Empty;
            }

            // Setting the align if neccessary
            tmpString = GetAlignmentSettings(tmpString, itemInfo);

            return tmpString;
        }

        private string ReadCardTenderDataSet(DataSet ds, IEFTInfo eftInfo, RetailTransaction theTransaction)
        {
            String returnString = string.Empty;

            // Note: Receipt templates are persisted as searilzed DataTable objects. Don't change the case
            // of these tables/fields for backward compatibility.

            DataTable lineTable = ds.Tables["line"];
            DataTable charPosTable;
            FormItemInfo itemInfo = null;

            if (lineTable != null)
            {
                foreach (DataRow dr in lineTable.Select(string.Empty, "nr asc"))
                {
                    String lineString = string.Empty;
                    string idVariable = (string)dr["ID"];

                    if (idVariable == "CRLF")
                    {
                        lineString += Environment.NewLine;
                    }
                    else if ((idVariable == "CardHolderSignature") && (eftInfo.CardType != EFTCardTypes.CREDIT_CARD))
                    {
                        // Skip card holder signature line for other than Credit Cards.
                    }
                    else
                    {
                        string drLineId = Utility.ToString(dr["line_id"]);
                        charPosTable = ds.Tables["charpos"];
                        if (charPosTable != null)
                        {
                            int nextCharNr = 1;
                            foreach (DataRow row in charPosTable.Select(lineTable.TableName + "_Id='" + drLineId + "'", "nr asc"))
                            {
                                try
                                {
                                    itemInfo = new FormItemInfo(row);
                                    // Adding possible whitespace at the beginning of line
                                    lineString += CreateWhitespace(itemInfo.CharIndex - nextCharNr, ' ');

                                    if (itemInfo.FontStyle == System.Drawing.FontStyle.Bold)
                                    {
                                        lineString += esc + "|2C";
                                    }
                                    else
                                    {
                                        lineString += esc + "|1C";
                                    }

                                    // Parsing the itemInfo
                                    lineString += ParseCardTenderVariable(itemInfo, eftInfo, theTransaction);

                                    // Closing the string with a single space command to make sure spaces are always single spaced
                                    lineString += esc + "|1C";

                                    // Specifing the position of the next char in the current line - bold take twice as much space
                                    nextCharNr = itemInfo.CharIndex + (itemInfo.Length * itemInfo.SizeFactor);

                                }
                                catch (Exception ex)
                                {
                                    ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                                }
                            }
                        }

                        lineString += Environment.NewLine;
                    }

                    returnString += lineString;
                }
            }

            return returnString.ToString();
        }

        private string ReadTenderDataSet(DataSet ds, ITenderLineItem tenderLineItem, RetailTransaction theTransaction)
        {
            String returnString = string.Empty;

            DataTable lineTable = ds.Tables["line"];
            DataTable charPosTable;
            FormItemInfo itemInfo = null;

            if (lineTable != null)
            {
                foreach (DataRow dr in lineTable.Select(string.Empty, "nr asc"))
                {
                    String lineString = string.Empty;
                    string idVariable = (string)dr["ID"];

                    switch (idVariable)
                    {
                        case "CRLF":
                            lineString += Environment.NewLine;
                            break;
                        case "Text":
                            string drLineId = Utility.ToString(dr["line_id"]);
                            charPosTable = ds.Tables["charpos"];
                            if (charPosTable != null)
                            {
                                int nextCharNr = 1;
                                foreach (DataRow row in charPosTable.Select(lineTable.TableName + "_Id='" + drLineId + "'", "nr asc"))
                                {
                                    try
                                    {
                                        itemInfo = new FormItemInfo(row);
                                        // Adding possible whitespace at the beginning of line
                                        lineString += CreateWhitespace(itemInfo.CharIndex - nextCharNr, ' ');

                                        if (itemInfo.FontStyle == System.Drawing.FontStyle.Bold)
                                        {
                                            lineString += esc + "|2C";
                                        }
                                        else
                                        {
                                            lineString += esc + "|1C";
                                        }

                                        // Parsing the itemInfo
                                        lineString += ParseTenderVariable(itemInfo, tenderLineItem, theTransaction);

                                        // Closing the string with a single space command to make sure spaces are always single spaced
                                        lineString += esc + "|1C";

                                        // Specifing the position of the next char in the current line - bold take twice as much space
                                        nextCharNr = itemInfo.CharIndex + (itemInfo.Length * itemInfo.SizeFactor);
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }

                            lineString += Environment.NewLine;
                            break;
                    }

                    returnString += lineString;
                }
            }

            return returnString.ToString();
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Grandfather")]
        private void ParseSaleItem(DataSet ds, ref FormItemInfo itemInfo, StringBuilder lIneStringBuilder, SaleLineItem saleLineItem, IPosTransaction theTransaction, DataRow drCustOrder = null, DataSet dsCustOrder = null)
        {
            #region SaleLineItem NOT NULL - NOT A CUSTOMER ORDER
            if (saleLineItem != null)
            {
                string idVariable = string.Empty;
                string drLineId = string.Empty;
                saleLineItem.GetType();
                // Only non-voided items will be printed
                if (!saleLineItem.Voided)
                {
                    DataTable lineTable = ds.Tables["line"];
                    DataTable charPosTable;
                    if (lineTable != null)
                    {
                        foreach (DataRow dr in lineTable.Select(string.Empty, "nr asc"))
                        {
                            idVariable = Utility.ToString(dr["ID"]);
                            switch (idVariable)
                            {
                                case "CRLF":
                                    {
                                        lIneStringBuilder.Append(Environment.NewLine);
                                        break;
                                    }
                                default:
                                    {
                                        drLineId = Utility.ToString(dr["line_id"]);

                                        if ((idVariable == "PharmacyLine") && (!(saleLineItem is PharmacySalesLineItem))) { /*donothing*/}
                                        else if ((idVariable == "TotalDiscount") && (saleLineItem.TotalDiscount == 0)) { /*donothing*/}
                                        else if ((idVariable == "LineDiscount") && (saleLineItem.LineDiscount == 0)) { /*donothing*/}
                                        else if ((idVariable == "PeriodicDiscount") && (saleLineItem.PeriodicDiscount == 0)) { /*donothing*/}
                                        else if ((idVariable == "Batch") && (string.IsNullOrEmpty(saleLineItem.BatchId))) { /*donothing*/}
                                        else if (idVariable == "UnitOfMeasure" && !saleLineItem.ScaleItem) { /*donothing*/ }
                                        else if (idVariable == "ManualWeight" && !(saleLineItem.ScaleItem && saleLineItem.WeightManuallyEntered)) { /*donothing*/ }
                                        else if ((idVariable == "Dimension")
                                            && ((string.IsNullOrEmpty(saleLineItem.Dimension.ColorName))
                                            && (string.IsNullOrEmpty(saleLineItem.Dimension.SizeName))
                                            && (string.IsNullOrEmpty(saleLineItem.Dimension.StyleName))
                                            && (string.IsNullOrEmpty(saleLineItem.Dimension.ConfigName)))) { /*donothing*/ }
                                        else if ((idVariable == "Comment") && (string.IsNullOrEmpty(saleLineItem.Comment))) { /*donothing*/}
                                        else
                                        {
                                            // options for idVariable:
                                            // Itemlines
                                            // TotalDiscount
                                            // LineDiscount
                                            charPosTable = ds.Tables["charpos"];
                                            if (charPosTable != null)
                                            {
                                                int nextCharNr = 1;
                                                foreach (DataRow row in charPosTable.Select(lineTable.TableName + "_Id='" + drLineId + "'", "nr asc"))
                                                {
                                                    try
                                                    {
                                                        itemInfo = new FormItemInfo(row);
                                                        // Adding possible whitespace at the beginning of line
                                                        lIneStringBuilder.Append(CreateWhitespace(itemInfo.CharIndex - nextCharNr, ' '));

                                                        if (itemInfo.FontStyle == System.Drawing.FontStyle.Bold)
                                                        {
                                                            lIneStringBuilder.Append(esc + "|2C");
                                                        }
                                                        else
                                                        {
                                                            lIneStringBuilder.Append(esc + "|1C");
                                                        }

                                                        // Parsing the itemInfo
                                                        lIneStringBuilder.Append(ParseItemVariable(itemInfo, saleLineItem, theTransaction));

                                                        // Closing the string with a single space command to make sure spaces are always single spaced
                                                        lIneStringBuilder.Append(esc + "|1C");

                                                        // Specifing the position of the next char in the current line - bold take twice as much space
                                                        nextCharNr = itemInfo.CharIndex + (itemInfo.Length * itemInfo.SizeFactor);
                                                    }
                                                    catch (Exception)
                                                    {
                                                    }
                                                }
                                            }

                                            lIneStringBuilder.Append(Environment.NewLine);
                                        }

                                        break;
                                    }
                            }
                        }
                    }
                }
            }
            #endregion

            else
            {
                string idVariable = string.Empty;
                string drLineId = string.Empty;

                // Only non-voided items will be printed

                DataTable lineTable = ds.Tables["line"];
                DataTable charPosTable;
                if (lineTable != null)
                {
                    foreach (DataRow dr in lineTable.Select(string.Empty, "nr asc"))
                    {
                        idVariable = Utility.ToString(dr["ID"]);
                        switch (idVariable)
                        {
                            case "CRLF":
                                {
                                    lIneStringBuilder.Append(Environment.NewLine);
                                    break;
                                }
                            default:
                                {
                                    drLineId = Utility.ToString(dr["line_id"]);

                                    if ((idVariable == "PharmacyLine")) { /*donothing*/}
                                    else if ((idVariable == "TotalDiscount")) { /*donothing*/}
                                    else if ((idVariable == "LineDiscount")) { /*donothing*/}
                                    else if ((idVariable == "PeriodicDiscount")) { /*donothing*/}
                                    else if ((idVariable == "Batch")) { /*donothing*/}
                                    else if (idVariable == "UnitOfMeasure") { /*donothing*/ }
                                    else if (idVariable == "ManualWeight") { /*donothing*/ }
                                    else if ((idVariable == "Comment")) { /*donothing*/}
                                    else
                                    {
                                        // options for idVariable:
                                        // Itemlines
                                        // TotalDiscount
                                        // LineDiscount
                                        charPosTable = ds.Tables["charpos"];
                                        if (charPosTable != null)
                                        {
                                            int nextCharNr = 1;
                                            if (isDetailTable)
                                            {
                                                DataTable dtOrderSubDetails = null;

                                                if (dsCustOrder != null && dsCustOrder.Tables.Count > 1 && dsCustOrder.Tables[2].Rows.Count > 0)
                                                    dtOrderSubDetails = dsCustOrder.Tables[2].Clone();
                                                DataRow drNew = null;
                                                foreach (DataRow rowNew in dsCustOrder.Tables[2].Select("ORDERNUM='" + drCustOrder["ORDERNUM"] + "' AND ORDERDETAILNUM='" + drCustOrder["LINENUM"] + "'"))
                                                {
                                                    dtOrderSubDetails.ImportRow(rowNew);

                                                }

                                                if (dtOrderSubDetails != null && dtOrderSubDetails.Rows.Count > 0)
                                                {
                                                    foreach (DataRow rowNew in dtOrderSubDetails.Rows)
                                                    {
                                                        drNew = rowNew;
                                                        nextCharNr = 1;
                                                        foreach (DataRow row in charPosTable.Select(lineTable.TableName + "_Id='" + drLineId + "'", "nr asc"))
                                                        {
                                                            try
                                                            {



                                                                itemInfo = new FormItemInfo(row);
                                                                // Adding possible whitespace at the beginning of line
                                                                lIneStringBuilder.Append(CreateWhitespace(itemInfo.CharIndex - nextCharNr, ' '));

                                                                if (itemInfo.FontStyle == System.Drawing.FontStyle.Bold)
                                                                {
                                                                    lIneStringBuilder.Append(esc + "|2C");
                                                                }
                                                                else
                                                                {
                                                                    lIneStringBuilder.Append(esc + "|1C");
                                                                }

                                                                // Parsing the itemInfo
                                                                //DataTable dtOrderSubDetails = null;
                                                                //if (dsCustOrder != null && dsCustOrder.Tables.Count > 1 && dsCustOrder.Tables[2].Rows.Count > 0)
                                                                //    dtOrderSubDetails = dsCustOrder.Tables[2];


                                                                lIneStringBuilder.Append(ParseItemVariable(itemInfo, null, null, drNew, null));


                                                                // Closing the string with a single space command to make sure spaces are always single spaced
                                                                lIneStringBuilder.Append(esc + "|1C");

                                                                // Specifing the position of the next char in the current line - bold take twice as much space
                                                                nextCharNr = itemInfo.CharIndex + (itemInfo.Length * itemInfo.SizeFactor);
                                                            }
                                                            catch (Exception)
                                                            {
                                                            }
                                                        }
                                                        lIneStringBuilder.Append(Environment.NewLine);

                                                    }
                                                }

                                                isDetailTable = false;
                                            }
                                            else
                                            {
                                                foreach (DataRow row in charPosTable.Select(lineTable.TableName + "_Id='" + drLineId + "'", "nr asc"))
                                                {
                                                    try
                                                    {
                                                        itemInfo = new FormItemInfo(row);
                                                        // Adding possible whitespace at the beginning of line
                                                        lIneStringBuilder.Append(CreateWhitespace(itemInfo.CharIndex - nextCharNr, ' '));

                                                        if (itemInfo.FontStyle == System.Drawing.FontStyle.Bold)
                                                        {
                                                            lIneStringBuilder.Append(esc + "|2C");
                                                        }
                                                        else
                                                        {
                                                            lIneStringBuilder.Append(esc + "|1C");
                                                        }

                                                        // Parsing the itemInfo
                                                        //DataTable dtOrderSubDetails = null;
                                                        //if (dsCustOrder != null && dsCustOrder.Tables.Count > 1 && dsCustOrder.Tables[2].Rows.Count > 0)
                                                        //    dtOrderSubDetails = dsCustOrder.Tables[2];


                                                        lIneStringBuilder.Append(ParseItemVariable(itemInfo, null, null, drCustOrder, null));

                                                        if (ParseItemVariable(itemInfo, null, null, drCustOrder, null).Contains("*****"))
                                                            isDetailTable = true;

                                                        // Closing the string with a single space command to make sure spaces are always single spaced
                                                        lIneStringBuilder.Append(esc + "|1C");

                                                        // Specifing the position of the next char in the current line - bold take twice as much space
                                                        nextCharNr = itemInfo.CharIndex + (itemInfo.Length * itemInfo.SizeFactor);
                                                    }
                                                    catch (Exception)
                                                    {
                                                    }
                                                }
                                            }

                                        }

                                        lIneStringBuilder.Append(Environment.NewLine);

                                    }

                                    break;
                                }
                        }
                    }
                }

            }
        }

        //Creates a string to print out for each of the items in the transaction. If 
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "GrandFather PS6015")]
        private string ReadItemDataSet(DataSet ds, RetailTransaction theTransaction, DataSet dt = null)
        {
            FormItemInfo itemInfo = null;
            StringBuilder lIneStringBuilder = new StringBuilder(string.Empty);
            System.Collections.ArrayList itemArray = null;
            ISaleLineItem tempItem = null;
            bool found = false;

            if (dt != null && dt.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Tables[1].Rows)
                    ParseSaleItem(ds, ref itemInfo, lIneStringBuilder, null, null, dr, dt);
            }
            else
            {
                if (LSRetailPosis.Settings.FunctionalityProfiles.Functions.AggregateItemsForPrinting == true)
                {
                    itemArray = new System.Collections.ArrayList();

                    foreach (ISaleLineItem item in theTransaction.SaleItems)
                    {
                        foreach (ISaleLineItem itemInArray in itemArray)
                        {
                            if (itemInArray.ItemId == item.ItemId)
                            {
                                //Check if any of the price related attributes of an item is not matching, then create a new line in receipt
                                if ((itemInArray.LineDiscount == item.LineDiscount)
                                    && (itemInArray.LinePctDiscount == item.LinePctDiscount)
                                    && (itemInArray.TotalDiscount == item.TotalDiscount)
                                    && (itemInArray.TotalPctDiscount == item.TotalPctDiscount)
                                    && (itemInArray.PeriodicPctDiscount == item.PeriodicPctDiscount)
                                    && (itemInArray.Price == item.Price))
                                {
                                    itemInArray.Quantity += item.Quantity;
                                    itemInArray.GrossAmount += item.GrossAmount;
                                    itemInArray.LineDiscount += item.LineDiscount;
                                    itemInArray.TotalDiscount += item.TotalDiscount;

                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            tempItem = Printing.InternalApplication.BusinessLogic.Utility.CreateSaleLineItem(
                                item, Printing.InternalApplication.Services.Rounding, theTransaction);
                            itemArray.Add(tempItem);
                        }

                        found = false;
                    }
                }


                if (LSRetailPosis.Settings.FunctionalityProfiles.Functions.AggregateItemsForPrinting)
                {
                    //Go through the sale items and parse each line
                    foreach (SaleLineItem saleLineItem in itemArray)
                    {
                        ParseSaleItem(ds, ref itemInfo, lIneStringBuilder, saleLineItem, theTransaction);
                    }
                }
                else
                {
                    //Go through the sale items and parse each line
                    if (theTransaction is IRetailTransaction)
                    {
                        foreach (SaleLineItem saleLineItem in theTransaction.SaleItems)
                        {
                            ParseSaleItem(ds, ref itemInfo, lIneStringBuilder, saleLineItem, theTransaction);
                        }
                    }
                }
            }
            return lIneStringBuilder.ToString();
        }

        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "GrandFather PS6015")]
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private string ReadDataset(DataSet ds, ITenderLineItem tenderItem, RetailTransaction theTransaction, DataTable dt = null)
        {
            String tempString = string.Empty;
            DataTable lineTable = ds.Tables["line"];
            DataTable charPosTable;

            FormItemInfo itemInfo = null;

            if (lineTable != null)
            {
                //foreach (DataRow dr in lineTable.Rows)
                foreach (DataRow dr in lineTable.Select(string.Empty, "nr asc"))
                {
                    string idVariable = Utility.ToString(dr["ID"]);
                    string nrVariable = Utility.ToString(dr["nr"]);
                    string drLineId = string.Empty;
                    switch (idVariable)
                    {
                        case "CRLF":
                            tempString += Environment.NewLine;
                            break;
                        case "Text":
                            drLineId = Utility.ToString(dr["line_id"]);
                            charPosTable = ds.Tables["charpos"];
                            if (charPosTable != null)
                            {
                                int nextCharNr = 1;

                                foreach (DataRow row in charPosTable.Select("line_id='" + drLineId + "'", "nr asc"))
                                {
                                    try
                                    {
                                        itemInfo = new FormItemInfo(row);
                                        // Adding possible whitespace at the beginning of line
                                        tempString += CreateWhitespace(itemInfo.CharIndex - nextCharNr, ' ');

                                        // Parsing the itemInfo
                                        if (itemInfo.FontStyle == System.Drawing.FontStyle.Bold)
                                        {
                                            tempString += esc + "|2C";
                                        }
                                        else
                                        {
                                            tempString += esc + "|1C";
                                        }

                                        tempString += ParseVariable(itemInfo, tenderItem, theTransaction, dt);

                                        // Closing the string with a single space command to make sure spaces are always single spaced
                                        tempString += esc + "|1C";

                                        // Specifing the position of the next char in the current line - bold take twice as much space
                                        nextCharNr = itemInfo.CharIndex + (itemInfo.Length * itemInfo.SizeFactor);
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }

                            tempString += Environment.NewLine;
                            break;

                        case "Taxes":
                            switch (Functions.CountryRegion)
                            {
                                case SupportedCountryRegion.IN:
                                    PopulateTaxSummaryForIndia(theTransaction);
                                    break;
                                default:
                                    break;
                            }

                            charPosTable = ds.Tables["charpos"];
                            drLineId = Utility.ToString(dr["line_id"]);
                            if (charPosTable != null)
                            {
                                foreach (ITaxItem taxItem in theTransaction.TaxLines)
                                {
                                    int nextCharNr = 1;
                                    foreach (DataRow row in charPosTable.Select(lineTable.TableName + "_Id='" + drLineId + "'", "nr asc"))
                                    {
                                        try
                                        {
                                            itemInfo = new FormItemInfo(row);
                                            // Adding possible whitespace at the beginning of line
                                            tempString += CreateWhitespace(itemInfo.CharIndex - nextCharNr, ' ');

                                            if (itemInfo.FontStyle == System.Drawing.FontStyle.Bold)
                                            {
                                                tempString += esc + "|2C";
                                            }
                                            else
                                            {
                                                tempString += esc + "|1C";
                                            }

                                            // Parsing the itemInfo
                                            tempString += ParseTaxVariable(itemInfo, taxItem);

                                            // Closing the string with a single space command to make sure spaces are always single spaced
                                            tempString += esc + "|1C";

                                            // Specifing the position of the next char in the current line - bold take twice as much space
                                            nextCharNr = itemInfo.CharIndex + (itemInfo.Length * itemInfo.SizeFactor);

                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }

                                    tempString += Environment.NewLine;
                                }
                            }
                            break;

                        case "LoyaltyItem":
                            if (theTransaction.LoyaltyItem != null)
                            {
                                if (theTransaction.LoyaltyItem.LoyaltyCardNumber != null)
                                {
                                    charPosTable = ds.Tables["charpos"];
                                    drLineId = Utility.ToString(dr["line_id"]);
                                    if (charPosTable != null)
                                    {
                                        int nextCharNr = 1;
                                        foreach (DataRow row in charPosTable.Select(lineTable.TableName + "_Id='" + drLineId + "'", "nr asc"))
                                        {
                                            try
                                            {
                                                itemInfo = new FormItemInfo(row);
                                                // Adding possible whitespace at the beginning of line
                                                tempString += CreateWhitespace(itemInfo.CharIndex - nextCharNr, ' ');

                                                if (itemInfo.FontStyle == System.Drawing.FontStyle.Bold)
                                                {
                                                    tempString += esc + "|2C";
                                                }
                                                else
                                                {
                                                    tempString += esc + "|1C";
                                                }

                                                // Parsing the itemInfo
                                                tempString += ParseVariable(itemInfo, tenderItem, theTransaction);

                                                // Closing the string with a single space command to make sure spaces are always single spaced
                                                tempString += esc + "|1C";

                                                // Specifing the position of the next char in the current line - bold take twice as much space
                                                nextCharNr = itemInfo.CharIndex + (itemInfo.Length * itemInfo.SizeFactor);

                                            }
                                            catch (Exception)
                                            {
                                            }
                                        }

                                        tempString += Environment.NewLine;
                                    }
                                }
                            }
                            break;

                        case "Tenders":
                            charPosTable = ds.Tables["charpos"];
                            drLineId = Utility.ToString(dr["line_id"]);

                            if (charPosTable != null)
                            {
                                System.Collections.Generic.LinkedList<TenderLineItem> tenderLines;
                                if (theTransaction is SalesInvoiceTransaction)
                                {
                                    if (dt != null && dt.Rows.Count > 0)
                                        tenderLines = null;
                                    else
                                        tenderLines = ((SalesInvoiceTransaction)theTransaction).TenderLines;
                                }
                                else if (theTransaction is SalesOrderTransaction)
                                {
                                    if (dt != null && dt.Rows.Count > 0)
                                        tenderLines = null;
                                    else
                                        tenderLines = ((SalesOrderTransaction)theTransaction).TenderLines;
                                }
                                else
                                {
                                    if (dt != null && dt.Rows.Count > 0)
                                        tenderLines = null;
                                    else
                                        tenderLines = theTransaction.TenderLines;
                                }
                                if (tenderLines != null)
                                {
                                    foreach (ITenderLineItem tenderLineItem in tenderLines)
                                    {
                                        if (tenderLineItem.Voided == false)
                                        {
                                            int nextCharNr = 1;
                                            foreach (DataRow row in charPosTable.Select(lineTable.TableName + "_Id='" + drLineId + "'", "nr asc"))
                                            {
                                                try
                                                {
                                                    itemInfo = new FormItemInfo(row);
                                                    // If tender is a Change Back tender, then a carrage return is put in front of the next line
                                                    if ((tenderLineItem.Amount < 0) && (nextCharNr == 1))
                                                    {
                                                        tempString += Environment.NewLine;
                                                    }

                                                    // Adding possible whitespace at the beginning of line
                                                    tempString += CreateWhitespace(itemInfo.CharIndex - nextCharNr, ' ');

                                                    if (itemInfo.FontStyle == System.Drawing.FontStyle.Bold)
                                                    {
                                                        tempString += esc + "|2C";
                                                    }
                                                    else
                                                    {
                                                        tempString += esc + "|1C";
                                                    }

                                                    // Parsing the itemInfo
                                                    tempString += ParseTenderLineVariable(itemInfo, tenderLineItem, theTransaction);

                                                    // Closing the string with a single space command to make sure spaces are always single spaced
                                                    tempString += esc + "|1C";

                                                    // Specifing the position of the next char in the current line - bold take twice as much space
                                                    nextCharNr = itemInfo.CharIndex + (itemInfo.Length * itemInfo.SizeFactor);
                                                }
                                                catch (Exception)
                                                {
                                                }
                                            }

                                            tempString += Environment.NewLine;
                                        }
                                    }
                                }
                            }
                            break;

                        case "Tender":
                            charPosTable = ds.Tables["charpos"];
                            drLineId = Utility.ToString(dr["line_id"]);
                            if (charPosTable != null)
                            {
                                int nextCharNr = 1;
                                foreach (DataRow row in charPosTable.Select(lineTable.TableName + "_Id='" + drLineId + "'", "nr asc"))
                                {
                                    try
                                    {
                                        itemInfo = new FormItemInfo(row);
                                        // If tender is a Change Back tender, then a carrage return is put in front of the next line
                                        if ((tenderItem.Amount < 0) && (nextCharNr == 1))
                                        {
                                            tempString += Environment.NewLine;
                                        }

                                        // Adding possible whitespace at the beginning of line
                                        tempString += CreateWhitespace(itemInfo.CharIndex - nextCharNr, ' ');

                                        if (itemInfo.FontStyle == System.Drawing.FontStyle.Bold)
                                        {
                                            tempString += esc + "|2C";
                                        }
                                        else
                                        {
                                            tempString += esc + "|1C";
                                        }

                                        // Parsing the itemInfo
                                        tempString += ParseTenderLineVariable(itemInfo, tenderItem, theTransaction);

                                        // Closing the string with a single space command to make sure spaces are always single spaced
                                        tempString += esc + "|1C";

                                        // Specifing the position of the next char in the current line - bold take twice as much space
                                        nextCharNr = itemInfo.CharIndex + (itemInfo.Length * itemInfo.SizeFactor);
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }

                                tempString += Environment.NewLine;
                            }
                            break;

                        case "TenderRounding":
                            if (theTransaction.TransSalePmtDiff == 0)
                            {
                                break;
                            }

                            charPosTable = ds.Tables["charpos"];
                            drLineId = Utility.ToString(dr["line_id"]);
                            if (charPosTable != null)
                            {
                                int nextCharNr = 1;

                                foreach (DataRow row in charPosTable.Select("line_id='" + drLineId + "'", "nr asc"))
                                {
                                    try
                                    {
                                        itemInfo = new FormItemInfo(row);
                                        // Adding possible whitespace at the beginning of line
                                        tempString += CreateWhitespace(itemInfo.CharIndex - nextCharNr, ' ');

                                        // Parsing the itemInfo
                                        if (itemInfo.FontStyle == System.Drawing.FontStyle.Bold)
                                        {
                                            tempString += esc + "|2C";
                                        }
                                        else
                                        {
                                            tempString += esc + "|1C";
                                        }

                                        tempString += ParseVariable(itemInfo, tenderItem, theTransaction);

                                        // Closing the string with a single space command to make sure spaces are always single spaced
                                        tempString += esc + "|1C";

                                        // Specifing the position of the next char in the current line - bold take twice as much space
                                        nextCharNr = itemInfo.CharIndex + (itemInfo.Length * itemInfo.SizeFactor);
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }

                            tempString += Environment.NewLine;
                            break;
                    }
                }
            }

            return tempString.ToString();

        }

        private static void PopulateTaxSummaryForIndia(RetailTransaction theTransaction)
        {
            if (ApplicationSettings.Terminal.IndiaTaxDetailsType == Terminal.IndiaReceiptTaxDetailsType.PerTaxComponent)
            {
                IList<TaxItemIndia> indiaTaxItems = new List<TaxItemIndia>();
                foreach (ISaleLineItem item in theTransaction.SaleItems)
                {
                    foreach (TaxItem taxItem in item.TaxLines)
                    {
                        indiaTaxItems.Add(taxItem as TaxItemIndia);
                    }
                }

                if (ApplicationSettings.Terminal.ShowIndiaTaxOnTax)
                {
                    theTransaction.TaxLines = BuildIndiaTaxSummaryPerComponent1(indiaTaxItems);
                }
                else
                {
                    theTransaction.TaxLines = BuildIndiaTaxSummaryPerComponent2(indiaTaxItems);
                }
            }
            else if (ApplicationSettings.Terminal.IndiaTaxDetailsType == Terminal.IndiaReceiptTaxDetailsType.PerLine)
            {
                theTransaction.TaxLines = BuildIndiaTaxSummaryPerLine(theTransaction);
            }
        }

        /// <summary>
        /// Build tax summary lines of the India receipt, with tax amounts be aggregated by sale line items.
        /// </summary>
        /// <param name="theTransaction">The retail transaction.</param>
        /// <returns>The tax summary lines of the India receipt.</returns>
        /// <remarks>
        /// In this case, the settings of "RetailStoreTable > Misc > Receipts" is as follows,
        /// 1) The "Tax details" option is set as "Per line"
        /// 2) The "Show tax on tax" option is set as "N/A", as it is disabled in this case
        /// 
        /// For example, the retail transaction has four sale line items, as follows,
        /// Item ID | Price | Tax code | Formula       | Tax basis | Tax rate | Tax amount
        /// 0001    | 100   | SERV5    | Line amount   | 100.00    |  5%      |  5.00
        ///         |       | E-CSS5   | Excl.+[SERV5] |   5.00    |  5%      |  0.25
        /// 0002    | 100   | VAT10    | Line amount   | 100.00    | 10%      | 10.00
        ///         |       | Surchg2  | Excl.+[VAT10] |  10.00    |  2%      |  0.20
        /// 0003    | 100   | SERV4    | Line amount   | 100.00    |  4%      |  4.00
        ///         |       | E-CSS5   | Excl.+[SERV4] |   4.00    |  5%      |  0.20
        /// 0004    | 100   | VAT12    | Line amount   | 100.00    | 12%      | 12.00
        ///         |       | Surchg2  | Excl.+[VAT12] |  12.00    |  2%      |  0.24
        /// 
        /// And the tax summary lines will be as follows,
        /// Tax code | Tax basis | Tax rate | Tax amount
        /// AA       | 100.00    |  5.25%   |  5.25
        /// AB       | 100.00    | 10.20%   | 10.20
        /// AC       | 100.00    |  4.20%   |  4.20
        /// AD       | 100.00    | 12.24%   | 12.24
        /// 
        /// Tax codes are automatically named from "AA" to "AZ", ...
        /// </remarks>
        private static LinkedList<TaxItem> BuildIndiaTaxSummaryPerLine(RetailTransaction theTransaction)
        {
            LinkedList<TaxItem> lines = new LinkedList<TaxItem>();

            char code1 = 'A', code2 = 'A';
            foreach (ISaleLineItem saleItem in theTransaction.SaleItems)
            {
                TaxItemIndia t = new TaxItemIndia();
                t.TaxCode = "" + code1 + code2;
                t.TaxBasis = saleItem.TaxLines.First(x => !(x as TaxItemIndia).IsTaxOnTax).TaxBasis;
                t.Amount = saleItem.TaxLines.Sum(x => x.Amount);
                t.Percentage = t.TaxBasis != decimal.Zero ? 100 * t.Amount / t.TaxBasis : decimal.Zero;

                lines.AddLast(t);

                // Generate tax code of the next line
                code2++;
                if (code2 > 'Z')
                {
                    code2 = 'A';
                    code1++;

                    if (code1 > 'Z')
                    {
                        code1 = 'A';
                    }
                }
            }

            return lines;
        }

        /// <summary>
        /// Build tax summary line of the India receipt, with tax amounts be aggregated by "main" tax codes (which
        /// are not India tax on tax codes).
        /// </summary>
        /// <param name="indiaTaxItems">All tax items of the India retail transaction.</param>
        /// <returns>The tax summary lines of the India receipt.</returns>
        /// <remarks>
        /// In this case, the settings of "RetailStoreTable > Misc > Receipts" is as follows,
        /// 1) The "Tax details" option is set as "Per tax component"
        /// 2) The "Show tax on tax" option is turned OFF
        /// 
        /// For example, the retail transaction has four sale line items, as follows,
        /// Item ID | Price | Tax code | Formula       | Tax basis | Tax rate | Tax amount
        /// 0001    | 100   | SERV5    | Line amount   | 100.00    |  5%      |  5.00
        ///         |       | E-CSS5   | Excl.+[SERV5] |   5.00    |  5%      |  0.25
        /// 0002    | 100   | VAT10    | Line amount   | 100.00    | 10%      | 10.00
        ///         |       | Surchg2  | Excl.+[VAT10] |  10.00    |  2%      |  0.20
        /// 0003    | 100   | SERV4    | Line amount   | 100.00    |  4%      |  4.00
        ///         |       | E-CSS5   | Excl.+[SERV4] |   4.00    |  5%      |  0.20
        /// 0004    | 100   | VAT12    | Line amount   | 100.00    | 12%      | 12.00
        ///         |       | Surchg2  | Excl.+[VAT12] |  12.00    |  2%      |  0.24
        /// 
        /// And the tax summary lines will be as follows,
        /// Tax code | Tax basis | Tax rate | Tax amount
        /// SERV5    | 100.00    |  5.25%   |  5.25
        /// SERV4    | 100.00    |  4.20%   |  4.20
        /// VAT10    | 100.00    | 10.20%   | 10.20
        /// VAT12    | 100.00    | 12.24%   | 12.24
        /// </remarks>
        private static LinkedList<TaxItem> BuildIndiaTaxSummaryPerComponent2(IList<TaxItemIndia> indiaTaxItems)
        {
            LinkedList<TaxItem> lines = new LinkedList<TaxItem>();

            var groups = indiaTaxItems.GroupBy(x =>
            {
                string taxCode = !x.IsTaxOnTax ?
                    x.TaxCode :
                    x.TaxCodesInFormula.First();

                return new { x.TaxGroup, taxCode };
            });
            foreach (var group in groups)
            {
                TaxItemIndia t = new TaxItemIndia();
                t.TaxGroup = group.Key.TaxGroup;
                t.TaxCode = group.Key.taxCode;
                t.TaxBasis = group.First(x => !x.IsTaxOnTax).TaxBasis;
                t.Amount = group.Sum(x => x.Amount);
                t.Percentage = t.TaxBasis != decimal.Zero ? 100 * t.Amount / t.TaxBasis : decimal.Zero;

                lines.AddLast(t);
            }

            // Order by tax component
            lines = new LinkedList<TaxItem>(lines.OrderBy(x => (x as TaxItemIndia).TaxComponent));

            return lines;
        }

        /// <summary>
        /// Build tax summary line of the India receipt, with tax amounts be aggregated by tax codes.
        /// </summary>
        /// <param name="indiaTaxItems">All tax items of the India retail transaction.</param>
        /// <returns>The tax summary lines of the India receipt.</returns>
        /// <remarks>
        /// In this case, the settings of "RetailStoreTable > Misc > Receipts" is as follows,
        /// 1) The "Tax details" option is set as "Per tax component"
        /// 2) The "Show tax on tax" option is turned ON
        /// 
        /// For example, the retail transaction has four sale line items, as follows,
        /// Item ID | Price | Tax code | Formula       | Tax basis | Tax rate | Tax amount
        /// 0001    | 100   | SERV5    | Line amount   | 100.00    |  5%      |  5.00
        ///         |       | E-CSS5   | Excl.+[SERV5] |   5.00    |  5%      |  0.25
        /// 0002    | 100   | VAT10    | Line amount   | 100.00    | 10%      | 10.00
        ///         |       | Surchg2  | Excl.+[VAT10] |  10.00    |  2%      |  0.20
        /// 0003    | 100   | SERV4    | Line amount   | 100.00    |  4%      |  4.00
        ///         |       | E-CSS5   | Excl.+[SERV4] |   4.00    |  5%      |  0.20
        /// 0004    | 100   | VAT12    | Line amount   | 100.00    | 12%      | 12.00
        ///         |       | Surchg2  | Excl.+[VAT12] |  12.00    |  2%      |  0.24
        /// 
        /// And the tax summary lines will be as follows,
        /// Tax component | Tax code | Tax basis | Tax rate | Tax amount
        /// Service       | SERV5    | 100.00    |  5%      |  5.00
        /// Service       | SERV4    | 100.00    |  4%      |  4.00
        /// E-CSS         | E-CSS5   |   9.00    |  5%      |  0.45
        /// VAT           | VAT10    | 100.00    | 10%      | 10.00
        /// VAT           | VAT12    | 100.00    | 12%      | 12.00
        /// Surcharge     | Surchg2  |  22.00    |  2%      |  0.44
        /// </remarks>
        private static LinkedList<TaxItem> BuildIndiaTaxSummaryPerComponent1(IList<TaxItemIndia> indiaTaxItems)
        {
            LinkedList<TaxItem> lines = new LinkedList<TaxItem>();

            var groups = indiaTaxItems.GroupBy(x => new { x.TaxComponent, x.TaxCode });
            foreach (var group in groups)
            {
                TaxItemIndia t = new TaxItemIndia();
                t.TaxComponent = group.Key.TaxComponent;
                t.TaxCode = group.Key.TaxCode;
                t.Amount = group.Sum(x => x.Amount);
                t.Percentage = group.First().Percentage;
                t.TaxBasis = group.Sum(x => x.TaxBasis);
                t.TaxGroup = group.First().TaxGroup;

                lines.AddLast(t);
            }

            // Order by tax component
            lines = new LinkedList<TaxItem>(lines.OrderBy(x => (x as TaxItemIndia).TaxComponent));

            return lines;
        }

        private static string Wrap(string text, FormItemInfo itemInfo)
        {
            // Define our regex
            string pattern = @"(?<Line>.{1," + itemInfo.Length + @"})(?:\W)";

            // Split the string based on the pattern
            string[] lines = Regex.Split(text, pattern, RegexOptions.IgnoreCase
                | RegexOptions.IgnorePatternWhitespace
                | RegexOptions.Multiline
                | RegexOptions.ExplicitCapture
                | RegexOptions.CultureInvariant
                | RegexOptions.Compiled);

            // Empty string to return value
            string returnValue = string.Empty;

            // Concacitante all lines adding a hard return on each line
            foreach (string line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    returnValue += GetAlignmentSettings(line.Trim(), itemInfo) + Environment.NewLine;
                }
            }

            // No NewLine at the end of the string.
            return returnValue.TrimEnd(Environment.NewLine.ToCharArray());
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Grandfather")]
        private static DataSet ConvertToDataSet(string templateXml, DataRow dataRow)
        {
            DataSet template = null;

            if (templateXml.Length > 0)
            {
                template = new DataSet("New DataSet");
                byte[] buffer = null;
                int discarded;
                buffer = LSRetailPosis.DataAccess.DataUtil.HexEncoding.GetBytes(templateXml, out discarded);

                if (buffer != null)
                {
                    using (System.IO.MemoryStream myStream = new System.IO.MemoryStream())
                    {
                        myStream.Write(buffer, 0, buffer.Length);
                        myStream.Position = 0;

                        template.ReadXml(myStream);
                    }
                }

                // Adding detail table to the dataset
                DataTable formDetails = new DataTable();
                formDetails.TableName = "FORMDETAILS";

                // Adding columns to items data
                DataColumn col = new DataColumn("ID", Type.GetType("System.String"));
                formDetails.Columns.Add(col);
                col = new DataColumn("TITLE", Type.GetType("System.String"));
                formDetails.Columns.Add(col);
                col = new DataColumn("DESCRIPTION", Type.GetType("System.String"));
                formDetails.Columns.Add(col);
                col = new DataColumn("UPPERCASE", Type.GetType("System.Boolean"));
                formDetails.Columns.Add(col);

                Object[] row = new Object[4];

                row[0] = dataRow["FORMLAYOUTID"];
                row[1] = dataRow["TITLE"].ToString();
                row[2] = dataRow["DESCRIPTION"].ToString();
                row[3] = dataRow["UPPERCASE"].ToString() == "1";

                formDetails.Rows.Add(row);
                template.Tables.Add(formDetails);
            }

            return template;
        }

        /// <summary>
        /// Returns transformed card tender as string.
        /// </summary>
        /// <param name="formNumber"></param>
        /// <param name="EFTInfo"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public string GetTransformedCardTender(FormInfo formInfo, IEFTInfo EFTInfo, RetailTransaction theTransaction)
        {
            if (formInfo == null)
            {
                throw new ArgumentNullException("formInfo");
            }

            StringBuilder returnString = new StringBuilder();
            DataSet ds = null;

            // Getting a dataset containing the headerpart of the current form
            ds = formInfo.HeaderTemplate;
            returnString.Append(ReadCardTenderDataSet(ds, EFTInfo, theTransaction));

            // Getting a dataset containing the footerpart of the current form
            ds = formInfo.FooterTemplate;
            returnString.Append(ReadCardTenderDataSet(ds, EFTInfo, theTransaction));

            // further modification of the string
            DataTable formDetails = ds.Tables["FORMDETAILS"];
            if (formDetails != null)
            {
                DataRow detailRow = formDetails.Rows[0];
                {
                    if (Convert.ToBoolean(detailRow["UPPERCASE"]) == true)
                    {
                        string tempstring = returnString.ToString();
                        return tempstring.ToUpper();
                    }
                }
            }

            return returnString.ToString();
        }

        /// <summary>
        /// Returns transformed tender data as string.
        /// </summary>
        /// <param name="formNumber"></param>
        /// <param name="tenderLineItem"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Changing public API will break backwards compatibility.")]
        public string GetTransformedTender(FormInfo formInfo, TenderLineItem tenderLineItem, RetailTransaction theTransaction)
        {
            if (formInfo == null)
            {
                throw new ArgumentNullException("formInfo");
            }

            StringBuilder returnString = new StringBuilder();
            DataSet ds = null;

            // Getting a dataset containing the headerpart of the current form
            ds = formInfo.HeaderTemplate;
            returnString.Append(ReadTenderDataSet(ds, tenderLineItem, theTransaction));

            // Getting a dataset containing the footerpart of the current form
            ds = formInfo.FooterTemplate;
            returnString.Append(ReadTenderDataSet(ds, tenderLineItem, theTransaction));

            // further modification of the string
            DataTable formDetails = ds.Tables["FORMDETAILS"];
            if (formDetails != null)
            {
                DataRow detailRow = formDetails.Rows[0];
                {
                    if (Convert.ToBoolean(detailRow["UPPERCASE"]) == true)
                    {
                        string tempstring = returnString.ToString();
                        return tempstring.ToUpper();
                    }
                }
            }

            return returnString.ToString();
        }

        /// <summary>
        /// Gets transformed transaction details as per given form number.
        /// </summary>
        /// <param name="formInfo"></param>
        /// <param name="trans"></param>
        public void GetTransformedTransaction(FormInfo formInfo, RetailTransaction theTransaction, DataSet dt = null)
        {
            if (formInfo == null)
            {
                throw new ArgumentNullException("formInfo");
            }

            try
            {
                DataSet ds = null;

                if (LSRetailPosis.Settings.ApplicationSettings.Terminal.TrainingMode == true)
                {
                    formInfo.Header = ApplicationLocalizer.Language.Translate(true, null, 13042)
                        + Environment.NewLine + "***********************************" + Environment.NewLine;
                }

                if (formInfo.Reprint)
                {
                    copyText = ApplicationLocalizer.Language.Translate(true, null, 13039);
                }
                else
                {
                    copyText = string.Empty;
                }

                // Getting a dataset containing the headerpart of the current form
                ds = formInfo.HeaderTemplate;
                if (dt != null && dt.Tables[0].Rows.Count > 0)
                    formInfo.Header += ReadDataset(ds, null, theTransaction, dt.Tables[0]);
                else
                    formInfo.Header += ReadDataset(ds, null, theTransaction);
                formInfo.HeaderLines = ds.Tables[0].Rows.Count;

                // Getting a dataset containing the linepart of the current form
                ds = formInfo.DetailsTemplate;
                if (dt != null && dt.Tables[1].Rows.Count > 0)
                    formInfo.Details = ReadItemDataSet(ds, theTransaction, dt);
                else
                    formInfo.Details = ReadItemDataSet(ds, theTransaction);
                formInfo.DetailLines = ds.Tables[0].Rows.Count;

                // Getting a dataset containing the footerpart of the current form
                ds = formInfo.FooterTemplate;
                if (dt != null && dt.Tables[0].Rows.Count > 0)
                    formInfo.Footer = ReadDataset(ds, null, theTransaction, dt.Tables[0]);
                else
                    formInfo.Footer = ReadDataset(ds, null, theTransaction);
                formInfo.FooterLines = ds.Tables[0].Rows.Count;

                if (LSRetailPosis.Settings.ApplicationSettings.Terminal.TrainingMode == true)
                {
                    formInfo.Footer = Environment.NewLine + "***********************************"
                        + Environment.NewLine + ApplicationLocalizer.Language.Translate(true, null, 13042);
                }

                // further modification of the string
                DataTable formDetails = ds.Tables["FORMDETAILS"];
                if (formDetails != null)
                {
                    DataRow detailRow = formDetails.Rows[0];
                    {
                        if (Convert.ToBoolean(detailRow["UPPERCASE"]) == true)
                        {
                            formInfo.Header = formInfo.Header.ToUpper();
                            formInfo.Details = formInfo.Details.ToUpper();
                            formInfo.Footer = formInfo.Footer.ToUpper();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NetTracer.Warning("Printing [FormModulation] - Exception when trying to get transformed transcation");

                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        /// <summary>
        /// Gets form information as per given form id.
        /// </summary>
        /// <param name="formId">The form id.</param>
        /// <param name="copyReceipt">if set to <c>true</c> [copy receipt].</param>
        /// <param name="receiptProfileId">The receipt profile id.</param>
        /// <returns></returns>
        public FormInfo GetInfoForForm(FormType formId, bool copyReceipt, string receiptProfileId)
        {
            FormInfo formInfo = new FormInfo();
            string layoutId = receiptData.GetFormLayoutId(formId, receiptProfileId);
            if (formId != FormType.Unknown)
                layoutId = receiptData.GetFormLayoutId(formId, receiptProfileId);
            else
                layoutId = "33";
            formInfo.Reprint = copyReceipt;
            try
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT * FROM dbo.RETAILFORMLAYOUT WHERE FORMLAYOUTID = @FORMID";
                    command.Parameters.Add("@FORMID", SqlDbType.NVarChar).Value = layoutId;

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    using (System.Data.SqlClient.SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(command))
                    {
                        using (System.Data.DataTable table = new System.Data.DataTable())
                        {
                            adapter.Fill(table);

                            if (table.Rows.Count > 0)
                            {
                                if (table.Rows[0]["PRINTASSLIP"] != DBNull.Value)
                                {
                                    formInfo.PrintAsSlip = (1 == Convert.ToInt32(table.Rows[0]["PRINTASSLIP"].ToString()));
                                }

                                //int isSlip = Convert.ToInt32(table.Rows[0]["PrintAsSlip"].ToString());
                                if (table.Rows[0]["PRINTBEHAVIOUR"] != DBNull.Value)
                                {
                                    formInfo.PrintBehaviour = Convert.ToInt32(table.Rows[0]["PRINTBEHAVIOUR"].ToString());
                                }

                                if (table.Rows[0]["HEADERXML"] != DBNull.Value)
                                {
                                    formInfo.HeaderTemplate = ConvertToDataSet(table.Rows[0]["HEADERXML"].ToString(), table.Rows[0]);
                                }

                                if (table.Rows[0]["LINESXML"] != DBNull.Value)
                                {
                                    formInfo.DetailsTemplate = ConvertToDataSet(table.Rows[0]["LINESXML"].ToString(), table.Rows[0]);
                                }

                                if (table.Rows[0]["FOOTERXML"] != DBNull.Value)
                                {
                                    formInfo.FooterTemplate = ConvertToDataSet(table.Rows[0]["FOOTERXML"].ToString(), table.Rows[0]);
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return formInfo;
        }
    }
}
