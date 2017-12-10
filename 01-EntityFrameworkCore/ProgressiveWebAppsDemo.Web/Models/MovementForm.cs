using System;
using Microsoft.AspNetCore.Http;

namespace ProgressiveWebAppsDemo.Web.Models
{
    public class MovementForm
    {
        public short CategoryId { get; set; }

        public decimal Amount { get; set; }

        public string Type { get; set; }

        public string Reason { get; set; }

        public IFormFile Picture { get; set; }

        public DateTime? CreatedOn { get; set; }
    }
}