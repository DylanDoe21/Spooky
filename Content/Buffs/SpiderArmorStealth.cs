using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs
{
	public class SpiderArmorStealth : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.endurance += 0.12f;
			player.aggro -= 200;
		}
    }
}
