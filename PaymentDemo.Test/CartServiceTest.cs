using AutoMapper;
using FluentAssertions;
using FluentValidation;
using PaymentDemo.Manage.Configurations.Mapper;
using PaymentDemo.Manage.Enums;
using PaymentDemo.Manage.Models;
using PaymentDemo.Manage.Services.Implements;
using PaymentDemo.Test.Mocks;

namespace PaymentDemo.Test
{
    public class CartServiceTest
    {
        private IMapper _mapper;
        private IValidator<CartViewModel> _cartValidator;

        public CartServiceTest()
        {
            if (_mapper == null)
            {
                var mappingConfig = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new ServiceProfile());
                });
                _mapper = mappingConfig.CreateMapper();
            }

            if (_cartValidator == null)
            {
                _cartValidator = new CartViewModelValidator();
            }
        }

        [Fact]
        public async Task AddToCart_AddOneItemToNewCart_Success()
        {
            //Arrange            
            var cartItems = new List<CartItemViewModel>()
            {
                new CartItemViewModel(1,3, 100),
            };

            var cart = new CartViewModel(0,1, CartStatus.Created, cartItems);
            var unitOfWork = MockUnitOfWork.GetMock();
            var cartService = new CartService(unitOfWork.Object, _mapper, _cartValidator);

            //Act
            bool result = await cartService.AddToCartAsync(cart);

            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task AddToCart_AddItemsToNewCart_Success()
        {
            //Arrange                        
            var cartItems = new List<CartItemViewModel>()
            {
                new CartItemViewModel(1,3, 100),
                new CartItemViewModel(2,1, 100),
                new CartItemViewModel(3,2, 100),
            };

            var cart = new CartViewModel(0,1, CartStatus.Created, cartItems);

            var unitOfWork = MockUnitOfWork.GetMock();
            var cartService = new CartService(unitOfWork.Object, _mapper, _cartValidator);

            //Act
            bool result = await cartService.AddToCartAsync(cart);

            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task AddToCart_AddOneItemToExistCart_Success()
        {
            //Arrange
            var newCartItems = new List<CartItemViewModel>()
            {
                new CartItemViewModel(1,3, 100),
            };

            var cartRecord = new CartViewModel(1,1, CartStatus.Created, newCartItems);

            var unitOfWork = MockUnitOfWork.GetMock();
            var cartService = new CartService(unitOfWork.Object, _mapper, _cartValidator);

            //Act
            bool result = await cartService.AddToCartAsync(cartRecord);

            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task AddToCart_AddItemsToExistCart_Success()
        {
            //Arrange
            // add product item to cart

            var cartItems = new List<CartItemViewModel>()
            {
                new CartItemViewModel(1,3, 100),
                new CartItemViewModel(2,1, 100),
                new CartItemViewModel(3,2, 100),
            };

            var cartItem = new CartViewModel(1,1, CartStatus.Created, cartItems);

            var unitOfWork = MockUnitOfWork.GetMock();
            var cartService = new CartService(unitOfWork.Object, _mapper, _cartValidator);

            //Act
            bool result = await cartService.AddToCartAsync(cartItem);

            //Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(1,0)]
        [InlineData(1,-1)]
        [InlineData(0,0)]
        [InlineData(-1,1)]
        public async Task AddToCart_AddItemToNewCart__Fail(int productId,int number)
        {
            //Arrange
            // add product item to cart
            var cartItems = new List<CartItemViewModel>()
            {
                new CartItemViewModel(productId,number, 100),                
            };

            var cartItem = new CartViewModel(0, 1, CartStatus.Created, cartItems);
            var unitOfWork = MockUnitOfWork.GetMock();
            var cartService = new CartService(unitOfWork.Object, _mapper, _cartValidator);

            //Act
            bool result = await cartService.AddToCartAsync(cartItem);

            //Assert
            result.Should().BeFalse();
        }
        
        [Theory]
        [InlineData(-99,100)]
        [InlineData(-1, 100)]
        [InlineData(0, 100)]
        [InlineData(1, 100)]
        [InlineData(2, 100)]
        public async Task UpdateCart_ChangeCartItem_Success(int number, decimal price)
        {            
            //Arrange
            var cartItem = new AddToCartViewModel(1,1,number, price);
            var unitOfWork = MockUnitOfWork.GetMock();
            var cartService = new CartService(unitOfWork.Object, _mapper, _cartValidator);

            //Act
            bool result = await cartService.ChangeCartItemAsync(cartItem);

            //Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(1,null)]
        [InlineData(null,1)]
        [InlineData(1,1)]
        public async Task DeleteCart_DeleteCartByUserId_Success(int? userId, int? cartId)
        {
            //Arrange            
            var unitOfWork = MockUnitOfWork.GetMock();
            var cartService = new CartService(unitOfWork.Object, _mapper, _cartValidator);

            //Act
            bool result = await cartService.DeleteCartAsync(userId, cartId);

            //Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task GetCart_GetById_ReturnValue(int cartId)
        {
            //Arrange
            var unitOfWork = MockUnitOfWork.GetMock();            
            var cartService = new CartService(unitOfWork.Object, _mapper, _cartValidator);

            //Act
            var cartDetail = await cartService.GetCartAsync(cartId, false);
            
            //Assert
            cartDetail.Should().NotBeNull();
            cartDetail.Should().BeOfType<CartViewModel>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(99)]
        public async Task GetCart_GetById_ReturnNull(int cartId)
        {
            //Arrange
            var unitOfWork = MockUnitOfWork.GetMock();
            var cartService = new CartService(unitOfWork.Object, _mapper, _cartValidator);
            //Act
            var cartDetail = await cartService.GetCartAsync(cartId, false);

            //Assert
            cartDetail.Should().BeNull();            
        }
    }
}
