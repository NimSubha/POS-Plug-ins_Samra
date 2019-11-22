using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using System.Data;
using LSRetailPosis.Transaction.Line.SaleItem;
using System.Collections;
using System.IO;
using LSRetailPosis.Transaction;
using Microsoft.Dynamics.Retail.Pos.BlankOperations;

namespace DBLibraryClass
{
    public class DBLibrary : IDisposable
    {
        #region  Initialization - Constructors - Variable Declaration
        SqlConnection connection = new SqlConnection();
        IPosTransaction posTransaction = null;
        IApplication application = null;

        public DBLibrary(IApplication app, IPosTransaction pos = null)
        {
            connection = application != null ? application.Settings.Database.Connection : ApplicationSettings.Database.LocalConnection;
            posTransaction = pos;
            application = app;
        }
        #endregion

        #region Common Functions
        #region Function for No Value Return and Execute Non Query
        private void ReturnExecuteNonQuery(string query)
        {
            try
            {
                connection = ApplicationSettings.Database.LocalConnection;
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                command.CommandTimeout = 0;
                command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();

            }

        }
        #endregion

        #region Function for Single Value Return
        private string ReturnExecuteScalar(string query)
        {
            SqlCommand cmd = null;
            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                cmd = new SqlCommand(query, connection);
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();

            }
            return Convert.ToString(cmd.ExecuteScalar());

        }
        #endregion

