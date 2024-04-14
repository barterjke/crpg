using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Draggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {
    public Item item;
    public CanvasGroup canvasGroup;

    public bool isPlaced;

    private Vector2 prevPosition;
    private bool wasRotated;
    private Vector2 offset;

    void OnValidate() {
        Assert.IsNotNull(item);
        Assert.IsNotNull(canvasGroup);
    }

    public ItemData GetItemData() {
        return item.itemData;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (!GetItemData().inventoryData.ControlledByPlayer) {
            eventData.pointerDrag = null;
            return;
        }
        item.inventory.transform.SetAsLastSibling();
        item.transform.SetAsLastSibling();
        wasRotated = GetItemData().isRotated;
        prevPosition = transform.parent.position;
        offset = transform.parent.position - Input.mousePosition;
        transform.parent.SetAsLastSibling();
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = .5f;
    }

    public void OnDrag(PointerEventData eventData) {
        transform.parent.position = Input.mousePosition + (Vector3)offset;//eventData.position + offset;
    }

    public void OnCompleteDrag(EmptyInvSlot slot) {
        if (!slot.inventory.inventoryData.ControlledByPlayer) return;
        if (item.MoveTo(slot.inventory, slot.position)) {
            isPlaced = true;
            transform.parent.position = slot.transform.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
        if (isPlaced) {
            isPlaced = false;
            return;
        }
        GetItemData().isRotated = wasRotated;
        transform.parent.position = prevPosition;
    }

    void Update() {
        bool isDragged = !canvasGroup.blocksRaycasts;
        if (isDragged && Input.GetKeyDown(KeyCode.R)) {
            GetItemData().isRotated = !GetItemData().isRotated;
            GetItemData().size = new(GetItemData().size.y, GetItemData().size.x);
            transform.Rotate(new(0, 0, 90));
        }
    }
}
