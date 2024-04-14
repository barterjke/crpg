using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
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

    [HideInInspector] public Item currentItem;
    [HideInInspector] public Unit currentTarget;
    [HideInInspector] public List<Unit> playerUnits = new();
    [HideInInspector] public List<Unit> enemyUnits;
    private List<AbstractAI> AIs;


    public bool playerTurn { get; private set; } = false;

    private void OnValidate() {
        Assert.IsNotNull(turnLabel);
        Assert.IsNotNull(turnButton);
        Assert.IsNotNull(audioSource);
    }

    public void EndBattle() {
        SceneManager.LoadSceneAsync(GlobalManager.Instance.mapScene);
    }

    public void SwitchTurn() {
        playerTurn = !playerTurn;
        turnButton.interactable = playerTurn;
        turnLabel.text = playerTurn ? "player turn" : "enemy turn";
        //StatusCountdown();
        if (playerTurn) {
            StartTurn();
        } else {
            EndTurn();
        }
    }

    private void EndTurn() {
        StartCoroutine(AIs.PickRandom().Think());
    }

    private void StartTurn() {
        //StatusCountdown();
    }

    void Start() {
        turnButton.onClick.AddListener(SwitchTurn);
        var counter = 0;
        enemyUnits = FindObjectsOfType<Unit>().Where(unit => !unit.unitData.controlledByPlayer).ToList();
        AIs = enemyUnits.Select(unit => unit.GetComponent<AbstractAI>()).ToList();
        SwitchTurn();
        foreach (var unitData in GlobalManager.Instance.units) {
            var obj = Instantiate(unitPrefab, canvas.transform);
            (obj.transform as RectTransform).anchoredPosition = new(Screen.width / 6.0f * ++counter, 0);
            var rect = obj.transform as RectTransform;
            var unit = obj.GetComponent<Unit>();
            unit.unitData = unitData;
            playerUnits.Add(unit);

            obj = Instantiate(inventoryPrefab, canvas.transform);
            var inventory = obj.GetComponent<Inventory>();
            inventory.owner = unit;
            unit.inventory = inventory;
            inventory.inventoryData = unitData.inventoryData;
            float width = inventory.inventoryData.size.x * (10 + inventory.emptyCellPrefab.GetComponent<RectTransform>().sizeDelta.x);
            //(obj.transform as RectTransform).anchoredPosition = new(Screen.width / 6.0f * counter - width / 2.0f, rect.sizeDelta.y * 1.5f);
            (obj.transform as RectTransform).anchoredPosition = new(Screen.width / 3 - width / 2.0f, rect.sizeDelta.y * 1.5f);
        }
    }

    public void Reset() {
        currentItem = null;
        currentTarget = null;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void ChooseTarget(Item item) {
        currentItem = item;
        Cursor.SetCursor((item.itemData as ActiveItemData).crosshair, Vector2.zero, CursorMode.Auto);
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
                    var unitsToPull = playerTurn ? enemyUnits : playerUnits;
                    if (unitsToPull.Count > 0) {
                        units.Add(unitsToPull.PickRandom());
                    } else {
                        print("no enemies to select from??");
                    }
                    break;
                }
            case Area.RandomAlly: {
                    var unitsToPull = !playerTurn ? enemyUnits : playerUnits;
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
