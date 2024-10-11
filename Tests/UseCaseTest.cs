using NUnit.Framework;

public class UseCaseTest
{
    [Test]
    public void TestBasicUsage()
    {
        AvatarData avatar = new AvatarData();
        avatar.AvatarCfg = new AvatarConfig()
        {
            MaxHP = 2000f,
            Attack = 1000f,
            Defense = 500f,
            Speed = 100f,
            CritRate = 0.1f,
            CritDamage = 0.5f,
        };

        // Setup weapon.
        avatar.Weapon = new Weapon(
            new EquipmentProp(AvatarStatType.Attack, 100f),
            new EquipmentProp(AvatarStatType.CritRate, 0.31f)
            );

        // Setup gears.
        Gear head = new Gear(GearType.Head, new EquipmentProp(AvatarStatType.MaxHP, 100f));
        head.AddProp(new EquipmentProp(AvatarStatType.MaxHP, 0.1f, true));
        Gear hand = new Gear(GearType.Hand, new EquipmentProp(AvatarStatType.Attack, 100f));
        hand.AddProp(new EquipmentProp(AvatarStatType.Attack, 0.17f, true));
        Gear body = new Gear(GearType.Body, new EquipmentProp(AvatarStatType.CritDamage, 0.62f));
        Gear feet = new Gear(GearType.Feet, new EquipmentProp(AvatarStatType.Speed, 100f));
        GearLoadout gearLoadout = new GearLoadout();
        gearLoadout.SetGear(head);
        gearLoadout.SetGear(hand);
        gearLoadout.SetGear(body);
        gearLoadout.SetGear(feet);
        avatar.GearLoadout = gearLoadout;

        // Init stats.
        avatar.InitStats();
        Assert.That(avatar.Stats.Get(AvatarStatType.MaxHP).Value, Is.EqualTo(2300f));
    }
}
