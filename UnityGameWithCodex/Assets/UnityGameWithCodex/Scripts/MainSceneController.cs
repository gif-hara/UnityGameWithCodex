using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UnityGameWithCodex
{
    public class MainSceneController : MonoBehaviour
    {
        [SerializeField] private List<BattleSystem.BattleCharacter> allies = new();
        [SerializeField] private List<BattleSystem.BattleCharacter> enemies = new();

        private async UniTask Start()
        {
            EnsureCharactersExist();

            BattleSystem battleSystem = new(allies, enemies);
            await battleSystem.BeginAsync();
        }

        private void EnsureCharactersExist()
        {
            if (allies.Count == 0)
            {
                allies.Add(new BattleSystem.BattleCharacter());
            }

            if (enemies.Count == 0)
            {
                enemies.Add(new BattleSystem.BattleCharacter());
            }
        }
    }
}
