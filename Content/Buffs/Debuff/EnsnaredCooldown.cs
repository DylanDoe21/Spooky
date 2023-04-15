using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs.Debuff
{
	public class EnsnaredCooldown : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
		}
    }
}
