using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.VisualBasic;

using Authifi.Core.DBController;
using Authifi.Spotify;

namespace Authifi.Views
{
    public partial class MainPage : UserControl
    {
        readonly Controller r;
        
        
        
        public MainPage()
        {
            Spotify.Tools.getLyrics("", "");
            InitializeComponent();
            ProfileFillUp();
            PlaylistFillUp();

            r = new Controller();

        }


        public async Task ProfileFillUp()
        {
            
            PrivateUser p = await Spotify.Tools.GetUserInfoAsync();
            System.Threading.Thread.Sleep(1000);
            string displayname = p.DisplayName;
            string email = p.Email;
            r.AddUser(displayname, email);
            string name = r.GetUserbyEmail(email).GivenName;

            ProfileName.Text = name;
            LikedSongsPlayListTxt.Text = name + "'s Liked Songs";
            
            /*r.AddPlaylist("chilltime", false, r.GetUserbyEmail(email).Id);
            r.AddPlaylist("depi", false, r.GetUserbyEmail(email).Id);
            r.AddPlaylist("LOVE", false, r.GetUserbyEmail(email).Id);
        
            r.AddSongtoPlaylist("spotify:track:0bUYgpgrUaoFouvS0vf6qe", "Lofi Rain", "Chilled Cougar", "2:00", 1);
            r.AddSongtoPlaylist("spotify:track:1spA7tGixupxqsGadOn2YL", "Chillin", "Wale", "3:24", 1);
            r.AddSongtoPlaylist("spotify:track:6HbxpoChDH9ThadFOSAnma", "Autumn Town Leaves", "Iron & Wine", "3:15", 1);
            r.AddSongtoPlaylist("spotify:track:17S4XrLvF5jlGvGCJHgF51", "Learning To Fly", "Tom Petty and the Heartbreakers", "4:02", 1);
            r.AddSongtoPlaylist("spotify:track:4bJygwUKrRgq1stlNXcgMg", "All The Things She Said", "t.A.T.u.", "3:34", 2);
            r.AddSongtoPlaylist("spotify:track:0COqiPhxzoWICwFCS4eZcp", "Bring Me To Life", "Evanescence", "3:55", 2);
            r.AddSongtoPlaylist("spotify:track:2nLtzopw4rPReszdYBJU6h", "Numb", "Linkin Park", "3:05", 2);
            r.AddSongtoPlaylist("spotify:track:2nMeu6UenVvwUktBCpLMK9", "Young And Beautiful", "Lana Del Rey", "3:56", 2);
            r.AddSongtoPlaylist("spotify:track:3JvKfv6T31zO0ini8iNItO", "Another Love", "Tom Odell", "4:04", 3);
            r.AddSongtoPlaylist("spotify:track:6jBUP2KCe821yqf1hiBqPR", "Broken Strings", "James Morrison", "4:10", 3);
            r.AddSongtoPlaylist("spotify:track:0NlGoUyOJSuSHmngoibVAs", "All I Want", "Kodaline", "5:05", 3);
            r.AddSongtoPlaylist("spotify:track:0eg0MBViQnDlaZhfg2H3gY", "Perfect Ruin", "Kwabs", "4:12", 3);*/
            
            
        }




        //SEARCH

        public string SearchString { set; get; }

        void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchByString();
        }

