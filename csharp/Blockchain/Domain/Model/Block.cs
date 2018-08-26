using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace Blockchain.Domain.Model
{
    public class Block : ValueObject
    {
        protected readonly long index;
        private readonly long timestamp;
        private readonly ProofOfWork proof;
        private readonly Sha256Hash previousHash;
        private readonly ICollection<Transaction> transactions;

        public Block(long index, long timestamp, ProofOfWork proof, Sha256Hash previousHash, IEnumerable<Transaction> transactions)
        {
            if (index < 0)
                throw new ArgumentException("Index must not be negative");
            if (timestamp < 0)
                throw new ArgumentException("Timestamp must not be negative");
            if (proof == null)
                throw new ArgumentNullException("Proof must not be null");
            if (previousHash == null)
                throw new ArgumentNullException("PreviousHash must not be null");
            if (!transactions.Any())
                throw new ArgumentException("Transactions must not be empty");

            this.index = index;
            this.timestamp = timestamp;
            this.proof = proof;
            this.previousHash = previousHash;
            this.transactions = transactions.ToList();
        }

        // Used by genesis block only
        protected Block() { }

        public virtual long Index => index;

        public virtual long Timestamp => timestamp;

        public virtual ProofOfWork Proof => proof;

        public virtual Sha256Hash PreviousHash => previousHash;

        public virtual IReadOnlyCollection<Transaction> Transactions
        {
            get
            {
                return new List<Transaction>(transactions).AsReadOnly();
            }
        }
    }
}