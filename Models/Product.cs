namespace Shoper.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public bool IsPurchased { get; set; }

        public ShoppingList? ShoppingList { get; set; }
    }
}
