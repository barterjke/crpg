using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class AddRandomItem : AbstractOption {
    public List<ItemData> items;
    public List<GameObject> optionsPrefabs; // doesn't work for self reference!

    private void OnValidate() {
        Assert.IsTrue(items.Count > 0);
        Assert.IsTrue(optionsPrefabs.Count > 0);
    }

    public override void Do(GenerateOptions gen) {
        var item = items.PickRandom();
        print(item);
        gen.optionPrefabs = optionsPrefabs;
        gen.RefreshOptions();
    }
} 
