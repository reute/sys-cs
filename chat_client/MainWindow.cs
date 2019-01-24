using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using Terminal.Gui;


namespace chat_client
{
    public class MainWindow
    {
        TextField tfChatMessage, tfUserName;
        TextView tvChat;
        Button btnConnect, btnSend;
        Window win;
        Label tfUserNameLabel;

        TcpClient clientSocket = new TcpClient();
        NetworkStream serverStream = default(NetworkStream);
        string readData = null;

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

        private void btnSendClicked()
        {
            byte[] outStream = Encoding.ASCII.GetBytes(tfChatMessage.Text.ToString() + "$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
        }

        private void btnConnectClicked()
        {
            readData = "Conected to Chat Server ...";
            msg();
            clientSocket.Connect("127.0.0.1", 8888);
            serverStream = clientSocket.GetStream();

            byte[] outStream = Encoding.ASCII.GetBytes(tfUserName.Text.ToString() + "$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();

            Thread ctThread = new Thread(getMessage);
            ctThread.Start();
        }

        private void getMessage()
        {
            while (true)
            {
                serverStream = clientSocket.GetStream();
                int buffSize = 0;
                byte[] inStream = new byte[ushort.MaxValue+1];
                buffSize = clientSocket.ReceiveBufferSize;
                serverStream.Read(inStream, 0, buffSize);
                string returndata = System.Text.Encoding.ASCII.GetString(inStream);
                readData = "" + returndata;
                msg();
            }
        }

        private void msg()
        {          
            tvChat.Text = tvChat.Text + Environment.NewLine + " >> " + readData;
        }
    }
}
