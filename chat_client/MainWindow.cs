using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using Terminal.Gui;
using System.Net;

namespace chat_client
{
    public class MainWindow
    {
        TextField tfChatMessage, tfUserName;
        TextView tvChat;
        Button btnConnect, btnDisconnect, btnSend;
        Window win;
        Label tfUserNameLabel;
        Thread ctThread; 
        TcpClient clientSocket;
        NetworkStream networkStream = default(NetworkStream);       

        public void Init()
        {
            Application.Init();     

            win = new Window("Chat Client")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1
            };
            Application.Top.Add(win);

            tfUserNameLabel = new Label("Chat Name")
            {
                X = Pos.At(1),
                Y = Pos.At(1),       
            };
            win.Add(tfUserNameLabel);

            tfUserName = new TextField("")
            {
                X = Pos.At(15),
                Y = Pos.At(1),
                Width = 20         
            };
            win.Add(tfUserName);

            btnConnect = new Button("Connect to Server")
            {
                X = Pos.At(1),
                Y = Pos.At(3),        
                Clicked = btnConnectClicked
            };
            win.Add(btnConnect);

            btnDisconnect = new Button("Disconnect from Server")
            {
                X = Pos.At(25),
                Y = Pos.At(3),
                Clicked = btnDisconnectClicked
            };
            win.Add(btnDisconnect);

            tvChat = new TextView()
            {
                X = Pos.At(1),
                Y = Pos.At(5),            
                Height = 10,
            };
            win.Add(tvChat);

            tfChatMessage = new TextField("")
            {
                X = Pos.At(1),
                Y = Pos.At(18),
                Width = 30        
            };
            win.Add(tfChatMessage);  

            btnSend = new Button("Send")
            {
                X = Pos.At(1),
                Y = Pos.At(20),            
                Clicked = btnSendClicked
            };
            win.Add(btnSend);     

            Application.Run();
        }

        public MainWindow()
        {   
        }

        private void btnSendClicked()
        {
            byte[] outStream = Encoding.ASCII.GetBytes(tfChatMessage.Text.ToString() + "$");
            networkStream.Write(outStream, 0, outStream.Length);
            networkStream.Flush();
        }

        private void btnConnectClicked()
        {
            clientSocket = new TcpClient()
            {
                ReceiveBufferSize = 1024
            };

            clientSocket.Connect("127.0.0.1", 8888);
            PrintMessage("Conected to Chat Server ...");      
            networkStream = clientSocket.GetStream();

            byte[] outStream = Encoding.ASCII.GetBytes(tfUserName.Text.ToString() + "$");
            networkStream.Write(outStream, 0, outStream.Length);
            networkStream.Flush();

            ctThread = new Thread(getMessage);
            ctThread.Start();
        }

        private void btnDisconnectClicked()
        {
            // TODO: closing thread gracefully
            // TODO: sending disconnect message to server
            clientSocket.Close(); 
        }

        private void getMessage()
        {
            var bufferSize = clientSocket.ReceiveBufferSize;

            try
            {
                while (true)
                {
                    byte[] inStream = new byte[bufferSize];
                    networkStream.Read(inStream, 0, bufferSize);
                    var message = Encoding.ASCII.GetString(inStream);
                    //message = "" + returndata;
                    PrintMessage(message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                ClearMessages();
                PrintMessage("Disconnected from Chat Server ...");
            }         
        }

        private void PrintMessage(string message)
        {          
            tvChat.Text = tvChat.Text + Environment.NewLine + " >> " + message;
        }


        private void ClearMessages()
        {
            tvChat.Text = "";
        }
    }
}
