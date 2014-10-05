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

namespace FluffIt.System.Reactive
{
	public static class DisposableExtensions
	{
		public static void DisposeWith(this IDisposable disposable, SingleAssignmentDisposable disposer)
		{
			disposer.Disposable = disposable;
		}

		public static void DisposeWith(this IDisposable disposable, MultipleAssignmentDisposable disposer)
		{
			disposer.Disposable = disposable;
		}

		public static void DisposeWith(this IDisposable disposable, SerialDisposable disposer)
		{
			disposer.Disposable = disposable;
		}

		public static void DisposeWith(this IDisposable disposable, CompositeDisposable disposer)
		{
			disposer.Add(disposable);
		}

		public static void DisposeWith(this IDisposable disposable, IDisposable disposer)
		{
			var supported = false;

			disposer
				.As((SingleAssignmentDisposable d) => { disposable.DisposeWith(d); supported = true; })
				.As((MultipleAssignmentDisposable d) => { disposable.DisposeWith(d); supported = true; })
				.As((SerialDisposable d) => { disposable.DisposeWith(d); supported = true; })
				.As((CompositeDisposable d) => { disposable.DisposeWith(d); supported = true; });

			if (!supported)
			{
				throw new NotSupportedException("Unsupported disposer type");
			}
		}

		public static IDisposable DisposeOn(this IDisposable disposable, IScheduler scheduler)
		{
			return new ScheduledDisposable(scheduler, disposable);
		}

		public static IDisposable DisposeOn(this IDisposable disposable, SynchronizationContext context)
		{
			return new ContextDisposable(context, disposable);
		}

		public static RefCountDisposable RefCounted(this IDisposable disposable)
		{
			return new RefCountDisposable(disposable);
		}
	}
}