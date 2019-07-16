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

            CreateMap<Entities.Book, Models.BookDto>();
        }
    }
}
