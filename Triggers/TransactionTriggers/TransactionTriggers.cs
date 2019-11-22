//Microsoft Dynamics AX for Retail POS Plug-ins p
//The following project is provided as SAMPLE code. Because this software is "as is," we may not provide support services for it.

using System.ComponentModel.Composition;
using System.Data.SqlClient;
using LSRetailPosis;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Triggers;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using LSRetailPosis.Transaction;
using System;
using LSRetailPosis.Settings;
using System.Data;
using LSRetailPosis.Transaction.Line.SaleItem;
using System.Text;
using System.IO;
using Microsoft.CSharp.RuntimeBinder;
using System.Collections.ObjectModel;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch;
using LSRetailPosis.Transaction.Line.TenderItem;
using Microsoft.Dynamics.Retail.Notification.Contracts;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.SystemCore;


namespace Microsoft.Dynamics.Retail.Pos.TransactionTriggers
{
    [Export(typeof(ITransactionTrigger))]
    public class TransactionTriggers:ITransactionTrigger
    {
        string preTransType = string.Empty; ////Start : Changes on 08/04/2014 RHossain   
        string curTransType = string.Empty; //Start : Changes on 08/04/2014 RHossain   
        string sSalesInfoCode = string.Empty;//Start : Changes on 22/05/2014 RHossain  

        string sSplReturn = string.Empty;

        #region Constructor - Destructor

        public TransactionTriggers()
        {

            // Get all text through the Translation function in the ApplicationLocalizer
            // TextID's for TransactionTriggers are reserved at 50300 - 50349
        }

        #endregion

        #region enum CRWRetailDiscPermission
        private enum CRWRetailDiscPermission
        {
            Cashier = 0,
            Salesperson = 1,
            Manager = 2,
            Other = 3,
            Manager2 = 4,
        }
        #endregion

        #region ITransactionTriggers Members


        public void BeginTransaction(IPosTransaction posTransaction)
        {
            #region Changed By Nimbus

            RetailTransaction retailTrans = posTransaction as RetailTransaction;

            if(retailTrans != null)
            {
                retailTrans.PartnerData.AdjustmentOrderNum = string.Empty;
                retailTrans.PartnerData.AdjustmentCustAccount = string.Empty;
                retailTrans.PartnerData.SKUBookedItems = false;
                retailTrans.PartnerData.SKUBookedItemsExists = "N";
                retailTrans.PartnerData.ItemIsReturnItem = false;
                retailTrans.PartnerData.IsGSSMaturity = false;
                retailTrans.PartnerData.SaleAdjustment = false;
                retailTrans.PartnerData.EFTCardNo = string.Empty;
                retailTrans.PartnerData.LCCustomerName = string.Empty;
                retailTrans.PartnerData.PackingMaterial = string.Empty;
                retailTrans.PartnerData.CertificateIssue = string.Empty;
                retailTrans.PartnerData.IsRepairRetTrans = false;
                retailTrans.PartnerData.RefRepairId = string.Empty;
                retailTrans.PartnerData.Remarks = string.Empty;
                retailTrans.PartnerData.TouristNumber = string.Empty;
                retailTrans.PartnerData.TotalValueChanged = false;

                if(string.IsNullOrEmpty(Convert.ToString(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).InventLocationId))
                    && string.IsNullOrEmpty(Convert.ToString(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).InventSiteId)))
                {
                    string commandtext = " SELECT     RETAILCHANNELTABLE.INVENTLOCATION, INVENTLOCATION.INVENTSITEID  " +
                                     " FROM         RETAILCHANNELTABLE INNER JOIN " +
                                     " INVENTLOCATION ON RETAILCHANNELTABLE.INVENTLOCATION = INVENTLOCATION.INVENTLOCATIONID INNER JOIN " +
                                     " RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID " +
                                     " WHERE     (RETAILSTORETABLE.STORENUMBER = '" + posTransaction.StoreId + "') ";

                    SqlConnection connection = new SqlConnection();
                    if(application != null)
                        connection = application.Settings.Database.Connection;
                    else
                        connection = ApplicationSettings.Database.LocalConnection;
                    if(connection.State == ConnectionState.Closed)
                        connection.Open();
                    SqlCommand command = new SqlCommand(commandtext, connection);
                    command.CommandTimeout = 0;
                    SqlDataReader reader = command.ExecuteReader();

                    if(reader.HasRows)
                    {
                        while(reader.Read())
                        {
                            ((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).InventLocationId = Convert.ToString(reader.GetValue(0));
                            ((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).InventSiteId = Convert.ToString(reader.GetValue(1));



                        }
                    }
                    if(connection.State == ConnectionState.Open)
                        connection.Close();

                }


            }

            #endregion


            LSRetailPosis.ApplicationLog.Log("TransactionTriggers.BeginTransaction", "When starting the transaction...", LSRetailPosis.LogTraceLevel.Trace);
        }

        #region Changed By Nimbus - Update Customer Advance Adjustment
        private void updateCustomerAdvanceAdjustment(string transactionid,
                                                     string sStoreId, string sTerminalId, int adjustment)
        {
            SqlConnection connection = new SqlConnection();
            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if(connection.State == ConnectionState.Closed)
                connection.Open();
            string commandText = string.Empty;

            //commandText = " UPDATE RETAILTRANSACTIONPAYMENTTRANS SET isAdjusted = '" + adjustment + "' WHERE TRANSACTIONID ='" + transactionid.Trim() + "' ";
            commandText += " UPDATE RETAILADJUSTMENTTABLE SET ISADJUSTED = '" + adjustment + "' WHERE TRANSACTIONID ='" + transactionid.Trim() + "' AND RETAILSTOREID = '" + sStoreId + "' AND RETAILTERMINALID ='" + sTerminalId + "' ";

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;
            command.ExecuteNonQuery();

        }
        #endregion

