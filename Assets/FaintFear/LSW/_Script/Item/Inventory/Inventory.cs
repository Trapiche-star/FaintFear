using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    private HashSet<string> items = new HashSet<string>();

    private void Awake()
    {
        Instance = this;
    }

    public void AddItem(string itemName)
    {
        items.Add(itemName);
    }

    public bool HasItem(string itemName)
    {
        return items.Contains(itemName);
    }
}
