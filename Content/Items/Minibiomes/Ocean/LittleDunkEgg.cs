using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.NPCs.Tameable;

namespace Spooky.Content.Items.Minibiomes.Ocean
{
    public class LittleDunkEgg : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
			Item.height = 58;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.maxStack = 9999;
            Item.consumable = true;
			Item.autoReuse = false;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
			Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 20);
        }

        public override bool CanUseItem(Player player)
        {
            return player.wet;
        }

        public override bool? UseItem(Player player)
        {
            int Egg = NPC.NewNPC(player.GetSource_ItemUse(Item), (int)player.Center.X, (int)player.Center.Y, ModContent.NPCType<LittleDunkEggNPC>());

            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                NetMessage.SendData(MessageID.SyncNPC, number: Egg);
            }
            
            return true;
        }
    }
}