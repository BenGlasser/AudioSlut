using System;
using System.Drawing;
using System.Windows.Forms;
using System.Management;
using System.Collections;

namespace AudioSlut
{
    public class AudioSlut : Form
    {
        public static ArrayList devices = new ArrayList();
        [STAThread]
        public static void Main()
        {
            Application.Run(new AudioSlut());
        }
        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;
        public AudioSlut()
        {
        
            // Create a simple tray menu .  
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Exit", OnExit);

            MenuItem playbackMenu = new MenuItem("Playback Devices");
            playbackMenu.MenuItems.AddRange(deviceMenu());
            trayMenu.MenuItems.Add(playbackMenu);
        
            // Create a tray icon. In this example we use a  
            // standard system icon for simplicity, but you  
            // can of course use your own custom icon too. 
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
            ManagementObjectSearcher objSearcher = new ManagementObjectSearcher(
                "SELECT * FROM Win32_SoundDevice");
            ManagementObjectCollection objCollection = objSearcher.Get();
            foreach (ManagementObject obj in objCollection)
            {
                foreach (PropertyData property in obj.Properties)
                {
                    Console.Out.WriteLine(String.Format("{0}:{1}", property.Name, property.Value));
                    if (property.Name == "Name"){
                        devices.Add(property.Value);
                    }
                }
            }
        }
        private MenuItem[] deviceMenu()
        {
            getDevices();
            int i = 0;
            MenuItem[] deviceMenu = new MenuItem[devices.Count];
            foreach(String device in devices)
            {
                deviceMenu[i] = new MenuItem(device);
                ++i;
            }
            return deviceMenu;

        }
        private void setDevice()
        {

        }
    }
}

