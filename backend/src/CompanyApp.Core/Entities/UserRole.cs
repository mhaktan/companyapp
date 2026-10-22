using System;
using Abp.Domain.Entities.Auditing;

namespace CompanyApp.Entities
{
    public class UserRole : CreationAuditedEntity<long>
    {
        public long UserId { get; set; }
        public virtual AppUser User { get; set; }

        public long RoleId { get; set; }
        public virtual AppRole Role { get; set; }
    }
}
