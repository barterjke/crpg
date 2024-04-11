using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class MapCell : MonoBehaviour
{
    public TMP_Text text;
    public Vector2Int coord;
    public LevelType level;

    private void OnValidate() {
        Assert.IsNotNull(text);
    }

    public void Click() {
        string sceneName = level switch {
            LevelType.Battle => GlobalManager.Instance.battleScene,
            LevelType.Dialog => GlobalManager.Instance.dialogueScene,
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

    void Update()
    {
        
    }
}
