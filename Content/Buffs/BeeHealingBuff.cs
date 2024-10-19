using Terraria;
using Terraria.ModLoader;

using Spooky.Content.Dusts;

namespace Spooky.Content.Buffs
{
	public class BeeHealingBuff : ModBuff
	{
		public override void Update(NPC npc, ref int buffIndex)
        {
            npc.lifeRegen += 50;

			if (Main.rand.NextBool(10))
			{
				Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<BeeHealingDust>());
			}
		}
    }
}
