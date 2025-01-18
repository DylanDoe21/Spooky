using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs.WhipDebuff
{
	public class EggplantWhipDebuff : ModBuff
	{
		public override string Texture => "Spooky/Content/Buffs/Debuff/DebuffPlaceholder";

		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
		}
    }
}
