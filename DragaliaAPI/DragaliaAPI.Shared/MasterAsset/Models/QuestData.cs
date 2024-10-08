﻿using DragaliaAPI.Photon.Shared.Enums;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models.Event;
using MessagePack;

namespace DragaliaAPI.Shared.MasterAsset.Models;

public record QuestData(
    int Id,
    int Gid,
    QuestGroupType GroupType,
    QuestPlayModeTypes QuestPlayModeType,
    UnitElement LimitedElementalType,
    UnitElement LimitedElementalType2,
    int LimitedWeaponTypePatternId,
    bool IsPayForceStaminaSingle,
    int PayStaminaSingle,
    int CampaignStaminaSingle,
    int PayStaminaMulti,
    int CampaignStaminaMulti,
    DungeonTypes DungeonType,
    VariationTypes VariationType,
    string Scene01,
    string AreaName01,
    string Scene02,
    string AreaName02,
    string Scene03,
    string AreaName03,
    string Scene04,
    string AreaName04,
    string Scene05,
    string AreaName05,
    string Scene06,
    string AreaName06,
    int RebornLimit,
    int ContinueLimit,
    int Difficulty,
    PayTargetType PayEntityTargetType,
    EntityTypes PayEntityType,
    int PayEntityId,
    int PayEntityQuantity,
    EntityTypes HoldEntityType,
    int HoldEntityId,
    int HoldEntityQuantity,
    bool IsSumUpTotalDamage
)
{
    private int IdSuffix => this.Id % 1000;

    [IgnoreMember]
    public IReadOnlyList<AreaInfo> AreaInfo { get; } =
        new List<AreaInfo>()
        {
            new(Scene01, AreaName01),
            new(Scene02, AreaName02),
            new(Scene03, AreaName03),
            new(Scene04, AreaName04),
            new(Scene05, AreaName05),
            new(Scene06, AreaName06),
        }
            .Where(x => !string.IsNullOrEmpty(x.ScenePath) && !string.IsNullOrEmpty(x.AreaName))
            .ToList()
            .AsReadOnly();

    [IgnoreMember]
    public bool IsEventRegularBattle =>
        this.EventKindType switch
        {
            EventKindType.Build => this.IdSuffix is 301 or 302 or 303 or 401, // Boss battle (or EX boss battle)
            EventKindType.Raid => this.IdSuffix is 201 or 202 or 203, // Boss battle
            EventKindType.Earn => this.IdSuffix is 201 or 202 or 203 or 401, // Invasion quest
            _ => false,
        };

    [IgnoreMember]
    public bool IsEventChallengeBattle =>
        this.EventKindType switch
        {
            EventKindType.Build => this.IdSuffix is 501 or 502,
            _ => false,
        };

    [IgnoreMember]
    public bool IsEventTrial =>
        this.EventKindType switch
        {
            EventKindType.Build => this.IdSuffix is 701 or 702,
            EventKindType.Earn => this.IdSuffix is 301 or 302 or 303,
            _ => false,
        };

    [IgnoreMember]
    public bool IsEventExBattle =>
        this.EventKindType switch
        {
            EventKindType.Build => this.IdSuffix is 401,
            _ => false,
        };

    [IgnoreMember]
    public EventKindType EventKindType =>
        MasterAsset.EventData.TryGetValue(this.Gid, out EventData? eventData)
            ? eventData.EventKindType
            : EventKindType.None;

    [IgnoreMember]
    public bool IsEventQuest => GroupType == QuestGroupType.Event;

    [IgnoreMember]
    public bool CanPlayCoOp => this.PayStaminaMulti > 0;
}
