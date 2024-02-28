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
            var item = productCartRepository.GetAll().AsQueryable().FirstOrDefault(x => x.CartId == cartItem.CartId && x.ProductId == cartItem.ProductId);
            if (item == null) return false;

            productCartRepository.Update(_mapper.Map<ProductCart>(cartItem));
            await _unitOfWork.SaveAsync();

            return true;
        }

        public async Task<bool> DeleteCartAsync(int? userId, int? cartId)
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

            if (userId != null)
            {
                var cart = await cartRepository.GetAll().AsQueryable().FirstOrDefaultAsync(x => x.UserId == userId && x.Status == Enums.CartStatus.Created);
                if(cart == null) return false;

                cart.Status = Enums.CartStatus.Deleted;
                cartRepository.Update(cart);
                await _unitOfWork.SaveAsync();

                return true;
            }

            return false;
        }

    }
}
