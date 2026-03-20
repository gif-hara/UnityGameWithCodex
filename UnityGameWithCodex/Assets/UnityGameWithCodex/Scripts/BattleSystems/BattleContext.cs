using HKFeedback;

namespace UnityGameWithCodex.BattleControllers
{
    public readonly struct BattleContext : IProvider<BattleContext>
    {
        public BattleContext(BattleSystem.BattleCharacter actingCharacter, BattleSystem.Party allyParty, BattleSystem.Party opponentParty)
        {
            ActingCharacter = actingCharacter;
            AllyParty = allyParty;
            OpponentParty = opponentParty;
        }

        public BattleSystem.BattleCharacter ActingCharacter { get; }
        public BattleSystem.Party AllyParty { get; }
        public BattleSystem.Party OpponentParty { get; }

        BattleContext IProvider<BattleContext>.Provide() => this;
    }
}
