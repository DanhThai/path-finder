using AutoMapper;
using Common.Repository;
using Server.Domain.Admin;
using Server.Domain.Learner;

namespace Server.Service.Admin
{
    public class LearnerMappingProfile : Profile
    {
        public LearnerMappingProfile()
        {
            CreateMap<AnswerEntity, AnswerDto>().ReverseMap();
            CreateMap<CourseEntity, CourseDetailDto>().ReverseMap();
            CreateMap<CourseTaskEntity, CourseTaskViewDetailDto>().ReverseMap();
            CreateMap<FeedBackEntity, LearnerFeedbackDto>().ReverseMap();
            CreateMap<UserQuizAttempDto, UserQuizAttempEntity>().ReverseMap();
            CreateMap<QuestionProperty, QuestionEntity>().ReverseMap();
            CreateMap<QuestionProperty, QuestionDto>().ReverseMap();
        }
    }
}
