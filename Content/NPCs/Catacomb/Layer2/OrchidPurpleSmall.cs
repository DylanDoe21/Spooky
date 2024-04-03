using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Spooky.Core;

namespace Spooky.Content.NPCs.Catacomb.Layer2
{
    public class OrchidPurpleSmall : ModNPC  
    {
        int MoveSpeedX;
        int MoveSpeedY;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/OrchidPurpleSmallBestiary",
                Position = new Vector2(0f, 10f),
                PortraitPositionYOverride = 16f
            };
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 160;
            NPC.damage = 45;
            NPC.defense = 10;
            NPC.width = 64;
			NPC.height = 58;
            NPC.npcSlots = 1f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noTileCollide = false;
            NPC.noGravity = true;
            NPC.behindTiles = true;
            NPC.HitSound = SoundID.Grass;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome2>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.OrchidPurpleSmall"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
        }

        public override void FindFrame(int frameHeight)
		{
            NPC.frameCounter++;
            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 8)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC Parent = Main.npc[(int)NPC.ai[0]];

            //only draw if the parent is active
            if (Parent.active && Parent.type == ModContent.NPCType<OrchidStem>())
            {
                Vector2 ParentCenter = Parent.Center;

                Asset<Texture2D> chainTexture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Catacomb/Layer2/OrchidStem1");

                Rectangle? chainSourceRectangle = null;
                float chainHeightAdjustment = 0f;

                Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (chainTexture.Size() / 2f);
                Vector2 chainDrawPosition = NPC.Center;
                Vector2 vectorFromProjectileToPlayerArms = ParentCenter.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
                Vector2 unitVectorFromProjectileToPlayerArms = vectorFromProjectileToPlayerArms.SafeNormalize(Vector2.Zero);
                float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : chainTexture.Height()) + chainHeightAdjustment;

                if (chainSegmentLength == 0)
                {
                    chainSegmentLength = 10;
                }

                float chainRotation = unitVectorFromProjectileToPlayerArms.ToRotation() + MathHelper.PiOver2;
                int chainCount = 0;
                float chainLengthRemainingToDraw = vectorFromProjectileToPlayerArms.Length() + chainSegmentLength / 2f;

                while (chainLengthRemainingToDraw > 0f)
                {
                    Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

                    var chainTextureToDraw = chainTexture;

                    Main.spriteBatch.Draw(chainTextureToDraw.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

                    chainDrawPosition += unitVectorFromProjectileToPlayerArms * chainSegmentLength;
                    chainCount++;
                    chainLengthRemainingToDraw -= chainSegmentLength;

                    if (chainCount % 2 == 0)
                    {
                        chainTexture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Catacomb/Layer2/OrchidStem2");
                    }
                    else
                    {
                        chainTexture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Catacomb/Layer2/OrchidStem1");
                    }
                }
            }

            return true;
        }

        public override void AI()
		{
            NPC Parent = Main.npc[(int)NPC.ai[0]];

            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            if (Parent.active && Parent.type == ModContent.NPCType<OrchidStem>())
            {
                ChasePlayer(player, Parent, 3, 0.01f);
            }
        }

        public void ChasePlayer(Player player, NPC Parent, int MaxSpeed, float Acceleration)
        {
            //fly towards the player
            if (player.Distance(Parent.Center) <= 360f)
            {
                //rotation
                Vector2 vector = new(NPC.Center.X, NPC.Center.Y);
                float RotateX = player.Center.X - vector.X;
                float RotateY = player.Center.Y - vector.Y;
                NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

                //flies to players X position
                if (NPC.Center.X >= player.Center.X && MoveSpeedX >= -MaxSpeed) 
                {
                    MoveSpeedX--;
                }
                else if (NPC.Center.X <= player.Center.X && MoveSpeedX <= MaxSpeed)
                {
                    MoveSpeedX++;
                }

                NPC.velocity.X += MoveSpeedX * Acceleration;
                NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -MaxSpeed, MaxSpeed);
                
                //flies to players Y position
                if (NPC.Center.Y >= player.Center.Y && MoveSpeedY >= -MaxSpeed)
                {
                    MoveSpeedY--;
                }
                else if (NPC.Center.Y <= player.Center.Y && MoveSpeedY <= MaxSpeed)
                {
                    MoveSpeedY++;
                }

                NPC.velocity.Y += MoveSpeedY * Acceleration;
                NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -MaxSpeed, MaxSpeed);
            }
            //if too far away, move back to the parent stem
            else
            {
                //rotation
                Vector2 vector = new(NPC.Center.X, NPC.Center.Y);
                float RotateX = Parent.Center.X - vector.X;
                float RotateY = Parent.Center.Y - vector.Y;
                NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

                //flies to parent X position
                if (NPC.Center.X >= Parent.Center.X && MoveSpeedX >= -MaxSpeed) 
                {
                    MoveSpeedX--;
                }
                else if (NPC.Center.X <= Parent.Center.X && MoveSpeedX <= MaxSpeed)
                {
                    MoveSpeedX++;
                }

                NPC.velocity.X += MoveSpeedX * Acceleration;
                NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -MaxSpeed, MaxSpeed);
                
                //flies to parent Y position
                if (NPC.Center.Y >= Parent.Center.Y - 150 && MoveSpeedY >= -MaxSpeed)
                {
                    MoveSpeedY--;
                }
                else if (NPC.Center.Y <= Parent.Center.Y - 150 && MoveSpeedY <= MaxSpeed)
                {
                    MoveSpeedY++;
                }

                NPC.velocity.Y += MoveSpeedY * Acceleration;
                NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -MaxSpeed, MaxSpeed);

                NPC.velocity *= 0.98f;
            }
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 5; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/OrchidPurpleSmallGore" + numGores).Type);
                    }
                }

                //kill the parent npc on death as well
                NPC Parent = Main.npc[(int)NPC.ai[0]];

                Parent.active = false;
            }
        }
    }
}