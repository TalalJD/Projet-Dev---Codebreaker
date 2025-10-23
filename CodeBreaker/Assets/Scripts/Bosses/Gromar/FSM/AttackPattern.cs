using System;
using System.Collections.Generic;

namespace CodeBreaker
{
    /// <summary>One invocation of a state with its argument object.</summary>
    [Serializable]
    public class StateCall
    {
        public Type stateType;
        public object arg; // single arg object (e.g., WarpArgs, ConeArgs, etc.)

        public StateCall(Type stateType, object arg = null)
        {
            this.stateType = stateType;
            this.arg = arg;
        }
    }

    /// <summary>A named sequence of state calls, followed by an end-delay.</summary>
    [Serializable]
    public class AttackPattern
    {
        public string name;
        public float delay;
        public List<StateCall> sequence = new List<StateCall>();

        public AttackPattern(string name, float endDelay)
        {
            this.name = name;
            this.delay = endDelay;
        }

        public AttackPattern(string name, float endDelay, params StateCall[] calls) : this(name, endDelay)
        {
            if (calls != null) sequence.AddRange(calls);
        }

        public static AttackPattern BuildAlternatingPattern(
            string name,
            float endDelay,
            int repeatCount,
            StateCall callA,
            StateCall callB)
        {
            var ap = new AttackPattern(name, endDelay);
            for (int i = 0; i < repeatCount; i++)
            {
                ap.sequence.Add(callA);
                ap.sequence.Add(callB);
            }
            return ap;
        }
    }
}
