using AutoMapper;
using FluentValidation;
using PaymentDemo.Manage.Entities;
using PaymentDemo.Manage.Models;
using PaymentDemo.Manage.Repositories.Abstracts;
using PaymentDemo.Manage.Services.Abstractions;

namespace PaymentDemo.Manage.Services.Implements
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;        
        private readonly IValidator<ProductViewModel> _validator;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<ProductViewModel> validator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<int> CreateProductAsync(ProductViewModel newProduct)
        {
            try
            {
                var validateResult = await _validator.ValidateAsync(newProduct);
                if (!validateResult.IsValid) return 0;

                await _unitOfWork.CreateTransactionAsync();
                var product = _mapper.Map<Product>(newProduct);
                product.Id = 0;
                var createdProduct = await _unitOfWork.ProductRepository.CreateAsync(product);
                await _unitOfWork.SaveAsync();
                if (createdProduct == null || createdProduct.Id == 0) return 0;
                
                // create productCategory if any
                if(newProduct.ProductCategories != null && newProduct.ProductCategories.Any())
                {
                    var categoryRepo = _unitOfWork.GetRepository<Category>();
                    var productCategoryRepo = _unitOfWork.GetRepository<ProductCategory>();
                    foreach (CategoryViewModel category in newProduct.ProductCategories)
                    {
                        if (category == null || category.Id <= 0) continue;
                        var createdCategoryId = 0;
                        if (await categoryRepo.GetByIdAsync(category.Id, false) == null)
                        {
                            var createdCategory = await categoryRepo.CreateAsync(new Category() { Name = category.Name });
                            await _unitOfWork.SaveAsync();
                            createdCategoryId = createdCategory.Id;
                        }
                        
                        await productCategoryRepo.CreateAsync(
                                new ProductCategory()
                                {
                                    CategoryId = createdCategoryId > 0 ? createdCategoryId : category.Id,
                                    ProductId = createdProduct.Id
                                });
                    }
                }

                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();

                return createdProduct.Id;

            }catch(Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return 0;
            }
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(productId);
            if (product == null || product.Id == 0) return false;

            await _unitOfWork.ProductRepository.DeleteAsync(productId);
            await _unitOfWork.SaveAsync();

            return true;
        }

        public async Task<ProductViewModel> GetProductAsync(int productId, bool isTracking = true)
        {
            if (productId == 0) return new ProductViewModel();
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(productId, isTracking);
            if (product == null || product.Id == 0) return new ProductViewModel();

            return _mapper.Map<ProductViewModel>(product);
        }

        public async Task<PagedResponse<ProductViewModel>> GetProductsAsync(ProductQueryParams queryParams)
        {
            var items = _unitOfWork.ProductRepository.GetAll()
                            .AsQueryable();

            // filter
            if (!string.IsNullOrWhiteSpace(queryParams.SearchText)) items = items.Where(x => x.Name.Contains(queryParams.SearchText) || x.DisplayName.Contains(queryParams.SearchText));

            // order
            var orderCondition = queryParams.OrderBy;
            if (orderCondition != null)
            {
                if (orderCondition == OrderType.Asc) items = items.OrderBy(x => x.Id);
                else if (orderCondition == OrderType.Desc) items = items.OrderByDescending(y => y.Id);
            }

            // paging
            queryParams.PageNumber = queryParams.PageNumber <= 0 ? CommonConstant.PageIndexDefault : queryParams.PageNumber;
            queryParams.PageSize = queryParams.PageSize <= 0 ? CommonConstant.PageSizeDefault : queryParams.PageSize;

            var songs = await PagedResponse<Product>.CreateAsync(items, queryParams.PageNumber, queryParams.PageSize);

            var result = _mapper.Map<PagedResponse<ProductViewModel>>(songs);
            return result;
        }

        public async Task<bool> UpdateProductAsync(ProductViewModel newProduct)
        {
            try
            {                
                var validateResult = await _validator.ValidateAsync(newProduct);
                if (!validateResult.IsValid || newProduct.Id == 0) return false;

                await _unitOfWork.CreateTransactionAsync();

                var targetProduct = await _unitOfWork.ProductRepository.GetByIdAsync(newProduct.Id ?? 0, false);
                if(targetProduct == null || targetProduct.Id == 0) return false;

                var res = _unitOfWork.ProductRepository.Update(_mapper.Map<Product>(newProduct));
                if (!res) return false;

                // add product category if any
                var productCategoryRepository = _unitOfWork.GetRepository<ProductCategory>();
                var currentProductCategories = productCategoryRepository
                    .GetAll().AsQueryable()
                    .Where(x => x.ProductId == newProduct.Id)
                    .ToList();

                // remove => then => create new product category record.
                if(currentProductCategories != null && currentProductCategories.Any())
                    await productCategoryRepository.DeleteRangeAsync(currentProductCategories);

                if (newProduct.ProductCategories != null && newProduct.ProductCategories.Any())
                {
                    var categoryRepo = _unitOfWork.GetRepository<Category>();                    
                    foreach (CategoryViewModel category in newProduct.ProductCategories)
                    {
                        if (category == null || category.Id <= 0) continue;
                        var createdCategoryId = 0;
                        if (await categoryRepo.GetByIdAsync(category.Id, false) == null)
                        {
                            var createdCategory = await categoryRepo.CreateAsync(new Category() { Name = category.Name });
                            await _unitOfWork.SaveAsync();
                            createdCategoryId = createdCategory.Id;
                        }

                        // create product category
                        await productCategoryRepository.CreateAsync(
                                new ProductCategory()
                                {
                                    CategoryId = createdCategoryId > 0 ? createdCategoryId : category.Id,
                                    ProductId = newProduct.Id ?? 0
                                });
                    }
                }
                
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return false;
            }
        }
    }
}
