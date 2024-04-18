using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class Unit : MonoBehaviour {
    public UnitData unitData;
    public Inventory inventory;
    public Image image;
    public TMP_Text healthLabel;
    public TMP_Text statusLabel;
    public Image healthBar;
    public Material grayscaleMaterial;
    public SelectableUnit selectable;

    private BattleManager battleManager;
    private Material defaultMaterial;
    private float fullWidth;

    private void OnValidate() {
        Assert.IsNotNull(unitData, name);
        Assert.IsNotNull(image, name);
        Assert.IsNotNull(statusLabel, name);
        Assert.IsNotNull(healthLabel, name);
        Assert.IsNotNull(healthBar, name);
        Assert.IsNotNull(grayscaleMaterial, name);
        Assert.IsNotNull(selectable, name);
        unitData.inventoryData.ownerUnit = unitData; // possibly use a property?
    }

    void Start() {
        battleManager = FindAnyObjectByType<BattleManager>();
        Assert.IsNotNull(battleManager);
        Assert.IsNotNull(inventory);
        fullWidth = (healthBar.transform as RectTransform).sizeDelta.x;
        image.sprite = unitData.sprite;
        image.SetNativeSize();
        var rect = image.transform as RectTransform;
        rect.sizeDelta = rect.sizeDelta / rect.sizeDelta.y * Screen.width / 5;
        defaultMaterial = image.material;
        UpdateHealthDisplay();
        UpdateStatusLabel();
    }

    public void UpdateHealthDisplay() {
        healthLabel.text = $"{unitData.health} / {unitData.maxHealth}";
        var procent = (float)unitData.health / unitData.maxHealth;
        var sizeDelta = healthBar.rectTransform.sizeDelta;
        sizeDelta.x = fullWidth * procent;
        healthBar.rectTransform.sizeDelta = sizeDelta;
    }

    public void UpdateStatusLabel() {
        statusLabel.text = "";
        foreach (var status in unitData.activeStatuses) {
            statusLabel.text += status.ToString() + "\n";
        }
    }

    public void StatusCountdown() {
        var statusesToDelete = new List<Status>();
        foreach (var status in unitData.activeStatuses) {
            status.duration--;
            if (status.duration == 0) {
                statusesToDelete.Add(status);
            }
        }
        foreach (var status in statusesToDelete) {
            unitData.activeStatuses.Remove(status);
            if (status.type == EffectType.Tired) {
                image.material = defaultMaterial;
            }
        }
        UpdateStatusLabel();
    }

    public void MakeTired() {
        image.material = grayscaleMaterial;
        var nextUnit = battleManager.getUnitsOfFraction(unitData.controlledByPlayer).Where(unit => !unit.isTired()).FirstOrDefault();
        if (nextUnit) {
            nextUnit.selectable.UnselectOther();
            nextUnit.selectable.Select();
        }
    }

    public void ApplyStatus(Attack attack) {
        var alreadyExisted = unitData.activeStatuses.Where(status => status.type == attack.type).FirstOrDefault();
        if (alreadyExisted != null) {
            alreadyExisted.duration += attack.duration;
            if (alreadyExisted.value != attack.value) {
                print("It's not expected!");
            }
        } else {
            unitData.activeStatuses.Add(new Status(attack.type, attack.duration, attack.value));
            if (attack.type == EffectType.Tired) {
                MakeTired();
            }
        }
        UpdateStatusLabel();
    }

    IEnumerator WaitAndRecolor(float delay) {
        yield return new WaitForSeconds(delay);
        image.color = Color.white;
    }

    public bool isTired() {
        return unitData.activeStatuses.Any(s => s.type == EffectType.Tired);
    }

    public float ApplyAttack(Attack attack) {
        switch (attack.type) {
            case EffectType.Damage:
                unitData.health -= attack.value;
                image.color = Color.red;
                StartCoroutine(WaitAndRecolor(attack.animation.duration * 2)); // todo: can override each other
                break;
            case EffectType.Heal:
                unitData.health += attack.value;
                if (unitData.health > unitData.maxHealth) {
                    unitData.health = unitData.maxHealth;
                }
                break;
            case EffectType.Tired:
            case EffectType.Block:
                ApplyStatus(attack);
                break;
            default:
                print($"{attack.type} is not yet implemented");
                break;
        }
        if (attack.animation) {
            var anim = Instantiate(attack.animation.gameObject, transform, false);
            anim.transform.SetAsLastSibling();
            Destroy(anim, attack.animation.duration * 2);
            return attack.animation.duration * 2;
        }
        return 0.3f;
    }
}
