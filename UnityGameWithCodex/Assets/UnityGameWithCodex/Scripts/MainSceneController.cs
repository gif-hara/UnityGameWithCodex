using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityGameWithCodex.BattleControllers;

namespace UnityGameWithCodex
{
    public class MainSceneController : MonoBehaviour
    {
        [SerializeField] private BattleSystem.Party playerParty = new();
        [SerializeField] private BattleSystem.Party enemyParty = new();

        private async UniTaskVoid Start()
        {
            var battleSystem = new BattleSystem(playerParty, enemyParty);
            await battleSystem.BeginAsync(destroyCancellationToken);
            Debug.Log("Battle ended.");
        }
    }
}
