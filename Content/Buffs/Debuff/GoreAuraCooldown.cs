using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs.Debuff
{
	public class GoreAuraCooldown : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;  
            Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			Main.persistentBuff[Type] = true;
		}
    }
}
