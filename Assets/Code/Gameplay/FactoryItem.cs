using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryItem : MonoBehaviour
{
    /// An item on the factory line.
    /// Contains a state: Finished or Unfinished
    /// Conditions: Must be "worked" on a specific number of times 'n'. If worked on less or more than 'n', will be Unfinished, otherwise Finished.
    /// Can only be "worked" on when inside a worker's trigger on the line.

    [SerializeField]
    private ItemCondition myCondition;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private GameConfig config;

    //
    // Public Methods
    // 

    public void Init(GameConfig config)
    {
        this.config = config;
        spriteRenderer = GetComponent<SpriteRenderer>();
        Reset();
        
        UpdateItemGraphics();
    }
    
    public ItemCondition GetCondition()
    {
        return myCondition;
    }

    public void Reset()
    {
        myCondition = ItemCondition.Raw;
    }

    public void AdvanceCondition()
    {
        if (myCondition == ItemCondition.Raw)
        {
            myCondition = ItemCondition.Finished;
            // spriteRenderer.color = Color.green;
        }
        else if (myCondition == ItemCondition.Finished)
        {
            myCondition = ItemCondition.Ruined;
            // spriteRenderer.color = Color.red;
        }
        UpdateItemGraphics();
    }

    private void UpdateItemGraphics()
    {
        spriteRenderer.sprite = config.GetSprite(myCondition);
    }
}
