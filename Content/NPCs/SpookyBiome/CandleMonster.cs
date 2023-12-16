using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.Food;
using Spooky.Content.NPCs.SpookyBiome.Projectiles;

namespace Spooky.Content.NPCs.SpookyBiome
{
    public class CandleMonster : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 200;
            NPC.damage = 40;
            NPC.defense = 5;
            NPC.width = 32;
			NPC.height = 60;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 0, 50);
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath2;
            NPC.aiStyle = 0;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiomeUg>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.CandleMonster"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiomeUg>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpookyBiome/CandleMonsterGlow").Value;

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < 4; i++)
            {
                int XOffset = Main.rand.Next(-2, 3);
                int YOffset = Main.rand.Next(-2, 3);
                
                Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(XOffset, NPC.gfxOffY + 4 + YOffset), 
                NPC.frame, Color.White * 0.5f, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
            }
		}

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;

            //idle frame
            if (NPC.ai[1] < 1)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
            //attacking animation
            if (NPC.ai[1] == 1)
            {
            }
        }
        
        public override void AI()
		{
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

			NPC.spriteDirection = NPC.direction;

            if (player.Distance(NPC.Center) <= 300f)
            {
                NPC.ai[0]++;

                if (NPC.ai[0] == 300)
                {
                    NPC.ai[1] = 1;
                }
            }
        }

        /*
        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 6; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ZomboidPumpkinGore" + numGores).Type);
                    }
                }
            }
        }
        */
    }
}