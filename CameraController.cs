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
            await TCPConnection.Instance.SendCommandAsync("camera_move", "axis,direction", 1, 1);
        }
        public async void Down()
        {
            await TCPConnection.Instance.SendCommandAsync("camera_move", "axis,direction", 1, 0);
        }
        public async void Right()
        {
            await TCPConnection.Instance.SendCommandAsync("camera_move","axis,direction",1,1);
        }
        public async void Left()
        {
            await TCPConnection.Instance.SendCommandAsync("camera_move", "axis,direction", 1, 0);
        }
        public async void Reset()
        {

        }

    }
}

