using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {
    [Header("prefabs")]
    public GameObject itemPrefab;
    public GameObject emptyCellPrefab;

    [Header("refs")]
    public Transform empyCellFather;
    public GameObject itemsFather;
    public Button switchVisibilityButton;

    [Header("data")]
    public InventoryData inventoryData;

    [HideInInspector] public List<Item> items = new();
    [HideInInspector] public Unit owner;

    private Transform[,] emptyCells = new Transform[50, 50];

    public bool IsTrader { get { return inventoryData.ownerUnit.isTrader; } }

    private void OnValidate() {
        Assert.IsNotNull(itemPrefab);
        Assert.IsNotNull(emptyCellPrefab);
        Assert.IsNotNull(inventoryData);
        Assert.IsNotNull(empyCellFather);
        Assert.IsNotNull(switchVisibilityButton);
        foreach (var item in inventoryData.items) { // possibly use a prop?
            item.inventoryData = inventoryData;
        }
    }

    public void PlaceIntoPosition(Item item, Vector2Int newPos) {
        Assert.IsTrue(item.itemData.TryToFitSelf(item.itemData.position));
        //var rect = emptyCellPrefab.transform as RectTransform;
        item.transform.position = emptyCells[newPos.x, newPos.y].position;
    }

    public void SwitchVisibility() {
        itemsFather.SetActive(!itemsFather.activeSelf);
    }

    public Item AddItem(ItemData itemData) {
        var obj = Instantiate(itemPrefab, itemsFather.transform, false);
        var item = obj.GetComponent<Item>();
        items.Add(item);
        item.itemData = itemData;
        PlaceIntoPosition(item, itemData.position);
        return item;
    }

    private void Start() {
        var rect = emptyCellPrefab.transform as RectTransform;
        for (int x = 0; x < inventoryData.size.x; x++) {
            for (int y = 0; y < inventoryData.size.y; y++) {
                var obj = Instantiate(emptyCellPrefab, empyCellFather, false);
                emptyCells[x, y] = obj.transform;
                var slot = obj.GetComponent<EmptyInvSlot>();
                slot.position = new(x, y);
                slot.inventory = this;
                obj.name = $"{x} {y}";
                obj.transform.position += new Vector3(x * (rect.sizeDelta.x + 10), y * (rect.sizeDelta.y + 10));
            }
        }

        foreach (var itemData in inventoryData.items) {
            AddItem(itemData);
        }

        switchVisibilityButton.onClick.AddListener(SwitchVisibility);
    }
}
