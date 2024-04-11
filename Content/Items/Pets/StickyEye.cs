using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Buffs.Pets;
using Spooky.Content.Projectiles.Pets;

namespace Spooky.Content.Items.Pets
{
	public class StickyEye : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Fish);
			Item.width = 32;
			Item.height = 34;
			Item.shoot = ModContent.ProjectileType<StickyEyePet>();
			Item.buffType = ModContent.BuffType<StickyEyeBuff>();
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