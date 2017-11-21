﻿using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            try
            {
                authResult = await App.PublicClientApp.AcquireTokenAsync(App.ApiScopes, GetUserByPolicy(App.PublicClientApp.Users, App.PolicySignUpSignIn), UIBehavior.SelectAccount, string.Empty, null, App.Authority);
                DisplayBasicTokenInfo(authResult);
                UpdateSignInState(true);
            }
            catch (MsalServiceException ex)
            {
                try
                {
                    if (ex.Message.Contains("AADB2C90118"))
                    {
                        authResult = await App.PublicClientApp.AcquireTokenAsync(App.ApiScopes, GetUserByPolicy(App.PublicClientApp.Users, App.PolicySignUpSignIn), UIBehavior.SelectAccount, string.Empty, null, App.AuthorityResetPassword);
                    }
                    else
                    {
                        ResultText.Text = $"Error Acquiring Token:{Environment.NewLine}{ex}";
                    }
                }
                catch (Exception)
                {
                }
            }
            catch (Exception ex)
            {
                ResultText.Text = $"Users:{string.Join(",",App.PublicClientApp.Users.Select(u => u.Identifier))}{Environment.NewLine}Error Acquiring Token:{Environment.NewLine}{ex}";
            }
        }
        
        private async void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ResultText.Text = $"Calling API:{App.AuthorityEditProfile}";
                AuthenticationResult authResult = await App.PublicClientApp.AcquireTokenAsync(App.ApiScopes, GetUserByPolicy(App.PublicClientApp.Users, App.PolicyEditProfile), UIBehavior.SelectAccount, string.Empty, null, App.AuthorityEditProfile);
                DisplayBasicTokenInfo(authResult);
            }
            catch (Exception ex)
            {
                ResultText.Text = $"{App.AuthorityEditProfile}{Environment.NewLine}{ex}"; 
            }
        }

        private async void CallApiButton_Click(object sender, RoutedEventArgs e)
        {
            AuthenticationResult authResult = null;
            try
            {
                authResult = await App.PublicClientApp.AcquireTokenSilentAsync(App.ApiScopes, GetUserByPolicy(App.PublicClientApp.Users, App.PolicySignUpSignIn), App.Authority, false);
            }
            catch (MsalUiRequiredException ex)
            {
                // A MsalUiRequiredException happened on AcquireTokenSilentAsync. This indicates you need to call AcquireTokenAsync to acquire a token
                Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");

                try
                {
                    authResult = await App.PublicClientApp.AcquireTokenAsync(App.ApiScopes, GetUserByPolicy(App.PublicClientApp.Users, App.PolicySignUpSignIn));
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
                ResultText.Text = await GetHttpContentWithToken(App.ApiEndpoint, authResult.AccessToken);
                DisplayBasicTokenInfo(authResult);
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
            catch (HttpRequestException ex)
            {
                return "Unable to call API. Verify the service is running at " + App.ApiEndpoint +
                    "\nException message: " + ex.InnerException.Message;
            }            
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        
        private void SignOutButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.PublicClientApp.Users.Any())
            {
                try
                {
                    foreach (var user in App.PublicClientApp.Users)
                    {
                        App.PublicClientApp.Remove(user);
                    }
                    UpdateSignInState(false);
                }
                catch (MsalException ex)
                {
                    ResultText.Text = $"Error signing-out user: {ex.Message}";
                }
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
        
        private void DisplayBasicTokenInfo(AuthenticationResult authResult)
        {
            TokenInfoText.Text = "";
            if (authResult != null)
            {
                TokenInfoText.Text += $"Name: {authResult.User.Name}" + Environment.NewLine;
                TokenInfoText.Text += $"Token Expires: {authResult.ExpiresOn.ToLocalTime()}" + Environment.NewLine;
                TokenInfoText.Text += $"Access Token: {authResult.AccessToken}" + Environment.NewLine;
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                AuthenticationResult authResult = await App.PublicClientApp.AcquireTokenSilentAsync(App.ApiScopes, GetUserByPolicy(App.PublicClientApp.Users, App.PolicySignUpSignIn), App.Authority, true);
                DisplayBasicTokenInfo(authResult);
                UpdateSignInState(true);
            }
            catch (MsalUiRequiredException ex)
            {
                // Ignore, user will need to sign in interactively.
                ResultText.Text = "You need to sign-in first";

                //Un-comment for debugging purposes
                //ResultText.Text = $"Error Acquiring Token Silently:{Environment.NewLine}{ex}";
            }
            catch (Exception ex)
            {
                ResultText.Text = $"Error Acquiring Token Silently:{Environment.NewLine}{ex}";
            }
        }

        private IUser GetUserByPolicy(IEnumerable<IUser> users, string policy)
        {
            foreach (var user in users)
            {
                string userIdentifier = Base64UrlDecode(user.Identifier.Split('.')[0]);
                if (userIdentifier.EndsWith(policy.ToLower())) return user;
            }

            return null;
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
