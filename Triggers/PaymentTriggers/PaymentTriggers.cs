//Microsoft Dynamics AX for Retail POS Plug-ins 
//The following project is provided as SAMPLE code. Because this software is "as is," we may not provide support services for it.

using System;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Triggers;
using LSRetailPosis.Transaction;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction.Line.SaleItem;
using System.Data;
using System.Windows.Forms;
using System.Text;

using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Notification.Contracts;

namespace Microsoft.Dynamics.Retail.Pos.PaymentTriggers
{
    [Export(typeof(IPaymentTrigger))]
    public class PaymentTriggers:IPaymentTrigger
    {
        #region enum MetalType
        enum MetalType
        {
            Other = 0,
            Gold = 1,
            Silver = 2,
            Platinum = 3,
            Alloy = 4,
            Diamond = 5,
            Pearl = 6,
            Stone = 7,
            Consumables = 8,
            Watch = 11,
            LooseDmd = 12,
            Palladium = 13,
            Jewellery = 14,
            Metal = 15,
            PackingMaterial = 16,
            Certificate = 17,
        }
        #endregion
        #region Constructor - Destructor

        public PaymentTriggers()
        {

            // Get all text through the Translation function in the ApplicationLocalizer
            // TextID's for PaymentTriggers are reserved at 50400 - 50449

        }

        #endregion

        #region IPaymentTriggers Members

        public void PrePayCustomerAccount(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction, decimal amount)
        {
            LSRetailPosis.ApplicationLog.Log("PaymentTriggers.PrePayCustomerAccount", "Before charging to a customer account", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PrePayCardAuthorization(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction, ICardInfo cardInfo, decimal amount)
        {
            LSRetailPosis.ApplicationLog.Log("PaymentTriggers.PrePayCardAuthorization", "Before the EFT authorization", LSRetailPosis.LogTraceLevel.Trace);
        }

        /// <summary>
        /// <example><code>
        /// // In order to delete the already-added payment you use the following code:
        /// if (retailTransaction.TenderLines.Count > 0)
        /// {
        ///     retailTransaction.TenderLines.RemoveLast();
        ///     retailTransaction.LastRunOpererationIsValidPayment = false;
        /// }
        /// </code></example>
        /// </summary>
        public void OnPayment(IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("PaymentTriggers.OnPayment", "On the addition of a tender...", LSRetailPosis.LogTraceLevel.Trace);
        }

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

            }
        }

