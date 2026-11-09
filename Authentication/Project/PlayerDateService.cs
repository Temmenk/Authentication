using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudCode.Shared;
using Unity.Services.CloudSave.Model;

namespace Authentication;


public class PlayerDateService
{
    public const string k_PlayerNameKey = "Player_Name";
    private readonly ILogger<PlayerDateService> _logger;

    public PlayerDateService(ILogger<PlayerDateService> logger)
    {
        _logger = logger;
    }

    
    private async Task SaveGameData(IExecutionContext context, IGameApiClient gameApiClient, string customId, string key, string value)
    {
        try
        {
            await gameApiClient.CloudSaveData.SetCustomItemAsync(
                context,
                context.ServiceToken,
                context.ProjectId!,
                customId,
                new SetItemBody(key, value));

            _logger.LogInformation("Successfully saved game data for key: {Key}", key);
        }
        catch (ApiException ex)
        {
            _logger.LogError("Failed to save game data for key {Key}. Error: {Error}", key, ex.Message);
            throw new Exception($"Unable to save game data: {ex.Message}");
        }
    }

    
    private async Task<string> GetGameData(IExecutionContext context, IGameApiClient gameApiClient, string customId, string key)
    {
        try
        {
            var result = await gameApiClient.CloudSaveData.GetCustomItemsAsync(
                context,
                context.ServiceToken,
                context.ProjectId!,
                customId,
                new List<string> { key });

            var data = result.Data.Results.FirstOrDefault()?.Value?.ToString() ?? string.Empty;
            _logger.LogInformation("Successfully retrieved game data for key: {Key}", key);
            return data;
        }
        catch (ApiException ex)
        {
            _logger.LogError("Failed to retrieve game data for key {Key}. Error: {Error}", key, ex.Message);
            throw new Exception($"Unable to retrieve game data: {ex.Message}");
        }
    }

    
   
    readonly IGameApiClient m_GameApiClient;
    readonly ILogger<PlayerDateService> m_Logger;

    public PlayerDateService(IGameApiClient gameApiClient, ILogger<PlayerDateService> logger)
    {
        m_GameApiClient = gameApiClient;
        m_Logger = logger;
    }

    [CloudCodeFunction("SayHello")]
    public string Hello(string name)
    {
        return $"Hello, {name}!";
    }
    [CloudCodeFunction("HandleNewPlayerNameEntry")]
    public async Task<string> HandleNewPlayerNameEntry(string newName)
    {
        if(IsPlayerNameValid(newName))
        {
            await SaveGameData(context, gameApiClient, k_PlayerNameKey, newName);
            return newName;
        }
        throw new ArgumentException("not valid name hommie!");
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

    [CloudCodeFunction("GetServerTime")]
    public string GetServerTime(IExecutionContext context)
    {
        return DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
    }
}


