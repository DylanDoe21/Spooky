using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.PandoraBox
{
    public class SheldonShield : ModNPC
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 5;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 24;
            NPC.height = 26;
            NPC.knockBackResist = 0f;
			NPC.immortal = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.reflectsProjectiles = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.aiStyle = -1;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC Parent = Main.npc[(int)NPC.ai[1]];

            if (Parent.ai[0] == 1)
            {
                Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

                float num108 = 4;
                float num107 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 6.28318548f)) / 2f + 0.5f;
                float num106 = 0f;

                SpriteEffects spriteEffects3 = (NPC.spriteDirection == 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                Vector2 vector33 = new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4) - NPC.velocity;
                Color color29 = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.LightBlue);

                for (int num103 = 0; num103 < 4; num103++)
                {
                    Color color28 = color29;
                    color28 = NPC.GetAlpha(color28);
                    color28 *= 1f - num107;
                    Vector2 vector29 = new Vector2(NPC.Center.X, NPC.Center.Y) + ((float)num103 / (float)num108 * 6.28318548f + NPC.rotation + num106).ToRotationVector2() * (4f * num107 + 2f) - screenPos + new Vector2(0, NPC.gfxOffY + 4) - NPC.velocity * (float)num103;
                    Main.spriteBatch.Draw(tex, vector29, NPC.frame, color28, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.2f, spriteEffects3, 0f);
                }

                Main.spriteBatch.Draw(tex, vector33, NPC.frame, color29, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.2f, spriteEffects3, 0f);
            }

            return true;
        }

        public override void AI()
        {
            NPC Parent = Main.npc[(int)NPC.ai[1]];

            //kill the shield if the parent does not exist
            if (!Parent.active || Parent.life <= 0 || Parent.type != ModContent.NPCType<Sheldon>())
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/SheldonShieldGore").Type);
                }

                NPC.active = false;
            }

            NPC.spriteDirection = Parent.spriteDirection;

            //while sheldon is charging, always set the shield position directly infront of the npc
            if (Parent.ai[0] == 1 && Parent.localAI[0] > 75)
            {
                NPC.velocity *= 0;
                NPC.position.X = (Parent.spriteDirection == -1 ? Parent.Center.X - 45 : Parent.Center.X + 45) - NPC.width / 2;
                NPC.position.Y = Parent.Center.Y - NPC.width / 2;
            }
            //otherwise go in front and move up and down
            else
            {
                GoToPosition(Parent.spriteDirection == -1 ? -45 : 45, 0);
            }
        }

        public void GoToPosition(float X, float Y)
        {
            NPC Parent = Main.npc[(int)NPC.ai[1]];

            float goToX = (Parent.Center.X + X) - NPC.Center.X;
            float goToY = (Parent.Center.Y + Y) - NPC.Center.Y;
            
            float speed = 0.55f;

            //move up and down 
            NPC.ai[0]++;
            goToY += (float)Math.Sin(NPC.ai[0] / 30) * 15;
            
            if (NPC.velocity.X > speed)
            {
                NPC.velocity.X *= 0.95f;
            }
            if (NPC.velocity.Y > speed)
            {
                NPC.velocity.Y *= 0.95f;
            }

            if (NPC.velocity.X < goToX)
            {
                NPC.velocity.X = NPC.velocity.X + speed;
                if (NPC.velocity.X < 0f && goToX > 0f)
                {
                    NPC.velocity.X = NPC.velocity.X + speed;
                }
            }
            else if (NPC.velocity.X > goToX)
            {
                NPC.velocity.X = NPC.velocity.X - speed;
                if (NPC.velocity.X > 0f && goToX < 0f)
                {
                    NPC.velocity.X = NPC.velocity.X - speed;
                }
            }
            if (NPC.velocity.Y < goToY)
            {
                NPC.velocity.Y = NPC.velocity.Y + speed;
                if (NPC.velocity.Y < 0f && goToY > 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y + speed;
                    return;
                }
            }
            else if (NPC.velocity.Y > goToY)
            {
                NPC.velocity.Y = NPC.velocity.Y - speed;
                if (NPC.velocity.Y > 0f && goToY < 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y - speed;
                    return;
                }
            }
        }
    }
}