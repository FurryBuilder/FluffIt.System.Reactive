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
using FluffIt.System.Reactive.StaticExtensions;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluffIt.System.Reactive.Tests.ObservableExtensionsTests
{
    [TestClass]
    public class GivenValueObservable
    {
        [TestMethod]
        public void WhenSelectUnit_ThenResultIsUnit()
        {
            var validator = false;

            var testScheduler = new TestScheduler();

            var source = Observable
                .Return(true, testScheduler)
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
        public void WhenSelectManyDisposePrevious()
        {
            var outterRanCount = 0;
            var innerRanCount = 0;

            var testSchedulerOutter = new TestScheduler();
            var testSchedulerInner = new TestScheduler();

            Observable
                .Range(0, 2, testSchedulerOutter)
                .SelectManyDisposePrevious(
                    _ =>
                    {
                        ++outterRanCount;

                        return ObservableEx.DeferedStart(() => ++innerRanCount, testSchedulerInner);
                    })
                .Subscribe();

            testSchedulerOutter.AdvanceBy(2);
            testSchedulerInner.AdvanceBy(4);

            Assert.AreEqual(2, outterRanCount);
            Assert.AreEqual(1, innerRanCount);
        }

        [TestMethod]
        public void WhenSelectUnitWithNull_ThenResultIsUnit()
        {
            var validator = false;

            var testScheduler = new TestScheduler();

            var source = Observable
                .Return((object) null, testScheduler)
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
        public void WhenSubscribeSafe_ThenErrorIsHandled()
        {
            var isHandled = false;

            var testScheduler = new TestScheduler();

            Observable
                .Return(Unit.Default, testScheduler)
                .SubscribeSafe(
                    _ => { throw new Exception(); },
                    ex => isHandled = true
                );

            testScheduler.AdvanceBy(1);

            Assert.IsTrue(isHandled);
        }

        [TestMethod]
        public void WhenSubscribeSafeWithPreviousValue_ThenErrorIsHandled()
        {
            var isHandled = false;

            var testScheduler = new TestScheduler();

            Observable
                .Return(Unit.Default, testScheduler)
                .WithPreviousValue()
                .SubscribeSafe(
                    v => { throw new Exception(); },
                    ex => isHandled = true
                );

            testScheduler.AdvanceBy(1);

            Assert.IsTrue(isHandled);
        }

        [TestMethod]
        public void WhenSubscribeSafeWithPreviousValue_ThenValuesAreValid()
        {
            var testScheduler = new TestScheduler();

            var prev = 0;
            var curr = 0;

            Observable
                .Range(1, 2, testScheduler)
                .WithPreviousValue()
                .SubscribeSafe(
                    v =>
                    {
                        prev = v.PreviousValue;
                        curr = v.CurrentValue;
                    });

            testScheduler.AdvanceBy(2);

            Assert.AreEqual(1, prev);
            Assert.AreEqual(2, curr);
        }

        [TestMethod]
        public void WhenSelectWithPreviousValue_ThenValuesAreValid()
        {
            var testScheduler = new TestScheduler();

            var prev = 0;
            var curr = 0;

            Observable
                .Range(1, 2, testScheduler)
                .WithPreviousValue()
                .Select(
                    v =>
                    {
                        prev = v.PreviousValue;
                        curr = v.CurrentValue;

                        return Unit.Default;
                    })
                .Subscribe();

            testScheduler.AdvanceBy(2);

            Assert.AreEqual(1, prev);
            Assert.AreEqual(2, curr);
        }

        [TestMethod]
        public void WhenDoWithPreviousValue_ThenValuesAreValid()
        {
            var testScheduler = new TestScheduler();

            var prev = 0;
            var curr = 0;

            Observable
                .Range(1, 2, testScheduler)
                .WithPreviousValue()
                .Do(
                    v =>
                    {
                        prev = v.PreviousValue;
                        curr = v.CurrentValue;
                    })
                .Subscribe();

            testScheduler.AdvanceBy(2);

            Assert.AreEqual(1, prev);
            Assert.AreEqual(2, curr);
        }
    }
}