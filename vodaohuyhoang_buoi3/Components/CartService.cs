using System.Security.Claims;
using vodaohuyhoang_buoi3.Models;

namespace vodaohuyhoang_buoi3.Components
{
    public class CartService
    {
        private readonly ApplicationDbContext _context;

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        //public int GetCartItemCount(ClaimsPrincipal user)
        //{
        //    if (user.Identity.IsAuthenticated)
        //    {
        //        var userId = _context.Users.FirstOrDefault(u => u.UserName == user.Identity.Name)?.Id;
        //        return _context.CartItem.Where(c => c. == userId).Sum(c => c.Quantity);
        //    }
        //    return 0;
        //}
    }
}
