﻿using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Tutorial;
using DragaliaAPI.Infrastructure.Results;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Dungeon;

/// <summary>
/// Tests <see cref="DragaliaAPI.Features.Dungeon.Start.DungeonStartController"/>.
/// </summary>
public class DungeonStartTest : TestFixture
{
    private static VerifySettings VerifySettings
    {
        get
        {
            VerifySettings settings = new();
            settings.IgnoreMember<DragonList>(x => x.DragonKeyId);
            settings.IgnoreMember<TalismanList>(x => x.TalismanKeyId);
            return settings;
        }
    }

    public DungeonStartTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        this.ImportSave().Wait();
    }

    [Fact]
    public async Task Start_OneTeam_HasExpectedPartyUnitList()
    {
        DungeonStartStartResponse response = (
            await Client.PostMsgpack<DungeonStartStartResponse>(
                "/dungeon_start/start",
                new DungeonStartStartRequest()
                {
                    PartyNoList = new List<int>() { 1 },
                    QuestId = 100010103,
                },
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        await Verify(response.IngameData.PartyInfo.PartyUnitList, VerifySettings);

        response.IngameData.PartyInfo.PartyUnitList.Should().HaveCount(4);
        response.IngameData.PartyInfo.PartyUnitList.Should().BeInAscendingOrder(x => x.Position);
        response.IngameData.PartyInfo.PartyUnitList.Should().OnlyHaveUniqueItems(x => x.Position);
        response.IngameData.IsBotTutorial.Should().BeFalse();
    }

    [Fact]
    public async Task Start_TwoTeams_HasExpectedPartyUnitList()
    {
        DungeonStartStartResponse response = (
            await Client.PostMsgpack<DungeonStartStartResponse>(
                "/dungeon_start/start",
                new DungeonStartStartRequest()
                {
                    PartyNoList = new List<int>() { 37, 38 },
                    QuestId = 100010103,
                },
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        // Abuse of snapshots here is lazy, but the resulting JSON is thousands of lines long...
        await Verify(response.IngameData.PartyInfo.PartyUnitList, VerifySettings);

        response.IngameData.PartyInfo.PartyUnitList.Should().HaveCount(8);
        response.IngameData.PartyInfo.PartyUnitList.Should().BeInAscendingOrder(x => x.Position);
        response.IngameData.PartyInfo.PartyUnitList.Should().OnlyHaveUniqueItems(x => x.Position);
    }

    [Fact]
    public async Task Start_WeaponPassivesUnlocked_IncludedInPartyUnitList()
    {
        DungeonStartStartResponse response = (
            await Client.PostMsgpack<DungeonStartStartResponse>(
                "/dungeon_start/start",
                new DungeonStartStartRequest()
                {
                    PartyNoList = new List<int>() { 38 },
                    QuestId = 100010103,
                },
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response
            .IngameData.PartyInfo.PartyUnitList.First(x =>
                x.CharaData!.CharaId == Charas.GalaMascula
            )
            .GameWeaponPassiveAbilityList.Should()
            .Contain(x => x.WeaponPassiveAbilityId == 1020211);
    }

    [Fact]
    public async Task StartAssignUnit_HasExpectedPartyList()
    {
        DungeonSkipStartAssignUnitRequest request = new()
        {
            QuestId = 100010103,
            RequestPartySettingList = new List<PartySettingList>()
            {
                new()
                {
                    UnitNo = 1,
                    CharaId = Charas.GalaLeonidas,
                    EquipWeaponBodyId = WeaponBodies.Draupnir,
                    EquipDragonKeyId = (ulong)GetDragonKeyId(DragonId.Horus),
                    EquipCrestSlotType1CrestId1 = AbilityCrestId.PrimalCrisis,
                    EquipCrestSlotType1CrestId2 = AbilityCrestId.TheCutieCompetition,
                    EquipCrestSlotType1CrestId3 = AbilityCrestId.AnIndelibleDate,
                    EquipCrestSlotType2CrestId1 = AbilityCrestId.BeautifulGunman,
                    EquipCrestSlotType2CrestId2 = AbilityCrestId.DragonArcanum,
                    EquipTalismanKeyId = (ulong)GetTalismanKeyId(Talismans.GalaLeonidas),
                    EquipCrestSlotType3CrestId1 = AbilityCrestId.AKnightsDreamAxesBoon,
                    EquipCrestSlotType3CrestId2 = AbilityCrestId.CrownofLightSerpentsBoon,
                    EditSkill1CharaId = Charas.GalaZethia,
                    EditSkill2CharaId = Charas.GalaMascula,
                },
                new()
                {
                    UnitNo = 2,
                    CharaId = Charas.GalaGatov,
                    EquipWeaponBodyId = WeaponBodies.Mjoelnir,
                    EquipDragonKeyId = (ulong)GetDragonKeyId(DragonId.GalaMars),
                    EquipCrestSlotType1CrestId1 = AbilityCrestId.TheCutieCompetition,
                    EquipCrestSlotType1CrestId2 = AbilityCrestId.KungFuMasters,
                    EquipCrestSlotType1CrestId3 = AbilityCrestId.BondsBetweenWorlds,
                    EquipCrestSlotType2CrestId1 = AbilityCrestId.DragonArcanum,
                    EquipCrestSlotType2CrestId2 = AbilityCrestId.BeautifulNothingness,
                    EquipTalismanKeyId = (ulong)GetTalismanKeyId(Talismans.GalaMym),
                    EquipCrestSlotType3CrestId1 = AbilityCrestId.TutelarysDestinyWolfsBoon,
                    EquipCrestSlotType3CrestId2 = AbilityCrestId.TestamentofEternityFishsBoon,
                },
            },
        };

        DungeonStartStartAssignUnitResponse response = (
            await Client.PostMsgpack<DungeonStartStartAssignUnitResponse>(
                "/dungeon_start/start_assign_unit",
                request,
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        // Only test the first two since the others are empty
        await Verify(response.IngameData.PartyInfo.PartyUnitList.Take(2), VerifySettings);

        response.IngameData.PartyInfo.PartyUnitList.Should().HaveCount(4);
        response
            .IngameData.PartyInfo.PartyUnitList.Should()
            .Contain(x => x.CharaData!.CharaId == Charas.GalaLeonidas)
            .And.Contain(x => x.CharaData!.CharaId == Charas.GalaGatov);
    }

    [Theory]
    [InlineData("start")]
    [InlineData("start_assign_unit")]
    public async Task Start_InsufficientStamina_ReturnsError(string endpoint)
    {
        await this.ApiContext.PlayerUserData.ExecuteUpdateAsync(
            p => p.SetProperty(e => e.StaminaSingle, e => 0),
            cancellationToken: TestContext.Current.CancellationToken
        );
        await this.ApiContext.PlayerUserData.ExecuteUpdateAsync(
            p => p.SetProperty(e => e.StaminaMulti, e => 0),
            cancellationToken: TestContext.Current.CancellationToken
        );
        await this.ApiContext.PlayerUserData.ExecuteUpdateAsync(
            p => p.SetProperty(e => e.LastStaminaSingleUpdateTime, e => DateTimeOffset.UtcNow),
            cancellationToken: TestContext.Current.CancellationToken
        );
        await this.ApiContext.PlayerUserData.ExecuteUpdateAsync(
            p => p.SetProperty(e => e.LastStaminaMultiUpdateTime, e => DateTimeOffset.UtcNow),
            cancellationToken: TestContext.Current.CancellationToken
        );

        (
            await Client.PostMsgpack<DungeonStartStartResponse>(
                $"/dungeon_start/{endpoint}",
                new DungeonStartStartRequest() { QuestId = 100010104, PartyNoList = [1] },
                ensureSuccessHeader: false,
                cancellationToken: TestContext.Current.CancellationToken
            )
        )
            .DataHeaders.ResultCode.Should()
            .Be(ResultCode.QuestStaminaSingleShort);
    }

    [Fact]
    public async Task Start_ZeroStamina_FirstClearOfMainStory_Allows()
    {
        await this.ApiContext.PlayerQuests.ExecuteDeleteAsync(
            cancellationToken: TestContext.Current.CancellationToken
        );

        await this.ApiContext.PlayerUserData.ExecuteUpdateAsync(
            p => p.SetProperty(e => e.StaminaSingle, e => 0),
            cancellationToken: TestContext.Current.CancellationToken
        );
        await this.ApiContext.PlayerUserData.ExecuteUpdateAsync(
            p => p.SetProperty(e => e.StaminaMulti, e => 0),
            cancellationToken: TestContext.Current.CancellationToken
        );
        await this.ApiContext.PlayerUserData.ExecuteUpdateAsync(
            p => p.SetProperty(e => e.LastStaminaSingleUpdateTime, e => DateTimeOffset.UtcNow),
            cancellationToken: TestContext.Current.CancellationToken
        );
        await this.ApiContext.PlayerUserData.ExecuteUpdateAsync(
            p => p.SetProperty(e => e.LastStaminaMultiUpdateTime, e => DateTimeOffset.UtcNow),
            cancellationToken: TestContext.Current.CancellationToken
        );

        (
            await Client.PostMsgpack<DungeonStartStartResponse>(
                $"/dungeon_start/start",
                new DungeonStartStartRequest()
                {
                    QuestId = 100260101,
                    PartyNoList = new List<int>() { 1 },
                },
                ensureSuccessHeader: false,
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).DataHeaders.ResultCode.Should().Be(ResultCode.Success);
    }

    [Fact]
    public async Task Start_ChronosClash_HasRareEnemy()
    {
        DragaliaResponse<DungeonStartStartResponse> response =
            await this.Client.PostMsgpack<DungeonStartStartResponse>(
                $"/dungeon_start/start",
                new DungeonStartStartRequest() { QuestId = 204270302, PartyNoList = [1] },
                cancellationToken: TestContext.Current.CancellationToken
            );

        response.Data.OddsInfo.Enemy.Should().Contain(x => x.ParamId == 204130320 && x.IsRare);
    }

    [Fact]
    public async Task Start_EarnEvent_EnemiesDuplicated()
    {
        const int earnEventQuestId = 229031201; // Repelling the Frosty Fiends: Standard (Solo)

        DragaliaResponse<DungeonStartStartResponse> response =
            await this.Client.PostMsgpack<DungeonStartStartResponse>(
                $"/dungeon_start/start",
                new DungeonStartStartRequest() { QuestId = earnEventQuestId, PartyNoList = [1] },
                cancellationToken: TestContext.Current.CancellationToken
            );

        response.Data.OddsInfo.Enemy.Should().HaveCount(31);

        QuestData questData = MasterAsset.QuestData[earnEventQuestId];
        IEnumerable<int> enemies = MasterAsset
            .QuestEnemies[$"{questData.Scene01}/{questData.AreaName01}".ToLowerInvariant()]
            .Enemies[questData.VariationType];

        response.Data.OddsInfo.Enemy.Should().HaveCountGreaterThan(enemies.Count());
    }

    [Fact]
    public async Task Start_CoopTutorial_SetsIsBotTutorialTrue()
    {
        await this.ApiContext.PlayerUserData.ExecuteUpdateAsync(
            e =>
                e.SetProperty(
                    p => p.TutorialStatus,
                    TutorialService.TutorialStatusIds.CoopTutorial
                ),
            cancellationToken: TestContext.Current.CancellationToken
        );

        DragaliaResponse<DungeonStartStartResponse> response =
            await this.Client.PostMsgpack<DungeonStartStartResponse>(
                $"/dungeon_start/start",
                new DungeonStartStartRequest()
                {
                    QuestId = TutorialService.TutorialQuestIds.AvenueToPowerBeginner,
                    PartyNoList = [1],
                },
                cancellationToken: TestContext.Current.CancellationToken
            );

        response.Data.IngameData.IsBotTutorial.Should().BeTrue();
    }

    [Fact]
    public async Task Start_AtpBeginner_NotCoopTutorial_SetsIsBotTutorialFalse()
    {
        await this.ApiContext.PlayerUserData.ExecuteUpdateAsync(
            e =>
                e.SetProperty(
                    p => p.TutorialStatus,
                    TutorialService.TutorialStatusIds.CoopTutorial + 1
                ),
            cancellationToken: TestContext.Current.CancellationToken
        );

        DragaliaResponse<DungeonStartStartResponse> response =
            await this.Client.PostMsgpack<DungeonStartStartResponse>(
                $"/dungeon_start/start",
                new DungeonStartStartRequest()
                {
                    QuestId = TutorialService.TutorialQuestIds.AvenueToPowerBeginner,
                    PartyNoList = [1],
                },
                cancellationToken: TestContext.Current.CancellationToken
            );

        response.Data.IngameData.IsBotTutorial.Should().BeFalse();
    }

    [Fact]
    public async Task Start_OffElementWeapon_SendsCorrectWeaponBonuses()
    {
        int flameDullRes = 1010104;
        WeaponBodies waterSword = WeaponBodies.AbsoluteAqua;

        await this
            .ApiContext.PlayerPassiveAbilities.Where(x => x.ViewerId == this.ViewerId)
            .ExecuteDeleteAsync(cancellationToken: TestContext.Current.CancellationToken);

        await this.AddToDatabase(
            new DbWeaponPassiveAbility() { WeaponPassiveAbilityId = flameDullRes }
        );

        await this
            .ApiContext.PlayerPartyUnits.Where(x =>
                x.ViewerId == this.ViewerId && x.PartyNo == 1 && x.UnitNo == 1
            )
            .ExecuteUpdateAsync(
                e =>
                    e.SetProperty(u => u.EquipWeaponBodyId, waterSword)
                        .SetProperty(u => u.CharaId, Charas.ThePrince),
                cancellationToken: TestContext.Current.CancellationToken
            );

        DragaliaResponse<DungeonStartStartResponse> response =
            await this.Client.PostMsgpack<DungeonStartStartResponse>(
                $"/dungeon_start/start",
                new DungeonStartStartRequest()
                {
                    QuestId = TutorialService.TutorialQuestIds.AvenueToPowerBeginner,
                    PartyNoList = [1],
                },
                cancellationToken: TestContext.Current.CancellationToken
            );

        response
            .Data.IngameData.PartyInfo.PartyUnitList.First(x => x.Position == 1)
            .GameWeaponPassiveAbilityList.Should()
            .Contain(x => x.WeaponPassiveAbilityId == flameDullRes);
    }
}
