using System;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using Contropad.Core.Joystick;
using Contropad.Core.Streams;
using Contropad.Webclient.Service;

namespace Contropad.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private JoystickFeeder _feeder;
        private WebsocketStream _websocketStream;
        private JoystickStreamHandler _streamHandler;
        private WebclientServer _httpServer;
        private readonly System.Windows.Forms.NotifyIcon _icon;

        public MainWindow()
        {
            InitializeComponent();

            _icon = new System.Windows.Forms.NotifyIcon
            {
                Icon = System.Drawing.Icon.FromHandle(Properties.Resources.joystick.GetHicon()),
                Text = "Contropad Server"
            };
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
