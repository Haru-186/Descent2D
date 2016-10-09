using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HeroAction : Photon.PunBehaviour
{
    #region Public Variables

    #endregion
    
    #region Private Variables
    private int actionCount;
    private Text actionCountText;
    private HeroMove moveScript;
    private TurnManager turnManager;
    #endregion

    #region MonoBehaviour CallBacks
    void Start ()
    {
        actionCountText = References.Instance.ComponentForHeros.actionCountText;
        turnManager = References.Instance.TurnManager;

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
        if (PhotonNetwork.connected)
        {
            if (!photonView.isMine)
            {
                yield break;
            }

            Debug.Log("[PlayerManager::ActionStart] Player: " + gameObject.name + " ActionStart!");

            actionCount = 2;
            UpdateActionText();

            while (!(actionCount <= 0 && moveScript.GetMovePoints() <= 0))
            {
                yield return null;
            }

            photonView.RPC("RPC_turnEnd", PhotonTargets.All, null);
        }
        else
        {
            // [DEBUG CODE] For Debugging without Launcher scene.

            actionCount = 2;
            UpdateActionText();

            while (!(actionCount <= 0 && moveScript.GetMovePoints() <= 0))
            {
                yield return null;
            }

            RPC_turnEnd();
        }
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
        turnManager.ChangeNextPlayerTurn();
    }
    #endregion
}
