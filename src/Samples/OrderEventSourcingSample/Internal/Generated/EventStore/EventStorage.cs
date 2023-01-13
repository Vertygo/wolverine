// <auto-generated/>
#pragma warning disable
using Marten;
using Marten.Events;
using System;

namespace Marten.Generated.EventStore
{
    // START: GeneratedEventDocumentStorage
    public class GeneratedEventDocumentStorage : Marten.Events.EventDocumentStorage
    {
        private readonly Marten.StoreOptions _options;

        public GeneratedEventDocumentStorage(Marten.StoreOptions options) : base(options)
        {
            _options = options;
        }



        public override Marten.Internal.Operations.IStorageOperation AppendEvent(Marten.Events.EventGraph events, Marten.Internal.IMartenSession session, Marten.Events.StreamAction stream, Marten.Events.IEvent e)
        {
            return new Marten.Generated.EventStore.AppendEventOperation(stream, e);
        }


        public override Marten.Internal.Operations.IStorageOperation InsertStream(Marten.Events.StreamAction stream)
        {
            return new Marten.Generated.EventStore.GeneratedInsertStream(stream);
        }


        public override Marten.Linq.QueryHandlers.IQueryHandler<Marten.Events.StreamState> QueryForStream(Marten.Events.StreamAction stream)
        {
            return new Marten.Generated.EventStore.GeneratedStreamStateQueryHandler(stream.Id);
        }


        public override Marten.Internal.Operations.IStorageOperation UpdateStreamVersion(Marten.Events.StreamAction stream)
        {
            return new Marten.Generated.EventStore.GeneratedStreamVersionOperation(stream);
        }


        public override void ApplyReaderDataToEvent(System.Data.Common.DbDataReader reader, Marten.Events.IEvent e)
        {
            if (!reader.IsDBNull(3))
            {
            var sequence = reader.GetFieldValue<long>(3);
            e.Sequence = sequence;
            }
            if (!reader.IsDBNull(4))
            {
            var id = reader.GetFieldValue<System.Guid>(4);
            e.Id = id;
            }
            var streamId = reader.GetFieldValue<System.Guid>(5);
            e.StreamId = streamId;
            if (!reader.IsDBNull(6))
            {
            var version = reader.GetFieldValue<long>(6);
            e.Version = version;
            }
            if (!reader.IsDBNull(7))
            {
            var timestamp = reader.GetFieldValue<System.DateTimeOffset>(7);
            e.Timestamp = timestamp;
            }
            if (!reader.IsDBNull(8))
            {
            var tenantId = reader.GetFieldValue<string>(8);
            e.TenantId = tenantId;
            }
            var isArchived = reader.GetFieldValue<bool>(9);
            e.IsArchived = isArchived;
        }


        public override async System.Threading.Tasks.Task ApplyReaderDataToEventAsync(System.Data.Common.DbDataReader reader, Marten.Events.IEvent e, System.Threading.CancellationToken token)
        {
            if (!(await reader.IsDBNullAsync(3, token)))
            {
            var sequence = await reader.GetFieldValueAsync<long>(3, token);
            e.Sequence = sequence;
            }
            if (!(await reader.IsDBNullAsync(4, token)))
            {
            var id = await reader.GetFieldValueAsync<System.Guid>(4, token);
            e.Id = id;
            }
            var streamId = await reader.GetFieldValueAsync<System.Guid>(5, token);
            e.StreamId = streamId;
            if (!(await reader.IsDBNullAsync(6, token)))
            {
            var version = await reader.GetFieldValueAsync<long>(6, token);
            e.Version = version;
            }
            if (!(await reader.IsDBNullAsync(7, token)))
            {
            var timestamp = await reader.GetFieldValueAsync<System.DateTimeOffset>(7, token);
            e.Timestamp = timestamp;
            }
            if (!(await reader.IsDBNullAsync(8, token)))
            {
            var tenantId = await reader.GetFieldValueAsync<string>(8, token);
            e.TenantId = tenantId;
            }
            var isArchived = await reader.GetFieldValueAsync<bool>(9, token);
            e.IsArchived = isArchived;
        }

    }

    // END: GeneratedEventDocumentStorage
    
    
    // START: AppendEventOperation
    public class AppendEventOperation : Marten.Events.Operations.AppendEventOperationBase
    {
        private readonly Marten.Events.StreamAction _stream;
        private readonly Marten.Events.IEvent _e;

        public AppendEventOperation(Marten.Events.StreamAction stream, Marten.Events.IEvent e) : base(stream, e)
        {
            _stream = stream;
            _e = e;
        }


        public const string SQL = "insert into orders.mt_events (data, type, mt_dotnet_type, seq_id, id, stream_id, version, timestamp, tenant_id) values (?, ?, ?, ?, ?, ?, ?, ?, ?)";


