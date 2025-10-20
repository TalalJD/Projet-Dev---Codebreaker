using System.Collections.Generic;
using System;

[Serializable]
public class StateCall
{
    public Type stateType;
    public object[] args;

    public StateCall(Type type, params object[] args)
    {
        stateType = type;
        this.args = args;
    }
}

[Serializable]
public class AttackPattern
{
    public string name;
    public float delay;
    public List<StateCall> sequence = new List<StateCall>();

    public AttackPattern(string name, float delay, params StateCall[] calls)
    {
        this.name = name;
        this.delay = delay;
        sequence.AddRange(calls);
    }

    public static AttackPattern BuildAlternatingPattern(
    string name,
    float endDelay,
    int repeatCount,
    Type stateA, object[] argsA,
    Type stateB, object[] argsB)
    {
        var pattern = new AttackPattern(name, endDelay);
        for (int i = 0; i < repeatCount; i++)
        {
            pattern.sequence.Add(new StateCall(stateA, argsA));
            pattern.sequence.Add(new StateCall(stateB, argsB));
        }
        return pattern;
    }


}
