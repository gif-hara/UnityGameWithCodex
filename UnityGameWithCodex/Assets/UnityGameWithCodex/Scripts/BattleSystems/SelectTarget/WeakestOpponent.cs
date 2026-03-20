using System;

namespace UnityGameWithCodex.BattleControllers.SelectTarget
{
    [Serializable]
    public class WeakestOpponent : ISelectTarget
    {
        public BattleSystem.BattleCharacter Select(BattleContext context)
        {
            BattleSystem.BattleCharacter weakest = null;
            foreach (var character in context.OpponentParty.Characters)
            {
                if (character.IsDead)
                {
                    continue;
                }

                if (weakest == null || character.Hp < weakest.Hp)
                {
                    weakest = character;
                }
            }
            return weakest;
        }
    }
}
