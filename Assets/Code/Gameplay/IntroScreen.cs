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
    public static int renaissanceLegacyEventValue;
    
    private bool startedLoginProcess;

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
        totemIntegration.OnLoginSucceededAndLoaded += OnLogin;
        totemIntegration.OnLoginFailed += OnLoginFail;
        totemIntegration.LoginUser();
    }

    private void OnLogin(int val)
    {
        totemLoginSuccessful = true;
        renaissanceLegacyEventValue = val;
        SceneManager.LoadScene("Game");
    }

    public void OnLoginFail()
    {
        startedLoginProcess = false;
    }
}
