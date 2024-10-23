using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.SpiderCave
{
	public class OrbWeaverShieldProj : ModProjectile
	{
        int SaveDirection;
        float SaveRotation;

        float SaveKnockback;
        bool SavedKnockback = false;

		public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
            Projectile.width = 52;
            Projectile.height = 52;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.localNPCHitCooldown = 30;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 30;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
		}

        public override bool? CanDamage()
        {
			return Projectile.ai[2] == 0;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player owner = Main.player[Projectile.owner];

            //since this projectile is weird and only knocks enemies back in one direction, manually handle knockback here
            Vector2 Knockback = owner.Center - target.Center;
            Knockback.Normalize();
            Knockback *= SaveKnockback * 2;

            if (target.knockBackResist > 0f)
            {
                target.velocity = -Knockback * target.knockBackResist;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) 
        {
            Player player = Main.player[Projectile.owner];

			modifiers.SourceDamage += player.velocity.Length() / 7f * 0.9f;
		}

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];

            if (Projectile.localAI[0] >= 180 && Projectile.frame == 0)
            {
                Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

                Color color = new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0).MultiplyRGBA(Color.Red);

                Vector2 drawOrigin = new(tex.Width * 0.5f, Projectile.height * 0.5f);

                float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6.28318548f)) / 2f + 0.5f;

                for (int i = 0; i < 4; i++)
                {
                    Color newColor = color;
                    newColor = Projectile.GetAlpha(newColor);
                    newColor *= 1f;

                    var effects = player.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                    Vector2 drawPos = Projectile.position - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                    Rectangle rectangle = new(0, (tex.Height / Main.projFrames[Projectile.type]) * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
                    Main.EntitySpriteDraw(tex, drawPos, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale + (fade / 6), effects, 0);
                }
            }

            return true;
        }

		public override void AI()
		{
            Player player = Main.player[Projectile.owner];

            if (!player.active || player.dead || player.noItems || player.CCed) 
            {
                Projectile.Kill();
            }

            if (!SavedKnockback)
            {
                SaveKnockback = Projectile.knockBack;
                SavedKnockback = true;
            }
            else
            {
                Projectile.knockBack = 0;
            }

            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 ProjDirection = Main.MouseWorld - player.Center;
                ProjDirection.Normalize();
                Projectile.ai[0] = ProjDirection.X;
				Projectile.ai[1] = ProjDirection.Y;
            }

            Vector2 direction = new Vector2(Projectile.ai[0], Projectile.ai[1]);

            Projectile.direction = Projectile.spriteDirection = direction.X > 0 ? 1 : -1;

            if (Projectile.direction >= 0)
            {
                Projectile.rotation = direction.ToRotation() - 1.57f * (float)Projectile.direction;
            }
            else
            {
                Projectile.rotation = direction.ToRotation() + 1.57f * (float)Projectile.direction;
            }

			if (player.channel && Projectile.ai[2] == 0) 
            {
                Projectile.timeLeft = 30;

                player.itemRotation = Projectile.rotation;
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);

				Projectile.position = new Vector2(player.MountedCenter.X - Projectile.width / 2, player.MountedCenter.Y - 5 - Projectile.height / 2);
				player.velocity.X *= 0.99f;

                Projectile.localAI[0]++;

                if (Projectile.localAI[0] == 180)
                {
                    SoundEngine.PlaySound(SoundID.Item79, Projectile.Center);
                }
                
                if (direction.X > 0) 
                {
					player.direction = 1;
				}
				else 
                {
					player.direction = -1;
				}
			}
			else 
            {
				if (Projectile.owner == Main.myPlayer)
				{
                    Projectile.position = new Vector2(player.MountedCenter.X - Projectile.width / 2, player.MountedCenter.Y - 5 - Projectile.height / 2);

                    //set ai[2] to 1 so it cannot shoot again
                    Projectile.ai[2] = 1;

                    //shoot code here
                    if (Projectile.timeLeft >= 29)
                    {
                        SaveDirection = Projectile.spriteDirection;
                        SaveRotation = Projectile.rotation;
                        
                        if (Projectile.localAI[0] >= 180)
                        {
                            SoundEngine.PlaySound(SoundID.Item17, Projectile.Center);

                            Projectile.frame = 1;

                            for (int numProjectiles = -2; numProjectiles <= 2; numProjectiles++)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, 
                                    Main.rand.Next(9, 18) * Projectile.DirectionTo(Main.MouseWorld).RotatedBy(MathHelper.ToRadians(8) * numProjectiles),
                                    ModContent.ProjectileType<OrbWeaverShieldSpike>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
                                }
                            }
                        }
                    }

                    Projectile.spriteDirection = SaveDirection;
                    Projectile.rotation = SaveRotation;
                    player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, SaveRotation);
                }
			}

            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
        }
	}
}