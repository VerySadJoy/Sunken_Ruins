using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class WoodTile : Tile
{
    private bool isUpLeftFull;
    private bool isUpRightFull;
    private bool isDownLeftFull;
    private bool isDownRightFull;

    // Coral Reef Prefabs
    [SerializeField] private GameObject[] coralReefPrefabs;

    [Space(10)]

    [SerializeField]
    protected Sprite NoBlank;

    [SerializeField]
    protected Sprite RightBlank;

    [SerializeField]
    protected Sprite LeftBlank;

    [SerializeField]
    protected Sprite LeftRightBlank;

    [SerializeField]
    protected Sprite UpBlank;

    [SerializeField]
    protected Sprite UpRightBlank;

    [SerializeField]
    protected Sprite UpLeftRightBlank;

    [SerializeField]
    protected Sprite UpLeftBlank;

    [SerializeField]
    protected Sprite UpDownRightBlank;

    [SerializeField]
    protected Sprite UpDownLeftBlank;

    [SerializeField]
    protected Sprite UpDownBlank;

    [SerializeField]
    protected Sprite AllBlank;

    [SerializeField]
    protected Sprite DownRightBlank;

    [SerializeField]
    protected Sprite DownLeftBlank;

    [SerializeField]
    protected Sprite DownLeftRightBlank;

    [SerializeField]
    protected Sprite DownBlank;

    // Corners Start from Below:

    [SerializeField]
    protected Sprite UpLeftCorner;

    [SerializeField]
    protected Sprite UpRightCorner;

    [SerializeField]
    protected Sprite UpCorners;

    [SerializeField]
    protected Sprite DownFullUpLeftCorner;

    [SerializeField]
    protected Sprite DownFullUpRightCorner;

    [SerializeField]
    protected Sprite DownLeftFullUpRightCorner;

    [SerializeField]
    protected Sprite DownFullUpCorners;

    [SerializeField]
    protected Sprite DownRightFullUpLeftCorner;

    [SerializeField]
    protected Sprite LeftFullUpRightCorner;

    [SerializeField]
    protected Sprite RightFullUpLeftCorner;

    private void Start()
    {
        // Checking Each Side
        checkTileBoundaries();

        bool areFourSidesFull = isUpFull || isLeftFull || isRightFull || isDownFull;
        bool areFourCornersFull = isUpLeftFull || isUpRightFull || isDownLeftFull || isDownRightFull;
        if (areFourSidesFull)
        {
            // When a block is above this block, no coral reef is instantiated
            if (isUpFull)
            {
                if (!isUpLeftFull)
                {
                    if (!isDownFull)
                    {
                        spriteRenderer.sprite = DownFullUpLeftCorner;
                    }
                    else if (!isRightFull)
                    {
                        spriteRenderer.sprite = RightFullUpLeftCorner;
                    }
                    else if (!isLeftFull)
                    {
                    }
                    else
                    {
                        spriteRenderer.sprite = UpLeftCorner;
                        spriteRenderer.sprite = LeftBlank;
                    }
                }
                else if (!isUpRightFull)
                {
                    if (!isDownFull && !isLeftFull)
                    {
                        // 오른쪽 대각선 + 왼쪽 and 아래 테두리
                    }
                    else if (!isDownFull)
                    {
                        spriteRenderer.sprite = DownFullUpRightCorner;
                    }
                    else if (!isLeftFull)
                    {
                        spriteRenderer.sprite = DownFullUpRightCorner;
                    }
                    else if (!isRightFull)
                    {
                    }
                    else
                    {
                        spriteRenderer.sprite = UpRightCorner;
                        spriteRenderer.sprite = RightBlank;
                    }
                }

                if (isDownFull)
                {
                    if (isRightFull && isLeftFull)
                    {
                        if (isUpLeftFull)
                        {
                            if (isUpRightFull)
                            {
                                spriteRenderer.sprite = AllBlank;
                            }
                            else
                            {
                                spriteRenderer.sprite = UpRightCorner;
                            }
                        }
                        else if (isUpRightFull)
                        {
                            spriteRenderer.sprite = UpLeftCorner;
                        }
                        else 
                        {
                            spriteRenderer.sprite = UpCorners;
                        }
                    }
                    else if (isRightFull)
                    {
                        if (isDownRightFull && isUpRightFull)
                        {
                            spriteRenderer.sprite = UpDownRightBlank;
                        }
                        else if (isDownRightFull)
                        {
                            spriteRenderer.sprite = LeftFullUpRightCorner;
                        }
                    }
                    else if (isLeftFull)
                    {
                        if (isUpLeftFull && isDownLeftFull)
                        {
                            spriteRenderer.sprite = UpDownLeftBlank;
                        }
                        else if (isDownLeftFull)
                        {
                            spriteRenderer.sprite = RightFullUpLeftCorner;
                        }
                    }
                    else
                    {
                        spriteRenderer.sprite = UpDownBlank;
                    }
                }
                else
                {
                    if (isRightFull && isLeftFull)
                    {
                        if (isUpLeftFull && isUpRightFull)
                        {
                            spriteRenderer.sprite = UpLeftRightBlank; 
                        }
                        else if (isUpLeftFull)
                        {
                            spriteRenderer.sprite = DownFullUpRightCorner;
                        }
                        else if (isUpRightFull)
                        {
                            spriteRenderer.sprite = DownFullUpLeftCorner;
                        }
                        else
                            spriteRenderer.sprite = DownFullUpCorners;
                    }
                    else if (isLeftFull) // 아래랑 오른쪽이 비었음
                    {
                        if (!isUpLeftFull)
                        {
                            spriteRenderer.sprite = DownRightFullUpLeftCorner;
                        }
                        else spriteRenderer.sprite = UpLeftBlank;
                    }
                    else if (isRightFull) // 아래랑 왼쪽이 비었음
                    {
                        if (!isUpRightFull)
                        {
                            spriteRenderer.sprite = DownLeftFullUpRightCorner;
                        }
                        else spriteRenderer.sprite = UpRightBlank;
                    }
                    else // 위를 제외하고 다 비었음
                    {
                        spriteRenderer.sprite = UpBlank;
                    }
                }
            }
            else // Since there is no block above this block, we can instantiate coral reef
            {
                RandomInstantiateCoralReef();

                if (isLeftFull) // 위가 비었음
                {
                    if (!isUpLeftFull && !isUpRightFull)
                    {
                        spriteRenderer.sprite = UpCorners;
                    }

                    if (isRightFull)
                    {
                        if (isDownFull)
                        {
                            spriteRenderer.sprite = DownLeftRightBlank; 
                        }
                        else
                        {
                            spriteRenderer.sprite = LeftRightBlank;
                        }
                    }
                    else // 위랑 오른쪽이 비었음
                    {
                        if (isDownFull)
                        {
                            spriteRenderer.sprite = DownLeftBlank;
                        }
                        else
                        {
                            spriteRenderer.sprite = LeftBlank;
                        }
                    }
                }
                else if (isRightFull) // 위랑 왼쪽이 비었음
                {
                    if (isDownFull)
                    {
                        spriteRenderer.sprite = DownRightBlank;
                    }
                    else
                    {
                        spriteRenderer.sprite = RightBlank;
                    }
                }
                else if (isDownFull) // 위, 왼, 오가 비었음
                {
                    spriteRenderer.sprite = DownBlank;
                }
            }
        }
        else
        {
            spriteRenderer.sprite = NoBlank;
        }
    }

    private void checkOnlyForCorners(bool areFourCornersFull)
    {
        if (areFourCornersFull)
        {
            if (isUpLeftFull)
            {
                if (isUpRightFull)
                {
                    spriteRenderer.sprite = AllBlank;
                }
                else
                {
                    spriteRenderer.sprite = UpRightCorner;
                }
            }
            else if (isUpRightFull)
            {
                spriteRenderer.sprite = UpLeftCorner;
            }
            else 
            {
                spriteRenderer.sprite = UpCorners;
            }
        }
        else
        {
            Debug.Log("Error in Tile Creation: Script [WoodTile] Line 298");
        }
    }

    public override void checkTileBoundaries()
    {
        base.checkTileBoundaries();
        isUpLeftFull = Physics2D.OverlapCircle(transform.position + 2*Vector3.left + 2*Vector3.up, 0.1f, tileLayerMask);
        isUpRightFull = Physics2D.OverlapCircle(transform.position + 2*Vector3.right + 2*Vector3.up, 0.1f, tileLayerMask);
        isDownLeftFull = Physics2D.OverlapCircle(transform.position + 2*Vector3.left + 2*Vector3.down, 0.1f, tileLayerMask);
        isDownRightFull = Physics2D.OverlapCircle(transform.position + 2*Vector3.right + 2*Vector3.down, 0.1f, tileLayerMask);
    }

    private void RandomInstantiateCoralReef()
    {
        // Probability of having coral reef above block = 20%
        bool isInstantiate = Random.Range(0, 10) < 2;

        if (isInstantiate)
        {
            // Randomly select coral reef from coralReefPrefabs
            int randomIndex = Random.Range(0, coralReefPrefabs.Length);
            Instantiate(coralReefPrefabs[randomIndex], this.transform.position, Quaternion.identity);
        }
    }
}