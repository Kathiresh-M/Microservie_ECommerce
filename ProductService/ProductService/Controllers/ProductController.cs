using AutoMapper;
using Contracts;
using Contracts.Responses;
using Entities.Dto;
using Entities.RequestDto;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace ProductService.Controllers
{
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILog _log;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="productService">Communication between service and controller</param>
        /// <param name="mapper">used to map dto</param>
        /// <returns></returns>
        public ProductController(IMapper mapper, IProductService productService)
        {
            _productService = productService;
            _mapper = mapper;
            _log = LogManager.GetLogger(typeof(ProductController));
        }

        /// <Summary>
        /// Create a new Product with Admin Authentication
        /// </Summary>
        /// <param name="adminid"></param>
        /// <response code="200">Product is Added Successfully</response>
        /// <response code="409">Product already exists</response>
        /// <response code="404">Admin Id was Not Found</response>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("api/product/admin/{adminid}")]
        public IActionResult CreateProduct(Guid adminid, [FromBody] CreateProductDto updateProduct)
        {
            _log.Info("Create a New Product with Admin Authentication");

            try
            {
                /*Guid tokenUserId;
                var isValidToken = Guid.TryParse(User.FindFirstValue("user_id"), out tokenUserId);*/

                Guid tokenUserId = Guid.Parse("3039a302-5cdd-48a7-b112-7b3d32b8b48d");
               
                ProductResponse response = _productService.AddProduct(adminid , updateProduct, tokenUserId);

                if(!response.IsSuccess && response.Message.Contains("found"))
                {
                    return NotFound(response.Message);
                }

                if (!response.IsSuccess && response.Message.Contains("exists"))
                {
                    return Conflict(response.Message);
                }

                _log.Info("Product is Added Successfully");
                return Created("Prduct is Added Successfully ", response.Productdto.Id);
            }
            catch (Exception exception)
            {
                _log.Error("Something went wrong please try again");
                return StatusCode(StatusCodes.Status500InternalServerError, exception);
            }
        }

        /// <summary>
        /// Get all Product details and Filter Product Name
        /// </summary>
        /// <response code="200">get user details</response>
        /// <response code="404">Given Product Not Found</response>
        [Authorize]
        [HttpGet]
        [Route("api/product")]
        public IActionResult GetAllProductDetails([FromQuery] string parameterResource= "")
        {
            _log.Info("Admin can Get all User Details");

            try 
            { 
                if(parameterResource != string.Empty)
                {
                    ProductResponse GetSingleProduct = _productService.GetSingleProduct(parameterResource);

                    if(!GetSingleProduct.IsSuccess)
                    {
                        return NotFound(GetSingleProduct.Message);
                    }

                    var user = _mapper.Map<ProductReturnDto>(GetSingleProduct.Productdto);

                    return Ok(user);
                }

                List<ProductReturnDto> productDetails = _productService.GetAllProductDetails();

                if (productDetails.Count == 0)
                {
                    return NoContent();
                }

                return Ok(productDetails);
            }
            catch (Exception ex)
            {
                _log.Error("Something went wrong", ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Update product count and product visibility
        /// </summary>
        /// <response code="200">Product Count and Visibility was updated</response>
        /// <response code="404">Product Id was not found</response>
        [Authorize(Roles = "Admin")]
        [HttpPatch]
        [Route("api/product/updatecountandvisibility/{productid}")]
        public IActionResult UpdateProductCountAndVisibility([FromBody] JsonPatchDocument jsonPatchDocument, Guid productid)
        {
            try
            {
                ProductResponse result = _productService.UpdateCountAndVisibility(jsonPatchDocument, productid);

                if (!result.IsSuccess && result.Message.Contains("found"))
                    return NotFound(result.Message);

                return Ok(result.Message);
            }
            catch(Exception exception)
            {
                _log.Error("Something went wrong", exception);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Update product details by using admin Id
        /// </summary>
        /// <param name="productid"></param>
        /// <param name="userData"></param>
        /// <response code="200">Updated details dto</response>
        /// <response code="404">User not found</response>
        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Route("api/product/{productid}")]
        public IActionResult UpdateProduct(Guid productid, [FromBody] ProductUpdateDto userData)
        {
            _log.Info("Update User Details");

            /*Guid tokenUserId;
            var isValidToken = Guid.TryParse(User.FindFirstValue("user_id"), out tokenUserId);*/

            Guid tokenUserId = Guid.Parse("3039a302-5cdd-48a7-b112-7b3d32b8b48d");

            try
            {
                ProductResponse response = _productService.UpdateProduct(productid, tokenUserId, userData);

                if (!response.IsSuccess)
                {
                    return NotFound(response.Message);
                }

                //var product = _mapper.Map<ProductReturnDto>(response.Productdto);

                return Ok("Updated Successfully");
            }
            catch (Exception exception)
            {
                _log.Error("Something went wrong", exception);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <Summary>
        /// Add Products to Cart
        /// </Summary>
        /// <param name="userid"></param>
        /// <param name="addToCart"></param>
        /// <response code="200">Product is Added Successfully</response>
        /// <response code="409">Product already exists</response>
        /// <response code="404">Given product was not found</response>
        [Authorize(Roles = "Customer")]
        [HttpPost]
        [Route("api/product/{userid}")]
        public IActionResult AddProductToCart(Guid userid, [FromBody] AddToCartDto addToCart)
        {
            _log.Info("Add Product to Cart ");

            /*Guid tokenUserId;
            var isValidToken = Guid.TryParse(User.FindFirstValue("user_id"), out tokenUserId);*/
           
            Guid tokenUserId = Guid.Parse("97088ab0-6c00-432d-81ce-87828a674b55");

            try
            {
                CartResponses response = _productService.AddtoCartProducts(userid, addToCart, tokenUserId);

                if (!response.IsSuccess && response.Message.Contains("stock"))
                    return NotFound(response.Message);

                if (!response.IsSuccess && response.Message.Contains("found"))
                    return NotFound(response.Message);

                if (!response.IsSuccess && response.Message.Contains("valuable"))
                    return BadRequest(response.Message);

                //return Ok("Successfully added product to cart "+response.Cartdto.Id);
                return Ok(response.Message);
            }
            catch(Exception exception)
            {
                _log.Error("Something went wrong", exception);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <Summary>
        /// Add Products to WishList
        /// </Summary>
        /// <param name="productToWishList"></param>
        /// <response code="200">Product is Added Successfully</response>
        /// <response code="409">Product already exists</response>
        [Authorize(Roles = "Customer")]
        [HttpPost]
        [Route("api/product/producttowishlist")]
        public IActionResult AddProductToWishList([FromBody] AddProductToWishlist productToWishList)
        {
            _log.Info("Add Product details To Wishlist");

            /*Guid tokenUserId;
            Guid.TryParse(User.FindFirstValue("user_id"), out tokenUserId);*/

            Guid tokenUserId = Guid.Parse("97088ab0-6c00-432d-81ce-87828a674b55");

            try
            {
                WishListResponse response = _productService.AddProductToWishlist(productToWishList, tokenUserId);

                if (!response.IsSuccess && response.Message.Contains("exist"))
                {
                    return Conflict(response.Message);
                }

                if (response.Message.Contains("found"))
                {
                    return NotFound(response.Message);
                }

                return Ok(response.Message);
            }
            catch(Exception exception)
            {
                _log.Error("Something went wrong", exception);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Add Prodct in Wishlist to cart
        /// </summary>
        /// <param name="wishtocart"></param>
        /// <response code="200">Product Added successfully</response>
        /// <response code="404">Id was not found</response>
        [Authorize(Roles = "Customer")]
        [HttpPost]
        [Route("api/product/wishlisttocart")]
        public IActionResult AddProductWishToCart([FromBody] WishlistToCartDto wishtocart)
        {
            _log.Info("Add Product to wishlist to cart");

            /*Guid tokenUserId;
            var tokenid = Guid.TryParse(User.FindFirstValue("user_id"), out tokenUserId);*/

            Guid tokenUserId = Guid.Parse("97088ab0-6c00-432d-81ce-87828a674b55");

            try
            {
                var response = _productService.MoveWishToCart(wishtocart, tokenUserId);

                if (!response.IsSuccess && response.Message.Contains("stock"))
                    return NotFound(response.Message);

                if (!response.IsSuccess && response.Message.Contains("found"))
                    return NotFound(response.Message);

                if (!response.IsSuccess && response.Message.Contains("valuable"))
                    return BadRequest(response.Message);

                return Ok(response.Message);
            }
            catch(Exception exception)
            {
                _log.Error("Something went wrong", exception);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Delete Product Details
        /// </summary>
        /// <param name="userId"></param>
        /// <response code="200">Deleted Successfully</response>
        /// <response code="404">Id was not found</response>
        [HttpDelete]
        [Authorize]
        [Route("api/product/{userid}/{productid}")]
        public IActionResult DeleteProductInCart(Guid userid, Guid productid)
        {
            _log.Info("Delete User");

            /*Guid tokenUserId;
            var isValidToken = Guid.TryParse(User.FindFirstValue("user_id"), out tokenUserId);*/

            Guid tokenUserId = Guid.Parse("97088ab0-6c00-432d-81ce-87828a674b55");

            if (productid == null || productid == Guid.Empty)
            {
                _log.Debug("Trying to delete user data with not a valid user ID by user Id: " + tokenUserId);
                return BadRequest("Not a valid user ID.");
            }

            try
            {
                CartResponses response = _productService.DeleteProductInCart(userid, productid, tokenUserId);

                if (!response.IsSuccess && response.Message.Contains("found"))
                {
                    return NotFound(response.Message);
                }

                _log.Info($"User with ID: {response.Cartdto.Id}, deleted successfully.");

                return Ok("Deleted Successsully in Cart " + productid);
            }
            catch (Exception exception)
            {
                _log.Error("Something went wrong", exception);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Delete Wishlist Details by product Id
        /// </summary>
        /// <param name="userId"></param>
        /// <response code="200">User Id</response>
        /// <response code="204">User not found</response>
        /// <response code="404">Product Id was not found</response>
        [HttpDelete]
        [Authorize]
        [Route("api/product/wishlist/{userid}/{productid}")]
        public IActionResult DeleteProductInWishlist(Guid userid, Guid productid)
        {
            _log.Info("Delete Product in Wishlist");

            /*Guid tokenUserId;
            var isValidToken = Guid.TryParse(User.FindFirstValue("user_id"), out tokenUserId);*/

            Guid tokenUserId = Guid.Parse("97088ab0-6c00-432d-81ce-87828a674b55");

            if (productid == null || productid == Guid.Empty)
            {
                _log.Debug("Trying to delete user data with not a valid user ID by user Id: " + tokenUserId);
                return BadRequest("Not a valid user ID.");
            }

            try
            {
                WishListResponse response = _productService.DeleteProductInWishlist(userid, productid, tokenUserId);

                if (!response.IsSuccess)
                {
                    return NotFound(response.Message);
                }

                _log.Info($"User with ID: {response.Wishlistdto.Id}, deleted successfully.");

                return Ok("Deleted Successsully in WishList " + productid);
            }
            catch (Exception exception)
            {
                _log.Error("Something went wrong", exception);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
