/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using LSRetailPosis;
using LSRetailPosis.DataAccess;
using LSRetailPosis.DevUtilities;
using LSRetailPosis.POSProcesses;
using LSRetailPosis.Settings;
using LSRetailPosis.Settings.FunctionalityProfiles;
using LSRetailPosis.Transaction;
using Microsoft.Dynamics.Retail.Diagnostics;
using Microsoft.Dynamics.Retail.Notification.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.BusinessLogic;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using Microsoft.Dynamics.Retail.Pos.Contracts.Triggers;
using Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using DE = Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using DM = Microsoft.Dynamics.Retail.Pos.DataManager;

namespace Microsoft.Dynamics.Retail.Pos.Customer
{
    /// <summary>
    /// AX Sales Order Status
    /// </summary>
    internal enum SalesOrderStatus
    {
        None = 0,
        Backorder,
        Delivered,
        Invoiced,
        Canceled
    }

    [Export(typeof(ICustomer))]
    public class Customer : ICustomer
    {
        // Get all text through the Translation function in the ApplicationLocalizer
        // TextID's for Customer project are reserved at 51000 - 51199
        // This class 51000 - 51019, now used 51007

        private IApplication application;

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

        internal static IApplication InternalApplication { get; private set; }

        private ICustomerSystem CustomerSystem
        {
            get { return this.Application.BusinessLogic.CustomerSystem; }
        }

        /// <summary>
        /// Enter the customer id and add the customer to the transaction
        /// </summary>
        /// <param name="retailTransaction">The retail tranaction</param>
        /// <returns>The retail tranaction</returns>
        public void EnterCustomerId(DE.IRetailTransaction retailTransaction)
        {
        }

        /// <summary>
        /// Search for the customer and add to the retailtransaction
        /// </summary>
        /// <param name="retailTransaction">The retail tranaction</param>
        public void Search(DE.IPosTransaction posTransaction)
        {
            // Show the search dialog
            DE.ICustomer customer = this.Search();

            AddCustomerToTransaction(customer, posTransaction);
        }

        public DE.ICustomer AddNew(out string CustId, out string CustName, out string CustCurrency)
        {
            DE.ICustomer customer = null;
            DE.IAddress address = null;

            if (Application != null)
            {
                CustomerData custData = new CustomerData(Application.Settings.Database.Connection, Application.Settings.Database.DataAreaID);
            }
            else
            {
                CustomerData custData = new CustomerData(LSRetailPosis.Settings.ApplicationSettings.Database.LocalConnection, LSRetailPosis.Settings.ApplicationSettings.Database.DATAAREAID);
            }

            //CustomerData custData = new CustomerData(Application.Settings.Database.Connection, Application.Settings.Database.DataAreaID);

            customer = GetNewCustomerDefaults();
            address = GetNewAddressDefaults(customer);
            using (frmNewCustomer newCustDialog = new frmNewCustomer(customer, address))
            {

                if (Application != null)
                {
                    this.Application.ApplicationFramework.POSShowForm(newCustDialog);
                    // customer = newCustDialog.Customer;
                    //address = newCustDialog.Address;
                }
                else
                {
                    Customer.InternalApplication.ApplicationFramework.POSShowForm(newCustDialog);

                    customer = newCustDialog.Customer;
                    CustId = customer.CustomerId;
                    CustName = customer.Name;
                    CustCurrency = customer.Currency;
                    address = newCustDialog.Address;
                }

                // this.Application.ApplicationFramework.POSShowForm(newCustDialog);

                if (newCustDialog.DialogResult == DialogResult.OK)
                {
                    customer = newCustDialog.Customer;
                    CustId = customer.CustomerId;
                    CustName = customer.Name;
                    CustCurrency = customer.Currency;
                    address = newCustDialog.Address;
                }
            }
            CustName = customer.Name;
            CustId = customer.CustomerId;
            CustCurrency = customer.Currency;
            return customer;
        }

