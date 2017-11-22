using System.Collections.Generic;
using System.Linq;
using Huoyaoyuan.AdmiralRoom.API;
using Meowtrix.Linq;

namespace Huoyaoyuan.AdmiralRoom.Officer.Battle
{
    public class Attack
    {
        public ShipInBattle Friend { get; set; }
        public ShipInBattle Enemy { get; set; }
        public int FromHP { get; set; }
        public int ToHP { get; set; }
        public int MaxHP { get; set; }
        public LimitedValue ResultHP => new LimitedValue(ToHP, MaxHP);
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
        public void Apply(Battle battle = null)
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
            else if (battle != null)
            {
                if (Direction) battle.AnonymousFriendDamage += Damage;
                else battle.AnonymousEnemyDamage += Damage;
            }
            if (dest != null)
            {
                MaxHP = dest.MaxHP;
                FromHP = dest.ToHP;
                dest.Damage += Damage;
                dest.ToHP -= Damage;
                if (dest.ToHP <= 0)
                    if (dest.IsEnemy || dest.DamageControl == null)
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
        public Attack[] Attacks { get; private set; }
        public void ApplyAttacks(IEnumerable<Attack> attacks, Battle battle = null)
        {
            Attacks = attacks.Where(x => x.Damage >= 0).ToArray();
            foreach (var attack in Attacks)
                attack.Apply(battle);
        }
    }

    #region 航空戦
    public abstract class AerialBase : Stage
    {
        public virtual bool IsFullAerialStage => false;
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

            void ParseStage3(sortie_battle.airbattle.stage3 stage3, ShipInBattle[] friend, ShipInBattle[] enemy)
            {
                if (stage3 == null) return;
                if (stage3.api_fdam != null)
                    result = result.Concat(ParseAttack(stage3.api_fdam, stage3.api_frai_flag, stage3.api_fbak_flag, stage3.api_fcl_flag,
                        false, friend, enemytorpedo, enemybomb));
                if (stage3.api_edam != null)
                    result = result.Concat(ParseAttack(stage3.api_edam, stage3.api_erai_flag, stage3.api_ebak_flag, stage3.api_ecl_flag,
                        true, enemy, friendtorpedo, friendbomb));
            }

            ParseStage3(api.api_stage3, battle.Fleet1, battle.EnemyFleet);
            ParseStage3(api.api_stage3_combined, battle.Fleet2, battle.EnemyFleet2);

            ApplyAttacks(result, battle);
        }
        private static IEnumerable<Attack> ParseAttack(decimal[] damageList, int[] torpedoFlags, int[] bombFlags, int[] criticalList, bool direction,
            ShipInBattle[] fleet, ShipInBattle torpedoSource, ShipInBattle bombSource)
        {
            for (int i = 0; i < damageList.Length; i++)
            {
                ShipInBattle source = null;
                if (torpedoSource != null && torpedoSource == bombSource) source = torpedoSource;
                else
                {
                    // TODO: api bug
                    bool torpedo = i < torpedoFlags.Length && torpedoFlags[i] != 0,
                        bomb = i < bombFlags.Length && bombFlags[i] != 0;
                    if (torpedo && bomb) source = null;
                    else if (torpedo) source = torpedoSource;
                    else if (bomb) source = bombSource;
                    else if (damageList[i] == 0) continue;
                }
                var damage = Attack.ParseDamage(damageList[i]);
                (var friend, var enemy) = direction ? (source, fleet[i]) : (fleet[i], source);
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
        public override bool IsFullAerialStage => true;
        public bool IsFaked => FriendStage1.Max == 0 && EnemyStage1.Max == 0;
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
            if (!isSupport)
            {
                if (api.api_plane_from[0]?.Length == 1 && api.api_plane_from[0][0] > 0)
                    friendjet = battle.Fleet1[api.api_plane_from[0][0]];
                if (api.api_plane_from.Length >= 2 && api.api_plane_from[1]?.Length == 1 && api.api_plane_from[1][0] > 0)
                    enemyjet = battle.EnemyFleet[api.api_plane_from[1][0]];
            }
            ParseAttacks(battle, api, null, friendjet, null, enemyjet);
        }
    }
    public class AirBaseAttack : AerialBase
    {
        public class Squadron
        {
            public EquipInfo Plane { get; set; }
            public int Count { get; set; }
        }
        /// <summary>
        /// not works
        /// </summary>
        public Squadron[] SquadronList { get; }
        public AirBaseAttack(Battle battle, sortie_battle.airbattle api) : base(api)
        {
            //SquadronList = api.api_squadron_plane.Select(x => new Squadron
            //{
            //    Plane = Staff.Current.MasterData.EquipInfo[x.api_mst_id],
            //    Count = x.api_count
            //}).ToArray();
            if (api.api_stage1 != null)
                AirControl = (AirControl)api.api_stage1.api_disp_seiku;

            ParseAttacks(battle, api, null, null, null, null);
        }
    }
    #endregion

