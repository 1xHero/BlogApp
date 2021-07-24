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

namespace BlogApp
{
    class Constant
    {
        public static string URL = "http://192.168.1.215/";
        public static string HOME = URL + "api";
        public static string LOGIN = HOME + "/login";
        public static string REGISTER = HOME + "/register";
        public static string RSAVEUSERINFO = REGISTER + "/save_user_info";
        public static string POSTS = HOME + "/posts";
        public static string ADD_POST = POSTS + "/create";
        public static string UPDATE_POST = POSTS + "/update";
        public static string DELETE_POST = POSTS + "/delete";
        public static string LIKE_POST = POSTS + "/like";
    }
}