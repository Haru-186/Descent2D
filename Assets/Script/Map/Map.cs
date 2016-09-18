using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour
{
    #region Public Variables
    static public Map Instance;

    public int width;
    public int height;

    public TextAsset mapdata;
    #endregion
    
    #region Private Variables
    private enum objects
    {
        NORMAL,
        OBSTACLES,
        WARTER,
        PIT,
        LAVA
    };

    private List<objects> mapArray = new List<objects>();
    #endregion
    
    #region MonoBehaviour CallBacks
    void Start ()
    {
        Instance = this;

        if (width * height <= 0)
        {
            Debug.LogError("Input width/height is invalid.");
        }

        int num = 0;
        int mapsize = width * height;
        for (int i = 0; num < mapsize; i++)
        {
            switch (mapdata.text[i])
            {
                case '_':
                    num++;
                    mapArray.Add(objects.NORMAL);
                    break;
                case '#':
                    num++;
                    mapArray.Add(objects.OBSTACLES);
                    break;
                case '~':
                    num++;
                    mapArray.Add(objects.WARTER);
                    break;
                case '*':
                    num++;
                    mapArray.Add(objects.PIT);
                    break;
                case '+':
                    num++;
                    mapArray.Add(objects.LAVA);
                    break;
                case '\n':
                    break;
                default:
                    //Debug.LogError("Input map data is invalid. " + mapdata.text[i].ToString());
                    break;
            }
        }
    }
    #endregion
    
    #region PhotonBehaviour CallBacks
    
    #endregion
    
    #region Public Methods
    public bool isPossibleToMove(Vector2 targetPos)
    {
        bool ret;
        if (-targetPos.y < 0 || -targetPos.y >= height
            || targetPos.x < 0 || targetPos.x >= width)
        {
            ret = false;
        }
        else
        {
            int index = (int)(-targetPos.y * width + targetPos.x);
            if (mapArray[index] != objects.OBSTACLES)
            {
                ret = true;
            }
            else
            {
                ret = false;
            }
        }
        return ret;
    }
    #endregion
    
    #region Private Methods
    
    #endregion
}
