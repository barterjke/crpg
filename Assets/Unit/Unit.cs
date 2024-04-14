using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class Unit : MonoBehaviour {
    public UnitData unitData;
    public Inventory inventory;
    public Image image;
    public TMP_Text healthLabel;
    public Image healthBar;

    private float fullWidth;

    private void OnValidate() {
        Assert.IsNotNull(unitData);
        Assert.IsNotNull(image);
        Assert.IsNotNull(healthLabel);
        Assert.IsNotNull(healthBar);
        unitData.inventoryData.ownerUnit = unitData; // possibly use a property?
    }

    public void UpdateHealthDisplay() {
        healthLabel.text = $"{unitData.health} / {unitData.maxHealth}";
        var procent = (float)unitData.health / unitData.maxHealth;
        var sizeDelta = healthBar.rectTransform.sizeDelta;
        sizeDelta.x = fullWidth * procent;
        healthBar.rectTransform.sizeDelta = sizeDelta;
    }

    IEnumerator WaitAndRecolor(float delay) {
        yield return new WaitForSeconds(delay);
        image.color = Color.white;
    }

    public float ApplyAttack(Attack attack) {
        switch (attack.type) {
            case EffectType.Damage:
                unitData.health -= attack.value;
                break;
            case EffectType.Heal:
                unitData.health += attack.value;
                if (unitData.health > unitData.maxHealth) {
                    unitData.health = unitData.maxHealth;
                }
                break;
            case EffectType.Burning:
            case EffectType.Silence:
            case EffectType.Stun:
            case EffectType.Block:
                unitData.activeStatuses.Add(new Status(attack.type, attack.duration, attack.value));
                break;
            default:
                print($"{attack.type} is not yet implemented");
                break;
        }
        var anim = Instantiate(attack.animation.gameObject, transform, false);
        anim.transform.SetAsLastSibling();
        Destroy(anim, attack.animation.duration * 2);
        image.color = Color.red;
        StartCoroutine(WaitAndRecolor(attack.animation.duration * 2)); // todo: can override each other
        return attack.animation.duration * 2;
    }

    void Start() {
        Assert.IsNotNull(inventory);
        fullWidth = (healthBar.transform as RectTransform).sizeDelta.x;
        image.sprite = unitData.sprite;
        image.SetNativeSize();
        var rect = image.transform as RectTransform;
        rect.sizeDelta = rect.sizeDelta / rect.sizeDelta.y * 512;
        UpdateHealthDisplay();
    }

    void Update() {

    }
}
