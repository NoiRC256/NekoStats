namespace NekoLib.Stats
{
    public enum StatModifierEffect
    {
        Add,
        Mult,
    }

    /// <summary>
    /// Modifies the value of a stat.
    /// </summary>
    [System.Serializable]
    public class StatModifier
    {

        public float Value;
        public StatModifierEffect Effect;

        public StatModifier(float value, StatModifierEffect effect)
        {
            Value = value;
            Effect = effect;
        }
    }
}