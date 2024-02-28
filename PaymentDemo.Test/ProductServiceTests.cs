using AutoMapper;
using FluentAssertions;
using FluentValidation;
using PaymentDemo.Manage;
using PaymentDemo.Manage.Configurations.Mapper;
using PaymentDemo.Manage.Entities;
using PaymentDemo.Manage.Models;
using PaymentDemo.Manage.Services.Implements;
using PaymentDemo.Test.Mocks;

namespace PaymentDemo.Test
{
    public class ProductServiceTests
    {
        private IMapper _mapper;
        private IValidator<ProductViewModel> _productValidator;

        public ProductServiceTests()
        {
            if (_mapper == null)
            {
                var mappingConfig = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new ServiceProfile());
                });
                _mapper = mappingConfig.CreateMapper();
            }

            if (_productValidator == null)
            {
                _productValidator = new ProductViewModelValidator();
            }
        }

        [Theory]
        [InlineData("test", "test dname", "test description", 123, 20, "abc.jpg", 1, "hhh")]
        [InlineData("ff", "", "", 123, 30, "", 1, "hhh")]
        [InlineData("abc", null, null, 100, 33, null, 1, "hhh")]
        [InlineData("tt", " ", " ", 123, 30, " ", 1, "hhh")]
        [InlineData("af ", "d ", "f ", 123, 30, "e ", 1, "hhh")]
        public async Task Add_Product_SuccessAdded(string name, string? displayName, string? description, decimal price, int number, string? image, int categoryId, string categoryName)
        {
            //Arrange
            var newProduct = new ProductViewModel()
            {
                Name = name,
                DisplayName = displayName,
                Description = description,
                Price = price,
                Number = number,
                Image = image,
                ProductCategories = new List<CategoryViewModel>
                {
                    new CategoryViewModel(){ Id = categoryId, Name = categoryName },
                }
            };

            var unitOfWork = MockUnitOfWork.GetMock();
            var productService = new ProductService(unitOfWork.Object, _mapper, _productValidator);

            //Act
            int productId = await productService.CreateProductAsync(newProduct);

            //Assert
            productId.Should().BeGreaterThan(0);
        }

        [Theory]
        [InlineData("test", "test", "test", -123, 30, "ddd.jpg", 1, "abc")]
        [InlineData("test", "test", "test", -123, -30, "ddd.jpg", 1, "abc")]
        [InlineData("test", "f", "d", 123, 0, "", -1, "abc")]
        [InlineData("", "f", "d", 123, 0, "", 1, "abc")]
        public async Task Add_ProductEmpty_Fail(string name, string? displayName, string? description, decimal price, int number, string? image, int categoryId, string categoryName)
        {
            //Arrange
            var newProduct = new ProductViewModel()
            {
                Name = name,
                DisplayName = displayName,
                Description = description,
                Price = price,
                Number = number,
                Image = image,
                ProductCategories = new List<CategoryViewModel>
                {
                    new CategoryViewModel(){ Id = categoryId, Name = categoryName },
                }
            };
            var unitOfWork = MockUnitOfWork.GetMock();
            var productService = new ProductService(unitOfWork.Object, _mapper, _productValidator);

            //Act
            int productId = await productService.CreateProductAsync(newProduct);
            productId.Should().Be(0);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task GetByID_Product_Success(int productId)
        {
            //Arrange
            var unitOfWork = MockUnitOfWork.GetMock();
            var productService = new ProductService(unitOfWork.Object, _mapper, _productValidator);

            //Act
            ProductViewModel product = await productService.GetProductAsync(productId);

            //Assert
            product.Should().BeOfType<ProductViewModel>();
            product.Id.Should().Be(productId);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(9)]
        public async Task GetByID_Product_Fail(int productId)
        {
            //Arrange
            var unitOfWork = MockUnitOfWork.GetMock();
            var productService = new ProductService(unitOfWork.Object, _mapper, _productValidator);

            //Act
            ProductViewModel product = await productService.GetProductAsync(productId);

            //Assert
            product.Should().BeOfType<ProductViewModel>();
            product.Id.Should().BeNull();
        }

        [Theory]
        [InlineData(1, "test 1", "test 1", "test 1 description", 200, 50, "dfff.jpg", 1, "ratioCategory")]
        [InlineData(1, "test 2", "test 2", "test 2 description", 211, 23, "fff.jpg", 2, "ratioCategory2")]
        [InlineData(1, "test 3", "test 3", "test 3 description", 211, 23, "fff.jpg", null, null)]
        public async Task Update_Product_SuccessUpdated(int productId, string name, string? displayName, string? description, decimal price, int number, string? image, int? categoryId, string? categoryName)
        {
            //Arrange            
            var newProduct = new ProductViewModel()
            {
                Id = productId,
                Name = name,
                DisplayName = displayName,
                Description = description,
                Price = price,
                Number = number,
                Image = image,
            };            

            if(categoryId != null )
            {
                newProduct.ProductCategories = new List<CategoryViewModel>
                    {
                        new CategoryViewModel(){ Id = categoryId ?? 0, Name = categoryName },
                    };
            }

            var unitOfWork = MockUnitOfWork.GetMock();
            var productService = new ProductService(unitOfWork.Object, _mapper, _productValidator);

            //Act
            bool result = await productService.UpdateProductAsync(newProduct);

            //Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(89, "test 1", "test 1", "test 1 description", 200, 50, "dfff.jpg", 1, "ratioCategory")]
        [InlineData(1, "test 2", "test 2", "test 2 description", 211, 23, "fff.jpg", -2, "ratioCategory2")]
        public async Task Update_Product_FailUpdated(int productId, string name, string? displayName, string? description, decimal price, int number, string? image, int categoryId, string categoryName)
        {
            //Arrange            
            var newProduct = new ProductViewModel()
            {
                Id = productId,
                Name = name,
                DisplayName = displayName,
                Description = description,
                Price = price,
                Number = number,
                Image = image,
                ProductCategories = new List<CategoryViewModel>
                {
                    new CategoryViewModel(){ Id = categoryId, Name = categoryName },
                }
            };
            var unitOfWork = MockUnitOfWork.GetMock();
            var productService = new ProductService(unitOfWork.Object, _mapper, _productValidator);

            //Act
            bool result = await productService.UpdateProductAsync(newProduct);

            //Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task Delete_Product_Success(int productId)
        {
            //Arrange            
            var unitOfWork = MockUnitOfWork.GetMock();
            var productService = new ProductService(unitOfWork.Object, _mapper, _productValidator);

            //Act
            bool deletedResult = await productService.DeleteProductAsync(productId);

            //Assert            
            deletedResult.Should().BeTrue();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task Delete_Product_Fail(int productId)
        {
            //Arrange
            var unitOfWork = MockUnitOfWork.GetMock();
            var productService = new ProductService(unitOfWork.Object, _mapper, _productValidator);

            //Act
            ProductViewModel getProductBefore = await productService.GetProductAsync(productId);
            bool deletedResult = await productService.DeleteProductAsync(productId);
            var getProductAfterDeleted = await productService.GetProductAsync(productId);

            //Assert
            getProductBefore.Should().BeOfType<ProductViewModel>();
            getProductBefore.Id.Should().BeNull();
            deletedResult.Should().BeFalse();
            getProductAfterDeleted.Should().BeOfType<ProductViewModel>();
            getProductAfterDeleted.Id.Should().BeNull();
        }
    }
}