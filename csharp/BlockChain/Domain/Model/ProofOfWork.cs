using System;

namespace BlockChain.Domain.Model
{
    public class ProofOfWork : ValueObject
    {
        private readonly long proof;
        private readonly ProofOfWork lastProof;

        public ProofOfWork(long proof,ProofOfWork lastProof)
        {
            if (proof < 0)
                throw new ArgumentException("Proof must not be negative");
            if (lastProof == null)
                throw new ArgumentNullException("Last proof must not be null");
            this.proof = proof;
            this.lastProof = lastProof;
        }

        public long Proof => proof;
        
        public bool Verify()
        {
            return Sha256Hash
                .Of($"{lastProof.Proof}{proof}")
                .StartsWith("0000");
        }

    }
}