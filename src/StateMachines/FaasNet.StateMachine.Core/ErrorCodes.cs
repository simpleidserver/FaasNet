namespace FaasNet.StateMachine.Core
{
    public static class ErrorCodes
    {
        public static string UNKNOWN_STATEMACHINE_INSTANCE = "UNKNOWN_STATEMACHINE_INSTANCE";
        public static string UNKNOWN_STATEMACHINE_DEF = "UNKNOWN_STATEMACHINE_DEF";
        public static string UNKNOWN_ASYNCAPI_OPERATION = "UNKNOWN_ASYNCAPI_OPERATION";
        public static string UNKNOWN_OPENAPI_OPERATION = "UNKNOWN_OPENAPI_OPERATION";
        public static string INVALID_INPUT = "INVALID_INPUT";
        public static string INVALID_ASYNCAPI_URL = "INVALID_ASYNCAPI_URL";
        public static string INVALID_OPENAPI_URL = "INVALID_OPENAPI_URL";
        public static string STATEMACHINE_ALREADY_EXISTS = "STATEMACHINE_ALREADY_EXISTS";
    }
}
