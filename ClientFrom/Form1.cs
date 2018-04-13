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

namespace ClientFrom
{
    public partial class Frm : Form
    {
        Socket client;
        byte[] data = new byte[1024];
        IPEndPoint ipServer;
        public Frm()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            ipServer = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                client.BeginConnect(ipServer, new AsyncCallback(Connectserver), client);
                //client.Connect(ipServer);
                listBChat.Items.Add("Ket Noi Thanh Cong!");
            }
            catch (SocketException ex)
            {
                listBChat.Items.Add("Khong The Ket Noi Voi Server.");
                listBChat.Items.Add(ex.ToString());
                return;
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string text = txtMassage.Text;
            
            data = new byte[1024];
            data = Encoding.ASCII.GetBytes(text);
            /*listBChat.Items.Add("Client: " + text);
                       
            //txtMassage.Text = "";         
            client.Send(data);
            data = new byte[1024];
            client.Receive(data);
            listBChat.Items.Add("Server: " + Encoding.ASCII.GetString(data));
            txtMassage.Clear();*/
            client.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendData), client);
            client.BeginReceive(data, 0, data.Length, SocketFlags.None, new AsyncCallback(ReceiveData), client);
            

        }

        private void SendData(IAsyncResult i)
        {
            string input = txtMassage.Text;
            Socket server = (Socket)i.AsyncState;
            int sent = server.EndSend(i);

            if (listBChat.InvokeRequired)
            {
                listBChat.Invoke(new MethodInvoker(delegate {
                    listBChat.Items.Add("Client: " + input);
                }));

            }
            txtMassage.Clear();
        }

        private void ReceiveData(IAsyncResult i)
        {
            string rec = Encoding.ASCII.GetString(data, 0, ((Socket)i.AsyncState).EndReceive(i));
            if (listBChat.InvokeRequired)
            {
                listBChat.Invoke(new MethodInvoker(delegate {
                    listBChat.Items.Add("Server: " + rec);
                }));
            }
        }

        public void Connectserver(IAsyncResult i)
        {
            Socket sock = ((Socket)i.AsyncState);
            sock.EndConnect(i);
        }
    }
}
