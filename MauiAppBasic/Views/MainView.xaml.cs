// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using MAUI.MSALClient;
using Microsoft.Identity.Client;

namespace MauiAppBasic.Views
{
    public partial class MainView : ContentPage
    {
        public MainView()
        {
            InitializeComponent();

            // Initializes the Public Client app and loads any already signed in user from the token cache
            IAccount cachedUserAccount = Task.Run(async () => await PublicClientSingleton.Instance.MSALClientHelper.FetchSignedInUserFromCache()).Result;

            _ = Dispatcher.DispatchAsync(async () =>
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
            // Sign-in the user
            PublicClientSingleton.Instance.UseEmbedded = this.useEmbedded.IsChecked;

            try
            {
                await PublicClientSingleton.Instance.AcquireTokenSilentAsync();
            }
            catch (MsalClientException ex) when (ex.ErrorCode == MsalError.AuthenticationCanceledError)
            {
                await ShowMessage("Login failed", "User cancelled sign in."); 
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

        protected override bool OnBackButtonPressed()
        { return true; }
    }
}