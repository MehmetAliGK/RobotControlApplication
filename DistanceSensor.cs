using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace control
{
    internal class DistanceSensor
    {
        public event Action<string> ? OnDistanceDataReceived;

        public async Task StartReading()
        {
            //while (true) // veya durdurma koşuluna kadar döngüye devam edin
            //{
            //    try
            //    {
            //        Packet data = await TCPConnection.Instance.ReceivePacketAsync();
            //        if (data != null && !string.IsNullOrEmpty(data.Distance))
            //        {
            //            OnDistanceDataReceived?.Invoke(data.Distance);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        // Hata durumunda uygun bir işlem yapın
            //        // Örneğin: Loglama, kullanıcıya bilgi verme, yeniden bağlantı denemesi vs.
            //    }
            //}
        }

    }
}
