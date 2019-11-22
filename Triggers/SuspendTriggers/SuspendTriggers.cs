//Microsoft Dynamics AX for Retail POS Plug-ins 
//The following project is provided as SAMPLE code. Because this software is "as is," we may not provide support services for it.

using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Triggers;
using Microsoft.Dynamics.Retail.Pos.Printing;
using LSRetailPosis.Transaction;
using System.Collections.Generic;
using LSRetailPosis.Settings;
using System.Drawing;

using System.Data.SqlClient;
using System.Data;
using LSRetailPosis.Transaction.Line.SaleItem;
using System;
using System.IO;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.Report;
//using Microsoft.CSharp.RuntimeBinder;

namespace Microsoft.Dynamics.Retail.Pos.SuspendTriggers
{
    [Export(typeof(ISuspendTrigger))]
    public class SuspendTriggers : ISuspendTrigger
    {

        #region Constructor - Destructor

        public SuspendTriggers()
        {
            
            // Get all text through the Translation function in the ApplicationLocalizer
            // TextID's for SuspendTriggers are reserved at 62000 - 62999

        }

        #endregion

        #region ISuspendTriggers Members

        public void OnSuspendTransaction(IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("SuspendTriggers.OnSuspendTransaction", "On the suspension of a transaction...", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void OnRecallTransaction(IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("SuspendTriggers.OnRecallTransaction", "On the recall of a suspended transaction...", LSRetailPosis.LogTraceLevel.Trace);
        }

        #endregion

        #region ISuspendTriggers Members

        public void PreSuspendTransaction(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction)
        {
            System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem> saleline
                = new System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem>(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).SaleItems);
            bool isServiceItem = false;
            foreach (var sale in saleline)
            {
                if (sale.ItemType == LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service && !sale.Voided)
                {
                    isServiceItem = true;
                    break;
                }
            }
            if (isServiceItem)
            {
                preTriggerResult.MessageId = 62999;
                preTriggerResult.ContinueOperation = false;
                return;
            }
            LSRetailPosis.ApplicationLog.Log("SuspendTriggers.PreSuspendTransaction", "Prior to the suspension of a transaction...", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PostSuspendTransaction(IPosTransaction posTransaction)
        {
            decimal dNetWt = 0;
            #region
            RetailTransaction retailTrans = posTransaction as RetailTransaction;
            if (retailTrans != null)
            {
                DataTable dtDetail = new DataTable("Detail");
                DataTable dtIngredient = new DataTable("Ingredient");
                DataRow drDtl;
                DataRow drIngrd;

                dtDetail.Columns.Add("ITEMID", typeof(string));
                dtDetail.Columns.Add("LINENUM", typeof(int));
                dtDetail.Columns.Add("MAKINGAMOUNT", typeof(decimal));
                dtDetail.Columns.Add("MakingDisc", typeof(decimal)); 
                dtDetail.Columns.Add("WastageAmount", typeof(decimal));
                dtDetail.AcceptChanges();

                dtIngredient.Columns.Add("SKUNUMBER", typeof(string)); 
                dtIngredient.Columns.Add("ITEMID", typeof(string));
                dtIngredient.Columns.Add("LINENUM", typeof(int));
                dtIngredient.Columns.Add("REFLINENUM", typeof(int));

                dtIngredient.Columns.Add("InventSizeID", typeof(string));
                dtIngredient.Columns.Add("InventColorID", typeof(string));
                dtIngredient.Columns.Add("ConfigID", typeof(string));

                dtIngredient.Columns.Add("UnitID", typeof(string));
                dtIngredient.Columns.Add("METALTYPE", typeof(int));

                dtIngredient.Columns.Add("QTY", typeof(decimal));
                dtIngredient.Columns.Add("PCS", typeof(decimal));
                dtIngredient.Columns.Add("CRATE", typeof(decimal));
                dtIngredient.Columns.Add("CVALUE", typeof(decimal));
                dtIngredient.Columns.Add("INGRDDISCAMT", typeof(decimal));
                dtIngredient.AcceptChanges();

                int i = 1;
                foreach (SaleLineItem saleLineItem in retailTrans.SaleItems)
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

                    drDtl = dtDetail.NewRow();

                    drDtl["ITEMID"] = saleLineItem.ItemId;
                    drDtl["LINENUM"] = i;
                    drDtl["MAKINGAMOUNT"] = decimal.Round(Convert.ToDecimal(saleLineItem.PartnerData.MakingAmount),2,MidpointRounding.AwayFromZero);
                    drDtl["MakingDisc"] = decimal.Round(Convert.ToDecimal(saleLineItem.PartnerData.MakingTotalDiscount),2,MidpointRounding.AwayFromZero);
                    drDtl["WastageAmount"] = decimal.Round(Convert.ToDecimal(saleLineItem.PartnerData.WastageAmount),2,MidpointRounding.AwayFromZero);

                    dtDetail.Rows.Add(drDtl);
                    dtDetail.AcceptChanges();
                   
                    if (dsIngredients != null && dsIngredients.Tables[0].Rows.Count > 0)
                    {
                        int iGrd = 1;

                        foreach (DataRow dr in dsIngredients.Tables[0].Rows)
                        {
                            drIngrd = dtIngredient.NewRow();

                            drIngrd["SKUNUMBER"] = Convert.ToString(dr["SKUNUMBER"]);
                            drIngrd["ITEMID"] = Convert.ToString(dr["ITEMID"]);
                            drIngrd["LINENUM"] = iGrd;

                            drIngrd["REFLINENUM"] = i;
                            drIngrd["InventSizeID"] = Convert.ToString(dr["InventSizeID"]);
                            drIngrd["InventColorID"] = Convert.ToString(dr["InventColorID"]);
                            drIngrd["ConfigID"] = Convert.ToString(dr["ConfigID"]);

                            drIngrd["UnitID"] = Convert.ToString(dr["UnitID"]);
                            drIngrd["METALTYPE"] = Convert.ToInt32(dr["METALTYPE"]);
                            
                            if(iGrd==1)
                            dNetWt = decimal.Round(Convert.ToDecimal(dr["QTY"]), 3, MidpointRounding.AwayFromZero); // added on 24/03/2014 R.Hossain

                            drIngrd["QTY"] = decimal.Round(Convert.ToDecimal(dr["QTY"]), 3, MidpointRounding.AwayFromZero);
                            drIngrd["PCS"] = decimal.Round(Convert.ToDecimal(dr["PCS"]),3,MidpointRounding.AwayFromZero);
                            drIngrd["CRATE"] = decimal.Round(Convert.ToDecimal(dr["RATE"]),2,MidpointRounding.AwayFromZero);
                            if (Convert.ToBoolean(saleLineItem.PartnerData.isMRP) == true) // added on 31/03/2014 and mod on 25/04/14
                                if (iGrd == 1) // added on 08/07/2014-- for esteemate of mrp item-- bom value should not come
                                drIngrd["CVALUE"] = decimal.Round(Convert.ToDecimal(retailTrans.NetAmountWithNoTax), 2, MidpointRounding.AwayFromZero);
                            else
                                drIngrd["CVALUE"] = decimal.Round(Convert.ToDecimal(dr["CVALUE"]),2,MidpointRounding.AwayFromZero);
                            drIngrd["INGRDDISCAMT"] = decimal.Round(Convert.ToDecimal(dr["IngrdDiscTotAmt"]), 2, MidpointRounding.AwayFromZero);

                            dtIngredient.Rows.Add(drIngrd);
                            dtIngredient.AcceptChanges();

                            iGrd++;
                        }

                    }

                    i++;
                }

                if ((dtDetail != null && dtDetail.Rows.Count > 0)
                    && (dtIngredient != null && dtIngredient.Rows.Count > 0))
                {
                    DataRow drLIngrd;

                    foreach (DataRow dr in dtDetail.Rows)
                    {
                        drLIngrd = dtIngredient.NewRow();
                        drLIngrd["SKUNUMBER"] = Convert.ToString(dr["ITEMID"]);
                        drLIngrd["ITEMID"] = "Labour";
                        drLIngrd["LINENUM"] = dtIngredient.Rows.Count + 1; 

                        drLIngrd["REFLINENUM"] = Convert.ToInt32(dr["LINENUM"]); 
                        drLIngrd["InventSizeID"] = string.Empty;  
                        drLIngrd["InventColorID"] = string.Empty; 
                        drLIngrd["ConfigID"] = string.Empty; 

                        drLIngrd["UnitID"] = string.Empty; 
                        drLIngrd["METALTYPE"] = 0; 

                        drLIngrd["QTY"] = 0;
                        drLIngrd["PCS"] = 0;
                        drLIngrd["CRATE"] = Convert.ToString(decimal.Round((Convert.ToDecimal(dr["MAKINGAMOUNT"]) + Convert.ToDecimal(dr["WastageAmount"])) / Convert.ToDecimal(dNetWt), 2, MidpointRounding.AwayFromZero));  
                        drLIngrd["CVALUE"] = Convert.ToDecimal(dr["MAKINGAMOUNT"]) + Convert.ToDecimal(dr["WastageAmount"]);
                        drLIngrd["INGRDDISCAMT"] = Convert.ToDecimal(dr["MakingDisc"]);
                        dtIngredient.Rows.Add(drLIngrd);
                        dtIngredient.AcceptChanges();
                    }

                }

                if ((dtDetail != null && dtDetail.Rows.Count > 0)
                    && (dtIngredient != null && dtIngredient.Rows.Count > 0))
                {

                    frmEstimationReport objEstimationReport = new frmEstimationReport(posTransaction, dtDetail, dtIngredient);
                    objEstimationReport.ShowDialog();
                }
                else
                {
                    #region Nimbus
                    FormModulation formMod = new FormModulation(ApplicationSettings.Database.LocalConnection);
                    RetailTransaction retailTransaction = posTransaction as RetailTransaction;
                    if (retailTransaction != null)
                    {
                        ICollection<Point> signaturePoints = null;
                        if (retailTransaction.TenderLines != null
                            && retailTransaction.TenderLines.Count > 0
                            && retailTransaction.TenderLines.First.Value != null)
                        {
                            signaturePoints = retailTransaction.TenderLines.First.Value.SignatureData;
                        }
                        FormInfo formInfo = formMod.GetInfoForForm(Microsoft.Dynamics.Retail.Pos.Contracts.Services.FormType.QuotationReceipt, false, LSRetailPosis.Settings.HardwareProfiles.Printer.ReceiptProfileId);
                        formMod.GetTransformedTransaction(formInfo, retailTransaction);

                        string textForPreview = formInfo.Header;
                        textForPreview += formInfo.Details;
                        textForPreview += formInfo.Footer;
                        textForPreview = textForPreview.Replace("|1C", string.Empty);
                        textForPreview = textForPreview.Replace("|2C", string.Empty);
                        frmReportList preview = new frmReportList(textForPreview, signaturePoints);
                        LSRetailPosis.POSControls.POSFormsManager.ShowPOSForm(preview);
                    }
                    #endregion

                }

            }

            #endregion


            LSRetailPosis.ApplicationLog.Log("SuspendTriggers.PostSuspendTransaction", "After the suspension of a transaction...", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PreRecallTransaction(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("SuspendTriggers.PreRecallTransaction", "Prior to the recall of a transaction from suspension...", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PostRecallTransaction(IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("SuspendTriggers.PostRecallTransaction", "After the recall of a transaction from suspension...", LSRetailPosis.LogTraceLevel.Trace);
        }

        #endregion

    }
}
