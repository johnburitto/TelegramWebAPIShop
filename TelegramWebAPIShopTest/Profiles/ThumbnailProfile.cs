using AutoMapper;
using Core.Dtos.Create;
using Core.Dtos.Update;
using Core.Entities;

namespace TelegramWebAPIShopTest.Profiles
{
    public class ThumbnailProfile : Profile
    {
        public ThumbnailProfile() 
        {
            CreateMap<ThumbnailCreateDto, Thumbnail>();
            CreateMap<ThumbnailUpdateDto, Thumbnail>();
        }
    }
}
