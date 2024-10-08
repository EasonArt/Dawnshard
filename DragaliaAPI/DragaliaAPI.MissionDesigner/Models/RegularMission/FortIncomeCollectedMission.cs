namespace DragaliaAPI.MissionDesigner.Models.RegularMission;

public class FortIncomeCollectedMission : Mission
{
    public required EntityTypes EntityType { get; init; }

    protected override MissionCompleteType CompleteType => MissionCompleteType.FortIncomeCollected;

    protected override int? Parameter => (int)this.EntityType;
}
