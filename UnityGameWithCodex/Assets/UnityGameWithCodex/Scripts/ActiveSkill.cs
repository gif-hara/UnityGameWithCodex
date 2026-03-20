using System.Threading;
using Cysharp.Threading.Tasks;
using HKFeedback;
using UnityEngine;

namespace UnityGameWithCodex
{
    public class ActiveSkill : ScriptableObject
    {
        [SerializeReference, SubclassSelector]
        private IFeedback<BattleContext> feedback;

        public virtual UniTask InvokeAsync(BattleContext battleContext)
        {
            if (feedback == null)
            {
                return UniTask.CompletedTask;
            }

            return feedback.PlayAsync(battleContext, CancellationToken.None);
        }
    }
}
