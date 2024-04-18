using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour {
    [Header("prefabs")]
    public GameObject inventoryPrefab;
    public GameObject unitPrefab;

    [Header("refs")]
    public TMP_Text turnLabel;
    public Button turnButton;
    public Canvas canvas;
    public AudioSource audioSource;
    public NotificationBox notification;

    public List<UnitData> enemyUnitsData;

    [HideInInspector] public Item currentItem;
    [HideInInspector] public Unit currentTarget;

    [HideInInspector] public List<Unit> playerUnits = new();
    [HideInInspector] public List<Unit> enemyUnits = new();
    [HideInInspector] public List<AbstractAI> AIs = new();

    public List<Unit> AllUnits {
        get {
            var allUnits = playerUnits;
            allUnits.AddRange(enemyUnits);
            allUnits = allUnits.Where(unit => unit != null).ToList();
            return allUnits;
        }
    }

    //todo: make playerUnits, enemyUnits, allunits a property with null check

    [HideInInspector] public bool toSkipMouseCheck = false;

    public bool playerTurn { get; private set; } = false;

    private void OnValidate() {
        Assert.IsNotNull(inventoryPrefab, name);
        Assert.IsNotNull(unitPrefab, name);
        Assert.IsNotNull(turnLabel, name);
        Assert.IsNotNull(turnButton, name);
        Assert.IsNotNull(audioSource, name);
        Assert.IsNotNull(notification, name);
        Assert.IsNotNull(canvas, name);
    }

    void Start() {
        foreach (var unitData in GlobalManager.Instance.PopulateEnemies()) {
            enemyUnitsData.Add(unitData.Clone());
        }
        Assert.IsTrue(enemyUnitsData.Count > 0);

        turnButton.onClick.AddListener(SwitchTurn);
        int counter = 1;
        foreach (var unitData in GlobalManager.Instance.playerUnits) {
            var unit = Spawn(unitData, ref counter);
            playerUnits.Add(unit);
        }
        playerUnits.First().selectable.Select();
        counter = 4;
        foreach (var unitData in enemyUnitsData) {
            var unit = Spawn(unitData, ref counter);
            enemyUnits.Add(unit);
            var ai = enemyUnits.Last().AddComponent<RandomAI>();
            ai.unit = unit;
            ai.battleManager = this;
            AIs.Add(ai);
        }
        SwitchTurn();
    }

    private void LateUpdate() {
        if (toSkipMouseCheck) {
            toSkipMouseCheck = false;
            return;
        }
        bool isMouseClicked = Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1);
        if (currentItem && isMouseClicked) {
            Reset();
        }
    }

    public void EndBattle() {
        SceneManager.LoadSceneAsync(GlobalManager.Instance.mapScene);
    }

    public void SwitchTurn() {
        playerTurn = !playerTurn;
        turnButton.interactable = playerTurn;
        turnLabel.text = playerTurn ? "player turn" : "enemy turn";
        if (playerTurn) {
            StartTurn();
        } else {
            EndTurn();
        }
    }

    private void StatusCountdown() {
        foreach (var unit in AllUnits) {
            unit.StatusCountdown();
        }
    }

    private void EndTurn() {
        SceneManager.LoadScene(GlobalManager.Instance.mapScene);
        //StartCoroutine(AIs.PickRandom().Think());
    }

    private void StartTurn() {
        StatusCountdown();
    }

    public Unit Spawn(UnitData unitData, ref int counter) {
        var obj = Instantiate(unitPrefab, canvas.transform);
        obj.name = unitData.name[3..];
        (obj.transform as RectTransform).anchoredPosition = new(Screen.width / 6.0f * counter, 0);
        var rect = obj.transform as RectTransform;
        var unit = obj.GetComponent<Unit>();
        unit.unitData = unitData;
        playerUnits.Add(unit);

        obj = Instantiate(inventoryPrefab, canvas.transform);
        obj.name = unitData.inventoryData.name[3..];
        var inventory = obj.GetComponent<Inventory>();
        inventory.owner = unit;
        unit.inventory = inventory;
        inventory.inventoryData = unitData.inventoryData;
        var ind = Mathf.Floor(counter / 4) + 1;
        (obj.transform as RectTransform).anchoredPosition = new(ind * Screen.width / 3, rect.sizeDelta.y + 200);
        obj.SetActive(counter == 1 || counter == 4);
        counter++;
        return unit;
    }

    public void Reset() {
        currentItem = null;
        currentTarget = null;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void ChooseTarget(Item item) {
        currentItem = item;
        Cursor.SetCursor((item.itemData as ActiveItemData).crosshair, Vector2.zero, CursorMode.Auto);
        toSkipMouseCheck = true;
    }

    public List<Unit> getUnitsOfFraction(bool controlledByPlayer) {
        return controlledByPlayer ? playerUnits.Where(unit => unit != null).ToList() : enemyUnits.Where(unit => unit != null).ToList();
    }

    public List<Unit> GetAffectedUnits(Unit caster, Area area) {
        var selectedUnit = currentItem ? currentTarget : null;
        var units = new List<Unit>();
        switch (area) {
            case Area.Self:
                units.Add(caster);
                break;
            case Area.SelectedUnit:
                units.Add(selectedUnit);
                break;
            case Area.RandomEnemy: {
                    var unitsToPull = getUnitsOfFraction(!playerTurn);
                    if (unitsToPull.Count > 0) {
                        units.Add(unitsToPull.PickRandom());
                    } else {
                        print("no enemies to select from??");
                    }
                    break;
                }
            case Area.RandomAlly: {
                    var unitsToPull = getUnitsOfFraction(!playerTurn);
                    if (unitsToPull.Count > 0) {
                        units.Add(unitsToPull.PickRandom());
                    } else {
                        print("no allies to select from??");
                    }
                    break;
                }
            case Area.SelfAndEnemy:
                units.Add(selectedUnit);
                units.Add(caster);
                break;
            case Area.Global:
                units.AddRange(playerUnits);
                units.AddRange(enemyUnits);
                break;
        }
        return units;
    }
}
