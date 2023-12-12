using AutoMapper;
using eShop.Database.Entities;
using eShop.ViewModels.Catalogs.Sales.Carts;

namespace eShop.BackendAPI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Mapping Domain Models -> DTOs
            CreateMap<Cart, CartItemViewModel>().ReverseMap();
        }
    }
}