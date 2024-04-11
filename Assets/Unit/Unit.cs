using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Unit : MonoBehaviour
{
    public UnitData unitData;

    private void OnValidate() {
        Assert.IsNotNull(unitData);
        unitData.inventoryData.ownerUnit = unitData; // possibly use a property?
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
