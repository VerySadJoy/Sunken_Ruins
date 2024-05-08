using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunkenRuins {
    public class Item : MonoBehaviour
    {
        [SerializeField] private ItemSO itemSO;

        public ItemSO GetItemSO() {
            return itemSO;
        }
        
    }
}