using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public class Hoverable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public TMP_Text descriptionLabel;
    public Item item;

    void SwitchVisibility(bool value) {
        descriptionLabel.transform.parent.gameObject.SetActive(value);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        SwitchVisibility(true);
    }

    public void OnPointerExit(PointerEventData eventData) {
        SwitchVisibility(false);
    }

    void OnValidate() {
        Assert.IsNotNull(descriptionLabel);
        Assert.IsNotNull(item);
    }

    void Start()
    {
        descriptionLabel.text = item.itemData.description;
        SwitchVisibility(false);
    }
}
