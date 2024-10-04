using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs.Debuff
{
	public class StitchedCloakCooldown : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;  
            Main.pvpBuff[Type] = true;  
            Main.buffNoSave[Type] = true;
			BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
	}
}
