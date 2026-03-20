using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UnityGameWithCodex
{
    public class MainSceneController : MonoBehaviour
    {
        [SerializeField] private BattleSystem.Party allies = new();
        [SerializeField] private BattleSystem.Party enemies = new();

        private async UniTask Start()
        {
            EnsureCharactersExist();

            BattleSystem battleSystem = new(allies, enemies);
            await battleSystem.BeginAsync();
        }

        private void EnsureCharactersExist()
        {
            if (allies.Characters.Count == 0)
            {
                allies.Characters.Add(new BattleSystem.BattleCharacter("Ally"));
            }

            if (enemies.Characters.Count == 0)
            {
                enemies.Characters.Add(new BattleSystem.BattleCharacter("Enemy"));
            }
        }
    }
}
