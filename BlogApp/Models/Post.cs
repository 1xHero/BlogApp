using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlogApp.Models
{
    public class Post
    {
        private int id, like, comments;
        private string date, desc, photo;
        private User user;
        private bool selfLike;

        public int Id { get => id; set => id = value; }
        public int Like { get => like; set => like = value; }
        public int Comments { get => comments; set => comments = value; }
        public string Date { get => date; set => date = value; }
        public string Desc { get => desc; set => desc = value; }
        public string Photo { get => photo; set => photo = value; }
        public bool SelfLike { get => selfLike; set => selfLike = value; }
        internal User User { get => user; set => user = value; }
    }
}