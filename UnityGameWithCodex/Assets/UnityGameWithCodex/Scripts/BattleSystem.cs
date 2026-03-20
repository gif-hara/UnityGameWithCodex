using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UnityGameWithCodex
{
    public class BattleSystem
    {
        [System.Serializable]
        public sealed class BattleCharacter
        {
            [SerializeField] private string name = "Character";
            [SerializeField] private int hp = 100;
            [SerializeField] private int physicalAttackPower = 10;
            [SerializeField] private int magicalAttackPower = 10;
            [SerializeField] private int physicalDefensePower = 10;
            [SerializeField] private int magicalDefensePower = 10;
            [SerializeField] private int agility = 10;
            [SerializeField] private float cooldown;
            [SerializeField] private ActiveSkill activeSkill;

            public BattleCharacter()
            {
            }

            public BattleCharacter(string name)
            {
                this.name = name;
            }

            public string Name => name;
            public int Hp => hp;
            public int PhysicalAttackPower => physicalAttackPower;
            public int MagicalAttackPower => magicalAttackPower;
            public int PhysicalDefensePower => physicalDefensePower;
            public int MagicalDefensePower => magicalDefensePower;
            public int Agility => agility;
            public bool IsDead => hp <= 0;
            public ActiveSkill ActiveSkill => activeSkill;
            public float Cooldown
            {
                get => cooldown;
                set => cooldown = value;
            }
        }

        [System.Serializable]
        public sealed class Party
        {
            [SerializeField] private List<BattleCharacter> characters = new();

            public Party()
            {
            }

            public Party(List<BattleCharacter> characters)
            {
                this.characters = characters;
            }

            public List<BattleCharacter> Characters => characters;
            public bool IsAllDead
            {
                get
                {
                    if (characters.Count == 0)
                    {
                        return true;
                    }

                    for (int index = 0; index < characters.Count; index++)
                    {
                        if (!characters[index].IsDead)
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }
        }

        private readonly Party allies;
        private readonly Party enemies;

        public BattleSystem(Party allies, Party enemies)
        {
            this.allies = allies;
            this.enemies = enemies;
        }

        public async UniTask BeginAsync()
        {
            while (!allies.IsAllDead && !enemies.IsAllDead)
            {
                var deltaTime = Time.deltaTime;
                await TickCharactersAsync(allies, enemies, deltaTime);
                await TickCharactersAsync(enemies, allies, deltaTime);
                await UniTask.NextFrame();
            }
        }

        private static async UniTask TickCharactersAsync(Party allyParty, Party opponentParty, float deltaTime)
        {
            for (var index = 0; index < allyParty.Characters.Count; index++)
            {
                var character = allyParty.Characters[index];
                if (character.IsDead)
                {
                    continue;
                }

                character.Cooldown += deltaTime * (1f + (character.Agility / 100.0f));

                if (character.Cooldown < 1f)
                {
                    continue;
                }

                character.Cooldown = 0f;

                if (character.ActiveSkill == null)
                {
                    Debug.LogWarning($"{character.Name} has no active skill.");
                    continue;
                }

                var battleContext = new BattleContext(character, allyParty, opponentParty);
                await character.ActiveSkill.InvokeAsync(battleContext);
            }
        }
    }
}
