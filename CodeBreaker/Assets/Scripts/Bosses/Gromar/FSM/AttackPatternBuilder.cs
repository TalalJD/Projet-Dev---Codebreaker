using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBreaker
{
    /// <summary>
    /// Classe utilitaire permettant de construire facilement un AttackPattern
    /// avec une syntaxe fluide (builder). Chaque etape correspond a un etat du boss.
    /// </summary>
    public class AttackPatternBuilder
    {
        private readonly string _name;          // nom du pattern
        private readonly float _endDelay;       // delai apres la fin du pattern
        private readonly List<StateCall> _seq = new(); // sequence des appels d'etats
       
        private AttackPatternBuilder(string name, float endDelay)
        {
            _name = name;
            _endDelay = endDelay;
        }

        /// <summary>
        /// Cree un nouveau builder d'AttackPattern.
        /// </summary>
        public static AttackPatternBuilder New(string name, float endDelay) => new(name, endDelay);

        public AttackPatternBuilder ForAllNextStateDelay(float seconds)
        {
            foreach (var sc in _seq)
            {
                if (sc.arg is StateBaseArgs args)
                {
                    if (Mathf.Approximately(args.nextStateDelay, 0.3f))
                    {
                        args.nextStateDelay = seconds;
                    }
                }
            }

            return this;
        }


        /// <summary>
        /// Ajoute un etat avec ses arguments a la sequence.
        /// </summary>
        /// 
        public AttackPatternBuilder Add(Type stateType, object arg = null)
        {
            _seq.Add(new StateCall(stateType, arg));
            return this;
        }

        // --- Methodes simplifiees pour ajouter des etats connus du boss ---

        public AttackPatternBuilder Warp(WarpArgs args = null)
            => Add(typeof(GS_Warp), args ?? new WarpArgs());

        public AttackPatternBuilder Cone(ConeArgs args)
            => Add(typeof(GS_Cone), args);

        public AttackPatternBuilder ParabolicMissile(ParabolicMissileArgs args)
            => Add(typeof(GS_MissilAttack), args);

        public AttackPatternBuilder Laser(LaserArgs args = null)
            => Add(typeof(GS_LaserAttack), args ?? new LaserArgs());

        /// <summary>
        /// Ajoute un etat d'attente (Idle) pour une duree precise.
        /// </summary>
        public AttackPatternBuilder Wait(float seconds)
            => Add(typeof(GS_Idle), new IdleArgs { Duration = seconds });

        /// <summary>
        /// Repete un sous-pattern un certain nombre de fois.
        /// Le contenu de la sous-fonction est ajoute plusieurs fois a la sequence.
        /// </summary>
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

        /// <summary>
        /// Termine la construction et retourne l'AttackPattern final.
        /// </summary>
        public AttackPattern Build()
        {
            var ap = new AttackPattern(_name, _endDelay);
            ap.sequence.AddRange(_seq);
            return ap;
        }
    }
}
