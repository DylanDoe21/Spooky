using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;

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
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 120;
            NPC.damage = 45;
            NPC.defense = 20;
            NPC.width = 76;
            NPC.height = 36;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noTileCollide = false;
            NPC.noGravity = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath5;
            NPC.aiStyle = -1;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

			if (!(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust) && 
            ((!Main.pumpkinMoon && !Main.snowMoon) && !Main.eclipse && (SpawnCondition.GoblinArmy.Chance == 0)) &&
            Main.LocalPlayer.InModBiome(ModContent.GetInstance<Content.Biomes.SpookyHellBiome>()))
			{
                return 30f;
            }
            return 0f;
        }

        public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter++;
			
            if (NPC.frameCounter >= 9)
			{
				NPC.frame.Y = (NPC.frame.Y + frameHeight) % (Main.npcFrameCount[NPC.type] * frameHeight);
				NPC.frameCounter = 1;
            }
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.ai[0]++;
            if (NPC.ai[0] > 400 && NPC.ai[0] <= 500)
            {
                if (NPC.Distance(player.Center) <= 650f && Main.rand.Next(15) == 0)
                {
                    float Spread = (float)Main.rand.Next(-250, 250) * 0.01f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, 0 + Spread, -10, ModContent.ProjectileType<ToxicSpit>(), NPC.damage / 2, 1, Main.myPlayer, 0, 0);

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
            if (Main.rand.Next(2) == 0)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MonsterChunk>(), Main.rand.Next(1, 2)));
            }
        }

        public override bool CheckDead() 
		{
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Spooky/ManHoleGore1").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Spooky/ManHoleGore2").Type);

            return true;
		}
    }
}