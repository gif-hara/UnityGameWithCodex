using UnityEngine;

namespace UnityGameWithCodex
{
    public sealed class BattleContext
    {
        public BattleContext(BattleSystem.BattleCharacter actingCharacter)
        {
            ActingCharacter = actingCharacter;
        }

        public BattleSystem.BattleCharacter ActingCharacter { get; }
    }
}
