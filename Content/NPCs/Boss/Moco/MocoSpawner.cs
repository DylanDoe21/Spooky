using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Biomes;
using Spooky.Content.NPCs.Boss.Moco.Projectiles;
using Spooky.Content.Tiles.NoseTemple.Furniture;

namespace Spooky.Content.NPCs.Boss.Moco
{
    public class MocoSpawner : ModNPC  
    {
        public bool Shake = false;

        private static Asset<Texture2D> NPCTexture;

        public static readonly SoundStyle DeathSound = new("Spooky/Content/Sounds/Moco/MocoIdolShatter", SoundType.Sound);

        public override void SetStaticDefaults()
		{
			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		}
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 5;
            NPC.width = 38;
			NPC.height = 50;
            NPC.npcSlots = 0f;
			NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.immortal = true;
			NPC.dontTakeDamage = true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y + 10) - Main.screenPosition;
            float time = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f;

            //DrawPrettyStarSparkle(NPC.Opacity, SpriteEffects.None, vector, Color.White, Color.White, 0.5f, 0f, 0.5f, 0.5f, 1f, 0f, new Vector2(5f * time, 4f * time), new Vector2(2, 2));
            //DrawPrettyStarSparkle(NPC.Opacity, SpriteEffects.None, vector, Color.Lime, Color.Lime, 0.5f, 0f, 0.5f, 0.5f, 1f, 90f, new Vector2(5f * time, 4f * time), new Vector2(2, 2));
        }

        private static void DrawPrettyStarSparkle(float opacity, SpriteEffects dir, Vector2 drawpos, Color drawColor, Color shineColor, float flareCounter, float fadeInStart, float fadeInEnd, float fadeOutStart, float fadeOutEnd, float rotation, Vector2 scale, Vector2 fatness) 
        {
			Texture2D Texture = TextureAssets.Extra[98].Value;
			Color color = shineColor * opacity * 0.5f;
			color.A = (byte)0;
			Vector2 origin = Texture.Size() / 2f;
			Color color2 = drawColor * 0.5f;
			float Intensity = Utils.GetLerpValue(fadeInStart, fadeInEnd, flareCounter, clamped: true) * Utils.GetLerpValue(fadeOutEnd, fadeOutStart, flareCounter, clamped: true);
			Vector2 vector = new Vector2(fatness.X * 0.5f, scale.X) * Intensity;
			Vector2 vector2 = new Vector2(fatness.Y * 0.5f, scale.Y) * Intensity;
			color *= Intensity;
			color2 *= Intensity;
			Main.EntitySpriteDraw(Texture, drawpos, null, color, (float)Math.PI / 2f + rotation, origin, vector, dir);
			Main.EntitySpriteDraw(Texture, drawpos, null, color, 0f + rotation, origin, vector2, dir);
			Main.EntitySpriteDraw(Texture, drawpos, null, color2, (float)Math.PI / 2f + rotation, origin, vector * 0.6f, dir);
			Main.EntitySpriteDraw(Texture, drawpos, null, color2, 0f + rotation, origin, vector2 * 0.6f, dir);
		}

        public override bool CheckActive()
        {
            return false;
        }

        public override void AI()
        {
            NPC.ai[0]++;

            //spawn moco
            if (NPC.ai[0] == 30)
            {
                int MocoSpawnOffsetX = ((int)NPC.Center.X / 16) < (Main.maxTilesX / 2) ? 2000 : -2000;

                int MocoIntro = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + MocoSpawnOffsetX, (int)NPC.Center.Y, ModContent.NPCType<MocoIntro>(), ai3: NPC.whoAmI);

                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.SyncNPC, number: MocoIntro);
                }
            }

            if (NPC.ai[1] > 0)
            {
                NPC.ai[2]++;

                NPC.ai[3] += 0.05f;

                if (Shake)
                {
                    NPC.rotation += NPC.ai[3] / 20;
                    if (NPC.rotation > 0.5f)
                    {
                        Shake = false;
                    }
                }
                else
                {
                    NPC.rotation -= NPC.ai[3] / 20;
                    if (NPC.rotation < -0.5f)
                    {
                        Shake = true;
                    }
                }

                if (NPC.ai[2] >= 300)
                {
                    SoundEngine.PlaySound(DeathSound, NPC.Center);

                    //spawn dusts
                    for (int numDusts = 0; numDusts < 45; numDusts++)
                    {
                        int dustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemEmerald, 0f, -2f, 0, default, 3f);
                        Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-16f, 16f);
                        Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-4f, 4f);
                        Main.dust[dustGore].noGravity = true;
                    }

                    NPC.active = false;
                }
            }
        }
    }
}