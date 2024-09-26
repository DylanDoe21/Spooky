using Terraria;
using Terraria.ModLoader;

using Spooky.Content.Dusts;

namespace Spooky.Content.Buffs
{
	public class StrawberryBoostBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetDamage(DamageClass.Generic) += 0.1f;
			player.GetCritChance(DamageClass.Generic) += 10;
            player.GetKnockback(DamageClass.Generic) += 1.5f;
		}
	}
}
