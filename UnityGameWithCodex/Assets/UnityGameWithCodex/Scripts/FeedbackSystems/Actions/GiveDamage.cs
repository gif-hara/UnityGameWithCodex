using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HKFeedback;
using UnityEngine;
using UnityGameWithCodex.BattleControllers;
using UnityGameWithCodex.BattleControllers.SelectTarget;

namespace UnityGameWithCodex.FeedbackSystems.Actions
{
    [Serializable]
    public sealed class GiveDamage<TContext> : IFeedback<TContext> where TContext : IProvider<BattleContext>
    {
        [SerializeField] private float power = 1f;
        [SerializeReference, SubclassSelector]
        private ISelectTarget selectTarget;

        public UniTask PlayAsync(TContext context, CancellationToken cancellationToken)
        {
            var battleContext = context.Provide();
            if (selectTarget == null)
            {
                Debug.LogWarning("GiveDamage target selector is not configured.");
                return UniTask.CompletedTask;
            }

            var target = selectTarget.Select(battleContext);
            if (target == null || target.IsDead)
            {
                return UniTask.CompletedTask;
            }

            var calculatedDamage = Mathf.RoundToInt(battleContext.ActingCharacter.PhysicalAttackPower * power);
            target.TakeDamage(calculatedDamage);
            return UniTask.CompletedTask;
        }
    }
}