        public void PrePayment(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction, object posOperation, string tenderId)
        {
            RetailTransaction retailTransaction = posTransaction as RetailTransaction;
            LSRetailPosis.Transaction.CustomerPaymentTransaction custTrans = posTransaction as LSRetailPosis.Transaction.CustomerPaymentTransaction;
            if(custTrans != null)
            {
                InputConfirmation inputconfirm = new InputConfirmation();
                inputconfirm.PromptText = "Remarks ";
                inputconfirm.InputType = InputType.Normal;

                Interaction.frmInput Oinput = new Interaction.frmInput(inputconfirm);
                Oinput.ShowDialog();
                if (!string.IsNullOrEmpty(Oinput.InputText))
                    custTrans.PartnerData.Remarks = Oinput.InputText;
                else
                    custTrans.PartnerData.Remarks = "";

                if((PosisOperations)posOperation == PosisOperations.PayCreditMemo)
                {
                    preTriggerResult.ContinueOperation = false;
                    return;
                }

                SqlConnection connection = new SqlConnection();

                if(application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;

                Enums.EnumClass oEnum = new Enums.EnumClass();
                string sMaxAmount = string.Empty;
                string sTerminalID = ApplicationSettings.Terminal.TerminalId;
                string sMinAmt = Convert.ToString(oEnum.ValidateMinDeposit(connection, out sMaxAmount, sTerminalID, Convert.ToDecimal((((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction))).CustomerDepositItem.Amount)));
                if(Convert.ToDecimal(sMinAmt) != 0 && Convert.ToDecimal(sMaxAmount) != 0)
                {
                    if(Convert.ToDecimal(sMinAmt) > Convert.ToDecimal((((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction))).CustomerDepositItem.Amount)
                        || Convert.ToDecimal(sMaxAmount) < Convert.ToDecimal((((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction))).CustomerDepositItem.Amount))
                    {
                        preTriggerResult.ContinueOperation = false;
                        preTriggerResult.MessageId = 50448;
                        return;
                    }
                }
            }

            //start : RH on 05/11/2014
            if(retailTransaction != null)
            {

                /* InputConfirmation inC = new InputConfirmation();
                 inC.PromptText = "Remarks";
                 inC.InputType = InputType.Normal;

                 Interaction.frmInput Oinput = new Interaction.frmInput(inC);
                 Oinput.ShowDialog();
                 if(!string.IsNullOrEmpty(Oinput.InputText))
                     retailTransaction.PartnerData.Remarks = Oinput.InputText;
                 else
                     retailTransaction.PartnerData.Remarks = "";*/

                int iPM = 100;
                int iCF = 100;
                int isSale = 0;
                foreach(SaleLineItem saleLineItem in retailTransaction.SaleItems)
                {
                    if(!saleLineItem.Voided)
                    {
                        isSale = 1;
                        iPM = getMetalType(saleLineItem.ItemId);
                        if(iPM == (int)MetalType.PackingMaterial)
                            break;
                    }
                }
                if(isSale == 1 && string.IsNullOrEmpty(retailTransaction.PartnerData.Remarks))
                {
                   
                    InputConfirmation inC = new InputConfirmation();
                    inC.PromptText = "Remarks";
                    inC.InputType = InputType.Normal;

                    Interaction.frmInput Oinput = new Interaction.frmInput(inC);
                    Oinput.ShowDialog();
                    if(!string.IsNullOrEmpty(Oinput.InputText))
                        retailTransaction.PartnerData.Remarks = Oinput.InputText;
                    else
                        retailTransaction.PartnerData.Remarks = "";
                }



                if ((isSale == 1 || retailTransaction.SaleIsReturnSale) && string.IsNullOrEmpty(retailTransaction.PartnerData.TouristNumber))
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Tourist VAT Applicable.", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        //command.Parameters.Add("@GOLDFIXING", SqlDbType.Bit).Value = Convert.ToString(dialog.DialogResult).ToUpper().Trim() == "YES" ? "True" : "False";
                        if (Convert.ToString(dialog.DialogResult).ToUpper().Trim() == "YES")
                        {
                            InputConfirmation inC = new InputConfirmation();
                            inC.PromptText = "Tourist VAT Applicable";
                            inC.InputType = InputType.Normal;
                            inC.MaxLength = 20;

                            Interaction.frmInput Oinput = new Interaction.frmInput(inC);
                            Oinput.ShowDialog();
                            if (!string.IsNullOrEmpty(Oinput.InputText))
                                retailTransaction.PartnerData.TouristNumber = Oinput.InputText;
                            else
                                retailTransaction.PartnerData.TouristNumber = "";
                        }
                    }
                    
                }

               

                string sAdjustmentId = AdjustmentItemID();
                foreach(SaleLineItem SLineItem in retailTransaction.SaleItems)
                {
                    //if(SLineItem.ItemId == sAdjustmentId && retailTransaction.SaleItems.Count == 1)
                    //{
                    //    retailTransaction.RefundReceiptId = "1";
                    //    //break;
                    //}

                    if(SLineItem.ItemId == sAdjustmentId && retailTransaction.SaleItems.Count > 0)
                    {
                        retailTransaction.RefundReceiptId = "1";
                    }
                    if(SLineItem.ItemId != sAdjustmentId)
                    {
                        retailTransaction.RefundReceiptId = "";
                        break;
                    }
                }


                foreach(SaleLineItem saleLineItem in retailTransaction.SaleItems)
                {
                    if(!saleLineItem.Voided)
                    {
                        iCF = getMetalType(saleLineItem.ItemId);
                        if(iCF == (int)MetalType.Certificate)
                            break;
                    }
                }

                foreach(SaleLineItem saleLineItem in retailTransaction.SaleItems)
                {
                    if(saleLineItem.ReturnLineId == 0)
                    {
                        if(retailTransaction.PartnerData.PackingMaterial != "Y")
                        {
                            if(IsRetailItem(saleLineItem.ItemId))
                            {
                                if(iPM != (int)MetalType.PackingMaterial)
                                {
                                    #region Commented
                                    //using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Have you issued packing material?", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                                    //{
                                    //    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                    //    if(Convert.ToString(dialog.DialogResult).ToUpper().Trim() == "NO")
                                    //    {
                                    //        preTriggerResult.ContinueOperation = false;
                                    //        return;
                                    //    }
                                    //    else
                                    //    {
                                    //        retailTransaction.PartnerData.PackingMaterial = "Y";

                                    //        if(IsCertificateItem(saleLineItem.ItemId))
                                    //        {
                                    //            if(iCF != (int)MetalType.Certificate)
                                    //            {
                                    //                using(LSRetailPosis.POSProcesses.frmMessage dialog1 = new LSRetailPosis.POSProcesses.frmMessage("Have you issued the certificate?", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                                    //                {
                                    //                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog1);
                                    //                    if(Convert.ToString(dialog1.DialogResult).ToUpper().Trim() == "NO")
                                    //                    {
                                    //                        preTriggerResult.ContinueOperation = false;
                                    //                        return;
                                    //                    }
                                    //                    else
                                    //                        retailTransaction.PartnerData.CertificateIssue = "Y";
                                    //                }
                                    //            }
                                    //            else
                                    //                retailTransaction.PartnerData.CertificateIssue = "Y";
                                    //        }
                                    //    }
                                    //}
                                    #endregion
                                    using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Proceed without packing material?", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                                    {
                                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                        if(Convert.ToString(dialog.DialogResult).ToUpper().Trim() == "NO")
                                        {
                                            preTriggerResult.ContinueOperation = false;
                                            return;
                                        }
                                    }
                                }
                                else
                                {
                                    retailTransaction.PartnerData.PackingMaterial = "Y";

                                    #region Commented
                                    //if(IsCertificateItem(saleLineItem.ItemId))
                                    //{
                                    //    if(iCF != (int)MetalType.Certificate)
                                    //    {
                                    //        using(LSRetailPosis.POSProcesses.frmMessage dialog1 = new LSRetailPosis.POSProcesses.frmMessage("Have you issued the certificate?", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                                    //        {
                                    //            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog1);
                                    //            if(Convert.ToString(dialog1.DialogResult).ToUpper().Trim() == "NO")
                                    //            {
                                    //                preTriggerResult.ContinueOperation = false;
                                    //                return;
                                    //            }
                                    //            else
                                    //                retailTransaction.PartnerData.CertificateIssue = "Y";
                                    //        }
                                    //    }
                                    //    else
                                    //        retailTransaction.PartnerData.CertificateIssue = "Y";
                                    //}
                                    #endregion
                                }
                            }
                        }
                        #region Commented
                        //    else if(retailTransaction.PartnerData.CertificateIssue != "Y")
                        //    {
                        //        if(IsCertificateItem(saleLineItem.ItemId))
                        //        {
                        //            if(iCF != (int)MetalType.Certificate)
                        //            {
                        //                using(LSRetailPosis.POSProcesses.frmMessage dialog1 = new LSRetailPosis.POSProcesses.frmMessage("Have you issued the certificate?", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                        //                {
                        //                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog1);
                        //                    if(Convert.ToString(dialog1.DialogResult).ToUpper().Trim() == "NO")
                        //                    {
                        //                        preTriggerResult.ContinueOperation = false;
                        //                        return;
                        //                    }
                        //                    else
                        //                        retailTransaction.PartnerData.CertificateIssue = "Y";
                        //                }
                        //            }
                        //            else
                        //                retailTransaction.PartnerData.CertificateIssue = "Y";
                        //        }
                        //    }
                        #endregion
                    }
                }
            }
            // end: RH on 05/11/2014

