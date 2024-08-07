using System.Collections;
using System.Collections.Generic;
using SunkenRuins;
using UnityEngine;

public class ThrowingCrabStat : EnemyStat
{
    public float patrolRange { get { return 5f; } }
    public float initialMoveSpeed { get { return 4f; } }
}