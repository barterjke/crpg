using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Item : MonoBehaviour {
    public ItemData itemData;
    public Image spriteImage;
    public TMP_Text countLabel;
    public TMP_Text costLabel;

    public CanvasGroup canvasGroup;
    public Draggable draggable;

    public Inventory inventory;

    private BattleManager battleManager;
    private TradingManager tradingManager;

    void OnValidate() {
        Assert.IsNotNull(itemData, name);
        Assert.IsNotNull(spriteImage, name);
        Assert.IsNotNull(countLabel, name);
        Assert.IsNotNull(costLabel, name);
        Assert.IsNotNull(canvasGroup, name);
        Assert.IsNotNull(draggable, name);
    }

    void Start() {
        battleManager = FindAnyObjectByType<BattleManager>();
        tradingManager = FindAnyObjectByType<TradingManager>();
        Assert.IsNotNull(inventory);

        countLabel.gameObject.SetActive(itemData.expendable);
        SetCount(itemData.GetCount());

        if (!tradingManager) {
            costLabel.gameObject.SetActive(false);
        } else {
            UpdateCostLabel();
        }

        spriteImage.sprite = itemData.sprite;
        InitRotation();
    }

    public void InitRotation() {
        var rects = inventory.emptyCellPrefab.GetComponentsInChildren<RectTransform>();
        //Assert.IsTrue(slotRect.sizeDelta.x == slotRect.sizeDelta.y);
        var width = rects[0].sizeDelta.x;
        var padding = width - rects[1].sizeDelta.x;
        var slotSize = new Vector2(itemData.size.x * width - padding,
                                itemData.size.y * width - padding);
        if (itemData.isRotated) {
            //draggable.Rotate();
            spriteImage.transform.Rotate(0, 0, 90);
            (spriteImage.transform as RectTransform).sizeDelta = new(slotSize.y, slotSize.x);
        } else {
            (spriteImage.transform as RectTransform).sizeDelta = slotSize;
        }
    }

    public void TransferToInventory(Inventory newInventory) {
        print($"item {itemData} removing from inv {itemData.inventoryData}");
        foreach (var item in itemData.inventoryData.items) {
            print($"{item} {itemData} {item == itemData}");
        }
        bool res = itemData.inventoryData.items.Remove(itemData);
        print(res);
        newInventory.inventoryData.items.Add(itemData);
        itemData.inventoryData = newInventory.inventoryData; // should be a two way binding?
        inventory = newInventory;
        transform.SetParent(newInventory.itemsFather.transform);
    }

    public void RemoveSelf() {
        DestroyImmediate(itemData, true); // todo check if it works
        Destroy(gameObject);
    }

    public bool MoveTo(Inventory targetInventory, Vector2Int newPos) {
        var belongsToEnemy = !inventory.inventoryData.ControlledByPlayer;
        if (battleManager && belongsToEnemy) {
            print("item belongs to enemy");
            return false;
        }

        if (targetInventory.inventoryData.IsFreeSpot(itemData, newPos)) {
            if (inventory != targetInventory) {
                if (!tradingManager.Trade(this)) return false;
                TransferToInventory(targetInventory);
            }
            itemData.position = newPos;
            return true;
        }

        var stackableItem = targetInventory.GetStackable(itemData, newPos);
        if (stackableItem) {
            print("stacking two items");
            stackableItem.SetCount(stackableItem.itemData.GetCount() + itemData.GetCount());
            draggable.SwitchAllItemsCanvasGroup(true);
            RemoveSelf();
            return true;
        }

        return false;
    }

    public float ApplyAttacks(ActiveItemData itemData) {
        float maxTime = 0f;
        foreach (var attack in itemData.attacks) {
            var units = battleManager.GetAffectedUnits(inventory.owner, attack.area);
            units.ForEach(unit => {
                maxTime = Mathf.Max(maxTime, unit.ApplyAttack(attack));
            });
            units.ForEach(unit => unit.UpdateHealthDisplay());
        }
        battleManager.audioSource.PlayOneShot(itemData.audioClip, 0.5f);
        battleManager.Reset();
        return maxTime;
    }

    public void UpdateCostLabel() {
        costLabel.text = itemData.cost == 0 ? "" : "g" + itemData.cost.ToString();
    }

    public void UpdateCountLabel() {
        if (itemData.expendable)
            countLabel.text = "x" + itemData.GetCount().ToString();
    }

    public void SetCount(int value) {
        itemData.SetCount(value, UpdateCountLabel);
    }

    public float Act() {
        if (tradingManager) {
            tradingManager.Trade(this);
            return 0;
        }
        if (itemData.inventoryData.ControlledByPlayer != battleManager.playerTurn) return 0;
        var activeItemData = itemData as ActiveItemData;
        if (!activeItemData) return 0f;
        bool containsTargetSpell = activeItemData.attacks.Any(i => i.area == Area.SelectedUnit || i.area == Area.SelfAndEnemy);
        if (containsTargetSpell && !battleManager.currentTarget) {
            battleManager.ChooseTarget(this);
            return 0;
        } else {
            var time = ApplyAttacks(activeItemData);
            if (itemData.expendable) {
                itemData.SetCount(itemData.GetCount() - 1, UpdateCountLabel);
            }
            return time;
        }
    }
}
