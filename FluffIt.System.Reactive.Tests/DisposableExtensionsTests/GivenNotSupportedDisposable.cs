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
    public class GivenNotSupportedDisposable
    {
        [TestMethod]
        [ExpectedException(typeof (NotSupportedException))]
        public void WhenDisposableEmpty_ThenThrow()
        {
            var disposable = new BooleanDisposable();
            var disposer = Disposable.Empty;

            disposable.DisposeWith(disposer);
        }

        [TestMethod]
        [ExpectedException(typeof (NotSupportedException))]
        public void WhenCancellationDisposable_ThenThrow()
        {
            var disposable = new BooleanDisposable();
            var disposer = new CancellationDisposable();

            disposable.DisposeWith(disposer);
        }

        [TestMethod]
        [ExpectedException(typeof (NotSupportedException))]
        public void WhenRefCountDisposable_ThenThrow()
        {
            var disposable = new BooleanDisposable();
            var disposer = new RefCountDisposable(disposable);

            disposable.DisposeWith(disposer);
        }

        [TestMethod]
        [ExpectedException(typeof (NotSupportedException))]
        public void WhenScheduledDisposable_ThenThrow()
        {
            var disposable = new BooleanDisposable();
            var disposer = new ScheduledDisposable(new TestScheduler(), disposable);

            disposable.DisposeWith(disposer);
        }

        [TestMethod]
        [ExpectedException(typeof (NotSupportedException))]
        public void WhenUnsupportedUntypedDisposable_ThenThrow()
        {
            var disposable = new BooleanDisposable();
            var disposer = Disposable.Empty;

            disposable.DisposeWith(disposer.As<IDisposable>());
        }
    }
}