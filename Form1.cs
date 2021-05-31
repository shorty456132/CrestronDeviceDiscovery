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

namespace CrestronDevices
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void btnGetDevs_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            richTextBox1.Text += "getting devices\n";
            UdpClient client = new UdpClient();
            IPEndPoint IP = new IPEndPoint(IPAddress.Broadcast, 41794);
            byte[] bytes = Encoding.ASCII.GetBytes("\x14\x00\x00\x00\x01\x04\x00\x03\x00\x00");
            client.Send(bytes, bytes.Length, IP);
            client.Close();
            StartListening();
        }

        private readonly UdpClient udp = new UdpClient(41794);
        private void StartListening()
        {
            try
            {
                this.udp.BeginReceive(new AsyncCallback(Receive), null);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void Receive(IAsyncResult ar)
        {
            IPEndPoint IP = new IPEndPoint(IPAddress.Any, 41796);
            byte[] bytes = udp.EndReceive(ar, ref IP);
            var data = Encoding.ASCII.GetString(bytes).Split(new[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);

            this.Invoke(new MethodInvoker(delegate
            {
                if (data.Length >= 5)
                    richTextBox1.Text += $"[{IP.Address}] {data[3]} - {data[4]}\n";
            }));

            StartListening();
        }
    }
}
