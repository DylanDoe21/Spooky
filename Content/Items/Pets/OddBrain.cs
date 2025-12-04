using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Pets;
using Spooky.Content.Buffs.Pets;

namespace Spooky.Content.Items.Pets
{
	public class OddBrain : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Fish);
			Item.width = 28;
			Item.height = 24;
			Item.shoot = ModContent.ProjectileType<ZombieCultistPet>();
			Item.buffType = ModContent.BuffType<ZombieCultistPetBuff>();
		}

        public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
            {
                player.AddBuff(Item.buffType, 2, true);
            }

			return true;
        }

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position - new Vector2(0, 20), velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
	}
}