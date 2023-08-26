using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Spooky.Content.Projectiles.Sentient
{
	public class SentientLeatherWhipProj : ModProjectile
	{
		public static readonly SoundStyle SlurpSound = new("Spooky/Content/Sounds/Slurp", SoundType.Sound) { PitchVariance = 0.6f };

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.IsAWhip[Type] = true;
		}

		public override void SetDefaults() 
		{
			Projectile.width = 22;
			Projectile.height = 120;
			Projectile.DamageType = DamageClass.SummonMeleeSpeed;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.ownerHitCheck = true;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.penetrate = -1;
			Projectile.extraUpdates = 1;
			Projectile.localNPCHitCooldown = -1;
			Projectile.WhipSettings.Segments = 11;
			Projectile.WhipSettings.RangeMultiplier = 0.95f;
		}

		public override void AI() 
		{
			Player owner = Main.player[Projectile.owner];
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

			Projectile.Center = Main.GetPlayerArmPosition(Projectile) + Projectile.velocity * Projectile.ai[0];
			Projectile.spriteDirection = Projectile.velocity.X >= 0f ? 1 : -1;

			if (!Charge(owner)) 
			{ 
				return;
			}

			Projectile.ai[0]++;

			float swingTime = owner.itemAnimationMax * Projectile.MaxUpdates;
			if (Projectile.ai[0] >= swingTime || owner.itemAnimation <= 0) 
			{
				Projectile.Kill();
				return;
			}

			owner.heldProj = Projectile.whoAmI;

			if (Projectile.ai[0] == 1)
			{
				SoundEngine.PlaySound(SlurpSound, owner.Center);
			}

			if (Projectile.ai[0] == swingTime / 2) 
			{
				List<Vector2> points = Projectile.WhipPointsForCollision;
				Projectile.FillWhipControlPoints(Projectile, points);
			}
		}

		//charge mechanic stuff
		private bool Charge(Player owner) 
		{
			//120 is one second
			if (!owner.channel || Projectile.ai[1] >= 120)
			{
				return true;
			}

			Projectile.ai[1]++;

			//increase the amount of segments and damage when charged
			if (Projectile.ai[1] % 12 == 0) 
			{
				Projectile.WhipSettings.Segments++;
				Projectile.damage += 3;
			}

			//increase whip range
			Projectile.WhipSettings.RangeMultiplier += 1 / 120f;

			owner.itemAnimation = owner.itemAnimationMax;
			owner.itemTime = owner.itemTimeMax;

			return false;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) 
		{
			Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
			Projectile.damage = (int)(Projectile.damage * 0.85f);
		}

		public override bool PreDraw(ref Color lightColor) 
		{
			Player owner = Main.player[Projectile.owner];

			List<Vector2> list = new List<Vector2>();
			Projectile.FillWhipControlPoints(Projectile, list);

			SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			Main.instance.LoadProjectile(Type);
			Texture2D texture = TextureAssets.Projectile[Type].Value;

			Vector2 pos = list[0];

			for (int i = 0; i < list.Count - 1; i++) 
			{
				Rectangle frame = new Rectangle(0, 0, 14, 14);
				Vector2 origin = new Vector2(7, 7);

				//scale the whip down as it retracts, and scale it up as it is swung out
				Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
				float t = Projectile.ai[0] / timeToFlyOut;
				float scale = MathHelper.Lerp(0.75f, 1.2f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));

				//tip of the whip
				if (i == list.Count - 2)
				{
					frame.Y = 86;
					frame.Height = 22;
				}
				//loop between the two middle segments
				else if (i % 2 == 0) 
				{
					frame.Y = 68;
					frame.Height = 16;
				}
				else if (i % 1 == 0) 
				{
					frame.Y = 50;
					frame.Height = 16;
				}
				//bottom part of the whip?
				else if (i > 0)
				{
					frame.Y = 32;
					frame.Height = 16;
				}

				Vector2 element = list[i];
				Vector2 diff = list[i + 1] - element;

				float rotation = diff.ToRotation() - MathHelper.PiOver2; //This projectile's sprite faces down, so PiOver2 is used to correct rotation.
				Color color = Lighting.GetColor(element.ToTileCoordinates());

				Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);

				pos += diff;
			}

			return false;
		}
	}
}