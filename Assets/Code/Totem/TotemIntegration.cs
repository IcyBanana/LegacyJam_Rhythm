using System;
using System.Collections.Generic;
using TotemEntities;
using TotemServices;
using UnityEngine;
using UnityEngine.Events;

public class TotemIntegration
{
    ///Id of your game
    ///Used for legacy records identification
    private string industrialGameId = "legacyjam22-industrial";
    private string legacyGameIdInput = "legacyjam22-renaissance";

    private string _accessToken;

    private TotemDB totemDB;
    private List<TotemAvatar> _userAvatars;
    private TotemAvatar firstAvatar;
    private string _publicKey;

    public Action OnLoginFailed;
    public Action<int> OnLoginSucceededAndLoadedIndustrial;
    public Action<int> OnLoginSucceededAndLoadedRenaissance;

    public void Init()
    {
        //Initialize TotemDB
        totemDB = new TotemDB(industrialGameId);

        //Subscribe to the events
        totemDB.OnSocialLoginCompleted.AddListener(OnTotemUserLoggedIn);
        totemDB.OnUserProfileLoaded.AddListener(OnUserProfileLoaded);
        totemDB.OnAvatarsLoaded.AddListener(OnAvatarsLoaded);
        totemDB.OnAvatarsLoaded.AddListener(OnAvatarsLoaded);
    }
    
    public void LoginUser()
    {
        //Authenticate user through social login in web browser
        totemDB.AuthenticateCurrentUser();
    }

    private void OnTotemUserLoggedIn(TotemAccountGateway.SocialLoginResponse loginResult)
    {
        _accessToken = loginResult.accessToken;
        totemDB.GetUserProfile(_accessToken);
    }

    private void OnUserProfileLoaded(string publicKey)
    {
        _publicKey = publicKey;
        totemDB.GetUserAvatars(_publicKey);
    }

    private void OnAvatarsLoaded(List<TotemAvatar> avatars)
    {
        _userAvatars = avatars;

        //Reference the first Avatar in the list
        firstAvatar = avatars[0];

        GetLastLegacyRecord(OnFetchLegacyEventIndustrial, industrialGameId);
        GetLastLegacyRecord(OnFetchLegacyEventRenaissance, legacyGameIdInput);
    }

    public void AddLegacyRecord(string data)
    {
        AddLegacyRecord(firstAvatar, data);
    }
    public void AddLegacyRecord(ITotemAsset asset, string data)
    {
        totemDB.AddLegacyRecord(asset, data, (record) =>
        {
            Debug.Log("New legacy record data:" + record.data);
        });
    }
    public void GetLegacyRecords(ITotemAsset asset, UnityAction<List<TotemLegacyRecord>> onSuccess, string gameId)
    {
        totemDB.GetLegacyRecords(asset, onSuccess, gameId);
    }
    public void GetLastLegacyRecord(UnityAction<TotemLegacyRecord> onSuccess, string gameId)
    {
        GetLegacyRecords(firstAvatar, (records) => {
            if (records.Count > 0) {
                onSuccess.Invoke(records[records.Count - 1]);
            } else {
                onSuccess.Invoke(null);
            }
        }, gameId);
    }
    
    private void OnFetchLegacyEventRenaissance(TotemLegacyRecord lastRecord)
    {
        Debug.Log("OnFetchLegacyEventRenaissance");
        int result = -1;

        if (lastRecord == null) {
            Debug.Log("No legacy events found");
        } else {
            Debug.Log(lastRecord.data);
            var record = Int32.TryParse(lastRecord.data, out result);
        }

        OnLoginSucceededAndLoadedRenaissance.Invoke(result);
    }

    private void OnFetchLegacyEventIndustrial(TotemLegacyRecord lastRecord)
    {
        Debug.Log("OnFetchLegacyEventIndustrial");
        int result = -1;

        if (lastRecord == null) {
            Debug.Log("No legacy events found");
        } else {
            Debug.Log(lastRecord.data);
            var record = Int32.TryParse(lastRecord.data, out result);
        }

        OnLoginSucceededAndLoadedIndustrial.Invoke(result);
    }
}