        #region Function for Double Value Return
        private void ReturnDoubleValues(string query, out string val1, out string val2)
        {
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                cmd = new SqlCommand(query, connection);
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    string ReturnVal1 = null;
                    string ReturnVal2 = null;
                    while (reader.Read())
                    {
                        ReturnVal1 = Convert.ToString(reader.GetValue(0));
                        ReturnVal2 = Convert.ToString(reader.GetValue(1));
                    }
                    reader.Close();
                    val1 = Convert.ToString(ReturnVal1);
                    val2 = Convert.ToString(ReturnVal2);
                }
                else
                {
                    reader.Close();
                    val1 = null;
                    val2 = null;
                }

            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                reader.Dispose();

            }

        }
        #endregion
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

        #region Item Triggers Operations
        #region Check if MRP exists for the item
        public bool isMRP(string itemid)
        {
            return Convert.ToBoolean(ReturnExecuteScalar(" SELECT TOP(1) MRP FROM [INVENTTABLE] WHERE ITEMID='" + itemid + "'  "));
        }
        #endregion

        #region Check for current Item ID to be SKU
        public void CheckForSKUExistence(string itemid, out int? isLock, out int? isAvailable)
        {
            StringBuilder sbQuery = new StringBuilder();

            /* //SKU Table New
            sbQuery.Append(" IF EXISTS(SELECT TOP 1 * FROM skutable_posted WHERE SkuNumber='" + itemid + "') ");
            sbQuery.Append(" BEGIN  ");
            sbQuery.Append(" SELECT isLocked,isAvailable FROM skutable_posted WHERE SkuNumber='" + itemid + "' ");
            sbQuery.Append(" END ");
            */
            sbQuery.Append(" IF EXISTS(SELECT TOP 1 * FROM SKUTableTrans WHERE SkuNumber='" + itemid + "') "); //SKU Table New
            sbQuery.Append(" BEGIN  ");
            sbQuery.Append(" SELECT isLocked,isAvailable FROM SKUTableTrans WHERE SkuNumber='" + itemid + "' ");//SKU Table New
            sbQuery.Append(" END ");

            string Val1 = null;
            string Val2 = null;
            ReturnDoubleValues(sbQuery.ToString(), out Val1, out Val2);

            if (!string.IsNullOrEmpty(Val1) && !string.IsNullOrEmpty(Val2))
            {
                isLock = Convert.ToInt16(Val1);
                isAvailable = Convert.ToInt16(Val2);
            }
            else
            {
                isLock = null;
                isAvailable = null;
            }
        }
        #endregion

        #region Check for current return item not in SKU
        public void CheckForReturnSKUExistence(string itemid, out int? isRetunLock, out int? isReturnAvailable)
        {
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append(" DECLARE @RETURN BIT ");
            sbQuery.Append(" SELECT @RETURN=ITEMRETURN FROM RETAILTEMPTABLE WHERE ID=2 ");
            sbQuery.Append(" IF(@RETURN=1) ");
            sbQuery.Append(" BEGIN  ");
           // sbQuery.Append(" IF EXISTS(SELECT TOP 1 * FROM skutable_posted WHERE SKUNUMBER='" + itemid + "')  "); 
            sbQuery.Append(" IF EXISTS(SELECT TOP 1 * FROM SKUTableTrans WHERE SKUNUMBER='" + itemid + "')  "); //SKU Table New
            sbQuery.Append(" BEGIN  ");
            //sbQuery.Append(" SELECT isLocked,isAvailable FROM skutable_posted WHERE SKUNUMBER='" + itemid + "'  ");
            sbQuery.Append(" SELECT isLocked,isAvailable FROM SKUTableTrans WHERE SKUNUMBER='" + itemid + "'  "); //SKU Table New
            sbQuery.Append(" END  ");
            sbQuery.Append(" END  ");

            string Val1 = null;
            string Val2 = null;
            ReturnDoubleValues(sbQuery.ToString(), out Val1, out Val2);

            if (!string.IsNullOrEmpty(Val1) && !string.IsNullOrEmpty(Val2))
            {
                isRetunLock = Convert.ToInt16(Val1);
                isReturnAvailable = Convert.ToInt16(Val2);
            }
            else
            {
                isRetunLock = null;
                isReturnAvailable = null;
            }


        }
        #endregion

        #region Updating Order Delivery, Locking of SKU Item, Retail Temp Table Updation
        private void updateOrderDelivery(string orderNum, string orderLineNum, bool voided, string itemid = "")
        {
            StringBuilder commandText = new StringBuilder();
            if (orderNum == null && orderLineNum == null && !voided && !string.IsNullOrEmpty(itemid))
            {
                // commandText.Append(" UPDATE SKUTable_Posted SET isLocked='True' "); //SKU Table New
                commandText.Append(" UPDATE SKUTableTrans SET isLocked='True' ");
                commandText.Append(" WHERE SkuNumber='" + itemid + "' ");
                commandText.Append(" AND DATAAREAID='" + application.Settings.Database.DataAreaID + "' ");
                commandText.Append(" UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 ");
            }
            else
            {
                if (!voided)
                {
                    commandText.Append(" UPDATE CUSTORDER_DETAILS SET isDelivered = 1 WHERE ORDERNUM='" + orderNum.Trim() + "' AND  LINENUM='" + orderLineNum.Trim() + "' ");
                    commandText.Append(" DECLARE @COUNT INT ");
                    commandText.Append(" SELECT @COUNT=COUNT(LINENUM) FROM CUSTORDER_DETAILS WHERE ORDERNUM='" + orderNum.Trim() + "' AND isDelivered = 0  ");
                    commandText.Append(" IF(@COUNT=0) ");
                    commandText.Append(" BEGIN ");
                    commandText.Append(" UPDATE CUSTORDER_HEADER SET isDelivered = 1 WHERE ORDERNUM='" + orderNum.Trim() + "'  ");
                    commandText.Append(" END ");
                }
                else
                {
                    if (string.IsNullOrEmpty(orderNum))
                        orderNum = "0";
                    if (string.IsNullOrEmpty(orderLineNum))
                        orderLineNum = "0";
                    commandText.Append(" UPDATE CUSTORDER_DETAILS SET isDelivered = 0 WHERE ORDERNUM='" + orderNum.Trim() + "' AND  LINENUM='" + orderLineNum.Trim() + "' ");
                    commandText.Append(" DECLARE @COUNT INT ");
                    commandText.Append(" SELECT @COUNT=COUNT(LINENUM) FROM CUSTORDER_DETAILS WHERE ORDERNUM='" + orderNum.Trim() + "' AND isDelivered = 0  ");
                    commandText.Append(" IF(@COUNT>0) ");
                    commandText.Append(" BEGIN ");
                    commandText.Append(" UPDATE CUSTORDER_HEADER SET isDelivered = 0 WHERE ORDERNUM='" + orderNum.Trim() + "'  ");
                    commandText.Append(" END ");

                }
                //commandText.Append(" UPDATE SKUTable_Posted SET isLocked='False' "); //SKU Table New
                commandText.Append(" UPDATE SKUTableTrans SET isLocked='False' ");
                commandText.Append(" WHERE SkuNumber='" + itemid + "' ");
                commandText.Append(" AND DATAAREAID='" + application.Settings.Database.DataAreaID + "' ");
                commandText.Append(" UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 ");
            }

            ReturnExecuteNonQuery(commandText.ToString());
        }
        #endregion

        #region SaleOperation
        private bool SaleOperation(ISaleLineItem saleLineItem)
        {
            SaleLineItem saleLine = (SaleLineItem)saleLineItem;

            string sMRPRate = string.Empty;

            ArrayList al = new ArrayList();

            if (!string.IsNullOrEmpty(Convert.ToString(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).Customer.CustomerId)))
            {
                #region Adding Customer Order to Array List
                string query = " SELECT ORDERNUM AS CUSTOMERORDER FROM CUSTORDER_HEADER WHERE isDelivered=0 AND " +
                               " IsConfirmed=1 and custaccount='" + Convert.ToString(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).Customer.CustomerId) + "'";

                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                SqlCommand cmd = new SqlCommand(query.ToString(), connection);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        al.Add(Convert.ToString(reader["CUSTOMERORDER"]));
                    }
                }
                reader.Close();
                reader.Dispose();
                connection.Close();
                #endregion

            }

            bool isMRPExists = isMRP(saleLineItem.ItemId);
            sMRPRate = Convert.ToString(application.Services.Price.GetItemPrice(saleLine.ItemId, ((LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem)(saleLine)).BackofficeSalesOrderUnitOfMeasure));
            connection.Close();

            BlankOperations.WinFormsTouch.frmCustomFieldCalculations
                
                oCustomCalc = new BlankOperations.WinFormsTouch.frmCustomFieldCalculations(connection,
                                                                                           saleLine.ItemId,
                                                                                           ApplicationSettings.Database.StoreID,
                                                                                           string.IsNullOrEmpty(Convert.ToString(saleLine.Dimension.ConfigId)) ? "" : Convert.ToString(saleLine.Dimension.ConfigId),
                                                                                       al, sMRPRate, isMRPExists);
              


            oCustomCalc.ShowDialog();
            if (oCustomCalc.isCancelClick)
            {
                ReturnExecuteNonQuery(" UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 ");
                return false;
            }

            updateOrderDelivery(null, null, false, saleLine.ItemId);
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
            if (Convert.ToInt16(oCustomCalc.RadioChecked) == (int)Enums.EnumClass.TransactionType.Sale)
                list = oCustomCalc.saleList;
            else
                list = oCustomCalc.purchaseList;

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

            if (oCustomCalc.dtIngredientsClone != null && oCustomCalc.dtIngredientsClone.Rows.Count > 0)
            {
                oCustomCalc.dtIngredientsClone.TableName = "Ingredients";

                MemoryStream mstr = new MemoryStream();
                oCustomCalc.dtIngredientsClone.WriteXml(mstr, true);
                mstr.Seek(0, SeekOrigin.Begin);
                StreamReader sr = new StreamReader(mstr);
                string sXML = string.Empty;
                sXML = sr.ReadToEnd();
                saleLine.PartnerData.Ingredients = sXML;

            }
            else
            {
                saleLine.PartnerData.Ingredients = string.Empty;
            }


            saleLine.PartnerData.OrderNum = list[15].Value;
            saleLine.PartnerData.OrderLineNum = list[16].Value;
            saleLine.PartnerData.CustNo = Convert.ToString(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).Customer.CustomerId) == null ? string.Empty : Convert.ToString(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).Customer.CustomerId);
            saleLine.PartnerData.SampleReturn = string.IsNullOrEmpty(Convert.ToString(list[17].Value)) ? "0" : (Convert.ToBoolean(list[17].Value) ? Convert.ToString("1") : Convert.ToString("0"));
            // Added for Wastage
            saleLine.PartnerData.WastageType = list[18].Value;
            saleLine.PartnerData.WastageQty = list[19].Value;
            saleLine.PartnerData.WastageAmount = list[20].Value;
            saleLine.PartnerData.WastagePercentage = list[21].Value;
            saleLine.PartnerData.WastageRate = list[22].Value;
            //
            // Making Discount Type -- Extended
            saleLine.PartnerData.MakingDiscountType = list[23].Value;
            saleLine.PartnerData.MakingTotalDiscount = list[24].Value;
            //
            if (saleLineItem != null)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(saleLine.PartnerData.OrderNum)) && !string.IsNullOrEmpty(Convert.ToString(saleLine.PartnerData.OrderLineNum)))
                {
                    updateOrderDelivery(Convert.ToString(saleLine.PartnerData.OrderNum), Convert.ToString(saleLine.PartnerData.OrderLineNum), false);
                }
            }
            return true;
        }
        #endregion

        #region Automatically Disposable Dispose()
        public void Dispose()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Pre Sale Trigger Operation
        public ISaleLineItem PreSaleTriggerOperation(ISaleLineItem saleLineItem, ref string messageid, ref bool ContinueOperation)
        {
            RetailTransaction retailTrans = posTransaction as RetailTransaction;
            retailTrans.PartnerData.ItemIsReturnItem = false;

            #region - Changed By Nimbus
            SaleLineItem saleLine = (SaleLineItem)saleLineItem;

            string sMRPRate = string.Empty;
            if (saleLineItem != null)
            {
                #region Service Item
                if (saleLineItem.ItemId == AdjustmentItemID())
                {
                    System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem> sale = new System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem>(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).SaleItems);

                    DataRow drAdjustment = null;
                    CustomerAdvanceAdjustment oAdjustment = new CustomerAdvanceAdjustment(posTransaction);

                    if (sale.Count == 0)
                        drAdjustment = oAdjustment.AmountToBeAdjusted(retailTrans.Customer.CustomerId);
                    else
                        drAdjustment = oAdjustment.AmountToBeAdjusted(retailTrans.Customer.CustomerId, true);

                    if (drAdjustment == null)
                    {
                        ContinueOperation = false;
                        messageid = string.Empty;
                    }
                    else
                    {
                       
                        saleLine.PartnerData.ServiceItemCashAdjustmentPrice = Convert.ToString(drAdjustment["Total Amount"]);
                        saleLine.PartnerData.ServiceItemCashAdjustmentTransactionID = Convert.ToString(drAdjustment["Transaction ID"]);
                        ReturnExecuteNonQuery(" UPDATE RETAILADJUSTMENTTABLE SET ISADJUSTED='1' WHERE TRANSACTIONID='" + Convert.ToString(drAdjustment["Transaction ID"]) + "'");
                        ContinueOperation = true;
                        messageid = string.Empty;
                    }

                    return saleLineItem;
                }
                #endregion

                int? isLocked = null;
                int? isAvailable = null;
                int? isReturnLocked = null;
                int? isReturnAvailable = null;
                CheckForReturnSKUExistence(saleLine.ItemId, out isReturnLocked, out isReturnAvailable);

                if (isReturnAvailable == null && isReturnLocked == null)
                {
                    CheckForSKUExistence(saleLine.ItemId, out isLocked, out isAvailable);
                    if (isLocked != null && isAvailable != null)
                    {
                        if (Convert.ToBoolean(isLocked))
                        {
                            ContinueOperation = false;
                            messageid = "50379";
                            ReturnExecuteNonQuery(" UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 ");

                        }
                        else if (!Convert.ToBoolean(isLocked) && !Convert.ToBoolean(isAvailable))
                        {
                            ContinueOperation = false;
                            messageid = "50398";
                            ReturnExecuteNonQuery(" UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 ");

                        }
                        else
                        {
                            if (!SaleOperation(saleLine))
                            {
                                ContinueOperation = false;
                                messageid = string.Empty;
                            }
                        }

                    }

                    else
                    {

                        if (!SaleOperation(saleLineItem))
                        {
                            ContinueOperation = false;
                            messageid = string.Empty;
                        }


                    }
                }
                else if (isReturnAvailable == 1 && isReturnLocked == 1)
                {
                    ContinueOperation = false;
                    messageid = "50398";
                    ReturnExecuteNonQuery(" UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 ");


                }
                else if (isReturnAvailable == 1 && isReturnLocked == 0)
                {
                    ContinueOperation = false;
                    messageid = "50395";
                    ReturnExecuteNonQuery(" UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 ");

                }
                else if ((isReturnAvailable == 0 && isReturnLocked == 0))
                {
                    if (!SaleOperation(saleLineItem))
                    {
                        ContinueOperation = false;
                        messageid = string.Empty;
                    }
                }
            }
            return saleLineItem;
            #endregion
        }
        #endregion

        #region Miscellaneous Item Trigger Operations
        public void SaleTriggerOperations(string operation)
        {
            if (operation.ToUpper().Trim() == "POSTSALE")
                ReturnExecuteNonQuery(" UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 ");
            else if (operation.ToUpper().Trim() == "PRERETURN")
            {
                ReturnExecuteNonQuery(" UPDATE RETAILTEMPTABLE SET ITEMRETURN=(CASE  WHEN ITEMRETURN='False' THEN 'True' ELSE 'False' END) WHERE ID=2 ");
                RetailTransaction retailTrans = posTransaction as RetailTransaction;
                retailTrans.PartnerData.ItemIsReturnItem = true;
            }
        }
        #endregion

        #region Pre Void Operation
        public bool preVoid(int lineId)
        {
            bool returnVal = false;
            RetailTransaction retailTrans = posTransaction as RetailTransaction;
            if (retailTrans != null)
            {
                foreach (SaleLineItem saleLineItem in retailTrans.SaleItems)
                {
                    if (saleLineItem.ItemId != AdjustmentItemID())
                    {
                        if (Convert.ToString(lineId) == Convert.ToString(saleLineItem.LineId) && saleLineItem.Voided)
                        {
                            returnVal = true;
                            break;
                        }
                    }
                }

            }
            return returnVal;
        }
        #endregion

        #region Post Void Operation
        public void postvoidOperation(int lineId)
        {
            #region changed By Nimbus

            RetailTransaction retailTrans = posTransaction as RetailTransaction;
            if (retailTrans != null)
            {
                foreach (SaleLineItem saleLineItem in retailTrans.SaleItems)
                {
                    if (saleLineItem.ItemId != AdjustmentItemID())
                    {
                        if (Convert.ToString(lineId) == Convert.ToString(saleLineItem.LineId))
                        {
                            updateOrderDelivery(Convert.ToString(saleLineItem.PartnerData.OrderNum), Convert.ToString(saleLineItem.PartnerData.OrderLineNum), true, saleLineItem.ItemId);
                        }
                    }

                }

            }

            #endregion
        }
        #endregion

        #region Check for Service Item
        private bool isServiceItem(IPosTransaction transaction, int lineid, ISaleLineItem saleLineItem, string operation)
        {
            if (lineid == 0)
                return false;

            bool isServiceItemExists = false;
            SaleLineItem saleitem = saleLineItem as SaleLineItem;
            if (operation.ToUpper().Trim() != "QTY")
            {
                saleitem = null;
            }
            if (saleitem == null)
            {
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

            }
            return isServiceItemExists;
        }
        #endregion
        #endregion
    }
}
