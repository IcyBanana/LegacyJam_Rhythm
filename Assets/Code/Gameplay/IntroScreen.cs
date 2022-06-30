using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TotemEntities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class IntroScreen : MonoBehaviour
{
    public static TotemIntegration totemIntegration;
    public static bool totemLoginSuccessful;
    public static int industrialLegacyEventValue;
    public static int renaissanceLegacyEventValue;
    
    private bool startedLoginProcess;
    private int loadedVars = 0;

    public IntroScreen()
    {
        totemLoginSuccessful = false;
    }

    public void Login()
    {
        if (startedLoginProcess) {
            return;
        }
        startedLoginProcess = true;

        totemIntegration = new TotemIntegration();
        totemIntegration.Init();
        totemIntegration.OnLoginSucceededAndLoadedIndustrial += OnGotValueIndustrial;
        totemIntegration.OnLoginSucceededAndLoadedRenaissance += OnGotValueRenaissance;
        totemIntegration.OnLoginFailed += OnLoginFail;
        totemIntegration.LoginUser();
    }

    private void OnGotValueIndustrial(int val)
    {
        totemLoginSuccessful = true;
        industrialLegacyEventValue = val;
        SceneManager.LoadScene("Game");

        loadedVars++;

        if (loadedVars == 2) {
            loadedVars = 999;
            SceneManager.LoadScene("Game");
        }
    }

    private void OnGotValueRenaissance(int val)
    {
        totemLoginSuccessful = true;
        renaissanceLegacyEventValue = val;
        
        loadedVars++;

        if (loadedVars == 2) {
            loadedVars = 999;
            SceneManager.LoadScene("Game");
        }
    }

    public void OnLoginFail()
    {
        startedLoginProcess = false;
    }
}
