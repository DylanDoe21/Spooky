using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;
using Spooky.Content.NPCs.SpookyHell.Projectiles;
using Spooky.Content.Items.SpookyHell;


namespace Spooky.Content.NPCs.SpookyHell
{
    public class EyeBall : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eye Ball");
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 65;
            NPC.damage = 40;
            NPC.defense = 5;
            NPC.width = 44;
			NPC.height = 44;
			NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit9;
			NPC.DeathSound = SoundID.NPCDeath22;
            NPC.aiStyle = -1;
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

        public override void AI()
		{
			if (NPC.spriteDirection == 1)
				NPC.rotation += 0.05f;
			if (NPC.spriteDirection == -1)
				NPC.rotation += -0.05f;
            
            NPC.velocity *= 0.98f;

            NPC.ai[0]++;

            if (NPC.ai[0] == 300)
            {
                NPC.velocity.X = Main.rand.Next(-10, 10);
                NPC.velocity.Y = Main.rand.Next(-10, 10);

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

        public override void HitEffect(int hitDirection, double damage)
		{
			if (NPC.life <= 0)
			{
                int Damage = Main.expertMode ? 18 : 25;

                Vector2 vel = new Vector2(2f, 0f).RotatedByRandom(2 * Math.PI);
                for (int i = 0; i < 3; i++)
                {
                    
                    Vector2 speed = vel.RotatedBy(2 * Math.PI / 7 * (i + Main.rand.NextDouble() - 0.5));
                    Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, speed, ModContent.ProjectileType<ToxicSpit>(), Damage, 0f, Main.myPlayer, 0, 0);
                }

                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Spooky/EyeBallGore1").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Spooky/EyeBallGore2").Type);
            }
        }
    }
}