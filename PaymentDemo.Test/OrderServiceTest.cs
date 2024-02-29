using AutoMapper;
using FluentAssertions;
using FluentValidation;
using PaymentDemo.Manage.Configurations.Mapper;
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

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task Get_OrderById_ReturnResult(int orderId)
        {
            //Arrange abc
            var unitOfWork = MockUnitOfWork.GetMock();
            var orderService = new OrderService(unitOfWork.Object, _mapper, _orderValidator);

            //Act
            OrderViewModel order = await orderService.GetAsync(orderId);

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
            OrderViewModel order = await orderService.GetAsync(orderId);

            //Arrange            
            order.Should().BeNull();
        }
    }
}
