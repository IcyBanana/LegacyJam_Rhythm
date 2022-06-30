using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/*

  Tasks:
- [X] Score when objects get to end + score field
- [X] Score effect when objects get to line end
- [X] Implement FactoryItem changing graphics (swapped sprites)
- [X] Worker animation for processing (cloud)
- [X] Factory Line art + Animation
- [X] Intro screen with "Log In"
- [X] Background art
- [X] Add Boss avatar in the corner in the UI - change face based on recent (or total) score
- [X] If getting a very bad score for a while - move into "YOU'RE FIRED" scene.
- [ ] Game over Button to restart game
- [ ] Set new font for texts
- [ ] When song ends - either get fired, or, go into a VICTORY screen (also button to restart game)
- [ ] Send the score as LEGACY EVENT when any GameOver is reached
- [ ] Add special FactoryItems based on read legacy events from Renaissance
- [ ] Worker animation for 180 turns
- [ ] Add some FactoryItems with 4 states (not just 3)
- [ ] Soundtrack + Midi
- [ ] Add help about the SCORING system in the Intro screen
----- Optional ------
- [ ] Stun/Advanced cooldown system
- [ ] PPFX
- [ ] Artistic Shaders

*/

public class SyncGame : MonoBehaviour
{
    [SerializeField]
    private MidiScriptableObj[] midiDataArray;

    private List<PlaybackEvent> eventsInput = new List<PlaybackEvent>();

    // Music events:
    private PlaybackEvent[] events;
    
    // Editor dependencies:
    [SerializeField] public GameConfig gameConfig;
    [SerializeField] private FactoryLine[] factoryLines;
    [SerializeField] private FactoryWorker[] factoryWorkers;
    [SerializeField] private ParticleSystem gameOverParticleEffect;

    // Runtime dependencies:
    private GameUI gameUI;

    // Gameplay status
    private bool gameOver;
    private float fakePlaybackTime;
    private int nextEventIndex;
    private int loopCount = 0;
    private int score;


    // Start is called before the first frame update
    private void Start()
    {
        score = 0;
        gameUI = FindObjectOfType<GameUI>();
        gameUI.Init(gameConfig);
        gameUI.SetScoreDisplay(score);
        
        eventsInput.Clear();
        foreach(MidiScriptableObj midiData in midiDataArray) {
            WritePlaybackEvents(midiData.noteStartTimes, midiData.typeID);
        }
        SortEventsAscending();
        events = eventsInput.ToArray();

        foreach (var factoryWorker in factoryWorkers) {
            factoryWorker.Init(gameConfig);
        }

        foreach (var factoryLine in factoryLines) {
            factoryLine.Init(gameConfig);
            factoryLine.ScoreItemOnReachEnd += ScoreItemOnReachEnd;
        }
        

        fakePlaybackTime = -1f;
        nextEventIndex = 0;
    }

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

    private void ScoreItemOnReachEnd(FactoryItem item)
    {
        int scoreToAdd = 0;
        var condition = item.GetCondition();
        switch (condition) {
            case ItemCondition.Raw:
                scoreToAdd = gameConfig.scoreOnRaw;
                break;
            case ItemCondition.Finished:
                scoreToAdd = gameConfig.scoreOnFinished;
                break;
            case ItemCondition.Ruined:
                scoreToAdd = gameConfig.scoreOnRuined;
                break;
        }
        score += scoreToAdd;
        gameUI.SetScoreDisplay(score);
        gameUI.AddScoreEffect(scoreToAdd, item.transform.position);

        if (score < gameConfig.scoreToGetFired) {
            gameOverParticleEffect.Play();
            gameUI.ShowGameOver();
            gameOver = true;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (gameOver) {
            return;
        }

        fakePlaybackTime += Time.deltaTime;

        var time = GetPlaybackTime();

        MaybeStartNextEvent();

        foreach (var line in factoryLines) {
            line.DoUpdate();
        }

        foreach (var worker in factoryWorkers) {
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
                    worker.StartWorkingOnItem();
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
        return fakePlaybackTime;
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
