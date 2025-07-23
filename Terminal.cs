using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Security.Policy;
using System.Threading;
using System.Windows.Forms;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;






namespace JDY23Terminal {
    public partial class CC245XTerminal : Form {

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);


        const int EM_GETFIRSTVISIBLELINE = 0xCE;
        const int EM_LINESCROLL = 0xB6;
        const int WM_SETREDRAW = 0x0B;


        Guid SimpleKeyServiceGUID = new Guid("0000ffe0-0000-1000-8000-00805f9b34fb");
        Guid SimpleKeyStateGUID = new Guid("0000ffe1-0000-1000-8000-00805f9b34fb");

        static Color SysMsgCol = Color.Blue;
        static Color RXMsgCol = Color.Black;
        static Color TXMsgCol = Color.Green;
        static Color ErrMsgCol = Color.Red;

        ToolStripMenuItem connectMenu;
        ToolStripMenuItem scanItem;
        ToolStripMenuItem disconnectItem;

        BluetoothLEAdvertisementWatcher watcher;
        List<ulong> MACDevices = new List<ulong>();
        BluetoothLEDevice device = null;
        GattDeviceServicesResult deviceServices = null;
        GattCharacteristic selectedChar = null;

        bool autoScroll = false;


        public CC245XTerminal() {
            InitializeComponent();
        }

        private void CC245XTerminal_Layout(object sender, LayoutEventArgs e) {
            rtx_dataRX.Width = Width - 40;
            rtx_dataRX.Height = Height - 105;
            btn_clear.Left = rtx_dataRX.Width - btn_clear.Width + 12;
            btn_copy.Left = btn_clear.Left - btn_send.Width - 6;
            chk_autoScroll.Left = btn_copy.Left - chk_autoScroll.Width - 6;
            btn_send.Left = chk_autoScroll.Left - btn_send.Width - 6;
            txt_dataTX.Width = btn_send.Left - 20;
        }

        private async void btn_send_Click(object sender, EventArgs e) {
            if (selectedChar != null) {
                var writer = new DataWriter();
                writer.WriteString(txt_dataTX.Text);
                try {
                    var status = await selectedChar.WriteValueAsync(writer.DetachBuffer());

                    if (status == GattCommunicationStatus.Success) {
                        addRXText(txt_dataTX.Text, TXMsgCol);
                    }
                }
                catch (System.ObjectDisposedException exc) { }
                catch (System.OperationCanceledException exc) { }
                txt_dataTX.Clear();
            }
        }

        private void scanToolStripMenuItem_Click(object sender, EventArgs e) {
            scanItem.Enabled = false;
            addRXText("Starting BLE scan ...", SysMsgCol);
            MACDevices.Clear();
            connectMenu.DropDownItems.Clear();
            connectMenu.Enabled = false;
            watcher.Start();
        }

        private void addRXText(String txt, Color col) {
            Invoke(new Action<String, Color>((String S, Color col) => {

                int firstLine = SendMessage(rtx_dataRX.Handle, EM_GETFIRSTVISIBLELINE, 0, 0);
                int selectionStart = rtx_dataRX.SelectionStart;
                int selectionLength = rtx_dataRX.SelectionLength;

                SendMessage(rtx_dataRX.Handle, WM_SETREDRAW, (int)IntPtr.Zero, (int)IntPtr.Zero);   // Disabilita il redraw della finestra

                rtx_dataRX.SelectionStart = rtx_dataRX.TextLength;
                rtx_dataRX.SelectionLength = 0;
                rtx_dataRX.SelectionColor = col;
                rtx_dataRX.AppendText(S + Environment.NewLine);
//                rtx_dataRX.SelectionColor = rtx_dataRX.ForeColor;

                if (autoScroll) {
                    rtx_dataRX.ScrollToCaret(); // auto-scroll normale
                }
                else {
                    rtx_dataRX.SelectionStart = selectionStart;
                    rtx_dataRX.SelectionLength = selectionLength;
                    int currentFirstLine = SendMessage(rtx_dataRX.Handle, EM_GETFIRSTVISIBLELINE, 0, 0);
                    int linesToScroll = firstLine - currentFirstLine;
                    SendMessage(rtx_dataRX.Handle, EM_LINESCROLL, 0, linesToScroll);
                }
                SendMessage(rtx_dataRX.Handle, WM_SETREDRAW, (int)new IntPtr(1), (int)IntPtr.Zero); // Riabilita il redraw della finestra
                rtx_dataRX.Invalidate();
            }), txt, col);
        }

