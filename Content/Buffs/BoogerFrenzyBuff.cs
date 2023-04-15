using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs
{
	public class BoogerFrenzyBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoSave[Type] = true;
		}
	}
}
