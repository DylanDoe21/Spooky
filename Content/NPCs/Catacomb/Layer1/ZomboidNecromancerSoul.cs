using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Catacomb.Layer1
{
	public class ZomboidNecromancerSoul : ModNPC
	{
        public override string Texture => "Spooky/Content/Projectiles/TrailCircle";

        bool runOnce = true;
		Vector2[] trailLength = new Vector2[6];

		private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 5;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 20;
            NPC.height = 20;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.alpha = 255;
            NPC.aiStyle = -1;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			if (runOnce)
			{
				return false;
			}

			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			Vector2 drawOrigin = new(NPCTexture.Width() * 0.5f, NPCTexture.Height() * 0.5f);
			Vector2 previousPosition = NPC.Center;

			for (int k = 0; k < trailLength.Length; k++)
			{
				float scale = NPC.scale * (trailLength.Length - k) / (float)trailLength.Length;
				scale *= 1f;

                Color color = Color.Lerp(Color.Gold, Color.Indigo, scale);

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

					//gives the after images a shaking effect
					float x = Main.rand.Next(-1, 2) * scale;
					float y = Main.rand.Next(-1, 2) * scale;

					Main.spriteBatch.Draw(NPCTexture.Value, drawPos + new Vector2(x, y), null, color, NPC.rotation, drawOrigin, scale * 0.6f, SpriteEffects.None, 0f);
				}

				previousPosition = currentPos;
			}

			return true;
		}

        public override void AI()
		{
            NPC Parent = Main.npc[(int)NPC.ai[0]];

            if (!Parent.active || Parent.type != ModContent.NPCType<ZomboidNecromancer>())
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

            Vector2 HomingSpeed = Parent.Center - NPC.Center;
            HomingSpeed.Normalize();
            HomingSpeed *= 12;
            NPC.velocity = HomingSpeed;

            if (NPC.Hitbox.Intersects(Parent.Hitbox))
            {
                SoundEngine.PlaySound(SoundID.NPCDeath52, NPC.Center);

                Parent.localAI[1]++;

                for (int numDusts = 0; numDusts < 20; numDusts++)
                {
                    int dustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, Main.rand.NextBool() ? Color.Gold : Color.Indigo, 0.1f);
                    Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-2f, 3f);
                    Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-2f, 3f);
                    Main.dust[dustGore].noGravity = true;
                }

                NPC.active = false;
            }
		}
	}
}