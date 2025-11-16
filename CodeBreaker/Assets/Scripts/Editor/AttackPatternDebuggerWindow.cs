#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using CodeBreaker;

public class AttackPatternDebuggerWindow : EditorWindow
{
    private Object rawObject;
    private IAttackPatternDebuggable target;

    [MenuItem("Window/CodeBreaker/Attack Pattern Debugger")]
    public static void ShowWindow()
    {
        GetWindow<AttackPatternDebuggerWindow>("Attack Pattern Debugger");
    }

    private void OnGUI()
    {
        GUILayout.Label("Attack Pattern Debugger", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        rawObject = EditorGUILayout.ObjectField(
            "Target Machine",
            rawObject,
            typeof(Object),
            true
        );

        target = GetDebuggableFromObject(rawObject);

        if (rawObject != null && target == null)
        {
            EditorGUILayout.HelpBox(
                "L’objet sélectionné ne possède pas IAttackPatternDebuggable.",
                MessageType.Warning);
            return;
        }

        if (target == null)
        {
            EditorGUILayout.HelpBox(
                "Glisse ici ton GameObject Gromar (avec GromarStateMachine).",
                MessageType.Info);
            return;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Machine", target.MachineName);
        EditorGUILayout.LabelField("Current State", target.CurrentStateName);

        EditorGUILayout.Space();
        bool auto = target.AutoMode;
        bool newAuto = EditorGUILayout.Toggle("Auto mode", auto);
        if (newAuto != auto)
            target.AutoMode = newAuto;

        EditorGUILayout.Space();
        GUILayout.Label("Patterns (click pour démarrer)", EditorStyles.boldLabel);

        using (new EditorGUI.DisabledScope(target.AutoMode))
        {
            var patterns = target.DebugPatternNames;
            if (patterns != null)
            {
                for (int i = 0; i < patterns.Count; i++)
                {
                    if (GUILayout.Button($"{i} - {patterns[i]}"))
                        target.DebugStartPatternByIndex(i);
                }
            }

            EditorGUILayout.Space();
            GUILayout.Label("États (click pour forcer l’état)", EditorStyles.boldLabel);

            var states = target.DebugStateNames;
            if (states != null)
            {
                for (int i = 0; i < states.Count; i++)
                {
                    if (GUILayout.Button(states[i]))
                        target.DebugSetStateByIndex(i);
                }
            }
        }

        EditorGUILayout.Space();
        GUILayout.Label("Contrôles de pattern", EditorStyles.boldLabel);

        if (GUILayout.Button("▶ Debug Start Pattern (random / normal)"))
            target.DebugStartPattern();

        if (GUILayout.Button("⏭ Debug Step (Next State)"))
            target.DebugStep();

        if (GUILayout.Button("⏹ Debug Stop"))
            target.DebugStop();
    }


    private IAttackPatternDebuggable GetDebuggableFromObject(Object obj)
    {
        if (obj == null) return null;

        if (obj is GameObject go)
            return go.GetComponent<IAttackPatternDebuggable>();

        if (obj is Component comp)
            return comp.GetComponent<IAttackPatternDebuggable>();

        return null;
    }
}
#endif
