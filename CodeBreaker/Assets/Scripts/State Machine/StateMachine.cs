using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CodeBreaker
{
    /// <summary>
    /// Machine à état dynamique. Doit être héritée pour produire le resultat désiré
    /// </summary>
    /// <typeparam name="T">type "placholder" pour le type d'état qui hérite de la classe State</typeparam>
    public abstract class StateMachine<T> : MonoBehaviour where T : State
    {
        public T CurrentState;
        public T PreviousState;
        public T NextState;

        [SerializeReference]
        public List<T> AvailableStates= new List<T>(); //liste des états disponibles

        /// <summary>
        /// Permet d'ajouter un nouvel état a la liste des états
        /// </summary>
        /// <param name="state">état quelconque choisi</param>
        public virtual void Add(T state)
        {
            if (AvailableStates == null) AvailableStates = new();
            if (AvailableStates.Find(s => s.GetType() == state.GetType()) == null)
            {
                AvailableStates.Add(state);
            }
        }

        /// <summary>
        /// Permet d'intialiser un état de base
        /// </summary>
        /// <typeparam name="G">état quelconque choisi</typeparam>
        public void Initialize<G>()
        {
            CurrentState = AvailableStates.Find(s => s is G);
            PreviousState = CurrentState;
            CurrentState.OnEnter();
        }

        /// <summary>
        /// Verifie si l'état en question est l'état actuel
        /// </summary>
        public bool IsCurrentState<G>(bool StrictRequest = false)
        {
            if (StrictRequest)
            {
                return CurrentState.GetType() == typeof(G);
            }
            return CurrentState is G;
        }

        /// <summary>
        /// Désigne l'état choisi en tant qu'état actif
        /// </summary>
        /// <typeparam name="G">état quelconque choisi</typeparam>
        public virtual void Set<G>(bool TriggerEnter = true) where G : T
        {
            NextState = AvailableStates.Find(s => s is G);
            CurrentState.OnExit();
            PreviousState = CurrentState;
            CurrentState = NextState;
            NextState = null;
            if (TriggerEnter) CurrentState.OnEnter();
        }

        public virtual void ForceSet(T state)
        {
            NextState = state;
            CurrentState.OnExit();
            PreviousState = CurrentState;
            CurrentState = NextState;
            NextState = null;
            CurrentState.OnEnter();
        }


        public G Get<G>() where G : T
        {
            return AvailableStates.Find((s => s is G)) as G;
        }

      

    }
}
