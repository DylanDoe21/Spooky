using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.SpiderCave.SpiderWar
{
    public class SpotlightSpiderFloor : ModNPC
    {
		int SaveDirection = 0;

        float Opacity = 0f;

        private static Asset<Texture2D> NPCTexture;
        private static Asset<Texture2D> NPCLightTexture;
        private static Asset<Texture2D> NPCLightGlowTexture;
		private static Asset<Texture2D> SpotlightTexture;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 5;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 88;
			NPC.height = 44;
            NPC.npcSlots = 0f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = false;
			NPC.noTileCollide = false;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.aiStyle = -1;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);
            NPCLightTexture ??= ModContent.Request<Texture2D>(Texture + "Light");
            NPCLightGlowTexture ??= ModContent.Request<Texture2D>(Texture + "LightGlow");
			SpotlightTexture ??= ModContent.Request<Texture2D>("Spooky/ShaderAssets/LightCone");

			var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			Vector2 frameOrigin = new Vector2(SpotlightTexture.Width() / 2f, 0f);
            Vector2 drawPos = NPC.Center - new Vector2(0, 6) - screenPos + frameOrigin + new Vector2(-SpotlightTexture.Width() / 2, NPC.gfxOffY + 4);

            //spotlight
			Main.EntitySpriteDraw(SpotlightTexture.Value, drawPos, null, new Color(125, 125, 125, 0) * Opacity, NPC.rotation, frameOrigin, 2f, effects, 0);

			//spotlight tail
			Main.EntitySpriteDraw(NPCLightTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

			//spotlight tail glow
			Main.EntitySpriteDraw(NPCLightGlowTexture.Value, NPC.Center - screenPos, NPC.frame, Color.White * Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            //npc itself
			Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), 0f, NPC.frame.Size() / 2, NPC.scale, effects, 0);

			return false;
        }

        public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            if (SaveDirection == 0)
            {
				SaveDirection = Main.rand.NextBool() ? -1 : 1;
            }
			else
			{
				NPC.spriteDirection = SaveDirection;
			}

            bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height) && NPC.Distance(player.Center) <= 550f;
            if (lineOfSight)
            {
				Lighting.AddLight(NPC.Center, 0.5f, 0.5f, 0.5f);

				Vector2 RotateTowards = player.Center - NPC.Center;

                float RotateDirection = (float)Math.Atan2(RotateTowards.Y, RotateTowards.X) + 4.71f;
                float RotateSpeed = 0.05f;

                NPC.rotation = NPC.rotation.AngleTowards(RotateDirection - MathHelper.TwoPi, RotateSpeed);

                if (Opacity < 1f)
                {
                    Opacity += 0.05f;
                }
            }
            if (!lineOfSight)
            {
                Vector2 RotateTowards = new Vector2(NPC.Center.X, NPC.Center.Y - 30) - NPC.Center;

                float RotateDirection = (float)Math.Atan2(RotateTowards.Y, RotateTowards.X) + 4.71f;
                float RotateSpeed = 0.05f;

                NPC.rotation = NPC.rotation.AngleTowards(RotateDirection - MathHelper.TwoPi, RotateSpeed);

				if (Opacity > 0f)
				{
					Opacity -= 0.1f;
				}
			}
		}
	}
}