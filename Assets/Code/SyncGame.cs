using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class SyncGame : MonoBehaviour
{
    [SerializeField]
    private MidiScriptableObj[] midiDataArray;

    private List<PlaybackEvent> eventsInput = new List<PlaybackEvent>();

    // events:
    private PlaybackEvent[] events;
    
    // editor dependencies:
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private FactoryLine[] factoryLines;
    [SerializeField] private FactoryWorker[] factoryWorkers;

    // runtime dependencies:
    private TotemIntegration totemIntegration;

    // gameplay status
    private float fakePlayback;
    private int nextEventIndex;
    private int loopCount = 0;

    // Gets float array of note start times and line identifier and creates the eventsInput list.
    public void WritePlaybackEvents (List<float> noteTimes, int line)
    { 
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
        //totemIntegration = new TotemIntegration();
        //totemIntegration.Init();

        eventsInput.Clear();
        foreach(MidiScriptableObj midiData in midiDataArray) {
            WritePlaybackEvents(midiData.noteStartTimes, midiData.typeID);
        }
        SortEventsAscending();
        events = eventsInput.ToArray();

        foreach (FactoryWorker factoryWorker in factoryWorkers) {
            factoryWorker.Init(gameConfig);
        }


        fakePlayback = -1f;
        nextEventIndex = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        fakePlayback += Time.deltaTime;

        var time = GetPlaybackTime();

        MaybeStartNextEvent();

        foreach (FactoryWorker worker in factoryWorkers)
        {
            if (!worker.OnCooldown(time)) {
                worker.EndCooldown();
            }
        }

        var activateWorkerIndex = -1;
        if (Input.GetKeyDown(KeyCode.Q)) { activateWorkerIndex = 0; }
        if (Input.GetKeyDown(KeyCode.W)) { activateWorkerIndex = 1; }
        if (Input.GetKeyDown(KeyCode.E)) { activateWorkerIndex = 2; }

        var items = GetAllActiveItems();
        
        if (activateWorkerIndex != -1) {
            var worker = factoryWorkers[activateWorkerIndex];

            if (worker.OnCooldown(time)) {
                Debug.Log("Worker is on cooldown");
            } else {
                worker.FlipVerticalDirection();
                
                var itemToActivate = (FactoryItem)null;
                if (!worker.OnCooldown(time)) {
                    foreach (var item in items) {
                        var toItem = item.transform.position - worker.transform.position;
                        if (Mathf.Abs(toItem.x) <= gameConfig.workerActivateDistanceX &&
                            Mathf.Abs(toItem.y) <= gameConfig.workerActivateDistanceY)
                        {
                            if (
                                (toItem.y < 0 && worker.verticalDirection == 1)
                                ||
                                (toItem.y > 0 && worker.verticalDirection == -1)
                            ) {
                                itemToActivate = item;
                            }
                        }
                    }
                }

                if (itemToActivate) {
                    itemToActivate.AdvanceCondition();
                }

                worker.StartCooldown(time);
            }
        }
    }


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
