using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;

using Spooky.Content.Items.Food;

namespace Spooky.Content.NPCs.NoseCult
{
    public class NoseCultistGrunt : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 11;
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 150;
            NPC.damage = 35;
            NPC.defense = 5;
            NPC.width = 34;
			NPC.height = 42;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.5f;
            NPC.HitSound = SoundID.NPCHit48 with { Pitch = -0.1f };
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 3;
			AIType = NPCID.GoblinScout;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.NoseTempleBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.NoseCultistGrunt"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.NoseTempleBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Main.bloodMoon)
            {
                Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpookyBiome/ZomboidRainBlood").Value;

                var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                    
                Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
            }
		}

        public override void FindFrame(int frameHeight)
        {   
            //running animation
            if (NPC.ai[1] <= 0)
            {
                NPC.frameCounter++;
                if (NPC.velocity.Y == 0)
                {
                    if (NPC.frameCounter > 5)
                    {
                        NPC.frame.Y = NPC.frame.Y + frameHeight;
                        NPC.frameCounter = 0;
                    }
                    if (NPC.frame.Y >= frameHeight * 5)
                    {
                        NPC.frame.Y = 0 * frameHeight;
                    }
                }
                //falling frame
                else
                {
                    NPC.frame.Y = 6 * frameHeight;
                }
            }
            //sneezing animation
            else
            {

            }
        }
        
        public override void AI()
		{
			NPC.spriteDirection = NPC.direction;

            //NPC.ai[0]++;

            if (NPC.ai[0] >= 360 && NPC.velocity.Y == 0)
            {
                NPC.ai[1] = 1;
            }

            if (NPC.ai[1] > 0)
            {

            }
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/NoseCultistGruntGore" + numGores).Type);
                    }
                }
            }
        }
    }
}