    #region 砲雷撃戦
    public class FireCombat : Stage
    {
        public FireCombat(Battle battle, sortie_battle.fire api)
        {
            var result = new List<Attack>();
            for (int i = 0; i < api.api_df_list.Length; i++)
            {
                int sourceidx = api.api_at_list[i];
                bool direction = api.api_at_eflag[i] == 0;
                var source = direction ?
                    battle.FindFriend(sourceidx) :
                    battle.FindEnemy(sourceidx);
                for (int j = 0; j < api.api_df_list[i].Length; j++)
                {
                    int destidx = api.api_df_list[i][j];
                    var dest = direction ?
                        battle.FindEnemy(destidx) :
                        battle.FindFriend(destidx);
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
            ApplyAttacks(result);
        }
    }
    public class TorpedoCombat : Stage
    {
        public TorpedoCombat(Battle battle, sortie_battle.torpedo api)
        {
            var result = new List<Attack>();
            for (int i = 0; i < api.api_fydam.Length; i++)
            {
                int target = api.api_frai[i];
                if (target == -1) continue;
                var damage = Attack.ParseDamage(api.api_fydam[i]);
                result.Add(new Attack
                {
                    Friend = battle.FindFriend(i),
                    Enemy = battle.FindEnemy(target),
                    Direction = true,
                    Damage = damage.damage,
                    Shield = damage.shield,
                    IsCritical = api.api_fcl[i] == 2
                });
            }
            for (int i = 0; i < api.api_eydam.Length; i++)
            {
                int target = api.api_erai[i];
                if (target == -1) continue;
                var damage = Attack.ParseDamage(api.api_eydam[i]);
                result.Add(new Attack
                {
                    Friend = battle.FindFriend(target),
                    Enemy = battle.FindEnemy(i),
                    Direction = false,
                    Damage = damage.damage,
                    Shield = damage.shield,
                    IsCritical = api.api_ecl[i] == 2
                });
            }
            ApplyAttacks(result);
        }
    }
    public class NightCombat : FireCombat
    {
        public EquipInfo FriendTouch { get; }
        public EquipInfo EnemyTouch { get; }
        public ShipInBattle FriendFlare { get; }
        public ShipInBattle EnemyFlare { get; }
        public ShipInBattle FriendLight { get; }
        public ShipInBattle EnemyLight { get; }
        public FireCombat Combat { get; }
        public NightCombat(Battle battle, sortie_battle api, ShipInBattle[] friends, ShipInBattle[] enemies)
            : base(battle, api.api_hougeki)
        {
            if (api.api_touch_plane != null)
            {
                FriendTouch = Staff.Current.MasterData.EquipInfo[api.api_touch_plane[0]];
                EnemyTouch = Staff.Current.MasterData.EquipInfo[api.api_touch_plane[1]];
            }
            if (api.api_flare_pos != null)
            {
                int pos = api.api_flare_pos[0];
                if (pos >= 1 && pos <= 6) FriendFlare = friends[pos - 1];
                pos = api.api_flare_pos[1];
                if (pos >= 1 && pos <= 6) EnemyFlare = enemies[pos - 1];
            }
            foreach (var ship in friends)
            {
                if (FriendLight != null) break;
                if (ship.IsEscaped) continue;
                foreach (var equip in ship.Equipments)
                {
                    int id = equip.EquipInfo.EquipType.Id;
                    if (id == 29 || id == 42)
                    {
                        FriendLight = ship;
                        break;
                    }
                }
            }
            foreach (var ship in enemies)
            {
                if (EnemyLight != null) break;
                foreach (var equip in ship.Equipments)
                {
                    int id = equip.EquipInfo.EquipType.Id;
                    if (id == 29 || id == 42)
                    {
                        EnemyLight = ship;
                        break;
                    }
                }
            }
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
            if (Type == SupportType.Aerial || Type == SupportType.ASW)
                Aerial = new AerialSupport(battle, api.api_support_airatack);
            else
            {
                if (api.api_support_hourai == null) return;
                var result = new List<Attack>();
                for (int i = 0; i < api.api_support_hourai.api_damage.Length; i++)
                {
                    var damage = Attack.ParseDamage(api.api_support_hourai.api_damage[i]);
                    if (damage.damage == 0 && !damage.shield) continue;
                    result.Add(new Attack
                    {
                        Friend = null,
                        Enemy = battle.FindEnemy(i),
                        Direction = true,
                        Damage = damage.damage,
                        Shield = damage.shield,
                        IsCritical = api.api_support_hourai.api_cl_list[i] == 2
                    });
                }
                ApplyAttacks(result);
            }
        }
    }
    #endregion
    public enum SupportType { None = 0, Aerial = 1, Fire = 2, Torpedo = 3, ASW = 4 }
}
