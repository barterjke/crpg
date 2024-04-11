using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        Assert.IsNotNull(inventoryData);
        Assert.IsNotNull(sprite);
        Assert.IsTrue(maxHealth > 0);
    }
}