using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace ProgressiveWebAppsDemo.Web.Data
{
    public class Movement
    {
        public Guid Id { get; private set; }

        [Required]
        public decimal Amount { get; private set; }

        [Required]
        public string Reason { get; private set; }

        public DateTime CreatedOn { get; private set; }

        [Required]
        public Category Category { get; private set; }

        [JsonIgnore]
        public byte[] Picture { get; private set; }

        public bool HasPicture => this.Picture?.Length > 0;

        protected Movement()
        {
        }

        public Movement(decimal amount, string reason, Category category, byte[] picture, DateTime? createdOn = null)
        {
            this.Id = Guid.NewGuid();
            this.Amount = amount;
            this.Reason = reason ?? string.Empty;
            this.Category = category;
            this.Picture = picture;
            this.CreatedOn = createdOn ?? DateTime.Now;
        }
    }
}