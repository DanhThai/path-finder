using AutoMapper;
using Common.Repository;
using Server.Domain.Admin;

namespace Server.Service.Admin
{
    public class AdminMappingProfile : Profile
    {
        public AdminMappingProfile()
        {
            CreateMap<AnswerEntity, AnswerDto>().ReverseMap();
        }
    }
}
