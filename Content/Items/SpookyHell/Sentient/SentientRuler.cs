using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.Sentient;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientRuler : ModItem, ICauldronOutput
    {
        public override void SetDefaults()
        {
            Item.damage = 35;
			Item.DamageType = DamageClass.Melee;
            Item.autoReuse = true;
            Item.noMelee = true;
			Item.noUseGraphic = true;
            Item.width = 32;
            Item.height = 60;
            Item.useTime = 7;
			Item.useAnimation = 7;
			Item.useStyle = ItemUseStyleID.Rapier;
			Item.knockBack = 2;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 10);
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<SentientRulerProj>();
            Item.shootSpeed = 3.5f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(18));

			Projectile.NewProjectile(source, position, newVelocity, type, damage, knockback, player.whoAmI);
			
			return false;
		}
    }
}