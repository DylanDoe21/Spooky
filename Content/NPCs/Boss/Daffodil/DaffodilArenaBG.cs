using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs.Boss.Daffodil
{
    public class DaffodilArenaBG : ModNPC
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        private static Asset<Texture2D> BGTexture;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 5;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 1;
            NPC.height = 1;
            NPC.knockBackResist = 0f;
            NPC.dontTakeDamage = true;
			NPC.dontCountMe = true;
			NPC.immortal = true;
			NPC.noGravity = true;
			NPC.hide = true;
            NPC.aiStyle = -1;
            NPC.alpha = 255;
		}

        public override bool CheckActive()
        {
            return false;
        }

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			return false;
		}

		public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
            if (!NPC.IsABestiaryIconDummy)
			{
                //wall background
                BGTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Backgrounds/Catacomb/DaffodilArenaBG");

                float XParallax = (Main.LocalPlayer.Center.X - NPC.Center.X) * 0.02f;

                Vector2 DrawPosition = (NPC.Center - new Vector2((1454 / 2), (576 / 2))) - Main.screenPosition;
                Vector2 DrawPositionParallax = (NPC.Center - new Vector2((1454 / 2) + XParallax, (576 / 2))) - Main.screenPosition;
                
                spriteBatch.Draw(BGTexture.Value, DrawPosition, new Rectangle(0, 0, 1454, 576), new Color(47, 59, 55));
                spriteBatch.Draw(BGTexture.Value, DrawPositionParallax, new Rectangle(0, 0, 1454, 576), new Color(47, 59, 55));

                for (int X = (int)NPC.Center.X - 700; X <= (int)NPC.Center.X + 700; X += 20)
                {
                    for (int Y = (int)NPC.Center.Y - 300; Y <= (int)NPC.Center.Y + 300; Y += 20)
                    {
                        if (!Main.tile[X / 16, Y / 16].HasTile)
                        {
                            Lighting.AddLight(new Vector2(X, Y), new Vector3(0.47f, 0.59f, 0.55f));
                        }
                    }
                }
            }

			return false;
        }

        public override void AI()
        {
            NPC.velocity *= 0;
        }
    }
}