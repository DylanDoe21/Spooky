using Terraria;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Buffs.Debuff
{
	public class CatacombDebuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Catacomb's Curse");
			Description.SetDefault("You cannot build, break blocks, or use the rod of discord");
			Main.debuff[Type] = true;  
            Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
		}
    }
}
