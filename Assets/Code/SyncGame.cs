using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SyncGame : MonoBehaviour
{
    [SerializeField]
    private MidiScriptableObj[] midiDataArray;

    private List<PlaybackEvent> eventsInput = new List<PlaybackEvent>();

    // config
    [SerializeField]
    private float workerActivateDistance;
    
    // events
    private PlaybackEvent[] events;

    // deps
    [SerializeField]
    private FactoryLine[] factoryLines;

    private GameObject[] factoryWorkers;

    // gameplay status
    private float fakePlayback;
    private int nextEventIndex;

    // Gets float array of note start times and line identifier and creates the eventsInput list.
    public void WritePlaybackEvents (List<float> noteTimes, int line) { 
        foreach(float noteTime in noteTimes) {
            PlaybackEvent playbackEvent = new PlaybackEvent();
            playbackEvent.time = noteTime;
            playbackEvent.type = PlaybackEventType.SpawnInLine;
            playbackEvent.arg1 = line;

            eventsInput.Add(playbackEvent);
        }
    }


    // Start is called before the first frame update
    private void Start()
    {
        eventsInput.Clear();
        foreach(MidiScriptableObj midiData in midiDataArray) {
            WritePlaybackEvents(midiData.noteStartTimes, midiData.typeID);
        }
        SortEventsAscending();
        events = eventsInput.ToArray();

        factoryLines = FindObjectsOfType<FactoryLine>();
        factoryWorkers = FindObjectsOfType<GameObject>()
            .Where(x => x.name == "factoryWorker")
            .ToArray();

        fakePlayback = -1f;
        nextEventIndex = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        fakePlayback += Time.deltaTime;

        MaybeStartNextEvent();


        var activateWorkerIndex = -1;
        if (Input.GetKeyDown(KeyCode.Q)) { activateWorkerIndex = 0; }
        if (Input.GetKeyDown(KeyCode.W)) { activateWorkerIndex = 1; }
        if (Input.GetKeyDown(KeyCode.E)) { activateWorkerIndex = 2; }

        var items = GetAllActiveItems();
        
        if (activateWorkerIndex != -1) {
            var worker = factoryWorkers[activateWorkerIndex];
            foreach (var item in items) {
                var toWorker = item.transform.position - worker.transform.position;
                if (toWorker.magnitude <= workerActivateDistance) {
                    // item.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
                    var color = new Color(1f, 1f, 1f, 1f);
                    color.r = UnityEngine.Random.Range(0f, 1f);
                    color.g = UnityEngine.Random.Range(0f, 1f);
                    color.b = UnityEngine.Random.Range(0f, 1f);
                    item.GetComponent<SpriteRenderer>().color = color;
                }
            }
        }
    }

    private int loopCount = 0;
    private void MaybeStartNextEvent()
    {
        if (nextEventIndex >= events.Length) {
            return;
        }

        var nextEvent = events[nextEventIndex];
        var time = GetPlaybackTime();

        if (time >= nextEvent.time) {
            StartEvent(nextEvent);
            nextEventIndex++;
            loopCount++;
            if(loopCount < 10000)
                MaybeStartNextEvent();
        }
    }

    private void StartEvent(PlaybackEvent eventToStart)
    {
        switch (eventToStart.type) {
            case PlaybackEventType.SpawnInLine:
                var line = factoryLines[eventToStart.arg1];
                line.SpawnNewItem();
                break;
        }
    }

    private float GetPlaybackTime()
    {
        return fakePlayback;
    }
    
    private FactoryItem[] GetAllActiveItems()
    {
        return factoryLines.SelectMany(line => line.GetAllActiveItems()).ToArray();
    }

    private void SortEventsAscending () { // Sort playback events by note timings. (Ascending)
        eventsInput.Sort(CompareNoteTimes);
    }
    public static int CompareNoteTimes (PlaybackEvent event1, PlaybackEvent event2) {
        return event1.time.CompareTo(event2.time);
    }
}
