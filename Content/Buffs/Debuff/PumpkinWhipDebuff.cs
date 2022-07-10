using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs.Debuff
{
	public class PumpkinWhipDebuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fly Infestation");
			Main.debuff[Type] = true;
		} 
    }
}
