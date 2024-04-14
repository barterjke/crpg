using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public class SelectableUnit : MonoBehaviour, IPointerClickHandler {
    public Unit unit;
    
    private BattleManager battleManager;

    void Start() {
        battleManager = FindAnyObjectByType<BattleManager>();
        Assert.IsNotNull(battleManager);
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left || !battleManager.currentItem) return;
        battleManager.currentTarget = unit;
        battleManager.currentItem.Act();
    }
}
