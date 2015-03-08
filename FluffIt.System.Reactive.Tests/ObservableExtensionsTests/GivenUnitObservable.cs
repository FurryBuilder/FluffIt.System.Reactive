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

            source.Subscribe(
                u =>
                {
                    validator = true;
                    Assert.IsInstanceOfType(u, typeof (Unit));
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

            source.Subscribe(
                u =>
                {
                    validator = true;
                    Assert.AreEqual(Unit.Default, u);
                });

            testScheduler.AdvanceBy(1);

            Assert.IsTrue(validator);
        }
    }
}