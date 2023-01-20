﻿using System.Data.Common;
using IntegrationTests;
using Lamar;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using PersistenceTests.Marten;
using Shouldly;
using TestingSupport;
using Wolverine;
using Wolverine.Persistence.Durability;
using Wolverine.Postgresql;
using Xunit;

namespace PersistenceTests.Postgresql;

public class extension_registrations : PostgresqlContext
{
    [Fact]
    public async Task registrations()
    {
        var runtime = await WolverineHost.ForAsync(x =>
            x.PersistMessagesWithPostgresql(Servers.PostgresConnectionString));

        var container = runtime.Get<IContainer>();
        container.Model.HasRegistrationFor<NpgsqlConnection>().ShouldBeTrue();
        container.Model.HasRegistrationFor<DbConnection>().ShouldBeTrue();

        container.Model.For<NpgsqlConnection>().Default.Lifetime.ShouldBe(ServiceLifetime.Scoped);


        container.Model.HasRegistrationFor<IMessageStore>().ShouldBeTrue();


        runtime.Get<NpgsqlConnection>().ConnectionString.ShouldBe(Servers.PostgresConnectionString);
        runtime.Get<DbConnection>().ShouldBeOfType<NpgsqlConnection>()
            .ConnectionString.ShouldBe(Servers.PostgresConnectionString);
        await runtime.StopAsync();
    }
}