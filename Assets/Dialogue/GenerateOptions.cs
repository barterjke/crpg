using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class GenerateOptions : MonoBehaviour
{
    public List<GameObject> optionPrefabs;
    public Transform optionsFather;

    void Start()
    {
        GenerateOptionButtons();
    }

    public void GenerateOptionButtons() {
        var counter = 0;
        foreach (var optionPrefab in optionPrefabs) {
            print(optionPrefab);
            var obj = Instantiate(optionPrefab, optionsFather, false);
            obj.transform.position += Vector3.down * 40 * counter++;
            obj.GetComponentInChildren<TMP_Text>().text = optionPrefab.name;
            var option = obj.GetComponent<AbstractOption>();
            obj.GetComponent<Button>().onClick.AddListener(()=> option.Do(this));
        }
    }

    public void RefreshOptions() {
        foreach (Transform child in optionsFather) {
            child.gameObject.SetActive(false);
        }
        GenerateOptionButtons();
    }

    void Update()
    {
        
    }
}
