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

    public partial class PlaylistDialog : Window
    {
        public PlaylistDialog()
        {
            InitializeComponent();
            
        }

        public string PlaylistName = "";

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            PlaylistName = PlaylistNameInput.Text;

            if (PlaylistName=="")
                PlaylistName = "New Playlist";

            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}



