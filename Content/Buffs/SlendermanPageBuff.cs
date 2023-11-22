using Terraria;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Buffs;

namespace Spooky.Content.Buffs
{
	public class SlendermanPageBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			//set page delay so pages cant drop for another 30 seconds
            player.GetModPlayer<SpookyPlayer>().SlendermanPageDelay = 1800;
		}
	}
}
