using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HeroManager : Photon.PunBehaviour
{
    #region Public Variables
    static public GameObject LocalPlayerInstance;
    #endregion
    
    #region Private Variables
    private HeroMove moveScript;
    #endregion
    
    #region MonoBehaviour CallBacks
    void Awake()
    {
        if (PhotonNetwork.connected)
        {
            Debug.Log("[PlayerManager::Awake] " + photonView.owner.name);
            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
            if (photonView.isMine)
            {
                Debug.Log("[PlayerManager::Awake] photonView.isMine is true.");
                HeroManager.LocalPlayerInstance = this.gameObject;
            }
            gameObject.name = photonView.owner.name;
        }
        else
        {
            // [DEBUG CODE] For Debugging without Launcher scene.
            HeroManager.LocalPlayerInstance = this.gameObject;
        }
    }

    void Start()
    {
        transform.SetParent(GameObject.FindGameObjectWithTag("map").transform);
        transform.localScale = Vector3.one;

        if (photonView.isMine && PhotonNetwork.connected)
        {
            photonView.RPC("RPC_incrementPlayerCount", PhotonTargets.All, gameObject.name);
        }
        else
        {
            // [DEBUG CODE] For Debugging without Launcher scene.
            RPC_incrementPlayerCount(gameObject.name);
        }

        moveScript = GetComponent<HeroMove>();
        if (moveScript == null)
        {
            Debug.Log("[HeroManager::Start] HeroMove script is missing.");
        }
    }

    void Update ()
    {
        // we don't do anything if we are not the local player.
        if (!photonView.isMine && PhotonNetwork.connected) 
        {
            return;
        }

        moveScript.move();
    }
    #endregion
    
    #region PhotonBehaviour CallBacks
    
    #endregion
    
    #region Public Methods

    #endregion

    #region Private Methods
    [PunRPC]
    void RPC_incrementPlayerCount(string name)
    {
        Debug.Log("[RPC_incrementPlayerCount] player: " + gameObject.name);
        TurnManager.Instance.AppendNewPlayer(name);
    }
    #endregion
}
