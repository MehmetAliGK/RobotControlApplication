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
using System.IO;
using Xamarin.Forms;
using Microsoft.VisualBasic;

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
        private HashSet<Key> pressedKeys = new HashSet<Key>();  // MAGOK: Birden fazla tusa basilma kontrolü ?
        private DispatcherTimer _dataPollingTimer;
        private DispatcherTimer _dispatcherTimer;
        private int _currentIndex = 0;
        private StringBuilder _dataBuffer = new StringBuilder();
        private LibVLC _libVLC;
        private MediaPlayer _mediaPlayer;
        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
            InitializeMap();
            Loaded += MainWindow_Loaded;

            //_videoStreamManager = new VideoStreamManager();
            //videoView.MediaPlayer = _videoStreamManager.MediaPlayer;
            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC)
            {
                AspectRatio = "16:9"       //MAGOK: video kenar boşlukları icin
            };
            videoView.MediaPlayer = _mediaPlayer;

            _robotController = new RobotController();
            _cameraController = new CameraController();

        }
      
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Core.Initialize();

            this.KeyDown += MainWindow_KeyDown;
            this.KeyUp += MainWindow_KeyUp;
            //_videoStreamManager.StreamManager();
        }
        private async void TcpConnectButton_Click(object sender, RoutedEventArgs e)
        {
            bool isConnected = await TCPConnection.Instance.Connect("192.168.19.243", 5000);
            if (isConnected)
            {
                _dataPollingTimer = new DispatcherTimer()
                {
                    Interval = TimeSpan.FromSeconds(1),
                };
                _dataPollingTimer.Tick += DataPollingTimer_Tick;
                _dataPollingTimer.Start();
                MessageBox.Show("CONNECTION SUCCESFULL");
            }

            else
            {
                MessageBox.Show("CONNNECTION FAILED");
            }
        }

        private void InitializeMap()
        {
            Map.MapProvider = GMapProviders.OpenStreetMap;
            Map.MinZoom = 5;
            Map.MaxZoom = 100;
            Map.Zoom = 15;


            //Map.Position = new PointLatLng(54.6961334816182, 25.2985095977783);

            //var marker = new GMapMarker(new PointLatLng(54.6961334816182, 25.2985095977783))
            //{
            //    Shape = new Ellipse
            //    {
            //        Width = 10,
            //        Height = 10,
            //        Stroke = Brushes.Red,
            //        StrokeThickness = 1.5
            //    }
            //};
            //Map.Markers.Add(marker);
        }
        public void UpdateMapPosition(double lat, double lng)
        {
            Dispatcher.Invoke(() =>
            {
                Map.Position = new PointLatLng(lat, lng);

                if (Map.Markers.Count > 0)
                {
                    Map.Markers[0].Position = new PointLatLng(lat, lng);
                }
                else
                {
                    var marker = new GMapMarker(new PointLatLng(lat, lng))
                    {
                        Shape = new Ellipse
                        {
                            Width = 10,
                            Height = 10,
                            Stroke = Brushes.Red,
                            StrokeThickness = 2,
                        }
                    };
                }

            });
           
        }

        private async void DataPollingTimer_Tick(object sender, EventArgs e)
        {
            await DataFromServer();            
        }

        private async Task DataFromServer()
        {
            try
            {
                var robotData = await TCPConnection.Instance.ReceiveDataAsync<RobotData>();

                Dispatcher.Invoke(() =>
                {
                    
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

        private readonly (int Time, double Voltage)[] _batteryData = new[]
        {
            //MAGOK: Data Structure -> (time,voltage)
            (0,11.1),
            (2,10.93),
            (4,10.91),
            (6,10.89),
            (8,10.88),
            (10,10.87),
            (15,10.85),
            (45,10.76)
        };
        private void InitializeTimer()
        {
            _dispatcherTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(_batteryData[_currentIndex].Time)
            };
            _dispatcherTimer.Tick += Timer_Tick;
            _dispatcherTimer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_currentIndex < _batteryData.Length)
            {
                double voltage = _batteryData[_currentIndex].Voltage;
                progressBar.Value = VoltageToPercentage(voltage);
                string formattedValue = VoltagePercentage(voltage);
                batteryStatus.Text = $"%{formattedValue}";
                UpdateProgressBarColor();

                _currentIndex++;
                if (_currentIndex < _batteryData.Length)
                {
                    _dispatcherTimer.Interval = TimeSpan.FromMinutes(_batteryData[_currentIndex].Time - _batteryData[_currentIndex - 1].Time);
                }
                else
                {
                    _dispatcherTimer.Stop();
                    MessageBox.Show("Battery level update finished.");
                }
            }
        }
        private double VoltageToPercentage(double voltage)
        {
            double maxVoltage = 11.1;
            return (voltage / maxVoltage) * 100;
        }
        private string VoltagePercentage(double voltage)
        {
            double maxVoltage = 11.1;
            string percentage = Convert.ToString((voltage / maxVoltage) * 100);
            //String.Format("{0.F2}", percentage);
            return percentage;
        }

        private void UpdateProgressBarColor()
        {
            if (progressBar.Value > 50)
            {
                progressBar.Foreground = new SolidColorBrush(Colors.Green);
            }
            else if (progressBar.Value > 20)
            {
                progressBar.Foreground = new SolidColorBrush(Colors.Yellow);
            }
            else
            {
                progressBar.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        private async void btnStart_Click(object sender, RoutedEventArgs e)
        {
            GpsDefaultImage.Visibility = Visibility.Collapsed;
            Map.Visibility = Visibility.Visible;
            //UpdateMapPosition(39.74875, 30.47566);
            CameraDefaultImage.Visibility = Visibility.Collapsed;
            //_videoStreamManager.Play("rtsp://192.168.1.13:8080/stream1");
            var media = new Media(_libVLC, new Uri("http://192.168.19.243:5000/video_feed"));
            _mediaPlayer.Play(media);
            videoView.Visibility = Visibility.Visible;
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            CameraDefaultImage.Visibility = Visibility.Visible;
            Map.Visibility = Visibility.Collapsed;
            GpsDefaultImage.Visibility = Visibility.Visible;
            //_videoStreamManager.Stop();
            _mediaPlayer.Stop();
        }
        private async void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int valueSlider = (int)robotControlSlider.Value;
            string value = valueSlider.ToString();

            try
            {
                await TCPConnection.Instance.SendCommandAsync(
                    type: "robot_move",
                    command: "move",
                    Y: 1.0,
                    X: (valueSlider - 50.0) / 50.0);
            }
            catch (Exception ex)
            {
                MessageBox.Show("There is no Connection. Check the connection and try again.");
            }

        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (pressedKeys.Contains(e.Key)) return;
            pressedKeys.Add(e.Key);
            switch (e.Key)
            {
                //robot control
                case Key.W:
                    _robotController.Forward();                   
                    ForwardButton.Background = new SolidColorBrush(Colors.Green);
                    break;

                case Key.S:
                    _robotController.Backward();
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

        // MAGOK: uygulamanin hemen kapanmaması icin
        public partial class App : System.Windows.Application
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