using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.StateMachines;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace FaasNet.EventMesh.Client.StateMachines
{
    public class ClientStateMachine : IStateMachine
    {
        public ClientStateMachine()
        {
            Purposes = new List<ClientPurposeTypes>();
        }

        public string Id { get; set; }
        public string ClientSecret { get; set; }
        public string Vpn { get; set; }
        public int SessionExpirationTimeMS { get; set; }
        public ICollection<ClientPurposeTypes> Purposes { get; set; }

        public void Apply(ICommand cmd)
        {
            switch(cmd)
            {
                case AddClientCommand addClient:
                    ClientSecret = addClient.Secret;
                    Vpn = addClient.Vpn;
                    SessionExpirationTimeMS = addClient.SessionExpirationTimeMS;
                    Purposes = addClient.Purposes;
                    break;
            }
        }

        public void Deserialize(ReadBufferContext context)
        {
            Id = context.NextString();
            Vpn = context.NextString();
            SessionExpirationTimeMS = context.NextInt();
            var nb = context.NextInt();
            for(var i = 0; i < nb; i++)
                Purposes.Add((ClientPurposeTypes)context.NextInt());
        }

        public byte[] Serialize()
        {
            var writeBufferContext = new WriteBufferContext();
            writeBufferContext.WriteString(Id);
            writeBufferContext.WriteString(Vpn);
            writeBufferContext.WriteInteger(Purposes.Count);
            foreach (var purpose in Purposes) writeBufferContext.WriteInteger((int)purpose);
            return writeBufferContext.Buffer.ToArray();
        }

        public bool CheckPassword(string pwd)
        {
            return ComputePassword(pwd) == ClientSecret;
        }

        public static string ComputePassword(string pwd)
        {
            var payload = ASCIIEncoding.ASCII.GetBytes(pwd);
            using (var sha = SHA256.Create())
            {
                return ASCIIEncoding.ASCII.GetString(sha.ComputeHash(payload));
            }
        }

        public bool HasPurpose(ClientPurposeTypes purpose) => Purposes.Contains(purpose);
    }
    public enum ClientPurposeTypes
    {
        PUBLISH = 0,
        SUBSCRIBE = 1
    }

    public class AddClientCommand : ICommand
    {
        public AddClientCommand()
        {
            Purposes = new List<ClientPurposeTypes>();
        }

        public string Vpn { get; set; }
        public string Secret { get; set; }
        public int SessionExpirationTimeMS { get; set; }
        public ICollection<ClientPurposeTypes> Purposes { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Vpn = context.NextString();
            Secret = context.NextString();
            SessionExpirationTimeMS = context.NextInt();
            var result = new List<ClientPurposeTypes>();
            var nb = context.NextInt();
            for (var i = 0; i < nb; i++) result.Add((ClientPurposeTypes)context.NextInt());
            Purposes = result;
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Vpn);
            context.WriteString(Secret);
            context.WriteInteger(SessionExpirationTimeMS);
            context.WriteInteger(Purposes.Count);
            foreach (var purpose in Purposes) context.WriteInteger((int)purpose);
        }
    }
}
