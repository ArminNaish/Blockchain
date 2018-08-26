using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Blockchain.Domain.Model
{
    public class BlockChain : Entity<Guid>, IAggregateRoot
    {
        private readonly ICollection<Block> chain;
        private readonly ICollection<Transaction> currentTransactions;

        public BlockChain() : base(Guid.NewGuid())
        {
            chain = new List<Block>();
            currentTransactions = new List<Transaction>();

            NewBlock(new ProofOfWork(100), Sha256Hash.Of("1"));
        }

        private Block LastBlock
        {
            get
            {
                return chain.Last();
            }
        }

        // todo: add method Mine()

        private long NewTransaction(Guid sender, Guid receiver, long amount)
        {
            currentTransactions.Add(new Transaction(sender, receiver, amount));
            return LastBlock.Index + 1;
        }

        private Block NewBlock(ProofOfWork proof, Sha256Hash previousHash = null)
        {
            var block = new Block(
                index: chain.Count + 1,
                timestamp: DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                proof: proof,
                previousHash: previousHash ?? Sha256Hash.Of(LastBlock),
                transactions: currentTransactions);

            currentTransactions.Clear();
            chain.Add(block);

            return block;
        }
    }
}