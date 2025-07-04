using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Enums;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Minibiomes.Ocean
{
    public class SpearfishLanceSlashProj : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Minibiomes/Ocean/SpearfishLanceProj";

        public float CollisionWidth => 30f * Projectile.scale;

        Vector2 SaveVelocity;

        public static readonly SoundStyle PokeSound = new("Spooky/Content/Sounds/SpearfishPoke", SoundType.Sound);

        public override void SetDefaults()
        {
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.hide = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 15;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(PokeSound, target.Center);
        }

        public override void CutTiles() 
        {
			DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
			Vector2 start = Projectile.Center;
			Vector2 end = start + Projectile.velocity.SafeNormalize(-Vector2.UnitY) * 10f;
			Utils.PlotTileLine(start, end, CollisionWidth, DelegateMethods.CutTiles);
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) 
        {
			Vector2 start = Projectile.Center;
			Vector2 end = start + Projectile.velocity * 4.5f;
			float collisionPoint = 0f;

			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, CollisionWidth, ref collisionPoint);
		}

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Vector2 playerRelativePoint = player.RotatedRelativePoint(player.MountedCenter, true);

            Projectile.direction = player.direction;
            player.heldProj = Projectile.whoAmI;
            Projectile.Center = playerRelativePoint;

            if (!player.active || player.dead || player.noItems || player.CCed)
            {
                Projectile.Kill();
            }

            Projectile.spriteDirection = Projectile.direction = player.direction;

            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[1] = Projectile.velocity.ToRotation() + MathHelper.PiOver2 - MathHelper.PiOver4 * Projectile.spriteDirection;

                if (Projectile.owner == Main.myPlayer)
                {
                    Vector2 ChargeDirection = Main.MouseWorld - player.Center;
                    ChargeDirection.Normalize();
                    ChargeDirection *= 30;
                    SaveVelocity = ChargeDirection;
                }

                Projectile.localAI[0]++;
            }
            else
            {
                Projectile.alpha = 0;
                
                player.immune = true;
                player.immuneTime = 2;
                player.velocity = SaveVelocity;
                player.GetModPlayer<SpookyPlayer>().SpearfishChargeCooldown = 180;
                
                Projectile.rotation = Projectile.localAI[1];

                Vector2 start = Projectile.Center;
			    Vector2 DustPosition = start + player.velocity * 3f;

                Dust dust = Dust.NewDustPerfect(DustPosition, DustID.WaterCandle, new Vector2(player.velocity.X + Main.rand.Next(-7, 8), player.velocity.Y + Main.rand.Next(-7, 8)) * 0.5f, default, default, 2f);
                dust.noGravity = true;
                dust.velocity -= player.velocity;
            }

            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
        }

        public override void OnKill(int timeLeft)
		{
            Player player = Main.player[Projectile.owner];

            player.velocity *= 0.15f;
        }
    }
}