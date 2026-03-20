using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UnityGameWithCodex
{
    public class MainSceneController : MonoBehaviour
    {
        [SerializeField] private BattleSystem.Party playerParty = new();
        [SerializeField] private BattleSystem.Party enemyParty = new();

        private async UniTaskVoid Start()
        {
            EnsureCharactersExist();

            var battleSystem = new BattleSystem(playerParty, enemyParty);
            await battleSystem.BeginAsync();
        }

        private void EnsureCharactersExist()
        {
            if (playerParty.Characters.Count == 0)
            {
                playerParty.Characters.Add(new BattleSystem.BattleCharacter("Ally"));
            }

            if (enemyParty.Characters.Count == 0)
            {
                enemyParty.Characters.Add(new BattleSystem.BattleCharacter("Enemy"));
            }
        }
    }
}
