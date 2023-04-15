using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs
{
	public class FlyBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoSave[Type] = true;
		}
    }
}
