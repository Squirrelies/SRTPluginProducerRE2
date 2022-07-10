using System.Diagnostics;

namespace SRTPluginProducerRE2.Structs
{
    [DebuggerDisplay("{_DebuggerDisplay,nq}")]
    public struct EnemyHP
    {
        /// <summary>
        /// Debugger display message.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#pragma warning disable IDE1006 // Naming Styles
        public string _DebuggerDisplay
#pragma warning restore IDE1006 // Naming Styles
        {
            get
            {
                if (IsTrigger)
                    return string.Format("TRIGGER", CurrentHP, MaximumHP, Percentage);
                else if (IsAlive)
                    return string.Format("{0} / {1} ({2:P1})", CurrentHP, MaximumHP, Percentage);
                else
                    return "DEAD / DEAD (0%)";
            }
        }

        public int? MaximumHP { get => maximumHP; }
        internal int? maximumHP;

        public int? CurrentHP { get => currentHP; }
        internal int? currentHP;

        public bool IsTrigger => MaximumHP == 1 && CurrentHP == 1; // Some triggers load in as enemies as 1/1 hp. We're excluding that by checking to make sure max hp is greater than 1 rather than greater than 0.
        public bool IsAlive => !IsTrigger && MaximumHP > 0 && CurrentHP > 0 && CurrentHP <= MaximumHP;
        public bool IsDamaged => MaximumHP > 0 && CurrentHP > 0 && CurrentHP < MaximumHP;
        public float Percentage => ((IsAlive) ? (float)(CurrentHP ?? 0) / (float)(MaximumHP ?? 0) : 0f);
    }
}
