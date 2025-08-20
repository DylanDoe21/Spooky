using Terraria;
using Terraria.ModLoader;

using Spooky.Content.Dusts;

namespace Spooky.Content.Buffs
{
	public class BustlingGlowshroomHeal : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			//increase life regen
			player.lifeRegen += 4;

			if (Main.rand.NextBool(10))
			{
				Dust.NewDust(player.position, (player.width / 2) + Main.rand.Next(-20, 20), 
				(player.height / 2) + Main.rand.Next(-20, 20), ModContent.DustType<GlowshroomHealDust>());
			}
		}
	}
}
