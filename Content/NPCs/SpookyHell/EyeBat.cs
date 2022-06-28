using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Content.Items.SpookyHell;

namespace Spooky.Content.NPCs.SpookyHell
{
    public class EyeBat : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bat n' Eye");
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 60;
            NPC.damage = 35;
            NPC.defense = 0;
            NPC.width = 88;
            NPC.height = 84;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 14;
            AIType = NPCID.Raven;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), //Plain black background
				new FlavorTextBestiaryInfoElement("These flying abominations are adept and fast hunters. They have eyes all around their body, allowing them to see in all directions.")
			});
		}

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

			if (player.InModBiome(ModContent.GetInstance<Biomes.SpookyHellBiome>()))
			{
                return 30f;
            }
            return 0f;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1;

            //flying
            if (NPC.frameCounter > 4)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = 0 * frameHeight;
            }

            //charging frame
            if (NPC.localAI[0] >= 420)
            {
                NPC.frame.Y = 3 * frameHeight;
            }
        }

        public override void AI()
		{
            Player player = Main.player[NPC.target];

			NPC.spriteDirection = NPC.direction;
            NPC.rotation = NPC.velocity.X * 0.04f;

            NPC.localAI[0]++;
            if (NPC.localAI[0] >= 360 && NPC.localAI[0] < 420)
            {
                NPC.velocity *= 0.95f;
            }

            if (NPC.localAI[0] == 420)
            {
                SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack, NPC.Center);

                Vector2 ChargeDirection = player.Center - NPC.Center;
                ChargeDirection.Normalize();
                        
                ChargeDirection.X *= 12;
                ChargeDirection.Y *= 12;  
                NPC.velocity.X = ChargeDirection.X;
                NPC.velocity.Y = ChargeDirection.Y;
            }

            if (NPC.localAI[0] >= 480)
            {
                NPC.localAI[0] = 0;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CreepyChunk>(), 3, 1, 2));
        }

        public override bool CheckDead() 
		{
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/EyeBatGore1").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/EyeBatGore2").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/EyeBatGore3").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/EyeBatGore4").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/EyeBatGore5").Type);

            return true;
		}
    }
}