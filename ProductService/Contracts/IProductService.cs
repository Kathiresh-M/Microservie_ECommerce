using Contracts.Responses;
using Entities.Dto;
using Entities.RequestDto;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IProductService
    {
        ProductResponse AddProduct(Guid adminId, CreateProductDto createProductDto, Guid tokenAdminId);
        ProductResponse UpdateProduct(Guid productId, Guid tokenUserId, ProductUpdateDto userData);
        WishListResponse AddProductToWishlist(AddProductToWishlist wishlist, Guid tokenUserId);
        CartResponses MoveWishToCart(WishlistToCartDto wishtocart, Guid tokenUserId);
        ProductResponse UpdateCountAndVisibility(JsonPatchDocument data, Guid productId);
        ProductResponse GetSingleProduct(string productName);
        WishListResponse DeleteProductInWishlist(Guid userId, Guid productId, Guid tokenUserId);
        CartResponses DeleteProductInCart(Guid userId, Guid productId, Guid tokenUserId);
        List<ProductReturnDto> GetAllProductDetails();
        CartResponses AddtoCartProducts(Guid userId, AddToCartDto product, Guid TokenUserId);

        List<CartReturnDto> Orderid(Guid id);
    }
}
