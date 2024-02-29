using AutoMapper;
using FluentValidation;
using PaymentDemo.Manage.Models;
using PaymentDemo.Manage.Repositories.Abstracts;
using PaymentDemo.Manage.Services.Abstractions;

namespace PaymentDemo.Manage.Services.Implements
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<OrderViewModel> _validator;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<OrderViewModel> validator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validator = validator;
        }

        public Task<OrderViewModel> GetAsync(int orderId)
        {
            throw new NotImplementedException();
        }
    }
}
