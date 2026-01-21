using TMPro;
using UnityEngine;

public class Coin : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    TextMeshProUGUI _coinText;
    public void UpdateCoin(string moneyvalue)
    {
        _coinText.text = moneyvalue;
    }
}