        /// <summary>
        /// Triggered during save of a transaction to database.
        /// </summary>
        /// <param name="posTransaction">PosTransaction object.</param>
        /// <param name="sqlTransaction">SqlTransaction object.</param>
        /// <remarks>
        /// Use provided sqlTransaction to write to the DB. Don't commit, rollback transaction or close the connection.
        /// Any exception thrown from this trigger will rollback the saving of pos transaction.
        /// </remarks>
        public void SaveTransaction(IPosTransaction posTransaction, SqlTransaction sqlTransaction)
        {
            ApplicationLog.Log("TransactionTriggers.SaveTransaction", "Saving a transaction.", LogTraceLevel.Trace);

            #region Saving Custom Fields to DB - CHANGED BY NIMBUS
            try
            {
                RetailTransaction retailTransaction = posTransaction as RetailTransaction;

                if(retailTransaction != null && sqlTransaction != null)
                {
                    // START : DEV ON :22/05/2014

                    decimal dBillAmt = 0.0m;
                    dBillAmt = retailTransaction.AmountDue;
                    foreach(SaleLineItem saleLineItem in retailTransaction.SaleItems)
                    {
                        if(saleLineItem.ItemType == LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                        {
                            if(!saleLineItem.Voided)
                            {
                                dBillAmt = dBillAmt + saleLineItem.OriginalPrice;
                            }
                        }
                    }

                    //decimal dAmount = GetMaxInvoiceAmount(sqlTransaction);
                    //if (dBillAmt >= dAmount && retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                    //{
                    //    BlankOperations.WinFormsTouch.frmSalesInfoCode frmSalesCode = new frmSalesInfoCode();
                    //    frmSalesCode.ShowDialog();

                    //    sSalesInfoCode = frmSalesCode.sCodeOrRemarks;
                    //}
                    // END : DEV ON :22/05/2014

                    #region  GSS Maturity

                    if(retailTransaction.PartnerData.IsGSSMaturity)
                    {
                        if(retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                        {
                            if(this.Application.TransactionServices.CheckConnection())
                            {
                                bool bResult = false;
                                string sGSSNo = Convert.ToString(retailTransaction.PartnerData.GSSMaturityNo);

                                ReadOnlyCollection<object> containerArray;
                                containerArray = this.Application.TransactionServices.InvokeExtension("UpdateGSSMaturityAdjustment",
                                                                                                      sGSSNo, true, DateTime.Now, ((LSRetailPosis.Transaction.PosTransaction)(retailTransaction)).ReceiptId);

                                bResult = Convert.ToBoolean(containerArray[1]);
                                if(!bResult)
                                    return;
                            }
                        }
                    }

                    #endregion

                    int iIngredient = 0;

                    string commandString = " INSERT INTO RETAIL_CUSTOMCALCULATIONS_TABLE " +
                                           " ([DATAAREAID],[STOREID],[TERMINALID],[TRANSACTIONID],[LINENUM],[PIECES],[QUANTITY],[RATETYPE],[MAKINGRATE] " +
                                           " ,[MAKINGTYPE],[AMOUNT],[MAKINGDISCOUNT],[MAKINGAMOUNT],[TOTALAMOUNT],[TOTALWEIGHT],[LOSSPERCENTAGE],[LOSSWEIGHT] " +
                                           " ,[EXPECTEDQUANTITY],[TRANSACTIONTYPE],[OWN],[SKUDETAILS],[ORDERNUM],[ORDERLINENUM],[CUSTNO],[RETAILSTAFFID] " +
                                           " ,[IsIngredient],[MakingDiscountAmount],[STUDDAMOUNT],[CRate],[ADVANCEADJUSTMENTID],[SAMPLERETURN] " +
                                           " ,[WastageType],[WastageQty],[WastageAmount],[WastagePercentage],[WastageRate],[MAKINGDISCTYPE]," +
                                           " [CONFIGID],[ADVANCEADJUSTMENTSTOREID],[ADVANCEADJUSTMENTTERMINALID],[PURITY]" + // Changed for wastage and Making Discount
                                           " ,OGGROSSUNIT,OGDMDPCS,OGDMDWT,OGDMDUNIT,OGDMDAMT,OGSTONEPCS,OGSTONEWT,OGSTONEUNIT,OGSTONEAMT," +
                                           " OGNETWT,OGNETRATE,OGNETUNIT,OGNETAMT,REFINVOICENO,SALESCHANNELTYPE," +
                                           " SELLINGPRICE,ITEMIDPARENT,UPDATEDCOSTPRICE,GROUPCOSTPRICE,PROMOTIONCODE,FLAT,SPECIALDISCINFO,REMARKS )" +
                        //  " ,LocalCustomerName,LocalCustomerAddress,LocalCustomerContactNo)" +
                                           " VALUES " +
                                           " (@DATAAREAID,@STOREID,@TERMINALID,@TRANSACTIONID,@LINENUM,@PIECES,@QUANTITY," +
                                           " @RATETYPE,@MAKINGRATE,@MAKINGTYPE,@AMOUNT,@MAKINGDISCOUNT,@MAKINGAMOUNT,@TOTALAMOUNT, " +
                                           " @TOTALWEIGHT,@LOSSPERCENTAGE,@LOSSWEIGHT,@EXPECTEDQUANTITY,@TRANSACTIONTYPE,@OWN,@SKUDETAILS, " +
                                           " @ORDERNUM,@ORDERLINENUM,@CUSTNO,@RETAILSTAFFID,@ISINGREDIENT,@MAKINGDISCAMT,@STUDDAMOUNT " +
                                           " ,@RATE,@ADVANCEADJUSTMENTID,@SAMPLERETURN,@WastageType,@WastageQty,@WastageAmount,@WastagePercentage," +
                                           " @WastageRate,@MAKINGDISCTYPE,@CONFIGID,@ADVANCEADJUSTMENTSTOREID,@ADVANCEADJUSTMENTTERMINALID,@PURITY" +  // Changed for wastage
                                           " ,@OGGROSSUNIT,@OGDMDPCS,@OGDMDWT,@OGDMDUNIT,@OGDMDAMT,@OGSTONEPCS,@OGSTONEWT,@OGSTONEUNIT,@OGSTONEAMT," +
                                           " @OGNETWT,@OGNETRATE,@OGNETUNIT,@OGNETAMT,@REFINVOICENO,@SALESCHANNELTYPE," +
                                           " @SELLINGPRICE,@ITEMIDPARENT,@UPDATEDCOSTPRICE,@GROUPCOSTPRICE,@PROMOTIONCODE,@FLAT,@SPECIALDISCINFO,@REMARKS )" +
                        // " ,@LocalCustomerName,@LocalCustomerAddress,@LocalCustomerContactNo)" +
                                           " " +
                                           " DECLARE @isVoided bit " +
                                           " SET @isVoided = @isVoid ";

                    if(retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                    {
                        commandString = commandString +
                                         " IF(@isVoided='False' OR @isVoided='0') " +
                                         " BEGIN ";
                        commandString = commandString +
                            //   " IF CAST(@ITEMAMOUNT AS NUMERIC(28,2)) > 0 " + 
                                  " IF CAST(@ITEMAMOUNT AS NUMERIC(28,2)) >= 0 " +
                                  " BEGIN " +
                            //  " UPDATE SKUTable_Posted SET isAvailable='False',isLocked='False' WHERE SkuNumber=@INVENTITEMID " +
                                  " UPDATE SKUTableTrans SET isAvailable='False',isLocked='False' WHERE SkuNumber=@INVENTITEMID " + //SKU Table New
                            //    " AND STOREID=@STOREID " + 
                                  " AND DATAAREAID=@DATAAREAID  " +
                                  " END " +
                                  " ELSE " +
                            // " UPDATE SKUTable_Posted SET isAvailable='True',isLocked='False' WHERE SkuNumber=@INVENTITEMID " +
                                  " UPDATE SKUTableTrans SET isAvailable='True',isLocked='False' WHERE SkuNumber=@INVENTITEMID " + //SKU Table New
                            //" AND STOREID=@STOREID " +
                                  " AND DATAAREAID=@DATAAREAID  ";
                        commandString = commandString +
                                         " UPDATE CUSTORDER_DETAILS SET isDelivered = 1 WHERE ORDERNUM=@ORDERNUM AND LINENUM=@ORDERLINENUM " +
                                         " DECLARE @COUNT INT " +
                                         " SELECT @COUNT=COUNT(LINENUM) FROM CUSTORDER_DETAILS WHERE ORDERNUM=@ORDERNUM AND isDelivered = 0  " +
                                         " IF(@COUNT=0) " +
                                         " BEGIN " +
                                         " UPDATE CUSTORDER_HEADER SET isDelivered = 1 WHERE ORDERNUM=@ORDERNUM  " +
                                         " END " +
                                         " END " +
                                         " ELSE " +
                                         " BEGIN " +
                                         " UPDATE CUSTORDER_DETAILS SET isDelivered = 0 WHERE ORDERNUM=@ORDERNUM AND LINENUM=@ORDERLINENUM " +
                                         " DECLARE @COUNT1 INT " +
                                         " SELECT @COUNT1=COUNT(LINENUM) FROM CUSTORDER_DETAILS WHERE ORDERNUM=@ORDERNUM AND isDelivered = 0  " +
                                         " IF(@COUNT1>0) " +
                                         " BEGIN " +
                                         " UPDATE CUSTORDER_HEADER SET isDelivered = 0 WHERE ORDERNUM=@ORDERNUM  " +
                                         " END " +
                                         " END; ";

                        if(retailTransaction.PartnerData.SKUBookedItemsExists == "Y")
                        {
                            string skunumber = string.Empty;
                            foreach(SaleLineItem saleLineItem in retailTransaction.SaleItems)
                            {
                                if(!saleLineItem.Voided)
                                {
                                    if(string.IsNullOrEmpty(skunumber))
                                        skunumber = "'" + saleLineItem.ItemId + "'";
                                    else
                                        skunumber += ",'" + saleLineItem.ItemId + "'";
                                }
                            }
                            if(string.IsNullOrEmpty(skunumber))
                                skunumber = "''";

                            commandString = commandString + " UPDATE RETAILCUSTOMERDEPOSITSKUDETAILS SET DELIVERED=1 WHERE SKUNUMBER IN (" + skunumber + ")";
                        }
                        //foreach (SaleLineItem saleLineItem in retailTransaction.SaleItems)
                        //{
                        //    if (!string.IsNullOrEmpty(Convert.ToString(saleLineItem.ReturnTransId)))
                        //    {
                        //        commandString = commandString +
                        //                        " UPDATE SKUTable_Posted SET isAvailable='True',isLocked='False' WHERE SkuNumber=@INVENTITEMID ";
                        //    }
                        //}

                        if(retailTransaction.SaleIsReturnSale == true)
                        {
                            //commandString = commandString + "UPDATE SKUTableTrans SET TOCOUNTER = (SELECT TOP (1)COUNTERCODE " +
                            //                " FROM RETAILSTORECOUNTERTABLE WHERE  RETAILSTOREID = '" + retailTransaction.StoreId + "'" +
                            //                " AND DEFAULTSC = 1) WHERE SKUNUMBER = @INVENTITEMID";

                            commandString = commandString + " DECLARE @COUNTERCODE AS NVARCHAR(20)" +
                                            " SELECT @COUNTERCODE = COUNTERCODE FROM RETAILSTORECOUNTERTABLE WHERE  RETAILSTOREID = '" + ApplicationSettings.Terminal.StoreId + "' AND DEFAULTSC = 1" +
                                            " IF EXISTS (SELECT SKUNUMBER FROM SKUTableTrans WHERE SKUNUMBER = @INVENTITEMID) BEGIN " +
                                            " UPDATE SKUTableTrans SET TOCOUNTER = @COUNTERCODE WHERE SKUNUMBER = @INVENTITEMID END ELSE BEGIN" +
                                            " IF EXISTS (SELECT SKUNUMBER FROM SKUTable_Posted WHERE SKUNUMBER = @INVENTITEMID) BEGIN " +
                                            " INSERT INTO SKUTableTrans (SkuDate,SkuNumber,DATAAREAID,CREATEDON" +
                                            " ,isLocked,isAvailable,PDSCWQTY,QTY,INGREDIENT,FROMCOUNTER,TOCOUNTER,ECORESCONFIGURATIONNAME,INVENTCOLORID,INVENTSIZEID,RETAILSTOREID)" +
                                            " SELECT SkuDate,SkuNumber,DATAAREAID,CREATEDON,isLocked," +
                                            " 1,PDSCWQTY,QTY,INGREDIENT,FROMCOUNTER,@COUNTERCODE,ECORESCONFIGURATIONNAME,INVENTCOLORID,INVENTSIZEID,'" + ApplicationSettings.Terminal.StoreId + "'" +
                                            " FROM SKUTable_Posted WHERE SKUNUMBER = @INVENTITEMID END END"
                                            ;
                        }
                    }
                    else
                    {
                        commandString = commandString +
                                         " UPDATE CUSTORDER_DETAILS SET isDelivered = 0 WHERE ORDERNUM=@ORDERNUM AND LINENUM=@ORDERLINENUM " +
                                         " DECLARE @COUNT2 INT " +
                                         " SELECT @COUNT2=COUNT(LINENUM) FROM CUSTORDER_DETAILS WHERE ORDERNUM=@ORDERNUM AND isDelivered = 0  " +
                                         " IF(@COUNT2>0) " +
                                         " BEGIN " +
                                         " UPDATE CUSTORDER_HEADER SET isDelivered = 0 WHERE ORDERNUM=@ORDERNUM  " +
                                         " END ";
                        commandString = commandString +

                                // " UPDATE SKUTable_Posted SET isAvailable=(CASE  WHEN isAvailable='False' THEN 'False' ELSE 'True' END),isLocked='False' WHERE SkuNumber=@INVENTITEMID " +
                                 " UPDATE SKUTableTrans SET isAvailable=(CASE  WHEN isAvailable='False' THEN 'False' ELSE 'True' END),isLocked='False' WHERE SkuNumber=@INVENTITEMID " + //SKU Table New
                            // " AND STOREID=@STOREID " +
                                 " AND DATAAREAID=@DATAAREAID  " +
                                                         " ";
                    }

                    foreach(SaleLineItem saleLineItem in retailTransaction.SaleItems)
                    {
                        if(saleLineItem.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                        {
                            //object o = ((System.Dynamic.DynamicObject)(saleLineItem.PartnerData));

                            if(!string.IsNullOrEmpty(Convert.ToString(saleLineItem.PartnerData.Ingredients)))
                            {
                                string stest = Convert.ToString(saleLineItem.PartnerData.Ingredients);

                                iIngredient = 1;
                                StringBuilder sbIngredients = new StringBuilder();
                                sbIngredients.Append(" INSERT INTO [RETAIL_SALE_INGREDIENTS_DETAILS] ");
                                sbIngredients.Append("  ([DATAAREAID],[STOREID],[TERMINALID],[TRANSACTIONID],[REFLINENUM],[LINENUM],[STAFFID],[SKUNUMBER] ");
                                sbIngredients.Append(" ,[ITEMID],[INVENTDIMID],[INVENTLOCATIONID],[INVENTSIZEID],[INVENTCOLORID] ");
                                sbIngredients.Append(" ,[CONFIGID],[INVENTBATCHID],[PCS],[GRWEIGHT],[QTY] ,[CVALUE],[CRATE],[INVENTSITEID] ");
                                sbIngredients.Append("  ,[UNITID],[CREATEDON],[SKUDATE],[CTYPE] ");
                                sbIngredients.Append(" ,[IngrdDiscType],[IngrdDiscAmt],[IngrdDiscTotAmt])"); // Stone Discount
                                sbIngredients.Append(" VALUES ");
                                sbIngredients.Append("  (@DATAAREAID,@STOREID,@TERMINALID, @TRANSACTIONID,@REFLINENUM, @LINENUM,@STAFFID, @SKUNUMBER ");
                                sbIngredients.Append(" ,@ITEMID,@INVENTDIMID, @INVENTLOCATIONID,@INVENTSIZEID, @INVENTCOLORID, @CONFIGID ");
                                sbIngredients.Append(" ,@INVENTBATCHID,@PCS, @GRWEIGHT, @QTY,@CVALUE, @RATE, @INVENTSITEID, @UNITID, GETDATE(),GETDATE(),@CTYPE,@IngrdDiscType,@IngrdDiscAmt,@IngrdDiscTotAmt) ");

                                DataSet dsIngredients = new DataSet();
                                StringReader reader = new StringReader(Convert.ToString(saleLineItem.PartnerData.Ingredients));
                                dsIngredients.ReadXml(reader);
                                int i = 1;
                                int index = 1;
                                foreach(DataRow drIngredients in dsIngredients.Tables[0].Rows)
                                {
                                    index = i;
                                    using(SqlCommand sqlCmd = new SqlCommand(sbIngredients.ToString(), sqlTransaction.Connection, sqlTransaction))
                                    {
                                        sqlCmd.Parameters.Add(new SqlParameter("@DATAAREAID", ApplicationSettings.Database.DATAAREAID));
                                        sqlCmd.Parameters.Add(new SqlParameter("@STOREID", posTransaction.StoreId));
                                        sqlCmd.Parameters.Add(new SqlParameter("@TERMINALID", posTransaction.TerminalId));
                                        sqlCmd.Parameters.Add(new SqlParameter("@TRANSACTIONID", posTransaction.TransactionId));
                                        sqlCmd.Parameters.Add(new SqlParameter("@REFLINENUM", saleLineItem.LineId));
                                        sqlCmd.Parameters.Add(new SqlParameter("@LINENUM", index));
                                        sqlCmd.Parameters.Add(new SqlParameter("@STAFFID", posTransaction.OperatorId));
                                        sqlCmd.Parameters.Add(new SqlParameter("@SKUNUMBER", !string.IsNullOrEmpty(Convert.ToString(drIngredients["SkuNumber"])) ? drIngredients["SkuNumber"] : string.Empty));

                                        //     sqlCmd.Parameters.Add(new SqlParameter("@LTYPE", !string.IsNullOrEmpty(Convert.ToString(drIngredients["LType"])) ? drIngredients["LType"] : DBNull.Value));
                                        //   sqlCmd.Parameters.Add(new SqlParameter("@LTYPE", DBNull.Value));
                                        sqlCmd.Parameters.Add(new SqlParameter("@ITEMID", !string.IsNullOrEmpty(Convert.ToString(drIngredients["ItemID"])) ? drIngredients["ItemID"] : string.Empty));
                                        sqlCmd.Parameters.Add(new SqlParameter("@INVENTDIMID", !string.IsNullOrEmpty(Convert.ToString(drIngredients["InventDimID"])) ? drIngredients["InventDimID"] : string.Empty));
                                        //  sqlCmd.Parameters.Add(new SqlParameter("@INVENTLOCATIONID", !string.IsNullOrEmpty(Convert.ToString(drIngredients["InventLocationID"])) ? drIngredients["InventLocationID"] : DBNull.Value));
                                        //  sqlCmd.Parameters.Add(new SqlParameter("@INVENTLOCATIONID", DBNull.Value));
                                        if(string.IsNullOrEmpty(Convert.ToString(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).InventLocationId)))
                                            sqlCmd.Parameters.Add(new SqlParameter("@INVENTLOCATIONID", string.Empty));
                                        else
                                            sqlCmd.Parameters.Add(new SqlParameter("@INVENTLOCATIONID", Convert.ToString(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).InventLocationId)));
                                        sqlCmd.Parameters.Add(new SqlParameter("@INVENTSIZEID", !string.IsNullOrEmpty(Convert.ToString(drIngredients["InventSizeID"])) ? drIngredients["InventSizeID"] : string.Empty));
                                        sqlCmd.Parameters.Add(new SqlParameter("@INVENTCOLORID", !string.IsNullOrEmpty(Convert.ToString(drIngredients["InventColorID"])) ? drIngredients["InventColorID"] : string.Empty));
                                        sqlCmd.Parameters.Add(new SqlParameter("@CONFIGID", !string.IsNullOrEmpty(Convert.ToString(drIngredients["ConfigID"])) ? drIngredients["ConfigID"] : string.Empty));
                                        sqlCmd.Parameters.Add(new SqlParameter("@INVENTBATCHID", !string.IsNullOrEmpty(Convert.ToString(drIngredients["InventBatchID"])) ? drIngredients["InventBatchID"] : string.Empty));
                                        sqlCmd.Parameters.Add(new SqlParameter("@PCS", !string.IsNullOrEmpty(Convert.ToString(drIngredients["PCS"])) ? drIngredients["PCS"] : string.Empty));
                                        //   sqlCmd.Parameters.Add(new SqlParameter("@GRWEIGHT", !string.IsNullOrEmpty(Convert.ToString(drIngredients["GrWeight"])) ? drIngredients["GrWeight"] : DBNull.Value));
                                        //  sqlCmd.Parameters.Add(new SqlParameter("@GRWEIGHT", DBNull.Value));

                                        sqlCmd.Parameters.Add(new SqlParameter("@GRWEIGHT", !string.IsNullOrEmpty(Convert.ToString(drIngredients["QTY"])) ? drIngredients["QTY"] : string.Empty));
                                        sqlCmd.Parameters.Add(new SqlParameter("@QTY", !string.IsNullOrEmpty(Convert.ToString(drIngredients["QTY"])) ? drIngredients["QTY"] : string.Empty));
                                        sqlCmd.Parameters.Add(new SqlParameter("@CVALUE", !string.IsNullOrEmpty(Convert.ToString(drIngredients["CValue"])) ? drIngredients["CValue"] : string.Empty));
                                        sqlCmd.Parameters.Add(new SqlParameter("@RATE", !string.IsNullOrEmpty(Convert.ToString(drIngredients["Rate"])) ? drIngredients["Rate"] : string.Empty));

                                        //  sqlCmd.Parameters.Add(new SqlParameter("@CTYPE", Convert.ToString((int)Enums.EnumClass.CalcType.Weight)));
                                        sqlCmd.Parameters.Add(new SqlParameter("@CTYPE", !string.IsNullOrEmpty(Convert.ToString(drIngredients["CTYPE"])) ? drIngredients["CTYPE"] : "0"));

                                        //   sqlCmd.Parameters.Add(new SqlParameter("@CTYPE", DBNull.Value));
                                        //sqlCmd.Parameters.Add(new SqlParameter("@INVENTSITEID", !string.IsNullOrEmpty(Convert.ToString(drIngredients["InventSiteId"])) ? drIngredients["InventSiteId"] : DBNull.Value));
                                        //   sqlCmd.Parameters.Add(new SqlParameter("@INVENTSITEID", DBNull.Value));
                                        if(string.IsNullOrEmpty(Convert.ToString(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).InventSiteId)))
                                            sqlCmd.Parameters.Add(new SqlParameter("@INVENTSITEID", string.Empty));
                                        else
                                            sqlCmd.Parameters.Add(new SqlParameter("@INVENTSITEID", Convert.ToString(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).InventSiteId)));
                                        sqlCmd.Parameters.Add(new SqlParameter("@UNITID", !string.IsNullOrEmpty(Convert.ToString(drIngredients["UnitID"])) ? drIngredients["UnitID"] : string.Empty));

                                        if(string.IsNullOrEmpty(Convert.ToString(saleLineItem.ReturnTransId))) // added on 16.07.2013
                                        {
                                            sqlCmd.Parameters.Add(new SqlParameter("@IngrdDiscType", !string.IsNullOrEmpty(Convert.ToString(drIngredients["IngrdDiscType"])) ? drIngredients["IngrdDiscType"] : "0")); // Stone Discount
                                            sqlCmd.Parameters.Add(new SqlParameter("@IngrdDiscAmt", !string.IsNullOrEmpty(Convert.ToString(drIngredients["IngrdDiscAmt"])) ? drIngredients["IngrdDiscAmt"] : "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@IngrdDiscTotAmt", !string.IsNullOrEmpty(Convert.ToString(drIngredients["IngrdDiscTotAmt"])) ? drIngredients["IngrdDiscTotAmt"] : "0"));
                                        }
                                        else
                                        {
                                            sqlCmd.Parameters.Add(new SqlParameter("@IngrdDiscType", "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@IngrdDiscAmt", "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@IngrdDiscTotAmt", "0"));
                                        }

                                        //studdamount += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["CValue"])) ? drIngredients["CValue"] : 0);

                                        sqlCmd.ExecuteNonQuery();
                                        i++;
                                    }
                                }
                            }

                            using(SqlCommand sqlCmd = new SqlCommand(commandString, sqlTransaction.Connection, sqlTransaction))
                            {
                                #region [ Sale / Purchase Return / Exchange Return]
                                if((Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 0
                                   || Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 2
                                   || Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 4) && (!retailTransaction.SaleIsReturnSale))
                                {
                                    decimal studdamount = 0;
                                    if(!string.IsNullOrEmpty(Convert.ToString(saleLineItem.PartnerData.Ingredients)))
                                    {
                                        DataSet dsIngredients = new DataSet();
                                        StringReader reader = new StringReader(Convert.ToString(saleLineItem.PartnerData.Ingredients));
                                        dsIngredients.ReadXml(reader);
                                        foreach(DataRow drIngredients in dsIngredients.Tables[0].Rows)
                                        {
                                            studdamount += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["CValue"])) ? drIngredients["CValue"] : 0);
                                        }
                                    }

                                    sqlCmd.Parameters.Add(new SqlParameter("@PIECES", !string.IsNullOrEmpty(saleLineItem.PartnerData.Pieces) ? Convert.ToString(Convert.ToDecimal(saleLineItem.PartnerData.Pieces) * -1) : "0"));

                                    // sqlCmd.Parameters.Add(new SqlParameter("@QUANTITY", !string.IsNullOrEmpty(saleLineItem.PartnerData.Quantity) ? saleLineItem.PartnerData.Quantity : "0"));
                                    sqlCmd.Parameters.Add(new SqlParameter("@QUANTITY", !string.IsNullOrEmpty(saleLineItem.PartnerData.Quantity) ? Convert.ToString(Convert.ToDecimal(saleLineItem.PartnerData.Quantity) * -1) : "0"));


                                    // sqlCmd.Parameters.Add(new SqlParameter("@RATE", !string.IsNullOrEmpty(saleLineItem.PartnerData.Rate) ? saleLineItem.PartnerData.Rate : "0"));

                                    if(Convert.ToInt16(saleLineItem.PartnerData.RateType) == 2) // Tot
                                    {
                                        sqlCmd.Parameters.Add(new SqlParameter("@RATE", !string.IsNullOrEmpty(saleLineItem.PartnerData.Rate) ? Convert.ToString(Convert.ToDecimal(saleLineItem.PartnerData.Rate) * -1) : "0"));
                                    }
                                    else
                                    {
                                        sqlCmd.Parameters.Add(new SqlParameter("@RATE", !string.IsNullOrEmpty(saleLineItem.PartnerData.Rate) ? saleLineItem.PartnerData.Rate : "0"));
                                    }


                                    //sqlCmd.Parameters.Add(new SqlParameter("@MAKINGRATE", !string.IsNullOrEmpty(saleLineItem.PartnerData.MakingRate) ? saleLineItem.PartnerData.MakingRate : "0"));

                                    if(Convert.ToInt16(saleLineItem.PartnerData.MakingType) == 3) // Tot
                                    {
                                        sqlCmd.Parameters.Add(new SqlParameter("@MAKINGRATE", !string.IsNullOrEmpty(saleLineItem.PartnerData.MakingRate) ? Convert.ToString(Convert.ToDecimal(saleLineItem.PartnerData.MakingRate) * -1) : "0"));
                                    }
                                    else
                                    {
                                        sqlCmd.Parameters.Add(new SqlParameter("@MAKINGRATE", !string.IsNullOrEmpty(saleLineItem.PartnerData.MakingRate) ? saleLineItem.PartnerData.MakingRate : "0"));
                                    }


                                    //sqlCmd.Parameters.Add(new SqlParameter("@MAKINGAMOUNT", !string.IsNullOrEmpty(saleLineItem.PartnerData.MakingAmount) ? saleLineItem.PartnerData.MakingAmount : "0"));
                                    sqlCmd.Parameters.Add(new SqlParameter("@MAKINGAMOUNT", !string.IsNullOrEmpty(saleLineItem.PartnerData.MakingAmount) ? Convert.ToString(Convert.ToDecimal(saleLineItem.PartnerData.MakingAmount) * -1) : "0"));

                                    //sqlCmd.Parameters.Add(new SqlParameter("@AMOUNT", !string.IsNullOrEmpty(saleLineItem.PartnerData.Amount) ? saleLineItem.PartnerData.Amount : "0"));
                                    sqlCmd.Parameters.Add(new SqlParameter("@AMOUNT", !string.IsNullOrEmpty(saleLineItem.PartnerData.Amount) ? Convert.ToString(Convert.ToDecimal(saleLineItem.PartnerData.Amount) * -1) : "0"));

                                    //sqlCmd.Parameters.Add(new SqlParameter("@TOTALAMOUNT", !string.IsNullOrEmpty(saleLineItem.PartnerData.TotalAmount) ? saleLineItem.PartnerData.TotalAmount : "0"));
                                    sqlCmd.Parameters.Add(new SqlParameter("@TOTALAMOUNT", !string.IsNullOrEmpty(saleLineItem.PartnerData.TotalAmount) ? Convert.ToString(Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) * -1) : "0"));

                                    //sqlCmd.Parameters.Add(new SqlParameter("@TOTALWEIGHT", !string.IsNullOrEmpty(saleLineItem.PartnerData.TotalWeight) ? saleLineItem.PartnerData.TotalWeight : "0"));
                                    sqlCmd.Parameters.Add(new SqlParameter("@TOTALWEIGHT", !string.IsNullOrEmpty(saleLineItem.PartnerData.TotalWeight) ? Convert.ToString(Convert.ToDecimal(saleLineItem.PartnerData.TotalWeight) * -1) : "0"));


                                    //  // sqlCmd.Parameters.Add(new SqlParameter("@LOSSPERCENTAGE", !string.IsNullOrEmpty(saleLineItem.PartnerData.LossPct) ? saleLineItem.PartnerData.LossPct : "0"));

                                    //  sqlCmd.Parameters.Add(new SqlParameter("@LOSSWEIGHT", !string.IsNullOrEmpty(saleLineItem.PartnerData.LossWeight) ? saleLineItem.PartnerData.LossWeight : "0"));
                                    sqlCmd.Parameters.Add(new SqlParameter("@LOSSWEIGHT", !string.IsNullOrEmpty(saleLineItem.PartnerData.LossWeight) ? Convert.ToString(Convert.ToDecimal(saleLineItem.PartnerData.LossWeight) * -1) : "0"));

                                    //  sqlCmd.Parameters.Add(new SqlParameter("@EXPECTEDQUANTITY", !string.IsNullOrEmpty(saleLineItem.PartnerData.ExpectedQuantity) ? saleLineItem.PartnerData.ExpectedQuantity : "0"));
                                    sqlCmd.Parameters.Add(new SqlParameter("@EXPECTEDQUANTITY", !string.IsNullOrEmpty(saleLineItem.PartnerData.ExpectedQuantity) ? Convert.ToString(Convert.ToDecimal(saleLineItem.PartnerData.ExpectedQuantity) * -1) : "0"));

                                    // sqlCmd.Parameters.Add(new SqlParameter("@MAKINGDISCAMT", saleLineItem.PartnerData.MakingTotalDiscount));
                                    sqlCmd.Parameters.Add(new SqlParameter("@MAKINGDISCAMT", !string.IsNullOrEmpty(saleLineItem.PartnerData.MakingTotalDiscount) ? Convert.ToString(Convert.ToDecimal(saleLineItem.PartnerData.MakingTotalDiscount) * -1) : "0"));


                                    sqlCmd.Parameters.Add(new SqlParameter("@STUDDAMOUNT", (studdamount * -1)));


                                    //sqlCmd.Parameters.Add(new SqlParameter("@WastageQty", saleLineItem.PartnerData.WastageQty));
                                    sqlCmd.Parameters.Add(new SqlParameter("@WastageQty", !string.IsNullOrEmpty(saleLineItem.PartnerData.WastageQty) ? Convert.ToString(Convert.ToDecimal(saleLineItem.PartnerData.WastageQty) * -1) : "0"));


                                    //sqlCmd.Parameters.Add(new SqlParameter("@WastageAmount", saleLineItem.PartnerData.WastageAmount));
                                    sqlCmd.Parameters.Add(new SqlParameter("@WastageAmount", !string.IsNullOrEmpty(saleLineItem.PartnerData.WastageAmount) ? Convert.ToString(Convert.ToDecimal(saleLineItem.PartnerData.WastageAmount) * -1) : "0"));

                                }
                                else
                                {

                                    decimal studdamount = 0;
                                    if(!string.IsNullOrEmpty(Convert.ToString(saleLineItem.PartnerData.Ingredients)))
                                    {
                                        DataSet dsIngredients = new DataSet();
                                        StringReader reader = new StringReader(Convert.ToString(saleLineItem.PartnerData.Ingredients));
                                        dsIngredients.ReadXml(reader);
                                        foreach(DataRow drIngredients in dsIngredients.Tables[0].Rows)
                                        {
                                            studdamount += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["CValue"])) ? drIngredients["CValue"] : 0);
                                        }
                                    }

