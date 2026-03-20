namespace UnityGameWithCodex.BattleControllers
{
    public interface ISelectTarget
    {
        BattleSystem.BattleCharacter Select(BattleContext context);
    }
}
