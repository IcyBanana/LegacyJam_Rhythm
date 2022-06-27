using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Standards;

public class WriteMIDIToText : MonoBehaviour
{
    
   /* public static void ConvertMidiToText(string midiFilePath, string textFilePath)
    {
        var midiFile = MidiFile.Read(midiFilePath);
        TempoMap tempoMap = midiFile.GetTempoMap();

        File.WriteAllLines(textFilePath,
            midiFile.GetNotes()
                .Select(n => $"{n.NoteNumber} {n.TimeAs<MetricTimeSpan>(tempoMap)} {n.LengthAs<MetricTimeSpan>(tempoMap)}"));
    }*/
}
