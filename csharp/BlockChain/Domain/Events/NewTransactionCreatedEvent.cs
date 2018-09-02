using BlockChain.Domain.Model;

namespace BlockChain.Domain.Events
{
    internal class NewTransactionCreatedEvent : IDomainEvent
    {
        private readonly Node sender;
        private readonly Node recepient;
        private readonly long amount;

        public NewTransactionCreatedEvent(Node sender, Node recepient, long amount)
        {
            this.sender = sender;
            this.recepient = recepient;
            this.amount = amount;
        }
    }
}