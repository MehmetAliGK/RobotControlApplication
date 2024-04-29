using LibVLCSharp.Shared;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace control
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private VideoStreamManager _videoStreamManager;
        private RobotController _robotController;
        private CameraController _cameraController;
        private HashSet<Key> pressedKeys = new HashSet<Key>();  // Birden fazla tusa basildi mi?
        private DispatcherTimer _dataPollingTimer;

        public MainWindow()
        {
            InitializeComponent();
            InitializeMap();
            Loaded += MainWindow_Loaded;

            _videoStreamManager = new VideoStreamManager();
            videoView.MediaPlayer = _videoStreamManager.MediaPlayer;

            _robotController = new RobotController();
            _cameraController = new CameraController();

        }
      
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Core.Initialize();

            this.KeyDown += MainWindow_KeyDown;
            this.KeyUp += MainWindow_KeyUp;
            _videoStreamManager.StreamManager();
        }
        private void InitializeMap()
        {
            Map.MapProvider = GMapProviders.OpenStreetMap;
            Map.MinZoom = 5;
            Map.MaxZoom = 100;
            Map.Zoom = 15;


            Map.Position = new PointLatLng(54.6961334816182, 25.2985095977783);

            var marker = new GMapMarker(new PointLatLng(54.6961334816182, 25.2985095977783))
            {
                Shape = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Stroke = Brushes.Red,
                    StrokeThickness = 1.5
                }
            };
            Map.Markers.Add(marker);
        }
        public void UpdateMapPosition(double lat, double lng)
        {
            Map.Position = new PointLatLng(lat, lng);

            if (Map.Markers.Count > 0)
            {
                Map.Markers[0].Position = new PointLatLng(lat, lng);
            }
        }
        private async void TcpConnectButton_Click(object sender, RoutedEventArgs e)
        {
            bool isConnected = await TCPConnection.Instance.Connect("raspberry_pi_ip", 123); //raspberry'e göre düzenle
            if (isConnected)
            {

                _dataPollingTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(10)  // 10 saniyelik aralıkla deger al
                };

                _dataPollingTimer.Tick += DataPollingTimer_Tick;  
                _dataPollingTimer.Start();
                MessageBox.Show("Connection successful!");
            }
            else
            {
                MessageBox.Show("Connection failed.");
            }

        }

        private async void DataPollingTimer_Tick(object sender, EventArgs e)
        {
            try
            {      
                var robotData = await TCPConnection.Instance.ReceiveDataAsync<RobotData>();

                Dispatcher.Invoke(() =>
                {
                    
                    progressBar.Value = robotData.Battery;
                    batteryStatus.Content = robotData.Battery;
                    distanceSensor.Content = robotData.Distance;
                    
                });
            }
            catch (Exception ex)
            {
                _dataPollingTimer.Stop();
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Error receiving data: {ex.Message}");
                });
            }
        }
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            GpsDefaultImage.Visibility = Visibility.Collapsed;
            Map.Visibility = Visibility.Visible;
            UpdateMapPosition(39.74875, 30.47566);
            CameraDefaultImage.Visibility = Visibility.Collapsed;
            //_videoStreamManager.Play("rtsp://192.168.1.13:8080/stream1");
            
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            CameraDefaultImage.Visibility = Visibility.Visible;
            //_videoStreamManager.Stop();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
        
            switch (e.Key)
            {
                //robot control
                case Key.W:
                    _robotController.Forward();
                    ForwardButton.Background = new SolidColorBrush(Colors.Green);
                    break;

                case Key.S:
                    _robotController.Back();
                    BackButton.Background = new SolidColorBrush(Colors.Green);
                    break;

                case Key.A:
                    _robotController.TurnLeft();
                    LeftButton.Background = new SolidColorBrush(Colors.Green);
                    break;

                case Key.D:
                    _robotController.TurnRight();
                    RightButton.Background = new SolidColorBrush(Colors.Green);
                    break;

                case Key.Space:
                    _robotController.Stop();
                    StopButton.Background = new SolidColorBrush(Colors.Green);
                    break;

                //camera control
                case Key.Up:
                    _cameraController.Up();
                    CamUpButton.Background = new SolidColorBrush(Colors.Red);
                    break;

                case Key.Down:
                    _cameraController.Down();
                    CamDownButton.Background = new SolidColorBrush(Colors.Red);
                    break;
                    
                case Key.Left:
                    _cameraController.Right();
                    CamLeftButton.Background = new SolidColorBrush(Colors.Red);
                    break;
                    
                case Key.Right:
                    _cameraController.Left();
                    CamRightButton.Background = new SolidColorBrush(Colors.Red);
                    break;
                case Key.NumPad0:
                    _cameraController.Reset();
                    ResetButton.Background = new SolidColorBrush(Colors.Red);
                    break;
            }
            
            pressedKeys.Add(e.Key);

            if (pressedKeys.Contains(Key.Up) && pressedKeys.Contains(Key.Left))
            {
                _cameraController.UpLeft();
                var defaultColorCameraButton = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFA4CCFD");
                CamUpButton.Background = defaultColorCameraButton;
                CamLeftButton.Background = defaultColorCameraButton;
                UpLeftButton.Background = new SolidColorBrush(Colors.Red);
            }
            if (pressedKeys.Contains(Key.Up) && pressedKeys.Contains(Key.Right))
            {
                _cameraController.UpRight();
                var defaultColorCameraButton = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFA4CCFD");
                CamUpButton.Background = defaultColorCameraButton;
                CamRightButton.Background = defaultColorCameraButton;
                UpRightButton.Background = new SolidColorBrush(Colors.Red);
            }
            if (pressedKeys.Contains(Key.Down) && pressedKeys.Contains(Key.Right))
            {
                _cameraController.DownRight();
                var defaultColorCameraButton = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFA4CCFD");
                CamDownButton.Background = defaultColorCameraButton;
                CamRightButton.Background = defaultColorCameraButton;
                DownRightButton.Background = new SolidColorBrush(Colors.Red);
            }
            if (pressedKeys.Contains(Key.Down) && pressedKeys.Contains(Key.Left))
            {
                _cameraController.DownLeft();
                var defaultColorCameraButton = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFA4CCFD");
                CamDownButton.Background = defaultColorCameraButton;
                CamLeftButton.Background = defaultColorCameraButton;
                DownLeftButton.Background = new SolidColorBrush(Colors.Red);
            }
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            
            var defaultColorRobotButton = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFAACCDD");
            ForwardButton.Background = defaultColorRobotButton;
            BackButton.Background = defaultColorRobotButton;
            RightButton.Background = defaultColorRobotButton;
            LeftButton.Background = defaultColorRobotButton;
            StopButton.Background = defaultColorRobotButton;

            var defaultColorCameraButton = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFA4CCFD");
            CamUpButton.Background = defaultColorCameraButton;
            CamDownButton.Background = defaultColorCameraButton;
            CamLeftButton.Background = defaultColorCameraButton;
            CamRightButton.Background = defaultColorCameraButton;
            ResetButton.Background = defaultColorCameraButton;

            var defaultColorCrossButton = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFC3D0D3");

            if(!(pressedKeys.Contains(Key.Up) && pressedKeys.Contains(Key.Left))) 
            {
                UpLeftButton.Background = defaultColorCrossButton;
                pressedKeys.Remove(e.Key);
            }
            if (!(pressedKeys.Contains(Key.Up) && pressedKeys.Contains(Key.Right)))
            {
                UpRightButton.Background = defaultColorCrossButton;
                pressedKeys.Remove(e.Key);
            }
            if (!(pressedKeys.Contains(Key.Down) && pressedKeys.Contains(Key.Right)))
            {
                DownRightButton.Background = defaultColorCrossButton;
                pressedKeys.Remove(e.Key);
            }
            if (!(pressedKeys.Contains(Key.Down) && pressedKeys.Contains(Key.Left)))
            {
                DownLeftButton.Background = defaultColorCrossButton;
                pressedKeys.Remove(e.Key);
            }
        }


        private void MediaPlayer_EncounteredError(object? sender, EventArgs e)
        {
            var mediaPlayer = sender as MediaPlayer;
            if (mediaPlayer != null)
            {
                Debug.WriteLine("MediaPlayer encountered an error.");
            }
            MessageBox.Show("An error occurred while trying to play the media.", "Media Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        // uygulamanin hemen kapanmaması icin
        public partial class App : Application
        {
            public App()
            {
                Core.Initialize();
                this.DispatcherUnhandledException += OnDispatcherUnhandledException;
            }

            private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
            {
               
                MessageBox.Show("An unhandled exception occurred: " + e.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                e.Handled = true; 
            }
        }

    }    
}