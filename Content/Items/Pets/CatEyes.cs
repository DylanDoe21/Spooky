using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Pets;
using Spooky.Content.Buffs.Pets;

namespace Spooky.Content.Items.Pets
{
	public class CatEyes : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Fish);
			Item.width = 28;
			Item.height = 20;
			Item.shoot = ModContent.ProjectileType<CatPet>();
			Item.buffType = ModContent.BuffType<CatBuff>();
		}

        public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
            {
                player.AddBuff(Item.buffType, 2, true);
            }

			return true;
        }
	}
}