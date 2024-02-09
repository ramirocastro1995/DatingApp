using API.Entities;
using AutoMapper;

namespace API;

//Profile is from automapper nuget
public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        //.ForMember Modifies the pointed value to map from appUser to MemberDto
        CreateMap<AppUser, MemberDto>()
            .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x=> x.IsMain).Url))
            .ForMember(dest => dest.Age,opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
        //Use the same mapping because it has the same names
        CreateMap<Photo, PhotoDto>();
        //Use the same mapping because it has the same names
        CreateMap<MemberUpdateDto,AppUser>();
    }

}
