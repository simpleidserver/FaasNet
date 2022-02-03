namespace EventMesh.Runtime.Messages
{
    public class Errors
    {
        public static Errors NO_ACTIVE_SESSION = new Errors("NO_ACTIVE_SESSION");

        private Errors(string code)
        {
            Code = code;
        }

        public string Code { get; private set; }
    }
}
