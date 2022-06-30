using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryWorker : MonoBehaviour
{
    //deps:
    private GameConfig config;
    [SerializeField] private GameObject hat;

    //gameplay status:
    public float cooldownUntil;
    public int verticalDirection;


    public void Init(GameConfig config)
    {
        this.config = config;
        verticalDirection = 1;
        SetRotation();
    }
    
    public void FlipVerticalDirection()
    {
        verticalDirection *= -1;
        SetRotation();
    }



    public void SetRotation()
    {
        transform.rotation = Quaternion.Euler(0, 0, 90 + verticalDirection * 90);
        hat.transform.rotation = Quaternion.Euler(0, 0, 0);
    }


    public void StartCooldown(float currentTime)
    {
        // transform.localScale = new Vector3(0.4f, 1f, 1f);
        cooldownUntil = currentTime + config.workerCooldownTime;
    }
    
    public void EndCooldown()
    {
        // transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public bool OnCooldown(float time)
    {
        return time < cooldownUntil;
    }

    public void StartWorkingOnItem()
    {
        GetComponent<Animator>().SetTrigger("StartWork");
    }
}
