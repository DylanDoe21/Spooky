using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Minibiomes.Christmas;

namespace Spooky.Content.Items.Minibiomes.Christmas
{
    public class JacksBag : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 10;
            Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.width = 28;
            Item.height = 36;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);
            Item.UseSound = SoundID.Item1;
			Item.shoot = ModContent.ProjectileType<RainbowJacks>();
            Item.shootSpeed = 7f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            for (int numProjs = 0; numProjs <= 2; numProjs++)
            {
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(45));

                Projectile.NewProjectile(source, position, newVelocity, type, damage, knockback, player.whoAmI, ai1: Main.rand.Next(0, 8));
            }
			
			return false;
		}
    }
}