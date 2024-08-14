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
        GetComponent<Tile>().checkTileBoundaries();

        bool areFourSidesFull = isUpFull || isLeftFull || isRightFull || isDownFull;
        bool areFourCornersFull = isUpLeftFull || isUpRightFull || isDownLeftFull || isDownRightFull;
        if (areFourSidesFull)
        {
            if (isUpFull)
            {
                if (isDownFull)
                {
                    if (isRightFull && isLeftFull)
                    {
                        // �̶� ���� ������ �𼭸��� ���� �� �ֱ⿡
                        // ���� ���� ����� ���� �����Ѵ�.
                        checkOnlyForCorners(areFourCornersFull);
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
                        else if (isUpRightFull)
                        {
                        }
                    }
                    else if (isLeftFull)
                    {
                        if (isUpLeftFull && isDownLeftFull)
                        {
                            spriteRenderer.sprite = UpDownLeftBlank;
                        }
                        else if (isUpLeftFull)
                        {
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
                    else if (isLeftFull)
                    {
                        if (!isUpLeftFull)
                        {
                            spriteRenderer.sprite = DownRightFullUpLeftCorner;
                        }
                        else spriteRenderer.sprite = UpLeftBlank;
                    }
                    else if (isRightFull)
                    {
                        if (!isUpRightFull)
                        {
                            spriteRenderer.sprite = DownLeftFullUpRightCorner;
                        }
                        else spriteRenderer.sprite = UpRightBlank;
                    }
                    else
                    {
                        spriteRenderer.sprite = UpBlank;
                    }
                }
            }
            else if (isLeftFull)
            {
                if (isRightFull)
                {
                    if (isDownFull)
                    {
                        if (isDownLeftFull && isDownRightFull)
                        {
                            spriteRenderer.sprite = DownLeftRightBlank; 
                        }
                    }
                    else
                    {
                        spriteRenderer.sprite = LeftRightBlank;
                    }
                }
                else
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
            else if (isRightFull)
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
            else if (isDownFull)
            {
                spriteRenderer.sprite = DownBlank;
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
                if (isDownLeftFull)
                {
                    if (isUpRightFull && isDownRightFull)
                    {
                        spriteRenderer.sprite = AllBlank;
                    }
                    else if (isUpRightFull)
                    {
                    }
                    else if (isDownRightFull)
                    {
                        spriteRenderer.sprite = UpRightCorner;
                    }
                }
                else
                {
                    if (isUpRightFull && isDownRightFull)
                    {
                    }
                    else if (isUpRightFull)
                    {
                    }
                    else if (isDownRightFull)
                    {
                    }
                }
            }
            // UpLeftCorner �ʼ�
            else if (isUpRightFull)
            {
                if (isDownRightFull && isDownLeftFull)
                {
                    spriteRenderer.sprite = UpLeftCorner;
                }
                else if (isDownRightFull)
                {
                }
                else if (isDownLeftFull)
                {
                }
            }
            else if (isDownLeftFull)
            {
                if (isDownRightFull)
                {
                    spriteRenderer.sprite = UpCorners;
                }
            }
            else if (isDownRightFull)
            {
            }
        }
        else
        {
        }
    }

    // WoodTile�� �밢�� ��ġ�� Ȯ���ؾ� ��
    public override void checkTileBoundaries()
    {
        base.checkTileBoundaries();
        isUpLeftFull = Physics2D.OverlapCircle(transform.position + Vector3.left + Vector3.up, 0.1f, tileLayerMask);
        isUpRightFull = Physics2D.OverlapCircle(transform.position + Vector3.right + Vector3.up, 0.1f, tileLayerMask);
        isDownLeftFull = Physics2D.OverlapCircle(transform.position + Vector3.left + Vector3.down, 0.1f, tileLayerMask);
        isDownRightFull = Physics2D.OverlapCircle(transform.position + Vector3.right + Vector3.down, 0.1f, tileLayerMask);
    }
}