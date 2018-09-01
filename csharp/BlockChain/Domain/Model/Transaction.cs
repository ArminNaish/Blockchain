using System;

namespace BlockChain.Domain.Model
{
    public class Transaction : ValueObject
    {
        private readonly Node sender;
        private readonly Node recepient;
        private readonly long amount;

        public Transaction(Node sender, Node recipient, long amount)
        {
            if (object.Equals(sender, default(Guid)))
                throw new ArgumentException("Sender must not be empty");
            if (object.Equals(recipient, default(Guid)))
                throw new ArgumentException("Recipient must not be empty");
            if (amount <=0)
                throw new ArgumentException("Amount must not be negative or zero");

            this.sender = sender;
            this.recepient = recipient;
            this.amount = amount;
        }

        public Node Sender => sender;

        public Node Recepient => recepient;

        public long Amount => amount;
    }
}