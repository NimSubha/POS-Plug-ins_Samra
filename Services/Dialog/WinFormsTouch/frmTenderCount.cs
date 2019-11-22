/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraGrid;
using LSRetailPosis;
using LSRetailPosis.DataAccess;
using LSRetailPosis.POSProcesses;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.TenderItem;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.UI;
using Microsoft.Dynamics.Retail.Pos.DataManager;
using TypeOfTransaction = LSRetailPosis.Transaction.PosTransaction.TypeOfTransaction;

namespace Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch
{
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Grandfathered")]
    partial class frmTenderCount : frmTouchBase
    {
        // Which tenders are we counting....?
        private DataTable tendersToCount;
        //which amounts per tendertype (wher counting = required) does the POS expect?
        private DataTable requiredAmounts;
        private DataTable allowances;
        private TenderCountTransaction transaction;
        private bool cashCounted; // = false;
        private bool foreignCurrencyCounted; // = false;
        private bool otherTendersCounted; // = false;
        private string formHeadingText = string.Empty;
        private int[] attemptsDone;
        private decimal total;
        private int qtyPressedRowHandle = GridControl.InvalidRowHandle;
        private int qtyHighlightedRowHandle = GridControl.InvalidRowHandle;
        private int totalPressedRowHandle = GridControl.InvalidRowHandle;
        private int totalHighlightedRowHandle = GridControl.InvalidRowHandle;

        private const string COLTYPE = "TenderType";
        private const string COLNAME = "DisplayName";
        private const string COLTOTAL = "Total";
        private const string COLQTY = "QTY";
        private const int BUTTONMARGIN = 3;
        private const int STRING_ID_TENDER_NAME = 1965;


        private Dictionary<int, List<DenominationViewModel>> denominationDataSources;
        System.Collections.Generic.List<TenderViewModel> gridSource;

        struct TenderAllowances
        {
            private decimal maxCountingDifference;
            /// <summary>
            /// Get/set max counting difference property.
            /// </summary>
            public decimal MaxCountingDifference
            {
                get
                {
                    return maxCountingDifference;
                }
                set
                {
                    maxCountingDifference = value;
                }
            }

            private int maxRecount;
            /// <summary>
            /// Get/set maxRecount property.
            /// </summary>
            public int MaxRecount
            {
                get
                {
                    return maxRecount;
                }
                set
                {
                    maxRecount = value;
                }
            }
        }

        protected frmTenderCount()
        {
            // Required for Windows Form Designer support
            InitializeComponent();
        }

