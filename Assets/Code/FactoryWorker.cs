using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryWorker : MonoBehaviour
{
    public float cooldownUntil;
    
    private GameConfig config;

    public void Init(GameConfig config)
    {
        this.config = config;
    }
    
    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }


    public void StartCooldown(float currentTime)
    {
        transform.localScale = new Vector3(0.4f, 1f, 1f);
        cooldownUntil = currentTime + config.workerCooldownTime;
    }
    public void EndCooldown()
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public bool OnCooldown(float time)
    {
        return time < cooldownUntil;
    }
}
