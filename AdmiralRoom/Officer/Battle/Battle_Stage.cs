using System.Collections.Generic;
using System.Linq;
using Huoyaoyuan.AdmiralRoom.API;
using Meowtrix.Linq;

namespace Huoyaoyuan.AdmiralRoom.Officer.Battle
{
    public partial class Battle
    {
        public class Attack
        {
            public ShipInBattle Friend { get; set; }
            public ShipInBattle Enemy { get; set; }
            public int FromHP { get; set; }
            public int ToHP { get; set; }
            public int Damage { get; set; }
            public bool IsCritical { get; set; }
            /// <summary>
            /// <para>
            /// true: Friend-&gt;Enemy
            /// </para>
            /// <para>
            /// false: Friend&lt;-Enemy
            /// </para>
            /// </summary>
            public bool Direction { get; set; }
            public bool Shield { get; set; }
            private bool applied;
            public void Apply(Battle battle)
            {
                if (applied) return;
                applied = true;
                ShipInBattle source, dest;
                if (Direction)
                {
                    source = Friend;
                    dest = Enemy;
                }
                else
                {
                    source = Enemy;
                    dest = Friend;
                }
                if (source != null)
                    source.DamageGiven += Damage;
                else if (Direction) battle.AnonymousFriendDamage += Damage;
                else battle.AnonymousEnemyDamage += Damage;
                if (dest != null)
                {
                    FromHP = dest.ToHP;
                    dest.Damage += Damage;
                    dest.ToHP -= Damage;
                    if (dest.ToHP <= 0)
                        if (dest.DamageControl == null)
                            dest.ToHP = 0;
                        else if (dest.DamageControl.Id == 42)//応急修理要員
                            dest.ToHP = (int)(dest.MaxHP * 0.2);
                        else if (dest.DamageControl.Id == 43)//応急修理女神
                            dest.ToHP = dest.MaxHP;
                    ToHP = dest.ToHP;
                }
            }
            public static (int damage, bool shield) ParseDamage(decimal damage) => ((int)damage, damage - (int)damage > 0.05m);
        }
        public abstract class Stage
        {
            public Attack[] Attacks { get; protected set; }
            protected void Apply(Battle battle)
            {
                foreach (var attack in Attacks)
                    attack.Apply(battle);
            }
        }
        public class AerialCombat : Stage
        {
            public AirControl AirControl { get; }
            public LimitedValue FriendStage1 { get; }
            public LimitedValue EnemyStage1 { get; }
            public LimitedValue FriendStage2 { get; }
            public LimitedValue EnemyStage2 { get; }
            public AerialCombat(Battle battle, sortie_battle.airbattle api, bool isSupport)
            {
                if (api.api_stage1 != null)
                {
                    AirControl = (AirControl)api.api_stage1.api_disp_seiku;
                    FriendStage1 = new LimitedValue(api.api_stage1.api_f_count - api.api_stage1.api_f_lostcount, api.api_stage1.api_f_count);
                    EnemyStage1 = new LimitedValue(api.api_stage1.api_e_count - api.api_stage1.api_e_lostcount, api.api_stage1.api_e_count);
                }
                if (api.api_stage2 != null)
                {
                    FriendStage2 = new LimitedValue(api.api_stage2.api_f_count - api.api_stage2.api_f_lostcount, api.api_stage2.api_f_count);
                    EnemyStage2 = new LimitedValue(api.api_stage2.api_e_count - api.api_stage2.api_e_lostcount, api.api_stage2.api_e_count);
                }
                ShipInBattle friendtorpedo = null, friendbomb = null, enemytorpedo = null, enemybomb = null;
                if (!isSupport)
                {
                    friendtorpedo = battle.Fleet1.Where(x => x.CanAerialTorpedo).TakeIfSingle();
                    friendbomb = battle.Fleet1.Where(x => x.CanAerialBomb).TakeIfSingle();
                }
                enemytorpedo = battle.EnemyFleet.Where(x => x.CanAerialTorpedo).TakeIfSingle();
                enemybomb = battle.EnemyFleet.Where(x => x.CanAerialBomb).TakeIfSingle();

                var result = Enumerable.Empty<Attack>();

                void ParseStage3(sortie_battle.airbattle.stage3 stage3)
                {
                    if (stage3 == null) return;
                    if (stage3.api_fdam != null)
                        result.Concat(ParseAttack(stage3.api_fdam, stage3.api_frai_flag, stage3.api_fbak_flag, stage3.api_fcl_flag,
                            false, battle.Fleet1, enemytorpedo, enemybomb));
                    if (stage3.api_edam != null)
                        result.Concat(ParseAttack(stage3.api_edam, stage3.api_erai_flag, stage3.api_ebak_flag, stage3.api_ecl_flag,
                            true, battle.EnemyFleet, friendtorpedo, friendbomb));
                }

                ParseStage3(api.api_stage3);
                ParseStage3(api.api_stage3_combined);

                Attacks = result.ToArray();
            }
            private static IEnumerable<Attack> ParseAttack(decimal[] damageList, int[] torpedoFlags, int[] bombFlags, int[] criticalList, bool direction,
                ShipInBattle[] fleet, ShipInBattle torpedoSource, ShipInBattle bombSource)
            {
                for (int i = 1; i < damageList.Length; i++)
                {
                    ShipInBattle source;
                    bool torpedo = torpedoFlags[i] != 0, bomb = bombFlags[i] != 0;
                    if (torpedo && bomb) source = null;
                    else if (torpedo) source = torpedoSource;
                    else if (bomb) source = bombSource;
                    else continue;
                    var damage = Attack.ParseDamage(damageList[i]);
                    (var friend, var enemy) = direction ? (source, fleet[i - 1]) : (fleet[i - 1], source);
                    yield return new Attack
                    {
                        Friend = friend,
                        Enemy = enemy,
                        Direction = direction,
                        IsCritical = criticalList[i] != 0,
                        Damage = damage.damage,
                        Shield = damage.shield
                    };
                }
            }
        }
    }
}
