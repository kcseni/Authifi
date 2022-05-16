using Authifi.Core.DBController;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Authifi.Views
{

    public partial class SongAdderDeleterDialog : Window
    {

        List<Playlist> ToAddPlaylists;
        List<Playlist> ToDeletePlaylists;
        Song SongToUse;
        Playlist LikedPlaylist;

        readonly Controller r;

        public SongAdderDeleterDialog(Song song, Controller con)
        {
            InitializeComponent();

            ToAddPlaylists = new List<Playlist>();
            ToDeletePlaylists = new List<Playlist>();

            SongToUse = song;
            r = con;
        }


        public void PlaylistGetter(List<Playlist> ListOfPlaylists, Playlist LikedPlaylist)
        {

            this.LikedPlaylist = LikedPlaylist;

            bool contains = false;
            
            for (int i = 0; i < ListOfPlaylists.Count; i++)
            {
                contains = false;

                for (int j = 0; j < ListOfPlaylists[i].Songs.Count; j++)
                {
                    if (ListOfPlaylists[i].Songs[j].HashCode == SongToUse.HashCode)
                    {
                        contains = true;
                        break;
                    }
                }

                if (!contains)
                    ToAddPlaylists.Add(ListOfPlaylists[i]);
                else 
                    ToDeletePlaylists.Add(ListOfPlaylists[i]);
                
            }

            ListOfAddPlaylists.ItemsSource = ToAddPlaylists;
            ListOfDeletePlaylists.ItemsSource = ToDeletePlaylists;


        }



        private void Adder_DoubleClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Playlist playlist = button.DataContext as Playlist;

            playlist.Songs.Add(SongToUse);

            //TODO save
            //r.AddSongtoPlaylist(s.HashCode,s.SongTitle, s.Artist, s.Duration, )

            Close();
        }
        
        private void Delete_DoubleClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Playlist playlist = button.DataContext as Playlist;

            playlist.Songs.Remove(SongToUse);
            //r.DeleteSongbyHash(SongToUse.HashCode);

            Close();
        }

        private void LikeButton_Click(object sender, RoutedEventArgs e)
        {
            bool contains = false;
            for (int i = 0; i < LikedPlaylist.Songs.Count; i++)
            {
                if (LikedPlaylist.Songs[i].HashCode == SongToUse.HashCode)
                {
                    contains = true;
                    break;
                }
            }

            if (!contains)
            {
                LikedPlaylist.Songs.Add(SongToUse);
            }
            else
            {
                LikedPlaylist.Songs.Remove(SongToUse);
            }

            Close();
        }

        
    }
}
