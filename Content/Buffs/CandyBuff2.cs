using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs
{
	public class CandyBuff2 : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Summoner Candy");
			Description.SetDefault("1 extra minion slot");
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.maxMinions += 1;
		}
    }
}
