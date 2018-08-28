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

        public Block(long index, ProofOfWork proof, Sha256Hash previousHash, IEnumerable<Transaction> transactions)
        {
            if (index <= 0)
                throw new ArgumentException("Index must be greater than zero");
            if (index == 1)
                throw new ArgumentException("Index 1 is already used by genesis block");
            if (proof == null)
                throw new ArgumentNullException("Proof must not be null");
            if (previousHash == null)
                throw new ArgumentNullException("PreviousHash must not be null");
            if (!transactions.Any())
                throw new ArgumentException("Transactions must not be empty");

            this.index = index;
            this.timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            this.proof = proof;
            this.previousHash = previousHash;
            this.transactions = transactions.ToList();
        }

        
        private Block()
        {
            // This constructor is used by genesis block
            // which does not neither proof nor previous hash
            this.index = 1;
            this.timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            this.transactions = new List<Transaction>();
        }

        public static Block Genesis()
        {
            return new Block();
        }

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