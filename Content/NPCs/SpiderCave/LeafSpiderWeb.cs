using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.SpiderCave
{
    public class LeafSpiderWeb : ModNPC
    {
        public override string Texture => "Spooky/Content/NPCs/SpiderCave/BallSpiderWeb";

        private static Asset<Texture2D> ChainTexture;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 5;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 2;
            NPC.height = 2;
            NPC.knockBackResist = 0f;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.behindTiles = true;
            NPC.aiStyle = -1;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC Parent = Main.npc[(int)NPC.ai[2]];

            //only draw if the parent is active
            if (Parent.active && Parent.type == ModContent.NPCType<LeafSpider>())
            {
                ChainTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpiderCave/BallSpiderWeb");

                Vector2 ParentCenter = Parent.Center;
                

                Rectangle? chainSourceRectangle = null;
                float chainHeightAdjustment = 0f;

                Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (ChainTexture.Size() / 2f);
                Vector2 chainDrawPosition = new Vector2(NPC.Center.X, NPC.Center.Y - 10);
                Vector2 VectorToNPC = ParentCenter.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
                Vector2 unitVectorToNPC = VectorToNPC.SafeNormalize(Vector2.Zero);
                float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : ChainTexture.Height()) + chainHeightAdjustment;

                if (chainSegmentLength == 0)
                {
                    chainSegmentLength = 10;
                }

                float chainRotation = unitVectorToNPC.ToRotation() + MathHelper.PiOver2;
                int chainCount = 0;
                float chainLengthRemainingToDraw = VectorToNPC.Length() + chainSegmentLength / 2f;

                while (chainLengthRemainingToDraw > 0f)
                {
                    Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

                    Main.spriteBatch.Draw(ChainTexture.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

                    chainDrawPosition += unitVectorToNPC * chainSegmentLength;
                    chainCount++;
                    chainLengthRemainingToDraw -= chainSegmentLength;
                }
            }

            return true;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC Parent = Main.npc[(int)NPC.ai[2]];

            if (Parent.active && Parent.type == ModContent.NPCType<LeafSpider>())
            {
                switch ((int)NPC.ai[0])
                {
                    case 0: 
                    {
                        NPC.velocity.Y = -35;

                        if (Collision.SolidCollision(NPC.Center, NPC.width, NPC.height))
                        {
                            NPC.ai[0]++;
                        }

                        break;
                    }

                    case 1: 
                    {
                        NPC.velocity *= 0;

                        ParentGoTo(0, 150);

                        if (Vector2.Distance(NPC.Center, Parent.Center) <= 200f)
                        {
                            Parent.velocity *= 0.95f;
                        }

                        break;
                    }
                }
            }
            else
            {
                NPC.active = false;
            }
        }

        public void ParentGoTo(float X, float Y)
        {
            NPC Parent = Main.npc[(int)NPC.ai[2]];

            float goToX = (NPC.Center.X + X) - Parent.Center.X;
            float goToY = (NPC.Center.Y + Y) - Parent.Center.Y;

            Parent.ai[0]++;
            goToY += (float)Math.Sin(Parent.ai[0] / 30) * 15;

            float speed = 0.3f;
            
            if (Parent.velocity.X > speed)
            {
                Parent.velocity.X *= 0.98f;
            }
            if (Parent.velocity.Y > speed)
            {
                Parent.velocity.Y *= 0.98f;
            }

            if (Parent.velocity.X < goToX)
            {
                Parent.velocity.X = Parent.velocity.X + speed;
                if (Parent.velocity.X < 0f && goToX > 0f)
                {
                    Parent.velocity.X = Parent.velocity.X + speed;
                }
            }
            else if (Parent.velocity.X > goToX)
            {
                Parent.velocity.X = Parent.velocity.X - speed;
                if (Parent.velocity.X > 0f && goToX < 0f)
                {
                    Parent.velocity.X = Parent.velocity.X - speed;
                }
            }
            if (Parent.velocity.Y < goToY)
            {
                Parent.velocity.Y = Parent.velocity.Y + speed;
                if (NPC.velocity.Y < 0f && goToY > 0f)
                {
                    Parent.velocity.Y = Parent.velocity.Y + speed;
                    return;
                }
            }
            else if (Parent.velocity.Y > goToY)
            {
                Parent.velocity.Y = Parent.velocity.Y - speed;
                if (NPC.velocity.Y > 0f && goToY < 0f)
                {
                    Parent.velocity.Y = Parent.velocity.Y - speed;
                    return;
                }
            }
        }
    }
}
