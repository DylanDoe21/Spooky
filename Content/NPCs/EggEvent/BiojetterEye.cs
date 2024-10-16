using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.NPCs.EggEvent.Projectiles;

namespace Spooky.Content.NPCs.EggEvent
{
	public class BiojetterEye : ModNPC
	{
        private static Asset<Texture2D> GlowTexture;
        private static Asset<Texture2D> ChainTexture1;
        private static Asset<Texture2D> ChainTexture2;
        private static Asset<Texture2D> ChainTexture3;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 500;
            NPC.damage = 60;
            NPC.defense = 12;
            NPC.width = 20;
            NPC.height = 20;
            NPC.npcSlots = 0f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit9;
            NPC.DeathSound = SoundID.NPCDeath22;
            NPC.aiStyle = -1;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC Parent = Main.npc[(int)NPC.ai[2]];

            //only draw if the parent is active
            if (Parent.active && Parent.type == ModContent.NPCType<Biojetter>())
            {
                ChainTexture1 ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/BiojetterEyeSegment1");
                ChainTexture2 ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/BiojetterEyeSegment2");
                ChainTexture3 ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/BiojetterEyeSegment3");
                
                Vector2 ParentCenter = Parent.Center;

                Rectangle? chainSourceRectangle = null;
                float chainHeightAdjustment = 0f;

                Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (ChainTexture1.Size() / 2f);
                Vector2 chainDrawPosition = new Vector2(NPC.Center.X, NPC.Center.Y + 3);
                Vector2 vectorToParent = ParentCenter.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
                Vector2 unitVectorToParent = vectorToParent.SafeNormalize(Vector2.Zero);
                float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : ChainTexture1.Height()) + chainHeightAdjustment;

                if (chainSegmentLength == 0)
                {
                    chainSegmentLength = 10;
                }

                float chainRotation = unitVectorToParent.ToRotation() + MathHelper.PiOver2;
                int chainCount = 0;
                float chainLengthRemainingToDraw = vectorToParent.Length() + chainSegmentLength / 2f;

                while (chainLengthRemainingToDraw > 0f)
                {
                    Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

                    if (chainCount == 1)
                    {
                        Main.spriteBatch.Draw(ChainTexture1.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);
                    }
                    else if (chainCount == 2)
                    {
                        Main.spriteBatch.Draw(ChainTexture2.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);
                    }
                    else if (chainCount > 2)
                    {
                        Main.spriteBatch.Draw(ChainTexture3.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);
                    }

                    chainDrawPosition += unitVectorToParent * chainSegmentLength;
                    chainCount++;
                    chainLengthRemainingToDraw -= chainSegmentLength;
                }
            }

            return true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/BiojetterEyeGlow");

            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 7)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 6)
            {
                NPC.frame.Y = Main.rand.Next(0, 2) * frameHeight;
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void AI()
		{
            NPC Parent = Main.npc[(int)NPC.ai[2]];

            //die if the parent npc is dead
            if (!Parent.active || Parent.type != ModContent.NPCType<Biojetter>())
            {
                NPC.active = false;
            }

            Vector2 GoTo = new Vector2(Parent.Center.X + NPC.ai[0], Parent.Center.Y + NPC.ai[1]);

            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 14, 22);
            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
		}

        public override void HitEffect(NPC.HitInfo hit) 
        {
            NPC Parent = Main.npc[(int)NPC.ai[2]];

            if (NPC.life <= 0) 
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/BiojetterEyeGore").Type);
                }

                Parent.ai[1]++;
                Parent.netUpdate = true;
            }
        }
	}
}