        /// <summary>
        /// Add customer to transaction
        /// </summary>
        /// <param name="customer">Customer to add</param>
        /// <param name="posTransaction">Transaction</param>
        public void AddCustomerToTransaction(DE.ICustomer customer, DE.IPosTransaction posTransaction)
        {
            // !! Note - this code should follow the same steps to set the customer as PosProcesses\Customer.cs :: Execute()
            //Get information about the selected customer and add it to the transaction
            if (customer != null)
            {
                SalesOrderTransaction soTransaction = posTransaction as SalesOrderTransaction;
                if (soTransaction != null)
                {
                    // Must check for ISalesOrderTransaction before IRetailTransaction because it derives from IRetailTransaction
                    soTransaction.Customer = customer as LSRetailPosis.Transaction.Customer;
                }
                else
                {
                    RetailTransaction asRetailTransaction = posTransaction as RetailTransaction;
                    if (asRetailTransaction != null)
                    {
                        if (!asRetailTransaction.IsCustomerAllowed())
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSMessageDialog(4544);
                            return;
                        }

                        DE.ICustomer invoicedCustomer = customer;
                        string invoiceAccount = customer.InvoiceAccount;

                        //If the customer has another account as invoice account
                        if (!string.IsNullOrWhiteSpace(invoiceAccount))
                        {
                            invoicedCustomer = this.CustomerSystem.GetCustomerInfo(invoiceAccount);
                        }

                        // Trigger: PreCustomerSet trigger for the operation
                        var preTriggerResult = new PreTriggerResult();

                        PosApplication.Instance.Triggers.Invoke<ICustomerTrigger>(t => t.PreCustomerSet(preTriggerResult, posTransaction, customer.CustomerId));

                        if (!TriggerHelpers.ProcessPreTriggerResults(preTriggerResult))
                        {
                            return;
                        }

                        this.CustomerSystem.SetCustomer(asRetailTransaction, customer, invoicedCustomer);

                        //If CheckCustomer returns false then the customer isn't allowed to be added to the transaction. Msg has already been displayed
                        if (!CheckCustomer(posTransaction))
                        {
                            return;
                        }

                        //If CheckInvoicedCustomer removed the customer then it isn't allowed to be added to the transaction. Msg has already been displayed
                        if (!CheckInvoicedCustomer(posTransaction))
                        {
                            return;
                        }

                        if (asRetailTransaction.Customer.UsePurchRequest)
                        {
                            asRetailTransaction.CustomerPurchRequestId = GetPurchRequestId();
                        }
                    }
                    else if (posTransaction is CustomerPaymentTransaction)
                    {
                        // Customer is not allowed to be changed  (or cleared) once a customer account deposit has been made.
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSMessageDialog(3084);
                    }
                }
            }
            else
            {
                NetTracer.Warning("Customer::AddCustomerToTransaction: customer parameter is null");
            }
        }
        
        /// <summary>
        /// Search for the address and add to the retailtransaction
        /// </summary>
        /// <param name="retailTransaction">The retail tranaction</param>
        /// <returns>The retail tranaction</returns>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Grandfathered")]
        public void SearchShippingAddress(DE.IPosTransaction posTransaction)
        {
            RetailTransaction retailTransaction = (RetailTransaction)posTransaction;

            string shippingname = string.Empty;
            string shippingaddress = string.Empty;

            DM.CustomerDataManager customerDataManager = new DM.CustomerDataManager(Application.Settings.Database.Connection, Application.Settings.Database.DataAreaID);
            DE.IAddress address = null;
            if (customerDataManager.HasAddress(retailTransaction.Customer.PartyId))
            {
                address = SearchShippingAddress(retailTransaction.Customer);
            }
            else
            {
                // Create and add customer in AX
                address = AddNewShippingAddress(retailTransaction.Customer);
            }

            if (address != null)
            {
                Customer.InternalApplication.BusinessLogic.CustomerSystem.SetShippingAddress(retailTransaction, address);
            }
        }

        public DE.IAddress SearchShippingAddress(DE.ICustomer customer)
        {
            DE.IAddress address = null;

            using (frmShippingAddressSearch searchShippingAddressDialog = new frmShippingAddressSearch(customer))
            {
                this.Application.ApplicationFramework.POSShowForm(searchShippingAddressDialog);

                if (searchShippingAddressDialog.DialogResult == DialogResult.OK)
                {
                    address = searchShippingAddressDialog.SelectedAddress;
                }
            }

            return address;
        }

