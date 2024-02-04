using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;
using Terraria.GameContent.Events;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Friendly
{
    public class GiantWebAnimationBase : ModNPC
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 1;
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
                    int NewNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<GiantWebAnimation>());
                    Main.npc[NewNPC].ai[0] = NPC.whoAmI;
                    Main.npc[NewNPC].ai[2] = numPieces * distance;
                    Main.npc[NewNPC].ai[3] = numPieces;
                }
            }
        }
    }

    public class GiantWebAnimation : ModNPC
    {
        float distance = 0f;
        float rotationSpeed = 2f;

        private List<Vector2> cache;
        private Trail trail;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 1;
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
            Main.spriteBatch.End();
            Effect effect = ShaderLoader.GlowyTrail;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("Spooky/ShaderAssets/MagicTrail").Value);
            effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.05f);
            effect.Parameters["repeats"].SetValue(1);

            trail?.Render(effect);

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            return true;
        }

        const int TrailLength = 5;

        private void ManageCaches()
        {
            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int i = 0; i < TrailLength; i++)
                {
                    cache.Add(NPC.Center);
                }
            }

            cache.Add(NPC.Center);

            while (cache.Count > TrailLength)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, TrailLength, new RoundedTip(4), factor => 4, factor =>
            {
                return Color.Gray * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = NPC.oldPosition;
        }

        public override void AI()
        {
            NPC parent = Main.npc[(int)NPC.ai[0]];

            if (!parent.active)
            {
                NPC.active = false;
            }

            if (!Main.dedServ)
            {
                ManageCaches();
                ManageTrail();
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

            SpookyPlayer.ScreenShakeAmount = NPC.ai[1] / 30f;

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
                rotationSpeed *= 0.7f;
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
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<OldHunter>());
                }

                MoonlordDeathDrama.RequestLight(0f, NPC.Center);
                
                SpookyPlayer.ScreenShakeAmount = 0;

                parent.active = false;
                NPC.active = false;
            }
            else
            {
                MoonlordDeathDrama.RequestLight(NPC.ai[1] / 330f, NPC.Center);
            }
        }
    }
}