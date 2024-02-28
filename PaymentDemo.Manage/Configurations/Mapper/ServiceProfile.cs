using AutoMapper;
using PaymentDemo.Manage.Entities;
using PaymentDemo.Manage.Models;

namespace PaymentDemo.Manage.Configurations.Mapper
{
    public class ServiceProfile : Profile
    {
        public ServiceProfile()
        {

            //CreateMap<SongApiRequest, Song>();
            //CreateMap<SongApiRequest, SongViewModel>()
            //    .ForMember(des => des.Song, des => des.MapFrom(s => s));
            //CreateMap<Song, SongViewModel>()
            //    .ForMember(des => des.Song, x => x.MapFrom(des => des));

            //CreateMap<ArtistApiRequest, ArtistViewModel>()
            //    .ForMember(des => des.Artist, des => des.MapFrom(s => s));

            CreateMap<Product, ProductViewModel>()
                .ForMember(x => x.ProductCategories, f => f.Ignore());
            CreateMap<ProductViewModel, Product>()
                .ForMember(x=>x.ProductCategories, f => f.Ignore());
            CreateMap<ProductCart, ProductCartViewModel>();                
            CreateMap<Category, CategoryViewModel>();
            CreateMap(typeof(PagedResponse<>), typeof(PagedResponse<>));
        }

    }
}
