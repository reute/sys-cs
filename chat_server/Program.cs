using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Net;

namespace chat_server
{
    static class Program
    {
        public static Hashtable clientsList = new Hashtable();

        static void Main(string[] args)
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

                byte[] clientBytes = new byte[ushort.MaxValue+1];
                clientName = string.Empty;

                // provides methods for sending and receiving data over Stream sockets in blocking mode
                var networkStream = clientSocket.GetStream();
                // The size of the receive buffer, in bytes. The default value is 8192 bytes.
                networkStream.Read(clientBytes, 0, clientSocket.ReceiveBufferSize);
                // Received Bytes -> Ascii
                clientName = Encoding.ASCII.GetString(clientBytes);
                // Clip String
                clientName = clientName.Substring(0, clientName.IndexOf("$"));

                clientsList.Add(clientName, clientSocket);
                // Send nameClient to everybody
                broadcast(clientName + " Joined ", clientName, false);

                Console.WriteLine(clientName + " Joined chat room ");

                var client = new HandleClient();
                client.startClient(clientSocket, clientName);
            }

            clientSocket.Close();
            serverSocket.Stop();
            Console.WriteLine("exit");
            Console.ReadLine();
        }

        public static void broadcast(string msg, string uName, bool flag)
        {
            foreach (DictionaryEntry Item in clientsList)
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

    public class HandleClient
    {
        TcpClient clientSocket;
        string clientName;
        //Hashtable clientsList;

        // 
        public void startClient(TcpClient inClientSocket, string inClientName)
        {
            clientSocket = inClientSocket;
            clientName = inClientName;
            //clientsList = cList;
            Thread clientThread = new Thread(doChat);
            clientThread.Start();
        }

        private void doChat()
        {
            //int requestCount = 0;
            byte[] bytesClient = new byte[ushort.MaxValue+1];
            string textClient;
            //Byte[] sendBytes = null;
            //string serverResponse = null;
            //string rCount;
            //requestCount = 0;

            while (true)
            {
                try
                {
                    //requestCount = requestCount + 1;
                    // provides methods for sending and receiving data over Stream sockets in blocking mode
                    // This Thread listenes jsut for this Client on this socket
                    var networkStream = clientSocket.GetStream();
                    // if data arrives from client, store in bytesClient
                    networkStream.Read(bytesClient, 0, clientSocket.ReceiveBufferSize);
                    textClient = Encoding.ASCII.GetString(bytesClient);
                    textClient = textClient.Substring(0, textClient.IndexOf("$"));
                    Console.WriteLine("From client - " + clientName + " : " + textClient);
                    //rCount = Convert.ToString(requestCount);
                    // Send textClient to everybody
                    Program.broadcast(textClient, clientName, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }     
    }
}
