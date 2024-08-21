using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coral_Reef : MonoBehaviour
{
    private SpriteRenderer SR;

    [SerializeField] private GameObject[] coralReefObjects;

    private void Awake()
    {
        SR = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        
    }
}