        /// <summary>
        /// Required designer variable.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Grandfather")]
        public frmTenderCount(IPosTransaction transaction)
            : this()
        {
            LogMessage("Creating frmTenderCount for transaction: " +
                transaction.GetType().ToString(),
                LSRetailPosis.LogTraceLevel.Trace,
                this.ToString());

            this.transaction = (TenderCountTransaction)transaction;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!this.DesignMode)
            {
                LogMessage("Loading frmTenderCount",
                    LSRetailPosis.LogTraceLevel.Trace,
                    this.ToString());

                TranslateLabels();


                // Check which amounts the POS will expect per tendertype
                // Start by checking which tender types to count....
                TenderData tenderData = new TenderData(
                    ApplicationSettings.Database.LocalConnection,
                    ApplicationSettings.Database.DATAAREAID);

                LoadFromTenderData(tenderData);

                LogMessage("Counting " + tendersToCount.Rows.Count + " tenders",
                    LSRetailPosis.LogTraceLevel.Trace,
                    this.ToString());

                gridSource = new System.Collections.Generic.List<TenderViewModel>();
                denominationDataSources = new Dictionary<int, List<DenominationViewModel>>();

                foreach (DataRow row in tendersToCount.Rows)
                {
                    ITender tmpTender = Dialog.InternalApplication.BusinessLogic.Utility.CreateTender();
                    tmpTender.TenderID = row["TENDERTYPEID"].ToString();
                    tmpTender.TenderName = row["NAME"].ToString();
                    tmpTender.PosisOperation = (PosisOperations)(row["POSOPERATION"]);

                    LSRetailPosis.ApplicationLog.Log(this.ToString(), "Counting: " + tmpTender.TenderID
                        + " " + tmpTender.TenderName + " "
                        + tmpTender.PosisOperation.ToString(),
                        LSRetailPosis.LogTraceLevel.Trace);

                    switch (tmpTender.PosisOperation)
                    {
                        case PosisOperations.PayCash:
                            cashCounted = true;

                            TenderViewModel cashRow = new TenderViewModel()
                            {
                                TenderTypeId = tmpTender.TenderID,
                                TenderOperationType = tmpTender.PosisOperation,
                                TenderName = tmpTender.TenderName,
                                DisplayName = ApplicationLocalizer.Language.Translate(STRING_ID_TENDER_NAME, ApplicationSettings.Terminal.StoreCurrency),
                                Currency = ApplicationSettings.Terminal.StoreCurrency
                            };

                            gridSource.Insert(0, cashRow);
                            break;

                        case PosisOperations.PayCurrency:
                            foreignCurrencyCounted = true;

                            ExchangeRateDataManager exchangeRateData = new ExchangeRateDataManager(ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID);
                            //Initialize with all number of currencies avialable
                            foreach (string currency in exchangeRateData.GetCurrencyPair(
                                                            ApplicationSettings.Terminal.StoreCurrency, 
                                                            ApplicationSettings.Terminal.ExchangeRateType, 
                                                            ApplicationSettings.Terminal.StorePrimaryId))
                            {
                                TenderViewModel currencyRow = new TenderViewModel()
                                {
                                    TenderTypeId = tmpTender.TenderID,
                                    TenderOperationType = tmpTender.PosisOperation,
                                    TenderName = tmpTender.TenderName,
                                    DisplayName = ApplicationLocalizer.Language.Translate(STRING_ID_TENDER_NAME, currency),
                                    Currency = currency
                                };

                                gridSource.Insert(gridSource.Count > 0 ? 1 : 0, currencyRow);
                            }
                            break;

                        default:
                            otherTendersCounted = true;
                            TenderViewModel otherRow = new TenderViewModel()
                            {
                                TenderTypeId = tmpTender.TenderID,
                                TenderOperationType = tmpTender.PosisOperation,
                                TenderName = tmpTender.TenderName,
                                DisplayName = tmpTender.TenderName
                            };

                            gridSource.Add(otherRow);
                            break;
                    }
                }

                this.gridTenders.DataSource = gridSource;

                this.Text = lblHeader.Text = formHeadingText;
                this.gvTenders.Appearance.HeaderPanel.ForeColor = this.ForeColor;
                this.gvTenders.Appearance.Row.ForeColor = this.ForeColor;

                UpdateTotalAmount();
            }

            base.OnLoad(e);
        }

