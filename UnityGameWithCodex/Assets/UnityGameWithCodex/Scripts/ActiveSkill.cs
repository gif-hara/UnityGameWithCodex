using System.Threading;
using Cysharp.Threading.Tasks;
using HKFeedback;
using HKFeedback.Extensions;
using UnityEngine;

namespace UnityGameWithCodex
{
    [CreateAssetMenu(menuName = "UnityGameWithCodex/Active Skill")]
    public class ActiveSkill : ScriptableObject
    {
        [SerializeField] private string skillName = "Active Skill";
        [SerializeField] private float coolTime = 1f;
        [SerializeReference, SubclassSelector]
        private IFeedback<BattleContext>[] feedbacks;

        public string SkillName => skillName;
        public float CoolTime => coolTime;

        public virtual UniTask InvokeAsync(BattleContext battleContext, CancellationToken cancellationToken)
        {
            if (feedbacks == null)
            {
                return UniTask.CompletedTask;
            }

            return feedbacks.PlayAsync(battleContext, cancellationToken);
        }
    }
}
