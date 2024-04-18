using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Diagnostics;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EmptyInvSlot : MonoBehaviour, IDropHandler {
    public Image image;
    [HideInInspector] public Inventory inventory;
    [HideInInspector] public Vector2Int position;

    void OnValidate() {
        Assert.IsNotNull(image, name);
    }

    IEnumerator WaitAndRecolor(float  delay, Color color) {
        yield return new WaitForSeconds(delay);
        image.color = color;
    }

    public void OnDrop(PointerEventData eventData) {
        Draggable draggable;
        if (eventData.pointerDrag
                && (draggable = eventData.pointerDrag.GetComponent<Draggable>())
            ) {
            draggable.OnCompleteDrag(this);
            var prevColor = image.color;
            image.color = Color.yellow;
            StartCoroutine(WaitAndRecolor(0.3f, prevColor));
        }
    }
}
