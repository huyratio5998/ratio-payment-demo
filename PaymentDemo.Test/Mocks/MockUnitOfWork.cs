using Moq;
using PaymentDemo.Manage.Entities;
using PaymentDemo.Manage.Repositories.Abstracts;

namespace PaymentDemo.Test.Mocks
{
    public class MockUnitOfWork
    {
        public static Mock<IUnitOfWork> GetMock()
        {
            var mock = new Mock<IUnitOfWork>();            

            //setup mock            
            mock.Setup(x => x.ProductRepository).Returns(() => MockIProductRepository.GetMock().Object);
            mock.Setup(x => x.GetRepository<Category>()).Returns(() => MockRepository.GetMockCategory().Object);
            mock.Setup(x => x.GetRepository<ProductCategory>()).Returns(() => MockRepository.GetMockProductCategory().Object);
            mock.Setup(x => x.SaveAsync()).Callback(() => { return; });

            return mock;
        }
    }
}
