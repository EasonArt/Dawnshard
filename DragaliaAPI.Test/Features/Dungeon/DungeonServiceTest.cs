﻿using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Test.Features.Dungeon;

public class DungeonServiceTest
{
    private readonly Mock<IOptionsMonitor<RedisOptions>> mockOptions;
    private readonly IDungeonService dungeonService;

    public DungeonServiceTest()
    {
        mockOptions = new(MockBehavior.Strict);

        IOptions<MemoryDistributedCacheOptions> opts = Options.Create(
            new MemoryDistributedCacheOptions()
        );
        IDistributedCache testCache = new MemoryDistributedCache(opts);

        mockOptions
            .SetupGet(x => x.CurrentValue)
            .Returns(new RedisOptions() { DungeonExpiryTimeMinutes = 1 });

        dungeonService = new DungeonService(testCache, mockOptions.Object);
    }

    [Fact]
    public async Task StartDungeon_CanGetAfterwards()
    {
        DungeonSession session =
            new()
            {
                QuestData = MasterAsset.QuestData.Get(100010303),
                Party = new List<PartySettingList>()
                {
                    new() { chara_id = Shared.Definitions.Enums.Charas.Addis }
                }
            };

        string key = await dungeonService.StartDungeon(session);

        (await dungeonService.GetDungeon(key)).Should().BeEquivalentTo(session);
    }
}