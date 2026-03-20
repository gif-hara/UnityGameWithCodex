using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UnityGameWithCodex
{
    public class BattleSystem
    {
        public sealed class BattleCharacter
        {
            [SerializeField] private int hp = 100;
            [SerializeField] private int physicalAttackPower = 10;
            [SerializeField] private int magicalAttackPower = 10;
            [SerializeField] private int physicalDefensePower = 10;
            [SerializeField] private int magicalDefensePower = 10;
            [SerializeField] private int agility = 10;
            [SerializeField] private float cooldown;

            public int Hp => hp;
            public int PhysicalAttackPower => physicalAttackPower;
            public int MagicalAttackPower => magicalAttackPower;
            public int PhysicalDefensePower => physicalDefensePower;
            public int MagicalDefensePower => magicalDefensePower;
            public int Agility => agility;
            public float Cooldown
            {
                get => cooldown;
                set => cooldown = value;
            }
        }

        private readonly IReadOnlyList<BattleCharacter> allies;
        private readonly IReadOnlyList<BattleCharacter> enemies;

        public BattleSystem(IReadOnlyList<BattleCharacter> allies, IReadOnlyList<BattleCharacter> enemies)
        {
            this.allies = allies;
            this.enemies = enemies;
        }

        public async UniTask BeginAsync()
        {
            while (allies.Count > 0 && enemies.Count > 0)
            {
                float deltaTime = Time.deltaTime;
                TickCharacters(allies, "ally", deltaTime);
                TickCharacters(enemies, "enemy", deltaTime);
                await UniTask.NextFrame();
            }
        }

        private static void TickCharacters(IReadOnlyList<BattleCharacter> characters, string sideLabel, float deltaTime)
        {
            for (int index = 0; index < characters.Count; index++)
            {
                BattleCharacter character = characters[index];
                character.Cooldown += deltaTime * (1f + (character.Agility / 100.0f));

                if (character.Cooldown < 1f)
                {
                    continue;
                }

                Debug.Log($"{sideLabel}[{index}] is ready");
                character.Cooldown = 0f;
            }
        }
    }
}