        private void btnClear_Click(object sender, EventArgs e) {
            rtx_dataRX.Clear();
        }

        private async Task connectBLE(object sender, EventArgs e) {
            if (watcher.Status == BluetoothLEAdvertisementWatcherStatus.Started)
                watcher.Stop();

            UInt64 devMAC = MACDevices[int.Parse(((ToolStripMenuItem)(sender)).ToString().Split(":")[0])];
            device = await BluetoothLEDevice.FromBluetoothAddressAsync(devMAC);
            if (device == null) {
                addRXText("Unable to connect to device.", ErrMsgCol);
                return;
            }

            deviceServices = await device.GetGattServicesForUuidAsync(SimpleKeyServiceGUID);
            if ((deviceServices == null) || (deviceServices.Status != GattCommunicationStatus.Success) || (deviceServices.Services.Count == 0)) {
                addRXText("Error retrieving GATT services.", ErrMsgCol);
                device.Dispose();
                deviceServices = null;
                device = null;
                return;
            }

            GattDeviceService gatt = deviceServices.Services[0];
            GattCharacteristicsResult charResult = await gatt.GetCharacteristicsForUuidAsync(SimpleKeyStateGUID);
            if ((charResult == null) || (charResult.Status != GattCommunicationStatus.Success) || (charResult.Characteristics.Count == 0)) {
                addRXText("Error retrieving features.", ErrMsgCol);
                device.Dispose();
                gatt.Dispose();
                gatt = null;
                deviceServices = null;
                device = null;
                return;
            }

            selectedChar = charResult.Characteristics[0];
            if ((!selectedChar.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Write)) ||
                 (!selectedChar.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify))) {
                addRXText("Feature error", ErrMsgCol);
                device.Dispose();
                gatt.Dispose();
                selectedChar = null;
                gatt = null;
                deviceServices = null;
                device = null;
                return;
            }

            selectedChar.ValueChanged += OnValueChanged;
            await selectedChar.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);

            addRXText("Connected", SysMsgCol);
            device.ConnectionStatusChanged += BleDevice_ConnectionStatusChanged;
            connectMenu.Enabled = false;
            scanItem.Enabled = false;
            disconnectItem.Enabled = true;
            btn_send.Enabled = !string.IsNullOrWhiteSpace(txt_dataTX.Text);
            return;
        }


        private void BleDevice_ConnectionStatusChanged(BluetoothLEDevice sender, object args) {
            if (sender.ConnectionStatus == BluetoothConnectionStatus.Disconnected) {
                // Esegui azioni di pulizia, aggiornamento UI, ecc.
                _ = deviceDisconnect();
                addRXText("The device has disconnected or is no longer reachable.", ErrMsgCol);
            }
        }


        private void OnValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args) {
            var reader = DataReader.FromBuffer(args.CharacteristicValue);
            string received = reader.ReadString(args.CharacteristicValue.Length);
            addRXText(received, RXMsgCol);
        }


        private void CC245XTerminal_Load(object sender, EventArgs e) {
            scanItem = (ToolStripMenuItem)((ToolStripMenuItem)menu_BLE.Items[0]).DropDownItems[0];
            connectMenu = (ToolStripMenuItem)((ToolStripMenuItem)menu_BLE.Items[0]).DropDownItems[1];
            disconnectItem = (ToolStripMenuItem)((ToolStripMenuItem)menu_BLE.Items[0]).DropDownItems[2];

            watcher = new BluetoothLEAdvertisementWatcher {
                ScanningMode = BluetoothLEScanningMode.Active
            };

            watcher.Received += async (w, btAdv) => {
                var address = btAdv.BluetoothAddress;
                var name = btAdv.Advertisement.LocalName;

                // Evita dispositivi già visti
                if (MACDevices.Contains(address))
                    return;

                String dev = MACDevices.Count + ": " + (string.IsNullOrEmpty(name) ? "(no nome)" : name) + " - ";
                dev += BluetoothAddressToString(address);
                MACDevices.Add(address);
                addRXText(dev, SysMsgCol);

                ToolStripMenuItem newDev = new ToolStripMenuItem(dev);
                newDev.Enabled = false;

                BluetoothLEDevice device = await BluetoothLEDevice.FromBluetoothAddressAsync(address);
                if (device != null) {
                    GattDeviceServicesResult gattServices = await device.GetGattServicesForUuidAsync(SimpleKeyServiceGUID);
                    if ((gattServices != null) && (gattServices.Status == GattCommunicationStatus.Success) && (gattServices.Services.Count > 0)) {
                        GattDeviceService gatt = gattServices.Services[0];
                        GattCharacteristicsResult charResult = await gatt.GetCharacteristicsForUuidAsync(SimpleKeyStateGUID);
                        if ((charResult != null) && (charResult.Status == GattCommunicationStatus.Success) && (charResult.Characteristics.Count > 0)) {
                            GattCharacteristic gattChar = charResult.Characteristics[0];

                            if ((gattChar.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Write)) &&
                                    (gattChar.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify))) {
                                newDev.Enabled = true;
                                newDev.Click += async (s, e) => await connectBLE(s, e);
                            }
                            gatt.Dispose();
                        }
                    }
                    device.Dispose();
                }


                Invoke(new Action<ToolStripMenuItem>((ToolStripMenuItem newDev) => {
                    connectMenu.DropDownItems.Add(newDev);
                }), newDev);
                connectMenu.Enabled = true;
            };

            watcher.Stopped += (w, args) => {
                addRXText("Scanning completed", SysMsgCol);
            };

        }




        string BluetoothAddressToString(ulong address) {
            return string.Format("{0:X2}:{1:X2}:{2:X2}:{3:X2}:{4:X2}:{5:X2}",
                (address >> 40) & 0xFF,
                (address >> 32) & 0xFF,
                (address >> 24) & 0xFF,
                (address >> 16) & 0xFF,
                (address >> 8) & 0xFF,
                address & 0xFF);
        }



        private async void disconnectToolStripMenuItem_Click(object sender, EventArgs e) {
            await deviceDisconnect();
        }

        private async Task deviceDisconnect() {
            if (selectedChar != null) {
                selectedChar.ValueChanged -= OnValueChanged;
                device.ConnectionStatusChanged -= BleDevice_ConnectionStatusChanged;
                try {
                    await selectedChar.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.None);
                }
                catch (System.ObjectDisposedException e) { }


                foreach (GattDeviceService service in deviceServices.Services) {
                    service.Dispose();
                }
            }

            if (device != null) device.Dispose();
            selectedChar = null;
            deviceServices = null;
            device = null;

            connectMenu.Enabled = true;
            scanItem.Enabled = true;
            disconnectItem.Enabled = false;
            addRXText("Disconnected", SysMsgCol);
            btn_send.Enabled = false;
        }


        private void txt_dataTX_KeyPress(object sender, KeyPressEventArgs e) {
            if ((e.KeyChar == (char)Keys.Return) && (!string.IsNullOrWhiteSpace(txt_dataTX.Text))) {
                btn_send_Click(sender, e);
            }
        }

        private void txt_dataTX_TextChanged(object sender, EventArgs e) {
            btn_send.Enabled = (!string.IsNullOrWhiteSpace(txt_dataTX.Text)) && (selectedChar != null);
        }

        private void btnCopy_Click(object sender, EventArgs e) {
            if (rtx_dataRX.SelectionLength == 0) {
                Clipboard.SetText(rtx_dataRX.Text);
            }
            else {
                Clipboard.SetText(rtx_dataRX.SelectedText);
                rtx_dataRX.SelectionLength = 0;
            }

        }

        private void chk_autoScroll_CheckedChanged(object sender, EventArgs e) {
            autoScroll = chk_autoScroll.Checked;
        }

 
        private void rtx_dataRX_MouseDown(object sender, MouseEventArgs e) {
            autoScroll = false; // L’utente sta interagendo
            chk_autoScroll.Checked = false;
        }
    }
}
