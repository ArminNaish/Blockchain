using System;

namespace BlockChain.Domain.Model
{
    public class Transaction : ValueObject
    {
        private readonly Guid sender;
        private readonly Guid recepient;
        private readonly long amount;

        public Transaction(Guid sender, Guid receiver, long amount)
        {
            if (object.Equals(sender, default(Guid)))
                throw new ArgumentException("Sender must not be empty");
            if (object.Equals(receiver, default(Guid)))
                throw new ArgumentException("Receiver must not be empty");
            if (amount <=0)
                throw new ArgumentException("Amount must not be negative or zero");

            this.sender = sender;
            this.recepient = receiver;
            this.amount = amount;
        }
    }
}