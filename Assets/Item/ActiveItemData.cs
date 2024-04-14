using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ActiveItemData", order = 1)]
public class ActiveItemData : ItemData
{
    public AudioClip audioClip;
    public List<Attack> attacks;
    public Texture2D crosshair;
}