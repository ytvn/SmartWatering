using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartWatering.Services
{
    public class SocketServer
    {
        TcpListener server;
        public static Dictionary<int, string> Sockets = new Dictionary<int, string>();

        public static List<Socket> listClient;
        public SocketServer()
        {
            this.Connect();
        }
        void Connect()
        {
            listClient = new List<Socket>();
            server = new TcpListener(IPAddress.Any, 13000);
            Thread listen = new Thread(() =>
            {
                try
                {
                    server.Start();
                    while (true)
                    {
                        Socket client = server.AcceptSocket();
                        listClient.Add(client);
                        send(client, "200OK-Connected");

                        Thread receive = new Thread(Receice);
                        receive.IsBackground = true;
                        receive.Start(client);
                    }
                }
                catch
                {
                    server = new TcpListener(IPAddress.Any, 13000);
                }

            });
            listen.IsBackground = true;
            listen.Start();
        }

        public static void send(Socket client, string Text)
        {
            ASCIIEncoding asen = new ASCIIEncoding();
            if (client != null && Text != string.Empty)
                client.Send(asen.GetBytes(Text));
        }

        public static void Receice(object obj)
        {
            Socket client = (Socket)obj;
            try
            {
                while (true)
                {
                    //Mang chua thong diep tu client
                    byte[] rec = new byte[1024000];
                    // Luu thong diep cua client vao mang byte
                    int k = client.Receive(rec);

                    //Chuyen thong diep tu dang byte sang string
                    string message = "";
                    for (int i = 0; i < k; i++)
                    {
                        message += (Convert.ToChar(rec[i]));
                    }
                    if (Sockets.ContainsValue(message))
                    {
                        var item = Sockets.First(kvp => kvp.Value == message);

                        Sockets.Remove(item.Key);
                        listClient.RemoveAt(item.Key);
                    }
                    Sockets.Add(listClient.Count - 1, message);
                    foreach (var pair in Sockets)
                    {
                        int key = pair.Key;
                        string value = pair.Value;
                        Console.WriteLine(key + "/" + value);
                    }
                }
            }
            catch
            {
                //Neu socket nao loi thi remove khoi list va dong ket noi
                listClient.Remove(client);
                client.Close();
            }
        }
        void close()
        {
            server.Stop();
            foreach (var i in listClient)
                i.Close();


        }

    }
}
