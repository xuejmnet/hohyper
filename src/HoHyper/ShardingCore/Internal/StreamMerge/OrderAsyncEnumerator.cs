using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HoHyper.ShardingCore.Internal.PriorityQueues;

namespace HoHyper.ShardingCore.Internal.StreamMerge
{
/*
* @Author: xjm
* @Description:
* @Date: Monday, 25 January 2021 08:06:12
* @Email: 326308290@qq.com
*/
#if !EFCORE2
    internal class OrderAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly StreamMergeContext _mergeContext;
        private readonly List<IAsyncEnumerator<T>> _sources;
        private readonly PriorityQueue<OrderMergeItem<T>> _queue;
        private bool isFirst;
        private IAsyncEnumerator<T> _currentEnumerator;

        public OrderAsyncEnumerator(StreamMergeContext mergeContext,List<IAsyncEnumerator<T>> sources)
        {
            _mergeContext = mergeContext;
            _sources = sources;
            _queue = new PriorityQueue<OrderMergeItem<T>>(sources.Count);
            isFirst = true;
            SetOrderEnumerator();
        }

        public void SetOrderEnumerator()
        {
            foreach (var source in _sources)
            {
                var orderMergeItem = new OrderMergeItem<T>(_mergeContext, source);
                if (null!=orderMergeItem.GetCurrentEnumerator().Current)
                    _queue.Offer(orderMergeItem);
            }
            _currentEnumerator = _queue.IsEmpty() ? _sources.FirstOrDefault() : _queue.Peek().GetCurrentEnumerator();
        }

        public async ValueTask<bool> MoveNextAsync()
        {
            if (_queue.IsEmpty())
                return false;
            if (isFirst)
            {
                isFirst = false;
                return true;
            }

            var first = _queue.Poll();
            if (await first.MoveNextAsync())
            {
                _queue.Offer(first);
            }

            if (_queue.IsEmpty())
            {
                return false;
            }

            _currentEnumerator = _queue.Peek().GetCurrentEnumerator();
            return true;
        }

        public async ValueTask DisposeAsync()
        {
            foreach (var source in _sources)
            {
                await source.DisposeAsync();
            }
        }

        public T Current => _currentEnumerator.Current;
    }
#endif
#if EFCORE2

    internal class OrderAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly StreamMergeContext _mergeContext;
        private readonly List<IAsyncEnumerator<T>> _sources;
        private readonly PriorityQueue<OrderMergeItem<T>> _queue;
        private bool isFirst;
        private IAsyncEnumerator<T> _currentEnumerator;

        public OrderAsyncEnumerator(StreamMergeContext mergeContext, List<IAsyncEnumerator<T>> sources)
        {
            _mergeContext = mergeContext;
            _sources = sources;
            _queue = new PriorityQueue<OrderMergeItem<T>>(sources.Count);
            isFirst = true;
            SetOrderEnumerator();
        }

        public void SetOrderEnumerator()
        {
            foreach (var source in _sources)
            {
                var orderMergeItem = new OrderMergeItem<T>(_mergeContext, source);
                if (null!=orderMergeItem.GetCurrentEnumerator().Current)
                    _queue.Offer(orderMergeItem);
            }

            _currentEnumerator = _queue.IsEmpty() ? _sources.FirstOrDefault() : _queue.Peek().GetCurrentEnumerator();
        }

        public async Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            if (_queue.IsEmpty())
                return false;
            if (isFirst)
            {
                isFirst = false;
                return true;
            }

            var first = _queue.Poll();
            if (await first.MoveNextAsync())
            {
                _queue.Offer(first);
            }

            if (_queue.IsEmpty())
            {
                return false;
            }

            _currentEnumerator = _queue.Peek().GetCurrentEnumerator();
            return true;
        }

        public void Dispose()
        {
            foreach (var source in _sources)
            {
                source.Dispose();
            }
        }


        public T Current => _currentEnumerator.Current;
    }
#endif
}