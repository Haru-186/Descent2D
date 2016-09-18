using UnityEngine;
using System.Collections;

public class Launcher : Photon.PunBehaviour
{
    #region Public Variables
    public PhotonLogLevel Loglevel = PhotonLogLevel.Informational;
    public byte MaxPlayersPerRoom = 5;

    [Tooltip("The UI Panel to let the user enter name, connect and play")]
    public GameObject controlPanel;
    [Tooltip("The UI Lavel to inform the user that the connection is in progress")]
    public GameObject progressLabel;
    #endregion
    
    #region Private Variables
    private string _gameVersion = "1";
    //bool isConnecting;
    #endregion
    
    #region MonoBehaviour CallBacks
    void Awake () 
    {
        // #NotImoprtant
        // Force Full LogLebel
        PhotonNetwork.logLevel = Loglevel;

        // #Critical
        // we don't join the lobby. There is no need to join a lobby to get the list of rooms.
        PhotonNetwork.autoJoinLobby = false;

        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.automaticallySyncScene = true;
    }

    void Start()
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }
    #endregion
    
    #region PhotonBehaviour CallBacks
    public override void OnConnectedToMaster()
    {
        Debug.Log("[OnConnectedToMaster]");

        // we don't want to do anything if we are not attemping to join a room.
        // this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
        // we don't want to do anything.
        //if (isConnecting)
        {
            // #Critical: The first we try to do is to join a potential existing room. If there is , good, else, we'll be called back with OnPhotonRanddomJoinFailed()
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnDisconnectedFromPhoton()
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);

        Debug.Log("DemoAnimator/Launcher: OnDisconnectedFromPhoton() was called by PUN");
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        Debug.Log("[OnPhotonRandomJoinFailed] was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.Createroom(null, new RoomOptions() {maxPlayers = 4}, null);");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions() { maxPlayers = MaxPlayersPerRoom }, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("[OnJoinedRoom] called by PUN. Now this client is in a room.");
        // #Critical
        // Load the Room Level.
        PhotonNetwork.LoadLevel("quest01");
    }
    #endregion
    
    #region Public Methods
    public void Connect()
    {
        progressLabel.SetActive(true);
        controlPanel.SetActive(false);

        // keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
        //isConnecting = true;

        if (PhotonNetwork.connected)
        {
            Debug.Log("[Launcher::Connect] PhotonNetwork.connected is true.");
            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnPhotonRandomJoinFailed() and we'll create one.
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            Debug.Log("[Launcher::Connect] PhotonNetwork.connected is false.");
            // #Critical, we must first andd foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings(_gameVersion);
        }
    }    
    #endregion
    
    #region Private Methods
    
    #endregion
}
