using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Tiles.SpookyHell.Furniture;
using System.Text;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientBloodRainBow : ModItem, ICauldronOutput
    {
        public override void SetDefaults()
        {
            Item.damage = 22;
			Item.DamageType = DamageClass.Ranged;
			Item.autoReuse = true;
            Item.width = 48;
            Item.height = 76;
            Item.useTime = 12;
			Item.useAnimation = 12;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 1;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 20);
            Item.UseSound = SoundID.NPCDeath13;
            Item.shoot = ProjectileID.PurificationPowder;
			Item.useAmmo = AmmoID.Arrow;
			Item.shootSpeed = 20f;
        }

        public override Vector2? HoldoutOffset()
		{
			return new Vector2(-3, 0);
		}

        /*
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 60f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

            Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(50));

            Projectile.NewProjectile(source, position.X, position.Y, newVelocity.X + Main.rand.Next(-1, 2),
            newVelocity.Y + Main.rand.Next(-1, 2), type, damage, knockback, player.whoAmI, 0f, 0f);

            return false;
        }
        */
    }
}