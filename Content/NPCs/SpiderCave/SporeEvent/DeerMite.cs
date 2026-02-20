using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Content.Items.SpiderCave.Misc;
using Spooky.Content.Tiles.Blooms;

namespace Spooky.Content.NPCs.SpiderCave.SporeEvent
{
    public class DeerMite : ModNPC  
    {
        int SaveDirection;
        bool hasCollidedWithWall = false;

        Vector2 SaveNPCPosition;
        Vector2 SaveNPCVelocity;

        private static Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 400;
            NPC.damage = 70;
            NPC.defense = 25;
            NPC.width = 54;
			NPC.height = 54;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.HitSound = SoundID.NPCHit45 with { Pitch = -0.25f };
			NPC.DeathSound = SoundID.NPCDeath8 with { Pitch = 0.5f };
            NPC.aiStyle = 26;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SporeEventBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.DeerMite"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SporeEventBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
        }

        public override void FindFrame(int frameHeight)
        {   
            Player player = Main.player[NPC.target];

            //moving fast frames
            if (hasCollidedWithWall)
            {
                //running animation
                NPC.frameCounter++;
                if (NPC.frameCounter > 3)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }

                if (NPC.frame.Y < frameHeight * 5)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }

                if (NPC.frame.Y >= frameHeight * 8)
                {
                    NPC.frame.Y = 5 * frameHeight;
                }
            }
            //normal walk frames
            else
            {
                //running animation
                NPC.frameCounter++;
                if (NPC.frameCounter > 8 - (NPC.velocity.X > 0 ? NPC.velocity.X : -NPC.velocity.X))
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }

                if (NPC.frame.Y >= frameHeight * 4)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }
        }
        
        public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            //collide with walls and play a sound
            if (!hasCollidedWithWall && (NPC.oldVelocity.X >= 5 || NPC.oldVelocity.X <= -5) && NPC.collideX)
            {
                SoundEngine.PlaySound(SoundID.Dig with { Pitch = 0.5f }, NPC.Center);

                for (int numDusts = 0; numDusts < 18; numDusts++)
                {
                    Dust dust = Dust.NewDustPerfect(NPC.Center + new Vector2(NPC.velocity.X < 0 ? -20 : 20), DustID.Mud, 
                    -NPC.velocity + new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-15, 16)) * 0.5f, default, default, 2f);
                    dust.noGravity = true;
                }
                
                SaveNPCPosition = NPC.Center;
                SaveNPCVelocity = NPC.velocity;

                NPC.velocity = Vector2.Zero;

                NPC.aiStyle = -1;

                //set velocity to zero
                NPC.localAI[1] = 1;

                hasCollidedWithWall = true;
            }

            if (NPC.localAI[1] <= 0)
            {
                NPC.spriteDirection = NPC.direction = NPC.velocity.X < 0 ? -1 : 1;
                SaveDirection = NPC.direction;
            }
            else
            {
                NPC.spriteDirection = SaveDirection;

                NPC.Center = new Vector2(SaveNPCPosition.X, SaveNPCPosition.Y);
				NPC.Center += Main.rand.NextVector2Square(-2, 2);

                NPC.noGravity = true;
                NPC.noTileCollide = true;
                NPC.velocity = Vector2.Zero;

                NPC.localAI[2]++;
                if (NPC.localAI[2] % 60 == 0)
                {
                    SoundEngine.PlaySound(SoundID.NPCHit45 with { Volume = 0.5f, Pitch = -0.5f }, NPC.Center);
                }

                if (NPC.localAI[2] >= 170)
                {
                    SoundEngine.PlaySound(SoundID.Dig with { Pitch = 0.5f }, NPC.Center);

                    NPC.noGravity = false;
                    NPC.noTileCollide = false;
                    NPC.velocity = -SaveNPCVelocity;

                    NPC.aiStyle = 26;

                    hasCollidedWithWall = false;

                    NPC.localAI[1] = 0;
                    NPC.localAI[2] = 0;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MiteMandibles>(), 3, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FungusSeed>(), 120));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/DeerMiteGore" + numGores).Type);
                    }
                }
            }
        }
    }
}