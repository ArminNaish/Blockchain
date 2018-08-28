using System;

namespace BlockChain.Infrastructure.Entities
{
    public class Transaction
    {
        public  Guid Sender {get;set;}
        public  Guid Recepient{get;set;}
        public  long Amount{get;set;}
    }
}