using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Events;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.NPCs.Friendly
{
    public class GiantWebAnimationBase : ModNPC
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 5;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 30;
            NPC.height = 28;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.alpha = 255;
        }

        public override void AI()
        {
            NPC.ai[0]++;

            if (NPC.ai[0] == 1)
            {
                for (int numPieces = 0; numPieces < 4; numPieces++)
                {
                    int distance = 360 / 4;
                    int NewNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<GiantWebAnimationPiece>());
                    Main.npc[NewNPC].ai[0] = NPC.whoAmI;
                    Main.npc[NewNPC].ai[2] = numPieces * distance;
                    Main.npc[NewNPC].ai[3] = numPieces;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {   
                        NetMessage.SendData(MessageID.SyncNPC, number: NewNPC);
                    }
                }
            }
        }
    }

    public class GiantWebAnimationPiece : ModNPC
    {
        float distance = 0f;
        float rotationSpeed = 2f;

        bool runOnce = true;
		Vector2[] trailLength = new Vector2[6];

		private static Asset<Texture2D> TrailTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 5;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 30;
            NPC.height = 28;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.alpha = 255;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Y = frameHeight * (int)NPC.ai[3];
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			if (runOnce)
			{
				return false;
			}

			TrailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/TrailCircle");

			Vector2 drawOrigin = new(TrailTexture.Width() * 0.5f, TrailTexture.Height() * 0.5f);
			Vector2 previousPosition = NPC.Center;

			for (int k = 0; k < trailLength.Length; k++)
			{
				float scale = NPC.scale * (trailLength.Length - k) / (float)trailLength.Length;
				scale *= 1f;

                Color color = Color.Lerp(Color.Brown, Color.Gold, scale);

				if (trailLength[k] == Vector2.Zero)
				{
					return true;
				}

				Vector2 drawPos = trailLength[k] - Main.screenPosition;
				Vector2 currentPos = trailLength[k];
				Vector2 betweenPositions = previousPosition - currentPos;

				float max = betweenPositions.Length() / (4 * scale);

				for (int i = 0; i < max; i++)
				{
					drawPos = previousPosition + -betweenPositions * (i / max) - Main.screenPosition;

					Main.spriteBatch.Draw(TrailTexture.Value, drawPos, null, color, NPC.rotation, drawOrigin, scale * 0.5f, SpriteEffects.None, 0f);
				}

				previousPosition = currentPos;
			}

			return true;
		}

        public override void AI()
        {
            NPC parent = Main.npc[(int)NPC.ai[0]];

            if (!parent.active)
            {
                NPC.active = false;
            }

            if (runOnce)
			{
				for (int i = 0; i < trailLength.Length; i++)
				{
					trailLength[i] = Vector2.Zero;
				}
				runOnce = false;
			}

			Vector2 current = NPC.Center;
			for (int i = 0; i < trailLength.Length; i++)
			{
				Vector2 previousPosition = trailLength[i];
				trailLength[i] = current;
				current = previousPosition;
			}

            if (NPC.alpha > 0)
            {
                NPC.alpha -= 5;
            }

            NPC.ai[2] += rotationSpeed;
            double rad = NPC.ai[2] * (Math.PI / 180);
            NPC.position.X = parent.Center.X - (int)(Math.Cos(rad) * distance) - NPC.width / 2;
            NPC.position.Y = parent.Center.Y - (int)(Math.Sin(rad) * distance) - NPC.height / 2;

            NPC.ai[1] += 0.5f;

            Screenshake.ShakeScreenWithIntensity(NPC.Center, NPC.ai[1] / 30f, 400f);

            if (NPC.ai[1] < 180)
            {
                distance += 0.5f;
            }

            if (NPC.ai[1] > 120 && NPC.ai[1] < 240)
            {
                rotationSpeed += 0.05f;
            }

            if (NPC.ai[1] > 200)
            {
                rotationSpeed *= 0.745f;
            }

            if (NPC.ai[1] > 240 && distance > 0)
            {
                distance -= 2;
            }

            if (NPC.ai[1] >= 340)
            {
                SoundEngine.PlaySound(SoundID.Item4, NPC.Center);
                SoundEngine.PlaySound(SoundID.DD2_DarkMageSummonSkeleton, NPC.Center);

                if (NPC.ai[3] == 0)
                {
                    int Skeleton = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<OldHunterSleeping>());

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {   
                        NetMessage.SendData(MessageID.SyncNPC, number: Skeleton);
                    }
                }

                //immediately disable the dramatic lighting when the animation is done
                if (ModContent.GetInstance<SpookyConfig>().OldHunterDramaticLight)
                {
                    MoonlordDeathDrama.RequestLight(0f, NPC.Center);
                }
                
                //immediately stop screen shake when the animation is done
                Screenshake.ShakeScreenWithIntensity(NPC.Center, 0f, 0f);

                parent.active = false;
                NPC.active = false;
            }
            else
            {
                //dont apply dramatic light if the player has the config option for it turned off
                if (ModContent.GetInstance<SpookyConfig>().OldHunterDramaticLight)
                {
                    MoonlordDeathDrama.RequestLight(NPC.ai[1] / 330f, NPC.Center);
                }
            }
        }
    }
}