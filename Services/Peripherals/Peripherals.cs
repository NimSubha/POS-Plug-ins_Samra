/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Interop.OposConstants;
using LSRetailPosis;
using LSRetailPosis.Settings;
using LSRetailPosis.Settings.HardwareProfiles;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;

namespace Microsoft.Dynamics.Retail.Pos.Services
{
    /// <summary>
    /// Class implements IPeripherals interface.
    /// </summary>
    [Export(typeof(IPeripherals))]
    public class Peripherals : IPeripherals
    {

        #region Fields

        internal static int ClaimTimeOut = 10000;

        #endregion

        #region Properties

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
        internal static IApplication InternalApplication { get; private set; }

        /// <summary>
        /// Biometric peripheral device.
        /// </summary>
        /// <remarks>This is an optional device.</remarks>
        [Import(AllowDefault = true)]
        public IBiometricDevice BiometricDevice { get; protected set; }

        /// <summary>
        /// CashDrawer peripheral device.
        /// </summary>
        public ICashDrawer CashDrawer { get; protected set; }

        /// <summary>
        /// DualDisplay peripheral device.
        /// </summary>
        public IDualDisplay DualDisplay { get; protected set; }

        /// <summary>
        /// KeyLock peripheral device.
        /// </summary>
        public IKeyLock KeyLock { get; protected set; }

        /// <summary>
        /// LineDisplay peripheral device.
        /// </summary>
        public ILineDisplay LineDisplay { get; protected set; }

        /// <summary>
        /// LogOn peripheral device.
        /// </summary>
        public ILogOnDevice LogOnDevice { get; protected set; }

        /// <summary>
        /// MSR peripheral device.
        /// </summary>
        public IMSR MSR { get; protected set; }

        /// <summary>
        /// PinPad peripheral device.
        /// </summary>
        public IPinPad PinPad { get; protected set; }

        /// <summary>
        /// Printer peripheral device.
        /// </summary>
        public IPrinter Printer { get; protected set; }

        /// <summary>
        /// RFIDScanner peripheral device.
        /// </summary>
        public IRadioFrequencyIDScanner RFIDScanner { get; protected set; }

        /// <summary>
        /// Scale peripheral device.
        /// </summary>
        public IScale Scale { get; protected set; }

        /// <summary>
        /// Scanner peripheral device.
        /// </summary>
        public IScanner Scanner { get; protected set; }

        /// <summary>
        /// Signature capture peripheral device.
        /// </summary>
        public ISignatureCapture SignatureCapture { get; protected set; }

        /// <summary>
        /// Printer peripheral device (2nd).
        /// </summary>
        public IPrinter Printer2 { get; protected set; }
        
        #endregion

        #region IPeripherals Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public Peripherals()
        {
            // TextID's for Peripherals are reserved at 6200 - 6299
        }

        /// <summary>
        /// Load all peripheral devices.
        /// </summary>
        public void Load()
        {
            // Construct all peripheral objects.
            Printer = Application.CompositionContainer.GetExportedValues<IPrinter>().First();
            Printer.SetUpPrinter(
                LSRetailPosis.Settings.HardwareProfiles.Printer.DeviceType.ToString(),
                LSRetailPosis.Settings.HardwareProfiles.Printer.DeviceName,
                LSRetailPosis.Settings.HardwareProfiles.Printer.DeviceDescription);
            Printer2 = Application.CompositionContainer.GetExportedValues<IPrinter>().First();
            Printer2.SetUpPrinter(
                LSRetailPosis.Settings.HardwareProfiles.Printer2.DeviceType.ToString(),
                LSRetailPosis.Settings.HardwareProfiles.Printer2.DeviceName,
                LSRetailPosis.Settings.HardwareProfiles.Printer2.DeviceDescription);
            CashDrawer = Application.CompositionContainer.GetExportedValues<ICashDrawer>().First();
            DualDisplay = Application.CompositionContainer.GetExportedValues<IDualDisplay>().First();
            KeyLock = Application.CompositionContainer.GetExportedValues<IKeyLock>().First();
            LineDisplay = Application.CompositionContainer.GetExportedValues<ILineDisplay>().First();
            LogOnDevice = Application.CompositionContainer.GetExportedValues<ILogOnDevice>().First();
            MSR = Application.CompositionContainer.GetExportedValues<IMSR>().First();
            PinPad = Application.CompositionContainer.GetExportedValues<IPinPad>().First();
            RFIDScanner = Application.CompositionContainer.GetExportedValues<IRadioFrequencyIDScanner>().First();
            Scale = Application.CompositionContainer.GetExportedValues<IScale>().First();
            Scanner = Application.CompositionContainer.GetExportedValues<IScanner>().First();
            SignatureCapture = Application.CompositionContainer.GetExportedValues<ISignatureCapture>().First();

            LoadPeripheral(6209, Printer);
            LoadPeripheral(6209, Printer2);
            LoadPeripheral(6203, CashDrawer);
            LoadPeripheral(6205, KeyLock);
            LoadPeripheral(6207, LineDisplay);
            LoadPeripheral(6204, MSR);
            LoadPeripheral(6210, PinPad);
            LoadPeripheral(6202, RFIDScanner);
            LoadPeripheral(6206, Scale);
            LoadPeripheral(6208, Scanner);
            LoadPeripheral(6201, DualDisplay);
            LoadPeripheral(6213, SignatureCapture);

            if (BiometricDevice != null)
            {
                LoadPeripheral(6214, BiometricDevice);
            }

            LogOnDevice.Load(); // It is a proxy device, no error handling required on load
        }

