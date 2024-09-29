# Overview
NekoStats is a lightweight reactive stat system for Unity. It provides the building blocks for character stats, perks, buffs & debuffs, all that stat-related logic you might see in data-heavy genres such as rogue-likes and RPGs.

## Use Cases

**Flat and Percentage Modifiers**

Increase ATK by 25 and 10%
```cs
Stat atk = new Stat(200f);
Modifier flatMod = new Modifier(35);
Modifier percentMod = new PercentModifier(0.1f, atk.BaseValue);
atk.AddModifier(flatMod); // atk = 200 + 35 = 235
atk.AddModifier(percentMod); // atk = 200 + 35 + (200 * 10%) = 255
```

**Flexible Modifier Value Sourcing**

Increase max HP by a percentage equal to 0.1% of ATK
```cs
Stat atk = new Stat(200f);
Stat maxHp = new Stat(500f);
Modifier percent = new PercentModifier(0.001f, atk.BaseValue);
// percent = 0.1% * 200 = 0.2 = 20%
Modifier mod = new PercentModifier(percent, maxHp.BaseValue);
maxHp.AddModifier(mod); // maxHp = 500 + (500 * 20%) = 600

```
