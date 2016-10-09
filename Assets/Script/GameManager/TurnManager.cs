using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnManager : Photon.PunBehaviour
{
    #region Public Variables

    #endregion
    
    #region Private Variables
    private int currentPlayerId = 0;
    private int numberOfPlayer = 0;  // player number which Start() function has done. (cannot use "PhotonNetwork.room.playerCount" because it is incremented before Start() function called)
    private bool isNoPlayerActing = true;
    private bool isSycronized = false;
    private List<GameObject> playerList = new List<GameObject>();
    #endregion
    
    #region MonoBehaviour CallBacks
    void Update ()
    {
        if (isNoPlayerActing && isSycronized)
        {
            Debug.Log("[TurnManager::Upadate] Player: " + currentPlayerId + "'s turn start.");

            isNoPlayerActing = false;

            // Diasable Moveable flag. If LocalPlayer is current player, Moveable flag to be enabled in StartCurrentPlayerTurn().
            HeroManager.LocalPlayerInstance.GetComponent<HeroMove>().DisableMovement();

            StartCurrentPlayerTurn();
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
        References.Instance.ComponentForHeros.playerNameTexts[currentPlayerId].color = Color.black;

        isSycronized = false;

        if (PhotonNetwork.connected)
        {
            if (PhotonNetwork.isMasterClient)
            {
                // Increment current player id.
                currentPlayerId++;
                if (currentPlayerId >= PhotonNetwork.room.playerCount)
                {
                    currentPlayerId = 0;
                }

                // Then, set turn end flag.
                isNoPlayerActing = true;

                // Syncronize
                photonView.RPC("RPC_SyncronizeTurnInformation", PhotonTargets.All, currentPlayerId, isNoPlayerActing);
            }
        }
        else
        {
            // [DEBUG CODE] For Debugging without Launcher scene.
            isNoPlayerActing = true;
            isSycronized = true;
        }
    }

    /// <summary>
    /// Called from RPC. (target is All)
    /// </summary>
    public void AppendNewPlayer(string name)
    {
        Debug.Log("[AppendNewPlayer] append: " + name + ", playerCount: " + numberOfPlayer + " + 1.");

        /* before setting currentPlayerId to 0 */
        isSycronized = false;

        if (PhotonNetwork.connected)
        {
            if (PhotonNetwork.isMasterClient)
            {
                numberOfPlayer++;

                playerList.Add(FindHeroWithName(name));

                // Syncronization
                photonView.RPC("RPC_SyncronizeNumberOfPlayer", PhotonTargets.Others, numberOfPlayer);
                for (int index = 0; index < playerList.Count; index++)
                {
                    photonView.RPC("RPC_SyncronizePlayerList", PhotonTargets.Others, index, playerList[index].name);
                }

                // The flag 'isSyncronized' it to be set in this RPC.
                photonView.RPC("RPC_SyncronizeTurnInformation", PhotonTargets.All, currentPlayerId, isNoPlayerActing);
            }
        }
        else
        {
            // [DEBUG CODE] For Debugging without Launcher scene.
            playerList.Add(FindHeroWithName(name));
            isSycronized = true;
        }
    }
    #endregion
    
    #region Private Methods
    /// <summary>
    /// Search the current player object, and start coroutine for player action.
    /// </summary>
    private void StartCurrentPlayerTurn()
    {
        Debug.Log("[CallPlayerActionStart] currentPlayer[" + currentPlayerId + "]: " + playerList[currentPlayerId].name);

        // Change the color of the current player's name.
        References.Instance.ComponentForHeros.playerNameTexts[currentPlayerId].color = Color.red;

        // Enable Moveable flag.
        playerList[currentPlayerId].GetComponent<HeroMove>().EnableMovement();

        // Call a coroutine for starting the current player turn.
        StartCoroutine(playerList[currentPlayerId].GetComponent<HeroAction>().ActionStart());
    }

    GameObject FindHeroWithName(string name)
    {
        GameObject[] heroObjects = GameObject.FindGameObjectsWithTag("Hero");
        for (int i = 0; i< heroObjects.Length; i++)
        {
            if (heroObjects[i].name.Equals(name))
            {
                return heroObjects[i];
            }
        }
        Debug.LogError("[TurnManager] Cannot find the player with name:" + name);
        return null;
    }

    [PunRPC]
    void RPC_SyncronizeTurnInformation(int currentId, bool noPlayerActive)
    {
        Debug.Log("[RPC_SyncronizeTurnInfo] Set to " + currentId + ", " + noPlayerActive);

        this.currentPlayerId = currentId;
        this.isNoPlayerActing = noPlayerActive;

        isSycronized = true;
    }

    [PunRPC]
    void RPC_SyncronizePlayerList(int index, string name)
    {
        Debug.Log("[RPC_SyncronizePlayerList] Add " + name + ", index " + index);

        while (playerList.Count <= index)
        {
            playerList.Add(null);
        }

        playerList[index] = FindHeroWithName(name);
    }

    [PunRPC]
    void RPC_SyncronizeNumberOfPlayer(int count)
    {
        Debug.Log("[RPC_SyncronizePlayerCount] Set to " + count);
        this.numberOfPlayer = count;
    }
    #endregion
}
