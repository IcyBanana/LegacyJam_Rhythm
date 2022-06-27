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

    // config
    [SerializeField]
    private float workerActivateDistance;
    
    // events
    private PlaybackEvent[] events;

    // deps
    private FactoryLine[] factoryLines;
    private GameObject[] factoryWorkers;

    // gameplay status
    private float fakePlayback;
    private int nextEventIndex;


    // Start is called before the first frame update
    private void Start()
    {
        events = eventsInput.ToArray();

        factoryLines = FindObjectsOfType<FactoryLine>();
        factoryWorkers = FindObjectsOfType<GameObject>()
            .Where(x => x.name == "factoryWorker")
            .ToArray();

        fakePlayback = 0f;
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
