using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Content.Items.SpiderCave.Misc;

namespace Spooky.Content.NPCs.SpiderCave
{
    public class WolfSpider : ModNPC  
    {
        bool SpawnedSpider1 = false;
        bool SpawnedSpider2 = false;
        bool SpawnedSpider3 = false;
        bool SpawnedSpider4 = false;
        bool SpawnedSpider5 = false;

        private static Asset<Texture2D> BabyTexture1;
        private static Asset<Texture2D> BabyTexture2;
        private static Asset<Texture2D> BabyTexture3;
        private static Asset<Texture2D> BabyTexture4;
        private static Asset<Texture2D> BabyTexture5;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Position = new Vector2(23f, 0f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 0f
            };
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 650;
            NPC.damage = 45;
            NPC.defense = 20;
            NPC.width = 76;
			NPC.height = 76;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.HitSound = SoundID.NPCHit29 with { Pitch = -0.5f };
			NPC.DeathSound = SoundID.NPCDeath41 with { Pitch = -0.5f };
            NPC.aiStyle = 3;
			AIType = NPCID.GoblinScout;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.WolfSpider"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			BabyTexture1 ??= ModContent.Request<Texture2D>(Texture + "Baby1");
            BabyTexture2 ??= ModContent.Request<Texture2D>(Texture + "Baby2");
            BabyTexture3 ??= ModContent.Request<Texture2D>(Texture + "Baby3");
            BabyTexture4 ??= ModContent.Request<Texture2D>(Texture + "Baby4");
            BabyTexture5 ??= ModContent.Request<Texture2D>(Texture + "Baby5");

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (!SpawnedSpider1)
            {
                Main.EntitySpriteDraw(BabyTexture1.Value, NPC.Center - new Vector2(0, 6) - screenPos, NPC.frame, NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            }
            if (!SpawnedSpider2)
            {
                Main.EntitySpriteDraw(BabyTexture2.Value, NPC.Center - new Vector2(0, 6) - screenPos, NPC.frame, NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            }
            if (!SpawnedSpider3)
            {
                Main.EntitySpriteDraw(BabyTexture3.Value, NPC.Center - new Vector2(0, 6) - screenPos, NPC.frame, NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            }
            if (!SpawnedSpider4)
            {
                Main.EntitySpriteDraw(BabyTexture4.Value, NPC.Center - new Vector2(0, 6) - screenPos, NPC.frame, NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            }
            if (!SpawnedSpider5)
            {
                Main.EntitySpriteDraw(BabyTexture5.Value, NPC.Center - new Vector2(0, 6) - screenPos, NPC.frame, NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            }
        }

        public override void FindFrame(int frameHeight)
        {   
            //running animation
            NPC.frameCounter++;
            if (NPC.frameCounter > 7 - (NPC.velocity.X > 0 ? NPC.velocity.X : -NPC.velocity.X))
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 6)
            {
                NPC.frame.Y = 0 * frameHeight;
            }

            //jumping frame
            if (NPC.velocity.Y < 0)
            {
                NPC.frame.Y = 2 * frameHeight;
            }

            //falling frame
            if (NPC.velocity.Y > 0)
            {
                NPC.frame.Y = 5 * frameHeight;
            }
        }

        public override void AI()
		{
			NPC.spriteDirection = NPC.direction;

            if (NPC.life < (NPC.lifeMax * 0.9f) && !SpawnedSpider1)
            {
                int NewNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<WolfSpiderBaby>());
                if (Main.netMode == NetmodeID.Server)
                {   
                    NetMessage.SendData(MessageID.SyncNPC, number: NewNPC);
                }

                SpawnedSpider1 = true;
                NPC.netUpdate = true;
            }
            if (NPC.life < (NPC.lifeMax * 0.8f) && !SpawnedSpider2)
            {
                int NewNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<WolfSpiderBaby>());
                if (Main.netMode == NetmodeID.Server)
                {   
                    NetMessage.SendData(MessageID.SyncNPC, number: NewNPC);
                }

                SpawnedSpider2 = true;
                NPC.netUpdate = true;
            }
            if (NPC.life < (NPC.lifeMax * 0.7f) && !SpawnedSpider3)
            {
                int NewNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<WolfSpiderBaby>());
                if (Main.netMode == NetmodeID.Server)
                {   
                    NetMessage.SendData(MessageID.SyncNPC, number: NewNPC);
                }

                SpawnedSpider3 = true;
                NPC.netUpdate = true;
            }
            if (NPC.life < (NPC.lifeMax * 0.6f) && !SpawnedSpider4)
            {
                int NewNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<WolfSpiderBaby>());
                if (Main.netMode == NetmodeID.Server)
                {   
                    NetMessage.SendData(MessageID.SyncNPC, number: NewNPC);
                }

                SpawnedSpider4 = true;
                NPC.netUpdate = true;
            }
            if (NPC.life < (NPC.lifeMax * 0.5f) && !SpawnedSpider5)
            {
                int NewNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<WolfSpiderBaby>());
                if (Main.netMode == NetmodeID.Server)
                {   
                    NetMessage.SendData(MessageID.SyncNPC, number: NewNPC);
                }

                SpawnedSpider5 = true;
                NPC.netUpdate = true;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SpiderChitin>(), 3, 1, 3));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 8; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/WolfSpiderGore" + numGores).Type);
                    }
                }
            }
        }
    }
}