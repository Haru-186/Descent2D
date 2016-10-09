using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HeroManager : Photon.PunBehaviour
{
    #region Public Variables
    static public GameObject LocalPlayerInstance;   // For referencing from anywhere.
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
        transform.position = References.Instance.ComponentForHeros.startPositions[0].position;

        if (PhotonNetwork.connected)
        {
            if (photonView.isMine)
            {
                photonView.RPC("RPC_joinGame", PhotonTargets.All, gameObject.name);
            }
        }
        else
        {
            // [DEBUG CODE] For Debugging without Launcher scene.
            RPC_joinGame(gameObject.name);
        }

        moveScript = GetComponent<HeroMove>();
        if (moveScript == null)
        {
            Debug.Log("[HeroManager::Start] HeroMove script is missing.");
        }
    }

    void Update ()
    {
        if (PhotonNetwork.connected)
        {
            // we don't do anything if we are not the local player.
            if (!photonView.isMine)
            {
                return;
            }
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
    void RPC_joinGame(string name)
    {
        Debug.Log("[RPC_joinGame] player: " + gameObject.name);
        References.Instance.TurnManager.AppendNewPlayer(name);
    }
    #endregion
}
