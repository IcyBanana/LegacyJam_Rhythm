using System;
using UnityEngine;

[Serializable]
public struct PlaybackEvent : ISerializationCallbackReceiver
{
    [SerializeField] public float time;
    [SerializeField] public PlaybackEventType type;
    [SerializeField] public int arg1;

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        if (type == PlaybackEventType.None) {
            type = PlaybackEventType.SpawnInLine;
        }
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize() { }
}

public enum PlaybackEventType
{
    None,

    SpawnInLine,
}
