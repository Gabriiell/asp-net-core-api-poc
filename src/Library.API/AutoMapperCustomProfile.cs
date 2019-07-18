using AutoMapper;
using Library.API.Helpers;

namespace Library.API
{
    public class AutoMapperCustomProfile : Profile
    {
        public AutoMapperCustomProfile()
        {
            CreateMap<Entities.Author, Models.AuthorDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.GetCurrentAge()));

            CreateMap<Models.AuthorForCreationDto, Entities.Author>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Books, opt => opt.Ignore());

            CreateMap<Entities.Book, Models.BookDto>();
        }
    }
}
