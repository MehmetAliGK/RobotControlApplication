using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

namespace control
{
    class CameraController
    {
        // MAGOK: TCP komutları ("TPYE", "COMMAND")
        public async void Up()
        {
            await TCPConnection.Instance.SendCommandAsync("camera_move","up");
        }
        public async void Down()
        {
            await TCPConnection.Instance.SendCommandAsync("camera_move","down");
        }
        public async void Right()
        {
            await TCPConnection.Instance.SendCommandAsync("camera_move","right");
        }
        public async void Left()
        {
            await TCPConnection.Instance.SendCommandAsync("camera_move","left");
        }
        public async void Reset()
        {
            await TCPConnection.Instance.SendCommandAsync("camera_move","reset");
        }
        public async void UpLeft()
        {
            await TCPConnection.Instance.SendCommandAsync("camera_move","upleft");
        }
        public async void UpRight()
        {
            await TCPConnection.Instance.SendCommandAsync("camera_move","upright");
        }
        public async void DownLeft()
        {
            await TCPConnection.Instance.SendCommandAsync("camera_move","downleft");
        }
        public async void DownRight()
        {
            await TCPConnection.Instance.SendCommandAsync("camera_move","downright");
        }

    }
}

