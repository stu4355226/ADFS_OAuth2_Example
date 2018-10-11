using System;
using RestSharp;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace test
{
    public class Program
    {
   
        public static void Main(string[] args)
        {
            Credential credential = new Credential("your_adfs_server", 
                                                   "your_client_id", 
                                                   "your_resource_url", 
                                                   "your_redirect_url", 
                                                   "account@company.com", 
                                                   "password");
            string accessCode = credential.GetAccessCode();
            AccessToken accessToken = credential.GetAccessToken(accessCode);
            Console.WriteLine("Done");
        }
    }

    public static class Helper
    {
        public static AccessToken GetAccessToken(this Credential credential, string accessCode)
        {
            IRestResponse response = GetAccessTokenResponse(credential, accessCode);
            return JsonConvert.DeserializeObject<AccessToken>(response.Content);
        }

        private static IRestResponse GetAccessTokenResponse(Credential credential, string accessCode)
        {
            var client = new RestClient(credential.authURL + "/adfs/oauth2/token");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("undefined", "client_id=" + credential.clientID + "&redirect_uri=" + credential.redirectUri + "&grant_type=authorization_code&code=" + accessCode, ParameterType.RequestBody);
            return client.Execute(request); ;
        }

        //if you are not getting the access code, please debug here and verify your response.
        public static string GetAccessCode(this Credential credential)
        {
            string result = string.Empty;
            IRestResponse response = GetAuthorizeResponse(credential);
            for (int i = 0; i < response.Headers.Count; i++)
            {
                if (response.Headers[i].ToString().Contains("auth/callback?"))
                {
                    string[] stringArray = response.Headers[i].ToString().Split('?');
                    foreach (string value in stringArray)
                    {
                        if (value.Contains("code")) 
                            return value.Replace("code=", string.Empty);
                    }
                }
            }
            return string.Empty;
        }

        private static IRestResponse GetAuthorizeResponse(Credential credential)
        {
            var client = new RestClient(credential.authURL + "/adfs/oauth2/authorize?response_type=code&client_id=" + credential.clientID + "&resource=" + credential.resource + "&redirect_uri=" + credential.redirectUri);
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("cache-control", "no-cache");
            request.AddParameter("undefined", "UserName=" + credential.userName + "&Password=" + credential.password + "&AuthMethod=FormsAuthentication", ParameterType.RequestBody);
            return client.Execute(request);
        }
    }

    public class Credential
    {
        public Credential(string authURL, string clientID, string resource, string redirectUri, string userName, string password)
        {
            this.authURL = authURL;
            this.clientID = clientID;
            this.resource = resource;
            this.redirectUri = redirectUri;
            this.userName = userName;
            this.password = password;
        }

        public string authURL
        {
            get;
            set;
        }

        public string clientID
        {
            get;
            set;
        }

        public string resource
        {
            get;
            set;
        }

        public string redirectUri
        {
            get;
            set;
        }

        public string userName
        {
            get;
            set;
        }
        public string password
        {
            get;
            set;
        }
    }

    [DataContract]
    public class AccessToken
    {
        [DataMember(Name = "access_token")]
        public string accessToken
        {
            get;
            set;
        }

        [DataMember(Name = "token_type")]
        public string tokenType
        {
            get;
            set;
        }

        [DataMember(Name = "expires_in")]
        public int expiresIn
        {
            get;
            set;
        }

        [DataMember(Name = "refresh_token")]
        public string refreshToken
        {
            get;
            set;
        }
    }
}
