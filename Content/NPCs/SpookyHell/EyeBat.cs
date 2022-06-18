using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.NPCs.SpookyHell.Projectiles;
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
            NPC.lifeMax = 50;
            NPC.damage = 35;
            NPC.defense = 0;
            NPC.width = 46;
            NPC.height = 54;
            NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 14;
            AIType = NPCID.CaveBat;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

			if (!(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust) && 
            ((!Main.pumpkinMoon && !Main.snowMoon) && !Main.eclipse && (SpawnCondition.GoblinArmy.Chance == 0)) && 
            Main.LocalPlayer.InModBiome(ModContent.GetInstance<Content.Biomes.SpookyHellBiome>()))
			{
                return 35f;
            }
            return 0f;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1;
            if (NPC.frameCounter > 5)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = 0;
            }
        }

        public override void AI()
        {
            NPC.spriteDirection = NPC.direction;
            NPC.rotation = NPC.velocity.X * 0.08f;

            NPC.TargetClosest(true);
            NPC.netUpdate = true;
            Player player = Main.player[NPC.target];

            //Go to the upper corner of the player
            float distance = player.Distance(NPC.Center);
            if (distance < 450)
            {
                NPC.ai[0]++;

                if (NPC.ai[0] >= 360 && distance < 450)
                {
                    NPC.velocity *= 0.98f;
                    
                    if (NPC.ai[0] == 400 || NPC.ai[0] == 430 || NPC.ai[0] == 460)
                    {
                        int Damage = Main.expertMode ? 15 : 22;

                        Vector2 ShootDirection = Main.player[NPC.target].Center - NPC.Center;
                        ShootDirection.Normalize();
                        ShootDirection.X *= 3.5f;
                        ShootDirection.Y *= 3.5f;

                        float RandomDirection = (float)Main.rand.Next(-100, 100) * 0.01f;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, ShootDirection.X + RandomDirection, 
                        ShootDirection.Y + RandomDirection, ModContent.ProjectileType<MocoBooger>(), Damage, 1, Main.myPlayer, 0, 0);
                    }
                }
            }

            if (NPC.ai[0] >= 490)
            {
                NPC.ai[0] = 0;
            }

            NPC.netUpdate = true;
            NPC.netAlways = true;
		}

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            if (Main.rand.Next(2) == 0)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MonsterChunk>(), Main.rand.Next(1, 2)));
            }
        }

        public override bool CheckDead() 
		{
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Spooky/EyeBatGore1").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Spooky/EyeBatGore2").Type);

            return true;
		}
    }
}