using Terraria;
using Terraria.ModLoader;

using Spooky.Content.Dusts;

namespace Spooky.Content.Buffs
{
	public class BeeDamageBuff : ModBuff
	{
        private bool initializeStats;
        private int storedDamage;

		public override void Update(NPC npc, ref int buffIndex)
        {
            if (!initializeStats)
            {
                storedDamage = npc.damage;
				npc.damage = (int)(npc.damage * 1.75f);

                initializeStats = true;
			}

			if (npc.buffTime[buffIndex] < 5)
            {
                npc.damage = storedDamage;
                initializeStats = false;
                npc.buffTime[buffIndex] = 0;
            }

            if (Main.rand.NextBool(10))
			{
				Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<BeeDamageDust>());
			}
		}
    }
}
