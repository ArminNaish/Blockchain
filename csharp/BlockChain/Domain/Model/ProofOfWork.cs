using System;

namespace BlockChain.Domain.Model
{
    public class ProofOfWork : ValueObject
    {
        private readonly long value;
        private readonly long? lastValue;

        public ProofOfWork(long value, long? lastValue = null)
        {
            if (value < 0)
                throw new ArgumentException("Value must not be negative");
              if (lastValue.HasValue && lastValue.Value < 0)
                throw new ArgumentException("Last value must not be negative");
            this.value = value;
            this.lastValue = lastValue;
        }

        public virtual long Value => value;

        public virtual bool Verify()
        {
            if (lastValue == null)
                throw new InvalidOperationException("Genesis block cannot be verified");

            return Sha256Hash
                .Of($"{lastValue}{value}")
                .StartsWith("0000");
        }

    }
}