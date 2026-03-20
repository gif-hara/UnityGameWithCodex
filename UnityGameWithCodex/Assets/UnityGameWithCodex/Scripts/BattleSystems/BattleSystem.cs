using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UnityGameWithCodex.BattleControllers
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

                    coolTimes = resizedCoolTimes;
                }
            }

            public void TakeDamage(int damage)
            {
                hp = Mathf.Max(0, hp - Mathf.Max(0, damage));
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

                    foreach (var character in characters)
                    {
                        if (!character.IsDead)
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }

            public void Initialize()
            {
                foreach (var character in characters)
                {
                    character.Initialize();
                }
            }
        }

        private readonly Party playerParty;
        private readonly Party enemyParty;

        public BattleSystem(Party playerParty, Party enemyParty)
        {
            this.playerParty = playerParty;
            this.enemyParty = enemyParty;
            playerParty.Initialize();
            enemyParty.Initialize();
        }

        public async UniTask BeginAsync(CancellationToken cancellationToken)
        {
            while (!playerParty.IsAllDead && !enemyParty.IsAllDead)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var deltaTime = Time.deltaTime;
                await TickCharactersAsync(playerParty, enemyParty, deltaTime, cancellationToken);
                await TickCharactersAsync(enemyParty, playerParty, deltaTime, cancellationToken);
                await UniTask.NextFrame(cancellationToken);
            }
        }

        private static async UniTask TickCharactersAsync(Party allyParty, Party opponentParty, float deltaTime, CancellationToken cancellationToken)
        {
            foreach (var character in allyParty.Characters)
            {
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
                    await activeSkill.InvokeAsync(battleContext, cancellationToken);
                }
            }
        }
    }
}
