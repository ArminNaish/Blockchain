using System;
using BlockChain.Infrastructure;
using BlockChain.Domain.Model;

namespace BlockChain.Application.Queries
{
    public class GetBlockchainById // todo: extract interface
    {
        private readonly ApiContext db;

        public GetBlockchainById(ApiContext db)
        {
            this.db = db;
        }

        public Blockchain Execute(Guid id)
        {
            // todo: add automapper
            return db.Blockchains.Find(id);
        }
    }
}