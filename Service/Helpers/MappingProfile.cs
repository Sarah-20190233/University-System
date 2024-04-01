using AutoMapper;
using Core.Models;
using Core.Models.DTOs;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //var fullIdentifiation = StudentDto.
        CreateMap<Student, StudentDto>()
        //.ForMember(src => src.StudentId, opt => opt.MapFrom(dest => dest.StudentId));

        .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => $"{src.StudentId} - {src.Name}"));

        CreateMap<StudentDto, Student>()
        .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.StudentId));

        CreateMap<StudentUpdatedDto, Student>();


    }
}
