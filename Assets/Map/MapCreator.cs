using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MapCreator : MonoBehaviour
{
    public MapData mapData;
    public GameObject mapCellPrefab;

    private void OnValidate() {
        Assert.IsNotNull(mapData);
        Assert.IsNotNull(mapCellPrefab);
        Assert.IsNotNull(mapCellPrefab.GetComponent<MapCell>());
    }

    void Start() {
        var rect = mapCellPrefab.GetComponent<RectTransform>();
        for (int x = 0; x < mapData.map.GetLength(0); x++) {
            for (int y = 0; y < mapData.map.GetLength(1); y++) {
                var levelType = mapData.map[x, y];
                if (levelType != LevelType.Undiscovered) {
                    var obj = Instantiate(mapCellPrefab, transform, false);
                    var width = rect.sizeDelta.x + 10;
                    obj.transform.position += new Vector3(x * width, -y * width);
                    var mapCell = obj.GetComponent<MapCell>();
                    mapCell.coord = new(x, y);
                    mapCell.level = levelType;
                }
            }
        }
    }

    

    void Update()
    {
        
    }
}
