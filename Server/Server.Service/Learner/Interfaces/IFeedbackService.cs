using Common.Domain;
using Common.Service;
using Server.Domain.Learner;

namespace Server.Service.Learner
{
    public interface IFeedbackService : IScopeDependency
    {
        Task<TableInfo<FeedbackDto>> GetPaging(CTableParameter parameter);
    }
}
