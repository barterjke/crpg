using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapCell : MonoBehaviour
{
    public Image image;
    public CanvasGroup canvasGroup;
    public TMP_Text text;
    public Vector2Int coord;
    public LevelType level;

    private void OnValidate() {
        Assert.IsNotNull(image);
        Assert.IsNotNull(canvasGroup);
    }

    public void Click() {
        var current = GlobalManager.Instance.mapData.current;
        GlobalManager.Instance.mapData.map[current.x + current.y * MapData.SIZE] = LevelType.Cleared;
        GlobalManager.Instance.mapData.current = coord;
        string sceneName = level switch {
            LevelType.Battle => GlobalManager.Instance.battleScene,
            LevelType.Dialogue => GlobalManager.Instance.dialogueScene,
            LevelType.Shop => GlobalManager.Instance.shopScene,
            _ => ""
        };
        if (sceneName.Length > 0 ) { 
            SceneManager.LoadScene(sceneName);
        }
    }

    void Start()
    {
        name = $"cell{coord} {level}";
        text.text = level.ToString()[..1];
    }
}
