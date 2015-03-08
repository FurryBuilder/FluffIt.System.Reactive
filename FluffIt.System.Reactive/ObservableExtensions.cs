//////////////////////////////////////////////////////////////////////////////////
//
// The MIT License (MIT)
//
// Copyright (c) 2014 Furry Builder
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
//////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using JetBrains.Annotations;

namespace FluffIt.System.Reactive
{
    [PublicAPI]
    public static class ObservableExtensions
    {
        /// <summary>
        ///     Project each element of an observable sequence into a generic value representing nothing.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements in the sequence</typeparam>
        /// <param name="source">Sequence to monitor</param>
        /// <returns>A new sequence of Units</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source" /> is null.</exception>
        [PublicAPI]
        public static IObservable<Unit> SelectUnit<TSource>([NotNull] this IObservable<TSource> source)
        {
            return source.Select(_ => Unit.Default);
        }

        /// <summary>
        ///     Project each element of the source observable sequence to the other observable sequence and merges the
        ///     resulting observable sequences into one observable sequence while stopping work on the previous inner sequence
        ///     when a new value arrives before completion.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements in the sequence</typeparam>
        /// <typeparam name="TResult">Type of the elements in the new sequence</typeparam>
        /// <param name="source">Sequence to alter</param>
        /// <param name="selector">A transform function to be applied on each element of the sequence</param>
        /// <returns>A new sequence of projected values</returns>
        [PublicAPI]
        public static IObservable<TResult> SelectManyDisposePrevious<TSource, TResult>(
            [NotNull] this IObservable<TSource> source,
            [NotNull] Func<TSource, IObservable<TResult>> selector)
        {
            var projectedSubscriptions = new SerialDisposable();

            return Observable.Create<TResult>(
                o => source.SubscribeSafe(
                    v => selector(v)
                        .Subscribe(o.OnNext, o.OnError, () => { })
                        .DisposeWith(projectedSubscriptions),
                    o.OnError,
                    () =>
                    {
                        projectedSubscriptions.Dispose();
                        o.OnCompleted();
                    }
                )
            );
        }

        /// <summary>
        ///     Replace each element of an observable sequence into a specified value when the source is
        ///     considered to be a default value for the specified type.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements in the sequence</typeparam>
        /// <param name="source">Sequence to alter</param>
        /// <param name="defaultValueFactory">Value provider when a default value is found</param>
        /// <param name="valueTypeComparer">Comparer to determine if the value is a default value</param>
        /// <returns>A new sequence of defaulted values</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source" /> is null.</exception>
        [PublicAPI]
        public static IObservable<TSource> Default<TSource>(
            [NotNull] this IObservable<TSource> source,
            [NotNull] Func<TSource> defaultValueFactory,
            [CanBeNull] IEqualityComparer<TSource> valueTypeComparer = null)
        {
            return source.Select(v => v.Default(defaultValueFactory, valueTypeComparer));
        }

        /// <summary>
        ///     Subscribe to an observable sequence while rerouting errors happening on the onNext channel to the onError channel.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements in the sequence</typeparam>
        /// <param name="source">Sequence to subscribe to</param>
        /// <param name="onNext">Action to execute when a new value is passed in the sequence</param>
        /// <param name="onError">Action to execute when an error occured in the sequence</param>
        /// <param name="onCompleted">Action to execute when the sequence completes</param>
        /// <returns>A disposable that controls the lifetime of the sequence</returns>
        /// <exception cref="Exception">A delegate callback throws an exception. </exception>
        /// <exception cref="ArgumentNullException"><paramref name="source" /> is null.</exception>
        [PublicAPI]
        public static IDisposable SubscribeSafe<TSource>(
            [NotNull] this IObservable<TSource> source,
            [CanBeNull] Action<TSource> onNext = null,
            [CanBeNull] Action<Exception> onError = null,
            [CanBeNull] Action onCompleted = null)
        {
            return source
                .Do(v => onNext.Maybe(a => a.Invoke(v)))
                .Subscribe(
                    _ => { },
                    ex => onError.Maybe(a => a.Invoke(ex)),
                    () => onCompleted.Maybe(a => a.Invoke())
                );
        }

        /// <summary>
        ///     Reinject the previous value from the observable sequence into the new sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements in the sequence</typeparam>
        /// <param name="source">Sequence to alter</param>
        /// <returns>A new sequence of values and their previous value</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source" /> is null.</exception>
        [PublicAPI]
        public static IObservable<PhasingWrapper<TSource>> WithPreviousValue<TSource>(
            [NotNull] this IObservable<TSource> source)
        {
            var previousValue = default(TSource);

            return source.Select(
                v =>
                {
                    var wrapper = new PhasingWrapper<TSource>(previousValue, v);
                    previousValue = v;
                    return wrapper;
                }
            );
        }

        /// <summary>
        ///     Inject an index into the obserrvable sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements in the sequence</typeparam>
        /// <typeparam name="TIndex">Type of the index to generate</typeparam>
        /// <param name="source">Sequence to alter</param>
        /// <param name="indexer">Method to generate an index from a source value</param>
        /// <returns>A new sequence of values and their indexes</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source" /> is null.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception. </exception>
        [PublicAPI]
        public static IObservable<IndexingWrapper<TSource, TIndex>> WithIndex<TSource, TIndex>(
            [NotNull] this IObservable<TSource> source,
            [NotNull] Func<TSource, TIndex> indexer)
        {
            return source.Select(v => new IndexingWrapper<TSource, TIndex>(v, indexer.Invoke(v)));
        }

        /// <summary>
        ///     Inject a sequential index into the observable sequence.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements in the sequence</typeparam>
        /// <param name="source">Sequence to alter</param>
        /// <returns>A new sequence of values and their indexes</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source" /> is null.</exception>
        [PublicAPI]
        public static IObservable<IndexingWrapper<TSource, int>> WithIndex<TSource>(
            [NotNull] this IObservable<TSource> source)
        {
            var count = 0;

            return source.Select(v => new IndexingWrapper<TSource, int>(v, ++count));
        }

        /// <summary>
        ///     Wraps values from an observable sequence with the previous value from the same sequence.
        /// </summary>
        /// <typeparam name="T">Type of values in the sequence</typeparam>
        [PublicAPI]
        public class PhasingWrapper<T>
        {
            internal PhasingWrapper([CanBeNull] T previousValue, [CanBeNull] T currentValue)
            {
                PreviousValue = previousValue;
                CurrentValue = currentValue;
            }

            [PublicAPI]
            public T PreviousValue { get; private set; }

            [PublicAPI]
            public T CurrentValue { get; private set; }
        }

        /// <summary>
        ///     Wraps values from an observable sequence with indexing information.
        /// </summary>
        /// <typeparam name="TValue">Type of values in the sequence</typeparam>
        /// <typeparam name="TIndex">Type of the index</typeparam>
        [PublicAPI]
        public class IndexingWrapper<TValue, TIndex>
        {
            internal IndexingWrapper([CanBeNull] TValue currentValue, [NotNull] TIndex indexValue)
            {
                CurrentValue = currentValue;
                IndexValue = indexValue;
            }

            [PublicAPI]
            public TValue CurrentValue { get; private set; }

            [PublicAPI]
            public TIndex IndexValue { get; private set; }
        }
    }
}