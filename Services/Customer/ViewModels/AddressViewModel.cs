/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using LSRetailPosis.DataAccess;
using LSRetailPosis.Settings;
using LSRetailPosis.Settings.FunctionalityProfiles;
using LSRetailPosis.Transaction;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using CS = Microsoft.Dynamics.Retail.Pos.Customer.Customer;
using DM = Microsoft.Dynamics.Retail.Pos.DataManager;

namespace Microsoft.Dynamics.Retail.Pos.Customer.ViewModels
{
    public class AddressViewModel : INotifyPropertyChanged
    {
        private readonly string CaptionState = LSRetailPosis.ApplicationLocalizer.Language.Translate(51119); // State:
        private readonly string CaptionPostalCode = LSRetailPosis.ApplicationLocalizer.Language.Translate(51118); // ZIP/Postal code:
        private readonly string CaptionCountry = LSRetailPosis.ApplicationLocalizer.Language.Translate(51117); // Country/Region:

        /// <summary>
        /// Entity key index for the address record identifier.
        /// </summary>
        private const int AddressRecordIdIndex = 3;

        private LSRetailPosis.Settings.FunctionalityProfiles.SupportedCountryRegion supportedCountryRegion;
        private Dictionary<LogisticsAddressElement, bool> elementSelection = new Dictionary<LogisticsAddressElement, bool>();

        public AddressViewModel(IAddress address, ICustomer customer)
        {
            this.Address = address;
            this.supportedCountryRegion = Functions.CountryRegion;
            this.IsAddressForExistingCustomer = customer != null && !string.IsNullOrWhiteSpace(customer.RecordId);
        }

        /// <summary>
        /// <c>True</c> if the address is being saved for an existing customer, <c>false</c> otherwise.
        /// </summary>
        public bool IsAddressForExistingCustomer { get; private set; }

        public IAddress Address
        {
            get;
            private set;
        }

