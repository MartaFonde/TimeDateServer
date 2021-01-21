using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TimeDateServer
{
    class Program
    {
        public static bool connected = true;        

        public static string actions(string msg)
        {
            switch (msg)
            {
                case "DATE":
                    return DateTime.Now.Date.ToString("dd/MM/yyyy");
                case "TIME":
                    return DateTime.Now.ToString("HH:mm");
                case "DATETIME":
                    return DateTime.Now.ToString();
                case "CLOSE":
                    connected = false;
                    return "CLOSE";
                default:
                    return "COMMAND NOT VALID";
            }
        }

        static void Main(string[] args)
        {
            string msg = "";
            int port = 31415;

            IPEndPoint ie = new IPEndPoint(IPAddress.Any, port);
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                s.Bind(ie);
                Console.WriteLine($"Port {port} free");
                s.Listen(5);
            }
            catch (SocketException e) when (e.ErrorCode == (int)SocketError.AddressAlreadyInUse)
            {
                Console.WriteLine($"Port {port} in use");
                connected = false;
            }

            while (connected)
            {
                Socket sClient = s.Accept();
                IPEndPoint ieClient = (IPEndPoint)sClient.RemoteEndPoint;
                Console.WriteLine("Client connected: {0} at port {1}", ieClient.Address, ieClient.Port);

                using (NetworkStream ns = new NetworkStream(sClient))
                using (StreamReader sr = new StreamReader(ns))
                using (StreamWriter sw = new StreamWriter(ns))
                {
                    try
                    {
                        msg = sr.ReadLine();
                        Console.WriteLine(msg);
                        if (msg != null)
                        {
                            string a = actions(msg);
                            Console.WriteLine(a);
                            sw.WriteLine(a);
                            sw.Flush();
                        }
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    Console.WriteLine("Client disconnected");
                }
                sClient.Close();
            }
        }        
    }
}

