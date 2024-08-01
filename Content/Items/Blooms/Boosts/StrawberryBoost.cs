using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Buffs;

namespace Spooky.Content.Items.Blooms.Boosts
{
	//TODO: change this to a projectile that can be collided with (also so i dont have to have a bloat folder in the blooms folder)
	public class StrawberryBoost : ModItem
	{
		public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            ItemID.Sets.ItemIconPulse[Item.type] = true;
		}
		
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 38;
			Item.maxStack = 1;
		}

		public override bool ItemSpace(Player player)
		{
			return true;
		}

		public override bool OnPickup(Player player)
		{
			player.AddBuff(ModContent.BuffType<StrawberryBoostBuff>(), 600);
			
			return false;
		}
	}
}