// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using Steeltoe.Common;

namespace Steeltoe.CircuitBreaker.Hystrix.Metric.Consumer;

public class RollingCommandEventCounterStream : BucketedRollingCounterStream<HystrixCommandCompletion, long[], long[]>
{
    private static readonly ConcurrentDictionary<string, RollingCommandEventCounterStream> Streams = new();

    private static readonly int NumEventTypes = HystrixEventTypeHelper.Values.Count;

    public override long[] EmptyBucketSummary => new long[NumEventTypes];

    public override long[] EmptyOutputValue => new long[NumEventTypes];

    private RollingCommandEventCounterStream(IHystrixCommandKey commandKey, int numCounterBuckets, int counterBucketSizeInMs,
        Func<long[], HystrixCommandCompletion, long[]> reduceCommandCompletion, Func<long[], long[], long[]> reduceBucket)
        : base(HystrixCommandCompletionStream.GetInstance(commandKey), numCounterBuckets, counterBucketSizeInMs, reduceCommandCompletion, reduceBucket)
    {
    }

    public static RollingCommandEventCounterStream GetInstance(IHystrixCommandKey commandKey, IHystrixCommandOptions properties)
    {
        int counterMetricWindow = properties.MetricsRollingStatisticalWindowInMilliseconds;
        int numCounterBuckets = properties.MetricsRollingStatisticalWindowBuckets;
        int counterBucketSizeInMs = counterMetricWindow / numCounterBuckets;

        return GetInstance(commandKey, numCounterBuckets, counterBucketSizeInMs);
    }

    public static RollingCommandEventCounterStream GetInstance(IHystrixCommandKey commandKey, int numBuckets, int bucketSizeInMs)
    {
        RollingCommandEventCounterStream result = Streams.GetOrAddEx(commandKey.Name, _ =>
        {
            var stream = new RollingCommandEventCounterStream(commandKey, numBuckets, bucketSizeInMs, HystrixCommandMetrics.AppendEventToBucket,
                HystrixCommandMetrics.BucketAggregator);

            stream.StartCachingStreamValuesIfUnstarted();
            return stream;
        });

        return result;
    }

    public static void Reset()
    {
        foreach (RollingCommandEventCounterStream stream in Streams.Values)
        {
            stream.Unsubscribe();
        }

        HystrixCommandCompletionStream.Reset();
        Streams.Clear();
    }

    public long GetLatest(HystrixEventType eventType)
    {
        return Latest[(int)eventType];
    }
}