        public static DE.IAddress AddNewShippingAddress(DE.ICustomer customer)
        {
            DE.IAddress address = null;

            address = GetNewAddressDefaults(customer);
            using (frmNewShippingAddress dlg = new frmNewShippingAddress(customer, address))
            {
                InternalApplication.ApplicationFramework.POSShowForm(dlg);

                if (dlg.DialogResult == DialogResult.OK)
                {
                    address = dlg.Address;
                }
            }

            return address;
        }

        /// <summary>
        /// Invoke the 'Add Shipping Address' dialog to edit an existing address
        /// </summary>
        /// <param name="addressRecId">address rec id</param>
        /// <param name="existingCustomer">customer that this address is to be associated with</param>
        internal static DE.IAddress EditShippingAddress(long addressRecId, DE.ICustomer existingCustomer)
        {
            DM.CustomerDataManager customerDataManager = new DM.CustomerDataManager(
                ApplicationSettings.Database.LocalConnection, 
                ApplicationSettings.Database.DATAAREAID);

            DE.IAddress address = customerDataManager.GetAddress(addressRecId);

            using (frmNewShippingAddress dlg = new frmNewShippingAddress(existingCustomer, address))
            {
                InternalApplication.ApplicationFramework.POSShowForm(dlg);

                if (dlg.DialogResult == DialogResult.OK)
                {
                    address = dlg.Address;
                }
            }
            return address;
        }

        /// <summary>
        /// Prettier concrete wrapper around call to customer system to get blank customer
        /// </summary>
        /// <returns>Newly initialized blank customer</returns>
        private static LSRetailPosis.Transaction.Customer GetBlankCustomer()
        {
            return (LSRetailPosis.Transaction.Customer)InternalApplication.BusinessLogic.CustomerSystem.GetEmptyCustomer();
        }

        private static bool CheckCustomer(DE.IPosTransaction posTransaction)
        {
            RetailTransaction retailTransaction = (RetailTransaction)posTransaction;

            if (retailTransaction.Customer.Blocked == DE.BlockedEnum.All)
            {
                //Display a message
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(51002)) //This customer has been blocked. No sales or transactions are allowed.
                {
                    Customer.InternalApplication.ApplicationFramework.POSShowForm(dialog);
                }

                //Cancel the customer account
                retailTransaction.Customer = GetBlankCustomer();

                return false;
            }

