using Silo.Sync.Core.Tests.Fixtures;
using Xunit;

namespace Silo.Sync.Core.Tests;

[CollectionDefinition("SyncDatabaseCollection", DisableParallelization = true)]
public class SyncDatabaseCollection : ICollectionFixture<SyncTestDatabaseFixture>
{
}
