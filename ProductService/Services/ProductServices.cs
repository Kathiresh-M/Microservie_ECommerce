using AutoMapper;
using Contracts;
using Contracts.IRepository;
using Contracts.Responses;
using Entities.Dto;
using Entities.Model;
using Entities.RequestDto;
using log4net;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ProductServices : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILog _log;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="productRepository">Communication between user respository and service</param>
        /// <param name="mapper">used to map dto</param>
        /// <returns></returns>
        public ProductServices(IMapper mapper, IProductRepository productRepository)
        {
            _log = LogManager.GetLogger(typeof(ProductServices));
            _mapper = mapper;
            _productRepository = productRepository;
        }

        /// <summary>
        /// Create New Product details
        /// </summary>
        /// <param name="createProductDto"></param>
        /// <param name="adminId"></param>
        /// <param name="tokenAdminId"></param>
        /// <returns></returns>
        public ProductResponse AddProduct(Guid adminId, CreateProductDto createProductDto, Guid tokenAdminId)
        {
            _log.Info("Create a new product from service layer");

            if (!adminId.Equals(tokenAdminId))
                return new ProductResponse(false, "Admin Id not found", null);

            try
            { 
                var productExist = _productRepository.ProductExist(createProductDto.Name);

                if (productExist != null)
                    return new ProductResponse(false, "Product Name is already exists", null);

                    var product = new ProductModel()
                    {
                        Name = createProductDto.Name,
                        Description = createProductDto.Description,
                        Visibility = createProductDto.Visibility,
                        Price = createProductDto.Price,
                        Image = createProductDto.Image,
                        ProductCount = createProductDto.ProductCount,
                        Category = createProductDto.Category
                    };

                _productRepository.CreateProduct(product);
                _productRepository.Save();

                _log.Info("Product is added successfully");

                return new ProductResponse(true, null, product);
            }
            catch (Exception ex)
            {
                _log.Error("Something went wrong please try again "+ex);
                return new ProductResponse(false, "something went wrong ", null);
            }
        }

        /// <summary>
        /// Add to Cart product
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cart"></param>
        /// <param name="tokenUserId"></param>
        /// <returns></returns>
        public CartResponses AddtoCartProducts(Guid userId, AddToCartDto cart, Guid tokenUserId)
        {
            _log.Info("Add products to cart");

            if (!userId.Equals(tokenUserId))
                return new CartResponses(false, "User not found", null);

            try
            {
                bool checkProductIdExists = _productRepository.ChckProductIdInCart(cart.ProductId, userId);

                int checkProductQuantityCount = _productRepository.GetProductQuantityCount(cart.ProductId);

                if (checkProductQuantityCount < cart.Quantity)
                    return new CartResponses(false, "Given Product Quantity is out of stock", null);

                if (!checkProductIdExists)
                {
                    var Cart = new Cart()
                    {
                        Product_Id = cart.ProductId,
                        Quantity = cart.Quantity,
                        User_Id = tokenUserId
                    };

                    _productRepository.AddProductToCart(Cart);
                    _productRepository.Save();

                    return new CartResponses(true, "Successfully Added", Cart);
                }

                if (cart.Quantity == 0)
                    return new CartResponses(false, "Please give valuable Quantity", null);

                bool checkUserAndProductId = _productRepository.CheckUserAndProductId(userId, cart.ProductId);

                //checkProductQuantityCount = checkProductQuantityCount - cart.Quantity;

                int checkCartQuantityCount = _productRepository.GetCartQuantityCount(cart.ProductId, userId);

                checkCartQuantityCount = checkCartQuantityCount + cart.Quantity;

                if (checkProductQuantityCount < checkCartQuantityCount)
                    return new CartResponses(false, "Given Product Quantity is out of stock", null);

                var product = _productRepository.GetProductInCarts(cart.ProductId);

                if (product == null)
                    return new CartResponses(false, "Product not found", null);

                product.Product_Id = cart.ProductId;

                product.User_Id = tokenUserId;

                product.Quantity = checkCartQuantityCount;

                product.DateUpdated = DateTime.UtcNow;
                _productRepository.UpdateCartQuantity(product);
                _productRepository.Save();

                return new CartResponses(true, "Successfully updated In Cart", null);
            }
            catch(Exception exception)
            {
                _log.Error("Something went wrong please try again " + exception);
                return new CartResponses(false, "something went wrong ", null);
            }
        }

        /// <summary>
        /// Product Move to Wishlist to cart
        /// </summary>
        /// <param name="wishtocart"></param>
        /// <param name="tokenUserId"></param>
        /// <returns></returns>
        public CartResponses MoveWishToCart(WishlistToCartDto wishtocart, Guid tokenUserId)
        {
            try
            {
                var checkProductIdAndUserIdInWishList = _productRepository.ChckUserIdAndProductIdInWishlist(wishtocart.ProductId, wishtocart.UserId);

                bool checkProductIdExists = _productRepository.ChckProductIdInCart(wishtocart.ProductId, wishtocart.UserId);

                if (!checkProductIdAndUserIdInWishList)
                    return new CartResponses(false, "User Id ans Product Id was not found", null);

                int checkProductQuantityCount = _productRepository.GetProductQuantityCount(wishtocart.ProductId);

                if (checkProductQuantityCount < wishtocart.Quantity)
                    return new CartResponses(false, "Given Product Quantity is out of stock", null);

                if (!checkProductIdExists)
                {
                    var Cart = new Cart()
                    {
                        Product_Id = wishtocart.ProductId,
                        Quantity = wishtocart.Quantity,
                        User_Id = tokenUserId
                    };

                    _productRepository.AddProductToCart(Cart);
                    _productRepository.Save();

                    return new CartResponses(true, "Successfully Added", Cart);
                }

                if (wishtocart.Quantity == 0)
                    return new CartResponses(false, "Please give valuable Quantity", null);

                bool checkUserAndProductId = _productRepository.CheckUserAndProductId(wishtocart.UserId, wishtocart.ProductId);

                //checkProductQuantityCount = checkProductQuantityCount - cart.Quantity;

                int checkCartQuantityCount = _productRepository.GetCartQuantityCount(wishtocart.ProductId, wishtocart.UserId);

                checkCartQuantityCount = checkCartQuantityCount + wishtocart.Quantity;

                if (checkProductQuantityCount < checkCartQuantityCount)
                    return new CartResponses(false, "Given Product Quantity is out of stock", null);

                var product = _productRepository.GetProductInCarts(wishtocart.ProductId);

                if (product == null)
                    return new CartResponses(false, "Product not found", null);

                product.Product_Id = wishtocart.ProductId;

                product.User_Id = tokenUserId;

                product.Quantity = checkCartQuantityCount;

                product.DateUpdated = DateTime.UtcNow;
                _productRepository.UpdateCartQuantity(product);
                _productRepository.Save();

                return new CartResponses(true, "Product is already exist in cart, so given product is updated to cart", null);
            }
            catch(Exception exception)
            {
                _log.Error("Something went wrong please try again " + exception);
                return new CartResponses(false, "something went wrong ", null);
            }
        }

        /// <summary>
        /// Add to product to wishlist
        /// </summary>
        /// <param name="wishlist"></param>
        /// <param name="tokenUserId"></param>
        /// <returns></returns>
        public WishListResponse AddProductToWishlist(AddProductToWishlist wishlist, Guid tokenUserId)
        {
            _log.Info("Add products to cart");

            if (!(wishlist.UserId).Equals(tokenUserId))
                return new WishListResponse(false, "User not found", null);

            bool checkProductIdExists = _productRepository.ChckUserIdAndProductIdInWishlist(wishlist.Product, wishlist.UserId);

            if (!checkProductIdExists)
            {
                var wish = new Wishlist()
                {
                    Name = wishlist.Name,
                    Product = wishlist.Product,
                    UserId = tokenUserId
                };

                _productRepository.AddProducttoWish(wish);
                _productRepository.Save();

                return new WishListResponse(true, "Successfully Product Details are added to Wishlist", null);
            }

            return new WishListResponse(false, "Product is already exist in Wishlist", null);
        }

        /// <summary>
        /// Get all product details
        /// </summary>
        /// <returns></returns>
        public List<ProductReturnDto> GetAllProductDetails()
        {
            List<ProductReturnDto> productDetails = _mapper.Map<List<ProductReturnDto>>(_productRepository.GetAllProductDetails());
            return productDetails;
        }

        /// <summary>
        /// Get filtered product
        /// </summary>
        /// <param name="productName"></param>
        /// <returns></returns>
        public ProductResponse GetSingleProduct(string productName)
        {
            var getName = _productRepository.ProductExist(productName);

            if(getName == null)
            {
                return new ProductResponse(false, "Product name is not found", null);
            }
            return new ProductResponse(true,null,getName);
        }

        /// <summary>
        /// Update Product details
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="tokenUserId"></param>
        /// <param name="productData"></param>
        /// <returns></returns>
        public ProductResponse UpdateProduct(Guid productId, Guid tokenUserId, ProductUpdateDto productData)
        {
            try
            {
                var product = _productRepository.GetProduct(productId);

                if (product == null)
                    return new ProductResponse(false, "Product not found", null);

                if (!string.IsNullOrEmpty(productData.Name))
                    product.Name = productData.Name;

                if (!string.IsNullOrEmpty(productData.Description))
                    product.Description = productData.Description;

                if ((productData.Visibility) != null)
                    product.Visibility = productData.Visibility;

                if ((productData.Price) != null)
                    product.Price = productData.Price;

                if ((productData.Image) != null)
                    product.Image = productData.Image;

                if ((productData.ProductCount) != null)
                    product.ProductCount = productData.ProductCount;

                if ((productData.Category) != null)
                    product.Category = productData.Category;

                product.DateUpdated = DateTime.UtcNow;
                _productRepository.UpdateProduct(product);
                _productRepository.Save();

                return new ProductResponse(true, "Product Data Updated Successfully", product);
            }
            catch (Exception exception)
            {
                _log.Error("Something went wrong please try again " + exception);
                return new ProductResponse(false, "something went wrong ", null);
            }
        }

        /// <summary>
        /// Update Product Count and Visibilities
        /// </summary>
        /// <param name="data"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public ProductResponse UpdateCountAndVisibility(JsonPatchDocument data, Guid productId)
        {
            var checkproductId = _productRepository.CheckProductId(productId);

            if (!checkproductId)
                return new ProductResponse(false, "Product Id was not found",null);

            _productRepository.UpdateData(data, productId);

            return new ProductResponse(true, "Updated Successfully", null);
        }

        /// <summary>
        /// Delete product details in Cart
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="productId"></param>
        /// <param name="tokenUserId"></param>
        /// <returns></returns>
        public CartResponses DeleteProductInCart(Guid userId, Guid productId, Guid tokenUserId)
        {
            try
            {
                var getproductexists = _productRepository.ChckProductIdInCart(productId, userId);

                if (!getproductexists)
                    return new CartResponses(false, "Product and User Id was not found", null);

                var product = _productRepository.ChckProductIdInCartAndGetCartId(productId, userId);

                Cart getCartIdUsingCartId = _productRepository.GetCartIdUsingCartId(product);

                _productRepository.DeleteProduct(getCartIdUsingCartId);
                _productRepository.Save();

                return new CartResponses(true, null, getCartIdUsingCartId);
            }
            catch(Exception exception)
            {
                _log.Error("Something went wrong please try again " + exception);
                return new CartResponses(false, "something went wrong ", null);
            }
        }

        /// <summary>
        /// Delete product details in wishlist
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="tokenUserId"></param>
        /// <returns></returns>
        public WishListResponse DeleteProductInWishlist(Guid userId, Guid productId, Guid tokenUserId)
        {
            var getproductexist = _productRepository.ChckUserIdAndProductIdInWishlist(productId, userId);

            if(!getproductexist)
                return new WishListResponse(false, "Product Id and User Id is not found in Wishlist", null);

            var getProductFromWishlist = _productRepository.ChckUserIdProductIdInWishlist(productId, userId);

            Wishlist getCartIdUsingCartId = _productRepository.GetwishlistIdUsingwishlistId(getProductFromWishlist);

            _productRepository.DeleteProductInWishlist(getCartIdUsingCartId);
            _productRepository.Save();

            return new WishListResponse(true, null, getCartIdUsingCartId);
        }

    }
}
