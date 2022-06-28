using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Content.Items.SpookyBiome;
using Spooky.Content.Items.SpookyBiome.Armor;
using Spooky.Content.NPCs.SpookyBiome.Projectiles;

namespace Spooky.Content.NPCs.SpookyBiome
{
    public class SpookySkeletonWarlock : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spooky Warlock");
            Main.npcFrameCount[NPC.type] = 8;
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 120;
            NPC.damage = 22;
            NPC.defense = 5;
            NPC.width = 25;
			NPC.height = 60;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, 1, 75);
            NPC.HitSound = SoundID.NPCHit2;
			NPC.DeathSound = SoundID.NPCDeath2;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiomeUg>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), //Plain black background
				new FlavorTextBestiaryInfoElement("The skeletal warlocks of the underground spooky forest possess great magic, allowing them to conjure magic attacks at foes.")
			});
		}

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

			if (!spawnInfo.Invasion && Main.invasionType == 0 && !Main.pumpkinMoon && !Main.snowMoon && !Main.eclipse &&
            !(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust))
            {
                //spawn underground
                if (player.InModBiome(ModContent.GetInstance<Biomes.SpookyBiomeUg>()) && !NPC.AnyNPCs(ModContent.NPCType<SpookySkeletonWarlock>()))
                {
                    return 10f;
                }
            }

            return 0f;
        }

        public override void FindFrame(int frameHeight)
        {
            //use regular walking anim when in walking state
            if (NPC.localAI[0] <= 420)
            {
                //running animation
                NPC.frameCounter += 1;

                if (NPC.frameCounter > 10)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 4)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }

                //jumping frame when falling/jumping
                if (NPC.velocity.Y > 0 || NPC.velocity.Y < 0)
                {
                    NPC.frame.Y = 7 * frameHeight;
                }
            }
            //use casting animation during casting ai
            if (NPC.localAI[0] > 420)
            {
                //casting animation
                NPC.frameCounter += 1;

                if (NPC.frameCounter > 10)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 7)
                {
                    NPC.frame.Y = 6 * frameHeight;
                }
            }
        }
        
        public override void AI()
		{
            Player player = Main.player[NPC.target];

            int Damage = Main.expertMode ? 12 : 15;

            NPC.spriteDirection = NPC.direction;

            NPC.localAI[0]++;

            if (NPC.localAI[0] <= 420)
            {
                NPC.aiStyle = 3;
                AIType = NPCID.Crab;
            }

            if (NPC.localAI[0] > 420)
            {
                NPC.aiStyle = 0;

                if (NPC.localAI[0] == 480 || NPC.localAI[0] == 500 || NPC.localAI[0] == 520)
                {
                    SoundEngine.PlaySound(SoundID.Item8, NPC.Center);

                    Vector2 ShootSpeed = player.Center - NPC.Center;
                    ShootSpeed.Normalize();
                    ShootSpeed.X *= 4.5f;
                    ShootSpeed.Y *= 4.5f;
                    
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X - 25, NPC.Center.Y, ShootSpeed.X, 
                        ShootSpeed.Y, ModContent.ProjectileType<WarlockSkull>(), Damage, 1, NPC.target, 0, 0);
                    }
                }
            }

            if (NPC.localAI[0] >= 560)
            {
                NPC.localAI[0] = 0;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<WarlockHood>(), 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SkullWispStaff>(), 10));
        }

        public override bool CheckDead() 
		{
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/SkeletonGore1").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/SkeletonGore2").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/SkeletonGore3").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/SkeletonGore4").Type);

            return true;
		}
    }
}