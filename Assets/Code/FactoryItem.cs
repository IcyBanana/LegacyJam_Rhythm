using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryItem : MonoBehaviour
{
    /// An item on the factory line.
    /// Contains a state: Finished or Unfinished
    /// Conditions: Must be "worked" on a specific number of times 'n'. If worked on less or more than 'n', will be Unfinished, otherwise Finished.
    /// Can only be "worked" on when inside a worker's trigger on the line.

    public enum ItemCondition {
        Finished,
        Unfinished
    }

    [SerializeField]
    private ItemCondition myCondition;

    [SerializeField]
    private int requiredTaps = 1;
    [SerializeField]
    private int tapCount = 0; // How many times this item has been worked on.
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    
    //
    // Public Methods
    // 
    public void Tap () {
        tapCount++;
        UpdateCondition();
    }

    public ItemCondition GetCondition () {
        return myCondition;
    }

    public void Reset () {    
        tapCount = 0;
    }

    //
    // Private Methods
    //
    void Start () {
        UpdateCondition();
    }

    void UpdateCondition () {
        if(tapCount == requiredTaps)  
            myCondition = ItemCondition.Finished;
        else
            myCondition = ItemCondition.Unfinished;
    }
}
