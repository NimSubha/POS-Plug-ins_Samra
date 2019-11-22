/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using LSRetailPosis;
using LSRetailPosis.ButtonGrid;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Pos.DataEntity;
using Microsoft.Dynamics.Retail.Pos.DataManager;
using Microsoft.Dynamics.Retail.Pos.Interaction.Properties;
using Microsoft.Dynamics.Retail.Pos.SystemCore;

namespace Microsoft.Dynamics.Retail.Pos.Interaction.ViewModels
{
    /// <summary>
    /// View model class for the product details form.
    /// </summary>
    internal sealed class ProductDetailsViewModel
    {
        /// <summary>
        /// Gets or sets the product details information.
        /// </summary>
        private ProductDetails ProductDetails { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductDetailsViewModel"/> class.
        /// </summary>
        /// <param name="itemNumber">The item number.</param>
        public ProductDetailsViewModel(string itemNumber)
        {
            if (string.IsNullOrWhiteSpace(itemNumber))
                throw new ArgumentNullException("itemNumber");

            ItemDataManager dataManager = new ItemDataManager(
                PosApplication.Instance.Settings.Database.Connection,
                PosApplication.Instance.Settings.Database.DataAreaID);

            this.ProductDetails = dataManager.GetProductDetails(
                ApplicationSettings.Terminal.StorePrimaryId,
                itemNumber,
                ApplicationSettings.Terminal.CultureName);

            Image image = GUIHelper.GetBitmap(this.ProductDetails.ImageData);

            this.Image = image ?? Resources.ProductUnavailable;

            decimal price = PosApplication.Instance.Services.Price.GetItemPrice(itemNumber, this.ProductDetails.UnitOfMeasure);
            this.Price = PosApplication.Instance.Services.Rounding.RoundForDisplay(price, true, true);

            this.FormattedProductCategoryHierarchy = GetFormattedProductCategoryHierarchy();
        }

        /// <summary>
        /// Gets the formatted product category hierarchy string.
        /// </summary>
        /// <returns>The formatted product category hierarchy string.</returns>
        private string GetFormattedProductCategoryHierarchy()
        {
            StringBuilder builder = new StringBuilder();

            Collection<CategoryInformation> categoryHierarchy = this.ProductDetails.CategoryHierarchy;
            for (int i = 0; i < categoryHierarchy.Count; i++)
            {
                // only uses the format when collection has more than one and it's not the last one
                bool useFormat = (categoryHierarchy.Count > 1) && (i != (categoryHierarchy.Count - 1));

                builder.AppendFormat(useFormat ?
                    ApplicationLocalizer.Language.Translate(99817) // "{0} > "
                    : "{0}",
                    categoryHierarchy[i].CategoryName);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Gets or sets the product description.
        /// </summary>
        public string Description
        {
            get { return this.ProductDetails.Description; }
        }

        /// <summary>
        /// Gets the item image.
        /// </summary>
        public Image Image { get; private set; }

        /// <summary>
        /// Gets the date blocked value.
        /// </summary>
        public DateTime? DateBlocked
        {
            get 
            {
                DateTime dateBlocked = this.ProductDetails.DateBlocked;

                return (dateBlocked == LSRetailPosis.DevUtilities.Utility.POSNODATE)
                    ? (DateTime?)null
                    : dateBlocked;
            }
        }

        /// <summary>
        /// Gets the date to be blocked value.
        /// </summary>
        public DateTime? DateToBeBlocked
        {
            get
            {
                DateTime dateToBeBlocked = this.ProductDetails.DateToBeBlocked;

                return (dateToBeBlocked == LSRetailPosis.DevUtilities.Utility.POSNODATE)
                    ? (DateTime?)null
                    : dateToBeBlocked;
            }
        }

        /// <summary>
        /// Gets the issue date.
        /// </summary>
        public DateTime? IssueDate
        {
            get 
            {
                DateTime issueDate = this.ProductDetails.IssueDate;

                return (issueDate == LSRetailPosis.DevUtilities.Utility.POSNODATE)
                    ? (DateTime?)null
                    : issueDate;
            }
        }

        /// <summary>
        /// Gets the search name.
        /// </summary>
        public string SearchName
        {
            get { return this.ProductDetails.SearchName; }
        }

        /// <summary>
        /// Gets the bard code.
        /// </summary>
        public string Barcode
        {
            get { return this.ProductDetails.Barcode; }
        }

        /// <summary>
        /// Gets the price.
        /// </summary>
        public string Price { get; private set; }

        /// <summary>
        /// Gets the unit of measure.
        /// </summary>
        public string UnitOfMeasure
        {
            get { return this.ProductDetails.UnitOfMeasure; }
        }

        /// <summary>
        /// Gets the product category.
        /// </summary>
        public string ProductCategory
        {
            get { return this.ProductDetails.ProductCategory; }
        }

        /// <summary>
        /// Gets the formatted product category hierarchy.
        /// </summary>
        public string FormattedProductCategoryHierarchy { get; private set; }

        /// <summary>
        /// Gets the product attributes collection.
        /// </summary>
        public Collection<NameValuePair> ProductAttributes {
            get { return this.ProductDetails.ProductAttributes; }
        }

        /// <summary>
        /// Shows the inventory lookup form.
        /// </summary>
        public void ShowInventoryLookup()
        {
            PosApplication.Instance.Services.Dialog.InventoryLookup(this.ProductDetails.ID);
        }
    }
}
