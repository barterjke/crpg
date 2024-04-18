using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/UnitData", order = 1)]
public class UnitData : ScriptableObject {
    public bool isTrader = false;
    public bool controlledByPlayer;
    public InventoryData inventoryData;
    public Sprite sprite;
    public List<Status> activeStatuses = new();
    [Min(1)] public int maxHealth;
    [Min(1)] public int health;

    private void OnValidate() {
        Assert.IsNotNull(inventoryData, name);
        Assert.IsNotNull(sprite, name);
        Assert.IsTrue(maxHealth > 0, name);
    }

    public UnitData Clone(bool createAsset = false) {
        var newUnitData = CreateInstance<UnitData>();
        newUnitData.controlledByPlayer = controlledByPlayer;
        newUnitData.sprite = sprite;
        newUnitData.activeStatuses.AddRange(activeStatuses);
        newUnitData.maxHealth = maxHealth;
        newUnitData.health = maxHealth;
        newUnitData.inventoryData = inventoryData.Clone();
        newUnitData.inventoryData.ownerUnit = this;
        var path = AssetLoader.ConstructPath(this);
        newUnitData.name = path.Split("/").Last();
        if (newUnitData.name.EndsWith(".asset")) {
            newUnitData.name = newUnitData.name[..^6];
        }
        if (createAsset) {
            AssetDatabase.CreateAsset(newUnitData, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        return newUnitData;
    }
}