        private void LoadFromTenderData(TenderData tenderData)
        {
            allowances = tenderData.GetAllowancesPerTender(ApplicationSettings.Terminal.StoreId);

            int counter = 0;
            if (transaction is TenderDeclarationTransaction)
            {
                counter = tenderData.HowManyTendersNEW(ApplicationSettings.Terminal.StoreId, 0);
                attemptsDone = new int[counter];
                tendersToCount = tenderData.GetTenderDeclarationData(ApplicationSettings.Database.DATAAREAID, ApplicationSettings.Terminal.StoreId);
                formHeadingText = LSRetailPosis.ApplicationLocalizer.Language.Translate(3493);  // Tender Declaration
                requiredAmounts = tenderData.GetRequiredAmountsPerTender(transaction.Shift, TypeOfTransaction.TenderDeclaration);
            }
            else if (transaction is BankDropTransaction)
            {
                counter = tenderData.HowManyTendersNEW(ApplicationSettings.Terminal.StoreId, 1);
                attemptsDone = new int[counter];
                tendersToCount = tenderData.GetBankDropData(ApplicationSettings.Database.DATAAREAID, ApplicationSettings.Terminal.StoreId);
                formHeadingText = LSRetailPosis.ApplicationLocalizer.Language.Translate(3923);  // Bank Drop                    
            }
            else if (transaction is SafeDropTransaction)
            {
                counter = tenderData.HowManyTendersNEW(ApplicationSettings.Terminal.StoreId, 2);
                attemptsDone = new int[counter];
                tendersToCount = tenderData.GetSafeDropData(ApplicationSettings.Database.DATAAREAID, ApplicationSettings.Terminal.StoreId);
                formHeadingText = LSRetailPosis.ApplicationLocalizer.Language.Translate(3902);  // Safe Drop                    
            }
            else
            {
                tendersToCount = new DataTable();
                formHeadingText = "Not specified operation";
                requiredAmounts = new DataTable();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (e != null && this.DialogResult == DialogResult.OK)
            {
                e.Cancel = !CreateTransaction();
            }

            base.OnClosing(e);
        }

        private void TranslateLabels()
        {
            //
            // Get all text through the Translation function in the ApplicationLocalizer
            //
            // TextID's for frmTenderCount are reserved at 1940 - 1959
            // In use now are ID's - 1959
            //

            btnOK.Text          = LSRetailPosis.ApplicationLocalizer.Language.Translate(1261);
            btnCancel.Text      = LSRetailPosis.ApplicationLocalizer.Language.Translate(1260);
            colTender.Caption   = LSRetailPosis.ApplicationLocalizer.Language.Translate(1956); // Payment method
            colQty.Caption      = LSRetailPosis.ApplicationLocalizer.Language.Translate(1962); // Quantity
            colTotal.Caption    = LSRetailPosis.ApplicationLocalizer.Language.Translate(1963); // Total
        }

        private void UpdateTotalAmount()
        {
            // Sum the total for each tender. Convert any foreign currency to store currency.
            total = gridSource.Sum(tender => tender.TenderOperationType == PosisOperations.PayCurrency
                ? Dialog.InternalApplication.Services.Currency.CurrencyToCurrency(tender.Currency, ApplicationSettings.Terminal.StoreCurrency, tender.Total)
                : tender.Total);

            // Refresh the total area in the grid
            gvTenders.UpdateTotalSummary();
        }

        private void ResetTenderValueToZero(string currentTenderTypeID, string currentTenderName)
        {
            TenderData tenderData = new TenderData(ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID);
            string tenderTypeID = tenderData.GetTenderID(ApplicationSettings.Terminal.StoreId, ((int)(PosisOperations.PayCurrency)).ToString());

            if (!currentTenderTypeID.Equals(tenderTypeID, StringComparison.OrdinalIgnoreCase))
            {
                gridSource.First(tender =>
                   tender.TenderTypeId.Equals(currentTenderTypeID, StringComparison.OrdinalIgnoreCase)).Total = 0m;
            }
            else
            {
                gridSource.First(tender =>
                   tender.TenderName.Equals(currentTenderName, StringComparison.OrdinalIgnoreCase)).Total = 0m;
            }

            gvTenders.InvalidateRows();

            UpdateTotalAmount();
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Grandfathered")]
        private bool CreateTransaction()
        {
            if (total > 0)
            {
                // Are you sure you want to save these amounts....
                DialogResult result = DialogResult.None;
                using (frmMessage dialog = new frmMessage(LSRetailPosis.ApplicationLocalizer.Language.Translate(1953), System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    result = dialog.DialogResult;
                }

                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    TenderLineItem tenderLine;
                    int i = 0;  //counter variable for the int Array

                    // Did we count cash?
                    if (cashCounted)
                    {
                        TenderViewModel cashViewTender = gridSource.First(tender => tender.TenderOperationType == PosisOperations.PayCash);
                        string cashTenderId = cashViewTender.TenderTypeId;
                        string cashTenderDesc = cashViewTender.TenderName;
                        string cashTenderCode = ApplicationSettings.Terminal.StoreCurrency;

                        decimal totalCashAmount = gridSource.Sum(tender => tender.TenderOperationType == PosisOperations.PayCash ? tender.Total : 0m);

                        //This allows the operator to recount a given number of times.
                        if (transaction is TenderDeclarationTransaction)
                        {
                            int rowHandle = gridSource.IndexOf(cashViewTender);

                            if (NeedForRecount(cashTenderId, totalCashAmount, cashTenderDesc, ref i))
                            {
                                // Select the cash row
                                if (rowHandle != -1)
                                {
                                    gvTenders.SelectRow(rowHandle);
                                }

                                return false;
                            }
                            else
                            {
                                // Disable the cash tender row
                                cashViewTender.Enabled = false;
                                gvTenders.InvalidateRow(rowHandle);

                                i++; //proceed to the next counter that belongs to the next tendertype.
                            }
                        }

                        //Adding a Tenderline (or not)
                        if (totalCashAmount > 0)
                        {
                            tenderLine = (TenderLineItem)Dialog.InternalApplication.BusinessLogic.Utility.CreateTenderLineItem();
                            tenderLine.Amount = totalCashAmount;
                            tenderLine.TenderTypeId = cashTenderId;
                            tenderLine.Description = cashTenderDesc;
                            tenderLine.ExchrateMST = Dialog.InternalApplication.Services.Currency.ExchangeRate(transaction.StoreCurrencyCode) * 100;
                            tenderLine.CompanyCurrencyAmount = Dialog.InternalApplication.Services.Currency.CurrencyToCurrency(transaction.StoreCurrencyCode, ApplicationSettings.Terminal.CompanyCurrency, totalCashAmount);
                            tenderLine.CurrencyCode = transaction.StoreCurrencyCode;
                            transaction.Add(tenderLine);
                        }
                    }

                    // Did we count other tenders?
                    if (otherTendersCounted)
                    {
                        List<TenderViewModel> otherTenders = gridSource.FindAll(
                            tender => tender.TenderOperationType != PosisOperations.PayCash && tender.TenderOperationType != PosisOperations.PayCurrency);

                        foreach (TenderViewModel otherCount in otherTenders)
                        {
                            if (transaction is TenderDeclarationTransaction)
                            {
                                int rowHandle = gridSource.IndexOf(otherCount);

                                if (NeedForRecount(otherCount.TenderTypeId, otherCount.Total, otherCount.TenderName, ref i))
                                {
                                    // Select the row
                                    if (rowHandle != -1)
                                    {
                                        gvTenders.SelectRow(rowHandle);
                                    }

                                    return false;
                                }
                                else
                                {
                                    // Disable the row
                                    otherCount.Enabled = false;
                                    gvTenders.InvalidateRow(rowHandle);
                                    i++;
                                }
                            }

                            if (otherCount.Total > 0)
                            {
                                tenderLine = (TenderLineItem)Dialog.InternalApplication.BusinessLogic.Utility.CreateTenderLineItem();
                                tenderLine.Amount = otherCount.Total;
                                tenderLine.TenderTypeId = otherCount.TenderTypeId;
                                tenderLine.Description = otherCount.TenderName;
                                tenderLine.ExchrateMST = Dialog.InternalApplication.Services.Currency.ExchangeRate(transaction.StoreCurrencyCode) * 100;
                                tenderLine.CompanyCurrencyAmount = Dialog.InternalApplication.Services.Currency.CurrencyToCurrency(transaction.StoreCurrencyCode, ApplicationSettings.Terminal.CompanyCurrency, otherCount.Total);
                                tenderLine.CurrencyCode = transaction.StoreCurrencyCode;
                                transaction.Add(tenderLine);
                            }
                        }
                    }

                    // Did we count foreign currency?
                    if (foreignCurrencyCounted)
                    {
                        List<TenderViewModel> currencyTenders = gridSource.FindAll(tender => tender.TenderOperationType == PosisOperations.PayCurrency);

                        foreach (TenderViewModel currencyCount in currencyTenders)
                        {
                            //Convert to StoreCurrency...
                            decimal currAmountInStoreCurr = Dialog.InternalApplication.Services.Currency.CurrencyToCurrency(
                                    currencyCount.Currency, ApplicationSettings.Terminal.StoreCurrency, currencyCount.Total);

                            if (transaction is TenderDeclarationTransaction)
                            {
                                int rowHandle = gridSource.IndexOf(currencyCount);

                                if (NeedForRecount(currencyCount.TenderTypeId, currAmountInStoreCurr, currencyCount.Currency, ref i))
                                {
                                    // Select the row
                                    if (rowHandle != -1)
                                    {
                                        gvTenders.SelectRow(rowHandle);
                                    }

                                    return false;
                                }
                                else
                                {
                                    // Disable the row
                                    currencyCount.Enabled = false;
                                    gvTenders.InvalidateRow(rowHandle);

                                    i++;
                                }
                            }

                            if (currencyCount.Total > 0)
                            {
                                tenderLine = (TenderLineItem)Dialog.InternalApplication.BusinessLogic.Utility.CreateTenderLineItem();
                                tenderLine.Amount = currencyCount.Total;
                                tenderLine.ForeignCurrencyAmount = currAmountInStoreCurr;
                                tenderLine.CompanyCurrencyAmount = Dialog.InternalApplication.Services.Currency.CurrencyToCurrency(currencyCount.Currency, ApplicationSettings.Terminal.CompanyCurrency, currencyCount.Total);
                                tenderLine.ExchangeRate = ((Dialog.InternalApplication.Services.Currency.ExchangeRate(currencyCount.Currency)) / (Dialog.InternalApplication.Services.Currency.ExchangeRate(ApplicationSettings.Terminal.StoreCurrency))) * 100;
                                tenderLine.ExchrateMST = Dialog.InternalApplication.Services.Currency.ExchangeRate(currencyCount.Currency) * 100;
                                tenderLine.TenderTypeId = currencyCount.TenderTypeId;
                                tenderLine.Description = currencyCount.TenderName;
                                tenderLine.CurrencyCode = currencyCount.Currency;
                                transaction.Add(tenderLine);
                            }
                        }
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }

            // No amount has been entered yet
            using (frmMessage frm = new frmMessage(LSRetailPosis.ApplicationLocalizer.Language.Translate(1959), System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Hand))
            {
                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(frm);
            }

            return false;
        }

        private bool NeedForRecount(string currentTender, decimal currentAmount, string currentTenderName, ref int counter)
        {
            //Get allowances for that Tender from In-Memory table. The values are fetched before the counting dialog is opened.
            TenderAllowances tenderAllow = GetAllowancesForCurrentTender(currentTender);
            decimal allowance = tenderAllow.MaxCountingDifference;
            int recountsAllowed = tenderAllow.MaxRecount;

            //decimal expected = currentValues.ExpectedAmount;
            decimal expected = GetExpectedAmount(currentTender, currentTenderName);

            if (attemptsDone[counter] < recountsAllowed)
            {
                attemptsDone[counter]++;
                if (Math.Abs((currentAmount - expected)) > allowance)
                {
                    ResetTenderValueToZero(currentTender, currentTenderName);
                    transaction.TenderLines.Clear();
                    string msg = string.Format(LSRetailPosis.ApplicationLocalizer.Language.Translate(3494), currentTenderName,
                           Dialog.InternalApplication.Services.Rounding.RoundAmount(expected, ApplicationSettings.Terminal.StoreId, currentTender, true));

                    using (frmMessage dialog = new frmMessage(msg, MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    }

                    return true;  //yes, recount is necessary....
                }
            }

            return false; // no need for recount
        }

        private TenderAllowances GetAllowancesForCurrentTender(string testTender)
        {
            TenderAllowances ta = new TenderAllowances();

            foreach (DataRow row in allowances.Rows)
            {
                string tenderID = row["TENDERTYPEID"].ToString();

                if (tenderID.Equals(testTender))
                {
                    if (row["MAXCOUNTINGDIFFERENCE"] != DBNull.Value)
                    {
                        ta.MaxCountingDifference = (Convert.ToDecimal(row["MAXCOUNTINGDIFFERENCE"].ToString()));
                    }
                    else
                    {
                        ta.MaxCountingDifference = 0M;
                    }

                    if (row["MAXRECOUNT"] != DBNull.Value)
                    {
                        ta.MaxRecount = (Convert.ToInt16(row["MAXRECOUNT"].ToString()));
                    }
                    else
                    {
                        ta.MaxRecount = 0;
                    }

                    return ta;
                }
            }

            return ta;
        }

        private decimal GetExpectedAmount(string testTender, string testTenderName)
        {
            decimal exp = 0M;

            foreach (DataRow row in requiredAmounts.Rows)
            {
                string tenderID = row["TENDERTYPE"].ToString();
                string tenderName = row["CURRENCY"].ToString().Trim();

                if (tenderID.Equals(testTender))
                {
                    if (!tenderID.Equals("6"))
                    {
                        return (Convert.ToDecimal(row["AMOUNTTENDERED"].ToString().Trim()));
                    }
                    else
                    {
                        if (tenderName.Equals(testTenderName))
                        {
                            exp = Convert.ToDecimal(row["AMOUNTCUR"].ToString().Trim());
                            exp = Dialog.InternalApplication.Services.Currency.CurrencyToCurrency(
                                testTenderName, ApplicationSettings.Terminal.StoreCurrency, exp);
                        }
                    }
                }
            }
            return exp;
        }

        /// <summary>
        /// Log a message to the file.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="traceLevel"></param>
        /// <param name="args"></param>
        private void LogMessage(string message, LogTraceLevel traceLevel, params object[] args)
        {
            ApplicationLog.Log(this.GetType().Name, string.Format(message, args), traceLevel);
        }

        #region Custom grid code

        /// <summary>
        /// Calculates the total field
        /// </summary>        
        private void gvTender_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            switch (e.SummaryProcess)
            {
                case DevExpress.Data.CustomSummaryProcess.Start:
                    e.TotalValueReady = true;
                    break;
                case DevExpress.Data.CustomSummaryProcess.Finalize:
                    e.TotalValue = Dialog.InternalApplication.Services.Rounding.RoundForDisplay(total, true, true);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Handles the display text for custom grid columns
        /// </summary>
        private void gvTenders_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            switch (e.Column.FieldName)
            {
                case COLQTY:
                    e.DisplayText = LSRetailPosis.ApplicationLocalizer.Language.Translate(1957); // "Count..."
                    break;
                case COLTOTAL:
                    // Get amount value
                    decimal amount = gridSource[e.ListSourceRowIndex].Total;

                    // Get currency code for symbol display
                    PosisOperations operationTenderType = gridSource[e.ListSourceRowIndex].TenderOperationType;
                    string currCode = operationTenderType == PosisOperations.PayCurrency
                        ? gridSource[e.ListSourceRowIndex].Currency
                        : ApplicationSettings.Terminal.StoreCurrency;

                    // Display amount with currency code
                    e.DisplayText = Dialog.InternalApplication.Services.Rounding.RoundForDisplay(amount, currCode, true, true);
                    break;
                default:
                    break;
            }
        }

        private void gvTenders_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            string column = e.Column.FieldName;

            if (column == COLQTY || column == COLTOTAL)
            {
                // Determine the tender type
                PosisOperations operationTenderType = gridSource[e.RowHandle].TenderOperationType;

                // Draw the calculator icon in the quantity column if the tender type is not cash or currency
                bool drawIcon = (column == COLQTY) && (operationTenderType != PosisOperations.PayCash) && (operationTenderType != PosisOperations.PayCurrency);

                e.Appearance.FillRectangle(e.Cache, e.Bounds);
                DrawButton(e.Cache, e.Bounds, gridTenders.LookAndFeel.ActiveLookAndFeel.ActiveStyle, e.Appearance, GetButtonState(e.RowHandle, column), e.DisplayText, drawIcon);

                e.Handled = true;
            }
        }

        private void DrawButton(GraphicsCache cache, Rectangle bounds, ActiveLookAndFeelStyle lookAndFeel, AppearanceObject appearance, ObjectState state, string caption, bool drawIcon)
        {
            EditorButtonObjectInfoArgs args = new EditorButtonObjectInfoArgs(cache, Button, appearance);
            BaseLookAndFeelPainters painters = LookAndFeelPainterHelper.GetPainter(lookAndFeel);

            // Create some margin
            bounds.Inflate(-BUTTONMARGIN, -BUTTONMARGIN);

            args.Bounds = bounds;
            args.State = state;

            painters.Button.DrawObject(args);

            if (drawIcon)
            {
                Image calcIcon = Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.PriceCalc;
                if (calcIcon != null)
                {
                    // Determine origin point to draw centered
                    int x = args.Bounds.Left + ((args.Bounds.Width - calcIcon.Width) / 2);
                    int y = args.Bounds.Top + ((args.Bounds.Height - calcIcon.Height) / 2);

                    if (state == ObjectState.Disabled)
                    {
                        ControlPaint.DrawImageDisabled(cache.Graphics, calcIcon, x, y, Color.Transparent);
                    }
                    else
                    {
                        cache.Graphics.DrawImageUnscaled(calcIcon, x, y);
                    }
                }
            }
            else
            {
                // Draw the text 
                // DO NOT dispose of the brush, as it is owned by the cache
                Brush brush = GetButtonForeBrush(state, cache);

                StringFormat sf = appearance.GetStringFormat(appearance.GetTextOptions());
                painters.Button.DrawCaption(args, caption, appearance.Font, brush, args.Bounds, sf);
            }
        }

        private Brush GetButtonForeBrush(ObjectState state, GraphicsCache cache)
        {
            if (state == ObjectState.Disabled)
            {
                return SystemBrushes.GrayText;
            }

            return gvTenders.Appearance.HeaderPanel.GetForeBrush(cache);
        }

        protected ObjectState GetButtonState(int rowHandle, string column)
        {
            if (!gridSource[rowHandle].Enabled)
            {
                return ObjectState.Disabled;
            }

            int pressedRowHandle = GridControl.InvalidRowHandle;
            int highlightedRowHandle = GridControl.InvalidRowHandle;

            switch (column)
            {
                case COLQTY:
                    pressedRowHandle = QtyPressedRowHandle;
                    highlightedRowHandle = QtyHighlightedRowHandle;
                    break;
                case COLTOTAL:
                    pressedRowHandle = TotalPressedRowHandle;
                    highlightedRowHandle = TotalHighlightedRowHandle;
                    break;
                default:
                    break;
            }

            if (rowHandle == pressedRowHandle)
            {
                return ObjectState.Pressed;
            }
            else
            {
                // Show hot if row is highlighted or cell is focused
                if (rowHandle == highlightedRowHandle
                    || (gridTenders.IsFocused && gvTenders.FocusedColumn.FieldName == column && gvTenders.FocusedRowHandle == rowHandle))
                {
                    return ObjectState.Hot;
                }
                else
                {
                    return ObjectState.Normal;
                }
            }
        }

        private DevExpress.XtraEditors.Controls.EditorButton button;

        private DevExpress.XtraEditors.Controls.EditorButton Button
        {
            get
            {
                if (button == null)
                {
                    button = new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.OK);
                }

                return button;
            }
        }

        private int QtyPressedRowHandle
        {
            get { return qtyPressedRowHandle; }
            set
            {
                if (qtyPressedRowHandle != value)
                {
                    SetPressedRowHandle(ref qtyPressedRowHandle, COLQTY, value);

                    gvTenders.InvalidateRowCell(qtyPressedRowHandle, gvTenders.Columns[COLQTY]);
                }
            }
        }

        private int QtyHighlightedRowHandle
        {
            get { return qtyHighlightedRowHandle; }
            set
            {
                if (qtyHighlightedRowHandle != value)
                {
                    SetHighlightedRowHandle(ref qtyHighlightedRowHandle, COLQTY, value);

                    gvTenders.InvalidateRowCell(qtyHighlightedRowHandle, gvTenders.Columns[COLQTY]);
                }
            }
        }

        private int TotalPressedRowHandle
        {
            get { return totalPressedRowHandle; }
            set
            {
                if (totalPressedRowHandle != value)
                {
                    SetPressedRowHandle(ref totalPressedRowHandle, COLTOTAL, value);

                    gvTenders.InvalidateRowCell(totalPressedRowHandle, gvTenders.Columns[COLTOTAL]);
                }
            }
        }

        private int TotalHighlightedRowHandle
        {
            get { return totalHighlightedRowHandle; }
            set
            {
                if (totalHighlightedRowHandle != value)
                {
                    SetHighlightedRowHandle(ref totalHighlightedRowHandle, COLTOTAL, value);

                    gvTenders.InvalidateRowCell(totalHighlightedRowHandle, gvTenders.Columns[COLTOTAL]);
                }
            }
        }

        private void SetPressedRowHandle(ref int rowHandle, string column, int value)
        {
            if (rowHandle != GridControl.InvalidRowHandle)
            {
                int tempRowHandle = rowHandle;
                rowHandle = GridControl.InvalidRowHandle;
                gvTenders.InvalidateRowCell(tempRowHandle, gvTenders.Columns[column]);
            }

            rowHandle = value;
        }

        private void SetHighlightedRowHandle(ref int rowHandle, string column, int value)
        {
            if (rowHandle == value)
            {
                return;
            }

            if (rowHandle != GridControl.InvalidRowHandle)
            {
                int tempRowHandle = rowHandle;
                rowHandle = GridControl.InvalidRowHandle;
                gvTenders.InvalidateRowCell(tempRowHandle, gvTenders.Columns[column]);
            }
            else
            {
                rowHandle = value;
                QtyPressedRowHandle = GridControl.InvalidRowHandle;
            }
        }

        private void gvTenders_MouseDown(object sender, MouseEventArgs e)
        {
            if (QtyHighlightedRowHandle != GridControl.InvalidRowHandle)
            {
                QtyPressedRowHandle = QtyHighlightedRowHandle;
            }

            if (TotalHighlightedRowHandle != GridControl.InvalidRowHandle)
            {
                TotalPressedRowHandle = TotalHighlightedRowHandle;
            }
        }

        private void gvTenders_MouseUp(object sender, MouseEventArgs e)
        {
            if (QtyPressedRowHandle != GridControl.InvalidRowHandle)
            {
                OnQtyButtonClick(QtyPressedRowHandle);
                QtyPressedRowHandle = GridControl.InvalidRowHandle;
            }

            if (TotalPressedRowHandle != GridControl.InvalidRowHandle)
            {
                OnTotalButtonClick(TotalPressedRowHandle);
                TotalPressedRowHandle = GridControl.InvalidRowHandle;
            }
        }

        private void gvTenders_MouseMove(object sender, MouseEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (view != null)
            {
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo info = view.CalcHitInfo(e.X, e.Y);
                if (info.InRowCell && info.Column.FieldName == COLQTY && IsMouseOverButton(info.RowHandle, COLQTY, new Point(e.X, e.Y)))
                {
                    QtyHighlightedRowHandle = info.RowHandle;
                }
                else
                {
                    QtyHighlightedRowHandle = GridControl.InvalidRowHandle;
                }

                if (info.InRowCell && info.Column.FieldName == COLTOTAL && IsMouseOverButton(info.RowHandle, COLTOTAL, new Point(e.X, e.Y)))
                {
                    TotalHighlightedRowHandle = info.RowHandle;
                }
                else
                {
                    TotalHighlightedRowHandle = GridControl.InvalidRowHandle;
                }
            }
        }

        private bool IsMouseOverButton(int rowHandle, string column, Point point)
        {
            bool result = false;

            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridViewInfo vInfo = gvTenders.GetViewInfo() as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridViewInfo;
            if (vInfo != null)
            {
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridCellInfo cellInfo = vInfo.GetGridCellInfo(rowHandle, gvTenders.Columns[column]);
                result = cellInfo.Bounds.Contains(point);
            }

            return result;
        }

        private void OnQtyButtonClick(int rowHandle)
        {
            if (!gridSource[rowHandle].Enabled)
            {
                return;
            }

            // Determine the tender type            
            PosisOperations operationTenderType = gridSource[rowHandle].TenderOperationType;

            // Open the denominations form for currencies
            if (operationTenderType == PosisOperations.PayCash || operationTenderType == PosisOperations.PayCurrency)
            {
                using (frmDenominations frmDenom = new frmDenominations(gridSource[rowHandle].Currency,
                    (denominationDataSources.ContainsKey(rowHandle) ? denominationDataSources[rowHandle] : null)))
                {
					POSFormsManager.ShowPOSForm(frmDenom);
                    if (frmDenom.DialogResult == DialogResult.OK)
                    {
                        gridSource[rowHandle].Total = frmDenom.Total;
                        this.denominationDataSources[rowHandle] = frmDenom.GridSource;
                        UpdateTotalAmount();
                    }
                }
            }
            else
            {
                // Launch the system calculator
                try
                {
                    System.Diagnostics.Process.Start("calc.exe");
                }
                catch (System.ComponentModel.Win32Exception)
                {
                }
                catch (System.IO.FileNotFoundException)
                {
                }
            }
        }

        private void OnTotalButtonClick(int rowHandle)
        {
            if (!gridSource[rowHandle].Enabled)
            {
                return;
            }

            using (frmInputNumpad inputDialog = new frmInputNumpad())
            {
                inputDialog.EntryTypes = NumpadEntryTypes.Price;
                inputDialog.PromptText = LSRetailPosis.ApplicationLocalizer.Language.Translate(1443);

                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(inputDialog);

                if (inputDialog.DialogResult == DialogResult.OK)
                {
                    decimal subTotal;
                    if (decimal.TryParse(inputDialog.InputText, out subTotal))
                    {
                        gridSource[rowHandle].Total = subTotal;
                        UpdateTotalAmount();

                        //Reseting the data source of denomination grid since user has altered the total amount
                        denominationDataSources[rowHandle] = null;
                    }
                }
            }
        }

        private void btnPageUp_Click(object sender, EventArgs e)
        {
            gvTenders.MovePrevPage();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            gvTenders.MovePrev();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            gvTenders.MoveNext();
        }

        private void btnPageDown_Click(object sender, EventArgs e)
        {
            gvTenders.MoveNextPage();
        }

        private void gvTenders_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                switch (gvTenders.FocusedColumn.FieldName)
                {
                    case COLQTY:
                        OnQtyButtonClick(gvTenders.FocusedRowHandle);
                        break;
                    case COLTOTAL:
                        OnTotalButtonClick(gvTenders.FocusedRowHandle);
                        break;
                    default:
                        break;
                }
            }
        }

        # endregion

    }

    /// <summary>
    /// Model of tender row for grid to bind to
    /// </summary>
    sealed internal class TenderViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TenderViewModel"/> class.
        /// </summary>
        public TenderViewModel()
        {
            this.Enabled = true;
        }

        /// <summary>
        /// Gets or sets the type of the tender operation.
        /// </summary>
        /// <value>
        /// The type of the tender operation.
        /// </value>
        public PosisOperations TenderOperationType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the tender type id.
        /// </summary>
        /// <value>
        /// The tender type id.
        /// </value>
        public string TenderTypeId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the tender.
        /// </summary>
        /// <value>
        /// The name of the tender.
        /// </value>
        public string TenderName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "DisplayName.get() is used for grid binding")]
        public string DisplayName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the currency.
        /// </summary>
        /// <value>
        /// The currency.
        /// </value>
        public string Currency
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the total.
        /// </summary>
        /// <value>
        /// The total.
        /// </value>
        public decimal Total
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TenderViewModel"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled
        {
            get;
            set;
        }
    }
}
