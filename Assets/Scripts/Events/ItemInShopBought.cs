using UnityEngine;

namespace Assets.Scripts.Events
{
    public class ItemInShopBought
    {
        public Color? ItemColor { get; set; }
        public int ItemPrice { get; set; }
        public string ItemSkinName { get; set; }
    }
}
