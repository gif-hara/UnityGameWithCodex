using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HKFeedback;
using UnityEngine;

namespace UnityGameWithCodex.FeedbackSystems.Actions
{
    [Serializable]
    public sealed class GiveDamage : IFeedback<BattleContext>
    {
        [SerializeField] private float damage = 1f;

        public UniTask PlayAsync(BattleContext context, CancellationToken cancellationToken)
        {
            foreach (var character in context.OpponentParty.Characters)
            {
                if (character.IsDead)
                {
                    continue;
                }

                var calculatedDamage = Mathf.RoundToInt(context.ActingCharacter.PhysicalAttackPower * damage);
                character.TakeDamage(calculatedDamage);
                return UniTask.CompletedTask;
            }

            return UniTask.CompletedTask;
        }
    }
}
