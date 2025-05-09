using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Pets;
using Spooky.Content.Buffs.Pets;

namespace Spooky.Content.Items.Pets
{
	public class BrownieOrange : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Fish);
			Item.width = 18;
			Item.height = 16;
			Item.shoot = ModContent.ProjectileType<Columborange>();
			Item.buffType = ModContent.BuffType<ColumborangeBuff>();
		}

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
			if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 2, true);
            }
        }
	}
}