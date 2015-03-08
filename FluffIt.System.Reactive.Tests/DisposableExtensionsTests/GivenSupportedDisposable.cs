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
using System.Reactive.Disposables;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluffIt.System.Reactive.Tests.DisposableExtensionsTests
{
    [TestClass]
    public class GivenSupportedDisposable
    {
        [TestMethod]
        public void WhenSingleAssignment_ThenDispose()
        {
            var disposable = new BooleanDisposable();
            var disposer = new SingleAssignmentDisposable();

            disposable.DisposeWith(disposer.As<IDisposable>());
            Assert.IsFalse(disposable.IsDisposed);

            disposer.Dispose();
            Assert.IsTrue(disposable.IsDisposed);
        }

        [TestMethod]
        public void WhenMultipleAssignment_ThenDispose()
        {
            var disposable = new BooleanDisposable();
            var disposer = new MultipleAssignmentDisposable();

            disposable.DisposeWith(disposer.As<IDisposable>());
            Assert.IsFalse(disposable.IsDisposed);

            disposer.Dispose();
            Assert.IsTrue(disposable.IsDisposed);
        }

        [TestMethod]
        public void WhenSerial_ThenDispose()
        {
            var disposable = new BooleanDisposable();
            var disposer = new SerialDisposable();

            disposable.DisposeWith(disposer.As<IDisposable>());
            Assert.IsFalse(disposable.IsDisposed);

            disposer.Disposable = Disposable.Empty;
            Assert.IsTrue(disposable.IsDisposed);
        }

        [TestMethod]
        public void WhenComposite_ThenDispose()
        {
            var disposable = new[] { new BooleanDisposable(), new BooleanDisposable() };
            var disposer = new CompositeDisposable();

            disposable[0].DisposeWith(disposer.As<IDisposable>());
            disposable[1].DisposeWith(disposer.As<IDisposable>());

            Assert.IsFalse(disposable[0].IsDisposed);
            Assert.IsFalse(disposable[1].IsDisposed);

            disposer.Dispose();

            Assert.IsTrue(disposable[0].IsDisposed);
            Assert.IsTrue(disposable[1].IsDisposed);
        }

        [TestMethod]
        public void WhenDisposeOnScheduler_ThenDispose()
        {
            var scheduler = new TestScheduler();
            var disposable = new BooleanDisposable();

            var disposer = disposable.DisposeOn(scheduler);

            disposer.Dispose();
            Assert.IsFalse(disposable.IsDisposed);

            scheduler.AdvanceBy(1);
            Assert.IsTrue(disposable.IsDisposed);
        }

        [TestMethod]
        public void WhenDisposeOnContext_ThenDispose()
        {
            var context = new ManualSynchronizationContext();
            var disposable = new BooleanDisposable();

            var disposer = disposable.DisposeOn(context);

            disposer.Dispose();
            Assert.IsFalse(disposable.IsDisposed);

            context.Execute();
            Assert.IsTrue(disposable.IsDisposed);
        }

        [TestMethod]
        public void WhenRefCounted_ThenDispose()
        {
            var disposable = new BooleanDisposable();

            var disposer = disposable.RefCounted();

            var dependent1 = disposer.GetDisposable();
            Assert.IsFalse(disposable.IsDisposed);

            var dependent2 = disposer.GetDisposable();
            Assert.IsFalse(disposable.IsDisposed);

            dependent1.Dispose();
            disposer.Dispose();
            Assert.IsFalse(disposable.IsDisposed);

            dependent2.Dispose();
            disposer.Dispose();
            Assert.IsTrue(disposable.IsDisposed);
        }
    }
}