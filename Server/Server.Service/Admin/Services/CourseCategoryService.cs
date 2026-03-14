using Common.Domain;
using Common.Repository;
using Microsoft.EntityFrameworkCore;
using Server.Domain.Admin;
using System.Linq.Expressions;

namespace Server.Service.Admin
{
    internal class CourseCategoryService(
        IDBRepository _repository
        ) : ICourseCategoryService
    {
        public async Task<TableInfo<CourseCategoryDto>> GetPaging(CTableParameter parameter)
        {
            var query = new TableQueryParameter<CourseCategoryEntity>
            {
                Pager = new Pager { PageIndex = parameter.PageIndex, PageSize = parameter.PageSize },
                Sorter = GenerateSorter(parameter),
                Filter = GenerateFilter(parameter)
            };

            return await _repository.GetWithPagingAsync(query, s => new CourseCategoryDto
            {
                Id = s.Id,
                Name = s.Name,
                IsDeleted = s.IsDeleted,
                ModifiedAt = s.ModifiedAt,
            });
        }

        public async Task<CourseCategoryDto> GetDetail(Guid id)
        {
            var entity = await _repository.FindAsync<CourseCategoryEntity>(p => p.Id == id) ?? throw new NotExistException("Major");

            return new CourseCategoryDto
            {
                Id = entity.Id,
                Name = entity.Name,
            };
        }

        public async Task<bool> Add(CourseCategoryDto dto)
        {
            var major = new CourseCategoryEntity
            {
                Name = dto.Name,
            };

            await _repository.AddAsync(major);

            return true;
        }

        public async Task<bool> Update(Guid id, CourseCategoryDto dto)
        {
            var entity = await _repository.FindAsync<CourseCategoryEntity>(p => p.Id == id) ?? throw new NotExistException("Major");
            entity.Name = dto.Name;

            await _repository.UpdateAsync(entity);

            return true;
        }

        public async Task<bool> Delete(Guid id)
        {
            var entity = await _repository.FindAsync<CourseCategoryEntity>(p => p.Id == id) ?? throw new NotExistException("Major");
            entity.IsDeleted = true;

            await _repository.UpdateAsync(entity);
            return true;
        }

        public async Task<List<CComboxItem>> GetCategorySelectbox()
        {
            var categories = await _repository.GetSet<CourseCategoryEntity>(p => !p.IsDeleted).OrderBy(p => p.Name).ToListAsync();
            var result = categories.ConvertAll(i => new CComboxItem
            {
                Name = i.Name,
                Value = i.Id
            });

            return result;
        }

        #region Private method
        private Expression<Func<CourseCategoryEntity, bool>> GenerateFilter(CTableParameter param)
        {
            var filter = PredicateBuilder.True<CourseCategoryEntity>();
            filter = filter.And(p => !p.IsDeleted);

            if (!string.IsNullOrWhiteSpace(param.SearchContent))
            {
                var content = SearchBuilder.BuildContent(param.SearchContent);
                filter = filter.And(p => EF.Functions.Like(p.Name.ToLower(), content.Pattern, content.EscapeCharacter));
            }

            return filter;
        }

        private Sorter<CourseCategoryEntity, object> GenerateSorter(CTableParameter param)
        {
            var result = new Sorter<CourseCategoryEntity, object> { IsAscending = param.IsAscending };

            switch (param.SortKey ?? "")
            {
                default:
                    result.SortBy = s => s.Name;
                    break;
            }

            return result;
        }
        #endregion
    }
}
