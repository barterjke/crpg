using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TextCore.Text;

public class RandomAI : AbstractAI {
    public Unit unit;
    public BattleManager battleManager;

    private void Start() {
        Assert.IsNotNull(unit, name);
        Assert.IsNotNull(battleManager, name);
    }

    public override IEnumerator Think() {
        var items = unit.inventory.items;
        if (items.Count == 0) {
            battleManager.SwitchTurn();
        }
        var randomItem = items.PickRandom();
        foreach (var item in items) {
            item.spriteImage.color = Color.yellow;
            yield return new WaitForSeconds(1);
            item.spriteImage.color = Color.white;
            if (item == randomItem) {
                battleManager.currentItem = item;
                battleManager.currentTarget = battleManager.playerUnits.PickRandom();
                var time = item.Act();
                yield return new WaitForSeconds(time);
                battleManager.SwitchTurn();
                break;
            }
        }
    }
}
