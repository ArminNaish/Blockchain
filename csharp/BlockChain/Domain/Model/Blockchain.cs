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
        private readonly List<Block> chain;
        private readonly ICollection<Transaction> currentTransactions;
        private readonly ICollection<Node> nodes;
        private readonly Node self;

        public Blockchain(Node self = null) : base(Guid.NewGuid())
        {
            this.chain = new List<Block> { Block.Genesis()};
            this.currentTransactions = new List<Transaction>();
            this.nodes = new List<Node>();
            this.self = self ?? Node.Default();
        }

        public IReadOnlyCollection<Block> Chain => chain.AsReadOnly();
        public IReadOnlyCollection<Transaction> CurrentTransactions => currentTransactions.AsReadOnly();
        public ICollection<Node> Nodes => nodes;
        private Block LastBlock => chain.Last();

        public static Blockchain From(IEnumerable<Block> chain, Node node)
        {
            if (node == null)
                throw new ArgumentNullException("Node must not be null");
            if (!chain.Any())
                throw new ArgumentException("Chain must not be empty");
            if (!chain.First().IsGenesisBlock())
                throw new Exception("Chain must start with genesis block");

            var blockchain = new Blockchain(node);
            blockchain.chain.Clear();
            blockchain.chain.AddRange(chain);
            return blockchain;
        }

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
        public bool ResolveConflicts(ICollection<Blockchain> otherBlockchains)
        {
            Blockchain newBlockchain = null;
            foreach (var otherBlockchain in otherBlockchains)
            {
                if (otherBlockchain.Chain.Count > chain.Count && otherBlockchain.Validate())
                {
                    newBlockchain = otherBlockchain;
                }
            }

            if (newBlockchain != null)
            {
                chain.Clear();
                chain.AddRange(newBlockchain.Chain);
                DomainEvents.Raise(new BlockchainReplacedEvent());
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determine if a given blockchain is valid
        /// </summary>
        public bool Validate()
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

        /// <summary>
        /// Create a new block in the blockchain
        /// </summary>
        private Block NewBlock(ProofOfWork proof)
        {
            var block = new Block(
                index: chain.Count + 1,
                proof: proof,
                previousHash: LastBlock.Hash(),
                transactions: currentTransactions);

            currentTransactions.Clear();
            chain.Add(block);

            DomainEvents.Raise(new NewBlockForgedEvent());

            return block;
        }
    }
}