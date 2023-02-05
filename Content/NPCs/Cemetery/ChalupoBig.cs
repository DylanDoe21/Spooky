using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.NPCs.Cemetery
{
    public class ChalupoBig : ModNPC
    {
        public int MoveSpeedX = 0;
		public int MoveSpeedY = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chalupo");
            Main.npcFrameCount[NPC.type] = 5;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 65;
            NPC.damage = 25;
            NPC.defense = 5;
            NPC.width = 32;
            NPC.height = 52;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 2, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CemeteryBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("These ghosts seem small and innocent at first, but when approached they will grow and begin to chase down their target."),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CemeteryBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1;

            if (NPC.frameCounter > 5)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 5)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override void AI()
		{
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            NPC.value = Item.buyPrice(0, 0, 5, 0);
            
            NPC.spriteDirection = NPC.direction;
            NPC.rotation = NPC.velocity.X * 0.05f;

            //flies to players X position
            if (NPC.Center.X >= player.Center.X && MoveSpeedX >= -25) 
            {
                MoveSpeedX--;
            }
            else if (NPC.Center.X <= player.Center.X && MoveSpeedX <= 25)
            {
                MoveSpeedX++;
            }

            NPC.velocity.X = MoveSpeedX * 0.1f;
            
            //flies to players Y position
            if (NPC.Center.Y >= player.Center.Y - 60f && MoveSpeedY >= -25)
            {
                MoveSpeedY--;
            }
            else if (NPC.Center.Y <= player.Center.Y - 60f && MoveSpeedY <= 25)
            {
                MoveSpeedY++;
            }

            NPC.velocity.Y = MoveSpeedY * 0.1f;
        }

        public override void HitEffect(int hitDirection, double damage) 
        {
			if (NPC.life <= 0) 
            {
                for (int numDust = 0; numDust < 20; numDust++)
                {                                                                                  
                    int DustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.RedTorch, 0f, -2f, 0, default(Color), 1.5f);
                    Main.dust[DustGore].noGravity = true;
                    Main.dust[DustGore].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    Main.dust[DustGore].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                }
            }
        }
    }
}