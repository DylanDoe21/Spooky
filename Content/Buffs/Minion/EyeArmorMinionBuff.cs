using Terraria;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Buffs.Minion
{
	public class EyeArmorMinionBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (player.GetModPlayer<SpookyPlayer>().EyeArmorSet)
			{
				player.buffTime[buffIndex] = 2;
			}
			else
			{
				player.buffTime[buffIndex] = 0;
			}
		}
	}
}
