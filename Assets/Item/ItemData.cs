using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeEditor;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ItemData", order = 1)]
public class ItemData : ScriptableObject {
    public InventoryData inventoryData;
    public Sprite sprite;
    public string description;
    [Min(0)] public int cost;
    [Min(0)] public Vector2Int size;
    [Min(0)] public Vector2Int position;
    public bool expendable = false;
    public bool isRotated;

    [SerializeField] protected int _count = 1;
    public int GetCount() {
        return _count;
    }
    public void SetCount(int newCount, Action callback) {
        _count = newCount;
        callback();
    }

    public bool TryToFitSelf(Vector2Int newPos) {
        return inventoryData.IsFreeSpot(this, newPos);
    }

    public override  bool Equals(object other) {
        if (other == null || other.GetType() != GetType()) {
            return false;
        }
        if (ReferenceEquals(this, other)) return true;
        var otherItem = (ItemData)other;
        return sprite == otherItem.sprite && size == otherItem.size && expendable == otherItem.expendable; // TODO: description? name? cost?
    }

    void OnValidate() {
        Assert.IsTrue(size.x > 0 && size.y > 0, name);
        Assert.IsNotNull(sprite, name);
    }

    private void OnEnable() {
        //Assert.IsNotNull(inventoryData, name);
    }

    public virtual ItemData Clone(bool createAsset = false) {
        var newItemData = CreateInstance<ItemData>();
        newItemData._count = _count;
        newItemData.cost = cost;
        newItemData.description = description;
        newItemData.expendable = expendable;
        newItemData.sprite = sprite;
        newItemData.size = size;
        newItemData.position = position;
        newItemData.isRotated = isRotated;
        var path = AssetLoader.ConstructPath(this);
        newItemData.name = path.Split("/").Last();
        if (newItemData.name.EndsWith(".asset")) {
            newItemData.name = newItemData.name[..^6];
        }
        if (createAsset) {
            AssetLoader.CreateAsset(this, path);
        }
        return newItemData;
    }
}
