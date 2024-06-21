using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    protected SpriteRenderer spriteRenderer;

    [SerializeField]
    protected GameObject edgeUp;

    [SerializeField]
    protected GameObject edgeDown;

    [SerializeField]
    protected GameObject edgeLeft;

    [SerializeField]
    protected GameObject edgeRight;

    [Space(10)]

    [SerializeField]
    protected LayerMask tileLayerMask;

    protected bool isUpFull;
    protected bool isLeftFull;
    protected bool isRightFull;
    protected bool isDownFull;

    public virtual void checkTileBoundaries()
    {
        // Checking Each Side
        isUpFull = Physics2D.OverlapCircle(transform.position + Vector3.up, 0.1f, tileLayerMask);
        isLeftFull = Physics2D.OverlapCircle(transform.position + Vector3.left, 0.1f, tileLayerMask);
        isRightFull = Physics2D.OverlapCircle(transform.position + Vector3.right, 0.1f, tileLayerMask);
        isDownFull = Physics2D.OverlapCircle(transform.position + Vector3.down, 0.1f, tileLayerMask);
    }
}