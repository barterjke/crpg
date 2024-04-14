using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public record Status {
    public EffectType type;
    public int duration = 1;
    public int value;

    public Status(EffectType type, int duration, int value) {
        this.type = type;
        this.duration = duration;
        this.value = value;
    }

    public override string ToString() {
        var forDuration = duration > 0 ? " " + duration.ToString() : "ever";
        return $"{type} {value} for{forDuration}";

    }
}