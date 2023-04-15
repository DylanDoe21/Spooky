using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs
{
	public class EmbryoRevival : ModBuff
	{
		public override void Update(Player player, ref int buffIndex)
		{
			player.lifeRegen += 80;
			player.statDefense += 15;
		}
    }
}
