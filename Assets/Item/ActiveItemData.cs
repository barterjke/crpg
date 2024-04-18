using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ActiveItemData", order = 1)]
public class ActiveItemData : ItemData {
    public AudioClip audioClip;
    public List<Attack> attacks;
    public Texture2D crosshair;

    public override bool Equals(object other) {
        if (!base.Equals(other)) return false;
        if (ReferenceEquals(this, other)) return true;
        var otherItem = (ActiveItemData)other;
        return audioClip == otherItem.audioClip && crosshair == otherItem.audioClip && attacks.SequenceEqual(otherItem.attacks);
    }

    public override ItemData Clone(bool createAsset = false) {
        var newItemData = CreateInstance<ActiveItemData>();
        newItemData._count = _count;
        newItemData.cost = cost;
        newItemData.position = position;
        newItemData.description = description;
        newItemData.expendable = expendable;
        newItemData.sprite = sprite;
        newItemData.size = size;
        newItemData.audioClip = audioClip;
        newItemData.attacks = attacks.ToList();
        newItemData.crosshair = crosshair;
        newItemData.inventoryData = inventoryData; // todo: check if this is needed?
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