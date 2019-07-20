//------------------------------------------------------------------------------
//
// Copyright (c) Microsoft Corporation.
// All rights reserved.
//
// This code is licensed under the MIT License.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//------------------------------------------------------------------------------

using System.Windows;
using Microsoft.Identity.Client;

namespace active_directory_b2c_wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly string Tenant = "b2cDemo2019Tenant.onmicrosoft.com";
        public static readonly string ClientId = "f3557bf1-2d23-4537-a79f-8569c1876fb2"; //"841e1190-d73a-450c-9d68-f5cf16b78e81";
        public static string PolicySignUpSignIn = "B2C_1_signup_signin";
        public static string PolicyEditProfile = "B2C_1_edit_profile";
        public static string PolicyResetPassword = "B2C_1_password_reset";
        public static string PolicyROPC = "B2C_1_ropc";

        public static string[] ApiScopes = { "https://b2cDemo2019Tenant.onmicrosoft.com/api/hello.read" };
        public static string ApiEndpoint = "https://b2cDemo2019Tenant.onmicrosoft.com/api";

        private static string BaseAuthority = "https://b2cDemo2019Tenant.b2clogin.com/tfp/{tenant}/{policy}/";
        public static string Authority = BaseAuthority.Replace("{tenant}", Tenant).Replace("{policy}", PolicySignUpSignIn);
        public static string AuthorityEditProfile = BaseAuthority.Replace("{tenant}", Tenant).Replace("{policy}", PolicyEditProfile);
        public static string AuthorityResetPassword = BaseAuthority.Replace("{tenant}", Tenant).Replace("{policy}", PolicyResetPassword);
        public static string AuthorityROPCPassword = BaseAuthority.Replace("{tenant}", Tenant).Replace("{policy}", PolicyROPC);

        public static IPublicClientApplication PublicClientApp { get; private set; }

        static App()
        {
            PublicClientApp = PublicClientApplicationBuilder.Create(ClientId)
                .WithB2CAuthority(Authority)
                .Build();

            TokenCacheHelper.Bind(PublicClientApp.UserTokenCache);
        }
    }
}