using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Core
{
    public class NPCGlobal : GlobalNPC
    {
        public static int Orro = -1;
        public static int Boro = -1;

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        { 
            //all bosses drop goodie bags
            if (npc.boss)
            {
                npcLoot.Add(ItemDropRule.Common(ItemID.GoodieBag, 1, 2, 5));
            }
        }

        public override void OnKill(NPC npc)
        {
            if (npc.HasBuff(ModContent.BuffType<PumpkinWhipDebuff>()))
            {
                Vector2 Speed = new Vector2(3f, 0f).RotatedByRandom(2 * Math.PI);

                for (int numProjectiles = 0; numProjectiles < 3; numProjectiles++)
                {
                    Vector2 speed = Speed.RotatedBy(2 * Math.PI / 2 * (numProjectiles + Main.rand.NextDouble() - 0.5));

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, speed,
                        ModContent.ProjectileType<PumpkinWhipFly>(), 15, 0f, Main.myPlayer, 0, 0);
                    }
                }
            }
        }
    }
}