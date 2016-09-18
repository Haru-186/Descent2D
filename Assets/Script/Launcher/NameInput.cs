using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class NameInput : MonoBehaviour
{
    #region Public Variables
    
    #endregion
    
    #region Private Variables
    static string playerNamePrefKey = "PlayerName";
    #endregion
    
    #region MonoBehaviour CallBacks
    void Start ()
    {
        string defaultName = "";
        InputField _inputField = this.GetComponent<InputField>();
        if (_inputField != null)
        {
            if (PlayerPrefs.HasKey(playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                _inputField.text = defaultName;
            }
        }

        PhotonNetwork.playerName = defaultName;
    }
    #endregion
    
    #region PhotonBehaviour CallBacks
    
    #endregion
    
    #region Public Methods
    public void SetPlayerName(string value)
    {
        // #Important
        PhotonNetwork.playerName = value + " "; // force a trailing space in case value is an empty string, else playerName would not be updated.

        PlayerPrefs.SetString(playerNamePrefKey, value);
    }
    #endregion
    
    #region Private Methods
    
    #endregion
}
