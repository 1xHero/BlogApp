using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using BlogApp.Adapters;
using BlogApp.Models;
using Org.Json;
using RestSharp;
using System;
using System.Collections.Generic;



namespace BlogApp.Fragments
{
    [Obsolete]
    public class HomeFragment : Fragment
    {
        View view;
        public static RecyclerView recyclerview;
        public static List<Post> list;

         ProgressDialog dialog;

        AndroidX.SwipeRefreshLayout.Widget.SwipeRefreshLayout refreshLayout;
        PostsAdapter postsAdapter;
        Toolbar toolbar;
        IRestResponse response;
        ISharedPreferences pref;
        ISharedPreferencesEditor editor;



        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here

            

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            view = inflater.Inflate(Resource.Layout.layout_home,container,false);
            init();
            return view;
        }

        private void init()
        {
            recyclerview = view.FindViewById<RecyclerView>(Resource.Id.recyclerHome);
            recyclerview.HasFixedSize = true;
            recyclerview.SetLayoutManager(new LinearLayoutManager(Context));
            refreshLayout = view.FindViewById<AndroidX.SwipeRefreshLayout.Widget.SwipeRefreshLayout>(Resource.Id.swipeHome);
            toolbar = view.FindViewById<Toolbar>(Resource.Id.toolbarHome);
            ((HomeActivity)Context).SetActionBar(toolbar);
          //  SetHasOptionsMenu(true);
            

           getPosts(true);

            refreshLayout.Refresh += delegate
            {
                getPosts(false);
            };
        }

        private void getPosts(bool first_time)
        {

                dialog = new ProgressDialog(Context);
                dialog.SetCancelable(true);

            if(first_time==true)
            {
                
                dialog.SetMessage("Loading Posts");
                dialog.Indeterminate = true;
                dialog.SetProgressStyle(Android.App.ProgressDialogStyle.Spinner);
                dialog.Show();
            }

            list = new List<Post>();
           // refreshLayout.Refreshing = true;



            pref = PreferenceManager.GetDefaultSharedPreferences(Context);

            string Token = pref.GetString("token", "");

            string query = Constant.POSTS;

            string Bearer = "Bearer " + Token;

            var client = new RestClient(query);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", Bearer);


            //request.AddParameter("application/octet-stream", Bitmaptostring(bitmap) , ParameterType.RequestBody);

            //request.AddHeader("content-type", "application/x-www-form-urlencoded");

            //request.
            //    AddParameter("application/x-www-form-urlencoded",
            //    $"name={name}&lastname={lastname}" +
            //    $"&photo={System.Web.HttpUtility.UrlEncode(Bitmaptostring(bitmap))}",
            //    ParameterType.RequestBody);

            // request.AddParameter("application/x-www-form-urlencoded; charset=UTF-8", "photo="+Bitmaptostring(bitmap), ParameterType.RequestBody);
            //request.AddBody("photo="+ParameterType.RequestBody);

            response = client.Execute(request);


            System.Console.WriteLine("[*] Response: " + response.Content);

            try
            {
                JSONObject jobject = new JSONObject(response.Content);
                if (jobject.GetBoolean("success"))
                {
                    if (jobject.Has("message"))
                    {
                    if (jobject.GetString("message").Equals("token_expired"))
                    {
                        editor = pref.Edit();
                        editor.PutString("token", jobject.GetString("token"));
                        editor.Apply();    
                        getPosts(false);
                        return;
                    }
                    }
                    

                    JSONArray array = new JSONArray(jobject.GetString("posts"));
                    for (int i = 0; i < array.Length(); i++)
                    {
                        JSONObject postObject = array.GetJSONObject(i);
                        JSONObject userObject = postObject.GetJSONObject("user");

                        User user = new User();

                        user.Id = userObject.GetInt("id");
                        user.UserName = userObject.GetString("name") +" "+ userObject.GetString("lastname");
                        user.Photo=(userObject.GetString("photo"));

                        Post post = new Post();
                        post.Id = postObject.GetInt("id");
                        post.User = user;
                        post.Like=(postObject.GetInt("likesCount"));
                        post.Comments = postObject.GetInt("commentsCount");
                        post.Date = postObject.GetString("created_at");
                        post.Desc = postObject.GetString("desc");
                        post.Photo = postObject.GetString("photo");
                        post.SelfLike = postObject.GetBoolean("selfLike");

                        list.Add(post);
                    }

                    postsAdapter = new PostsAdapter(Context, list);
                    recyclerview.SetAdapter(postsAdapter);
                }
            }
            catch (JSONException e)
            {

               // throw e;
                Console.WriteLine("[-] Exc :"+e);
                refreshLayout.Refreshing = false;
            }
            refreshLayout.Refreshing = false;

            if(first_time==true)
            {
                dialog.Dismiss();
            }
            dialog.Dismiss();

        }

/*
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            
            inflater.Inflate(Resource.Menu.menu_search, menu);


           var item = menu.FindItem(Resource.Id.search);
            var searchView = MenuItemCompat.GetActionView(item);

           var _searchView = searchView.JavaCast<SearchView>();

            //_searchView.QueryTextChange += (s, e) =>
            //{

            //};

            //_searchView.QueryTextSubmit += (s, e) =>
            //{

            //};

            base.OnCreateOptionsMenu(menu, inflater);
        }

        */


    }


}