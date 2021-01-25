using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HoHyper.Extensions;

namespace HoHyper.ShardingCore.Internal.StreamMerge
{
/*
* @Author: xjm
* @Description:
* @Date: Monday, 25 January 2021 11:33:57
* @Email: 326308290@qq.com
*/
    internal class OrderMergeItem<T> : IComparable<OrderMergeItem<T>>, IAsyncDisposable
    {
        /// <summary>
        /// 合并数据上下文
        /// </summary>
        private readonly StreamMergeContext _mergeContext;

        private readonly IAsyncEnumerator<T> _source;
        private List<IComparable> _orderValues;

        public OrderMergeItem(StreamMergeContext mergeContext, IAsyncEnumerator<T> source)
        {
            _mergeContext = mergeContext;
            _source = source;
        }

        public IAsyncEnumerator<T> GetCurrentEnumerator() => _source;

        public async Task<bool> MoveNextAsync()
        {
            var has = await _source.MoveNextAsync();
            _orderValues = has ? GetCurrentOrderValues() : new List<IComparable>(0);
            return has;
        }

        private List<IComparable> GetCurrentOrderValues()
        {
            if (!_mergeContext.Orders.Any())
                return new List<IComparable>(0);
            var list = new List<IComparable>(_mergeContext.Orders.Count());
            foreach (var order in _mergeContext.Orders)
            {
                var value = GetCurrentEnumerator().Current.GetValueByExpression(order.PropertyExpression);
                if (value is IComparable comparable)
                    list.Add(comparable);
                else
                    throw new NotSupportedException($"order by value [{order}] must  implements IComparable");
            }

            return list;
        }

        public int CompareTo(OrderMergeItem<T> other)
        {
            int i = 0;
            foreach (var order in _mergeContext.Orders) {
                int result = CompareHelper.CompareToWith(_orderValues[i], other._orderValues[i], order.IsAsc);
                if (0 != result) {
                    return result;
                }
                i++;
            }
            return 0;
        }

        public ValueTask DisposeAsync()
        {
            return _source.DisposeAsync();
        }
    }
}