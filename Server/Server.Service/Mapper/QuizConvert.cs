using Common.Repository;
using Server.Domain.Admin;

namespace Common.Service
{
    public class QuizConvert
    {

        public static QuestionDto QuestionConvertToDto(QuestionEntity entity)
        {
            return new QuestionDto()
            {
                Id = entity.Id,
                QuestionOrder = entity.QuestionOrder,
                Name = entity.Name,
                IsDeleted = entity.IsDeleted,
                Answers = entity.Answers.Select(y => AnswerConvertToDto(y)).ToList() ?? new List<AnswerPropertyDto>(),
            };
        }

        public static AnswerPropertyDto AnswerConvertToDto(AnswerProperty entity)
        {
            return new AnswerPropertyDto()
            {
                Id = entity.Id,
                Order = entity.Order,
                IsCorrect = entity.IsCorrect,
            };
        }
    }
}
