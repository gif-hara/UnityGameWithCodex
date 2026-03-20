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
        [SerializeField] private int damage = 1;

        public UniTask PlayAsync(BattleContext context, CancellationToken cancellationToken)
        {
            foreach (var character in context.OpponentParty.Characters)
            {
                if (character.IsDead)
                {
                    continue;
                }

                character.TakeDamage(damage);
                return UniTask.CompletedTask;
            }

            return UniTask.CompletedTask;
        }
    }
}
