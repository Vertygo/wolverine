﻿using System;
using MemoryPack;
using Wolverine.Runtime.Serialization.MemoryPack.Internal;

namespace Wolverine.Runtime.Serialization.MemoryPack;

public static class WolverineMemoryPackSerializationExtensions
{
    public static void UseMemoryPackSerialization(this WolverineOptions options, Action<MemoryPackSerializerOptions>? configuration = null)
    {
        var serializerOptions = MemoryPackSerializerOptions.Default;

        configuration?.Invoke(serializerOptions);

        var serializer = new MemoryPackMessageSerializer(serializerOptions);

        options.DefaultSerializer = serializer;
    }
}