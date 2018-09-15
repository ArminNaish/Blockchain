using System;

namespace BlockChain.Domain.Model
{
    public class Transaction : ValueObject
    {
        private readonly Node sender;
        private readonly Node recipient;
        private readonly long amount;

        public Transaction(Node sender, Node recipient, long amount)
        {
            if (recipient == null)
                throw new ArgumentNullException("Recipient must not be null");
            if (amount <= 0)
                throw new ArgumentException("Amount must not be negative or zero");

            this.sender = sender;
            this.recipient = recipient;
            this.amount = amount;
        }

        public Node Sender => sender;

        public Node Recepient => recipient;

        public long Amount => amount;
    }
}