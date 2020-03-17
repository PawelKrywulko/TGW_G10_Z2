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
    [SerializeField] private Transform tabsContainer;
    [SerializeField] private GameObject tabPrefab;
    [SerializeField] private List<string> tabNames;


    private List<GameObject> currentTabShopItemsReferences = new List<GameObject>();
    private List<GameObject> tabObjectReferences = new List<GameObject>();
    private Dictionary<string, List<ShopItem>> shopItemsDict = new Dictionary<string, List<ShopItem>>();
    private Color defaultBackground = new Color(129.0f / 255f, 218.0f / 255f, 238.0f / 255f, 150.0f / 255f);
    private Color lastlychosenColor = new Color(129.0f / 255f, 218.0f / 255f, 238.0f / 255f, 255.0f / 255f);

    void Start()
    {
        PopulateShopItemsDict();
        PopulateTabs();
        PopulateShop(tabNames.First());
    }

    private void PopulateShopItemsDict()
    {
        //Item name in the editor must contain tabName!
        tabNames.ForEach(tabName =>
        {
            shopItemsDict.Add(tabName.ToLower(), shopItems.Where(shopItem => shopItem.name.ToLower().Contains(tabName.ToLower())).ToList());
        });
    }

    private void PopulateTabs()
    {
        tabNames.ForEach(tabName =>
        {
            GameObject tabObject = Instantiate(tabPrefab, tabsContainer);
            tabObject.GetComponent<Button>().onClick.AddListener(() => OnTabClick(tabObject, tabName));
            tabObject.transform.GetChild(0).GetComponent<Text>().text = tabName;
            tabObjectReferences.Add(tabObject);
        });

        tabObjectReferences.First().transform.GetChild(0).GetComponent<Shadow>().enabled = true;
        tabObjectReferences.First().transform.GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3(0, 15);
    }

    private void OnTabClick(GameObject tabObject, string tabName)
    {
        tabObjectReferences.ForEach(tab =>
        {
            tab.transform.GetChild(0).GetComponent<Shadow>().enabled = false;
            tab.transform.GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3(0, 0);
        });

        var chosenTab = tabObjectReferences.First(tabObj => tabObj == tabObject).transform.GetChild(0);
        chosenTab.GetComponent<Shadow>().enabled = true;
        chosenTab.GetComponent<RectTransform>().localPosition = new Vector3(0, 15);

        //clear container
        foreach (Transform child in shopContainer)
        {
            Destroy(child.gameObject);
        }

        PopulateShop(tabName);
    }

    private void PopulateShop(string tabName)
    {
        tabName = tabName.ToLower();
        currentTabShopItemsReferences = new List<GameObject>();

        shopItemsDict[tabName].ForEach(item =>
        {
            GameObject itemObject = Instantiate(shopItemPrefab, shopContainer);
            itemObject.transform.GetChild(1).GetComponent<Image>().sprite = item.sprite;
            itemObject.transform.GetChild(1).GetComponent<Image>().color = item.skinColor;
            itemObject.transform.GetChild(0).GetComponent<Text>().text = item.itemName;
            itemObject.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => OnButtonClick(item, tabName));

            //change price to SOLD if bought previously
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
            if (tabName == "color" && currentPlayerColorHexStr == PlayerPrefs.GetString("CurrentPlayerColor", "#000000"))
            {
                //change background ShopItem if currently used
                itemObject.GetComponent<Image>().color = lastlychosenColor;
            }
            var currentPlayerSkin = PlayerPrefs.GetString("CurrentPlayerSkin", "Skin00");
            if (tabName == "skin" && item.itemName == Player.skinDictionary[currentPlayerSkin])
            {
                //change background ShopItem if currently used
                itemObject.GetComponent<Image>().color = lastlychosenColor;
            }
            
            currentTabShopItemsReferences.Add(itemObject);
        });
    }

    private void OnButtonClick(ShopItem item, string tabName)
    {
        var itemReference = currentTabShopItemsReferences.First(itemRef => itemRef.transform.GetChild(0).GetComponent<Text>().text == item.itemName);
        var itemColor = itemReference.transform.GetChild(1).GetComponent<Image>().color;
        var colorHexStr = $"#{ColorUtility.ToHtmlStringRGBA(itemColor)}";
        var buttonAnimator = itemReference.transform.GetChild(2).GetComponent<Animator>();

        if (GameManager.allCoins >= item.price && PlayerPrefs.GetInt(item.itemName) == 0)
        {
            var coinImageTransform = itemReference.transform.GetChild(2).GetChild(0).GetChild(0).transform;
            Destroy(coinImageTransform.gameObject);
            itemReference.transform.GetChild(2).GetChild(0).GetChild(1).GetComponent<Text>().text = "SOLD";
            
            PlayerPrefs.SetInt(item.itemName, 1); //itemName == 1 => item SOLD
            
            if(tabName == "color")
            {
                PlayerPrefs.SetString("CurrentPlayerColor", colorHexStr);
                
                GameEvents.Instance.HandleItemInShopBought(new ItemInShopBought
                {
                    ItemColor = item.skinColor,
                    ItemPrice = item.price,
                    ItemSkinName = null
                });
            }
            else if (tabName == "skin")
            {
                var boughtSkin = Player.skinDictionary.First(x => x.Value == item.itemName).Key;
                PlayerPrefs.SetString("CurrentPlayerSkin", boughtSkin);
                
                GameEvents.Instance.HandleItemInShopBought(new ItemInShopBought
                {
                    ItemColor = null,
                    ItemPrice = item.price,
                    ItemSkinName = boughtSkin
                });
            }
            else
            {
                Debug.LogWarning("Wrong tabName");
            }
        }

        if(tabName == "color" && PlayerPrefs.GetInt(item.itemName) == 1)
        {
            PlayerPrefs.SetString("CurrentPlayerColor", colorHexStr);

            GameEvents.Instance.HandleItemInShopBought(new ItemInShopBought
            {
                ItemColor = item.skinColor,
                ItemPrice = 0,
                ItemSkinName = null
            });
        }
        if (tabName == "skin" && PlayerPrefs.GetInt(item.itemName) == 1)
        {
            var boughtSkin = Player.skinDictionary.First(x => x.Value == item.itemName).Key;
            PlayerPrefs.SetString("CurrentPlayerSkin", boughtSkin);

            GameEvents.Instance.HandleItemInShopBought(new ItemInShopBought
            {
                ItemColor = null,
                ItemPrice = 0,
                ItemSkinName = boughtSkin
            });
        }

        if (GameManager.allCoins < item.price && PlayerPrefs.GetInt(item.itemName) == 0)
        {
            buttonAnimator.SetTrigger("NotEnoughMoney");
        } 
        else
        {
            //change backgrounds
            currentTabShopItemsReferences.ForEach(si => si.GetComponent<Image>().color = defaultBackground);
            itemReference.GetComponent<Image>().color = lastlychosenColor;
        }
    }
}
