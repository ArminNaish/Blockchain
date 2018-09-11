using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using BlockChain.Domain.Events;
using System.Runtime.CompilerServices;

// Make internal methods visible to test project
[assembly:InternalsVisibleTo("Blockchain.Tests")]

namespace BlockChain.Domain.Model
{
    public class Blockchain : Entity<Guid>, IAggregateRoot
    {
        private IList<Block> blocks;
        private readonly ICollection<Transaction> currentTransactions;
        private readonly ICollection<Node> nodes;
        private readonly Node self;

        public Blockchain(Node self) : base(Guid.NewGuid())
        {
            this.blocks = new List<Block> { Block.Genesis()};
            this.currentTransactions = new List<Transaction>();
            this.nodes = new List<Node>();
            this.self = self;
        }

        public IReadOnlyCollection<Block> Blocks => blocks.AsReadOnly();
        public IReadOnlyCollection<Transaction> CurrentTransactions => currentTransactions.AsReadOnly();
        public ICollection<Node> Nodes => nodes;
        internal Block LastBlock => blocks.Last();

        /// <summary>
        /// Creates a new transaction to go into the next mined block
        /// </summary>
        public long NewTransaction(Node sender, Node recipient, long amount)
        {
            var transaction = new Transaction(sender, recipient, amount);
            currentTransactions.Add(transaction);
            DomainEvents.Raise(new NewTransactionCreatedEvent(transaction.Sender, transaction.Recepient,transaction.Amount));
            return LastBlock.Index + 1;
        }

        /// <summary>
        /// Mines a new block:
        /// 1) Calculate the Proof of Work
        /// 2) Reward the miner (us) by adding a transaction granting us 1 coin
        /// 3) Forge the new Block by adding it to the chain
        /// </summary>
        public Block Mine()
        {
            var proof = new Challenge().Solve(LastBlock.Proof);
            // the sender is 'null' to signify that this node has mined a new coin.
            currentTransactions.Add(new Transaction(null, self, 1));
            return NewBlock(proof);
        }

        /// <summary>
        /// Add a new node to the list of nodes
        /// </summary>
        public void Register(Uri address)
        {
            if (address == null)
                throw new ArgumentNullException("Address must not be null");
            var node = new Node(address);
            if (!Nodes.Contains(node))
            {
                Nodes.Add(node);
                DomainEvents.Raise(new NodeRegisteredEvent(node.Address));
            }
        }

        /// <summary>
        /// Consensus algorithm, resolves conflicts by replacing
        /// our chain with the longest one in the network.
        /// </summary>
        public bool ResolveConflicts(IEnumerable<IList<Block>> otherChains)
        {
            IList<Block> newChain = null;
            foreach (var otherChain in otherChains)
            {
                if (otherChain.Count > blocks.Count && Validate(otherChain))
                {
                    newChain = otherChain;
                }
            }

            if (newChain != null)
            {
                blocks = newChain;
                DomainEvents.Raise(new BlockchainReplacedEvent());
                return true;
            }

            return false;
        }

        /// <summary>
        /// Create a new block in the blockchain
        /// </summary>
        internal Block NewBlock(ProofOfWork proof)
        {
            if (proof == null)
                throw new ArgumentNullException("Proof must not be null");

            var block = new Block(
                index: blocks.Count + 1,
                proof: proof,
                previousHash: LastBlock.Hash(),
                transactions: currentTransactions);

            currentTransactions.Clear();
            blocks.Add(block);

            DomainEvents.Raise(new NewBlockForgedEvent());

            return block;
        }

        /// <summary>
        /// Determine if a given blockchain is valid
        /// </summary>
        private bool Validate(IList<Block> chain)
        {
            var lastBlock = chain.First();
            var currentIndex = 1;

            while (currentIndex < chain.Count)
            {
                var block = chain[currentIndex];
                if (block.PreviousHash != lastBlock.Hash())
                    return false;

                if (!block.VerifyProofOfWork(lastBlock))
                    return false;

                lastBlock = block;
                currentIndex += 1;
            }

            return true;
        }
    }
}