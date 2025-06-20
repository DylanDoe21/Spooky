using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Buffs
{
	public class EggEventEnemyBuff : ModBuff
	{
		public override string Texture => "Spooky/Content/Buffs/Debuff/DebuffPlaceholder";

		private bool initializeStats;
        private int storedDefense;
        private int storedDamage;

		public override void Update(NPC npc, ref int buffIndex)
        {
            if (!initializeStats)
            {
                storedDamage = npc.damage;
				npc.damage = storedDamage * 2;

                initializeStats = true;
			}

            if (npc.buffTime[buffIndex] < 5)
            {
                npc.damage = storedDamage;
            }
		}
    }
}
