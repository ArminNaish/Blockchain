using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace BlockChain.Domain.Model
{
    public class Block : ValueObject
    {
        private readonly long index;
        private readonly long timestamp;
        private readonly ProofOfWork proof;
        private readonly Sha256Hash previousHash;
        private readonly ICollection<Transaction> transactions;

        public Block(long index, ProofOfWork proof, Sha256Hash previousHash, IEnumerable<Transaction> transactions, long? timestamp = null)
        {
            if (index <= 0)
                throw new ArgumentException("Index must be greater than zero");
            if (timestamp.HasValue && timestamp.Value <= 0)
                throw new ArgumentException("Timestamp must be greater than zero");
            if (index == 1)
                throw new ArgumentException("Index 1 is already used by genesis block");
            if (proof == null)
                throw new ArgumentNullException("Proof must not be null");
            if (previousHash == null)
                throw new ArgumentNullException("PreviousHash must not be null");
            if (!transactions.Any())
                throw new ArgumentException("Transactions must not be empty");

            this.index = index;
            this.timestamp = timestamp ?? DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            this.proof = proof;
            this.previousHash = previousHash;
            this.transactions = transactions.ToList();
        }

        public static Block Reconstitute(long index, long timestamp, long proof, string previousHash, IEnumerable<Transaction> transactions)
        {
            return new Block(index, new ProofOfWork(proof), new Sha256Hash(previousHash), transactions, timestamp);
        }

        public long Index => index;

        public long Timestamp => timestamp;

        public ProofOfWork Proof => proof;

        public Sha256Hash PreviousHash => previousHash;

        public IReadOnlyCollection<Transaction> Transactions
        {
            get
            {
                return new List<Transaction>(transactions).AsReadOnly();
            }
        }

        public Sha256Hash Hash()
        {
            return Sha256Hash.Of(this);
        }

        public bool VerifyProofOfWork(Block other)
        {
            return Proof.Verify(other.Proof);
        }
    }
}