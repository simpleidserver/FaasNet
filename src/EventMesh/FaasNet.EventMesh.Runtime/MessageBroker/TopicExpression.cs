
namespace FaasNet.EventMesh.Runtime.MessageBroker
{
    public class TopicExpression
    {
        public TopicExpression(string path, TopicExpressionTypes type)
        {
            Path = path;
            Type = type;
        }

        public string Path { get; private set; }
        public TopicExpressionTypes Type { get; private set; }
    }
}
