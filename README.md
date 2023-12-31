# Overview
NekoStats is a lightweight reactive stat system for Unity. It provides the building blocks for character stats, perks, buffs & debuffs, all that stat-related logic you might see in data-heavy genres such as rogue-likes and RPGs.
## Key Features

(Click to expand each section)

</details>
<details><summary><b>Flexible value modification</b></summary>
<br>
Each stat instance maintains its own collection of modifiers for non-destructive value modification.

```csharp
Stat speed = new Stat(10f);

StatModifier speedMultiplyModifier = new StatModifier(0.5f, StatModifierEffect.Mult);
StatModifier speedAddModifier = new StatModifier(2f, StatModifierEffect.Add);

// Add multiplicative modifier.
speed.AddModifier(speedMultiplyModifier);
Assert.AreEqual(speed.Value, 15f);

// Add additive modifier.
speed.AddModifier(speedAddModifier);
Assert.AreEqual(speed.Value, 17f);

// Remove multiplicative modifier.
speed.RemoveModifier(speedMultiplyModifier);
Assert.AreEqual(speed.Value, 12f);
```
</details>

</details>
<details><summary><b>Listen for changes</b></summary>
<br>
You can easily subcribe to value change events on a stat to avoid polling for changes every frame.

- Subscribe to `ValueChanged` event to observe value
- Subscribe to `StatChanged` event to observe stat instance

```csharp
void OnEnable() {
    _character.Health.StatChanged += HandleHealthChanged;
}

void OnDisable() {
    _character.Health.StatChanged -= HandleHealthChanged;
}

void HandleHealthChanged(Stat stat) {
    Debug.Log("Character health has changed: " + stat.Value);
}
```
</details>

</details>
<details><summary><b>Optimized value caching</b></summary>
<br>
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
<details><summary><b>Upper & lower bounds</b></summary>
<br>
Enable upper or lower bounds for a stat.

Changes in a stat's upper bound and lower bound will propagate to the stat and cause it to be marked dirty.

```csharp
// currentHealth will be bounded between 0 and maxHealth.
Stat maxHealth = new Stat(500f);
Stat currentHealth = new Stat(500f).SetLowerBound().SetUpperBound(maxHealth);

Assert.AreEqual(maxHealth.Value, 500f);
Assert.AreEqual(currentHealth.Value, 500f);

// maxHealth has become lower than currentHealth.
// currentHealth is be marked dirty and re-calculated on next access.
maxHealth.Value = 450f;
Assert.AreEqual(maxHealth.Value, 450f);
Assert.AreEqual(currentHealth.Value, 450f);
```
</details>

</details>
<details><summary><b>Easily create and access stats</b></summary>
<br>
Manage stat creation with StatContainer. Access stats efficiently through custom enum keys.

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
