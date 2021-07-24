using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using BlogApp.Models;

using Refractored.Controls;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Square.Picasso;
using Java.Lang;
using RecyclerViewSearch;
using Android.Preferences;
using RestSharp;
using Org.Json;
using BlogApp.Fragments;

namespace BlogApp.Adapters
{
    public class PostsAdapter:RecyclerView.Adapter,IFilterable
    {
         Context context;
        public List<Post> list;
        public List<Post> listALL;

        public Filter Filter { get; private set; }

        ISharedPreferences pref;
        IRestResponse response;
        

                
        

        public PostsAdapter(Context context, IEnumerable<Post> list)
        {
            this.context = context;
            this.list = (List<Post>)list;
            this.listALL = new List<Post>(list);

            pref = PreferenceManager.GetDefaultSharedPreferences(context);
            //context.GetSharedPreferences("user", FileCreationMode.Private);//private sharedpref


            Filter = new FilterHelper(this);

        }

        public override int ItemCount => list.Count;


       public Filter GetFilter()
        {
            return Filter;
        }

        [Obsolete]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
        {
            Post post = list[position];
            PostsHolder vh = holder as PostsHolder;
                
            
            
            Picasso.With(context).Load(Constant.URL+"storage/profiles/"+ post.User.Photo).Into(vh.imgProfile);
            Picasso.With(context).Load(Constant.URL + "storage/posts/" + post.Photo).Into(vh.imgPost);
            vh.txtName.Text=post.User.UserName;
            vh.txtComments.Text = "View all " + post.Comments;
            vh.txtLike.Text = post.Like + "Likes";
            vh.txtDate.Text = post.Date;
            vh.txtDesc.Text = post.Desc;
            vh.btnLike.SetImageResource(post.SelfLike?Resource.Drawable.ic_favorite_red:Resource.Drawable.ic_favorite_outline);
             
            

            vh.btnLike.Click += (sender,e)=>
            {   
                
                

                string Token = pref.GetString("token", "");

                string query = Constant.LIKE_POST;

                string Bearer = "Bearer " + Token;

                var client = new RestClient(query);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", Bearer);

                request.
                    AddParameter("application/x-www-form-urlencoded",
                    $"id={post.Id}"
                    , ParameterType.RequestBody);



                response = client.Execute(request);


                System.Console.WriteLine("[*] Response: " + response.Content);
                    
                    Post mpost = list[position];
                try
                {
                    JSONObject jobject = new JSONObject(response.Content);
                    if (jobject.GetBoolean("success"))
                    {

                        mpost.SelfLike = !post.SelfLike;
                        mpost.Like = mpost.SelfLike ? post.Like + 1 : post.Like - 1;


                        //list.RemoveAt(position);
                        //list.Insert(position, mpost);
                        list[position] = mpost;
                        NotifyItemChanged(position);
                        NotifyDataSetChanged();

                        vh.btnLike.SetImageResource(post.SelfLike ? Resource.Drawable.ic_favorite_outline : Resource.Drawable.ic_favorite_red);
                    }
                    else
                    {
                        vh.btnLike.SetImageResource(post.SelfLike ? Resource.Drawable.ic_favorite_red : Resource.Drawable.ic_favorite_outline);
                    }
                }
                catch (JSONException ee)
                {

                    ee.PrintStackTrace();
                }

            };

            if (post.User.Id == pref.GetInt("id", 0))
            {
                vh.btnPostOption.Visibility = ViewStates.Visible;

            }else
            {
                vh.btnPostOption.Visibility = ViewStates.Gone;
            }

            
            vh.btnPostOption.Click += (s,e)=>
            {
                PopupMenu popupMenu = new PopupMenu(context, vh.btnPostOption);
                popupMenu.Inflate(Resource.Menu.menu_post_options);
                popupMenu.MenuItemClick += (s, e) =>
                {
                    switch (e.Item.ItemId)
                    {
                        case Resource.Id.item_edit:
                            {
                                Intent i = new Intent(((HomeActivity)context), typeof(EditPostActivity));
                                i.PutExtra("postId", post.Id);
                                i.PutExtra("position", position);
                                i.PutExtra("text", post.Desc);
                                context.StartActivity(i);

                            } 
                            break;
                        case Resource.Id.item_delete:
                            {
                                deletePost(post.Id,position);
                            }
                            break;
                        default:
                            break;
                    }
                };
                popupMenu.Show();

            };

        }

        
        private void deletePost(int postId,int position)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(context);
            builder.SetTitle("Confirm");
            builder.SetMessage("Delete post?");
            builder.SetPositiveButton("Delete", (s, e) => {

               

                pref = PreferenceManager.GetDefaultSharedPreferences(context);

                string Token = pref.GetString("token", "");

                string query = Constant.DELETE_POST;

                string Bearer = "Bearer " + Token;

                var client = new RestClient(query);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", Bearer);

                request.
                    AddParameter("application/x-www-form-urlencoded",
                    $"id={postId}", ParameterType.RequestBody);

                response = client.Execute(request);
                 System.Console.WriteLine("[*] Response: " + response.Content);

                try
                {
                    JSONObject jobject = new JSONObject(response.Content);
                    if (jobject.GetBoolean("success"))
                    {

                        list.RemoveAt(position);
                        NotifyItemRemoved(position);
                        NotifyDataSetChanged();

                        listALL.Clear();

                        listALL.AddRange(list);

                        Toast.MakeText(context, "Post deleted successfully", ToastLength.Short).Show();
                    }
                }
                catch (JSONException ee)
                {

                    ee.PrintStackTrace();
                }
              
            });
            builder.SetNegativeButton("Cancel", (s, e) => {
            

            });

