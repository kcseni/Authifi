using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
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

    public partial class KaraokeDialog : Window
    {
        public KaraokeDialog(Song song)
        {
            InitializeComponent();
            _song = song;
            LyricsGetter();
            
            
        }


        int CurrentLine = 0;
        string LyricsWhole;
        List<string> LyricsLines = new List<string>();
        Song _song;

        public async Task LyricsGetter()
        {
            LyricsLines = await Spotify.Tools.getLyrics(_song.SongTitle,_song.Artist);
            LyricsScreen.Text = String.Format("{0}\n{1}", LyricsLines[0], LyricsLines[1]);
        }



        //STEP
        void RightArrow_Click(object sender, RoutedEventArgs e)
        {
            NextTwoLines();
        }

        void LeftArrow_Click(object sender, RoutedEventArgs e)
        {
            PreviousTwoLines();
        }

        void OnEnterDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right) NextTwoLines();
            if (e.Key == Key.Left) PreviousTwoLines();
        }

        void NextTwoLines()
        {
            if (CurrentLine != LyricsLines.Count - 2)
            {
                CurrentLine = CurrentLine + 2;
                LyricsScreen.Text = String.Format("{0}\n{1}", LyricsLines[CurrentLine], LyricsLines[CurrentLine + 1]);
            }
        }

        void PreviousTwoLines()
        {
            if (CurrentLine != 0)
            {
                CurrentLine = CurrentLine - 2;
                LyricsScreen.Text = String.Format("{0}\n{1}", LyricsLines[CurrentLine], LyricsLines[CurrentLine + 1]);
            };
        }
        
        void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
    }
}
