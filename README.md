---
page_type: sample
languages:
- csharp
products:
- azure
description: "This simple sample demonstrates how to use the Microsoft Authentication Library (MSAL) for .NET to get an access token and call an API secured by Azure AD B2C."
urlFragment: active-directory-b2c-dotnet-desktop
---

# WPF application signing in users with Azure Active Directory B2C and calling an API

> This branch is using MSAL.NET 4.x. If you are interested in a previous version of the sample using
> MSAL.NET 2.x, go to the [master](https://github.com/Azure-Samples/active-directory-b2c-dotnet-desktop/tree/master) branch


This simple sample demonstrates how to use the [Microsoft Authentication Library (MSAL) for .NET](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet) to get an access token and call an API secured by Azure AD B2C.

## How To Run This Sample

There are two ways to run this sample:

1. **Using the demo environment** - The sample is already configured to use a demo environment and can be run simply by downloading this repository and running the app on your machine. See steps below for Running with demo environment.
2. **Using your own Azure AD B2C tenant** - If you would like to use your own Azure AD B2C configuration, follow the steps listed below for Using your own Azure AD B2C tenant. Please note, the api call will only work with domains using `{tenantName}.b2clogin.com`, as the node.js api used for the api call has been updated to handle `b2clogin.com` and not `login.microsoftonline.com`. If using `login.microsoftonline.com` or a custom b2c domain, you will need to host your own web api (see step 3 below), otherwise, you will see "authorized" when making the api call with this sample as-is. 

## Using the demo environment

This sample demonstrates how to sign in or sign up for an account at "Wingtip Toys" (the demo environment for this sample) using a WPF Desktop application.  

Once signed-in, clicking on the **Call API** button shows the display name you used when you created your account. The **Edit Profile** button allows you to change your display name and city. The **Logout** button logs you out of the application.

### Step 1: Clone or download this repository

From your shell or command line:

```
git clone https://github.com/Azure-Samples/active-directory-b2c-dotnet-desktop.git
```

### Step 2: Run the project

Open the `active-directory-b2c-wpf.sln` and run the project. 

The sample demonstrates the following functionality: 

1. Click the sign-in button at the top of the application screen. The sample works exactly in the same way regardless of the account type you choose, apart from some visual differences in the authentication and consent experience. Upon successful sign in, the application screen will list some basic profile info for the authenticated user and show buttons that allow you to edit your profile, call an API and sign out.
2. Close the application and reopen it. You will see that the app retains access to the API and retrieves the user info right away, without the need to sign in again.
3. Sign out by clicking the Sign out button and confirm that you lose access to the API until the exit interactive sign in. 


## Using your own Azure AD B2C Tenant

In the previous section, you learned how to run the sample application using the demo environment. In this section, you'll learn how to configure this WPF application and a related [Node.js Web API with Azure AD B2C sample](https://github.com/Azure-Samples/active-directory-b2c-javascript-nodejs-webapi) to work with your own Azure AD B2C Tenant.

### Step 1: Get your own Azure AD B2C tenant

First, you'll need an Azure AD B2C tenant. If you don't have an existing Azure AD B2C tenant that you can use for testing purposes, you can create your own by following [these instruction](https://azure.microsoft.com/documentation/articles/active-directory-b2c-get-started/).

### Step 2: Create your own policies

This sample uses three types of policies: a unified sign-up/sign-in policy, a profile editing policy, and a password reset policy.  Create one policy of each type by following [the built-in policy instructions](https://azure.microsoft.com/documentation/articles/active-directory-b2c-reference-policies). You may choose to include as many or as few identity providers as you wish.

If you already have existing policies in your Azure AD B2C tenant, feel free to re-use those policies in this sample.

### Step 3: Register your own Web API with Azure AD B2C

As you saw in the demo environment, this sample calls a Web API at https://fabrikamb2chello.azurewebsites.net. This demo Web API uses the same code found in the sample [Node.js Web API with Azure AD B2C](https://github.com/Azure-Samples/active-directory-b2c-javascript-nodejs-webapi), in case you need to reference it for debugging purposes. 

You must replace the demo environment Web API with your own Web API. If you do not have your own Web API, you can clone the [Node.js Web API with Azure AD B2C](https://github.com/Azure-Samples/active-directory-b2c-javascript-nodejs-webapi) sample and register it with your tenant. 

#### How to setup and register the Node.js Web API sample

First, clone the Node.js Web API sample repository into its own directory, for example:  

```
cd ..
git clone https://github.com/Azure-Samples/active-directory-b2c-javascript-nodejs-webapi.git
```

Second, follow the instructions at [register a Web API with Azure AD B2C](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-app-registration#register-a-web-api) to register the Node.js Web API sample with your tenant. Registering your Web API allows you to define the scopes that your single page application will request access tokens for. 

Provide the following values for the Node.js Web API registration: 

- Provide a descriptive Name for the Node.js Web API, for example, `My Test Node.js Web API`. You will identify this application by its Name whenever working in the Azure portal.
- Mark **Yes** for the **Web App/Web API** setting for your application.
- Set the **Reply URL** to `http://localhost:5000`. This is the port number that the Node.js Web API sample is configured to run on. 
- Set the **AppID URI** to `demoapi`. This AppID URI is a unique identifier representing this Node.jS Web API. The AppID URI is used to construct the scopes that are configured in you single page application's code. For example, in this Node.js Web API sample, the scope will have the value `https://<your-tenant-name>.onmicrosoft.com/demoapi/demo.read` 
- Create the application. 
- Once the application is created, open your `My Test Node.js Web API` application and then open the **Published Scopes** window (in the left nav menu) and add the scope `demo.read` followed by a description `demoing a read scenario`. Click **Save**.

Third, in the `index.html` file of the Node.js Web API sample, update the following variables to refer to your Web API registration.  

```
var tenantID = "<your-tenant-name>.onmicrosoft.com";
var clientID = "<Application ID for your Node.js Web API - found on Properties page in Azure portal>";
var policyName = "<Name of your sign in / sign up policy, e.g. B2C_1_SiUpIn>";
```

Lastly, to run your Node.js Web API, run the following command from your shell or command line

```
npm install && npm update
node index.js
```

Your Node.js Web API sample is now running on Port 5000. 


### Step 4: Register your Native app

Now you need to [register your native app in your B2C tenant](https://docs.microsoft.com/azure/active-directory-b2c/add-native-application?tabs=app-reg-ga), so that it has its own Application ID. 

Your native application registration should include the following information:

- Provide a descriptive Name for the single page application, for example, `My Test WPF App`. You will identify this application by its Name within the Azure portal.
- Mark **Yes** for the **Native Client** setting for your application.
- Create your application.
- Once the application is created, open your `My Test WPF App` and open the **API Access** window (in the left nav menu). Click **Add** and select the name of the Node.js Web API you registered previously, for example `My Test Node.js Web API`. Select the scope(s) you defined previously, for example, `demo.read` and hit **Save**.

### Step 5: Configure your Visual Studio project with your Azure AD B2C app registrations

1. Open the solution in Visual Studio.
1. Open the `App.xaml.cs` file.
1. Find the assignment for `public static string Tenant` and replace the value with your tenant name.
1. Find the assignment for `public static string ClientID` and replace the value with the Application ID from your Native app registration, for example `My Test WPF App`.
1. Find the assignment for each of the policies, for example `public static string PolicySignUpSignIn`, and replace the names of the policies you created in Step 2, e.g. `b2c_1_SiUpIn`
1. Find the assignment for the scopes `public static string[] ApiScopes` and replace with the scope you created in Step 3, for example, `https://<your-tenant-name>.onmicrosoft.com/demoapi/demo.read`.
1. Change the `ApiEndpoint` variable to point to your Node.js Web API `hello` endpoint running locally at `"http://localhost:5000/hello"`

### Step 6:  Run the sample

1. Rebuild the solution and run the app.
2. Click the sign-in button at the top of the application screen. The sample works exactly in the same way regardless of the account type you choose, apart from some visual differences in the authentication and consent experience. Upon successful sign in, the application screen will list some basic profile info for the authenticated user and show buttons that allow you to edit your profile, call an API and sign out.
3. Close the application and reopen it. You will see that the app retains access to the API and retrieves the user info right away, without the need to sign in again.
4. Sign out by clicking the Sign out button and confirm that you lose access to the API until the exit interactive sign in.  

## More information
For more information on Azure B2C, see [the Azure AD B2C documentation homepage](http://aka.ms/aadb2c). 
