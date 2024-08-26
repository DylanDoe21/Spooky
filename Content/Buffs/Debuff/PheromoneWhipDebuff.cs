using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs.Debuff
{
	public class PheromoneWhipDebuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
		}
    }
}
