using System;
using System.Collections.Generic;
using System.Text;
using Authifi.Core.Models;
using System.Linq;
using System.Collections;
using System.IO;

namespace Authifi.Core.DBController
{
   public class Controller
    {
       

        public Controller() { }

       

        public static List<Song> GetSongsofPlaylist(Playlist pl)
        {
            using (var db = new DatabaseContext())
            {
                return (from s in db.Songs
                        where s.PlaylistID == pl.Id
                        select s).ToList();

            }
        }

        public void AddPlaylist(string name, bool liked, int userid)
        {
            using (var db = new DatabaseContext())
            {
                Playlist p = new Playlist();
                p.Name = name;
                p.Liked = liked;
                p.UserID = userid;
                db.Add(p);
                db.SaveChanges();
            }
        }

        public void AddSongtoPlaylist(string hash, string title, string artist, string duration, int playlistID)
        {
            using (var db = new DatabaseContext())
            {
                Song s = new Song();
                s.Hash = hash;
                s.PlaylistID = playlistID;
                s.Artist = artist;
                s.Duration = duration;
                s.Title = title;
                s.Lyrics = "";
                db.Add(s);
                db.SaveChanges();
            }
        }


        public void AddUser(string name, string mail)
        {

            using (var db = new DatabaseContext())
            {
                if (GetUserbyEmail(mail) == null)
                {




                    User u = new User();
                    u.GivenName = name;
                    u.Mail = mail;
                    
                    db.Add(u);
                    db.SaveChanges();
                }
            }

        }

        public User GetUserbyEmail(string email)
        {
            using (var db = new DatabaseContext())
            {
                return (from p in db.Users
                        where p.Mail.Equals(email)
                        select p).FirstOrDefault();
            }
        }

        public void DeleteUserbyID(int id)
        {
            using (var db = new DatabaseContext())
            {
                User u = GetUserById(id);
                db.Remove(u);
                db.SaveChanges();
            }
        }

        public void DeleteUserbyName(string name)
        {
            using (var db = new DatabaseContext())
            {
                User u = GetUserbyName(name);
                db.Remove(u);
                db.SaveChanges();
            }
        }

        public User GetUserbyName(string name)
        {
            using (var db = new DatabaseContext())
            {
                return (from p in db.Users
                        where p.GivenName.Equals(name)
                        select p).FirstOrDefault();
            }
        }

        public User GetUserById(int id)
        {
            using (var db = new DatabaseContext())
            {
                return (from p in db.Users
                        where p.Id.Equals(id)
                        select p).FirstOrDefault();
            }
        }

        public Playlist GetPlayListbyId(int id)
        {
            using (var db = new DatabaseContext())
            {
                return (from p in db.Playlists
                        where p.Id.Equals(id)
                        select p).FirstOrDefault();
            }
        }

        public Song GetSongbyHash(string hash)
        {
            using (var db = new DatabaseContext())
            {
                return (from p in db.Songs
                        where p.Hash.Equals(hash)
                        select p).FirstOrDefault();
            }
        }

        public List<Playlist> GetPlaylistsofUser(string email)
        {
            using (var db = new DatabaseContext())
            {
                User u = GetUserbyEmail(email);
                List<Playlist> List = (from p in db.Playlists
                                       where p.UserID == u.Id
                                       select p).ToList();

                return List;

                /*ArrayList Name = new ArrayList();

                for (int i = 0; i < List.Count; i++)
                {
                    Name.Add(List[i].Name);

                }



                File.WriteAllLines("proba.txt", (string[])Name.ToArray(typeof(string)));*/


            }
        }

        public void DeleteSongbyHash(string hash)
        {
            using (var db = new DatabaseContext())
            {
                db.Remove(GetSongbyHash(hash));
                db.SaveChanges();
            }
        }

        public void DeletePlayList(int id)
        {
            using (var db = new DatabaseContext())
            {
                db.Remove(GetPlayListbyId(id));
                db.SaveChanges();
            }
        }

    }
}
