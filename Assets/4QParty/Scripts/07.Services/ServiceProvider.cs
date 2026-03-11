using FQParty.Services;
using FQParty.Services.Local;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FQParty.Services
{
    public class ServiceProvider : MonoBehaviour
    {
        [SerializeField]
        private IMultiplayLobbyService m_LobbyService;

        [SerializeField]
        private bool m_IsLocal = false;

        [SerializeField]
        NetworkManager m_NetworkManager;

        void Awake()
        {
            if (m_IsLocal) // Local
            {
                m_LobbyService = new LocalLobbyService(m_NetworkManager);
            }
            else // Steam
            {
               // m_Service = GetComponent<SteamService>();
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }

    }

}