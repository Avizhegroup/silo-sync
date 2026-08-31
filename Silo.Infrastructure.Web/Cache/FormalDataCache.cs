using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using CacheItemPriority = Microsoft.Extensions.Caching.Memory.CacheItemPriority;

namespace Silo.Infrastructure.Web;
public partial class FormalDataCache(IMemoryCache memoryCache
    , IServiceProvider serviceProvider
    , RfidConnectApi api) : IFormalDataCache
{
    private readonly SemaphoreSlim _cacheLock = new(1, 1);
    // Static because FormalDataCache is registered as scoped (a new instance per request/circuit)
    // while the underlying IMemoryCache is a singleton; the locks must be shared across all
    // instances so concurrent requests for the same key still serialize into a single API call.
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> _keyLocks = new();
    private static readonly MemoryCacheEntryOptions _cacheEntryOptions = new MemoryCacheEntryOptions()
        .SetAbsoluteExpiration(TimeSpan.FromDays(1))
        .SetPriority(CacheItemPriority.Normal)
        .SetSize(1024);

     public async Task<List<GetAllWarehousesVm>> GetWarehouses() =>
        await GetOrCreateAsync(
            FormalCacheKeyManager.Cache_Key_Warehouse,
            async api => (await api.PostAsyncByContextAndOption<List<GetAllWarehousesVm>>(
                "SGetAllWarehouses",
                new GetAllWarehousesVmContext(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            )).Value
        );

    public async Task UpdateWarehouses(List<GetAllWarehousesVm> warehouses) => 
        memoryCache.Set(FormalCacheKeyManager.Cache_Key_Warehouse, warehouses, _cacheEntryOptions);

    public Task<List<GetAllProductQcsVm>> GetQcs() =>
        GetOrCreateAsync(
            FormalCacheKeyManager.Cache_Key_Qc,
            async api => (await api.PostAsync<List<GetAllProductQcsVm>>(
                "SGetAllProductStatus",
                new("userToken", ""),
                new("haveNotSelect", false)
            )).Value
        );

    public async Task UpdateQcs(List<GetAllProductQcsVm> qcs) =>
        memoryCache.Set(FormalCacheKeyManager.Cache_Key_Qc, qcs, _cacheEntryOptions);

    public Task<List<GetAllProductSizeTitleAndCodeVm>> GetSizes() =>
        GetOrCreateAsync(
            FormalCacheKeyManager.Cache_Key_Size,
            async api => (await api.PostAsync<List<GetAllProductSizeTitleAndCodeVm>>(
                "SGetAllProductPropertyC",
                new("userToken", ""),
                new("haveNotSelect", false)
            )).Value
        );

    public async Task UpdateSizes(List<GetAllProductSizeTitleAndCodeVm> sizes) =>
        memoryCache.Set(FormalCacheKeyManager.Cache_Key_Size, sizes, _cacheEntryOptions);

    public Task<List<GetAllProductBrandVm>> GetBrands() =>
        GetOrCreateAsync(
            FormalCacheKeyManager.Cache_Key_Brand,
            async api => (await api.PostAsync<List<GetAllProductBrandVm>>("SGetAllProductBrands")).Value
        );

    public async Task UpdateBrands(List<GetAllProductBrandVm> brands) =>
        memoryCache.Set(FormalCacheKeyManager.Cache_Key_Brand, brands, _cacheEntryOptions);

    public Task<List<GetAllLinesVm>> GetLines() =>
        GetOrCreateAsync(
            FormalCacheKeyManager.Cache_Key_Lines,
            async api => (await api.PostAsyncByContext<List<GetAllLinesVm>>(
                "SGetAllLines",
                new GetLineContext()
            )).Value
        );

    public async Task UpdateLines(List<GetAllLinesVm> lines) =>
        memoryCache.Set(FormalCacheKeyManager.Cache_Key_Lines, lines, _cacheEntryOptions);

    public Task<List<GetAllProductGroupVm>> GetGroups() =>
        GetOrCreateAsync(
            FormalCacheKeyManager.Cache_Key_Groups,
            async api => (await api.PostAsync<List<GetAllProductGroupVm>>("SGetAllProductGroups")).Value
        );

    public async Task UpdateGroups(List<GetAllProductGroupVm> groups) =>
        memoryCache.Set(FormalCacheKeyManager.Cache_Key_Groups, groups, _cacheEntryOptions);

    public Task<List<GetAllProductSubGroupVm>> GetSubGroups() =>
        GetOrCreateAsync(
            FormalCacheKeyManager.Cache_Key_SubGroups,
            async api => (await api.PostAsync<List<GetAllProductSubGroupVm>>("SGetAllProductSubGroups")).Value
        );

    public async Task UpdateSubGroups(List<GetAllProductSubGroupVm> subGroups) =>
        memoryCache.Set(FormalCacheKeyManager.Cache_Key_SubGroups, subGroups, _cacheEntryOptions);

    public Task<List<GetAllProductClassVm>> GetProductClass() =>
        GetOrCreateAsync(
            FormalCacheKeyManager.Cache_Key_ProductClass,
            async api => (await api.PostAsync<List<GetAllProductClassVm>>("SGetAllProductClasses")).Value
        );

    public async Task UpdateProductClass(List<GetAllProductClassVm> productClass) =>
        memoryCache.Set(FormalCacheKeyManager.Cache_Key_ProductClass, productClass, _cacheEntryOptions);

    public Task<List<GetAllProductTypeVm>> GetTypes() =>
        GetOrCreateAsync(
            FormalCacheKeyManager.Cache_Key_Types,
            async api => (await api.PostAsync<List<GetAllProductTypeVm>>(
                "SGetAllProductType",
                new("userToken", ""),
                new("haveNotSelect", false)
            )).Value
        );

    public async Task UpdateType(List<GetAllProductTypeVm> types) =>
        memoryCache.Set(FormalCacheKeyManager.Cache_Key_Types, types, _cacheEntryOptions);

    public async Task<List<GetAllShiftsVm>> GetShifts() =>
        await GetOrCreateAsync(
            FormalCacheKeyManager.Cache_Key_Shifts,
            async api =>
            (await api.PostAsyncByContext<List<GetAllShiftsVm>>("SGetAllShifts"
           , new GetShiftObjectContext())).Value
        );

    public async Task<List<GetAllTextResourcesVm>> GetTextResources()
    {
        var textResources = await GetOrCreateAsync(
            FormalCacheKeyManager.Cache_Key_TextResources,
            async api => (await api.SendAsyncObjectByUri<List<GetAllTextResourcesVm>>(
                HttpMethod.Post,
                "TextResource/ReadAll",
                new GetAllTextResourcesQuery(),
                new GetAllTextResourcesVmContext()
            )).Value
        );

        SyncResourceManager(textResources);

        return textResources;
    }

    public async Task<List<GetAllTextResourcesVm>> RefreshTextResources()
    {
        memoryCache.Remove(FormalCacheKeyManager.Cache_Key_TextResources);

        return await GetTextResources();
    }

    public async Task UpdateTextResources(List<GetAllTextResourcesVm> textResources)
    {
        memoryCache.Set(FormalCacheKeyManager.Cache_Key_TextResources, textResources, _cacheEntryOptions);

        SyncResourceManager(textResources);
    }

    /// <summary>
    /// Keeps the in-memory <see cref="ResourceManager"/> (used by the static
    /// <see cref="TextResources"/> accessors) in sync with the cached text resources,
    /// so every refresh path (startup, hard refresh, manual save) reflects immediately
    /// without requiring an application restart.
    /// </summary>
    private static void SyncResourceManager(List<GetAllTextResourcesVm> textResources)
    {
        if (textResources is null)
        {
            return;
        }

        ResourceManager.Load(textResources.ToDictionary(x => x.Key, x => x.Value));
    }

    public async Task HardRefreshCache()
    {
        await _cacheLock.WaitAsync();
      
        try
        {
            ClearAllCaches();

            await Task.WhenAll(
                GetWarehouses(),
                GetQcs(),
                GetSizes(),
                GetBrands(),
                GetLines(),
                GetGroups(),
                GetSubGroups(),
                GetProductClass(),
                GetTypes(),
                GetShifts(),
                GetTextResources()
            );

            memoryCache.Set(
                FormalCacheKeyManager.Cache_Key_Dates,
                DateTime.UtcNow,
                new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                    .SetPriority(CacheItemPriority.Normal)
                    .SetSize(1)
            );
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    private async Task<T> GetOrCreateAsync<T>(string cacheKey, Func<RfidConnectApi, Task<T>> fetchData)
    {
        if (memoryCache.TryGetValue(cacheKey, out T cachedValue))
        {
            return cachedValue;
        }

        // Guard against a cache stampede: only one caller per key should hit the API,
        // any other concurrent caller for the same key waits and reuses the cached result.
        var keyLock = _keyLocks.GetOrAdd(cacheKey, _ => new SemaphoreSlim(1, 1));
        await keyLock.WaitAsync();

        try
        {
            if (memoryCache.TryGetValue(cacheKey, out cachedValue))
            {
                return cachedValue;
            }

            var data = await fetchData(api);

            memoryCache.Set(cacheKey, data, _cacheEntryOptions);

            return data;
        }
        finally
        {
            keyLock.Release();
        }
    }

    /// <summary>
    /// Clears all caches in the memory cache. 
    /// </summary>
    private void ClearAllCaches()
    {
        memoryCache.Remove(FormalCacheKeyManager.Cache_Key_Warehouse);
        memoryCache.Remove(FormalCacheKeyManager.Cache_Key_Qc);
        memoryCache.Remove(FormalCacheKeyManager.Cache_Key_Size);
        memoryCache.Remove(FormalCacheKeyManager.Cache_Key_Brand);
        memoryCache.Remove(FormalCacheKeyManager.Cache_Key_Lines);
        memoryCache.Remove(FormalCacheKeyManager.Cache_Key_Groups);
        memoryCache.Remove(FormalCacheKeyManager.Cache_Key_SubGroups);
        memoryCache.Remove(FormalCacheKeyManager.Cache_Key_ProductClass);
        memoryCache.Remove(FormalCacheKeyManager.Cache_Key_Types);
        memoryCache.Remove(FormalCacheKeyManager.Cache_Key_Shifts);
        memoryCache.Remove(FormalCacheKeyManager.Cache_Key_TextResources);
        memoryCache.Remove(FormalCacheKeyManager.Cache_Key_Dates);
    }

}
