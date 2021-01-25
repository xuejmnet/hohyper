using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoHyper.ShardingCore.Internal.StreamMerge
{
/*
* @Author: xjm
* @Description:
* @Date: Monday, 25 January 2021 07:57:59
* @Email: 326308290@qq.com
*/
    internal class StreamMergeListEngine<T>
    {
        private const int defaultCapacity = 0x10;//默认容量为16
        private readonly StreamMergeContext _mergeContext;
        private readonly List<IAsyncEnumerator<T>> _sources;

        public StreamMergeListEngine(StreamMergeContext mergeContext,List<IAsyncEnumerator<T>> sources)
        {
            _mergeContext = mergeContext;
            _sources = sources;
        }

        public async Task<List<T>> Execute()
        {
            //如果合并数据的时候不需要跳过也没有take多少那么就是直接next
            var skip = _mergeContext.Skip;
            var take = _mergeContext.Take;
            var list = new List<T>(skip.GetValueOrDefault() + take ?? defaultCapacity);
            var enumerator=new MultiAsyncEnumerator<T>(_mergeContext,_sources);
            await enumerator.LinkAsync();
            var realSkip = 0;
            var realTake = 0;
            while (await enumerator.MoveNextAsync())
            {
                //获取真实的需要跳过的条数
                if (skip.HasValue)
                {
                    if (realSkip < skip)
                    {
                        realSkip++;
                        continue;
                    }
                }
                list.Add(enumerator.Current);
                if (take.HasValue)
                {
                    realTake++;
                    if(realTake<=take.Value)
                        break;
                }
            }

            return list;
        }
        
    }
}