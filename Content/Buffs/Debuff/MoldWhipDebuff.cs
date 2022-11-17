using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs.Debuff
{
	public class MoldWhipDebuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Moldy");
			BuffID.Sets.IsAnNPCWhipDebuff[Type] = true;
		} 
    }
}
