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
    }
}
