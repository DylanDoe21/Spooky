using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Buffs
{
	public class EmbryoRevival : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Embryotic Revival");
			Description.SetDefault("The embyro has saved you from death"
			+ "\nMajorly increases defense and life regeneration");
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.lifeRegen += 80;
			player.statDefense += 15;
		}
    }
}