        /// <summary>
        /// Unload all peripheral devices.
        /// </summary>
        public void Unload()
        {
            LogOnDevice.Unload();
            CashDrawer.Unload();
            KeyLock.Unload();
            MSR.Unload();
            LineDisplay.Unload();
            PinPad.Unload();
            RFIDScanner.Unload();
            Scale.Unload();
            Scanner.Unload();
            SignatureCapture.Unload();
            Printer2.Unload();
            Printer.Unload();
            LogOnDevice.Unload();

            if (BiometricDevice != null)
            {
                BiometricDevice.Unload();
            }
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Convert string from Unicode to given character set and pack in BCD format.
        /// </summary>
        /// <param name="source">String to be converted</param>
        /// <param name="characterSet">Target character set</param>
        /// <returns>String as BCD</returns>
        internal static string ConvertToBCD(string source, int characterSet)
        {
            Encoding sourceEncoding = Encoding.Unicode;
            Encoding targetEncoding = Encoding.GetEncoding(characterSet);

            // Reading the bytes from the unicode string
            byte[] sourceBytes = sourceEncoding.GetBytes(source);

            // Converting those bytes 
            byte[] targetBytes = Encoding.Convert(sourceEncoding, targetEncoding, sourceBytes);

            StringBuilder result = new StringBuilder();

            // UPOS Binary conversion accepts each character formatted in 3 bytes padded with zeros.
            foreach (byte b in targetBytes)
            {
                result.AppendFormat("{0:000}", b);
            }

            return result.ToString();
        }

        /// <summary>
        /// Returns the number of bytes in a string considering double byte characters.
        /// </summary>
        /// <param name="source">Source string</param>
        /// <param name="charSet">Character set</param>
        /// <returns></returns>
        internal static int GetByteCount(string source, int charSet)
        {
            return Encoding.GetEncoding(charSet).GetByteCount(source);
        }

        /// <summary>
        /// Check the OPOS result code from last operation.
        /// </summary>
        /// <param name="resultCode"></param>
        /// <exception cref="IOException"></exception>
        internal static void CheckResultCode(IPeripheral source, int resultCode)
        {
            OPOS_Constants result = (OPOS_Constants)resultCode;

            if (result != OPOS_Constants.OPOS_SUCCESS)
            {
                string message = string.Format("{0} device failed with error '{1}'.", source.GetType().Name, result);
                throw new IOException(message);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Load a peripheral device with Retry prompt.
        /// </summary>
        /// <param name="peripheralStringId"></param>
        /// <param name="perphiral"></param>
        private void LoadPeripheral(int peripheralStringId, IPeripheral perphiral)
        {
            bool retry;

            do
            {
                try
                {
                    perphiral.Load();
                    retry = false;
                }
                catch (PosStartupException startupException)
                {
                    // The Fiscal printer validates law requirements and it must be able to abort the POS startup.
                    throw new PosStartupException(startupException.Message);
                }
                catch (Exception ex)
                {
                    ApplicationExceptionHandler.HandleException(this.ToString(), ex);

                    retry = PromptForRetry(peripheralStringId);
                }
            } while (retry);
        }

        /// <summary>
        /// Prompt error message and ask for retry.
        /// </summary>
        /// <param name="deviceStringId">Device name string id..</param>
        /// <returns>True if user opted retry, False otherwise.</returns>
        static private bool PromptForRetry(int deviceStringId)
        {
            string errorMessage = string.Format(ApplicationLocalizer.Language.Translate(6200),
                ApplicationSettings.ShortApplicationTitle,
                ApplicationLocalizer.Language.Translate(deviceStringId));

            DialogResult dialogResult = Peripherals.InternalApplication.Services.Dialog.ShowMessage(errorMessage,
                MessageBoxButtons.RetryCancel,
                MessageBoxIcon.Error);

            if (dialogResult == DialogResult.Retry)
            {
                return true;
            }

            return false;
        }

        #endregion

    }
}
