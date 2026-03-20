using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HKFeedback;
using UnityEngine;
using UnityGameWithCodex.BattleControllers;

namespace UnityGameWithCodex.FeedbackSystems.Actions
{
    [Serializable]
    public sealed class GiveDamage : IFeedback<BattleContext>
    {
        [SerializeField] private float power = 1f;
        [SerializeReference, SubclassSelector]
        private ISelectTarget selectTarget;

        public UniTask PlayAsync(BattleContext context, CancellationToken cancellationToken)
        {
            if (selectTarget == null)
            {
                Debug.LogWarning("GiveDamage target selector is not configured.");
                return UniTask.CompletedTask;
            }

            var target = selectTarget.Select(context);
            if (target == null || target.IsDead)
            {
                return UniTask.CompletedTask;
            }

            var calculatedDamage = Mathf.RoundToInt(context.ActingCharacter.PhysicalAttackPower * power);
            target.TakeDamage(calculatedDamage);
            return UniTask.CompletedTask;
        }
    }
}
