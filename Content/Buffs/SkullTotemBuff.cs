using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs
{
	public class SkullTotemBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Skull Idol's Power");
			Description.SetDefault("Grants various summoner stat increases");
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
