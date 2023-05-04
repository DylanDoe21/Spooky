using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Sentient;
using Spooky.Content.Tiles.SpookyHell.Furniture;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientBloodRainBow : ModItem, ICauldronOutput
    {
        public override void SetDefaults()
        {
            Item.damage = 20;
			Item.DamageType = DamageClass.Ranged;
			Item.autoReuse = true;
            Item.width = 48;
            Item.height = 76;
            Item.useTime = 32;
			Item.useAnimation = 32;
			Item.useStyle = ItemUseStyleID.EatFood;
			Item.knockBack = 1;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 20);
            Item.UseSound = SoundID.NPCDeath13;
            Item.shoot = ProjectileID.PurificationPowder;
			Item.useAmmo = AmmoID.Arrow;
			Item.shootSpeed = 5f;
        }

        public override Vector2? HoldoutOffset()
		{
			return new Vector2(-3, 0);
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 50f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

            for (int numProjs = 0; numProjs <= 4; numProjs++)
            {
                Projectile.NewProjectile(source, position.X + Main.rand.Next(-25, 25), position.Y + Main.rand.Next(-25, 25), 
                velocity.X, velocity.Y, ModContent.ProjectileType<BloodTooth>(), damage, knockback, player.whoAmI, 0f, 0f);
            }

            return false;
        }
    }
}