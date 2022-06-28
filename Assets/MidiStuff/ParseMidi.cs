using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Standards;


public class ParseMidi : MonoBehaviour
{   

    public MidiScriptableObj bakeTarget = null;
    /* The formula is 60000 / (BPM * PPQ) (milliseconds).
        Where BPM is the tempo of the track (Beats Per Minute).
            (i.e. a 120 BPM track would have a MIDI time of (60000 / (120 * 96)) or 2.604 ms for 1 tick. */

    public string filePath = @"\testMidi.mid";
    public float BPM = 120;

    public void Bake() {
        if(bakeTarget != null) {
            IEnumerable<NoteInfo> notesInfo = GetNotesInfo(Application.dataPath + @"\" + filePath);
            int i = 0;
            float lastNoteTime = -1f;
            bakeTarget.noteStartTimes.Clear();
            foreach(NoteInfo noteInfo in notesInfo) {
                float newNoteTime = noteInfo.Time * (60000 / (BPM * 96f)) / 1000f;
                if(newNoteTime != lastNoteTime) {
                    bakeTarget.noteStartTimes.Add(newNoteTime);
                    lastNoteTime = newNoteTime;
                }     
                i++;
            }
        }
    }

    public class NoteInfo
    {
        public int? ProgramNumber { get; set; }
        public long Time { get; set; }
        public long Length { get; set; }
        public int NoteNumber { get; set; }
    }

    private static IEnumerable<NoteInfo> GetNotesInfo(string filePath)
    {
        var midiFile = MidiFile.Read(filePath);

        // build the program changes map

        var programChanges = new Dictionary<FourBitNumber, Dictionary<long, SevenBitNumber>>();
        foreach (var timedEvent in midiFile.GetTimedEvents())
        {
            var programChangeEvent = timedEvent.Event as ProgramChangeEvent;
            if (programChangeEvent == null)
                continue;

            var channel = programChangeEvent.Channel;

            Dictionary<long, SevenBitNumber> changes;
            if (!programChanges.TryGetValue(channel, out changes))
                programChanges.Add(channel, changes = new Dictionary<long, SevenBitNumber>());

            changes[timedEvent.Time] = programChangeEvent.ProgramNumber;
        }

        // collect notes info

        return midiFile.GetNotes()
                       .Select(n => new NoteInfo
                       {
                           ProgramNumber = GetProgramNumber(n.Channel, n.Time, programChanges),
                           Time = n.Time,
                           Length = n.Length,
                           NoteNumber = n.NoteNumber
                       });
    }

    private static int? GetProgramNumber(FourBitNumber channel, long time, Dictionary<FourBitNumber, Dictionary<long, SevenBitNumber>> programChanges)
    {
        Dictionary<long, SevenBitNumber> changes;
        if (!programChanges.TryGetValue(channel, out changes))
            return null;

        var times = changes.Keys.Where(t => t <= time).ToArray();
        return times.Any()
            ? (int?)changes[times.Max()]
            : null;
    }
}
