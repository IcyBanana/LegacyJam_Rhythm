using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SyncGame : MonoBehaviour
{
    // blah input
    [SerializeField]
    private List<PlaybackEvent> eventsInput;

    // events
    private PlaybackEvent[] events;

    // deps
    private FactoryLine[] factoryLines;

    // gameplay status
    private float fakePlayback;
    private int nextEventIndex;


    // Start is called before the first frame update
    void Start()
    {
        events = eventsInput.ToArray();

        factoryLines = FindObjectsOfType<FactoryLine>();

        fakePlayback = 0f;
        nextEventIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        fakePlayback += Time.deltaTime;

        MaybeStartNextEvent();
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
}
