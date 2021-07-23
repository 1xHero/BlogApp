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

namespace BlogApp.Adapters
{
    public class PostsAdapter:RecyclerView.Adapter,IFilterable
    {
         Context context;
        public List<Post> list;
        public List<Post> listALL;

        public Filter Filter { get; private set; }


        public PostsAdapter(Context context, IEnumerable<Post> list)
        {
            this.context = context;
            this.list = (List<Post>)list;
            this.listALL = new List<Post>(list);


            Filter = new FilterHelper(this);

        }

        public override int ItemCount => list.Count;


       public Filter GetFilter()
        {
            return Filter;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
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


        