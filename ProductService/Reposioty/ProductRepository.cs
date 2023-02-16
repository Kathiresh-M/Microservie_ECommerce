using Contracts.IRepository;
using Entities;
using Entities.Model;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDbContext _databaseContext;

        public ProductRepository(ProductDbContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public void CreateProduct(ProductModel productModel)
        {
            _databaseContext.Products.Add(productModel);
        }

        public void AddProductToCart(Cart cartDetails)
        {
            _databaseContext.Carts.Add(cartDetails);
        }

        public int GetProductQuantityCount(Guid productId)
        {
            //return _databaseContext.Products.Select(p => p.ProductCount).Count();
            return _databaseContext.Products.First(p => p.Id == productId).ProductCount;
        }

        public int GetCartQuantityCount(Guid productId, Guid userId)
        {
            //return _databaseContext.Products.Select(p => p.ProductCount).Count();
            return _databaseContext.Carts.First(p => p.Product_Id == productId && p.User_Id == userId).Quantity;
        }

        public bool ChckUserIdAndProductIdInWishlist(Guid productId, Guid userId)
        {
            return _databaseContext.Wishlist.Any(p => p.UserId == userId && p.Product == productId);
        }

        public Guid ChckUserIdProductIdInWishlist(Guid productId, Guid userId)
        {
            return _databaseContext.Wishlist.First(p => p.UserId == userId && p.Product == productId).Id;
        }

        public bool ChckProductIdInCart(Guid chckProductId, Guid userId)
        {
            return _databaseContext.Carts.Any(p => p.Product_Id == chckProductId && p.User_Id == userId);
        }

        public Guid ChckProductIdInCartAndGetCartId(Guid chckProductId, Guid userId)
        {
            return _databaseContext.Carts.First(p => p.Product_Id == chckProductId && p.User_Id == userId).Id;
        }

        public Cart GetCartIdUsingCartId(Guid Id)
        {
            return _databaseContext.Carts.First(p => p.Id == Id);
        }

        public Wishlist GetwishlistIdUsingwishlistId(Guid Id)
        {
            return _databaseContext.Wishlist.First(p => p.Id == Id);
        }

        public bool CheckProductId(Guid chckProductId)
        {
            return _databaseContext.Products.Any(p => p.Id == chckProductId);
        }

        public void UpdateData(JsonPatchDocument data, Guid productId)
        {
            var id = _databaseContext.Products.Find(productId);

            if (id != null)
            {
                data.ApplyTo(id);
                _databaseContext.SaveChanges();
            }
        }

        public ProductModel ProductExist(string productName)
        {
            return _databaseContext.Products.SingleOrDefault(p => p.Name == productName);
        }

        public List<ProductModel> GetAllProductDetails()
        {
            return _databaseContext.Products.Where(a => a.IsActive == true && a.Visibility == true && a.ProductCount != 0).ToList();
        }

        public bool CheckUserAndProductId(Guid userId, Guid productId)
        {
            return _databaseContext.Carts.Any(a => a.User_Id == userId && a.Product_Id == productId);
        }

        public void UpdateCartQuantity(Cart cart)
        {
            _databaseContext.Carts.Update(cart);
            
        }

        //Get product by product id
        public ProductModel GetProduct(Guid productId)
        {
            if (productId == null || productId == Guid.Empty)
                throw new ArgumentNullException(nameof(productId) + " was null in GetUser from repository");

            return _databaseContext.Products.SingleOrDefault(user => user.Id == productId);
        }

        public Cart GetProductInCarts(Guid productId)
        {
            if (productId == null || productId == Guid.Empty)
                throw new ArgumentNullException(nameof(productId) + " was null in GetUser from repository");

            return _databaseContext.Carts.SingleOrDefault(user => user.Product_Id == productId);
        }

        public Cart GetProductInCart(Guid productId)
        {
            if (productId == null || productId == Guid.Empty)
                throw new ArgumentNullException(nameof(productId) + " was null in GetUser from repository");

            return _databaseContext.Carts.SingleOrDefault(user => user.Product_Id == productId);
        }

        public Wishlist CheckNameFromWishlistExist(string name)
        {
            return _databaseContext.Wishlist.SingleOrDefault(user => user.Name == name);
        }

        public void AddProducttoWish(Wishlist wish)
        {
            _databaseContext.Wishlist.Add(wish);
        }

        public Wishlist GetProductFromWishlist(Guid Product)
        {
            return _databaseContext.Wishlist.SingleOrDefault(pro => pro.Product == Product);
        }

        public void DeleteProduct(Cart cart)
        {
            if (cart == null)
                throw new ArgumentNullException("product data was null in Deleteproduct from repository");

            _databaseContext.Carts.Remove(cart);
        }

        public void DeleteProductInWishlist(Wishlist wish)
        {
            if (wish == null)
                throw new ArgumentNullException("product data was null in Deleteproduct from repository");

            _databaseContext.Wishlist.Remove(wish);
        }

        public void UpdateProduct(ProductModel product)
        {
            if (product == null)
                throw new ArgumentNullException("User data was null in Deleteuser from repository");

            _databaseContext.Products.Update(product);
        }

        public void UpdateCart(Cart cart)
        {
            if (cart == null)
                throw new ArgumentNullException("User data was null in Deleteuser from repository");

            _databaseContext.Carts.Update(cart);
        }

        public void Save()
        {
            _databaseContext.SaveChanges();
        }

        public List<Cart> GetProducts(Guid id)
        {
            //return _databaseContext.Carts.Where(a => a.IsActive == true && a.User_Id == id).ToList();
            return _databaseContext.Carts.Where(a => a.IsActive == true && a.Id == id && a.IsPurchase == false).ToList();
        }

        public Cart GetCartID(Guid cartId)
        {
            if (cartId == null || cartId == Guid.Empty)
                throw new ArgumentNullException(nameof(cartId) + " was null in GetUser from repository");

            return _databaseContext.Carts.SingleOrDefault(user => user.Id == cartId);
        }
    }
}
