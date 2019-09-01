using System.Net.Sockets;
using System;
using SimpleTCP;

namespace rlsensor_emu
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Sensordata!");

            var server = new SimpleTcpServer().Start(8910);

            Console.Write("Listen on ... ");
            foreach(var ip in server.GetListeningIPs())
            {
                if(ip.AddressFamily == AddressFamily.InterNetwork)
                    Console.Write($"{ip} ");
            }
            Console.WriteLine();

            server.ClientConnected += (s, e) =>
            {
                Console.WriteLine($"Client connected {e.Client.RemoteEndPoint}");
            };

            server.DataReceived += (s, e) =>
            {
                Console.WriteLine($"Client data {e.MessageString}");
            };

            server.Delimiter = (byte)'\n';

            var timer = new System.Timers.Timer(2000);
            timer.Elapsed += (s, e) =>
            {
                if(server.IsStarted)
                {
                    if(server.ConnectedClientsCount > 0)
                    {
                        //Console.WriteLine("Send Data ...");
                        SendDummyData(server);
                    }
                    else
                    {
                        Console.WriteLine("No Client connected ...");                        
                    }
                }
            };

            timer.Start();            

            while(true)
                System.Threading.Thread.Sleep(1000);
        }

        static void SendDummyData(SimpleTcpServer server)
        {
            var data = System.IO.File.ReadAllBytes("clima_sensor_d.txt");
            server.Broadcast(data);
        }
    }
}
