using AutoMapper;
using Lessons.ClientCardApi.Abstraction.Data.Models;
using Lessons.ClientCardApi.Abstraction.Requests;

namespace Lessons.ClientCardApi.NuGet.AutoMapper.MapperProfile
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CreateRequest, CreditCardInfoModel>();
            CreateMap<CreditCardInfoModel, CreateRequest>();
        }
    }
}