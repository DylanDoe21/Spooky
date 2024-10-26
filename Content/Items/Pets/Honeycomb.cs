using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Pets;
using Spooky.Content.Buffs.Pets;

namespace Spooky.Content.Items.Pets
{
	public class Honeycomb : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Fish);
			Item.width = 26;
			Item.height = 30;
			Item.shoot = ModContent.ProjectileType<BeePet>();
			Item.buffType = ModContent.BuffType<BeePetBuff>();
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