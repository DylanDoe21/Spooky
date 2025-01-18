using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs.WhipDebuff
{
	public class FleshWhipCooldown : ModBuff
	{
		public override string Texture => "Spooky/Content/Buffs/Debuff/DebuffPlaceholder";

		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
		}
    }
}
