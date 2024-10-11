using CC.Stats;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum AvatarStatType
{
    MaxHP,
    Attack,
    Defense,
    Speed,
    CritRate,
    CritDamage,
}

public class AvatarConfig
{
    public float MaxHP = 2000f;
    public float Attack = 1000f;
    public float Defense = 500f;
    public float Speed = 100f;
    public float CritRate = 0.1f;
    public float CritDamage = 0.5f;
}

public class EquipmentProp
{
    public AvatarStatType StatType;
    public float Value;
    public bool IsPercent;

    public EquipmentProp(AvatarStatType statType, float value, bool isPercent = false)
    {
        StatType = statType;
        Value = value;
        IsPercent = isPercent;
    }

    public Modifier CreateModifier(Stat stat)
    {
        if (IsPercent) return new PercentModifier(Value, stat.Base);
        return new Modifier(Value);
    }
}

public class Weapon
{
    public EquipmentProp MainProp;
    public EquipmentProp SubProp;

    public Weapon(EquipmentProp mainProp, EquipmentProp subProp)
    {
        MainProp = mainProp;
        SubProp = subProp;
    }

    public void Apply(AvatarStats stats)
    {
        stats.AddEquipmentModifier(MainProp);
        stats.AddEquipmentModifier(SubProp);
    }
}


public enum GearType
{
    Head,
    Hand,
    Body,
    Feet,
}

public class Gear
{
    public GearType Type;
    public EquipmentProp MainProp;
    public List<EquipmentProp> SubProps;

    public Gear(GearType type, EquipmentProp mainProp)
    {
        Type = type;
        MainProp = mainProp;
        SubProps = new List<EquipmentProp>();
    }

    public Gear AddProp(EquipmentProp prop)
    {
        SubProps.Add(prop);
        return this;
    }

    public void Apply(AvatarStats stats, AvatarStats.Snapshot modSnapshot)
    {
        float modValue = stats.AddEquipmentModifier(MainProp);
        modSnapshot.Add(MainProp.StatType, modValue);
        for (int i = 0; i < SubProps.Count; i++)
        {
            var subProp = SubProps[i];
            modValue = stats.AddEquipmentModifier(subProp);
            modSnapshot.Add(subProp.StatType, modValue);
        }
    }
}

public class GearLoadout
{
    public Gear[] Gears = new Gear[4];
    public AvatarStats.Snapshot ModValuesSnapshot = new AvatarStats.Snapshot();

    public AvatarStats.Snapshot Apply(AvatarStats stats)
    {
        ModValuesSnapshot.Init();
        for (int i = 0; i < Gears.Length; i++)
        {
            Gears[i].Apply(stats, ModValuesSnapshot);
        }
        return ModValuesSnapshot;
    }

    public void SetGear(Gear gear)
    {
        Gears[(int)gear.Type] = gear;
    }
}

public abstract class Stats<EStatType> where EStatType : Enum, IConvertible
{
    public class Snapshot
    {
        public Dictionary<int, float> _dict = new Dictionary<int, float>();

        public Snapshot()
        {
            Init();
        }

        public void Init()
        {
            for (int i = 0; i < Types.Count(); i++)
            {
                _dict[i] = 0f;
            }
        }

        public void Init(Stats<EStatType> stats)
        {
            for (int i = 0; i < Types.Count(); i++)
            {
                _dict[i] = stats.Get(i).Value;
            }
        }

        public void Add(EStatType statType, float value)
        {
            _dict[statType.ToInt32(null)] += value;
        }
    }

    public static IEnumerable<EStatType> Types = Enum.GetValues(typeof(EStatType)).Cast<EStatType>();
    public Dictionary<int, Stat> _dict = new Dictionary<int, Stat>();

    public abstract void Init();

    public void Freeze()
    {
        foreach (var stat in _dict.Values)
        {
            stat.Base.Value = stat.Value;
            stat.RemoveAllModifiers();
        }
    }

    public Stat Get(EStatType statType)
    {
        return _dict[statType.ToInt32(null)];
    }

    public Stat Get(int i)
    {
        return _dict[i];
    }
}

public class AvatarStats : Stats<AvatarStatType>
{
    private AvatarConfig _cfg;

    public void SetConfig(AvatarConfig cfg)
    {
        _cfg = cfg;
    }

    public override void Init()
    {
        _dict[(int)AvatarStatType.MaxHP] = new Stat(_cfg.MaxHP);
        _dict[(int)AvatarStatType.Attack] = new Stat(_cfg.Attack);
        _dict[(int)AvatarStatType.Defense] = new Stat(_cfg.Defense);
        _dict[(int)AvatarStatType.Speed] = new Stat(_cfg.Speed);
        _dict[(int)AvatarStatType.CritRate] = new Stat(_cfg.CritRate);
        _dict[(int)AvatarStatType.CritDamage] = new Stat(_cfg.CritDamage);
    }

    public float AddEquipmentModifier(EquipmentProp prop)
    {
        Stat stat = Get((int)prop.StatType);
        Modifier mod = prop.CreateModifier(stat);
        stat.AddModifier(mod);
        return mod.Value;
    }
}

public class AvatarData
{
    public AvatarConfig AvatarCfg = new AvatarConfig();
    public Weapon Weapon = new Weapon(
        new EquipmentProp(AvatarStatType.Attack, 100),
        new EquipmentProp(AvatarStatType.CritRate, 0.31f)
        );
    public GearLoadout GearLoadout = new GearLoadout();

    public AvatarStats Stats = new AvatarStats();

    public void InitStats()
    {
        Stats.SetConfig(AvatarCfg);
        Stats.Init();
        // Apply weapon modifiers to stats.
        Weapon.Apply(Stats);
        // Apply gear modifiers to stats.
        GearLoadout.Apply(Stats);
        // Freeze stats values as base values.
        Stats.Freeze();
    }
}
