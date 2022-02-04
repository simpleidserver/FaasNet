namespace EventMesh.Runtime.Messages
{
    public class Errors
    {
        public static Errors NO_ACTIVE_SESSION = new Errors("NO_ACTIVE_SESSION");
        public static Errors NOT_AUTHORIZED = new Errors("NOT_AUTHORIZED");

        private Errors(string code)
        {
            Code = code;
        }

        public string Code { get; private set; }
    }
}
