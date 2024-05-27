using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunkenRuins
{
    public interface IDamageable
    {
        TeamType teamType { get; }
        void Damage(int damageAmount);
    }
}