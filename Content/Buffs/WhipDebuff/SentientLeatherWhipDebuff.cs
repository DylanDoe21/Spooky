using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs.WhipDebuff
{
	public class SentientLeatherWhipDebuff : ModBuff
	{
		public override string Texture => "Spooky/Content/Buffs/Debuff/DebuffPlaceholder";
        public static readonly int tagDamage = 7;

		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
		}
    }
}
