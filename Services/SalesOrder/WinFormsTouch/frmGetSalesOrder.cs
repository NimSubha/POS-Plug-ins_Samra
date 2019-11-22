/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Data;
using System.Windows.Forms;
using LSRetailPosis;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using CustomerOrderType = LSRetailPosis.Transaction.CustomerOrderType;
using SalesStatus = LSRetailPosis.Transaction.SalesStatus;

namespace Microsoft.Dynamics.Retail.Pos.SalesOrder.WinFormsTouch
{
    public partial class frmGetSalesOrder : LSRetailPosis.POSProcesses.frmTouchBase
    {
        private DataTable salesOrders;
        private SalesStatus selectedOrderStatus;
        private string selectedOrderPickupDeliveryMode;

        private const string DeliveryModeString = "DELIVERYMODE";
        private const string SalesIdString = "SALESID";
        private const string StatusString = "STATUS";
        private const string OrderTypeString = "ORDERTYPE";
        private const string CustomerAccountString = "CUSTOMERACCOUNT";
        private const string CustomerNameString = "CUSTOMERNAME";
        private const string EmailString = "EMAIL";
        private const string ReferenceIdString = "CHANNELREFERENCEID";

        private const string FilterFormat =
            "[SALESID] LIKE '%{0}%' OR [CHANNELREFERENCEID] LIKE '%{0}%' OR [CUSTOMERACCOUNT] LIKE '%{0}%' OR [CUSTOMERNAME] LIKE '%{0}%' OR [EMAIL] LIKE '%{0}%' OR [DATE] LIKE '%{0}%' OR [TOTALAMOUNT] LIKE '%{0}%'";

        /// <summary>
        /// Returns selected sales order id as string.
        /// </summary>
        public string SelectedSalesOrderId { get; private set; }

        /// <summary>
        /// Returns the order type of the selected order
        /// </summary>
        public CustomerOrderType SelectedOrderType { get; private set; }

        /// <summary>
        /// Get the selected (and instantiated) order
        /// </summary>
        public CustomerOrderTransaction SelectedOrder { get; private set; }

        private OrderListModel dataModel;

        /// <summary>
        /// Keeps in sales order form.
        /// </summary>
        public frmGetSalesOrder()
        {
            InitializeComponent();
        }

        internal frmGetSalesOrder(OrderListModel data)
            : this()
        {
            this.dataModel = data;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!this.DesignMode)
            {
                this.colStatus.DisplayFormat.Format = new CustomerOrderStatusFormatter();
                this.colOrderType.DisplayFormat.Format = new CustomerOrderTypeFormatter();

                this.TranslateLabels();
                this.RefreshGrid();
            }

            base.OnLoad(e);
        }

        // See PS#3312 - Appears this should be invoked but is not.
        private void TranslateLabels()
        {
            //
            // Get all text through the Translation function in the ApplicationLocalizer
            //
            // TextID's are reserved at 56200 - 56299
            // 
            // The last Text ID in use is:  56211
            //

            // Translate everything
            btnCreatePackSlip.Text  = ApplicationLocalizer.Language.Translate(56218);   //Create packing slip
            btnPrintPackSlip.Text   = ApplicationLocalizer.Language.Translate(56219);   //Print packing slip
            btnCreatePickList.Text  = ApplicationLocalizer.Language.Translate(56104);   //Create picking list
            btnReturn.Text          = ApplicationLocalizer.Language.Translate(56398);   //Return Order
            btnCancelOrder.Text     = ApplicationLocalizer.Language.Translate(56215);   //Cancel order
            btnEdit.Text            = ApplicationLocalizer.Language.Translate(56212);   //View details
            btnPickUp.Text          = ApplicationLocalizer.Language.Translate(56213);   //Pickup order
            btnClose.Text           = ApplicationLocalizer.Language.Translate(56205);   //Close

            colOrderType.Caption        = ApplicationLocalizer.Language.Translate(56216); //Order type
            colStatus.Caption           = ApplicationLocalizer.Language.Translate(56217); //Status
            colSalesOrderID.Caption     = ApplicationLocalizer.Language.Translate(56206); //Sales order
            colCreationDate.Caption     = ApplicationLocalizer.Language.Translate(56207); //Created date and time
            colTotalAmount.Caption      = ApplicationLocalizer.Language.Translate(56210); //Total
            colCustomerAccount.Caption  = ApplicationLocalizer.Language.Translate(56224); //Customer Account
            colCustomerName.Caption     = ApplicationLocalizer.Language.Translate(56225); //Customer
            colReferenceId.Caption      = ApplicationLocalizer.Language.Translate(56235); //Reference
            colEmail.Caption            = ApplicationLocalizer.Language.Translate(56236); //E-mail

            //title
            this.Text = ApplicationLocalizer.Language.Translate(56106); //Sales orders
            lblHeading.Text = ApplicationLocalizer.Language.Translate(56106); //Sales orders

            //Do not allow filtering from the grid UI
            gridView1.OptionsCustomization.AllowFilter = false;
            gridView1.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
        }

