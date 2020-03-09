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
    private Color defaultBackground = new Color(0.0f, 0.0f, 0.0f, 100.0f / 255f);
    private Color lastlychosenColor = new Color(27.0f / 255f, 27.0f / 255f, 27.0f / 255f, 255.0f / 255f);
    private Animator buttonAnimator;

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

            var currentPlayerColorHexStr = $"#{ColorUtility.ToHtmlStringRGBA(itemObject.transform.GetChild(1).GetComponent<Image>().color)}";
            if (currentPlayerColorHexStr == PlayerPrefs.GetString("CurrentPlayerColor", "#000000"))
            {
                //change background ShopItem if currently used
                itemObject.GetComponent<Image>().color = lastlychosenColor;
            }
            
            shopItemsReferences.Add(itemObject);
        });
    }

    private void OnButtonClick(ShopItem item)
    {
        var itemReference = shopItemsReferences.First(itemRef => itemRef.transform.GetChild(0).GetComponent<Text>().text == item.itemName);
        var itemColor = itemReference.transform.GetChild(1).GetComponent<Image>().color;
        var colorHexStr = $"#{ColorUtility.ToHtmlStringRGBA(itemColor)}";
        var buttonAnimator = itemReference.transform.GetChild(2).GetComponent<Animator>();

        if (GameManager.allCoins >= item.price && PlayerPrefs.GetInt(item.itemName) == 0)
        {
            var coinImageTransform = itemReference.transform.GetChild(2).GetChild(0).GetChild(0).transform;
            Destroy(coinImageTransform.gameObject);
            itemReference.transform.GetChild(2).GetChild(0).GetChild(1).GetComponent<Text>().text = "SOLD";
            
            PlayerPrefs.SetString("CurrentPlayerColor", colorHexStr);
            PlayerPrefs.SetInt(item.itemName, 1); //itemName == 1 => item SOLD

            GameEvents.Instance.HandleItemInShopBought(new ItemInShopBought
            {
                ItemColor = item.skinColor,
                ItemPrice = item.price
            });
        }

        if(PlayerPrefs.GetInt(item.itemName) == 1)
        {
            PlayerPrefs.SetString("CurrentPlayerColor", colorHexStr);

            GameEvents.Instance.HandleItemInShopBought(new ItemInShopBought
            {
                ItemColor = item.skinColor,
                ItemPrice = 0
            });
        }

        if(GameManager.allCoins < item.price && PlayerPrefs.GetInt(item.itemName) == 0)
        {
            buttonAnimator.SetTrigger("NotEnoughMoney");
        } 
        else
        {
            //change backgrounds
            shopItemsReferences.ForEach(si => si.GetComponent<Image>().color = defaultBackground);
            itemReference.GetComponent<Image>().color = lastlychosenColor;
        }
    }
}
