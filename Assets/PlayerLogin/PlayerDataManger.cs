using UnityEngine;
using Unity.Services.CloudCode.GeneratedBindings;
using Unity.Services.CloudCode;
using System;
using System.Threading.Tasks;
using System.Linq;

public class PlayerDataManger : MonoBehaviour
{
    private PlayerDataServiceBindings m_Bindings;
    public LoginManager loginManager;
    public string PlayerName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        loginManager.PlayerSignedIn += InitializePlayer;
        m_Bindings = new PlayerDataServiceBindings(CloudCodeService.Instance);
    }

    private async void InitializePlayer()
    {
        try
        {
            var resultFromCloud = await m_Bindings.SayHello(PlayerName);
            Debug.Log($"{resultFromCloud}");
        }

        catch (CloudCodeException ex)
        {
            Debug.LogException(ex);
        }
    }

    public async void SaveNewPlayerName()
    {
        try
        {
            PlayerName = await m_Bindings.HandleNewPlayerNameEntry(PlayerName);
            Debug.Log($"save new player name to cloud: {PlayerName}");
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }
    private bool IsPlayerNameValid(string name)
    {
        if(name.Length is < 4 or > 16)
        {
            return false;
        }
        if (!name.All(c => char.IsLetterOrDigit(c)))
        {
            return false;
        }
        return true;
    }

    private void OnDisable()
    {
        loginManager.PlayerSignedIn -= InitializePlayer;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

