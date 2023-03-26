using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles;
using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientFleshWhip : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sentient Eye Lasher");
            Tooltip.SetDefault("Your summons will focus struck enemies"
			+ "\nLashes out two long range whips at once"
            + "\nHitting enemies may lower their defense massively for a short time");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 50;
			Item.DamageType = DamageClass.SummonMeleeSpeed;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.width = 52;
            Item.height = 50;
			Item.useTime = 42;
			Item.useAnimation = 42;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 2;
			Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 15);
			Item.UseSound = SoundID.Item152;
			Item.shoot = ModContent.ProjectileType<Blank>();
			Item.shootSpeed = 4f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			for (int numProjectiles = 0; numProjectiles < 2; numProjectiles++)
			{
				Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
				int i = Main.myPlayer;
				float num72 = Item.shootSpeed + Main.rand.Next(0, 1);
				int num73 = damage;
				float num74 = knockback;
				float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
				float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
				float f = Main.rand.NextFloat() * 6.28318548f;
				float value12 = 20f;
				float value13 = 60f;
				Vector2 vector13 = vector2 + f.ToRotationVector2() * MathHelper.Lerp(value12, value13, Main.rand.NextFloat());
				
				for (int num202 = 0; num202 < 50; num202++)
				{
					vector13 = vector2 + f.ToRotationVector2() * MathHelper.Lerp(value12, value13, Main.rand.NextFloat());
					if (Collision.CanHit(vector2, 0, 0, vector13 + (vector13 - vector2).SafeNormalize(Vector2.UnitX) * 8f, 0, 0))
					{
						break;
					}

					f = Main.rand.NextFloat() * 6.28318548f;
				}

				Vector2 mouseWorld = Main.MouseWorld;
				Vector2 vector14 = mouseWorld - vector13;
				Vector2 vector15 = new Vector2(num78, num79).SafeNormalize(Vector2.UnitY) * num72;
				vector14 = vector14.SafeNormalize(vector15) * num72;
				vector14 = Vector2.Lerp(vector14, vector15, Main.rand.NextFloat(-0.25f, 0.25f));

                if (numProjectiles == 0)
                {
				    Projectile.NewProjectile(source, player.Center.X, player.Center.Y - 10, vector14.X, vector14.Y, 
                    ModContent.ProjectileType<SentientFleshWhipProj1>(), num73, num74, i, 0f, 0f);
                }

                if (numProjectiles == 1)
                {
				    Projectile.NewProjectile(source, player.Center.X, player.Center.Y + 10, vector14.X, vector14.Y, 
                    ModContent.ProjectileType<SentientFleshWhipProj2>(), num73, num74, i, 0f, 0f);
                }
			}
			
			return true;
        }
    }
}