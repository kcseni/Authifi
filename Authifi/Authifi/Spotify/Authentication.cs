using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;
using System.Windows.Controls;
using Microsoft.Net.Http.Server;


namespace Authifi.Spotify
{

    class Authentication
    {
        private static string challenge;
        private static string verifier;
        private static string url = "http://localhost:5000/callback";
        private static string clientId = "bab6b94c22f24fe38b88b66090362566";


        private static SpotifyClient _client;
        public static SpotifyClient Client { get => _client; set => _client = value; }

        public static async Task InitAsync()
        {

            var (v, c) = PKCEUtil.GenerateCodes();
            verifier = v;
            challenge = c;



            var loginRequest = new LoginRequest(
                                    new Uri(url),
                                    clientId,
                                    LoginRequest.ResponseType.Code
                                )
            {
                CodeChallengeMethod = "S256",
                CodeChallenge = challenge,
                Scope = new[] { Scopes.AppRemoteControl, Scopes.PlaylistModifyPrivate, Scopes.PlaylistModifyPublic, Scopes.PlaylistReadCollaborative, Scopes.PlaylistReadPrivate, Scopes.Streaming, Scopes.UgcImageUpload, Scopes.UserFollowModify, Scopes.UserFollowRead, Scopes.UserLibraryModify, Scopes.UserLibraryRead, Scopes.UserModifyPlaybackState, Scopes.UserReadCurrentlyPlaying, Scopes.UserReadEmail, Scopes.UserReadPlaybackPosition, Scopes.UserReadPlaybackState, Scopes.UserReadPrivate, Scopes.UserReadRecentlyPlayed, Scopes.UserTopRead }
            };
            var uri = loginRequest.ToUri();


            Process.Start(new ProcessStartInfo(uri.AbsoluteUri) { UseShellExecute = true });

            var settings = new WebListenerSettings();
            settings.UrlPrefixes.Add(url);
            var http = new WebListener(settings);

            http.Start();

            var context = await http.AcceptAsync();
            string code = context.Request.QueryString.Substring(context.Request.QueryString.IndexOf('=') + 1);

            http.Dispose();

            await GetCallback(code);
        }


        private static async Task GetCallback(string code)
        {

            Uri uri = new Uri(url);
            var initialResponse = await new OAuthClient().RequestToken(
              new PKCETokenRequest(clientId, code, uri, verifier)
            );


            var authenticator = new PKCEAuthenticator(clientId, initialResponse);

            var config = SpotifyClientConfig.CreateDefault()
              .WithAuthenticator(authenticator);
            _client = new SpotifyClient(config);
        }

        public static void onExit()
        {
            PlayerPausePlaybackRequest request = new PlayerPausePlaybackRequest();

            Authentication.Client.Player.PausePlayback(request);

            System.Threading.Thread.Sleep(1000);
        }

    }
}
