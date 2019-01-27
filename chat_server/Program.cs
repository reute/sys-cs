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

    public delegate void MessageEvent(object source, MessageArgs e);
    public delegate void DisconnectEvent(object source, EventArgs e);

    public class MessageArgs : EventArgs
    {
        public string Message, UserName;
        public bool ShowName;

        public MessageArgs(string message, string userName, bool showName)
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

                byte[] incomingByteStream = new byte[clientSocket.ReceiveBufferSize];
                clientName = string.Empty;

                // provides methods for sending and receiving data over Stream sockets in blocking mode
                var networkStream = clientSocket.GetStream();
                // The size of the receive buffer, in bytes. The default value is 8192 bytes.
                networkStream.Read(incomingByteStream, 0, clientSocket.ReceiveBufferSize);
                // Received Bytes -> Ascii
                clientName = Encoding.ASCII.GetString(incomingByteStream);
                // Clip String
                clientName = clientName.Substring(0, clientName.IndexOf("$"));              
               
                clients.Add(clientName, clientSocket);
                // Send nameClient to everybody  
                var message = clientName + " joined chat room";
                BroadcastMesssage(message);
                Console.WriteLine(message);

                var clientHandler = new ClientHandler(clientSocket, clientName, bufferSize);
                clientHandler.Message += MessageEventHandler;
                clientHandler.Disconnect += DisconnectEventHandler;
                clientHandler.StartListen();
            }

            clientSocket.Close();
            serverSocket.Stop();            
        }

        private void DisconnectEventHandler(object source, EventArgs e)
        {
            ClientHandler handler = source as ClientHandler;
            var clientName = handler.clientName;
            if (clients.Contains(clientName))
            {
                clients.Remove(clientName);
            }
            // Send nameClient to everybody
            var message = clientName + " left chat room";
            BroadcastMesssage(message);
            Console.WriteLine(message);

            handler.Message -= MessageEventHandler;
            handler.Disconnect -= DisconnectEventHandler;
        }

        private void MessageEventHandler(object source, MessageArgs e)
        {
            BroadcastMesssage(e.Message, e.UserName, e.ShowName);
        }     

        private void BroadcastMesssage(string msg, string uName = "", bool flag = false)
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
        public readonly string clientName;
        private readonly Thread clientHandlerThread;
        private readonly int bufferSize; 

        public event MessageEvent Message;
        public event DisconnectEvent Disconnect;

        public ClientHandler(TcpClient inClientSocket, string inClientName, int inBufferSize)
        {
            clientSocket = inClientSocket;
            clientName = inClientName;
            bufferSize = inBufferSize;
            clientHandlerThread = new Thread(ListenToClient);         
        }

        public void StartListen()
        {
            clientHandlerThread.Start();
        }

        public void StopListen()
        { 
            clientHandlerThread.Abort();
        }

        private void ListenToClient()
        {          
            byte[] bytesClient = new byte[bufferSize];
            string textClient;  
           
            try
            {
                while (true)
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
                    if (Message != null)
                    {
                        Message.Invoke(this, new MessageArgs(textClient, clientName, true));
                    }
                }
            }
            catch (Exception ex)
            {
                if (Disconnect != null)
                {
                    Disconnect.Invoke(this, new EventArgs());
                }
                Console.WriteLine(ex.ToString());
            }            
        }     
    }   
}
