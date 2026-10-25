using AutoMapper;
using MiA_Lr_3.Dtos.Clients;
using MiA_Lr_3.DTOs.Cars;
using MiA_Lr_3.DTOs.Clients;
using MiA_Lr_3.DTOs.Contracts;
using MiA_Lr_3.Models;
namespace MiA_Lr_3.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Мапінг для Клієнтів
            CreateMap<CreateClientDto, Client>();
            CreateMap<UpdateClientDto, Client>();
            CreateMap<Client, ClientResponseDto>();
            // Мапінг для Автомобілів
            CreateMap<CreateCarDto, Car>();
            CreateMap<UpdateCarDto, Car>();
            CreateMap<Car, CarResponseDto>();
            // Мапінг для Договорів
            CreateMap<CarContract, ContractResponseDto>();
        }
    }
}
