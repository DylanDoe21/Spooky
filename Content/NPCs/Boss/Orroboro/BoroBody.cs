using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs.Boss.Orroboro
{
    public class BoroBody : OrroBody
    {
        public override string Texture => "Spooky/Content/NPCs/Boss/Orroboro/OrroBody";

        private static Asset<Texture2D> NPCTexture;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<OrroHead>()))
            {
                NPCTexture ??= ModContent.Request<Texture2D>(Texture);

                Color color = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.Red) * 0.5f;

                for (int numEffect = 0; numEffect < 4; numEffect++)
                {
                    Color newColor = color;
                    newColor = NPC.GetAlpha(newColor);
                    newColor *= 1f;
                    Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y) + (numEffect / 4 * 6 + NPC.rotation + 0f).ToRotationVector2() * 6f - Main.screenPosition + new Vector2(0, NPC.gfxOffY) - NPC.velocity * numEffect;
                    Main.EntitySpriteDraw(NPCTexture.Value, vector, NPC.frame, newColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.5f, SpriteEffects.None, 0);
                }
            }

            return true;
        }
    }

    public class BoroBodyP1 : OrroBodyP1
    {
        public override string Texture => "Spooky/Content/NPCs/Boss/Orroboro/OrroBody";
    }
}
