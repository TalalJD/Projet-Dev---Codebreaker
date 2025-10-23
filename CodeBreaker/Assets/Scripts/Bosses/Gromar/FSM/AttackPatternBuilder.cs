using System;
using System.Collections.Generic;

namespace CodeBreaker
{
    /// <summary>Fluent builder for AttackPattern using simple arg objects.</summary>
    public class AttackPatternBuilder
    {
        private readonly string _name;
        private readonly float _endDelay;
        private readonly List<StateCall> _seq = new();

        private AttackPatternBuilder(string name, float endDelay)
        {
            _name = name;
            _endDelay = endDelay;
        }

        public static AttackPatternBuilder New(string name, float endDelay) => new(name, endDelay);

        public AttackPatternBuilder Add(Type stateType, object arg = null)
        {
            _seq.Add(new StateCall(stateType, arg));
            return this;
        }

        // Convenience methods. The concrete state classes are assumed to exist.
        public AttackPatternBuilder Warp(WarpArgs args = null)
            => Add(typeof(GS_Warp), args ?? new WarpArgs());

        public AttackPatternBuilder Cone(ConeArgs args)
            => Add(typeof(GS_Cone), args);

        public AttackPatternBuilder ParabolicMissile(ParabolicMissileArgs args)
            => Add(typeof(GS_MissilAttack), args);

        public AttackPatternBuilder Laser(LaserArgs args = null)
            => Add(typeof(GS_LaserAttack), args ?? new LaserArgs());

        /// <summary>Insert an Idle with a given delay.</summary>
        public AttackPatternBuilder Wait(float seconds)
            => Add(typeof(GS_Idle), new IdleArgs { Duration = seconds });

        /// <summary>Repeat the provided sub-sequence builder N times (args are treated as immutable per call).</summary>
        public AttackPatternBuilder Repeat(int times, Func<AttackPatternBuilder, AttackPatternBuilder> sub)
        {
            for (int i = 0; i < times; i++)
            {
                var tmp = new AttackPatternBuilder("_tmp", 0f);
                sub(tmp);
                _seq.AddRange(tmp._seq);
            }
            return this;
        }

        public AttackPattern Build()
        {
            var ap = new AttackPattern(_name, _endDelay);
            ap.sequence.AddRange(_seq);
            return ap;
        }
    }
}
