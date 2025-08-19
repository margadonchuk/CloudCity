using CloudCityCenter.Models;
using System.Linq;

namespace CloudCityCenter.Tests;

public class ServerGroupingTests
{
    [Fact]
    public void Chunk_GroupsItemsIntoSetsOfThree()
    {
        var servers = Enumerable.Range(1, 7).Select(i => new Product { Id = i, Name = $"S{i}", Type = ProductType.DedicatedServer }).ToList();

        var chunks = servers.Chunk(3).ToList();

        Assert.Equal(3, chunks.Count);
        Assert.Equal(3, chunks[0].Length);
        Assert.Equal(3, chunks[1].Length);
        Assert.Single(chunks[2]);
    }
}
