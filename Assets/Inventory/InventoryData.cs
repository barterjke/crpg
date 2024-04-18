using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.U2D;
using static UnityEditor.Progress;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/InventoryData", order = 1)]
public class InventoryData : ScriptableObject {
    public List<ItemData> items;
    [Min(1)] public Vector2Int size;
    public UnitData ownerUnit;

    public bool ControlledByPlayer { get { return ownerUnit.controlledByPlayer || ownerUnit.isTrader; } }

    private void OnValidate() {
        Assert.IsTrue(size.x > 0 && size.y > 0, name);
        foreach (ItemData item in items) {
            Assert.IsNotNull(item, name);
        }
    }

    public void OnEnable() {
        //Assert.IsNotNull(ownerUnit, name);
    }

    public bool IsFreeSpot(ItemData newItem, Vector2Int newPos) {
        bool isInsideInv = newPos.x >= 0
            && newPos.x + newItem.size.x <= size.x
            && newPos.y >= 0
            && newPos.y + newItem.size.y <= size.y;
        if (!isInsideInv) return false;
        foreach (var item in items) {
            if (item == newItem) continue;
            bool insideX = newPos.x < item.position.x + item.size.x && newPos.x + newItem.size.x > item.position.x;
            bool insideY = newPos.y < item.position.y + item.size.y && newPos.y + newItem.size.y > item.position.y;
            if (insideX && insideY) return false;
        }
        return true;
    }

    public Vector2Int GetFreeSpot(ItemData item) {
        // TODO: can stack items!
        for (int x = 0; x <= size.x - item.size.x; x++) {
            for (int y = 0; y <= size.y - item.size.y; y++) {
                if (IsFreeSpot(item, new(x, y))) {
                    return new(x, y);
                }
            }
        }
        return new(-1, -1);
    }

    public InventoryData Clone(bool createAsset = false) {
        var newInvData = CreateInstance<InventoryData>();
        newInvData.ownerUnit = ownerUnit;
        newInvData.size = size;
        newInvData.items = items.Select(x => x.Clone(true)).ToList();
        newInvData.items.ForEach(x => x.inventoryData = newInvData);
        var path = AssetLoader.ConstructPath(this);
        newInvData.name = path.Split("/").Last();
        if (newInvData.name.EndsWith(".asset")) {
            newInvData.name = newInvData.name[..^6];
        }
        if (createAsset) {
            AssetLoader.CreateAsset(this, path);
        }
        return newInvData;
    }
}