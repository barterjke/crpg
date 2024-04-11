using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public class ContextMenu : MonoBehaviour, IPointerClickHandler, IBeginDragHandler {
    public GameObject ctxMenuFather;

    public void OnBeginDrag(PointerEventData eventData) {
        ctxMenuFather.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Right) return;
        ctxMenuFather.transform.position = eventData.position;
        ctxMenuFather.SetActive(true);
    }

    private void OnValidate() {
        Assert.IsNotNull(ctxMenuFather);
    }

    void Start() {
    }

    void LateUpdate() {
        var rect = ctxMenuFather.transform as RectTransform;
        bool mouseIsClicked = Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1);
        if (mouseIsClicked
            && !RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition)) {
            ctxMenuFather.SetActive(false);
        }
    }
}