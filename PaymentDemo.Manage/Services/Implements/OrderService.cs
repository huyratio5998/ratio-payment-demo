using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PaymentDemo.Manage.Entities;
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
        private readonly IBaseRepository<Order> _orderRepository;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<OrderViewModel> validator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validator = validator;
            _orderRepository = _unitOfWork.GetRepository<Order>();
        }

        public async Task<PagedResponse<OrderViewModel>> GetOrdersAsync(OrderQueryParams queryParams)
        {
            var items = _orderRepository.GetAll().AsQueryable();

            // filter
            if (!string.IsNullOrWhiteSpace(queryParams.SearchText)) items = items.Where(x => x.OrderNumber.Contains(queryParams.SearchText));

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

            var songs = await PagedResponse<Order>.CreateAsync(items, queryParams.PageNumber, queryParams.PageSize);

            var result = _mapper.Map<PagedResponse<OrderViewModel>>(songs);
            return result;
        }

        public async Task<OrderViewModel?> GetOrderAsync(int orderId)
        {
            if (orderId <= 0) return null;

            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return null;

            return _mapper.Map<OrderViewModel>(order);
        }

        public async Task<OrderViewModel?> GetOrderAsync(string orderNumber)
        {
            var newOrderNumber = orderNumber.Trim();
            if (string.IsNullOrWhiteSpace(newOrderNumber)) return null;

            var order = await _orderRepository.GetAll().AsQueryable().FirstOrDefaultAsync(x => x.OrderNumber.Equals(orderNumber));
            if (order == null) return null;

            return _mapper.Map<OrderViewModel>(order);
        }

        public async Task<OrderViewModel> CreateOrderAsync(OrderViewModel newOrder)
        {
            try
            {
                var orderValidate = await _validator.ValidateAsync(newOrder);
                if (!orderValidate.IsValid) return new OrderViewModel();

                var order = _mapper.Map<Order>(newOrder);
                await _orderRepository.CreateAsync(order);
                await _unitOfWork.SaveAsync();

                newOrder.Id = order.Id;
                return newOrder;

            }
            catch (Exception e)
            {
                return new OrderViewModel();
            }
        }

    }
}
