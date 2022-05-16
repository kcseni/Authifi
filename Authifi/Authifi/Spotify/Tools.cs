using Microsoft.Extensions.Configuration;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MuxicMatchApi;

namespace Authifi.Spotify
{
    class Tools
    {

        public static  async Task<List<FullTrack>> SearchTrack(string text)
        {
            List<FullTrack> tracks = new List<FullTrack>();

            SearchRequest request = new SearchRequest(SearchRequest.Types.Track, text);
            SearchResponse response = await Authentication.Client.Search.Item(request);

            //TracksSearchResult result = await Authentication.TracksApi.SearchTracks(text);
            for (int i = 0; i < 20; i++)
            {
                tracks.Add(response.Tracks.Items[i]);
            }
            return tracks;
            
        }

        public static async Task Playsong(FullTrack track)
        {
            await Playsong(track.Uri);

        }

        public static async Task Playsong(string Uri)
        {
            DeviceResponse devices = await Authentication.Client.Player.GetAvailableDevices();

            if (devices.Devices.Count > 0)
            {
                PlayerTransferPlaybackRequest ptp_request = new PlayerTransferPlaybackRequest(new List<string> { devices.Devices[0].Id });
                await Authentication.Client.Player.TransferPlayback(ptp_request);

                PlayerResumePlaybackRequest request = new PlayerResumePlaybackRequest();
                request.Uris = new List<string> { Uri };
                await Authentication.Client.Player.ResumePlayback(request);
            }
        }

        public static async Task PauseSong()
        {
            DeviceResponse devices = await Authentication.Client.Player.GetAvailableDevices();

            if (devices.Devices.Count > 0)
            {
                PlayerPausePlaybackRequest request = new PlayerPausePlaybackRequest();

                await Authentication.Client.Player.PausePlayback(request);
            }
        }

        public static async Task ResumeSong()
        {
            DeviceResponse devices = await Authentication.Client.Player.GetAvailableDevices();

            if (devices.Devices.Count > 0)
            {
                PlayerResumePlaybackRequest request = new PlayerResumePlaybackRequest();
                await Authentication.Client.Player.ResumePlayback(request);
            }
        }

        public static async Task<PrivateUser> GetUserInfoAsync()
        {
            return await Authentication.Client.UserProfile.Current();
        }

        public static async Task SetVolume(int volume)
        {
            PlayerVolumeRequest request = new PlayerVolumeRequest(volume);
            await Authentication.Client.Player.SetVolume(request);
        }

        public static async Task<List<String>> getLyrics(string Title, string Artist)
        {
            List<String> lyrics_list = new List<String>();

            var client = new MuxicMatchClient("4a0b607b9dd50aa21cb18ca20d9a96bd");
            string track = await client.GetMusixMatchLyricsApi(Title, Artist);

            string[] lyrics_array = track.Split("\n");
            for (int i = 0; i < lyrics_array.Length; i++)
            {
                lyrics_list.Add(lyrics_array[i]);
            }

            return lyrics_list;
        }
    }
}
