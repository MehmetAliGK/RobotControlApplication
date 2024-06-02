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
        //MAGOK: TCP komutları ("TPYE", "COMMAND")
        public async void Forward()
        {                
            await TCPConnection.Instance.SendCommandAsync("robot_move","forward");
        }
        public async void Backward()
        {
            await TCPConnection.Instance.SendCommandAsync("robot_move","backward");
        }
        public async void TurnRight()
        {
            await TCPConnection.Instance.SendCommandAsync("robot_move","right");
        }
        public async void TurnLeft()
        {
            await TCPConnection.Instance.SendCommandAsync("robot_move","left");
        }
        public async void Stop()
        {
            await TCPConnection.Instance.SendCommandAsync("robot_move","stop");
        }
    }
}
