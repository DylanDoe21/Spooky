using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.Minibiomes.Ocean
{
    public class DivingSuit  : ModItem
    {
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                EquipLoader.AddEquipTexture(Mod, "Spooky/Content/Items/Minibiomes/Ocean/DivingSuitHead", EquipType.Head, this);
                EquipLoader.AddEquipTexture(Mod, "Spooky/Content/Items/Minibiomes/Ocean/DivingSuitBody", EquipType.Body, this);
                EquipLoader.AddEquipTexture(Mod, "Spooky/Content/Items/Minibiomes/Ocean/DivingSuitLegs", EquipType.Legs, this);
            }
        }

        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
            ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;

            int equipSlotBody = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;

            int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
            ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 30;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<SpookyPlayer>().DivingSuit = true;
            player.ignoreWater = true;
            player.accFlipper = true;
            player.accDivingHelm = true;

            if (player.wet)
            {
                player.velocity.X *= 0.975f;
                player.velocity.Y *= 0.925f;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup(RecipeGroupID.Wood, 5)
            .AddRecipeGroup(RecipeGroupID.IronBar, 5)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}
