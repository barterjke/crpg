using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TradingManager : MonoBehaviour {
    [Header("prefabs")]
    public GameObject inventoryPrefab;

    [Header("refs")]
    public Canvas canvas;
    public Button continueButton;

    [Header("data")]
    public InventoryData traderInventoryData;

    private void OnValidate() {
        Assert.IsNotNull(traderInventoryData);
    }

    public void Awake() {
        //traderInventoryData.items = traderInventoryData.items.Select(item => {
        //    var copyItem = Instantiate(item);
        //    copyItem.cost = 69;
        //    return copyItem;
        //}).ToList();
    }

    void Start() {
        foreach (var unitData in GlobalManager.Instance.units) {
            var obj = Instantiate(inventoryPrefab, canvas.transform);
            var inventory = obj.GetComponent<Inventory>();
            inventory.inventoryData = unitData.inventoryData;
            float width = inventory.inventoryData.size.x * (10 + inventory.emptyCellPrefab.GetComponent<RectTransform>().sizeDelta.x);
            (obj.transform as RectTransform).anchoredPosition = new(Screen.width / 3.0f - width / 2.0f, Screen.height / 4);
        }
        {
            var obj = Instantiate(inventoryPrefab, canvas.transform);
            var inventory = obj.GetComponent<Inventory>();
            inventory.inventoryData = traderInventoryData;
            float width = inventory.inventoryData.size.x * (10 + inventory.emptyCellPrefab.GetComponent<RectTransform>().sizeDelta.x);
            (obj.transform as RectTransform).anchoredPosition = new(Screen.width * 2 / 3.0f - width / 2.0f, Screen.height / 4);
        }
        continueButton.onClick.AddListener(() => {
            SceneManager.LoadScene(GlobalManager.Instance.mapScene);
        });
    }

    void Update() {

    }
}
