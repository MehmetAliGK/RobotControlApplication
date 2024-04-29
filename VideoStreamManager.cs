using LibVLCSharp.Shared;
using System.Diagnostics;
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
using System.Xml.Serialization;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace control
{
    class VideoStreamManager
    {
        private LibVLC? _libVLC;
        private MediaPlayer? _mediaPlayer;

        public void StreamManager()
        {
            Core.Initialize();
            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);
        }

        public MediaPlayer MediaPlayer => _mediaPlayer;

        public void Play(string uri)
        {
            var media = new Media(_libVLC, new Uri(uri));
            _mediaPlayer.Play(media);
        }

        public void Stop() 
        {
            _mediaPlayer.Stop();
        }

    }
}
