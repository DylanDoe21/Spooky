using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.Food;

namespace Spooky.Content.NPCs.SpookyBiome
{
    public class ZomboidRain : ModNPC  
    {
        private static Asset<Texture2D> BloodTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 9;
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 65;
            NPC.damage = 20;
            NPC.defense = 5;
            NPC.width = 36;
			NPC.height = 42;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.75f;
            NPC.value = Item.buyPrice(0, 0, 0, 50);
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath2;
            NPC.aiStyle = 3;
			AIType = NPCID.Crab;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.ZomboidRain"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.Rain,
                new BestiaryBackgroundOverlay("Spooky/Content/Biomes/SpookyBiomeNight_Background", Color.White)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //if a blood moon is happening make the rain blood water
            if (Main.bloodMoon)
            {
                BloodTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpookyBiome/ZomboidRainBlood");

                var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                    
                Main.EntitySpriteDraw(BloodTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
            }
		}

        public override void FindFrame(int frameHeight)
        {   
            //running animation
            NPC.frameCounter++;
            if (NPC.velocity.Y == 0)
            {
                if (NPC.frameCounter > 9)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 6)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }
            //falling animation
            else
            {
                if (NPC.frame.Y < frameHeight * 6)
                {
                    NPC.frame.Y = 6 * frameHeight;
                }

                if (NPC.frameCounter > 6)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 9)
                {
                    NPC.frame.Y = 6 * frameHeight;
                }
            }
        }
        
        public override void AI()
		{
			NPC.spriteDirection = NPC.direction;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FrankenMarshmallow>(), 100));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 6; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ZomboidRainGore" + numGores).Type);
                    }
                }
            }
        }
    }
}