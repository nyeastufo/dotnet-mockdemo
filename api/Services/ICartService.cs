using api.Models;

namespace api.Services
{
    public interface ICartService
    {
        double Total();
        IEnumerable<CartItem> Items();
    }
}
