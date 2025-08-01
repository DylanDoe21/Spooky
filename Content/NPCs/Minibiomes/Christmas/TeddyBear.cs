using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.IO;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Minibiomes.Christmas
{
    public class TeddyBear1 : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 50;
            NPC.damage = 25;
            NPC.defense = 0;
            NPC.width = 24;
			NPC.height = 36;
            NPC.npcSlots = 1f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noGravity = false;
			NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit15;
			NPC.DeathSound = SoundID.NPCDeath15;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.ChristmasDungeonBiome>().Type };
        }

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
			{
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.TeddyBear1"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.ChristmasDungeonBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 8 - (NPC.velocity.X > 0 ? NPC.velocity.X : -NPC.velocity.X))
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 8)
            {
                NPC.frame.Y = 0 * frameHeight;
            }

            //jumping/falling frame
            if (NPC.velocity.Y < 0)
            {
                NPC.frame.Y = 6 * frameHeight;
            }
            if (NPC.velocity.Y > 0)
            {
                NPC.frame.Y = 2 * frameHeight;
            }
        }
        
        public override void AI()
		{
            NPC Parent = Main.npc[(int)NPC.ai[0]];
            Player player = Main.player[NPC.target];
 
            //follow the parent snow teddy bear
            if (Parent.type == ModContent.NPCType<TeddyBearSnow>() && Parent.life == Parent.lifeMax)
            {
                NPC.spriteDirection = NPC.direction = NPC.velocity.X <= 0 ? -1 : 1;

                //prevents the pet from getting stuck on sloped tiled
                Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);

                Vector2 center2 = NPC.Center;
                Vector2 vector48 = Parent.Center - center2;
                float ParentDistance = vector48.Length();

                if (NPC.velocity.Y == 0 && (((HoleBelow() && ParentDistance > 100f) || (ParentDistance > 100f && NPC.position.X == NPC.oldPosition.X)) || Main.rand.NextBool(350)))
                {
                    NPC.velocity.Y = -8f;
                    NPC.netUpdate = true;
                }

                NPC.velocity.Y += 0.35f;

                if (NPC.velocity.Y > 15f)
                {
                    NPC.velocity.Y = 15f;
                }

                if (ParentDistance > 55f)
                {
                    if (Parent.position.X - NPC.position.X > 0f)
                    {
                        NPC.velocity.X += 0.5f;
                        if (NPC.velocity.X > 6f)
                        {
                            NPC.velocity.X = 6f;
                        }
                    }
                    else
                    {
                        NPC.velocity.X -= 0.5f;
                        if (NPC.velocity.X < -6f)
                        {
                            NPC.velocity.X = -6f;
                        }
                    }
                }
                else
                {
                    if (NPC.velocity.X >= 0)
                    {
                        NPC.velocity.X += 0.5f;
                        if (NPC.velocity.X > 6f)
                        {
                            NPC.velocity.X = 6f;
                        }
                    }
                    else
                    {
                        NPC.velocity.X -= 0.5f;
                        if (NPC.velocity.X < -6f)
                        {
                            NPC.velocity.X = -6f;
                        }
                    }
                }
            }
            //if the snow bear is dead, then it should attack on its own
            else
            {
                NPC.spriteDirection = NPC.direction;

                NPC.aiStyle = 3;
                AIType = NPCID.DesertGhoul;
            }
        }

        public bool HoleBelow()
        {
            int tileWidth = 4;
            int tileX = (int)(NPC.Center.X / 16f) - tileWidth;
            if (NPC.velocity.X > 0)
            {
                tileX += tileWidth;
            }
            int tileY = (int)((NPC.position.Y + NPC.height) / 16f);
            for (int y = tileY; y < tileY + 2; y++)
            {
                for (int x = tileX; x < tileX + tileWidth; x++)
                {
                    if (Main.tile[x, y].HasTile && (Main.tile[x - 1, y].HasTile || Main.tile[x + 1, y].HasTile))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 6; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        //Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/CornGore" + numGores).Type);
                    }
                }
            }
        }
    }

    public class TeddyBear2 : TeddyBear1  
    {
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
			{
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.TeddyBear2"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.ChristmasDungeonBiome>().ModBiomeBestiaryInfoElement)
			});
		}
    }

    public class TeddyBear3 : TeddyBear1  
    {
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
			{
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.TeddyBear3"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.ChristmasDungeonBiome>().ModBiomeBestiaryInfoElement)
			});
		}
    }
}