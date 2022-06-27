using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs
{
	public class CandyBuff1 : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Summoner Candy");
			Description.SetDefault("5% increased summon damage");
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetDamage<SummonDamageClass>() += 0.05f;
		}
	}
}
