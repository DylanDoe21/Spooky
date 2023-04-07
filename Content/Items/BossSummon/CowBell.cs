using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria.Audio;

using Spooky.Content.NPCs;

namespace Spooky.Content.Items.BossSummon
{
    public class CowBell : ModItem
    {
        public static readonly SoundStyle CowBellSound = new("Spooky/Content/Sounds/CowBell", SoundType.Sound);

        public override void SetStaticDefaults()
		{
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
		}

        public override void SetDefaults()
        {
            Item.damage = 12;
			Item.mana = 10;                        
            Item.DamageType = DamageClass.Summon;
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
            Item.UseSound = CowBellSound;
        }

        public override bool CanUseItem(Player player)
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<TheEntity>()))
            {
                return true;
            }

            return false;
        }
		
        public override bool? UseItem(Player player)
        {
            Main.dayTime = false;
            Main.time = 0;
            NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<TheEntity>());

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