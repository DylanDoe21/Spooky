using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.NPCs.Boss.Orroboro.Phase2
{
    public class OrroTail : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orro");
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = Main.masterMode ? 18000 / 3 : Main.expertMode ? 14500 / 2 : 10000;
            NPC.damage = 50;
            NPC.width = 50;
            NPC.height = 50;
            NPC.knockBackResist = 0f;
            NPC.behindTiles = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.noGravity = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.HitSound = SoundID.NPCHit9;
        }

        //rotate the bosses map icon to the NPCs direction
        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //if orro is in its death state
            if (Main.npc[NPCGlobal.Orro].ai[3] > 0)
            {
                float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 6.28318548f)) / 2f + 0.5f;

                Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

                Color color = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.Red);

                for (int numEffect = 0; numEffect < 4; numEffect++)
                {
                    Color newColor = color;
                    newColor = NPC.GetAlpha(newColor);
                    newColor *= 1f - fade;
                    Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y) + (numEffect / 4 * 6.28318548f + NPC.rotation + 0f).ToRotationVector2() * (4f * fade + 2f) - Main.screenPosition + new Vector2(0, NPC.gfxOffY) - NPC.velocity * numEffect;
                    Main.EntitySpriteDraw(tex, vector, NPC.frame, newColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.5f, SpriteEffects.None, 0);
                }
            }

            return true;
        }

        public override bool PreAI()
        {
            if (!Main.npc[(int)NPC.ai[1]].active)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/TailGore1").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/TailGore2").Type);
                NPC.active = false;
            }

            if (NPC.ai[1] < (double)Main.npc.Length)
            {
                Vector2 npcCenter = new(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
                float dirX = Main.npc[(int)NPC.ai[1]].position.X + (Main.npc[(int)NPC.ai[1]].width / 2) - npcCenter.X;
                float dirY = Main.npc[(int)NPC.ai[1]].position.Y + (Main.npc[(int)NPC.ai[1]].height / 2) - npcCenter.Y;
                NPC.rotation = (float)Math.Atan2(dirY, dirX) + 1.57f;
                float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
                float dist = (length - NPC.width) / length;
                float posX = dirX * dist;
                float posY = dirY * dist;

                NPC.velocity = Vector2.Zero;
                NPC.position.X = NPC.position.X + posX;
                NPC.position.Y = NPC.position.Y + posY;
            }
            
            return false;
        }
        
        public override bool CheckActive()
        {
            return false;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }
    }
}