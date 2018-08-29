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
            chain = new List<Block>{};
            currentTransactions = new List<Transaction>();
            NewBlock(new ProofOfWork(1),  Sha256Hash.Of("Genesis"));
        }

        private Blockchain(Guid id, ICollection<Block> chain, ICollection<Transaction> currentTransactions):base(id)
        {
            this.chain = chain.ToList();
            this.currentTransactions = currentTransactions.ToList();
        }

        public static Blockchain Reconstitute(Guid id, ICollection<Block> chain, ICollection<Transaction> currentTransactions)
        {
            return new Blockchain(id, chain, currentTransactions);
        }

        public ICollection<Block> Chain => chain;

        private Block LastBlock
        {
            get
            {
                return chain.Last();
            }
        }

        public long NewTransaction(Guid sender, Guid recipient, long amount)
        {
            currentTransactions.Add(new Transaction(sender, recipient, amount));
            return LastBlock.Index + 1;
            // todo: raise domain event
        }

        public Block Mine(Guid nodeId)
        {
            // Solve challenge and receive proof of work
            var proof = new Challenge().Solve(LastBlock.Proof);
            // We must receive a reward for finding the proof
            // The sender is 'null' to signify that this node has mined a new coin.
            currentTransactions.Add(new Transaction(null, nodeId, 1));
            // Forge the new Block by adding it to the chain
            return NewBlock(proof);
            // todo: raise domain event
        }

        private Block NewBlock(ProofOfWork proof, Sha256Hash previousHash = null)
        {
            var block = new Block(
                index: chain.Count + 1,
                proof: proof,
                previousHash: previousHash ?? Sha256Hash.Of(LastBlock),
                transactions: currentTransactions);

            currentTransactions.Clear();
            chain.Add(block);

            return block;
        }
    }
}