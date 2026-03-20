using System.Threading;
using Cysharp.Threading.Tasks;
using HKFeedback;
using UnityEngine;

namespace UnityGameWithCodex
{
    [CreateAssetMenu(menuName = "UnityGameWithCodex/Active Skill")]
    public class ActiveSkill : ScriptableObject
    {
        [SerializeReference, SubclassSelector]
        private IFeedback<BattleContext>[] feedbacks;

        public virtual UniTask InvokeAsync(BattleContext battleContext)
        {
            if (feedbacks == null)
            {
                return UniTask.CompletedTask;
            }

            for (var index = 0; index < feedbacks.Length; index++)
            {
                var feedback = feedbacks[index];
                if (feedback == null)
                {
                    continue;
                }

                feedback.PlayAsync(battleContext, CancellationToken.None).Forget();
            }

            return UniTask.CompletedTask;
        }
    }
}
