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
    #endregion
    
    #region MonoBehaviour CallBacks
    void Start ()
    {
        Instance = this;
        movePoints = 0;
    }
    #endregion
    
    #region PhotonBehaviour CallBacks
    
    #endregion
    
    #region Public Methods
    public void AddMovePoints()
    {
        movePoints += 4;
        UpdateMovePointsText();
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
