//Microsoft Dynamics AX for Retail POS Plug-ins 
//The following project is provided as SAMPLE code. Because this software is "as is," we may not provide support services for it.

using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Triggers;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System;
using System.Collections;
using LSRetailPosis.Transaction.Line.SaleItem;
using System.Collections.Generic;
using LSRetailPosis.Settings;
using System.IO;
using LSRetailPosis.Transaction;
using BlankOperations.WinFormsTouch;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using Microsoft.Dynamics.Retail.Pos.SystemCore;

namespace Microsoft.Dynamics.Retail.Pos.ItemTriggers
{
    ///  <summary>
    /// <example><code>
    /// // In order to get a copy of the last item added to the transaction, use the following code:
    /// LinkedListNode<SaleLineItem> saleItem = ((RetailTransaction)posTransaction).SaleItems.Last;
    /// // To remove the last line use:
    /// ((RetailTransaction)posTransaction).SaleItems.RemoveLast();
    /// </code></example>
    /// </summary>
    [Export(typeof(IItemTrigger))]
    public class ItemTriggers:IItemTrigger
    {
        string sItemIdParent = string.Empty;
        decimal dGroupCostPrice = 0;
        decimal dUpdatedCostPrice = 0;
        decimal dSellingCostPrice = 0;

        #region enum MetalType
        public enum MetalType
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

        public ItemTriggers()
        {

            // Get all text through the Translation function in the ApplicationLocalizer
            // TextID's for ItemTriggers are reserved at 50350 - 50399
        }

        #endregion

