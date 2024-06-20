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
            await TCPConnection.Instance.SendCommandAsync("axis","direction",0,1);
        }
        public async void Down()
        {
            await TCPConnection.Instance.SendCommandAsync("camera_move","down",0,-1);
        }
        public async void Right()
        {
            await TCPConnection.Instance.SendCommandAsync("camera_move","right",1,0);
        }
        public async void Left()
        {
            await TCPConnection.Instance.SendCommandAsync("camera_move","left",-1,0);
        }
        public async void Reset()
        {
            await TCPConnection.Instance.SendCommandAsync("camera_move","reset",0,0);
        }
        public async void UpLeft()
        {
            await TCPConnection.Instance.SendCommandAsync("camera_move","upleft",-1,1);
        }
        public async void UpRight()
        {
            await TCPConnection.Instance.SendCommandAsync("camera_move","upright",1,1);
        }
        public async void DownLeft()
        {
            await TCPConnection.Instance.SendCommandAsync("camera_move","downleft",-1,-1);
        }
        public async void DownRight()
        {
            await TCPConnection.Instance.SendCommandAsync("camera_move","downright",1,-1);
        }

    }
}

