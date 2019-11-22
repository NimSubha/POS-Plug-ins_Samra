using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using LSRetailPosis.Settings;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using LSRetailPosis.Transaction;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using System.Collections.ObjectModel;
using System.IO;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations
{
    public class CustomerAdvanceAdjustment
    {
        IPosTransaction pos;
        public CustomerAdvanceAdjustment(IPosTransaction posTransaction)
        {
            pos = posTransaction;
        }

        #region - Changed By Nimbus - FOR AMOUNT ADJUSTMENT
        public DataRow AmountToBeAdjusted(string custAccount, bool isTranIdExists = false, string custaccount = null, string ordernum = null)
        {
            string TransID = string.Empty;

            #region Multiple Adjustment
            if (custaccount != null && ordernum != null)
            {
                BlankOperations oBlank = new BlankOperations();
                DataTable dt = oBlank.CustomerAdvanceData(custAccount);
                RetailTransaction retailTrans = pos as RetailTransaction;
                string order = Convert.ToString(retailTrans.PartnerData.AdjustmentOrderNum);
                string cust = Convert.ToString(retailTrans.PartnerData.AdjustmentCustAccount);
                DataRow drReturn = null;
                foreach (DataRow drNew in dt.Select("ORDERNUM='" + order + "' AND  CustomerAccount='" + cust + "' AND ISADJUSTED=0"))
                {
                    if (string.IsNullOrEmpty(TransID))
                        TransID = "'" + Convert.ToString(drNew["TransactionID"]) + "'";
                    else
                        TransID += ",'" + Convert.ToString(drNew["TransactionID"]) + "'";
                    drNew["ISADJUSTED"] = 1;
                    drReturn = drNew;
                    break;
                }
                return drReturn;
            }
            #endregion

            #region Single Adjustment
            else
            {
                System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem> saleline = new System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem>(((LSRetailPosis.Transaction.RetailTransaction)(pos)).SaleItems);
                if (isTranIdExists)
                {
                    foreach (var sale in saleline)
                    {
                        if (sale.ItemType == LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem.ItemTypes.Service && !sale.Voided)
                        {
                            if (string.IsNullOrEmpty(TransID))
                                TransID = "'" + sale.PartnerData.ServiceItemCashAdjustmentTransactionID + "'";
                            else
                                TransID += ",'" + sale.PartnerData.ServiceItemCashAdjustmentTransactionID + "'";
                        }

                    }
                }
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                // Create a Command  
                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                DataRow drSelected = null;
                #region Commented qUERY
                //SqlComm.CommandText = " SELECT     RETAILTRANSACTIONTABLE.TRANSACTIONID AS [Transaction ID], RETAILTRANSACTIONTABLE.CUSTACCOUNT AS [Customer Account], " +
                //    " DIRPARTYTABLE.NAMEALIAS AS [Customer Name], CAST(SUM(RETAILTRANSACTIONPAYMENTTRANS.AMOUNTCUR) AS NUMERIC(28,3)) AS [Total Amount] " +
                //    " FROM         DIRPARTYTABLE INNER JOIN CUSTTABLE ON DIRPARTYTABLE.RECID = CUSTTABLE.PARTY INNER JOIN " +
                //    " RETAILTRANSACTIONTABLE INNER JOIN RETAILTRANSACTIONPAYMENTTRANS ON RETAILTRANSACTIONTABLE.TRANSACTIONID = RETAILTRANSACTIONPAYMENTTRANS.TRANSACTIONID ON  " +
                //    " CUSTTABLE.ACCOUNTNUM = RETAILTRANSACTIONTABLE.CUSTACCOUNT WHERE     (RETAILTRANSACTIONTABLE.CUSTACCOUNT = '" + custAccount + "') " +
                //    " AND (RETAILTRANSACTIONPAYMENTTRANS.isAdjusted = 0) AND (RETAILTRANSACTIONTABLE.[TYPE] = 3) ";
                //if (isTranIdExists && !string.IsNullOrEmpty(TransID))
                //{
                //    SqlComm.CommandText += " AND (RETAILTRANSACTIONPAYMENTTRANS.TRANSACTIONID NOT IN (" + TransID + ")) ";
                //}
                //SqlComm.CommandText += " GROUP BY RETAILTRANSACTIONTABLE.TRANSACTIONID, RETAILTRANSACTIONTABLE.CUSTACCOUNT,DIRPARTYTABLE.NAMEALIAS ";
                #endregion

                ////SqlComm.CommandText = " SELECT     RETAILADJUSTMENTTABLE.TRANSACTIONID AS [TransactionID], " +
                ////   " RETAILADJUSTMENTTABLE.CUSTACCOUNT AS [CustomerAccount], " +
                ////   " DIRPARTYTABLE.NAMEALIAS AS [CustomerName],   " +
                ////   //" CAST(RETAILADJUSTMENTTABLE.AMOUNT AS NUMERIC(28,3)) AS [TotalAmount]  " +
                ////   " CAST(RETAILADJUSTMENTTABLE.AMOUNT AS NUMERIC(28,2)) AS [TotalAmount],ISNULL(GOLDFIXING,0) AS GoldFixing,(CASE WHEN GOLDFIXING = 0 THEN 0 ELSE CAST(ISNULL(GOLDQUANTITY,0) AS NUMERIC(28,3)) END) AS GoldQty " +  // Avg Gold Rate Adjustment
                ////   " ,ISNULL(RETAILADJUSTMENTTABLE.RETAILSTOREID,'') AS RETAILSTOREID,ISNULL(RETAILADJUSTMENTTABLE.RETAILTERMINALID,'') AS RETAILTERMINALID" +
                ////   " FROM         DIRPARTYTABLE INNER JOIN " +
                ////   " CUSTTABLE ON DIRPARTYTABLE.RECID = CUSTTABLE.PARTY INNER JOIN " +
                ////   " RETAILADJUSTMENTTABLE ON CUSTTABLE.ACCOUNTNUM = RETAILADJUSTMENTTABLE.CUSTACCOUNT " +
                ////   " WHERE     (RETAILADJUSTMENTTABLE.ISADJUSTED = 0) AND (RETAILADJUSTMENTTABLE.RETAILDEPOSITTYPE = 1) " +
                ////   " AND (RETAILADJUSTMENTTABLE.CUSTACCOUNT = '" + custAccount + "') ";
                try
                {
                    if (PosApplication.Instance.TransactionServices.CheckConnection())
                    {
                        ReadOnlyCollection<object> containerArray;
                        string sStoreId = ApplicationSettings.Terminal.StoreId;
                        containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("getUnadjustedAdvance", custAccount);

                        DataSet dsWH = new DataSet();
                        StringReader srTransDetail = new StringReader(Convert.ToString(containerArray[3]));
                        
                        if (Convert.ToString(containerArray[3]).Trim().Length > 38)
                        {
                            dsWH.ReadXml(srTransDetail);
                        }
                        if (dsWH != null && dsWH.Tables[0].Rows.Count > 0)
                        {
                            Dialog.WinFormsTouch.frmGenericSearch OSearch = new Dialog.WinFormsTouch.frmGenericSearch(dsWH.Tables[0], drSelected, "Advance Adjustment");
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
                    }
                }
                catch (Exception ex)
                {


                }
                return drSelected;


                //DataRow drSelected = null;
                //DataTable AdjustmentDT = new DataTable();


                //SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                //SqlDa.Fill(AdjustmentDT);
                //if (AdjustmentDT != null && AdjustmentDT.Rows.Count > 0)
                //{
                //    Dialog.WinFormsTouch.frmGenericSearch OSearch = new Dialog.WinFormsTouch.frmGenericSearch(AdjustmentDT, drSelected, "Advance Adjustment");
                //    OSearch.ShowDialog();
                //    drSelected = OSearch.SelectedDataRow;

                //    return drSelected;

                //}
                //else
                //{
                //    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Active Deposit found for the selected customer.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                //    {
                //        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                //    }
                //    return null;
                //}
            }
            #endregion
        }
        #endregion
    }
}
