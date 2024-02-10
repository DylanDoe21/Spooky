using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Events;
using Terraria.Audio;
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
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        private List<Vector2> cache;
        private Trail trail;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 5;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 22;
            NPC.height = 20;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.alpha = 255;
            NPC.aiStyle = -1;
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
                return Color.Lerp(Color.Cyan, Color.Purple, factor.X) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = NPC.oldPosition;
        }

        public override void AI()
		{
            NPC Parent = Main.npc[(int)NPC.ai[0]];

            if (!Main.dedServ)
            {
                ManageCaches();
                ManageTrail();
            }

            Vector2 HomingSpeed = Parent.Center - NPC.Center;
            HomingSpeed.Normalize();
            HomingSpeed *= 12;
            NPC.velocity = HomingSpeed;

            if (NPC.Hitbox.Intersects(Parent.Hitbox))
            {
                Parent.localAI[1]++;

                for (int numDusts = 0; numDusts < 20; numDusts++)
                {
                    int dustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, default, 0.1f);
                    Main.dust[dustGore].color = Main.rand.NextBool() ? Color.Cyan : Color.Purple;
                    Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-2f, 3f);
                    Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-2f, 3f);
                    Main.dust[dustGore].noGravity = true;
                }

                NPC.active = false;
            }
		}
	}
}