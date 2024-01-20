using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Content.NPCs.SpiderCave.Projectiles;

namespace Spooky.Content.NPCs.SpiderCave
{
    public class LeafSpider : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/LeafSpiderBestiary",
                Position = new Vector2(0f, -25f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = -20f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 90;
            NPC.damage = 25;
			NPC.defense = 15;
			NPC.width = 58;
			NPC.height = 102;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.75f;
            NPC.value = Item.buyPrice(0, 0, 2, 0);
            NPC.noTileCollide = true;
            NPC.noGravity = true;
			NPC.HitSound = SoundID.NPCHit32;
			NPC.DeathSound = SoundID.NPCDeath35;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.LeafSpider"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
        }

        public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter++;
            if (NPC.frameCounter > 4)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 3)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
		}

        public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

			NPC.spriteDirection = NPC.direction;

            if (NPC.ai[1] == 0)
            {
                SoundEngine.PlaySound(SoundID.Zombie76, NPC.Center);

                NPC.ai[2] = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LeafSpiderWeb>(), ai2: NPC.whoAmI);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {   
                    NetMessage.SendData(MessageID.SyncNPC, number: (int)NPC.ai[2]);
                }

                NPC.ai[1]++;
            }

            if (NPC.ai[1] > 0)
            {
                if (NPC.Distance(player.Center) <= 450f)
                {
                    NPC.ai[0]++;
                }

                if (NPC.ai[0] >= 400)
                {
                    SoundEngine.PlaySound(SoundID.Item17, NPC.Center);

                    for (int numProjectiles = -2; numProjectiles <= 2; numProjectiles++)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X, NPC.Center.Y + 50),
                            Main.rand.NextFloat(5f, 11f) * NPC.DirectionTo(new Vector2(NPC.Center.X, NPC.Center.Y + 150)).RotatedBy(MathHelper.ToRadians(Main.rand.Next(6, 13)) * numProjectiles),
                            ModContent.ProjectileType<LeafSpiderAcid>(), NPC.damage / 4, 0f, Main.myPlayer);
                        }
                    }

                    NPC.ai[0] = 0;
                    NPC.netUpdate = true;
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 6; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/LeafSpiderGore" + numGores).Type);
                    }
                }
            }
        }
    }
}