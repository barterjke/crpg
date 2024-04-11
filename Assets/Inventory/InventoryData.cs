using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/InventoryData", order = 1)]
public class InventoryData : ScriptableObject {
    public List<ItemData> items;
    [Min(1)] public Vector2Int size;
    public UnitData ownerUnit;

    public bool ControlledByPlayer {  get { return ownerUnit.controlledByPlayer || ownerUnit.isTrader; } }

    private void OnValidate() {
        Assert.IsNotNull(ownerUnit);
        Assert.IsTrue(size.x > 0 &&  size.y > 0);
        foreach (ItemData item in items) {
            Assert.IsNotNull(item);
        }
    }

    public bool TryToFit(ItemData newItem, Vector2Int newPos) {
        foreach (var item in items) {
            if (item == newItem) continue;
            bool insideX = newPos.x < item.position.x + item.size.x && newPos.x + newItem.size.x > item.position.x;
            bool insideY = newPos.y < item.position.y + item.size.y && newPos.y + newItem.size.y > item.position.y;
            if (insideX && insideY) return false;
        }
        return true;
    }
}