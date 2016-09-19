using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MoveManager : MonoBehaviour
{
    #region Public Variables
    static public MoveManager Instance;
    public Text moveText;
    #endregion
    
    #region Private Variables
    private int movePoints;
    private PlayerManager playerManagerScript;
    #endregion
    
    #region MonoBehaviour CallBacks
    void Start ()
    {
        Instance = this;
        playerManagerScript = PlayerManager.LocalPlayerInstance.GetComponent<PlayerManager>();
        movePoints = 0;
    }

    void OnDisable()
    {
        movePoints = 0;
        UpdateMovePointsText();
    }
    #endregion
    
    #region PhotonBehaviour CallBacks
    
    #endregion
    
    #region Public Methods
    public void AddMovePoints()
    {
        bool ret;
        ret = playerManagerScript.DecrementActionCount();
        if (ret)
        {
            movePoints += 4;
            UpdateMovePointsText();
        }
    }

    public void DecrementMovePoint()
    {
        movePoints--;
        UpdateMovePointsText();
    }

    public int GetMovePoints()
    {
        return movePoints;
    }
    #endregion
    
    #region Private Methods
    void UpdateMovePointsText()
    {
        moveText.text = "MovePoints: " + movePoints.ToString();
    }
    #endregion
}
