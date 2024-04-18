using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CoinData", order = 1)]
public class CoinData : ItemData
{
    public override ItemData Clone(bool createAsset = false) {
        var newItemData = CreateInstance<CoinData>();
        newItemData.description = description;
        newItemData.expendable = expendable;
        newItemData.sprite = sprite;
        newItemData.size = size;
        newItemData.position = position;
        newItemData.isRotated = isRotated;
        newItemData._count = _count;
        newItemData.cost = cost;
        var path = AssetLoader.ConstructPath(this);
        newItemData.name = path.Split("/").Last();
        if (newItemData.name.EndsWith(".asset")) {
            newItemData.name = newItemData.name[..^6];
        }
        if (createAsset) {
            AssetDatabase.CreateAsset(newItemData, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        return newItemData;
    }
}
