namespace UnityGameWithCodex.FeedbackSystems.Actions
{
    public interface ISelectTarget
    {
        BattleSystem.BattleCharacter Select(BattleContext context);
    }
}
