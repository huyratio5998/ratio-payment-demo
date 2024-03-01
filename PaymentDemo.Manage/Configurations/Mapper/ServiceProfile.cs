using AutoMapper;
using PaymentDemo.Manage.Entities;
using PaymentDemo.Manage.Models;

namespace PaymentDemo.Manage.Configurations.Mapper
{
    public class ServiceProfile : Profile
    {
        public ServiceProfile()
        {
            CreateMap<Product, ProductViewModel>()
                .ForMember(x => x.ProductCategories, f => f.Ignore());
            CreateMap<ProductViewModel, Product>()
                .ForMember(x=>x.ProductCategories, f => f.Ignore());
            CreateMap<Category, CategoryViewModel>();
            CreateMap<Cart, CartViewModel>();
            CreateMap<CartViewModel, Cart>();
            CreateMap<CartItemViewModel, ProductCart>();
            CreateMap<AddToCartViewModel, ProductCart>();
            CreateMap<User, UserViewModel>();
            CreateMap<UserViewModel, User>();
            CreateMap<OrderViewModel, Order>()
                .ForMember(x=>x.CartId, f=>f.MapFrom(t=>t.Cart.Id));
            CreateMap<Order, OrderViewModel>();            
            CreateMap(typeof(PagedResponse<>), typeof(PagedResponse<>));
        }

    }
}
