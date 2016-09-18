using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class References : MonoBehaviour
{
    #region Public Variables
    static public References Instance;
    public Text[] playerTexts;
    #endregion
    
    #region Private Variables
    
    #endregion
    
    #region MonoBehaviour CallBacks
    void Awake ()
    {
        Instance = this;
    }
    #endregion
    
    #region PhotonBehaviour CallBacks
    
    #endregion
    
    #region Public Methods
    
    #endregion
    
    #region Private Methods
    
    #endregion
}
