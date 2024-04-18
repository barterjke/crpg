using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public class NotificationBox : MonoBehaviour, IPointerClickHandler {
    public GameObject mainParent;
    public TMP_Text messageLabel;

    void OnValidate() {
        Assert.IsNotNull(mainParent, name);
    }

    public void Notify(string message) {
        mainParent.SetActive(true);
        messageLabel.text = message;
    }

    public void OnEnable() {
        
    }

    public void OnPointerClick(PointerEventData eventData) {
        mainParent.SetActive(false);
    }

    void Start() {

    }

    void LateUpdate() {
        if (mainParent.activeSelf)
            mainParent.transform.SetAsLastSibling();
    }
}
