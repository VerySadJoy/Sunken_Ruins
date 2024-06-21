using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DwellingTile : Tile
{
    [Space(10)]
    [SerializeField]
    Sprite[] dwellingTilePairs;

    private static int randomTileNumber = -1;
    private SpriteRenderer dwellingTileSR;

    private void Start()
    {
        // Checking Each Side
        GetComponent<Tile>().checkTileBoundaries();
        dwellingTileSR = GetComponent<SpriteRenderer>();

        // [LEFT/RIGHT] Whether Tile Pairs are to be added
        if (randomTileNumber != -1)
        {
            dwellingTileSR = GetComponent<SpriteRenderer>();
            dwellingTileSR.sprite = dwellingTilePairs[++randomTileNumber];
            randomTileNumber = -1;
        }
        else if (isRightFull)
        {
            int probability = Random.Range(0, 3);
            if (probability <= 1)
            {
                do
                { randomTileNumber = Random.Range(0, dwellingTilePairs.Length); }
                while (randomTileNumber % 2 != 0);
                dwellingTileSR.sprite = dwellingTilePairs[randomTileNumber];
            }
        }

        // [UP/DOWN]
        

        // Determining which sides need edges
        if (isUpFull || isLeftFull || isRightFull || isDownFull)
        {
            if (isUpFull)
            {
                if (isDownFull)
                {
                    if (isRightFull && isLeftFull)
                    {
                        noEdges();
                    }
                    else if (isRightFull)
                    {
                        leftEdge();
                    }
                    else if (isLeftFull)
                    {
                        rightEdge();
                    }
                    else
                    {
                        leftRightEdge();
                    }
                }
                else
                {
                    if (isRightFull && isLeftFull)
                    {
                        downEdge();
                    }
                    else if (isLeftFull)
                    {
                        rightDownEdge();
                    }
                    else if (isRightFull)
                    {
                        leftDownEdge();
                    }
                    else
                    {
                        leftRightDownEdge();
                    }
                }
            }
            else if (isLeftFull)
            {
                if (isRightFull)
                {
                    if (isDownFull)
                    {
                        upEdge();
                    }
                    else
                    {
                        upDownEdge();
                    }
                }
                else
                {
                    if (isDownFull)
                    {
                        upRightEdge();
                    }
                    else
                    {
                        upRightDownEdge();
                    }
                }
            }
            else if (isRightFull)
            {
                if (isDownFull)
                {
                    upLeftEdge();
                }
                else
                {
                    upDownLeftEdge();
                }
            }
            else if (isDownFull)
            {
                upLeftRightEdge();
            }
        }
        else
        {
            allEdges();
        }
    }

    private void noEdges()
    {
        // Leave Alone as Default
        edgeUp.gameObject.SetActive(false);
        edgeDown.gameObject.SetActive(false);
        edgeLeft.gameObject.SetActive(false);
        edgeRight.gameObject.SetActive(false);
    }
    private void leftEdge()
    {
        edgeUp.gameObject.SetActive(false);
        edgeDown.gameObject.SetActive(false);
        edgeLeft.gameObject.SetActive(true);
        edgeRight.gameObject.SetActive(false);
    }
    private void rightEdge()
    {
        edgeUp.gameObject.SetActive(false);
        edgeDown.gameObject.SetActive(false);
        edgeLeft.gameObject.SetActive(false);
        edgeRight.gameObject.SetActive(true);
    }
    private void leftRightEdge()
    {
        edgeUp.gameObject.SetActive(false);
        edgeDown.gameObject.SetActive(false);
        edgeLeft.gameObject.SetActive(true);
        edgeRight.gameObject.SetActive(true);
    }
    private void downEdge()
    {
        edgeUp.gameObject.SetActive(false);
        edgeDown.gameObject.SetActive(true);
        edgeLeft.gameObject.SetActive(false);
        edgeRight.gameObject.SetActive(false);
    }
    private void rightDownEdge()
    {
        edgeUp.gameObject.SetActive(false);
        edgeDown.gameObject.SetActive(true);
        edgeLeft.gameObject.SetActive(false);
        edgeRight.gameObject.SetActive(true);
    }
    private void leftDownEdge()
    {
        edgeUp.gameObject.SetActive(false);
        edgeDown.gameObject.SetActive(true);
        edgeLeft.gameObject.SetActive(true);
        edgeRight.gameObject.SetActive(false);
    }
    private void leftRightDownEdge()
    {
        edgeUp.gameObject.SetActive(false);
        edgeDown.gameObject.SetActive(true);
        edgeLeft.gameObject.SetActive(true);
        edgeRight.gameObject.SetActive(true);
    }
    private void upEdge()
    {
        edgeUp.gameObject.SetActive(true);
        edgeDown.gameObject.SetActive(false);
        edgeLeft.gameObject.SetActive(false);
        edgeRight.gameObject.SetActive(false);
    }
    private void upDownEdge()
    {
        edgeUp.gameObject.SetActive(true);
        edgeDown.gameObject.SetActive(true);
        edgeLeft.gameObject.SetActive(false);
        edgeRight.gameObject.SetActive(false);
    }
    private void upRightEdge()
    {
        edgeUp.gameObject.SetActive(true);
        edgeDown.gameObject.SetActive(false);
        edgeLeft.gameObject.SetActive(false);
        edgeRight.gameObject.SetActive(true);
    }
    private void upRightDownEdge()
    {
        edgeUp.gameObject.SetActive(true);
        edgeDown.gameObject.SetActive(true);
        edgeLeft.gameObject.SetActive(false);
        edgeRight.gameObject.SetActive(true);
    }
    private void upLeftEdge()
    {
        edgeUp.gameObject.SetActive(true);
        edgeDown.gameObject.SetActive(false);
        edgeLeft.gameObject.SetActive(true);
        edgeRight.gameObject.SetActive(false);
    }
    private void upDownLeftEdge()
    {
        edgeUp.gameObject.SetActive(true);
        edgeDown.gameObject.SetActive(true);
        edgeLeft.gameObject.SetActive(true);
        edgeRight.gameObject.SetActive(false);
    }
    private void upLeftRightEdge()
    {
        edgeUp.gameObject.SetActive(true);
        edgeDown.gameObject.SetActive(false);
        edgeLeft.gameObject.SetActive(true);
        edgeRight.gameObject.SetActive(true);
    }
    private void allEdges()
    {
        edgeUp.gameObject.SetActive(true);
        edgeDown.gameObject.SetActive(true);
        edgeLeft.gameObject.SetActive(true);
        edgeRight.gameObject.SetActive(true);
    }
}