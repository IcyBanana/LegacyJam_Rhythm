using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameConfig", order = 1)]
public class GameConfig : ScriptableObject
{
    public float workerCooldownTime;
    public float workerActivateDistanceX;
    public float workerActivateDistanceY;
}