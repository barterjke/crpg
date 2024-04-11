using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class Exit : AbstractOption {
    private void OnValidate() {
    }

    public override void Do(GenerateOptions gen) {
        SceneManager.LoadScene(GlobalManager.Instance.mapScene);
    }
} 
