using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TradingManager : MonoBehaviour {
    [Header("prefabs")]
    public GameObject inventoryPrefab;
    public CoinData coinDataPrefab;

    [Header("refs")]
    public Canvas canvas;
    public Button continueButton;

    [Header("data")]
    public InventoryData traderInventoryData;

    private List<Inventory> playerInventories = new();
    private Inventory traderInventory;
    private List<ItemData> generatedItems;

    private void OnValidate() {
        Assert.IsNotNull(inventoryPrefab);
        Assert.IsNotNull(canvas);
        Assert.IsNotNull(continueButton);
        Assert.IsNotNull(coinDataPrefab);
        Assert.IsNotNull(traderInventoryData);
    }

    void Start() {
        CloneInventory();
        foreach (var unitData in GlobalManager.Instance.playerUnits) {
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
            OnLeave();
            SceneManager.LoadScene(GlobalManager.Instance.mapScene);
        });
    }

    void OnLeave() {
        foreach (var inv in playerInventories) {
            foreach (var item in inv.items) {
                var itemData = item.itemData;
                if (generatedItems.Contains(itemData)) {
                    var path = AssetLoader.ConstructPath(itemData);
                    AssetLoader.CreateAsset(itemData, path);
                }
            }
        }
    }

    private void OnDisable() {
        OnLeave();
    }

    void CloneInventory() {
        var counter = 0;
        traderInventoryData = traderInventoryData.Clone();
        traderInventoryData.items = traderInventoryData.items.OrderBy(i => i is CoinData ? 0 : 1 + RandUtil.rand.Next(100)).Take(3).ToList();
        foreach (var item in traderInventoryData.items) {
            item.position = new(counter, 0);
            counter += item.size.x;
        }
        generatedItems = traderInventoryData.items.ToList();
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
            var newCoinData = coinDataPrefab.Clone();
            //var newCoinData = (CoinData)AssetDatabase.LoadAssetAtPath(path, typeof(CoinData));
            foreach (var inventory in inventories) {
                var newPos = inventory.inventoryData.GetFreeSpot(newCoinData);
                if (newPos != new Vector2(-1, -1)) {
                    newCoinData.position = newPos;
                    var coinItem = inventory.AddItem(newCoinData);
                    inventory.inventoryData.items.Add(newCoinData);
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

    public bool Trade(Inventory inventory, Item item, Vector2Int newPos) {
        if (!PerformCoinOperations(item, inventory)) {
            return false;
        }
        item.TransferToInventory(inventory);
        item.itemData.position = newPos;
        inventory.PlaceIntoPosition(item, newPos);
        return true;
    }

    public bool Trade(Item item) {
        var targetInventories = GetInventories(item.inventory.IsTrader);
        foreach (var inventory in targetInventories) {
            var newPos = inventory.inventoryData.GetFreeSpot(item.itemData);
            if (newPos != new Vector2(-1, -1)) {
                return Trade(inventory, item, newPos);
            }
        }
        return false;
    }
}
