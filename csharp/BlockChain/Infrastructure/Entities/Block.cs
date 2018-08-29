using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlockChain.Infrastructure.Entities
{
    public class Block
    {
        public long Index  {get; set;}
        public long Timestamp  {get; set;}
        public ProofOfWork Proof {get; set;}
        public string PreviousHash {get; set;}
        public ICollection<Transaction> Transactions {get; set;}
    }
}