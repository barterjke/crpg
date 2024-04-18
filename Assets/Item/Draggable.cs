using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Draggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {
    public Item item;
    public CanvasGroup canvasGroup;

    [HideInInspector] public bool isPlaced;

    private Vector2 prevPosition;
    private bool wasRotated;
    private Vector2 offset;
    private TradingManager tradingManager;

    void Start() {
        tradingManager = FindAnyObjectByType<TradingManager>();
        Assert.IsNotNull(tradingManager);
    }

    void OnValidate() {
        Assert.IsNotNull(item, name);
        Assert.IsNotNull(canvasGroup, name);
    }

    public ItemData itemData {
        get {
            return item.itemData;
        }
    }

    void Update() {
        bool isDragged = canvasGroup.alpha < 1;
        if (isDragged && Input.GetKeyDown(KeyCode.R)) {
            Rotate();
        }
    }

    public void SwitchAllItemsCanvasGroup(bool val) {
        foreach (var item in FindObjectsByType<Item>(FindObjectsSortMode.None)) {
            item.canvasGroup.interactable = val;
            item.canvasGroup.blocksRaycasts = val;
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (!itemData.inventoryData.ControlledByPlayer || (item.inventory.owner && item.inventory.owner.isTired())) {
            eventData.pointerDrag = null;
            return;
        }
        SwitchAllItemsCanvasGroup(false);
        item.inventory.transform.SetAsLastSibling();
        item.transform.SetAsLastSibling();
        wasRotated = itemData.isRotated;
        prevPosition = transform.parent.position;
        offset = transform.parent.position - Input.mousePosition;
        transform.parent.SetAsLastSibling();
        canvasGroup.alpha = .5f;
    }

    public void OnDrag(PointerEventData eventData) {
        transform.parent.position = Input.mousePosition + (Vector3)offset;//eventData.position + offset;
    }

    public static Vector2Int Apply(Vector2 vec, Func<float, int> f) {
        return new(f(vec.x), f(vec.y));
    }

    public void OnCompleteDrag(EmptyInvSlot slot) {
        if (!slot.inventory.inventoryData.ControlledByPlayer) return;
        var rect = slot.transform as RectTransform;
        Assert.IsTrue(rect.sizeDelta.x == rect.sizeDelta.y);
        var delta = Apply(offset, (x) => Mathf.FloorToInt(Mathf.Abs(x) / rect.sizeDelta.x));
        var newPos = slot.position - delta;
        if (item.inventory.IsTrader != slot.inventory.IsTrader) {
            isPlaced = tradingManager.Trade(slot.inventory, item, newPos);
            return;
        }
        if (item.MoveTo(slot.inventory, newPos)) {
            isPlaced = true;
            item.inventory.PlaceIntoPosition(item, slot.position - delta);
        }
    }

    public void OnCancelDrag() {
        itemData.isRotated = wasRotated;
        transform.parent.position = prevPosition;
    }

    public void OnEndDrag(PointerEventData eventData) {
        SwitchAllItemsCanvasGroup(true);
        canvasGroup.alpha = 1f;
        if (isPlaced) {
            isPlaced = false;
        } else {
            OnCancelDrag();
        }
    }

    public void Rotate() {
        itemData.isRotated = !itemData.isRotated;
        itemData.size = new(itemData.size.y, itemData.size.x);
        var angle = transform.rotation.eulerAngles.z;
        angle = angle == 0 ? -90 : 0;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        var rect = transform as RectTransform;
        transform.position += (angle == -90 ? Vector3.up : Vector3.down) * rect.sizeDelta.x;
    }

    
}
