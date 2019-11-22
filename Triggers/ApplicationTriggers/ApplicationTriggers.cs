//Microsoft Dynamics AX for Retail POS Plug-ins 
//The following project is provided as SAMPLE code. Because this software is "as is," we may not provide support services for it.

using System;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using LSRetailPosis;
using LSRetailPosis.Settings;
using LSRetailPosis.Settings.FunctionalityProfiles;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.Triggers;

namespace Microsoft.Dynamics.Retail.Pos.ApplicationTriggers
{
    [Export(typeof(IApplicationTrigger))]
    public class ApplicationTriggers : IApplicationTrigger
    {

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
                InternalApplication = value;
            }
        }

        /// <summary>
        /// Gets or sets the static IApplication instance.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal static IApplication InternalApplication { get; private set; }

        public ApplicationTriggers()
        {
        }

        #region IApplicationTriggers Members

        public void ApplicationStart()
        {
            string source = "IApplicationTriggers.ApplicationStart";
            string value = "Application has started";
            LSRetailPosis.ApplicationLog.Log(source, value, LSRetailPosis.LogTraceLevel.Debug);
            LSRetailPosis.ApplicationLog.WriteAuditEntry(source, value);

            // If the store is in Brazil, we should only allow to run the POS if the Functionality profile's ISO locale is Brazil
            if (ApplicationSettings.Terminal.StoreCountry.Equals(SupportedCountryRegion.BR.ToString(), StringComparison.OrdinalIgnoreCase)
                && Functions.CountryRegion != SupportedCountryRegion.BR)
            {
                var message = ApplicationLocalizer.Language.Translate(85082, // The locale must be Brazil. In the POS functionality profile form, on the General tab, in the Locale field, select Brazil.
                                                                      ApplicationSettings.Terminal.StoreCountry);

                using (var form = new LSRetailPosis.POSProcesses.frmMessage(message, MessageBoxButtons.OK, MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(form);
                }

                throw new LSRetailPosis.PosStartupException(message);
            }
        }

        public void ApplicationStop()
        {
            string source = "IApplicationTriggers.ApplicationStop";
            string value = "Application has stopped";
            LSRetailPosis.ApplicationLog.Log(source, value, LSRetailPosis.LogTraceLevel.Debug);
            LSRetailPosis.ApplicationLog.WriteAuditEntry(source, value);
        }

        public void PostLogon(bool loginSuccessful, string operatorId, string name)
        {
            string source = "IApplicationTriggers.PostLogon";
            string value = loginSuccessful ? "User has successfully logged in. OperatorID: " + operatorId : "Failed user login attempt. OperatorID: " + operatorId;
            LSRetailPosis.ApplicationLog.Log(source, value, LSRetailPosis.LogTraceLevel.Debug);
            LSRetailPosis.ApplicationLog.WriteAuditEntry(source, value);
        }

        public void PreLogon(IPreTriggerResult preTriggerResult, string operatorId, string name)
        {
            LSRetailPosis.ApplicationLog.Log("IApplicationTriggers.PreLogon", "Before the user has been logged on...", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void Logoff(string operatorId, string name)
        {
            string source = "IApplicationTriggers.Logoff";
            string value = "User has successfully logged off. OperatorID: " + operatorId;
            LSRetailPosis.ApplicationLog.Log(source, value, LSRetailPosis.LogTraceLevel.Debug);
            LSRetailPosis.ApplicationLog.WriteAuditEntry(source, value);
        }

        public void LoginWindowVisible()
        {
            LSRetailPosis.ApplicationLog.Log("IApplicationTriggers.LoginWindowVisible", "When the login window is visible", LSRetailPosis.LogTraceLevel.Trace);
        }

        #endregion

    }
}
