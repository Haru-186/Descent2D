using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : Photon.PunBehaviour, IPunCallbacks
{
    #region Public Variables
    public GameObject playerPrefab;
    public Transform startPosition;
    #endregion
    
    #region Private Variables

    #endregion
    
    #region MonoBehaviour CallBacks
    void Start()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it in GameObject 'Game Manager'", this);
        }
        else
        {
            Debug.Log("[GameManager::Start]");
            if (PhotonNetwork.connected)
            {
                if (HeroManager.LocalPlayerInstance == null)
                {
                    // We're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork. Instanciate
                    PhotonNetwork.Instantiate(this.playerPrefab.name, Vector3.zero, Quaternion.identity, 0);
                    Debug.Log("[GameManager::Start] Hero is instantiated by PhotonNetwork.Instantiate.");
                }
                for (int i = 0; i < PhotonNetwork.room.playerCount; i++)
                {
                    References.Instance.CompornentForHeros.playerNameTexts[i].text = PhotonNetwork.playerList[i].name;
                }
            }
            else
            {
                // [DEBUG CODE] For Debugging without Launcher scene.
                Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
                References.Instance.CompornentForHeros.playerNameTexts[0].text = "Haru";
            }
        }
    }
    #endregion
    
    #region PhotonBehaviour CallBacks
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        References.Instance.CompornentForHeros.playerNameTexts[newPlayer.ID - 1].text = newPlayer.name;
    }
    #endregion
    
    #region Public Methods
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    #endregion
    
    #region Private Methods
    
    #endregion
}
