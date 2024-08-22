using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomDetect : MonoBehaviour
{
    [SerializeField] private LayerMask wallLayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == (int)wallLayer)
        {
            
        }
    }
}
