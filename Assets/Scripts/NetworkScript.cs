// TAKEN FROM
// https://docs.unity3d.com/Packages/com.unity.netcode.gameobjects@2.7/manual/tutorials/get-started-with-ngo.html#the-helloworldmanagercs-script

using Unity.Netcode;
using UnityEngine;

namespace Networking
{
    public class NetworkScript : MonoBehaviour
    {
        NetworkManager networkManager;
        
        public GameObject starterCamera;

        bool started = false;

        void Awake()
        {
            networkManager = GetComponent<NetworkManager>();
        }
        
        void Update()
        {
            if (started)
                return;

            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                print("Starting HOST");
                networkManager.StartHost();
                StartGame();
            }
                
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                print("Starting CLIENT");
                networkManager.StartClient();
                StartGame();
            }
                
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                print("Starting SERVER");
                networkManager.StartClient();
                StartGame();
            } 
        }

        void StartGame()
		{
			started = true;
            if (starterCamera != null) Destroy(starterCamera);
		}
    }
}