using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs.Boss.Orroboro
{
    public class BoroBody : OrroBody
    {
        private static Asset<Texture2D> GlowTexture;

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Orroboro/BoroBodyGlow");

            Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture).Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White) * 0.5f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

			return false;
		}
	}

    public class BoroBodyP1 : OrroBodyP1
    {
        public override string Texture => "Spooky/Content/NPCs/Boss/Orroboro/BoroBody";

		private static Asset<Texture2D> GlowTexture;

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Orroboro/BoroBodyGlow");

            Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture).Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White) * 0.5f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

			return false;
		}
	}
}
