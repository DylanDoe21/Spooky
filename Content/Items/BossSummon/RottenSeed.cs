using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Chat;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.NPCs.Boss.RotGourd;

namespace Spooky.Content.Items.BossSummon
{
    public class RottenSeed : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
            Item.ResearchUnlockCount = 3;
        }
		
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 28;
            Item.consumable = true;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.maxStack = 9999;
        }
		
        public override bool CanUseItem(Player player)
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<RotGourd>()) && player.InModBiome(ModContent.GetInstance<Content.Biomes.SpookyBiome>()))
            {
                return true;
            }

            return false;
        }
		
        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.Center);

            int RotGourd = NPC.NewNPC(player.GetSource_ItemUse(Item), (int)player.Center.X, (int)player.Center.Y - 1000, ModContent.NPCType<RotGourd>(), player.whoAmI, -1);

            //spawn message
            string text = Language.GetTextValue("Mods.Spooky.EventsAndBosses.RotGourdSpawn");

            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text), new Color(171, 64, 255));

                NetMessage.SendData(MessageID.SyncNPC, number: RotGourd);
            }
            else 
            {
                Main.NewText(text, 171, 64, 255);
            }
            
            return true;
        }
    }
}
