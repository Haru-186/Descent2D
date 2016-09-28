using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : Photon.PunBehaviour, IPunCallbacks
{
    #region Public Variables
    static public GameManager Instance;
    public GameObject playerPrefab;
    public Transform startPosition;
    #endregion
    
    #region Private Variables
    
    #endregion
    
    #region MonoBehaviour CallBacks
    void Start()
    {
        Instance = this;

        if (playerPrefab == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it in GameObject 'Game Manager'", this);
        }
        else
        {
            Debug.Log("[GameManager::Start]");
            if (PhotonNetwork.connected)
            {
                if (PlayerManager.LocalPlayerInstance == null)
                {
                    // We're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork. Instanciate
                    PhotonNetwork.Instantiate(this.playerPrefab.name, startPosition.position, Quaternion.identity, 0);
                    Debug.Log("[GameManager::Start] Hero is instantiated by PhotonNetwork.Instantiate.");
                }
                for (int i = 0; i < PhotonNetwork.room.playerCount; i++)
                {
                    References.Instance.playerTexts[i].text = PhotonNetwork.playerList[i].name;
                }
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
        References.Instance.playerTexts[newPlayer.ID - 1].text = newPlayer.name;
        TurnManager.Instance.isSycronized = false;
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
