using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryWorker : MonoBehaviour
{
    public float cooldownUntil;

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public bool OnCooldown(float time)
    {
        return time < cooldownUntil;
    }
}
