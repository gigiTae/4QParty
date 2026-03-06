using System;
using Unity.Services.Multiplayer;
using UnityEngine;



namespace FQParty.Session.Common
{
    [CreateAssetMenu(fileName = "SessionSettings", menuName = "Settings/SessionSettings")]
    public class SessionSettingSO : ScriptableObject
    {
        [Header("Session options")]
        public int MaxPlayer = 4;

        public string SessionName = "DefaultSession";

        public string SessionType = "DefaultSession";

        public bool UsePlayerName = true;

        [Header("NetworkOptions")]
        public bool CreateNetworkSession = true;

        public NetworkType NetType = NetworkType.Relay;

        [Header("DirectConnect Option")]
        public string IpAddress = "127.0.0.1";
        public ushort Port = 7777;

        public SessionOptions ToSessionOptions()
        {
            var options = BuildSessionOptions();

            if(UsePlayerName)
            {
                options.WithPlayerName();
            }

            if(CreateNetworkSession)
            {
                AddCreateNetworkOptions(options);
            }

            return options;
        }

        SessionOptions BuildSessionOptions()
        {
            return new SessionOptions
            {
                MaxPlayers = MaxPlayer,
                Name = SessionName,
                Type = SessionType,
            };
        }

        void AddCreateNetworkOptions(SessionOptions options)
        {
            switch (NetType)
            {
                case NetworkType.Direct:
                    options.WithDirectNetwork(new DirectNetworkOptions(new ListenIPAddress(IpAddress), new PublishIPAddress(IpAddress), Port));
                    break;
                case NetworkType.Relay:
                    options.WithRelayNetwork();
                    break;
                case NetworkType.DistributedAuthority:
#if GAMEOBJECTS_NETCODE_2_AVAILABLE
                    options.WithDistributedAuthorityNetwork();
                    break;
#else
                    throw new InvalidOperationException(
                        "Distributed Authority network is not supported without Netcode for Gameobjects v2.0.");
#endif
            }
        }

        public JoinSessionOptions ToJoinSessionOptions()
        {
            var options = new JoinSessionOptions
            {
                Type = SessionType,
            };


            if (UsePlayerName)
            {
                options.WithPlayerName();
            }

            return options;
        }

    }

}