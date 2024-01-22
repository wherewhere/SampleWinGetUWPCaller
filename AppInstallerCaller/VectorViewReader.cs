using System.Collections;
using System.Collections.Generic;
using Windows.Foundation.Collections;

namespace AppInstallerCaller
{
    /// <summary>
    /// The reader of <see cref="IVectorView{T}"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements of <paramref name="List"/>.</typeparam>
    /// <param name="source">The <typeparamref name="IVectorView{TSource}"/> to be redden.</param>
    public readonly struct VectorViewReader<T>(IReadOnlyList<T> source) : IReadOnlyList<T>
    {
        /// <inheritdoc/>
        public T this[int index] => source[index];

        /// <inheritdoc/>
        public int Count => source.Count;

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < source.Count; i++)
            {
                yield return source[i];
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
