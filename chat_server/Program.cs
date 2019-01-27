using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Net;

namespace chat_server
{
    class Program
    {
        static void Main(string[] args)
        {
            new Server().Start();
        }
    }

    public delegate void BroadcastEvent(object source, BroadcastMessageArgs e);

    public class BroadcastMessageArgs : EventArgs
    {
        public string Message, UserName;
        public bool ShowName;

        public BroadcastMessageArgs(string message, string userName, bool showName)
        {
            Message = message;
            UserName = userName;
            ShowName = showName;
        }
    }

    public class Server
    {
        const int bufferSize = 1024;

        private Hashtable clients = new Hashtable();       

        public void Start()
        {
            TcpClient clientSocket;
            string clientName;
            // numberClients
            int counter = 0;
            // listen for and accept incoming connection requests in blocking synchronous mode.
            var serverSocket = new TcpListener(IPAddress.Loopback, 8888);
            serverSocket.Start();
            Console.WriteLine("Chat Server Started ....");            

            while (true)
            {
                counter += 1;
                // blocking method that returns a TcpClient that you can use to send and receive data
                clientSocket = serverSocket.AcceptTcpClient();
                clientSocket.ReceiveBufferSize = bufferSize;            

                byte[] clientBytes = new byte[clientSocket.ReceiveBufferSize];
                clientName = string.Empty;

                // provides methods for sending and receiving data over Stream sockets in blocking mode
                var networkStream = clientSocket.GetStream();
                // The size of the receive buffer, in bytes. The default value is 8192 bytes.
                networkStream.Read(clientBytes, 0, clientSocket.ReceiveBufferSize);
                // Received Bytes -> Ascii
                clientName = Encoding.ASCII.GetString(clientBytes);
                // Clip String
                clientName = clientName.Substring(0, clientName.IndexOf("$"));

                clients.Add(clientName, clientSocket);
                // Send nameClient to everybody
                BroadcastMesssage(clientName + " Joined ", clientName, false);

                Console.WriteLine(clientName + " Joined chat room ");

                var clientHandler = new ClientHandler(clientSocket, clientName, bufferSize);
                clientHandler.Broadcast += BroadcastEventHandler;
                clientHandler.StartListen();
            }

            clientSocket.Close();
            serverSocket.Stop();
            Console.WriteLine("exit");
            Console.ReadLine();
        }

        private void BroadcastEventHandler(object source, BroadcastMessageArgs e)
        {
            BroadcastMesssage(e.Message, e.UserName, e.ShowName);
        }

        private void BroadcastMesssage(string msg, string uName, bool flag)
        {
            foreach (DictionaryEntry Item in clients)
            {
                TcpClient broadcastSocket;
                broadcastSocket = (TcpClient)Item.Value;
                NetworkStream broadcastStream = broadcastSocket.GetStream();
                Byte[] broadcastBytes = null;

                if (flag == true)
                {
                    broadcastBytes = Encoding.ASCII.GetBytes(uName + " says : " + msg);
                }
                else
                {
                    broadcastBytes = Encoding.ASCII.GetBytes(msg);
                }

                broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                broadcastStream.Flush();
            }
        }
    }

    public class ClientHandler
    {
        private readonly TcpClient clientSocket;
        private readonly string clientName;
        private readonly Thread clientThread;
        private readonly int bufferSize; 

        public event BroadcastEvent Broadcast;

        public ClientHandler(TcpClient inClientSocket, string inClientName, int inBufferSize)
        {
            clientSocket = inClientSocket;
            clientName = inClientName;
            bufferSize = inBufferSize;
            clientThread = new Thread(DoChat);         
        }

        public void StartListen()
        {
            clientThread.Start();
        }

        public void StopListen()
        { 
            clientThread.Abort();
        }

        private void DoChat()
        {          
            byte[] bytesClient = new byte[bufferSize];
            string textClient;           

            while (true)
            {
                try
                {                 
                    // provides methods for sending and receiving data over Stream sockets in blocking mode
                    // This Thread listenes just for this Client on this socket
                    var networkStream = clientSocket.GetStream();
                    clientSocket.ReceiveBufferSize = 512;
                    // if data arrives from client, store in bytesClient
                    networkStream.Read(bytesClient, 0, clientSocket.ReceiveBufferSize);
                    textClient = Encoding.ASCII.GetString(bytesClient);
                    textClient = textClient.Substring(0, textClient.IndexOf("$"));
                    Console.WriteLine("From client - " + clientName + " : " + textClient);

                    // Send textClient to everybody
                    if (Broadcast != null)
                    {
                        Broadcast.Invoke(this, new BroadcastMessageArgs(textClient, clientName, true));
                    }                   
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }     
    }   
}
