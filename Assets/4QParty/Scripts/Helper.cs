using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Helper : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current == null) return;

        // GetKeyDown 대신 wasPressedThisFrame을 사용합니다.
        if (Keyboard.current.oKey.wasPressedThisFrame)
        {
            NetworkManager.Singleton.StartHost();
            Debug.Log("Host Started");
        }

        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            NetworkManager.Singleton.StartClient();
            Debug.Log("Client Started");
        }
    }
}
