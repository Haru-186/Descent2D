using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HeroMove : MonoBehaviour
{
    #region Public Variables
    public float moveTime = 0.3f;           //Time it will take object to move, in seconds.
    #endregion
    
    #region Private Variables
    private bool isMoving;
    private bool isEnable;
    private int movePoints;
    private Text movePointsText;
    private HeroAction actionScript;
    #endregion
    
    #region MonoBehaviour CallBacks
    void Start ()
    {
        movePointsText = References.Instance.heroCompo.movePointsText;
        if (movePointsText == null)
        {
            Debug.Log("[HeroMove::Start] movePointsText is missing.");
        }
        movePoints = 0;

        actionScript = GetComponent<HeroAction>();
        if (actionScript == null)
        {
            Debug.Log("[HeroMove::Start] HeroAction script is missing");
        }

        References.Instance.heroCompo.moveButton.onClick.AddListener(AddMovePoints);
    }
    #endregion
    
    #region PhotonBehaviour CallBacks
    
    #endregion
    
    #region Public Methods
    public void move()
    {    
        if (!isEnable)
        {
            return;
        }

        if ((Input.GetButton("Horizontal") || Input.GetButton("Vertical") || Input.GetButton("LeftUp") || Input.GetButton("RightUp"))
            && !isMoving
            && movePoints > 0)
        {
            int xDir = 0;
            int yDir = 0;

            Vector2 start = transform.localPosition;

            if (Input.GetButton("Horizontal"))
            {
                float horizontal = Input.GetAxis("Horizontal");
                if (Mathf.Abs(horizontal) > 0.0f)
                {
                    xDir = (horizontal > 0.0f) ? 1 : -1;
                }
            }
            else if (Input.GetButton("Vertical"))
            {
                float vertical = Input.GetAxis("Vertical");
                if (Mathf.Abs(vertical) > 0.0f)
                {
                    yDir = (vertical > 0.0f) ? 1 : -1;
                }
            }
            else if (Input.GetButton("LeftUp"))
            {
                float LeftUp = Input.GetAxis("LeftUp");
                if (Mathf.Abs(LeftUp) > 0.0f)
                {
                    if (LeftUp > 0.0f)
                    {
                        yDir = 1;
                        xDir = -1;
                    }
                    else
                    {
                        yDir = -1;
                        xDir = 1;
                    }
                }
            }
            else if (Input.GetButton("RightUp"))
            {
                float RightUp = Input.GetAxis("RightUp");
                if (Mathf.Abs(RightUp) > 0.0f)
                {
                    if (RightUp > 0.0f)
                    {
                        yDir = 1;
                        xDir = 1;
                    }
                    else
                    {
                        yDir = -1;
                        xDir = -1;
                    }
                }
            }

            if (xDir != 0 || yDir != 0)
            {
                Vector2 end = start + new Vector2(xDir, yDir);

                if (Map.Instance.isPossibleToMove(end))
                {
                    isMoving = true;
                    StartCoroutine(SmoothMovement(end));
                }
            }
        }
    }

    public void EnableMovement()
    {
        isEnable = true;
    }

    public void DisableMovement()
    {
        isEnable = false;
        movePoints = 0;
        if (movePointsText != null)
        {
            UpdateMovePointsText();
        }
    }

    public int GetMovePoints()
    {
        return movePoints;
    }
    #endregion
    
    #region Private Methods
    //Co-routine for moving units from one space to next, takes a parameter end to specify where to move to.
    protected IEnumerator SmoothMovement (Vector3 end)
    {
        float startTime = Time.time;

        //Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter. 
        //Square magnitude is used instead of magnitude because it's computationally cheaper.
        float sqrRemainingDistance = (transform.localPosition - end).sqrMagnitude;

        //While that distance is greater than a very small amount (Epsilon, almost zero):
        while(sqrRemainingDistance > float.Epsilon)
        {
            float moveRate = (Time.time - startTime) / moveTime;
            if (moveRate > 1.0f)
            {
                moveRate = 1.0f;
            }

            //Find a new position proportionally closer to the end, based on the moveTime
            Vector3 newPostion = Vector3.Lerp(transform.localPosition, end, moveRate);

            transform.localPosition = newPostion;

            //Recalculate the remaining distance after moving.
            sqrRemainingDistance = (transform.localPosition - end).sqrMagnitude;

            //Return and loop until sqrRemainingDistance is close enough to zero to end the function
            yield return null;
        }

        isMoving = false;
        DecrementMovePoint();
    }   

    void AddMovePoints()
    {
        if (!isEnable)
        {
            return;
        }
        bool ret;
        ret = actionScript.DecrementActionCount();
        if (ret)
        {
            movePoints += 4;
            UpdateMovePointsText();
        }
    }

    void DecrementMovePoint()
    {
        if (!isEnable)
        {
            return;
        }
        movePoints--;
        UpdateMovePointsText();
    }

    void UpdateMovePointsText()
    {
        movePointsText.text = "MovePoints: " + movePoints.ToString();
    }
    #endregion
}
