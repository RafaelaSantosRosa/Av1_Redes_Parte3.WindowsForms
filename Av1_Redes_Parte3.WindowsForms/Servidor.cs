using System;
using System.Net.Sockets;
using System.Threading;

namespace Av1_Redes_Parte3.WindowsForms
{
    public class Servidor
    {
        public static void Receber(Socket socket, byte[] buffer, int offset, int tamanho, int timeout)
        {
            int startTickCount = Environment.TickCount;
            int recebidos = 0;  // quantos bytes foram recebidos
            do
            {
                if (Environment.TickCount > startTickCount + timeout)
                    throw new Exception("Tempo esgotado...");
                try
                {
                    recebidos += socket.Receive(buffer, offset + recebidos, tamanho - recebidos, SocketFlags.None);
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.WouldBlock ||
                        ex.SocketErrorCode == SocketError.IOPending ||
                        ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                    {
                        // o buffer do socket esta vazio , aguarde e tente novamente
                        Thread.Sleep(30);
                    }
                    else
                        throw ex;  //ocorreu um erro
                }
            } while (recebidos < tamanho);
        }
    }
}
