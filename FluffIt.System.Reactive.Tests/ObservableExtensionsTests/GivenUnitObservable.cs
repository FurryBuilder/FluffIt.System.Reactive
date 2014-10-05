using System;
using System.Reactive;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluffIt.System.Reactive.Tests.ObservableExtensionsTests
{
	[TestClass]
	public class GivenUnitObservable
	{
		[TestMethod]
		public void WhenSelectUnit_ThenResultIsUnit()
		{
			var validator = false;

			var testScheduler = new TestScheduler();

			var source = Observable
				.Return(Unit.Default, testScheduler)
				.SelectUnit();

			testScheduler.AdvanceBy(1);

			source.Subscribe(u =>
			{
				validator = true;
				Assert.IsInstanceOfType(u, typeof(Unit));
			});

			testScheduler.AdvanceBy(1);

			Assert.IsTrue(validator);
		}

		[TestMethod]
		public void WhenDefault_ThenResultIsSet()
		{
			var validator = false;

			var testScheduler = new TestScheduler();

			var source = Observable
				.Return(null as Unit?, testScheduler)
				.Default(() => Unit.Default);

			testScheduler.AdvanceBy(1);

			source.Subscribe(u =>
			{
				validator = true;
				Assert.AreEqual(Unit.Default, u);
			});

			testScheduler.AdvanceBy(1);

			Assert.IsTrue(validator);
		}
	}
}
