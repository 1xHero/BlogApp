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
    public class User
    {
        private int id;
        private string userName;
        private string photo;

        public int Id { get => id; set => id = value; }
        public string UserName { get => userName; set => userName = value; }
        public string Photo { get => photo; set => photo = value; }
    }
}