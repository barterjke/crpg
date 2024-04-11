using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public bool playerTurn { get; private set; } = false;

    private void OnValidate() {
        Assert.IsNotNull(turnLabel);
        Assert.IsNotNull(turnButton);
    }

    public void EndBattle() {
        SceneManager.LoadSceneAsync("bookMenu");
    }

    public void SwitchTurn() {
        playerTurn = !playerTurn;
        turnButton.interactable = playerTurn;
        turnLabel.text = playerTurn ? "player turn" : "enemy turn";
        //StatusCountdown();
        if (playerTurn) {
            EndTurn();
        } else {
            StartTurn();
        }
    }

    private void EndTurn() {
        //var ai = FindAnyObjectByType<AbstractAI>();
        //StartCoroutine(ai.Think());
    }

    private void StartTurn() {
        //StatusCountdown();
    }

    void Start() {
        turnButton.onClick.AddListener(SwitchTurn);
        SwitchTurn();
        var counter = 0;
        foreach (var unitData in GlobalManager.Instance.units) {
            var obj = Instantiate(unitPrefab, canvas.transform);
            (obj.transform as RectTransform).anchoredPosition = new(Screen.width / 6.0f * ++counter, 0);
            var rect = obj.transform as RectTransform;
            var unit = obj.GetComponent<Unit>();
            unit.unitData = unitData;

            obj = Instantiate(inventoryPrefab, canvas.transform);
            var inventory = obj.GetComponent<Inventory>();
            inventory.inventoryData = unitData.inventoryData;
            float width = inventory.inventoryData.size.x * (10 + inventory.emptyCellPrefab.GetComponent<RectTransform>().sizeDelta.x);
            //(obj.transform as RectTransform).anchoredPosition = new(Screen.width / 6.0f * counter - width / 2.0f, rect.sizeDelta.y * 1.5f);
            (obj.transform as RectTransform).anchoredPosition = new(Screen.width / 3 - width / 2.0f, rect.sizeDelta.y * 1.5f);
        }
    }

    void Update() {

    }
}
