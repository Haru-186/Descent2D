using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnManager : Photon.PunBehaviour
{
    #region Public Variables
    static public TurnManager Instance;     // FIXME
    [HideInInspector] public bool isSycronized = false;       // Accessed from GameManager when new player connnected.
    #endregion
    
    #region Private Variables
    private int currentPlayerId = 0;
    private int playerCount = 0;  // player number which Start() function has done. (cannot use "PhotonNetwork.room.playerCount" because it is incremented before Start() function called)
    private HeroManager currentHero;
    private bool isTurnEnd = true;
    private List<GameObject> playerList = new List<GameObject>();
    private HeroMove heromove;
    private GameObject LocalPlayer;
    #endregion
    
    #region MonoBehaviour CallBacks
    void Awake ()
    {
        Instance = this;
    }

    void Start ()
    {
        LocalPlayer = HeroManager.LocalPlayerInstance;
        heromove = LocalPlayer.GetComponent<HeroMove>();
        if (heromove == null)
        {
            Debug.Log("[TurnManager::Start] HeroMove script is missing.");
        }
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
            References.Instance.heroCompo.playerNameTexts[prevId].color = Color.red;

            if (LocalPlayer.name.Equals(playerList[currentPlayerId].name))
            {
                heromove.EnableMovement();
            }

            CallCurrentPlayerActionStart();
        }
    }
    #endregion
    
    #region PhotonBehaviour CallBacks

    #endregion
    
    #region Public Methods
    /// <summary>
    /// Called from RPC (targets is All).
    /// </summary>
    public void ChangeNextPlayerTurn()
    {
        Debug.Log("[ChangeNextPlayerTurn] currentPlayerId:" + currentPlayerId + " + 1.");

        // Set player name color black
        References.Instance.heroCompo.playerNameTexts[currentPlayerId].color = Color.black;
        isSycronized = false;

        if (PhotonNetwork.isMasterClient && PhotonNetwork.connected)
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
        else
        {
            // [DEBUG CODE] For Debugging without Launcher scene.
            isTurnEnd = true;
            isSycronized = true;
        }
    }

    /// <summary>
    /// Called from RPC. (target is All)
    /// </summary>
    public void AppendNewPlayer(string name)
    {
        Debug.Log("[AppendNewPlayer] append: " + name + ", playerCount: " + playerCount + " + 1.");

        /* before setting currentPlayerId to 0 */
        isSycronized = false;
        if (HeroAction.isActing)
        {
            // If Player ActionStart coroutine has been started, set stop flag
            HeroAction.isInitFlag = true;
        }
        heromove.DisableMovement();
        References.Instance.heroCompo.playerNameTexts[currentPlayerId].color = Color.black;

        currentPlayerId = 0;
        isTurnEnd = true;

        if (PhotonNetwork.isMasterClient && PhotonNetwork.connected)
        {
            playerCount++;

            GameObject[] heroObjects = GameObject.FindGameObjectsWithTag("Hero");
            for (int i = 0; i < heroObjects.Length; i++)
            {
                if (heroObjects[i].name.Equals(name))
                {
                    playerList.Add(heroObjects[i]);
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
        else
        {
            // [DEBUG CODE] For Debugging without Launcher scene.
            GameObject[] heroObjects = GameObject.FindGameObjectsWithTag("Hero");
            for (int i = 0; i < heroObjects.Length; i++)
            {
                if (heroObjects[i].name.Equals(name))
                {
                    playerList.Add(heroObjects[i]);
                    break;
                }
            }
            isSycronized = true;
            playerCount = 2;
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
        StartCoroutine(playerList[currentPlayerId].GetComponent<HeroAction>().ActionStart());
    }

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
