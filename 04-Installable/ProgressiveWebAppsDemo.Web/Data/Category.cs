using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace ProgressiveWebAppsDemo.Web.Data
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short Id { get; private set; }

        [Required]
        public string Name { get; private set; }

        [JsonIgnore]
        public List<Movement> Movements { get; private set; }

        protected Category()
        {
        }

        public Category(string name)
        {
            this.Name = name;
        }
    }
}