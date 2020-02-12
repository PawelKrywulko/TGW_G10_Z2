using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] Text bankText;
    [SerializeField] Text collectedCoinsText;

    public void UpdateCollectedCoins(int collectedCoins)
    {
        collectedCoinsText.text = collectedCoins.ToString();
    }

    public void UpdateBankedCoins(int collectedCoins)
    {
        bankText.text = collectedCoins.ToString();
    }
}
