using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

using Spooky.Content.NPCs.Boss.Daffodil;
using Spooky.Content.Projectiles.Catacomb;

namespace Spooky.Content.Items.BossSummon
{
    public class Brick : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
        }
		
        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 26;
			Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.consumable = true;
            Item.useTime = 35;
			Item.useAnimation = 35;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 12;
            Item.maxStack = 9999;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<BrickProj>();
			Item.shootSpeed = 12f;
        }

        /*
        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<DaffodilEye>());
        }
        */

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.ClayBlock, 15)
            .AddIngredient(ItemID.SoulofNight, 2)
			.AddIngredient(ItemID.SoulofLight, 2)
            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}