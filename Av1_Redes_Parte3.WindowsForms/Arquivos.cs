using System;
using System.Net.Sockets;
using System.Threading;

namespace Av1_Redes_Parte3.WindowsForms
{
    public class Arquivos
    {
        public static void Enviar(Socket socket, byte[] buffer, int offset, int tamanho, int timeout)
        {
            //Obtém o número de milissegundos decorridos desde a inicialização do sistema.
            int iniciaContagemTick = Environment.TickCount;
            //define o número de bytes enviados
            int enviados = 0;  // quantos bytes ja foram enviados

            do
            {
                //verifica se o timeout ocorreu
                if (Environment.TickCount > iniciaContagemTick + timeout)
                    throw new Exception("Tempo esgotado.");
                try
                {
                    //envia o arquivo e computa os bytes enviados
                    enviados += socket.Send(buffer, offset + enviados, tamanho - enviados, SocketFlags.None);
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.WouldBlock ||
                        ex.SocketErrorCode == SocketError.IOPending ||
                        ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                    {
                        // o buffer do socket buffer esta cheio , aguarde e tente novamente
                        Thread.Sleep(30);
                    }
                    else
                    {
                        throw ex;  // ocorreu um erro catastrófico
                    }
                }
            } while (enviados < tamanho);
        }
    }
}
