using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ParseMidi))]
public class CustomMidiEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ParseMidi myTarget = (ParseMidi)target;

        
        if(GUILayout.Button("Bake Midi")) {
            myTarget.Bake();
            Selection.objects = new Object[] {myTarget.bakeTarget};
        }
    }
}
