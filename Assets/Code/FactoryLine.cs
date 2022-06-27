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

    [SerializeField]
    private GameObject itemPrefab;
    [SerializeField]
    private float lineSpeed = 1f;

    private Transform startPoint;
    private Transform endPoint;
    
    private List<GameObject> activeItems = new List<GameObject>();
    private List<GameObject> inactiveItems = new List<GameObject>();

    private void Start()
    {
        startPoint = transform.GetComponentsInChildren<Transform>().First(x => x.name == "startPoint");
        endPoint = transform.GetComponentsInChildren<Transform>().First(x => x.name == "endPoint");
    }

    public void SpawnNewItem()
    {
        activeItems.Add(GameObject.Instantiate(itemPrefab, startPoint.position, Quaternion.identity));
    }

    private void Update()
    {
        MoveItems(Time.deltaTime);
        // RespawnAll();

        var itemsToUpdate = activeItems.ToArray();
        foreach (var item in itemsToUpdate) {

            if (item.transform.position.x >= endPoint.position.x) {
                DisableItem(item);
            }
        }
    }

    private void MoveItems(float deltaTime)
    {
        if (activeItems.Count <= 0)
            return;
        foreach (GameObject item in activeItems) {
            item.transform.position += Vector3.right * lineSpeed * deltaTime;
        }
    }

    public FactoryItem[] GetAllActiveItems()
    {
        return activeItems.Select(x => x.GetComponent<FactoryItem>()).ToArray();
    }

    private void RespawnAll()
    {
        if (inactiveItems.Count <= 0)
            return;

        var toEnable = inactiveItems.ToArray();
        foreach (GameObject item in toEnable) {
            RespawnItem(item);
        }
    }

    private void RespawnItem(GameObject item)
    {
        activeItems.Add(item);
        inactiveItems.Remove(item);
        item.SetActive(true);
        item.transform.position = startPoint.position;
    }
    
    private void DisableItem(GameObject item)
    {
        item.GetComponent<FactoryItem>().Reset();
        activeItems.Remove(item);
        inactiveItems.Add(item);
        item.SetActive(false);
    }
}
