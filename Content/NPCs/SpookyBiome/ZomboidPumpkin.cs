using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.Food;
using Spooky.Content.NPCs.SpookyBiome.Projectiles;

namespace Spooky.Content.NPCs.SpookyBiome
{
    public class ZomboidPumpkin : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 9;
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 50;
            NPC.damage = 15;
            NPC.defense = 10;
            NPC.width = 35;
			NPC.height = 50;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 0, 50);
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath2;
            NPC.aiStyle = 0;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.ZomboidPumpkin"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            //idle animation
            NPC.frameCounter++;
            if (NPC.ai[1] < 1)
            {
                if (NPC.frameCounter > 8)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 4)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }
            //attacking animation
            if (NPC.ai[1] == 1)
            {
                if (NPC.frame.Y < frameHeight * 5)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }

                if (NPC.frameCounter > 4)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y == frameHeight * 8)
                {
                    SpookyPlayer.ScreenShakeAmount = 4;
                    SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, NPC.Center);
                }
                if (NPC.frame.Y >= frameHeight * 9)
                {
                    NPC.frame.Y = 0 * frameHeight;
                    NPC.ai[1] = 0;
                }
            }
        }
        
        public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

			NPC.spriteDirection = NPC.direction;

            if (player.Distance(NPC.Center) <= 500f)
            {
                NPC.ai[0]++;

                if (NPC.ai[0] == 300)
                {
                    NPC.ai[1] = 1;
                }
            }

            //spawn thorns out of the ground and reset ai
            if (NPC.ai[0] > 300 && NPC.ai[1] == 0)
            {
                for (int j = 1; j <= 3; j++)
                {
                    for (int i = -1; i <= 1; i += 2)
                    {
                        Vector2 center = new Vector2(NPC.Center.X, NPC.Center.Y + NPC.height / 4);
                        center.X += j * 45 * i; //35 is the distance between each one
                        int numtries = 0;
                        int x = (int)(center.X / 16);
                        int y = (int)(center.Y / 16);
                        while (y < Main.maxTilesY - 10 && Main.tile[x, y] != null && !WorldGen.SolidTile2(x, y) && 
                        Main.tile[x - 1, y] != null && !WorldGen.SolidTile2(x - 1, y) && Main.tile[x + 1, y] != null && !WorldGen.SolidTile2(x + 1, y)) 
                        {
                            y++;
                            center.Y = y * 16;
                        }
                        while ((WorldGen.SolidOrSlopedTile(x, y) || WorldGen.SolidTile2(x, y)) && numtries < 10) 
                        {
                            numtries++;
                            y--;
                            center.Y = y * 16;
                        }

                        if (numtries >= 10)
                        {
                            break;
                        }

                        Projectile.NewProjectile(NPC.GetSource_FromThis(), center.X - 3, center.Y + 20, 0, 0, 
                        ModContent.ProjectileType<ZomboidRootThornTelegraph>(), NPC.damage / 4, 0, Main.myPlayer);
                    }
                }

                NPC.ai[0] = 0;
                NPC.ai[1] = 0;
            }
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
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ZomboidPumpkinGore" + numGores).Type);
                    }
                }
            }
        }
    }

    public class ZomboidPumpkinFire : ZomboidPumpkin  
    {
        private static Asset<Texture2D> GlowTexture;

        public override void SetDefaults()
		{
            NPC.lifeMax = 300;
            NPC.damage = 40;
            NPC.defense = 10;
            NPC.width = 35;
			NPC.height = 52;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath2;
            NPC.aiStyle = 0;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.ZomboidPumpkinFire"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpookyBiome/ZomboidPumpkinFireGlow");

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < 3; i++)
            {
                int XOffset = Main.rand.Next(-1, 2);
                int YOffset = Main.rand.Next(-1, 2);
                
                Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition + new Vector2(XOffset, NPC.gfxOffY + 4 + YOffset), 
                NPC.frame, Color.White * 0.5f, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
            }
		}

        public override void AI()
		{
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

			NPC.spriteDirection = NPC.direction;

            if (player.Distance(NPC.Center) <= 500f)
            {
                NPC.ai[0]++;

                if (NPC.ai[0] == 300)
                {
                    NPC.ai[1] = 1;
                }
            }

            //spawn thorns out of the ground and reset ai
            if (NPC.ai[0] > 300 && NPC.ai[1] == 0)
            {
                for (int j = 1; j <= 5; j++)
                {
                    for (int i = -2; i <= 2; i += 2)
                    {
                        if (i != 0)
                        {
                            Vector2 center = new Vector2(NPC.Center.X, NPC.Center.Y + NPC.height / 2);
                            center.X += j * 35 * i; //35 is the distance between each one
                            int numtries = 0;
                            int x = (int)(center.X / 16);
                            int y = (int)(center.Y / 16);
                            while (y < Main.maxTilesY - 10 && Main.tile[x, y] != null && !WorldGen.SolidTile2(x, y) && 
                            Main.tile[x - 1, y] != null && !WorldGen.SolidTile2(x - 1, y) && Main.tile[x + 1, y] != null && !WorldGen.SolidTile2(x + 1, y)) 
                            {
                                y++;
                                center.Y = y * 16;
                            }
                            while ((WorldGen.SolidOrSlopedTile(x, y) || WorldGen.SolidTile2(x, y)) && numtries < 10) 
                            {
                                numtries++;
                                y--;
                                center.Y = y * 16;
                            }

                            if (numtries >= 10)
                            {
                                break;
                            }

                            Projectile.NewProjectile(NPC.GetSource_FromThis(), center.X - 4, center.Y + 20, 0, 0, 
                            ModContent.ProjectileType<ZomboidRootThornTelegraphFire>(), NPC.damage / 4, 0, Main.myPlayer);
                        }
                    }
                }

                NPC.ai[0] = 0;
                NPC.ai[1] = 0;
            }
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ZomboidPumpkinFireGore").Type);
                }

                for (int numGores = 2; numGores <= 6; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ZomboidPumpkinGore" + numGores).Type);
                    }
                }
            }
        }
    }
}