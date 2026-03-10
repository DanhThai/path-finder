using AutoMapper;
using Common.Repository;
using Server.Domain.Admin;

namespace Server.Service.Learner
{
    public class QuestionService(
        IDBRepository _repository,
        IMapper _mapper
        ) : IQuestionService
    {

        public async Task<List<QuestionDto>> GetQuestionByCourseId(Guid courseId)
        {
            var result = new List<QuestionDto>();
            var questions = await _repository.GetAsync<QuestionEntity>(x => x.CourseId == courseId);

            foreach (var question in questions)
            {
                var questiondto = _mapper.Map<QuestionDto>(question);
                questiondto.Answers = _mapper.Map<List<AnswerPropertyDto>>(question.Answers);
                result.Add(questiondto);
            }

            return result;
        }
    }
}
