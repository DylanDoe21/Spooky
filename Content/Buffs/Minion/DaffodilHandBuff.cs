using Terraria;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.Catacomb;

namespace Spooky.Content.Buffs.Minion
{
	public class DaffodilHandBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (player.ownedProjectileCounts[ModContent.ProjectileType<DaffodilHandMinion>()] > 0) 
			{
				player.buffTime[buffIndex] = 18000;
			}
			else
			{
				player.DelBuff(buffIndex);
				buffIndex--;
			}
		}
	}
}
