﻿using System.Windows;
using Microsoft.Identity.Client;

namespace active_directory_b2c_wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static string Tenant = "fabrikamb2c.onmicrosoft.com";
        private static string ClientId = "841e1190-d73a-450c-9d68-f5cf16b78e81";
        public static string PolicySignUpSignIn = "b2c_1_susi";
        public static string PolicyEditProfile = "b2c_1_edit_profile";
        public static string PolicyResetPassword = "b2c_1_reset";

        public static string[] ApiScopes = { "https://fabrikamb2c.onmicrosoft.com/helloapi/demo.read" };
        public static string ApiEndpoint = "https://fabrikamb2chello.azurewebsites.net/hello";

        private static string BaseAuthority = "https://fabrikamb2c.b2clogin.com/tfp/{tenant}/{policy}/";
        public static string Authority = BaseAuthority.Replace("{tenant}", Tenant).Replace("{policy}", PolicySignUpSignIn);
        public static string AuthorityEditProfile = BaseAuthority.Replace("{tenant}", Tenant).Replace("{policy}", PolicyEditProfile);
        public static string AuthorityResetPassword = BaseAuthority.Replace("{tenant}", Tenant).Replace("{policy}", PolicyResetPassword);

        public static IPublicClientApplication PublicClientApp { get; private set; }

        static App()
        {
            PublicClientApp = PublicClientApplicationBuilder.Create(ClientId)
                .WithB2CAuthority(Authority)
                .WithDefaultRedirectUri()
                .Build();

            TokenCacheHelper.Bind(PublicClientApp.UserTokenCache);
        }
    }
}