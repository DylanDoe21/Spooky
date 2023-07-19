using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

using Spooky.Content.NPCs.Hallucinations;

namespace Spooky.Content.Items.BossSummon
{
    public class Horseshoe : ModItem
    {
        public override void SetStaticDefaults()
		{
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
		}

        public override void SetDefaults()
        {
            Item.damage = 12;                 
            Item.DamageType = DamageClass.Melee;
			Item.noMelee = true;
			Item.autoReuse = true;                  
            Item.width = 20;
            Item.height = 28;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
            Item.rare = ItemRarityID.Blue;   
            Item.value = Item.buyPrice(silver: 50);
            Item.UseSound = SoundID.Item1;
        }

        public override bool CanUseItem(Player player)
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<TheHorse>()) && !Main.dayTime)
            {
                return true;
            }

            return false;
        }
		
        public override bool? UseItem(Player player)
        {
            int type = ModContent.NPCType<TheHorse>();

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.SpawnOnPlayer(player.whoAmI, type);
            }
            else 
            {
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
            }

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup(RecipeGroupID.IronBar, 8)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}