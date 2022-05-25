using Amqp;
using System.Collections.Generic;

namespace FaasNet.EventMesh.Protocols.AMQP.Handlers
{
    public class RequestResult
    {
        public RequestResultStatus Status { get; private set; }
        public IEnumerable<ByteBuffer> Content { get; private set; }

        public static RequestResult Ok()
        {
            return new RequestResult { Status = RequestResultStatus.OK, Content = new ByteBuffer[0] };
        }

        public static RequestResult Ok(ByteBuffer record)
        {
            return new RequestResult { Status = RequestResultStatus.OK, Content = new ByteBuffer[1] { record } };
        }

        public static RequestResult Ok(IEnumerable<ByteBuffer> content)
        {
            return new RequestResult { Status = RequestResultStatus.OK, Content = content };
        }

        public static RequestResult ExitSession(ByteBuffer record)
        {
            return new RequestResult { Status = RequestResultStatus.EXIT_SESSION, Content = new ByteBuffer[1] { record } };
        }

        public static RequestResult ExitConnection(ByteBuffer record)
        {
            return new RequestResult { Status = RequestResultStatus.EXIT_SESSION, Content = new ByteBuffer[1] { record } };
        }
    }

    public enum RequestResultStatus
    {
        OK = 0,
        EXIT_SESSION =  1,
        EXIT_CONNECTION = 1
    }
}
