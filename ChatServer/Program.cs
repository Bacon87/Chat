using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Chat.Model;

namespace ChatServer {
    class Program {
        static void Main(string[] args) {
            string input = default(string);

            CancellationTokenSource src = new CancellationTokenSource();
            Task listener = StartListener(src.Token);

            do {
                Console.Write(">");
                input = Console.ReadLine();
            } while (!input.ToUpper().Equals("EXIT"));

            src.Cancel();
            listener.Wait();

            
        }

        private static Task StartListener(CancellationToken token) {
            return Task.Factory.StartNew(() => {
                TcpListener listener = new TcpListener(IPAddress.Any, 5555);

                listener.Start();

                while (!token.IsCancellationRequested) {
                    if (listener.Pending()) {
                        TcpClient remoteClient = listener.AcceptTcpClient();
                        IPEndPoint remoteEndpoint = remoteClient.Client.RemoteEndPoint as IPEndPoint;

                        if (remoteEndpoint != null) {
                            Console.WriteLine($"Message sent from {remoteEndpoint.Address.ToString()}");
                            Console.Write(">");

                            string message = default(string);
                            
                            XmlSerializer serializer = new XmlSerializer(typeof(Message));
                            using (Stream nStream = remoteClient.GetStream()) 
                            using (StreamReader sr = new StreamReader(nStream)) {
                                message = sr.ReadToEnd();
                            }

                            using (UdpClient udpClient = new UdpClient()) {
                                byte[] msgBytes = ASCIIEncoding.ASCII.GetBytes(message);
                                udpClient.Send(msgBytes, msgBytes.Length, new IPEndPoint(IPAddress.Broadcast, 5556));

                                Console.WriteLine($"Message from {remoteEndpoint.Address.ToString()} broadcasted on port 5556");
                                Console.Write(">");
                            }
                        }
                    }
                }
            }, token);
        }
    }
}
