using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerManager : Photon.PunBehaviour
{
    #region Public Variables
    static public GameObject LocalPlayerInstance;
    static public bool isActing = false;
    static public bool isInitFlag = false;
    #endregion
    
    #region Private Variables
    private int actionCount;
    private Text actionText;
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
                PlayerManager.LocalPlayerInstance = this.gameObject;
            }
            gameObject.name = photonView.owner.name;
        }
    }

    void Start()
    {
        transform.SetParent(GameObject.FindGameObjectWithTag("map").transform);
        transform.localScale = Vector3.one;

        if (photonView.isMine)
        {
            photonView.RPC("RPC_incrementPlayerCount", PhotonTargets.All, gameObject.name);
        }
        actionText = References.Instance.actionText;
    }
    #endregion
    
    #region PhotonBehaviour CallBacks
    
    #endregion
    
    #region Public Methods
    public IEnumerator ActionStart()
    {
        Debug.Log("[PlayerManager::ActionStart]");
        if (!photonView.isMine)
        {
            yield break;
        }

        Debug.Log("[PlayerManager::ActionStart] Player: " + gameObject.name + " ActionStart!");
        isActing = true;
        actionCount = 2;
        UpdateActionText();
        while (!(actionCount <= 0 && MoveManager.Instance.GetMovePoints() <= 0))
        {
            if (isInitFlag)
            {
                isInitFlag = false;
                yield break;
            }
            else
            {
                yield return null;
            }
        }
        photonView.RPC("RPC_turnEnd", PhotonTargets.All, null);
        isActing = false;
    }

    public bool DecrementActionCount()
    {
        if (actionCount <= 0)
        {
            return false;
        }
        else
        {
            actionCount--;
            UpdateActionText();
            return true;
        }
    }
    #endregion

    #region Private Methods
    [PunRPC]
    void RPC_turnEnd()
    {
        Debug.Log("[RPC_turnEnd] player: " + gameObject.name);
        TurnManager.Instance.ChangeNextPlayerTurn();
    }

    [PunRPC]
    void RPC_incrementPlayerCount(string name)
    {
        Debug.Log("[RPC_incrementPlayerCount] player: " + gameObject.name);
        TurnManager.Instance.AppendNewPlayer(name);
    }

    private void UpdateActionText()
    {
        actionText.text = "Action Count: " + actionCount;
    }
    #endregion
}
