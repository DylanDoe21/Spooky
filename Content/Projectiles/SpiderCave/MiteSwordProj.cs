using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Enums;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.SpiderCave
{
    public class MiteSwordProj : ModProjectile
    {
		public float SwingRadians = MathHelper.Pi * 1.35f;

		public int Phase;
		
		private bool initialized = false;

		Vector2 direction = Vector2.Zero;

		private bool flip = false;
		private bool HasShot = false;

		public int Timer;

		private float rotation;

		public int SwingDirection
		{
			get
			{
				return Phase switch
				{
					0 => -1 * Math.Sign(direction.X),
					1 => 1 * Math.Sign(direction.X),
					_ => -1 * Math.Sign(direction.X),
				};
			}
		}

		private static Asset<Texture2D> ProjTexture;
		private static Asset<Texture2D> SlashTexture;

		public static readonly SoundStyle CrunchSound = new("Spooky/Content/Sounds/SentientKeybrandCrunch", SoundType.Sound);

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(52, 52);
			Projectile.DamageType = DamageClass.Melee;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.netImportant = true;
			Projectile.ownerHitCheck = true;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 16;
			Projectile.penetrate = -1;
			Projectile.scale = 1.2f;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Player player = Main.player[Projectile.owner];

			ProjTexture ??= ModContent.Request<Texture2D>(Texture);
			SlashTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Sentient/SentientKeybrandProjSlash");

			//fade out stuff
			int SwingTime = ItemGlobal.ActiveItem(player).useTime;
			float progress = Timer / (float)SwingTime;
			progress = EaseFunction.EaseQuadOut.Ease(progress);
			float SlashAlpha = 1f - Math.Abs(progress);

			var Effects1 = Projectile.ai[0] == 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			var Effects2 = Projectile.ai[0] == 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			if (flip)
			{
				Main.spriteBatch.Draw(ProjTexture.Value, player.MountedCenter - Main.screenPosition, null, lightColor, rotation + 1.57f, new Vector2(ProjTexture.Width() / 2, ProjTexture.Height()), Projectile.scale, Effects1, 0f);
			}
			else
			{
				Main.spriteBatch.Draw(ProjTexture.Value, player.MountedCenter - Main.screenPosition, null, lightColor, rotation + 1.57f, new Vector2(ProjTexture.Width() / 2, ProjTexture.Height()), Projectile.scale, Effects2, 0f);
			}

			return false;
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			Player player = Main.player[Projectile.owner];

			Vector2 lineDirection = rotation.ToRotationVector2();
			float collisionPoint = 0;
			
			if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), player.Center, player.Center + (lineDirection * Projectile.width), Projectile.height, ref collisionPoint))
			{
				return true;
			}

			return false;
		}

		// Plot a line from the start of the Solar Eruption to the end of it, to change the tile-cutting collision logic. (Don't change this.)
		public override void CutTiles()
		{
			Player player = Main.player[Projectile.owner];
			
			Vector2 lineDirection = rotation.ToRotationVector2();
			DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
			Utils.PlotTileLine(player.Center, player.Center + (lineDirection * Projectile.width), Projectile.height, DelegateMethods.CutTiles);
		}

		public override bool? CanHitNPC(NPC target)
		{
			if (GetProgress() > 0.2f && GetProgress() < 0.8f)
			{
				return base.CanHitNPC(target);
			}

			return false;
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			modifiers.HitDirectionOverride = Math.Sign(direction.X);
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			int SwingTime = ItemGlobal.ActiveItem(player).useTime;

			Projectile.velocity = Vector2.Zero;
			player.itemTime = player.itemAnimation = 5;
			player.heldProj = Projectile.whoAmI;

			if (Projectile.owner != Main.myPlayer)
			{
				return;
			}

			if (!initialized)
			{
				initialized = true;
				direction = player.DirectionTo(Main.MouseWorld);
				direction.Normalize();
				Projectile.rotation = direction.ToRotation();

				if (Phase == 1) flip = !flip;
				if (direction.X < 0) flip = !flip;
			}

			Projectile.Center = player.MountedCenter + (direction.RotatedBy(-1.57f) * 20);

			Timer++;

			if (Timer > SwingTime)
			{
				Projectile.Kill();
			}

			float progress = GetProgress();

			//scales up the projectile a bit more based on its swing progress
			//unnecessary for this projectile but ill keep it here incase this ever gets reused
			//Projectile.scale = 1.2f - Math.Abs(0.5f - progress);

			if (progress > 0.5f && !HasShot)
			{
				if (Projectile.owner == Main.myPlayer)
				{
					Vector2 ShootSpeed = Main.MouseWorld - player.Center;
					ShootSpeed.Normalize();
					ShootSpeed *= 10;

					//shoot a bunch of spore clouds
					for (int numProjs = 0; numProjs < 2; numProjs++)
					{
						Vector2 newVelocity = ShootSpeed.RotatedByRandom(MathHelper.ToRadians(35));

						int newProj = Projectile.NewProjectile(Projectile.GetSource_Death(), player.Center, newVelocity, 
                        ModContent.ProjectileType<SporeCloud>(), Projectile.damage / 2, 0, ai0: 2);
                        Main.projectile[newProj].alpha = 125;
					}

					//shoot mite
					int newMite = Projectile.NewProjectile(Projectile.GetSource_Death(), player.Center, ShootSpeed, ModContent.ProjectileType<MiteProjectile>(), Projectile.damage, 0, Projectile.owner);
					Main.projectile[newMite].DamageType = DamageClass.Melee;
					Main.projectile[newMite].ai[0] = 7; //frame
					Main.projectile[newMite].ai[2] = 2; //behavior
					Main.projectile[newMite].ai[1] = 60; //bouncing behavior doesnt home until after 1 second (60 ticks) so this will start it instantly
					Main.projectile[newMite].timeLeft = 180;
					Main.projectile[newMite].penetrate = 1;
				}

				HasShot = true;
			}

			rotation = Projectile.rotation + MathHelper.Lerp(SwingRadians / 2 * SwingDirection, -SwingRadians / 2 * SwingDirection, progress);

			player.direction = Math.Sign(direction.X);

			player.itemRotation = rotation;

			if (player.direction != 1)
			{
				player.itemRotation -= 3.14f;
			}

			player.itemRotation = MathHelper.WrapAngle(player.itemRotation);
			player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation - 1.57f);
		}

		private float GetProgress()
		{
			Player player = Main.player[Projectile.owner];

			int SwingTime = ItemGlobal.ActiveItem(player).useTime;

			float progress = Timer / (float)SwingTime;
			progress = EaseFunction.EaseQuadOut.Ease(progress);

			return Projectile.ai[0] == 1 ? -progress + 0.98f : progress;
		}
	}
}
     
          






