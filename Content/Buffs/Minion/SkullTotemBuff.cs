using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs.Minion
{
	public class SkullTotemBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetDamage(DamageClass.Summon) += 0.12f;
			player.GetDamage(DamageClass.SummonMeleeSpeed) += 0.12f;
			player.whipRangeMultiplier += 0.5f;
			player.maxMinions += 2;
		}
	}
}
