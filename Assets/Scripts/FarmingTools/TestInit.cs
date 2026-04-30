using System.Collections.Generic;
using UnityEngine;

public class TestInit : MonoBehaviour
{
    [SerializeField] private List<ItemData> testItems;
    [SerializeField] private List<int> testItemCounts;

    void Start()
    {
        for (int i = 0; i < testItems.Count; i++)
        {
            Inventory.Instance.AddItem(testItems[i], testItemCounts[i]);
        }
    }
}