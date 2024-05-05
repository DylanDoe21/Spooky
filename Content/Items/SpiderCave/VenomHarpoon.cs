using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Items.SpiderCave.Misc;
using Spooky.Content.Projectiles.SpiderCave;
using Spooky.Content.Tiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave
{
    public class VenomHarpoon : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 118;
			Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.width = 110;
            Item.height = 44;
            Item.useTime = 18;
			Item.useAnimation = 18;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 3;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(platinum: 1);
            Item.UseSound = SoundID.Zombie45;
            Item.shoot = ModContent.ProjectileType<VenomHarpoonProj>();
			Item.shootSpeed = 0f;
        }

        public override Vector2? HoldoutOffset()
		{
			return new Vector2(-16, -10);
		}

        public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[ModContent.ProjectileType<VenomHarpoonProj>()] < 1;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(player.direction == -1 ? -50 : 50, 50)) * 55f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position.X += muzzleOffset.X;
            }

            position.Y -= 15;

            for (int numProjectiles = 0; numProjectiles < 3; numProjectiles++)
            {
			    Projectile.NewProjectile(source, position, new Vector2(player.direction == -1 ? -50 : 50, Main.rand.Next(-35, 36)), Item.shoot, damage, knockback, player.whoAmI);
            }
			
			return false;
		}
    }
}