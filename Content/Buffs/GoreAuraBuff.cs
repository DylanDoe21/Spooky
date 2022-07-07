using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs
{
	public class GoreAuraBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gore Monger Aura");
			Description.SetDefault("Your aura is protecting you");
			Main.buffNoSave[Type] = true;
		}
	}
}
