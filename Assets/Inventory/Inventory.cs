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
        Assert.IsNotNull(itemPrefab, name);
        Assert.IsNotNull(emptyCellPrefab, name);
        Assert.IsNotNull(inventoryData, name);
        Assert.IsNotNull(empyCellFather, name);
        Assert.IsNotNull(switchVisibilityButton, name);
        foreach (var item in inventoryData.items) { // possibly use a prop?
            item.inventoryData = inventoryData;
        }
    }

    public Item GetStackable(ItemData other, Vector2Int newPos) {
        foreach (var item in items) {
            var itemData = item.itemData;
            if (itemData.position == newPos) {
                return itemData.Equals(other) ? item : null;
            }
        }
        return null;
    }

    public void PlaceIntoPosition(Item item, Vector2Int newPos) {
        Assert.IsTrue(item.itemData.TryToFitSelf(item.itemData.position), name);
        item.transform.position = emptyCells[newPos.x, newPos.y].position;
    }

    public void SwitchVisibility() {
        itemsFather.SetActive(!itemsFather.activeSelf);
    }

    public Item AddItem(ItemData itemData) {
        var obj = Instantiate(itemPrefab, itemsFather.transform, false);
        obj.name = itemData.name[3..];
        var item = obj.GetComponent<Item>();
        items.Add(item);
        item.itemData = itemData;
        item.inventory = this;
        PlaceIntoPosition(item, itemData.position);
        return item;
    }

    private void Start() {
        var rect = emptyCellPrefab.transform as RectTransform;
        for (int x = 0; x < inventoryData.size.x; x++) {
            for (int y = 0; y < inventoryData.size.y; y++) {
                var obj = Instantiate(emptyCellPrefab, empyCellFather, false);
                var slot = obj.GetComponent<EmptyInvSlot>();
                emptyCells[x, y] = slot.image.transform;
                slot.position = new(x, y);
                slot.inventory = this;
                obj.name = $"{x} {y}";
                var updatedX = x - inventoryData.size.x / 2;
                obj.transform.position += new Vector3(updatedX * rect.sizeDelta.x, y * rect.sizeDelta.y);
            }
        }

        foreach (var itemData in inventoryData.items) {
            AddItem(itemData);
        }

        switchVisibilityButton.onClick.AddListener(SwitchVisibility);
    }
}
