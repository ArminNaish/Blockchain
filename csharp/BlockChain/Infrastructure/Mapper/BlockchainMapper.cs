using System.Collections.Generic;
using System.Linq;
using BlockChain.Domain.Model;

namespace BlockChain.Infrastructure.Mapper
{
    public class BlockchainMapper : IBlockchainMapper
    {
        public Blockchain Map(Entities.Blockchain entity)
        {
            var chain = entity.Chain.Select(Map).ToList();
            var currentTransactions = entity.CurrentTransactions.Select(Map).ToList();
            var blockchain = Blockchain.Reconstitute(entity.Id, chain, currentTransactions);
            return blockchain;
        }

        private static Block Map(Entities.Block block)
        {
            var proof = Map(block.Proof);
            var previousHash = Sha256Hash.Reconstitute(block.PreviousHash);
            var transactions = block.Transactions.Select(Map).ToList();
            return new Block(block.Index, block.Timestamp, proof, previousHash, transactions);
        }

        private static ProofOfWork Map(Entities.ProofOfWork proofOfWork)
        {
            return new ProofOfWork(proofOfWork.Proof, proofOfWork.LastProof);
        }

        private static Transaction Map(Entities.Transaction transaction)
        {
            return new Transaction(transaction.Sender, transaction.Recepient, transaction.Amount);
        }
    }
}