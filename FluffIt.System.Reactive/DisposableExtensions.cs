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
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Threading;
using JetBrains.Annotations;

namespace FluffIt.System.Reactive
{
    [PublicAPI]
    public static class DisposableExtensions
    {
        /// <summary>
        ///     Dispose the disposable on a single assignment disposable.
        /// </summary>
        /// <param name="disposable">The object to be disposed</param>
        /// <param name="disposer">The object to use as disposer</param>
        /// <exception cref="InvalidOperationException">Thrown if the SingleAssignmentDisposable has already been assigned to.</exception>
        [PublicAPI]
        public static void DisposeWith(
            [NotNull] this IDisposable disposable,
            [NotNull] SingleAssignmentDisposable disposer)
        {
            disposer.Disposable = disposable;
        }

        /// <summary>
        ///     Dispose the disposable on a multiple assignment disposable.
        /// </summary>
        /// <param name="disposable">The object to be disposed</param>
        /// <param name="disposer">The object to use as disposer</param>
        [PublicAPI]
        public static void DisposeWith(
            [NotNull] this IDisposable disposable,
            [NotNull] MultipleAssignmentDisposable disposer)
        {
            disposer.Disposable = disposable;
        }

        /// <summary>
        ///     Dispose the disposable on a serial disposable.
        /// </summary>
        /// <param name="disposable">The object to be disposed</param>
        /// <param name="disposer">The object to use as disposer</param>
        [PublicAPI]
        public static void DisposeWith([NotNull] this IDisposable disposable, [NotNull] SerialDisposable disposer)
        {
            disposer.Disposable = disposable;
        }

        /// <summary>
        ///     Dispose the disposable on a composite disposable.
        /// </summary>
        /// <param name="disposable">The object to be disposed</param>
        /// <param name="disposer">The object to use as disposer</param>
        /// <exception cref="ArgumentNullException"><paramref name="disposable" /> is null.</exception>
        [PublicAPI]
        public static void DisposeWith([NotNull] this IDisposable disposable, [NotNull] CompositeDisposable disposer)
        {
            disposer.Add(disposable);
        }

        /// <summary>
        ///     Dispose the disposable on a generic disposable implementation.
        /// </summary>
        /// <param name="disposable">The object to be disposed</param>
        /// <param name="disposer">The object to use as disposer</param>
        /// <exception cref="InvalidOperationException">Thrown if the SingleAssignmentDisposable has already been assigned to.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="disposable" /> is null.</exception>
        /// <exception cref="NotSupportedException">Unsupported disposer type</exception>
        [PublicAPI]
        public static void DisposeWith([NotNull] this IDisposable disposable, [NotNull] IDisposable disposer)
        {
            var supported = false;

            disposer
                .As((SingleAssignmentDisposable d) =>
                {
                    disposable.DisposeWith(d);
                    supported = true;
                })
                .As((MultipleAssignmentDisposable d) =>
                {
                    disposable.DisposeWith(d);
                    supported = true;
                })
                .As((SerialDisposable d) =>
                {
                    disposable.DisposeWith(d);
                    supported = true;
                })
                .As((CompositeDisposable d) =>
                {
                    disposable.DisposeWith(d);
                    supported = true;
                });

            if (!supported)
            {
                throw new NotSupportedException("Unsupported disposer type");
            }
        }

        /// <summary>
        ///     Dispose the disposable on a specified scheduler.
        /// </summary>
        /// <param name="disposable">The object to be disposed</param>
        /// <param name="scheduler">The scheduler to use when disposing the object</param>
        /// <returns>Returns a new disposable that handles disposal on a different scheduler</returns>
        /// <exception cref="ArgumentNullException"><paramref name="scheduler" /> or <paramref name="disposable" /> is null.</exception>
        [PublicAPI, Pure]
        public static IDisposable DisposeOn([NotNull] this IDisposable disposable, [NotNull] IScheduler scheduler)
        {
            return new ScheduledDisposable(scheduler, disposable);
        }

        /// <summary>
        ///     Dispose the disposable on a specified context.
        /// </summary>
        /// <param name="disposable">The object to be disposed</param>
        /// <param name="context">The context to use when disposing the object</param>
        /// <returns>Returns a new disposable that handles disposal on a different context</returns>
        /// <exception cref="ArgumentNullException"><paramref name="context" /> or <paramref name="disposable" /> is null.</exception>
        [PublicAPI, Pure]
        public static IDisposable DisposeOn(
            [NotNull] this IDisposable disposable,
            [NotNull] SynchronizationContext context)
        {
            return new ContextDisposable(context, disposable);
        }

        /// <summary>
        ///     Dispose the disposable when all of its dependent disposables have been disposed.
        /// </summary>
        /// <param name="disposable">The disposable that will be reference counted</param>
        /// <returns>Returns a new disposable that handles disposal based on a reference counter</returns>
        /// <exception cref="ArgumentNullException"><paramref name="disposable" /> is null.</exception>
        [PublicAPI, Pure]
        public static RefCountDisposable RefCounted([NotNull] this IDisposable disposable)
        {
            return new RefCountDisposable(disposable);
        }
    }
}