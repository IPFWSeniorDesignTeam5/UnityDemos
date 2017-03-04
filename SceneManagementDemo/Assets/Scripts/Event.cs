using System.Collections;
using System.Collections.Generic;

namespace Tribal
{
    /// <summary>
    /// Status Modifiers will affect one or more of the following aspects of a Family:
    ///     Property modification - i.e. Population/growth speed, Capability, Prosperity
    ///     Skill modification - i.e. Skill experience gain speed, gain/loss of raw material yield, gain/loss of finished good production time
    ///     Raw Materials - i.e. Gain/Loss of raw materials
    ///     Finished Goods - i.e. Gain/Loss of finished goods
    /// </summary>
    public class StatusModifier
    {
        public class Effect
        {
            public enum Modifier
            {
                Amount,
                Speed
            }

            public enum Target
            {
                Property,
                Skill,
                RawMaterial,
                FinishedGood
            }

            public Modifier modifier { get; private set; }
            public Target target { get; private set; }

            public Effect(Modifier m, Target g)
            {
                modifier = m;
                target = g;
            }
        }

        public string Description { get; private set; }

        public Effect StatusEffect { get; private set; }

        public float Value { get; private set; }

        public StatusModifier(Effect eff, string desc, float value)
        {
            Value = value;
            Description = desc;
            StatusEffect = eff;
        }
    }

    public delegate void TribalEventListener( List<TribalEvent> EventsOccurred);

    /// <summary>
    /// Tribal Events apply status modifications to individual families or the entire community (effectively applying the modifier each family.)
    /// </summary>
    public class TribalEvent
    {
        public string Description { get; private set; }

        public int SeasonsAffected { get; private set; }

        public List<StatusModifier> StatusModifiers { get; private set; }

        /// <summary>
        /// TribalEvent definition.
        /// </summary>
        /// <param name="desc">Literal description of the event.</param>
        /// <param name="seasons">Number of seasons event status modifier(s) will be applied.</param>
        /// <param name="modify">Modifiers applied to a family.</param>
        public TribalEvent( string desc, int seasons = 1, params StatusModifier [] modify )
        {
            StatusModifiers = new List<StatusModifier>();
            Description = desc;
            SeasonsAffected = seasons;

            foreach( StatusModifier m in modify )
                StatusModifiers.Add(m);
        }

        public void AddModifier( StatusModifier mod )
        {
            StatusModifiers.Add(mod);
        }
    }
}