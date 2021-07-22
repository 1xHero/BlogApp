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

namespace BlogApp.Adapters
{
    public class PostsAdapter:RecyclerView.Adapter
    {
        


         Context context;
        public List<Post> list;
        public List<Post> listALL;

        public PostsAdapter(Context context, List<Post> list)
        {
            this.context = context;
            this.list = list;
            this.listALL = new List<Post>(list);
        }

        public override int ItemCount => list.Count;
        

        //public Filter Filter
        //{
        //    get { return FilterHelper.newInstance(this.listALL, this); }
        //    //set { return FilterHelper.}
        //}


        //public Filter GetFilter()
        //{
        //    return Filter;
        //}

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

        public bool OnQueryTextChange(string newText)
        {
            
            return false;
        }

        public bool OnQueryTextSubmit(string query)
        {
            return false;
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
    /*
   public class FilterHelper : Filter
    {
        static List<Post> currentList;
        static PostsAdapter adapter;

        public static FilterHelper newInstance(List<Post> currentList, PostsAdapter adapter)
        {
            FilterHelper.adapter = adapter;
            FilterHelper.currentList = currentList;
            return new FilterHelper();
        }

        protected override FilterResults PerformFiltering(ICharSequence constraint)
        {


            List<Post> filteredList = new List<Post>();

            if(string.IsNullOrEmpty(constraint.ToString()))
            {
                filteredList.AddRange(currentList);
            }
            else
            {
                foreach (Post post in currentList)
                {
                    if (post.Desc.ToLower().Contains(constraint.ToString())
                        || post.User.UserName.ToLower().Contains(constraint.ToString()))
                    {
                        filteredList.Add(post);
                    }
                }

                
            }
            FilterResults results = new FilterResults();
            results.Count = filteredList.Count;
            results.Values = (Java.Lang.Object)(object)filteredList;

            return results;
        }

        protected override void PublishResults(ICharSequence constraint, FilterResults results)
        {
            adapter.list.Clear();
            adapter.list.AddRange((IEnumerable<Post>)results.Values);
            adapter.NotifyDataSetChanged();
        }
    }*/
}


        