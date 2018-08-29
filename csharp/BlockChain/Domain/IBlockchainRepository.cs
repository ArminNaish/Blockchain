using System;
using BlockChain.Domain.Model;

namespace BlockChain.Domain
{
    public interface IBlockchainRepository
    {
         Blockchain GetBlockChainById(Guid id);
    }
}