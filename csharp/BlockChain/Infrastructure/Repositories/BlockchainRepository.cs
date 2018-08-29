using System;
using BlockChain.Domain;
using BlockChain.Domain.Model;
using BlockChain.Infrastructure.Mapper;

namespace BlockChain.Infrastructure.Repositories
{
    public class BlockchainRepository : IBlockchainRepository
    {
        private readonly IBlockchainMapper mapper;
        private readonly ApiContext db;

        public BlockchainRepository(IBlockchainMapper mapper, ApiContext db)
        {
            this.mapper = mapper;
            this.db = db;
        }

        public Blockchain GetBlockChainById(Guid id)
        {
            var blockchain= db.Blockchains.Find(id);
            if (blockchain == null)
                throw new NullReferenceException($"Could not find blockchain: {id}");
            return mapper.Map(blockchain);
        }
    }
}