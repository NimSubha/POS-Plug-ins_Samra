//Microsoft Dynamics AX for Retail POS Plug-ins 
//The following project is provided as SAMPLE code. Because this software is "as is," we may not provide support services for it.

using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Triggers;
using System;
using System.Data.SqlClient;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.SaleItem;
using LSRetailPosis.Settings;
using System.Text;
using System.Data;
using System.Windows.Forms;

namespace Microsoft.Dynamics.Retail.Pos.DiscountTriggers
{

    [Export(typeof(IDiscountTrigger))]
    public class DiscountTriggers : IDiscountTrigger
    {

        #region Constructor - Destructor

        public DiscountTriggers()
        {

            // Get all text through the Translation function in the ApplicationLocalizer
            // TextID's for DiscountTriggers are reserved at 53000 - 53999

        }
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
                //InternalApplication = value;
            }
        }
        #endregion


        #region Check for Service Item
        private bool isServiceItem(IPosTransaction transaction, int lineid)
        {
            bool isServiceItemExists = false;
            System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem> saleline = new System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem>(((LSRetailPosis.Transaction.RetailTransaction)(transaction)).SaleItems);
            foreach (var sale in saleline)
            {
                if (sale.ItemType == LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service && !sale.Voided)
                {
                    if (sale.LineId == lineid)
                    {
                        isServiceItemExists = true;
                        break;
                    }
                }
            }
            return isServiceItemExists;
        }
        #endregion

        #region IDiscountTriggersV1 Members

        public void PreLineDiscountAmount(IPreTriggerResult preTriggerResult, IPosTransaction transaction, int LineId)
        {
            LSRetailPosis.ApplicationLog.Log("IDiscountTriggersV1.PreLineDiscountAmount", "Triggered before adding line discount amount.", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PreLineDiscountPercent(IPreTriggerResult preTriggerResult, IPosTransaction transaction, int LineId)
        {
            RetailTransaction retailTrans = transaction as RetailTransaction;

            //if (retailTrans.PartnerData.IsSpecialDisc == false)//IsDisableLineDiscount() &&
            //{
            //    MessageBox.Show("Discoutn not allowed for this item.");
            //    preTriggerResult.ContinueOperation = false;
            //}

            LSRetailPosis.ApplicationLog.Log("IDiscountTriggersV1.PreLineDiscountPercent", "Triggered before adding line discount percentange.", LSRetailPosis.LogTraceLevel.Trace);
        }

        #endregion

        #region IDiscountTriggersV2 Members

        public void PostLineDiscountAmount(IPosTransaction posTransaction, int lineId)
        {
            LSRetailPosis.ApplicationLog.Log("IDiscountTriggersV2.PostLineDiscountAmount", "Triggered after adding line discount amount.", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PostLineDiscountPercent(IPosTransaction posTransaction, int lineId)
        {

            LSRetailPosis.ApplicationLog.Log("IDiscountTriggersV2.PostLineDiscountPercent", "Triggered after adding line discount percentange.", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PreTotalDiscountAmount(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("IDiscountTriggersV2.PreTotalDiscountAmount", "Triggered before total discount amount.", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PostTotalDiscountAmount(IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("IDiscountTriggersV2.PostTotalDiscountAmount", "Triggered after total discount amount.", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PreTotalDiscountPercent(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("IDiscountTriggersV2.PreTotalDiscountPercent", "Triggered before total discount percent.", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PostTotalDiscountPercent(IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("IDiscountTriggersV2.PostTotalDiscountPercent", "Triggered after total discount percent.", LSRetailPosis.LogTraceLevel.Trace);
        }

        #endregion

        private void ClearDiscLines(IPosTransaction posTransaction)
        {
            System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem> saleline = new System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem>(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).SaleItems);
            foreach (var sale in saleline)
            {
                if (sale.ItemType == LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service && !sale.Voided)
                {
                    sale.DiscountLines.Clear();
                    // ((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).SaleItems.Remove(
                }
            }
        }
        

    }
}
