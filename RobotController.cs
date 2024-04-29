using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Net.Sockets;

namespace control
{
    class RobotController
    {
        //TCP komutları ("TPYE", "COMMAND")
        public async void Forward()
        {                
            //await TCPConnection.Instance.SendCommandAsync("robot_move","forward");
        }
        public async void Back()
        {
            //await TCPConnection.Instance.SendCommandAsync("robot_move","back");
        }
        public async void TurnRight()
        {
            //await TCPConnection.Instance.SendCommandAsync("robot_move","turn_right");
        }
        public async void TurnLeft()
        {
            //await TCPConnection.Instance.SendCommandAsync("robot_move","turn_left");

        }
        public async void Stop()
        {
            //await TCPConnection.Instance.SendCommandAsync("robot_move","stop");
        }

    }
}
