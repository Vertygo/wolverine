using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace TestingSupport.Sagas;

public interface ISagaHost
{
    Task<IHost> BuildHostAsync<TSaga>();

    Task<T> LoadState<T>(Guid id) where T : class;
    Task<T> LoadState<T>(int id) where T : class;
    Task<T> LoadState<T>(long id) where T : class;
    Task<T> LoadState<T>(string id) where T : class;
}