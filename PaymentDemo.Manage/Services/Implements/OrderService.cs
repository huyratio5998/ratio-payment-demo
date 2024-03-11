using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PaymentDemo.Manage.Entities;
using PaymentDemo.Manage.Enums;
using PaymentDemo.Manage.Models;
using PaymentDemo.Manage.Repositories.Abstracts;
using PaymentDemo.Manage.Services.Abstractions;
using System.Text.Json;

namespace PaymentDemo.Manage.Services.Implements
{
    public class OrderService : IOrderService
    {
        private readonly ICartService _cartService;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<OrderViewModel> _validator;
        private readonly IBaseRepository<Order> _orderRepository;
        private readonly ILogger _logger;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<OrderViewModel> validator, ICartService cartService, IUserService userService, ILogger<OrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validator = validator;
            _orderRepository = _unitOfWork.GetRepository<Order>();
            _cartService = cartService;
            _userService = userService;
            _logger = logger;
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
            await MapAdditionOrderViewModelInfo(result.Items);

            return result;
        }

        private async Task MapAdditionOrderViewModelInfo(List<OrderViewModel> listOrderViewModel)
        {
            if (listOrderViewModel == null || !listOrderViewModel.Any()) return;

            foreach (var item in listOrderViewModel)
            {
                var cartId = item.Cart.Id;
                if (cartId == 0) continue;

                var cart = await _cartService.GetCartAsync(cartId);
                if (cart == null) continue;

                var cartUser = await _userService.GetUserAsync(cart.UserId);
                if (cartUser == null) continue;

                item.Cart = cart;
                item.User = cartUser;
            }
        }

        public async Task<OrderViewModel?> GetOrderAsync(int orderId, bool isTracking = true)
        {
            if (orderId <= 0) return null;

            var order = await _orderRepository.GetByIdAsync(orderId, isTracking);
            if (order == null) return null;

            return _mapper.Map<OrderViewModel>(order);
        }

        public async Task<OrderViewModel?> GetOrderAsync(string orderNumber, bool isTracking = true)
        {
            if (string.IsNullOrWhiteSpace(orderNumber)) return null;

            var order = await _orderRepository
                .GetAll(isTracking).AsQueryable()
                .FirstOrDefaultAsync(x => x.OrderNumber.Equals(orderNumber));

            if (order == null) return null;

            return _mapper.Map<OrderViewModel>(order);
        }

        public async Task<bool> ShipmentTrack(string orderNumber, string orderStatus)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(orderNumber) || string.IsNullOrWhiteSpace(orderStatus)) return false;
                if (!Enum.TryParse(orderStatus, out OrderStatus orderStatusEnum)) return false;

                await _unitOfWork.CreateTransactionAsync();
                var order = await _orderRepository
                    .GetAll(true).AsQueryable()
                    .FirstOrDefaultAsync(x => x.OrderNumber.Equals(orderNumber));

                if (order == null) return false;
                if(!CanUpdateOrderStatus(order.OrderStatus)) return false;

                order.OrderStatus = orderStatusEnum;
                order.OrderHistory = JsonSerializer.Serialize(AddOrderHistory(order.OrderHistory, order, orderStatusEnum));

                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Fail to track shipment. Exception: " + ex.ToString());
                await _unitOfWork.RollbackAsync();
                return false;
            }
        }

        private bool CanUpdateOrderStatus(OrderStatus previousStatus)
        {
            switch (previousStatus)
            {
                case OrderStatus.Canceled:
                case OrderStatus.Refunded:
                case OrderStatus.Successfully:
                    {
                        return false;
                    }
                default: return true;
            }
        }

        private List<OrderHistoryViewModel> AddOrderHistory(string? orderHistory, Order order, OrderStatus newOrderStatus)
        {
            var result = new List<OrderHistoryViewModel>();
            if (!string.IsNullOrEmpty(orderHistory)) 
                result = JsonSerializer.Deserialize<List<OrderHistoryViewModel>>(orderHistory) ?? new List<OrderHistoryViewModel>();

            result.Add(new OrderHistoryViewModel()
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                OrderStatus = newOrderStatus,
                Description = $"Shipment tracking change order status to: {newOrderStatus.ToString()}",
                CreatedDate = DateTime.UtcNow,
            });
            return result;
        }

        public async Task<OrderViewModel?> GetOrderAsync(string orderNumber)
        {
            var newOrderNumber = orderNumber.Trim();
            if (string.IsNullOrWhiteSpace(newOrderNumber)) return null;

            var order = await _orderRepository.GetAll().AsQueryable().FirstOrDefaultAsync(x => x.OrderNumber.Equals(orderNumber));
            if (order == null) return null;

            return _mapper.Map<OrderViewModel>(order);
        }

        public async Task<OrderViewModel?> CreateOrderAsync(OrderViewModel newOrder)
        {
            try
            {
                var orderValidate = await _validator.ValidateAsync(newOrder);
                if (!orderValidate.IsValid) return null;
                if (newOrder.Cart.Status == Enums.CartStatus.Deleted) return null;

                newOrder.PaymentStatus = PaymentStatus.Pending;
                newOrder.ShipmentStatus = ShipmentStatus.Inprogress;
                newOrder.OrderStatus = OrderStatus.Created;                

                var order = _mapper.Map<Order>(newOrder);
                var orderHistory = new List<OrderHistoryViewModel>() { new OrderHistoryViewModel()
                {
                    OrderId = order.Id,
                    OrderNumber = order.OrderNumber,
                    OrderStatus = OrderStatus.Created,
                    Description = $"Initial order: {order.OrderNumber}",
                    CreatedDate = DateTime.UtcNow,
                }};
                order.OrderHistory = JsonSerializer.Serialize(orderHistory);

                await _orderRepository.CreateAsync(order);
                await _unitOfWork.SaveAsync();

                newOrder.Id = order.Id;
                return newOrder;

            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<bool> UpdateOrderAsync(OrderViewModel newOrder)
        {
            try
            {
                var orderValidate = await _validator.ValidateAsync(newOrder);
                if (!orderValidate.IsValid) return false;

                var order = _mapper.Map<Order>(newOrder);
                _orderRepository.Update(order);

                await _unitOfWork.SaveAsync();
                return true;

            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            try
            {
                if (orderId <= 0) return false;

                var result = await _orderRepository.DeleteAsync(orderId);
                if (!result) return false;

                await _unitOfWork.SaveAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> DeleteOrderAsync(string orderNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(orderNumber)) return false;

                var order = await _orderRepository.GetAll().AsQueryable().FirstOrDefaultAsync(x => x.OrderNumber.Equals(orderNumber));
                if (order == null) return false;

                var result = await _orderRepository.DeleteAsync(order);
                if (!result) return false;

                await _unitOfWork.SaveAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
