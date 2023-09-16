# Overview
NekoStats is a lightweight reactive stat system for Unity. It provides the building blocks for character stats, perks, buffs & debuffs, all that good stuff you might see in stats-heavy games such as rogue-likes and RPGs.

**Key Features**

</details>
<details><summary><b>Lightweight observable stats</b></summary>
Listen to ValueChanged to observe value;

Listen to StatChanged to observe stat instance.

```csharp
void OnEnable() {
    Character.Health.StatChanged += HandleHealthChanged;
}

void OnDisable() {
    Character.Health.StatChanged -= HandleHealthChanged;
}

void HandleHealthChanged(Stat stat) {
    Debug.Log("Character health has changed: " + stat.Value);
}
```
</details>

</details>
<details><summary><b>Non-destructive value modification</b></summary>

```csharp
Stat speed = new Stat(10f);

StatModifier speedMultiplyModifier = new StatModifier(0.5f, StatModifierEffect.Mult);

speed.AddModifier(speedMultiplyModifier);
Assert.AreEqual(speed.Value, 15f);

StatModifier speedAddModifier = new StatModifier(2f, StatModifierEffect.Add);

speed.AddModifier(speedAddModifier);
Assert.AreEqual(speed.Value, 17f);

speed.RemoveModifier(speedMultiplyModifier);
Assert.AreEqual(speed.Value, 12f);
```
</details>

</details>
<details><summary><b>Optimized Lazy Calculation</b></summary>
If a stat is marked dirty, its final value will be re-calculated upon next access, then cached until marked dirty again.

```csharp
// Default configuration. Every tick, if the stat is marked dirty, invokes events once to notify pending changes.
Stat stat1 = new Stat().SetObserveMode(StatObserveMode.EveryTick);

// Directly invokes events to notify changes.
// Note this might cause the stat to be re-calculated multiple times in one frame.
Stat stat2 = new Stat().SetObserveMode(StatObserveMode.EveryChange);
```
</details>

</details>
<details><summary><b>Reactive upper & lower bounds</b></summary>
Changes in the upper bound and lower bound will propagate to the stat and cause it to be marked dirty.

```csharp
Stat maxHealth = new Stat(500f);

// currentHealth will be bounded between 0 and maxHealth.
// If maxHealth becomes lower than currentHealth, currentHealth will be marked dirty and re-calculated on next access.
Stat currentHealth = new Stat(500f).SetLowerBound().SetUpperBound(maxHealth);
```
</details>

</details>
<details><summary><b>Stat collections with custom enum keys</b></summary>
Easily manage stat creation and access with StatContainer.

```csharp
    public enum AvatarStatType
    {
        MaxHealth,
        Health,
        MaxMana,
        Mana,
        Attack,
        Defence,
        Speed,
    }

    public class AvatarData : MonoBehaviour
    {
        [field: SerializeField] public AvatarConfig Config { get; private set; }
        [field: SerializeField] public StatContainer<AvatarStatType> Stats { get; private set; }
            = new StatContainer<AvatarStatType>();

        private void Awake()
        {
            Stats.RegisterStat(AvatarStatType.MaxHealth, Config.MaxHealth);
            Stats.RegisterResourceStat(AvatarStatType.Health, Config.MaxHealth, AvatarStatType.MaxHealth);
            Stats.RegisterStat(AvatarStatType.MaxMana, Config.MaxMana);
            Stats.RegisterResourceStat(AvatarStatType.Mana, Config.MaxMana, AvatarStatType.MaxMana);

            Stats.RegisterStat(AvatarStatType.Attack, Config.Attack).SetLowerBound(0f);
            Stats.RegisterStat(AvatarStatType.Defence, Config.Defence).SetLowerBound(0f);
            Stats.RegisterStat(AvatarStatType.Speed, Config.Speed).SetLowerBound(0f);
        }

        private void LateUpdate()
        {
            Stats.Tick();
        }

        public Stat GetStat(AvatarStatType statType)
        {
            return Stats.Get(statType);
        }
    }
```
</details>

<!-- A stat will be marked dirty if it has pending changes. These changes might originate from adding or removing a modifier, setting the base value, setting the final value, or changes that propagate from the upper & lower bounds.>