        /// <summary>
        /// Address formatted per the country formatting specification of the address
        /// </summary>
        public string FormattedAddress
        {
            get
            {
                if (this.Address != null && !string.IsNullOrEmpty(this.Address.Country))
                {
                    var formatter = new AddressFormatter(ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID);
                    var formattedAddress = formatter.Format(address: this.Address, multiline: true);
                    return formattedAddress;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Address formatted in a single line per the country formatting specification of the address
        /// </summary>
        public string LineAddress
        {
            get
            {
                if (this.Address != null && !string.IsNullOrEmpty(this.Address.Country))
                {
                    var formatter = new AddressFormatter(ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID);
                    return formatter.Format(address: this.Address, multiline: false);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Select address elements that need to be displayed per the default country
        /// Use properties from IAddressElementInputSelection to obtain element selection values
        /// </summary>
        public void SetElementSelection()
        {
            SetElementSelection(this.supportedCountryRegion);
        }
        
        /// <summary>
        /// Select address elements that need to be displayed per the given country
        /// Use properties from IAddressElementInputSelection to obtain element selection values
        /// </summary>
        /// <param name="country">Country for which the elements are to be selected</param>
        public void SetElementSelection(SupportedCountryRegion country)
        {
            var formatter = new AddressFormatter(ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID);
            string isoCode =string.Empty;
            isoCode = Enum.GetName(typeof(SupportedCountryRegion), country);
            var countryRegion = formatter.GetCountryRegion(searchISOCode: isoCode);
            elementSelection = formatter.ElementSelection(countryRegion);
            RefreshControlVisibility();
        }

        /// <summary>
        /// Select address elements that need to be displayed per the given country
        /// Use properties from IAddressElementInputSelection to obtain element selection values
        /// </summary>
        /// <param name="country">Country in the form of CountryRegion code</param>
        public void SetElementSelection(string country)
        {
            var formatter = new AddressFormatter(ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID);

            if(!string.IsNullOrEmpty(country))
            elementSelection = formatter.ElementSelection(country);

            RefreshControlVisibility();
        }

        private void RefreshControlVisibility()
        {
            OnPropertyChanged("Address");
        }

        /// <summary>
        /// Address name as a string
        /// </summary>
        public string AddressName
        {
            get { return this.Address.Name; }
            set
            {
                if (this.Address.Name != value)
                {
                    this.Address.Name = value;
                    OnPropertyChanged("AddressName");
                }
            }
        }

        /// <summary>
        /// Address type
        /// </summary>
        public AddressType AddressType
        {
            get { return this.Address.AddressType; }
            set
            {
                if (this.Address.AddressType != value)
                {
                    this.Address.AddressType = value;
                    OnPropertyChanged("AddressType");
                }
            }
        }

        /// <summary>
        /// Sales tax group the address belongs to
        /// </summary>
        public string SalesTaxGroup
        {
            get { return this.Address.SalesTaxGroup; }
            set
            {
                if (this.Address.SalesTaxGroup != value)
                {
                    this.Address.SalesTaxGroup = value;
                    OnPropertyChanged("SalesTaxGroup");
                }
            }
        }

        /// <summary>
        /// Value of address element Street
        /// </summary>
        public string Street
        {
            get { return this.Address.StreetName; }
            set
            {
                if (this.Address.StreetName != value)
                {
                    this.Address.StreetName = value;
                    OnPropertyChanged("Street");
                }
            }
        }

        /// <summary>
        /// Value of address element City
        /// </summary>
        public string City
        {
            get { return this.Address.City; }
            set
            {
                if (this.Address.City != value)
                {
                    this.Address.City = value;
                    OnPropertyChanged("City");
                }
            }
        }

        /// <summary>
        /// Value of address element State
        /// </summary>
        public string State
        {
            get { return this.Address.State; }
            set
            {
                if (this.Address.State != value)
                {
                    this.Address.State = value;
                    OnPropertyChanged("State");
                }
            }
        }

        /// <summary>
        /// Value of address element County
        /// </summary>
        public string County
        {
            get { return this.Address.County; }
            set
            {
                if (this.Address.County != value)
                {
                    this.Address.County = value;
                    OnPropertyChanged("County");
                }
            }
        }

        /// <summary>
        /// Value of address element BuildingComplement
        /// </summary>
        public string BuildingComplement
        {
            get { return this.Address.BuildingComplement; }
            set
            {
                if (this.Address.BuildingComplement != value)
                {
                    this.Address.BuildingComplement= value;
                    OnPropertyChanged("BuildingComplement");
                }
            }
        }

        /// <summary>
        /// Value of address element PostalCode
        /// </summary>
        public string Zip
        {
            get { return this.Address.PostalCode; }
            set
            {
                if (this.Address.PostalCode != value)
                {
                    this.Address.PostalCode = value;
                    OnPropertyChanged("Zip");
                }
            }
        }

        /// <summary>
        /// Value of address element Country (as a country region id)
        /// </summary>
        public string Country
        {
            get { return this.Address.Country; }
            set
            {
                if (this.Address.Country != value)
                {
                    this.Address.Country = value;
                    SetElementSelection(value);
                    OnPropertyChanged("Country");
                }
            }
        }

        /// <summary>
        /// Value of address element PostBox
        /// </summary>
        public string Postbox
        {
            get { return this.Address.PostBox; }
            set
            {
                if (this.Address.PostBox != value)
                {
                    this.Address.PostBox = value;
                    OnPropertyChanged("Postbox");
                }
            }
        }

        /// <summary>
        /// Value of address element House. Russian specific
        /// </summary>
        public string House
        {
            get { return this.Address.House; }
            set
            {
                if (this.Address.House != value)
                {
                    this.Address.House = value;
                    OnPropertyChanged("House");
                }
            }
        }

        /// <summary>
        /// Value of address element Flat. Russian specific
        /// </summary>
        public string Flat
        {
            get { return this.Address.Flat; }
            set
            {
                if (this.Address.Flat != value)
                {
                    this.Address.Flat = value;
                    OnPropertyChanged("Flat");
                }
            }
        }

        /// <summary>
        /// Value of address element CountryOKSMCode (Street). Russian specific
        /// </summary>
        public string CountryOKSMCode
        {
            get { return this.Address.CountryOKSMCode; }
            set
            {
                if (this.Address.CountryOKSMCode != value)
                {
                    this.Address.CountryOKSMCode = value;
                    OnPropertyChanged("CountryOKSMCode");
                }
            }
        }

        /// <summary>
        /// Value of address element DistrictName
        /// </summary>
        public string DistrictName
        {
            get { return this.Address.DistrictName; }
            set
            {
                if (this.Address.DistrictName != value)
                {
                    this.Address.DistrictName = value;
                    OnPropertyChanged("DistrictName");
                }
            }
        }

        /// <summary>
        /// Value of address element OrganizationNumber
        /// </summary>
        public string OrganizationNumber
        {
            get { return this.Address.OrgId; }
            set
            {
                if (this.Address.OrgId != value)
                {
                    this.Address.OrgId = value;
                    OnPropertyChanged("OrganizationNumber");
                }
            }
        }

        /// <summary>
        /// Value of address element StreetNumber
        /// </summary>
        public string StreetNumber
        {
            get { return this.Address.StreetNumber; }
            set
            {
                if (this.Address.StreetNumber != value)
                {
                    this.Address.StreetNumber = value;
                    OnPropertyChanged("StreetNumber");
                }
            }
        }

        /// <summary>
        /// Value of phone number linked to the postal address
        /// </summary>
        public string Phone
        {
            get { return this.Address.Telephone; }
            set
            {
                if (this.Address.Telephone != value)
                {
                    this.Address.Telephone = value;
                    OnPropertyChanged("Phone");
                }
            }
        }

        /// <summary>
        /// Value of URL linked to the postal address
        /// </summary>
        public string Website
        {
            get { return this.Address.URL; }
            set
            {
                if (this.Address.URL != value)
                {
                    this.Address.URL = value;
                    OnPropertyChanged("Website");
                }
            }
        }

        /// <summary>
        /// Value of Email address linked to the postal address
        /// </summary>
        public string Email
        {
            get { return this.Address.Email; }
            set
            {
                if (this.Address.Email != value)
                {
                    this.Address.Email = value;
                    OnPropertyChanged("Email");
                }
            }
        }

        /// <summary>
        /// True if at least on of the fields is non empty. False otherwise
        /// </summary>
        public bool Validated
        {
            get
            {
                bool allFieldsEmpty =
                    string.IsNullOrWhiteSpace(this.AddressName)

                    && string.IsNullOrWhiteSpace(this.Email)
                    && string.IsNullOrWhiteSpace(this.Phone)
                    && string.IsNullOrWhiteSpace(this.Website)
                    && string.IsNullOrWhiteSpace(this.SalesTaxGroup)

                    && string.IsNullOrWhiteSpace(this.Street)
                    && string.IsNullOrWhiteSpace(this.City)
                    && string.IsNullOrWhiteSpace(this.County)
                    && string.IsNullOrWhiteSpace(this.DistrictName)
                    && string.IsNullOrWhiteSpace(this.OrganizationNumber)
                    && string.IsNullOrWhiteSpace(this.StreetNumber)
                    && string.IsNullOrWhiteSpace(this.BuildingComplement)
                    && string.IsNullOrWhiteSpace(this.Postbox)
                    && string.IsNullOrWhiteSpace(this.House)
                    && string.IsNullOrWhiteSpace(this.Flat)
                    && string.IsNullOrWhiteSpace(this.CountryOKSMCode)
                    && string.IsNullOrWhiteSpace(this.State)
                    && string.IsNullOrWhiteSpace(this.Country)
                    && string.IsNullOrWhiteSpace(this.Zip)
                    ;

                return !allFieldsEmpty;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public LSRetailPosis.Settings.FunctionalityProfiles.SupportedCountryRegion SupportedCountryRegion
        {
            get { return this.supportedCountryRegion; }
            set
            {
                if (this.supportedCountryRegion != value)
                {
                    this.supportedCountryRegion = value;
                    OnPropertyChanged("SupportedCountryRegion");
                }
            }
        }

        internal void ExecuteSelectCountry()
        {
            DM.LocationDataManager dm = new DM.LocationDataManager(
                CS.InternalApplication.Settings.Database.Connection,
                CS.InternalApplication.Settings.Database.DataAreaID);

            var names = dm.GetCountryNames(ApplicationSettings.Terminal.CultureName);
            var list = names.ToList();
            string country;
            var dialogResult = CS.InternalApplication.Services.Dialog.GenericLookup(
                list, "ShortName", CaptionCountry, "CountryRegion", out country, null);
            if (dialogResult == DialogResult.OK && country != null)
            {
                this.Country = country;
                this.State = string.Empty;
                this.Zip = string.Empty;
            }
        }

        internal void ExecuteSelectState()
        {
            DM.LocationDataManager dm = new DM.LocationDataManager(
                CS.InternalApplication.Settings.Database.Connection,
                CS.InternalApplication.Settings.Database.DataAreaID);

            if (string.IsNullOrEmpty(this.Country))
            {
                // throw "must select country first" error
                Customer.InternalApplication.Services.Dialog.ShowMessage(903, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var names = dm.GetStates(this.Country);
            var list = names.ToList();
            string value;
            var dialogResult = CS.InternalApplication.Services.Dialog.GenericLookup(
                list, "Description", CaptionState, "Name", out value, null);
            if (dialogResult == DialogResult.OK && value != null)
            {
                this.State = value;
                this.Zip = string.Empty;
            }
        }

        internal void ExecuteSelectZipPostalCode()
        {
            DM.LocationDataManager dm = new DM.LocationDataManager(
                CS.InternalApplication.Settings.Database.Connection,
                CS.InternalApplication.Settings.Database.DataAreaID);

            if (string.IsNullOrEmpty(this.Country))
            {
                // throw "must select state and country first" error
                Customer.InternalApplication.Services.Dialog.ShowMessage(902, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var names = dm.GetZipCodes(this.Country, this.State);
            var list = names.ToList();
            string value;
            var dialogResult = CS.InternalApplication.Services.Dialog.GenericLookup(
                list, "Description", CaptionPostalCode, "Name", out value, null);
            if (dialogResult == DialogResult.OK && value != null)
            {
                this.Zip = value;

                if (string.IsNullOrEmpty(this.State))
                    this.State = dm.GetStateFromZip(value);
            }
        }

        internal void ExecuteSelectSalesTaxGroup()
        {
            string columnName = LSRetailPosis.ApplicationLocalizer.Language.Translate(51123); //Sales tax group
            CustomerData dm = new CustomerData(
                CS.InternalApplication.Settings.Database.Connection,
                CS.InternalApplication.Settings.Database.DataAreaID);
            using (DataTable groups = dm.GetSaleTaxGroups())
            {
                groups.Columns["TAXGROUPNAME"].Caption = columnName;
                DataRow row = null;
                DialogResult result = Customer.InternalApplication.Services.Dialog.GenericLookup(
                    groups, 1, ref row, columnName);

                if ((result == DialogResult.OK) && (row != null) && (row[0] != null))
                {
                    this.SalesTaxGroup = row[0].ToString();
                }
            }
        }

        internal void ExecuteSelectAddressType()
        {
            // Create a table of the AddressType enum values
            using (DataTable table = new DataTable("ADDRESSTYPES"))
            {
                table.Columns.Add("TYPE", typeof(int));
                table.Columns.Add("TYPENAME", typeof(string));
                table.Columns[1].Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(904); // layoutControlAddressType.Text;

                // AX base enum: AddressType
                table.Rows.Add(AddressType.None, LSRetailPosis.ApplicationLocalizer.Language.Translate(51170)); //"None");
                table.Rows.Add(AddressType.Invoice, LSRetailPosis.ApplicationLocalizer.Language.Translate(51171)); //"Invoice");
                table.Rows.Add(AddressType.Delivery, LSRetailPosis.ApplicationLocalizer.Language.Translate(51172)); //"Delivery");
                table.Rows.Add(AddressType.SWIFT, LSRetailPosis.ApplicationLocalizer.Language.Translate(51174)); //"SWIFT");
                table.Rows.Add(AddressType.Payment, LSRetailPosis.ApplicationLocalizer.Language.Translate(51175)); //"Payment");
                table.Rows.Add(AddressType.Service, LSRetailPosis.ApplicationLocalizer.Language.Translate(51176)); //"Service");
                table.Rows.Add(AddressType.Home, LSRetailPosis.ApplicationLocalizer.Language.Translate(51177)); //"Home");
                table.Rows.Add(AddressType.Other, LSRetailPosis.ApplicationLocalizer.Language.Translate(51178)); //"Other");
                table.Rows.Add(AddressType.Business, LSRetailPosis.ApplicationLocalizer.Language.Translate(51179)); //"Business");
                table.Rows.Add(AddressType.RemitTo, LSRetailPosis.ApplicationLocalizer.Language.Translate(51180)); //"RemitTo");
                table.Rows.Add(AddressType.ShipCarrierThirdPartyShipping, LSRetailPosis.ApplicationLocalizer.Language.Translate(51181)); //"ShipCarrierThirdPartyShipping");    
                table.Rows.Add(AddressType.Statement, LSRetailPosis.ApplicationLocalizer.Language.Translate(51195)); //"Statement");
                table.Rows.Add(AddressType.FixedAsset, LSRetailPosis.ApplicationLocalizer.Language.Translate(51196)); //"Fixed Asset");
                table.Rows.Add(AddressType.Onetime, LSRetailPosis.ApplicationLocalizer.Language.Translate(51197)); //"One Time");
                table.Rows.Add(AddressType.Recruit, LSRetailPosis.ApplicationLocalizer.Language.Translate(51198)); //"Recruit");

                DataRow row = null;
                DialogResult result = Customer.InternalApplication.Services.Dialog.GenericLookup(
                    table, 1, ref row, LSRetailPosis.ApplicationLocalizer.Language.Translate(904));    //Address type

                if ((result == DialogResult.OK) && (row != null) && (row[0] != null))
                {
                    //txtAddressType.Text = row[1].ToString();
                    this.AddressType = (AddressType)row[0];
                }
            }
        }

        internal void ExecuteClear()
        {
            this.Street = string.Empty;
            this.City = string.Empty;
            this.State = string.Empty;
            this.Zip = string.Empty;            
        }

        /// <summary>
        /// Saves the shipping address under the given customer.
        /// </summary>
        /// <param name="customer">The customer.</param>
        /// <returns><value>True</value> if the address was saved, or <value>false</value> otherwise.</returns>
        internal bool SaveShippingAddress(ICustomer customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException("customer");
            }

            try
            {
                bool createdAx = false;
                bool createdLocal = false;
                string comment = null;

                IDirPartyAddressRelationship dirRelationship = null;
                IDirPartyAddressRelationshipMapping dirMap = null;
                IList<Int64> entityKeys = new List<Int64>();

                IAddress tempAddress = this.Address;

                if (tempAddress.Id != 0)
                {
                    // address already has an ID, so try to update the existing entities.
                    CS.InternalApplication.TransactionServices.UpdateAddress(ref createdAx, ref comment, ref tempAddress, ref entityKeys);
                }
                else
                {
                    // try to create new entities under the created customer.
                    dirRelationship = CS.InternalApplication.BusinessLogic.Utility.CreateDirPartyAddressRelationship();
                    dirMap = CS.InternalApplication.BusinessLogic.Utility.CreateDirPartyAddressRelationshipMapping();

                    CS.InternalApplication.TransactionServices.CreateAddress(ref createdAx, ref comment, ref customer, ref tempAddress, ref dirRelationship, ref dirMap, ref entityKeys);
                }

                // Was the address created in AX
                if (createdAx)
                {
                    DM.CustomerDataManager customerDataManager = new DM.CustomerDataManager(
                        ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID);
                    createdLocal = customerDataManager.SaveCustomerAddressContactInfo((Address)tempAddress, entityKeys);

                    if (AddressViewModel.AddressRecordIdIndex < entityKeys.Count)
                    {
                        tempAddress.Id = entityKeys[AddressViewModel.AddressRecordIdIndex];
                    }
                }

                if (!createdAx)
                {
                    CS.InternalApplication.Services.Dialog.ShowMessage(51159, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (!createdLocal)
                {
                    CS.InternalApplication.Services.Dialog.ShowMessage(51156, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return createdAx && createdLocal;
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                CS.InternalApplication.Services.Dialog.ShowMessage(51193, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.VerifyPropertyName(propertyName);

            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerStepThrough]
        private void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;

                throw new Exception(msg);
            }
        }

        #endregion

        #region Input selection Members
        
        /// <summary>
        /// True if StreetName is required to be input. Run SetElementSelection before this
        /// </summary>
        public bool InputStreetName
        {
            get
            {
                bool visible;
                elementSelection.TryGetValue(LogisticsAddressElement.StreetName, out visible);
                return visible;
            }
        }

        /// <summary>
        /// True if PostalCode/ZipCode is required to be input. Run SetElementSelection before this
        /// </summary>
        public bool InputPostalCode
        {
            get
            {
                bool visible;
                elementSelection.TryGetValue(LogisticsAddressElement.ZipCode, out visible);
                return visible;
            }
        }

        /// <summary>
        /// True if City is required to be input. Run SetElementSelection before this
        /// </summary>
        public bool InputCity
        {
            get
            {
                bool visible;
                elementSelection.TryGetValue(LogisticsAddressElement.City, out visible);
                return visible;
            }
        }

        /// <summary>
        /// True if County is required to be input. Run SetElementSelection before this
        /// </summary>
        public bool InputCounty
        {
            get
            {
                bool visible;
                visible = false; // added on 12.08.2013 to hide county field
                //elementSelection.TryGetValue(LogisticsAddressElement.County, out visible); // blocked on 12.08.2013 to hide county field
                return visible;
            }
        }

        /// <summary>
        /// True if State is required to be input. Run SetElementSelection before this
        /// </summary>
        public bool InputState
        {
            get
            {
                bool visible;
                elementSelection.TryGetValue(LogisticsAddressElement.State, out visible);
                return visible;
            }
        }

        /// <summary>
        /// True if BuildingComplement is required to be input. Run SetElementSelection before this
        /// </summary>
        public bool InputBuildingComplement
        {
            get
            {
                bool visible;
                elementSelection.TryGetValue(LogisticsAddressElement.BuildingComplement, out visible);
                return visible;
            }
        }

        /// <summary>
        /// True if DistrictName is required to be input. Run SetElementSelection before this
        /// </summary>
        public bool InputDistrictName
        {
            get
            {
                bool visible;
                elementSelection.TryGetValue(LogisticsAddressElement.District, out visible);
                return visible;
            }
        }

        /// <summary>
        /// True if StreetNumber is required to be input. Run SetElementSelection before this
        /// </summary>
        public bool InputStreetNumber
        {
            get
            {
                bool visible;
                elementSelection.TryGetValue(LogisticsAddressElement.StreetNumber, out visible);
                return visible;
            }
        }

        /// <summary>
        /// True if Postbox is required to be input. Run SetElementSelection before this
        /// </summary>
        public bool InputPostbox
        {
            get
            {
                bool visible;
                elementSelection.TryGetValue(LogisticsAddressElement.Postbox, out visible);
                return visible;
            }
        }

        /// <summary>
        /// True if House (Russia) is required to be input. Run SetElementSelection before this
        /// </summary>
        public bool InputHouse
        {
            get
            {
                bool visible;
                elementSelection.TryGetValue(LogisticsAddressElement.House, out visible);
                return visible;
            }
        }

        /// <summary>
        /// True if Flat (Russia) is required to be input. Run SetElementSelection before this
        /// </summary>
        public bool InputFlat
        {
            get
            {
                bool visible;
                elementSelection.TryGetValue(LogisticsAddressElement.Flat, out visible);
                return visible;
            }
        }

        /// <summary>
        /// True if CountryOKSMCode (street - Russia) is required to be input. Run SetElementSelection before this
        /// </summary>
        public bool InputCountryOKSMCode
        {
            get
            {
                bool visible;
                elementSelection.TryGetValue(LogisticsAddressElement.CountryOKSMCode, out visible);
                return visible;
            }
        }

        /// <summary>
        /// True if OrganizationNumber is required to be input. Run SetElementSelection before this
        /// </summary>
        public bool InputOrganizationNumber
        {
            get
            {
                bool visible;
                elementSelection.TryGetValue(LogisticsAddressElement.OrganizationNumber, out visible);
                return visible;
            }
        }
    
        #endregion
    }
}
