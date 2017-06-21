---
services: active-directory-b2c
platforms: dotnet
author: jmprieur
---

# WPF application signing in users with Azure Active Directory B2C and calling an API

This simple sample demonstrates how to use the [Microsoft Authentication Library (MSAL) for .NET](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet) to get an access token and call an API secured by Azure AD B2C.

## How To Run This Sample

To run this sample you will need:
- Visual Studio 2017
- An Internet connection
- At least one of the following accounts:
- An Azure AD B2C tenant

If you don't have an Azure AD B2C tenant, you can follow [those instructions](https://azure.microsoft.com/documentation/articles/active-directory-b2c-get-started/) to create one. 
If you just want to see the sample in action, you don't need to create your own tenant as the project comes with some settings associated to a test tenant and application; however it is highly recommend that you register your own app and experience going through the configuration steps below.   

### Step 1: Clone or download this repository

From your shell or command line:

```powershell
git clone https://github.com/Azure-Samples/active-directory-b2c-wpf.git
```

### [OPTIONAL] Step 2: Get your own Azure AD B2C tenant

You can also modify the sample to use your own Azure AD B2C tenant.  First, you'll need to create an Azure AD B2C tenant by following [these instructions](https://azure.microsoft.com/documentation/articles/active-directory-b2c-get-started).

> *IMPORTANT*: if you choose to perform one of the optional steps, you have to perform ALL of them for the sample to work as expected.

### [OPTIONAL] Step 3: Create your own policies

This sample uses three types of policies: a unified sign-up/sign-in policy & a profile editing policy.  Create one policy of each type by following [the instructions here](https://azure.microsoft.com/documentation/articles/active-directory-b2c-reference-policies).  You may choose to include as many or as few identity providers as you wish.

If you already have existing policies in your Azure AD B2C tenant, feel free to re-use those.  No need to create new ones just for this sample.

### [OPTIONAL] Step 4: Create your own Web API

This sample calls an API at https://fabrikamb2chello.azurewebsites.net which has the same code as the sample [Node.js Web API with Azure AD B2C](https://github.com/Azure-Samples/active-directory-b2c-javascript-nodejs-webapi). You'll need your own API or at the very least, you'll need to [register a Web API with Azure AD B2C](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-app-registration#register-a-web-api) so that you can define the scopes that your single page application will request access tokens for. 

Your web API registration should include the following information:

- Enable the **Web App/Web API** setting for your application.
- Set the **Reply URL** to the appropriate value indicated in the sample or provide any URL if you're only doing the web api registration, for example `https://myapi`.
- Make sure you also provide a **AppID URI**, for example `demoapi`, this is used to construct the scopes that are configured in you single page application's code.
- (Optional) Once you're app is created, open the app's **Published Scopes** blade and add any extra scopes you want.
- Copy the **AppID URI** and **Published Scopes values**, so you can input them in your application's code.

### [OPTIONAL] Step 5: Create your own Native app

Now you need to [register your native app in your B2C tenant](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-app-registration#register-a-mobilenative-application), so that it has its own Application ID. Don't forget to grant your application API Access to the web API you registered in the previous step.

Your native application registration should include the following information:

- Enable the **Native Client** setting for your application.
- Once your app is created, open the app's **API access** blade and **Add** the API you created in the previous step.
- Copy the Application ID generated for your application, so you can use it in the next step.

### [OPTIONAL] Step 6: Configure the Visual Studio project with your app coordinates

1. Open the solution in Visual Studio.
1. Open the `App.xaml.cs` file.
1. Find the assignment for `public static string Tenant` and replace the value with your tenant name.
1. Find the assignment for `public static string ClientID` and replace the value with the Application ID from Step 2.
1. Find the assignment for each of the policies `public static string PolicyX` and replace the names of the policies you created in Step 3.
1. Find the assignment for the scopes `public static string[] Scopes` and replace the scopes with those you created in Step 4.

### Step 7:  Run the sample

1. Clean the solution, rebuild the solution, and run it.
1. Click the sign-in button at the top of the application screen. The sample works exactly in the same way regardless of the account type you choose, apart from some visual differences in the authentication and consent experience. Upon successful sign in, the application screen will list some basic profile info for the authenticated user and show buttons that allow you to edit your profile, call an API and sign out.
1. Close the application and reopen it. You will see that the app retains access to the API and retrieves the user info right away, without the need to sign in again.
1. Sign out by clicking the Sign out button and confirm that you lose access to the API until the exit interactive sign in.  

## More information
For more information on Azure B2C, see [the Azure AD B2C documentation homepage](http://aka.ms/aadb2c). 
