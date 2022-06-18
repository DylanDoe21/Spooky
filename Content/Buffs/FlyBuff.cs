using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Buffs
{
	public class FlyBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fly Swarm");
			Description.SetDefault("The swarm of flies will protect you");
		}
    }
}
