using System;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace Server
{
    public partial class Form1 : Form
    {
        private const int PORT = 8888;

        public Form1()
        {
            InitializeComponent();
            StartServer();
        }

        private void StartServer()
        {
            try
            {
                Socket listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listenerSocket.Bind(new IPEndPoint(IPAddress.Any, PORT));
                listenerSocket.Listen(10);

                while (true)
                {
                    Socket clientSocket = listenerSocket.Accept();

                    // Gauname duomenis
                    byte[] data = new byte[1024];
                    int bytesRead = clientSocket.Receive(data);

                    string messageWithSignature = Encoding.UTF8.GetString(data, 0, bytesRead);
                    string[] parts = messageWithSignature.Split(':');
                    string receivedMessage = parts[0];
                    string receivedSignature = parts[1];

                    bool signatureValid = VerifyDigitalSignature(receivedMessage, receivedSignature);

                    if (signatureValid)
                    {
                        MessageBox.Show($"Message received: {receivedMessage}\nSignature verified.");
                    }
                    else
                    {
                        MessageBox.Show($"Error: Signature verification failed for message: {receivedMessage}");
                    }

                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting server: {ex.Message}");
            }
        }

        private bool VerifyDigitalSignature(string message, string receivedSignature)
        {
            // priimane duomenis
            // naudojam SHA sugeneruot hashed value
            using (SHA256 sha256 = SHA256.Create())
            {
                // palyginame 2 gautus hashed value
                byte[] hashedData = sha256.ComputeHash(Encoding.UTF8.GetBytes(message));
                string computedSignature = Convert.ToBase64String(hashedData);
                return computedSignature.Equals(receivedSignature);
            }
        }
    }
}
