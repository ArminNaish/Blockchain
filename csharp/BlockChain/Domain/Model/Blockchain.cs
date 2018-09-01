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
        private readonly ICollection<Node> nodes;
        private readonly Node self; // todo: this must be configurable

        public Blockchain(Guid? id = null) : base(id ?? Guid.NewGuid())
        {
            chain = new List<Block> { };
            currentTransactions = new List<Transaction>();
            nodes = new List<Node>();
            self = new Node(new Uri(@"http://127.0.0.5:5000"));
            NewBlock(new ProofOfWork(1), Sha256Hash.Of("Genesis"));
        }

        public IReadOnlyCollection<Block> Chain => chain.AsReadOnly();

        public IReadOnlyCollection<Transaction> CurrentTransactions => currentTransactions.AsReadOnly();
        public ICollection<Node> Nodes => nodes;

        private Block LastBlock => chain.Last();

        public long NewTransaction(Node sender, Node recipient, long amount)
        {
            var transaction = new Transaction(sender, recipient, amount);
            currentTransactions.Add(transaction);
            return LastBlock.Index + 1;
            // todo: raise domain event
        }

        public Block Mine()
        {
            // Solve challenge and receive proof of work
            var proof = new Challenge().Solve(LastBlock.Proof);
            // We must receive a reward for finding the proof
            // The sender is 'null' to signify that this node has mined a new coin.
            currentTransactions.Add(new Transaction(null, self, 1));
            // Forge the new Block by adding it to the chain
            return NewBlock(proof);
        }

        public void Register(Uri address)
        {
            var node = new Node(address);
            if (!Nodes.Contains(node))
            {
                Nodes.Add(node);
                // todo: raise domain event
            }
        }

        private Block NewBlock(ProofOfWork proof, Sha256Hash previousHash = null)
        {
            if (proof == null)
                throw new ArgumentNullException("Proof must not be null");

            var block = new Block(
                index: chain.Count + 1,
                proof: proof,
                previousHash: previousHash ?? Sha256Hash.Of(LastBlock),
                transactions: currentTransactions);

            currentTransactions.Clear();
            chain.Add(block);

            // todo: raise domain event

            return block;
        }
    }
}