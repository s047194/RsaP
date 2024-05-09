using System;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace Client
{
    public partial class Form1 : Form
    {
        private const int PORT = 8888;
        private const string SERVER_IP = "127.0.0.1";

        public Form1()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.Connect(new IPEndPoint(IPAddress.Parse(SERVER_IP), PORT));

                string message = txtMessage.Text;

                // Generuojam parasa
                string digitalSignature = GenerateDigitalSignature(message);
                string messageWithSignature = $"{message}:{digitalSignature}";

                byte[] data = Encoding.UTF8.GetBytes(messageWithSignature);
                clientSocket.Send(data);

                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();

                MessageBox.Show("Message sent successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private string GenerateDigitalSignature(string message)
        {
            // Priimame pranešima kaip ivesti
            // Naudojam SHA sugeneruot hashed value
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedData = sha256.ComputeHash(Encoding.UTF8.GetBytes(message));
                // konvertuojam i 64bit eilute ir grazinam kaip skaitmenini parasa
                return Convert.ToBase64String(hashedData);
            }
        }
    }
}
