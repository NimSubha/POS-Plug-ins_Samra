/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System.ComponentModel.Composition;
using System.Text;
using System.Windows.Forms;
using LSRetailPosis;
using LSRetailPosis.Settings.FunctionalityProfiles;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.BusinessObjects;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;

using BlankOperations;
using System;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch;
using System.Data;
using System.IO;
using LSRetailPosis.Transaction;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction.Line.SaleItem;
using Microsoft.CSharp.RuntimeBinder;
using LSRetailPosis.DataAccess.DataUtil;
using System.Collections;
using BlankOperations.WinFormsTouch;
//using Microsoft.Dynamics.Retail.Pos.Printing;
using Microsoft.Dynamics.Retail.Notification.Contracts;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.Report;
using Microsoft.Dynamics.Retail.Pos.Contracts.BusinessLogic;
using LSRetailPosis.DataAccess;
using System.Collections.ObjectModel;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Imaging;
using DM = Microsoft.Dynamics.Retail.Pos.DataManager;
using System.Globalization;
using System.Collections.Generic;
using DecimalToText;
using Microsoft.Reporting.WinForms;
using System.Drawing.Printing;



namespace Microsoft.Dynamics.Retail.Pos.BlankOperations
{


    [Export(typeof(IBlankOperations))]
    public sealed class BlankOperations : IBlankOperations
    {
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
                //  InternalApplication = value;
            }
        }
        private DM.CustomerDataManager customerDataManager = new DM.CustomerDataManager(
               LSRetailPosis.Settings.ApplicationSettings.Database.LocalConnection,
               LSRetailPosis.Settings.ApplicationSettings.Database.DATAAREAID);
        string sSaleItem = string.Empty;
        private IList<Stream> m_streams;
        private int currentPageIndex;


        #region enum CRWRetailDiscPermission
        enum CRWRetailDiscPermission // added on 29/08/2014
        {
            Cashier = 0,
            Salesperson = 1,
            Manager = 2,
            Other = 3,
        }
        #endregion

        enum Salutation
        {
            Dr = 0,
            Mr = 1,
            Miss = 2,
            Mrs = 3,
            Ms = 4,
            None = 5,
        }
        DataTable dtSalutation = new DataTable();

        public string sPincode { get; set; }

        //private ISuspendRetrieveSystem CustomerSystem
        //{
        //    get { return this.Application.BusinessLogic.SuspendRetrieveSystem; }
        //}
        //  internal static IApplication InternalApplication { get; private set; }



        // Get all text through the Translation function in the ApplicationLocalizer
        // TextID's for BlankOperations are reserved at 50700 - 50999

        #region IBlankOperations Members
        /// <summary>
        /// Displays an alert message according operation id passed.
        /// </summary>
        /// <param name="operationInfo"></param>
        /// <param name="posTransaction"></param>        
        /// 


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Grandfather")]
        public void BlankOperation(IBlankOperationInfo operationInfo, IPosTransaction posTransaction)
        {

            string sCustAc = string.Empty;
            RetailTransaction retailTrans = posTransaction as RetailTransaction;
            SqlConnection connection = new SqlConnection();
            DataTable dt = new DataTable();


            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            switch (operationInfo.OperationId)
            {
                case "CUSTORDR":
                    #region CUSTORDR
                    if (CheckTerminal(ApplicationSettings.Terminal.TerminalId))
                    {
                        frmCustomerOrder objCustOrdr = new frmCustomerOrder(posTransaction, application);
                        objCustOrdr.ShowDialog();

                        if (objCustOrdr.bDataSaved)
                        {
                            if (objCustOrdr.iIsCustOrderWithAdv == 1)
                            {
                                SaveCustomerOrderDepositDetails(Convert.ToString(objCustOrdr.sCustOrder), Convert.ToString(objCustOrdr.sTotalAmt));
                                Application.RunOperation(PosisOperations.Customer, objCustOrdr.sCustAcc);
                                Application.RunOperation(PosisOperations.CustomerAccountDeposit, string.Empty);
                            }
                        }
                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(" Terminal ID : " + ApplicationSettings.Terminal.TerminalId + " set up not completed.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            operationInfo.OperationHandled = false;
                        }

                    }
                    break;

                    #endregion
                case "REPAIRORDR":
                    #region Repair
                    frmRepair objRepair = new frmRepair(posTransaction, application);
                    objRepair.ShowDialog();
                    if (objRepair.bDataSaved && AskUserToCustomerDeposit())
                    {
                        Application.RunOperation(PosisOperations.Customer, objRepair.sCustAcc);
                        SaveRepairOrderDepositDetails(Convert.ToString(objRepair.sCustOrder), Convert.ToString(objRepair.sTotalAmt));
                        Application.RunOperation(PosisOperations.CustomerAccountDeposit, string.Empty);
                    }
                    break;
                    #endregion
                case "REPAIRRETORDR":
                    #region commented
                    //if(retailTrans != null)
                    //{
                    //    if(Convert.ToString(retailTrans.Customer.CustomerId) == string.Empty)
                    //    {
                    //        using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please select customer", MessageBoxButtons.OK, MessageBoxIcon.Error))
                    //        {
                    //            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    //        }
                    //        operationInfo.OperationHandled = false; 
                    //    }

                    //    sCustAc = Convert.ToString(retailTrans.Customer.CustomerId);
                    //    DataTable dtReturn = new DataTable();
                    //    DataRow selRow = null;
                    //    string sRepairId = "";

                    //    dtReturn = GetRepairReturnData("", sCustAc);

                    //    if(dtReturn != null && dtReturn.Rows.Count > 0)
                    //    {
                    //        Dialog.Dialog oDialog = new Dialog.Dialog();
                    //        oDialog.GenericSearch(dtReturn, ref selRow, "Ornament Repair Return");

                    //        if(selRow != null)
                    //        {
                    //            DataTable dtSelect = new DataTable();
                    //            sRepairId = Convert.ToString(selRow["Repair No"]);

                    //            dtSelect = GetRepairReturnData(sRepairId);
                    //            if(dtSelect != null && dtSelect.Rows.Count > 0)
                    //            {
                    //                frmRepairReturn objRepairReturn = new frmRepairReturn(posTransaction, application, dtSelect.Rows[0]);
                    //                objRepairReturn.ShowDialog();
                    //            }
                    //        }
                    //        else
                    //            operationInfo.OperationHandled = false; 

                    //    }
                    //    else
                    //    {
                    //        using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Order Exists.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                    //        {
                    //            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    //        }
                    //        operationInfo.OperationHandled = false ; 
                    //    }

                    //}
                    //else
                    //{
                    //    using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please select customer", MessageBoxButtons.OK, MessageBoxIcon.Error))
                    //    {
                    //        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    //    }
                    //    operationInfo.OperationHandled = false; 
                    //}


                    #endregion

                    #region REPAIRRETORDR

                    if (retailTrans != null)
                    {
                        if (Convert.ToString(retailTrans.Customer.CustomerId) == string.Empty)
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please select customer", MessageBoxButtons.OK, MessageBoxIcon.Error))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                            operationInfo.OperationHandled = true;
                            return;
                        }

                        sCustAc = Convert.ToString(retailTrans.Customer.CustomerId);
                        DataTable dtReturn = new DataTable();
                        DataRow selRow = null;
                        string sRepairId = "";

                        dtReturn = GetRepairReturnData("", sCustAc);

                        if (dtReturn != null && dtReturn.Rows.Count > 0)
                        {
                            Dialog.Dialog oDialog = new Dialog.Dialog();
                            oDialog.GenericSearch(dtReturn, ref selRow, "Ornament Repair Return");

                            if (selRow != null)
                            {
                                DataTable dtSelect = new DataTable();
                                sRepairId = Convert.ToString(selRow["Repair No"]);

                                dtSelect = GetRepairReturnData(sRepairId);
                                if (dtSelect != null && dtSelect.Rows.Count > 0)
                                {
                                    frmRepairReturn objRepairReturn = new frmRepairReturn(posTransaction, application, dtSelect.Rows[0]);
                                    DialogResult dres = objRepairReturn.ShowDialog();
                                    if (dres == DialogResult.OK)
                                    {
                                        retailTrans.PartnerData.IsRepairRetTrans = true;
                                        retailTrans.PartnerData.RefRepairId = sRepairId;
                                        retailTrans.PartnerData.RepairRetTransId = objRepairReturn.sRepairRetId;

                                        retailTrans.PartnerData.REPAIRIDFORRETURN = sRepairId;
                                        retailTrans.PartnerData.COMMENT = "Repair Return Making Amount";
                                        retailTrans.PartnerData.REPAIRRETMAKINGAMT = objRepairReturn.dMakinCharges;
                                        retailTrans.PartnerData.TransAdjGoldQty = 0; // Avg Gold Rate Adjustment


                                        string sRepairRetAdejItemId = string.Empty;
                                        RepairAdjItemId(ref sRepairRetAdejItemId);
                                        DataTable dtSKU = objRepairReturn.dtRepairReturnCashAdvanceDataSku;

                                        //for making charge 
                                        Application.RunOperation(PosisOperations.ItemSale, AdjustmentItemID());//customer cash advance
                                        Application.RunOperation(PosisOperations.ItemSale, sRepairRetAdejItemId);//repair making charge


                                        if (dtSKU != null) // add 26/09/2014 RH
                                        {
                                            foreach (DataRow dr in dtSKU.Rows)
                                            {
                                                if (!string.IsNullOrEmpty(Convert.ToString(dr["SKUNUMBER"])))
                                                {
                                                    string sSKU = Convert.ToString(dr["SKUNUMBER"]);
                                                    Application.RunOperation(PosisOperations.ItemSale, sSKU);
                                                }
                                            }
                                        }
                                        //if weight loss...return to customer (minus saleline by repairitem from retailparameters table with repairitemconfig rate*wt-)
                                        else
                                        {
                                            if (objRepairReturn.dRepairNettWtDiff < 0)
                                            {//qty will be minus in price.cs
                                                retailTrans.PartnerData.REPAIRIDFORRETURN = sRepairId;
                                                retailTrans.PartnerData.COMMENT = "Return Amount for Deduction of Nett Wt while Repair Return";
                                                retailTrans.PartnerData.REPAIRRETMAKINGAMT = Convert.ToDecimal(GetRepairItemConfigRate(sRepairId)) * objRepairReturn.dRepairNettWtDiff;
                                                Application.RunOperation(PosisOperations.ItemSale, sRepairRetAdejItemId);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                operationInfo.OperationHandled = true;
                                return;
                            }

                        }
                        else
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Order Exists.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                            operationInfo.OperationHandled = true;
                            return;
                        }

                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please select customer", MessageBoxButtons.OK, MessageBoxIcon.Error))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                        operationInfo.OperationHandled = true;
                        return;
                    }
                    break;
                    #endregion
                case "FGRECEIVE":
                    #region FG Received
                    frmFGReceived objFGReceive = new frmFGReceived();
                    objFGReceive.ShowDialog();
                    break;
                    #endregion
                case "FGTRANSFER":
                    #region FG Transferred to company
                    frmFGTransferred objFGTransfer = new frmFGTransferred();
                    objFGTransfer.ShowDialog();
                    break;
                    #endregion
                case "TRANSORDERCREATE":
                    #region Transfer Order Create
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    frmTransferOrder objTransOrderCreate = new frmTransferOrder(connection);
                    objTransOrderCreate.ShowDialog();
                    break;
                    #endregion
                case "TRANSORDRCV":
                    #region Transfer Order Receive
                    frmTransferOrderReceive objTransOrderRcv = new frmTransferOrderReceive();
                    objTransOrderRcv.ShowDialog();
                    break;
                    #endregion
                case "CHANGEAMOUNT":
                    #region CHANGE CUSTOMER DEPOSIT AMOUNT
                    if (Convert.ToString(posTransaction.GetType().Name).ToUpper().Trim() == "CUSTOMERPAYMENTTRANSACTION")
                    {
                        LSRetailPosis.POSProcesses.frmInputNumpad oNum = new LSRetailPosis.POSProcesses.frmInputNumpad(true, true);
                        oNum.Text = "Customer account deposit";
                        oNum.EntryTypes = Contracts.UI.NumpadEntryTypes.Price;

                        oNum.PromptText = "Deposit amount";
                        oNum.ShowDialog();
                        if (!string.IsNullOrEmpty(oNum.InputText))
                        {
                            LSRetailPosis.Transaction.CustomerPaymentTransaction custTrans = posTransaction as LSRetailPosis.Transaction.CustomerPaymentTransaction;


                            custTrans.Amount = Convert.ToDecimal(oNum.InputText);
                            custTrans.CustomerDepositItem.Amount = Convert.ToDecimal(oNum.InputText);
                            custTrans.TransSalePmtDiff = Convert.ToDecimal(oNum.InputText);
                        }
                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Customer Deposit Found.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                    }
                    break;
                    #endregion
                case "SERVICEITEM":
                    #region SERVICE ITEM - AMOUNT ADJUSTMENT
                    //RetailTransaction retailTrans = posTransaction as RetailTransaction;
                    if (retailTrans != null)
                    {
                        if (operationInfo.ReturnItems)
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Adjustment Cannot be returned.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                            operationInfo.OperationHandled = false;
                        }
                        bool isServiceItemExists = false;
                        System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem> saleline = new System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem>(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).SaleItems);
                        if (saleline.Count == 0)
                        {
                            retailTrans.PartnerData.TransAdjGoldQty = 0; // Avg Gold Rate Adjustment
                            string sAdJustItem = AdjustmentItemID();
                            Application.RunOperation(PosisOperations.ItemSale, sAdJustItem);
                            //return;
                        }
                        if (saleline.Count > 0)
                        {
                            foreach (var sale in saleline)
                            {
                                if (sale.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                                {
                                    isServiceItemExists = true;
                                }
                            }
                            if (!isServiceItemExists)
                            {
                                Application.RunOperation(PosisOperations.ItemSale, AdjustmentItemID());
                            }
                            else
                            {
                                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Amount can only be adjusted in the beginning of transaction.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                                {
                                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                }
                                operationInfo.OperationHandled = false;
                            }
                        }

                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Amount cannot be adjusted without any customer. Please select the customer to adjust the Amount.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                        operationInfo.OperationHandled = false;
                    }

                    break;
                    #endregion
                case "GSSMATURITY":
                    #region GSS MATURITY
                    frmGSSMaturity objGSSMaturity = new frmGSSMaturity();
                    objGSSMaturity.ShowDialog();
                    break;
                    #endregion
                case "GSSMATURITYADJ":
                    #region [GSS Maturity Adjustment]
                    if (retailTrans != null)
                    {
                        if (operationInfo.ReturnItems)
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Adjustment Cannot be returned.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                            return;
                        }

                        if (Convert.ToString(retailTrans.Customer.CustomerId) == string.Empty)
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Customer found.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                            return;
                        }
                        if (retailTrans.SaleItems.Count > 0)
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Not a valid Transaction.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                            return;
                        }

                        sCustAc = retailTrans.Customer.CustomerId;

                        string commandText = string.Empty;

                        commandText = " SELECT   GSSACCOUNTOPENINGPOSTED.GSSACCOUNTNO AS [GSSACCOUNTNO.], " +
                                        " DIRPARTYTABLE.NAMEALIAS AS [CUSTOMERNAME],  " +
                                        "  CAST(GSSACCOUNTOPENINGPOSTED.INSTALLMENTAMOUNT AS NUMERIC(28,2)) AS [INSTALLMENTAMOUNT], " +
                                        " ( CASE  WHEN  GSSACCOUNTOPENINGPOSTED.SCHEMETYPE=0 THEN 'FIXED' ELSE 'FLEXIBLE' END) AS [SCHEMETYPE], " +
                                        " GSSACCOUNTOPENINGPOSTED.SCHEMECODE AS [SCHEMECODE], " +
                                        " ( CASE WHEN GSSACCOUNTOPENINGPOSTED.SCHEMEDEPOSITTYPE=0 THEN 'GOLD' ELSE 'AMOUNT' END) AS [DEPOSITTYPE]  " +
                            // " ,GSSACCOUNTOPENINGPOSTED.GSSConfirm AS [GSSCONFIRM] " +
                                        " FROM         DIRPARTYTABLE INNER JOIN " +
                                        " CUSTTABLE ON DIRPARTYTABLE.RECID = CUSTTABLE.PARTY INNER JOIN " +
                                        " GSSACCOUNTOPENINGPOSTED ON CUSTTABLE.ACCOUNTNUM = GSSACCOUNTOPENINGPOSTED.CUSTACCOUNT " +
                                        " WHERE CUSTTABLE.ACCOUNTNUM = '" + retailTrans.Customer.CustomerId + "' AND GSSACCOUNTOPENINGPOSTED.GSSADJUSTED = 0 "; //AND GSSACCOUNTOPENINGPOSTED.GSSADJUSTED = 0

                        if (connection.State == ConnectionState.Closed)
                            connection.Open();


                        SqlCommand cmd = new SqlCommand(commandText, connection);
                        SqlDataAdapter daGSS = new SqlDataAdapter(cmd);
                        DataTable dtGSS = new DataTable();
                        daGSS.Fill(dtGSS);
                        if (dtGSS != null && dtGSS.Rows.Count > 0)
                        {
                            DataRow drGSS = null;

                            Dialog.WinFormsTouch.frmGenericSearch oSearch = new Dialog.WinFormsTouch.frmGenericSearch(dtGSS, drGSS, "GSS Number");
                            oSearch.ShowDialog();
                            drGSS = oSearch.SelectedDataRow;
                            if (drGSS != null)
                            {
                                string sGSSNo = Convert.ToString(drGSS["GSSACCOUNTNO."]);
                            }
                        }
                        else
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No record found.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                        }
                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Customer not found", MessageBoxButtons.OK, MessageBoxIcon.Error))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                        return;
                    }
                    break;
                    #endregion
                case "SALESPERSON":
                    #region SALESPERSON
                    if (retailTrans != null)
                    {
                        if (retailTrans.SaleItems.Count == 0)
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("There are no items in the list.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                            return;
                        }
                        foreach (SaleLineItem saleLineItem in retailTrans.SaleItems)
                        {
                            if (Convert.ToString(operationInfo.ItemLineId) == Convert.ToString(saleLineItem.LineId))
                            {
                                LSRetailPosis.POSProcesses.frmSalesPerson dialog = new LSRetailPosis.POSProcesses.frmSalesPerson();
                                dialog.ShowDialog();
                                saleLineItem.SalesPersonId = dialog.SelectedEmployeeId;
                                saleLineItem.SalespersonName = dialog.SelectEmployeeName;
                            }
                        }
                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("There are no items in the list.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                        return;
                    }
                    break;
                    #endregion
                case "UNLOCKSKU":
                    #region Unlock SKU
                    if (retailTrans == null)
                    {
                        DataTable dtTrans = new DataTable();
                        string sOpTerminals = string.Empty;

                        dt = Application.BusinessLogic.SuspendRetrieveSystem.RetrieveTransactionList(ApplicationSettings.Terminal.StoreId);

                        string sQry = "SELECT isnull(TERMINALID,'') as TERMINALID FROM RETAILTRANSACTION WHERE STOREID = '" + ApplicationSettings.Terminal.StoreId + "'" +
                                      " AND DATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "' AND TERMINALID <> '" + ApplicationSettings.Terminal.TerminalId + "'";

                        if (connection.State == ConnectionState.Closed)
                            connection.Open();

                        SqlCommand cmd = new SqlCommand(sQry, connection);
                        cmd.CommandTimeout = 0;
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dtTrans);

                        if (dtTrans != null && dtTrans.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtTrans.Rows)
                            {
                                sOpTerminals = sOpTerminals + " : " + Convert.ToString(dr[0]);
                            }

                            string sValidMsg = "Please close the Terminal operation (" + sOpTerminals + ") properly before unlock";
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(sValidMsg, MessageBoxButtons.OK, MessageBoxIcon.Information))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                        }
                        else
                        {
                            SKUUnlock objSKUUnlock = new SKUUnlock(connection, dt);
                            objSKUUnlock.ShowDialog();
                        }
                    }
                    break;
                    #endregion
                case "REPORT":
                    #region REPORT
                    frmSearchOrder ofrmSearch = new frmSearchOrder(posTransaction, application, string.Empty);
                    ofrmSearch.ShowDialog();
                    string ordernum = ofrmSearch.sOrderNo;
                    if (!string.IsNullOrEmpty(ordernum))
                    {
                        //DataSet ds = GetCustomerOrderData(ordernum);

                        //FormModulation formMod = new FormModulation(Application.Settings.Database.Connection);
                        //RetailTransaction retailTransaction = null;

                        //FormInfo formInfo = formMod.GetInfoForForm(Microsoft.Dynamics.Retail.Pos.Contracts.Services.FormType.Unknown, false, LSRetailPosis.Settings.HardwareProfiles.Printer.ReceiptProfileId);
                        //formMod.GetTransformedTransaction(formInfo, retailTransaction, ds);

                        //string textForPreview = formInfo.Header;
                        //textForPreview += formInfo.Details;
                        //textForPreview += formInfo.Footer;
                        //textForPreview = textForPreview.Replace("|1C", string.Empty);
                        //textForPreview = textForPreview.Replace("|2C", string.Empty);
                        //frmReportList preview = new frmReportList(textForPreview, null);
                        //preview.ShowDialog();

                        //this.Application.ApplicationFramework.POSShowForm(preview);
                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Order Exists.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                        return;
                    }
                    break;
                    #endregion
                case "SKUDETAILS":
                    #region SKUDETAILS
                    string lsItems = string.Empty;
                    if (Convert.ToString(posTransaction.GetType().Name).ToUpper().Trim() == "CUSTOMERPAYMENTTRANSACTION")
                    {
                        if (string.IsNullOrEmpty(Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.SKUData)))
                        {
                            CustomerDepositSKUDetails oSkuDetails = new CustomerDepositSKUDetails(500, posTransaction, Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OrderNo));
                            oSkuDetails.ShowDialog();

                            lsItems = oSkuDetails.lsitems;
                            DataTable dtSKU = oSkuDetails.dtSkuGridItems;
                            if (dtSKU != null && dtSKU.Rows.Count > 0)
                            {
                                dtSKU.TableName = "Ingredients";

                                MemoryStream mstr = new MemoryStream();
                                dtSKU.WriteXml(mstr, true);
                                mstr.Seek(0, SeekOrigin.Begin);
                                StreamReader sr = new StreamReader(mstr);
                                string sXML = string.Empty;
                                sXML = sr.ReadToEnd();
                                ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.SKUData = sXML;
                            }
                            else
                            {
                                ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.SKUData = string.Empty;
                            }
                        }
                        else
                        {
                            DataSet ds = new DataSet();
                            StringReader reader = new StringReader(Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.SKUData));
                            ds.ReadXml(reader);
                            CustomerDepositSKUDetails oSkuDetails = new CustomerDepositSKUDetails(ds.Tables[0], 500, posTransaction, Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OrderNo));
                            oSkuDetails.ShowDialog();
                            lsItems = oSkuDetails.lsitems;
                            DataTable dtSKU = oSkuDetails.dtSkuGridItems;
                            if (dtSKU != null && dtSKU.Rows.Count > 0)
                            {
                                dtSKU.TableName = "Ingredients";

                                MemoryStream mstr = new MemoryStream();
                                dtSKU.WriteXml(mstr, true);
                                mstr.Seek(0, SeekOrigin.Begin);
                                StreamReader sr = new StreamReader(mstr);
                                string sXML = string.Empty;
                                sXML = sr.ReadToEnd();
                                ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.SKUData = sXML;
                            }
                            else
                            {
                                ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.SKUData = string.Empty;
                            }
                        }
                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ItemIds = lsItems;
                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Customer Deposit Found.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                    }
                    break;
                    #endregion
                case "SALESDETAILS":
                    #region SALESDETAILS
                    //RetailTransaction retailTrans = posTransaction as RetailTransaction;
                    if (retailTrans != null)
                    {
                        if (connection.State == ConnectionState.Closed)
                            connection.Open();

                        foreach (SaleLineItem saleLineItem in retailTrans.SaleItems)
                        {
                            if (Convert.ToString(operationInfo.ItemLineId) == Convert.ToString(saleLineItem.LineId))
                            {
                                try
                                {
                                    DataSet dsIngredients = new DataSet();
                                    if (!string.IsNullOrEmpty(Convert.ToString(saleLineItem.PartnerData.Ingredients)) && Convert.ToString(saleLineItem.PartnerData.Ingredients) != "0")
                                    {
                                        StringReader reader = new StringReader(Convert.ToString(saleLineItem.PartnerData.Ingredients));
                                        dsIngredients.ReadXml(reader);
                                    }
                                    else
                                    {
                                        dsIngredients = null;
                                    }
                                    frmCustomFieldCalculations oCustomCalc =
                                        new frmCustomFieldCalculations(
                                            Convert.ToString(saleLineItem.PartnerData.Pieces),
                                            Convert.ToString(saleLineItem.PartnerData.Quantity),
                                            Convert.ToString(saleLineItem.PartnerData.Rate),
                                            Convert.ToString(saleLineItem.PartnerData.RateType),
                                            Convert.ToString(saleLineItem.PartnerData.MakingRate),
                                            Convert.ToString(saleLineItem.PartnerData.MakingType),
                                            Convert.ToString(saleLineItem.PartnerData.Amount),
                                            Convert.ToString(saleLineItem.PartnerData.MakingDisc),
                                            Convert.ToString(saleLineItem.PartnerData.MakingAmount),
                                            Convert.ToString(saleLineItem.PartnerData.TotalAmount),
                                            Convert.ToString(saleLineItem.PartnerData.TotalWeight),
                                            Convert.ToString(saleLineItem.PartnerData.LossPct),
                                            Convert.ToString(saleLineItem.PartnerData.LossWeight),
                                            Convert.ToString(saleLineItem.PartnerData.ExpectedQuantity),
                                            Convert.ToString(saleLineItem.PartnerData.TransactionType),
                                            Convert.ToString(saleLineItem.PartnerData.OChecked), dsIngredients,
                                            Convert.ToString(saleLineItem.PartnerData.OrderNum),
                                            Convert.ToString(saleLineItem.PartnerData.OrderLineNum),
                                            Convert.ToString(saleLineItem.PartnerData.WastageType),
                                            Convert.ToString(saleLineItem.PartnerData.WastagePercentage),
                                            Convert.ToString(saleLineItem.PartnerData.WastageQty),
                                            Convert.ToString(saleLineItem.PartnerData.WastageAmount),
                                            Convert.ToString(saleLineItem.PartnerData.MakingDiscountType),
                                            Convert.ToString(saleLineItem.PartnerData.MakingTotalDiscount),
                                            Convert.ToString(saleLineItem.PartnerData.Purity),
                                            connection);

                                    oCustomCalc.ShowDialog();
                                }
                                catch (RuntimeBinderException)
                                {
                                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Details Present for this item.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                                    {
                                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("There are no items in the list.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                    }
                    break;
                    #endregion
                case "RELEASESKU":
                    #region RELEASESKU
                    frmSKURelease oRelease = new frmSKURelease(ReleaseSKU(), null, "SKU RELEASE FORM");
                    Application.ApplicationFramework.POSShowForm(oRelease);
                    break;
                    #endregion
                case "MULTIPLEADJUSTMENT":
                    #region Multiple Adjustment
                    if (retailTrans != null)
                    {
                        if (operationInfo.ReturnItems)
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Adjustment Cannot be returned.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                            return;
                        }
                        bool isServiceItemExists = false;

                        System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem> saleline = new System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem>(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).SaleItems);
                        if (saleline.Count == 0)
                        {
                            retailTrans.PartnerData.TransAdjGoldQty = 0;
                            dt = CustomerAdjustment(retailTrans.Customer.CustomerId);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                Dialog.WinFormsTouch.frmGenericSearch oSearch = new Dialog.WinFormsTouch.frmGenericSearch(dt, null, "Customer deposits");
                                this.Application.ApplicationFramework.POSShowForm(oSearch);
                                DataRow dr = null;
                                dr = oSearch.SelectedDataRow;
                                if (dr != null)
                                {
                                    DataTable dtBookedSKU = BookedSKU(Convert.ToString(dr["ORDER"]), Convert.ToString(dr["ACCOUNT"]));
                                    if (dtBookedSKU != null && dtBookedSKU.Rows.Count > 0)
                                    {
                                        retailTrans.PartnerData.SKUBookedItems = true;
                                        retailTrans.PartnerData.SKUBookedItemsExists = "Y";
                                    }
                                    retailTrans.PartnerData.AdjustmentOrderNum = string.IsNullOrEmpty(Convert.ToString(dr["ORDER"])) ? string.Empty : Convert.ToString(dr["ORDER"]);
                                    retailTrans.PartnerData.AdjustmentCustAccount = string.IsNullOrEmpty(Convert.ToString(dr["ACCOUNT"])) ? string.Empty : Convert.ToString(dr["ACCOUNT"]);
                                    DataTable dtNew = new DataTable();
                                    dtNew = CustomerAdvanceData(retailTrans.Customer.CustomerId);
                                    foreach (DataRow drNew in dtNew.Select("ORDERNUM='" + Convert.ToString(dr["ORDER"]) + "' AND  CustomerAccount='" + Convert.ToString(dr["ACCOUNT"]) + "'"))
                                    {
                                        Application.RunOperation(PosisOperations.ItemSale, AdjustmentItemID());
                                    }


                                    DataRow drOrd = null;
                                    drOrd = oSearch.SelectedDataRow;
                                    if (dr != null)
                                    {
                                        if (dtBookedSKU != null && dtBookedSKU.Rows.Count > 0)
                                        {
                                            retailTrans.PartnerData.SKUBookedItems = true;
                                            foreach (DataRow drNew in dtBookedSKU.Rows)
                                            {
                                                Application.RunOperation(PosisOperations.ItemSale, Convert.ToString(drNew["SKUNUMBER"]));
                                            }
                                            retailTrans.PartnerData.SKUBookedItems = false;
                                        }
                                        else
                                        {
                                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Booked SKU found for the customer.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                                            {
                                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                            }
                                        }
                                    }
                                }
                                //return;
                            }
                            else
                            {
                                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Active deposits for this customer.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                                {
                                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                }
                            }
                        }
                        if (saleline.Count > 0)
                        {
                            foreach (var sale in saleline)
                            {
                                if (sale.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                                {
                                    isServiceItemExists = true;
                                }
                            }
                            if (!isServiceItemExists)
                            {
                                dt = CustomerAdjustment(retailTrans.Customer.CustomerId);
                                if (dt != null && dt.Rows.Count > 0)
                                {
                                    Dialog.WinFormsTouch.frmGenericSearch oSearch = new Dialog.WinFormsTouch.frmGenericSearch(dt, null, "Customer Deposit");
                                    this.Application.ApplicationFramework.POSShowForm(oSearch);
                                    DataRow dr = null;
                                    dr = oSearch.SelectedDataRow;
                                    DataTable dtBookedSKU = new DataTable();
                                    if (dr != null)
                                    {
                                        dtBookedSKU = BookedSKU(Convert.ToString(dr["ORDER"]), Convert.ToString(dr["ACCOUNT"]));
                                        if (dtBookedSKU != null && dtBookedSKU.Rows.Count > 0)
                                        {
                                            retailTrans.PartnerData.SKUBookedItems = true;
                                            retailTrans.PartnerData.SKUBookedItemsExists = "Y";
                                        }
                                    }
                                    retailTrans.PartnerData.AdjustmentOrderNum = string.IsNullOrEmpty(Convert.ToString(dr["ORDER"])) ? string.Empty : Convert.ToString(dr["ORDER"]);
                                    retailTrans.PartnerData.AdjustmentCustAccount = string.IsNullOrEmpty(Convert.ToString(dr["ACCOUNT"])) ? string.Empty : Convert.ToString(dr["ACCOUNT"]);
                                    DataTable dtNew = new DataTable();
                                    dtNew = CustomerAdvanceData(retailTrans.Customer.CustomerId);
                                    foreach (DataRow drNew in dtNew.Select("ORDERNUM='" + Convert.ToString(dr["ORDER"]) + "' AND  CustomerAccount='" + Convert.ToString(dr["ACCOUNT"]) + "'"))
                                    {
                                        Application.RunOperation(PosisOperations.ItemSale, AdjustmentItemID());
                                    }

                                    DataRow drOrd = null;
                                    drOrd = oSearch.SelectedDataRow;
                                    if (dr != null)
                                    {
                                        if (dtBookedSKU != null && dtBookedSKU.Rows.Count > 0)
                                        {
                                            retailTrans.PartnerData.SKUBookedItems = true;
                                            foreach (DataRow drNew in dtBookedSKU.Rows)
                                            {
                                                Application.RunOperation(PosisOperations.ItemSale, Convert.ToString(drNew["SKUNUMBER"]));
                                            }
                                            retailTrans.PartnerData.SKUBookedItems = false;
                                        }
                                        else
                                        {
                                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Booked SKU found for the customer.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                                            {
                                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Active deposits for this customer.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                                    {
                                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                    }
                                }
                            }
                            else
                            {
                                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Amount can only be adjusted in the beginning of transaction.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                                {
                                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                }
                            }
                        }
                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Amount cannot be adjusted without any customer. Please select the customer to adjust the Amount.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }

                    }

                    break;
                    #endregion
                case "STOCKTRANSFER":
                    #region STOCK TRANSFER
                    frmStockTransfer objFrmStokTrans = new frmStockTransfer(posTransaction, application);
                    objFrmStokTrans.ShowDialog();
                    break;
                    #endregion
                //Start: RHossain on 17/09/2013
                case "GSSAC":
                    #region GSSAcOpenning
                    frmGSSAcOpenning objGss = new frmGSSAcOpenning();
                    objGss.ShowDialog();
                    break;
                    #endregion
                //End: RHossain on 17/09/2013
                case "LOCALCUST":
                    #region Local Customer
                    if (retailTrans != null)
                    {
                        if (string.IsNullOrEmpty(retailTrans.Customer.CustomerId))
                        {
                            frmAddLocalCustomer objAddLocalCustomer = new frmAddLocalCustomer(retailTrans);
                            objAddLocalCustomer.ShowDialog();
                        }
                        else
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Clear selected customer", MessageBoxButtons.OK, MessageBoxIcon.Information))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                        }
                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("There are no items in the list.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                        return;
                    }
                    break;
                    #endregion
                case "TRANSACTIONREPORT":
                    #region Transaction Report
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    TransReportFrm reportfrm = new TransReportFrm(connection);
                    reportfrm.ShowDialog();
                    break;
                    #endregion
                case "OGPREPORT":
                    #region OGP Report
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    frmR_OldGoldPurchase reportOGP = new frmR_OldGoldPurchase(connection);
                    reportOGP.ShowDialog();
                    break;
                    #endregion
                case "CWSTKREPORT":
                    #region Counter wise Stock Report
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    frmCounterWiseStkReport objStkReport = new frmCounterWiseStkReport(connection);
                    objStkReport.ShowDialog();
                    break;
                    #endregion
                case "CWSTKREPORTDTL":
                    #region Counter wise Stock Detail Report
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    frmCounterWiseStkDtlReport objStkReportDtl = new frmCounterWiseStkDtlReport(connection);
                    objStkReportDtl.ShowDialog();
                    break;
                    #endregion
                case "SALEINV":
                    #region Sales Inv Report
                    Application.RunOperation(PosisOperations.ShowJournal, string.Empty);
                    break;
                    #endregion
                case "CASHREGISTER":
                    #region Cash Register
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    frmCashRegister objCashRegister = new frmCashRegister();
                    objCashRegister.ShowDialog();
                    break;
                    #endregion
                case "TRANSORDRPRINT":
                    #region Print Transfer Order
                    frmTransferOrderPrint objTransOrdrPrint = new frmTransferOrderPrint();
                    objTransOrdrPrint.ShowDialog();
                    break;
                    #endregion
                case "METALRATE":
                    #region Metal Rate
                    frmDisplayMetalRate objMetalRate = new frmDisplayMetalRate();
                    objMetalRate.ShowDialog();
                    break;
                    #endregion
                case "GSSINSREC"://Start: RHossain on 18/09/2013
                    #region GSS INSTALLMENT RECEIPT REPORT
                    if (retailTrans != null)
                    {
                        string strSql = " select TRANSACTIONID, CUSTACCOUNT,GSSNUMBER , CAST(ISNULL(AMOUNT,0) AS DECIMAL(28,2))" +
                                          " AS AMOUNT, CAST(TERMINALID AS NVARCHAR(20)) AS TERMINALID from RETAILDEPOSITTABLE where isnull(GSSNUMBER,'') !=''" +
                                          " AND (CUSTACCOUNT = '" + retailTrans.Customer.CustomerId + "') ";

                        dt = getPaymentTransData(strSql);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            Dialog.WinFormsTouch.frmGenericSearch oSearch = new Dialog.WinFormsTouch.frmGenericSearch(dt, null, "GSS Instalment receipt");
                            oSearch.ShowDialog();
                            DataRow dr = null;
                            dr = oSearch.SelectedDataRow;
                            if (dr != null)
                            {
                                //string ScCode = getSchemeCodeByGssAcc(Convert.ToString(dr["GSSNUMBER"])); 
                                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                                frmR_GSSInstalmentReceipt objRGSS = new frmR_GSSInstalmentReceipt(posTransaction, SqlCon, Convert.ToString(dr["transactionid"]), Convert.ToString(dr["amount"]), Convert.ToString(dr["GSSNUMBER"]), Convert.ToString(dr["TERMINALID"]));
                                objRGSS.ShowDialog();
                            }
                        }
                    }
                    break;
                    #endregion
                //End: RHossain on 20/09/2013  
                case "PRODADVANCEREC": // PRODUCT ADVANCE RECEIPT REPORT //Start: RHossain on 20/09/2013
                    #region PRODUCT ADVANCE RECEIPT REPORT
                    if (retailTrans != null)
                    {
                        string strSql = " select CAST(TRANSACTIONID AS NVARCHAR(30)) AS TRANSACTIONID, CAST(CUSTACCOUNT AS NVARCHAR(30)) AS CUSTACCOUNT, CAST(CAST(ISNULL(AMOUNT,0) AS DECIMAL(28,2)) AS NVARCHAR(50))" +
                                         " AS AMOUNT, CAST(TERMINALID AS NVARCHAR(20)) AS TERMINALID from RETAILDEPOSITTABLE where isnull(GSSNUMBER,'') =''" +
                                         " AND (CUSTACCOUNT = '" + retailTrans.Customer.CustomerId + "') ORDER BY TRANSACTIONID DESC";

                        dt = getPaymentTransData(strSql);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            Dialog.WinFormsTouch.frmGenericSearch oSearch = new Dialog.WinFormsTouch.frmGenericSearch(dt, null, "Product advance receipt");
                            oSearch.ShowDialog();
                            DataRow dr = null;
                            dr = oSearch.SelectedDataRow;
                            if (dr != null)
                            {
                                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                                frmR_ProductAdvanceReceipt objProdAdv = new frmR_ProductAdvanceReceipt(posTransaction, SqlCon, Convert.ToString(dr["transactionid"]),
                                                Convert.ToString(dr["amount"]), Convert.ToString(dr["TERMINALID"]));
                                objProdAdv.ShowDialog();
                            }
                        }
                    }
                    break;
                    #endregion
                case "OGREPORT"://Start: RHossain on 16/04/2014
                    #region OGReport
                    frmOGReport objOg = new frmOGReport(1); //1 for OG Report
                    objOg.ShowDialog();
                    break;
                    #endregion
                //End: RHossain on 16/04/2014
                case "SALESREPORT":
                    #region SalesReport
                    frmOGReport objOGR = new frmOGReport(2);//2 for Sales Report
                    objOGR.ShowDialog();
                    break;
                    #endregion
                case "COUNTERMOVEMENT":
                    #region CounterMovementReport
                    frmOGReport objOGR2 = new frmOGReport(3);//2 for COUNTER MOVEMENT
                    objOGR2.ShowDialog();
                    break;
                    #endregion
                case "GSSACCSTATEMENT":
                    #region GSS Account Statement
                    frmGSSAccStatment objGSSACST = new frmGSSAccStatment();
                    objGSSACST.ShowDialog();
                    break;
                    #endregion
                case "SKUBOOKINGREPORT"://Start: RHossain on 6/05/2014
                    #region SKU Booking Report
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    frmR_SKUBookingReport objSKUBR = new frmR_SKUBookingReport(connection);
                    objSKUBR.ShowDialog();
                    break;
                    #endregion
                case "CUSTOMERFOOTFALL":
                    #region CUSTOMERFOOTFALL
                    frmCustomerFootfall objCF = new frmCustomerFootfall(posTransaction, application);
                    objCF.ShowDialog();
                    break;
                    #endregion
                //End: RHossain on 06/05/2014
                case "SPLINEDISC":
                    #region SPLINEDISC
                    if (retailTrans != null)
                    {
                        if (Convert.ToString(retailTrans.Customer.CustomerId) == string.Empty)
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please select customer", MessageBoxButtons.OK, MessageBoxIcon.Error))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                            operationInfo.OperationHandled = false;
                        }
                        else
                        {
                            if (operationInfo.ReturnItems)
                            {
                                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Discount cannot be apply.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                                {
                                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                }
                                operationInfo.OperationHandled = false;
                            }
                            bool isServiceItemExists = false;
                            System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem> saleline = new System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem>(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).SaleItems);

                            if (saleline.Count > 0)
                            {
                                foreach (SaleLineItem saleLineItem in retailTrans.SaleItems)
                                {
                                    if (Convert.ToString(operationInfo.ItemLineId) == Convert.ToString(saleLineItem.LineId))
                                    {
                                        saleLineItem.PartnerData.IsSpecialDisc = true;
                                    }
                                }

                                Application.RunOperation(PosisOperations.LineDiscountPercent, "");
                            }

                        }
                    }
                    break;
                    #endregion
                case "CUSTOMERFEEDBACK":
                    frmCustomerFeedBack custFeed = new frmCustomerFeedBack();
                    custFeed.ShowDialog();
                    break;
                case "SHOWTOTALDISC":
                    #region Show tot disc
                    decimal dTotDis = 0m;
                    if (retailTrans != null)
                    {
                        System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem> saleline = new System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem>(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).SaleItems);
                        if (saleline.Count > 0)
                        {
                            foreach (var sale in saleline)
                            {
                                if (sale.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service && !sale.Voided)
                                {
                                    dTotDis += decimal.Round((Math.Abs(Convert.ToDecimal(sale.TotalDiscount + sale.LineDiscount))), 2, MidpointRounding.AwayFromZero);
                                }
                            }
                        }
                    }

                    MessageBox.Show("Total Sales Discount is :" + " " + dTotDis);
                    #endregion
                    break;
                case "TOATLVALUECHANGEFORDISC":
                    #region Tot value changes
                    if (retailTrans != null)
                    {
                        decimal dTotChangedValue = 0m;
                        decimal dTotLineGrossAmt = 0;

                        foreach (SaleLineItem saleLineItem in retailTrans.SaleItems)
                        {
                            if (saleLineItem.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                            {
                                dTotLineGrossAmt +=Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                            }
                        }

                        frmTotalValueChanges frmTotV = new frmTotalValueChanges(dTotLineGrossAmt);
                        frmTotV.ShowDialog();

                        dTotChangedValue = frmTotV.dTotAmt;

                        if (dTotChangedValue > 0)
                        {
                            if (dTotChangedValue < Math.Abs(dTotLineGrossAmt))
                            {
                                decimal dTotDiscAmt = dTotLineGrossAmt - dTotChangedValue;
                                retailTrans.PartnerData.TotalValueChanged = true;

                                retailTrans.ClearAllDiscounts();
                                Application.RunOperation(PosisOperations.TotalDiscountAmount, dTotDiscAmt);
                               
                            }
                            else
                            {
                                MessageBox.Show("Total value can not exceeded from total due balance amount");
                            }
                        }
                    }

                    #endregion
                    break;
                case "LINEVALUECHANGEFORDISC":
                    #region Line value changes
                    if (retailTrans != null)
                    {
                        decimal dTotChangedValue = 0m;
                        decimal dTotLineGrossAmt = 0;

                        foreach (SaleLineItem saleLineItem in retailTrans.SaleItems)
                        {
                            if (Convert.ToString(operationInfo.ItemLineId) == Convert.ToString(saleLineItem.LineId))
                            {
                                if (saleLineItem.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                                {
                                    dTotLineGrossAmt = Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);

                                    frmTotalValueChanges frmTotV = new frmTotalValueChanges(dTotLineGrossAmt);
                                    frmTotV.ShowDialog();

                                    dTotChangedValue = frmTotV.dTotAmt;

                                    if (dTotChangedValue > 0)
                                    {
                                        if (dTotChangedValue < Math.Abs(dTotLineGrossAmt))
                                        {
                                            decimal dTotDiscAmt = dTotLineGrossAmt - dTotChangedValue;
                                            retailTrans.PartnerData.TotalValueChanged = true;

                                            //retailTrans.ClearAllDiscounts();
                                            saleLineItem.ClearCustomerDiscountLines(true);
                                            Application.RunOperation(PosisOperations.LineDiscountAmount, dTotDiscAmt);

                                        }
                                        else
                                        {
                                            MessageBox.Show("Changed value can not exceeded from total line amount");
                                        }
                                    }
                                }
                            }
                        }


                       
                    }

                    #endregion
                    break;
                default:
                    #region default
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please pass the correct parameters.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    }
                    break;
                    #endregion
            }
            operationInfo.OperationHandled = true;
        }
        #endregion

        #region - Changed By Nimbus - FOR AMOUNT ADJUSTMENT
        private DataRow AmountToBeAdjusted(string custAccount)
        {
            SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
            SqlCon.Open();

            SqlCommand SqlComm = new SqlCommand();
            SqlComm.Connection = SqlCon;
            SqlComm.CommandType = CommandType.Text;
            SqlComm.CommandText = " SELECT     RETAILTRANSACTIONTABLE.TRANSACTIONID AS [Transaction ID], RETAILTRANSACTIONTABLE.CUSTACCOUNT AS [Customer Account], " +
                " DIRPARTYTABLE.NAMEALIAS AS [Customer Name], CAST(SUM(RETAILTRANSACTIONPAYMENTTRANS.AMOUNTCUR) AS NUMERIC(28,3)) AS [Total Amount] " +
                " FROM DIRPARTYTABLE INNER JOIN CUSTTABLE ON DIRPARTYTABLE.RECID = CUSTTABLE.PARTY INNER JOIN " +
                " RETAILTRANSACTIONTABLE INNER JOIN RETAILTRANSACTIONPAYMENTTRANS ON RETAILTRANSACTIONTABLE.TRANSACTIONID = RETAILTRANSACTIONPAYMENTTRANS.TRANSACTIONID ON  " +
                " CUSTTABLE.ACCOUNTNUM = RETAILTRANSACTIONTABLE.CUSTACCOUNT WHERE (RETAILTRANSACTIONTABLE.CUSTACCOUNT = '" + custAccount + "') " +
                " GROUP BY RETAILTRANSACTIONTABLE.TRANSACTIONID, RETAILTRANSACTIONTABLE.CUSTACCOUNT,DIRPARTYTABLE.NAMEALIAS ";


            DataRow drSelected = null;
            DataTable AdjustmentDT = new DataTable();


            SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
            SqlDa.Fill(AdjustmentDT);
            if (AdjustmentDT != null && AdjustmentDT.Rows.Count > 0)
            {

                Dialog.WinFormsTouch.frmGenericSearch OSearch = new Dialog.WinFormsTouch.frmGenericSearch(AdjustmentDT, drSelected, "Adjustment");
                OSearch.ShowDialog();
                drSelected = OSearch.SelectedDataRow;
            }
            else
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Active Deposit found for the selected customer.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }
            }
            return drSelected;
        }
        #endregion

        #region Save Customer Order Deposit Details
        private void SaveCustomerOrderDepositDetails(string customerorder, string amt)
        {
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            bool bIsGoldFixing = CheckGoldFixing();

            if (connection.State == ConnectionState.Closed)
                connection.Open();



            // string commandText = " UPDATE RETAILTEMPTABLE SET CUSTORDER=@CUSTORDER,GOLDFIXING=@GOLDFIXING,MINIMUMDEPOSITFORCUSTORDER=@MINAMT WHERE ID=1 ";
            string commandText = " UPDATE RETAILTEMPTABLE SET CUSTORDER=@CUSTORDER,GOLDFIXING=@GOLDFIXING,MINIMUMDEPOSITFORCUSTORDER=@MINAMT WHERE ID=1 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE

            SqlCommand command = new SqlCommand(commandText, connection);
            command.Parameters.Clear();
            command.Parameters.Add("@CUSTORDER", SqlDbType.NVarChar).Value = customerorder;


            if (bIsGoldFixing)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please select your option for Gold Fixing.", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    command.Parameters.Add("@GOLDFIXING", SqlDbType.Bit).Value = Convert.ToString(dialog.DialogResult).ToUpper().Trim() == "YES" ? "True" : "False";
                }
            }
            else
            {
                command.Parameters.Add("@GOLDFIXING", SqlDbType.Bit).Value = "False";
            }

            command.Parameters.Add("@MINAMT", SqlDbType.Decimal).Value = Convert.ToDecimal(amt);
            command.ExecuteNonQuery();

            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

        private bool CheckTerminal(string sTerminalID)
        {
            bool bReturn = false;
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string commandText = " SELECT TERMINALID FROM RETAILTEMPTABLE WHERE TERMINALID = '" + sTerminalID + "' "; // RETAILTEMPTABLE


            SqlCommand command = new SqlCommand(commandText, connection);

            string sResult = Convert.ToString(command.ExecuteScalar());
            connection.Close();
            if (!string.IsNullOrEmpty(sResult))
            {
                bReturn = true;
            }
            return bReturn;
        }

        private bool CheckGoldFixing() // ADDED ON 09/04/2015
        {
            bool bReturn = false;
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if (connection.State == ConnectionState.Closed)
                connection.Open();


            string commandText = " SELECT top 1 GOLDFIXING FROM RETAILTEMPTABLE"; // RETAILTEMPTABLE


            SqlCommand command = new SqlCommand(commandText, connection);

            string sResult = Convert.ToString(command.ExecuteScalar());
            connection.Close();
            if (!string.IsNullOrEmpty(sResult))
            {
                bReturn = true;
            }
            return bReturn;
        }
        #endregion

        #region GetCustomerOrderData
        private DataSet GetCustomerOrderData(string ordernum)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                //SqlComm.CommandText = "SELECT STAFFID,TERMINALID,ORDERNUM,CUSTACCOUNT,CUSTNAME,ORDERDATE,DELIVERYDATE,TOTALAMOUNT,isDelivered FROM CUSTORDER_HEADER WHERE ORDERNUM='" + ordernum + "'" +
                //    " SELECT [ORDERNUM],[LINENUM],[STOREID],[TERMINALID],[ITEMID],[CONFIGID],[CODE],[SIZEID],[STYLE],[INVENTDIMID],[PCS],[QTY],[CRATE] AS [RATE] " +
                //    " ,[RATETYPE],[AMOUNT],[MAKINGRATE],[MAKINGRATETYPE],[MAKINGAMOUNT],[EXTENDEDDETAILSAMOUNT] FROM [CUSTORDER_DETAILS] WHERE ORDERNUM='" + ordernum + "' " +
                //    " SELECT *,CRATE AS RATE,0 AS MAKINGAMOUNT,0 AS EXTENDEDDETAILSAMOUNT FROM CUSTORDER_SUBDETAILS  WHERE ORDERNUM='" + ordernum + "'";

                SqlComm.CommandText = "SELECT STAFFID,TERMINALID,ORDERNUM,CUSTACCOUNT,CUSTNAME,ORDERDATE,DELIVERYDATE,TOTALAMOUNT,isDelivered,'CustOrdr' as TransType FROM CUSTORDER_HEADER WHERE ORDERNUM='" + ordernum + "'" +
                   " SELECT [ORDERNUM],[LINENUM],[STOREID],[TERMINALID],[ITEMID],[CONFIGID],[CODE],[SIZEID],[STYLE],[INVENTDIMID],[PCS],[QTY],[CRATE] AS [RATE] " +
                   " ,[RATETYPE],[AMOUNT],[MAKINGRATE],[MAKINGRATETYPE],[MAKINGAMOUNT],[EXTENDEDDETAILSAMOUNT],'CustOrdr' as TransType FROM [CUSTORDER_DETAILS] WHERE ORDERNUM='" + ordernum + "' " +
                   " SELECT *,CRATE AS RATE,0 AS MAKINGAMOUNT,0 AS EXTENDEDDETAILSAMOUNT,'CustOrdr' as TransType FROM CUSTORDER_SUBDETAILS  WHERE ORDERNUM='" + ordernum + "'";


                DataSet EmployeeDt = new DataSet();


                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(EmployeeDt);

                return EmployeeDt;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Adjustment Item Name
        private string AdjustmentItemID()
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT TOP(1) ADJUSTMENTITEMID FROM [RETAILPARAMETERS] ");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToString(cmd.ExecuteScalar());
        }
        #endregion

        #region GET SKU DATA FOR RELEASING
        private DataTable ReleaseSKU()
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                // SqlComm.CommandText = "select TRANSID AS [TRANSACTIONID], SKUNUMBER AS [SKU], ISNULL(CONVERT(VARCHAR(10),SKURELEASEDDATE,103),'') AS [RELEASEDATE],REASON FROM retailcustomerdepositskudetails";
                SqlComm.CommandText = "select TRANSID AS [TRANSACTIONID], SKUNUMBER AS [SKU], ISNULL(CONVERT(VARCHAR(10),SKURELEASEDDATE,103),'') AS [RELEASEDATE],REASON FROM retailcustomerdepositskudetails WHERE DELIVERED = 0 AND RELEASED = 0";

                DataTable SKUDt = new DataTable();

                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(SKUDt);

                return SKUDt;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region GET Customer Balance
        private DataSet CustomerBalance()
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = " SELECT     CUSTTRANS.ACCOUNTNUM AS ACCOUNT,DIRPARTYTABLE.NAMEALIAS AS CUSTOMER " +
  " ,CAST(CAST(SUM(CUSTTRANS.AMOUNTCUR) AS NUMERIC(16, 2)) AS VARCHAR(10)) AS BALANCE , CUSTTRANS.CURRENCYCODE AS CURRENCY " +
 " FROM         DIRPARTYTABLE INNER JOIN " +
                      " CUSTTABLE ON DIRPARTYTABLE.RECID = CUSTTABLE.PARTY INNER JOIN " +
                      " CUSTTRANS ON CUSTTABLE.ACCOUNTNUM = CUSTTRANS.ACCOUNTNUM " +
