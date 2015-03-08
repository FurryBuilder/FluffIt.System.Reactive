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
using System.Threading;

namespace FluffIt.System.Reactive.Tests
{
    /// <summary>
    ///     Manually handle posts on synchronization contexts to support
    ///     synchroneous calls.
    /// </summary>
    public sealed class ManualSynchronizationContext : SynchronizationContext
    {
        private readonly List<Tuple<SendOrPostCallback, object>> _operationQueue
            = new List<Tuple<SendOrPostCallback, object>>();

        public override void Send(SendOrPostCallback callback, object state)
        {
            callback.Invoke(state);
        }

        public override void Post(SendOrPostCallback callback, object state)
        {
            _operationQueue.Add(Tuple.Create(callback, state));
        }

        public void Execute()
        {
            _operationQueue.ForEach(x => x.Item1(x.Item2));
        }
    }
}