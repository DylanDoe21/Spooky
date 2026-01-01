using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.NPCs.EggEvent.Projectiles;

namespace Spooky.Content.Projectiles.Sentient
{
    public class SentientKeybrandThrown : ModProjectile
    {
        public override string Texture => "Spooky/Content/Items/SpookyHell/Sentient/SentientKeybrand";

		private static Asset<Texture2D> ProjTexture;

		public static readonly SoundStyle CrunchSound = new("Spooky/Content/Sounds/SentientKeybrandCrunch", SoundType.Sound);

		public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 62;
            Projectile.height = 56;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 10000;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                var effects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color realColor = Projectile.GetAlpha(Color.Red * 0.5f) * ((Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                Rectangle rectangle = new(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, realColor, Projectile.oldRot[oldPos], drawOrigin, scale, effects, 0);
            }
            
            return true;
        }

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			if (target.life < target.lifeMax * 0.5f)
			{
				modifiers.FinalDamage *= 1.5f;
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.active && target.CanBeChasedBy(this) && target.defense > 0 && !target.friendly && !target.dontTakeDamage && !NPCID.Sets.CountsAsCritter[target.type])
			{
                target.AddBuff(ModContent.BuffType<SentientKeybrandDebuff>(), int.MaxValue);

                if (Main.rand.NextBool(5) && target.GetGlobalNPC<NPCGlobal>().KeybrandDefenseStacks < 15)
                {
                    SoundEngine.PlaySound(CrunchSound, target.Center);

                    //spawn blood splatter
                    for (int i = 0; i < 4; i++)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center.X, target.Center.Y, Main.rand.Next(-7, 8), Main.rand.Next(-7, 8), ModContent.ProjectileType<RedSplatter>(), 0, 0);
                        }
                    }

                    target.GetGlobalNPC<NPCGlobal>().KeybrandDefenseStacks++;
                    target.GetGlobalNPC<NPCGlobal>().InitializedKeybrandDefense = false;
                }
            }
		}

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
            
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.025f * (float)Projectile.direction;

            if (!owner.active || owner.dead)
            {
                Projectile.Kill();
            }

            Projectile.ai[0]++;

            if (Projectile.ai[0] >= 20 && Projectile.ai[0] < 30)
            {
                Projectile.velocity *= 0.8f;
            }

            if (Projectile.ai[0] >= 30)
            {
                //remove knockback here so the projectile doesnt fling enemies directly towards you when returning
                Projectile.knockBack = 0;

                Vector2 ReturnSpeed = owner.Center - Projectile.Center;
                ReturnSpeed.Normalize();
                ReturnSpeed *= 42;

                Projectile.velocity = ReturnSpeed;

                if (Projectile.Hitbox.Intersects(owner.Hitbox))
                {
                    Projectile.Kill();
                }
            }
        }
	}
}