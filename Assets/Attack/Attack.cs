using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public record Attack {
    public EffectType type;
    public AnimatedSprite animation;
    public int duration = 1;
    public Area area = Area.SelectedUnit;
    public int value;
}
