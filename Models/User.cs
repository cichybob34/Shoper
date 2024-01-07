using Microsoft.AspNetCore.Identity;

namespace Shoper.Models;

public class User : IdentityUser
{
    public ICollection<ShoppingList> ShoppingLists { get; } = new List<ShoppingList>();
}