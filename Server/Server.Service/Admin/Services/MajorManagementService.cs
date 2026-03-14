using Common.Domain;
using Common.Repository;
using Microsoft.EntityFrameworkCore;
using Server.Domain.Admin;
using System.Linq.Expressions;

namespace Server.Service.Admin
{
    public class MajorManagementService(
        IDBRepository _repository
        ) : IMajorManagementService
    {
        public async Task<bool> Add(LearnerMajorDto dto)
        {
            var major = new LearnerMajorEntity
            {
                Name = dto.Name,
            };

            await _repository.AddAsync(major);

            return true;
        }

        public async Task<bool> Delete(Guid id)
        {
            var entity = await _repository.FindAsync<LearnerMajorEntity>(p => p.Id == id) ?? throw new NotExistException("Major");
            entity.IsDeleted = true;

            await _repository.UpdateAsync(entity);
            return true;
        }

        public async Task<LearnerMajorDto> GetDetail(Guid id)
        {
            var entity = await _repository.FindAsync<LearnerMajorEntity>(p => p.Id == id) ?? throw new NotExistException("Major");

            return new LearnerMajorDto
            {
                Id = entity.Id,
                Name = entity.Name,
            };
        }

        public async Task<TableInfo<LearnerMajorDto>> GetPaging(CTableParameter parameter)
        {
            var query = new TableQueryParameter<LearnerMajorEntity>
            {
                Pager = new Pager { PageIndex = parameter.PageIndex, PageSize = parameter.PageSize },
                Sorter = GenerateSorter(parameter),
                Filter = GenerateFilter(parameter)
            };

            return await _repository.GetWithPagingAsync(query, s => new LearnerMajorDto
            {
                Id = s.Id,
                Name = s.Name,
                IsDeleted = s.IsDeleted,
                ModifiedAt = s.ModifiedAt,
            });
        }

        public async Task<bool> Update(Guid id, LearnerMajorDto dto)
        {
            var entity = await _repository.FindAsync<LearnerMajorEntity>(p => p.Id == id) ?? throw new NotExistException("Major");
            entity.Name = dto.Name;

            await _repository.UpdateAsync(entity);

            return true;
        }

        public async Task<List<CComboxItem>> GetMajorSelectbox()
        {
            var majors = await _repository.GetSet<LearnerMajorEntity>(p => !p.IsDeleted).OrderBy(p => p.Name).ToListAsync();
            var result = majors.ConvertAll(i => new CComboxItem
            {
                Name = i.Name,
                Value = i.Id
            });

            return result;
        }

        private Expression<Func<LearnerMajorEntity, bool>> GenerateFilter(CTableParameter param)
        {
            var filter = PredicateBuilder.True<LearnerMajorEntity>();
            filter = filter.And(p => !p.IsDeleted);

            if (!string.IsNullOrWhiteSpace(param.SearchContent))
            {
                var content = param.SearchContent.ToLower().Trim();
                filter = filter.And(p => p.Name.ToLower().Contains(content));
            }

            return filter;
        }

        private Sorter<LearnerMajorEntity, object> GenerateSorter(CTableParameter param)
        {
            var result = new Sorter<LearnerMajorEntity, object> { IsAscending = param.IsAscending };

            switch (param.SortKey ?? "")
            {
                default:
                    result.SortBy = s => s.Name;
                    break;
            }

            return result;
        }
    }
}
