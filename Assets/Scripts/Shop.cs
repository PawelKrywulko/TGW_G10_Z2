using Assets.Scripts.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [Header("List of items sold")]
    [SerializeField] private List<ShopItem> shopItems;

    [Header("References")]
    [SerializeField] private Transform shopContainer;
    [SerializeField] private GameObject shopItemPrefab;

    private List<GameObject> shopItemsReferences = new List<GameObject>();

    void Start()
    {
        PopulateShop();
    }

    private void PopulateShop()
    {
        shopItems.ForEach(item =>
        {
            GameObject itemObject = Instantiate(shopItemPrefab, shopContainer);
            itemObject.transform.GetChild(1).GetComponent<Image>().sprite = item.sprite;
            itemObject.transform.GetChild(1).GetComponent<Image>().color = item.skinColor;
            itemObject.transform.GetChild(0).GetComponent<Text>().text = item.itemName;
            itemObject.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => OnButtonClick(item));

            if (PlayerPrefs.GetInt(item.itemName, 0) == 0)
            {
                itemObject.transform.GetChild(2).GetChild(0).GetChild(1).GetComponent<Text>().text = item.price.ToString();
            }
            else
            {
                var coinImageTransform = itemObject.transform.GetChild(2).GetChild(0).GetChild(0).transform;
                Destroy(coinImageTransform.gameObject);
                itemObject.transform.GetChild(2).GetChild(0).GetChild(1).GetComponent<Text>().text = "SOLD";
            }
            
            shopItemsReferences.Add(itemObject);
        });
    }

    private void OnButtonClick(ShopItem item)
    {
        if(GameManager.allCoins >= item.price && PlayerPrefs.GetInt(item.itemName) == 0)
        {
            var itemReference = shopItemsReferences.First(itemRef => itemRef.transform.GetChild(0).GetComponent<Text>().text == item.itemName);
            var coinImageTransform = itemReference.transform.GetChild(2).GetChild(0).GetChild(0).transform;
            Destroy(coinImageTransform.gameObject);
            itemReference.transform.GetChild(2).GetChild(0).GetChild(1).GetComponent<Text>().text = "SOLD";

            var itemColor = itemReference.transform.GetChild(1).GetComponent<Image>().color;
            var colorHashStr = $"#{ColorUtility.ToHtmlStringRGBA(itemColor)}";
            
            PlayerPrefs.SetString("CurrentPlayerColor", colorHashStr);
            PlayerPrefs.SetInt(item.itemName, 1); //itemName == 1 => item SOLD

            GameEvents.Instance.HandleItemInShopBought(new ItemInShopBought
            {
                ItemColor = item.skinColor,
                ItemPrice = item.price
            });
        }

        if(PlayerPrefs.GetInt(item.itemName) == 1)
        {
            GameEvents.Instance.HandleItemInShopBought(new ItemInShopBought
            {
                ItemColor = item.skinColor,
                ItemPrice = 0
            });
        }
    }
}
