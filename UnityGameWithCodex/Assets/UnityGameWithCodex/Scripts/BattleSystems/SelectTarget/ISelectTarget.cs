namespace UnityGameWithCodex.BattleControllers.SelectTarget
{
    public interface ISelectTarget
    {
        BattleSystem.BattleCharacter Select(BattleContext context);
    }
}
