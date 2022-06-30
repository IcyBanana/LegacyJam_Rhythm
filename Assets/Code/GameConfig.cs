﻿using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameConfig", order = 1)]
public class GameConfig : ScriptableObject
{
    [Header("Gameplay and balance")]
    public float factoryLineSpeed;
    public float workerCooldownTime;
    public float workerActivateDistanceX;
    public float workerActivateDistanceY;
    
    [Header("Scoring")]
    public int scoreOnRaw;
    public int scoreOnFinished;
    public int scoreOnRuined;
    public BossFaceState[] bossFaceStates;

    [Header("Prefabs")]
    public GameObject itemPrefab;

    [Header("Graphics")]
    public Sprite spriteRaw;
    public Sprite spriteFinished;
    public Sprite spriteRuined;

    public Sprite GetSprite(ItemCondition condition)
    {
        switch (condition) {
            case ItemCondition.Raw: return spriteRaw;
            case ItemCondition.Finished: return spriteFinished;
            case ItemCondition.Ruined: return spriteRuined;
            default: return null;
        }
    }
}

[Serializable]
public class BossFaceState
{
    public Sprite bossFace;
    public int minThreshold;
}