            LSRetailPosis.ApplicationLog.Log("PaymentTriggers.PrePayment", "On the start of a payment operation...", LSRetailPosis.LogTraceLevel.Trace);

            switch((PosisOperations)posOperation)
            {
                case PosisOperations.PayCash:
                    // Insert code here...
                    break;
                case PosisOperations.PayCard:
                    // Insert code here...
                    break;
                case PosisOperations.PayCheque:
                    // Insert code here...
                    break;
                case PosisOperations.PayCorporateCard:
                    // Insert code here...
                    break;
                case PosisOperations.PayCreditMemo:
                    // Insert code here...
                    break;
                case PosisOperations.PayCurrency:
                    // Insert code here...
                    break;
                case PosisOperations.PayCustomerAccount:
                    // Insert code here...
                    break;
                case PosisOperations.PayGiftCertificate:
                    // Insert code here...
                    break;
                case PosisOperations.PayLoyalty:
                    // Insert code here...
                    break;

                // etc.....
            }
        }

        /// <summary>
        /// Triggered before voiding of a payment.
        /// </summary>
        /// <param name="preTriggerResult"></param>
        /// <param name="posTransaction"></param>
        /// <param name="lineId"> </param>
        public void PreVoidPayment(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction, int lineId)
        {
            LSRetailPosis.ApplicationLog.Log("PaymentTriggers.PreVoidPayment", "Before the void payment operation...", LSRetailPosis.LogTraceLevel.Trace);
        }

        /// <summary>
        /// Triggered after voiding of a payment.
        /// </summary>
        /// <param name="posTransaction"></param>
        /// <param name="lineId"> </param>
        public void PostVoidPayment(IPosTransaction posTransaction, int lineId)
        {
            LSRetailPosis.ApplicationLog.Log("PaymentTriggers.PostVoidPayment", "After the void payment operation...", LSRetailPosis.LogTraceLevel.Trace);
        }

        #endregion

        /// <summary>
        /// retail =1 in inventtable , item can only sale
        /// Dev on  : 17/04/2014 by : RHossain
        /// </summary>
        /// <param name="sItemId"></param>
        /// <returns></returns>
        private bool IsRetailItem(string sItemId)
        {
            bool bRetailItem = false;

            SqlConnection connection = new SqlConnection();

            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "select RETAIL from inventtable WHERE ITEMID = '" + sItemId + "'";


            if(connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            bRetailItem = Convert.ToBoolean(command.ExecuteScalar());
            if(connection.State == ConnectionState.Open)
                connection.Close();

            return bRetailItem;
        }


        private bool IsCertificateItem(string sItemId)
        {
            bool bCertificateItem = false;

            SqlConnection connection = new SqlConnection();

            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "SELECT SKUCERTIFICATE FROM SKUTABLE_POSTED  WHERE SkuNumber = '" + sItemId + "'";


            if(connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            bCertificateItem = Convert.ToBoolean(command.ExecuteScalar());
            if(connection.State == ConnectionState.Open)
                connection.Close();

            return bCertificateItem;
        }

        private int getMetalType(string sItemId)
        {
            int iMetalType = 100;
            SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT METALTYPE FROM INVENTTABLE WHERE ITEMID='" + sItemId + "'");

            if(SqlCon.State == ConnectionState.Closed)
                SqlCon.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), SqlCon);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();
            if(reader.HasRows)
            {
                while(reader.Read())
                {
                    iMetalType = (int)reader.GetValue(0);
                }
            }
            if(SqlCon.State == ConnectionState.Open)
                SqlCon.Close();
            return iMetalType;
        }

        #region Adjustment Item Name
        private string AdjustmentItemID()
        {
            SqlConnection connection = new SqlConnection();
            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT TOP(1) ADJUSTMENTITEMID FROM [RETAILPARAMETERS] ");

            if(connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToString(cmd.ExecuteScalar());
        }
        #endregion
    }
}