        void OnEnterDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return) SearchByString();
        }


        List<FullTrack> tracks;
        List<Song> songs = new List<Song>();

        
        
        Playlist SearchPlaylist;
        bool searching = false;
        void SearchByString()
        {
            SearchString = SearchBoxInput.Text;

            songs.Clear();
            Task<List<FullTrack>> search = Task.Run<List<FullTrack>>(async () => await Spotify.Tools.SearchTrack(SearchString));

            tracks = search.Result;
            int result_count = tracks.Count;

            TrackAdder(result_count);

            SearchPlaylist = new Playlist(songs);
            OpenedPlaylist = SearchPlaylist;
            searching = true;

            ICollectionView view = CollectionViewSource.GetDefaultView(ListOfSongs.ItemsSource);
            if (view != null)
            {
                view.Refresh();
            }


        }
        
        void TrackAdder(int res_c)
        {
            for (int i = 0; i < res_c; i++)
            {
                TrackToSong(tracks[i]);
            }

            ListOfSongs.ItemsSource = songs;

        }

        void TrackToSong(FullTrack track)
        {
            string _HashCode = track.Uri;
            string _SongTitle = track.Name;
            string _Artist = track.Artists[0].Name;

            int _dur = track.DurationMs / 1000;
            int _sec = _dur % 60;
            int _min = (_dur - _sec) / 60;

            string _Duration = "";
            if (_sec >= 10)
                _Duration = $"{_min}:{_sec}";
            else
                _Duration = $"{_min}:0{_sec}";

            songs.Add(new Song(_SongTitle, _Artist, _dur, _sec, _min, _Duration, _HashCode));

        }


        void KaraokeModeButton_Click(object sender, RoutedEventArgs e)
        {

            KaraokeDialog kd = new KaraokeDialog(CurrentPlaylist.CurrentSong);
            kd.ShowDialog();

        }


        bool isPaused = false;
        async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (isPaused)
            {
                await Spotify.Tools.ResumeSong();
                isPaused = false;
            }
            else
            {
                await Spotify.Tools.PauseSong();
                isPaused = true;
            }
        }

        async void NextSong_Click(object sender, RoutedEventArgs e)
        {
            int current_idx = CurrentPlaylist.Songs.IndexOf(CurrentPlaylist.CurrentSong);
            if (current_idx + 1 < CurrentPlaylist.Songs.Count)
            {
                CurrentPlaylist.CurrentSong = CurrentPlaylist.Songs[current_idx + 1];
            }

            await Spotify.Tools.Playsong(CurrentPlaylist.CurrentSong.HashCode);


            CurrentSongTitle.Text = CurrentPlaylist.CurrentSong.SongTitle;
            CurrentArtist.Text = CurrentPlaylist.CurrentSong.Artist;

        }

        async void PreviousSong_Click(object sender, RoutedEventArgs e)
        {
            int current_idx = CurrentPlaylist.Songs.IndexOf(CurrentPlaylist.CurrentSong);
            if (current_idx - 1 >= 0)
            {
                CurrentPlaylist.CurrentSong = CurrentPlaylist.Songs[current_idx - 1];
            }
            await Spotify.Tools.Playsong(CurrentPlaylist.CurrentSong.HashCode);

            CurrentSongTitle.Text = CurrentPlaylist.CurrentSong.SongTitle;
            CurrentArtist.Text = CurrentPlaylist.CurrentSong.Artist;
        }




        //PLAYLIST

        List<Playlist> playlists = new List<Playlist>();
        Playlist LikedPlaylist;
        Playlist CurrentPlaylist;
        Playlist OpenedPlaylist;

        async Task PlaylistFillUp()
        {
            PrivateUser p = await Spotify.Tools.GetUserInfoAsync();
            System.Threading.Thread.Sleep(1000);
            string email = p.Email;
            List<Authifi.Core.Models.Playlist> _playlists = r.GetPlaylistsofUser(email);


            for (int i = 0; i < _playlists.Count; i++)
            {

                List<Authifi.Core.Models.Song> _songs = Authifi.Core.DBController.Controller.GetSongsofPlaylist(_playlists[i]);    


                Playlist pl = new Playlist();

                
                for (int j = 0; j < _songs.Count; j++)
                {
                    Authifi.Core.Models.Song s = _songs[j];
                    pl.Songs.Add(new Song(s.Title, s.Artist, 0, 0, 0, s.Duration, s.Hash));
                    
                    
                }
                pl.Name = _playlists[i].Name;
                pl.Liked = _playlists[i].Liked;

                if (pl.Liked)
                {
                    LikedPlaylist = pl;
                }
                else
                {
                    playlists.Add(pl);
                }
                                
            }

            if (LikedPlaylist == null)
            {
                LikedPlaylist = new Playlist();
                LikedPlaylist.Liked = true;
            }
                

            ListOfOtherPlaylists.ItemsSource = playlists;
        }

        void LikedPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            
            OpenedPlaylist = LikedPlaylist;
            searching = false;

            if(OpenedPlaylist!=null)
            {
                ListOfSongs.ItemsSource = OpenedPlaylist.Songs;
            }
            else
            {
                ListOfSongs.ItemsSource = null;
            }
            
            

            ICollectionView view = CollectionViewSource.GetDefaultView(ListOfSongs.ItemsSource);
            if (view != null)
            {
                view.Refresh();
            }

        }

        void PlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Playlist playlist = button.DataContext as Playlist;

            OpenedPlaylist = playlist;
            ListOfSongs.ItemsSource = OpenedPlaylist.Songs;
            searching = false;

            ICollectionView view = CollectionViewSource.GetDefaultView(ListOfSongs.ItemsSource);
            if (view != null)
            {
                view.Refresh();
            }            
        }

        void PlaylistsRefresh()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(ListOfOtherPlaylists.ItemsSource);
            if (view != null)
            {
                view.Refresh();
            }

            ListOfOtherPlaylists.ItemsSource = playlists;
        }

        void NewPlaylist_Click(object sender, RoutedEventArgs e)
        {
            string name = "";

            PlaylistDialog pld = new PlaylistDialog();
            if (pld.ShowDialog() == true)
                name = pld.PlaylistName;


            if (name!="")
            {
                Playlist pl = new Playlist();
                pl.Name = name;

                playlists.Add(pl);

                PlaylistsRefresh();

            }



        }

        void PlaylistDelete_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Playlist pl = button.DataContext as Playlist;

            int idx = -1;
            for (int i = 0; i < playlists.Count; i++)
            {
                if (playlists[i].PlaylistEquals(pl))
                {
                    idx = i;
                    break;
                }
            }

            playlists.RemoveAt(idx);

            PlaylistsRefresh();


        }




        //TRACK MANIP

        async void TrackButton_DoubleClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Song song = button.DataContext as Song;
                        
            CurrentPlaylist = new Playlist(OpenedPlaylist.Songs);

            int idx = 0;
            for (int i = 0; i < CurrentPlaylist.Songs.Count; i++)
            {
                if (song.HashCode == CurrentPlaylist.Songs[i].HashCode)
                {
                    idx = i;
                }
            }

            CurrentPlaylist.CurrentSong = CurrentPlaylist.Songs[idx];

            await Spotify.Tools.Playsong(CurrentPlaylist.CurrentSong.HashCode);

            CurrentSongTitle.Text = CurrentPlaylist.CurrentSong.SongTitle;
            CurrentArtist.Text = CurrentPlaylist.CurrentSong.Artist;

        }

        void LikeButton_Click(object sender, RoutedEventArgs e)
        {
            Song s = CurrentPlaylist.CurrentSong;
            bool contains = false;

            for (int i = 0; i < LikedPlaylist.Songs.Count; i++)
            {
                if (LikedPlaylist.Songs[i].HashCode == s.HashCode)
                {
                    contains = true;
                    break;
                }
            }

            if (!contains)
            {
                
                LikedPlaylist.Songs.Add(s);
                //TODO save
               // r.AddSongtoPlaylist(s.HashCode,s.SongTitle, s.Artist, s.Duration, )
            }
            else
            {
                LikedPlaylist.Songs.Remove(CurrentPlaylist.CurrentSong);
                //TODO save
                //Mintha jó lenne
                r.DeleteSongbyHash(s.HashCode);
            }
        }
       
        


        private void AddDeleteSong_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Song song = button.DataContext as Song;

            SongAdderDeleterDialog dialog = new SongAdderDeleterDialog(song, r);

            dialog.PlaylistGetter(playlists, LikedPlaylist);

            dialog.Show();

        }

        /*
        private int volumeValue = 50;

        async Task VolumeChange()
        {
            volumeValue = (int)VolumeSlider.Value;
            await Spotify.Tools.SetVolume(volumeValue);
        }
        */

        private int volumeValue = 50;
        private void VolumeSlider_ValueChanged(object sender, EventArgs e)
        {
            volumeValue = (int)VolumeSlider.Value;
            Spotify.Tools.SetVolume(volumeValue);
        }

    }


    public class Song
    {

        public string HashCode { get; set; }
        public string SongTitle { get; set; }
        public string Artist { get; set; }
        public int dur { get; set; }
        public int sec { get; set; }
        public int min { get; set; }
        public string Duration { get; set; }

        //TODO
        public string Lyrics { get; set; }

        public Song(string st, string a, int d, int s, int m, string s_D, string hc)
        {
            SongTitle = st;
            Artist = a;
            dur = d;
            sec = s;
            min = m;
            Duration = s_D;
            HashCode = hc;
        }

    }

    public class Playlist
    {


        public string Name { get; set; }

        public List<Song> Songs { get; set; }
        public Song CurrentSong { get; set; }

        public bool Liked { get; set; }

        public Playlist(List<Song> s)
        {
            this.Songs = s;
        }

        public Playlist() {
            this.Songs = new List<Song>();
            this.Liked = false;
        }

        public bool PlaylistEquals(Playlist pl)
        {
            bool eq = true;

            if (this.Name != pl.Name || this.Liked != pl.Liked)
            {
                eq = false;
            }
            else
            {
                if (this.Songs.Count != pl.Songs.Count)
                {
                    eq = false;
                }
                else
                {
                    for (int i = 0; i < this.Songs.Count; i++)
                    {
                        if (this.Songs[i].HashCode != pl.Songs[i].HashCode)
                            eq = false;
                    }
                }
            }
               
            return eq;
        }


    }


}
