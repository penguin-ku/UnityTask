namespace System.Threading.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;

    public class AggregateException : Exception
    {
        public AggregateException(IEnumerable<Exception> innerExceptions)
        {
            this.InnerExceptions = new ReadOnlyCollection<Exception>(innerExceptions.ToList<Exception>());
        }

        public AggregateException Flatten()
        {
            List<Exception> innerExceptions = new List<Exception>();
            foreach (Exception exception in this.InnerExceptions)
            {
                AggregateException exception2 = exception as AggregateException;
                if (exception2 == null)
                {
                    innerExceptions.Add(exception);
                }
                else
                {
                    innerExceptions.AddRange(exception2.Flatten().InnerExceptions);
                }
            }
            return new AggregateException(innerExceptions);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(base.ToString());
            foreach (Exception exception in this.InnerExceptions)
            {
                builder.AppendLine("\n-----------------");
                builder.AppendLine(exception.ToString());
            }
            return builder.ToString();
        }

        public ReadOnlyCollection<Exception> InnerExceptions { get; private set; }
    }
}

