using AutoMapper;
using Core.Dtos.Create;
using Core.Dtos.Update;
using Core.Entities;

namespace WebAPI.Profiles
{
    public class ThumbnailProfile : Profile
    {
        public ThumbnailProfile() 
        {
            CreateMap<ThumbnailCreateDto, Thumbnail>()
                .ForMember(dest => dest.ChangedBy, options => options.MapFrom(src => Environment.UserName))
                .ForMember(dest => dest.CreatedAt, options => options.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, options => options.MapFrom(src => DateTime.UtcNow));
            CreateMap<ThumbnailUpdateDto, Thumbnail>()
                .ForMember(dest => dest.ChangedBy, options => options.MapFrom(src => Environment.UserName))
                .ForMember(dest => dest.UpdatedAt, options => options.MapFrom(src => DateTime.UtcNow));
        }
    }
}
