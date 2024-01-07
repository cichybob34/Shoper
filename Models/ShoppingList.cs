using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shoper.Models
{
    public class ShoppingList
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string OwnerId { get; set; }
        
        public User Owner { get; set; } = null!;

        public ICollection<Product> Products { get; } = new List<Product>();

        public bool HasAccess(string? userId)
        {
            return Equals(this.OwnerId, userId);
        }
    }
}