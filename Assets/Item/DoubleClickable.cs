using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public class DoubleClickable : MonoBehaviour, IPointerClickHandler {
    public Item item;

    private TradingManager tradingManager;

    void OnValidate() {
        Assert.IsNotNull(item, name);
    }


    void Start() {
        tradingManager = FindAnyObjectByType<TradingManager>();
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.clickCount == 2) {
            if (tradingManager) {
                tradingManager.Trade(item);
            }
            else {
                item.Act();
            }
        }
    }


    void Update()
    {
        
    }
}
