using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs
{
	public class BoogerFrenzyBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Booger Frenzy");
			// Description.SetDefault("Your weapons have been blessed with boogers");
			Main.buffNoSave[Type] = true;
		}
	}
}
