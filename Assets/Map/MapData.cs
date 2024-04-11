using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public enum LevelType {
    Undiscovered,
    Battle,
    Shop,
    Dialog,
    Current,
    Cleared
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MapData", order = 1)]
public class MapData : ScriptableObject {
    public Vector2Int current;
    public LevelType[,] map = new LevelType[5, 5];

    void OnEnable() {
        map[0, 0] = LevelType.Current;
        Fill();
        Console.WriteLine($"{map[0, 1]}");
    }

    void Fill() {
        var counter = 0;
        foreach (var (dx, dy) in new (int, int)[] { (-1, 0), (1, 0), (0, -1), (0, 1) }) {
            var (x, y) = (current.x + dx, current.y + dy);
            if (x >= 0 && y >= 0 && x < map.GetLength(0) && y < map.GetLength(1) && map[x, y] == LevelType.Undiscovered) {
                map[x, y] = counter++ == 0 ? LevelType.Battle : (LevelType)(GlobalManager.rand.Next(2) + 1);
            }
        }
    }
}
