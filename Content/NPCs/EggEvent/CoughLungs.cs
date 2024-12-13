using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.SpookyHell.EggEvent;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.NPCs.EggEvent.Projectiles;

namespace Spooky.Content.NPCs.EggEvent
{
    public class CoughLungs : ModNPC  
    {
        private static Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 7;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 350;
            NPC.damage = 45;
            NPC.defense = 10;
            NPC.width = 72;
			NPC.height = 70;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.5f;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.value = Item.buyPrice(0, 0, 2, 0);
            NPC.HitSound = SoundID.Item177;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 22;
			AIType = NPCID.Wraith;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type, ModContent.GetInstance<Biomes.SpookyHellEventBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.CoughLungs"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellEventBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/CoughLungsGlow");

            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;

            //normal animations
            if (NPC.localAI[0] <= 480)
            {
                if (NPC.frameCounter > 6)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 6)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }
            //coughing animation
            else
            {
                if (NPC.frame.Y < frameHeight * 6)
                {
                    NPC.frame.Y = 5 * frameHeight;
                }

                if (NPC.frameCounter > 2)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 7)
                {
                    NPC.frame.Y = 6 * frameHeight;
                }
            }
        }

        public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.rotation = NPC.velocity.X * 0.05f;

            NPC.localAI[0]++;
			if (NPC.localAI[0] == 420)
            {
                NPC.velocity.Y = -20;
            }

            if (NPC.localAI[0] > 420)
            {
                NPC.aiStyle = -1;
                NPC.velocity *= 0.9f;
            }
            else
            {
                NPC.aiStyle = 22;
			    AIType = NPCID.Wraith;
            }

            if (NPC.localAI[0] == 480)
            {
                SoundEngine.PlaySound(SoundID.NPCHit27 with { Pitch = -1.2f }, NPC.Center);

                //spawn splatter
                for (int i = 0; i < 6; i++)
                {
                    NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X, NPC.Center.Y + 22), new Vector2(Main.rand.Next(-4, 5), 0), ModContent.ProjectileType<YellowSplatter>(), 0, 0f);
                }

                //spawn ichor clouds
                for (int i = 0; i < 3; i++)
                {
                    NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X, NPC.Center.Y + 30), new Vector2(Main.rand.Next(-1, 2), 12), ModContent.ProjectileType<CoughCloud>(), NPC.damage, 4.5f);
                }
            }

            if (NPC.localAI[0] == 520)
            {
                NPC.localAI[0] = 0;

                NPC.netUpdate = true;
            }
		}

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.ByCondition(new DropConditions.PostOrroboroCondition(), ModContent.ItemType<ArteryPiece>(), 5, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SmokerLung>(), 30));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                //spawn splatter
                for (int i = 0; i < 8; i++)
                {
                    NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, new Vector2(Main.rand.Next(-4, 5), Main.rand.Next(-4, -1)), ModContent.ProjectileType<YellowSplatter>(), 0, 0f);
                }

                for (int numGores = 1; numGores <= 5; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/CoughLungGore" + numGores).Type);
                    }
                }
            }
        }
    }
}