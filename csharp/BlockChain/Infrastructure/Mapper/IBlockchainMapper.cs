using BlockChain.Domain.Model;

namespace BlockChain.Infrastructure.Mapper
{
    public interface IBlockchainMapper
    {
         Blockchain Map(Entities.Blockchain entity);
    }
}