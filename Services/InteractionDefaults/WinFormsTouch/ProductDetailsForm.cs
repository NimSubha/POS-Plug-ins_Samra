/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using System;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Base;
using LSRetailPosis;
using LSRetailPosis.POSProcesses;
using Microsoft.Dynamics.Retail.Notification.Contracts;
using Microsoft.Dynamics.Retail.Pos.Interaction.ViewModels;

namespace Microsoft.Dynamics.Retail.Pos.Interaction.WinFormsTouch
{
    /// <summary>
    /// Form for display the product details.
    /// </summary>
    [Export("ProductDetailsForm", typeof(IInteractionView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ProductDetailsForm : frmTouchBase, IInteractionView
    {
        /// <summary>
        /// Gets or sets the confirmation object.
        /// </summary>
        private ProductDetailsConfirmation ConfirmationResult { get; set; }

        /// <summary>
        /// Gets or sets the item number.
        /// </summary>
        private string ItemNumber { get; set; }

        /// <summary>
        /// Gets or sets the product details view model.
        /// </summary>
        private ProductDetailsViewModel ViewModel { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductDetailsForm"/> class.
        /// </summary>
        public ProductDetailsForm()
        {
            InitializeComponent();

            this.ConfirmationResult = new ProductDetailsConfirmation();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductDetailsForm"/> class.
        /// </summary>
        /// <param name="productDetailsConfirmation">Product details confirmation.</param>
        [ImportingConstructor]
        public ProductDetailsForm(ProductDetailsConfirmation productDetailsConfirmation)
            : this()
        {
            if (productDetailsConfirmation == null)
            {
                throw new ArgumentNullException("productDetailsConfirmation");
            }

            if (string.IsNullOrWhiteSpace(productDetailsConfirmation.ItemNumber))
            {
                throw new ArgumentNullException("productDetailsConfirmation.ItemNumber");
            }

            this.ItemNumber = productDetailsConfirmation.ItemNumber;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!this.DesignMode)
            {
                this.ViewModel = new ProductDetailsViewModel(this.ItemNumber);
                this.bindingSource.Add(this.ViewModel);

                TranslateLabels();
                UpdateNavigationButtons();
            }

            base.OnLoad(e);
        }

        private void TranslateLabels()
        {
            // Header
            this.Text               = ApplicationLocalizer.Language.Translate(99801, this.ItemNumber, this.ViewModel.SearchName); // "{0}: {1}" - Item number: item name
            this.lblHeader.Text     = ApplicationLocalizer.Language.Translate(99801, this.ItemNumber, this.ViewModel.SearchName); // "{0}: {1}" - Item number: item name

            // Labels
            this.lblBarCode.Text                = ApplicationLocalizer.Language.Translate(99802); // Bar code:
            this.lblSearchName.Text             = ApplicationLocalizer.Language.Translate(99803); // Search name:
            this.lblCategory.Text               = ApplicationLocalizer.Language.Translate(99804); // Category:
            this.lblDescription.Text            = ApplicationLocalizer.Language.Translate(99805); // Description:
            this.lblPrice.Text                  = ApplicationLocalizer.Language.Translate(99806); // Price:
            this.lblIssueDate.Text              = ApplicationLocalizer.Language.Translate(99808); // Issue date:
            this.lblDateBlocked.Text            = ApplicationLocalizer.Language.Translate(99809); // Date blocked:
            this.lblDateToBeBlocked.Text        = ApplicationLocalizer.Language.Translate(99810); // Date to be blocked:
            this.lblProductAttributes.Text      = ApplicationLocalizer.Language.Translate(99811); // Product attributes:

            // Columns
            this.colName.Caption    = ApplicationLocalizer.Language.Translate(99812); // Name
            this.colValue.Caption   = ApplicationLocalizer.Language.Translate(99813); // Value

            // Buttons
            this.btnAddToSale.Text          = ApplicationLocalizer.Language.Translate(99814); // Add to sale
            this.btnInventoryLookup.Text    = ApplicationLocalizer.Language.Translate(99815); // Inventory lookup
            this.btnCancel.Text             = ApplicationLocalizer.Language.Translate(99816); // Cancel
        }

        #region IInteractionView implementation

        /// <summary>
        /// Initialize the form.
        /// </summary>
        /// <typeparam name="TArgs">Prism Notification type.</typeparam>
        /// <param name="args">Notification.</param>
        public void Initialize<TArgs>(TArgs args)
             where TArgs : Microsoft.Practices.Prism.Interactivity.InteractionRequest.Notification
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }
        }

        /// <summary>
        /// Return the results of the interation call.
        /// </summary>
        /// <typeparam name="TResults">The confirmation result of this interaction.</typeparam>
        /// <returns>Returns the TResults object.</returns>
        public TResults GetResults<TResults>() where TResults : class, new()
        {
            this.ConfirmationResult.Confirmed = true;
            return this.ConfirmationResult as TResults;
        }

        #endregion

        private void OnBtnAddToSale_Click(object sender, EventArgs e)
        {
            this.ConfirmationResult.AddToSale = true;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void OnBtnInventoryLookup_Click(object sender, System.EventArgs e)
        {
            ViewModel.ShowInventoryLookup();
        }

        private void OnBtnPgUp_Click(object sender, EventArgs e)
        {
            gridView.MovePrevPage();
        }

        private void OnBtnUp_Click(object sender, EventArgs e)
        {
            gridView.MovePrev();
        }

        private void OnBtnPgDown_Click(object sender, EventArgs e)
        {
            gridView.MoveNextPage();
        }

        private void OnBtnDown_Click(object sender, EventArgs e)
        {
            gridView.MoveNext();
        }

        private void OnGridView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            UpdateNavigationButtons();
        }

        private void UpdateNavigationButtons()
        {
            btnDown.Enabled = btnPgDown.Enabled = !gridView.IsLastRow;
            btnUp.Enabled = btnPgUp.Enabled = !gridView.IsFirstRow;
        }
    }
}
