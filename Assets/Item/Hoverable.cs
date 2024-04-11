using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public class Hoverable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public TMP_Text descriptionLabel;
    public ItemData itemData;

    public void OnPointerEnter(PointerEventData eventData) {
        descriptionLabel.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData) {
        descriptionLabel.gameObject.SetActive(false);
    }

    void OnValidate() {
        Assert.IsNotNull(descriptionLabel);
        Assert.IsNotNull(itemData);
    }

    void Start()
    {
        descriptionLabel.text = itemData.description;
        descriptionLabel.gameObject.SetActive(false);
    }

    void Update()
    {
        
    }
}