            builder.Show();


            
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.layout_post,parent,false);
            PostsHolder postsHolder = new PostsHolder(view);
            
            return postsHolder;
        }

        
    }


    public class PostsHolder : RecyclerView.ViewHolder
    {

        public TextView txtName, txtDate, txtDesc, txtLike, txtComments;
        public CircleImageView imgProfile;
        public ImageView imgPost;
        public ImageButton btnPostOption, btnLike, btnComment;

        public PostsHolder(View itemView) : base(itemView)
        {
            txtName = itemView.FindViewById<TextView>(Resource.Id.txtPostName);
            txtDate = itemView.FindViewById<TextView>(Resource.Id.txtPostDate);
            txtDesc = itemView.FindViewById<TextView>(Resource.Id.txtPostDesc);
            txtLike = itemView.FindViewById<TextView>(Resource.Id.txtPostLikes);
            txtComments = itemView.FindViewById<TextView>(Resource.Id.txtPostComments);

            imgProfile = itemView.FindViewById<CircleImageView>(Resource.Id.imgPostProfile);
            imgPost = itemView.FindViewById<ImageView>(Resource.Id.imgPostPhoto);

            btnPostOption = itemView.FindViewById<ImageButton>(Resource.Id.btnPostOption);
            btnLike = itemView.FindViewById<ImageButton>(Resource.Id.btnPostLike);
            btnComment = itemView.FindViewById<ImageButton>(Resource.Id.btnPostComment);
            btnPostOption.Visibility = ViewStates.Gone;
        }

    }
    
   public class FilterHelper : Filter
    {
                
        static PostsAdapter adapter;

                
        public FilterHelper(PostsAdapter adapter)
        {
            FilterHelper.adapter = adapter;
                        
        }

        protected override FilterResults PerformFiltering(ICharSequence constraint)
        {

            var returnObj = new FilterResults();
            var results = new List<Post>();

            if(string.IsNullOrEmpty(constraint.ToString()))
            {
                results.AddRange(adapter.listALL);
                
            }
            else
            {
                foreach (Post post in adapter.listALL)
                {
                    if (post.Desc.ToLower().Contains(constraint.ToString())
                        || post.User.UserName.ToLower().Contains(constraint.ToString()))
                    {
                        results.Add(post);
                    }
                }

                
            }


            //cringe code #_#
           
            returnObj.Values = FromArray(results.Select(r => r.ToJavaObject()).ToArray());
            returnObj.Count = results.Count;
            

            return returnObj;
        }

        

        protected override void PublishResults(ICharSequence constraint, FilterResults results)
        {

            using(var values = results.Values)
            {
                //cringe code #_#
                adapter.list = values.ToArray<Java.Lang.Object>().Select(r => r.ToNetObject<Post>()).ToList();
                adapter.NotifyDataSetChanged();

                constraint.Dispose();
                results.Dispose();
            }


        }
    }
}


        