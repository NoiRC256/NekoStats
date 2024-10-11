using NUnit.Framework;
using CC.Stats;

public class StatsTest
{
    /// <summary>
    /// Increase max health by a flat amount then by % of base max health.
    /// </summary>
    [Test]
    public void TestModifyStat()
    {
        Stat healthMax = new Stat(100f);
        Modifier flatMod = new Modifier(25f);
        Modifier percMod = new PercentModifier(0.1f, healthMax.Base);

        // Add flat and percent modifiers.
        healthMax.AddModifier(flatMod);
        Assert.That(healthMax.Value, Is.EqualTo(100f + 25f));
        healthMax.AddModifier(percMod);
        Assert.That(healthMax.Value, Is.EqualTo(100 + 25f + (100f * 0.1f)));

        // Remove flat modifier.
        healthMax.RemoveModifier(flatMod);
        Assert.That(healthMax.Value, Is.EqualTo(100f + (100f * 0.1f)));
    }

    /// <summary>
    /// Increase max shield by a flat amount equal to % of base max health.
    /// </summary>
    [Test]
    public void TestAddFlatValueByStatPercentage()
    {
        Stat healthMax = new Stat(100f);
        Stat shieldMax = new Stat(500f);
        Modifier mod = new PercentModifier(0.1f, healthMax.Base);
        shieldMax.AddModifier(mod);
        Assert.That(healthMax.Value, Is.EqualTo(100f));
        Assert.That(shieldMax.Value, Is.EqualTo(500f + (100f * 0.1f)));
    }

    /// <summary>
    /// Increase attack by a percentage equal to % of base max shield.
    /// </summary>
    [Test]
    public void TestAddStatPercentageByOtherStatPercentage()
    {
        Stat shieldMax = new Stat(500f);
        Stat attack = new Stat(60f);
        Modifier percent = new PercentModifier(0.001f, shieldMax.Base);
        Modifier mod = new PercentModifier(percent.Value, attack.Base);
        attack.AddModifier(mod);
        // Max shield is 500, 0.1% of that is 0.5 = 50%, so attack should increse by 50%.
        Assert.That(shieldMax.Value, Is.EqualTo(500f));
        Assert.That(attack.Value, Is.EqualTo(90f));
    }

    /// <summary>
    /// When a modifier's mod value changes, whatever stat it's applied to
    /// should reflect the change.
    /// </summary>
    [Test]
    public void TestModifierModValueChange()
    {
        Stat attack = new Stat(60f);
        Modifier mod = new Modifier(5f);
        attack.AddModifier(mod);
        Assert.That(attack.Value, Is.EqualTo(65f));
        mod.SetModValue(10f);
        Assert.That(attack.Value, Is.EqualTo(70f));
    }

    /// <summary>
    /// When a modifier's base value changes, whatever stat it's applied to
    /// should reflect the change.
    /// </summary>
    [Test]
    public void TestModifierBaseValueChange()
    {
        Stat attack = new Stat(60f);
        Stat defence = new Stat(50f);
        Modifier mod = new PercentModifier(0.1f, attack.Base);
        defence.AddModifier(mod);
        Assert.That(attack.Value, Is.EqualTo(60f));
        Assert.That(defence.Value, Is.EqualTo(56f));
        attack.Base.Value = 70f;
        Assert.That(attack.Value, Is.EqualTo(70f));
        Assert.That(defence.Value, Is.EqualTo(57f));
    }
}
