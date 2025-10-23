using System;
using System.Collections.Generic;

namespace CodeBreaker
{
    /// <summary>
    /// Represente un appel d'etat (State) avec ses arguments.
    /// Chaque StateCall contient le type de l'etat et un objet de parametres.
    /// Exemple : (GS_Warp, new WarpArgs {...})
    /// </summary>
    [Serializable]
    public class StateCall
    {
        public Type stateType; // type de l'etat a executer
        public object arg;     // objet d'arguments (WarpArgs, ConeArgs, etc.)

        public StateCall(Type stateType, object arg = null)
        {
            this.stateType = stateType;
            this.arg = arg;
        }
    }

    /// <summary>
    /// Represente un pattern d'attaque complet :
    /// un nom, une sequence ordonnee d'appels d'etats,
    /// et un delai final avant le prochain pattern.
    /// </summary>
    [Serializable]
    public class AttackPattern
    {
        public string name;                     // nom du pattern
        public float delay;                     // delai apres la fin du pattern
        public List<StateCall> sequence = new(); // liste des etapes du pattern

        public AttackPattern(string name, float endDelay)
        {
            this.name = name;
            this.delay = endDelay;
        }

        public AttackPattern(string name, float endDelay, params StateCall[] calls) : this(name, endDelay)
        {
            if (calls != null) sequence.AddRange(calls);
        }

       
    }
}
