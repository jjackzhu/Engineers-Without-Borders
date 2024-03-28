using Backend;
using TMPro;
using UnityEngine;

// Original Author: Andy Wang
// Displays how many of a certain product you have in your inventory
public class ProductAmountDisplayer : MonoBehaviour
{
    [SerializeField] string productName;
    [SerializeField] string formatString;
    private TextMeshProUGUI _text;

    void Update()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _text.text = string.Format(formatString, GameState.s_Player.Inventory.GetAmount(productName));
    }
}