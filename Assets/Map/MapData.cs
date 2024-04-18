using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

[Serializable]
public enum LevelType {
    Undiscovered,
    Battle,
    Shop,
    Dialogue,
    Current,
    Cleared
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MapData", order = 1)]
public class MapData : ScriptableObject {
    public Vector2Int current;
    public const int SIZE = 5;
    public List<LevelType> map;

    void OnValidate() {
        if (map == null || map.Count < 25)
            map = Enumerable.Repeat(LevelType.Undiscovered, SIZE * SIZE).ToList();
    }
}
