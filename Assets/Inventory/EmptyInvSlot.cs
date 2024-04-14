using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EmptyInvSlot : MonoBehaviour, IDropHandler {
    [HideInInspector] public Inventory inventory;
    [HideInInspector] public Vector2Int position;

    public void OnDrop(PointerEventData eventData) {
        print($"drop on {name}");
        Draggable draggable;
        if (eventData.pointerDrag
                && (draggable = eventData.pointerDrag.GetComponent<Draggable>())
            ) {
            draggable.OnCompleteDrag(this);
        }
    }

    void Start() {

    }

    void Update() {

    }
}
