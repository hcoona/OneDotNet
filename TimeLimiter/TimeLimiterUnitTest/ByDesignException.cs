using System;

namespace TimeLimiterUnitTest
{
    internal class ByDesignException : Exception
    {
    }

    internal class ByDesignException<T> : ByDesignException
    {
        public ByDesignException(T payload)
        {
            this.Data.Add("payload", payload);
        }

        public T Payload
        {
            get { return (T)this.Data["payload"]; }
        }
    }
}
