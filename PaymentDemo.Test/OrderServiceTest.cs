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
    public class OrderServiceTest
    {
        private IMapper _mapper;
        private IValidator<OrderViewModel> _orderValidator;

        public OrderServiceTest()
        {
            if (_mapper == null)
            {
                var mappingConfig = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new ServiceProfile());
                });
                _mapper = mappingConfig.CreateMapper();
            }

            if (_orderValidator == null)
            {
                _orderValidator = new OrderViewModelValidator();
            }
        }

        [Fact]       
        public async Task Order_GetOrders_ReturnResult()
        {
            //Arrange add comment
            var unitOfWork = MockUnitOfWork.GetMock();
            var orderService = new OrderService(unitOfWork.Object, _mapper, _orderValidator);
            var query = new OrderQueryParams()
            {
                PageNumber = 1,
                PageSize = 10
            };
            //Act
            PagedResponse<OrderViewModel> order = await orderService.GetOrdersAsync(query);

            //Arrange
            order.Should().NotBeNull();            
            order.Items.Should()                
                .HaveCount(2);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task Get_OrderById_ReturnResult(int orderId)
        {
            //Arrange add comment
            var unitOfWork = MockUnitOfWork.GetMock();
            var orderService = new OrderService(unitOfWork.Object, _mapper, _orderValidator);

            //Act
            OrderViewModel order = await orderService.GetOrderAsync(orderId);

            //Arrange
            order.Should().BeOfType<OrderViewModel>();
            order.Should().NotBeNull();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(99)]
        public async Task Get_OrderById_ReturnNull(int orderId)
        {
            //Arrange
            var unitOfWork = MockUnitOfWork.GetMock();
            var orderService = new OrderService(unitOfWork.Object, _mapper, _orderValidator);

            //Act
            OrderViewModel order = await orderService.GetOrderAsync(orderId);

            //Arrange            
            order.Should().BeNull();
        }

        [Fact]
        public async Task Create_CreateOrder_COD_ReturnSuccess()
        {
            //Arrange
            var unitOfWork = MockUnitOfWork.GetMock();
            var orderService = new OrderService(unitOfWork.Object, _mapper, _orderValidator);
            var cartItems = new List<CartItemViewModel>()
            {
                new CartItemViewModel(1,10,100),
                new CartItemViewModel(1,1,50)
            };
            var newOrder = new OrderViewModel()
            {
                Cart = new CartViewModel(1, 1, Manage.Enums.CartStatus.Created, cartItems),
                User = new UserViewModel(1, "ratio", "ratio last", "0131231", "abc.jpg", "Ha noi"),
                ShippingAddress = "HITC - Pham Hung - Viet Nam",
                PhoneNumber = "1234567890",
                PaymentType = PaymentType.COD,                
            };

            //Act
            OrderViewModel result = await orderService.CreateOrderAsync(newOrder);

            //Assert
            result.Should().NotBeNull();
            result.ShipmentStatus.Should().Be(ShipmentStatus.Inprogress);
            result.PaymentStatus.Should().Be(PaymentStatus.Pending);
            result.OrderStatus.Should().Be(OrderStatus.Created);
            result.Id.Should().BeGreaterThan(0);            
        }

        [Theory]
        [InlineData(1,1,CartStatus.Deleted)]
        [InlineData(0,0,CartStatus.Created)]
        [InlineData(-1,1,CartStatus.Created)]
        [InlineData(1,-1,CartStatus.Created)]
        public async Task Create_CreateOrder_COD_ReturnFailure(int cartId, int userId, CartStatus cartStatus)
        {
            //Arrange
            var unitOfWork = MockUnitOfWork.GetMock();
            var orderService = new OrderService(unitOfWork.Object, _mapper, _orderValidator);
            var cartItems = new List<CartItemViewModel>()
            {
                new CartItemViewModel(1,10,100),
                new CartItemViewModel(1,1,50)
            };
            var newOrder = new OrderViewModel()
            {
                Cart = new CartViewModel(cartId,userId, cartStatus, cartItems),
                User = new UserViewModel(userId, "ratio", "ratio last", "0131231", "abc.jpg", "Ha noi"),
                ShippingAddress = "HITC - Pham Hung - Viet Nam",
                PhoneNumber = "1234567890",
                PaymentType = PaymentType.COD,
            };

            //Act
            OrderViewModel? result = await orderService.CreateOrderAsync(newOrder);

            //Assert
            result.Should().BeNull();            
        }        
    }
}
