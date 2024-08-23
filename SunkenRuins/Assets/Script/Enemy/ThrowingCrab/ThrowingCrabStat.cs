using System.Collections;
using System.Collections.Generic;
using SunkenRuins;
using UnityEngine;

public class ThrowingCrabStat : EnemyStat
{
    private float patrolRange = 6f; public float PatrolRange { get { return patrolRange; } }
    private float initialMoveSpeed = 4f; public float InitialMoveSpeed { get { return initialMoveSpeed; } }
}