using System;

namespace BlockChain.Domain.Model
{
    public class ProofOfWork : ValueObject
    {
        private readonly long proof;

        public ProofOfWork(long proof)
        {
            if (proof < 0)
                throw new ArgumentException("Proof must not be negative");
            this.proof = proof;
        }

        internal long Proof => proof;

        public virtual bool Verify(ProofOfWork lastProof)
        {
            if (lastProof == null)
                throw new InvalidOperationException("Last proof must not be null");

            return Sha256Hash
                .Of($"{lastProof.Proof}{Proof}")
                .StartsWith("0000");
        }
    }
}