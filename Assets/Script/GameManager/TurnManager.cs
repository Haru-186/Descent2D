using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnManager : Photon.PunBehaviour
{
    #region Public Variables
    static public TurnManager Instance;
    [HideInInspector] public bool isSycronized = false;       // Accessed from GameManager when new player connnected.
    #endregion
    
    #region Private Variables
    private int currentPlayerId = 0;
    private int playerCount = 0;  // player number which Start() function has done. (cannot use "PhotonNetwork.room.playerCount" because it is incremented before Start() function called)
    private PlayerManager currentHero;
    private bool isTurnEnd = true;
    private List<GameObject> playerList = new List<GameObject>();
    #endregion
    
    #region MonoBehaviour CallBacks
    void Awake ()
    {
        Instance = this;
    }

    void Update ()
    {
        if (isTurnEnd 
            && playerCount >= 2
            && isSycronized)
        {
            Debug.Log("[TurnManager::Upadate] Player: " + currentPlayerId + "'s turn start.");
            int prevId = currentPlayerId;
            isTurnEnd = false;
            References.Instance.playerTexts[prevId].color = Color.red;
            CallCurrentPlayerActionStart();
        }
    }
    #endregion
    
    #region PhotonBehaviour CallBacks
//    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
//    {
//        if (stream.isWriting)
//        {
//            // We own this player: send the others our data
//            if (PhotonNetwork.isMasterClient && !isSycronized)
//            {
//                Debug.Log("[OnPhotonSerializeView] write data: " + currentPlayerId + ", " + isTurnEnd + ", " + playerCount);
//                stream.SendNext(currentPlayerId);
//                stream.SendNext(isTurnEnd);
//                stream.SendNext(playerCount);
//            }
//        }
//        else
//        {
//            // Network player, receive data
//            if (!PhotonNetwork.isMasterClient && !isSycronized)
//            {
//                this.currentPlayerId = (int)stream.ReceiveNext();
//                this.isTurnEnd = (bool)stream.ReceiveNext();
//                this.playerCount = (int)stream.ReceiveNext();
//
//                Debug.Log("[OnPhotonSerializeView] receive data: " + currentPlayerId + ", " + isTurnEnd + ", " + playerCount);
//
//                // Syncronize
//                photonView.RPC("RPC_SycronizeComleted", PhotonTargets.All, null);
//            }
//        }
//    }
    #endregion
    
    #region Public Methods
    /// <summary>
    /// Called from RPC (targets is All).
    /// </summary>
    public void ChangeNextPlayerTurn()
    {
        Debug.Log("[ChangeNextPlayerTurn] currentPlayerId:" + currentPlayerId + " + 1.");

        // Set player name color black
        References.Instance.playerTexts[currentPlayerId].color = Color.black;
        isSycronized = false;

        if (PhotonNetwork.isMasterClient)
        {
            // Increment current player id.
            currentPlayerId++;
            if (currentPlayerId >= PhotonNetwork.room.playerCount)
            {
                currentPlayerId = 0;
            }

            // Then, set turn end flag.
            isTurnEnd = true;

            // Syncronize
            photonView.RPC("RPC_SyncronizeTurnInfo", PhotonTargets.All, currentPlayerId, isTurnEnd);
        }
    }

    /// <summary>
    /// Called from RPC. (target is All)
    /// </summary>
    public void AppendNewPlayer(string name)
    {
        Debug.Log("[AppendNewPlayer] append: " + name + ", playerCount: " + playerCount + " + 1.");
        isSycronized = false;
        if (PhotonNetwork.isMasterClient)
        {
            playerCount++;

            GameObject[] heroObjects = GameObject.FindGameObjectsWithTag("Hero");
            foreach (GameObject tempObject in heroObjects)
            {
                if (tempObject.name.Equals(name))
                {
                    playerList.Add(tempObject);
                    break;
                }
            }

            // Syncronize
            photonView.RPC("RPC_SyncronizePlayerCount", PhotonTargets.Others, playerCount); 
            for (int index = 0; index < playerList.Count; index++)
            {
                photonView.RPC("RPC_SyncronizePlayerList", PhotonTargets.Others, index, playerList[index].name);
            }
            photonView.RPC("RPC_SyncronizeTurnInfo", PhotonTargets.All, currentPlayerId, isTurnEnd);
        }
    }
    #endregion
    
    #region Private Methods
    /// <summary>
    /// Search the current player object, and start coroutine for player action.
    /// </summary>
    private void CallCurrentPlayerActionStart()
    {
        Debug.Log("[CallPlayerActionStart] currentPlayer[" + currentPlayerId + "]: " + playerList[currentPlayerId].name);
        StartCoroutine(playerList[currentPlayerId].GetComponent<PlayerManager>().ActionStart());
    }
//
//    /// <summary>
//    /// Called from OnPhotonSerializeView
//    /// </summary>
//    [PunRPC]
//    void RPC_SycronizeComleted()
//    {
//        Debug.Log("[RPC_SycronizeComleted]");
//        isSycronized = true;
//    }

    [PunRPC]
    void RPC_SyncronizeTurnInfo(int curId, bool turnEnd)
    {
        Debug.Log("[RPC_SyncronizeTurnInfo] Set to " + curId + ", " + turnEnd);
        isSycronized = true;

        this.currentPlayerId = curId;
        this.isTurnEnd = turnEnd;
    }

    [PunRPC]
    void RPC_SyncronizePlayerList(int index, string name)
    {
        Debug.Log("[RPC_SyncronizePlayerList] Add " + name + ", index " + index);

        if (playerList.Count <= index)
        {
            while (playerList.Count <= index)
            {
                playerList.Add(null);
            }
        }

        GameObject[] heroObjects = GameObject.FindGameObjectsWithTag("Hero");
        foreach (GameObject tempObject in heroObjects)
        {
            if (tempObject.name.Equals(name))
            {
                playerList[index] = tempObject;
                break;
            }
        }
    }

    [PunRPC]
    void RPC_SyncronizePlayerCount(int count)
    {
        Debug.Log("[RPC_SyncronizePlayerCount] Set to " + count);
        this.playerCount = count;
    }
    #endregion
}
