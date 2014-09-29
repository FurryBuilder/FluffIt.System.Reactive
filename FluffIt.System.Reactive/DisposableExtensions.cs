using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Threading;

namespace Fluff.Extensions
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
				.As<SingleAssignmentDisposable>(d => { disposable.DisposeWith(d); supported = true; })
				.As<MultipleAssignmentDisposable>(d => { disposable.DisposeWith(d); supported = true; })
				.As<SerialDisposable>(d => { disposable.DisposeWith(d); supported = true; })
				.As<CompositeDisposable>(d => { disposable.DisposeWith(d); supported = true; });

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