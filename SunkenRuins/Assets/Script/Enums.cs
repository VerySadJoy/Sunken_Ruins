using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunkenRuins
{
    public enum ItemType
    {
        HealthPotion,
        PowerBattery,
        BubbleShield,
    }

    public enum TeamType
    {
        Player,
        Monster,
        NPC,
    }

    /// <summary>
    /// Determines the Event type that is used for the EventManager.
    /// </summary>
    public enum EventType
    {
        StingRayAttack,
        StingRayParalyze,
        StingRayMoveTowardsPlayer,
        StingRayPrepareAttack,
        HypnoCuttleFishHypnotize,
        ShellAbsorb,
        ShellRelease,
        ShellAttack,

    }
}