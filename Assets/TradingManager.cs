using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TradingManager : MonoBehaviour {
    [Header("prefabs")]
    public GameObject inventoryPrefab;
    public CoinData coinData;

    [Header("refs")]
    public Canvas canvas;
    public Button continueButton;

    [Header("data")]
    public InventoryData traderInventoryData;

    private List<Inventory> playerInventories = new();
    private Inventory traderInventory;

    private void OnValidate() {
        Assert.IsNotNull(inventoryPrefab);
        Assert.IsNotNull(coinData);
        Assert.IsNotNull(canvas);
        Assert.IsNotNull(continueButton);
        Assert.IsNotNull(traderInventoryData);
    }

    void Start() {
        foreach (var unitData in GlobalManager.Instance.units) {
            var obj = Instantiate(inventoryPrefab, canvas.transform);
            var inventory = obj.GetComponent<Inventory>();
            playerInventories.Add(inventory);
            inventory.inventoryData = unitData.inventoryData;
            float width = inventory.inventoryData.size.x * (10 + inventory.emptyCellPrefab.GetComponent<RectTransform>().sizeDelta.x);
            (obj.transform as RectTransform).anchoredPosition = new(Screen.width / 3.0f - width / 2.0f, Screen.height / 4);
        }
        {
            var obj = Instantiate(inventoryPrefab, canvas.transform);
            traderInventory = obj.GetComponent<Inventory>();
            traderInventory.inventoryData = traderInventoryData;
            float width = traderInventory.inventoryData.size.x * (10 + traderInventory.emptyCellPrefab.GetComponent<RectTransform>().sizeDelta.x);
            (obj.transform as RectTransform).anchoredPosition = new(Screen.width * 2 / 3.0f - width / 2.0f, Screen.height / 4);
        }
        continueButton.onClick.AddListener(() => {
            SceneManager.LoadScene(GlobalManager.Instance.mapScene);
        });

        AssetDatabase.CreateAsset(coinData, $"Assets/{coinData.name}(clone).asset");
        print(AssetDatabase.GetAssetPath(coinData));
    }

    public List<Inventory> GetInventories(bool isOwnedByPlayer) {
        return isOwnedByPlayer ? playerInventories : new List<Inventory> { traderInventory };
    }

    public List<Item> GetCoins(bool isOwnedByPlayer, bool createIfMissing = false) {
        var coins = new List<Item>();
        var inventories = GetInventories(isOwnedByPlayer);
        foreach (var inventory in inventories) {
            coins.AddRange(inventory.items.Where(i => i.itemData as CoinData));
        }
        if (coins.Count == 0 && createIfMissing) {
            //var obj = Instantiate(traderInventory.itemPrefab, playerInventories.First().transform, true);
            //var item = obj.GetComponent<Item>();
            var path = $"Assets/items/{coinData.name}{GlobalManager.Instance.assetIndex++}.asset";
            AssetDatabase.CreateAsset(coinData, path);
            var newCoinData = (CoinData)AssetDatabase.LoadAssetAtPath(path, typeof(CoinData));
            foreach (var inventory in inventories) {
                var newPos = inventory.inventoryData.GetFreeSpot(newCoinData);
                if (newPos != new Vector2(-1, -1)) {
                    newCoinData.position = newPos;
                    var coinItem = inventory.AddItem(newCoinData);
                    coins.Add(coinItem);
                }
            }
        }
        return coins;
    }

    public static bool hasEnoughGold(List<Item> coins, int cost) {
        return coins.Sum(coin => coin.itemData.GetCount()) >= cost;
    }

    public static void substractCoins(List<Item> coins, int cost) {
        foreach (var coin in coins) {
            var data = coin.itemData;
            if (cost > data.GetCount()) {
                cost -= data.GetCount();
                data.SetCount(0, coin.UpdateCountLabel);
            } else {
                coin.SetCount(data.GetCount() - cost);
                return;
            }
        }
    }

    public bool PerformCoinOperations(Item item, Inventory targetInventory) {
        if (targetInventory.IsTrader) {
            // sell
            var playerCoins = GetCoins(true, true);
            var traderCoins = GetCoins(false);
            if (playerCoins.Count > 0 && hasEnoughGold(traderCoins, item.itemData.cost)) {
                substractCoins(traderCoins, item.itemData.cost);
                var coin = playerCoins.First();
                coin.SetCount(coin.itemData.GetCount() + item.itemData.cost);
                //item.itemData.cost *= 2;
                return true;
            }
            return false;
        } else {
            var playerCoins = GetCoins(true);
            var traderCoins = GetCoins(false, true);
            if (traderCoins.Count > 0 && hasEnoughGold(playerCoins, item.itemData.cost)) {
                substractCoins(playerCoins, item.itemData.cost);
                var coin = traderCoins.First();
                coin.SetCount(coin.itemData.GetCount() + item.itemData.cost);
                //item.itemData.cost *= 2;
                return true;
            }
            return false;
        }
    }

    public bool Trade(Item item) {
        var targetInventories = GetInventories(item.inventory.IsTrader);
        foreach (var inventory in targetInventories) {
            var newPos = inventory.inventoryData.GetFreeSpot(item.itemData);
            if (newPos != new Vector2(-1, -1)) {
                PerformCoinOperations(item, inventory);
                item.TransferToInventory(inventory);
                item.itemData.position = newPos;
                inventory.PlaceIntoPosition(item, newPos);
                return true;
            }
        }
        return false;
    }
}
