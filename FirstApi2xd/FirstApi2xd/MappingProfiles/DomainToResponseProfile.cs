using System.Linq;
using AutoMapper;
using FirstApi2xd.Contracts.v1.Responses;
using FirstApi2xd.Domain;

namespace FirstApi2xd.MappingProfiles
{
    public class DomainToResponseProfile : Profile
    {
        public DomainToResponseProfile()
        {
            CreateMap<Post, PostResponse>()
                .ForMember(dest => dest.Tags, 
                    opt =>
                    opt.MapFrom(src => 
                        src.Tags.Select(x => new TagResponse{Name = x.TagName})));
            CreateMap<Tags, TagResponse>();

        }
    }
}