        public override void ConfigureCommand(Weasel.Postgresql.CommandBuilder builder, Marten.Internal.IMartenSession session)
        {
            var parameters = builder.AppendWithParameters(SQL);
            parameters[0].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Jsonb;
            parameters[0].Value = session.Serializer.ToJson(Event.Data);
            parameters[1].Value = Event.EventTypeName != null ? (object)Event.EventTypeName : System.DBNull.Value;
            parameters[1].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text;
            parameters[2].Value = Event.DotNetTypeName != null ? (object)Event.DotNetTypeName : System.DBNull.Value;
            parameters[2].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text;
            parameters[3].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Bigint;
            parameters[3].Value = Event.Sequence;
            parameters[4].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Uuid;
            parameters[4].Value = Event.Id;
            parameters[5].Value = Stream.Id;
            parameters[5].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Uuid;
            parameters[6].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Bigint;
            parameters[6].Value = Event.Version;
            parameters[7].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.TimestampTz;
            parameters[7].Value = Event.Timestamp;
            parameters[8].Value = Stream.TenantId != null ? (object)Stream.TenantId : System.DBNull.Value;
            parameters[8].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text;
        }

    }

    // END: AppendEventOperation
    
    
    // START: GeneratedInsertStream
    public class GeneratedInsertStream : Marten.Events.Operations.InsertStreamBase
    {
        private readonly Marten.Events.StreamAction _stream;

        public GeneratedInsertStream(Marten.Events.StreamAction stream) : base(stream)
        {
            _stream = stream;
        }


        public const string SQL = "insert into orders.mt_streams (id, type, version, tenant_id) values (?, ?, ?, ?)";


        public override void ConfigureCommand(Weasel.Postgresql.CommandBuilder builder, Marten.Internal.IMartenSession session)
        {
            var parameters = builder.AppendWithParameters(SQL);
            parameters[0].Value = Stream.Id;
            parameters[0].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Uuid;
            parameters[1].Value = Stream.AggregateTypeName != null ? (object)Stream.AggregateTypeName : System.DBNull.Value;
            parameters[1].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text;
            parameters[2].Value = Stream.Version;
            parameters[2].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Bigint;
            parameters[3].Value = Stream.TenantId != null ? (object)Stream.TenantId : System.DBNull.Value;
            parameters[3].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text;
        }

    }

    // END: GeneratedInsertStream
    
    
    // START: GeneratedStreamStateQueryHandler
    public class GeneratedStreamStateQueryHandler : Marten.Events.Querying.StreamStateQueryHandler
    {
        private readonly System.Guid _streamId;

        public GeneratedStreamStateQueryHandler(System.Guid streamId)
        {
            _streamId = streamId;
        }


        public const string SQL = "select id, version, type, timestamp, created as timestamp, is_archived from orders.mt_streams where id = ?";


        public override void ConfigureCommand(Weasel.Postgresql.CommandBuilder builder, Marten.Internal.IMartenSession session)
        {
            var npgsqlParameterArray = builder.AppendWithParameters(SQL);
            npgsqlParameterArray[0].Value = _streamId;
            npgsqlParameterArray[0].DbType = System.Data.DbType.Guid;
        }


        public override Marten.Events.StreamState Resolve(Marten.Internal.IMartenSession session, System.Data.Common.DbDataReader reader)
        {
            var streamState = new Marten.Events.StreamState();
            var id = reader.GetFieldValue<System.Guid>(0);
            streamState.Id = id;
            var version = reader.GetFieldValue<long>(1);
            streamState.Version = version;
            SetAggregateType(streamState, reader, session);
            var lastTimestamp = reader.GetFieldValue<System.DateTimeOffset>(3);
            streamState.LastTimestamp = lastTimestamp;
            var created = reader.GetFieldValue<System.DateTimeOffset>(4);
            streamState.Created = created;
            var isArchived = reader.GetFieldValue<bool>(5);
            streamState.IsArchived = isArchived;
            return streamState;
        }


        public override async System.Threading.Tasks.Task<Marten.Events.StreamState> ResolveAsync(Marten.Internal.IMartenSession session, System.Data.Common.DbDataReader reader, System.Threading.CancellationToken token)
        {
            var streamState = new Marten.Events.StreamState();
            var id = await reader.GetFieldValueAsync<System.Guid>(0, token);
            streamState.Id = id;
            var version = await reader.GetFieldValueAsync<long>(1, token);
            streamState.Version = version;
            await SetAggregateTypeAsync(streamState, reader, session, token);
            var lastTimestamp = await reader.GetFieldValueAsync<System.DateTimeOffset>(3, token);
            streamState.LastTimestamp = lastTimestamp;
            var created = await reader.GetFieldValueAsync<System.DateTimeOffset>(4, token);
            streamState.Created = created;
            var isArchived = await reader.GetFieldValueAsync<bool>(5, token);
            streamState.IsArchived = isArchived;
            return streamState;
        }

    }

    // END: GeneratedStreamStateQueryHandler
    
    
    // START: GeneratedStreamVersionOperation
    public class GeneratedStreamVersionOperation : Marten.Events.Operations.UpdateStreamVersion
    {
        private readonly Marten.Events.StreamAction _stream;

        public GeneratedStreamVersionOperation(Marten.Events.StreamAction stream) : base(stream)
        {
            _stream = stream;
        }


        public const string SQL = "update orders.mt_streams set version = ? where id = ? and version = ?";


        public override void ConfigureCommand(Weasel.Postgresql.CommandBuilder builder, Marten.Internal.IMartenSession session)
        {
            var parameters = builder.AppendWithParameters(SQL);
            parameters[0].Value = Stream.Version;
            parameters[0].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Bigint;
            parameters[1].Value = Stream.Id;
            parameters[1].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Uuid;
            parameters[2].Value = Stream.ExpectedVersionOnServer;
            parameters[2].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Bigint;
        }

    }

    // END: GeneratedStreamVersionOperation
    
    
}