" WHERE (CUSTTRANS.SETTLEAMOUNTCUR = 0) AND (CUSTTRANS.SETTLEAMOUNTMST = 0) " +
" GROUP BY CUSTTRANS.CURRENCYCODE, CUSTTRANS.ACCOUNTNUM, DIRPARTYTABLE.NAMEALIAS ;";

                SqlComm.CommandText += "  SELECT CUSTTRANS.VOUCHER,    CUSTTRANS.ACCOUNTNUM AS ACCOUNT,DIRPARTYTABLE.NAMEALIAS AS CUSTOMER,CUSTTRANS.TRANSTYPE " +
                                     " ,CAST(CAST(SUM(CUSTTRANS.AMOUNTCUR) AS NUMERIC(16, 2)) AS VARCHAR(10)) AS BALANCE , CUSTTRANS.CURRENCYCODE AS CURRENCY " +
                     " FROM         DIRPARTYTABLE INNER JOIN " +
                      " CUSTTABLE ON DIRPARTYTABLE.RECID = CUSTTABLE.PARTY INNER JOIN " +
                      " CUSTTRANS ON CUSTTABLE.ACCOUNTNUM = CUSTTRANS.ACCOUNTNUM " +
                      " WHERE    (CUSTTRANS.SETTLEAMOUNTCUR = 0) AND (CUSTTRANS.SETTLEAMOUNTMST = 0) " +
                      " GROUP BY CUSTTRANS.CURRENCYCODE, CUSTTRANS.ACCOUNTNUM, DIRPARTYTABLE.NAMEALIAS,CUSTTRANS.TRANSTYPE,CUSTTRANS.VOUCHER ";


                DataSet CustBalDt = new DataSet();


                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(CustBalDt);

                return CustBalDt;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region GET Adjustment
        private DataTable CustomerAdjustment(string custAccount)
        {
            try
            {

                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = " SELECT     RETAILADJUSTMENTTABLE.ORDERNUM AS [ORDER], RETAILADJUSTMENTTABLE.CUSTACCOUNT AS ACCOUNT, CAST(CAST(SUM(RETAILADJUSTMENTTABLE.AMOUNT) AS NUMERIC(16, 2)) AS VARCHAR(18))  " +
                    " AS AMOUNT, CAST(CAST(SUM(RETAILADJUSTMENTTABLE.GOLDQUANTITY) AS NUMERIC(16, 3)) AS VARCHAR(18)) AS QUANTITY, DIRPARTYTABLE.NAMEALIAS AS NAME " +
                    " FROM         DIRPARTYTABLE INNER JOIN " +
                    " CUSTTABLE ON DIRPARTYTABLE.RECID = CUSTTABLE.PARTY INNER JOIN " +
                    " RETAILADJUSTMENTTABLE ON CUSTTABLE.ACCOUNTNUM = RETAILADJUSTMENTTABLE.CUSTACCOUNT " +
                    " WHERE RETAILADJUSTMENTTABLE.RETAILDEPOSITTYPE=1 AND RETAILADJUSTMENTTABLE.ISADJUSTED=0 AND  (RETAILADJUSTMENTTABLE.CUSTACCOUNT = '" + custAccount + "') " +
                    " GROUP BY RETAILADJUSTMENTTABLE.ORDERNUM, RETAILADJUSTMENTTABLE.CUSTACCOUNT, DIRPARTYTABLE.NAMEALIAS ";


                DataTable CustBalDt = new DataTable();


                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(CustBalDt);

                return CustBalDt;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region GET Advance Data
        public DataTable CustomerAdvanceData(string custAccount)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = " SELECT  RETAILADJUSTMENTTABLE.ORDERNUM, RETAILADJUSTMENTTABLE.ISADJUSTED,   RETAILADJUSTMENTTABLE.TRANSACTIONID AS [TransactionID], " +
               " RETAILADJUSTMENTTABLE.CUSTACCOUNT AS [CustomerAccount], " +
               " DIRPARTYTABLE.NAMEALIAS AS [CustomerName],   " +
                    // " CAST(RETAILADJUSTMENTTABLE.AMOUNT AS NUMERIC(28,3)) AS [TotalAmount]  " +
               " CAST(RETAILADJUSTMENTTABLE.AMOUNT AS NUMERIC(28,3)) AS [TotalAmount], ISNULL(GOLDFIXING,0) AS GoldFixing,(CASE WHEN GOLDFIXING = 0 THEN 0 ELSE ISNULL(GOLDQUANTITY,0) END) AS GoldQty " + //// Avg Gold Rate Adjustment
               " ,ISNULL(RETAILADJUSTMENTTABLE.RETAILSTOREID,'') AS RETAILSTOREID,ISNULL(RETAILADJUSTMENTTABLE.RETAILTERMINALID,'') AS RETAILTERMINALID" +
               " FROM         DIRPARTYTABLE INNER JOIN " +
               " CUSTTABLE ON DIRPARTYTABLE.RECID = CUSTTABLE.PARTY INNER JOIN " +
               " RETAILADJUSTMENTTABLE ON CUSTTABLE.ACCOUNTNUM = RETAILADJUSTMENTTABLE.CUSTACCOUNT " +
               " WHERE     (RETAILADJUSTMENTTABLE.ISADJUSTED = 0) AND (RETAILADJUSTMENTTABLE.RETAILDEPOSITTYPE = 1) " +
               " AND (RETAILADJUSTMENTTABLE.CUSTACCOUNT = '" + custAccount + "') ";


                DataTable CustBalDt = new DataTable();


                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(CustBalDt);

                return CustBalDt;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region GSS Maturity Adjustment Item ID
        private void GSSAdjustmentItemID(ref string sGSSAdjItemId, ref string sGSSDiscItemId)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT GSSADJUSTMENTITEMID,GSSDISCOUNTITEMID FROM [RETAILPARAMETERS] WHERE DATAAREAID = '" + application.Settings.Database.DataAreaID + "' ");
            DataTable dtGSS = new DataTable();
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            SqlDataAdapter daGss = new SqlDataAdapter(cmd);
            daGss.Fill(dtGSS);

            if (dtGSS != null && dtGSS.Rows.Count > 0)
            {
                sGSSAdjItemId = Convert.ToString(dtGSS.Rows[0]["GSSADJUSTMENTITEMID"]);
                sGSSDiscItemId = Convert.ToString(dtGSS.Rows[0]["GSSDISCOUNTITEMID"]);
            }

        }
        #endregion

        #region GET OrderData
        private DataTable OrderData(string custAccount)
        {
            try
            {

                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = "select CUSTACCOUNT AS ACCOUNT,CUSTNAME AS NAME ,ORDERNUM AS [ORDER],CONVERT(VARCHAR(10),ORDERDATE,103) AS ORDERDATE " +
                                      " ,CONVERT(VARCHAR(10),DELIVERYDATE,103) AS DELIVERYDATE,CAST(TOTALAMOUNT AS NUMERIC(26,2)) AS AMOUNT " +
                                      " FROM CUSTORDER_HEADER WHERE isDelivered=0 AND isConfirmed=1 " +
                                      " AND CUSTACCOUNT='" + custAccount + "' ";


                DataTable CustBalDt = new DataTable();


                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(CustBalDt);

                return CustBalDt;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region GET BookedSKU
        private DataTable BookedSKU(string orderNum, string account)
        {
            try
            {

                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = "select distinct SKUNUMBER FROM RETAILCUSTOMERDEPOSITSKUDETAILS WHERE" +
                                      " ORDERNUMBER='" + orderNum + "' AND CUSTOMERID='" + account + "' AND DELIVERED=0 AND RELEASED = 0";

                DataTable CustBalDt = new DataTable();

                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(CustBalDt);

                return CustBalDt;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region GetRepairData

        private DataSet GetRepairData(string ordernum)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                //SqlComm.CommandText = "SELECT STAFFID,TERMINALID,ORDERNUM,CUSTACCOUNT,CUSTNAME,ORDERDATE,DELIVERYDATE,TOTALAMOUNT,isDelivered FROM CUSTORDER_HEADER WHERE ORDERNUM='" + ordernum + "'" +
                //    " SELECT [ORDERNUM],[LINENUM],[STOREID],[TERMINALID],[ITEMID],[CONFIGID],[CODE],[SIZEID],[STYLE],[INVENTDIMID],[PCS],[QTY],[CRATE] AS [RATE] " +
                //    " ,[RATETYPE],[AMOUNT],[MAKINGRATE],[MAKINGRATETYPE],[MAKINGAMOUNT],[EXTENDEDDETAILSAMOUNT] FROM [CUSTORDER_DETAILS] WHERE ORDERNUM='" + ordernum + "' " +
                //    " SELECT *,CRATE AS RATE,0 AS MAKINGAMOUNT,0 AS EXTENDEDDETAILSAMOUNT FROM CUSTORDER_SUBDETAILS  WHERE ORDERNUM='" + ordernum + "'";

                // consider One item at a time will take for repair 
                SqlComm.CommandText = "SELECT A.RETAILSTAFFID AS STAFFID ,A.RETAILTERMINALID AS TERMINALID, A.RETAILSTOREID AS STOREID,A.REPAIRID AS ORDERNUM,A.CUSTACCOUNT,A.CUSTNAME,A.ORDERDATE,'Repair' as TransType, B.AMOUNT AS [TOTALAMOUNT]" +
                    " FROM RetailRepairHdr A INNER JOIN RetailRepairDetail B ON A.REPAIRID = B.REPAIRID WHERE A.REPAIRID ='" + ordernum + "'" +
                    " SELECT REPAIRID AS [ORDERNUM],[LINENUM],[RETAILSTOREID],[RETAILTERMINALID],[ITEMID],[INVENTDIMID],[PCS],[QTY],[AMOUNT],'Repair' as TransType" +
                    " FROM [RetailRepairDetail] WHERE REPAIRID ='" + ordernum + "' " +
                    " SELECT REPAIRID AS [ORDERNUM],[LINENUM],[LINENUMDTL] AS [ORDERDETAILNUM],[RETAILSTOREID],[RETAILTERMINALID],[ITEMID],[INVENTDIMID],[PCS],[QTY],[AMOUNT],'Repair' as TransType FROM RetailRepairSubDetail  WHERE REPAIRID='" + ordernum + "'";


                DataSet EmployeeDt = new DataSet();


                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(EmployeeDt);

                return EmployeeDt;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region [Repair Return]
        private DataTable GetRepairReturnData(string sRepairId, string sCustAccount = "")
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;

                if (sRepairId == "")
                {
                    SqlComm.CommandText = "SELECT REPAIRID AS [Repair No],OrderDate as [Order Date],CUSTACCOUNT AS [Customer Account], CustName as [Customer Name] from RetailRepairReturnHdr where CUSTACCOUNT = '" + sCustAccount + "' AND IsDelivered = 0";
                }
                else
                {
                    SqlComm.CommandText = "SELECT A.REPAIRID,A.OrderDate,A.CUSTACCOUNT, A.CustName,ISNULL(A.CUSTADDRESS,'') AS CUSTADDRESS,ISNULL(A.CUSTPHONE,'') AS CUSTPHONE,B.ITEMID" +
                        //" ,ISNULL(B.AMOUNT,0) AS ESTAMT,ISNULL(CHARGEBLEAMOUNT,0) AS CHARGEBLEAMOUNT,ISNULL(X.AMOUNT,0) AS ADVAMT," +
                                          " ,ISNULL(X.TRANSACTIONID,'') AS TRANSACTIONID,ISNULL(A.RetailStoreId,'') AS RETAILSTOREID, ISNULL(A.RetailTerminalId,'') AS RETAILTERMINALID" +
                                          " from RetailRepairReturnHdr A INNER JOIN RetailRepairReturnDetail B ON A.REPAIRID = B.REPAIRID" +
                                          " LEFT OUTER JOIN RETAILADJUSTMENTTABLE X ON A.REPAIRID = X.REPAIRID AND X.ISADJUSTED = 0" +
                                          " WHERE A.REPAIRID = '" + sRepairId + "' ";
                }

                DataTable dtRepairRet = new DataTable();

                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(dtRepairRet);

                return dtRepairRet;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region getPaymentTransData
        public DataTable getPaymentTransData(string _sSqlStr)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = _sSqlStr;


                DataTable dtGSS = new DataTable();

                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(dtGSS);

                return dtGSS;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region getSchemeCodeByGssAcc
        private string getSchemeCodeByGssAcc(string sGssAcc)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT SCHEMECODE FROM [GSSACCOUNTOPENINGPOSTED] where GSSACCOUNTNO='" + sGssAcc + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToString(cmd.ExecuteScalar());
        }
        #endregion

        public string GetCompanyName(SqlConnection conn)
        {
            string sCName = string.Empty;

            //string sQry = "SELECT ISNULL(A.NAME,'') FROM DIRPARTYTABLE A INNER JOIN COMPANYINFO B" +
            //    " ON A.RECID = B.RECID WHERE B.DATAAREA = '" + ApplicationSettings.Database.DATAAREAID + "'";
            string sQry = " select P.Name from DIRPARTYTABLE P " +
                         "   left join RETAILCHANNELTABLE c on p.RECID= c.OMOPERATINGUNITID" +
                         "   left join RETAILSTORETABLE s on c.RECID=s.RECID" +
                         "   where s.STORENUMBER = '" + ApplicationSettings.Terminal.StoreId + "'";

            //using (SqlCommand cmd = new SqlCommand(sQry, conn))
            //{
            SqlCommand cmd = new SqlCommand(sQry, conn);
            cmd.CommandTimeout = 0;
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            sCName = Convert.ToString(cmd.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            //}

            return sCName;

        }

        /// <summary>
        /// Purpose : added for dynamically comp logo into report
        /// Created Date :26/08/2014
        /// Created By : RHossain
        /// </summary>
        /// <param name="sTransId"></param>
        /// <returns></returns>
        public byte[] GetCompLogo(string sDataAreaId)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            string commandText = string.Empty;
            //////commandText= ("SELECT top 1 [IMAGE] FROM CompanyImage WHERE DATAAREAID='" + sDataAreaId + "'");
            //////commandText = " select  [IMAGE] from CompanyImage  where refrecid in" +
            //////  " (select RECID from COMPANYINFO where DATAAREA='" + sDataAreaId + "')" +
            //////  " and refcompanyid ='" + sDataAreaId + "'";--and a.refcompanyid='" + sDataAreaId + "'
            //commandText = "select  [IMAGE] from companyimage a" +
            //            " inner join COMPANYINFO b on a.refrecid=b.recid and a.REFTABLEID=b.TABLEID " +
            //            " and b.DATAAREA='" + sDataAreaId + "'";

            commandText = "select  [IMAGE]  from companyimage a " +
            " inner join COMPANYINFO b on a.REFRECID=b.RECID and a.REFCOMPANYID=b.DATAAREA" +
            " and b.DATAAREA='" + sDataAreaId + "'";

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            object sResult = command.ExecuteScalar();

            if (conn.State == ConnectionState.Open)
                conn.Close();

            if (sResult != null)
                return (byte[])sResult;
            else
                return null;

        }

        public string GetCurrencySymbol()
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT SYMBOL FROM CURRENCY WHERE CURRENCYCODE='" + ApplicationSettings.Terminal.StoreCurrency + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
            {
                return sResult.Trim();
            }
            else
            {
                return ApplicationSettings.Terminal.StoreCurrency;
            }
        }

        //public string Amtinwds(double amt)
        //{
        //    object[] words = new object[28];
        //    string Awds = null;
        //    string x = null;
        //    string y = null;
        //    string a = null;
        //    string t = null;
        //    string cror = null;
        //    string lakh = null;
        //    string lak2 = null;
        //    string thou = null;
        //    string tho2 = null;
        //    string hund = null;
        //    string rupe = null;
        //    string rup2 = null;
        //    string pais = null;
        //    string pai2 = null;

        //    words[1] = "One ";
        //    words[2] = "Two ";
        //    words[3] = "Three ";
        //    words[4] = "Four ";
        //    words[5] = "Five ";
        //    words[6] = "Six ";
        //    words[7] = "Seven ";
        //    words[8] = "Eight ";
        //    words[9] = "Nine ";
        //    words[10] = "Ten ";
        //    words[11] = "Eleven ";
        //    words[12] = "Twelve ";
        //    words[13] = "Thirteen ";
        //    words[14] = "Fourteen ";
        //    words[15] = "Fifteen ";
        //    words[16] = "Sixteen ";
        //    words[17] = "Seventeen ";
        //    words[18] = "Eighteen ";
        //    words[19] = "Ninteen ";
        //    words[20] = "Twenty ";
        //    words[21] = "Thirty ";
        //    words[22] = "Forty ";
        //    words[23] = "Fifty ";
        //    words[24] = "Sixty ";
        //    words[25] = "Seventy ";
        //    words[26] = "Eighty ";
        //    words[27] = "Ninety ";

        //    if(amt >= 1)
        //    {
        //        Awds = ApplicationSettings.Terminal.StoreCurrency; //"Rupees "
        //    }
        //    else
        //    {
        //        Awds = ApplicationSettings.Terminal.StoreCurrency; //"Rupee "
        //    }
        //    x = (amt.ToString("0.00")).PadLeft(12, Convert.ToChar("0"));
        //    cror = x.Substring(1, 1);
        //    lakh = x.Substring(2, 2);
        //    lak2 = x.Substring(3, 1);
        //    thou = x.Substring(4, 2);
        //    tho2 = x.Substring(5, 1);
        //    hund = x.Substring(6, 1);
        //    rupe = x.Substring(7, 2);
        //    rup2 = x.Substring(8, 1);
        //    pais = x.Substring(10, 2);
        //    pai2 = x.Substring(11, 1);
        //    y = "";
        //    if(Convert.ToInt32(cror) > 0)
        //    {
        //        y = words[Convert.ToInt32(cror)].ToString() + "crores ";
        //    }
        //    t = Convert.ToString(lakh);
        //    if(Convert.ToInt32(t) > 0)
        //    {
        //        if(Convert.ToInt32(t) > 20)
        //        {
        //            a = lakh.Substring(0, 1);
        //            y = y + words[18 + Convert.ToInt32(a)];
        //            if(Convert.ToInt32(lak2) != 0)
        //                y = y + words[Convert.ToInt32(lak2)];
        //            else
        //                y = y + "";
        //        }
        //        else
        //        {
        //            y = y + words[Convert.ToInt32(t)];
        //        }
        //        y = y + "lakhs ";
        //    }
        //    t = Convert.ToString(thou);
        //    if(Convert.ToInt32(t) > 0)
        //    {
        //        if(Convert.ToInt32(t) > 20)
        //        {
        //            a = thou.Substring(0, 1);
        //            y = y + words[18 + Convert.ToInt32(a)];
        //            if(Convert.ToInt32(tho2) != 0)
        //                y = y + words[Convert.ToInt32(tho2)];
        //            else
        //                y = y + "";
        //        }
        //        else
        //        {
        //            y = y + words[Convert.ToInt32(t)];
        //        }
        //        y = y + "thousand ";
        //    }
        //    if(Convert.ToInt32(hund) > 0)
        //    {
        //        y = y + words[Convert.ToInt32(hund)] + "hundred ";
        //    }
        //    t = Convert.ToString(rupe);
        //    if(Convert.ToInt32(t) > 0)
        //    {
        //        if(Convert.ToInt32(t) > 20)
        //        {
        //            a = rupe.Substring(0, 1);
        //            y = y + words[18 + Convert.ToInt32(a)];
        //            if(Convert.ToInt32(rup2) != 0)
        //                y = y + words[Convert.ToInt32(rup2)];
        //            else
        //                y = y + "";
        //        }
        //        else
        //        {
        //            y = y + words[Convert.ToInt32(t)];
        //        }
        //    }
        //    t = Convert.ToString(pais);
        //    if(Convert.ToInt32(t) > 0)
        //    {
        //        y = y + "paise ";
        //        if(Convert.ToInt32(t) > 20)
        //        {
        //            a = pais.Substring(0, 1);
        //            y = y + words[18 + Convert.ToInt32(a)];
        //            if(Convert.ToInt32(pai2) != 0)
        //                y = y + words[Convert.ToInt32(pai2)];
        //            else
        //                y = y + "";
        //        }
        //        else
        //        {
        //            y = y + words[Convert.ToInt32(t)];
        //        }
        //    }
        //    string amtwrd = "";
        //    if(y.Length > 0)
        //    {
        //        amtwrd = Awds + y + "only ";
        //    }
        //    return amtwrd;
        //}

        public string Amtinwds(double amt)
        {
            MultiCurrency objMulC = null;
            if (Convert.ToString(ApplicationSettings.Terminal.StoreCurrency) != "INR")
                objMulC = new MultiCurrency(Criteria.Foreign);
            else
                objMulC = new MultiCurrency(Criteria.Indian);
            Color cBlack = Color.FromName("Black");

            return ApplicationSettings.Terminal.StoreCurrency + " " + objMulC.ConvertToWord(Convert.ToString(amt)) + " Only"; //GetCurrencyNameWithCode() 
        }

        private string GetCurrencyNameWithCode()
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT TXT FROM CURRENCY WHERE CURRENCYCODE='" + ApplicationSettings.Terminal.StoreCurrency + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
            {
                return sResult.Trim();
            }
            else
            {
                return "-";
            }
        }

        public DataTable GetHeaderInfo(string sOrderNo)
        {
            try
            {

                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = "select ORDERNUM as ORDERNO,CONVERT(VARCHAR(15),ORDERDATE,103) ORDERDATE,CONVERT(VARCHAR(15),DELIVERYDATE,103) DELIVERYDATE,CUSTACCOUNT as CUSTID,CUSTNAME," +
                                      " CUSTADDRESS as CUSTADD,CUSTPHONE,CAST(ISNULL(TOTALAMOUNT,0)AS DECIMAL(18,2)) AS TOTALAMOUNT FROM CUSTORDER_HEADER" +
                                      " WHERE ORDERNUM='" + sOrderNo + "'";

                DataTable CustBalDt = new DataTable();

                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(CustBalDt);
                var SelectedCust = customerDataManager.GetTransactionalCustomer(CustBalDt.Rows[0]["CUSTID"].ToString());
                CustBalDt.Rows[0]["CUSTNAME"] = Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations.GetCustomerNameWithSalutation(SelectedCust);
                CustBalDt.Rows[0]["CUSTADD"] = Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations.AddressLines(SelectedCust);
                sPincode = SelectedCust.PostalCode;
                return CustBalDt;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static string getFullStateName(string sStateId)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;
            string commandText = string.Empty;

            commandText = "SELECT [NAME] FROM [dbo].[LOGISTICSADDRESSSTATE] WHERE [STATEID]=@STATEID";

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            command.Parameters.Add("@STATEID", SqlDbType.VarChar, 10).Value = sStateId;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
            {
                return sResult.Trim().ToUpper();
            }
            else
            {
                return "-";
            }
        }

        private static string GetCountryFullName(string sCountryRegionId, string sLanguageId)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;
            StringBuilder commandText = new StringBuilder();
            if (string.IsNullOrEmpty(sLanguageId))
                sLanguageId = CultureInfo.CurrentUICulture.IetfLanguageTag;
            commandText.Append("select SHORTNAME from LOGISTICSADDRESSCOUNTRYREGIONTRANSLATION where COUNTRYREGIONID='" + sCountryRegionId + "' and LANGUAGEID='" + sLanguageId + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
            {
                return sResult.Trim().ToUpper();
            }
            else
            {
                return "-";
            }
        }

        internal static string AddressLines(LSRetailPosis.Transaction.Customer customer)
        {
            string[] Addresslines = new string[6];

            Addresslines[0] = customer.StreetName;
            Addresslines[1] = customer.City;
            Addresslines[2] = customer.DistrictName;
            Addresslines[3] = BlankOperations.getFullStateName(customer.State);
            Addresslines[4] = BlankOperations.GetCountryFullName(customer.Country, customer.Language);
            Addresslines[5] = customer.PostalCode;
            string sConcatedAddr = null;
            sConcatedAddr += Addresslines[0];
            if (!string.IsNullOrWhiteSpace(sConcatedAddr))
                sConcatedAddr += "\n";
            if (!string.IsNullOrWhiteSpace(sConcatedAddr) && !string.IsNullOrWhiteSpace(Addresslines[1]))
                sConcatedAddr += Addresslines[1];
            if (!string.IsNullOrWhiteSpace(sConcatedAddr) && !string.IsNullOrWhiteSpace(Addresslines[2]))
                sConcatedAddr += ", " + Addresslines[2];
            sConcatedAddr += "\n";
            sConcatedAddr += Addresslines[3];
            if (!string.IsNullOrWhiteSpace(Addresslines[3]))
            {
                if (!string.IsNullOrWhiteSpace(Addresslines[4]))
                    sConcatedAddr += ", " + Addresslines[4];

            }
            return sConcatedAddr;
        }

        private static string GetSalutation(string sCustomerId)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;
            StringBuilder commandText = new StringBuilder();

            commandText.Append("SELECT RETAILSALUTATION from CUSTTABLE WHERE ACCOUNTNUM='" + sCustomerId + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            int iResult = Convert.ToInt16(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            string salutation = Enum.GetName(typeof(Salutation), iResult);

            #region added for Other Salutation @p.jana 18/12/2014
            if (salutation == "")
                salutation = string.Empty;
            #endregion

            return salutation;
        }

        internal static string GetCustomerNameWithSalutation(LSRetailPosis.Transaction.Customer customer)
        {
            string sName = BlankOperations.GetSalutation(customer.CustomerId);
            if (sName == "None" || string.IsNullOrEmpty(sName))
                sName = Convert.ToString(customer.Name).Trim();
            else
                sName = Convert.ToString(sName + " " + customer.Name).Trim();
            return sName;
        }

        public DataTable GetDetailInfo(string sOrderNo)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = "select A.ITEMID as SKUID, A.ITEMID + '-' + F.NAME as ITEMID,PCS,QTY,CRate as RATE,AMOUNT,MAKINGRATE,MAKINGAMOUNT," +
                                      " LineTotalAmt as TOTALAMOUNT,REMARKSDTL as REMARKS,IsBookedSKU as IsBooked,A.CONFIGID,A.CODE,A.SIZEID,A.WastageAmount " +
                                      " AS WastageAmt FROM CUSTORDER_DETAILS A" +
                                      " INNER JOIN INVENTTABLE D ON A.ITEMID = D.ITEMID " +
                                      " LEFT OUTER JOIN ECORESPRODUCT E ON D.PRODUCT = E.RECID " +
                                      " LEFT OUTER JOIN ECORESPRODUCTTRANSLATION F ON E.RECID = F.PRODUCT" +
                                      " WHERE ORDERNUM='" + sOrderNo + "'"; // SKUID for get the itemid for image selection of parent item id of that item

                DataTable CustBalDt = new DataTable();

                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(CustBalDt);

                # region //ADDED ON 28/10/14 RH For image show in sales voucher

                string sItemParentId = string.Empty;
                string sArchivePath = string.Empty;

                DataTable dtDetail = new DataTable();
                dtDetail = CustBalDt;

                dtDetail.Columns.Add("ORDERLINEIMAGE", typeof(string));
                int i = 1;
                foreach (DataRow d in dtDetail.Rows)
                {
                    sArchivePath = GetArchivePathFromImage();
                    string path = sArchivePath + "" + sOrderNo + "_" + i + ".jpeg"; //

                    if (File.Exists(path))
                    {
                        Image img = Image.FromFile(path);
                        byte[] arr;
                        using (MemoryStream ms1 = new MemoryStream())
                        {
                            img.Save(ms1, System.Drawing.Imaging.ImageFormat.Jpeg);
                            arr = ms1.ToArray();
                        }

                        d["ORDERLINEIMAGE"] = Convert.ToBase64String(arr);
                    }
                    else
                    {
                        sSaleItem = Convert.ToString(d["SKUID"]);
                        sItemParentId = GetItemParentId(sSaleItem);

                        if (sItemParentId == "-")
                            sItemParentId = sSaleItem;

                        path = sArchivePath + "" + sItemParentId + "" + ".jpeg"; //

                        if (File.Exists(path))
                        {
                            Image img = Image.FromFile(path);
                            byte[] arr;
                            using (MemoryStream ms1 = new MemoryStream())
                            {
                                img.Save(ms1, System.Drawing.Imaging.ImageFormat.Jpeg);
                                arr = ms1.ToArray();
                            }

                            d["ORDERLINEIMAGE"] = Convert.ToBase64String(arr);
                        }
                        else
                            d["ORDERLINEIMAGE"] = "";

                    }
                    i++;
                }
                dtDetail.AcceptChanges();
                #endregion//end

                return dtDetail;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string GetItemParentId(string sSalesItem)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select ITEMIDPARENT  from INVENTTABLE  where ITEMID='" + sSalesItem + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
            {
                return sResult.Trim();
            }
            else
            {
                return "-";
            }
        }

        public string GetArchivePathFromImage()
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select TOP(1) ARCHIVEPATH  from RETAILSTORETABLE where STORENUMBER='" + ApplicationSettings.Database.StoreID + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
            {
                return sResult.Trim();
            }
            else
            {
                return "-";
            }
        }

        public void NIM_GetOGSketch(string sImage, string sArchivePath, string sPurchInvoiceNo, int iLineId)
        {
            if (!string.IsNullOrEmpty(sImage))
            {
                string path = sArchivePath + "" + sPurchInvoiceNo + "_" + iLineId + ".jpeg"; //

                Bitmap b = new Bitmap(Convert.ToString(sImage));
                b.Save(Convert.ToString(path));
            }
        }

        public void NIM_SaveOrderSampleSketch(string sImage, string sArchivePath, string sPurchInvoiceNo)
        {
            if (!string.IsNullOrEmpty(sImage))
            {
                if (ImageFormat.Jpeg == GetImageFormat(sImage))
                {
                    string path = sArchivePath + "" + sPurchInvoiceNo + "" + ImageFormat.Jpeg;//".jpeg"; //

                    Bitmap b = new Bitmap(Convert.ToString(sImage));
                    b.Save(Convert.ToString(path));
                }
            }
        }

        private static ImageFormat GetImageFormat(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension))
                throw new ArgumentException(
                    string.Format("Unable to determine file extension for fileName: {0}", fileName));

            switch (extension.ToLower()) //.jpg, *.jpeg, *.jpe, *.jfif
            {
                case @".jpg":
                case @".jpe":
                case @".jfif":
                case @".jpeg":
                    return ImageFormat.Jpeg;

                default:
                    throw new NotImplementedException();
            }
        }

        private bool AskUserToCustomerDeposit()
        {
            bool IsDeposit = false;
            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Customer Deposit?", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
            {
                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                if (dialog.DialogResult == DialogResult.Yes)
                {
                    IsDeposit = true;
                }
            }
            return IsDeposit;
        }

        private void SaveRepairOrderDepositDetails(string repairId, string repairAmnt)
        {
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            if (string.IsNullOrWhiteSpace(repairAmnt))
                repairAmnt = "0";

            string commandText = " UPDATE RETAILTEMPTABLE SET CUSTID='REPAIR', CUSTORDER=@REPAIRID,GOLDFIXING=@GOLDFIXING,MINIMUMDEPOSITFORCUSTORDER=@MINAMT WHERE ID=1 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE

            SqlCommand command = new SqlCommand(commandText, connection);
            command.Parameters.Clear();
            command.Parameters.Add("@REPAIRID", SqlDbType.NVarChar).Value = repairId;

            command.Parameters.Add("@GOLDFIXING", SqlDbType.Bit).Value = "False";

            command.Parameters.Add("@MINAMT", SqlDbType.Decimal).Value = Convert.ToDecimal(repairAmnt);
            command.ExecuteNonQuery();
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        private void RepairAdjItemId(ref string NIM_REPAIRADJITEM)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT CRWREPAIRADJITEM FROM [RETAILPARAMETERS] WHERE DATAAREAID = '" + application.Settings.Database.DataAreaID + "' ");
            DataTable dtGSS = new DataTable();
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    NIM_REPAIRADJITEM = Convert.ToString(reader.GetValue(0));
                }
            }
            reader.Close();
            reader.Dispose();
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        private Decimal GetRepairItemConfigRate(string sRepairId)
        {
            decimal dRetAmt = 0M;
            //get rate from db
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            StringBuilder commandText = new StringBuilder();
            commandText.AppendLine(" DECLARE @INVENTLOCATION VARCHAR(20) ");
            commandText.AppendLine(" DECLARE @CONFIGID VARCHAR(20) ");
            commandText.AppendLine(" DECLARE @ITEMID VARCHAR(20) ");
            commandText.AppendLine(" DECLARE @METALTYPE INT  ");
            commandText.AppendLine(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM RETAILCHANNELTABLE INNER JOIN  ");
            commandText.AppendLine(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.AppendLine(" WHERE RETAILSTORETABLE.STORENUMBER='" + ApplicationSettings.Database.StoreID + "' ");
            commandText.AppendLine(" SELECT TOP 1 @CONFIGID=CONFIGID,@ITEMID=ITEMID FROM RETAILREPAIRDETAIL WHERE REPAIRID='" + sRepairId + "'; ");
            commandText.AppendLine(" SELECT TOP 1 @METALTYPE=METALTYPE FROM [INVENTTABLE] WHERE ITEMID=@ITEMID; ");
            commandText.AppendLine(" SELECT TOP 1 CAST(RATES AS NUMERIC(28,2))AS RATES ");
            commandText.AppendLine(" FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION  AND METALTYPE = @METALTYPE AND ACTIVE=1  AND CONFIGIDSTANDARD=@CONFIGID   ");
            commandText.AppendLine(" --AND RATETYPE = 4   ");
            commandText.AppendLine(" ORDER BY DATEADD(SECOND, [TIME], [TRANSDATE]) DESC ");

            using (SqlCommand cmd = new SqlCommand(commandText.ToString(), connection))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            dRetAmt = Convert.ToDecimal(reader.GetValue(reader.GetOrdinal("RATES")));
                        }
                    }
                    reader.Close();
                    reader.Dispose();
                }
            }

            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
            return dRetAmt;
        }

        public DataTable NIM_LoadCombo(string _tableName, string _fieldName, string _condition = null, string _sqlStr = null)
        {
            try
            {
                // Open Sql Connection  
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                // Create a Command  
                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;

                if (!string.IsNullOrEmpty(_sqlStr))
                    SqlComm.CommandText = _sqlStr;
                else
                    SqlComm.CommandText = "select  " + _fieldName + " " +
                                            " FROM " + _tableName + " " +
                                            " " + _condition + " ";

                DataTable dtComboField = new DataTable();
                DataRow row = dtComboField.NewRow();
                dtComboField.Rows.InsertAt(row, 0);

                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(dtComboField);

                return dtComboField;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string AmtinwdsInArabic(double amt)//newAmtToWordsInAra
        {
            MultiCurrency objMulC = null;
            ToArabic objAr = new ToArabic();

            if (Convert.ToString(ApplicationSettings.Terminal.StoreCurrency) != "INR")
                objMulC = new MultiCurrency(Criteria.Foreign);
            else
                objMulC = new MultiCurrency(Criteria.Indian);
            Color cBlack = Color.FromName("Black");

            return objAr.ConvertToText(amt) + "  " + GetCurrencyNameInArabicWithCode();
        }

        private string GetCurrencyNameInArabicWithCode()
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT TXTARABIC FROM CURRENCY WHERE CURRENCYCODE='" + ApplicationSettings.Terminal.StoreCurrency + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
                return sResult.Trim();
            else
                return "";
        }

        #region start :Multi copy print
        private Stream CreateStream(string name, string fileNameExtension, Encoding encoding, string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }

        public void Export(LocalReport report)
        {
            PageSettings m_pageSettings = new PageSettings();
            m_pageSettings.PaperSize = report.GetDefaultPageSettings().PaperSize;

            string deviceInfo =
              "<DeviceInfo>" +
              "  <OutputFormat>EMF</OutputFormat>" +
              "  <PageWidth>5.83in</PageWidth>" +
              "  <PageHeight>8.27in</PageHeight>" +
              "  <MarginTop>0.11811in</MarginTop>" +
              "  <MarginLeft>0.11811in</MarginLeft>" +
              "  <MarginRight>0.11811in</MarginRight>" +
              "  <MarginBottom>0.19685in</MarginBottom>" +
              "</DeviceInfo>";
            Warning[] warnings;
            m_streams = new List<Stream>();
            report.Render("Image", deviceInfo, CreateStream, out warnings);
            foreach (Stream stream in m_streams)
            {
                stream.Position = 0;
            }
        }

        public void ExportForEstimation(LocalReport report)
        {
            string deviceInfo =
              "<DeviceInfo>" +
              "  <OutputFormat>EMF</OutputFormat>" +
              "  <PageWidth>3in</PageWidth>" +
              "  <PageHeight>11in</PageHeight>" +
              "  <MarginTop>0.005in</MarginTop>" +
              "  <MarginLeft>0in</MarginLeft>" +
              "  <MarginRight>0.001in</MarginRight>" +
              "  <MarginBottom>0in</MarginBottom>" +
              "</DeviceInfo>";
            Warning[] warnings;
            m_streams = new List<Stream>();
            report.Render("Image", deviceInfo, CreateStream, out warnings);
            foreach (Stream stream in m_streams)
            {
                stream.Position = 0;
            }
        }
        private void PrintPage(object sender, PrintPageEventArgs ev)
        {
            Metafile pageImage = new
           Metafile(m_streams[currentPageIndex]);

            // Adjust rectangular area with printer margins.
            Rectangle adjustedRect = new Rectangle(
                ev.PageBounds.Left - (int)ev.PageSettings.HardMarginX,
                ev.PageBounds.Top - (int)ev.PageSettings.HardMarginY,
                ev.PageBounds.Width,
                ev.PageBounds.Height);

            // Draw a white background for the report
            ev.Graphics.FillRectangle(Brushes.White, adjustedRect);

            // Draw the report content
            ev.Graphics.DrawImage(pageImage, adjustedRect);

            // Prepare for the next page. Make sure we haven't hit the end.
            currentPageIndex++;
            ev.HasMorePages = (currentPageIndex < m_streams.Count);
        }

        public void Print_Estimation()
        {

            if (m_streams == null || m_streams.Count == 0)
                throw new Exception("Error: no stream to print.");
            PrintDocument printDoc = new PrintDocument();
            if (!printDoc.PrinterSettings.IsValid)
            {
                throw new Exception("Error: cannot find the default printer.");
            }
            else
            {
                try
                {
                    printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
                    currentPageIndex = 0;
                    printDoc.Print();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }
            Dispose();
        }

        public void Print_Invoice(LocalReport report, Int16 iNoOfCopy)
        {

            if (m_streams == null || m_streams.Count == 0)
                throw new Exception("Error: no stream to print.");
            PrintDocument printDoc = new PrintDocument();
            if (!printDoc.PrinterSettings.IsValid)
            {
                throw new Exception("Error: cannot find the default printer.");
            }
            else
            {
                try
                {
                    PageSettings m_pageSettings = new PageSettings();
                    m_pageSettings.PaperSize = report.GetDefaultPageSettings().PaperSize;
                    printDoc.DefaultPageSettings.PaperSize = report.GetDefaultPageSettings().PaperSize;

                    printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
                    currentPageIndex = 0;
                    printDoc.PrinterSettings.Copies = iNoOfCopy;
                    if (iNoOfCopy > 0)
                        printDoc.Print();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }
            Dispose();
        }
        public void Dispose()
        {
            if (m_streams != null)
            {
                foreach (Stream stream in m_streams)
                    stream.Close();
                m_streams = null;
            }
        }
        #endregion start : print
    }
}
