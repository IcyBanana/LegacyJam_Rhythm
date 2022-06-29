using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOffsetAnim : MonoBehaviour
{
    public float mult;
    private GameConfig myConfig;
    private Material myMat;
    void Start () {
        myConfig = GameObject.Find("SyncGame").GetComponent<SyncGame>().gameConfig;
        myMat = GetComponent<SpriteRenderer>().material;
    }
    void Update()
    {
        myMat.SetTextureOffset("_MainTex", new Vector2(Time.time * myConfig.factoryLineSpeed * mult, 0f));
    }
}
