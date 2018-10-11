# This is to demonstrate how to get the access token through from ADFS 3.0 with Oauth2 authentication.

In order to get authenticated, you need to understand below terms that are necessary to get by pass the authorization server and get the secure token and use the token to bypass your authentication checks.

## System relationship.

##### The Third party application: 
Your application that is trying to access your resource server with secure token.

##### The resource server: 
It's the server/website endpoint you want to access/browse.

##### The authorization server: 
It's your ADFS server endpoint to get the secure token.

Your request will be sent from your third party application, and the gold is to get the data from your resource server. However, you need to get the secure token and attached it to request header in order to bypass the secure check.

## Necessary values:

Redirect URIs
Client Id

You need to execute below command to request your client id and redirect url on your ADFS server.

    Set-AdfsClient -TargetName "App Name" -RedirectUri "your redriect url"

For example:
    
    Set-AdfsClient -TargetName "testApps" -RedirectUri "http://localhost:3000/auth/callback"


Execute below command to get your adfs client information.

    get-adfsclient "App Name"


Process:
you will interact with your ADFS server to get below few things.    
Access code: one time token that uses to get your access token. it's invalid once you gain the token.   
Access token: the secure token that lets you pass the secure check. 
Refresh token: the token to renew your access token if it's expired.    

1. Send a get request to your ADFS server to get access code:

the http get request should look like this:

    https://authorization-server.com/auth?response_type=code&client_id=CLIENT_ID&redirect_uri=REDIRECT_URI

        response_type=code - Indicates that your server expects to receive an authorization code
        client_id - the id you registered on your ADFS server
        REDIRECT_URI - the url you registered on your ADFS server

if ADFS server redirect you to login page for your crendetional. you need to change the request to Post request and add below body:

        AuthMethod : FormsAuthentication
        UserName : your AD account
        Password : your AD PW

2. ADFS will redriect you to the REDIRECT_URI endpoint with below querystring.


        https://REDIRECT_URI?code=ACCESS_CODE_HERE


3. You need to grab the access code add it to a post request like below:


        POST https://authorization-server.com//token

        header:
            Content-Type : application/x-www-form-urlencoded

        body:
        grant_type : authorization_code
        code : ACCESS_CODE_HERE 
        redirect_uri : REDIRECT_URI 
        client_id : CLIENT_ID

If above steps are correct, it should return you the access token.

You just need to attached it to your request header like below:

    header:
        Authorization : Bearer {access token}
