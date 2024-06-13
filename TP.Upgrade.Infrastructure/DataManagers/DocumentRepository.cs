using Microsoft.EntityFrameworkCore;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Domain.Models.DBEntity;
using TP.Upgrade.Infrastructure.DBContext;

namespace TP.Upgrade.Infrastructure.DataManagers
{
    public class DocumentRepository : Repository<ProductDocument>, IDocumentRepository
    {
        public DocumentRepository(TP_DbContext dbContext) : base(dbContext){}
    }
}
