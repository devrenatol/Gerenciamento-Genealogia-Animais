using AutoMapper;
using ProjetoLO.Application.DTO;
using ProjetoLO.Domain.Entities;

namespace ProjetoLO.Application.Mapping;

public class AnimalDTOMappingProfile : Profile
{
    public AnimalDTOMappingProfile()
    {
        CreateMap<Animal, AnimalDTO>().ReverseMap();
    }
}
