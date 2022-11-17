using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs
{
	public class SkullTotemBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Skull Idol's Power");
			Description.SetDefault("12% increased summon damage\n5% increased whip range\n+2 maximum minions");
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetDamage(DamageClass.Summon) += 0.12f;
			player.whipRangeMultiplier = 1.5f;
			player.maxMinions += 2;
		}
	}
}
