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
        private bool storedDontTakeDamage;
        private bool storedImmortal;

		public override void Update(NPC npc, ref int buffIndex)
        {
            if (!initializeStats)
            {
                storedDontTakeDamage = npc.dontTakeDamage;
                storedImmortal = npc.immortal;
				npc.dontTakeDamage = true;
                npc.immortal = true;

                initializeStats = true;
			}

			if (npc.buffTime[buffIndex] < 5)
            {
                npc.dontTakeDamage = storedDontTakeDamage;
                npc.immortal = storedImmortal;
            }
		}
    }
}
