namespace System
{
    using System.Collections;
    using System.Text;

    public interface ITuple
    {
        int GetHashCode(IEqualityComparer comparer);
        string ToString(StringBuilder sb);

        int Size { get; }
    }
}
