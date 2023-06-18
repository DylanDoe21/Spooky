using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome
{
    public class OldWoodGreatsword : SwingWeaponBase
    {
        public override int Length => 70;
		public override int TopSize => 12;
		public override float SwingDownSpeed => 12f;
		public override bool CollideWithTiles => true;
        static bool hasHitSomething = false;

        public override void SetDefaults()
        {
            Item.damage = 25;
			Item.DamageType = DamageClass.Melee;
			Item.autoReuse = true;
            Item.width = 74;
            Item.height = 74;
            Item.useTime = 45;
			Item.useAnimation = 45;
			Item.useStyle = SwingUseStyle;
			Item.knockBack = 5;
            Item.rare = ItemRarityID.White;
            Item.value = Item.buyPrice(gold: 1);
            Item.UseSound = SoundID.DD2_MonkStaffSwing;
        }

        public override void UseAnimation(Player player)
        {
            hasHitSomething = false;
        }

        public override void OnHitTiles(Player player)
        {
            if (!hasHitSomething)
            {
                hasHitSomething = true;

                SpookyPlayer.ScreenShakeAmount = 1;

                SoundEngine.PlaySound(SoundID.Dig, player.Center);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<SpookyWoodItem>(), 35)
            .AddRecipeGroup(RecipeGroupID.IronBar, 8)
            .AddIngredient(ItemID.Amethyst, 2)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
    }
}