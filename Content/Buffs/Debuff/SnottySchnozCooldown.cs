using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Buffs.Debuff
{
	public class SnottySchnozCooldown : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;  
            Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			Main.persistentBuff[Type] = true;
			BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
		}
		
		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<SpookyPlayer>().MocoBoogerCharge = 0;
		}
    }
}
