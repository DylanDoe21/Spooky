using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Blooms
{
    public class Barnacle : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public static readonly SoundStyle BiteSound = new("Spooky/Content/Sounds/Bite", SoundType.Sound) { PitchVariance = 0.7f, Volume = 0.5f };

        public override void SetDefaults()
        {
			Projectile.width = 26;
            Projectile.height = 24;
			Projectile.localNPCHitCooldown = 100;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;                 					
            Projectile.timeLeft = 600;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			var spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			Vector2 drawOrigin = new Vector2(ProjTexture.Width() * 0.5f, ProjTexture.Height() * 0.5f);
			Vector2 drawPos = Projectile.Center - Main.screenPosition;

			Main.EntitySpriteDraw(ProjTexture.Value, drawPos, null, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0f);

			return false;
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			Player player = Main.player[Projectile.owner];

			Projectile.scale = 1.5f;

			int ChanceToSpread = Main.raining ? 5 : 10;

			if (Main.rand.NextBool(ChanceToSpread) && player.ownedProjectileCounts[ModContent.ProjectileType<Barnacle>()] < 10)
			{
				int randomX = Main.rand.Next(-target.width / 3, target.width / 3);
				int randomY = Main.rand.Next(-target.height / 3, target.height / 3);

				Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center + new Vector2(randomX, randomY), Vector2.Zero,
				Type, Projectile.damage, Projectile.knockBack, player.whoAmI, ai0: target.whoAmI, ai1: randomX, ai2: randomY);
			}
		}

		//only deal damage to the npc the barnacle is attached to
		public override bool? CanHitNPC(NPC target)
		{
			NPC Parent = Main.npc[(int)Projectile.ai[0]];

			return target.whoAmI == Parent.whoAmI;
		}

		public override void AI()
        {
			Player player = Main.player[Projectile.owner];
			NPC Parent = Main.npc[(int)Projectile.ai[0]];

			if (!Parent.active)
			{
				Projectile.Kill();
			}

			int PosX = (int)Projectile.ai[1]; //x-offset on the npcs hitbox
			int PosY = (int)Projectile.ai[2]; //y-offset on the npcs hitbox

			float DirectionX = Parent.Center.X - Projectile.Center.X;
			float DirectionY = Parent.Center.Y - Projectile.Center.Y;
			Projectile.rotation = (float)Math.Atan2((double)DirectionY, (double)DirectionX) + 4.71f;

			if (Projectile.scale > 1)
			{
				Projectile.scale -= 0.1f;
			}

			Projectile.Center = Parent.Center + new Vector2(PosX, PosY);
            Projectile.gfxOffY = Parent.gfxOffY;
		}

		/*
        public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.NPCHit2, Projectile.Center);

            for (int numGores = 1; numGores <= 2; numGores++)
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.Find<ModGore>("Spooky/SentientSkullGore" + numGores).Type);
                }
            }
        }
		*/
    }
}