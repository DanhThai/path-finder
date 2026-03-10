using Common.Domain;
using Common.Repository;
using Microsoft.EntityFrameworkCore;
using Server.Domain.Learner;
using System.Linq.Expressions;

namespace Server.Service.Learner
{
    public class FeedbackService(
        IDBRepository _repository
        ) : IFeedbackService
    {

        public async Task<TableInfo<FeedbackDto>> GetPaging(CTableParameter parameter)
        {
            var query = new TableQueryParameter<FeedBackEntity>
            {
                Pager = new Pager { PageIndex = parameter.PageIndex, PageSize = parameter.PageSize },
                Sorter = GenerateSorter(parameter),
                Filter = GenerateFilter(parameter)
            };

            var result = await _repository.GetWithPagingAsync(query, entity => new FeedbackDto()
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Content = entity.Content,
                CreatedAt = entity.CreatedAt,
                ModifiedAt = entity.ModifiedAt
            });

            return result;
        }

        private Expression<Func<FeedBackEntity, bool>> GenerateFilter(CTableParameter param)
        {
            var userId = RuntimeContext.Current.UserId;
            var filter = PredicateBuilder.True<FeedBackEntity>();
            filter = filter.And(p => !p.IsDeleted && p.UserId == userId);
            return filter;
        }

        private Sorter<FeedBackEntity, object> GenerateSorter(CTableParameter param)
        {
            var result = new Sorter<FeedBackEntity, object> { IsAscending = param.IsAscending };

            switch (param.SortKey ?? "")
            {
                default:
                    result.SortBy = s => s.CreatedAt;
                    break;
            }

            return result;
        }
    }
}
