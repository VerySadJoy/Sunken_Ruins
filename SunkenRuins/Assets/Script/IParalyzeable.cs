using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunkenRuins
{
    public interface IParalyzeable
    {
        public bool IsParalyzed { get; }

        public void Paralyze(Dictionary<string, object> message);
    }
}