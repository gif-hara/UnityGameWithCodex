using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UnityGameWithCodex
{
    public class ActiveSkill : ScriptableObject
    {
        public virtual UniTask InvokeAsync(BattleContext battleContext)
        {
            Debug.Log($"{name} invoked by {battleContext.ActingCharacter.Name}");
            return UniTask.CompletedTask;
        }
    }
}
