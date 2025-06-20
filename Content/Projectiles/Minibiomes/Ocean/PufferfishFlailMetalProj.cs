using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Minibiomes.Ocean
{
	public class PufferfishFlailMetalProj : ModProjectile
	{
		public enum AIState
		{
			Spinning,
			LaunchingForward,
			ForcedRetracting,
			Dropping
		}

		public AIState CurrentAIState
		{
			get => (AIState)Projectile.ai[0];
			set => Projectile.ai[0] = (float)value;
		}
		public ref float StateTimer => ref Projectile.ai[1];
		public ref float CollisionCounter => ref Projectile.localAI[0];
		public ref float SpinningStateTimer => ref Projectile.localAI[1];
		
		bool HasSlammed = false;

		private static Asset<Texture2D> ChainTexture;
		private static Asset<Texture2D> ProjTexture;

		public static readonly SoundStyle SlamSound = new("Spooky/Content/Sounds/PufferfishFlailSlam", SoundType.Sound);

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.friendly = true;
			Projectile.netImportant = true;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.penetrate = -1;
			Projectile.localNPCHitCooldown = 10;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			ChainTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Minibiomes/Ocean/PufferfishFlailMetalProjChain");

			Vector2 playerArmPosition = Main.GetPlayerArmPosition(Projectile);

			playerArmPosition.Y -= Main.player[Projectile.owner].gfxOffY;

			Rectangle? chainSourceRectangle = null;
			float chainHeightAdjustment = 0f;

			Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (ChainTexture.Size() / 2f);
			Vector2 chainDrawPosition = Projectile.Center;
			Vector2 vectorFromProjectileToPlayerArms = playerArmPosition.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
			Vector2 unitVectorFromProjectileToPlayerArms = vectorFromProjectileToPlayerArms.SafeNormalize(Vector2.Zero);
			float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : ChainTexture.Height()) + chainHeightAdjustment;

			if (chainSegmentLength == 0)
			{
				chainSegmentLength = 10;
			}

			float chainRotation = unitVectorFromProjectileToPlayerArms.ToRotation() + MathHelper.PiOver2;
			int chainCount = 0;
			float chainLengthRemainingToDraw = vectorFromProjectileToPlayerArms.Length() + chainSegmentLength / 2f;

			while (chainLengthRemainingToDraw > 0f)
			{
				Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

				Main.spriteBatch.Draw(ChainTexture.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

				chainDrawPosition += unitVectorFromProjectileToPlayerArms * chainSegmentLength;
				chainCount++;
				chainLengthRemainingToDraw -= chainSegmentLength;
			}

			if (CurrentAIState == AIState.LaunchingForward)
			{
				ProjTexture ??= ModContent.Request<Texture2D>(Texture);

				Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
				SpriteEffects spriteEffects = SpriteEffects.None;

				if (Projectile.spriteDirection == -1)
				{
					spriteEffects = SpriteEffects.FlipHorizontally;
				}
				for (int oldPos = 0; oldPos < Projectile.oldPos.Length && oldPos < StateTimer; oldPos++)
				{
					Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
					Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
					Main.spriteBatch.Draw(ProjTexture.Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale - oldPos / (float)Projectile.oldPos.Length / 3, spriteEffects, 0f);
				}
			}

			return true;
		}

		// This AI code was adapted from vanilla code: Terraria.Projectile.AI_015_Flails() 
		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			// Kill the projectile if the player dies or gets crowd controlled
			if (!player.active || player.dead || player.noItems || player.CCed || Vector2.Distance(Projectile.Center, player.Center) > 900f)
			{
				Projectile.Kill();
				return;
			}
			if (Main.myPlayer == Projectile.owner && Main.mapFullscreen)
			{
				Projectile.Kill();
				return;
			}

			Projectile.direction = Projectile.spriteDirection = Projectile.Center.X > player.Center.X ? -1 : 1;

			Vector2 mountedCenter = player.MountedCenter;
			bool shouldOwnerHitCheck = false;
			int launchTimeLimit = 15;
			float launchSpeed = 18f;
			float maxLaunchLength = 800f;
			float retractAcceleration = 3f;
			float maxRetractSpeed = 18f;
			float forcedRetractAcceleration = 30f;
			float maxForcedRetractSpeed = 20f;
			float unusedRetractAcceleration = 1f;
			float unusedMaxRetractSpeed = 14f;
			int unusedChainLength = 60;
			int defaultHitCooldown = 10;
			int spinHitCooldown = 20;
			int movingHitCooldown = 20;
			int ricochetTimeLimit = launchTimeLimit + 5;

			//melee speed scaling
			float meleeSpeedMultiplier = player.GetAttackSpeed(DamageClass.Melee);
			launchSpeed *= meleeSpeedMultiplier;
			unusedRetractAcceleration *= meleeSpeedMultiplier;
			unusedMaxRetractSpeed *= meleeSpeedMultiplier;
			retractAcceleration *= meleeSpeedMultiplier;
			maxRetractSpeed *= meleeSpeedMultiplier;
			forcedRetractAcceleration *= meleeSpeedMultiplier;
			maxForcedRetractSpeed *= meleeSpeedMultiplier;

			float launchRange = launchSpeed * launchTimeLimit;
			float maxDroppedRange = launchRange + 160f;
			Projectile.localNPCHitCooldown = defaultHitCooldown;

			switch (CurrentAIState)
			{
				case AIState.Spinning:
				{
					shouldOwnerHitCheck = true;
					if (Projectile.owner == Main.myPlayer)
					{
						Vector2 unitVectorTowardsMouse = mountedCenter.DirectionTo(new Vector2(Main.MouseWorld.X, Main.MouseWorld.Y)).SafeNormalize(Vector2.UnitX * player.direction);
						player.ChangeDir((unitVectorTowardsMouse.X > 0f) ? 1 : (-1));

						if (!player.channel) // If the player releases then change to moving forward mode
						{
							CurrentAIState = AIState.LaunchingForward;
							StateTimer = 0f;
							Projectile.velocity = unitVectorTowardsMouse * launchSpeed + player.velocity;
							Projectile.Center = mountedCenter;
							Projectile.netUpdate = true;
							Projectile.ResetImmunity();
							Projectile.localNPCHitCooldown = movingHitCooldown;
							break;
						}
					}

					SpinningStateTimer += 0.8f;
                    // This line creates a unit vector that is constantly rotated around the player. 10f controls how fast the projectile visually spins around the player
                    Vector2 offsetFromPlayer = new Vector2(player.direction).RotatedBy((float)Math.PI * 10f * (SpinningStateTimer / 60f) * player.direction);

                    offsetFromPlayer.Y *= 0.7f;
                    if (offsetFromPlayer.Y * player.gravDir > 0f) 
                    {
                        offsetFromPlayer.Y *= 1f;
                    }

					offsetFromPlayer.X *= 1.2f;
                    if (offsetFromPlayer.X * player.gravDir > 0f) 
                    {
                        offsetFromPlayer.X *= 1f;
                    }

					Projectile.Center = mountedCenter + offsetFromPlayer * 22f;
					Projectile.velocity = Vector2.Zero;
					Projectile.localNPCHitCooldown = spinHitCooldown; // set the hit speed to the spinning hit speed

					break;
				}
				case AIState.LaunchingForward:
				{
					bool shouldSwitchToRetracting = StateTimer++ >= launchTimeLimit;
					shouldSwitchToRetracting |= Projectile.Distance(mountedCenter) >= maxLaunchLength;

					if (shouldSwitchToRetracting)
					{
						CurrentAIState = AIState.Dropping;
						StateTimer = 0f;
						Projectile.netUpdate = true;
						Projectile.velocity *= 0.2f;
						break;
					}
					player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
					Projectile.localNPCHitCooldown = movingHitCooldown;
					break;
				}
				case AIState.ForcedRetracting:
				{
					Projectile.tileCollide = false;
					Vector2 unitVectorTowardsPlayer = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);

					if (Projectile.Distance(mountedCenter) <= maxForcedRetractSpeed)
					{
						Projectile.Kill(); // Kill the projectile once it is close enough to the player
						return;
					}

					Projectile.velocity *= 0.98f;
					Projectile.velocity = Projectile.velocity.MoveTowards(unitVectorTowardsPlayer * maxForcedRetractSpeed, forcedRetractAcceleration);
					Vector2 target = Projectile.Center + Projectile.velocity;
					Vector2 value = mountedCenter.DirectionFrom(target).SafeNormalize(Vector2.Zero);

					if (Vector2.Dot(unitVectorTowardsPlayer, value) < 0f)
					{
						Projectile.Kill(); // Kill projectile if it will pass the player
						return;
					}

					player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));

					break;
				}
				case AIState.Dropping:
				{
					if (Projectile.Distance(mountedCenter) > maxDroppedRange)
					{
						CurrentAIState = AIState.ForcedRetracting;
						StateTimer = 0f;
						Projectile.netUpdate = true;
					}
					else
					{
						Projectile.velocity.Y += 1.2f;
						Projectile.velocity.X *= 0.98f;
						player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));

						if (HasSlammed)
						{	
							Projectile.ai[2]++;
							if (Projectile.ai[2] >= 35)
							{
								CurrentAIState = AIState.ForcedRetracting;
							}
						}
					}

					break;
				}
			}

			// This is where Flower Pow launches projectiles. Decompile Terraria to view that code.
			Projectile.spriteDirection = Projectile.direction;
			Projectile.ownerHitCheck = shouldOwnerHitCheck; // This prevents attempting to damage enemies without line of sight to the player. The custom Colliding code for spinning makes this necessary.

			Vector2 vectorTowardsPlayer = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
			Projectile.rotation = vectorTowardsPlayer.ToRotation() + MathHelper.PiOver2;

			// If you have a ball shaped flail, you can use this simplified rotation code instead
			/*
			if (Projectile.velocity.Length() > 1f)
				Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.velocity.X * 0.1f; // skid
			else
				Projectile.rotation += Projectile.velocity.X * 0.1f; // roll
			*/

			Projectile.timeLeft = 2; // Makes sure the flail doesn't die (good when the flail is resting on the ground)
			player.heldProj = Projectile.whoAmI;
			player.SetDummyItemTime(2); //Add a delay so the player can't button mash the flail
			player.itemRotation = Projectile.DirectionFrom(mountedCenter).ToRotation();

			if (Projectile.Center.X < mountedCenter.X)
			{
				player.itemRotation += (float)Math.PI;
			}

			player.itemRotation = MathHelper.WrapAngle(player.itemRotation);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (CurrentAIState != AIState.Spinning && CurrentAIState != AIState.ForcedRetracting)
			{
				if (!HasSlammed)
				{
					SoundEngine.PlaySound(SlamSound, Projectile.Center);

					Screenshake.ShakeScreenWithIntensity(Projectile.Center, 5f * Projectile.oldVelocity.Y / 10, 300f);

					int ShockwaveAmount = (int)Projectile.oldVelocity.Y / 6;

					for (int i = -ShockwaveAmount; i <= ShockwaveAmount; i++)
					{
						Vector2 center = new Vector2(Projectile.Center.X, Projectile.Center.Y);
						center.X += 45 * i; //45 is the distance between each one
						int numtries = 0;
						int x = (int)(center.X / 16);
						int y = (int)(center.Y / 16);
						while (y < Projectile.Center.Y + 20 && Main.tile[x, y] != null && !WorldGen.SolidTile2(x, y) && 
						Main.tile[x - 1, y] != null && !WorldGen.SolidTile2(x - 1, y) && Main.tile[x + 1, y] != null && !WorldGen.SolidTile2(x + 1, y))
						{
							y++;
							center.Y = y * 16;
						}

						Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2(center.X, center.Y - 10), Vector2.Zero,
						ModContent.ProjectileType<PufferfishFlailMetalSmash>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
					}

					Projectile.velocity = Vector2.Zero;

					HasSlammed = true;
				}
			}

			return false;
		}

		public override bool? CanDamage()
		{
			// Flails in spin mode won't damage enemies within the first 12 ticks. Visually this delays the first hit until the player swings the flail around for a full spin before damaging anything.
			if ((CurrentAIState == AIState.Spinning && SpinningStateTimer <= 12f) || (CurrentAIState != AIState.Spinning && Projectile.velocity == Vector2.Zero))
			{
				return false;
			}

			return base.CanDamage();
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			// Flails do special collision logic that serves to hit anything within an ellipse centered on the player when the flail is spinning around the player. For example, the projectile rotating around the player won't actually hit a bee if it is directly on the player usually, but this code ensures that the bee is hit. This code makes hitting enemies while spinning more consistant and not reliant of the actual position of the flail projectile.
			if (CurrentAIState == AIState.Spinning)
			{
				Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
				Vector2 shortestVectorFromPlayerToTarget = targetHitbox.ClosestPointInRect(mountedCenter) - mountedCenter;
				shortestVectorFromPlayerToTarget.Y /= 0.8f; // Makes the hit area an ellipse. Vertical hit distance is smaller due to this math.
				float hitRadius = 55f; // The length of the semi-major radius of the ellipse (the long end)
				return shortestVectorFromPlayerToTarget.Length() <= hitRadius;
			}
			// Regular collision logic happens otherwise.
			return base.Colliding(projHitbox, targetHitbox);
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			//flails do 60% of their base damage while spinning
			if (CurrentAIState == AIState.Spinning)
			{
				modifiers.SourceDamage *= 0.6f;
			}
			//flails do full damage when launching/retracting
			else
			{
				modifiers.SourceDamage *= 1f;
			}

			modifiers.HitDirectionOverride = (Main.player[Projectile.owner].Center.X < target.Center.X).ToDirectionInt();

			// Knockback is only 25% as powerful when in spin mode
			if (CurrentAIState == AIState.Spinning)
			{
				modifiers.Knockback *= 0.25f;
			}
			// Knockback is only 50% as powerful when in drop down mode
			else if (CurrentAIState == AIState.Dropping)
			{
				modifiers.SourceDamage *= (1f * Projectile.velocity.Y / 8);
				modifiers.Knockback *= 0.5f;
			}
		}
	}
}