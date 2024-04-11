using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ItemData", order = 1)]
public class ItemData : ScriptableObject
{
    public InventoryData inventoryData;
    public Sprite sprite;
    [Min(0)] public int cost;
    public string description;
    [Min(0)] public Vector2Int size;
    [Min(0)] public Vector2Int position;
    public bool expendable = false;
    public bool isRotated;

    public void SetCount()
    {

    }
    [SerializeField] protected int _count = 1;

    public bool TryToFitSelf(Vector2Int newPos) {
        return inventoryData.TryToFit(this, newPos);
    }

    private void OnValidate()
    {
        Assert.IsNotNull(inventoryData);
        Assert.IsNotNull(sprite);
    }

    void Awake()
    {
        foreach (var field in GetType().GetFields())
        {
            if (description.Contains($"{{{field.Name}}}"))
                description = description.Replace($"{{{field.Name}}}", field.GetValue(this).ToString());
        }
    }
}
