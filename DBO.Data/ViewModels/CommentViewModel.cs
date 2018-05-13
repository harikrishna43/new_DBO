using DBO.Data.Models;
using DBO.Data.ValidationAttributes;

using System;

namespace DBO.Data.ViewModels
{
    public class CommentViewModel
    {
        public CommentViewModel()
        {

        }

        public CommentViewModel(Comment comment)
        {
            Content = comment.Content;
            UserId = comment.UserId;
            NewsId = comment.NewsId;
            CreatedAt = comment.CreatedAt;
        }

        [LocalizedRequired("CommentContentRequired")]
        public string Content { get; set; }

        public string UserId { get; set; }

        public DateTime CreatedAt { get; set; }

        public int NewsId { get; set; }
    }
}
