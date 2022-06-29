using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FaasNet.DHT.Chord.Client.Extensions
{
    public static class SocketExtensions
    {
        public static void Send(this Socket socket, byte[] buffer, SocketFlags socketFlags, int millisecondsTimeout = 500)
        {
            var result = socket.SendAsync(buffer, socketFlags);
            Task.WaitAny(new[] { result }, millisecondsTimeout);
            if (result.Status != TaskStatus.RanToCompletion) throw new TimeoutException();
        }

        public static int Receive(this Socket socket, byte[] buffer, SocketFlags socketFlags, int millisecondsTimeout = 500)
        {
            var result = socket.ReceiveAsync(buffer, socketFlags);
            Task.WaitAny(new[] { result }, millisecondsTimeout);
            if (result.Status != TaskStatus.RanToCompletion) throw new TimeoutException();
            return result.Result;
        }
    }
}
