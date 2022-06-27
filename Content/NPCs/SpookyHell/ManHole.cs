using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.NPCs.SpookyHell.Projectiles;
using Spooky.Content.Items.SpookyHell;

namespace Spooky.Content.NPCs.SpookyHell
{
    public class ManHole : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Man Hole");
            Main.npcFrameCount[NPC.type] = 5;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 120;
            NPC.damage = 45;
            NPC.defense = 20;
            NPC.width = 78;
            NPC.height = 36;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noTileCollide = false;
            NPC.noGravity = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath5;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Content.Biomes.SpookyHellBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), //Plain black background
				new FlavorTextBestiaryInfoElement("These horrid mouths form on the ground in the living hell, and can be very dangerous to unsuspecting explorers. They can also spit toxic saliva to digest prey from afar.")
			});
		}

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

			if (Main.LocalPlayer.InModBiome(ModContent.GetInstance<Content.Biomes.SpookyHellBiome>()))
			{
                return 30f;
            }
            return 0f;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1;

            //idle
            if (NPC.frameCounter > 4)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = 0 * frameHeight;
            }

            //open mouth for spit attack
            if (NPC.ai[0] >= 400)
            {
                NPC.frame.Y = 4 * frameHeight;
            }
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            int Damage = Main.expertMode ? 25 : 35;

            NPC.ai[0]++;
            if (NPC.ai[0] > 400 && NPC.ai[0] <= 500)
            {
                if (Main.rand.Next(8) == 0)
                {
                    float Spread = (float)Main.rand.Next(-250, 250) * 0.01f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, 0 + Spread, -10, 
                    ModContent.ProjectileType<SalivaBall>(), Damage, 1, Main.myPlayer, 0, 0);

                    SoundEngine.PlaySound(SoundID.NPCDeath13, NPC.position);
                }
            }
            
            if (NPC.ai[0] >= 500)
            {
                NPC.ai[0] = 0;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CreepyChunk>(), 3, 1, 2));
        }

        public override bool CheckDead() 
		{
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ManHoleGore1").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ManHoleGore2").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ManHoleGore3").Type);

            return true;
		}
    }
}