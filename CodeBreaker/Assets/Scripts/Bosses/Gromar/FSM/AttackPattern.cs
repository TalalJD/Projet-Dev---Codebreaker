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
}
