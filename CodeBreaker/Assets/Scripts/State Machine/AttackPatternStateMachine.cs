using System.Collections.Generic;
using UnityEngine;

namespace CodeBreaker
{
    /// <summary>
    /// StateMachine générique qui gère une liste d'AttackPattern
    /// et une queue de StateCall. Sert de base pour tous les boss à patterns.
    /// </summary>
    public abstract class AttackPatternStateMachine<TState>
        : StateMachine<TState>, IAttackPatternDebuggable
        where TState : State
    {
        [Header("Patterns")]
        [SerializeField]
        protected List<AttackPattern> attackPatterns = new();   // configuré par le boss concret

        protected AttackPattern currentPattern;
        protected Queue<StateCall> currentPatternQueue;

        // true = on est en train d'exécuter un AttackPattern (via pattern queue)
        // false = on est en mode "manuel" / hors pattern
        protected bool inPatternMode = false;

        [SerializeField]
        private bool autoMode = true;

        // ----------- API runtime générique -----------

        /// <summary>
        /// Lance un pattern suivant en utilisant la logique de sélection (par défaut random).
        /// Respecte AutoMode : si AutoMode == false, ne lancera rien.
        /// </summary>
        public void StartRandomPattern()
        {
            StartRandomPatternInternal(respectAutoMode: true);
        }

        /// <summary>
        /// Appelé par les states (Cone, Laser, etc.) lorsqu'ils ont fini leur boulot.
        /// </summary>
        public void ExecuteNextState()
        {
            ExecuteNextStateInternal();
        }

        /// <summary>
        /// Sélectionne un pattern via ChooseNextPatternIndex puis le démarre.
        /// </summary>
        protected void StartRandomPatternInternal(bool respectAutoMode)
        {
            if (respectAutoMode && !AutoMode)
                return; // mode manuel : pas de lancement auto de pattern

            int index = ChooseNextPatternIndex();
            if (index < 0) return;

            StartPatternAtIndexInternal(index);
        }

        /// <summary>
        /// Logique de choix du prochain pattern.
        /// Par défaut : random uniforme. Chaque boss peut override pour faire mieux.
        /// </summary>
        protected virtual int ChooseNextPatternIndex()
        {
            if (attackPatterns.Count == 0) return -1;
            return Random.Range(0, attackPatterns.Count); // 0 .. Count-1
        }

        /// <summary>
        /// Démarre un pattern précis par index (sans check AutoMode).
        /// </summary>
        protected void StartPatternAtIndexInternal(int index)
        {
            if (index < 0 || index >= attackPatterns.Count)
                return;

            currentPattern = attackPatterns[index];
            currentPatternQueue = new Queue<StateCall>(currentPattern.sequence);
            inPatternMode = true;

            Debug.Log($"Starting pattern: {currentPattern.name}");
            ExecuteNextStateInternal();
        }

        /// <summary>
        /// Avance dans la queue du pattern courant.
        /// Quand il n'y a plus d'étapes, appelle OnPatternFinished.
        /// </summary>
        protected void ExecuteNextStateInternal()
        {
            // Si on n'est pas dans un pattern, un state manuel qui appelle
            // Machine.ExecuteNextState() ne doit rien faire.
            if (!inPatternMode)
                return;

            if (currentPatternQueue == null || currentPatternQueue.Count == 0)
            {
                float delay = currentPattern != null ? currentPattern.delay : 1f;

                // pattern terminé : nettoyage + sortie du mode pattern
                currentPattern = null;
                currentPatternQueue = null;
                inPatternMode = false;

                OnPatternFinished(delay);
                return;
            }

            var nextCall = currentPatternQueue.Dequeue();
            SetStateCall(nextCall);
        }

        /// <summary>
        /// Applique un StateCall : set les args et force l'état.
        /// </summary>
        protected virtual void SetStateCall(StateCall call)
        {
            var state = AvailableStates.Find(s => s.GetType() == call.stateType);
            if (state == null) return;

            state.SetParam(call.arg);
            ForceSet(state);
        }

        /// <summary>
        /// Hook appelé quand un pattern est terminé (queue vide).
        /// Le boss concret décide quoi faire (Idle, Warp, relancer un pattern...).
        /// </summary>
        protected abstract void OnPatternFinished(float delay);

        // ----------- AutoMode + hook -----------

        public bool AutoMode
        {
            get => autoMode;
            set
            {
                if (autoMode == value) return;
                autoMode = value;
                OnAutoModeChanged(autoMode);
            }
        }

        /// <summary>
        /// Hook appelé quand AutoMode change (ON/OFF).
        /// Par défaut ne fait rien, chaque boss peut override si besoin.
        /// </summary>
        protected virtual void OnAutoModeChanged(bool isAuto) { }

        // ----------- Impl IAttackPatternDebuggable -----------

        public string MachineName => name;
        public string CurrentStateName => CurrentState?.GetType().Name ?? "None";

        public void DebugStartPattern()
        {
            // Debug doit pouvoir lancer des patterns même en AutoMode false
            StartRandomPatternInternal(respectAutoMode: false);
        }

        public void DebugStartPatternByIndex(int i)
        {
            // Ignore AutoMode en debug : c'est intentionnel
            StartPatternAtIndexInternal(i);
        }

        public void DebugStep()
        {
            // Avance d’une étape de la queue courante (si on est en mode pattern)
            ExecuteNextStateInternal();
        }

        public void DebugStop()
        {
            currentPattern = null;
            currentPatternQueue = null;
            inPatternMode = false;
        }

        public IReadOnlyList<string> DebugStateNames
            => AvailableStates.ConvertAll(s => s.GetType().Name);

        public IReadOnlyList<string> DebugPatternNames
            => attackPatterns.ConvertAll(p => p.name);

        public void DebugSetStateByIndex(int index)
        {
            if (index < 0 || index >= AvailableStates.Count)
                return;

            var state = AvailableStates[index];

            // on sort du mode pattern pour ne pas qu'il reprenne la main
            currentPattern = null;
            currentPatternQueue = null;
            inPatternMode = false;

            // IMPORTANT : reset les paramètres du state à leurs valeurs par défaut
            state.SetParam(null);

            // on force l'état demandé
            ForceSet(state);
        }

    }
}
