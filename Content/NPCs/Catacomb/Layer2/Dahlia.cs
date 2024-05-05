using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Content.Items.Catacomb.Misc;
using Spooky.Content.Items.Food;

namespace Spooky.Content.NPCs.Catacomb.Layer2
{
    public class Dahlia : ModNPC  
    {
        private static Asset<Texture2D> LipsTexture;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/DahliaBestiary"
            };

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 1000;
            NPC.damage = 40;
            NPC.defense = 0;
            NPC.width = 80;
            NPC.height = 72;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.value = Item.buyPrice(0, 0, 50, 0);
            NPC.HitSound = SoundID.Grass;
            NPC.DeathSound = SoundID.NPCDeath42 with { Pitch = 0.45f };
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome2>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
                new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Dahlia"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            LipsTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Catacomb/Layer2/DahliaLips");

            Vector2 drawOrigin = new Vector2(LipsTexture.Width() / 2, LipsTexture.Height() / 2);
            Vector2 drawPos = new Vector2(NPC.Center.X, NPC.Center.Y + 2) - screenPos;

            spriteBatch.Draw(LipsTexture.Value, drawPos, null, drawColor, 0, drawOrigin, NPC.scale, SpriteEffects.None, 0f);
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.rotation += 0.05f * (float)NPC.direction;

            if (NPC.ai[0] == 0)
            {
                for (int numEyes = 0; numEyes < 2; numEyes++)
                {
                    int distance = 360 / 2;
                    int NewNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DahliaEye>(), ai0: NPC.whoAmI, ai1: numEyes * distance, ai2: Main.rand.Next(0, 6));

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {   
                        NetMessage.SendData(MessageID.SyncNPC, number: NewNPC);
                    }
                }

                NPC.ai[0] = 1;
            }

            Vector2 desiredVelocity = NPC.DirectionTo(player.Center) * 8;
            NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PlantChunk>(), 5, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CandyCorn>(), 100));
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
			if (NPC.life <= 0) 
            {
                for (int numDusts = 0; numDusts < 12; numDusts++)
                {                                                                                  
                    int DustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Grass, 0f, -2f, 0, default, 1.5f);
                    Main.dust[DustGore].position.X += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
                    Main.dust[DustGore].position.Y += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
                }

                for (int numGores = 1; numGores <= 9; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center + new Vector2(Main.rand.Next(-30, 30), Main.rand.Next(-30, 30)), NPC.velocity, ModContent.Find<ModGore>("Spooky/DahliaGore" + Main.rand.Next(1, 3)).Type);
                    }
                }
            }
        }
    }
}