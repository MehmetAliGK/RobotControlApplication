using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net.Http;
using Newtonsoft.Json;  
namespace control
{
    class TCPConnection

    {
        private TcpClient client;
        private static NetworkStream? stream;
        private static TCPConnection? instance;

        // design pattern singleton uygulamasi: uygulama boyunca yalnizca bir örnek
        // olusturulmasini saglar
        public static TCPConnection Instance
        {
            get
            {
                if (instance == null)
                    instance = new TCPConnection();
                return instance;
            }
        }

        private TCPConnection() { }   //Constructor


        // Tcp bağlantısı
        public async Task<bool> Connect(string server, int port)
        {
            try
            {
                client = new TcpClient(server, port);
                stream = client.GetStream();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //robot control data send
        public async Task SendCommandAsync(string type, string command)
        {
            if (stream == null)
            {
                throw new Exception("Network stream is not available");
            }
            try
            {
                var commandData = new { Type = type, Command = command };
                string jsonData = JsonConvert.SerializeObject(commandData);
                byte[] data = Encoding.UTF8.GetBytes(jsonData);
                await stream.WriteAsync(data, 0, data.Length);
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Failed to send command");
            }
        }

        //data receive
        public async Task<T> ReceiveDataAsync<T>()
        {
            byte[] buffer = new byte[4096];
            if (stream != null)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                return JsonConvert.DeserializeObject<T>(receivedData);
            }
            throw new InvalidOperationException("Stream is not available or no data received");
        }

        public void CloseConnection()
        {
            stream?.Close();
            client?.Close();
        }

    }
    public class RobotData
    {
        [JsonProperty("GPS")]
        public string GPS { get; set; }

        [JsonProperty("Distance")]
        public double Distance { get; set; }

        [JsonProperty("Battery")]
        public double Battery { get; set; }
    }

}
