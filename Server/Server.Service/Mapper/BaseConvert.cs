using Common.Domain;
using Common.Repository;

namespace Server.Service
{
    public class BaseConvert
    {
        public static T MapBase<T>(BaseEntity entity) where T : BaseDto, new()
        {
            return new T
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                ModifiedAt = entity.ModifiedAt,
                ModifiedBy = entity.ModifiedBy,
                CreatedBy = entity.CreatedBy,
                IsDeleted = entity.IsDeleted
            };
        }
    }
}
