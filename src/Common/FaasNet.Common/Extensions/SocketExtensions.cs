using System.Threading.Tasks;

namespace System.Net.Sockets
{
    public static class SocketExtensions
    {
        public static int Receive(this Socket socket, byte[] buffer, SocketFlags socketFlags, int millisecondsTimeout = 500)
        {
            var result = socket.ReceiveAsync(buffer, socketFlags);
            Task.WaitAny(new[] { result }, millisecondsTimeout);
            if (result.Status != TaskStatus.RanToCompletion) throw new TimeoutException();
            return result.Result;
        }

        public static void Send(this Socket socket, byte[] buffer, SocketFlags socketFlags, int millisecondsTimeout = 500)
        {
            var result = socket.SendAsync(buffer, socketFlags);
            Task.WaitAny(new[] { result }, millisecondsTimeout);
            if (result.Status != TaskStatus.RanToCompletion) throw new TimeoutException();
        }
    }
}
