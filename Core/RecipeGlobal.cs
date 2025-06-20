using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Core
{
    public class RecipeGlobal : ModSystem
    {
        public static int AnyDemoniteBarGroup { get; private set; }

        public override void AddRecipeGroups()
        {
            //old wood counts as a vanilla wood type
            RecipeGroup wood = RecipeGroup.recipeGroups[RecipeGroupID.Wood];
            wood.ValidItems.Add(ModContent.ItemType<SpookyWoodItem>());

			RecipeGroup BaseGroup(object GroupName, object GroupName2, int[] Items)
			{
				string Name = "";
				Name += GroupName switch
				{
					//modcontent items
					int i => Lang.GetItemNameValue((int)GroupName),
					//vanilla item ids
					short s => Lang.GetItemNameValue((short)GroupName),
					//custom group names
					_ => GroupName.ToString(),
				};

                string Name2 = "";
				Name2 += GroupName switch
				{
					//modcontent items
					int i => Lang.GetItemNameValue((int)GroupName2),
					//vanilla item ids
					short s => Lang.GetItemNameValue((short)GroupName2),
					//custom group names
					_ => GroupName2.ToString(),
				};
                
				return new RecipeGroup(() => Name + "/" + Name2, Items);
			}

            RecipeGroup.RegisterGroup("SpookyMod:GoldBars", BaseGroup(ItemID.GoldBar, ItemID.PlatinumBar, new int[] { ItemID.GoldBar, ItemID.PlatinumBar }));
            RecipeGroup.RegisterGroup("SpookyMod:DemoniteBars", BaseGroup(ItemID.DemoniteBar, ItemID.CrimtaneBar, new int[] { ItemID.DemoniteBar, ItemID.CrimtaneBar }));
            RecipeGroup.RegisterGroup("SpookyMod:ShadowScales", BaseGroup(ItemID.ShadowScale, ItemID.TissueSample, new int[] { ItemID.ShadowScale, ItemID.TissueSample }));
            RecipeGroup.RegisterGroup("SpookyMod:AdamantiteBars", BaseGroup(ItemID.AdamantiteBar, ItemID.TitaniumBar, new int[] { ItemID.AdamantiteBar, ItemID.TitaniumBar }));
		}

        public override void AddRecipes()
        {
            //furnace crafting recipe with mossy stone from the spooky forest
            Recipe furnaceRecipe = Recipe.Create(ItemID.Furnace);
            furnaceRecipe.AddIngredient(ModContent.ItemType<SpookyStoneItem>(), 20);
            furnaceRecipe.AddRecipeGroup(RecipeGroupID.Wood, 4);
            furnaceRecipe.AddIngredient(ItemID.Torch, 3);
            furnaceRecipe.AddTile(TileID.WorkBenches);
            furnaceRecipe.Register();
        }
    }
}