            if (retailTransaction.Customer.Blocked == DE.BlockedEnum.Invoice)
            {
                //Display message
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(51003)) //This customer has been blocked. This account can not be charged to.
                {
                    Customer.InternalApplication.ApplicationFramework.POSShowForm(dialog);
                }
            }

            return true;
        }

        private static bool CheckInvoicedCustomer(DE.IPosTransaction posTransaction)
        {
            RetailTransaction retailTransaction = (RetailTransaction)posTransaction;

            //If the Invoiced customer has All as blocked then the selected customer can not be added to the transaction
            if (retailTransaction.InvoicedCustomer.Blocked == DE.BlockedEnum.All)
            {
                //If Invoiced Customer is blocked then the original customer should be blocked too.
                retailTransaction.Customer.Blocked = DE.BlockedEnum.All;

                //Display the message
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(51004)) //The invoiced customer has been blocked. Charging to this account will not be allowed.
                {
                    Customer.InternalApplication.ApplicationFramework.POSShowForm(dialog);
                }

                //Cancel all customer accounts
                retailTransaction.Customer = GetBlankCustomer();
                retailTransaction.InvoicedCustomer = GetBlankCustomer();

                return false;
            }

            if (retailTransaction.InvoicedCustomer.Blocked == DE.BlockedEnum.Invoice)
            {
                //If a similar message has already been displayed for the original customer then don't display it again.
                if (retailTransaction.Customer.Blocked != DE.BlockedEnum.Invoice)
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(51005)) //This customer has been blocked. This account can not be charged to.
                    {
                        Customer.InternalApplication.ApplicationFramework.POSShowForm(dialog);
                    }
                }

                //If Invoiced Customer is blocked then the original customer should be blocked too.
                retailTransaction.Customer.Blocked = DE.BlockedEnum.Invoice;
            }

            return true;
        }

        /// <summary>
        /// Displays the customer search UI
        /// </summary>        
        /// <returns>Customer if found, null otherwise</returns>
        public DE.ICustomer Search()
        {
            string selectedCustomerId = string.Empty;

            // Show the search dialog
            using (frmCustomerSearch searchDialog = new frmCustomerSearch())
            {
                this.Application.ApplicationFramework.POSShowForm(searchDialog);

                // Quit if cancel is pressed...
                if (searchDialog.DialogResult != System.Windows.Forms.DialogResult.OK)
                {
                    return null;
                }
                selectedCustomerId = searchDialog.SelectedCustomerId;
            }

            //Get information about the selected customer and return it
            if (selectedCustomerId.Length != 0)
            {
                //Get the customer info...
                return this.CustomerSystem.GetCustomerInfo(selectedCustomerId);
            }
            else
            {
                //No customer was selected
                return null;
            }
        }


        /// <summary>
        /// Sets the customer balance of the customer
        /// </summary>
        /// <param name="retailTransaction">The retail tranaction</param>
        public void Balance(DE.IRetailTransaction retailTransaction)
        {
        }

        /// <summary>
        /// Sets the customer status of the customer
        /// </summary>
        /// <param name="retailTransaction">The retail tranaction</param>
        public void Status(DE.IRetailTransaction retailTransaction)
        {
        }

        /// <summary>
        /// Register information about a new customer into the database
        /// </summary>
        /// <returns>Returns new customer of successful, null otherwise</returns>
        public DE.ICustomer AddNew()
        {
            DE.ICustomer customer = null;
            DE.IAddress address = null;

            CustomerData custData = new CustomerData(Application.Settings.Database.Connection, Application.Settings.Database.DataAreaID);

            customer = GetNewCustomerDefaults();
            address = GetNewAddressDefaults(customer);
            using (frmNewCustomer newCustDialog = new frmNewCustomer(customer, address))
            {
                this.Application.ApplicationFramework.POSShowForm(newCustDialog);

                if (newCustDialog.DialogResult == DialogResult.OK)
                {
                    customer = newCustDialog.Customer;
                    address = newCustDialog.Address;
                }
            }

            return customer;
        }

        private static DE.IAddress GetNewAddressDefaults(DE.ICustomer customer)
        {
            DE.IAddress address = Customer.InternalApplication.BusinessLogic.Utility.CreateAddress();

            address.AddressType = DE.AddressType.Home;
            address.BuildingComplement  = (customer.AddressComplement)  ?? string.Empty;
            address.City                = (customer.City)               ?? string.Empty;
            address.CNPJCPFNumber       = (customer.CNPJCPFNumber)      ?? string.Empty;
            address.Country             = (customer.Country)            ?? string.Empty;
            address.County              = string.Empty; // We don't allow user to input/modify this field in UI; so keep this empty.
            address.DistrictName        = (customer.DistrictName)       ?? string.Empty;
            address.Email               = (customer.Email)              ?? string.Empty;
            address.OrgId               = (customer.OrgId)              ?? string.Empty;
            address.PostalCode          = (customer.PostalCode)         ?? string.Empty;
            address.State               = (customer.State)              ?? string.Empty;
            address.Telephone           = (customer.Telephone)          ?? string.Empty;
            address.SalesTaxGroup       = (customer.SalesTaxGroup)      ?? string.Empty;
            address.StreetName          = (customer.StreetName)         ?? string.Empty;
            address.StreetNumber        = (customer.AddressNumber)      ?? string.Empty;
            address.URL                 = (customer.WwwAddress)         ?? string.Empty;

            return address;
        }

        private static DE.ICustomer GetNewCustomerDefaults()
        {
            DE.ICustomer customer = Customer.InternalApplication.BusinessLogic.Utility.CreateCustomer();

            SettingsData settingsData = new SettingsData(ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID);
            using (DataTable storeData = settingsData.GetStoreInformation(ApplicationSettings.Terminal.StoreId))
            {
                if (storeData.Rows.Count > 0)
                {
                    customer.Country = storeData.Rows[0]["COUNTRYREGIONID"].ToString();
                }
            }
            customer.Currency = ApplicationSettings.Terminal.StoreCurrency;
            customer.Language = ApplicationSettings.Terminal.CultureName;
            customer.RelationType = DE.RelationType.Person;

            return customer;
        }

        /// <summary>
        /// Updates the customer information in the database
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <returns>Returns true if operations is successful</returns>
        public bool Update(string customerId)
        {
            return false;
        }

        /// <summary>
        /// Updates the customer information in the database
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <returns>Returns the updated customer if succeded, null otherwise</returns>
        public DE.ICustomer UpdateCustomer(string customerId)
        {
            DM.CustomerDataManager customerDataManager = new DM.CustomerDataManager(
                    ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID);

            DE.ICustomer customer = customerDataManager.GetTransactionalCustomer(customerId);
            DE.IAddress address = customerDataManager.GetAddress(customer.PrimaryAddress.Id);

            using (frmNewCustomer newCustDialog = new frmNewCustomer(customer, address))
            {
                this.Application.ApplicationFramework.POSShowForm(newCustDialog);

                if (newCustDialog.DialogResult == DialogResult.OK)
                {
                    return customer;
                }
            }

            return null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Param 'customer' is already validated. Oddly, this CA message only appears when parameterList has more than 18 fields."),
        System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "4", Justification = "Param 'entityKeys' is already validated. Oddly, this CA message only appears when parameterList has more than 18 fields.")]
        internal static void UpdateCustomer(ref bool retValue, ref string comment, ref DE.ICustomer customer, ref IList<Int64> entityKeys)
        {
            if (customer == null)
            {
                throw new ArgumentNullException("customer");
            }

            string partyName = null;
            switch (customer.RelationType)
            {
                case DE.RelationType.Person:
                    // send party name in fixed format
                    partyName = string.Format("{0} {1} {2}",
                        (customer.FirstName) ?? string.Empty,
                        (customer.MiddleName) ?? string.Empty,
                        (customer.LastName) ?? string.Empty);

                    if (string.IsNullOrWhiteSpace(partyName))
                    {
                        partyName = customer.Name;
                    }
                    break;
                default:
                    partyName = customer.Name;
                    break;
            }

            // Some of the parameters has been disabled, as they don't currently have corresponding values in the SDK, only the party name is being set.
            object[] parameterList = new object[] 
            {
                customer.RecordId,
                partyName,
                customer.CustGroup,
                customer.Currency,
                customer.Language,
                customer.Telephone,
                customer.TelephoneId,
                customer.MobilePhone,
                customer.Email,
                customer.EmailId,
                customer.WwwAddress,
                customer.UrlId,
                customer.MultiLineDiscountGroup,
                customer.TotalDiscountGroup,
                customer.LineDiscountGroup,
                customer.PriceGroup,
                customer.SalesTaxGroup,
                customer.CreditLimit,
                (int)customer.Blocked,
                (customer.OrgId) ?? string.Empty,
                customer.UsePurchRequest,
                customer.VatNum,
                customer.InvoiceAccount == null ? string.Empty : customer.InvoiceAccount,
                customer.MandatoryCreditLimit,
                customer.ContactPerson,
                customer.UseOrderNumberReference,
                (int)customer.ReceiptSettings,
                customer.ReceiptEmailAddress,
                (customer.IdentificationNumber) ?? string.Empty
            };

            ReadOnlyCollection<object> containerArray = InternalApplication.TransactionServices.Invoke("UpdateCustomer", parameterList);

            try
            {
                retValue = (bool)containerArray[1];
                comment = (string)containerArray[2];

                if (retValue && containerArray.Count > 3)   //container array puts data starting at index 3 - check for data being present
                {
                    customer.CustomerId = Utility.ToStr(containerArray[3]);
                    customer.SalesTaxGroup = Utility.ToStr(containerArray[4]);
                    customer.PartyId = Utility.ToInt64(containerArray[5]);
                    customer.RecordId = Utility.ToStr(containerArray[6]);

                    if (entityKeys == null)
                    {
                        entityKeys = new List<Int64>(containerArray.Count - 5);
                    }

                    entityKeys.Clear();

                    // save into entity keys collection, the data needed for saving the customer starting @ PartyId
                    for (int i = 5; i < containerArray.Count; i++)
                    {
                        entityKeys.Add(Utility.ToInt64(containerArray[i]));
                    }
                }
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException("LSRetailPosis.TransactionServices.UpdateCustomer", ex);
            }
        }

        /// <summary>
        /// Delete the customer from the database if the customer holds no customer transactions.
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <returns>Returns true if operations is successful</returns>
        public bool Delete(string customerId)
        {
            return false;
        }

        /// <summary>
        /// Show customertransactions for customer
        /// </summary>
        /// <param name="customerId">The id of the customer</param>
        public void Transactions(string customerId)
        {
            string[] custTransactionsLanguageTexts = new string[3];
            custTransactionsLanguageTexts[0] = ApplicationLocalizer.Language.Translate(51151);
            custTransactionsLanguageTexts[1] = ApplicationLocalizer.Language.Translate(51152);
            custTransactionsLanguageTexts[2] = ApplicationLocalizer.Language.Translate(51153);

            CustomerData customerData = new CustomerData(Application.Settings.Database.Connection, Application.Settings.Database.DataAreaID);
            using (DataTable dt = customerData.GetCustomerTransactions(string.Empty, customerId, 1, custTransactionsLanguageTexts))
            {
                if (dt.Rows.Count > 0)
                {
                    using (frmCustomerTransactions customerTransactions = new frmCustomerTransactions(customerId, Application))
                    {
                        this.Application.ApplicationFramework.POSShowForm(customerTransactions);
                    }
                }
                else
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(51000))//No transactions are found for the customer.
                    {
                        this.Application.ApplicationFramework.POSShowForm(dialog);
                    }
                }
            }
        }

        /// <summary>
        /// Print customertransactions for customer
        /// </summary>
        /// <param name="customerId">The id of the customer</param>
        public void TransactionsReport(string customerId)
        {
            using (frmPrintSelection printSelection = new frmPrintSelection(customerId))
            {
                this.Application.ApplicationFramework.POSShowForm(printSelection);
                if (printSelection.DialogResult != DialogResult.OK)
                {
                    return;
                }
                else
                {
                    ///Do the report.
                }
            }
        }

        /// <summary>
        /// Prints a balance report for all customer with balance not equal to zero.
        /// </summary>
        public void BalanceReport()
        {
            BalanceReport balanceReport = new BalanceReport();
            balanceReport.Print();
        }

        /// <summary>
        /// validate customer details from database with the transaction made by the customer.
        /// </summary>
        /// <param name="valid"></param>
        /// <param name="comment"></param>
        /// <param name="manualAuthenticationCode"></param>
        /// <param name="customerId"></param>
        /// <param name="amount"></param>
        /// <param name="retailTransaction"></param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "5", Justification = "Grandfather")]
        public void AuthorizeCustomerAccountPayment(ref bool valid, ref string comment, ref string manualAuthenticationCode, string customerId, decimal amount, DE.IRetailTransaction retailTransaction)
        {
            try
            {
                LSRetailPosis.ApplicationLog.Log(this.ToString(), "Customer.AuthorizeCustomerAccountPayment()", LSRetailPosis.LogTraceLevel.Trace);
                //Get the customer information for the customer
                DM.CustomerDataManager customerDataManager = new DM.CustomerDataManager(
                    ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID);
                DE.ICustomer tempCust = customerDataManager.GetTransactionalCustomer(customerId);

                if (!string.IsNullOrEmpty(tempCust.InvoiceAccount))
                {
                    DE.ICustomer tempInvCust = customerDataManager.GetTransactionalCustomer(tempCust.InvoiceAccount);
                    if (tempInvCust.Blocked == DE.BlockedEnum.All)
                    {
                        tempCust.Blocked = tempInvCust.Blocked;
                    }
                    else if (tempInvCust.Blocked == DE.BlockedEnum.Invoice && tempCust.Blocked != DE.BlockedEnum.All)
                    {
                        tempCust.Blocked = DE.BlockedEnum.Invoice;
                    }
                }

                valid = true;

                if (LSRetailPosis.Settings.ApplicationSettings.Terminal.Standalone == true)
                {
                    decimal balance = customerDataManager.GetBalance(tempCust.CustomerId);
                    if (((balance * -1) + Convert.ToDecimal(amount)) > tempCust.CreditLimit)
                    {
                        valid = false;
                        comment = LSRetailPosis.ApplicationLocalizer.Language.Translate(51007);  // The amount charged is higher than existing creditlimit
                    }
                    return;
                }

                // Using the transaction services to validate the transaction
                this.Application.TransactionServices.ValidateCustomerStatus(ref valid, ref comment, customerId, amount, retailTransaction.StoreCurrencyCode);
                if (comment.Length > 0)
                {
                    if (comment.Substring(0, 1) == "\t")
                        comment = comment.Remove(0, 1);
                }
            }
            catch (LSRetailPosis.PosisException px)
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

        private static string GetPurchRequestId()
        {
            string retVal = string.Empty;

            try
            {

                InputConfirmation inputConfirmation = new InputConfirmation()
                {
                    PromptText = LSRetailPosis.ApplicationLocalizer.Language.Translate(51001), // Enter purchase request id
                };

                InteractionRequestedEventArgs request = new InteractionRequestedEventArgs(inputConfirmation, () =>
                {
                    retVal = inputConfirmation.EnteredText;
                    if (retVal.Length > 20)
                    {
                        retVal = retVal.Substring(0, 20);
                    }
                }
                );

                InternalApplication.Services.Interaction.InteractionRequest(request);
                
            }
            catch (Exception ex)
            {
                NetTracer.Error(ex, "Customer::GetPurchRequestId failed");
            }

            return retVal;
        }

        /// <summary>
        /// Displays UI to enter a customer ID and returns the customer based on the customer ID entered.
        /// </summary>
        /// <returns>Customer if found, otherwise null</returns>
        public DE.ICustomer GetCustomer()
        {
            DE.ICustomer customer = null;

            InputConfirmation inputConfirmation = new InputConfirmation()
            {
                MaxLength = 30,
                PromptText = LSRetailPosis.ApplicationLocalizer.Language.Translate(3060), // "Enter a customer id.";
            };

            InteractionRequestedEventArgs request = new InteractionRequestedEventArgs(inputConfirmation, () =>
            {
                if (inputConfirmation.Confirmed)
                {
                    customer = this.CustomerSystem.GetCustomerInfo(inputConfirmation.EnteredText);
                }
            }
            );

            InternalApplication.Services.Interaction.InteractionRequest(request);

            return customer;
        }

        public void AddShippingAddress(DE.IPosTransaction posTransaction)
        {
            RetailTransaction retailTransaction = (RetailTransaction)posTransaction;
            DE.IAddress address = AddNewShippingAddress(retailTransaction.Customer);
            if (address != null)
            {
                Customer.InternalApplication.BusinessLogic.CustomerSystem.SetShippingAddress(retailTransaction, address);
            }
        }

        /// <summary>
        /// Get sales order history for the given customer
        /// </summary>
        /// <param name="customerId">id of the customer (must be non-null/empty)</param>
        /// <returns>CustomerHistory object if successfull, NULL if not.</returns>
        internal static CustomerHistory GetCustomerHistory(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId)) throw new ArgumentNullException("customerId");

            const int successIndex = 1; // index of the success/fail result
            const int commentIndex = 2; // index of the error message or comment
            const int payloadIndex = 3; // index of the content/payload.

            try
            {
                CustomerHistory history = null;
                ReadOnlyCollection<object> containerArray;
                bool retValue;
                string comment;

                // Begin by checking if there is a connection to the Transaction Service
                if (Customer.InternalApplication.TransactionServices.CheckConnection())
                {
                    // Send request to AX
                    containerArray = InternalApplication.TransactionServices.Invoke("getCustomerHistory", 
                        customerId,
                        (int)Functions.DaysCustomerHistory);

                    retValue = (bool)containerArray[successIndex];
                    comment = containerArray[commentIndex].ToString();

                    if (retValue)
                    {
                        // Only set the Id if we successfully created the order/quote
                        string xmlString = containerArray[payloadIndex].ToString();
                        history = CustomerHistory.FromXml(xmlString);
                    }
                    else
                    {
                        ApplicationLog.Log(
                            typeof(Customer).ToString(),
                            string.Format("{0}\n{1}", ApplicationLocalizer.Language.Translate(99412), comment), //"an error occured in the operation"
                            LogTraceLevel.Error);
                        Customer.InternalApplication.Services.Dialog.ShowMessage(99412, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                if (history != null)
                {
                    history.Parse();
                }

                return history;
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(typeof(Customer).ToString(), ex);
                throw;
            }
        }

        /// <summary>
        /// Convert SalesOrderStatus to SalesStatus
        /// </summary>
        /// <param name="salesOrderStatus">SalesOrderStatus</param>
        /// <returns>SalesStatus</returns>
        internal static DE.SalesStatus GetSalesStatus(SalesOrderStatus salesOrderStatus)
        {
            switch (salesOrderStatus)
            {
                case SalesOrderStatus.Backorder:    return DE.SalesStatus.Created;
                case SalesOrderStatus.Delivered:    return DE.SalesStatus.Delivered;
                case SalesOrderStatus.Invoiced:     return DE.SalesStatus.Invoiced;
                case SalesOrderStatus.Canceled:     return DE.SalesStatus.Canceled;
                default:                            return DE.SalesStatus.Unknown;
            }
        }
    }

    /// <summary>
    /// String formatting class for Customer Order SalesStatus enum
    /// </summary>
    internal class SalesOrderStatusFormatter : IFormatProvider, ICustomFormatter
    {
        private readonly string Unknown;
        private readonly string Confirmed;
        private readonly string Created;
        private readonly string Processing;
        private readonly string Lost;
        private readonly string Canceled;
        private readonly string Sent;
        private readonly string Delivered;
        private readonly string Invoiced;

        /// <summary>
        /// Formats status settings.
        /// </summary>
        public SalesOrderStatusFormatter()
        {
            Unknown = ApplicationLocalizer.Language.Translate(56375); // "None";
            Confirmed = ApplicationLocalizer.Language.Translate(56404); // "Confirmed";
            Created = ApplicationLocalizer.Language.Translate(56376); // "Created";
            Processing = ApplicationLocalizer.Language.Translate(56402); // "Processing";
            Lost = ApplicationLocalizer.Language.Translate(56403); // "Lost";
            Canceled = ApplicationLocalizer.Language.Translate(56379); // "Canceled";
            Sent = ApplicationLocalizer.Language.Translate(56405); // "Sent";
            Delivered = ApplicationLocalizer.Language.Translate(56377); // "Delivered";
            Invoiced = ApplicationLocalizer.Language.Translate(56378); // "Invoiced";
        }

        /// <summary>
        /// The GetFormat method of the IFormatProvider interface.
        /// This must return an object that provides formatting services for the specified type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object GetFormat(System.Type type)
        {
            return this;
        }

        /// <summary>
        /// The Format method of the ICustomFormatter interface.
        /// This must format the specified value according to the specified format settings.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg is int || arg is DE.SalesStatus)
            {
                switch ((int)arg)
                {
                    case (int)DE.SalesStatus.Unknown: return this.Unknown;
                    case (int)DE.SalesStatus.Confirmed: return this.Confirmed;
                    case (int)DE.SalesStatus.Created: return this.Created;
                    case (int)DE.SalesStatus.Processing: return this.Processing;
                    case (int)DE.SalesStatus.Lost: return this.Lost;
                    case (int)DE.SalesStatus.Canceled: return this.Canceled;
                    case (int)DE.SalesStatus.Sent: return this.Sent;
                    case (int)DE.SalesStatus.Delivered: return this.Delivered;
                    case (int)DE.SalesStatus.Invoiced: return this.Invoiced;
                    default: return string.Empty;
                }
            }
            return (arg == null) ? string.Empty : arg.ToString();
        }
    }
}
