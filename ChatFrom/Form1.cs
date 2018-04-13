using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace ChatFrom
{
    public partial class Frm : Form
    {
        Socket server, client;
        //IPEndPoint ipClient;
        byte[] data = new byte[1024];
        public Frm()
        {
            InitializeComponent();
            
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string text = txtMassage.Text;
            
            data = new byte[1024];
            data = Encoding.ASCII.GetBytes(text);
            /*listBChat.Items.Add("Server: " + text);
            //txtMassage.Text = ""; 
            client.Send(data);
            data = new byte[1024];
            client.Receive(data);
            listBChat.Items.Add("Client: " + Encoding.ASCII.GetString(data));
            txtMassage.Clear();*/
            


            client.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendData), client);
            client.BeginReceive(data, 0, data.Length, SocketFlags.None, new AsyncCallback(ReceiveData), client);
            


        }

        private void Frm_Load(object sender, EventArgs e)
        {
            IPEndPoint ipServer = new IPEndPoint(IPAddress.Any, 9999);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(ipServer);
            server.Listen(5);
            listBChat.Items.Add("Cho Ket Noi Tu Client...!");

            //client = server.Accept();

            server.BeginAccept(new AsyncCallback(AcceptClient), server);
            /*IPEndPoint clientEP = (IPEndPoint)client.RemoteEndPoint;
            string lk = "Ket Noi Tu '" + clientEP.Address + "' Port '" + clientEP.Port + "'";
            listBChat.Items.Add(lk);
            data = new byte[1024];
            client.Receive(data);
            listBChat.Items.Add("Client: " + Encoding.ASCII.GetString(data));*/
        }

        private void AcceptClient(IAsyncResult i)
        {
            client = ((Socket)i.AsyncState).EndAccept(i);
            Thread tReceive = new Thread(new ThreadStart(ReceiveDataThread));
            IPEndPoint clientEP = (IPEndPoint)client.RemoteEndPoint;
            string lk = "Ket Noi Tu '" + clientEP.Address + "' Port '" + clientEP.Port + "'";
            
            tReceive.Start();
            listBChat.Items.Add(lk);
        }

        private void ReceiveDataThread()
        {
            client.BeginReceive(data, 0, data.Length, SocketFlags.None, new AsyncCallback(ReceiveData), client);
        }
       


        private void SendData(IAsyncResult i)
        {
            string input = txtMassage.Text;
            server = (Socket)i.AsyncState;
            int sent = server.EndSend(i);

            if (listBChat.InvokeRequired)
            {
                listBChat.Invoke(new MethodInvoker(delegate
                {
                    listBChat.Items.Add("Server: " + input);
                }));

            }
            txtMassage.Clear();
        }

        private void ReceiveData(IAsyncResult i)
        {
            string rec = Encoding.ASCII.GetString(data, 0, ((Socket)i.AsyncState).EndReceive(i));
            if (listBChat.InvokeRequired)
            {
                listBChat.Invoke(new MethodInvoker(delegate
                {
                    listBChat.Items.Add("Client: " + rec);
                }));
            }



        }
    }
}
