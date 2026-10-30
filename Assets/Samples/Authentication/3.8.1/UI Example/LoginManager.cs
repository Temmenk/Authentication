using System.Threading.Tasks;
using NUnit.Framework;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using Unity.VisualScripting;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    private async void Awake()
    {
        if(UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            Debug.Log("services initializing");
            await UnityServices.InitializeAsync();
        }
        PlayerAccountService.Instance.SignedIn += SignInOrLinkWithUnity;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public async void StartAnonymousSignIn()
    {
        await SignUpAnonymouslyAsync();
    }
    private async Task SignUpAnonymouslyAsync()
{
    try
    {
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        Debug.Log("Sign in anonymously succeeded!");

        // Shows how to get the playerID
        Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

    }
    catch (AuthenticationException ex)
    {
        // Compare error code to AuthenticationErrorCodes
        // Notify the player with the proper error message
        Debug.LogException(ex);
    }
    catch (RequestFailedException ex)
    {
        // Compare error code to CommonErrorCodes
        // Notify the player with the proper error message
        Debug.LogException(ex);
     }
}
 public async void StartUnitySignInAsync()
        {
            if (PlayerAccountService.Instance.IsSignedIn)
            {
                SignInOrLinkWithUnity();
                return;
            }

            try
            {
                await PlayerAccountService.Instance.StartSignInAsync();
            }
            catch (RequestFailedException ex)
            {
                Debug.LogException(ex);
                
            }
        }
        private async void SignInOrLinkWithUnity()
        {
            try
            {
                if(!AuthenticationService.Instance.IsSignedIn)
            {
                Debug.Log("signed up with unity player account...");
                await AuthenticationService.Instance.SignInWithUnityAsync(PlayerAccountService.Instance.AccessToken);
                Debug.Log("successfully signed up eith unity player account");
                return;
            }

            if(!HasUnityID())
            {
                Debug.Log("Linking anonymous account to unity...");
                await LinkWithUnityAsync(PlayerAccountService.Instance.AccessToken);
                Debug.Log("successfully linked anonymous accout!");
                return;
            }

            Debug.Log("player is alread signed in to their unity player account");
            }
            catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
        }
        private bool HasUnityID()
    {
        return AuthenticationService.Instance.PlayerInfo.GetUnityId() != null;
    }
private async Task LinkWithUnityAsync(string accessToken)
        {
            try
            {
                await AuthenticationService.Instance.LinkWithUnityAsync(accessToken);
                Debug.Log("Link is successful.");
            }
            catch (AuthenticationException ex) when (ex.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
            {
                // Prompt the player with an error message.
                Debug.LogError("This user is already linked with another account. Log in instead.");
            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
        }

}
