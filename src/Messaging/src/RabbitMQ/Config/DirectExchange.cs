// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information.

namespace Steeltoe.Messaging.RabbitMQ.Config;

public class DirectExchange : AbstractExchange, IDirectExchange
{
    public static readonly DirectExchange Default = new(string.Empty);

    public override string Type { get; } = ExchangeType.Direct;

    public DirectExchange(string name)
        : base(name)
    {
    }

    public DirectExchange(string name, bool durable, bool autoDelete)
        : base(name, durable, autoDelete)
    {
    }

    public DirectExchange(string name, bool durable, bool autoDelete, Dictionary<string, object> arguments)
        : base(name, durable, autoDelete, arguments)
    {
    }
}
