using System;
using UnityEngine;
using UnityEngine.Assertions;

public class MapCreator : MonoBehaviour {
    public GameObject mapCellPrefab;
    public Sprite shopSprite;
    public Sprite dialogueSprite;
    public Sprite battleSprite;
    public Sprite clearedSprite;
    public Sprite currentSprite;

    private void OnValidate() {
        Assert.IsNotNull(mapCellPrefab);
        Assert.IsNotNull(mapCellPrefab.GetComponent<MapCell>());
    }

    void CreateMapCell(LevelType levelType, int x, int y) {
        var rect = mapCellPrefab.GetComponent<RectTransform>();
        var obj = Instantiate(mapCellPrefab, transform, false);
        var width = rect.sizeDelta.x + 10;
        obj.transform.position += new Vector3(x * width, -y * width);
        var mapCell = obj.GetComponent<MapCell>();
        mapCell.coord = new(x, y);
        mapCell.level = levelType;

        bool isInteractable = levelType == LevelType.Shop || levelType == LevelType.Battle || levelType == LevelType.Dialogue;
        var distance = mapCell.coord - GlobalManager.Instance.mapData.current;
        isInteractable &= (Math.Abs(distance.x) == 1 && distance.y == 0) || (Math.Abs(distance.y) == 1 && distance.x == 0);
        mapCell.canvasGroup.interactable = isInteractable;
        mapCell.image.sprite = levelType switch {
            LevelType.Shop => shopSprite,
            LevelType.Dialogue => dialogueSprite,
            LevelType.Battle => battleSprite,
            LevelType.Cleared => clearedSprite,
            LevelType.Current => currentSprite,
            _ => null
        };
    }

    void Start() {
        var mapData = GlobalManager.Instance.mapData;
        mapData.map[mapData.current.x + mapData.current.y * MapData.SIZE] = LevelType.Current;
        Fill();
        for (int i = 0; i < mapData.map.Count; i++) {
            var x = i % MapData.SIZE;
            var y = i / MapData.SIZE;
            var levelType = mapData.map[i];
            if (levelType != LevelType.Undiscovered) {
                CreateMapCell(levelType, x, y);
            }
        }
    }

    void Fill() {
        var counter = 0;
        var mapData = GlobalManager.Instance.mapData;
        var current = mapData.current;
        var map = mapData.map;
        foreach (var (dx, dy) in new (int, int)[] { (-1, 0), (1, 0), (0, -1), (0, 1) }) {
            var (x, y) = (current.x + dx, current.y + dy);
            if (x >= 0 && y >= 0 && x < MapData.SIZE && y < MapData.SIZE && map[x + MapData.SIZE * y] == LevelType.Undiscovered) {
                map[x + MapData.SIZE * y] = counter++ == 0 ? LevelType.Battle : (LevelType)(RandUtil.rand.Next(3) + 1);
            }
        }
    }
}
