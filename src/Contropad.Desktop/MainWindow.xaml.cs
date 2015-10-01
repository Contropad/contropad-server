using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Contropad.Core.Joystick;
using Contropad.Core.Streams;
using Contropad.Webclient.Service;
using System.Windows.Controls;
using ContextMenu = System.Windows.Controls.ContextMenu;
using MenuItem = System.Windows.Controls.MenuItem;

namespace Contropad.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private JoystickFeeder _feeder;
        private WebsocketStream _websocketStream;
        private JoystickStreamHandler _streamHandler;
        private WebclientServer _httpServer;
        private System.Windows.Forms.NotifyIcon _icon;

        protected string SelectedIP { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            _icon = new System.Windows.Forms.NotifyIcon();
            _icon.Click += new EventHandler(notifyIcon_Click);
            _icon.Icon = System.Drawing.Icon.FromHandle(Properties.Resources.joystick.GetHicon());
            _icon.Text = "Contropad Server";
        }

        private void notifyIcon_Click(object sender, EventArgs e)
        {
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadNetworkInterfaces();
        }

        private void LoadNetworkInterfaces()
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                .SelectMany(adapter => adapter.GetIPProperties().UnicastAddresses)
                .Where(adr => adr.Address.AddressFamily == AddressFamily.InterNetwork && adr.IsDnsEligible)
                .Select(adr => adr.Address.ToString());

            foreach (var ip in interfaces)
            {
                listBox.Items.Add(ip);
            }
        }

        private void StartServer(string ip)
        {
            _feeder = new JoystickFeeder();
            _httpServer = new WebclientServer("http://" + ip + ":1236");
            _websocketStream = new WebsocketStream("ws://" + ip + ":8181");
            _streamHandler = new JoystickStreamHandler(_feeder);
            _websocketStream.CreateStream().Subscribe(_streamHandler);
            _websocketStream.Start();
            _httpServer.Start();
            _feeder.StartFeeding(CancellationToken.None);
            Process.Start("http://" + ip + ":1236/controller/1");
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var selectedIp = (string)listBox.SelectedValue;
            StartServer(selectedIp);
        }
    }
}
