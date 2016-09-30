using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HeroAction : Photon.PunBehaviour
{
    #region Public Variables
    static public bool isActing = false;
    static public bool isInitFlag = false;
    #endregion
    
    #region Private Variables
    private int actionCount;
    private Text actionCountText;
    private HeroMove moveScript;
    #endregion

    #region MonoBehaviour CallBacks
    void Start ()
    {
        actionCountText = References.Instance.heroCompo.actionCountText;
        if (actionCountText == null)
        {
            Debug.Log("[HeroAction::Start] actionCountText is missing.");
        }

        moveScript = GetComponent<HeroMove>();
        if (moveScript == null)
        {
            Debug.Log("[HeroAction] HeroMove script is missing.");
        }
    }
    #endregion
    
    #region PhotonBehaviour CallBacks
    
    #endregion
    
    #region Public Methods
    public IEnumerator ActionStart()
    {
        Debug.Log("[PlayerManager::ActionStart]");
        if (!photonView.isMine && PhotonNetwork.connected)
        {
            yield break;
        }

        Debug.Log("[PlayerManager::ActionStart] Player: " + gameObject.name + " ActionStart!");
        isActing = true;
        actionCount = 2;
        UpdateActionText();
        while (!(actionCount <= 0 && moveScript.GetMovePoints() <= 0))
        {
            if (isInitFlag)
            {
                isInitFlag = false;
                actionCount = 0;
                UpdateActionText();
                yield break;
            }
            else
            {
                yield return null;
            }
        }

        if (PhotonNetwork.connected)
        {
            photonView.RPC("RPC_turnEnd", PhotonTargets.All, null);
        }
        else
        {
            // [DEBUG CODE] For Debugging without Launcher scene.
            RPC_turnEnd();
        }
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
    private void UpdateActionText()
    {
        actionCountText.text = "Action Count: " + actionCount;
    }

    [PunRPC]
    void RPC_turnEnd()
    {
        Debug.Log("[RPC_turnEnd] player: " + gameObject.name);
        TurnManager.Instance.ChangeNextPlayerTurn();
    }
    #endregion
}
