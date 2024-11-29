using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.Tiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class EyeBody : ModItem
	{
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);

            ArmorIDs.Body.Sets.HidesArms[equipSlot] = true;
        }

        public override void SetDefaults() 
		{
			Item.defense = 5;
			Item.width = 36;
			Item.height = 22;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 2);
		}

		public override void UpdateEquip(Player player) 
		{
			player.maxMinions += 1;
			player.maxTurrets += 1;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
			.AddRecipeGroup("SpookyMod:DemoniteBars", 15)
			.AddIngredient(ModContent.ItemType<CreepyChunk>(), 30)
			.AddIngredient(ModContent.ItemType<LivingFleshItem>(), 80)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}
