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
            : this(index, DateTimeOffset.UtcNow.ToUnixTimeSeconds(), proof, previousHash, transactions)
        {
        }

        public Block(long index, long timestamp, ProofOfWork proof, Sha256Hash previousHash, IEnumerable<Transaction> transactions)
        {
            if (index <= 0)
                throw new ArgumentException("Index must be greater than zero");
            if (timestamp <= 0)
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
            this.timestamp = timestamp;
            this.proof = proof;
            this.previousHash = previousHash;
            this.transactions = transactions.ToList();
        }

    public long Index => index;

    public long Timestamp => timestamp;

    public long Proof => proof.Value;

    public string PreviousHash => previousHash.ToString();

    public IReadOnlyCollection<Transaction> Transactions
    {
        get
        {
            return new List<Transaction>(transactions).AsReadOnly();
        }
    }
}
}