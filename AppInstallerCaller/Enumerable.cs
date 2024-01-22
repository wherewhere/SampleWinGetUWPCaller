using System.Collections.Generic;
using Windows.Foundation.Collections;

namespace AppInstallerCaller
{
    public static class Enumerable
    {
        /// <summary>
        /// Get the <see cref="VectorViewReader{TSource}"/> of <see cref="IVectorView{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The <typeparamref name="IVectorView{TSource}"/> to be redden.</param>
        /// <returns>The <see cref="VectorViewReader{TSource}"/> of <see cref="IVectorView{TSource}"/>.</returns>
        public static VectorViewReader<TSource> AsReader<TSource>(this IReadOnlyList<TSource> source) => new(source);
    }
}
