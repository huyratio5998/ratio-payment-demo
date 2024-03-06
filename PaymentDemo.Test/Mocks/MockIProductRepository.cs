using Microsoft.EntityFrameworkCore;
using Moq;
using PaymentDemo.Manage.Entities;
using PaymentDemo.Manage.Repositories.Abstracts;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PaymentDemo.Test.Mocks
{
    public class MockIProductRepository
    {
        public static Mock<IProductRepository> GetMock()
        {
            var mock = new Mock<IProductRepository>();

            //setup mock
            var products = new List<Product>()
            {
                new Product()
                {
                    Id = 1,
                    Name = "Test",
                    DisplayName = "Test",
                    Description = "Test product",
                    Price = 125,
                    Number = 20,
                    Image = "abc.jpg",
                    ProductCategories = new List<ProductCategory>()
                    {
                        new ProductCategory(){ProductId=1,CategoryId =1}
                    }                    
                },
                new Product()
                {
                    Id = 2,
                    Name = "Test1",
                    DisplayName = "Test1",
                    Description = "Test1 product",
                    Price = 500,
                    Number = 30,
                    Image = "abc.jpg",
                    ProductCategories = new List<ProductCategory>()
                    {
                        new ProductCategory(){ProductId=2,CategoryId =1}
                    }
                },
                new Product()
                {
                    Id = 3,
                    Name = "Test3",
                    DisplayName = "Test3",
                    Description = "Test3 product",
                    Price = 133,
                    Number = 20,
                    Image = "abc.jpg",
                    ProductCategories = new List<ProductCategory>()
                    {
                        new ProductCategory(){ProductId=3,CategoryId =1}
                    }
                },
                new Product()
                {
                    Id = 4,
                    Name = "Test4",
                    DisplayName = "Test4",
                    Description = "Test4 product",
                    Price = 200,
                    Number = 20,
                    Image = "abc.jpg",
                    ProductCategories = new List<ProductCategory>()
                    {
                        new ProductCategory(){ProductId=4,CategoryId =1}
                    }
                }
            };
            var newProduct = new Product()
            {
                Id = 55,
                Name = "abc",
                DisplayName = "abc",
                Description = "abc",
                Price = 123,
                Number = 5,
                Image = "abc",
                ProductCategories = new List<ProductCategory>()
                {
                    new ProductCategory(){ProductId=55,CategoryId =1 }
                }
            };

            mock.Setup(x => x.GetAll(false)).Returns(() => products);            
            mock.Setup(x => x.GetByIdIncludeAsync(It.IsAny<int>(), It.IsAny<bool>())).ReturnsAsync((int id, bool isTracking) => products.FirstOrDefault(x => x.Id == id));
            mock.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>())).ReturnsAsync((int id, bool isTracking) => products.FirstOrDefault(x=>x.Id == id));
            mock.Setup(x => x.CreateAsync(It.IsAny<Product>())).ReturnsAsync(newProduct);
            mock.Setup(x => x.Update(It.IsAny<Product>())).Returns(true);
            mock.Setup(x => x.DeleteAsync(It.IsAny<int>())).ReturnsAsync((int id) => products.Remove(products.FirstOrDefault(x => x.Id == id)));

            return mock;
        }
    }
}
