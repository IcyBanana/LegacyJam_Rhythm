using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FactoryLine : MonoBehaviour
{
    /// Factory Line
    /// Will move factory items across the line.
    /// Handles spawning new items on one end and despawning on the other.
    /// Keeps a list of all active (and later inactive - see pooling) items on that specific line.
    /// Destroys/Pools items that reach the end of the line and checks their status.

    // Events:
    public Action<FactoryItem> ScoreItemOnReachEnd;

    // Dependencies:
    private Transform startPoint;
    private Transform endPoint;
    
    // Status;
    private List<FactoryItem> activeItems = new List<FactoryItem>();
    private List<FactoryItem> inactiveItems = new List<FactoryItem>();
    private GameConfig config;

    public void Init(GameConfig config)
    {
        this.config = config;
    }

    private void Start()
    {
        startPoint = transform.GetComponentsInChildren<Transform>().First(x => x.name == "startPoint");
        endPoint = transform.GetComponentsInChildren<Transform>().First(x => x.name == "endPoint");
    }

    public void SpawnNewItem()
    {
        var itemGameObject = GameObject.Instantiate(config.itemPrefab, startPoint.position, Quaternion.identity);
        var item = itemGameObject.GetComponent<FactoryItem>();
        item.Init(config);
        activeItems.Add(item);
        
        //var color = new Color(1f, 1f, 1f, 1f);
        //color.r = UnityEngine.Random.Range(0f, 1f);
        //color.g = UnityEngine.Random.Range(0f, 1f);
        //color.b = UnityEngine.Random.Range(0f, 1f);
        //itemGameObject.GetComponent<SpriteRenderer>().color = color;
    }

    private void Update()
    {
        MoveItems(Time.deltaTime);
        // RespawnAll();

        var itemsToUpdate = activeItems.ToArray();
        foreach (var item in itemsToUpdate) {

            if (item.transform.position.x >= endPoint.position.x) {
                ScoreItemOnReachEnd.Invoke(item);
                DisableItem(item);
            }
        }
    }

    private void MoveItems(float deltaTime)
    {
        if (activeItems.Count <= 0)
            return;
        foreach (var item in activeItems) {
            item.transform.position += Vector3.right * config.factoryLineSpeed * deltaTime;
        }
    }

    public FactoryItem[] GetAllActiveItems()
    {
        return activeItems.ToArray();
    }

    private void RespawnAll()
    {
        if (inactiveItems.Count <= 0)
            return;

        var toEnable = inactiveItems.ToArray();
        foreach (var item in toEnable) {
            RespawnItem(item);
        }
    }

    private void RespawnItem(FactoryItem item)
    {
        activeItems.Add(item);
        inactiveItems.Remove(item);
        item.gameObject.SetActive(true);
        item.transform.position = startPoint.position;
    }
    
    private void DisableItem(FactoryItem item)
    {
        item.GetComponent<FactoryItem>().Reset();
        activeItems.Remove(item);
        inactiveItems.Add(item);
        item.gameObject.SetActive(false);
    }
}
