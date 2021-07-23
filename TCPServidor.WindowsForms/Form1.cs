using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCPServidor.WindowsForms
{
    public partial class Form1 : Form
    {
        public delegate void FileRecievedEventHandler(object fonte, string nomeArquivo);
        public event FileRecievedEventHandler NovoArquivoRecebido;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.NovoArquivoRecebido += new FileRecievedEventHandler(Form1_NovoArquivoRecebido);
        }

        private void Form1_NovoArquivoRecebido(object fonte, string nomeArquivo)
        {
            this.BeginInvoke(
             new Action(
             delegate ()
             {
                 MessageBox.Show("Novo Arquivo recebido\n" + nomeArquivo);
                 System.Diagnostics.Process.Start("explorer", @"c:\");
             }));
        }

        private void btnEstabelecerConexao_Click(object sender, EventArgs e)
        {
            int porta = int.Parse(txtPorta.Text);
            string endIP = txtEnderecoIP.Text;
            try
            {
                Task.Factory.StartNew(() => TratamentoArquivoRecebido(porta, endIP));
                MessageBox.Show("Escutando na porta...: " + porta);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro : " + ex.Message);
            }

        }

        public void TratamentoArquivoRecebido(int porta, string enderecoIp)
        {
            try
            {
                IPAddress ip = IPAddress.Parse(enderecoIp);
                TcpListener tcpListener = new TcpListener(ip, porta);
                tcpListener.Start();

                while (true)
                {
                    Socket manipularSocket = tcpListener.AcceptSocket();
                    if (manipularSocket.Connected)
                    {
                        string nomeArquivo = string.Empty;
                        NetworkStream networkStream = new NetworkStream(manipularSocket);
                        int thisRead = 0;
                        int blockSize = 1024;
                        Byte[] dataByte = new Byte[blockSize];
                        lock (this)
                        {
                            string caminhoPastaDestino = @"c:\dados";
                            manipularSocket.Receive(dataByte);
                            int tamanhoNomeArquivo = BitConverter.ToInt32(dataByte, 0);
                            nomeArquivo = Encoding.ASCII.GetString(dataByte, 4, tamanhoNomeArquivo);
                            //
                            Stream fileStream = File.OpenWrite(caminhoPastaDestino + nomeArquivo);
                            fileStream.Write(dataByte, 4 + tamanhoNomeArquivo, (1024 - (4 + tamanhoNomeArquivo)));
                            while (true)
                            {
                                thisRead = networkStream.Read(dataByte, 0, blockSize);
                                fileStream.Write(dataByte, 0, thisRead);
                                if (thisRead == 0)
                                    break;
                            }
                            //
                            fileStream.Close();
                        }
                        NovoArquivoRecebido?.Invoke(this, nomeArquivo);
                        manipularSocket = null;
                    }
                }
            }
            catch
            {
                throw;
            }
        }
       
    }
}
