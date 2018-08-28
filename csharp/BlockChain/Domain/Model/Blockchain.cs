using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace BlockChain.Domain.Model
{
    public class Blockchain : Entity<Guid>, IAggregateRoot
    {
        private readonly ICollection<Block> chain;
        private readonly ICollection<Transaction> currentTransactions;

        public Blockchain() : base(Guid.NewGuid())
        {
            chain = new List<Block>{Block.Genesis()};
            currentTransactions = new List<Transaction>();
        }

        public ICollection<Block> Chain => chain;

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

        private Block NewBlock(ProofOfWork proof) // or challenge??
        {
            var block = new Block(
                index: chain.Count + 1,
                proof: proof,
                previousHash: Sha256Hash.Of(LastBlock),
                transactions: currentTransactions);

            currentTransactions.Clear();
            chain.Add(block);

            return block;
        }
    }
}