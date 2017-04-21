#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ChordQualities))]
public class CQModuleEditor : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        if (GUILayout.Button("Build Lights")) {
            ((ChordQualities)target).BuildLights();
        }

        if (GUILayout.Button("Build Manual")) {
            ((ChordQualities)target).BuildManual();
        }
    }
}
#endif