                                    saleLineItem.PartnerData.PROMOCODE = string.Empty;
                                    saleLineItem.PartnerData.SALESCHANNELTYPE = string.Empty;
                                    saleLineItem.PartnerData.SellingCostPrice = 0;
                                    saleLineItem.PartnerData.ItemIdParent = string.Empty;
                                    saleLineItem.PartnerData.UpdatedCostPrice = 0;
                                    saleLineItem.PartnerData.GroupCostPrice = 0;
                                    saleLineItem.PartnerData.FLAT = 0;
                                    // saleLineItem.PartnerData.SpecialDiscInfo = "";

                                    // sqlCmd.Parameters.Add(new SqlParameter("@PIECES", !string.IsNullOrEmpty(saleLineItem.PartnerData.Pieces) ? saleLineItem.PartnerData.Pieces : "0"));
                                    sqlCmd.Parameters.Add(new SqlParameter("@PIECES", !string.IsNullOrEmpty(saleLineItem.PartnerData.Pieces) ? Convert.ToString(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Pieces))) : "0"));

                                    //sqlCmd.Parameters.Add(new SqlParameter("@QUANTITY", !string.IsNullOrEmpty(saleLineItem.PartnerData.Quantity) ? saleLineItem.PartnerData.Quantity : "0"));
                                    sqlCmd.Parameters.Add(new SqlParameter("@QUANTITY", !string.IsNullOrEmpty(saleLineItem.PartnerData.Quantity) ? Convert.ToString(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Quantity))) : "0"));


                                    // sqlCmd.Parameters.Add(new SqlParameter("@RATE", !string.IsNullOrEmpty(saleLineItem.PartnerData.Rate) ? saleLineItem.PartnerData.Rate : "0"));
                                    sqlCmd.Parameters.Add(new SqlParameter("@RATE", !string.IsNullOrEmpty(saleLineItem.PartnerData.Rate) ? Convert.ToString(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Rate))) : "0"));

                                    //sqlCmd.Parameters.Add(new SqlParameter("@MAKINGRATE", !string.IsNullOrEmpty(saleLineItem.PartnerData.MakingRate) ? saleLineItem.PartnerData.MakingRate : "0"));
                                    sqlCmd.Parameters.Add(new SqlParameter("@MAKINGRATE", !string.IsNullOrEmpty(saleLineItem.PartnerData.MakingRate) ? Convert.ToString(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.MakingRate))) : "0"));


                                    //sqlCmd.Parameters.Add(new SqlParameter("@MAKINGAMOUNT", !string.IsNullOrEmpty(saleLineItem.PartnerData.MakingAmount) ? saleLineItem.PartnerData.MakingAmount : "0"));
                                    sqlCmd.Parameters.Add(new SqlParameter("@MAKINGAMOUNT", !string.IsNullOrEmpty(saleLineItem.PartnerData.MakingAmount) ? Convert.ToString(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.MakingAmount))) : "0"));


                                    //sqlCmd.Parameters.Add(new SqlParameter("@AMOUNT", !string.IsNullOrEmpty(saleLineItem.PartnerData.Amount) ? saleLineItem.PartnerData.Amount : "0"));
                                    sqlCmd.Parameters.Add(new SqlParameter("@AMOUNT", !string.IsNullOrEmpty(saleLineItem.PartnerData.Amount) ? Convert.ToString(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Amount))) : "0"));


                                    //sqlCmd.Parameters.Add(new SqlParameter("@TOTALAMOUNT", !string.IsNullOrEmpty(saleLineItem.PartnerData.TotalAmount) ? saleLineItem.PartnerData.TotalAmount : "0"));
                                    sqlCmd.Parameters.Add(new SqlParameter("@TOTALAMOUNT", !string.IsNullOrEmpty(saleLineItem.PartnerData.TotalAmount) ? Convert.ToString(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount))) : "0"));


                                    //sqlCmd.Parameters.Add(new SqlParameter("@TOTALWEIGHT", !string.IsNullOrEmpty(saleLineItem.PartnerData.TotalWeight) ? saleLineItem.PartnerData.TotalWeight : "0"));
                                    sqlCmd.Parameters.Add(new SqlParameter("@TOTALWEIGHT", !string.IsNullOrEmpty(saleLineItem.PartnerData.TotalWeight) ? Convert.ToString(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.TotalWeight))) : "0"));


                                    // sqlCmd.Parameters.Add(new SqlParameter("@LOSSWEIGHT", !string.IsNullOrEmpty(saleLineItem.PartnerData.LossWeight) ? saleLineItem.PartnerData.LossWeight : "0"));
                                    sqlCmd.Parameters.Add(new SqlParameter("@LOSSWEIGHT", !string.IsNullOrEmpty(saleLineItem.PartnerData.LossWeight) ? Convert.ToString(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.LossWeight))) : "0"));


                                    //sqlCmd.Parameters.Add(new SqlParameter("@EXPECTEDQUANTITY", !string.IsNullOrEmpty(saleLineItem.PartnerData.ExpectedQuantity) ? saleLineItem.PartnerData.ExpectedQuantity : "0"));
                                    sqlCmd.Parameters.Add(new SqlParameter("@EXPECTEDQUANTITY", !string.IsNullOrEmpty(saleLineItem.PartnerData.ExpectedQuantity) ? Convert.ToString(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.ExpectedQuantity))) : "0"));


                                    // sqlCmd.Parameters.Add(new SqlParameter("@MAKINGDISCAMT", saleLineItem.PartnerData.MakingTotalDiscount));
                                    sqlCmd.Parameters.Add(new SqlParameter("@MAKINGDISCAMT", !string.IsNullOrEmpty(saleLineItem.PartnerData.MakingTotalDiscount) ? Convert.ToString(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.MakingTotalDiscount))) : "0"));


                                    //sqlCmd.Parameters.Add(new SqlParameter("@STUDDAMOUNT", (studdamount)));
                                    sqlCmd.Parameters.Add(new SqlParameter("@STUDDAMOUNT", (studdamount)));

                                    // sqlCmd.Parameters.Add(new SqlParameter("@WastageQty", saleLineItem.PartnerData.WastageQty));
                                    sqlCmd.Parameters.Add(new SqlParameter("@WastageQty", !string.IsNullOrEmpty(saleLineItem.PartnerData.WastageQty) ? Convert.ToString(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.WastageQty))) : "0"));


                                    //sqlCmd.Parameters.Add(new SqlParameter("@WastageAmount", saleLineItem.PartnerData.WastageAmount));
                                    sqlCmd.Parameters.Add(new SqlParameter("@WastageAmount", !string.IsNullOrEmpty(saleLineItem.PartnerData.WastageAmount) ? Convert.ToString(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.WastageAmount))) : "0"));

                                }
                                #endregion

                                sqlCmd.Parameters.Add(new SqlParameter("@DATAAREAID", ApplicationSettings.Database.DATAAREAID));
                                sqlCmd.Parameters.Add(new SqlParameter("@STOREID", posTransaction.StoreId));
                                sqlCmd.Parameters.Add(new SqlParameter("@TERMINALID", posTransaction.TerminalId));
                                sqlCmd.Parameters.Add(new SqlParameter("@TRANSACTIONID", posTransaction.TransactionId));
                                sqlCmd.Parameters.Add(new SqlParameter("@LINENUM", saleLineItem.LineId));
                                sqlCmd.Parameters.Add(new SqlParameter("@RATETYPE", !string.IsNullOrEmpty(saleLineItem.PartnerData.RateType) ? saleLineItem.PartnerData.RateType : "0"));
                                sqlCmd.Parameters.Add(new SqlParameter("@MAKINGTYPE", !string.IsNullOrEmpty(saleLineItem.PartnerData.MakingType) ? saleLineItem.PartnerData.MakingType : "0"));
                                sqlCmd.Parameters.Add(new SqlParameter("@MAKINGDISCOUNT", !string.IsNullOrEmpty(saleLineItem.PartnerData.MakingDisc) ? saleLineItem.PartnerData.MakingDisc : "0"));
                                sqlCmd.Parameters.Add(new SqlParameter("@LOSSPERCENTAGE", !string.IsNullOrEmpty(saleLineItem.PartnerData.LossPct) ? saleLineItem.PartnerData.LossPct : "0"));
                                sqlCmd.Parameters.Add(new SqlParameter("@TRANSACTIONTYPE", !string.IsNullOrEmpty(saleLineItem.PartnerData.TransactionType) ? saleLineItem.PartnerData.TransactionType : "0"));
                                sqlCmd.Parameters.Add(new SqlParameter("@OWN", Convert.ToBoolean(saleLineItem.PartnerData.OChecked) ? "1" : "0"));
                                sqlCmd.Parameters.Add(new SqlParameter("@SKUDETAILS", !string.IsNullOrEmpty(saleLineItem.PartnerData.Ingredients) ? saleLineItem.PartnerData.Ingredients : "0"));
                                sqlCmd.Parameters.Add(new SqlParameter("@ORDERNUM", !string.IsNullOrEmpty(saleLineItem.PartnerData.OrderNum) ? saleLineItem.PartnerData.OrderNum : string.Empty));
                                sqlCmd.Parameters.Add(new SqlParameter("@ORDERLINENUM", !string.IsNullOrEmpty(saleLineItem.PartnerData.OrderLineNum) ? saleLineItem.PartnerData.OrderLineNum : "0"));
                                sqlCmd.Parameters.Add(new SqlParameter("@CUSTNO", !string.IsNullOrEmpty(saleLineItem.PartnerData.CustNo) ? saleLineItem.PartnerData.CustNo : string.Empty));
                                sqlCmd.Parameters.Add(new SqlParameter("@isVoid", Convert.ToBoolean(saleLineItem.Voided)));
                                sqlCmd.Parameters.Add(new SqlParameter("@INVENTITEMID", Convert.ToString(saleLineItem.ItemId)));
                                sqlCmd.Parameters.Add(new SqlParameter("@ITEMAMOUNT", Convert.ToString(saleLineItem.NetAmount)));
                                sqlCmd.Parameters.Add(new SqlParameter("@RETAILSTAFFID", posTransaction.OperatorId));
                                sqlCmd.Parameters.Add(new SqlParameter("@ISINGREDIENT", !string.IsNullOrEmpty(saleLineItem.PartnerData.Ingredients) ? 1 : 0));

                                /* // Making Discount Type -- Blocked on 30.04.2013
                                decimal makingdiscamt = 0;
                                if (!string.IsNullOrEmpty(saleLineItem.PartnerData.MakingDisc))
                                {
                                    makingdiscamt = (Convert.ToDecimal(saleLineItem.PartnerData.MakingDisc) / 100) * Convert.ToDecimal(!string.IsNullOrEmpty(saleLineItem.PartnerData.MakingAmount) ? saleLineItem.PartnerData.MakingAmount : 0);

                                }

                                sqlCmd.Parameters.Add(new SqlParameter("@MAKINGDISCAMT", makingdiscamt));
                                */
                                //    sqlCmd.Parameters.Add(new SqlParameter("@MAKINGDISCAMT", saleLineItem.PartnerData.MakingTotalDiscount)); // Making Discount Type

                                //   sqlCmd.Parameters.Add(new SqlParameter("@STUDDAMOUNT", studdamount));


                                //Advance Adjustment ID
                                sqlCmd.Parameters.Add(new SqlParameter("@ADVANCEADJUSTMENTID", string.Empty));
                                sqlCmd.Parameters.Add(new SqlParameter("@SAMPLERETURN", saleLineItem.PartnerData.SampleReturn));

                                // Added for Wastage
                                sqlCmd.Parameters.Add(new SqlParameter("@WastageType", saleLineItem.PartnerData.WastageType));
                                //   sqlCmd.Parameters.Add(new SqlParameter("@WastageQty", saleLineItem.PartnerData.WastageQty));
                                //   sqlCmd.Parameters.Add(new SqlParameter("@WastageAmount", saleLineItem.PartnerData.WastageAmount));
                                sqlCmd.Parameters.Add(new SqlParameter("@WastagePercentage", saleLineItem.PartnerData.WastagePercentage));
                                sqlCmd.Parameters.Add(new SqlParameter("@WastageRate", saleLineItem.PartnerData.WastageRate));
                                //
                                // Making Discount Type
                                sqlCmd.Parameters.Add(new SqlParameter("@MAKINGDISCTYPE", saleLineItem.PartnerData.MakingDiscountType));
                                //
                                sqlCmd.Parameters.Add(new SqlParameter("@CONFIGID", saleLineItem.PartnerData.ConfigId)); //configid
                                sqlCmd.Parameters.Add(new SqlParameter("@ADVANCEADJUSTMENTSTOREID", string.Empty));
                                sqlCmd.Parameters.Add(new SqlParameter("@ADVANCEADJUSTMENTTERMINALID", string.Empty));

                                sqlCmd.Parameters.Add(new SqlParameter("@PURITY", saleLineItem.PartnerData.Purity)); // PURITY
                                sqlCmd.Parameters.Add(new SqlParameter("@OGGROSSUNIT", saleLineItem.PartnerData.GROSSUNIT));
                                sqlCmd.Parameters.Add(new SqlParameter("@OGDMDPCS", saleLineItem.PartnerData.DMDPCS));
                                sqlCmd.Parameters.Add(new SqlParameter("@OGDMDWT", saleLineItem.PartnerData.DMDWT));
                                sqlCmd.Parameters.Add(new SqlParameter("@OGDMDUNIT", saleLineItem.PartnerData.DMDUNIT));
                                sqlCmd.Parameters.Add(new SqlParameter("@OGDMDAMT", saleLineItem.PartnerData.DMDAMOUNT));
                                sqlCmd.Parameters.Add(new SqlParameter("@OGSTONEPCS", saleLineItem.PartnerData.STONEPCS));
                                sqlCmd.Parameters.Add(new SqlParameter("@OGSTONEWT", saleLineItem.PartnerData.STONEWT));
                                sqlCmd.Parameters.Add(new SqlParameter("@OGSTONEUNIT", saleLineItem.PartnerData.STONEUNIT));
                                sqlCmd.Parameters.Add(new SqlParameter("@OGSTONEAMT", saleLineItem.PartnerData.STONEAMOUNT));
                                sqlCmd.Parameters.Add(new SqlParameter("@OGNETWT", saleLineItem.PartnerData.NETWT));
                                sqlCmd.Parameters.Add(new SqlParameter("@OGNETRATE", saleLineItem.PartnerData.NETRATE));
                                sqlCmd.Parameters.Add(new SqlParameter("@OGNETUNIT", saleLineItem.PartnerData.NETUNIT));
                                sqlCmd.Parameters.Add(new SqlParameter("@OGNETAMT", saleLineItem.PartnerData.NETAMOUNT));
                                sqlCmd.Parameters.Add(new SqlParameter("@REFINVOICENO", saleLineItem.PartnerData.OGREFINVOICENO));
                                sqlCmd.Parameters.Add(new SqlParameter("@SALESCHANNELTYPE", saleLineItem.PartnerData.SALESCHANNELTYPE));

                                sqlCmd.Parameters.Add(new SqlParameter("@SELLINGPRICE", saleLineItem.PartnerData.SellingCostPrice));
                                sqlCmd.Parameters.Add(new SqlParameter("@ITEMIDPARENT", saleLineItem.PartnerData.ItemIdParent));
                                sqlCmd.Parameters.Add(new SqlParameter("@UPDATEDCOSTPRICE", saleLineItem.PartnerData.UpdatedCostPrice));
                                sqlCmd.Parameters.Add(new SqlParameter("@GROUPCOSTPRICE", saleLineItem.PartnerData.GroupCostPrice));
                                sqlCmd.Parameters.Add(new SqlParameter("@PROMOTIONCODE", saleLineItem.PartnerData.PROMOCODE)); // added on 20/11/2014
                                sqlCmd.Parameters.Add(new SqlParameter("@FLAT", saleLineItem.PartnerData.FLAT)); // added on 28/12/2015
                                sqlCmd.Parameters.Add(new SqlParameter("@SPECIALDISCINFO", saleLineItem.PartnerData.SpecialDiscInfo)); // added on 28/12/2015
                                sqlCmd.Parameters.Add(new SqlParameter("@REMARKS", saleLineItem.PartnerData.REMARKS));

                                sqlCmd.ExecuteNonQuery();
                            }

                        }
                        else
                        {
                            if(retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                            {
                                StringBuilder sbAdjustment = new StringBuilder();
                                sbAdjustment.Append(" INSERT INTO RETAIL_CUSTOMCALCULATIONS_TABLE VALUES ");
                                sbAdjustment.Append(" (@DATAAREAID,@STOREID,@TERMINALID,@TRANSACTIONID,@LINENUM,@PIECES,@QUANTITY,");
                                sbAdjustment.Append(" @RATETYPE,@MAKINGRATE,@MAKINGTYPE,@AMOUNT,@MAKINGDISCOUNT,@MAKINGAMOUNT,@TOTALAMOUNT, ");
                                sbAdjustment.Append(" @TOTALWEIGHT,@LOSSPERCENTAGE,@LOSSWEIGHT,@EXPECTEDQUANTITY,@TRANSACTIONTYPE,@OWN,@SKUDETAILS, ");
                                sbAdjustment.Append(" @ORDERNUM,@ORDERLINENUM,@CUSTNO,@RETAILSTAFFID,@ISINGREDIENT,@MAKINGDISCAMT, ");
                                sbAdjustment.Append(" @STUDDAMOUNT,@RATE,@ADVANCEADJUSTMENTID,@SAMPLERETURN,@WastageType,@WastageQty,");
                                sbAdjustment.Append(" @WastageAmount,@WastagePercentage,@WastageRate,@MAKINGDISCTYPE,@CONFIGID,");
                                sbAdjustment.Append(" @ADVANCEADJUSTMENTSTOREID,@ADVANCEADJUSTMENTTERMINALID,@PURITY,");
                                sbAdjustment.Append(" @OGGROSSUNIT,@OGDMDPCS,@OGDMDWT,@OGDMDUNIT,@OGDMDAMT,@OGSTONEPCS,@OGSTONEWT,");
                                sbAdjustment.Append(" @OGSTONEUNIT,@OGSTONEAMT,@OGNETWT,@OGNETRATE,@OGNETUNIT,@OGNETAMT,@REFINVOICENO,@SALESCHANNELTYPE,");
                                sbAdjustment.Append(" @SELLINGPRICE,@ITEMIDPARENT,@UPDATEDCOSTPRICE,@GROUPCOSTPRICE,@PROMOTIONCODE,@FLAT,@SPECIALDISCINFO,@REMARKS)");


                                if(!retailTransaction.PartnerData.IsGSSMaturity)
                                {
                                    sbAdjustment.Append(" UPDATE RETAILTRANSACTIONPAYMENTTRANS SET ");
                                    sbAdjustment.Append(" isAdjusted = @ADJUSTED WHERE TRANSACTIONID =@TRANSID AND STORE = @RETAILSTOREID AND TERMINAL = @RETAILTERMINALID; ");
                                    sbAdjustment.Append(" UPDATE RETAILADJUSTMENTTABLE SET ");
                                    sbAdjustment.Append(" isAdjusted = @ADJUSTED WHERE TRANSACTIONID = @TRANSID AND RETAILSTOREID = @RETAILSTOREID AND RETAILTERMINALID = @RETAILTERMINALID");
                                }

                                using(SqlCommand sqlCmd = new SqlCommand(sbAdjustment.ToString(), sqlTransaction.Connection, sqlTransaction))
                                {
                                    if(!retailTransaction.PartnerData.IsGSSMaturity)
                                    {
                                        sqlCmd.Parameters.Add(new SqlParameter("@TRANSID", Convert.ToString(saleLineItem.PartnerData.ServiceItemCashAdjustmentTransactionID)));

                                        sqlCmd.Parameters.Add(new SqlParameter("@RETAILSTOREID", Convert.ToString(saleLineItem.PartnerData.ServiceItemCashAdjustmentStoreId)));
                                        sqlCmd.Parameters.Add(new SqlParameter("@RETAILTERMINALID", Convert.ToString(saleLineItem.PartnerData.ServiceItemCashAdjustmentTerminalId)));

                                        if(saleLineItem.Voided)
                                            sqlCmd.Parameters.Add(new SqlParameter("@ADJUSTED", Convert.ToInt16(0)));
                                        else
                                            sqlCmd.Parameters.Add(new SqlParameter("@ADJUSTED", Convert.ToInt16(1)));
                                    }

                                    sqlCmd.Parameters.Add(new SqlParameter("@DATAAREAID", ApplicationSettings.Database.DATAAREAID));
                                    sqlCmd.Parameters.Add(new SqlParameter("@STOREID", posTransaction.StoreId));
                                    sqlCmd.Parameters.Add(new SqlParameter("@TERMINALID", posTransaction.TerminalId));
                                    sqlCmd.Parameters.Add(new SqlParameter("@TRANSACTIONID", posTransaction.TransactionId));
                                    sqlCmd.Parameters.Add(new SqlParameter("@LINENUM", saleLineItem.LineId));
                                    sqlCmd.Parameters.Add(new SqlParameter("@PIECES", Convert.ToDecimal("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@QUANTITY", Convert.ToDecimal("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@RATE", Convert.ToDecimal("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@RATETYPE", "0"));
                                    sqlCmd.Parameters.Add(new SqlParameter("@MAKINGRATE", Convert.ToDecimal("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@MAKINGTYPE", "0"));
                                    sqlCmd.Parameters.Add(new SqlParameter("@AMOUNT", Convert.ToDecimal("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@MAKINGDISCOUNT", Convert.ToDecimal("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@MAKINGAMOUNT", Convert.ToDecimal("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@TOTALAMOUNT", Convert.ToDecimal("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@TOTALWEIGHT", Convert.ToDecimal("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@LOSSPERCENTAGE", Convert.ToDecimal("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@LOSSWEIGHT", Convert.ToDecimal("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@EXPECTEDQUANTITY", Convert.ToDecimal("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@TRANSACTIONTYPE", (int)Enums.EnumClass.TransactionType.Adjustment));
                                    sqlCmd.Parameters.Add(new SqlParameter("@OWN", Convert.ToInt16("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@SKUDETAILS", string.Empty));
                                    sqlCmd.Parameters.Add(new SqlParameter("@ORDERNUM", string.Empty));
                                    sqlCmd.Parameters.Add(new SqlParameter("@ORDERLINENUM", "0"));
                                    sqlCmd.Parameters.Add(new SqlParameter("@ITEMAMOUNT", Convert.ToString(saleLineItem.NetAmount)));
                                    sqlCmd.Parameters.Add(new SqlParameter("@RETAILSTAFFID", posTransaction.OperatorId));
                                    sqlCmd.Parameters.Add(new SqlParameter("@ISINGREDIENT", iIngredient));
                                    sqlCmd.Parameters.Add(new SqlParameter("@CUSTNO", string.Empty));
                                    sqlCmd.Parameters.Add(new SqlParameter("@MAKINGDISCAMT", Convert.ToDecimal("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@STUDDAMOUNT", Convert.ToDecimal("0")));

                                    if(retailTransaction.PartnerData.IsGSSMaturity)
                                    {
                                        sqlCmd.Parameters.Add(new SqlParameter("@ADVANCEADJUSTMENTID", string.Empty));
                                        sqlCmd.Parameters.Add(new SqlParameter("@ADVANCEADJUSTMENTSTOREID", string.Empty));
                                        sqlCmd.Parameters.Add(new SqlParameter("@ADVANCEADJUSTMENTTERMINALID", string.Empty));
                                    }
                                    else
                                    {
                                        sqlCmd.Parameters.Add(new SqlParameter("@ADVANCEADJUSTMENTID", !string.IsNullOrEmpty(saleLineItem.PartnerData.ServiceItemCashAdjustmentTransactionID) ? saleLineItem.PartnerData.ServiceItemCashAdjustmentTransactionID : string.Empty));
                                        sqlCmd.Parameters.Add(new SqlParameter("@ADVANCEADJUSTMENTSTOREID", !string.IsNullOrEmpty(saleLineItem.PartnerData.ServiceItemCashAdjustmentStoreId) ? saleLineItem.PartnerData.ServiceItemCashAdjustmentStoreId : string.Empty));
                                        sqlCmd.Parameters.Add(new SqlParameter("@ADVANCEADJUSTMENTTERMINALID", !string.IsNullOrEmpty(saleLineItem.PartnerData.ServiceItemCashAdjustmentTerminalId) ? saleLineItem.PartnerData.ServiceItemCashAdjustmentTerminalId : string.Empty));
                                    }
                                    sqlCmd.Parameters.Add(new SqlParameter("@SAMPLERETURN", Convert.ToInt16("0")));

                                    sqlCmd.Parameters.Add(new SqlParameter("@WastageType", Convert.ToInt16("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@WastageQty", Convert.ToDecimal("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@WastageAmount", Convert.ToDecimal("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@WastagePercentage", Convert.ToDecimal("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@WastageRate", Convert.ToDecimal("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@MAKINGDISCTYPE", Convert.ToInt16("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@CONFIGID", string.Empty));
                                    sqlCmd.Parameters.Add(new SqlParameter("@PURITY", Convert.ToDecimal("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@OGGROSSUNIT", string.Empty));
                                    sqlCmd.Parameters.Add(new SqlParameter("@OGDMDPCS", Convert.ToDecimal("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@OGDMDWT", Convert.ToDecimal("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@OGDMDUNIT", string.Empty));
                                    sqlCmd.Parameters.Add(new SqlParameter("@OGDMDAMT", Convert.ToDecimal("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@OGSTONEPCS", Convert.ToDecimal("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@OGSTONEWT", Convert.ToDecimal("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@OGSTONEUNIT", Convert.ToDecimal("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@OGSTONEAMT", Convert.ToDecimal("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@OGNETWT", Convert.ToDecimal("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@OGNETRATE", Convert.ToDecimal("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@OGNETUNIT", Convert.ToDecimal("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@OGNETAMT", Convert.ToDecimal("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@REFINVOICENO", string.Empty));
                                    sqlCmd.Parameters.Add(new SqlParameter("@SALESCHANNELTYPE", string.Empty));

                                    sqlCmd.Parameters.Add(new SqlParameter("@SELLINGPRICE", Convert.ToDecimal("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@ITEMIDPARENT", string.Empty));
                                    sqlCmd.Parameters.Add(new SqlParameter("@UPDATEDCOSTPRICE", Convert.ToDecimal("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@GROUPCOSTPRICE", Convert.ToDecimal("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@PROMOTIONCODE", string.Empty));
                                    sqlCmd.Parameters.Add(new SqlParameter("@FLAT", Convert.ToInt16("0")));
                                    sqlCmd.Parameters.Add(new SqlParameter("@SPECIALDISCINFO", string.Empty));
                                    sqlCmd.Parameters.Add(new SqlParameter("@REMARKS", string.Empty));

                                    sqlCmd.ExecuteNonQuery();
                                }
                            }
                        }

                    }
                    #region
                    if(retailTransaction.PartnerData.IsGSSMaturity)
                    {
                        if(retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                        {
                            string scmdGssMaturity = "SET DATEFORMAT DMY;  UPDATE GSSACCOUNTOPENINGPOSTED" +
                                                     " SET GSSADJUSTED = 1,GSSADJUSTEDDATE = '" + DateTime.Now + "'" +
                                                     " WHERE GSSACCOUNTNO = '" + Convert.ToString(retailTransaction.PartnerData.GSSMaturityNo) + "';";

                            scmdGssMaturity = scmdGssMaturity + "  INSERT INTO RETAILGSSMATURITYADJUSTMENT (TRANSACTIONID,GSSACCOUNTNO,CUSTACCOUNT,GSSTOTALQTY,GSSTOTALAMOUNT," +
                                                " GSSAVGRATE,GSSROYALITYAMOUNT,STOREID,TERMINALID,STAFFID,DATAAREAID) VALUES (@TRANSACTIONID,@GSSACCOUNTNO,@CUSTACCOUNT,@GSSTOTALQTY,@GSSTOTALAMOUNT," +
                                                " @GSSAVGRATE,@GSSROYALITYAMOUNT,@STOREID,@TERMINALID,@STAFFID,@DATAAREAID);";


                            using(SqlCommand sqlCmd = new SqlCommand(scmdGssMaturity, sqlTransaction.Connection, sqlTransaction))
                            {
                                sqlCmd.Parameters.Add(new SqlParameter("@TRANSACTIONID", posTransaction.TransactionId));
                                sqlCmd.Parameters.Add(new SqlParameter("@GSSACCOUNTNO", retailTransaction.PartnerData.GSSMaturityNo));
                                sqlCmd.Parameters.Add(new SqlParameter("@CUSTACCOUNT", retailTransaction.Customer.CustomerId));

                                sqlCmd.Parameters.Add(new SqlParameter("@GSSTOTALQTY", retailTransaction.PartnerData.GSSTotQty));
                                sqlCmd.Parameters.Add(new SqlParameter("@GSSTOTALAMOUNT", retailTransaction.PartnerData.GSSTotAmt));
                                sqlCmd.Parameters.Add(new SqlParameter("@GSSAVGRATE", retailTransaction.PartnerData.GSSAvgRate));
                                sqlCmd.Parameters.Add(new SqlParameter("@GSSROYALITYAMOUNT", retailTransaction.PartnerData.GSSRoyaltyAmt));

                                sqlCmd.Parameters.Add(new SqlParameter("@STOREID", posTransaction.StoreId));
                                sqlCmd.Parameters.Add(new SqlParameter("@TERMINALID", posTransaction.TerminalId));
                                sqlCmd.Parameters.Add(new SqlParameter("@STAFFID", posTransaction.OperatorId));
                                sqlCmd.Parameters.Add(new SqlParameter("@DATAAREAID", ApplicationSettings.Database.DATAAREAID));

                                sqlCmd.ExecuteNonQuery();
                            }

                        }
                    }


                    #endregion

                    if(!string.IsNullOrEmpty(Convert.ToString(retailTransaction.PartnerData.LCCustomerName)))
                    {
                        if(retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                        {
                            string sLCName = Convert.ToString(retailTransaction.PartnerData.LCCustomerName);
                            string sLCAddress = Convert.ToString(retailTransaction.PartnerData.LCCustomerAddress);
                            string sLCContactNo = Convert.ToString(retailTransaction.PartnerData.LCCustomerContactNo);

                            string sLCQry = " UPDATE RETAILTRANSACTIONTABLE SET LocalCustomerName = '" + sLCName + "', LocalCustomerAddress = '" + sLCAddress + "'," +
                                            " LocalCustomerContactNo = '" + sLCContactNo + "' WHERE TRANSACTIONID='" + retailTransaction.TransactionId + "' AND TERMINAL = '" + ApplicationSettings.Terminal.TerminalId + "'";

                            using(SqlCommand cmdLC = new SqlCommand(sLCQry, sqlTransaction.Connection, sqlTransaction))
                            {
                                cmdLC.ExecuteNonQuery();
                            }
                        }
                    }
                    if(!string.IsNullOrEmpty(Convert.ToString(retailTransaction.PartnerData.Remarks)))
                    {
                        if(retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                        {
                            string sRemarks = Convert.ToString(retailTransaction.PartnerData.Remarks);

                            string sLCQry = " UPDATE RETAILTRANSACTIONTABLE SET Remarks = '" + sRemarks + "'"+
                                " WHERE TRANSACTIONID='" + retailTransaction.TransactionId + "'"+
                                " AND TERMINAL = '" + ApplicationSettings.Terminal.TerminalId + "'";

                            using(SqlCommand cmdLC = new SqlCommand(sLCQry, sqlTransaction.Connection, sqlTransaction))
                            {
                                cmdLC.ExecuteNonQuery();
                            }
                        }
                    }

                    if(!string.IsNullOrEmpty(Convert.ToString(retailTransaction.PartnerData.TouristNumber)))
                    {
                        if(retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                        {
                            string sTouristNumber = Convert.ToString(retailTransaction.PartnerData.TouristNumber);

                            string sLCQry = " UPDATE RETAILTRANSACTIONTABLE SET TouristVATNumber = '" + sTouristNumber + "'"+
                                " WHERE TRANSACTIONID='" + retailTransaction.TransactionId + "'"+
                                " AND TERMINAL = '" + ApplicationSettings.Terminal.TerminalId + "'";

                            using(SqlCommand cmdLC = new SqlCommand(sLCQry, sqlTransaction.Connection, sqlTransaction))
                            {
                                cmdLC.ExecuteNonQuery();
                            }
                        }
                    }

                    if(!string.IsNullOrEmpty(sSalesInfoCode))
                    {
                        string sLCQry = " UPDATE RETAILTRANSACTIONTABLE SET PANNO = '" + sSalesInfoCode + "'" +
                                        " WHERE TRANSACTIONID='" + retailTransaction.TransactionId + "'" +
                                        " AND TERMINAL = '" + ApplicationSettings.Terminal.TerminalId + "'";

                        using(SqlCommand cmdLC = new SqlCommand(sLCQry, sqlTransaction.Connection, sqlTransaction))
                        {
                            cmdLC.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch(RuntimeBinderException)
            {

            }
            #endregion


            #region
            try
            {

                RetailTransaction retailTransaction = posTransaction as RetailTransaction;

                if(Convert.ToString(posTransaction.GetType().Name).ToUpper().Trim() == "CUSTOMERPAYMENTTRANSACTION")
                {
                    #region custtrans
                    //
                    string custOrder = string.Empty;
                    string repairid = string.Empty;
                    bool IsRepair = false;
                    if(Convert.ToBoolean(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.IsRepair))
                    {
                        repairid = ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OrderNo;
                        IsRepair = true;
                    }
                    else
                    {
                        custOrder = ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OrderNo;
                    }
                    //
                    #endregion


                    #region // SKU allow
                    if(((LSRetailPosis.Transaction.PosTransaction)(posTransaction)).EntryStatus != PosTransaction.TransactionStatus.Voided)
                    {
                        string sBookedCustOrdrNo = Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OrderNo);
                        if(sBookedCustOrdrNo != string.Empty)
                        {
                            DataTable dtBookedSKU = new DataTable();
                            dtBookedSKU = GetBookedInfo(sBookedCustOrdrNo, sqlTransaction);
                            if(dtBookedSKU != null && dtBookedSKU.Rows.Count > 0)
                            {
                                foreach(DataRow dr in dtBookedSKU.Rows)
                                {
                                    SaveBookedSKU(posTransaction, sBookedCustOrdrNo, Convert.ToString(dr["ITEMID"]), sqlTransaction);
                                }
                            }
                        }
                    }
                    else
                    {
                        // handled below
                    }

                    #endregion

                    #region GSS EMI

                    if(Convert.ToBoolean(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationDone) == true)
                    {
                        // if (retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                        if(((LSRetailPosis.Transaction.PosTransaction)(posTransaction)).EntryStatus != PosTransaction.TransactionStatus.Voided)
                        {
                            if(this.Application.TransactionServices.CheckConnection())
                            {

                                bool bResult = false;
                                string sGSSNo = Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GSSNum);
                                int iNoOfMonth = Convert.ToInt32(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.NumMonths);
                                ReadOnlyCollection<object> containerArray;
                                //containerArray = this.Application.TransactionServices.InvokeExtension("UpdateGSSMaturityAdjustment",
                                //                                                                    sGSSNo, true, DateTime.Now);

                                containerArray = this.Application.TransactionServices.InvokeExtension("GSSEMICreate",
                                                                                                      posTransaction.TransactionId,
                                                                                                      ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).Customer.CustomerId,
                                                                                                      sGSSNo,
                                                                                                      iNoOfMonth,
                                    //((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.NumMonths,
                                    // Convert.ToInt32(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.NumMonths),
                                                                                                      Convert.ToDecimal(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).Amount),
                                                                                                      DateTime.Now, posTransaction.StoreId, posTransaction.TerminalId, posTransaction.OperatorId);

                                bResult = Convert.ToBoolean(containerArray[1]);
                                if(!bResult)
                                    return;
                            }
                        }
                    }
                    #endregion

                    int goldfixing = 0;
                    int GSS = 0;
                    if(Convert.ToBoolean(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GoldFixing))
                        goldfixing = 1;
                    if(Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationType) == "GSS")
                        GSS = 1;
                    string commandString = " UPDATE RETAILTRANSACTIONTABLE SET CUSTOMERORDER='" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OrderNo + "'" +
                        " , GOLDFIXING=" + goldfixing + ",ISGSS=" + GSS + " WHERE TRANSACTIONID='" + posTransaction.TransactionId + "' AND TERMINAL = '" + ApplicationSettings.Terminal.TerminalId + "';" +
                        // " UPDATE RETAILTEMPTABLE SET TRANSID=NULL,CUSTID=NULL,CUSTORDER=NULL,GOLDFIXING=NULL,ITEMRETURN='False',MINIMUMDEPOSITFORCUSTORDER=NULL WHERE ID IN (1,2); ";
                                          " UPDATE RETAILTEMPTABLE SET TRANSID=NULL,CUSTID=NULL,CUSTORDER=NULL,GOLDFIXING=NULL,ITEMRETURN='False',MINIMUMDEPOSITFORCUSTORDER=NULL WHERE ID IN (1,2) AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE

                    if(((LSRetailPosis.Transaction.PosTransaction)(posTransaction)).EntryStatus == PosTransaction.TransactionStatus.Voided)
                    {
                        string Items = string.Empty;
                        Items = ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ItemIds;
                        if(string.IsNullOrEmpty(Items))
                            Items = "''";
                        // commandString = commandString + " DELETE FROM RETAILCUSTOMERDEPOSITSKUDETAILS WHERE TRANSID='" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).TransactionId + "';" +
                        commandString = commandString + " DELETE FROM RETAILCUSTOMERDEPOSITSKUDETAILS WHERE TRANSID='" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).TransactionId + "' AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "';" + // -- CHANGED ON 24.01.2014
                            // " UPDATE SKUTable_Posted SET isLocked='False' WHERE SkuNumber IN (" + Items + ")";
                            //     " UPDATE SKUTableTrans SET isLocked='False' WHERE SkuNumber IN (" + Items + ")"; //SKU Table New 
                            //      " UPDATE SKUTableTrans SET isLocked='False',isAvailable = 'TRUE' WHERE SkuNumber IN (" + Items + ")"; //SKU Table New  // -- CHANGED ON 24.01.2014
                            " UPDATE SKUTableTrans SET isLocked='False',isAvailable = 'TRUE' WHERE SkuNumber" +
                            " IN (select b.ITEMID from CUSTORDER_HEADER a " +
                            " left join CUSTORDER_DETAILS b on a.ORDERNUM =b.ORDERNUM " +
                            " where a.IsConfirmed=0 and b.IsBookedSKU=1" +
                            " and a.ORDERNUM='" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OrderNo + "') ";

                    }
                    else
                    {
                        string Items = string.Empty;
                        Items = ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ItemIds;
                        if(!string.IsNullOrEmpty(Items))
                            //commandString = commandString + " UPDATE SKUTable_Posted SET isLocked='False',isAvailable='False' WHERE SkuNumber IN (" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ItemIds + "); ";
                            commandString = commandString + " UPDATE SKUTableTrans SET isLocked='False',isAvailable='False' WHERE SkuNumber IN (" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ItemIds + "); "; //SKU Table New
                        //commandString = commandString + " UPDATE CUSTORDER_HEADER SET IsConfirmed=1 WHERE ORDERNUM='" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OrderNo + "' ";

                        // Fixed Metal Rate New
                        if(!string.IsNullOrEmpty(Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRateCustOrdNo)))
                        {
                            string sCustOrdNo = Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRateCustOrdNo);
                            decimal dPayment = 0m;
                            decimal dFixedRatePercentage = 0m;
                            decimal dCustOrderTotalAmt = 0m;
                            decimal dMinPayAmt = 0m;

                            if(Convert.ToDecimal(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRateCustOrdPercentage) > 0)
                            {
                                LSRetailPosis.Transaction.CustomerPaymentTransaction custPayTrans = posTransaction as LSRetailPosis.Transaction.CustomerPaymentTransaction;
                                dFixedRatePercentage = Convert.ToDecimal(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRateCustOrdPercentage);
                                dCustOrderTotalAmt = Convert.ToDecimal(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRateCustOrdTotAmt);

                                dPayment = custPayTrans.Amount;

                                dMinPayAmt = (dFixedRatePercentage / 100) * dCustOrderTotalAmt;

                                if(dPayment >= dMinPayAmt)
                                {
                                    commandString = commandString + " UPDATE CUSTORDER_HEADER SET ISFIXEDMETALRATE = 1 WHERE ORDERNUM = '" + sCustOrdNo + "' ";
                                }
                            }
                        }

                        //--- end

                    }
                    if(((LSRetailPosis.Transaction.PosTransaction)(posTransaction)).EntryStatus != PosTransaction.TransactionStatus.Voided)
                    {
                        commandString += " DECLARE @INVENTLOCATION VARCHAR(20) " +
                                                   " DECLARE @CONFIGID VARCHAR(20) " +
                                                   " DECLARE @RATE numeric(28, 3) " +
                                                   " SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM         RETAILCHANNELTABLE INNER JOIN " +
                                                   " RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID " +
                                                   " WHERE RETAILSTORETABLE.STORENUMBER='" + posTransaction.StoreId + "'" +
                                                   " SELECT @CONFIGID = DEFAULTCONFIGIDGOLD FROM [INVENTPARAMETERS] WHERE DATAAREAID='" + application.Settings.Database.DataAreaID + "' " +
                                                   "  SELECT TOP 1 @RATE=RATES FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION  " +
                                                   " AND METALTYPE=" + (int)Enums.EnumClass.MetalType.Gold + " AND ACTIVE=1  " +
                                                   " AND CONFIGIDSTANDARD=@CONFIGID  AND RATETYPE=" + (int)Enums.EnumClass.RateType.GSS + " " +
                                                   " ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC;";
                        // " ORDER BY [TRANSDATE],[TIME] DESC; ";
                        if(Convert.ToBoolean(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GoldFixing))
                        {

                            commandString += " INSERT INTO [DEPOSIT_GOLD_DETAILS] " +
                                                  " ([ADVANCEID],[ORDERID],[CUSTACCOUNT],[PURCQTY],[AMOUNT],[STOREID] " +
                                                  " ,[TERMINALID],[STAFFID],[DATAAREAID],[CREATEDON]) " +
                                                  " VALUES " +
                                                  " ('" + posTransaction.TransactionId + "', " +
                                                  " '" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OrderNo + "', " +
                                                  " '" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).Customer.CustomerId + "', " +
                                                  " ISNULL(" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).Amount + "/@RATE,0), " +
                                                  " " + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).Amount + " , " +
                                                  "'" + posTransaction.StoreId + "', " +
                                                  " '" + posTransaction.TerminalId + "', " +
                                                  "'" + posTransaction.OperatorId + "', " +
                                                  "'" + application.Settings.Database.DataAreaID + "', " +
                                                  " GETDATE()); ";
                        }
                        else
                        {
                            commandString += " SET @RATE= NULL ; ";
                        }

                        #region - RETAIL GSS ACCOUNT DEPOSIT - COMMENTED
                        //   if (((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationType == "GSS")
                        //  {

                        //commandString += "  " +
                        //                       " INSERT INTO [RETAILGSSACCOUNTDEPOSIT] " +
                        //                       " ([TRANSACTIONID],[CUSTACCOUNT],[GSSNUMBER],[NOOFMONTHS],[AMOUNT],[GOLDFIXING],[STOREID] " +
                        //                       " ,[TERMINALID],[STAFFID],[DATAAREAID],[CREATEDON]) " +
                        //                       " VALUES " +
                        //                       " ('" + posTransaction.TransactionId + "', " +
                        //                       " '" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).Customer.CustomerId + "', " +
                        //                       " '" + Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GSSNum) + "', " +
                        //                       " '" + Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.NumMonths) + "', " +
                        //                       " " + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).Amount + " , " +
                        //                       " " + goldfixing + ", " +
                        //                       "'" + posTransaction.StoreId + "', " +
                        //                       " '" + posTransaction.TerminalId + "', " +
                        //                       "'" + posTransaction.OperatorId + "', " +
                        //                       "'" + application.Settings.Database.DataAreaID + "', " +
                        //                       " GETDATE()); ";

                        //  }
                        #endregion

                        commandString += " INSERT INTO [RETAILDEPOSITTABLE] " +
                                       " ([TRANSACTIONID],[CUSTACCOUNT],[ORDERNUM],[AMOUNT],[GOLDQUANTITY],[GOLDFIXING] " +
                                       " ,[DEPOSITTYPE],[STOREID],[TERMINALID],[STAFFID],[DATAAREAID],[CREATEDON] ";
                        if(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationType == "GSS")
                        {
                            commandString += ",[GSSNUMBER],[NOOFMONTHS] ";
                        }

                        commandString += " ) " +
                                       " VALUES " +
                                       " ('" + posTransaction.TransactionId + "' " +
                                       " ,'" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).Customer.CustomerId + "' " +
                                       " ,'" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OrderNo + "' " +
                                       " ," + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).Amount + " " +
                                       " , ISNULL(" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).Amount + "/@RATE,0) " +
                                       " ," + goldfixing + " " +
                                       " ," + Convert.ToInt16(Convert.ToBoolean(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationDone) ? (int)Enums.EnumClass.DepositType.GSS : (int)Enums.EnumClass.DepositType.Normal) + " " +
                                       " ,'" + posTransaction.StoreId + "' " +
                                       " ,'" + posTransaction.TerminalId + "' " +
                                       " ,'" + posTransaction.OperatorId + "' " +
                                       " ,'" + application.Settings.Database.DataAreaID + "' " +
                                       " ,GETDATE() ";
                        if(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationType == "GSS")
                        {
                            commandString += ",'" + Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GSSNum) + "' ";
                            commandString += "," + Convert.ToInt16(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.NumMonths) + " ";
                        }
                        commandString += " ); ";

                        if(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationType != "GSS")
                        {
                            commandString += " INSERT INTO [RETAILADJUSTMENTTABLE] " +
                                        " ([TRANSACTIONID],[CUSTACCOUNT],[ORDERNUM],[AMOUNT],[GOLDQUANTITY],[GOLDFIXING],[ISADJUSTED] " +
                                        " ,[RETAILDEPOSITTYPE],[RETAILSTOREID],[RETAILTERMINALID],[RETAILSTAFFID],[DATAAREAID]) " +
                                        " VALUES " +
                                        " ('" + posTransaction.TransactionId + "' " +
                                        " ,'" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).Customer.CustomerId + "' " +
                                        " ,'" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OrderNo + "' " +
                                        " ," + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).Amount + " " +
                                        " , ISNULL(" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).Amount + "/@RATE,0) " +
                                        " ," + goldfixing + " " +
                                        " ,0 " +
                                        " ," + Convert.ToInt16(Convert.ToBoolean(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationDone) ? (int)Enums.EnumClass.DepositType.GSS : (int)Enums.EnumClass.DepositType.Normal) + " " +
                                        " ,'" + posTransaction.StoreId + "' " +
                                        " ,'" + posTransaction.TerminalId + "' " +
                                        " ,'" + posTransaction.OperatorId + "' " +
                                        " ,'" + application.Settings.Database.DataAreaID + "' " +
                                        " ); ";
                        }


                        if (!string.IsNullOrEmpty(Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.Remarks)))
                        {
                            if (((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).EntryStatus != PosTransaction.TransactionStatus.Voided)
                            {
                                string sRemarks = Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.Remarks);

                                string sLCQry = " UPDATE RETAILTRANSACTIONTABLE SET Remarks = '" + sRemarks + "'" +
                                    " WHERE TRANSACTIONID='" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).TransactionId + "'" +
                                    " AND TERMINAL = '" + ApplicationSettings.Terminal.TerminalId + "'";

                                using (SqlCommand cmdLC = new SqlCommand(sLCQry, sqlTransaction.Connection, sqlTransaction))
                                {
                                    cmdLC.ExecuteNonQuery();
                                }
                            }
                        }

                        // Approval Code 

                        //// if (!string.IsNullOrEmpty(Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.EFTApprovalCode)))
                        // if (!string.IsNullOrEmpty(Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.EFTCardNo)))

                        // {
                        //     string sEFTAppovalCode = Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.EFTCardNo);
                        //     commandString = commandString + "  UPDATE RETAILTRANSACTIONPAYMENTTRANS SET EFTApprovalCode = '" + sEFTAppovalCode + "' WHERE TRANSACTIONID = '" + posTransaction.TransactionId + "' AND (ISNULL(CARDORACCOUNT,'') <> '');"; //AND TENDERTYPE = 2

                        // }
                        // //
                    }

            #endregion

                    using(SqlCommand command = new SqlCommand(commandString, sqlTransaction.Connection, sqlTransaction))
                    {
                        command.ExecuteNonQuery();
                    }

                }
            }
            catch(Exception)
            {
                // string s = " UPDATE RETAILTEMPTABLE SET TRANSID=NULL,CUSTID=NULL,CUSTORDER=NULL,GOLDFIXING=NULL,ITEMRETURN='False',MINIMUMDEPOSITFORCUSTORDER=NULL WHERE ID IN (1,2); ";
                string s = " UPDATE RETAILTEMPTABLE SET TRANSID=NULL,CUSTID=NULL,CUSTORDER=NULL,GOLDFIXING=NULL,ITEMRETURN='False',MINIMUMDEPOSITFORCUSTORDER=NULL WHERE ID IN (1,2) AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "'" + // RETAILTEMPTABLE
                    //Start: added on 16/06/2014 
                    // when gss emi is recived by real tiem service, after selecting the amount or scheme, if
                    //Connection is out then that payment already saved in these three table(base)
                    // so in our customization these data should not be there.
                          " DELETE FROM RETAILTRANSACTIONTABLE WHERE TRANSACTIONID = '" + posTransaction.TransactionId + "'" +
                          " AND STORE='" + posTransaction.StoreId + "'" +
                          " AND TERMINAL='" + ApplicationSettings.Terminal.TerminalId + "'" +
                          " AND DATAAREAID ='" + application.Settings.Database.DataAreaID + "'" +

                          " DELETE FROM RETAILTRANSACTIONTABLEEX5 WHERE TRANSACTIONID = '" + posTransaction.TransactionId + "'" +
                          " AND STORE='" + posTransaction.StoreId + "'" +
                          " AND TERMINAL='" + ApplicationSettings.Terminal.TerminalId + "'" +
                          " AND DATAAREAID ='" + application.Settings.Database.DataAreaID + "'" +

                          " DELETE FROM RETAILTRANSACTIONPAYMENTTRANS WHERE TRANSACTIONID = '" + posTransaction.TransactionId + "'" +
                          " AND STORE='" + posTransaction.StoreId + "'" +
                          " AND TERMINAL='" + ApplicationSettings.Terminal.TerminalId + "'" +
                          " AND DATAAREAID ='" + application.Settings.Database.DataAreaID + "'";
                //End : added on 16/06/2014 
                using(SqlCommand command = new SqlCommand(s, sqlTransaction.Connection, sqlTransaction))
                {
                    command.ExecuteNonQuery();
                }
                MessageBox.Show("Unable to save due to connection issue.");

            }

        #endregion

            //Example:
            //if (posTransaction is IRetailTransaction)
            //{
            //    string commandString = "INSERT INTO PARTNER_CUSTOMTRANSACTIONTABLE VALUES (@VAL1)";

            //    using (SqlCommand sqlCmd = new SqlCommand(commandString, sqlTransaction.Connection, sqlTransaction))
            //    {
            //        sqlCmd.Parameters.Add(new SqlParameter("@VAL1", posTransaction.PartnerData.TestData));
            //        sqlCmd.ExecuteNonQuery();
            //    }
            //}
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

        public void PreEndTransaction(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("TransactionTriggers.PreEndTransaction", "When concluding the transaction, prior to printing and saving...", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PostEndTransaction(IPosTransaction posTransaction)
        {
            try
            {
                RetailTransaction retailTransaction = posTransaction as RetailTransaction;
                string sStoreId = ApplicationSettings.Terminal.StoreId;
                string sTerminalId = ApplicationSettings.Terminal.TerminalId;

                if(Convert.ToString(posTransaction.GetType().Name).ToUpper().Trim() == "CUSTOMERPAYMENTTRANSACTION")
                {
                    LSRetailPosis.Transaction.CustomerPaymentTransaction custTrans = posTransaction as LSRetailPosis.Transaction.CustomerPaymentTransaction;
                    if(custTrans != null)
                    {
                        if(!string.IsNullOrEmpty(Convert.ToString(custTrans.PartnerData.EFTCardNo)))
                        {
                            DataTable dtCardInfo = new DataTable();
                            SqlConnection conn = new SqlConnection();

                            if(application != null)
                                conn = application.Settings.Database.Connection;
                            else
                                conn = ApplicationSettings.Database.LocalConnection;

                            dtCardInfo = GetCardIfo(custTrans.TransactionId);

                            if(dtCardInfo != null && dtCardInfo.Rows.Count > 0)
                            {
                                foreach(DataRow dr in dtCardInfo.Rows)
                                {
                                    string sEFTAppovalCode = Convert.ToString(dr["APPROVALCODE"]);
                                    string sExpMonth = Convert.ToString(dr["CARDEXPIRYMONTH"]);
                                    string sExpYear = Convert.ToString(dr["CARDEXPIRYYEAR"]);

                                    string sQuery = " UPDATE RETAILTRANSACTIONPAYMENTTRANS SET EFTApprovalCode = '" + sEFTAppovalCode + "'" +
                                                    " ,CARDEXPIRYMONTH = '" + sExpMonth + "',CARDEXPIRYYEAR ='" + sExpYear + "'" +
                                                    " WHERE TRANSACTIONID = '" + custTrans.TransactionId + "'" +
                                                    " AND (ISNULL(CARDORACCOUNT,'') = '" + Convert.ToString(dr["CARDNO"]) + "')" +
                                                    " AND STORE = '" + sStoreId + "' AND TERMINAL = '" + sTerminalId + "' ";

                                    // using (SqlCommand command = new SqlCommand(sQuery, sqlTransaction.Connection, sqlTransaction))
                                    using(SqlCommand command = new SqlCommand(sQuery, conn))
                                    {
                                        command.ExecuteNonQuery();
                                    }
                                }

                                string sCardTblName = "EXTNDCARDINFO" + ApplicationSettings.Terminal.TerminalId;

                                string sQry = "IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sCardTblName + "')" +
                                              " BEGIN  DROP TABLE " + sCardTblName + " END ";
                                using(SqlCommand command = new SqlCommand(sQry, conn))
                                {
                                    command.ExecuteNonQuery();
                                }
                            }
                        }

                        //REPAIR order cash advance update
                        try
                        {
                            if(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.IsRepair)
                            {
                                string repairid = Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OrderNo);

                                SqlConnection conn = new SqlConnection();

                                if(application != null)
                                    conn = application.Settings.Database.Connection;
                                else
                                    conn = ApplicationSettings.Database.LocalConnection;

                                if(conn.State == ConnectionState.Closed)
                                    conn.Open();

                                string sQuery = string.Empty;
                                sQuery += " UPDATE  [RetailRepairDetail] SET [CASHADVANCE]='" + custTrans.Amount +
                                          "' WHERE [RepairId]='" + repairid + "' AND [RetailStoreId]='" + custTrans.StoreId +
                                          "' AND [RetailTerminalId]='" + custTrans.TerminalId +
                                          "' AND [DATAAREAID]='" + ApplicationSettings.Database.DATAAREAID + "';";
                                using(SqlCommand command = new SqlCommand(sQuery, conn))
                                {
                                    command.ExecuteNonQuery();
                                }
                            }
                        }
                        finally
                        {

                        }
                    }

                }
                if(retailTransaction != null)
                {
                    if(!string.IsNullOrEmpty(Convert.ToString(retailTransaction.PartnerData.EFTCardNo)))
                    {
                        DataTable dtCardInfo = new DataTable();
                        SqlConnection conn = new SqlConnection();
                        if(application != null)
                            conn = application.Settings.Database.Connection;
                        else
                            conn = ApplicationSettings.Database.LocalConnection;

                        dtCardInfo = GetCardIfo(retailTransaction.TransactionId);

                        if(dtCardInfo != null && dtCardInfo.Rows.Count > 0)
                        {
                            foreach(DataRow dr in dtCardInfo.Rows)
                            {
                                string sEFTAppovalCode = Convert.ToString(dr["APPROVALCODE"]);
                                string sExpMonth = Convert.ToString(dr["CARDEXPIRYMONTH"]);
                                string sExpYear = Convert.ToString(dr["CARDEXPIRYYEAR"]);

                                string sQuery = " UPDATE RETAILTRANSACTIONPAYMENTTRANS SET EFTApprovalCode = '" + sEFTAppovalCode + "'" +
                                                " ,CARDEXPIRYMONTH = '" + sExpMonth + "',CARDEXPIRYYEAR ='" + sExpYear + "'" +
                                                " WHERE TRANSACTIONID = '" + retailTransaction.TransactionId + "'" +
                                                " AND (ISNULL(CARDORACCOUNT,'') = '" + Convert.ToString(dr["CARDNO"]) + "')" +
                                                " AND STORE = '" + sStoreId + "' AND TERMINAL = '" + sTerminalId + "'";
                                using(SqlCommand command = new SqlCommand(sQuery, conn))
                                {
                                    command.ExecuteNonQuery();
                                }
                            }
                            string sCardTblName = "EXTNDCARDINFO" + ApplicationSettings.Terminal.TerminalId;

                            string sQry = "IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sCardTblName + "')" +
                                          " BEGIN  DROP TABLE " + sCardTblName + " END ";
                            using(SqlCommand command = new SqlCommand(sQry, conn))
                            {
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }


            }
            catch(Exception)
            {

            }


            #region Start: Global Advance save
            LSRetailPosis.Transaction.CustomerPaymentTransaction custTrans1 = posTransaction as LSRetailPosis.Transaction.CustomerPaymentTransaction;
            if(custTrans1 != null)
            {
                try
                {
                    ReadOnlyCollection<object> containerArray;
                    string sMsg = string.Empty;
                    MemoryStream mstrRETAILDEPOSITTABLE = new MemoryStream();
                    MemoryStream mstrRETAILADJUSTMENTTABLE = new MemoryStream();
                    string sXMLRETAILDEPOSITTABLE = string.Empty;
                    string sXMLRETAILADJUSTMENTTABLE = string.Empty;

                    DataTable dtRETAILDEPOSITTABLE = new DataTable();
                    DataTable dtRETAILADJUSTMENTTABLE = new DataTable();

                    dtRETAILDEPOSITTABLE = GetAdvDetailInfo(" select * from RETAILDEPOSITTABLE where TERMINALID='" + ApplicationSettings.Terminal.TerminalId + "' and STOREID='" + ApplicationSettings.Terminal.StoreId + "' and TransactionId='" + custTrans1.TransactionId + "'");
                    dtRETAILADJUSTMENTTABLE = GetAdvDetailInfo(" select * from RETAILADJUSTMENTTABLE where RETAILTERMINALID='" + ApplicationSettings.Terminal.TerminalId + "' and RETAILSTOREID='" + ApplicationSettings.Terminal.StoreId + "'and TransactionId='" + custTrans1.TransactionId + "'");

                    if(dtRETAILDEPOSITTABLE != null && dtRETAILDEPOSITTABLE.Rows.Count > 0)
                    {
                        dtRETAILDEPOSITTABLE.TableName = "RETAILDEPOSITTABLE";
                        dtRETAILDEPOSITTABLE.WriteXml(mstrRETAILDEPOSITTABLE, true);
                        mstrRETAILDEPOSITTABLE.Seek(0, SeekOrigin.Begin);
                        StreamReader sr = new StreamReader(mstrRETAILDEPOSITTABLE);
                        sXMLRETAILDEPOSITTABLE = sr.ReadToEnd();
                    }
                    if(dtRETAILADJUSTMENTTABLE != null && dtRETAILADJUSTMENTTABLE.Rows.Count > 0)
                    {
                        dtRETAILADJUSTMENTTABLE.TableName = "RETAILADJUSTMENTTABLE";
                        dtRETAILDEPOSITTABLE.WriteXml(mstrRETAILADJUSTMENTTABLE, true);
                        mstrRETAILADJUSTMENTTABLE.Seek(0, SeekOrigin.Begin);
                        StreamReader sr1 = new StreamReader(mstrRETAILADJUSTMENTTABLE);
                        sXMLRETAILADJUSTMENTTABLE = sr1.ReadToEnd();
                    }


                    if(PosApplication.Instance.TransactionServices.CheckConnection())
                    {
                        containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("insertCustomerAdvanceData", sXMLRETAILDEPOSITTABLE, sXMLRETAILADJUSTMENTTABLE);
                        sMsg = Convert.ToString(containerArray[2]);
                        // MessageBox.Show(sMsg);
                    }
                }
                catch(Exception ex)
                {


                }
            }
            #endregion

            LSRetailPosis.ApplicationLog.Log("TransactionTriggers.PostEndTransaction", "When concluding the transaction, after printing and saving", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PreVoidTransaction(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("TransactionTriggers.PreVoidTransaction", "Before voiding the transaction...", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PostVoidTransaction(IPosTransaction posTransaction)
        {
            #region Changed By Nimbus

            CheckInventory(posTransaction, (int)Enums.EnumClass.InventoryTransactionType.Void);
            RetailTransaction retailtrans = posTransaction as RetailTransaction;
            if(retailtrans != null)
            {
                if(retailtrans.PartnerData.IsGSSMaturity)
                {
                    return;
                }
                bool IsRepairRet = Convert.ToBoolean(retailtrans.PartnerData.IsRepairRetTrans);
                if(IsRepairRet)
                {
                    VoidRepairReturn(retailtrans.PartnerData.RefRepairId, retailtrans.PartnerData.RepairRetTransId);
                }


                foreach(SaleLineItem saleLineItem in retailtrans.SaleItems)
                {
                    if(saleLineItem.ItemType == LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                    {
                        updateCustomerAdvanceAdjustment(Convert.ToString(saleLineItem.PartnerData.ServiceItemCashAdjustmentTransactionID),
                           Convert.ToString(saleLineItem.PartnerData.ServiceItemCashAdjustmentStoreId),
                           Convert.ToString(saleLineItem.PartnerData.ServiceItemCashAdjustmentTerminalId), 0);

                        //RTS is calling for update changed on 27/11/2015
                        string sTransId = Convert.ToString(saleLineItem.PartnerData.ServiceItemCashAdjustmentTransactionID);
                        string sStoreId = Convert.ToString(saleLineItem.PartnerData.ServiceItemCashAdjustmentStoreId);
                        string sTerminalId = Convert.ToString(saleLineItem.PartnerData.ServiceItemCashAdjustmentTerminalId);
                        try
                        {
                            ReadOnlyCollection<object> containerArray;
                            string sMsg = string.Empty;

                            if(PosApplication.Instance.TransactionServices.CheckConnection())
                            {
                                containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("updateAdvanceForVoid", sTransId, sStoreId, sTerminalId);
                                sMsg = Convert.ToString(containerArray[2]);
                                //MessageBox.Show(sMsg);

                            }
                        }
                        catch(Exception ex)
                        {

                        }
                    }
                }

                #region //Nimbus
                using(SqlConnection conn = (Application != null) ? Application.Settings.Database.Connection : ApplicationSettings.Database.LocalConnection)
                {
                    try
                    {
                        //Start : 18/09/14
                        StringBuilder commandText = new StringBuilder();

                        //if(!string.IsNullOrEmpty(retailtrans.PartnerData.REPAIRID))
                        //    commandText.AppendLine(" update RetailRepairDetail set IsDelivered=0 where BatchId ='" + retailtrans.PartnerData.REPAIRID + "'; ");

                        ////string sTblName = "NEGCASHPAY" + ApplicationSettings.Terminal.TerminalId;

                        //commandText.AppendLine("IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTblName + "')");
                        //commandText.AppendLine(" BEGIN  DROP TABLE " + sTblName + " END ");

                        // string sTableName = "ORDERINFO" + "" + ApplicationSettings.Database.TerminalID;

                        //commandText.AppendLine("IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTableName + "')");
                        if(!string.IsNullOrEmpty(retailtrans.PartnerData.AdjustmentOrderNum))
                            commandText.AppendLine(" update RETAILADJUSTMENTTABLE set ISADJUSTED=0 where ORDERNUM= '" + retailtrans.PartnerData.AdjustmentOrderNum + "'");
                        //commandText.AppendLine(" BEGIN  DROP TABLE " + sTableName + " END ");

                        if(!string.IsNullOrEmpty(commandText.ToString()))
                        {
                            if(conn.State == ConnectionState.Closed)
                                conn.Open();

                            using(SqlCommand cmd = new SqlCommand(commandText.ToString(), conn))
                            {
                                cmd.ExecuteNonQuery();
                            }
                        }
                        //End : 
                    }
                    catch { }
                    finally
                    {
                        if(conn.State == ConnectionState.Open)
                            conn.Close();
                    }
                }
                #endregion

            }


            #endregion
            string source = "TransactionTriggers.PostVoidTransaction";
            string value = "After voiding the transaction...";
            LSRetailPosis.ApplicationLog.Log(source, value, LSRetailPosis.LogTraceLevel.Trace);
            LSRetailPosis.ApplicationLog.WriteAuditEntry(source, value);
        }

        public void PreReturnTransaction(IPreTriggerResult preTriggerResult, IRetailTransaction originalTransaction, IPosTransaction posTransaction)
        {
            RetailTransaction SaleTrans = posTransaction as RetailTransaction; ;

            if(SaleTrans.SaleItems.Count > 0)
            {
                preTriggerResult.ContinueOperation = false;
                preTriggerResult.MessageId = 999998;
                return;
            }

            #region Chages BY NIMBUS
            DataTable dtRec = new DataTable();
            RetailTransaction retailTransaction = originalTransaction as RetailTransaction;

            bool isExchange = false;
            bool isFullSalesReturn = false;

            if(retailTransaction != null)
            {
                ReadOnlyCollection<object> containerArraySR;

                try
                {
                    if(this.Application.TransactionServices.CheckConnection())
                    {
                        string sTransactionId = originalTransaction.TransactionId;
                        string sStoreId = originalTransaction.StoreId;
                        string sTerminalId = originalTransaction.TerminalId;

                        containerArraySR = this.Application.TransactionServices.InvokeExtension
                                                                                    ("GetSalesReturnInfo", sTransactionId, sStoreId, sTerminalId);
                        DataSet dsTransDetail = new DataSet();
                        StringReader srTransDetail = new StringReader(Convert.ToString(containerArraySR[3]));

                        if(Convert.ToString(containerArraySR[3]).Trim().Length > 38)
                        {
                            dsTransDetail.ReadXml(srTransDetail);
                        }

                        if(dsTransDetail != null
                            && dsTransDetail.Tables.Count > 0 && dsTransDetail.Tables[0].Rows.Count > 0)
                        {
                            dtRec = dsTransDetail.Tables[0];

                            dtRec.Columns.Add("SKUDETAILS", typeof(string));

                            DataSet dsIngredient = new DataSet();
                            StringReader srIngredient = new StringReader(Convert.ToString(containerArraySR[4]));

                            if(Convert.ToString(containerArraySR[4]).Trim().Length > 38)
                            {
                                dsIngredient.ReadXml(srIngredient);
                                dsIngredient.AcceptChanges();
                            }

                            if(dsIngredient != null
                                && dsIngredient.Tables.Count > 0 && dsIngredient.Tables[0].Rows.Count > 0)
                            {
                                int d = 1; //changes decimal to int // 19/05/2014
                                foreach(DataRow drtrans in dtRec.Rows)
                                {
                                    DataTable dtfilter = new DataTable();
                                    string sfilter = "RefLineNum = '" + dtRec.Rows[Convert.ToInt32(d) - 1]["LineNum"] + "' "; //changes decimal to int // 19/05/2014 only d was there
                                    DataView dv = new DataView(dsIngredient.Tables[0]);
                                    dv.RowFilter = sfilter;
                                    dtfilter = dv.ToTable();
                                    dtfilter.AcceptChanges();
                                    if(dtfilter != null && dtfilter.Rows.Count > 0) //blocked  19/05/2014
                                    {
                                        MemoryStream mstr = new MemoryStream();
                                        dtfilter.WriteXml(mstr, true);
                                        mstr.Seek(0, SeekOrigin.Begin);
                                        StreamReader sr = new StreamReader(mstr);
                                        string sXML = string.Empty;
                                        sXML = sr.ReadToEnd();
                                        dtRec.Rows[Convert.ToInt32(d) - 1]["SKUDETAILS"] = sXML;
                                        dtRec.AcceptChanges();
                                    }
                                    d++;
                                }
                            }
                        }
                    }
                }
                catch(Exception ex)
                {


                }

                if(dtRec == null || dtRec.Rows.Count <= 0)
                {
                    string commandString = " SELECT [LINENUM],[PIECES],[QUANTITY],[CRATE] AS RATE,[RATETYPE],[MAKINGRATE],[MAKINGTYPE],[AMOUNT], " +
                                           " [MAKINGDISCOUNT],[MAKINGAMOUNT],[TOTALAMOUNT],[TOTALWEIGHT],[LOSSPERCENTAGE],[LOSSWEIGHT], " +
                                           " [EXPECTEDQUANTITY],[TRANSACTIONTYPE],[OWN],[SKUDETAILS],[ORDERNUM],[ORDERLINENUM],[CUSTNO]" +
                                           " ,ISNULL(WastageType,0) AS WastageType,ISNULL(WastageQty,0) AS WastageQty,ISNULL(WastageAmount,0) AS WastageAmount" + // Changed for wastage
                                           " ,ISNULL(WastagePercentage,0) AS WastagePercentage,ISNULL(WastageRate,0) AS WastageRate,ISNULL(MakingDiscountAmount,0) as MakingDiscountAmount,MAKINGDISCTYPE" + // Making Discount Type
                                           " ,T.TRANSDATE,ISNULL(IsIngredient,0) AS IsIngredient,ISNULL(RETAIL_CUSTOMCALCULATIONS_TABLE.CONFIGID,'') AS CONFIGID,ISNULL(RETAIL_CUSTOMCALCULATIONS_TABLE.PURITY,0) AS PURITY" +
                                           " FROM [RETAIL_CUSTOMCALCULATIONS_TABLE] " +
                                           " INNER JOIN RETAILTRANSACTIONTABLE T ON RETAIL_CUSTOMCALCULATIONS_TABLE.TRANSACTIONID = T.TRANSACTIONID " + // type = 2 for sales
                                           " AND RETAIL_CUSTOMCALCULATIONS_TABLE.STOREID = T.STORE AND RETAIL_CUSTOMCALCULATIONS_TABLE.TERMINALID = T.TERMINAL" +
                                           " WHERE  RETAIL_CUSTOMCALCULATIONS_TABLE.DATAAREAID=@DATAAREAID AND [STOREID]=@STOREID AND [TERMINALID]=@TERMINALID AND " + //
                                           " RETAIL_CUSTOMCALCULATIONS_TABLE.TRANSACTIONTYPE <> 5 AND RETAIL_CUSTOMCALCULATIONS_TABLE.TRANSACTIONID=@TRANSACTIONID AND T.TYPE = 2 ";

                    SqlConnection connection = new SqlConnection();

                    if(application != null)
                        connection = application.Settings.Database.Connection;
                    else
                        connection = ApplicationSettings.Database.LocalConnection;


                    if(connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }

                    SqlCommand command = new SqlCommand(commandString, connection);

                    command.Parameters.Add(new SqlParameter("@DATAAREAID", ApplicationSettings.Database.DATAAREAID));
                    command.Parameters.Add(new SqlParameter("@STOREID", originalTransaction.StoreId));
                    command.Parameters.Add(new SqlParameter("@TERMINALID", originalTransaction.TerminalId));
                    command.Parameters.Add(new SqlParameter("@TRANSACTIONID", originalTransaction.TransactionId));


                    command.CommandTimeout = 0;
                    SqlDataReader reader = command.ExecuteReader();
                    dtRec.Load(reader);
                }

                retailTransaction.SaleIsReturnSale = true;
            }



            if(dtRec != null && dtRec.Rows.Count > 0)
            {
                foreach(DataColumn col in dtRec.Columns)
                    col.ReadOnly = false;
            }

            System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem> lineitemoriginal = new System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem>(((LSRetailPosis.Transaction.RetailTransaction)(originalTransaction)).SaleItems);
            System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem> lineitem = new System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem>(((LSRetailPosis.Transaction.RetailTransaction)(originalTransaction)).SaleItems);
            if(dtRec != null && dtRec.Rows.Count > 0)
            {
                #region
                //commented for SAMRA on 15/10/15 req Sumanta
                //frmOptionSelectionExchangeBuyback objReturnOption = new frmOptionSelectionExchangeBuyback();
                //objReturnOption.ShowDialog();

                //if (objReturnOption.isCancel == true)
                //{
                //    preTriggerResult.ContinueOperation = false;
                //    return;
                //}


                //if (objReturnOption.isExchange == true)
                //    isExchange = true;
                //else
                //    isExchange = false;

                //if (objReturnOption.isFullReturn == true)
                isFullSalesReturn = true;
                //else
                //    isFullSalesReturn = false;
                int iSPId = 0;
                iSPId = getUserDiscountPermissionId();
                string sFieldName = string.Empty;

                if(Convert.ToInt16(iSPId) == (int)CRWRetailDiscPermission.Salesperson)
                {
                    if(isFullSalesReturn)
                    {
                        DateTime dtTransDate = Convert.ToDateTime(dtRec.Rows[0]["TRANSDATE"]);
                        int iFullExchngValidDay = GetFullExchngValidDay(posTransaction.StoreId);

                        if(Convert.ToInt16((DateTime.Now - dtTransDate).TotalDays) > iFullExchngValidDay)
                        {
                            preTriggerResult.ContinueOperation = false;
                            preTriggerResult.MessageId = 999997;
                            return;
                        }
                    }
                }
                else if(Convert.ToInt16(iSPId) == (int)CRWRetailDiscPermission.Manager)
                {
                    if(isFullSalesReturn)
                    {
                        BlankOperations.WinFormsTouch.frmSalesSplDiscInfo frmSalesSplDiscInfo = new BlankOperations.WinFormsTouch.frmSalesSplDiscInfo();
                        frmSalesSplDiscInfo.ShowDialog();

                        sSplReturn = frmSalesSplDiscInfo.sCodeOrRemarks;
                    }
                }
                else if(Convert.ToInt16(iSPId) == (int)CRWRetailDiscPermission.Manager2)
                {
                    if(isFullSalesReturn)
                    {
                        BlankOperations.WinFormsTouch.frmSalesSplDiscInfo frmSalesSplDiscInfo = new BlankOperations.WinFormsTouch.frmSalesSplDiscInfo();
                        frmSalesSplDiscInfo.ShowDialog();

                        sSplReturn = frmSalesSplDiscInfo.sCodeOrRemarks;
                    }
                }

                else
                {
                    if(isFullSalesReturn)
                    {
                        DateTime dtTransDate = Convert.ToDateTime(dtRec.Rows[0]["TRANSDATE"]);
                        int iFullExchngValidDay = GetFullExchngValidDay(posTransaction.StoreId);

                        if((DateTime.Now - dtTransDate).TotalDays > iFullExchngValidDay)
                        {
                            preTriggerResult.ContinueOperation = false;
                            preTriggerResult.MessageId = 999997;
                            return;
                        }

                    }
                }

                #endregion


                foreach(var item in lineitem)
                {
                    foreach(DataRow dr in dtRec.Rows)
                    {
                        if(isFullSalesReturn == false)
                        {
                            #region [Exchange / Buy Back]

                            DateTime dtDate = Convert.ToDateTime(dtRec.Rows[0]["TRANSDATE"]);

                            if(Convert.ToInt32(dr["IsIngredient"]) == 1)
                            {
                                DataSet dsIngredients = new DataSet();
                                StringReader reader = new StringReader(Convert.ToString(dr["SKUDETAILS"]));
                                dsIngredients.ReadXml(reader);

                                if(dsIngredients != null && dsIngredients.Tables[0].Rows.Count > 0)
                                {
                                    foreach(DataRow drIng in dsIngredients.Tables[0].Rows)
                                    {
                                        decimal dIngrdRate = 0m;
                                        decimal dNewIngrdCValue = 0m;

                                        if(Convert.ToInt32(drIng["METALTYPE"]) == (int)Enums.EnumClass.MetalType.Gold)
                                        #region
                                        {
                                            if(isExchange)
                                            {
                                                int iRateType = (int)Enums.EnumClass.RateType.Exchange;

                                                dIngrdRate = getGoldRate(Convert.ToString(drIng["ItemID"]), Convert.ToString(drIng["ConfigID"]),
                                                                        iRateType, originalTransaction.StoreId);
                                            }
                                            else
                                            {
                                                int iRateType = (int)Enums.EnumClass.RateType.OGP;

                                                dIngrdRate = getGoldRate(Convert.ToString(drIng["ItemID"]), Convert.ToString(drIng["ConfigID"]),
                                                                        iRateType, originalTransaction.StoreId);
                                            }

                                            dNewIngrdCValue = dIngrdRate * Convert.ToDecimal(drIng["QTY"]);

                                            decimal dOldHdrRate = 0m;

                                            // Making & wastage variables

                                            //decimal dOldHdrMKAmt = 0m; //
                                            //decimal dOldHdrWastageAmt = 0;

                                            //decimal dOldMakingIngrdAmt = 0m;//
                                            //decimal dNewMakingIngrdAmt = 0m;//

                                            //decimal dWastagePercentage = 0m; //WastagePercentage
                                            //decimal dIngrdWastageQty = 0m;
                                            //decimal dOldIngrdWastageAmt = 0m; //WastageAmount
                                            //decimal dNewIngrdWastageAmt = 0m;

                                            // ---- Making 

                                            //if ((int)Enums.EnumClass.MakingType.Percentage == Convert.ToInt32(dr["MAKINGTYPE"]))
                                            //{
                                            //    dOldHdrMKAmt = Math.Abs(Convert.ToDecimal(dr["MAKINGAMOUNT"]));

                                            //    dOldMakingIngrdAmt = (Math.Abs(Convert.ToDecimal(dr["MAKINGRATE"])) / 100) * Convert.ToDecimal(drIng["CValue"]);

                                            //    dNewMakingIngrdAmt = (Math.Abs(Convert.ToDecimal(dr["MAKINGRATE"])) / 100) * dNewIngrdCValue;

                                            //    dr["MAKINGAMOUNT"] = Math.Abs(Convert.ToDecimal(dr["MAKINGAMOUNT"])) - dOldMakingIngrdAmt + dNewMakingIngrdAmt;

                                            //    dr["TOTALAMOUNT"] = Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) - dOldHdrMKAmt + Math.Abs(Convert.ToDecimal(dr["MAKINGAMOUNT"]));
                                            //}

                                            //// --- Wastage
                                            //if (Convert.ToInt32(dr["WastageType"]) == 1) // percentage
                                            //{

                                            //    dOldHdrWastageAmt = Math.Abs(Convert.ToDecimal(dr["WastageAmount"]));

                                            //    dWastagePercentage = Math.Abs(Convert.ToDecimal(dr["WastagePercentage"]));
                                            //    dIngrdWastageQty = Convert.ToDecimal(drIng["QTY"]) * (dWastagePercentage / 100);

                                            //    dOldIngrdWastageAmt = dIngrdWastageQty * Convert.ToDecimal(drIng["Rate"]);
                                            //    dNewIngrdWastageAmt = dIngrdWastageQty * dIngrdRate;

                                            //    dr["WastageAmount"] = Math.Abs(Convert.ToDecimal(dr["WastageAmount"])) - dOldIngrdWastageAmt + dNewIngrdWastageAmt;
                                            //    dr["TOTALAMOUNT"] = Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) - dOldHdrWastageAmt + Math.Abs(Convert.ToDecimal(dr["WastageAmount"]));

                                            //}

                                            //

                                            //
                                            dOldHdrRate = Math.Abs(Convert.ToDecimal(dr["RATE"])); // Tot -- for ingredient  -- > Amount = Rate

                                            dr["RATE"] = (Math.Abs(Convert.ToDecimal(dr["RATE"])) - Convert.ToDecimal(drIng["CValue"])) + dNewIngrdCValue;

                                            /*
                                             RateType			
                                                    Weight = 0,
                                                    Pieces = 1,
                                                    Tot = 2,

                                             */

                                            // Tot -- for ingredient  -- > Amount = Rate
                                            dr["AMOUNT"] = Convert.ToDecimal(dr["RATE"]);
                                            dr["TOTALAMOUNT"] = (Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) - dOldHdrRate) + Convert.ToDecimal(dr["AMOUNT"]); // Tot -- for ingredient  -- > Amount = Rate

                                            dtRec.AcceptChanges();

                                            drIng["CValue"] = dNewIngrdCValue;
                                            drIng["Rate"] = dIngrdRate;
                                            dsIngredients.Tables[0].AcceptChanges();
                                            dsIngredients.Tables[0].TableName = "Ingredients";
                                            MemoryStream mstr = new MemoryStream();
                                            dsIngredients.Tables[0].WriteXml(mstr, true);
                                            mstr.Seek(0, SeekOrigin.Begin);
                                            StreamReader sr = new StreamReader(mstr);
                                            string sXML = string.Empty;
                                            sXML = sr.ReadToEnd();
                                            dr["SKUDETAILS"] = sXML;
                                            dtRec.AcceptChanges();
                                        }
                                        #endregion

                                        else if((Convert.ToInt32(drIng["METALTYPE"]) == (int)Enums.EnumClass.MetalType.LooseDmd)
                                                 || (Convert.ToInt32(drIng["METALTYPE"]) == (int)Enums.EnumClass.MetalType.Stone)) // add stone
                                        {
                                            #region

                                            decimal dExchangePercent = 0m;
                                            decimal dBuyBackPercent = 0m;
                                            decimal dStoneWtRange = 0m;
                                            decimal dStoneDiscAmt = 0m;

                                            int iCalcType = 0;
                                            decimal dDeductionAmt = 0;

                                            if(Convert.ToDecimal(drIng["PCS"]) > 0)
                                            {
                                                dStoneWtRange = Convert.ToDecimal(drIng["QTY"]) / Convert.ToDecimal(drIng["PCS"]);
                                            }
                                            else
                                            {
                                                dStoneWtRange = Convert.ToDecimal(drIng["QTY"]);
                                            }

                                            if(!string.IsNullOrEmpty(Convert.ToString(drIng["IngrdDiscTotAmt"])))
                                            {
                                                dStoneDiscAmt = Math.Abs(Convert.ToDecimal(drIng["IngrdDiscTotAmt"]));
                                            }

                                            dIngrdRate = GetIngredientInfo(ref dExchangePercent, ref dBuyBackPercent, ref iCalcType,
                                                                            originalTransaction.StoreId, Convert.ToString(drIng["ItemID"]),
                                                                            Convert.ToString(drIng["InventSizeID"]), Convert.ToString(drIng["InventColorID"]),
                                                                            dStoneWtRange, dtDate);
                                            if(isExchange)
                                            {
                                                // dNewIngrdCValue = Convert.ToDecimal(drIng["CValue"]) - ((dExchangePercent * Convert.ToDecimal(drIng["CValue"])) / 100);
                                                dDeductionAmt = (dIngrdRate * dExchangePercent) / 100;
                                            }
                                            else
                                            {
                                                // dNewIngrdCValue = Convert.ToDecimal(drIng["CValue"]) - ((dBuyBackPercent * Convert.ToDecimal(drIng["CValue"])) / 100);
                                                dDeductionAmt = (dIngrdRate * dBuyBackPercent) / 100;
                                            }
                                            // -- calc type
                                            // new cvalue - total deduction amount -- dStoneDiscAmt
                                            if(iCalcType == (int)Enums.EnumClass.CalcType.Weight)
                                            {
                                                // dNewIngrdCValue = (dIngrdRate * Convert.ToDecimal(drIng["QTY"])) - (dDeductionAmt * Convert.ToDecimal(drIng["QTY"]));
                                                dNewIngrdCValue = (dIngrdRate * Convert.ToDecimal(drIng["QTY"])) - (dDeductionAmt * Convert.ToDecimal(drIng["QTY"])) - dStoneDiscAmt;
                                            }
                                            else if(iCalcType == (int)Enums.EnumClass.CalcType.Pieces)
                                            {
                                                //dNewIngrdCValue = (dIngrdRate * Convert.ToDecimal(drIng["PCS"])) - (dDeductionAmt * Convert.ToDecimal(drIng["PCS"]));
                                                dNewIngrdCValue = (dIngrdRate * Convert.ToDecimal(drIng["PCS"])) - (dDeductionAmt * Convert.ToDecimal(drIng["PCS"])) - dStoneDiscAmt;
                                            }
                                            else
                                            {
                                                //dNewIngrdCValue = dIngrdRate - dDeductionAmt;
                                                dNewIngrdCValue = dIngrdRate - dDeductionAmt - dStoneDiscAmt;
                                            }

                                            decimal dOldHdrRate = Math.Abs(Convert.ToDecimal(dr["RATE"])); // Tot -- for ingredient  -- > Amount = Rate

                                            // dr["RATE"] = (Math.Abs(Convert.ToDecimal(dr["RATE"])) - Convert.ToDecimal(drIng["CValue"])) + dNewIngrdCValue;
                                            dr["RATE"] = (Math.Abs(Convert.ToDecimal(dr["RATE"])) - Convert.ToDecimal(drIng["CValue"])) + dNewIngrdCValue - dStoneDiscAmt;

                                            /*
                                             RateType			
                                                    Weight = 0,
                                                    Pieces = 1,
                                                    Tot = 2,

                                             */

                                            // Tot -- for ingredient  -- > Amount = Rate
                                            // NewAmount = oldAmount - oldheaderrate + newheaderrate
                                            dr["AMOUNT"] = Convert.ToDecimal(dr["RATE"]);
                                            dr["TOTALAMOUNT"] = (Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) - dOldHdrRate) + Convert.ToDecimal(dr["AMOUNT"]); // Tot -- for ingredient  -- > Amount = Rate

                                            dtRec.AcceptChanges();
                                            drIng["CValue"] = dNewIngrdCValue;
                                            drIng["Rate"] = dIngrdRate;

                                            dsIngredients.Tables[0].AcceptChanges();

                                            dsIngredients.Tables[0].TableName = "Ingredients";
                                            MemoryStream mstr = new MemoryStream();
                                            dsIngredients.Tables[0].WriteXml(mstr, true);
                                            mstr.Seek(0, SeekOrigin.Begin);
                                            StreamReader sr = new StreamReader(mstr);
                                            string sXML = string.Empty;
                                            sXML = sr.ReadToEnd();
                                            dr["SKUDETAILS"] = sXML;
                                            dtRec.AcceptChanges();
                                            #endregion
                                        }

                                    }

                                    dr["TOTALAMOUNT"] = Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) -
                                                            Math.Abs(Convert.ToDecimal(dr["MAKINGAMOUNT"])) -
                                                            Math.Abs(Convert.ToDecimal(dr["WastageAmount"]));

                                    dr["MAKINGRATE"] = 0;
                                    dr["MAKINGTYPE"] = 0;
                                    dr["MAKINGDISCOUNT"] = 0;
                                    dr["MAKINGAMOUNT"] = 0;
                                    dr["MAKINGDISCTYPE"] = 0;
                                    dr["MakingDiscountAmount"] = 0;

                                    dr["WastageType"] = 0;
                                    dr["WastageQty"] = 0;
                                    dr["WastageAmount"] = 0;
                                    dr["WastagePercentage"] = 0;
                                    dr["WastageRate"] = 0;

                                    dtRec.AcceptChanges();
                                }

                            }
                            else
                            {

                                int iMetalType = getMetalType(item.ItemId);
                                if((iMetalType == (int)Enums.EnumClass.MetalType.LooseDmd)
                                    || (iMetalType == (int)Enums.EnumClass.MetalType.Stone))
                                {
                                    #region

                                    decimal dExchangePercent = 0m;
                                    decimal dBuyBackPercent = 0m;
                                    decimal dStoneWtRange = 0m;

                                    decimal dStoneRate = 0m;
                                    decimal dDeductionAMt = 0m;

                                    int iCalcType = 0;
                                    decimal dNewCValue = 0m;

                                    if(Convert.ToDecimal(dr["PCS"]) > 0)
                                    {
                                        dStoneWtRange = Convert.ToDecimal(dr["QTY"]) / Convert.ToDecimal(dr["PCS"]);
                                    }
                                    else
                                    {
                                        dStoneWtRange = Convert.ToDecimal(dr["QTY"]);
                                    }

                                    dStoneRate = GetIngredientInfo(ref dExchangePercent, ref dBuyBackPercent, ref iCalcType,
                                                                    originalTransaction.StoreId,
                                        // Convert.ToString(drIng["ItemID"]),
                                        // Convert.ToString(drIng["InventSizeID"]), 
                                        // Convert.ToString(drIng["InventColorID"]),
                                                                   item.ItemId,
                                                                   item.Dimension.SizeId,
                                                                   item.Dimension.ColorId,
                                                                   dStoneWtRange, dtDate);
                                    if(isExchange)
                                    {
                                        //dDeductionAMt = ((dExchangePercent * Convert.ToDecimal(dr["AMOUNT"])) / 100);
                                        dDeductionAMt = ((dExchangePercent * dStoneRate) / 100);
                                    }
                                    else
                                    {
                                        // dDeductionAMt = ((dBuyBackPercent * Convert.ToDecimal(dr["AMOUNT"])) / 100);
                                        dDeductionAMt = ((dBuyBackPercent * dStoneRate) / 100);
                                    }

                                    if(iCalcType == (int)Enums.EnumClass.CalcType.Weight)
                                    {
                                        dNewCValue = (dStoneRate * Math.Abs(Convert.ToDecimal(dr["QUANTITY"]))) - (dDeductionAMt * Math.Abs(Convert.ToDecimal(dr["QUANTITY"])));

                                    }
                                    else if(iCalcType == (int)Enums.EnumClass.CalcType.Pieces)
                                    {
                                        dNewCValue = (dStoneRate * Math.Abs(Convert.ToDecimal(dr["PIECES"]))) - (dDeductionAMt * Math.Abs(Convert.ToDecimal(dr["PIECES"])));
                                    }
                                    else
                                    {

                                        dNewCValue = dStoneRate - dDeductionAMt;
                                    }

                                    dr["RATE"] = dStoneRate;
                                    dr["RATETYPE"] = iCalcType;
                                    dr["AMOUNT"] = dNewCValue;
                                    dr["TOTALAMOUNT"] = dNewCValue;

                                    dtRec.AcceptChanges();

                                    #endregion
                                }

                            }

                            #endregion

                            item.Price = (Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"])));
                        }
                        //  if (Convert.ToString(item.LineId) == Convert.ToString(Convert.ToInt16(dr["LINENUM"]))) 
                        if(Convert.ToDecimal(item.LineId) == Convert.ToDecimal(dr["LINENUM"]))
                        {

                            item.PartnerData.SpecialDiscInfo = sSplReturn;

                            item.PartnerData.Pieces = Convert.ToString(dr["PIECES"]);
                            item.PartnerData.Quantity = Convert.ToString(dr["QUANTITY"]);
                            item.PartnerData.Rate = Convert.ToString(dr["RATE"]);
                            item.PartnerData.RateType = Convert.ToString(dr["RATETYPE"]);
                            item.PartnerData.MakingRate = Convert.ToString(dr["MAKINGRATE"]); //
                            item.PartnerData.MakingType = Convert.ToString(dr["MAKINGTYPE"]); //
                            item.PartnerData.Amount = Convert.ToString(dr["AMOUNT"]);
                            item.PartnerData.MakingDisc = Convert.ToString(dr["MAKINGDISCOUNT"]); //
                            item.PartnerData.MakingAmount = Convert.ToString(dr["MAKINGAMOUNT"]); //
                            item.PartnerData.TotalAmount = Convert.ToString(dr["TOTALAMOUNT"]);
                            item.PartnerData.TotalWeight = Convert.ToString(dr["TOTALWEIGHT"]);
                            item.PartnerData.LossPct = Convert.ToString(dr["LOSSPERCENTAGE"]);
                            item.PartnerData.LossWeight = Convert.ToString(dr["LOSSWEIGHT"]);
                            item.PartnerData.ExpectedQuantity = Convert.ToString(dr["EXPECTEDQUANTITY"]);
                            item.PartnerData.TransactionType = Convert.ToString(dr["TRANSACTIONTYPE"]);
                            // item.PartnerData.OChecked = Convert.ToString(dr["OWN"]);
                            item.PartnerData.OChecked = false;

                            if(Convert.ToInt32(dr["IsIngredient"]) == 1)
                                item.PartnerData.Ingredients = Convert.ToString(dr["SKUDETAILS"]); //
                            else
                                item.PartnerData.Ingredients = "";
                            item.PartnerData.OrderNum = Convert.ToString(dr["ORDERNUM"]);
                            item.PartnerData.OrderLineNum = Convert.ToString(dr["ORDERLINENUM"]);
                            item.PartnerData.CustNo = Convert.ToString(dr["CUSTNO"]);
                            item.PartnerData.SampleReturn = false;

                            item.PartnerData.WastageType = Convert.ToString(dr["WastageType"]);
                            item.PartnerData.WastageQty = Convert.ToString(dr["WastageQty"]);
                            item.PartnerData.WastageAmount = Convert.ToString(dr["WastageAmount"]);
                            item.PartnerData.WastagePercentage = Convert.ToString(dr["WastagePercentage"]);
                            item.PartnerData.WastageRate = Convert.ToString(dr["WastageRate"]);

                            item.PartnerData.MakingDiscountType = Convert.ToString(dr["MAKINGDISCTYPE"]);
                            item.PartnerData.MakingTotalDiscount = Convert.ToString(dr["MakingDiscountAmount"]);
                            item.PartnerData.ConfigId = Convert.ToString(dr["CONFIGID"]);

                            item.PartnerData.Purity = "0";
                            item.PartnerData.GROSSWT = "0";
                            item.PartnerData.GROSSUNIT = string.Empty;
                            item.PartnerData.DMDWT = "0";
                            item.PartnerData.DMDPCS = "0";
                            item.PartnerData.DMDUNIT = string.Empty;
                            item.PartnerData.DMDAMOUNT = "0";
                            item.PartnerData.STONEWT = "0";
                            item.PartnerData.STONEPCS = "0";
                            item.PartnerData.STONEUNIT = string.Empty;
                            item.PartnerData.STONEAMOUNT = "0";
                            item.PartnerData.NETWT = "0";
                            item.PartnerData.NETRATE = "0";
                            item.PartnerData.NETUNIT = string.Empty;
                            item.PartnerData.NETPURITY = "0";
                            item.PartnerData.NETAMOUNT = "0";
                            item.PartnerData.OGREFINVOICENO = string.Empty;
                            item.PartnerData.REMARKS = "";
                            break;
                        }
                    }
                }
            }
            else
            {
                foreach(var item in lineitem)
                {
                    item.PartnerData.Pieces = "";
                    item.PartnerData.Quantity = "";
                    item.PartnerData.Rate = "";
                    item.PartnerData.RateType = "";
                    item.PartnerData.MakingRate = "";
                    item.PartnerData.MakingType = "";
                    item.PartnerData.Amount = "";
                    item.PartnerData.MakingDisc = "";
                    item.PartnerData.MakingAmount = "";
                    item.PartnerData.TotalAmount = "";
                    item.PartnerData.TotalWeight = "";
                    item.PartnerData.LossPct = "";
                    item.PartnerData.LossWeight = "";
                    item.PartnerData.ExpectedQuantity = "";
                    item.PartnerData.TransactionType = "";
                    //  item.PartnerData.OChecked = ""; 
                    item.PartnerData.OChecked = false;
                    item.PartnerData.Ingredients = "";
                    item.PartnerData.SampleReturn = false;

                    item.PartnerData.WastageType = "0";
                    item.PartnerData.WastageQty = "0";
                    item.PartnerData.WastageAmount = "";

                    item.PartnerData.WastagePercentage = "0";
                    item.PartnerData.WastageRate = "0";

                    item.PartnerData.MakingDiscountType = "0";
                    item.PartnerData.MakingTotalDiscount = "0";
                    item.PartnerData.ConfigId = string.Empty;
                    item.PartnerData.Purity = "0";
                    item.PartnerData.GROSSWT = "0";
                    item.PartnerData.GROSSUNIT = string.Empty;
                    item.PartnerData.DMDWT = "0";
                    item.PartnerData.DMDPCS = "0";
                    item.PartnerData.DMDUNIT = string.Empty;
                    item.PartnerData.DMDAMOUNT = "0";
                    item.PartnerData.STONEWT = "0";
                    item.PartnerData.STONEPCS = "0";
                    item.PartnerData.STONEUNIT = string.Empty;
                    item.PartnerData.STONEAMOUNT = "0";
                    item.PartnerData.NETWT = "0";
                    item.PartnerData.NETRATE = "0";
                    item.PartnerData.NETUNIT = string.Empty;
                    item.PartnerData.NETPURITY = "0";
                    item.PartnerData.NETAMOUNT = "0";
                    item.PartnerData.OGREFINVOICENO = string.Empty;
                    item.PartnerData.SpecialDiscInfo = string.Empty;
                    item.PartnerData.REMARKS = "";
                }
            }

            #endregion
            LSRetailPosis.ApplicationLog.Log("TransactionTriggers.PreReturnTransaction", "Before returning the transaction...", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PostReturnTransaction(IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("TransactionTriggers.PostReturnTransaction", "After returning the transaction...", LSRetailPosis.LogTraceLevel.Trace);
        }

        #region Changed By Nimbus
        private void CheckInventory(IPosTransaction posTransaction, int tType)
        {
            RetailTransaction retailTrans = posTransaction as RetailTransaction;
            if(retailTrans != null)
            {
                foreach(SaleLineItem saleLineItem in retailTrans.SaleItems)
                {
                    if(!saleLineItem.Voided)
                    {
                        string commandtext = "  " +
                            //  " UPDATE SKUTable_Posted SET isLocked='False' WHERE SkuNumber='" + saleLineItem.ItemId + "' " +
                                  " UPDATE SKUTableTrans SET isLocked='False' WHERE SkuNumber='" + saleLineItem.ItemId + "' " + //SKU Table New
                            //" AND STOREID='" + ApplicationSettings.Terminal.StoreId + "' AND TERMINALID='" + ApplicationSettings.Terminal.TerminalId + "' " +
                                  " AND DATAAREAID='" + application.Settings.Database.DataAreaID + "'  " +
                                  " AND isAvailable='True' ";

                        SqlConnection connection = new SqlConnection();
                        if(application != null)
                            connection = application.Settings.Database.Connection;
                        else
                            connection = ApplicationSettings.Database.LocalConnection;
                        if(connection.State == ConnectionState.Closed)
                            connection.Open();
                        SqlCommand command = new SqlCommand(commandtext, connection);
                        command.CommandTimeout = 0;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        #region [Exchange / Buy Back]
        // private string getGoldRate(string sItemId, string sConfigId, int iRateType, string StoreID)   
        private decimal getGoldRate(string sItemId, string sConfigId, int iRateType, string StoreID)
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) ");
            // commandText.Append(" DECLARE @METALTYPE INT ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM     RETAILCHANNELTABLE INNER JOIN ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + StoreID.Trim() + "'  ");

            //  commandText.Append(" SELECT @METALTYPE=METALTYPE FROM [INVENTTABLE] WHERE ITEMID ='" + sItemId.Trim() + "' ");

            //commandText.Append(" IF(@METALTYPE IN ('" + (int)MetalType.Gold + "','" + (int)MetalType.Silver + "','" + (int)MetalType.Platinum + "','" + (int)MetalType.Palladium + "')) ");
            //commandText.Append(" BEGIN ");

            commandText.Append(" SELECT TOP 1 CAST(ISNULL(RATES,0) AS decimal (6,2)) FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION AND METALTYPE = ");
            commandText.Append(1);

            // commandText.Append(" AND METALTYPE=@METALTYPE ");

            // commandText.Append(" AND RETAIL=1 AND RATETYPE='" + (int)RateTypeNew.Sale + "' ");
            if(iRateType == 3)
            {
                commandText.Append(" AND RETAIL=1 AND RATETYPE= ");
            }
            else
            {
                commandText.Append(" AND RATETYPE= ");
            }
            commandText.Append(iRateType);

            commandText.Append(" AND ACTIVE=1 AND CONFIGIDSTANDARD='" + sConfigId.Trim() + "' ");
            commandText.Append("   ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC ");

            //if (conn.State == ConnectionState.Closed)
            //    conn.Open();

            SqlConnection connection = new SqlConnection();
            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if(connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;

            decimal dGoldRate = Convert.ToDecimal(command.ExecuteScalar());

            if(connection.State == ConnectionState.Open)
                connection.Close();

            return dGoldRate;

        }

        private decimal GetIngredientInfo(ref decimal ExchangePercent, ref decimal BuyBackPercent, ref int CalcType,
                                           string StoreID,
                                          string ItemID, string SizeID, string ColorID, decimal dStoneWtRange, DateTime dtTransdate)
        {

            decimal dStoneRate = 0m;
            StringBuilder commandText = new StringBuilder();
            commandText.Append(" SET DATEFORMAT DMY;   DECLARE @INVENTLOCATION VARCHAR(20) ");
            commandText.Append(" DECLARE @METALTYPE INT ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM         RETAILCHANNELTABLE INNER JOIN ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + StoreID.Trim() + "'  ");

            //commandText.Append(" SELECT @METALTYPE=METALTYPE FROM [INVENTTABLE] WHERE ITEMID='" + ItemID.Trim() + "' ");

            //commandText.Append(" IF(@METALTYPE IN ('" + (int)MetalType.LooseDmd + "','" + (int)MetalType.Stone + "')) ");
            //commandText.Append(" BEGIN ");


            // commandText.Append(" SELECT     ISNULL(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.CALCTYPE,0) AS CALCTYPE,ISNULL(DISCTYPE,0) AS DISCTYPE, ISNULL(DISCAMT,0) AS DISCAMT ");

            commandText.Append(" SELECT TOP 1 CAST(ISNULL(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.UNIT_RATE,0) AS numeric(28, 2)) ");
            commandText.Append(" ,ISNULL(BUYBACKDEDUCTPCT,0) AS BUYBACKDEDUCTPCT, ISNULL(EXCHANGEDEDUCTPCT,0) AS EXCHANGEDEDUCTPCT,ISNULL(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.CALCTYPE,0) AS CALCTYPE ");

            commandText.Append(" FROM    RETAILCUSTOMERSTONEAGGREEMENTDETAIL INNER JOIN ");
            commandText.Append(" INVENTDIM ON RETAILCUSTOMERSTONEAGGREEMENTDETAIL.INVENTDIMID = INVENTDIM.INVENTDIMID ");
            //  commandText.Append(" WHERE     (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE = @INVENTLOCATION) AND ('" + grweigh + "' BETWEEN RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FROM_WEIGHT AND  ");
            commandText.Append(" WHERE     (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE = @INVENTLOCATION) AND (  ");
            commandText.Append(dStoneWtRange);
            commandText.Append(" BETWEEN RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FROM_WEIGHT AND  ");
            //  commandText.Append("  RETAILCUSTOMERSTONEAGGREEMENTDETAIL.TO_WEIGHT) AND (DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0) BETWEEN  ");
            // based on transaction date
            // commandText.Append("  RETAILCUSTOMERSTONEAGGREEMENTDETAIL.TO_WEIGHT) AND (DATEADD(dd, DATEDIFF(dd, 0, CAST('" + dtTransdate + "' AS DATETIME)), 0) BETWEEN  "); //dtTransdate
            // fetching diamond as per current rate
            commandText.Append("  RETAILCUSTOMERSTONEAGGREEMENTDETAIL.TO_WEIGHT) AND (DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0) BETWEEN  "); //dtTransdate
            commandText.Append(" RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ToDate) AND  ");
            commandText.Append("  (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID = '" + ItemID.Trim() + "')  AND  ");
            commandText.Append("  (INVENTDIM.INVENTSIZEID = '" + SizeID.Trim() + "') AND (INVENTDIM.INVENTCOLORID = '" + ColorID.Trim() + "') ");

            // commandText.Append(" AND (INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "')");
            commandText.Append(" AND (INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "') AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ACTIVATE = 1"); // modified on 02.09.2013
            //  commandText.Append(" ORDER BY RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate DESC");
            commandText.Append(" ORDER BY RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID DESC, RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate DESC");  // Changed order sequence on 03.06.2013 as per u.das

            //commandText.Append(" END ");
            // }
            //   commandText.Append("AND CAST(cast(([TIME] / 3600) as varchar(10)) + ':' + cast(([TIME] % 60) as varchar(10)) AS TIME)<=CAST(CONVERT(VARCHAR(8),GETDATE(),108) AS TIME)  ");

            //if (conn.State == ConnectionState.Closed)
            //    conn.Open();

            SqlConnection connection = new SqlConnection();
            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if(connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();

            if(reader.HasRows)
            {
                while(reader.Read())
                {
                    dStoneRate = Convert.ToDecimal(reader.GetValue(0));
                    BuyBackPercent = Convert.ToDecimal(reader.GetValue(1));
                    ExchangePercent = Convert.ToDecimal(reader.GetValue(2));
                    CalcType = Convert.ToInt32(reader.GetValue(3));
                    //iSDisctype = Convert.ToInt32(reader.GetValue(1));
                    //dSDiscAmt = Convert.ToDecimal(reader.GetValue(2));
                }
            }

            if(connection.State == ConnectionState.Open)
                connection.Close();
            return dStoneRate;
        }

        private int getMetalType(string sItemId)
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT ISNULL(METALTYPE,0) FROM INVENTTABLE WHERE ITEMID = '" + sItemId + "' AND DATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "'  ");

            SqlConnection connection = new SqlConnection();
            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if(connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;

            int iMetalType = Convert.ToInt32(command.ExecuteScalar());

            if(connection.State == ConnectionState.Open)
                connection.Close();

            return iMetalType;

        }


        #endregion

        private DataTable GetBookedInfo(string sOrderNo, SqlTransaction sqlTransaction)  // SKU allow
        {
            DataTable dt = new DataTable();
            string commandText = "SELECT ITEMID FROM CUSTORDER_DETAILS WHERE ORDERNUM = '" + sOrderNo + "' AND ISNULL(IsBookedSKU,0) = 1 "
                                 ;

            using(SqlCommand command = new SqlCommand(commandText, sqlTransaction.Connection, sqlTransaction))
            {
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(dt);
            }

            return dt;
        }

        private void SaveBookedSKU(IPosTransaction POSTrans, string sOrderNo, string sSKUNo, SqlTransaction sqlTransaction)
        {
            LSRetailPosis.Transaction.CustomerPaymentTransaction PayTrans = POSTrans as LSRetailPosis.Transaction.CustomerPaymentTransaction;

            string commandText = " IF NOT EXISTS(SELECT TOP 1 * FROM RETAILCUSTOMERDEPOSITSKUDETAILS " +
                                   " WHERE TRANSID=@TRANSID " +
                                   " AND SKUNUMBER=@SKUNUMBER) BEGIN  " +
                                   " INSERT INTO [RETAILCUSTOMERDEPOSITSKUDETAILS] " +
                                   " ([TRANSID],[CUSTOMERID],[ORDERNUMBER],[SKUNUMBER],[SKUBOOKINGDATE],[SKURELEASEDDATE] " +
                                   " ,[SKUSALEDATE],[DELIVERED],[STOREID],[TERMINALID],[DATAAREAID],[STAFFID]) " +
                                   "  VALUES " +
                                   " (@TRANSID,@CUSTOMERID,@ORDERNUMBER,@SKUNUMBER,@SKUBOOKINGDATE,@SKURELEASEDDATE,@SKUSALEDATE " +
                                   " ,@DELIVERED,@STOREID,@TERMINALID,@DATAAREAID,@STAFFID) END ";

            using(SqlCommand command = new SqlCommand(commandText, sqlTransaction.Connection, sqlTransaction))
            {
                command.Parameters.Add("@TRANSID", SqlDbType.NVarChar, 20).Value = Convert.ToString(PayTrans.TransactionId);
                command.Parameters.Add("@CUSTOMERID", SqlDbType.NVarChar, 20).Value = Convert.ToString(PayTrans.Customer.CustomerId);
                command.Parameters.Add("@ORDERNUMBER", SqlDbType.NVarChar, 20).Value = sOrderNo;
                command.Parameters.Add("@SKUNUMBER", SqlDbType.NVarChar, 20).Value = sSKUNo;
                command.Parameters.Add("@SKUBOOKINGDATE", SqlDbType.Date).Value = Convert.ToDateTime(DateTime.Now).Date;
                command.Parameters.Add("@SKURELEASEDDATE", SqlDbType.DateTime, 20).Value = Convert.ToDateTime("1/1/1900 12:00:00 AM");
                command.Parameters.Add("@SKUSALEDATE", SqlDbType.DateTime, 60).Value = Convert.ToDateTime("1/1/1900 12:00:00 AM");
                command.Parameters.Add("@DELIVERED", SqlDbType.Bit).Value = false;
                command.Parameters.Add("@STOREID", SqlDbType.NVarChar, 20).Value = ApplicationSettings.Terminal.StoreId;
                command.Parameters.Add("@TERMINALID", SqlDbType.NVarChar, 20).Value = ApplicationSettings.Terminal.TerminalId;
                command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 20).Value = ApplicationSettings.Database.DATAAREAID;

                command.Parameters.Add("@STAFFID", SqlDbType.NVarChar, 20).Value = Convert.ToString(PayTrans.OperatorId);

                command.ExecuteNonQuery();
            }

        }
        #endregion

        private DataTable GetCardIfo(string sTransactionId, SqlTransaction sqlTransaction = null)
        {
            string sTblName = "EXTNDCARDINFO" + ApplicationSettings.Terminal.TerminalId;

            DataTable dtCard = new DataTable();
            SqlConnection conn = new SqlConnection();
            if(application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTblName + "')");
            // commandText.Append(" BEGIN  SELECT ISNULL(TRANSACTIONID,'') AS TRANSACTIONID, ISNULL(CARDNO,'') AS CARDNO, ISNULL(APPROVALCODE,'') AS APPROVALCODE FROM " + sTblName + " END");

            commandText.Append(" BEGIN  SELECT ISNULL(CARDNO,'') AS CARDNO, ISNULL(APPROVALCODE,'') AS APPROVALCODE,ISNULL(CARDEXPIRYMONTH,'') AS CARDEXPIRYMONTH, ");
            commandText.Append(" ISNULL(CARDEXPIRYYEAR,'') AS CARDEXPIRYYEAR FROM " + sTblName + " WHERE TRANSACTIONID = '" + sTransactionId + "' END");

            if(conn.State == ConnectionState.Closed)
                conn.Open();

            using(SqlCommand command = new SqlCommand(commandText.ToString(), conn))
            {
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(dtCard);
            }

            return dtCard;
        }

        private int GetFullExchngValidDay(string sStoreId)
        {
            int iValidDay = 0;

            SqlConnection connection = new SqlConnection();

            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "SELECT ISNULL(FULLEXCHANGEVALIDDAY,0) FROM RETAILSTORETABLE WHERE STORENUMBER = '" + sStoreId + "' ";


            if(connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            iValidDay = Convert.ToInt32(command.ExecuteScalar());
            if(connection.State == ConnectionState.Open)
                connection.Close();

            return iValidDay;
        }

        private decimal GetMaxInvoiceAmount(SqlTransaction sqlTransaction) // added on 23/05/2014 for five PAN  if invoiceamount > 20000000(example) then PAN enter.
        {
            string commandText = "SELECT ISNULL(MAXINVOICEAMOUNT,0) FROM RETAILPARAMETERS";

            SqlCommand command = new SqlCommand(commandText.ToString(), sqlTransaction.Connection, sqlTransaction);
            command.CommandTimeout = 0;

            decimal dAmount = Convert.ToDecimal(command.ExecuteScalar());
            return dAmount;
        }

        private DataTable GetAdvDetailInfo(string sStringSql)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = sStringSql;

                DataTable CustAdvDt = new DataTable();

                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(CustAdvDt);
                return CustAdvDt;

            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        private int getUserDiscountPermissionId()
        {
            SqlConnection connection = new SqlConnection();
            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("select RETAILDISCPERMISSION from RETAILSTAFFTABLE where STAFFID='" + ApplicationSettings.Terminal.TerminalOperator.OperatorId + "'");

            if(connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToInt16(cmd.ExecuteScalar());
        }

        private void VoidRepairReturn(string sRepairId, string sRefReturnId)
        {
            #region //delete record from RETAILREPAIRRETURNTRANS table and update in RetailRepairReturnHdr table
            using(SqlConnection conn = (Application != null) ? Application.Settings.Database.Connection : ApplicationSettings.Database.LocalConnection)
            {
                try
                {
                    StringBuilder commandText = new StringBuilder();
                    commandText.AppendLine(" DELETE FROM RETAILREPAIRRETURNTRANS WHERE DATAAREAID='" + Application.Settings.Database.DataAreaID
                                                + "' AND RetailStoreId='" + ApplicationSettings.Terminal.StoreId + "' AND RetailTerminalId='"
                                                + ApplicationSettings.Terminal.TerminalId + "' AND RepairReturnId='" + sRefReturnId + "'");// retailtrans.PartnerData.REPAIRRETURNID
                    commandText.AppendLine();
                    commandText.AppendLine(" UPDATE RetailRepairReturnHdr SET IsDelivered = 0 WHERE RepairId ='" + sRepairId + "'; ");

                    if(conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    using(SqlCommand cmd = new SqlCommand(commandText.ToString(), conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                catch { }
                finally
                {
                    if(conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }
            //string commandtext = "  " +
            //          " update RetailRepairReturnHdr set IsDelivered=0 where RepairId='" + sRepairId + "' " +
            //          " AND RetailStoreId='" + ApplicationSettings.Terminal.StoreId + "'" +
            //          " AND RetailTerminalId='" + ApplicationSettings.Terminal.TerminalId + "' " +
            //          " AND DATAAREAID='" + application.Settings.Database.DataAreaID + "'";


            //SqlConnection connection = new SqlConnection();
            //if (application != null)
            //    connection = application.Settings.Database.Connection;
            //else
            //    connection = ApplicationSettings.Database.LocalConnection;
            //if (connection.State == ConnectionState.Closed)
            //    connection.Open();
            //SqlCommand command = new SqlCommand(commandtext, connection);
            //command.CommandTimeout = 0;
            //command.ExecuteNonQuery();
            #endregion

        }


    }
}
