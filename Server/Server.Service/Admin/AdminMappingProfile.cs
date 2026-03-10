using AutoMapper;
using Common.Repository;
using Server.Domain.Admin;

namespace Server.Service.Learner
{
    public class AdminMappingProfile : Profile
    {
        public AdminMappingProfile()
        {
            CreateMap<CourseTaskEntity, CourseTaskDto>().ReverseMap();
            CreateMap<QuestionEntity, QuestionDto>().ReverseMap();
            CreateMap<AnswerProperty, AnswerPropertyDto>().ReverseMap();
        }
    }
}
