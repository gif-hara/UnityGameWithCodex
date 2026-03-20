using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

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
            [SerializeField] private ActiveSkill[] activeSkills;
            [SerializeField] private float[] coolTimes;
            [SerializeField, FormerlySerializedAs("activeSkill")] private ActiveSkill legacyActiveSkill;
            [SerializeField, FormerlySerializedAs("cooldown")] private float legacyCoolTime;

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
            public ActiveSkill[] ActiveSkills => activeSkills;
            public float[] CoolTimes => coolTimes;

            public void Initialize()
            {
                activeSkills ??= System.Array.Empty<ActiveSkill>();

                if (activeSkills.Length == 0 && legacyActiveSkill != null)
                {
                    activeSkills = new[] { legacyActiveSkill };
                }

                if (coolTimes == null || coolTimes.Length != activeSkills.Length)
                {
                    var resizedCoolTimes = new float[activeSkills.Length];
                    if (coolTimes != null)
                    {
                        var copyLength = Mathf.Min(coolTimes.Length, resizedCoolTimes.Length);
                        for (var index = 0; index < copyLength; index++)
                        {
                            resizedCoolTimes[index] = coolTimes[index];
                        }
                    }
                    else if (resizedCoolTimes.Length > 0)
                    {
                        resizedCoolTimes[0] = legacyCoolTime;
                    }

                    coolTimes = resizedCoolTimes;
                }

                legacyActiveSkill = null;
                legacyCoolTime = 0f;
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

            public void Initialize()
            {
                for (var index = 0; index < characters.Count; index++)
                {
                    characters[index].Initialize();
                }
            }
        }

        private readonly Party allies;
        private readonly Party enemies;

        public BattleSystem(Party allies, Party enemies)
        {
            this.allies = allies;
            this.enemies = enemies;
            allies.Initialize();
            enemies.Initialize();
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

                for (var skillIndex = 0; skillIndex < character.ActiveSkills.Length; skillIndex++)
                {
                    var activeSkill = character.ActiveSkills[skillIndex];
                    character.CoolTimes[skillIndex] += deltaTime * (1f + (character.Agility / 100.0f));

                    if (activeSkill == null)
                    {
                        Debug.LogWarning($"{character.Name} has no active skill.");
                        continue;
                    }

                    if (character.CoolTimes[skillIndex] < activeSkill.CoolTime)
                    {
                        continue;
                    }

                    character.CoolTimes[skillIndex] = 0f;

                    var battleContext = new BattleContext(character, allyParty, opponentParty);
                    await activeSkill.InvokeAsync(battleContext);
                }
            }
        }
    }
}
