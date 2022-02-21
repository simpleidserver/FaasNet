namespace FaasNet.Gateway.Core
{
    public static class ErrorCodes
    {
        public const string InvalidStateMachineName = "InvalidStateMachineName";
        public const string InvalidExecutionInput = "InvalidExecutionInput";
        public const string InvalidStateMachineInstanceId = "InvalidStateMachineInstanceId";
        public const string UnsupportedFunctionProvider = "UnsupportedFunctionProvider";
        public const string UnknownFunction = "UnknownFunction";
        public const string UnknownStateMachine = "UnknownStateMachine";
        public const string UnknownOpenApiOperation = "UnknownOpenApiOperation";
        public const string UnknownAsyncApiOperation = "UnknownAsyncApiOperation";
        public const string StateMachineExists = "StateMachineExists";
        public const string FunctionExists = "FunctionExists";
        public const string EventMeshServerExists = "EventMeshServerExists";
    }
}
