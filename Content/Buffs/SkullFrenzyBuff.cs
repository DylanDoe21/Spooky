using Terraria;
using Terraria.ModLoader;

using Spooky.Content.Dusts;

namespace Spooky.Content.Buffs
{
	public class SkullFrenzyBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetDamage(DamageClass.Generic) += 0.15f;
			player.GetCritChance(DamageClass.Generic) += 15;
            player.GetKnockback(DamageClass.Generic) += 1.5f;
            player.moveSpeed += 0.15f;
		}
	}
}
