using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AttackPattern 
{

    public string name;
    public List<Type> stateSequence = new List<Type>();

    public AttackPattern(string name, params Type[] states){
        this.name = name;
        stateSequence.AddRange(states);
        
    }



}
