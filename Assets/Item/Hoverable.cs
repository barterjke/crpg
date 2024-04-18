using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
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
        SwitchVisibility(eventData.pointerDrag == null);
    }

    public void OnPointerExit(PointerEventData eventData) {
        SwitchVisibility(false);
    }

    void OnValidate() {
        Assert.IsNotNull(descriptionLabel, name);
        Assert.IsNotNull(item, name );
    }

    public void UpdateDesc() {
        var itemData = item.itemData;
        var finalDesc = itemData.name[3..] + "\n" + itemData.description;
        foreach (var field in itemData.GetType().GetFields()) {
            if (finalDesc.Contains($"{{{field.Name}}}"))
                finalDesc = finalDesc.Replace($"{{{field.Name}}}", field.GetValue(itemData).ToString());
        }
        var actItem = itemData as ActiveItemData;
        if (!actItem) return;
        if (actItem.attacks.Count == 0) return;
        string pattern = @"{(\d+)\.(type|duration|value|area)}";
        foreach (Match match in Regex.Matches(finalDesc, pattern)) {
            var ind = int.Parse(match.Groups[1].Value);
            string prop = match.Groups[2].Value;
            if (ind >= actItem.attacks.Count) continue;
            var fieldInfo = (FieldInfo)actItem.attacks[ind].GetType().GetMember(prop)[0];
            var filedValue = fieldInfo.GetValue(actItem.attacks[ind]).ToString();
            finalDesc = finalDesc.Replace(match.Value, filedValue);
        }
        descriptionLabel.text = finalDesc;
    }

    void Start()
    {
        UpdateDesc();
        SwitchVisibility(false);
    }
}
