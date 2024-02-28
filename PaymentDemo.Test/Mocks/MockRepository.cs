using Moq;
using PaymentDemo.Manage;
using PaymentDemo.Manage.Entities;

namespace PaymentDemo.Test.Mocks
{
    public class MockRepository
    {
        public static Mock<IBaseRepository<Category>> GetMockCategory()
        {
            var mock = new Mock<IBaseRepository<Category>>();
            var categoriesDemo = new List<Category>()
            {
                new Category()
                {
                    Name = "abc",
                    Id = 1,
                },
                new Category()
                {
                    Name = "def",
                    Id = 2,
                },
            };

            var newCategory = new Category() { Id = 66, Name = "ratio" };


            mock.Setup(x => x.GetAll(false)).Returns(() => categoriesDemo);
            mock.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>())).ReturnsAsync((int id, bool isTracking) => categoriesDemo.FirstOrDefault(x => x.Id == id));
            mock.Setup(x => x.CreateAsync(It.IsAny<Category>())).ReturnsAsync(newCategory);
            mock.Setup(x => x.Update(It.IsAny<Category>())).Callback(() => { return; });
            mock.Setup(x => x.DeleteAsync(It.IsAny<int>())).Callback(() => { return; });

            return mock;
        }

        public static Mock<IBaseRepository<ProductCategory>> GetMockProductCategory()
        {
            var mock = new Mock<IBaseRepository<ProductCategory>>();
            var categoriesDemo = new List<ProductCategory>()
            {
                new ProductCategory()
                {
                    ProductId = 1,
                    CategoryId = 1,
                }
            };

            var newProductCategory = new ProductCategory() { ProductId = 2, CategoryId = 1 };


            mock.Setup(x => x.GetAll(false)).Returns(() => categoriesDemo);
            mock.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>())).ReturnsAsync((int id, bool isTracking) => categoriesDemo.FirstOrDefault(x => x.Id == id));
            mock.Setup(x => x.CreateAsync(It.IsAny<ProductCategory>())).ReturnsAsync(newProductCategory);
            mock.Setup(x => x.Update(It.IsAny<ProductCategory>())).Callback(() => { return; });
            mock.Setup(x => x.DeleteAsync(It.IsAny<int>())).Callback(() => { return; });

            return mock;
        }

        public static Mock<IBaseRepository<Cart>> GetMockCart()
        {
            var mock = new Mock<IBaseRepository<Cart>>();
            var carts = new List<Cart>()
            {
                new Cart()
                {
                    Id = 1,
                    UserId = 1,
                    ProductCarts = new List<ProductCart>()
                    {
                        new ProductCart(){ ProductId = 1, Number = 1,CartId = 1, Price = 50 },
                        new ProductCart(){ ProductId = 2, Number = 2,CartId = 1, Price = 60 },
                    }
                },
                new Cart()
                {
                    Id = 2,
                    UserId = 2,
                    ProductCarts = new List<ProductCart>()
                    {
                        new ProductCart(){ ProductId = 1, Number = 1,CartId = 1, Price = 50 },
                        new ProductCart(){ ProductId = 2, Number = 2,CartId = 1, Price = 60 },
                    }
                }
            };

            var newCart = new Cart()
            {
                Id = 99,
                UserId = 1,
                ProductCarts = new List<ProductCart>()
                    {
                        new ProductCart(){ ProductId = 1, Number = 1,CartId = 99, Price = 50 },
                        new ProductCart(){ ProductId = 2, Number = 2,CartId = 99, Price = 60 },
                    }
            };

            mock.Setup(x => x.GetAll(false)).Returns(() => carts);
            mock.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>())).ReturnsAsync((int id, bool isTracking) => carts.FirstOrDefault(x => x.Id == id));
            mock.Setup(x => x.CreateAsync(It.IsAny<Cart>())).Callback(() => { return; });
            mock.Setup(x => x.Update(It.IsAny<Cart>())).Callback(() => { return; });
            mock.Setup(x => x.DeleteAsync(It.IsAny<int>())).Callback(() => { return; });

            return mock;
        }

        public static Mock<IBaseRepository<ProductCart>> GetMockProductCart()
        {
            var mock = new Mock<IBaseRepository<ProductCart>>();
            var carts = new List<ProductCart>()
            {
               new ProductCart(){ ProductId = 1, Number = 1,CartId = 1, Price = 50 },
               new ProductCart(){ ProductId = 2, Number = 2,CartId = 1, Price = 60 },
            };

            mock.Setup(x => x.GetAll(false)).Returns(() => carts);
            mock.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>())).ReturnsAsync((int id, bool isTracking) => carts.FirstOrDefault(x => x.Id == id));
            mock.Setup(x => x.CreateAsync(It.IsAny<ProductCart>())).Callback(() => { return; });
            mock.Setup(x => x.Update(It.IsAny<ProductCart>())).Callback(() => { return; });
            mock.Setup(x => x.DeleteAsync(It.IsAny<int>())).Callback(() => { return; });

            return mock;
        }
    }
}
