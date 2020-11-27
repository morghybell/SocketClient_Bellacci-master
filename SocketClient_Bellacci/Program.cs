using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketClient_Bellacci
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //ha bisogno di un endpoint dove connettersi
            string strIPAddress = "";
            string strPort = "";
            IPAddress ipAddress = null;
            int nPort;

            //il client è a più rischio di errore: cade connessione, morte server, lettura porta errata
            try
            {
                Console.WriteLine("IP del server: ");
                strIPAddress = Console.ReadLine();

                Console.WriteLine("Porta del server: ");
                strPort = Console.ReadLine();

                //prova a inserire la stringa che scrivo in un tipo ipaddress, se non ci riesce non da errore
                //se ci riesce da true
                //il punto esclamativo inverse le cose, quindi se NON ci riesce da true
                if (!IPAddress.TryParse(strIPAddress.Trim(), out ipAddress))
                {
                    Console.WriteLine("IP non valido.");
                    return;
                }

                if (!int.TryParse(strPort, out nPort))
                {
                    Console.WriteLine("Porta non valida.");
                    return;
                }

                if (nPort <= 0 || nPort >= 65535) 
                {
                    Console.WriteLine("Porta non valida.");
                    return;
                }

                Console.WriteLine("EndPoint Server: " + ipAddress.ToString() + " " + nPort);

                //effettiva connessione al serve
                //sblocca la asset nel server
                //per collegarsi al socket server 127.0.0.1  23000
                client.Connect(ipAddress, nPort);

                byte[] sendBuff = new byte[128];
                byte[] recvBuff = new byte[128];
                string sendString = "";
                string receiveString = "";
                int receivedBytes = 0;

                Console.WriteLine("Mando un messaggio: ");

                while (true)
                {
                    //prendo il messsaggio
                    sendString = Console.ReadLine();
                    sendBuff = Encoding.ASCII.GetBytes(sendString);
                    client.Send(sendBuff);

                    if (sendString.ToUpper().Trim() == "QUIT") 
                    {
                        break;
                    }

                    

                    //buff receive che è di 128
                    receivedBytes = client.Receive(recvBuff);
                    //trasformo in stringa
                    receiveString = Encoding.ASCII.GetString(recvBuff, 0, receivedBytes);
                    //stampo
                    Console.WriteLine("S: " + receiveString);
                    //pulisco buffer e invio il messaggio
                    Array.Clear(recvBuff, 0, recvBuff.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                /* In ogni occasione chiudo la connessione per sicurezza */
                if (client != null)
                {
                    if (client.Connected)
                    {
                        client.Shutdown(SocketShutdown.Both);//disabilita la send e receive
                    }
                    client.Close();
                    client.Dispose();
                }
            }
            Console.WriteLine("Premi Enter per chiudere....");
        }
    }
}
