using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs
{
	public class CandyBuff3 : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.whipRangeMultiplier = 1.3f;
		}
    }
}
