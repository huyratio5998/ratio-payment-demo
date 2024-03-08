using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PaymentDemo.Manage.Entities;
using PaymentDemo.Manage.Models;
using PaymentDemo.Manage.Repositories.Abstracts;
using PaymentDemo.Manage.Services.Abstractions;

namespace PaymentDemo.Manage.Services.Implements
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CartViewModel> _validator;

        public CartService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CartViewModel> validator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<bool> AddToCartAsync(CartViewModel cart)
        {
            try
            {
                var validateResult = await _validator.ValidateAsync(cart);
                if (!validateResult.IsValid) return false;

                cart.CartItems?.RemoveAll(x => x.Number <= 0 || x.ProductId <= 0);
                if (cart.CartItems == null || !cart.CartItems.Any()) return false;

                await _unitOfWork.CreateTransactionAsync();
                var cartRepository = _unitOfWork.GetRepository<Cart>();
                var productCartRepository = _unitOfWork.GetRepository<ProductCart>();

                //check cart exist
                var cartId = cart.Id;
                if (cartId == 0)
                {
                    var newCart = _mapper.Map<Cart>(cart);
                    cartId = (await cartRepository.CreateAsync(newCart)).Id;
                }

                foreach (var item in cart.CartItems)
                {
                    var existProduct = productCartRepository
                        .GetAll().AsQueryable()
                        .FirstOrDefault(x => x.ProductId == item.ProductId && x.CartId == cartId);

                    // check item already in cart
                    if (existProduct != null)
                    {
                        existProduct.Number += item.Number;
                        var updateResult = productCartRepository.Update(_mapper.Map<ProductCart>(item));
                        if (!updateResult)
                        {
                            await _unitOfWork.RollbackAsync();
                            return false;
                        }
                    }
                    else
                    {
                        await productCartRepository.CreateAsync(_mapper.Map<ProductCart>(item));
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

        public async Task<bool> ChangeCartItemAsync(AddToCartViewModel cartItem)
        {
            var productCartRepository = _unitOfWork.GetRepository<ProductCart>();
            var productRepository = _unitOfWork.ProductRepository;
            var item = productCartRepository.GetAll().AsQueryable().FirstOrDefault(x => x.CartId == cartItem.CartId && x.ProductId == cartItem.ProductId);
            if (item == null) return false;

            var addedItem = _mapper.Map<ProductCart>(cartItem);
            addedItem.Price = (await productRepository.GetByIdAsync(cartItem.ProductId))?.Price ?? 0;

            productCartRepository.Update(addedItem);
            await _unitOfWork.SaveAsync();

            return true;
        }

        public async Task<bool> DeleteCartAsync(int? userId, int? cartId)
        {
            try
            {
                var cartRepository = _unitOfWork.GetRepository<Cart>();
                if (cartId != null)
                {
                    var cart = await cartRepository.GetByIdAsync((int)cartId);
                    if (cart == null) return false;

                    cart.Status = Enums.CartStatus.Deleted;
                    cartRepository.Update(cart);
                    await _unitOfWork.SaveAsync();

                    return true;
                }
                else  if (userId != null)
                {
                    var cart = cartRepository.GetAll().AsQueryable().FirstOrDefault(x => x.UserId == userId && x.Status == Enums.CartStatus.Created);
                    if (cart == null) return false;

                    cart.Status = Enums.CartStatus.Deleted;
                    cartRepository.Update(cart);
                    await _unitOfWork.SaveAsync();

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }        

        public async Task<CartViewModel?> GetCartAsync(int cartId, bool isTracking = true)
        {
            if (cartId <= 0) return null;

            var cartDetail = await _unitOfWork.GetRepository<Cart>().GetByIdAsync(cartId, isTracking);
            if (cartDetail == null) return null;

            return _mapper.Map<CartViewModel>(cartDetail);
        }
    }
}
