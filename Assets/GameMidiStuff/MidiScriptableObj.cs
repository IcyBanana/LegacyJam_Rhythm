using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MidiData", menuName = "ScriptableObjects/MidiScriptableObject", order = 1)]
public class MidiScriptableObj : ScriptableObject
{
    public enum midiType {
        Bass,
        Mid,
        High
    }
    public midiType myType;
    public int typeID;
    public List<float> noteStartTimes;

}
