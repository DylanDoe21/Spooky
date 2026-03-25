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
using Spooky.Content.NPCs.SpiderCave.Projectiles;

namespace Spooky.Content.NPCs.SpiderCave.SporeEvent
{
    public class EvilMiteDeath : ModNPC
    {
		float addedStretch = 0f;
		float stretchRecoil = 0f;

		private static Asset<Texture2D> NPCTexture;

		public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

		public override void SetDefaults()
        {
            NPC.lifeMax = 4500;
            NPC.damage = 55;
            NPC.defense = 0;
            NPC.width = 60;
            NPC.height = 60;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
			NPC.dontCountMe = true;
			NPC.aiStyle = -1;
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			float stretch = 0f;
			stretch = Math.Abs(stretch) - addedStretch;

			Vector2 scaleStretch = new Vector2(1f + stretch, 1f - stretch);

			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, scaleStretch, effects, 0);

			return false;
		}

		public override void AI()
		{
			addedStretch = -stretchRecoil;

			NPC.rotation += NPC.velocity.X * 0.1f;

			NPC.ai[1]++;
            if (NPC.ai[1] > 5)
            {
                NPC.ai[1] = 5;
                if (NPC.velocity.Y == 0f && NPC.velocity.X != 0f)
                {
                    NPC.velocity.X *= 0.97f;
                    if ((double)NPC.velocity.X > -0.01 && (double)NPC.velocity.X < 0.01)
                    {
                        NPC.velocity.X = 0f;
                        NPC.netUpdate = true;
                    }
                }

                NPC.velocity.Y += 0.2f;
            }
			
			if (NPC.velocity.Y > 16f)
			{
				NPC.velocity.Y = 16f;
			}

			NPC.ai[0]++;
			if (NPC.ai[0] % 5 == 0)
			{
				stretchRecoil = 0.25f;
			}
			else
			{
				if (stretchRecoil > 0.1f)
				{
					stretchRecoil -= 0.125f;
				}
			}

			if (NPC.ai[0] >= 180)
			{
				SoundEngine.PlaySound(SoundID.NPCDeath21, NPC.Center);
				SoundEngine.PlaySound(SoundID.NPCDeath22, NPC.Center);
				SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact with { Pitch = 0.5f }, NPC.Center);

				Screenshake.ShakeScreenWithIntensity(NPC.Center, 10f, 350f);

				float maxAmount = 50;
				int currentAmount = 0;
				while (currentAmount <= maxAmount)
				{
					Vector2 velocity = new Vector2(Main.rand.NextFloat(2f, 12f), Main.rand.NextFloat(2f, 12f));
					Vector2 Bounds = new Vector2(Main.rand.NextFloat(2f, 12f), Main.rand.NextFloat(2f, 12f));
					float intensity = Main.rand.NextFloat(2f, 12f);

					Vector2 vector12 = Vector2.UnitX * 0f;
					vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
					vector12 = vector12.RotatedBy(velocity.ToRotation(), default);

					NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity, ModContent.ProjectileType<EvilMiteBlood>(), NPC.damage, 1f);

					int num104 = Dust.NewDust(NPC.Center, 1, 1, DustID.Blood, 0f, 0f, 100, default, 3f);
					Main.dust[num104].noGravity = true;
					Main.dust[num104].position = NPC.Center + vector12;
					Main.dust[num104].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;

					currentAmount++;
				}

				NPC.active = false;
			}
		}
	}
}