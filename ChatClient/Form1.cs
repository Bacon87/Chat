using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Chat.Model;

namespace ChatClient {
    public partial class Form1 : Form {
        private TcpClient _client = default(TcpClient);
        private Task _listener = default(Task);
        private CancellationTokenSource _tokenSource = default(CancellationTokenSource);

        public Form1() {
            InitializeComponent();

            _tokenSource = new CancellationTokenSource();
            _listener = StartUDPListener(_tokenSource.Token);
        }

        private Task StartUDPListener(CancellationToken token) {
            return Task.Factory.StartNew(() => {
                UdpClient listener = new UdpClient(5556);
                listener.EnableBroadcast = true;

                while (!token.IsCancellationRequested) {
                    IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 5556);
                    byte[] message = listener.Receive(ref endpoint);

                    Chat.Model.Message msg = default(Chat.Model.Message);
                    XmlSerializer serializer = new XmlSerializer(typeof(Chat.Model.Message));
                    using (Stream memStream = new MemoryStream(message)) {
                        memStream.Position = 0;
                        msg = serializer.Deserialize(memStream) as Chat.Model.Message;

                        memStream.Close();
                    }

                    if (msg != null) {
                        AppendToTextBox($"{msg.From} : {msg.Msg}");
                        LogMessageToFile($"{msg.From} : {msg.Msg}");
                    }
                }
            }, token);
        }

        private void btnSend_Click(object sender, EventArgs e) {
            Chat.Model.Message msg = new Chat.Model.Message(txtAlias.Text, txtMessage.Text);

            _client = new TcpClient("localhost", 5555);

            using (Stream networkStream = _client.GetStream())
            using (StreamWriter sw = new StreamWriter(networkStream)) {
                sw.Write(msg.Serialize());
                sw.Close();
            }
            txtMessage.Text = "";
            txtMessage.Focus();
        }

        public void AppendToTextBox(string value) {
            if (InvokeRequired) {
                this.Invoke(new Action<string>(AppendToTextBox), new object[] {value});
                return;
            }

            txtMessages.Text = $"{txtMessages.Text}{Environment.NewLine}{value}";
        }

        private void Form1_Load(object sender, EventArgs e) {
            txtAlias.Text = Environment.MachineName;
            txtMessage.Focus();
        }

        public string GetPath()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!path.EndsWith("\\")) path += "\\";
            return path;
        }

        public void LogMessageToFile(string msg)
        {
            System.IO.StreamWriter sw = System.IO.File.AppendText(
                GetPath() + "ChatLog.log");
            try
            {
                string logLine = System.String.Format(
                    "{0:G}: {1}", System.DateTime.Now, msg);
                sw.WriteLine(logLine);
            }
            finally
            {
                sw.Close();
            }
        }

    }
}
