using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum EffectType
{
    Silence,
    Stun,
    Burning,
    Block,
    DoubleTurn,
    Vampiric,
    Damage,
    Heal
}

public enum Area
{
    Self,
    SelectedUnit,
    RandomEnemy,
    RandomAlly,
    Global,
    SelfAndEnemy
}

[Serializable]
public record Status
{
    public EffectType type;
    public int duration = 1;
    public int value;

    public override string ToString()
    {
        var forDuration = duration > 0 ? " " + duration.ToString() : "ever";
        return $"{type} {value} for{forDuration}";

    }
}


[Serializable]
public record AttackType { 
    public EffectType type;
    public AnimatedSprite animation;
    public int duration = 1;
    public Area area = Area.SelectedUnit;
    public int value;
}