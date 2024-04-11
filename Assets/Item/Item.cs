using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class Item : MonoBehaviour {
    public ItemData itemData;
    public Image spriteImage;

    public Inventory GetInventory() {
        return transform.parent.parent.GetComponent<Inventory>();
    }

    void OnValidate() {
        Assert.IsNotNull(itemData);
        Assert.IsNotNull(spriteImage);
    }

    void Start() {
        spriteImage.sprite = itemData.sprite;
        if (itemData.isRotated) {
            spriteImage.transform.Rotate(0, 0, 90);
            (spriteImage.transform as RectTransform).sizeDelta = new Vector2(itemData.size.y, itemData.size.x) * 100;

        } else
        (spriteImage.transform as RectTransform).sizeDelta = itemData.size * 100;
    }

    public void TransferToInventory(Inventory newInventory) {
        itemData.inventoryData.items.Remove(itemData);
        newInventory.inventoryData.items.Add(itemData);
        itemData.inventoryData = newInventory.inventoryData; // should be a two way binding?
        transform.SetParent(newInventory.itemsFather.transform);
    }

    void Update() {

    }
}
