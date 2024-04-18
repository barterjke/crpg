using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public class SelectableUnit : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    public Unit unit;
    
    private BattleManager battleManager;

    void Start() {
        battleManager = FindAnyObjectByType<BattleManager>();
        Assert.IsNotNull(battleManager);
    }

    public void UnselectOther() {
        foreach (var unit in battleManager.AllUnits) {
            unit.image.transform.localScale = Vector3.one;
        }
        var curInv = FindObjectsByType<Inventory>(FindObjectsSortMode.None).First(
            inv => inv.inventoryData.ControlledByPlayer == unit.unitData.controlledByPlayer
        );
        curInv.gameObject.SetActive(false);
    }

    public void Select() {
        unit.image.transform.localScale = new Vector2(1.5f, 1.5f);
        unit.inventory.gameObject.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (battleManager.currentItem) {
            battleManager.currentTarget = unit;
            battleManager.currentItem.Act();
        } else {
            UnselectOther();
            Select();
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (unit.image.color == Color.white) {
            unit.image.color = Color.yellow;
        } 
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (unit.image.color == Color.yellow) {
            unit.image.color = Color.white;
        }
    }
}
