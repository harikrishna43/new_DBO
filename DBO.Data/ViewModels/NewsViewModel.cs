using DBO.Data.Models;

using System;
using System.Collections.Generic;

namespace DBO.Data.ViewModels
{
    public class NewsViewModel
    {
        public NewsViewModel()
        {

        }
        public NewsViewModel(News news)
        {
            Id = news.Id;
            Title = news.Title;
            ImagePath = news.ImagePath;
            Content = news.Description;
            CreatedAt = news.CreatedAt;
            Views = news.Views;
            UserId = news.UserId;
            CompanyId = news.CompanyId;
            Comments = news.Comments;
            Company = news.Company;
            User = news.User;
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public string ImagePath { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public string UserId { get; set; }

        public long Views { get; set; }

        public int CompanyId { get; set; }

        public bool IsCommentAllowed { get; set; }

        public Company Company { get; set; }

        public ApplicationUser User { get; set; }

        public ICollection<Comment> Comments { get; set; }


        public string ShortContent
        {
            get
            {
                return Content.Substring(0, Content.Length > 300 ? 300 : Content.Length);
            }
        }

        public bool IsLargeNews
        {
            get
            {
                return Content?.Length > 300;
            }
        }
    }
}