        private void btnPgUp_Click(object sender, EventArgs e)
        {
            gridView1.MovePrevPage();
        }

        private void btnPgDown_Click(object sender, EventArgs e)
        {
            gridView1.MoveNextPage();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            gridView1.MovePrev();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            gridView1.MoveNext();
        }

        private void textBoxSearch_Leave(object sender, EventArgs e)
        {
            this.AcceptButton = btnEdit;
        }

        private void textBoxSearch_Enter(object sender, EventArgs e)
        {
            this.AcceptButton = btnSearch;
        }

        private void RefreshGrid()
        {
            try
            {
                this.salesOrders = dataModel.Refresh();
                grSalesOrders.DataSource = this.salesOrders;

                if( this.salesOrders != null && this.salesOrders.Rows.Count == 0 )
                {
                    // There are no sales orders in the database for this customer....
                    SalesOrder.InternalApplication.Services.Dialog.ShowMessage(56123, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);

                // "An error occurred while refreshing the list."
                SalesOrder.InternalApplication.Services.Dialog.ShowMessage(56232, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.EnableButtons();
        }

        private void GetSelectedRow()
        {
            System.Data.DataRow Row = gridView1.GetDataRow(gridView1.GetSelectedRows()[0]);

            this.SelectedSalesOrderId = Row.Field<string>(SalesIdString);

            if (!Enum.TryParse<SalesStatus>(Row[StatusString].ToString(), out selectedOrderStatus))
            {
                this.selectedOrderStatus = SalesStatus.Unknown;
            }

            // CustomerOrderType does not have default, failing if something else.
            this.SelectedOrderType = (CustomerOrderType)Enum.Parse(typeof(CustomerOrderType), Row[OrderTypeString].ToString());

            this.selectedOrderPickupDeliveryMode = Row.Field<string>(DeliveryModeString);
        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (this.salesOrders != null && e != null && e.FocusedRowHandle >= 0 && e.FocusedRowHandle < this.salesOrders.Rows.Count)
            {
                GetSelectedRow();
                this.EnableButtons();
            }
        }
        /// <summary>
        /// Disable buttons in case no order is selected (like after searching with no results)
        /// </summary>
        private void DisableButtons()
        {
            this.btnEdit.Enabled = false;
            this.btnPickUp.Enabled = false;
            this.btnReturn.Enabled = false;
            this.btnCancelOrder.Enabled = false;
            this.btnCreatePickList.Enabled = false;
            this.btnCreatePackSlip.Enabled = false;
            this.btnPrintPackSlip.Enabled = false;
        }

        private void EnableButtons()
        {
            bool enableEdit = false;
            bool enablePickup = false;
            bool enableReturn = false;
            bool enableCancel = false;
            bool enablePickList = false;
            bool enablePackSlip = false;

            bool isSalesOrder = (this.SelectedOrderType == CustomerOrderType.SalesOrder);
            bool isShipping = !ApplicationSettings.Terminal.PickupDeliveryModeCode.Equals(selectedOrderPickupDeliveryMode, StringComparison.OrdinalIgnoreCase);

            switch (selectedOrderStatus)
            {
                case SalesStatus.Created:
                    enableEdit = true;
                    enableCancel = isSalesOrder;
                    enablePickList = isSalesOrder;
                    enablePickup = isSalesOrder;
                    break;

                case SalesStatus.Processing:
                    enableEdit = true;
                    enablePickup = isSalesOrder;
                    enableReturn = isSalesOrder;
                    enablePickList = isSalesOrder;
                    enablePackSlip = isSalesOrder && isShipping;
                    break;

                case SalesStatus.Delivered:
                    enableEdit = true;
                    enablePickup = isSalesOrder;
                    enableReturn = isSalesOrder;
                    enablePickList = isSalesOrder;
                    enablePackSlip = isSalesOrder && isShipping;
                    break;

                case SalesStatus.Invoiced:
                    enableEdit = true;
                    enablePickup = isSalesOrder;
                    enableReturn = isSalesOrder;
                    break;
            }

            // If the list is only for PackSlip creation, disable everything else
            if (this.dataModel is PackslipOrderListModel)
            {
                enableEdit = false;
                enablePickup = false;
                enableReturn = false;
                enableCancel = false;
                
                // Pick/Pack operations are unchanged (enablePickList, enablePackSlip);
            }

            this.btnEdit.Enabled = enableEdit;
            this.btnPickUp.Enabled = enablePickup;
            this.btnReturn.Enabled = enableReturn;
            this.btnCancelOrder.Enabled = enableCancel;
            this.btnCreatePickList.Enabled = enablePickList;
            this.btnCreatePackSlip.Enabled = enablePackSlip;
            this.btnPrintPackSlip.Enabled = enablePackSlip;
        }

        private void SetSelectedOrderAndClose(CustomerOrderTransaction transaction)
        {
            if (transaction != null)
            {               
                this.SelectedOrder = transaction;
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                Close();
            }
        }

        private void SetSearchFilter(string p)
        {
            string filter = string.Empty;

            if (!string.IsNullOrWhiteSpace(p))
            {
                filter = string.Format(FilterFormat, p);
            }
            gridView1.ActiveFilterString = filter;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SetSearchFilter(textBoxSearch.Text.Trim());
            //Check if we have results after searching
            if (gridView1.DataRowCount > 0)
            {
                GetSelectedRow();
                this.EnableButtons();
            }
            else
            {
                //Disable buttons as no order matches the search criteria
                this.DisableButtons();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            SetSearchFilter(string.Empty);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //Get order
            CustomerOrderTransaction cot = SalesOrderActions.GetCustomerOrder(this.SelectedSalesOrderId, this.SelectedOrderType, LSRetailPosis.Transaction.CustomerOrderMode.Edit);
            SalesOrderActions.ShowOrderDetails(cot, OrderDetailsSelection.ViewDetails); 
            SetSelectedOrderAndClose(cot);
                            
        }

        private void btnPickUp_Click(object sender, EventArgs e)
        {
            //Get order
            //set Mode = Pickup
            CustomerOrderTransaction cot = SalesOrderActions.GetCustomerOrder(this.SelectedSalesOrderId, this.SelectedOrderType, LSRetailPosis.Transaction.CustomerOrderMode.Pickup);
            SalesOrderActions.ShowOrderDetails(cot, OrderDetailsSelection.PickupOrder);
            SetSelectedOrderAndClose(cot);
        }

        private void btnCancelOrder_Click(object sender, EventArgs e)
        {
            //Get order
            //set Mode = Cancel
            CustomerOrderTransaction cot = SalesOrderActions.GetCustomerOrder(this.SelectedSalesOrderId, this.SelectedOrderType, LSRetailPosis.Transaction.CustomerOrderMode.Cancel);
            
            if(cot.OrderStatus == SalesStatus.Processing)
            {
                //Order cannot be cancelled at this time from POS
                SalesOrder.InternalApplication.Services.Dialog.ShowMessage(56237, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
                
            SalesOrderActions.ShowOrderDetails(cot, OrderDetailsSelection.CancelOrder);
            SetSelectedOrderAndClose(cot);                    
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            //Get the order from the grid
            CustomerOrderTransaction cot = SalesOrderActions.GetCustomerOrder(this.SelectedSalesOrderId, this.SelectedOrderType, LSRetailPosis.Transaction.CustomerOrderMode.Edit);
            
            //Now get an invoice from the order
            cot = SalesOrderActions.ReturnOrderInvoices(cot);
            SetSelectedOrderAndClose(cot);
        }

        private void btnCreatePickList_Click(object sender, EventArgs e)
        {
            SalesOrderActions.TryCreatePickListForOrder(this.selectedOrderStatus, this.SelectedSalesOrderId);
            RefreshGrid();
        }

        private void btnCreatePackSlip_Click(object sender, EventArgs e)
        {
            SalesOrderActions.TryCreatePackSlip(this.selectedOrderStatus, this.SelectedSalesOrderId);
            RefreshGrid();
            GetSelectedRow();  // to reload "selectedOrderStatus" object.
        }

        private void btnPrintPackSlip_Click(object sender, EventArgs e)
        {
            //to call pack Slip Method
            SalesOrderActions.TryPrintPackSlip(this.selectedOrderStatus, this.SelectedSalesOrderId);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal abstract class OrderListModel
    {
        internal abstract DataTable Refresh();
    }

    /// <summary>
    /// 
    /// </summary>
    internal class PackslipOrderListModel : OrderListModel
    {
        private string customerId = string.Empty;

        public PackslipOrderListModel(string customerId)
        {
            this.customerId = customerId;
        }

        internal override DataTable Refresh()
        {
            bool retVal;
            string comment;
            DataTable salesOrders = null;

            try
            {
                // Begin by checking if there is a connection to the Transaction Service
                if (SalesOrder.InternalApplication.TransactionServices.CheckConnection())
                {
                    // Publish the Sales order to the Head Office through the Transaction Services...
                    SalesOrder.InternalApplication.Services.SalesOrder.GetCustomerOrdersForPackSlip(out retVal, out comment, ref salesOrders, customerId);
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
                throw new LSRetailPosis.PosisException(52300, x);
            }
            return salesOrders;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class SearchOrderListModel : OrderListModel
    {
        private string customerId = string.Empty;
        private string orderId = string.Empty;
        DateTime? startDate = null;
        DateTime? endDate = null;
		int? resultMaxCount;

		public SearchOrderListModel(string customerSearchTerm, string orderSearchTerm, DateTime? startDateTerm, DateTime? endDateTerm, int? resultMaxCount)
        {
            this.customerId = customerSearchTerm;
            this.orderId = orderSearchTerm;
            this.startDate = startDateTerm;
            this.endDate = endDateTerm;
			this.resultMaxCount = resultMaxCount;
        }

        internal override DataTable Refresh()
        {
			DataTable salesOrders = SalesOrderActions.GetOrdersList(this.customerId, this.orderId, this.startDate, this.endDate, this.resultMaxCount);
            return salesOrders;
        }
    }
}
