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
    private string _gameId = "legacyjam22-industrial";
    private string legacyGameIdInput = "legacyjam22-renaissance";

    private string _accessToken;

    private TotemDB totemDB;
    private List<TotemAvatar> _userAvatars;
    private TotemAvatar firstAvatar;
    private string _publicKey;

    internal void Init()
    {
        //Initialize TotemDB
        totemDB = new TotemDB(_gameId);

        //Subscribe to the events
        totemDB.OnSocialLoginCompleted.AddListener(OnTotemUserLoggedIn);
        totemDB.OnUserProfileLoaded.AddListener(OnUserProfileLoaded);
        totemDB.OnAvatarsLoaded.AddListener(OnAvatarsLoaded);
        //legacyGameIdInput.onEndEdit.AddListener(OnGameIdInputEndEdit);

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

        GetLastLegacyRecord(OnFetchLegacyEvent);
    }

    public void AddLegacyRecord(ITotemAsset asset, string data)
    {
        totemDB.AddLegacyRecord(asset, data, (record) =>
        {
            Debug.Log("New legacy record data:" + record.data);
        });
    }
    public void GetLegacyRecords(ITotemAsset asset, UnityAction<List<TotemLegacyRecord>> onSuccess)
    {
        totemDB.GetLegacyRecords(asset, onSuccess, legacyGameIdInput);
    }
    public void GetLastLegacyRecord(UnityAction<TotemLegacyRecord> onSuccess)
    {
        GetLegacyRecords(firstAvatar, (records) => { onSuccess.Invoke(records[records.Count - 1]); });
    }
    private void OnFetchLegacyEvent(TotemLegacyRecord lastRecord)
    {
        Debug.Log("OnFetchLegacyEvent");
        Debug.Log(lastRecord.ToString());
    }


}