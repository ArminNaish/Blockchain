using System;

namespace Blockchain.Domain.Model
{
    public class ProofOfWork : ValueObject
    {
        private readonly ProofOfWork lastProof;
        private readonly long proof;

        public ProofOfWork(long proof,ProofOfWork lastProof = null)
        {
            if (proof < 0)
                throw new ArgumentException("Proof must not be negative");
            this.lastProof = lastProof;
            this.proof = proof;
        }

        public long Proof => proof;
        
        public static ProofOfWork Calculate(ProofOfWork lastProof)
        {
            var proof = 0;
            ProofOfWork pow;
            while (!(pow = new ProofOfWork(proof,lastProof)).Verify())
                proof += 1;
            return pow;
        }

        public bool Verify()
        {
            var sha256 = Sha256Hash.Of($"{lastProof.Proof}{proof}");
            return sha256.Hash.StartsWith("0000");
        }

    }
}