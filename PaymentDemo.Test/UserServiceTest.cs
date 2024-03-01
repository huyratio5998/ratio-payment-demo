using AutoMapper;
using FluentValidation;
using PaymentDemo.Manage.Configurations.Mapper;
using PaymentDemo.Manage.Models;
using PaymentDemo.Test.Mocks;
using FluentAssertions;
using PaymentDemo.Manage.Services.Implements;

namespace PaymentDemo.Test
{
    public class UserServiceTest
    {
        private readonly IMapper _mapper;
        private readonly IValidator<UserViewModel> _userValidator;

        public UserServiceTest()
        {
            if (_mapper == null)
            {
                var mappingConfig = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new ServiceProfile());
                });
                _mapper = mappingConfig.CreateMapper();
            }

            if (_userValidator == null)
            {
                _userValidator = new UserViewModelValidator();
            }
        }

        [Fact]
        public async Task Get_GetUsers_ReturnUsers()
        {
            //Arrange
            var unitOfWork = MockUnitOfWork.GetMock();
            var userService = new UserService(unitOfWork.Object, _mapper, _userValidator);

            //Act
            List<UserViewModel> users = await userService.GetUsersAsync();

            //Assert
            users.Should().HaveCount(2);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task Get_GetUserById_ReturnUser(int id)
        {
            //Arrange
            var unitOfWork = MockUnitOfWork.GetMock();
            var userService = new UserService(unitOfWork.Object, _mapper, _userValidator);

            //Act
            var users = await userService.GetUserAsync(id);
            
            //Assert
            users.Should().NotBeNull();
            users.Should().BeOfType<UserViewModel>();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(99)]
        public async Task Get_GetUserById_ReturnNull(int id)
        {
            //Arrange
            var unitOfWork = MockUnitOfWork.GetMock();
            var userService = new UserService(unitOfWork.Object, _mapper, _userValidator);

            //Act
            var users = await userService.GetUserAsync(id);

            //Assert
            users.Should().BeNull();            
        }        
    }
}
