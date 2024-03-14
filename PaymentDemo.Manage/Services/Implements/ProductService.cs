using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PaymentDemo.Manage.Constants;
using PaymentDemo.Manage.Entities;
using PaymentDemo.Manage.Helpers;
using PaymentDemo.Manage.Models;
using PaymentDemo.Manage.Repositories.Abstracts;
using PaymentDemo.Manage.Services.Abstractions;

namespace PaymentDemo.Manage.Services.Implements
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;
        private readonly IValidator<ProductViewModel> _validator;
        private const string wwwRootAddress = "wwwroot";

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<ProductViewModel> validator, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validator = validator;
            _webHostEnvironment = webHostEnvironment;
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
                Task uploadImage = null;
                if (newProduct.UploadedImage != null)
                {
                    var webRootAddress = string.IsNullOrWhiteSpace(_webHostEnvironment.WebRootPath) ? wwwRootAddress : _webHostEnvironment.WebRootPath;
                    uploadImage = FileHelpers.UploadFile(newProduct.UploadedImage, webRootAddress, FileConstants.ImageFolder, FileConstants.ProductFolder);
                    product.Image = newProduct.UploadedImage.FileName;
                }
                var createdProduct = await _unitOfWork.ProductRepository.CreateAsync(product);
                await _unitOfWork.SaveAsync();
                if (createdProduct == null || createdProduct.Id == 0) return 0;

                // create productCategory if any
                await AddProductCategoryInfo(createdProduct.Id, newProduct.ProductCategories);
                
                if (uploadImage != null) await uploadImage;
                await _unitOfWork.CommitAsync();

                return createdProduct.Id;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return 0;
            }
        }

        private async Task AddProductCategoryInfo(int productId, List<CategoryViewModel> categories)
        {
            if (categories == null || !categories.Any()) return;

            var categoryRepo = _unitOfWork.GetRepository<Category>();
            var productCategoryRepo = _unitOfWork.GetRepository<ProductCategory>();
            foreach (CategoryViewModel category in categories)
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
                            ProductId = productId
                        });
            }
            await _unitOfWork.SaveAsync();
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {           
            var result = await _unitOfWork.ProductRepository.DeleteAsync(productId);
            if(!result) return false;

            await _unitOfWork.SaveAsync();

            return true;
        }

        public async Task<ProductViewModel> GetProductAsync(int productId, bool isTracking = true)
        {
            if (productId <= 0) return new ProductViewModel();

            var product = await _unitOfWork.ProductRepository.GetByIdIncludeAsync(productId);
            if (product == null || product.Id == 0) return new ProductViewModel();

            return _mapper.Map<ProductViewModel>(product);
        }

        public async Task<PagedResponse<ProductViewModel>> GetProductsAsync(ProductQueryParams queryParams)
        {
            IQueryable<Product> items = _unitOfWork.ProductRepository
                            .GetAll().AsQueryable()
                            .Include(x => x.ProductCategories)
                            .ThenInclude(x => x.Category);

            // filter
            if (!string.IsNullOrWhiteSpace(queryParams.SearchText)) items =  items.Where(x => x.Name.Contains(queryParams.SearchText) || x.DisplayName.Contains(queryParams.SearchText));

            // order
            var orderCondition = queryParams.OrderBy;
            if (orderCondition != null)
            {
                if (orderCondition == OrderType.Asc) items = items.OrderBy(x => x.Id);
                else if (orderCondition == OrderType.Desc) items = items.OrderByDescending(y => y.Id);
            }

            // paging
            queryParams.PageNumber = queryParams.PageNumber <= 0 ? Models.CommonConstant.PageIndexDefault : queryParams.PageNumber;
            queryParams.PageSize = queryParams.PageSize <= 0 ? Models.CommonConstant.PageSizeDefault : queryParams.PageSize;

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

                Task uploadImage = null;
                if (newProduct.UploadedImage != null)
                {
                    var webRootAddress = string.IsNullOrWhiteSpace(_webHostEnvironment.WebRootPath) ? wwwRootAddress : _webHostEnvironment.WebRootPath;
                    uploadImage = FileHelpers.UploadFile(newProduct.UploadedImage, webRootAddress, FileConstants.ImageFolder, FileConstants.ProductFolder);                    
                    newProduct.Image = newProduct.UploadedImage.FileName;
                }
                var targetProduct = await _unitOfWork.ProductRepository.GetByIdIncludeAsync(newProduct.Id ?? 0, false);
                if (targetProduct == null || targetProduct.Id == 0) return false;

                var res = _unitOfWork.ProductRepository.Update(_mapper.Map<Product>(newProduct));
                if (!res) return false;

                // add product category if any
                var productCategoryRepository = _unitOfWork.GetRepository<ProductCategory>();
                var currentProductCategories = productCategoryRepository
                    .GetAll().AsQueryable()
                    .Where(x => x.ProductId == newProduct.Id)
                    .ToList();

                // remove => then => create new product category record.
                if (currentProductCategories != null && currentProductCategories.Any())
                    await productCategoryRepository.DeleteRangeAsync(currentProductCategories);

                // create productCategory if any
                await AddProductCategoryInfo((int)newProduct.Id, newProduct.ProductCategories);
                
                if (uploadImage != null) await uploadImage;
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
