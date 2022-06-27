using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryLine : MonoBehaviour
{
    /// Factory Line
    /// Will move factory items across the line.
    /// Handles spawning new items on one end and despawning on the other.
    /// Keeps a list of all active (and later inactive - see pooling) items on that specific line.
    /// Destroys/Pools items that reach the end of the line and checks their status.

    [SerializeField]
    private Transform startPoint;
    [SerializeField]
    private GameObject itemPrefab;
    [SerializeField]
    private float lineSpeed = 1f;

    private List<GameObject> activeItems = new List<GameObject>();
    private List<GameObject> inactiveItems = new List<GameObject>();

    void Start () {
        activeItems.Add(GameObject.Instantiate(itemPrefab, startPoint.position, Quaternion.identity));
    }

    void Update () {
        MoveItems(Time.deltaTime);
        Respawn();
    }

    void MoveItems (float deltaTime) {
        if(activeItems.Count <= 0)
            return;
        foreach(GameObject item in activeItems) {
            item.transform.position += Vector3.right * lineSpeed * deltaTime;
        }
    }

    void Respawn () {
        if(inactiveItems.Count <= 0)
            return;
        foreach(GameObject item in inactiveItems) {
            activeItems.Add(item);
            item.SetActive(true);
            item.transform.position = startPoint.position;
        }
        inactiveItems.Clear();
    }

    void OnTriggerEnter2D (Collider2D col) {
        print("TRIGGER ENTERED");
        col.GetComponent<FactoryItem>().Reset();
        activeItems.Remove(col.gameObject);
        inactiveItems.Add(col.gameObject);
        col.gameObject.SetActive(false);
    }
}
