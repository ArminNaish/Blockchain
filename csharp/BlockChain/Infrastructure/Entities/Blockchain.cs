using System;
using System.Collections.Generic;

namespace BlockChain.Infrastructure.Entities
{
    public class Blockchain
    {
        public Guid Id {get;set;}
        public ICollection<Block> Chain {get; set;}
        public ICollection<Transaction> CurrentTransactions {get;set;}
    }
}