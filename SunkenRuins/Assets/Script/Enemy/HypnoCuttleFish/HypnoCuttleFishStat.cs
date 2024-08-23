using System.Collections;
using System.Collections.Generic;
using SunkenRuins;
using UnityEngine;

public class HypnoCuttleFishStat : EnemyStat
{
    private void Start()
    {
        base.Start();
        damageAmount = 10;
    }
}