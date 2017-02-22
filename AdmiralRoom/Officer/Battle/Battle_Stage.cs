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
            protected static ShipInBattle FindShip(int index, ShipInBattle[] fleet1, ShipInBattle[] fleet2)
                => index <= 6 ? fleet1[index - 1] : fleet2[index - 7];
        }

        #region 航空戦
        public abstract class AerialBase : Stage
        {
            public AirControl AirControl { get; protected set; }
            public LimitedValue FriendStage1 { get; protected set; }
            public LimitedValue EnemyStage1 { get; protected set; }
            public LimitedValue FriendStage2 { get; protected set; }
            public LimitedValue EnemyStage2 { get; protected set; }
            protected AerialBase(sortie_battle.airbattle api)
            {
                if (api.api_stage1 != null)
                {
                    FriendStage1 = new LimitedValue(api.api_stage1.api_f_count - api.api_stage1.api_f_lostcount, api.api_stage1.api_f_count);
                    EnemyStage1 = new LimitedValue(api.api_stage1.api_e_count - api.api_stage1.api_e_lostcount, api.api_stage1.api_e_count);
                }
                if (api.api_stage2 != null)
                {
                    FriendStage2 = new LimitedValue(api.api_stage2.api_f_count - api.api_stage2.api_f_lostcount, api.api_stage2.api_f_count);
                    EnemyStage2 = new LimitedValue(api.api_stage2.api_e_count - api.api_stage2.api_e_lostcount, api.api_stage2.api_e_count);
                }
            }
            protected void ParseAttacks(Battle battle, sortie_battle.airbattle api, ShipInBattle friendtorpedo, ShipInBattle friendbomb, ShipInBattle enemytorpedo, ShipInBattle enemybomb)
            {
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
                        IsCritical = criticalList[i] == 2,
                        Damage = damage.damage,
                        Shield = damage.shield
                    };
                }
            }
        }
        public class AerialCombat : AerialBase
        {
            public EquipInfo FriendTouch { get; }
            public EquipInfo EnemyTouch { get; }
            public class AntiAirCutin
            {
                public ShipInBattle Ship { get; }
                public int Type { get; }
                public EquipInfo[] EquipList { get; }
                public AntiAirCutin(Battle battle, sortie_battle.airbattle.stage2.anti_air_cutin api)
                {
                    if (api.api_idx < 6) Ship = battle.Fleet1[api.api_idx];
                    else Ship = battle.Fleet2[api.api_idx - 6];
                    Type = api.api_kind;
                    EquipList = api.api_use_items.Select(x => Staff.Current.MasterData.EquipInfo[x]).ToArray();
                }
            }
            public AntiAirCutin AntiAir { get; }
            public AerialCombat(Battle battle, sortie_battle.airbattle api) : base(api)
            {
                if (api.api_stage1 != null)
                {
                    AirControl = (AirControl)api.api_stage1.api_disp_seiku;
                    FriendTouch = Staff.Current.MasterData.EquipInfo[api.api_stage1.api_touch_plane[0]];
                    EnemyTouch = Staff.Current.MasterData.EquipInfo[api.api_stage1.api_touch_plane[1]];
                }

                if (api.api_stage2?.api_air_fire != null)
                    AntiAir = new AntiAirCutin(battle, api.api_stage2.api_air_fire);

                ShipInBattle friendtorpedo = null, friendbomb = null, enemytorpedo = null, enemybomb = null;
                friendtorpedo = battle.Fleet1.Where(x => x.CanAerialTorpedo).TakeIfSingle();
                friendbomb = battle.Fleet1.Where(x => x.CanAerialBomb).TakeIfSingle();
                enemytorpedo = battle.EnemyFleet.Where(x => x.CanAerialTorpedo).TakeIfSingle();
                enemybomb = battle.EnemyFleet.Where(x => x.CanAerialBomb).TakeIfSingle();

                ParseAttacks(battle, api, friendtorpedo, friendbomb, enemytorpedo, enemybomb);
            }
        }
        public class AerialSupport : AerialBase
        {
            public AerialSupport(Battle battle, sortie_battle.airbattle api) : base(api)
            {
                if (api.api_stage1 != null)
                    AirControl = (AirControl)api.api_stage1.api_disp_seiku;

                ShipInBattle enemytorpedo = null, enemybomb = null;
                enemytorpedo = battle.EnemyFleet.Where(x => x.CanAerialTorpedo).TakeIfSingle();
                enemybomb = battle.EnemyFleet.Where(x => x.CanAerialBomb).TakeIfSingle();

                ParseAttacks(battle, api, null, null, enemytorpedo, enemybomb);
            }
        }
        public class JetPlaneAttack : AerialBase
        {
            public JetPlaneAttack(Battle battle, sortie_battle.airbattle api, bool isSupport) : base(api)
            {
                ShipInBattle friendjet = null, enemyjet = null;
                if (api.api_plane_from[0]?.Length == 1 && api.api_plane_from[0][0] > 0)
                    friendjet = battle.Fleet1[api.api_plane_from[0][0] - 1];
                if (api.api_plane_from.Length >= 2 && api.api_plane_from[1]?.Length == 1 && api.api_plane_from[1][0] > 0)
                    enemyjet = battle.EnemyFleet[api.api_plane_from[1][0] - 1];

                ParseAttacks(battle, api, null, friendjet, null, enemyjet);
            }
        }
        public class AirBaseAttack : AerialBase
        {
            sortie_battle.airbattle.squadron[] SquadronList { get; }
            public AirBaseAttack(Battle battle, sortie_battle.airbattle api) : base(api)
            {
                SquadronList = api.api_squadron_plane;
                if (api.api_stage1 != null)
                    AirControl = (AirControl)api.api_stage1.api_disp_seiku;

                ParseAttacks(battle, api, null, null, null, null);
            }
        }
        #endregion

        #region 砲雷撃戦
        public class FireCombat : Stage
        {
            public FireCombat(sortie_battle.fire api, ShipInBattle[] friends, ShipInBattle[] enemies)
            {
                var result = new List<Attack>();
                for (int i = 0; i < api.api_df_list.Length; i++)
                {
                    int sourceidx = api.api_at_list[i + 1];
                    bool direction = sourceidx <= 6;
                    var source = FindShip(sourceidx, friends, enemies);
                    for (int j = 0; j < api.api_df_list[i].Length; j++)
                    {
                        int destidx = api.api_df_list[i][0];
                        var dest = FindShip(destidx, friends, enemies);
                        var damage = Attack.ParseDamage(api.api_damage[i][j]);
                        (var friend, var enemy) = direction ? (source, dest) : (dest, source);
                        result.Add(new Attack
                        {
                            Friend = friend,
                            Enemy = enemy,
                            Direction = direction,
                            Damage = damage.damage,
                            IsCritical = api.api_cl_list[i][j] == 2,
                            Shield = damage.shield
                        });
                    }
                }
                Attacks = result.ToArray();
            }
        }
        public class ECFireCombat : Stage
        {
            public ECFireCombat(Battle battle, sortie_battle.fire api)
            {
                var result = new List<Attack>();
                for (int i = 0; i < api.api_df_list.Length; i++)
                {
                    int sourceidx = api.api_at_list[i + 1];
                    bool direction = api.api_at_eflag[i + 1] == 0;
                    var source = direction ?
                        FindShip(sourceidx, battle.Fleet1, battle.Fleet2) :
                        FindShip(sourceidx, battle.EnemyFleet, battle.EnemyFleet2);
                    for (int j = 0; j < api.api_df_list[i].Length; j++)
                    {
                        int destidx = api.api_df_list[i][j];
                        var dest = direction ?
                            FindShip(destidx, battle.Fleet1, battle.Fleet2) :
                            FindShip(destidx, battle.EnemyFleet, battle.EnemyFleet2);
                        var damage = Attack.ParseDamage(api.api_damage[i][j]);
                        (var friend, var enemy) = direction ? (source, dest) : (dest, source);
                        result.Add(new Attack
                        {
                            Friend = friend,
                            Enemy = enemy,
                            Direction = direction,
                            Damage = damage.damage,
                            IsCritical = api.api_cl_list[i][j] == 2,
                            Shield = damage.shield
                        });
                    }
                }
                Attacks = result.ToArray();
            }
        }
        public class TorpedoCombat : Stage
        {
            public TorpedoCombat(sortie_battle.torpedo api, ShipInBattle[] friends, ShipInBattle[] enemies)
            {
                var result = new List<Attack>();
                for (int i = 1; i < api.api_fydam.Length; i++)
                {
                    int target = api.api_frai[i - 1];
                    if (target == 0) continue;
                    var damage = Attack.ParseDamage(api.api_fydam[i]);
                    result.Add(new Attack
                    {
                        Friend = friends[i - 1],
                        Enemy = enemies[target - 1],
                        Direction = true,
                        Damage = damage.damage,
                        Shield = damage.shield,
                        IsCritical = api.api_fcl[i - 1] == 2
                    });
                }
                for (int i = 1; i < api.api_eydam.Length; i++)
                {
                    int target = api.api_erai[i - 1];
                    if (target == 0) continue;
                    var damage = Attack.ParseDamage(api.api_eydam[i]);
                    result.Add(new Attack
                    {
                        Friend = friends[target - 1],
                        Enemy = enemies[i - 1],
                        Direction = false,
                        Damage = damage.damage,
                        Shield = damage.shield,
                        IsCritical = api.api_ecl[i - 1] == 2
                    });
                }
                Attacks = result.ToArray();
            }
        }
        public class ECTorpedoCombat : Stage
        {
            public ECTorpedoCombat(Battle battle, sortie_battle.torpedo api)
            {
                var result = new List<Attack>();
                for (int i = 1; i < api.api_fydam.Length; i++)
                {
                    int target = api.api_frai[i - 1];
                    if (target == 0) continue;
                    var damage = Attack.ParseDamage(api.api_fydam[i]);
                    result.Add(new Attack
                    {
                        Friend = FindShip(i, battle.Fleet1, battle.Fleet2),
                        Enemy = FindShip(target, battle.EnemyFleet, battle.EnemyFleet2),
                        Direction = true,
                        Damage = damage.damage,
                        Shield = damage.shield,
                        IsCritical = api.api_fcl[i - 1] == 2
                    });
                }
                for (int i = 1; i < api.api_eydam.Length; i++)
                {
                    int target = api.api_erai[i - 1];
                    if (target == 0) continue;
                    var damage = Attack.ParseDamage(api.api_eydam[i]);
                    result.Add(new Attack
                    {
                        Friend = FindShip(target, battle.Fleet1, battle.Fleet2),
                        Enemy = FindShip(i, battle.EnemyFleet, battle.EnemyFleet2),
                        Direction = false,
                        Damage = damage.damage,
                        Shield = damage.shield,
                        IsCritical = api.api_ecl[i - 1] == 2
                    });
                }
                Attacks = result.ToArray();
            }
        }
        #endregion

        #region 支援艦隊
        public class SupportAttack : Stage
        {
            public SupportType Type { get; }
            public AerialSupport Aerial { get; }
            public SupportAttack(Battle battle, sortie_battle.support api, int type)
            {
                Type = (SupportType)type;
                if (Type == SupportType.Aerial)
                    Aerial = new AerialSupport(battle, api.api_support_airatack);
                else
                {
                    var result = new List<Attack>();
                    for (int i = 1; i < api.api_support_hourai.api_damage.Length; i++)
                    {
                        var damage = Attack.ParseDamage(api.api_support_hourai.api_damage[i]);
                        result.Add(new Attack
                        {
                            Friend = null,
                            Enemy = FindShip(i, battle.EnemyFleet, battle.EnemyFleet2),
                            Direction = true,
                            Damage = damage.damage,
                            Shield = damage.shield,
                            IsCritical = api.api_support_hourai.api_cl_list[i] == 2
                        });
                    }
                    Attacks = result.ToArray();
                }
            }
        }
        #endregion
    }
    public enum SupportType { None = 0, Aerial = 1, Fire = 2, Torpedo = 3 }
}
