using UnityEngine;

namespace UnityGameWithCodex
{
    public sealed class BattleContext
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
    }
}
