using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunkenRuins {
    [CreateAssetMenu(menuName = "MapAsset")]
    public class MapAsset : ScriptableObject {
        public int mapId;
        public string mapName;
    }
}