using System.Security.Claims;

namespace vodaohuyhoang_buoi3.Components
{
    public interface ICartService
    {
        int GetCartItemCount(ClaimsPrincipal user);

    }
}
