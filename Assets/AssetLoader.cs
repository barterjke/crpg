using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;



public class AssetLoader {
    static string RemoveSuffix(string input) {
        if (input.EndsWith(".asset")) {
            input = input[..^6];
        }
        string pattern = @"_(\d+)$";
        if (Regex.IsMatch(input, pattern)) {
            return Regex.Replace(input, pattern, "");
        } else {
            return input;
        }
    }

    static Dictionary<(Type, string), (string, int)> map = new();

    public static string ConstructPath(ScriptableObject obj) {
        var name = RemoveSuffix(obj.name);
        if (map.ContainsKey((obj.GetType(), name))) {
            var (basePath, ind) = map[(obj.GetType(), name)];
            map[(obj.GetType(), name)] = (basePath, ind + 1);
            return $"{basePath}_{ind + 1}.asset";
        }
        var paths = AssetDatabase.FindAssets($"t:{obj.GetType()} {name}", new[] { "Assets/" }).Select(AssetDatabase.GUIDToAssetPath).ToList();
        paths.Sort();
        var lastAssetPath = paths.Last();
        var underScore = lastAssetPath.LastIndexOf('_');
        var dot = lastAssetPath.LastIndexOf('.');
        var numAsStr = lastAssetPath[(underScore + 1)..dot];
        int num;
        try {
            num = int.Parse(numAsStr);
        } catch {
            num = 0;
        }
        var clearedPath = RemoveSuffix(lastAssetPath);
        map[(obj.GetType(), name)] = (clearedPath, num + 1);
        return $"{clearedPath}_{num + 1}.asset";
    }

    public static void CreateAsset(ScriptableObject obj, string path) {
        AssetDatabase.CreateAsset(obj, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
