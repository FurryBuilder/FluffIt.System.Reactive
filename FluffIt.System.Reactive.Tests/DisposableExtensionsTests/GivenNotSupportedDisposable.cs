using System;
using System.Reactive.Disposables;

using Fluff.Extensions;
using FluffIt;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fluff.Tests.Extensions.DisposableExtensionsTests
{
	[TestClass]
	public class GivenNotSupportedDisposable
	{
		[TestMethod]
		[ExpectedException(typeof(NotSupportedException))]
		public void WhenDisposableEmpty_ThenThrow()
		{
			var disposable = new BooleanDisposable();
			var disposer = Disposable.Empty;

			disposable.DisposeWith(disposer);
		}

		[TestMethod]
		[ExpectedException(typeof(NotSupportedException))]
		public void WhenCancellationDisposable_ThenThrow()
		{
			var disposable = new BooleanDisposable();
			var disposer = new CancellationDisposable();

			disposable.DisposeWith(disposer);
		}

		[TestMethod]
		[ExpectedException(typeof(NotSupportedException))]
		public void WhenRefCountDisposable_ThenThrow()
		{
			var disposable = new BooleanDisposable();
			var disposer = new RefCountDisposable(disposable);
			
			disposable.DisposeWith(disposer);
		}

		[TestMethod]
		[ExpectedException(typeof(NotSupportedException))]
		public void WhenScheduledDisposable_ThenThrow()
		{
			var disposable = new BooleanDisposable();
			var disposer = new ScheduledDisposable(new TestScheduler(), disposable);

			disposable.DisposeWith(disposer);
		}

		[TestMethod]
		[ExpectedException(typeof(NotSupportedException))]
		public void WhenUnsupportedUntypedDisposable_ThenThrow()
		{
			var disposable = new BooleanDisposable();
			var disposer = Disposable.Empty;

			disposable.DisposeWith(disposer.As<IDisposable>());
		}
	}
}
