using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.Items.SpookyHell.Sentient;
using Spooky.Content.NPCs.EggEvent.Projectiles;

namespace Spooky.Content.Projectiles.Sentient
{
    public class EyeTadpole : ModProjectile
    {
		private static Asset<Texture2D> GlowTexture;
        
        public static readonly SoundStyle SplatSound = new("Spooky/Content/Sounds/Splat", SoundType.Sound) { Volume = 0.5f, Pitch = -0.5f };

        public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
			Projectile.width = 20;
            Projectile.height = 30;
            Projectile.DamageType = DamageClass.Magic;
			Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 3000;
		}

        public override void PostDraw(Color lightColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

            Vector2 drawOrigin = new(GlowTexture.Width() * 0.5f, Projectile.height * 0.5f);
            Rectangle rectangle = new(0, GlowTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, GlowTexture.Width(), GlowTexture.Height() / Main.projFrames[Projectile.type]);
            Main.EntitySpriteDraw(GlowTexture.Value, Projectile.Center - Main.screenPosition, rectangle, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
        }

        public override bool? CanDamage()
		{
			return false;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
                Projectile.velocity.Y = -oldVelocity.Y;
            }

			return false;
		}
		
		public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 6)
                {
                    Projectile.frame = 0;
                }
            }

            if (Projectile.ai[0] <= 0)
            {
                Projectile.ai[1]++;

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                Projectile.rotation += 0f * (float)Projectile.direction;

                int foundTarget = HomeOnTarget();
                if (foundTarget != -1)
                {
                    NPC target = Main.npc[foundTarget];
                    Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 10f;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
                }
                if (Projectile.ai[1] >= 360)
                {
                    Projectile.ai[0] = 1;
                }

                if (ItemGlobal.ActiveItem(player).type == ModContent.ItemType<SentientBeeGun>() && Main.mouseRight && Main.mouseRightRelease)
                {
                    Projectile.ai[0] = 2;
                }
            }
            if (Projectile.ai[0] == 1)
            {
                Projectile.rotation += 0.25f * (float)Projectile.direction;
                Projectile.alpha += 5;

                if (Projectile.alpha >= 255)
                {
                    Projectile.Kill();
                }
            }
            if (Projectile.ai[0] == 2)
            {
                Projectile.Kill();
            }

            //prevent projectiles clumping together
            for (int num = 0; num < Main.projectile.Length; num++)
            {
                Projectile other = Main.projectile[num];
                if (num != Projectile.whoAmI && other.type == Projectile.type && other.active && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
                {
                    const float pushAway = 0.12f;
                    if (Projectile.position.X < other.position.X)
                    {
                        Projectile.velocity.X -= pushAway;
                    }
                    else
                    {
                        Projectile.velocity.X += pushAway;
                    }
                    if (Projectile.position.Y < other.position.Y)
                    {
                        Projectile.velocity.Y -= pushAway;
                    }
                    else
                    {
                        Projectile.velocity.Y += pushAway;
                    }
                }
            }
		}

        private int HomeOnTarget()
        {
            const float homingMaximumRangeInPixels = 375;

            int selectedTarget = -1;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (target.CanBeChasedBy(Projectile))
                {
                    float distance = Projectile.Distance(target.Center);
                    if (distance <= homingMaximumRangeInPixels && (selectedTarget == -1 || Projectile.Distance(Main.npc[selectedTarget].Center) > distance))
                    {
                        selectedTarget = i;
                    }
                }
            }

            return selectedTarget;
        }

        public override void OnKill(int timeLeft)
		{
            if (Projectile.ai[0] >= 2)
            {
                Player player = Main.player[Projectile.owner];

                SoundEngine.PlaySound(SplatSound, Projectile.Center);

                foreach (var npc in Main.ActiveNPCs)
                {
                    if (npc.active && npc.CanBeChasedBy(this) && !npc.friendly && !npc.dontTakeDamage && !NPCID.Sets.CountsAsCritter[npc.type] && npc.Distance(Projectile.Center) <= 100f)
                    {
                        player.ApplyDamageToNPC(npc, Projectile.damage + Main.rand.Next(0, 21), 0, 0, false, null, true);
                    }
                }

                float maxAmount = 10;
                int currentAmount = 0;
                while (currentAmount <= maxAmount)
                {
                    Vector2 velocity = new Vector2(Main.rand.NextFloat(1f, 15f), Main.rand.NextFloat(1f, 15f));
                    Vector2 Bounds = new Vector2(Main.rand.NextFloat(1f, 15f), Main.rand.NextFloat(1f, 15f));
                    float intensity = Main.rand.NextFloat(1f, 10f);

                    Vector2 vector12 = Vector2.UnitX * 0f;
                    vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
                    vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
                    int num104 = Dust.NewDust(Projectile.Center, 0, 0, DustID.KryptonMoss, 0f, 0f, 100, default, 3f);
                    Main.dust[num104].noGravity = true;
                    Main.dust[num104].position = Projectile.Center + vector12;
                    Main.dust[num104].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;
                    currentAmount++;
                }
            }
        }
    }
}