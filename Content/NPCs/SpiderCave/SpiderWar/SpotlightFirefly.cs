using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.SpiderCave.SpiderWar
{
    public class SpotlightFirefly : ModNPC
    {
        float Opacity = 0f;

        Vector2 GoToPosition;

        private static Asset<Texture2D> NPCTexture;
        private static Asset<Texture2D> GlowTexture;
		private static Asset<Texture2D> SpotlightTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 5;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 64;
			NPC.height = 68;
            NPC.npcSlots = 0f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.aiStyle = -1;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 1)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 3)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

		public override bool CheckActive()
		{
			return false;
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");
			SpotlightTexture ??= ModContent.Request<Texture2D>("Spooky/ShaderAssets/LightCone");

            Vector2 frameOrigin = new Vector2(SpotlightTexture.Width() / 2f, 0f);
            Vector2 drawPos = NPC.Center - Main.screenPosition + frameOrigin + new Vector2(-SpotlightTexture.Width() / 2, NPC.gfxOffY + 4);

            Color color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(new Color(255, 255, 255, 0));

			Main.EntitySpriteDraw(SpotlightTexture.Value, drawPos, null, color * Opacity, NPC.rotation, frameOrigin, 2f, SpriteEffects.None, 0);

			Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, 
			NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)), 0f, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - screenPos, NPC.frame, 
			NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(Color.White)), 0f, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

			return false;
        }

        public override void AI()
		{
            NPC Parent = Main.npc[(int)NPC.ai[3]];

			Lighting.AddLight(NPC.Center, 0.5f, 0.5f, 0.5f);

			if (NPC.ai[0] > 0)
            {
                if (Opacity > 0f)
                {
                    Opacity -= 0.05f;
                }

                NPC.velocity.Y -= 0.4f;
				NPC.EncourageDespawn(30);
            }
            else
            {            
                Vector2 RotateTowards = Parent.Center - NPC.Center;

                float RotateDirection = (float)Math.Atan2(RotateTowards.Y, RotateTowards.X) + 4.71f;
                float RotateSpeed = 0.05f;

                NPC.rotation = NPC.rotation.AngleTowards(RotateDirection - MathHelper.TwoPi, RotateSpeed);

                Vector2 GoTo = new Vector2(Parent.Center.X, Parent.Center.Y - 360);

                if (NPC.Distance(GoTo) > 10f)
                {
                    float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 6, 15, 100);
                    NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                }
                else
                {
                    NPC.velocity *= 0.975f;
                }

                if (Opacity < 1f)
                {
                    Opacity += 0.01f;
                }
            }
		}
	}
}