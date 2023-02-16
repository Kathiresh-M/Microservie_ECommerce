using Entities.Model;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.IRepository
{
    public interface IProductRepository
    {
        void CreateProduct(ProductModel productModel);
        void AddProductToCart(Cart cartDetails);
        ProductModel ProductExist(string productCategory);
        ProductModel GetProduct(Guid productId);
        bool CheckProductId(Guid chckProductId);
        void UpdateData(JsonPatchDocument data, Guid productId);
        Cart GetProductInCart(Guid productId);
        Wishlist GetProductFromWishlist(Guid Product);
        void DeleteProductInWishlist(Wishlist wish);
        bool CheckUserAndProductId(Guid userId, Guid productId);
        void DeleteProduct(Cart cart);
        int GetCartQuantityCount(Guid productId, Guid userId);
        Cart GetProductInCarts(Guid productId);
        bool ChckUserIdAndProductIdInWishlist(Guid productId, Guid userId);
        Guid ChckProductIdInCartAndGetCartId(Guid chckProductId, Guid userId);
        Cart GetCartIdUsingCartId(Guid Id);
        Guid ChckUserIdProductIdInWishlist(Guid productId, Guid userId);
        void UpdateCartQuantity(Cart cart);
        Wishlist GetwishlistIdUsingwishlistId(Guid Id);
        bool ChckProductIdInCart(Guid chckProductId, Guid userId);
        List<ProductModel> GetAllProductDetails();
        int GetProductQuantityCount(Guid productId);
        void AddProducttoWish(Wishlist wish);
        void UpdateProduct(ProductModel product);
        void Save();

        List<Cart> GetProducts(Guid id);
        Cart GetCartID(Guid cartId);
    }
}
