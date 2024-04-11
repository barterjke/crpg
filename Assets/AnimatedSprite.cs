using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AnimatedSprite : MonoBehaviour
{
    public float duration;
    [SerializeField, Min(1)] public int count;
    public List<Sprite> sprites;

    private Image image;
    private int index = 0;
    private float timer = 0;

    public static int NameToNumber(string name)
    {
        var ind = name.LastIndexOf('_');
        if (int.TryParse(name.Substring(ind + 1), out int num))
        {
            return num;
        }
        return 0;
    }

    public void OnValidate()
    {
        if (sprites.Count < 1 || sprites[0] == null)
        {
            throw new ArgumentNullException($"At least one sprite is required! {name}");
        }
        sprites.RemoveRange(1, sprites.Count - 1);
        var path = AssetDatabase.GetAssetPath(sprites[0]);
        var loaded = AssetDatabase.LoadAllAssetsAtPath(path);
        loaded = loaded.OrderBy(o => NameToNumber(o.name)).ToArray();
        for (int i = 0; i <= count && i < loaded.Length; i++)
        {
            var sprite = loaded[i] as Sprite;
            if (sprite && !sprite.name.EndsWith("_0"))
                sprites.Add(loaded[i] as Sprite);
        }
    }

    private void Start()
    {
        image = GetComponent<Image>();
        image.sprite = sprites[0];
    }

    private void Update()
    {
        if ((timer += Time.deltaTime) >= (duration / sprites.Count))
        {
            timer = 0;
            image.sprite = sprites[index];
            index = (index + 1) % sprites.Count;
        }
    }
}
