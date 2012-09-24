using System;
using System.Drawing;
using System.Windows.Forms;
using System.Management;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices; 
using System.ComponentModel;
using System.Text;


namespace AudioSlut
{
    public class AudioSlut : Form
    {
        private static Int32 METHOD_BUFFERED = 0;
        private static Int32 FILE_ANY_ACCESS = 0;
        private static Int32 FILE_DEVICE_HAL = 0x00000101;

        private const Int32 ERROR_NOT_SUPPORTED = 0x32;
        private const Int32 ERROR_INSUFFICIENT_BUFFER = 0x7A;

        private static Int32 IOCTL_HAL_GET_DEVICEID =
            ((FILE_DEVICE_HAL) << 16) | ((FILE_ANY_ACCESS) << 14)
            | ((21) << 2) | (METHOD_BUFFERED);

        [DllImport("coredll.dll", SetLastError = true)]
        private static extern bool KernelIoControl(Int32 dwIoControlCode,
            IntPtr lpInBuf, Int32 nInBufSize, byte[] lpOutBuf,
            Int32 nOutBufSize, ref Int32 lpBytesReturned);
        public static ArrayList devices = new ArrayList();

        [STAThread]
        public static void Main()
        {
            Application.Run(new AudioSlut());
        }
        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;
        private MenuItem playbackMenu;
        public AudioSlut()
        {

            // Create a simple tray menu .  
            trayMenu = new ContextMenu();
            playbackMenu = new MenuItem("Playback Devices", selectPlayback);
            playbackMenu.MenuItems.AddRange(deviceMenu());
            trayMenu.MenuItems.Add(playbackMenu);
            for (int i = 0; i < playbackMenu.MenuItems.Count; ++i)
            {
                playbackMenu.MenuItems[i].Click += new EventHandler(selectPlayback);
            }

            trayMenu.MenuItems.Add("Exit", OnExit);


            // Create a tray icon.  
            trayIcon = new NotifyIcon();
            trayIcon.Text = "AudioSlut";
            trayIcon.Icon = new Icon("../../Resources/speaker.ico", 40, 40);
            // Add menu to tray icon and show it.
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;
        }
        protected override void OnLoad(EventArgs e)
        {
            Visible = false; // Hide form window.           
            ShowInTaskbar = false; // Remove from taskbar. 
            base.OnLoad(e);
        }
        private void selectPlayback(object sender, EventArgs e)
        {
            MenuItem item = sender as MenuItem;
            if (item != null)
            {
                int index = (item.MenuItems.IndexOf(item));
                setDevice(index.ToString());
                Console.WriteLine(index.ToString());
            }

        }
        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }
        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                // Release the icon resource.
                trayIcon.Dispose();
            }
            base.Dispose(isDisposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // AudioSlut
            // 
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Name = "AudioSlut";
            this.Load += new System.EventHandler(this.AudioSlut_Load);
            this.ResumeLayout(false);

        }

        private void AudioSlut_Load(object sender, EventArgs e)
        {

        }
        private void getDevices()
        {
            devices = new ArrayList(Win32.GetSoundDevices());
        }
        private MenuItem[] deviceMenu(){
            getDevices();
            int i = 0;
            MenuItem[] deviceMenu = new MenuItem[devices.Count];
            foreach (String device in devices)
            {
                deviceMenu[i] = new MenuItem(device);
                ++i;
            }
            return deviceMenu;
        }
        private void setDevice(String newDeviceID)
        {
            // Set newDeviceID here, by some means. A common scenario
            // is to save the value using the registry so it can be toggled.

            Process myProc = Process.Start(new ProcessStartInfo("EndPointController.exe", newDeviceID.ToString())
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            });
        }
    }
}
