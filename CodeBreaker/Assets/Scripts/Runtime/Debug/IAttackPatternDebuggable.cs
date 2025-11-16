using System.Collections.Generic;

namespace CodeBreaker
{
    public interface IAttackPatternDebuggable
    {
        string MachineName { get; }
        string CurrentStateName { get; }

        /// <summary>Mode automatique (jeu normal) ou non (debug).</summary>
        bool AutoMode { get; set; }

        // Contrôle des patterns
        void DebugStartPattern();              // pattern aléatoire
        void DebugStartPatternByIndex(int i);  // pattern précis
        void DebugStep();                      // étape suivante du pattern
        void DebugStop();                      // stop / reset

        // Infos pour la fenêtre
        IReadOnlyList<string> DebugStateNames { get; }
        IReadOnlyList<string> DebugPatternNames { get; }

        void DebugSetStateByIndex(int index);
    }
}
