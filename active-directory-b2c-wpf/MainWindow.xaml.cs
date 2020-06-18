// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Newtonsoft.Json.Linq;
using System.Text;

namespace active_directory_b2c_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            AuthenticationResult authResult = null;
            var app = App.PublicClientApp;
            try
            {
                ResultText.Text = "";
                authResult = await app.AcquireTokenInteractive(App.ApiScopes)
                    .WithParentActivityOrWindow(new WindowInteropHelper(this).Handle)
                    .ExecuteAsync();

                DisplayUserInfo(authResult);
                UpdateSignInState(true);
            }
            catch (MsalException ex)
            {
                try
                {
                    if (ex.Message.Contains("AADB2C90118"))
                    {
                        authResult = await app.AcquireTokenInteractive(App.ApiScopes)
                            .WithParentActivityOrWindow(new WindowInteropHelper(this).Handle)
                            .WithPrompt(Prompt.SelectAccount)
                            .WithB2CAuthority(App.AuthorityResetPassword)
                            .ExecuteAsync();
                    }
                    else
                    {
                        ResultText.Text = $"Error Acquiring Token:{Environment.NewLine}{ex}";
                    }
                }
                catch (Exception exe)
                {
                    ResultText.Text = $"Error Acquiring Token:{Environment.NewLine}{exe}";
                }
            }
            catch (Exception ex)
            {
                ResultText.Text = $"Error Acquiring Token:{Environment.NewLine}{ex}";
            }

            DisplayUserInfo(authResult);
        }

        private async void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var app = App.PublicClientApp;
            try
            {
                ResultText.Text = $"Calling API:{App.AuthorityEditProfile}";

                AuthenticationResult authResult = await app.AcquireTokenInteractive(App.ApiScopes)
                            .WithParentActivityOrWindow(new WindowInteropHelper(this).Handle)
                            .WithB2CAuthority(App.AuthorityEditProfile)
                            .WithPrompt(Prompt.NoPrompt) 
                            .ExecuteAsync(new System.Threading.CancellationToken());

                DisplayUserInfo(authResult);
            }
            catch (Exception ex)
            {
                ResultText.Text = $"Session has expired, please sign out and back in.{App.AuthorityEditProfile}{Environment.NewLine}{ex}";
            }
        }

        private async void CallApiButton_Click(object sender, RoutedEventArgs e)
        {
            AuthenticationResult authResult = null;
            var app = App.PublicClientApp;
            var accounts = await app.GetAccountsAsync(App.PolicySignUpSignIn);
            try
            {
                authResult = await app.AcquireTokenSilent(App.ApiScopes, accounts.FirstOrDefault())
                    .ExecuteAsync();
            }
            catch (MsalUiRequiredException ex)
            {
                // A MsalUiRequiredException happened on AcquireTokenSilentAsync. 
                // This indicates you need to call AcquireTokenAsync to acquire a token
                Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");

                try
                {
                    authResult = await app.AcquireTokenInteractive(App.ApiScopes)
                        .WithParentActivityOrWindow(new WindowInteropHelper(this).Handle)
                        .ExecuteAsync();
                }
                catch (MsalException msalex)
                {
                    ResultText.Text = $"Error Acquiring Token:{Environment.NewLine}{msalex}";
                }
            }
            catch (Exception ex)
            {
                ResultText.Text = $"Error Acquiring Token Silently:{Environment.NewLine}{ex}";
                return;
            }

            if (authResult != null)
            {
                if (string.IsNullOrEmpty(authResult.AccessToken))
                {
                    ResultText.Text = "Access token is null (could be expired). Please do interactive log-in again." ;
                }
                else
                {
                    ResultText.Text = await GetHttpContentWithToken(App.ApiEndpoint, authResult.AccessToken);
                    DisplayUserInfo(authResult);
                }
            }
        }

        /// <summary>
        /// Perform an HTTP GET request to a URL using an HTTP Authorization header
        /// </summary>
        /// <param name="url">The URL</param>
        /// <param name="token">The token</param>
        /// <returns>String containing the results of the GET operation</returns>
        public async Task<string> GetHttpContentWithToken(string url, string token)
        {
            var httpClient = new HttpClient();
            HttpResponseMessage response;
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        private async void SignOutButton_Click(object sender, RoutedEventArgs e)
        {
            // SingOut will remove tokens from the token cache from ALL accounts, irrespective of user flow
            IEnumerable<IAccount> accounts = await App.PublicClientApp.GetAccountsAsync();
            try
            {
                while (accounts.Any())
                {
                    await App.PublicClientApp.RemoveAsync(accounts.FirstOrDefault());
                    accounts = await App.PublicClientApp.GetAccountsAsync();
                }

                UpdateSignInState(false);
            }
            catch (MsalException ex)
            {
                ResultText.Text = $"Error signing-out user: {ex.Message}";
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var app = App.PublicClientApp;
                var accounts = await app.GetAccountsAsync(App.PolicySignUpSignIn);

                AuthenticationResult authResult = await app.AcquireTokenSilent(App.ApiScopes, accounts.FirstOrDefault())
                    .ExecuteAsync();

                DisplayUserInfo(authResult);
                UpdateSignInState(true);
            }
            catch (MsalUiRequiredException)
            {
                // Ignore, user will need to sign in interactively.
                ResultText.Text = "You need to sign-in first, and then Call API";
            }
            catch (Exception ex)
            {
                ResultText.Text = $"Error Acquiring Token Silently:{Environment.NewLine}{ex}";
            }
        }
        
        private void UpdateSignInState(bool signedIn)
        {
            if (signedIn)
            {
                CallApiButton.Visibility = Visibility.Visible;
                EditProfileButton.Visibility = Visibility.Visible;
                SignOutButton.Visibility = Visibility.Visible;

                SignInButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                ResultText.Text = "";
                TokenInfoText.Text = "";

                CallApiButton.Visibility = Visibility.Collapsed;
                EditProfileButton.Visibility = Visibility.Collapsed;
                SignOutButton.Visibility = Visibility.Collapsed;

                SignInButton.Visibility = Visibility.Visible;
            }
        }

        private void DisplayUserInfo(AuthenticationResult authResult)
        {
            if (authResult != null)
            {
                JObject user = ParseIdToken(authResult.IdToken);

                TokenInfoText.Text = "";
                TokenInfoText.Text += $"Name: {user["name"]?.ToString()}" + Environment.NewLine;
                TokenInfoText.Text += $"User Identifier: {user["oid"]?.ToString()}" + Environment.NewLine;
                TokenInfoText.Text += $"Street Address: {user["streetAddress"]?.ToString()}" + Environment.NewLine;
                TokenInfoText.Text += $"City: {user["city"]?.ToString()}" + Environment.NewLine;
                TokenInfoText.Text += $"State: {user["state"]?.ToString()}" + Environment.NewLine;
                TokenInfoText.Text += $"Country: {user["country"]?.ToString()}" + Environment.NewLine;
                TokenInfoText.Text += $"Job Title: {user["jobTitle"]?.ToString()}" + Environment.NewLine;

                if (user["emails"] is JArray emails)
                {
                    TokenInfoText.Text += $"Emails: {emails[0].ToString()}" + Environment.NewLine;
                }
                TokenInfoText.Text += $"Identity Provider: {user["iss"]?.ToString()}" + Environment.NewLine;
            }
        }

        JObject ParseIdToken(string idToken)
        {
            // Parse the idToken to get user info
            idToken = idToken.Split('.')[1];
            idToken = Base64UrlDecode(idToken);
            return JObject.Parse(idToken);
        }

        private string Base64UrlDecode(string s)
        {
            s = s.Replace('-', '+').Replace('_', '/');
            s = s.PadRight(s.Length + (4 - s.Length % 4) % 4, '=');
            var byteArray = Convert.FromBase64String(s);
            var decoded = Encoding.UTF8.GetString(byteArray, 0, byteArray.Count());
            return decoded;
        }
    }
}