        public bool isMRP(string itemid, SqlConnection connection)
        {
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT TOP(1) MRP FROM [INVENTTABLE] WHERE ITEMID='" + itemid + "' ");


            if(connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToBoolean(cmd.ExecuteScalar());

        }

        [Import]
        private IApplication application;

        #region SKU Operations
        private void CheckForSKUExistence(string itemid, SqlConnection connection, out int? isLock, out int? isAvailable)
        {
            StringBuilder sbQuery = new StringBuilder();

            // sbQuery.Append(" IF EXISTS(SELECT TOP 1 * FROM skutable_posted WHERE SkuNumber='" + itemid + "') ");
            sbQuery.Append(" IF EXISTS(SELECT TOP 1 * FROM SKUTableTrans WHERE SkuNumber='" + itemid + "') ");
            sbQuery.Append(" BEGIN  ");
            // sbQuery.Append(" SELECT isAvailable,isLocked FROM skutable_posted WHERE SkuNumber='" + itemid + "' ");
            sbQuery.Append(" SELECT isAvailable,isLocked FROM SKUTableTrans WHERE SkuNumber='" + itemid + "' ");
            sbQuery.Append(" END ");
            if(connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            SqlDataReader reader = null;
            reader = cmd.ExecuteReader();

            //  return Convert.ToBoolean(cmd.ExecuteScalar());

            if(reader.HasRows)
            {
                bool isAvail = false;
                bool isLocked = false;
                while(reader.Read())
                {
                    isAvail = Convert.ToBoolean(reader.GetValue(0));
                    isLocked = Convert.ToBoolean(reader.GetValue(1));
                }
                reader.Close();
                isLock = Convert.ToInt16(isLocked);
                isAvailable = Convert.ToInt16(isAvail);

            }
            else
            {
                reader.Close();
                isLock = null;
                isAvailable = null;
            }

        }

        private void CheckForReturnSKUExistence(string itemid, SqlConnection connection, out int? isRetunLock, out int? isReturnAvailable)
        {
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append(" DECLARE @RETURN BIT ");
            // sbQuery.Append(" SELECT @RETURN=ITEMRETURN FROM RETAILTEMPTABLE WHERE ID=2 ");
            sbQuery.Append(" SELECT @RETURN=ITEMRETURN FROM RETAILTEMPTABLE WHERE ID=2 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "); // RETAILTEMPTABLE
            sbQuery.Append(" IF(@RETURN=1) ");
            sbQuery.Append(" BEGIN  ");
            //sbQuery.Append(" IF EXISTS(SELECT TOP 1 * FROM skutable_posted WHERE SKUNUMBER='" + itemid + "')  "); 
            sbQuery.Append(" IF EXISTS(SELECT TOP 1 * FROM SKUTableTrans WHERE SKUNUMBER='" + itemid + "')  ");
            sbQuery.Append(" BEGIN  ");
            //sbQuery.Append(" SELECT isLocked,isAvailable FROM skutable_posted WHERE SKUNUMBER='" + itemid + "'  "); 
            sbQuery.Append(" SELECT isLocked,isAvailable FROM SKUTableTrans WHERE SKUNUMBER='" + itemid + "'  ");
            sbQuery.Append(" END  ");
            sbQuery.Append(" END  ");


            if(connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            SqlDataReader reader = null;
            reader = cmd.ExecuteReader();



            if(reader.HasRows)
            {
                bool isAvail = false;
                bool isLocked = false;
                while(reader.Read())
                {
                    isLocked = Convert.ToBoolean(reader.GetValue(0));
                    isAvail = Convert.ToBoolean(reader.GetValue(1));
                }
                reader.Close();
                isRetunLock = Convert.ToInt16(isLocked);
                isReturnAvailable = Convert.ToInt16(isAvail);



            }
            else
            {
                reader.Close();
                isRetunLock = null;
                isReturnAvailable = null;
            }

        }
        #endregion

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

        private void RepairAdjItemId(ref string NIM_REPAIRADJITEM)
        {
            SqlConnection connection = new SqlConnection();
            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT CRWREPAIRADJITEM FROM [RETAILPARAMETERS] WHERE DATAAREAID = '" + application.Settings.Database.DataAreaID + "' ");
            DataTable dtGSS = new DataTable();
            if(connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            SqlDataReader reader = cmd.ExecuteReader();

            if(reader.HasRows)
            {
                while(reader.Read())
                {
                    NIM_REPAIRADJITEM = Convert.ToString(reader.GetValue(0));
                }
            }
            reader.Close();
            reader.Dispose();
            if(connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        #region GSS Maturity Adjustment Item ID
        private void GSSAdjustmentItemID(ref string sGSSAdjItemId, ref string sGSSDiscItemId)
        {
            SqlConnection connection = new SqlConnection();
            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT GSSADJUSTMENTITEMID,GSSDISCOUNTITEMID FROM [RETAILPARAMETERS] WHERE DATAAREAID = '" + application.Settings.Database.DataAreaID + "' ");
            DataTable dtGSS = new DataTable();
            if(connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            SqlDataAdapter daGss = new SqlDataAdapter(cmd);
            daGss.Fill(dtGSS);

            if(dtGSS != null && dtGSS.Rows.Count > 0)
            {
                sGSSAdjItemId = Convert.ToString(dtGSS.Rows[0]["GSSADJUSTMENTITEMID"]);
                sGSSDiscItemId = Convert.ToString(dtGSS.Rows[0]["GSSDISCOUNTITEMID"]);
            }

        }
        #endregion

        #region Changed By Nimbus - Update Order Delivery
        private void updateOrderDelivery(string orderNum, string orderLineNum, bool voided, ISaleLineItem saleline, string itemid = "")
        {
            SqlConnection connection = new SqlConnection();
            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if(connection.State == ConnectionState.Closed)
                connection.Open();
            string commandText = string.Empty;
            if(orderNum == null && orderLineNum == null && !voided && !string.IsNullOrEmpty(itemid))
            {
                //  if (saleline != null && saleline.ZeroPriceValid)
                if(saleline != null)
                {
                    /* //SKU Table New
                    commandText = commandText + " UPDATE SKUTable_Posted SET isLocked='True' " +  
                                              " WHERE SkuNumber='" + itemid + "' " +
                        //  " AND STOREID='" + ApplicationSettings.Terminal.StoreId + "' " +
                                              " AND DATAAREAID='" + application.Settings.Database.DataAreaID + "' ";
                     */
                    commandText = commandText + " UPDATE SKUTableTrans SET isLocked='True' " +
                                              " WHERE SkuNumber='" + itemid + "' " +
                        //  " AND STOREID='" + ApplicationSettings.Terminal.StoreId + "' " +
                                              " AND DATAAREAID='" + application.Settings.Database.DataAreaID + "' ";

                }
                //  commandText = commandText + " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2";
                commandText = commandText + " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE
            }
            else
            {
                if(!voided)
                {
                    commandText = " UPDATE CUSTORDER_DETAILS SET isDelivered = 1 WHERE ORDERNUM='" + orderNum.Trim() + "' AND  LINENUM='" + orderLineNum.Trim() + "' " +
                                  " DECLARE @COUNT INT " +
                                  " SELECT @COUNT=COUNT(LINENUM) FROM CUSTORDER_DETAILS WHERE ORDERNUM='" + orderNum.Trim() + "' AND isDelivered = 0  " +
                                  " IF(@COUNT=0) " +
                                  " BEGIN " +
                                  " UPDATE CUSTORDER_HEADER SET isDelivered = 1 WHERE ORDERNUM='" + orderNum.Trim() + "'  " +
                                  " END ";
                }
                else
                {
                    if(string.IsNullOrEmpty(orderNum))
                        orderNum = "0";
                    if(string.IsNullOrEmpty(orderLineNum))
                        orderLineNum = "0";
                    commandText = " UPDATE CUSTORDER_DETAILS SET isDelivered = 0 WHERE ORDERNUM='" + orderNum.Trim() + "' AND  LINENUM='" + orderLineNum.Trim() + "' " +
                                 " DECLARE @COUNT INT " +
                                 " SELECT @COUNT=COUNT(LINENUM) FROM CUSTORDER_DETAILS WHERE ORDERNUM='" + orderNum.Trim() + "' AND isDelivered = 0  " +
                                 " IF(@COUNT>0) " +
                                 " BEGIN " +
                                 " UPDATE CUSTORDER_HEADER SET isDelivered = 0 WHERE ORDERNUM='" + orderNum.Trim() + "'  " +
                                 " END ";

                }

                /* //SKU Table New
                commandText = commandText + " UPDATE SKUTable_Posted SET isLocked='False' " +  
                                            " WHERE SkuNumber='" + itemid + "' " +
                    //  " AND STOREID='" + ApplicationSettings.Terminal.StoreId + "' " +
                                            " AND DATAAREAID='" + application.Settings.Database.DataAreaID + "' ";
                 */
                commandText = commandText + " UPDATE SKUTableTrans SET isLocked='False' " +  //SKU Table New
                                            " WHERE SkuNumber='" + itemid + "' " +
                    //  " AND STOREID='" + ApplicationSettings.Terminal.StoreId + "' " +
                                            " AND DATAAREAID='" + application.Settings.Database.DataAreaID + "' ";
                //commandText = commandText + " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2";
                commandText = commandText + " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE
            }

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;
            command.ExecuteNonQuery();

        }
        #endregion

        #region SaleOperation
        private bool SaleOperation(ISaleLineItem saleLineItem, IPosTransaction posTransaction, SqlConnection connection)
        {
            SaleLineItem saleLine = (SaleLineItem)saleLineItem;

            string sMRPRate = string.Empty;

            ArrayList al = new ArrayList();

            if(!string.IsNullOrEmpty(Convert.ToString(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).Customer.CustomerId)))
            {
                StringBuilder sbQuery = new StringBuilder();

                sbQuery.Append("SELECT ORDERNUM AS CUSTOMERORDER FROM CUSTORDER_HEADER WHERE isDelivered=0 AND IsConfirmed=1 and custaccount='" + Convert.ToString(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).Customer.CustomerId) + "'");


                if(connection.State == ConnectionState.Closed)
                    connection.Open();
                SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
                SqlDataReader reader = cmd.ExecuteReader();
                if(reader.HasRows)
                {
                    while(reader.Read())
                    {
                        al.Add(Convert.ToString(reader["CUSTOMERORDER"]));
                    }
                }
                reader.Close();
                connection.Close();

            }

            bool isMRPExists = isMRP(saleLineItem.ItemId, connection);
            RetailTransaction posClone = posTransaction as RetailTransaction;
            if(isMRPExists)
            {
                saleLine.PartnerData.isMRP = true;
                saleLine.PartnerData.TransactionType = "0";
                saleLine.PartnerData.Ingredients = "";
                saleLine.PartnerData.Quantity = "0";
                saleLine.PartnerData.IsSpecialDisc = false;
                saleLine.PartnerData.SpecialDiscInfo = "";
                saleLine.PartnerData.CustNo = "";

                RetailTransaction pos = posClone.CloneTransaction() as RetailTransaction;
                pos.Add(saleLine);
                application.Services.Price.GetPrice(pos);
                SaleLineItem saleItemforMrp = pos.SaleItems.Last.Value;
                sMRPRate = Convert.ToString(saleItemforMrp.Price);

            }
            else
            {
                saleLine.PartnerData.TransactionType = "0";
                saleLine.PartnerData.isMRP = false;
                saleLine.PartnerData.IsSpecialDisc = false;
                saleLine.PartnerData.SpecialDiscInfo = "";
                saleLine.PartnerData.CustNo = "";
                saleLine.PartnerData.Quantity = "0";
                saleLine.PartnerData.Ingredients = "";
                saleLine.PartnerData.TotalAmount = "";

                //int iMetalType = GetMetalType(saleLine.ItemId, connection);
                //if(iMetalType != (int)MetalType.Gold || iMetalType != (int)MetalType.Silver
                //    || iMetalType != (int)MetalType.Platinum || iMetalType != (int)MetalType.Palladium)
                //{
                //    RetailTransaction pos = posClone.CloneTransaction() as RetailTransaction;
                //    pos.Add(saleLine);
                //    application.Services.Price.GetPrice(pos);
                //    SaleLineItem saleItemforMrp = pos.SaleItems.Last.Value;
                //    sMRPRate = Convert.ToString(saleItemforMrp.Price);
                //}
            }

            connection.Close();

            frmCustomFieldCalculations
                oCustomCalc = new frmCustomFieldCalculations(connection,
                                                             saleLine.ItemId,
                                                             ApplicationSettings.Database.StoreID,
                                                             string.IsNullOrEmpty(Convert.ToString(saleLine.Dimension.ConfigId)) ? "" : Convert.ToString(saleLine.Dimension.ConfigId),
                                                             al, sMRPRate, isMRPExists, posTransaction, saleLine.BackofficeSalesOrderUnitOfMeasure);

            bool zeropricevalid = true;
            if(oCustomCalc.isCancelClick)
            {
                string query = string.Empty;
                // query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 ";
                query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE
                isItemAvailableORReturn(query);

                return false;
            }
            oCustomCalc.ShowDialog();

            if(oCustomCalc.isCancelClick)
            {
                string query = string.Empty;
                //query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 ";
                query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE
                isItemAvailableORReturn(query);

                return false;
            }

            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
            if(Convert.ToInt16(oCustomCalc.RadioChecked) == (int)Enums.EnumClass.TransactionType.Sale)
                list = oCustomCalc.saleList;
            else
                list = oCustomCalc.purchaseList;

            if(IsGiftItem(saleLine.ItemId) && saleLine.ZeroPriceValid == false)// req by Bapi da on 14/08/2015
            {
                MessageBox.Show("Please check zero price valid or not");
                return false;
            }

            zeropricevalid = saleLine.ZeroPriceValid;
            string amount = Convert.ToString(list[9].Value);
            if(!zeropricevalid && !string.IsNullOrEmpty(amount) && Convert.ToDecimal(amount) == 0)
            {
                return true;
            }
            updateOrderDelivery(null, null, false, saleLineItem, saleLine.ItemId);
            GetSKUExtraInfo(saleLine.ItemId);

            saleLine.PartnerData.Pieces = list[0].Value;
            saleLine.PartnerData.Quantity = list[1].Value;
            saleLine.PartnerData.Rate = list[2].Value;
            saleLine.PartnerData.RateType = list[3].Value;
            saleLine.PartnerData.MakingRate = list[4].Value;
            saleLine.PartnerData.MakingType = list[5].Value;
            saleLine.PartnerData.Amount = list[6].Value;
            saleLine.PartnerData.MakingDisc = list[7].Value;
            saleLine.PartnerData.MakingAmount = list[8].Value;
            saleLine.PartnerData.TotalAmount = list[9].Value;
            saleLine.PartnerData.TotalWeight = list[10].Value;
            saleLine.PartnerData.LossPct = list[11].Value;
            saleLine.PartnerData.LossWeight = list[12].Value;
            saleLine.PartnerData.ExpectedQuantity = list[13].Value;
            saleLine.PartnerData.TransactionType = oCustomCalc.RadioChecked.ToUpper().Trim();
            saleLine.PartnerData.OChecked = list[14].Value;

            if(oCustomCalc.dtIngredientsClone != null && oCustomCalc.dtIngredientsClone.Rows.Count > 0)
            {
                oCustomCalc.dtIngredientsClone.TableName = "Ingredients";

                MemoryStream mstr = new MemoryStream();
                oCustomCalc.dtIngredientsClone.WriteXml(mstr, true);
                mstr.Seek(0, SeekOrigin.Begin);
                StreamReader sr = new StreamReader(mstr);
                string sXML = string.Empty;
                sXML = sr.ReadToEnd();
                saleLine.PartnerData.Ingredients = sXML;
                //   oCustomCalc.dtIngredientsClone.WriteXml(saleLine.ItemId + "-" + saleLine.LineId);
            }
            else
            {
                saleLine.PartnerData.Ingredients = string.Empty;
            }


            saleLine.PartnerData.OrderNum = list[15].Value;
            saleLine.PartnerData.OrderLineNum = list[16].Value;
            saleLine.PartnerData.CustNo = Convert.ToString(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).Customer.CustomerId) == null ? string.Empty : Convert.ToString(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).Customer.CustomerId);
            saleLine.PartnerData.SampleReturn = string.IsNullOrEmpty(Convert.ToString(list[17].Value)) ? "0" : (Convert.ToBoolean(list[17].Value) ? Convert.ToString("1") : Convert.ToString("0"));
            saleLine.PartnerData.WastageType = list[18].Value;
            saleLine.PartnerData.WastageQty = list[19].Value;
            saleLine.PartnerData.WastageAmount = list[20].Value;
            saleLine.PartnerData.WastagePercentage = list[21].Value;
            saleLine.PartnerData.WastageRate = list[22].Value;
            saleLine.PartnerData.MakingDiscountType = list[23].Value;
            saleLine.PartnerData.MakingTotalDiscount = list[24].Value;
            saleLine.PartnerData.ConfigId = list[25].Value;

            saleLine.PartnerData.Purity = list[26].Value;
            saleLine.PartnerData.GROSSWT = list[27].Value;
            saleLine.PartnerData.GROSSUNIT = list[28].Value;
            saleLine.PartnerData.DMDWT = list[29].Value;
            saleLine.PartnerData.DMDPCS = list[30].Value;
            saleLine.PartnerData.DMDUNIT = list[31].Value;
            saleLine.PartnerData.DMDAMOUNT = list[32].Value;
            saleLine.PartnerData.STONEWT = list[33].Value;
            saleLine.PartnerData.STONEPCS = list[34].Value;
            saleLine.PartnerData.STONEUNIT = list[35].Value;
            saleLine.PartnerData.STONEAMOUNT = list[36].Value;
            saleLine.PartnerData.NETWT = list[37].Value;
            saleLine.PartnerData.NETRATE = list[38].Value;
            saleLine.PartnerData.NETUNIT = list[39].Value;
            saleLine.PartnerData.NETPURITY = list[40].Value;
            saleLine.PartnerData.NETAMOUNT = list[41].Value;
            saleLine.PartnerData.OGREFINVOICENO = list[42].Value;

            //start : added on req of R.Nandy 151114
            saleLine.PartnerData.ItemIdParent = sItemIdParent;
            saleLine.PartnerData.GroupCostPrice = dGroupCostPrice;
            saleLine.PartnerData.UpdatedCostPrice = dUpdatedCostPrice;
            saleLine.PartnerData.SellingCostPrice = dSellingCostPrice;
            //end  : 151114
            saleLine.PartnerData.FLAT = list[43].Value;
            saleLine.PartnerData.PROMOCODE = list[44].Value;


            //saleLine.PartnerData.isMRP = false;
            saleLine.PartnerData.IsSpecialDisc = false;
            saleLine.PartnerData.SpecialDiscInfo = "";

            saleLine.PartnerData.REMARKS = list[45].Value;// ADDED on 10/04/17

            if(saleLineItem != null)
            {
                for(int i = 0; i < 1000; i++)// changes on 09/09/2015
                {
                    if(!string.IsNullOrEmpty(saleLineItem.SalesPersonId))
                        break;
                    else
                    {
                        LSRetailPosis.POSProcesses.frmSalesPerson dialog6 = new LSRetailPosis.POSProcesses.frmSalesPerson();
                        dialog6.ShowDialog();
                        if(!string.IsNullOrEmpty(dialog6.SelectedEmployeeId))
                        {
                            saleLineItem.SalesPersonId = dialog6.SelectedEmployeeId;
                            saleLineItem.SalespersonName = dialog6.SelectEmployeeName;
                        }
                    }
                }
                string sSCType = "";// GetSaLsChannelType();// commented on 13/10/15 req by s.giri

                saleLine.PartnerData.SALESCHANNELTYPE = sSCType;


                if(!string.IsNullOrEmpty(Convert.ToString(saleLine.PartnerData.OrderNum)) && !string.IsNullOrEmpty(Convert.ToString(saleLine.PartnerData.OrderLineNum)))
                {
                    updateOrderDelivery(Convert.ToString(saleLine.PartnerData.OrderNum), Convert.ToString(saleLine.PartnerData.OrderLineNum), false, saleLineItem);
                }

            }

            return true;
        }
        #endregion

        #region IItemTriggersV1 Members

        public void PreSale(IPreTriggerResult preTriggerResult, ISaleLineItem saleLineItem, IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("IItemTriggersV1.PreSale", "Prior to the sale of an item...", LSRetailPosis.LogTraceLevel.Trace);
            RetailTransaction retailTrans = posTransaction as RetailTransaction;

            if(retailTrans.SaleIsReturnSale)
            {
                preTriggerResult.ContinueOperation = false;
                preTriggerResult.MessageId = 999998;
                return;
            }

            if (retailTrans!=null)
            {
                if (retailTrans.PartnerData.TotalValueChanged == true)
                {
                    preTriggerResult.ContinueOperation = false;
                    MessageBox.Show("After total value changed not possible to add any item, please void the transaction and try again the same.");
                    return;
                }
            }

            #region - Changed By Nimbus
            SaleLineItem saleLine = (SaleLineItem)saleLineItem;

            string sMRPRate = string.Empty;
            if(saleLineItem != null)
            {
                #region
                retailTrans.PartnerData.IsGSSMaturity = false;
                string sGSSAdjItemId = "";
                string sGSSDiscItemId = "";
                GSSAdjustmentItemID(ref sGSSAdjItemId, ref sGSSDiscItemId);

                if(saleLineItem.ItemId == sGSSAdjItemId
                    || saleLineItem.ItemId == sGSSDiscItemId)
                {
                    saleLine.PartnerData.TransactionType = "0";// add on 16/04/2014
                    retailTrans.PartnerData.IsGSSMaturity = true;
                    return;
                }

                foreach(SaleLineItem SLItem in retailTrans.SaleItems)
                {
                    if(SLItem.ItemId == sGSSAdjItemId)
                    {
                        saleLine.PartnerData.TransactionType = "0";// add on 16/04/2014
                        retailTrans.PartnerData.IsGSSMaturity = true;
                        break;
                    }
                }

                #endregion

                #region
                string sAdjustmentId = AdjustmentItemID();
                foreach(SaleLineItem SLineItem in retailTrans.SaleItems)
                {
                    if(SLineItem.ItemId == sAdjustmentId)
                    {
                        saleLine.PartnerData.TransactionType = "0";// add on 16/04/2014
                        retailTrans.PartnerData.SaleAdjustment = true;
                        break;
                    }
                }
                #endregion

                #region Service Item
                if(saleLineItem.ItemId == AdjustmentItemID())
                {
                    saleLine.PartnerData.isMRP = false;
                    saleLine.PartnerData.TransactionType = "0";// add on 16/04/2014
                    saleLine.PartnerData.Ingredients = string.Empty; // added on 12/11/14
                    saleLine.PartnerData.TotalAmount = "0";// added on 12/11/14
                    saleLine.PartnerData.Rate = "0";// added on 12/11/14
                    saleLine.PartnerData.IsSpecialDisc = false;
                    saleLine.PartnerData.SpecialDiscInfo = "";
                    saleLine.PartnerData.CustNo = "";

                    if(string.IsNullOrEmpty(Convert.ToString(retailTrans.PartnerData.AdjustmentCustAccount)))
                    {
                        System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem> sale = new System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem>(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).SaleItems);

                        DataRow drAdjustment = null;
                        // BlankOperations.CustomerAdvanceAdjustment oAdjustment = new BlankOperations.CustomerAdvanceAdjustment(posTransaction);
                        BlankOperations.CustomerAdvanceAdjustment oAdjustment = new BlankOperations.CustomerAdvanceAdjustment(posTransaction);

                        if(sale.Count == 0)
                            drAdjustment = oAdjustment.AmountToBeAdjusted(retailTrans.Customer.CustomerId);
                        else
                            drAdjustment = oAdjustment.AmountToBeAdjusted(retailTrans.Customer.CustomerId, true);

                        if(drAdjustment == null)
                            preTriggerResult.ContinueOperation = false;
                        else
                        {
                            // Avg Gold Rate Adjustment
                            if(sale.Count == 0)
                            {
                                if(Convert.ToInt32(drAdjustment["GoldFixing"]) == 1)
                                {
                                    retailTrans.PartnerData.SaleAdjustmentGoldAmt = Convert.ToDecimal(drAdjustment["AMOUNT"]);
                                }
                                else
                                {
                                    retailTrans.PartnerData.SaleAdjustmentGoldAmt = 0;
                                }

                                retailTrans.PartnerData.SaleAdjustmentGoldQty = Convert.ToDecimal(drAdjustment["GoldQuantity"]);
                            }
                            else
                            {
                                if(Convert.ToInt32(drAdjustment["GoldFixing"]) == 1)
                                {
                                    retailTrans.PartnerData.SaleAdjustmentGoldAmt = Convert.ToDecimal(retailTrans.PartnerData.SaleAdjustmentGoldAmt) + Convert.ToDecimal(drAdjustment["TotalAmount"]);
                                }
                                retailTrans.PartnerData.SaleAdjustmentGoldQty = Convert.ToDecimal(retailTrans.PartnerData.SaleAdjustmentGoldQty) + Convert.ToDecimal(drAdjustment["GoldQuantity"]);
                            }
                            //

                            saleLine.PartnerData.ServiceItemCashAdjustmentPrice = Convert.ToString(drAdjustment["AMOUNT"]);
                            saleLine.PartnerData.ServiceItemCashAdjustmentTransactionID = Convert.ToString(drAdjustment["TransactionID"]);
                            saleLine.PartnerData.ServiceItemCashAdjustmentStoreId = Convert.ToString(drAdjustment["RETAILSTOREID"]);
                            saleLine.PartnerData.ServiceItemCashAdjustmentTerminalId = Convert.ToString(drAdjustment["RETAILTERMINALID"]);

                            //isItemAvailableORReturn(" UPDATE RETAILADJUSTMENTTABLE SET ISADJUSTED='1' WHERE TRANSACTIONID='" + Convert.ToString(drAdjustment["TransactionID"]) + "' AND RETAILSTOREID='" + Convert.ToString(drAdjustment["RETAILSTOREID"]) + "' AND RETAILTERMINALID='" + Convert.ToString(drAdjustment["RETAILTERMINALID"]) + "'");
                            //updateCustomerAdvanceAdjustment(Convert.ToString(drAdjustment["Transaction ID"]), 1);

                            //RTS is calling for update changed on 27/11/2015
                            try
                            {
                                ReadOnlyCollection<object> containerArray;
                                string sMsg = string.Empty;

                                if(PosApplication.Instance.TransactionServices.CheckConnection())
                                {
                                    containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("updateAdvanceForAdjust", Convert.ToString(drAdjustment["TransactionID"]), Convert.ToString(drAdjustment["RETAILSTOREID"]), Convert.ToString(drAdjustment["RETAILTERMINALID"]));
                                    sMsg = Convert.ToString(containerArray[2]);
                                    // MessageBox.Show(sMsg);

                                }
                            }
                            catch(Exception ex)
                            {


                            }
                        }
                    }
                    else
                    {
                        string order = Convert.ToString(retailTrans.PartnerData.AdjustmentOrderNum);
                        string cust = Convert.ToString(retailTrans.PartnerData.AdjustmentCustAccount);
                        BlankOperations.CustomerAdvanceAdjustment oAdjustment = new BlankOperations.CustomerAdvanceAdjustment(posTransaction);
                        DataRow drAdjustment = null;
                        System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem> sale = new System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem>(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).SaleItems);
                        if(sale.Count == 0)
                            drAdjustment = oAdjustment.AmountToBeAdjusted(retailTrans.Customer.CustomerId, false, cust, order);
                        else
                            drAdjustment = oAdjustment.AmountToBeAdjusted(retailTrans.Customer.CustomerId, true, cust, order);
                        if(drAdjustment == null)
                            preTriggerResult.ContinueOperation = false;
                        else
                        {
                            // Avg Gold Rate Adjustment
                            if(sale.Count == 0)
                            {
                                if(Convert.ToInt32(drAdjustment["GoldFixing"]) == 1)
                                {
                                    retailTrans.PartnerData.SaleAdjustmentGoldAmt = Convert.ToDecimal(drAdjustment["TotalAmount"]);
                                }
                                else
                                {
                                    retailTrans.PartnerData.SaleAdjustmentGoldAmt = 0;
                                }

                                retailTrans.PartnerData.SaleAdjustmentGoldQty = Convert.ToDecimal(drAdjustment["GoldQty"]);
                            }
                            else
                            {
                                if(Convert.ToInt32(drAdjustment["GoldFixing"]) == 1)
                                {
                                    retailTrans.PartnerData.SaleAdjustmentGoldAmt = Convert.ToDecimal(retailTrans.PartnerData.SaleAdjustmentGoldAmt) + Convert.ToDecimal(drAdjustment["TotalAmount"]);
                                }
                                retailTrans.PartnerData.SaleAdjustmentGoldQty = Convert.ToDecimal(retailTrans.PartnerData.SaleAdjustmentGoldQty) + Convert.ToDecimal(drAdjustment["GoldQty"]);
                            }
                            //

                            saleLine.PartnerData.ServiceItemCashAdjustmentPrice = Convert.ToString(drAdjustment["TotalAmount"]);
                            saleLine.PartnerData.ServiceItemCashAdjustmentTransactionID = Convert.ToString(drAdjustment["TransactionID"]);
                            saleLine.PartnerData.ServiceItemCashAdjustmentStoreId = Convert.ToString(drAdjustment["RETAILSTOREID"]);
                            saleLine.PartnerData.ServiceItemCashAdjustmentTerminalId = Convert.ToString(drAdjustment["RETAILTERMINALID"]);

                            isItemAvailableORReturn(" UPDATE RETAILADJUSTMENTTABLE SET ISADJUSTED='1' WHERE TRANSACTIONID='" + Convert.ToString(drAdjustment["TransactionID"]) + "' AND RETAILSTOREID='" + Convert.ToString(drAdjustment["RETAILSTOREID"]) + "' AND RETAILTERMINALID='" + Convert.ToString(drAdjustment["RETAILTERMINALID"]) + "'");

                        }
                    }
                    return;
                }
                else
                {
                #endregion


                    saleLine.PartnerData.isMRP = false;
                    saleLine.PartnerData.TransactionType = "0";// add on 16/04/2014
                    saleLine.PartnerData.Ingredients = string.Empty; // added on 12/11/14
                    saleLine.PartnerData.TotalAmount = "0";// added on 12/11/14
                    saleLine.PartnerData.Rate = "0";// added on 12/11/14
                    saleLine.PartnerData.IsSpecialDisc = false;
                    saleLine.PartnerData.SpecialDiscInfo = "";
                    saleLine.PartnerData.CustNo = "";
                    saleLine.PartnerData.ServiceItemCashAdjustmentPrice = "";
                    saleLine.PartnerData.ServiceItemCashAdjustmentTransactionID = "";
                    saleLine.PartnerData.ServiceItemCashAdjustmentStoreId = "";
                    saleLine.PartnerData.ServiceItemCashAdjustmentTerminalId = "";
                    saleLine.PartnerData.Quantity = string.Empty;


                }

                #region Repair Return Adjustment Item
                string sRepairRetAdejItemId = string.Empty;
                RepairAdjItemId(ref sRepairRetAdejItemId);
                if(saleLineItem.ItemId == sRepairRetAdejItemId) //"Repair Return Adj Item"
                {
                    //"Repair Return Adj Item"
                    return;
                }
                foreach(SaleLineItem SLItem in retailTrans.SaleItems)
                {
                    if(SLItem.ItemId == sRepairRetAdejItemId)
                    {
                        //"Repair Return Adj Item"
                        break;
                    }
                }
                #endregion




                SqlConnection connection = new SqlConnection();
                if(application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;

                #region Non Booked SKU
                if(!Convert.ToBoolean(retailTrans.PartnerData.SKUBookedItems))
                {

                    int? isLocked = null;
                    int? isAvailable = null;
                    int? isReturnLocked = null;
                    int? isReturnAvailable = null;
                    CheckForReturnSKUExistence(saleLine.ItemId, connection, out isReturnLocked, out isReturnAvailable);

                    if(isReturnAvailable == null && isReturnLocked == null)
                    {
                        CheckForSKUExistence(saleLine.ItemId, connection, out isLocked, out isAvailable);
                        if(isLocked != null && isAvailable != null)
                        {
                            if(Convert.ToBoolean(isLocked))
                            {
                                preTriggerResult.ContinueOperation = false;
                                preTriggerResult.MessageId = 50397;
                                string query = string.Empty;

                                // query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 ";
                                query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE
                                isItemAvailableORReturn(query);
                            }
                            else if(!Convert.ToBoolean(isLocked) && !Convert.ToBoolean(isAvailable))
                            {
                                preTriggerResult.ContinueOperation = false;
                                preTriggerResult.MessageId = 50398;
                                string query = string.Empty;
                                //query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 ";
                                query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE
                                isItemAvailableORReturn(query);
                            }
                            else
                            {
                                if(!SaleOperation(saleLine, posTransaction, connection))
                                {
                                    preTriggerResult.ContinueOperation = false;
                                }
                            }

                        }

                        else
                        {

                            if(!SaleOperation(saleLineItem, posTransaction, connection))
                            {
                                preTriggerResult.ContinueOperation = false;
                            }

                            #region - Sale Operation Commented
                            //    updateOrderDelivery(null, null, false, saleLine.ItemId);
                            //    ArrayList al = new ArrayList();

                            //    if (!string.IsNullOrEmpty(Convert.ToString(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).Customer.CustomerId)))
                            //    {
                            //        StringBuilder sbQuery = new StringBuilder();

                            //        sbQuery.Append("SELECT ORDERNUM AS CUSTOMERORDER FROM CUSTORDER_HEADER WHERE isDelivered=0 and custaccount='" + Convert.ToString(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).Customer.CustomerId) + "'");


                            //        if (connection.State == ConnectionState.Closed)
                            //            connection.Open();
                            //        SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
                            //        SqlDataReader reader = cmd.ExecuteReader();
                            //        if (reader.HasRows)
                            //        {
                            //            while (reader.Read())
                            //            {
                            //                al.Add(Convert.ToString(reader["CUSTOMERORDER"]));
                            //            }
                            //        }
                            //        reader.Close();
                            //        connection.Close();

                            //    }

                            //    bool isMRPExists = isMRP(saleLineItem.ItemId, connection);
                            //    sMRPRate = Convert.ToString(application.Services.Price.GetItemPrice(saleLine.ItemId, ((LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem)(saleLine)).BackofficeSalesOrderUnitOfMeasure));
                            //    connection.Close();

                            //    BlankOperations.WinFormsTouch.frmCustomFieldCalculations
                            //        oCustomCalc = new BlankOperations.WinFormsTouch.frmCustomFieldCalculations(connection,
                            //                                                                                   saleLine.ItemId,
                            //                                                                                   ApplicationSettings.Database.StoreID,
                            //                                                                                   string.IsNullOrEmpty(Convert.ToString(saleLine.Dimension.ConfigId)) ? "" : Convert.ToString(saleLine.Dimension.ConfigId),
                            //                                                                               al, sMRPRate, isMRPExists);


                            //    //  frmCustomFieldsCalculation.frmCustomFieldCalculation
                            //    //    oCustomCalc = new frmCustomFieldsCalculation.frmCustomFieldCalculation(connection,
                            //    //                                                                            saleLine.ItemId,
                            //    //                                                                            ApplicationSettings.Database.StoreID,
                            //    //                                                                            string.IsNullOrEmpty(Convert.ToString(saleLine.Dimension.ConfigId)) ? "" : Convert.ToString(saleLine.Dimension.ConfigId),
                            //    //                                                                            al);

                            //    oCustomCalc.ShowDialog();
                            //    List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
                            //    if (Convert.ToInt16(oCustomCalc.RadioChecked) == (int)Enums.EnumClass.TransactionType.Sale)
                            //        list = oCustomCalc.saleList;
                            //    else
                            //        list = oCustomCalc.purchaseList;

                            //    saleLine.PartnerData.Pieces = list[0].Value;
                            //    saleLine.PartnerData.Quantity = list[1].Value;
                            //    saleLine.PartnerData.Rate = list[2].Value;
                            //    saleLine.PartnerData.RateType = list[3].Value;
                            //    saleLine.PartnerData.MakingRate = list[4].Value;
                            //    saleLine.PartnerData.MakingType = list[5].Value;
                            //    saleLine.PartnerData.Amount = list[6].Value;
                            //    saleLine.PartnerData.MakingDisc = list[7].Value;
                            //    saleLine.PartnerData.MakingAmount = list[8].Value;
                            //    saleLine.PartnerData.TotalAmount = list[9].Value;
                            //    saleLine.PartnerData.TotalWeight = list[10].Value;
                            //    saleLine.PartnerData.LossPct = list[11].Value;
                            //    saleLine.PartnerData.LossWeight = list[12].Value;
                            //    saleLine.PartnerData.ExpectedQuantity = list[13].Value;
                            //    saleLine.PartnerData.TransactionType = oCustomCalc.RadioChecked.ToUpper().Trim();
                            //    saleLine.PartnerData.OChecked = list[14].Value;

                            //    if (oCustomCalc.dtIngredientsClone != null && oCustomCalc.dtIngredientsClone.Rows.Count > 0)
                            //    {
                            //        oCustomCalc.dtIngredientsClone.TableName = "Ingredients";

                            //        MemoryStream mstr = new MemoryStream();
                            //        oCustomCalc.dtIngredientsClone.WriteXml(mstr, true);
                            //        mstr.Seek(0, SeekOrigin.Begin);
                            //        StreamReader sr = new StreamReader(mstr);
                            //        string sXML = string.Empty;
                            //        sXML = sr.ReadToEnd();
                            //        saleLine.PartnerData.Ingredients = sXML;
                            //        //   oCustomCalc.dtIngredientsClone.WriteXml(saleLine.ItemId + "-" + saleLine.LineId);
                            //    }
                            //    else
                            //    {
                            //        saleLine.PartnerData.Ingredients = string.Empty;
                            //    }


                            //    saleLine.PartnerData.OrderNum = list[15].Value;
                            //    saleLine.PartnerData.OrderLineNum = list[16].Value;
                            //    saleLine.PartnerData.CustNo = Convert.ToString(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).Customer.CustomerId) == null ? string.Empty : Convert.ToString(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).Customer.CustomerId);





                            //    if (saleLineItem != null)
                            //    {
                            //        if (!string.IsNullOrEmpty(Convert.ToString(saleLine.PartnerData.OrderNum)) && !string.IsNullOrEmpty(Convert.ToString(saleLine.PartnerData.OrderLineNum)))
                            //        {
                            //            updateOrderDelivery(Convert.ToString(saleLine.PartnerData.OrderNum), Convert.ToString(saleLine.PartnerData.OrderLineNum), false);
                            //        }
                            //    }
                        }
                            #endregion
                    }
                    else if(isReturnAvailable == 1 && isReturnLocked == 1)
                    {
                        preTriggerResult.MessageId = 50398;
                        preTriggerResult.ContinueOperation = false;
                        string query = string.Empty;
                        //   query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 ";
                        query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE
                        isItemAvailableORReturn(query);

                    }
                    else if(isReturnAvailable == 1 && isReturnLocked == 0)
                    {
                        preTriggerResult.MessageId = 50395;
                        preTriggerResult.ContinueOperation = false;
                        string query = string.Empty;
                        //  query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 ";
                        query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE
                        isItemAvailableORReturn(query);
                    }
                    else if((isReturnAvailable == 0 && isReturnLocked == 0))
                    {
                        if(!SaleOperation(saleLineItem, posTransaction, connection))
                        {
                            preTriggerResult.ContinueOperation = false;
                        }
                    }
                }
                else
                {
                    if(!SaleOperation(saleLine, posTransaction, connection))
                    {
                        preTriggerResult.ContinueOperation = false;
                    }
                    //retailTrans.PartnerData.SKUBookedItems = false;
                }
                #endregion

            }
            #endregion
        }

        public void PostSale(IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("IItemTriggersV1.PostSale", "After the sale of an item...", LSRetailPosis.LogTraceLevel.Trace);

            string query = string.Empty;
            // query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 ";
            query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' ";
            isItemAvailableORReturn(query);


        }

        #region Changed By Nimbus
        private void isItemAvailableORReturn(string query)
        {
            SqlConnection connection = new SqlConnection();
            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if(connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand command = new SqlCommand(query, connection);
            command.CommandTimeout = 0;
            command.ExecuteNonQuery();

        }
        #endregion

        public void PreReturnItem(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction)
        {
            #region Changed By Nimbus
            string query = string.Empty;
            // query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN=(CASE  WHEN ITEMRETURN='False' THEN 'True' ELSE 'False' END) WHERE ID=2 ";
            query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN=(CASE  WHEN ITEMRETURN='False' THEN 'True' ELSE 'False' END) WHERE ID=2 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE
            isItemAvailableORReturn(query);

            #endregion

            RetailTransaction retailTrans = posTransaction as RetailTransaction;
            retailTrans.PartnerData.ItemIsReturnItem = true;

            LSRetailPosis.ApplicationLog.Log("IItemTriggersV1.PreReturnItem", "Prior to entering return state...", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PostReturnItem(IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("IItemTriggersV1.PostReturnItem", "After entering return state", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PreVoidItem(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction, int lineId)
        {
            RetailTransaction retailTrans = posTransaction as RetailTransaction;
            if(retailTrans != null)
            {
                if(retailTrans.PartnerData.IsGSSMaturity)
                {
                    retailTrans.PartnerData.GSSSaleWt = Convert.ToDecimal(retailTrans.PartnerData.GSSSaleWt) - Convert.ToDecimal(retailTrans.PartnerData.RunningQtyGSS);
                }

                if(retailTrans.PartnerData.SaleAdjustment)
                {
                    retailTrans.PartnerData.TransAdjGoldQty = Convert.ToDecimal(retailTrans.PartnerData.TransAdjGoldQty) - Convert.ToDecimal(retailTrans.PartnerData.RunningQtyAdjustment);
                }

                foreach(SaleLineItem saleLineItem in retailTrans.SaleItems)
                {
                    if(saleLineItem.ItemId != AdjustmentItemID())
                    {
                        if(Convert.ToString(lineId) == Convert.ToString(saleLineItem.LineId) && saleLineItem.Voided)
                        {
                            preTriggerResult.MessageId = 50396;
                            preTriggerResult.ContinueOperation = false;
                            break;
                        }


                    }
                }

            }
            LSRetailPosis.ApplicationLog.Log("IItemTriggersV1.PreVoidItem", "Before voiding an item", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PostVoidItem(IPosTransaction posTransaction, int lineId)
        {
            #region changed By Nimbus


            RetailTransaction retailTrans = posTransaction as RetailTransaction;
            if(retailTrans != null)
            {
                foreach(SaleLineItem saleLineItem in retailTrans.SaleItems)
                {
                    if(saleLineItem.ItemId != AdjustmentItemID())
                    {
                        if(Convert.ToString(lineId) == Convert.ToString(saleLineItem.LineId))
                        {
                            retailTrans.PartnerData.PackingMaterial = "";
                            updateOrderDelivery(Convert.ToString(saleLineItem.PartnerData.OrderNum), Convert.ToString(saleLineItem.PartnerData.OrderLineNum), true, saleLineItem, saleLineItem.ItemId);
                        }
                    }

                }

            }

            #endregion
            string source = "IItemTriggersV1.PostVoidItem";
            string value = "After voiding an item";
            LSRetailPosis.ApplicationLog.Log(source, value, LSRetailPosis.LogTraceLevel.Trace);
            LSRetailPosis.ApplicationLog.WriteAuditEntry(source, value);
        }

        #region Check for Service Item
        private bool isServiceItem(IPosTransaction transaction, int lineid, ISaleLineItem saleLineItem, string operation)
        {
            if(lineid == 0)
                return false;

            bool isServiceItemExists = false;
            SaleLineItem saleitem = saleLineItem as SaleLineItem;
            if(operation.ToUpper().Trim() != "QTY")
            {
                saleitem = null;
            }
            if(saleitem == null)
            {
                System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem> saleline = new System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem>(((LSRetailPosis.Transaction.RetailTransaction)(transaction)).SaleItems);
                foreach(var sale in saleline)
                {
                    if(sale.ItemType == LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service && !sale.Voided)
                    {
                        if(sale.LineId == lineid)
                        {
                            isServiceItemExists = true;
                            break;
                        }
                    }
                }

            }
            return isServiceItemExists;
        }
        #endregion

        public void PreSetQty(IPreTriggerResult preTriggerResult, ISaleLineItem saleLineItem, IPosTransaction posTransaction, int lineId)
        {
            if(isServiceItem(posTransaction, lineId, saleLineItem, "QTY"))
            {
                preTriggerResult.ContinueOperation = false;
                preTriggerResult.MessageId = 50399;
            }
            LSRetailPosis.ApplicationLog.Log("IItemTriggersV1.PreSetQty", "Before setting the qty for an item", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PostSetQty(IPosTransaction posTransaction, ISaleLineItem saleLineItem)
        {
            LSRetailPosis.ApplicationLog.Log("IItemTriggersV1.PostSetQty", "After setting the qty from an item", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PrePriceOverride(IPreTriggerResult preTriggerResult, ISaleLineItem saleLineItem, IPosTransaction posTransaction, int lineId)
        {
            string sRepairRetAdejItemId = string.Empty;
            RepairAdjItemId(ref sRepairRetAdejItemId);

            if(isServiceItem(posTransaction, lineId, saleLineItem, "POVERRIDE"))
            {
                preTriggerResult.ContinueOperation = false;
                if(saleLineItem.ItemId != sRepairRetAdejItemId) //"Repair Return Adj Item"
                {
                    preTriggerResult.MessageId = 50399;
                }
            }
            LSRetailPosis.ApplicationLog.Log("IItemTriggersV1.PrePriceOverride", "Before overriding the price on an item", LSRetailPosis.LogTraceLevel.Trace);

        }

        public void PostPriceOverride(IPosTransaction posTransaction, ISaleLineItem saleLineItem)
        {
            LSRetailPosis.ApplicationLog.Log("IItemTriggersV1.PostPriceOverride", "After overriding the price of an item", LSRetailPosis.LogTraceLevel.Trace);
        }

        #endregion

        #region IItemTriggersV2 Members

        public void PreClearQty(IPreTriggerResult preTriggerResult, ISaleLineItem saleLineItem, IPosTransaction posTransaction, int lineId)
        {
            if(isServiceItem(posTransaction, lineId, saleLineItem, "CQTY"))
            {
                preTriggerResult.ContinueOperation = false;
                preTriggerResult.MessageId = 50399;
            }
            LSRetailPosis.ApplicationLog.Log("IItemTriggersV2.PreClearQty", "Triggered before clear the quantity of an item.", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PostClearQty(IPosTransaction posTransaction, ISaleLineItem saleLineItem)
        {
            LSRetailPosis.ApplicationLog.Log("IItemTriggersV2.PostClearQty", "Triggered after clear the quantity of an item.", LSRetailPosis.LogTraceLevel.Trace);
        }

        #endregion

        private string GetSaLsChannelType()
        {
            string sSCType = string.Empty;
            SqlConnection connection = new SqlConnection();
            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = string.Empty;

            commandText = "SELECT CHANNELTYPE,[DESCRIPTION] FROM  [DBO].[CRWSALESCHANNEL] ORDER BY DEFAULTVALUE DESC";

            if(connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand cmd = new SqlCommand(commandText, connection);
            SqlDataAdapter daSC = new SqlDataAdapter(cmd);
            DataTable dtSC = new DataTable();
            daSC.Fill(dtSC);
            if(dtSC != null && dtSC.Rows.Count > 0)
            {
                BlankOperations.WinFormsTouch.frmGeneralSearch oSearch = new BlankOperations.WinFormsTouch.frmGeneralSearch(dtSC);
                oSearch.ShowDialog();

                sSCType = oSearch.SelectedChannel;
            }
            else
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No record found.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }
            }
            return sSCType;
        }

        #region GET SKU DATA FOR RELEASING
        private void GetSKUExtraInfo(string sSKUNo)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = "SELECT ITEMIDPARENT,GROUPCOSTPRICE,UPDATEDCOSTPRICE,SELLINGPRICE FROM SKUTable_Posted WHERE SKUNUMBER='" + sSKUNo + "'";

                SqlCommand command = new SqlCommand(SqlComm.CommandText.ToString(), SqlCon);
                command.CommandTimeout = 0;
                SqlDataReader reader = command.ExecuteReader();

                if(reader.HasRows)
                {
                    while(reader.Read())
                    {
                        sItemIdParent = Convert.ToString(reader.GetValue(0));
                        dGroupCostPrice = Convert.ToDecimal(reader.GetValue(1));
                        dUpdatedCostPrice = Convert.ToDecimal(reader.GetValue(2));
                        dSellingCostPrice = Convert.ToDecimal(reader.GetValue(3));
                    }
                }

            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        /// <summary>
        /// GIFT =1 in inventtable 
        /// Dev on  : 14/08/2015 by : RHossain
        /// </summary>
        /// <param name="sItemId"></param>
        /// <returns></returns>
        private bool IsGiftItem(string sItemId)
        {
            bool bGiftItem = false;

            SqlConnection connection = new SqlConnection();

            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "select GIFT from inventtable WHERE ITEMID = '" + sItemId + "'";


            if(connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            bGiftItem = Convert.ToBoolean(command.ExecuteScalar());
            if(connection.State == ConnectionState.Open)
                connection.Close();

            return bGiftItem;
        }

        protected int GetMetalType(string sItemId, SqlConnection connection)
        {
            int iMetalType = 100;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select metaltype from inventtable where itemid='" + sItemId + "'");

            if(connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();
            if(reader.HasRows)
            {
                while(reader.Read())
                {
                    iMetalType = (int)reader.GetValue(0);
                }
            }
            if(connection.State == ConnectionState.Open)
                connection.Close();
            return iMetalType;

        }
    }
}
