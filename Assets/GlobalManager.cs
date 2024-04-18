using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class GlobalManager : MonoBehaviour {
    public List<UnitData> playerUnits;
    public List<UnitData> enemyUnits;
    public MapData mapData;

    public string mapScene;
    public string battleScene;
    public string shopScene;
    public string dialogueScene;
    public string defeatScene; //todo

    public static GlobalManager Instance;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }


    void Start() {
        Assert.IsTrue(Application.CanStreamedLevelBeLoaded(mapScene));
        Assert.IsTrue(Application.CanStreamedLevelBeLoaded(battleScene));
        Assert.IsTrue(Application.CanStreamedLevelBeLoaded(dialogueScene));
        Assert.IsTrue(Application.CanStreamedLevelBeLoaded(shopScene));
    }

    public List<UnitData> PopulateEnemies() {
        List<UnitData> result = new() {
            enemyUnits.PickRandom()
        };
        return result;
    }

}
