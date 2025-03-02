﻿using System.Diagnostics.CodeAnalysis;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Extensions;
using DragaliaAPI.Infrastructure.Results;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;

namespace DragaliaAPI.Integration.Test.Features.Missions;

[SuppressMessage("Performance", "CA1861:Avoid constant arrays as arguments")]
public class MissionTest : TestFixture
{
    public MissionTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        this.MockTimeProvider.SetUtcNow(DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task UnlockDrillMissionGroup_ValidRequest_UnlocksGroup()
    {
        DragaliaResponse<MissionUnlockDrillMissionGroupResponse> resp =
            await this.Client.PostMsgpack<MissionUnlockDrillMissionGroupResponse>(
                "mission/unlock_drill_mission_group",
                new MissionUnlockDrillMissionGroupRequest(1),
                cancellationToken: TestContext.Current.CancellationToken
            );

        resp.DataHeaders.ResultCode.Should().Be(ResultCode.Success);
        resp.Data.DrillMissionList.Should()
            .HaveCount(55)
            .And.ContainEquivalentOf(
                new DrillMissionList(
                    100100,
                    0,
                    0,
                    DateTimeOffset.UnixEpoch,
                    DateTimeOffset.UnixEpoch
                )
            );
    }

    [Fact]
    public async Task UnlockMainStoryGroup_ValidRequest_UnlocksGroup()
    {
        DragaliaResponse<MissionUnlockMainStoryGroupResponse> resp =
            await this.Client.PostMsgpack<MissionUnlockMainStoryGroupResponse>(
                "mission/unlock_main_story_group",
                new MissionUnlockMainStoryGroupRequest(1),
                cancellationToken: TestContext.Current.CancellationToken
            );

        resp.DataHeaders.ResultCode.Should().Be(ResultCode.Success);
        resp.Data.MainStoryMissionList.Should().HaveCount(5);
        // Don't test for a specific quest as other tests mess with the quest progress
    }

    [Fact]
    public async Task DrillMission_ReadStory_CompletesMission()
    {
        await this.Client.PostMsgpack<MissionUnlockDrillMissionGroupResponse>(
            "mission/unlock_drill_mission_group",
            new MissionUnlockDrillMissionGroupRequest(1),
            cancellationToken: TestContext.Current.CancellationToken
        );

        DragaliaResponse<QuestReadStoryResponse> resp =
            await this.Client.PostMsgpack<QuestReadStoryResponse>(
                "/quest/read_story",
                new QuestReadStoryRequest() { QuestStoryId = 1000106 },
                cancellationToken: TestContext.Current.CancellationToken
            );

        resp.DataHeaders.ResultCode.Should().Be(ResultCode.Success);
        resp.Data.UpdateDataList.MissionNotice.DrillMissionNotice.IsUpdate.Should().BeTrue();
        resp.Data.UpdateDataList.MissionNotice.DrillMissionNotice.CompletedMissionCount.Should()
            .BeGreaterThan(1); // One has to be completed because of the above, multiple can be completed due to other factors
        resp.Data.UpdateDataList.MissionNotice.DrillMissionNotice.NewCompleteMissionIdList.Should()
            .Contain(100200);

        DragaliaResponse<MissionReceiveDrillRewardResponse> rewardResp =
            await this.Client.PostMsgpack<MissionReceiveDrillRewardResponse>(
                "/mission/receive_drill_reward",
                new MissionReceiveDrillRewardRequest(new[] { 100200 }, Enumerable.Empty<int>()),
                cancellationToken: TestContext.Current.CancellationToken
            );

        rewardResp.DataHeaders.ResultCode.Should().Be(ResultCode.Success);
        rewardResp.Data.EntityResult.ConvertedEntityList.Should().NotBeNull();
        rewardResp.Data.DrillMissionList.Should().HaveCount(55);
    }

    [Fact]
    public async Task DrillMission_TreasureTrade_CompletesMission()
    {
        await this.Client.PostMsgpack<MissionUnlockDrillMissionGroupResponse>(
            "mission/unlock_drill_mission_group",
            new MissionUnlockDrillMissionGroupRequest(3),
            cancellationToken: TestContext.Current.CancellationToken
        );

        DragaliaResponse<TreasureTradeTradeResponse> resp =
            await this.Client.PostMsgpack<TreasureTradeTradeResponse>(
                "/treasure_trade/trade",
                new TreasureTradeTradeRequest() { TreasureTradeId = 10020101, TradeCount = 1 },
                cancellationToken: TestContext.Current.CancellationToken
            );

        resp.DataHeaders.ResultCode.Should().Be(ResultCode.Success);
        resp.Data.UpdateDataList.MissionNotice.DrillMissionNotice.IsUpdate.Should().BeTrue();
        resp.Data.UpdateDataList.MissionNotice.DrillMissionNotice.CompletedMissionCount.Should()
            .BeGreaterThan(1); // One has to be completed because of the above, multiple can be completed due to other factors
        resp.Data.UpdateDataList.MissionNotice.DrillMissionNotice.NewCompleteMissionIdList.Should()
            .Contain(300100);
    }

    [Fact]
    public async Task DrillMission_WyrmprintBuildup_CompletesMission()
    {
        await this.AddToDatabase(
            new DbAbilityCrest()
            {
                ViewerId = ViewerId,
                AbilityCrestId = AbilityCrestId.Aromatherapy,
                LimitBreakCount = 4,
            }
        );

        await this.Client.PostMsgpack<MissionUnlockDrillMissionGroupResponse>(
            "mission/unlock_drill_mission_group",
            new MissionUnlockDrillMissionGroupRequest(3),
            cancellationToken: TestContext.Current.CancellationToken
        );

        DragaliaResponse<AbilityCrestBuildupPieceResponse> resp =
            await this.Client.PostMsgpack<AbilityCrestBuildupPieceResponse>(
                "/ability_crest/buildup_piece",
                new AbilityCrestBuildupPieceRequest()
                {
                    AbilityCrestId = AbilityCrestId.Aromatherapy,
                    BuildupAbilityCrestPieceList = Enumerable
                        .Range(2, 15)
                        .Select(x => new AtgenBuildupAbilityCrestPieceList()
                        {
                            BuildupPieceType = BuildupPieceTypes.Stats,
                            Step = x,
                        }),
                },
                cancellationToken: TestContext.Current.CancellationToken
            );

        resp.DataHeaders.ResultCode.Should().Be(ResultCode.Success);
        resp.Data.UpdateDataList.MissionNotice.DrillMissionNotice.IsUpdate.Should().BeTrue();
        resp.Data.UpdateDataList.MissionNotice.DrillMissionNotice.CompletedMissionCount.Should()
            .BeGreaterThan(1);
        resp.Data.UpdateDataList.MissionNotice.DrillMissionNotice.NewCompleteMissionIdList.Should()
            .Contain(301700);
    }

    [Fact]
    public async Task DrillMission_DragonExactLeveling_CompletesMission()
    {
        await this.AddToDatabase(
            new DbPlayerDragonData() { ViewerId = ViewerId, DragonId = DragonId.Midgardsormr }
        );

        await this.Client.PostMsgpack<MissionUnlockDrillMissionGroupResponse>(
            "mission/unlock_drill_mission_group",
            new MissionUnlockDrillMissionGroupRequest(1),
            cancellationToken: TestContext.Current.CancellationToken
        );

        DragaliaResponse<DragonBuildupResponse> resp =
            await this.Client.PostMsgpack<DragonBuildupResponse>(
                "dragon/buildup",
                new DragonBuildupRequest()
                {
                    BaseDragonKeyId = (ulong)this.GetDragonKeyId(DragonId.Midgardsormr),
                    GrowMaterialList = new List<GrowMaterialList>()
                    {
                        new GrowMaterialList()
                        {
                            Type = EntityTypes.Material,
                            Id = (int)Materials.Dragonfruit,
                            Quantity = 10,
                        },
                    },
                },
                cancellationToken: TestContext.Current.CancellationToken
            );

        resp.DataHeaders.ResultCode.Should().Be(ResultCode.Success);
        resp.Data.UpdateDataList.MissionNotice.Should().NotBeNull();
        resp.Data.UpdateDataList.MissionNotice.DrillMissionNotice.IsUpdate.Should().BeTrue();
        resp.Data.UpdateDataList.MissionNotice.DrillMissionNotice.CompletedMissionCount.Should()
            .BeGreaterThan(1);
        resp.Data.UpdateDataList.MissionNotice.DrillMissionNotice.NewCompleteMissionIdList.Should()
            .Contain(102000);
    }

    [Fact]
    public async Task DrillMission_DragonOverleveling_CompletesMission()
    {
        await this.AddToDatabase(
            new DbPlayerDragonData() { ViewerId = ViewerId, DragonId = DragonId.Midgardsormr }
        );

        await this.Client.PostMsgpack<MissionUnlockDrillMissionGroupResponse>(
            "mission/unlock_drill_mission_group",
            new MissionUnlockDrillMissionGroupRequest(1),
            cancellationToken: TestContext.Current.CancellationToken
        );

        DragaliaResponse<DragonBuildupResponse> resp =
            await this.Client.PostMsgpack<DragonBuildupResponse>(
                "dragon/buildup",
                new DragonBuildupRequest()
                {
                    BaseDragonKeyId = (ulong)this.GetDragonKeyId(DragonId.Midgardsormr),
                    GrowMaterialList = new List<GrowMaterialList>()
                    {
                        new GrowMaterialList()
                        {
                            Type = EntityTypes.Material,
                            Id = (int)Materials.SucculentDragonfruit,
                            Quantity = 1,
                        },
                    },
                },
                cancellationToken: TestContext.Current.CancellationToken
            );

        resp.DataHeaders.ResultCode.Should().Be(ResultCode.Success);
        resp.Data.UpdateDataList.MissionNotice.Should().NotBeNull();
        resp.Data.UpdateDataList.MissionNotice.DrillMissionNotice.IsUpdate.Should().BeTrue();
        resp.Data.UpdateDataList.MissionNotice.DrillMissionNotice.CompletedMissionCount.Should()
            .BeGreaterThan(1);
        resp.Data.UpdateDataList.MissionNotice.DrillMissionNotice.NewCompleteMissionIdList.Should()
            .Contain(102000);
    }

    [Fact]
    public async Task DrillMission_CharacterExactLeveling_CompletesMission()
    {
        this.AddCharacter(Charas.Karina);

        await this.Client.PostMsgpack<MissionUnlockDrillMissionGroupResponse>(
            "mission/unlock_drill_mission_group",
            new MissionUnlockDrillMissionGroupRequest(1),
            cancellationToken: TestContext.Current.CancellationToken
        );

        DragaliaResponse<CharaBuildupResponse> resp =
            await this.Client.PostMsgpack<CharaBuildupResponse>(
                "chara/buildup",
                new CharaBuildupRequest(
                    Charas.Karina,
                    new List<AtgenEnemyPiece>()
                    {
                        new AtgenEnemyPiece() { Id = Materials.GoldCrystal, Quantity = 10 },
                    }
                ),
                cancellationToken: TestContext.Current.CancellationToken
            );

        resp.DataHeaders.ResultCode.Should().Be(ResultCode.Success);
        resp.Data.UpdateDataList.MissionNotice.Should().NotBeNull();
        resp.Data.UpdateDataList.MissionNotice.DrillMissionNotice.IsUpdate.Should().BeTrue();
        resp.Data.UpdateDataList.MissionNotice.DrillMissionNotice.CompletedMissionCount.Should()
            .BeGreaterThan(1);
        resp.Data.UpdateDataList.MissionNotice.DrillMissionNotice.NewCompleteMissionIdList.Should()
            .Contain(102500);
    }

    [Fact]
    public async Task DrillMission_CharacterOverleveling_CompletesMission()
    {
        this.AddCharacter(Charas.Karina);

        await this.Client.PostMsgpack<MissionUnlockDrillMissionGroupResponse>(
            "mission/unlock_drill_mission_group",
            new MissionUnlockDrillMissionGroupRequest(1),
            cancellationToken: TestContext.Current.CancellationToken
        );

        DragaliaResponse<CharaBuildupResponse> resp =
            await this.Client.PostMsgpack<CharaBuildupResponse>(
                "chara/buildup",
                new CharaBuildupRequest(
                    Charas.Karina,
                    new List<AtgenEnemyPiece>()
                    {
                        new AtgenEnemyPiece() { Id = Materials.GoldCrystal, Quantity = 15 },
                    }
                ),
                cancellationToken: TestContext.Current.CancellationToken
            );

        resp.DataHeaders.ResultCode.Should().Be(ResultCode.Success);
        resp.Data.UpdateDataList.MissionNotice.Should().NotBeNull();
        resp.Data.UpdateDataList.MissionNotice.DrillMissionNotice.IsUpdate.Should().BeTrue();
        resp.Data.UpdateDataList.MissionNotice.DrillMissionNotice.CompletedMissionCount.Should()
            .BeGreaterThan(1);
        resp.Data.UpdateDataList.MissionNotice.DrillMissionNotice.NewCompleteMissionIdList.Should()
            .Contain(102500);
    }

    [Fact]
    public async Task ReceiveReward_Wyrmprint_DoesNotGive0Copies()
    {
        await this.AddToDatabase(
            new DbPlayerMission()
            {
                Id = 10220101,
                Type = MissionType.MemoryEvent,
                ViewerId = ViewerId,
                Progress = 1,
                State = MissionState.Completed,
            }
        );

        MissionReceiveMemoryEventRewardResponse response = (
            await this.Client.PostMsgpack<MissionReceiveMemoryEventRewardResponse>(
                "mission/receive_memory_event_reward",
                new MissionReceiveMemoryEventRewardRequest()
                {
                    MemoryEventMissionIdList = new[] { 10220101 }, // Participate in the Event (Toll of the Deep)
                },
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response
            .UpdateDataList.AbilityCrestList.Should()
            .Contain(x =>
                x.AbilityCrestId == AbilityCrestId.HavingaSummerBall && x.EquipableCount == 1
            );
    }

    [Fact]
    public async Task ReceiveReward_Daily_ClaimsReward()
    {
        int missionId1 = 15070301; // Clear a Quest
        int missionId2 = 15070401; // Clear Three Quests

        TimeProvider timeProvider = TimeProvider.System;

        DateOnly today = DateOnly.FromDateTime(timeProvider.GetLastDailyReset().Date);
        DateOnly yesterday = today.AddDays(-1);

        await this.AddToDatabase(
            [
                new DbCompletedDailyMission()
                {
                    ViewerId = this.ViewerId,
                    Id = missionId1,
                    Date = today,
                },
                new DbCompletedDailyMission()
                {
                    ViewerId = this.ViewerId,
                    Id = missionId2,
                    Date = today,
                },
                new DbCompletedDailyMission()
                {
                    ViewerId = this.ViewerId,
                    Id = missionId1,
                    Date = yesterday,
                },
                new DbCompletedDailyMission()
                {
                    ViewerId = this.ViewerId,
                    Id = missionId2,
                    Date = yesterday,
                },
            ]
        );

        await this.AddToDatabase(
            [
                new DbPlayerMission()
                {
                    ViewerId = this.ViewerId,
                    Id = missionId1,
                    Type = MissionType.Daily,
                    State = MissionState.Completed,
                    Start = timeProvider.GetLastDailyReset(),
                    End = timeProvider.GetLastDailyReset().AddDays(1),
                },
                new DbPlayerMission()
                {
                    ViewerId = this.ViewerId,
                    Id = missionId2,
                    Type = MissionType.Daily,
                    State = MissionState.Completed,
                    Start = timeProvider.GetLastDailyReset(),
                    End = timeProvider.GetLastDailyReset().AddDays(1),
                },
            ]
        );

        DragaliaResponse<MissionReceiveDailyRewardResponse> response =
            await this.Client.PostMsgpack<MissionReceiveDailyRewardResponse>(
                "mission/receive_daily_reward",
                new MissionReceiveDailyRewardRequest()
                {
                    MissionParamsList =
                    [
                        new() { DailyMissionId = missionId1, DayNo = today },
                        new() { DailyMissionId = missionId2, DayNo = today },
                        new() { DailyMissionId = missionId1, DayNo = yesterday },
                    ],
                },
                cancellationToken: TestContext.Current.CancellationToken
            );

        response
            .Data.DailyMissionList.Should()
            .BeEquivalentTo(
                [
                    new DailyMissionList()
                    {
                        DailyMissionId = missionId1,
                        DayNo = today,
                        State = MissionState.Claimed,
                    },
                    new DailyMissionList()
                    {
                        DailyMissionId = missionId2,
                        DayNo = today,
                        State = MissionState.Claimed,
                    },
                    new DailyMissionList()
                    {
                        DailyMissionId = missionId2,
                        DayNo = yesterday,
                        State = MissionState.Completed,
                    },
                ],
                opts =>
                    opts.Including(x => x.DailyMissionId)
                        .Including(x => x.DayNo)
                        .Including(x => x.State)
            );
    }

    [Fact]
    public async Task ReceiveReward_Stamp_GrantsWyrmite()
    {
        // The Miracle Of Dragonyule: Clear a Boss Battle Five Times. Grants 'Splendid!' sticker
        int missionId = 10020502;

        int oldWyrmite = this
            .ApiContext.PlayerUserData.First(x => x.ViewerId == this.ViewerId)
            .Crystal;

        await this.AddToDatabase(
            new DbPlayerMission()
            {
                Id = missionId,
                Type = MissionType.MemoryEvent,
                State = MissionState.Completed,
                Progress = 5,
            }
        );

        MissionReceiveMemoryEventRewardResponse response = (
            await this.Client.PostMsgpack<MissionReceiveMemoryEventRewardResponse>(
                "mission/receive_memory_event_reward",
                new MissionReceiveMemoryEventRewardRequest()
                {
                    MemoryEventMissionIdList = [missionId],
                },
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response.UpdateDataList.UserData.Crystal.Should().Be(oldWyrmite + 25);
    }

    [Fact]
    public async Task GetDailyMissionList_ReturnsUnionOfTables()
    {
        int missionId = 15070301; // Clear a Quest
        DateOnly today = DateOnly.FromDateTime(DateTime.Now);
        DateOnly yesterday = today.AddDays(-1);

        await this.AddToDatabase(
            [
                new DbCompletedDailyMission()
                {
                    ViewerId = this.ViewerId,
                    Id = missionId,
                    Date = today,
                },
                new DbCompletedDailyMission()
                {
                    ViewerId = this.ViewerId,
                    Id = missionId,
                    Date = yesterday,
                },
            ]
        );

        await this.AddToDatabase(
            new DbPlayerMission()
            {
                ViewerId = this.ViewerId,
                Id = missionId,
                Type = MissionType.Daily,
                State = MissionState.Completed,
            }
        );

        DragaliaResponse<MissionGetMissionListResponse> response =
            await this.Client.PostMsgpack<MissionGetMissionListResponse>(
                "mission/get_mission_list",
                cancellationToken: TestContext.Current.CancellationToken
            );

        response
            .Data.DailyMissionList.Should()
            .BeEquivalentTo(
                [
                    new DailyMissionList()
                    {
                        DailyMissionId = missionId,
                        DayNo = today,
                        State = MissionState.Completed,
                    },
                    new DailyMissionList()
                    {
                        DailyMissionId = missionId,
                        DayNo = yesterday,
                        State = MissionState.Completed,
                    },
                ],
                opts =>
                    opts.Including(x => x.DailyMissionId)
                        .Including(x => x.DayNo)
                        .Including(x => x.State)
            );
    }

    [Fact]
    public async Task ClearDaily_ExistingClearInDb_DoesNotThrow()
    {
        int missionId = 15070101; // Perform an Item Summon
        DateOnly date = DateOnly.FromDateTime(DateTime.UtcNow);

        await this.AddToDatabase(
            new DbCompletedDailyMission()
            {
                ViewerId = this.ViewerId,
                Id = missionId,
                Progress = 1,
                Date = date,
            }
        );

        await this.AddToDatabase(
            new DbPlayerMission()
            {
                ViewerId = this.ViewerId,
                Id = missionId,
                Type = MissionType.Daily,
                Progress = 0,
                State = MissionState.InProgress,
            }
        );

        DragaliaResponse<ShopItemSummonExecResponse> response =
            await this.Client.PostMsgpack<ShopItemSummonExecResponse>(
                "shop/item_summon_exec",
                new ShopItemSummonExecRequest() { PaymentType = PaymentTypes.Wyrmite },
                ensureSuccessHeader: false,
                cancellationToken: TestContext.Current.CancellationToken
            );

        response.DataHeaders.ResultCode.Should().Be(ResultCode.Success);
    }

    [Fact]
    public async Task GetDrillMissionList_ReturnsCompletedGroups()
    {
        DbPlayerMission ToDbMission(DrillMission mission)
        {
            return new()
            {
                ViewerId = this.ViewerId,
                Id = mission.Id,
                State = MissionState.Claimed,
                Type = MissionType.Drill,
            };
        }

        MissionGetDrillMissionListResponse response = (
            await this.Client.PostMsgpack<MissionGetDrillMissionListResponse>(
                "mission/get_drill_mission_list",
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response.DrillMissionGroupList.Should().BeEmpty();

        await this.AddRangeToDatabase(
            MasterAsset
                .MissionDrillData.Enumerable.Where(x => x.MissionDrillGroupId == 1)
                .Select(ToDbMission)
        );

        response = (
            await this.Client.PostMsgpack<MissionGetDrillMissionListResponse>(
                "mission/get_drill_mission_list",
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response.DrillMissionGroupList.Should().BeEquivalentTo([new DrillMissionGroupList(1)]);

        await this.AddRangeToDatabase(
            MasterAsset
                .MissionDrillData.Enumerable.Where(x => x.MissionDrillGroupId == 2)
                .Select(ToDbMission)
        );

        response = (
            await this.Client.PostMsgpack<MissionGetDrillMissionListResponse>(
                "mission/get_drill_mission_list",
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response
            .DrillMissionGroupList.Should()
            .BeEquivalentTo([new DrillMissionGroupList(1), new DrillMissionGroupList(2)]);

        await this.AddRangeToDatabase(
            MasterAsset
                .MissionDrillData.Enumerable.Where(x => x.MissionDrillGroupId == 3)
                .Select(ToDbMission)
        );

        response = (
            await this.Client.PostMsgpack<MissionGetDrillMissionListResponse>(
                "mission/get_drill_mission_list",
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response
            .DrillMissionGroupList.Should()
            .BeEquivalentTo(
                [
                    new DrillMissionGroupList(1),
                    new DrillMissionGroupList(2),
                    new DrillMissionGroupList(3),
                ]
            );
    }

    [Fact]
    public async Task GetMissionList_InBetweenDrillGroups_ReturnsRewardCount1()
    {
        await this.AddRangeToDatabase(
            MasterAsset
                .MissionDrillData.Enumerable.Where(x => x.MissionDrillGroupId == 1)
                .Select(x => new DbPlayerMission()
                {
                    ViewerId = this.ViewerId,
                    Id = x.Id,
                    State = MissionState.Claimed,
                    Type = MissionType.Drill,
                })
        );

        MissionGetMissionListResponse response = (
            await this.Client.PostMsgpack<MissionGetMissionListResponse>(
                "mission/get_mission_list",
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response
            .MissionNotice.DrillMissionNotice.ReceivableRewardCount.Should()
            .Be(1, "because otherwise the drill mission popup disappears");
    }

    [Fact]
    public async Task GetMissionList_DoesNotReturnOutOfDateMissions()
    {
        DbPlayerMission expiredMission = new()
        {
            Id = 11650101,
            Type = MissionType.Period,
            State = MissionState.InProgress,
            Start = DateTimeOffset.UtcNow.AddDays(-2),
            End = DateTimeOffset.UtcNow.AddDays(-1),
        };
        DbPlayerMission notStartedMission = new()
        {
            Id = 11650201,
            Type = MissionType.Period,
            State = MissionState.InProgress,
            Start = DateTimeOffset.UtcNow.AddDays(+1),
            End = DateTimeOffset.UtcNow.AddDays(+2),
        };
        DbPlayerMission expectedMission = new()
        {
            Id = 11650301,
            Type = MissionType.Period,
            State = MissionState.InProgress,
            Start = DateTimeOffset.UtcNow.AddDays(-1),
            End = DateTimeOffset.UtcNow.AddDays(+1),
        };
        DbPlayerMission otherExpectedMission = new()
        {
            Id = 11650302,
            Type = MissionType.Period,
            State = MissionState.InProgress,
            Start = DateTimeOffset.UnixEpoch,
            End = DateTimeOffset.UnixEpoch,
        };

        await this.AddRangeToDatabase(
            [expiredMission, notStartedMission, expectedMission, otherExpectedMission]
        );

        MissionGetMissionListResponse response = (
            await this.Client.PostMsgpack<MissionGetMissionListResponse>(
                "mission/get_mission_list",
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response
            .PeriodMissionList.Should()
            .HaveCount(2)
            .And.Contain(x => x.PeriodMissionId == expectedMission.Id)
            .And.Contain(x => x.PeriodMissionId == otherExpectedMission.Id);
    }

    [Fact]
    public async Task GetMissionList_DoesNotReturnPreviousMainStoryMissions()
    {
        IEnumerable<DbPlayerMission> completedMissions = MasterAsset
            .MissionMainStoryData.Enumerable.Where(x => x.MissionMainStoryGroupId == 1)
            .Select(x => new DbPlayerMission()
            {
                Id = x.Id,
                GroupId = 1,
                ViewerId = this.ViewerId,
                Type = MissionType.MainStory,
                State = MissionState.Claimed,
            });

        await this.AddRangeToDatabase(completedMissions);

        DragaliaResponse<MissionGetMissionListResponse> response =
            await this.Client.PostMsgpack<MissionGetMissionListResponse>(
                "mission/get_mission_list",
                cancellationToken: TestContext.Current.CancellationToken
            );

        response
            .Data.CurrentMainStoryMission.Should()
            .BeEquivalentTo(new CurrentMainStoryMission());
    }

    [Fact]
    public async Task GetMissionList_ReturnsCurrentMainStoryMissions()
    {
        IEnumerable<DbPlayerMission> inProgressMissions = MasterAsset
            .MissionMainStoryData.Enumerable.Where(x => x.MissionMainStoryGroupId == 1)
            .Select(x => new DbPlayerMission()
            {
                Id = x.Id,
                GroupId = 1,
                ViewerId = this.ViewerId,
                Type = MissionType.MainStory,
                State = MissionState.InProgress,
            });

        await this.AddRangeToDatabase(inProgressMissions);

        DragaliaResponse<MissionGetMissionListResponse> response =
            await this.Client.PostMsgpack<MissionGetMissionListResponse>(
                "mission/get_mission_list",
                cancellationToken: TestContext.Current.CancellationToken
            );

        response
            .Data.CurrentMainStoryMission.Should()
            .BeEquivalentTo(
                new CurrentMainStoryMission()
                {
                    MainStoryMissionGroupId = 1,
                    MainStoryMissionStateList = MasterAsset
                        .MissionMainStoryData.Enumerable.Where(x => x.MissionMainStoryGroupId == 1)
                        .Select(x => new AtgenMainStoryMissionStateList()
                        {
                            MainStoryMissionId = x.Id,
                            State = (int)MissionState.InProgress,
                        }),
                }
            );
    }
}
