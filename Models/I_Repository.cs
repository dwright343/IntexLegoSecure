namespace IntexLegoSecure.Models
{
    public interface I_Repository
    {
        IEnumerable<Product> Products { get; }
    }
}
