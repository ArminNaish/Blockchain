using System;
using Newtonsoft.Json;

namespace BlockChain.Domain.Model
{
    public abstract class ValueObject : IEquatable<ValueObject>
    {
        public bool Equals(ValueObject other)
        {
            if (other == null)
            {
                return false;
            }

            return this.AsJson() == other.AsJson();
        }

        public override bool Equals(object obj)
        {
            var entity = obj as ValueObject;
            if (entity != null)
            {
                return this.Equals(entity);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.AsJson().GetHashCode();
        }

        public override string ToString()
        {
            return this.AsJson();
        }

        public static bool operator ==(ValueObject a, ValueObject b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (((object)a == null) || ((object)b == null)) return false;

            return a.AsJson() == b.AsJson();
        }

        public static bool operator !=(ValueObject a, ValueObject b) => !(a == b);
    }
}