using AutoMapper;
using Contracts;
using Contracts.IRepository;
using Entities;
using Entities.Dto;
using Entities.Model;
using Entities.Profiles;
using Entities.RequestDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ProductService.Controllers;
using Repository;
using Services;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProductControllerTest
{
    public class ProductControllerTests
    {
        private readonly IMapper _mapper;
        private readonly ProductController _productController;
        private readonly IProductService _productService;
        private readonly IProductRepository _productRepositry;
        private readonly Mock<ILogger<ProductController>> _logger;
        private readonly IProductRepository _productRepository;

        public ProductControllerTests()
        {
            MapperConfiguration mapconfig = new MapperConfiguration(map =>
            {
                map.AddProfile(new ProductProfiles());
            });
            IMapper mapper = mapconfig.CreateMapper();
            _mapper = mapper;
            _logger = new Mock<ILogger<ProductController>>();
            _productRepository = new ProductRepository(GetDbContext());
            _productService = new ProductServices(_mapper, _productRepository);
            _productController = new ProductController(_mapper, _productService);

            string userId = "3039a302-5cdd-48a7-b112-7b3d32b8b48d";
            Mock<HttpContext> contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim("user_id",userId)
                           }, "TestAuthentication")));
            _productController.ControllerContext.HttpContext = contextMock.Object;
        }

        public ProductDbContext GetDbContext()
        {
            var option = new DbContextOptionsBuilder<ProductDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

            var context = new ProductDbContext(option);

            if (context != null)
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            context.Products.AddRange(new ProductModel
            {
                Id = Guid.Parse("4e5ed0c0-7dad-4198-a69e-e368485cbeb7"),
                Name = "Fan",
                Description = "Description",
                Visibility = true,
                Image = File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "C:\\Users\\Kathiresh M\\Downloads\\fan.jpg")),
                Price = 1299,
                ProductCount = 8,
                Category = "Electronics and Gadgets",
                DateCreated = DateTime.Now
            },
            new ProductModel
            {
                Id = Guid.Parse("310eeaa7-1cc1-4190-96e5-8c07f456cf82"),
                Name = "Shirt",
                Description = "light shirt",
                Visibility = true,
                Image = File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "C:\\Users\\Kathiresh M\\Downloads\\fan.jpg")),
                Price = 999,
                ProductCount = 0,
                Category = "Shirts",
                DateCreated = DateTime.Now
            },
            new ProductModel
            {
                Id = Guid.Parse("5e17c6ae-870d-432f-852d-47a61fe46f5e"),
                Name = "Keyboard",
                Description = "Wireless Keyboard",
                Visibility = true,
                Image = File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "C:\\Users\\Kathiresh M\\Downloads\\fan.jpg")),
                Price = 1999,
                ProductCount = 25,
                Category = "Electronics and Gadgets",
                DateCreated = DateTime.Now
            });

            context.Wishlist.AddRange(new Wishlist
            {
                Id = Guid.Parse("877b03b0-b55d-4a75-bd1a-6fbf0dfc0cfc"),
                UserId = Guid.Parse("97088ab0-6c00-432d-81ce-87828a674b55"),
                Name = "Shirt",
                Product = Guid.Parse("4e5ed0c0-7dad-4198-a69e-e368485cbeb7"),
                DateCreated = DateTime.Now
            },
            new Wishlist
            {
                Id = Guid.Parse("d5ee264d-1dbd-4ce1-845a-8263918ad5ae"),
                UserId = Guid.Parse("97088ab0-6c00-432d-81ce-87828a674b55"),
                Name = "Keyboard",
                Product = Guid.Parse("5e17c6ae-870d-432f-852d-47a61fe46f5e"),
                DateCreated= DateTime.Now
            });

            Cart cart = new Cart()
            {
                Id = Guid.Parse("69b38bee-8387-4c5f-911a-04aeb7c148cc"),
                User_Id = Guid.Parse("97088ab0-6c00-432d-81ce-87828a674b55"),
                Product_Id = Guid.Parse("4e5ed0c0-7dad-4198-a69e-e368485cbeb7"),
                Quantity = 5,
                DateCreated = DateTime.Now
            };

            Commonmodel common = new Commonmodel()
            {
                IsActive= true
            };

            context.Carts.Add(cart);
            context.SaveChanges();
            return context;
        }

        /// <summary>
        /// Create a new product with valid
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void CreateProduct_valid_ReturnCreatedStatus()
        {
            
            Guid id = Guid.Parse("3039a302-5cdd-48a7-b112-7b3d32b8b48d");

            CreateProductDto product = new CreateProductDto()
            {
                Name = "Watch",
                Description = "Noise ColorFit Pro 4 Advanced Bluetooth Calling Smart Watch",
                Visibility = true,
                Price = 2999,
                Image = File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "C:\\Users\\Kathiresh M\\Downloads\\fan.jpg")),
                ProductCount = 5,
                Category = "Electronics and Gadgets"
            };

            var result = _productController.CreateProduct(id, product);

            Assert.IsType<CreatedResult>(result);
        }

        /// <summary>
        /// Create a new product with invalid admin id
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void CreateProduct_InvalidAdmin_ReturnNotFoundStatus()
        {
            Guid id = Guid.Parse("3039a302-5cdd-48a7-b112-7b3d32b8b48b");

            CreateProductDto product = new CreateProductDto()
            {
                Name = "W",
                Description = "Fan model",
                Visibility = true,
                Price = 1999,
                Image = File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "C:\\Users\\Kathiresh M\\Downloads\\fan.jpg")),
                ProductCount = 4,
                Category = "Electronics and Gadgets"
            };

            var result = _productController.CreateProduct(id, product);

            var finalresult =  Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Admin Id not found", finalresult.Value);
        }

        /// <summary>
        /// Create a existing product (return conflict)
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void CreateProduct_InvalidProductExist_ReturnConflictStatus()
        {
            Guid id = Guid.Parse("3039a302-5cdd-48a7-b112-7b3d32b8b48d");

            CreateProductDto product = new CreateProductDto()
            {
                Name = "Fan",
                Description = "Fan model",
                Visibility = true,
                Price = 1999,
                Image = File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "C:\\Users\\Kathiresh M\\Downloads\\fan.jpg")),
                ProductCount = 4,
                Category = "Electronics and Gadgets"
            };

            var result = _productController.CreateProduct(id, product);

            var finalresult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("Product Name is already exists", finalresult.Value);
        }

        /// <summary>
        /// Get all product with valid
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void GetAllProduct_Valid_ReturnOkStatus()
        {
            var result = _productController.GetAllProductDetails();

            Assert.IsType<OkObjectResult>(result);
        }

        /// <summary>
        /// Get filtered product details with valid
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void GetSingleProduct_Valid_ReturnOkStatus()
        {
            string parameterResource = "Fan";
            var result = _productController.GetAllProductDetails(parameterResource);

            Assert.IsType<OkObjectResult>(result);
        }

        /// <summary>
        /// Get filtered product details with invalid product name
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void GetSingleProduct_InValidProductName_ReturnOkStatus()
        {
            string parameterResource = "Pen";
            var result = _productController.GetAllProductDetails(parameterResource);

            var finalresult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Product name was not found", finalresult.Value);
        }

        /// <summary>
        /// update product with valid
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void UpdateProduct_Valid_ReturnOkStatus()
        { 
            Guid productId = Guid.Parse("4e5ed0c0-7dad-4198-a69e-e368485cbeb7");

            ProductUpdateDto updateProduct = new ProductUpdateDto()
            {
                Name = "Fan",
                Description = "Super Power Fan",
                Visibility = true,
                Image = File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "C:\\Users\\Kathiresh M\\Downloads\\fan.jpg")),
                Price = 1299,
                ProductCount = 8,
                Category = "Electronics and Gadgets"
            };

            var result = _productController.UpdateProduct(productId, updateProduct);

            var finalresult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Updated Successfully", finalresult.Value);
        }

        /// <summary>
        /// Update product with invalid product id
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void UpdateProduct_InValidProductId_ReturnNotFoundStatus()
        {
            Guid productId = Guid.Parse("4e5ed0c0-7dad-4198-a69e-e368485cbebb");

            ProductUpdateDto updateProduct = new ProductUpdateDto()
            {
                Name = "Fan",
                Description = "Super Power Fan",
                Visibility = true,
                Image = File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "C:\\Users\\Kathiresh M\\Downloads\\fan.jpg")),
                Price = 1299,
                ProductCount = 8,
                Category = "Electronics and Gadgets"
            };

            var result = _productController.UpdateProduct(productId, updateProduct);

            var finalresult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Product not found", finalresult.Value);
        }

        /// <summary>
        /// Product add to cart, where already exist in cart
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void AddProductToCart_ValidAlreadyExistInCart_ReturnOkStatus()
        {
            /*string usersId = "97088ab0-6c00-432d-81ce-87828a674b55";
            Mock<HttpContext> contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(ClaimTypes.NameIdentifier,usersId),
                                        new Claim(ClaimTypes.Role,"Admin"),
                           }, "TestAuthentication")));

            _productController.ControllerContext.HttpContext = contextMock.Object;*/

            Guid userId = Guid.Parse("97088ab0-6c00-432d-81ce-87828a674b55");

            AddToCartDto AddToCart = new AddToCartDto()
            {
                ProductId = Guid.Parse("4e5ed0c0-7dad-4198-a69e-e368485cbeb7"),
                Quantity = 1
            };

            var result = _productController.AddProductToCart(userId, AddToCart);

            var finalresult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Successfully updated In Cart", finalresult.Value);
        }

        /// <summary>
        /// Product add to cart, where quantity is invalid (return badrequest)
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void AddProductToCart_InValidQuantity_ReturnBadRequestStatus()
        {
            Guid userId = Guid.Parse("97088ab0-6c00-432d-81ce-87828a674b55");

            AddToCartDto AddToCart = new AddToCartDto()
            {
                ProductId = Guid.Parse("4e5ed0c0-7dad-4198-a69e-e368485cbeb7"),
                Quantity = 0
            };

            var result = _productController.AddProductToCart(userId, AddToCart);

            var finalresult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Please give valuable Quantity", finalresult.Value);
        }

        /// <summary>
        /// Product add to cart with invalid product quantity
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void AddProductToCart_InValidProductQuantity_ReturnNotFoundStatus()
        {
            Guid userId = Guid.Parse("97088ab0-6c00-432d-81ce-87828a674b55");

            AddToCartDto AddToCart = new AddToCartDto()
            {
                ProductId = Guid.Parse("4e5ed0c0-7dad-4198-a69e-e368485cbeb7"),
                Quantity = 10
            };

            var result = _productController.AddProductToCart(userId, AddToCart);

            var finalresult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Given Product Quantity is out of stock", finalresult.Value);
        }

        /// <summary>
        /// Product add to cart with invalid user id
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void AddProductToCart_InValiUserId_ReturnNotFoundStatus()
        {
            Guid userId = Guid.Parse("15f8a71f-71c1-41f8-bc43-dc19dca05149");

            AddToCartDto AddToCart = new AddToCartDto()
            {
                ProductId = Guid.Parse("4e5ed0c0-7dad-4198-a69e-e368485cbeb7"),
                Quantity = 5
            };

            var result = _productController.AddProductToCart(userId, AddToCart);

            var finalresult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found", finalresult.Value);
        }

        /// <summary>
        /// Product add to cart with valid
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void AddProductToCart_Valid_ReturnOkStatus()
        {

            Guid userId = Guid.Parse("97088ab0-6c00-432d-81ce-87828a674b55");

            AddToCartDto AddToCart = new AddToCartDto()
            {
                ProductId = Guid.Parse("5e17c6ae-870d-432f-852d-47a61fe46f5e"),
                Quantity = 5
            };

            var result = _productController.AddProductToCart(userId, AddToCart);

            var finalresult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Successfully Added", finalresult.Value);
        }

        /// <summary>
        /// Product add to wishlist with valid
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void AddProductToWishlist_Valid_ReturnOkStatus()
        {
            AddProductToWishlist AddToWishList = new AddProductToWishlist()
            {
                UserId = Guid.Parse("97088ab0-6c00-432d-81ce-87828a674b55"),
                Name = "Keyboards",
                Product = Guid.Parse("5e17c6ae-870d-432f-8526-47a61fe46f5e")
            };

            var result = _productController.AddProductToWishList(AddToWishList);

            var finalresult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Successfully Product Details are added to Wishlist", finalresult.Value);
        }

        /// <summary>
        /// Product add to wishlist, where already exist in wishlist
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void AddProductToWishlist_ValidButAlreadyExist_ReturnConflictStatus()
        {
            Guid userId = Guid.Parse("97088ab0-6c00-432d-81ce-87828a674b55");

            AddProductToWishlist AddToWishList = new AddProductToWishlist()
            {
                UserId = Guid.Parse("97088ab0-6c00-432d-81ce-87828a674b55"),
                Name = "Shirt",
                Product = Guid.Parse("4e5ed0c0-7dad-4198-a69e-e368485cbeb7")
            };

            var result = _productController.AddProductToWishList(AddToWishList);

            var finalresult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("Product is already exist in Wishlist", finalresult.Value);
        }

        /// <summary>
        /// Product Add to Wishlist with invalid userid
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void AddProductToWishlist_InValidUserId_ReturnNotFoundStatus()
        {

            AddProductToWishlist AddToWishList = new AddProductToWishlist()
            {
                UserId = Guid.Parse("97088ab0-6c00-432d-81ce-87828a674b54"),
                Name = "Shirt",
                Product = Guid.Parse("4e5ed0c0-7dad-4198-a69e-e368485cbeb7")
            };

            var result = _productController.AddProductToWishList(AddToWishList);

            var finalresult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found", finalresult.Value);
        }

        /// <summary>
        /// Delete Product in cart with valid 
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void DeleteProductInCart_Valid_ReturnOkStatus()
        {
            Guid userId = Guid.Parse("97088ab0-6c00-432d-81ce-87828a674b55");
            Guid productId = Guid.Parse("4e5ed0c0-7dad-4198-a69e-e368485cbeb7");

            var result = _productController.DeleteProductInCart(userId, productId);

            var finalresult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Deleted Successsully in Cart "+productId , finalresult.Value);
        }

        /// <summary>
        /// Delete product in cart , where user id and product id was invalid
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void DeleteProductInCart_InValidUserIdAndProductId_ReturnNotFoundStatus()
        {
            Guid userId = Guid.Parse("97088ab0-6c00-432d-81ce-87828a674b54");
            Guid productId = Guid.Parse("4e5ed0c0-7dad-4198-a69e-e368485cbeb4");

            var result = _productController.DeleteProductInCart(userId, productId);

            var finalresult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Product and User Id was not found", finalresult.Value);
        }

        /// <summary>
        /// delete product in wishlist
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void DeleteProductInWishlist_Valid_ReturnOkStatus()
        {
            Guid userId = Guid.Parse("97088ab0-6c00-432d-81ce-87828a674b55");
            Guid productId = Guid.Parse("4e5ed0c0-7dad-4198-a69e-e368485cbeb7");

            var result = _productController.DeleteProductInWishlist(userId, productId);

            var finalresult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Deleted Successsully in WishList " + productId, finalresult.Value);
        }

        /// <summary>
        /// Delete product in wishlist, where userid and product id was invalid
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void DeleteProductInWishList_InValidUserIdAndProductId_ReturnNotFoundStatus()
        {
            Guid userId = Guid.Parse("97088ab0-6c00-432d-81ce-87828a674b54");
            Guid productId = Guid.Parse("4e5ed0c0-7dad-4198-a69e-e368485cbeb4");

            var result = _productController.DeleteProductInWishlist(userId, productId);

            var finalresult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Product Id and User Id is not found in Wishlist", finalresult.Value);
        }

        /// <summary>
        /// Update product count and visibility with valid
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void UpdateCountAndVisibility_Valid_ReturnOkStatus()
        {
            Guid productId = Guid.Parse("5e17c6ae-870d-432f-852d-47a61fe46f5e");

            JsonPatchDocument jsonpatch = new JsonPatchDocument();

            jsonpatch.Replace("/Visibility","false");

            var result = _productController.UpdateProductCountAndVisibility(jsonpatch, productId);

            var finalresult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Updated Successfully", finalresult.Value);
        }

        /// <summary>
        /// Update product count and visibility with invalid product id
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void UpdateCountAndVisibility_InValidProductId_ReturnOkStatus()
        {
            Guid productId = Guid.Parse("5e17c6ae-870d-432f-852d-47a61fe46f54");

            JsonPatchDocument jsonpatch = new JsonPatchDocument();

            jsonpatch.Replace("/Visibility", "true");

            var result = _productController.UpdateProductCountAndVisibility(jsonpatch, productId);

            var finalresult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Product Id was not found", finalresult.Value);
        }

        /// <summary>
        /// Product move wishlist to cart with valid
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void MoveWishlistToCart_ValidExistProductInCart_ReturnOkStatus()
        {
            WishlistToCartDto wishtocart = new WishlistToCartDto()
            {
                UserId = Guid.Parse("97088ab0-6c00-432d-81ce-87828a674b55"),
                ProductId = Guid.Parse("4e5ed0c0-7dad-4198-a69e-e368485cbeb7"),
                Quantity = 1
            };

            var result = _productController.AddProductWishToCart(wishtocart);

            var finalresult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Product is already exist in cart, so given product is updated to cart", finalresult.Value);
        }

        /// <summary>
        /// Move product in wishlist ticart with valid
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void MoveWishlistToCart_Valid_ReturnOkStatus()
        {
            WishlistToCartDto wishtocart = new WishlistToCartDto()
            {
                UserId = Guid.Parse("97088ab0-6c00-432d-81ce-87828a674b55"),
                ProductId = Guid.Parse("5e17c6ae-870d-432f-852d-47a61fe46f5e"),
                Quantity = 1
            };

            var result = _productController.AddProductWishToCart(wishtocart);

            var finalresult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Successfully Added", finalresult.Value);
        }

        /// <summary>
        /// Move product in wishlist ti cart where invalid userid and product id
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void MoveWishlistToCart_InValidUserIdAndProductId_ReturnNotFoundStatus()
        {
            WishlistToCartDto wishtocart = new WishlistToCartDto()
            {
                UserId = Guid.Parse("97088ab0-6c00-432d-81ce-87828a674b5b"),
                ProductId = Guid.Parse("5e17c6ae-870d-432f-852d-47a61fe46f5b"),
                Quantity = 1
            };

            var result = _productController.AddProductWishToCart(wishtocart);

            var finalresult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User Id ans Product Id was not found", finalresult.Value);
        }

        /// <summary>
        /// Move wishlist to cart with invalid quantity
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void MoveWishlistToCart_InValidOutOfStock_ReturnNotFoundStatus()
        {
            WishlistToCartDto wishtocart = new WishlistToCartDto()
            {
                UserId = Guid.Parse("97088ab0-6c00-432d-81ce-87828a674b55"),
                ProductId = Guid.Parse("5e17c6ae-870d-432f-852d-47a61fe46f5e"),
                Quantity = 30
            };

            var result = _productController.AddProductWishToCart(wishtocart);

            var finalresult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Given Product Quantity is out of stock", finalresult.Value);
        }
    }
}
