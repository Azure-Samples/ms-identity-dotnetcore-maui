// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using MAUI.MSALClient;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;

namespace MauiAppWithBroker.Views
{
    public partial class MainView : ContentPage
    {
        private MSALClientHelper MSALClientHelper;

        public MainView()
        {
            InitializeComponent();

            // Initializes the Public Client app and loads any already signed in user from the token cache
            IAccount cachedUserAccount = Task.Run(async () => await PublicClientSingleton.Instance.MSALClientHelper.FetchSignedInUserFromCache()).Result;

            _ = Application.Current.Dispatcher.Dispatch(async () =>
            {
                if (cachedUserAccount == null)
                {
                    SignInButton.IsEnabled = true;
                }
                else
                {
                    await Shell.Current.GoToAsync("userview");
                }
            });
  
        }

        private async void OnSignInClicked(object sender, EventArgs e)
        {
            try
            {
                await PublicClientSingleton.Instance.AcquireTokenSilentAsync();
            }
            catch (MsalClientException ex) when (ex.ErrorCode == MsalError.AuthenticationCanceledError)
            {
                await ShowMessage("Login failed", "User cancelled sign in."); 
                return;
            }
            catch (MsalServiceException ex) when (ex.Message.Contains("AADSTS65004"))
            {
                await ShowMessage("Login failed", "User did not consent to app requirements."); 
                return;
            }


            await Shell.Current.GoToAsync("userview");
        }

        private async Task ShowMessage(string title, string message)
        {
            _ = this.Dispatcher.Dispatch(async () =>
            {
                await DisplayAlert(title, message, "OK").ConfigureAwait(false);
            });
        }

        protected override bool OnBackButtonPressed() { return true; }

    }
}
