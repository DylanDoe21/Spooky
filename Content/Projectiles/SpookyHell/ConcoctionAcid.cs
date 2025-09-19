using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.NPCs.EggEvent;
using Spooky.Content.NPCs.Boss.Orroboro;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class ConcoctionAcid : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/TrailSquare";

        bool runOnce = true;
		Vector2[] trailLength = new Vector2[8];

		private static Asset<Texture2D> ProjTexture;

		public static readonly SoundStyle EggDecaySound = new("Spooky/Content/Sounds/Orroboro/EggDecay", SoundType.Sound);
		
        public override void SetDefaults()
        {
			Projectile.width = 18;
            Projectile.height = 18;
			Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 35;
            Projectile.alpha = 255;
		}

        public override bool PreDraw(ref Color lightColor)
		{
			if (runOnce)
			{
				return false;
			}

			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, ProjTexture.Height() * 0.5f);
			Vector2 previousPosition = Projectile.Center;

			for (int k = 0; k < trailLength.Length; k++)
			{
				Color color = Color.Lime.MultiplyRGBA(lightColor);
                color *= (Projectile.timeLeft) / 90f;

				if (trailLength[k] == Vector2.Zero)
				{
					return true;
				}

				Vector2 drawPos = trailLength[k] - Main.screenPosition;
				Vector2 currentPos = trailLength[k];
				Vector2 betweenPositions = previousPosition - currentPos;

				float max = betweenPositions.Length();

				for (int i = 0; i < max; i++)
				{
					drawPos = previousPosition + -betweenPositions * (i / max) - Main.screenPosition;

					Main.spriteBatch.Draw(ProjTexture.Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale * 1.2f, SpriteEffects.None, 0f);
				}

				previousPosition = currentPos;
			}

			return true;
		}

		public override bool? CanDamage()
        {
			return false;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.25f;

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (runOnce)
			{
				for (int i = 0; i < trailLength.Length; i++)
				{
					trailLength[i] = Vector2.Zero;
				}
				runOnce = false;
			}

			Vector2 current = Projectile.Center;
			for (int i = 0; i < trailLength.Length; i++)
			{
				Vector2 previousPosition = trailLength[i];
				trailLength[i] = current;
				current = previousPosition;
			}

			bool OrroboroDoesNotExist = !NPC.AnyNPCs(ModContent.NPCType<OrroHeadP1>()) && !NPC.AnyNPCs(ModContent.NPCType<OrroHead>()) && !NPC.AnyNPCs(ModContent.NPCType<BoroHead>());

			if (!EggEventWorld.EggEventActive && OrroboroDoesNotExist)
			{
				foreach (var npc in Main.ActiveNPCs)
				{
					if (npc.type == ModContent.NPCType<OrroboroEgg>() && npc.Hitbox.Intersects(Projectile.Hitbox))
					{
						SoundEngine.PlaySound(EggDecaySound, Projectile.Center);

						if (Flags.downedEggEvent)
						{
							npc.ai[0] += 0.2f;
						}
						else
						{
							npc.ai[2] += 0.2f;
						}

						npc.netUpdate = true;

						if (Main.netMode == NetmodeID.Server) 
                        {
                           	NetMessage.SendData(MessageID.SyncNPC, number: npc.whoAmI);
                        }

						Projectile.Kill();
					}
				}
			}
		}
    }
}
     
          






