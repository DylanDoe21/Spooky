using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria.Audio;

using Spooky.Content.NPCs.Boss.Pumpkin;

namespace Spooky.Content.Items.BossSummon
{
    public class RottenSeed : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rotten Seed");
            Tooltip.SetDefault("Summons the giant pumpkin\nCan be used in the spooky forest");
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }
		
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 32;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.useStyle = 4;
            Item.maxStack = 20;
            Item.consumable = true;
        }
		
        public override bool CanUseItem(Player player)
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<SpookyPumpkin>()) && !NPC.AnyNPCs(ModContent.NPCType<SpookyPumpkinP2>()) && 
            player.InModBiome(ModContent.GetInstance<Content.Biomes.SpookyBiome>()))
            {
                return true;
            }

            return false;
        }
		
        public override bool? UseItem(Player player)
        {
            NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<SpookyPumpkin>());
            SoundEngine.PlaySound(SoundID.Roar, player.position);
            return true;
        }
    }
}
